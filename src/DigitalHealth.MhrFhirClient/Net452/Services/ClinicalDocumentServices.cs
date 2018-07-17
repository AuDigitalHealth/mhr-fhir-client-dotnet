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
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Utility;
using Hl7.Fhir.Model;
using RestRequest = DigitalHealth.MhrFhirClient.Rest.RestRequest;

namespace DigitalHealth.MhrFhirClient.Services
{
    /// <summary>
    /// There are two APIs classified under this group: Prescription and Dispense
    /// List(GET) and  Allergies List – from Shared Health Summary document(GET) This API is
    /// accessible by both consumer and provider.
    /// </summary>
    internal class ClinicalDocumentServices
    {
        /// <summary>
        /// REST client.
        /// </summary>
        private readonly IMhrFhirRestClient _restClient;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        internal ClinicalDocumentServices(IMhrFhirRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Gets the prescriptions.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateWrittenFrom">From Date is a search criterion to select the MedicationOrder whose start date is after the specific period.</param>
        /// <param name="dateWrittenTo">To Date is a search criterion to select the MedicationOrder whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <returns>Returns a bundle of Medication Order if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">patientId</exception>
        internal async Task<Bundle> GetPrescriptions(string patientId, DateTime? dateWrittenFrom, DateTime? dateWrittenTo)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            RestRequest restRequest = CreateGetClinicalDocumentRequest(typeof(MedicationOrder).Name, patientId,
                FhirConstants.DateWrittenParameter, dateWrittenFrom, dateWrittenTo);

            return await _restClient.ExecuteRequest<Bundle>(restRequest);
        }

        /// <summary>
        /// Gets the dispenses.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateHandedOverFrom">From Date is a search criteria to select the documents whose start date is after the specific period.</param>
        /// <param name="dateHandedOverTo">To Date is a search criteria to select the documents whose start date is before the specific period.Any future date provided in the request for ‘le’ will be defaulted to server current date.</param>
        /// <param name="includeAuthorizingPrescription">if set to <c>true</c> [Include the prescription reference in the response].</param>
        /// <returns> Returns a bundle of Medication Dispense if successful or an Operational Outcome.</returns>
        /// <exception cref="System.ArgumentException">patientId</exception>
        internal async Task<Bundle> GetDispenses(string patientId, DateTime? dateHandedOverFrom, DateTime? dateHandedOverTo,
            bool includeAuthorizingPrescription)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            var restRequest = CreateGetClinicalDocumentRequest(typeof(MedicationDispense).Name, patientId, FhirConstants.WhenHandedOverParameter,
                dateHandedOverFrom, dateHandedOverTo);

            if (includeAuthorizingPrescription)
            {
                restRequest.AddQueryParameter(FhirConstants.IncludeParameter, FhirConstants.IncludeParameterValue);
            }

            return await _restClient.ExecuteRequest<Bundle>(restRequest);
        }

        /// <summary>
        /// Gets the shared health summary allergies.
        /// </summary>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">patientId</exception>
        internal async Task<Bundle> GetSharedHealthSummaryAllergies(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            var restRequest = CreateGetClinicalDocumentRequest(typeof(AllergyIntolerance).Name, patientId, null, null, null);

            restRequest.AddQueryParameter(FhirConstants.ReportTypeParameter, FhirConstants.PractitionerValue);

            return await _restClient.ExecuteRequest<Bundle>(restRequest);
        }

        /// <summary>
        /// Creates the get clinical document request.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="patientId">Logical identifier of the patient.Additional validation is performed on the IHI corresponding to the logical identifier of the patient in the URI to check if the logged in user is authorized to perform any operation to the IHI in context.</param>
        /// <param name="dateParam">The date parameter.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>Request instance </returns>
        private RestRequest CreateGetClinicalDocumentRequest(string resourceName, string patientId, string dateParam, DateTime? startDate, DateTime? endDate)
        {
            var request = _restClient.CreateMhrFhirRequest(resourceName, HttpMethod.Get);

            // Patient ID
            request.AddQueryParameter(FhirConstants.PatientParameter, patientId);

            // Add the start the date
            if (startDate != null)
            {
                request.AddQueryParameter(dateParam, $"{FhirConstants.GreaterThanOrEqualToPrefix}{startDate.Value:yyyy-MM-dd}");
            }

            // Add the end the date
            if (endDate != null)
            {
                request.AddQueryParameter(dateParam, $"{FhirConstants.LessThanOrEqualToPrefix}{endDate.Value:yyyy-MM-dd}");
            }

            return request;
        }

    }
}
