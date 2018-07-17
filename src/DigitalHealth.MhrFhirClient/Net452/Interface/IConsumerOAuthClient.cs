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
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Model.OAuth;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    ///  Auth client for consumer endpoints.
    /// </summary>
    public interface IConsumerOAuthClient
    {
        /// <summary>
        /// Get the OAuth Login Uri for the consumer client
        /// </summary>
        /// <returns>
        /// Login URI
        /// </returns>
        Uri GetLoginUri();

        /// <summary>
        /// Gets the Token using the authorisationCode
        /// </summary>
        /// <param name="authorisationCode">The Authorization Code</param>
        /// <returns>
        /// OAuthResponse
        /// </returns>
        Task<OAuthResponse> GetToken(string authorisationCode);

        /// <summary>
        /// Gets the Refresh Token
        /// </summary>
        /// <param name="refreshToken">The Refresh Token</param>
        /// <returns>
        /// OAuthResponse
        /// </returns>
        Task<OAuthResponse> GetRefreshToken(string refreshToken);
    }
}
