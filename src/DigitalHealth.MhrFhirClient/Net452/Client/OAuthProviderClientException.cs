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
using System.Net;
using DigitalHealth.MhrFhirClient.Rest;
using Newtonsoft.Json.Linq;

namespace DigitalHealth.MhrFhirClient.Client
{
    /// <summary>
    /// Exception generated on Provider OAuth authentication.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Rest.RestException" />
    public class OAuthProviderClientException : RestException
    {
        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// error description.
        /// </value>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthProviderClientException"/> class.
        /// </summary>
        /// <param name="statusCode">The StatusCode</param>
        /// <param name="responseContent">The Response Content</param>
        public OAuthProviderClientException(HttpStatusCode statusCode, string responseContent) : base(statusCode, responseContent)
        {
            try
            {
                dynamic json = JObject.Parse(ResponseContent);
                Error = json.error;
                ErrorDescription = json.error_description;
            }
            finally
            {                
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthProviderClientException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OAuthProviderClientException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthProviderClientException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public OAuthProviderClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
#endif