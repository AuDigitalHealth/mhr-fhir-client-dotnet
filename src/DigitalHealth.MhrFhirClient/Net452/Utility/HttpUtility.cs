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
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DigitalHealth.MhrFhirClient.Utility
{
    /// <summary>
    ///  Utility class for building HTTP uri 
    /// </summary>
    internal static class HttpUtility
    {
        /// <summary>
        /// Builds a URI using the base endpoint and a list of query parameters.
        /// </summary>
        /// <param name="endpoint">Base endpoint.</param>
        /// <param name="queryParameters">Query parameters.</param>
        /// <returns>
        /// URI
        /// </returns>
        public static Uri BuildUri(string endpoint, IList<KeyValuePair<string,string>> queryParameters)
        {
            string[] queryParamList = queryParameters.Select(item => $"{WebUtility.UrlEncode(item.Key)}={WebUtility.UrlEncode(item.Value)}").ToArray();

            var uriBuilder = new UriBuilder(endpoint)
            {
                Query = string.Join("&", queryParamList)
            };

            return new Uri(uriBuilder.ToString());
        }
    }
}