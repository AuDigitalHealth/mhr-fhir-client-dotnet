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
using System.Net;
using DigitalHealth.MhrFhirClient.Rest;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient
{
    /// <summary>
    ///  Custom exception detailing operation outcome for a REST request.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Rest.RestException" />
    public class MhrFhirException : RestException
    {
        /// <summary>
        /// Gets or sets the operation outcome.
        /// </summary>
        /// <value>
        /// The operation outcome.
        /// </value>
        public OperationOutcome OperationOutcome { get; set; }

        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>
        /// The status description.
        /// </value>
        public string StatusDescription { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MhrFhirException"/> class.
        /// </summary>
        /// <param name="operationOutcome">The operation outcome.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        /// <param name="responseContent">Content of the response.</param>
        public MhrFhirException(OperationOutcome operationOutcome, HttpStatusCode statusCode, string statusDescription, string responseContent):base(statusCode,responseContent)
        {
            OperationOutcome = operationOutcome;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            ResponseContent = responseContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MhrFhirException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MhrFhirException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MhrFhirException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MhrFhirException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
