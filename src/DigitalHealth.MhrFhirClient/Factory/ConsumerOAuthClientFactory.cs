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
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Rest;

namespace DigitalHealth.MhrFhirClient.Factory
{
    /// <summary>
    /// The Consumer OAuth client factory.
    /// </summary>
    public static class ConsumerOAuthClientFactory
    {
        /// <summary>
        /// Creates the specified consumer o authentication model.
        /// </summary>
        /// <param name="consumerOAuthModel">The consumer o authentication model.</param>
        /// <returns></returns>
        public static IConsumerOAuthClient Create(ConsumerOAuthModel consumerOAuthModel)
        {
            IRestClient restClient = new RestClient(consumerOAuthModel.TokenEndPointUrl);

            return new OAuthClient(consumerOAuthModel, restClient);
        }
    }
}
#endif