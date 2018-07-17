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
    /// Implementation of OAuth consumer client.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IConsumerOAuthClient" />
    internal class OAuthConsumerClient : IConsumerOAuthClient
    {
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IRestClient _restClient;
        /// <summary>
        /// The consumer authentication model
        /// </summary>
        private readonly ConsumerOAuthModel _consumerOAuthModel;

        /// <summary>
        /// Constructor for the Consumer OAuth Client
        /// </summary>
        /// <param name="restClient">The Rest Client</param>
        /// <param name="consumerOAuthModel">The Consumer OAuth Model</param>
        internal OAuthConsumerClient(IRestClient restClient, ConsumerOAuthModel consumerOAuthModel)
        {
            _restClient = restClient as RestClient;
            _consumerOAuthModel = consumerOAuthModel;
        }

        /// <summary>
        /// Get the OAuth Login Uri for the consumer client
        /// </summary>
        /// <returns>
        /// The Login URI
        /// </returns>
        public Uri GetLoginUri()
        {
            var queryParams = new List<KeyValuePair<string,string>>
            {
                new KeyValuePair<string, string>("client_id", _consumerOAuthModel.ClientIdentifier),
                new KeyValuePair<string, string>("response_type", "code"),
                new KeyValuePair<string, string>("redirect_uri", _consumerOAuthModel.RedirectUrl),
                new KeyValuePair<string, string>("scope", _consumerOAuthModel.ScopeUrl)
            };

            Uri uri = HttpUtility.BuildUri(_consumerOAuthModel.LoginUrl, queryParams);

            return uri;
        }

        /// <summary>
        /// Gets the Token using the authorisationCode
        /// </summary>
        /// <param name="authorisationCode">The Authorization Code</param>
        /// <returns>
        /// The OAuthResponse
        /// </returns>
        /// <exception cref="System.ArgumentNullException">authorisationCode</exception>
        public async Task<OAuthResponse> GetToken(string authorisationCode)
        {
            if (authorisationCode == null)
                throw new ArgumentNullException(nameof(authorisationCode));

            RestRequest request = new RestRequest(HttpMethod.Post, _restClient.EndPointUrl);

            var bodyParameters = new Dictionary<string, string>
            {
                {"client_id", _consumerOAuthModel.ClientIdentifier},
                {"client_secret", _consumerOAuthModel.ClientSecret},
                {"grant_type", "authorization_code"},
                {"redirect_uri", _consumerOAuthModel.RedirectUrl},
                {"format", "JSON"},
                {"code", authorisationCode}
            };

            request.SetJsonBodyParameters(bodyParameters);

            return await _restClient.ExecuteRequest<OAuthResponse>(request);
        }

        /// <summary>
        /// Gets the Refresh Token
        /// </summary>
        /// <param name="refreshToken">The Refresh Token</param>
        /// <returns>
        /// The OAuthResponse
        /// </returns>
        /// <exception cref="System.ArgumentNullException">refreshToken</exception>
        public async Task<OAuthResponse> GetRefreshToken(string refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            var request = new RestRequest(HttpMethod.Post, _restClient.EndPointUrl);

            var bodyParameters = new Dictionary<string, string>
            {
                {"client_id", _consumerOAuthModel.ClientIdentifier},
                {"client_secret", _consumerOAuthModel.ClientSecret},
                {"grant_type", "refresh_token"},
                {"format", "JSON"},
                {"refresh_token", refreshToken}
            };

            request.SetJsonBodyParameters(bodyParameters);

            return await _restClient.ExecuteRequest<OAuthResponse>(request);
        }
    }
}