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
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Model.OAuth;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    /// Provider OAuth client.
    /// </summary>
    public interface IProviderOAuthClient
    {
        /// <summary>
        /// Calls the Get Provider Token Service
        /// </summary>
        /// <param name="providerHpii">The provider hpii.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>
        /// OAuthResponse
        /// </returns>
        Task<OAuthResponse> GetProviderToken(string providerHpii, string providerName);
    }
}
#endif