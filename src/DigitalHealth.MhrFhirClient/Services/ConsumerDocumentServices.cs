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
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Utility;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Task = System.Threading.Tasks.Task;

namespace DigitalHealth.MhrFhirClient.Services
{
    /// <summary>
    /// The following APIs are used to manage medication and allergy information in an individual's Personal Health Summary document  (type/class code = ‘100.16685’) 
    /// in the My Health Record system.
    /// 
    /// The following APIs are classified under this group:
    ///   •	Personal Health Summary – Medications(GET)
    ///   •	Personal Health Summary – Medications(POST)
    ///   •	Personal Health Summary – Medications(PUT)
    ///   •	Personal Health Summary – Medications(DELETE)
    ///   •	Personal Health Summary – Allergies(GET)
    ///   •	Personal Health Summary – Allergies(POST)
    ///   •	Personal Health Summary – Allergies(PUT)
    ///   •	Personal Health Summary – Allergies(DELETE)
    /// </summary>
    internal class ConsumerDocumentServices
    {
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IMhrFhirRestClient _mhrFhirRestClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerDocumentServices"/> class.
        /// </summary>
        /// <param name="mhrFhirRestClient">The rest client.</param>
        internal ConsumerDocumentServices(IMhrFhirRestClient mhrFhirRestClient)
        {
            _mhrFhirRestClient = mhrFhirRestClient;
        }

