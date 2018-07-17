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

using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    /// Consumer MHR FHIR client.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IMhrFhirBaseClient" />
    public interface IMhrFhirConsumerClient : IMhrFhirBaseClient
    {
        /// <summary>
        /// Gets the patient details.
        /// </summary>
        /// <returns></returns>
        Task<Bundle> GetPatientDetails();
       
        /// <summary>
        /// Gets the record list.
        /// </summary>
        /// <returns></returns>
        Task<Bundle> GetRecordList();

        /// <summary>
        /// Creates the personal health summary allergies.
        /// </summary>
        /// <param name="allergies">The allergies.</param>
        /// <returns></returns>
        Task<Bundle> CreatePersonalHealthSummaryAllergies(Bundle allergies);
       
        /// <summary>
        /// Adds the personal health summary allergy.
        /// </summary>
        /// <param name="allergy">The allergy.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        Task<AllergyIntolerance> AddPersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId);
       
        /// <summary>
        /// Updates the personal health summary allergies.
        /// </summary>
        /// <param name="allergies">The allergies.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        Task<Bundle> UpdatePersonalHealthSummaryAllergies(Bundle allergies, string docId);
       
        /// <summary>
        /// Updates the personal health summary allergy.
        /// </summary>
        /// <param name="allergy">The allergy.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns></returns>
        Task<AllergyIntolerance> UpdatePersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId);
       
        /// <summary>
        /// Deletes the personal health summary allergy.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="docId">The document identifier.</param>
        /// <param name="allergyIntoleranceId">The allergy intolerance identifier.</param>
        /// <returns></returns>
        Task DeletePersonalHealthSummaryAllergy(string patientId, string docId, string allergyIntoleranceId);





        /// <summary>
        /// This API provides the ability to create an individual’s MedicationStatement in the Personal Health Summary document (type/class code = ‘100.16685’) stored in the My Health Record system. Use this API if there is no Personal Summary Document exits for the individual, or if the existing document doesn’t have any Medication section.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’. This is a custom implementation of Bundle where the Bundle is posted on the MedicationStatement URI instead of using the base URI.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome..</returns>
        Task<Bundle> CreatePersonalHealthSummaryMedications(Bundle medications);
       
        /// <summary>
        /// Adds the personal health summary medication.
        /// </summary>
        /// <param name="medication">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<MedicationStatement> AddPersonalHealthSummaryMedication(MedicationStatement medication, string docId);

        /// <summary>
        /// Updates the personal health summary medications.
        /// </summary>
        /// <param name="medications">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<Bundle> UpdatePersonalHealthSummaryMedications(Bundle medications, string docId);

        /// <summary>
        /// Updates the personal health summary medication.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<MedicationStatement> UpdatePersonalHealthSummaryMedication(MedicationStatement medication, string docId);

        /// <summary>
        /// Deletes the personal health summary medication.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <param name="medicationId">The medication identifier.</param>
        /// <returns></returns>
        Task DeletePersonalHealthSummaryMedication(string patientId, string docId, string medicationId);
        
    }
}
