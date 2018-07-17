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

namespace DigitalHealth.MhrFhirClient.Model.OAuth
{
    /// <summary>
    /// Input model for the Consumer OAuth Client
    /// </summary>
    public class ConsumerOAuthModel
    {
        /// <summary>
        /// Client Identifier
        /// </summary>
        /// <value>
        /// client identifier.
        /// </value>
        public string ClientIdentifier { get; set; }

        /// <summary>
        /// client Secret
        /// </summary>
        /// <value>
        /// client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Token End Point URL
        /// </summary>
        /// <value>
        /// token end point URL.
        /// </value>
        public Uri TokenEndPointUrl { get; set; }

        /// <summary>
        /// Redirect URL
        /// </summary>
        /// <value>
        /// redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Scope URL
        /// </summary>
        /// <value>
        /// scope URL.
        /// </value>
        public string ScopeUrl { get; set; }

        /// <summary>
        /// Login URL
        /// </summary>
        /// <value>
        /// login URL.
        /// </value>
        public string LoginUrl { get; set; }
    }
}


