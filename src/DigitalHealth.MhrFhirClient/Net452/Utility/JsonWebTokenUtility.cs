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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DigitalHealth.MhrFhirClient.Utility
{
    /// <summary>
    /// Helper class for constructing JSON Web tokens
    /// </summary>
    public static class JsonWebTokenUtility
    {
        /// <summary>
        /// The time offset
        /// </summary>
        const int TimeOffset = 60;

        /// <summary>
        /// Creates a JSON Web Token
        /// </summary>
        /// <param name="clientId">The Client Identifier</param>
        /// <param name="clientSecret">The Client Secret</param>
        /// <param name="redirectUrl">The RedirectUrl URL</param>
        /// <param name="hpio">The HPIO</param>
        /// <param name="userId">The User Identifier</param>
        /// <returns>
        /// A JSON Web Token string
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// clientId
        /// or
        /// clientSecret
        /// or
        /// redirectUrl
        /// or
        /// hpio
        /// or
        /// userId
        /// </exception>
        /// <exception cref="System.ArgumentException">clientSecret</exception>
        public static string GetJsonWebToken(string clientId, string clientSecret, string redirectUrl, string hpio, string userId)
        {
            if (clientId == null)
                throw new ArgumentNullException(nameof(clientId));

            if (clientSecret == null)
                throw new ArgumentNullException(nameof(clientSecret));

            if (redirectUrl == null)
                throw new ArgumentNullException(nameof(redirectUrl));

            if (hpio == null)
                throw new ArgumentNullException(nameof(hpio));

            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(clientSecret));
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(signingCredentials);

            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var epoch = (int)t.TotalSeconds;
            var uuid = "uuid:" + Guid.NewGuid();

            var payload = new JwtPayload
            {
                {"iss", clientId},
                {"aud", redirectUrl},
                {"exp", epoch + TimeOffset},
                {"iat", epoch - TimeOffset},
                {"jti", uuid},
                {"organisationID", hpio},
                {"userID", userId},
            };

            var secToken = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            try
            {
                return handler.WriteToken(secToken);
            }
            catch (ArgumentOutOfRangeException e)
            {
                if (e.Message.Contains("IDX10603"))
                {
                    throw new ArgumentException(nameof(clientSecret), e);
                }
            }

            return null;
        }

        /// <summary>
        /// Validates the token
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="secret">The clientSecret</param>
        /// <param name="clientId">The clientId (Optional Field)</param>
        /// <param name="redirectUrl">The redirectUrl (Optional Field)</param>
        /// <returns>
        /// Returns a security token or a null if not valid
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// secret
        /// </exception>
        public static SecurityToken VerifyToken(string token, string secret, string clientId = null, string redirectUrl = null)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (secret == null)
                throw new ArgumentNullException(nameof(secret));

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secret)),
                ValidIssuer = clientId,
                ValidAudience = redirectUrl,
                ValidateLifetime = true,
                ValidateAudience = redirectUrl != null,
                ValidateIssuer = clientId != null,
                ValidateIssuerSigningKey = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch 
            {
                validatedToken = null;
            }

            return validatedToken;
        }
    }
}
#endif