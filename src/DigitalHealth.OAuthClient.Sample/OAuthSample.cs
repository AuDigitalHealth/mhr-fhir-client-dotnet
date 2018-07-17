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
using System.Security.Cryptography.X509Certificates;
using DigitalHealth.MhrFhirClient.Factory;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model.OAuth;

namespace DigitalHealth.OAuthClient.Sample
{
    public static class OAuthSample
    {
        private static IProviderOAuthClient _sampleProviderOAuthClient;
        private static IConsumerOAuthClient _sampleConsumerOAuthClient;

        /// <summary>
        /// Provider Client Sample
        /// </summary>
        static void ProviderSample()
        {
            var providerOAuthModel = new ProviderOAuthModel
            {
                Certificate = new X509Certificate2("CertificateLocation","Password"),
                TokenProviderEndpointUrl = new Uri("TokenProviderEndpointUrl"),
                ClientIdentifier = "ClientIdentifier",
                ClientSecret = "ClientSecret",
                RedirectUrl = "RedirectUrl",
                Hpio = "Hpio",
                OrganisationName = "OrganisationName",
                DeviceModel = "DeviceModel",
                DeviceIdentifier = "DeviceIdentifier",
                DeviceMake = "DeviceMake"
            };

            _sampleProviderOAuthClient = ProviderOAuthClientFactory.Create(providerOAuthModel);

            var response = _sampleProviderOAuthClient.GetProviderToken("UserIdentifier", "UserName").Result;

            Console.WriteLine(response.AccessToken);
        }

        /// <summary>
        /// Consumer Client Sample
        /// </summary>
        static void ConsumerSample()
        {
            // Consumer SAMPLE

            var consumerOAuthModel = new ConsumerOAuthModel
            {
                ClientIdentifier = "ClientIdentifier",
                ClientSecret = "ClientSecret",
                LoginUrl = "LoginUrl",
                TokenEndPointUrl = new Uri("TokenEndPointUrl"),
                RedirectUrl = "RedirectUrl",
                ScopeUrl = "ScopeUrl"
            };

            _sampleConsumerOAuthClient = ConsumerOAuthClientFactory.Create(consumerOAuthModel);

            Uri loginUrl = _sampleConsumerOAuthClient.GetLoginUri();
            Console.WriteLine(loginUrl);

            var response = _sampleConsumerOAuthClient.GetToken("authorisationCode").Result;
            Console.WriteLine(response.AccessToken);
            Console.WriteLine(response.RefreshToken);

            response = _sampleConsumerOAuthClient.GetRefreshToken("refreshToken").Result;
            Console.WriteLine(response.AccessToken);
            Console.WriteLine(response.RefreshToken);
        }
    }
}
