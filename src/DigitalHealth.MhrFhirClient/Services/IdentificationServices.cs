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
using System.Text;
using System.Threading.Tasks;
using DigitalHealth.MhrFhirClient.Client;
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Extension;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model;
using DigitalHealth.MhrFhirClient.Utility;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DigitalHealth.MhrFhirClient.Services
{
    /// <summary>
    /// There are two APIs classified under this group:
    ///  •	Record List(GET)
    ///  •	Patient Details(GET)
    /// </summary>
    internal class IdentificationServices 
    {
        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IMhrFhirRestClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationServices"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        internal IdentificationServices(IMhrFhirRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// The Provider can retrieve (access control logic applied) individual’s demographic details as available in the My Health Record system.
        /// </summary>
        /// <param name="patientId">The Patient Id</param>
        /// <returns>
        /// A patient resource
        /// </returns>
        /// <exception cref="System.ArgumentException">GetPatientDetails - PatientId must be provided</exception>
        internal async Task<Patient> GetPatientDetails(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.EmptyOrNullError, nameof(patientId)), nameof(patientId));
            }

            var request = _restClient.CreateMhrFhirRequest($"{typeof(Patient).Name}/{patientId}", HttpMethod.Get);

            return await _restClient.ExecuteRequest<Patient>(request);
        }

        /// <summary>
        /// Return Access to the current Patient’s record to view the associated details
        /// </summary>
        /// <returns>
        /// A Bundle
        /// </returns>
        public async Task<Bundle> GetPatientDetails()
        {
            var request = _restClient.CreateMhrFhirRequest(typeof(Patient).Name, HttpMethod.Get);

            return await _restClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// This API provides the ability to retrieve the list of records the individual is permitted to access and returns a bundle containing
        /// the RelatedPerson resource for each accessible record.
        /// </summary>
        /// <returns>
        /// A Bundle
        /// </returns>
        public async Task<Bundle> GetRecordList()
        {
            var request = _restClient.CreateMhrFhirRequest(typeof(RelatedPerson).Name, HttpMethod.Get);

            return await _restClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Provider can verify if a particular patient exists in the My Health Record system without gaining access to the record.
        /// </summary>
        /// <param name="patientSearch">Patient Search parameters</param>
        /// <returns>
        /// A Bundle containing the result
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// VerifyPatientExists - PatientSearch needs to be provided
        /// or
        /// VerifyPatientExists - Please provide an identifier for searching the patient
        /// or
        /// VerifyPatientExists - If the IHI number is available then other demographic search criteria is not accepted
        /// or
        /// VerifyPatientExists - If the IHI number is not available for the patient, you must provide an alternative identifier along with birthdate, gender & family
        /// </exception>
        internal async Task<Bundle> VerifyPatientExists(PatientSearch patientSearch)
        {
            // Validation
            if (patientSearch == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(patientSearch)), nameof(patientSearch));
            }

            if (patientSearch.Identifier == null)
            {
                throw new ArgumentException(string.Format(MhrFhirClientResource.NullError, nameof(patientSearch.Identifier)), nameof(patientSearch.Identifier));
            }

            if (patientSearch.Identifier.IdentifierType == IdentifierType.Ihi && (patientSearch.Birthdate != null || 
                patientSearch.Gender != null || !string.IsNullOrWhiteSpace(patientSearch.FamilyName) || 
                !string.IsNullOrWhiteSpace(patientSearch.GivenName)))
            {
                throw new ArgumentException(MhrFhirClientResource.IhiWithDemographicError);
            }

            if (patientSearch.Identifier.IdentifierType != IdentifierType.Ihi && (patientSearch.Birthdate == null || 
                patientSearch.Gender == null || string.IsNullOrWhiteSpace(patientSearch.FamilyName)))
            {
                throw new ArgumentException(MhrFhirClientResource.NoIhiError);
            }
            
            // Build request
            var request = _restClient.CreateMhrFhirRequest(typeof(Patient).Name, HttpMethod.Get);

            if (patientSearch.Identifier.IdentifierType == IdentifierType.Ihi)
            {
                request.AddQueryParameter(FhirConstants.IdentifierParameter, patientSearch.Identifier.ToString());
            }
            else if (patientSearch.Identifier.IdentifierType != IdentifierType.Ihi)
            {
                request.AddQueryParameter(FhirConstants.CoverageIdParameter, patientSearch.Identifier.ToString());
            }

            if (patientSearch.Birthdate.HasValue)
            {
                request.AddQueryParameter(FhirConstants.BirthdateParameter, patientSearch.Birthdate.Value.MrhClientDateToString());
            }

            if (patientSearch.Gender.HasValue)
            {
                request.AddQueryParameter(FhirConstants.GenderParameter, patientSearch.Gender.Value.Description());
            }

            if (!string.IsNullOrWhiteSpace(patientSearch.FamilyName))
            {
                request.AddQueryParameter(FhirConstants.FamilyParameter, patientSearch.FamilyName);
            }

            if (!string.IsNullOrWhiteSpace(patientSearch.GivenName))
            {
                request.AddQueryParameter(FhirConstants.GivenParameter, patientSearch.GivenName);
            }

            request.AddQueryParameter(FhirConstants.ElementsParameter, FhirConstants.IdentifierValue);

            // Execute Request
            return await _restClient.ExecuteRequest<Bundle>(request);
        }

        /// <summary>
        /// Provider can gain access to a particular patient’s record to view the associated details.
        /// </summary>
        /// <param name="patientId">The Patient Id</param>
        /// <param name="patientSearch">Patient Search Criteria for alternative search</param>
        /// <param name="accessType">The Access Type</param>
        /// <param name="accessCode">The Access Code to be provided if the accessType == AccessCode</param>
        /// <returns>
        /// A Bundle containing the result
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// GainAccessToPatientRecord - Either patientId or patientSearch is mandatory for gaining access to a patient
        /// or
        /// GainAccessToPatientRecord - This field is conditional mandatory. If the value of 'accessType' parameter is 'AccessCode', then a value of 'accessCode' parameter has to be sent as well.
        /// or
        /// GainAccessToPatientRecord - If the IHI number is available then other demographic search criteria is not accepted
        /// or
        /// GainAccessToPatientRecord - If the IHI number is not available for the patient, you must provide an alternative identifier along with birthdate, gender & family
        /// </exception>
        internal async Task<Parameters> GainAccessToPatientRecord(string patientId, PatientSearch patientSearch, AccessType accessType, string accessCode = null)
        {
            Parameters param = new Parameters();

            // Validation
            if (string.IsNullOrWhiteSpace(patientId) && patientSearch?.Identifier == null)
            {
                throw new ArgumentException(MhrFhirClientResource.PatientIdOrPatientSearchRequiredError);
            }

            // NOTE for general access on the MHR the access code must be an empty string
            if (accessType == AccessType.AccessCode && accessCode == null)
            {
                throw new ArgumentException(MhrFhirClientResource.AccessTypeAndAccessCodeError);
            }

            if (patientSearch?.Identifier != null)
            {
                if (patientSearch.Identifier.IdentifierType == IdentifierType.Ihi && (patientSearch.Birthdate != null || 
                    patientSearch.Gender != null || !string.IsNullOrWhiteSpace(patientSearch.FamilyName) || 
                    !string.IsNullOrWhiteSpace(patientSearch.GivenName)))
                {
                    throw new ArgumentException(MhrFhirClientResource.IhiWithDemographicError);
                }

                if (patientSearch.Identifier.IdentifierType != IdentifierType.Ihi && (patientSearch.Birthdate == null || 
                    patientSearch.Gender == null || string.IsNullOrWhiteSpace(patientSearch.FamilyName)))
                {
                    throw new ArgumentException(MhrFhirClientResource.NoIhiError);
                }
            }

            // Build the endpoint based on if the patient ID is specified
            StringBuilder endpointBuilder = new StringBuilder($"{typeof(Patient).Name}/");
            if (!string.IsNullOrWhiteSpace(patientId))
            {
                endpointBuilder.Append($"{patientId}/");
            }
            endpointBuilder.Append(FhirConstants.AccessOperation);

            // Build request
            var request = _restClient.CreateMhrFhirRequest(endpointBuilder.ToString(), HttpMethod.Post);

            if (patientSearch?.Identifier != null)
            {
                if (patientSearch.Identifier.IdentifierType == IdentifierType.Ihi)
                {
                    param.Parameter.Add(new Parameters.ParameterComponent
                    {
                        Name = FhirConstants.SubjectParameter,
                        Resource = MhrFhirClientHelper.CreatePatientIhi(patientSearch.Identifier.Value)
                    });
                }
                else if (patientSearch.Identifier.IdentifierType != IdentifierType.Ihi)
                {                    
                    param.Parameter.Add(new Parameters.ParameterComponent
                    {
                        Name = FhirConstants.CoverageIdParameter,
                        Value = new FhirString(patientSearch.Identifier.IdentifierType.ToString())
                    });
                }
            }

            param.Parameter.Add(new Parameters.ParameterComponent
            {
                Name = FhirConstants.AccessTypeParameter,
                Value = new Code(accessType.ToString())
            });

            if (accessCode != null)
            {
                param.Parameter.Add(new Parameters.ParameterComponent
                {
                    Name = FhirConstants.AccessCodeParameter,
                    Value = new Code(accessCode)
                });
            }

            if (patientSearch != null)
            {
                if (patientSearch.Birthdate.HasValue || patientSearch.Gender.HasValue || !string.IsNullOrWhiteSpace(patientSearch.FamilyName) || !string.IsNullOrWhiteSpace(patientSearch.GivenName))
                {
                    param.Parameter.Add(new Parameters.ParameterComponent
                    {
                        Name = FhirConstants.SubjectParameter,
                        Resource = MhrFhirClientHelper.CreatePatientSearchDemographicDetails(patientSearch.Birthdate, patientSearch.Gender, patientSearch.FamilyName, patientSearch.GivenName)
                    });
                }
            }

            string json = FhirSerializer.SerializeResourceToJson(param);

            request.SetJsonBodyParameters(json, FhirConstants.FhirJsonMediaType);

            // Execute Request
            return await _restClient.ExecuteRequest<Parameters>(request);
        }
    }
}
