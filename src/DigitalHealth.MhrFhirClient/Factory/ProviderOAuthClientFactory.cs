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
using System.Net.Http;
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Rest;

namespace DigitalHealth.MhrFhirClient.Factory
{
    /// <summary>
    /// The Provider OAuth client factory.
    /// </summary>
    public static class ProviderOAuthClientFactory
    {
        /// <summary>
        /// Creates the specified provider o authentication model.
        /// </summary>
        /// <param name="providerOAuthModel">The provider o authentication model.</param>
        /// <returns></returns>
        public static IProviderOAuthClient Create(ProviderOAuthModel providerOAuthModel)
        {
            var webRequestHandler =  new WebRequestHandler
            {
                ClientCertificates = { providerOAuthModel.Certificate }
            };

            IRestClient restClient = new RestClient(providerOAuthModel.TokenProviderEndpointUrl, webRequestHandler);

            return new OAuthProviderClient(restClient, providerOAuthModel);
        }
    }
}
#endif