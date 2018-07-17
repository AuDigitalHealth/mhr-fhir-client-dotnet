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
using System.Net.Http;
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Interface;
#if !PORTABLE
using System.Security.Cryptography.X509Certificates;
#endif

namespace DigitalHealth.MhrFhirClient.Factory
{
    /// <summary>
    /// MHR FHIR Provider client factory.
    /// </summary>
    public static class MhrFhirProviderClientFactory
    {
        /// <summary>
        /// Creates the specified base endpoint.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientAppVersion">The client application version.</param>
        /// <returns></returns>
        public static IMhrFhirProviderClient Create(Uri baseEndpoint, string bearerToken, string clientId, string clientAppVersion)
        {
            IMhrFhirRestClient mhrFhirRestClient = new MhrFhirRestClient(baseEndpoint, bearerToken, clientId, clientAppVersion, new HttpClientHandler());

            return new MhrFhirProviderClient(mhrFhirRestClient);
        }

#if !PORTABLE
        /// <summary>
        /// Creates the specified base endpoint.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientAppVersion">The client application version.</param>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <returns></returns>
        public static IMhrFhirProviderClient Create(Uri baseEndpoint, string bearerToken, string clientId, string clientAppVersion, X509Certificate2 clientCertificate)
        {
            var webRequestHandler = new WebRequestHandler
            {
                ClientCertificates = { clientCertificate }
            };

            IMhrFhirRestClient mhrFhirRestClient = new MhrFhirRestClient(baseEndpoint, bearerToken, clientId, clientAppVersion, webRequestHandler);

            return new MhrFhirProviderClient(mhrFhirRestClient);
        }
#endif
    }
}