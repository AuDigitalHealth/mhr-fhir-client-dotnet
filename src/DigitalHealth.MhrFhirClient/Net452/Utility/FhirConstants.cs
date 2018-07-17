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

namespace DigitalHealth.MhrFhirClient.Utility
{
    /// <summary>
    /// 
    /// </summary>
    internal static class FhirConstants
    {
        /// <summary>
        /// The date written parameter
        /// </summary>
        public const string DateWrittenParameter = "datewritten";
        /// <summary>
        /// The when handed over parameter
        /// </summary>
        public const string WhenHandedOverParameter = "whenhandedover";
        /// <summary>
        /// The patient parameter
        /// </summary>
        public const string PatientParameter = "patient";

        /// <summary>
        /// The identifier parameter
        /// </summary>
        public const string IdentifierParameter = "identifier";
        /// <summary>
        /// The class parameter
        /// </summary>
        public const string ClassParameter = "class";
        /// <summary>
        /// The type parameter
        /// </summary>
        public const string TypeParameter = "type";
        /// <summary>
        /// The created parameter
        /// </summary>
        public const string CreatedParameter = "created";
        /// <summary>
        /// The author parameter
        /// </summary>
        public const string AuthorParameter = "author";
        /// <summary>
        /// The status parameter
        /// </summary>
        public const string StatusParameter = "status";
        /// <summary>
        /// The slot name parameter
        /// </summary>
        public const string SlotNameParameter = "slotName";
        /// <summary>
        /// The slot value parameter
        /// </summary>
        public const string SlotValueParameter = "slotValue";
        /// <summary>
        /// The patient reference parameter
        /// </summary>
        public const string PatientReferenceParameter = "patientreference";
        /// <summary>
        /// The coverage plan parameter
        /// </summary>
        public const string CoveragePlanParameter = "coverage.plan";

        /// <summary>
        /// The include parameter
        /// </summary>        
        public const string IncludeParameter = "_include";
        /// <summary>
        /// The doc ID parameter
        /// </summary>
        public const string DocIdParameter = "docId";
        /// <summary>
        /// The reporter type parameter
        /// </summary>
        public const string ReportTypeParameter = "reporter._type";
        /// <summary>
        /// The source type parameter
        /// </summary>
        public const string SourceTypeParameter = "source._type";
        /// <summary>
        /// The include parameter value
        /// </summary>
        public const string IncludeParameterValue = "MedicationDispense:authorizingPrescription";
        /// <summary>
        /// The report type practitioner value
        /// </summary>
        public const string PractitionerValue = "Practitioner";
        /// <summary>
        /// The source type patient value
        /// </summary>
        public const string PatientValue = "Patient";

        public const string MbsValue = "MBS";
        public const string PbsValue = "PBS";

        public const string IdentifierValue = "identifier";

        /// <summary>
        /// The greater than or equal to prefix
        /// </summary>
        public const string GreaterThanOrEqualToPrefix = "ge";
        /// <summary>
        /// The less than or equal to prefix
        /// </summary>
        public const string LessThanOrEqualToPrefix = "le";

        public const string DateFormatSpecifier = "yyyy-MM-dd";

        public const string SubjectParameter = "subject";

        public const string CoverageIdParameter = "coverageId";
        public const string BirthdateParameter = "birthdate";
        public const string GenderParameter = "gender";
        public const string FamilyParameter = "family";
        public const string GivenParameter = "given";
        public const string ElementsParameter = "_elements";

        public const string AccessCodeParameter = "accessCode";
        public const string AccessTypeParameter = "accessType";

        public const string AccessOperation = "$access";

        public const string FhirJsonMediaType = "application/json+fhir";
    }
}