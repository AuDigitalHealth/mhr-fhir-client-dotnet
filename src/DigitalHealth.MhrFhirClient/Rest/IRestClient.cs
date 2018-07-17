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
using System.Net.Http;
using System.Threading.Tasks;

namespace DigitalHealth.MhrFhirClient.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Executes a rest call given the request.
        /// </summary>
        /// <typeparam name="T">The Generic Type</typeparam>
        /// <param name="request">The request</param>
        /// <returns>
        /// Generic Type
        /// </returns>
        Task<T> ExecuteRequest<T>(RestRequest request);

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> ExecuteRequest(RestRequest request);

        /// <summary>
        /// Endpoint.
        /// </summary>
        Uri EndPointUrl { get; }
    }
}