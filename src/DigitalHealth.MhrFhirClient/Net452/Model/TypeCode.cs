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

namespace DigitalHealth.MhrFhirClient.Model
{
    /// <summary>
    /// Refer to typecode table for possible values.
    /// </summary>
    public class TypeCode
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the code system.
        /// </summary>
        /// <value>
        /// code system.
        /// </value>
        public string CodeSystem { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Code}^^{CodeSystem}";
        }
    }
}