        /// <summary>
        /// Retrieves medication items from an individual’s Personal Health Summary document. Use this operation to obtain existing medication items, and also 
        /// the Document ID of the Personal Health Summary document if medication items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'MedicationStatement' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">GetPersonalHealthSummaryMedications - patientId must be provided</exception>
        internal async Task<Bundle> GetPersonalHealthSummaryMedications(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }
                        
            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(MedicationStatement).Name, HttpMethod.Get);
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);

            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Creates one or more medication items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any medication items.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘MedicationStatement’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">CreatePersonalHealthSummaryMedications - medications must be provided</exception>
        public async Task<Bundle> CreatePersonalHealthSummaryMedications(Bundle medications)
        {
            if (medications == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(medications)), nameof(medications));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(MedicationStatement).Name, HttpMethod.Post);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(medications), FhirConstants.FhirJsonMediaType);
            
            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Adds a new medication item to existing medication items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing medication items.
        /// </summary>
        /// <param name="medication">The new 'MedicationStatement' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the newly created 'MedicationStatement' resource with the logical ID populated.</returns>
        /// <exception cref="System.ArgumentException">
        /// AddPersonalHealthSummaryMedication - medication must be provided
        /// or
        /// AddPersonalHealthSummaryMedication - docId must be provided
        /// </exception>
        public async Task<MedicationStatement> AddPersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            if (medication == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(medication)), nameof(medication));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }
           
            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(MedicationStatement).Name, HttpMethod.Post);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(medication), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<MedicationStatement>(request);
        }

        /// <summary>
        /// Updates medication items in the individual's Personal Health Summary document. Individual items can be created (using the verb POST), updated (PUT)
        /// and deleted (DELETE). 
        /// </summary>
        /// <param name="medications">The ‘Bundle’ of ‘MedicationStatement’ resources to be updated.
        /// The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ 
        /// value as ‘PUT’,‘POST’ or 'DELETE'.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Based on whether the ‘request.method’ is set as ‘PUT’ or ‘POST’ for each of the resources in the request Bundle entry, 
        /// the system will return a Bundle resource with HTTP Status Code 200 - Ok for ‘PUT’ request with the corresponding resource updated and 201 – Created for 
        /// ‘POST’ request with the corresponding resource being created. In case of error, an OperationOutcome resource with details as applicable will be returned.</returns>
        /// <exception cref="System.ArgumentException">
        /// UpdatePersonalHealthSummaryMedications - medications must be provided
        /// or
        /// UpdatePersonalHealthSummaryMedications - docId must be provided
        /// </exception>
        public async Task<Bundle> UpdatePersonalHealthSummaryMedications(Bundle medications, string docId)
        {
            if (medications == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(medications)), nameof(medications));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(MedicationStatement).Name, HttpMethod.Put);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(medications), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Updates a single medication item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        /// <exception cref="System.ArgumentException">
        /// UpdatePersonalHealthSummaryMedication - medication must be provided
        /// or
        /// UpdatePersonalHealthSummaryMedication - docId must be provided
        /// </exception>
        public async Task<MedicationStatement> UpdatePersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            if (medication == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(medication)), nameof(medication));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest($"{typeof(MedicationStatement).Name}/{medication.Id}", HttpMethod.Put);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(medication), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<MedicationStatement>(request);
        }

        /// <summary>
        /// Deletes a single medication item from the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <param name="medicationId">The medication identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// DeletePersonalHealthSummaryMedication - patientId must be provided
        /// or
        /// DeletePersonalHealthSummaryMedication - docId must be provided
        /// or
        /// DeletePersonalHealthSummaryMedication - medicationId must be provided
        /// </exception>
        public async Task DeletePersonalHealthSummaryMedication(string patientId, string docId, string medicationId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            if (string.IsNullOrWhiteSpace(medicationId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(medicationId)), nameof(medicationId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest($"{typeof(MedicationStatement).Name}/{medicationId}", HttpMethod.Delete);
            request.AddQueryParameter(FhirConstants.SourceTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);

            await _mhrFhirRestClient.ExecuteRequest(request);
        }


        

        
        /// <summary>
        /// Retrieves allergy items from an individual’s Personal Health Summary document. Use this operation to obtain existing allergy items, and also 
        /// the Document ID of the Personal Health Summary document if allergy items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'AllergyIntolerance' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">GetPersonalHealthSummaryAllergies - patientId must be provided</exception>
        internal async Task<Bundle> GetPersonalHealthSummaryAllergies(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }
            
            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(AllergyIntolerance).Name, HttpMethod.Get);
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);
            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);

            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Creates one or more allergy items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any allergy items.
        /// </summary>
        /// <param name="allergies">The bundle should contain ‘Bundle’ of type ‘AllergyIntolerance’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘AllergyIntolerance’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">CreatePersonalHealthSummaryAllergies - allergies must be provided</exception>
        public async Task<Bundle> CreatePersonalHealthSummaryAllergies(Bundle allergies)
        {
            if (allergies == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(allergies)), nameof(allergies));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(AllergyIntolerance).Name, HttpMethod.Post);
            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(allergies), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Adds a new allergy item to existing allergy items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing allergy items.
        /// </summary>
        /// <param name="allergy">The new 'AllergyIntolerance' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        /// <exception cref="System.ArgumentException">
        /// AddPersonalHealthSummaryAllergy - allergy must be provided
        /// or
        /// AddPersonalHealthSummaryAllergy - docId must be provided
        /// </exception>
        public async Task<AllergyIntolerance> AddPersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            if (allergy == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(allergy)), nameof(allergy));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(AllergyIntolerance).Name, HttpMethod.Post);
            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(allergy), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<AllergyIntolerance>(request);
        }

        /// <summary>
        /// Updates allergy items in the individual's Personal Health Summary document. Individual items can be created (using the verb POST), updated (PUT)
        /// and deleted (DELETE). 
        /// </summary>
        /// <param name="allergies">The ‘Bundle’ of ‘AllergyIntolerance’ resources to be updated.
        /// The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ 
        /// value as ‘PUT’,‘POST’ or 'DELETE'.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        /// <exception cref="System.ArgumentException">
        /// UpdatePersonalHealthSummaryAllergies - allergies must be provided
        /// or
        /// UpdatePersonalHealthSummaryAllergies - docId must be provided
        /// </exception>
        public async Task<Bundle> UpdatePersonalHealthSummaryAllergies(Bundle allergies, string docId)
        {
            if (allergies == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(allergies)), nameof(allergies));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest(typeof(AllergyIntolerance).Name, HttpMethod.Put);
            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(allergies), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Updates a single allergy item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="allergy">The ‘AllergyIntolerance’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        /// <exception cref="System.ArgumentException">
        /// UpdatePersonalHealthSummaryAllergy - allergy must be provided
        /// or
        /// UpdatePersonalHealthSummaryAllergy - docId must be provided
        /// </exception>
        public async Task<AllergyIntolerance> UpdatePersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            if (allergy == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(allergy)), nameof(allergy));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest($"{typeof(AllergyIntolerance).Name}/{allergy.Id}", HttpMethod.Put);
            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.SetJsonBodyParameters(FhirSerializer.SerializeResourceToJson(allergy), FhirConstants.FhirJsonMediaType);

            return await _mhrFhirRestClient.ExecuteRequest<AllergyIntolerance>(request);
        }

        /// <summary>
        /// Deletes a single allergy item from the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <param name="allergyId">The allergy identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// DeletePersonalHealthSummaryAllergy - patientId must be provided
        /// or
        /// DeletePersonalHealthSummaryAllergy - docId must be provided
        /// or
        /// DeletePersonalHealthSummaryAllergy - allergyIntoleranceId must be provided
        /// </exception>
        public async Task DeletePersonalHealthSummaryAllergy(string patientId, string docId, string allergyIntoleranceId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            if (string.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(docId)), nameof(docId));
            }

            if (string.IsNullOrWhiteSpace(allergyIntoleranceId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(allergyIntoleranceId)), nameof(allergyIntoleranceId));
            }

            var request = _mhrFhirRestClient.CreateMhrFhirRequest($"{typeof(AllergyIntolerance).Name}/{allergyIntoleranceId}", HttpMethod.Delete);

            request.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PatientValue);
            request.AddQueryParameter(FhirConstants.DocIdParameter, docId);
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);

            await _mhrFhirRestClient.ExecuteRequest(request);
        }
    }
}
