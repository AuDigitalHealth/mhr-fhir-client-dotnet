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
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Model;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient.Interface
{
    /// <summary>
    /// Provider MHR FHIR client.
    /// </summary>
    /// <seealso cref="DigitalHealth.MhrFhirClient.Interface.IMhrFhirBaseClient" />
    public interface IMhrFhirProviderClient : IMhrFhirBaseClient
    {
        /// <summary>
        /// Verifies the patient exists.
        /// </summary>
        /// <param name="patientSearch">The patient search.</param>
        /// <returns></returns>
        Task<Bundle> VerifyPatientExists(PatientSearch patientSearch);
        /// <summary>
        /// Gains the access to patient record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patientSearch">The patient search.</param>
        /// <param name="accessType">Type of the access.</param>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        Task<Parameters> GainAccessToPatientRecord(string id, PatientSearch patientSearch, AccessType accessType, string accessCode = null);
    }
}