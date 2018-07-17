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

namespace DigitalHealth.MhrFhirClient.Rest
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RestException : Exception
    {
        /// <summary>
        /// status code
        /// </summary>
        public HttpStatusCode StatusCode;
        /// <summary>
        /// response content
        /// </summary>
        public string ResponseContent;

        /// <summary>
        /// constructor for the Consumer OAuth Client Exception
        /// </summary>
        /// <param name="statusCode">The StatusCode</param>
        /// <param name="responseContent">The Response Content</param>
        public RestException(HttpStatusCode statusCode, string responseContent)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RestException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}