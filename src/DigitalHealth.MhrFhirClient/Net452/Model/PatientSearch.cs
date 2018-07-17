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
using DigitalHealth.MhrFhirClient.Enum;

namespace DigitalHealth.MhrFhirClient.Model
{
    /// <summary>
    /// Patient search query object.
    /// </summary>
    public class PatientSearch 
    {
        /// <summary>
        /// Identifier of the patient Ihi / Medicare Card Number / DVA File Number
        /// </summary>
        /// <value>
        /// identifier.
        /// </value>
        public Identifier Identifier { get; set; }

        /// <summary>
        /// Date of Birth of the patient.
        /// </summary>
        /// <value>
        /// birthdate.
        /// </value>
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Gender value as token.
        /// </summary>
        /// <value>
        /// gender.
        /// </value>
        public Gender? Gender { get; set; }

        /// <summary>
        /// FamilyName Name of the patient.
        /// </summary>
        /// <value>
        /// name of the family.
        /// </value>
        public string FamilyName { get; set; }

        /// <summary>
        /// GivenName Name of the patient.
        /// </summary>
        /// <value>
        /// name of the given.
        /// </value>
        public string GivenName { get; set; }


        /// <summary>
        /// Provide an IHI for a patient Search
        /// </summary>
        /// <param name="ihi">The Patients Ihi number</param>
        /// <exception cref="System.ArgumentException">PatientSearch - An IHI is required for a Patient IHI search</exception>
        public PatientSearch(string ihi)
        {
            if (string.IsNullOrWhiteSpace(ihi))
            {
                throw new ArgumentException("PatientSearch - An IHI is required for a Patient IHI search");
            }

            Identifier = new Identifier(ihi, IdentifierType.Ihi);
        }

        /// <summary>
        /// Alternative Search Criteria
        /// </summary>
        /// <param name="identifier">The values can be any of the following Medicare Card Number DVA File Number / Military Health Number</param>
        /// <param name="birthdate">Date of Birth of the patient</param>
        /// <param name="gender">Gender value as token</param>
        /// <param name="familyName">FamilyName Name of the patient</param>
        /// <param name="givenName">GivenName Name of the patient (Optional)</param>
        /// <exception cref="System.ArgumentException">
        /// PatientSearch - Identifier Alternative Search Criteria
        /// or
        /// PatientSearch - Ihi must not be provided for Alternative Search Criteria
        /// or
        /// PatientSearch - FamilyName is required type for Alternative Search Criteria
        /// </exception>
        public PatientSearch(Identifier identifier, DateTime birthdate, Gender gender, string familyName, string givenName = null)
        {
            if (identifier == null)
            {
                throw new ArgumentException("PatientSearch - Identifier Alternative Search Criteria");
            }

            if (identifier.IdentifierType == IdentifierType.Ihi)
            {
                throw new ArgumentException("PatientSearch - Ihi must not be provided for Alternative Search Criteria");
            }

            if (string.IsNullOrWhiteSpace(familyName))
            {
                throw new ArgumentException("PatientSearch - FamilyName is required type for Alternative Search Criteria");
            }

            Identifier = identifier;
            Birthdate = birthdate;
            Gender = gender;
            FamilyName = familyName;
            GivenName = givenName;
        }

    }
}
