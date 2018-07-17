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
using System.Collections.Generic;
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Extension;
using Hl7.Fhir.Model;

namespace DigitalHealth.MhrFhirClient.Utility
{
    /// <summary>
    /// Helper class providing basic functions in Consumer and Provider client implementations.
    /// </summary>
    internal class MhrFhirClientHelper
    {
        /// <summary>
        /// This returns a patient resource containing a patient and IHI
        /// </summary>
        /// <param name="ihi">ihi</param>
        /// <returns>
        /// A Patient Containing an Identifier IHI
        /// </returns>
        public static Patient CreatePatientIhi(string ihi)
        {
            return new Patient
                {
                    Identifier = new List<Identifier>
                    {
                        new Identifier("http://ns.electronichealth.net.au/id/hi/ihi/1.0", ihi)
                        {
                            Type = new CodeableConcept
                            {
                                Coding = new List<Coding>
                                {
                                    new Coding("http://hl7.org/fhir/v2/0203", "NI")
                                    {
                                        Display = "National unique individual identifier"
                                    }
                                },
                                Text = "IHI"
                            }
                        }
                    }
            };
        }

        /// <summary>
        /// Create Search Demographic Details Patient Resource
        /// </summary>
        /// <param name="birthdate">The birth date</param>
        /// <param name="gender">The gender</param>
        /// <param name="familyName">The familyName</param>
        /// <param name="givenName">The givenName</param>
        /// <returns>
        /// A Patient Resource
        /// </returns>
        public static Patient CreatePatientSearchDemographicDetails(DateTime? birthdate, Gender? gender, string familyName, string givenName)
        {
            var patient = new Patient();

            var humanName = new HumanName();

            if (!string.IsNullOrWhiteSpace(familyName))
            {
                humanName.AndFamily(familyName);
            }

            if (!string.IsNullOrWhiteSpace(givenName))
            {
                humanName.WithGiven(givenName);
            }

            patient.Name = new List<HumanName>
            {
                humanName
            };

            if (gender != null)
            {
                patient.Gender = (AdministrativeGender)gender.Value;
            }

            if (birthdate != null)
            {
                patient.BirthDate = birthdate.Value.MrhClientDateToString();
            }

            return patient;
        }
    }
}
