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
        /// Creates one or more allergy items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any allergy items.
        /// </summary>
        /// <param name="allergies">The bundle should contain ‘Bundle’ of type ‘AllergyIntolerance’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘AllergyIntolerance’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<Bundle> CreatePersonalHealthSummaryAllergies(Bundle allergies);

        /// <summary>
        /// Adds a new allergy item to existing allergy items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing allergy items.
        /// </summary>
        /// <param name="allergy">The new 'AllergyIntolerance' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        Task<AllergyIntolerance> AddPersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId);

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
        Task<Bundle> UpdatePersonalHealthSummaryAllergies(Bundle allergies, string docId);

        /// <summary>
        /// Updates a single allergy item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="allergy">The ‘AllergyIntolerance’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        Task<AllergyIntolerance> UpdatePersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId);

        /// <summary>
        /// Deletes a single allergy item from the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <param name="allergyId">The allergy identifier.</param>
        /// <returns></returns>
        Task DeletePersonalHealthSummaryAllergy(string patientId, string docId, string allergyIntoleranceId);





        /// <summary>
        /// Creates one or more medication items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any medication items.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘MedicationStatement’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<Bundle> CreatePersonalHealthSummaryMedications(Bundle medications);

        /// <summary>
        /// Adds a new medication item to existing medication items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing medication items.
        /// </summary>
        /// <param name="medication">The new 'MedicationStatement' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the newly created 'MedicationStatement' resource with the logical ID populated.</returns>
        Task<MedicationStatement> AddPersonalHealthSummaryMedication(MedicationStatement medication, string docId);

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
        Task<Bundle> UpdatePersonalHealthSummaryMedications(Bundle medications, string docId);

        /// <summary>
        /// Updates a single medication item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        Task<MedicationStatement> UpdatePersonalHealthSummaryMedication(MedicationStatement medication, string docId);

        /// <summary>
        /// Deletes a single medication item from the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <param name="medicationId">The medication identifier.</param>
        /// <returns></returns>
        Task DeletePersonalHealthSummaryMedication(string patientId, string docId, string medicationId);
        
    }
}
