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
using System.Net.Http;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Logging;
using Newtonsoft.Json;

namespace DigitalHealth.MhrFhirClient.Rest
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Rest.IRestClient" />
    public class RestClient :  IRestClient
    {
        /// <summary>
        /// logger
        /// </summary>
        private static readonly ILog Logger = LogProvider.For<RestClient>();

        /// <summary>
        /// Gets the end point URL.
        /// </summary>
        /// <value>
        /// end point URL.
        /// </value>
        public Uri EndPointUrl { get; }

        /// <summary>
        /// HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endPointUrl">Endpoint of the MHR FHIR service.</param>
        /// <exception cref="System.ArgumentNullException">endPointUrl</exception>
        public RestClient(Uri endPointUrl)
        {
            if (endPointUrl == null)
            {
                throw new ArgumentNullException(nameof(endPointUrl));
            }

            EndPointUrl = endPointUrl;

            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endPointUrl">The end point URL.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.ArgumentNullException">endPointUrl</exception>
        public RestClient(Uri endPointUrl, HttpMessageHandler handler)  
        {
            if (endPointUrl == null)
            {
                throw new ArgumentNullException(nameof(endPointUrl));
            }

            EndPointUrl = endPointUrl;

            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <typeparam name="T">The Generic Type</typeparam>
        /// <param name="request">The request</param>
        /// <returns>
        /// Generic Type
        /// </returns>
        public async Task<T> ExecuteRequest<T>(RestRequest request)
        {
            var result = await ExecuteRequest(request);

            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// Response
        /// </returns>
        /// <exception cref="DigitalHealth.MhrFhirClient.Rest.RestException"></exception>
        public async Task<HttpResponseMessage> ExecuteRequest(RestRequest request)
        {
            await LogRequest(request);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error sending request", ex);
                throw;
            }

            await LogResponse(response);

            // Check for the success code
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            throw new RestException(response.StatusCode, responseBody);
        }

        /// <summary>
        /// Logs the request message.
        /// </summary>
        /// <param name="httpRequestMessage">The HTTP request message.</param>
        /// <returns></returns>
        private static async Task LogRequest(HttpRequestMessage httpRequestMessage)
        {
            if (Logger.IsDebugEnabled())
            {
                string requestBody = null;
                string headers = null;

                if (httpRequestMessage.Content != null)
                {
                    requestBody = await httpRequestMessage.Content.ReadAsStringAsync();
                }
                if (httpRequestMessage.Headers != null && httpRequestMessage.Headers.Any())
                {
                    headers = httpRequestMessage.Headers.ToString();
                }

                Logger.DebugFormat("Request: {@requestMessage}", new
                {
                   
                    method = httpRequestMessage.Method.ToString(),
                    uri = httpRequestMessage.RequestUri,
                    headers = headers,
                    body = requestBody
                });
            }
        }

        /// <summary>
        /// Logs the response message.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        /// <returns></returns>
        private static async Task LogResponse(HttpResponseMessage httpResponseMessage)
        {
            if (Logger.IsDebugEnabled())
            {
                string responseBody = null;
                string headers = null;
                if (httpResponseMessage.Content != null)
                {
                    responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                }
                if (httpResponseMessage.Headers != null && httpResponseMessage.Headers.Any())
                {
                    headers = httpResponseMessage.Headers.ToString();
                }
                Logger.DebugFormat("Response: {@responseMessage}", new
                {
                    statusCode = httpResponseMessage.StatusCode,
                    uri = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri,
                    headers = headers,
                    body = responseBody
                });
            }
        }
    }
}

