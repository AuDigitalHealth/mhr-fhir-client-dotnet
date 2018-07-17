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
        /// This API provides the ability to retrieve Medications from an individual’s personal health summary document from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
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
        /// This API provides the ability to create an individual’s MedicationStatement in the Personal Health Summary document (type/class code = ‘100.16685’) stored in the My Health Record system. Use this API if there is no Personal Summary Document exits for the individual, or if the existing document doesn’t have any Medication section.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’. This is a custom implementation of Bundle where the Bundle is posted on the MedicationStatement URI instead of using the base URI.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
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
        /// Adds the personal health summary medication.
        /// </summary>
        /// <param name="medication">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
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
        /// Updates the personal health summary medications.
        /// </summary>
        /// <param name="medications">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
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
        /// Updates the personal health summary medication.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
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
        /// Deletes the personal health summary medication.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
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
        /// This API provides the ability to retrieve allergies and adverse reactions information from an individual’s Personal Health Summary document from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Bundle of allergies and adverse reactions</returns>
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
        /// This API provides the ability to create an individual’s allergies and adverse reactions information in the Personal Health Summary document stored in the My Health Record system.
        /// </summary>
        /// <param name="allergies">The request body should contain ‘Bundle’ of ‘AllergyIntolerance’ resources.</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
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
        /// Adds the personal health summary allergy.
        /// </summary>
        /// <param name="allergy">The request body should contain instance of allergy.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
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
        /// Updates the personal health summary allergies.
        /// </summary>
        /// <param name="allergies">The request body should contain ‘Bundle’ of ‘AllergyIntolerance’ resources with the document ID in the URI. </param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
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
        /// Updates the personal health summary allergy.
        /// </summary>
        /// <param name="allergy">The request body should contain instance of allergy.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
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
        /// Deletes the personal health summary allergy.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <param name="allergyIntoleranceId">The allergy intolerance identifier.</param>
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
