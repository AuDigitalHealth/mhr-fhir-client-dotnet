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
using RestRequest = DigitalHealth.MhrFhirClient.Rest.RestRequest;


namespace DigitalHealth.MhrFhirClient.Services
{
    /// <summary>
    /// There are two APIs classified under this group: 
    ///  •	PBS Items(GET)
    ///  •	MBS Items(GET)
    /// </summary>
    internal class MedicareInformationServices
    {
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IMhrFhirRestClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicareInformationServices"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        internal MedicareInformationServices(IMhrFhirRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Gets the PBS items.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createdDateTo">The created date to.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">GetPbsItems - PatientId must be provided</exception>
        internal async Task<Bundle> GetPbsItems(string patientId, DateTime? createdDateFrom, DateTime? createdDateTo)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            return await GetMedicareInfo(patientId, createdDateFrom, createdDateTo, FhirConstants.PbsValue);
        }

        /// <summary>
        /// Gets the MBS items.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createdDateTo">The created date to.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">GetMbsItems - PatientId must be provided</exception>
        internal async Task<Bundle> GetMbsItems(string patientId, DateTime? createdDateFrom, DateTime? createdDateTo)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            return await GetMedicareInfo(patientId, createdDateFrom, createdDateTo, FhirConstants.MbsValue);
        }


        /// <summary>
        /// Adds the request headers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createdDateTo">The created date to.</param>
        /// <param name="coverageType">Type of the coverage.</param>
        private void AddRequestHeaders(RestRequest request , string patientId, DateTime? createdDateFrom, DateTime? createdDateTo, string coverageType)
        {
            request.AddQueryParameter(FhirConstants.PatientReferenceParameter, patientId);
            request.AddQueryParameter(FhirConstants.CoveragePlanParameter, coverageType);
            if (createdDateFrom != null)
            {                                
                request.AddQueryParameter(FhirConstants.CreatedParameter, $"{FhirConstants.GreaterThanOrEqualToPrefix}{createdDateFrom.Value:yyyy-MM-dd}");
            }

            if (createdDateTo != null)
            {
                request.AddQueryParameter(FhirConstants.CreatedParameter, $"{FhirConstants.LessThanOrEqualToPrefix}{createdDateTo.Value:yyyy-MM-dd}");
            }
        }

        /// <summary>
        /// Gets the medicare information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="createdDateFrom">The created date from.</param>
        /// <param name="createdDateTo">The created date to.</param>
        /// <param name="coverageType">Type of the coverage.</param>
        /// <returns></returns>
        private async Task<Bundle> GetMedicareInfo(string patientId, DateTime? createdDateFrom, DateTime? createdDateTo , string coverageType)
        {
            RestRequest request = _restClient.CreateMhrFhirRequest(typeof(ExplanationOfBenefit).Name, HttpMethod.Get);
            AddRequestHeaders(request, patientId, createdDateFrom, createdDateTo, coverageType);

            return await _restClient.ExecuteRequest<Bundle>(request);
        }
    }
}
