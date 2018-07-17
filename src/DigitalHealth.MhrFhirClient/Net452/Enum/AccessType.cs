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

namespace DigitalHealth.MhrFhirClient.Enum
{
    /// <summary>
    /// Available Access types 
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// general access
        /// </summary>
        GeneralAccess,
        /// <summary>
        /// access code
        /// </summary>
        AccessCode,
        /// <summary>
        /// emergency access
        /// </summary>
        EmergencyAccess
    }
}
