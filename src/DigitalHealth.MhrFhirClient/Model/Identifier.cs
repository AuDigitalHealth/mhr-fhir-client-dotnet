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

using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Extension;

namespace DigitalHealth.MhrFhirClient.Model
{
    /// <summary>
    /// Generic Identifier.
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// value.
        /// </value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// namespace.
        /// </value>
        internal string Namespace { get; set; }
        /// <summary>
        /// Gets or sets the type of the identifier.
        /// </summary>
        /// <value>
        /// type of the identifier.
        /// </value>
        internal IdentifierType IdentifierType { get; set; }

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="identifierType">Type of the identifier.</param>
        public Identifier(string value, IdentifierType identifierType)
        {
            Namespace = identifierType.Description();
            Value = value;
            IdentifierType = identifierType;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Namespace}|{Value}";
        }
    }
}
