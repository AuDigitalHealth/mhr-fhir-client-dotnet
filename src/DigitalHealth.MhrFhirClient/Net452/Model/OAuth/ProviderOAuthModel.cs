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
using System.Security.Cryptography.X509Certificates;

namespace DigitalHealth.MhrFhirClient.Model.OAuth
{
    /// <summary>
    /// Input model for the Provider OAuth Client
    /// </summary>
    public class ProviderOAuthModel
    {
        /// <summary>
        /// provider Certificate
        /// </summary>
        /// <value>
        /// certificate.
        /// </value>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Token Provider Endpoint URL
        /// </summary>
        /// <value>
        /// token provider endpoint URL.
        /// </value>
        public Uri TokenProviderEndpointUrl { get; set; }

        /// <summary>
        /// Client Identifier
        /// </summary>
        /// <value>
        /// client identifier.
        /// </value>
        public string ClientIdentifier { get; set; }

        /// <summary>
        /// Client Secret
        /// </summary>
        /// <value>
        /// client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Redirect URL
        /// </summary>
        /// <value>
        /// redirect URL.
        /// </value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// HPIO
        /// </summary>
        /// <value>
        /// hpio.
        /// </value>
        public string Hpio { get; set; }

        /// <summary>
        /// Organisation Name
        /// </summary>
        /// <value>
        /// name of the organisation.
        /// </value>
        public string OrganisationName { get; set; }

        ///// <summary>
        ///// User Identifier
        ///// </summary>
        //public string UserIdentifier { get; set; }

        ///// <summary>
        ///// User Name 
        ///// </summary>
        //public string UserName { get; set; }

        /// <summary>
        /// Device Identifier
        /// </summary>
        /// <value>
        /// device identifier.
        /// </value>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// Device Make
        /// </summary>
        /// <value>
        /// device make.
        /// </value>
        public string DeviceMake { get; set; }

        /// <summary>
        /// Device Model
        /// </summary>
        /// <value>
        /// device model.
        /// </value>
        public string DeviceModel { get; set; }
    }
}
#endif

