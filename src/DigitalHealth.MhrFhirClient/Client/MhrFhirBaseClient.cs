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
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model;
using DigitalHealth.MhrFhirClient.Services;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace DigitalHealth.MhrFhirClient.Client
{
    /// <summary>
    /// Base client for both consumer and provider clients.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IMhrFhirBaseClient" />
    internal class MhrFhirBaseClient : IMhrFhirBaseClient
    {
        /// <summary>
        /// Clinical document services
        /// </summary>
        private readonly ClinicalDocumentServices _clinicalDocumentServices;

        /// <summary>
        /// Generic document services
        /// </summary>
        private readonly GenericDocumentServices _genericDocumentServices;

        /// <summary>
        /// Identification services
        /// </summary>
        private readonly IdentificationServices _identificationServices;

        /// <summary>
        /// Medicare information services
        /// </summary>
        private readonly MedicareInformationServices _medicareInformationServices;

        /// <summary>
        /// Consumer document services
        /// </summary>
        private readonly ConsumerDocumentServices _consumerDocumentServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="MhrFhirBaseClient"/> class.
        /// <param name="mhrFhirRestClient">MHR FHIR rest client.</param>
        /// </summary>
        public MhrFhirBaseClient(IMhrFhirRestClient mhrFhirRestClient)
        {
            _clinicalDocumentServices = new ClinicalDocumentServices(mhrFhirRestClient);
            _genericDocumentServices = new GenericDocumentServices(mhrFhirRestClient);
            _identificationServices = new IdentificationServices(mhrFhirRestClient);
            _medicareInformationServices = new MedicareInformationServices(mhrFhirRestClient);
            _consumerDocumentServices = new ConsumerDocumentServices(mhrFhirRestClient);
        }

        #region Clinical Document Service

        /// <summary>
        /// Gets the prescriptions.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateWrittenFrom">From Date is a search criterion to select the MedicationOrder whose start date is after the specific period.</param>
        /// <param name="dateWrittenTo">To Date is a search criterion to select the MedicationOrder whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <returns>Returns a bundle of Medication Order if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetPrescriptions(string patientId, DateTime? dateWrittenFrom, DateTime? dateWrittenTo)
        {
            return await _clinicalDocumentServices.GetPrescriptions(patientId, dateWrittenFrom, dateWrittenTo);
        }

        /// <summary>
        /// Gets the dispenses.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateHandedOverFrom">From Date is a search criteria to select the documents whose start date is after the specific period.</param>
        /// <param name="dateHandedOverTo">To Date is a search criteria to select the documents whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <param name="includeAuthorizingPrescription">if set to <c>true</c> [Include the prescription reference in the response].</param>
        /// <returns> Returns a bundle of Medication Dispense if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetDispenses(string patientId, DateTime? dateHandedOverFrom, DateTime? dateHandedOverTo, bool includeAuthorizingPrescription)
        {
            return await _clinicalDocumentServices.GetDispenses(patientId, dateHandedOverFrom, dateHandedOverTo, includeAuthorizingPrescription);
        }

        /// <summary>
        /// Gets the shared health summary allergies.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> Returns a bundle of Allergy Intolerance if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetSharedHealthSummaryAllergies(string patientId)
        {
            return await _clinicalDocumentServices.GetSharedHealthSummaryAllergies(patientId);
        }

        #endregion

        #region Generic Document Service

        /// <summary>
        /// This API provides the ability to retrieve a specific document for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">The document identifier.</param>
        /// <returns>The system will return ‘Binary’.</returns>
        public async Task<Binary> GetDocument(string patientId, string docId)
        {
            return await _genericDocumentServices.GetDocument(patientId, docId);
        }

        /// <summary>
        /// This API provides the ability to retrieve a list of document references for an individual from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="query">The query object containing parameters for search.</param>
        /// <returns> A Bundle containing the result from the document search</returns>
        public async Task<Bundle> SearchDocuments(string patientId, SearchQuery query)
        {
            return await _genericDocumentServices.SearchDocuments(patientId, query);
        }

        #endregion

        #region Identification Service

        /// <summary>
        ///This API offers the following capabilities:
        /// • Consumer and provider can retrieve(access control logic applied) individual’s demographic details as available in the My Health Record system.
        /// • Provider can verify if a particular patient exists in the My Health Record system without gaining access to the record.
        /// • Provider can gain access to a particular patient’s record to view the associated details.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> A patient resource</returns>
        public async Task<Patient> GetPatientDetails(string patientId)
        {
            return await _identificationServices.GetPatientDetails(patientId);
        }

        /// <summary>
        /// Verify if a particular patient exists in the My Health Record system without gaining access to the record.
        /// </summary>
        /// <param name="patientSearch">The patient search.</param>
        /// <returns>A Bundle with patient resource</returns>
        public async Task<Bundle> VerifyPatientExists(PatientSearch patientSearch)
        {
            return await _identificationServices.VerifyPatientExists(patientSearch);
        }

        /// <summary>
        /// Return Access to a Patient’s record to view the associated details.
        /// </summary>
        /// <param name="id">The Patient’s id</param>
        /// <param name="patientSearch">The PatientSearch Parameters</param>
        /// <param name="accessType">The accessType</param>
        /// <param name="accessCode">The accessCode</param>
        /// <returns>
        /// Parameters
        /// </returns>
        public async Task<Parameters> GainAccessToPatientRecord(string id, PatientSearch patientSearch, AccessType accessType, string accessCode = null)
        {
            return await _identificationServices.GainAccessToPatientRecord(id, patientSearch, accessType, accessCode);
        }

        /// <summary>
        /// Return Access to the current Patient’s record to view the associated details.
        /// </summary>
        /// <returns>
        /// A Bundle of patient resource
        /// </returns>
        public async Task<Bundle> GetPatientDetails()
        {
            return await _identificationServices.GetPatientDetails();
        }

        /// <summary>
        /// This API provides the ability to retrieve the list of records the individual is permitted to access and returns a bundle containing selected Person resource for each accessible record.
        /// </summary>
        /// <returns>
        /// A Bundle of list of records
        /// </returns>
        public async Task<Bundle> GetRecordList()
        {
            return await _identificationServices.GetRecordList();
        }

        #endregion

        #region Medicare Information Service

        /// <summary>
        /// Returns a bundle of Pharmaceutical Benefits Scheme (PBS) Items if successful or an Operational Outcome.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">Start Date of Search Range Period</param>
        /// <param name="createDateTo">End Date of Search Range Period</param>
        /// <returns></returns>
        public async Task<Bundle> GetPbsItems(string patientId, DateTime? createdDateFrom, DateTime? createDateTo)
        {
            return await _medicareInformationServices.GetPbsItems(patientId, createdDateFrom, createDateTo);
        }

        /// <summary>
        /// Returns a bundle of Medicare Benefits Scheme (MBS) Items if successful or an Operational Outcome.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">Start Date of Search Range Period</param>
        /// <param name="createdDateTo">End Date of Search Range Period</param>
        /// <returns></returns>
        public async Task<Bundle> GetMbsItems(string patientId, DateTime? createdDateFrom, DateTime? createdDateTo)
        {
            return await _medicareInformationServices.GetMbsItems(patientId, createdDateFrom, createdDateTo);
        }

        #endregion

        #region Consumer Document Service

        /// <summary>
        /// Retrieves medication items from an individual’s Personal Health Summary document. Use this operation to obtain existing medication items, and also 
        /// the Document ID of the Personal Health Summary document if medication items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'MedicationStatement' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetPersonalHealthSummaryMedications(string patientId)
        {
            return await _consumerDocumentServices.GetPersonalHealthSummaryMedications(patientId);
        }

        /// <summary>
        /// Creates one or more medication items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any medication items.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘MedicationStatement’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> CreatePersonalHealthSummaryMedications(Bundle medications)
        {
            return await _consumerDocumentServices.CreatePersonalHealthSummaryMedications(medications);
        }

        /// <summary>
        /// Adds a new medication item to existing medication items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing medication items.
        /// </summary>
        /// <param name="medication">The new 'MedicationStatement' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the newly created 'MedicationStatement' resource with the logical ID populated.</returns>
        public async Task<MedicationStatement> AddPersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            return await _consumerDocumentServices.AddPersonalHealthSummaryMedication(medication, docId);
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
        public async Task<Bundle> UpdatePersonalHealthSummaryMedications(Bundle medications, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryMedications(medications, docId);
        }

        /// <summary>
        /// Updates a single medication item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryMedications operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        public async Task<MedicationStatement> UpdatePersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryMedication(medication, docId);
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
        public async Task DeletePersonalHealthSummaryMedication(string patientId, string docId, string medicationId)
        {
            await _consumerDocumentServices.DeletePersonalHealthSummaryMedication(patientId, docId, medicationId);
        }

        /// <summary>
        /// Retrieves allergy items from an individual’s Personal Health Summary document. Use this operation to obtain existing allergy items, and also 
        /// the Document ID of the Personal Health Summary document if allergy items are available. 
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient. Additional validation is performed on the IHI corresponding to the logical identifier 
        /// of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Returns a bundle of 'AllergyIntolerance' items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetPersonalHealthSummaryAllergies(string patientId)
        {
            return await _consumerDocumentServices.GetPersonalHealthSummaryAllergies(patientId);
        }

        /// <summary>
        /// Creates one or more allergy items in the individual’s Personal Health Summary document. Use this operation if there isn't a Personal Health 
        /// Summary document for the individual, or if the existing Personal Health Summary document doesn't contain any allergy items.
        /// </summary>
        /// <param name="allergies">The bundle should contain ‘Bundle’ of type ‘AllergyIntolerance’ resources. The ‘Bundle.type’ in the Bundle resource 
        /// should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’.</param>
        /// <returns>Returns a bundle of ‘AllergyIntolerance’ items from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> CreatePersonalHealthSummaryAllergies(Bundle allergies)
        {
            return await _consumerDocumentServices.CreatePersonalHealthSummaryAllergies(allergies);
        }

        /// <summary>
        /// Adds a new allergy item to existing allergy items in the individual's Personal Health Summary document. This operation can only be used if there
        /// are already existing allergy items.
        /// </summary>
        /// <param name="allergy">The new 'AllergyIntolerance' resource to be added.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        public async Task<AllergyIntolerance> AddPersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            return await _consumerDocumentServices.AddPersonalHealthSummaryAllergy(allergy, docId);
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
        public async Task<Bundle> UpdatePersonalHealthSummaryAllergies(Bundle allergies, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryAllergies(allergies, docId);
        }

        /// <summary>
        /// Updates a single allergy item in the individual's Personal Health Summary document.
        /// </summary>
        /// <param name="allergy">The ‘AllergyIntolerance’ to be updated.</param>
        /// <param name="docId">The CDA Document ID of the individual's latest Personal Health Summary document (obtainable from the 
        /// GetPersonalHealthSummaryAllergies operation).</param>
        /// <returns>Returns the updated resource with the same logical ID as in the request. In case of error, an OperationOutcome resource with details 
        /// as applicable will be returned.</returns>
        public async Task<AllergyIntolerance> UpdatePersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryAllergy(allergy, docId);
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
        public async Task DeletePersonalHealthSummaryAllergy(string patientId, string docId, string allergyIntoleranceId)
        {
            await _consumerDocumentServices.DeletePersonalHealthSummaryAllergy(patientId, docId, allergyIntoleranceId);
        }

        #endregion
    }
}