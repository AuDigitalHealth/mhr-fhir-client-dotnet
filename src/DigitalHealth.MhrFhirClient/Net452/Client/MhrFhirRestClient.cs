/*
* Copyright 2017 Australian Digital Health Agency (The Agency)
*
* Licensed under the Agency’s Open Source (Apache) License; you may not use this 
* file except in compliance with the License. A copy of the License is in the
* ' Source Code Licence and Production Disclaimer.txt' file, which should be 
*  provided with this work.
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
* License for the specific language governing permissions and limitations
* under the License.
*/

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Rest;
using DigitalHealth.MhrFhirClient.Utility;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using RestRequest = DigitalHealth.MhrFhirClient.Rest.RestRequest;

namespace DigitalHealth.MhrFhirClient.Client
{
    /// <summary>
    /// MHR FHIR base client implementation.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Rest.RestClient" />
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IMhrFhirRestClient" />
    internal class MhrFhirRestClient : RestClient, IMhrFhirRestClient
    {
        private const string AcceptHeader = "Accept";
        private const string AppIdHeader = "App-Id";
        private const string AppVersionHeader = "App-Version";
        private const string BearerAuthType = "Bearer";

        /// <summary>
        /// The bearer token
        /// </summary>
        private readonly string _bearerToken;

        /// <summary>
        /// The client identifier
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        /// The application version
        /// </summary>
        private readonly string _appVersion;


        /// <summary>
        /// MhrFhirRestClient Constructor
        /// </summary>
        /// <param name="baseEndpoint">The baseEndpoint</param>
        /// <param name="bearerToken">The bearerToken</param>
        /// <param name="clientId">The clientId</param>
        /// <param name="appVersion">The appVersion</param>
        /// <param name="httpMessageHandler">The HTTP message handler.</param>
        internal MhrFhirRestClient(Uri baseEndpoint, string bearerToken, string clientId, string appVersion, HttpMessageHandler httpMessageHandler) 
            : base(baseEndpoint, httpMessageHandler)
        {
            _bearerToken = bearerToken;
            _clientId = clientId;
            _appVersion = appVersion;
        }

        /// <summary>
        /// Creates the MHR fhir request.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public RestRequest CreateMhrFhirRequest(string resource, HttpMethod method)
        {
            var endpoint = $"{EndPointUrl.AbsoluteUri}/{resource}";
            
            var request = new RestRequest(method, new Uri(endpoint));

            request.AddHeader(AcceptHeader, FhirConstants.FhirJsonMediaType);

            if (_bearerToken != string.Empty)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(BearerAuthType, _bearerToken);
            }

            if (_clientId != string.Empty)
            {
                request.AddHeader(AppIdHeader, _clientId);
            }

            if (_appVersion != string.Empty)
            {
                request.AddHeader(AppVersionHeader, _appVersion);
            }

            return request;
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <typeparam name="T">The Generic Type</typeparam>
        /// <param name="request">The request</param>
        /// <returns>
        /// The Generic Type
        /// </returns>
        /// <exception cref="MhrFhirException">
        /// </exception>
        public new async Task<T> ExecuteRequest<T>(RestRequest request) where T : Resource
        {           
            try
            {
                HttpResponseMessage response = await base.ExecuteRequest(request);

                string body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return (T) FhirParser.ParseResourceFromJson(body);
                }

                throw CreateMhrFhirException(body, response.StatusCode, response.ReasonPhrase);
            }
            catch (RestException e)
            {
                throw CreateMhrFhirException(e.ResponseContent, e.StatusCode, e.Message);
            }
        }

        /// <summary>
        /// Creates a MHR FHIR exception.
        /// </summary>
        /// <param name="responseContent">Response content.</param>
        /// <param name="httpStatusCode">Response HTTP status code.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>MhrFhirException</returns>
        private static MhrFhirException CreateMhrFhirException(string responseContent, HttpStatusCode httpStatusCode, string errorMessage)
        {
            Resource resource;
            try
            {
                // Attempt to parse the response content as FHIR JSON
                resource = FhirParser.ParseResourceFromJson(responseContent);
            }
            catch (FormatException)
            {
                throw new MhrFhirException(null, httpStatusCode, errorMessage, responseContent);
            }

            // Get the OperationOutcome out of the response
            OperationOutcome outcome = null;
            if (resource.TypeName == typeof(Bundle).Name)
            {
                outcome = (OperationOutcome)((Bundle)resource).Entry.First().Resource;
            }
            else if (resource.TypeName == typeof(OperationOutcome).Name)
            {
                outcome = (OperationOutcome)resource;
            }

            return new MhrFhirException(outcome, httpStatusCode, errorMessage, responseContent);
        }
    }
}
