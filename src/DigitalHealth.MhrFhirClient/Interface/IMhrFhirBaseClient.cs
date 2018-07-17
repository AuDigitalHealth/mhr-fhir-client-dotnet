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
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Model;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    /// Interface for the base client that represents common functionality shared between Consumer and Provider client.
    /// </summary>
    public interface IMhrFhirBaseClient
    {
        /// <summary>
        /// Gets the prescriptions.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateWrittenFrom">From Date is a search criterion to select the MedicationOrder whose start date is after the specific period.</param>
        /// <param name="dateWrittenTo">To Date is a search criterion to select the MedicationOrder whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <returns>Returns a bundle of Medication Order if successful or an Operational Outcome.</returns>
        Task<Bundle> GetPrescriptions(string patientId, DateTime? dateWrittenFrom, DateTime? dateWrittenTo);

        /// <summary>
        /// Gets the dispenses.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateHandedOverFrom">From Date is a search criteria to select the documents whose start date is after the specific period.</param>
        /// <param name="dateHandedOverTo">To Date is a search criteria to select the documents whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <param name="includeAuthorizingPrescription">if set to <c>true</c> [Include the prescription reference in the response].</param>
        /// <returns> Returns a bundle of Medication Dispense if successful or an Operational Outcome.</returns>
        Task<Bundle> GetDispenses(string patientId, DateTime? dateHandedOverFrom, DateTime? dateHandedOverTo, bool includeAuthorizingPrescription);

        /// <summary>
        /// Gets the shared health summary allergies.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> Returns a bundle of Allergy Intolerance if successful or an Operational Outcome.</returns>
        Task<Bundle> GetSharedHealthSummaryAllergies(string patientId);

        /// <summary>
        /// This API provides the ability to retrieve a specific document for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns>The system will return ‘Binary’.</returns>
        Task<Binary> GetDocument(string patientId, string docId);

        /// <summary>
        /// This API provides the ability to retrieve a list of document references for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="query">The query object containing parameters for search.</param>
        /// <returns> A Bundle containing the result from the document search</returns>
        Task<Bundle> SearchDocuments(string patientId, SearchQuery query);

        /// <summary>
        ///This API offers the following capabilities:
        /// • Consumer and provider can retrieve(access control logic applied) individual’s demographic details as available in the My Health Record system.
        /// • Provider can verify if a particular patient exists in the My Health Record system without gaining access to the record.
        /// • Provider can gain access to a particular patient’s record to view the associated details.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> A patient resource</returns>
        Task<Patient> GetPatientDetails(string patientId);

        /// <summary>
        /// This API provides the ability to retrieve PBS details in the form of ExplanationOfBenefit FHIR® resource from the My Health Record system. This API is accessible by both consumers and providers.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createDateTo">The create date to.</param>
        /// <returns> Returns a bundle of PBS Items if successful or an Operational Outcome.</returns>
        Task<Bundle> GetPbsItems(string patientId, DateTime? createdDateFrom, DateTime? createDateTo);

        /// <summary>
        /// This API provides the ability to retrieve MBS details for the individual and returns ExplanationOfBenefit resource from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createdDateTo">The created date to.</param>
        /// <returns>Returns a bundle of MBS Items if successful or an Operational Outcome.</returns>
        Task<Bundle> GetMbsItems(string patientId, DateTime? createdDateFrom, DateTime? createdDateTo);

        /// <summary>
        /// Retrieves medication items from an individual’s Personal Health Summary document. Use this operation to obtain existing medication items, and also 
        /// the Document ID of the Personal Health Summary document if medication items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'MedicationStatement' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<Bundle> GetPersonalHealthSummaryMedications(string patientId);

        /// <summary>
        /// Retrieves allergy items from an individual’s Personal Health Summary document. Use this operation to obtain existing allergy items, and also 
        /// the Document ID of the Personal Health Summary document if allergy items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'AllergyIntolerance' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        Task<Bundle> GetPersonalHealthSummaryAllergies(string patientId);
    }
}
