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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Extension;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model;
using DigitalHealth.MhrFhirClient.Utility;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient.Services
{
    /// <summary>
    /// There are two APIs classified under this group:
    /// •	Get Document(GET)
    /// •	Search Document List(GET)
    /// </summary>
    internal class GenericDocumentServices
    {
       

        private readonly IMhrFhirRestClient _restClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDocumentServices"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        internal GenericDocumentServices(IMhrFhirRestClient restClient)
        {
            _restClient = restClient;
        }


        /// <summary>
        /// This API provides the ability to retrieve a specific document for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.</param>
        /// <param name="documentId">The document id</param>
        /// <returns>
        /// The document returned as a Binary
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// GetDocument - PatientId must be provided
        /// or
        /// GetDocument - DocumentId must be provided
        /// </exception>
        internal async Task<Binary> GetDocument(string patientId, string documentId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(documentId)), nameof(documentId));
            }

            var request = _restClient.CreateMhrFhirRequest($"{typeof(Binary).Name}/{documentId}", HttpMethod.Get);
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);

            return await _restClient.ExecuteRequest<Binary>(request);
        }

        /// <summary>
        /// This API provides the ability to retrieve a list of document references for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.</param>
        /// <param name="searchQuery">The Search Query</param>
        /// <returns>
        /// A Bundle containing the result from the document search
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// SearchDocuments - PatientId must be provided
        /// or
        /// SearchDocuments - SearchQuery must be provided
        /// or
        /// SearchDocuments - Either ‘class’ or ‘type’ is required in each DocumentReference request
        /// or
        /// SearchDocuments - Search with identifier cannot be combined with any other search parameter
        /// </exception>
        internal async Task<Bundle> SearchDocuments(string patientId, SearchQuery searchQuery)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            if (searchQuery == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(searchQuery)), nameof(searchQuery));
            }

            if (searchQuery.ClassCode == null && searchQuery.TypeCode == null || searchQuery.ClassCode != null && searchQuery.TypeCode != null && !searchQuery.ClassCode.Any() && !searchQuery.TypeCode.Any())
            {
                throw new ArgumentException(MhrFhirClientResource.ClassOrTypeCodeRequiredError);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Identifier) &&

                   !(searchQuery.ClassCode == null &&
                    searchQuery.TypeCode == null &&
                    searchQuery.Status == null &&
                    !searchQuery.StartDate.HasValue &&
                    !searchQuery.EndDate.HasValue &&
                    string.IsNullOrWhiteSpace(searchQuery.Author) &&
                    string.IsNullOrWhiteSpace(searchQuery.SlotName) &&
                    string.IsNullOrWhiteSpace(searchQuery.SlotValue))
                )
            {
                throw new ArgumentException(MhrFhirClientResource.IdentifierSearchError);
            }
            
            var request = _restClient.CreateMhrFhirRequest(typeof(DocumentReference).Name, HttpMethod.Get);
            
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);

            if (!string.IsNullOrWhiteSpace(searchQuery.Identifier))
            {
                request.AddQueryParameter(FhirConstants.IdentifierParameter, searchQuery.Identifier);
            }

            if (searchQuery.ClassCode != null && searchQuery.ClassCode.Any())
            {
                request.AddQueryParameter(FhirConstants.ClassParameter, string.Join(",", searchQuery.ClassCode.ToList().Select(s => s.ToString())));
            }

            if (searchQuery.TypeCode != null && searchQuery.TypeCode.Any())
            {
                request.AddQueryParameter(FhirConstants.TypeParameter, string.Join(",", searchQuery.TypeCode.ToList().Select(s => s.ToString())));
            }

            if (searchQuery.StartDate.HasValue)
            {
                request.AddQueryParameter(FhirConstants.CreatedParameter, $"{FhirConstants.GreaterThanOrEqualToPrefix}{searchQuery.StartDate.Value.MrhClientDateToString()}");
            }

            if (searchQuery.EndDate.HasValue)
            {
                request.AddQueryParameter(FhirConstants.CreatedParameter, $"{FhirConstants.LessThanOrEqualToPrefix}{searchQuery.EndDate.Value.MrhClientDateToString()}");
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Author))
            {
                request.AddQueryParameter(FhirConstants.AuthorParameter, searchQuery.Author);
            }

            if (searchQuery.Status.HasValue)
            {
                request.AddQueryParameter(FhirConstants.StatusParameter, searchQuery.Status.Value.Description());
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.SlotName))
            {
                request.AddQueryParameter(FhirConstants.SlotNameParameter, searchQuery.SlotName);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.SlotValue))
            {
                request.AddQueryParameter(FhirConstants.SlotValueParameter, searchQuery.SlotValue);
            }

            return await _restClient.ExecuteRequest<Bundle>(request);
        }
    }
}
