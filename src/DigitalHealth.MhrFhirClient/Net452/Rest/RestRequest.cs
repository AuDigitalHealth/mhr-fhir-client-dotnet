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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DigitalHealth.MhrFhirClient.Utility;

namespace DigitalHealth.MhrFhirClient.Rest
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpRequestMessage" />
    public class RestRequest : HttpRequestMessage
    {
        /// <summary>
        /// query parameters
        /// </summary>
        private readonly IList<KeyValuePair<string,string>> _queryParameters = new List<KeyValuePair<string, string>>();


        /// <summary>
        /// Creates the rest request
        /// </summary>
        /// <param name="method">The HttpMethod</param>
        /// <param name="baseAddress">The baseAddress</param>
        public RestRequest(HttpMethod method, Uri baseAddress) :base (method, baseAddress)
        {
        }

        /// <summary>
        /// Sets Body Parameters for the rest request
        /// </summary>
        /// <param name="bodyParameters">The body parameters dictionary</param>
        /// <param name="contentType">The Content Type</param>
        public void SetJsonBodyParameters(Dictionary<string, string> bodyParameters, string contentType = "application/x-www-form-urlencoded")
        {
            Content = new FormUrlEncodedContent(bodyParameters);
        }

        /// <summary>
        /// Sets Body Parameters for the rest request
        /// </summary>
        /// <param name="jsonContent">Serialized Json Content</param>
        /// <param name="contentType">The Content Type</param>
        public void SetJsonBodyParameters(string jsonContent, string contentType)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, contentType);

            // NOTE by default when using StringContent the 'charset=utf-8' property is added after the content type which is rejected by the server
            Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        /// <summary>
        /// Sets Query Parameter for the rest request
        /// </summary>
        /// <param name="queryParameters">The query parameters dictionary</param>
        public void SetQueryParameters(IList<KeyValuePair<string,string>> queryParameters)
        {
            RequestUri = HttpUtility.BuildUri(RequestUri.AbsoluteUri, queryParameters);
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddHeader(string name, string value)
        {
            Headers.Add(name,value);
        }

        /// <summary>
        /// Adds the query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddQueryParameter(string key, string value)
        {
            _queryParameters.Add(new KeyValuePair<string, string>(key, value));

            SetQueryParameters(_queryParameters);
        }
    }
}
