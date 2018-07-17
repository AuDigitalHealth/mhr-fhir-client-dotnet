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

using Newtonsoft.Json;

namespace DigitalHealth.MhrFhirClient.Model.OAuth
{
    /// <summary>
    /// Response object representing authentication response
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        /// Access Token
        /// </summary>
        /// <value>
        /// access token.
        /// </value>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Expires In Property
        /// </summary>
        /// <value>
        /// expires in.
        /// </value>
        [JsonProperty(PropertyName = "expires_in")]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Token Type Property
        /// </summary>
        /// <value>
        /// type of the token.
        /// </value>
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        /// <value>
        /// scope.
        /// </value>
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <value>
        /// refresh token.
        /// </value>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
