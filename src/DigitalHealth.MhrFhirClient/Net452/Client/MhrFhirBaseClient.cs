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
        /// This API provides the ability to retrieve Medications from an individual’s personal health summary document from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns> Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> GetPersonalHealthSummaryMedications(string patientId)
        {
            return await _consumerDocumentServices.GetPersonalHealthSummaryMedications(patientId);
        }

        /// <summary>
        /// This API provides the ability to create an individual’s MedicationStatement in the Personal Health Summary document (type/class code = ‘100.16685’) stored in the My Health Record system. Use this API if there is no Personal Summary Document exits for the individual, or if the existing document doesn’t have any Medication section.
        /// </summary>
        /// <param name="medications">The bundle should contain ‘Bundle’ of type ‘MedicationStatement’ resources. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘POST’. This is a custom implementation of Bundle where the Bundle is posted on the MedicationStatement URI instead of using the base URI.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome..</returns>
        public async Task<Bundle> CreatePersonalHealthSummaryMedications(Bundle medications)
        {
            return await _consumerDocumentServices.CreatePersonalHealthSummaryMedications(medications);
        }

        /// <summary>
        /// Adds the personal health summary medication.
        /// </summary>
        /// <param name="medication">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<MedicationStatement> AddPersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            return await _consumerDocumentServices.AddPersonalHealthSummaryMedication(medication, docId);
        }

        /// <summary>
        /// Updates the personal health summary medications.
        /// </summary>
        /// <param name="medications">The request body should contain ‘Bundle’ of ‘MedicationStatement’ resources with the document ID in the URI. The ‘Bundle.type’ in the Bundle resource should be ‘transaction’ and the ‘Bundle.entry’ should contain ‘request’ element with ‘request.method’ value as ‘PUT’ or ‘POST’. This is a custom implementation of Bundle where the PUT request containing Bundle resource is submitted on the MedicationStatement URI instead of using the base URI.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<Bundle> UpdatePersonalHealthSummaryMedications(Bundle medications, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryMedications(medications, docId);
        }

        /// <summary>
        /// Updates the personal health summary medication.
        /// </summary>
        /// <param name="medication">The ‘MedicationStatement’.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>Returns a bundle of Medications from the Personal Health Summary if successful or an Operational Outcome.</returns>
        public async Task<MedicationStatement> UpdatePersonalHealthSummaryMedication(MedicationStatement medication, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryMedication(medication, docId);
        }

        /// <summary>
        /// Deletes the personal health summary medication.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <param name="medicationId">The medication identifier.</param>
        /// <returns></returns>
        public async Task DeletePersonalHealthSummaryMedication(string patientId, string docId, string medicationId)
        {
            await _consumerDocumentServices.DeletePersonalHealthSummaryMedication(patientId, docId, medicationId);
        }

        /// <summary>
        /// This API provides the ability to retrieve allergies and adverse reactions information from an individual’s Personal Health Summary document from the My Health Record system.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns>Bundle of allergies and adverse reactions</returns>
        public async Task<Bundle> GetPersonalHealthSummaryAllergies(string patientId)
        {
            return await _consumerDocumentServices.GetPersonalHealthSummaryAllergies(patientId);
        }

        /// <summary>
        /// This API provides the ability to create an individual’s allergies and adverse reactions information in the Personal Health Summary document stored in the My Health Record system.
        /// </summary>
        /// <param name="allergies">The request body should contain ‘Bundle’ of ‘AllergyIntolerance’ resources.</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        public async Task<Bundle> CreatePersonalHealthSummaryAllergies(Bundle allergies)
        {
            return await _consumerDocumentServices.CreatePersonalHealthSummaryAllergies(allergies);
        }

        /// <summary>
        /// Adds the personal health summary allergy.
        /// </summary>
        /// <param name="allergy">The request body should contain instance of allergy.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns>The system will return back the newly created resource with logical ID populated.</returns>
        public async Task<AllergyIntolerance> AddPersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            return await _consumerDocumentServices.AddPersonalHealthSummaryAllergy(allergy, docId);
        }

        /// <summary>
        /// Batch update of Allergy Intolerance items in the Personal Health Summary
        /// </summary>
        /// <param name="allergies">The allergies.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns></returns>
        public async Task<Bundle> UpdatePersonalHealthSummaryAllergies(Bundle allergies, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryAllergies(allergies, docId);
        }

        /// <summary>
        /// Update an Allergy Intolerance item in the Personal Health Summary.
        /// </summary>
        /// <param name="allergy">The allergy.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <returns></returns>
        public async Task<AllergyIntolerance> UpdatePersonalHealthSummaryAllergy(AllergyIntolerance allergy, string docId)
        {
            return await _consumerDocumentServices.UpdatePersonalHealthSummaryAllergy(allergy, docId);
        }

        /// <summary>
        /// Delete an Allergy Intolerance item from the Personal Health Summary.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="docId">Custom search parameter which should be used to provide the latest personal health summary CDA document ID retrieved from the document reference API.</param>
        /// <param name="allergyIntoleranceId">The allergy intolerance identifier.</param>
        /// <returns></returns>
        public async Task DeletePersonalHealthSummaryAllergy(string patientId, string docId, string allergyIntoleranceId)
        {
            await _consumerDocumentServices.DeletePersonalHealthSummaryAllergy(patientId, docId, allergyIntoleranceId);
        }

        #endregion
    }
}