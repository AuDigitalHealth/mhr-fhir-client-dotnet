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
using DigitalHealth.MhrFhirClient.Enum;

namespace DigitalHealth.MhrFhirClient.Model
{
    /// <summary>
    /// Search Query.
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// This code is to identify type of the document.
        /// Note: either ‘class’ or ‘type’ is required in each DocumentReference request.
        /// </summary>
        /// <value>
        /// class code.
        /// </value>
        public IList<ClassCode> ClassCode { get; set; }

        /// <summary>
        /// Kind of document
        /// Note: either ‘class’ or ‘type’ is required in each DocumentReference request.
        /// </summary>
        /// <value>
        /// type code.
        /// </value>
        public IList<TypeCode> TypeCode { get; set; }

        /// <summary>
        /// Master Version Specific Identifier
        /// Note, search with identifier cannot be combined with any other search parameter.
        /// </summary>
        /// <value>
        /// identifier.
        /// </value>
        /// Optional Request Parameters
        public string Identifier { get; set; }

        /// <summary>
        /// Who and/or what authored the document
        /// </summary>
        /// <value>
        /// author.
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Document Start creation time
        /// </summary>
        /// <value>
        /// start date.
        /// </value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Document End creation time. Any future date provided will be defaulted to server current date.
        /// </summary>
        /// <value>
        /// end date.
        /// </value>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// current (approved)| superseded (deprecated)| entered-in-error (deleted)
        /// </summary>
        /// <value>
        /// status.
        /// </value>
        public Status? Status { get; set; }

        /// <summary>
        /// Any other custom slots applicable to meta-data as per IHE standard.
        /// list should exclude corresponding IHE slots values for: ‘identifier’, ’authenticator’, ‘author’, ‘custodian’, ‘format’, ’created’, ’status’.
        /// </summary>
        /// <value>
        /// name of the slot.
        /// </value>
        public string SlotName { get; set; }

        /// <summary>
        /// Value of the custom slot. This can exist only if the custom ‘slotName’ is provided.
        /// </summary>
        /// <value>
        /// slot value.
        /// </value>
        public string SlotValue { get; set; }
    }
}
