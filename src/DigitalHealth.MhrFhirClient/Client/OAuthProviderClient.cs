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

#if !PORTABLE
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Rest;
using DigitalHealth.MhrFhirClient.Utility;

namespace DigitalHealth.MhrFhirClient.Client
{
    /// <summary>
    ///  Implementation of OAuth provider client.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IProviderOAuthClient" />
    internal class OAuthProviderClient : IProviderOAuthClient
    {
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IRestClient _restClient;

        /// <summary>
        /// The provider o authentication model
        /// </summary>
        private readonly ProviderOAuthModel _providerOAuthModel;

        /// <summary>
        /// Constructor for the Provider Token Service
        /// </summary>
        /// <param name="restClient">The ProviderRestClient</param>
        /// <param name="providerOAuthModel">The provider o authentication model.</param>
        internal OAuthProviderClient(IRestClient restClient, ProviderOAuthModel providerOAuthModel)
        {
            _restClient = restClient;
            _providerOAuthModel = providerOAuthModel;
        }

        /// <summary>
        /// Calls the Get Provider Token Service
        /// </summary>
        /// <param name="providerHpii"></param>
        /// <param name="providerName"></param>
        /// <returns>
        /// The OAuthResponse
        /// </returns>
        /// <exception cref="OAuthProviderClientException"></exception>
        public async Task<OAuthResponse> GetProviderToken(string providerHpii, string providerName)
        {
            // Create the JWT
            var jsonWebToken = JsonWebTokenUtility.GetJsonWebToken(
                _providerOAuthModel.ClientIdentifier,
                _providerOAuthModel.ClientSecret,
                _providerOAuthModel.RedirectUrl,
                _providerOAuthModel.Hpio,
                providerHpii
            );
            
            var request = new RestRequest(HttpMethod.Post, _restClient.EndPointUrl);

            // Setup the request body parameters
            var bodyParameters = new Dictionary<string, string>
            {
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"assertion", jsonWebToken},
                {"format", "json"},
                {"userName", providerName},
                {"organisationName", _providerOAuthModel.OrganisationName},
                {"DeviceID", _providerOAuthModel.DeviceIdentifier},
                {"DeviceMake", _providerOAuthModel.DeviceMake},
                {"DeviceModel", _providerOAuthModel.DeviceModel}
            };

            request.SetJsonBodyParameters(bodyParameters);

            // Execute the request
            try
            {
                return await _restClient.ExecuteRequest<OAuthResponse>(request);
            }
            catch (RestException ex)
            {
                throw new OAuthProviderClientException(ex.StatusCode, ex.ResponseContent);
            }
        }
    }
}
#endif