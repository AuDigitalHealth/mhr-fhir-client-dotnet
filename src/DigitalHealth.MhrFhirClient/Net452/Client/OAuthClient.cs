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
using System;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Rest;

namespace DigitalHealth.MhrFhirClient.Client
{
    /// <summary>
    /// Internal base OAuth client.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IConsumerOAuthClient" />
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IProviderOAuthClient" />
    internal class OAuthClient : IConsumerOAuthClient, IProviderOAuthClient
    {
        /// <summary>
        /// The consumer client
        /// </summary>
        private readonly IConsumerOAuthClient _consumerClient;
        /// <summary>
        /// The provider client
        /// </summary>
        private readonly IProviderOAuthClient _providerClient;

        /// <summary>
        /// The constructor for the OAuth consumer client
        /// </summary>
        /// <param name="consumerOAuthModel">The Consumer OAuth Model</param>
        /// <param name="restClient">The rest client</param>
        internal OAuthClient(ConsumerOAuthModel consumerOAuthModel, IRestClient restClient)
        {
            ValidateConsumerOAuthModel(consumerOAuthModel);

            _consumerClient = new OAuthConsumerClient(restClient, consumerOAuthModel);
        }

        /// <summary>
        /// The constructor for the OAuth provider client
        /// </summary>
        /// <param name="providerOAuthModel">The provider o authentication model.</param>
        /// <param name="restClient">The rest client</param>
        internal OAuthClient(ProviderOAuthModel providerOAuthModel, IRestClient restClient)
        {
            ValidateProviderOAuthModel(providerOAuthModel);

            _providerClient = new OAuthProviderClient(restClient, providerOAuthModel);
        }

        #region The Consumer Client

        /// <summary>
        /// Get the OAuth Login for the consumer client
        /// </summary>
        /// <returns>
        /// The Login URI
        /// </returns>
        public Uri GetLoginUri()
        {
            return _consumerClient.GetLoginUri();
        }

        /// <summary>
        /// Gets the Token
        /// </summary>
        /// <param name="authorisationCode">The Authorization Code</param>
        /// <returns>
        /// The OAuthResponse
        /// </returns>
        public async Task<OAuthResponse> GetToken(string authorisationCode)
        {
            return await _consumerClient.GetToken(authorisationCode);
        }

        /// <summary>
        /// Gets the Refresh Token
        /// </summary>
        /// <param name="refreshToken">The Refresh Token</param>
        /// <returns>
        /// The OAuthResponse
        /// </returns>
        public async Task<OAuthResponse> GetRefreshToken(string refreshToken)
        {
            return await _consumerClient.GetRefreshToken(refreshToken);
        }

        #endregion

        #region The Provider Client

        /// <summary>
        /// Calls the Get Provider Token Service and returns a OAuthResponse
        /// </summary>
        /// <param name="providerHpii"></param>
        /// <param name="providerName"></param>
        /// <returns>
        /// OAuthResponse
        /// </returns>
        public async Task<OAuthResponse> GetProviderToken(string providerHpii, string providerName)
        {
            return await _providerClient.GetProviderToken(providerHpii, providerName);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that all parameters provided for the provider model are correct
        /// </summary>
        /// <param name="providerOAuthModel">The provider model</param>
        /// <exception cref="System.ArgumentNullException">
        /// providerOAuthModel
        /// or
        /// Certificate
        /// or
        /// TokenProviderEndpointUrl
        /// or
        /// ClientIdentifier
        /// or
        /// ClientSecret
        /// or
        /// RedirectUrl
        /// or
        /// Hpio
        /// or
        /// OrganisationName
        /// or
        /// DeviceIdentifier
        /// or
        /// DeviceMake
        /// or
        /// DeviceModel
        /// </exception>
        private void ValidateProviderOAuthModel(ProviderOAuthModel providerOAuthModel)
        {
            if (providerOAuthModel == null)
                throw new ArgumentNullException(nameof(providerOAuthModel));

            if (providerOAuthModel.Certificate == null)
                throw new ArgumentNullException(nameof(providerOAuthModel.Certificate));

            if (providerOAuthModel.TokenProviderEndpointUrl == null)
                throw new ArgumentNullException(nameof(providerOAuthModel.TokenProviderEndpointUrl));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.ClientIdentifier))
                throw new ArgumentNullException(nameof(providerOAuthModel.ClientIdentifier));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.ClientSecret))
                throw new ArgumentNullException(nameof(providerOAuthModel.ClientSecret));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.RedirectUrl))
                throw new ArgumentNullException(nameof(providerOAuthModel.RedirectUrl));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.Hpio))
                throw new ArgumentNullException(nameof(providerOAuthModel.Hpio));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.OrganisationName))
                throw new ArgumentNullException(nameof(providerOAuthModel.OrganisationName));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.DeviceIdentifier))
                throw new ArgumentNullException(nameof(providerOAuthModel.DeviceIdentifier));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.DeviceMake))
                throw new ArgumentNullException(nameof(providerOAuthModel.DeviceMake));

            if (string.IsNullOrWhiteSpace(providerOAuthModel.DeviceModel))
                throw new ArgumentNullException(nameof(providerOAuthModel.DeviceModel));
        }

        /// <summary>
        /// Validates that all parameters provided for the consumer model are correct
        /// </summary>
        /// <param name="consumerOAuthModel">The Consumer OAuth Model</param>
        /// <exception cref="System.ArgumentNullException">
        /// ClientIdentifier
        /// or
        /// ClientSecret
        /// or
        /// RedirectUrl
        /// or
        /// ScopeUrl
        /// or
        /// TokenEndPointUrl
        /// or
        /// LoginUrl
        /// </exception>
        /// <exception cref="UriFormatException">LoginUrl</exception>
        private void ValidateConsumerOAuthModel(ConsumerOAuthModel consumerOAuthModel)
        {
            if (string.IsNullOrWhiteSpace(consumerOAuthModel.ClientIdentifier))
                throw new ArgumentNullException(nameof(consumerOAuthModel.ClientIdentifier));

            if (string.IsNullOrWhiteSpace(consumerOAuthModel.ClientSecret))
                throw new ArgumentNullException(nameof(consumerOAuthModel.ClientSecret));

            if (string.IsNullOrWhiteSpace(consumerOAuthModel.RedirectUrl))
                throw new ArgumentNullException(nameof(consumerOAuthModel.RedirectUrl));

            if (string.IsNullOrWhiteSpace(consumerOAuthModel.ScopeUrl))
                throw new ArgumentNullException(nameof(consumerOAuthModel.ScopeUrl));

            if (consumerOAuthModel.TokenEndPointUrl == null)
                throw new ArgumentNullException(nameof(consumerOAuthModel.TokenEndPointUrl));

            if (string.IsNullOrWhiteSpace(consumerOAuthModel.LoginUrl))
                throw new ArgumentNullException(nameof(consumerOAuthModel.LoginUrl));

            if (!Uri.IsWellFormedUriString(consumerOAuthModel.LoginUrl, UriKind.Absolute))
                throw new UriFormatException(nameof(consumerOAuthModel.LoginUrl));
        }
         
        #endregion
    }
}
#endif