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

namespace DigitalHealth.MhrFhirClient.Extension
{
    /// <summary>
    /// 
    /// </summary>
    internal static class MhrClientDateExtension
    {
        /// <summary>
        /// MRHs the client date to string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string MrhClientDateToString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
