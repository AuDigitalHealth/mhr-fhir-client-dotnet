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

using System.Net.Http;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Rest;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    /// Rest client implementation used in consumer and provider client.
    /// </summary>
    internal interface IMhrFhirRestClient
    {
        /// <summary>
        /// Creates the RestRequest
        /// </summary>
        /// <param name="resource">The URL resource</param>
        /// <param name="method">The HTTP Method type</param>
        /// <returns>
        /// Rest Request
        /// </returns>
        RestRequest CreateMhrFhirRequest(string resource, HttpMethod method);

        /// <summary>
        /// Executes a rest call given the request
        /// </summary>
        /// <typeparam name="T">The Generic Type</typeparam>
        /// <param name="request">The request</param>
        /// <returns>
        /// Generic Type
        /// </returns>
        Task<T> ExecuteRequest<T>(RestRequest request) where T : Resource;

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>
        /// Response
        /// </returns>
        Task<HttpResponseMessage> ExecuteRequest(RestRequest request);
    }
}