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
using System.Linq;
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DigitalHealth.MhrFhirClient.Sample.Shared
{

    /// <summary>
    /// Shared functionality implementation for consuming in consumer/provider sample clients.
    /// </summary>
    public static class SampleBase
    {
        /// <summary>
        /// The base client that implements shared functionality.
        /// </summary>
        public static IMhrFhirBaseClient SampleClient;

        public static readonly ConsoleColor DefaultBgColor = Console.BackgroundColor;
        public static readonly ConsoleColor Default = Console.ForegroundColor;
        public static bool IsLoggingEnabled = false;

        public static void SetConsole(ConsoleMode mode = ConsoleMode.Normal)
        {
            switch (mode)
            {
                case ConsoleMode.Normal:
                    Console.BackgroundColor = DefaultBgColor;
                    Console.ForegroundColor = Default;
                    break;
                case ConsoleMode.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ConsoleMode.Notify:
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case ConsoleMode.Success:
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

            }
        }


        public static void Log(string message)
        {
            if (IsLoggingEnabled)
            {
                
                StartLog();
                SetConsole(ConsoleMode.Success);
                Console.WriteLine("\t" + message);
                SetConsole();
                EndLog();
            }
            
        }
        
        /// <summary>
        /// Starts the log.
        /// </summary>
        public static void StartLog()
        {
            if (IsLoggingEnabled)
            {
                SetConsole(ConsoleMode.Success);
                Console.WriteLine(@"----------------------Start Log----------------------------");
                SetConsole();
            }
        }
       
        /// <summary>
        /// Ends the log.
        /// </summary>
        public static void EndLog()
        {
            if (IsLoggingEnabled)
            {
                SetConsole(ConsoleMode.Success);
                Console.WriteLine(@"----------------------End Log----------------------------");
                SetConsole();
            }
        }


        /// <summary>
        /// Outputs the result.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        public static void OutputResult(dynamic bundle)
        {

            EndLog();

            SetConsole(ConsoleMode.Notify);
            Console.WriteLine(@"====================Start=Formatted=Result=========================");
            var parsed = FhirSerializer.SerializeResourceToJson(bundle);
            Console.WriteLine(parsed);
            Console.WriteLine(@"====================End=Formatted=Result=========================");
            SetConsole();
        }

        /// <summary>
        /// Outputs the result.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        public static void OutputResult(Bundle bundle)
        {

            EndLog();

            SetConsole(ConsoleMode.Notify);
            Console.WriteLine(@"====================Start=Formatted=Result=========================");
            var parsed = FhirSerializer.SerializeResourceToJson(bundle);
            Console.WriteLine(parsed);
            Console.WriteLine(@"====================End=Formatted=Result=========================");
            SetConsole();
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void LogException(Exception ex)
        {
            //Exception type returned will be MhrFhirException type or ArgumentException type
            var mhrException = ex as MhrFhirException;
            if (mhrException != null)
            {
                var operationalOutcome = mhrException.OperationOutcome;
                Console.WriteLine(@"Exception : {0}", operationalOutcome);
            }
            else if (ex is ArgumentException)
            {
                var argException = ex as ArgumentException;
                Console.WriteLine(@"Argument Exception : {0}", argException);
            }
            else
            {
                SetConsole(ConsoleMode.Error);
                Console.WriteLine(@"Operation failed : " + ex.Message);
                

                if (ex.InnerException != null)
                {
                    Console.WriteLine("\t" + ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine("\t\t" + ex.InnerException.InnerException.Message);
                    }
                }
                SetConsole();

            }
        }

        /// <summary>
        /// Gets the inner exceptions.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static Exception[] GetInnerExceptions(Exception ex)
        {
            List<Exception> exceptions = new List<Exception>
            {
                ex
            };

            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                exceptions.Add(currentEx);
            }

            // Reverse the order to the innermost is first
            exceptions.Reverse();

            return exceptions.ToArray();
        }

        #region Patient Details
       
        /// <summary>
        /// Sample Code get Patient Details from MHR
        /// </summary>
        public static Patient GetPatientDetails(string patientId)
        {
            Patient result = null;
            try
            {

                var response = SampleClient.GetPatientDetails(patientId).Result; //Replace patientId with actual value
                EndLog();

                SetConsole(ConsoleMode.Notify);
                Console.WriteLine("====================Start=Result=========================");
                var parsed = FhirSerializer.SerializeResourceToJson(response);
                Console.WriteLine(parsed);
                Console.WriteLine("====================End=Result=========================");
                SetConsole();
                result = response;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return result;
        }

        #endregion

        #region Clinical Documents
        /// <summary>
        /// Sample Code to get a list of Prescriptions for the specified start and end date
        /// </summary>
        public static void GetPrescriptions(string patientId)
        {
            try
            {
                var response = SampleClient.GetPrescriptions(patientId, DateTime.Now.AddMonths(-24), DateTime.Now).Result;
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Sample Code to get a list of Medications Dispensed without the Prescriptions
        /// </summary>
        public static void GetDispenses(string patientId)
        {
            try
            {
                var response = SampleClient.GetPrescriptions(patientId, DateTime.Now.AddMonths(-24), DateTime.Now).Result;
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Sample Code to get a list of Medications Dispensed along with the Prescriptions
        /// </summary>
        public static void GetDispensesWithPrescriptions(string patientId)
        {
            try
            {
                var response = SampleClient.GetPrescriptions(patientId, DateTime.Now.AddMonths(-24), DateTime.Now).Result;
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Sample Code to get a list of Allergies in Shared Health Summary
        /// </summary>
        public static void GetSharedHealthSummaryAllergies(string patientId)
        {
            try
            {
                var response = SampleClient.GetSharedHealthSummaryAllergies(patientId).Result;
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        #endregion

        #region Consumer Documents

        /// <summary>
        /// Sample Code to retrieve Medications in Personal Health Summary
        /// </summary>
        public static Bundle GetPersonalHealthSummaryMedications(string patientId)
        {
            Bundle response = null;
            try
            {
                
                response = SampleClient.GetPersonalHealthSummaryMedications(patientId).Result;

                if (response != null && response.Entry != null && response.Entry.Any())
                {
                    Log(response.Entry.Count + " entries returned");
                }
                else
                {
                    Log("0 entries returned");
                }

                OutputResult(response);


            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return response;
        }

        /// <summary>
        /// Sample Code to retrieve Allergies in Personal Health Summary
        /// </summary>
        public static Bundle GetPersonalHealthSummaryAllergies(string patientId)
        {
            Bundle response =null;
            try
            {
                
                response = SampleClient.GetPersonalHealthSummaryAllergies(patientId).Result;
                if (response != null && response.Entry != null && response.Entry.Any())
                {
                    Log(response.Entry.Count + " entries returned");
                }
                else
                {
                    Log("0 entries returned");
                }
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return response;
        }
        #endregion

        #region Generic Document
        /// <summary>
        /// Sample Code to retrieve a Generic Document of a Patient
        /// </summary>
        public static void GetDocument(string patientId, string docId)
        {
            try
            {
                //Replace patientId and documentId with actual values
                var response = SampleClient.GetDocument(patientId, docId).Result;
                
                EndLog();
                //Result is a resource of Binary type 
                SetConsole(ConsoleMode.Notify);
                Console.WriteLine("====================Start=Result=========================");
                Console.WriteLine(response.Content?.Length);
                Console.WriteLine("====================End=Result=========================");
                SetConsole();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
         

        /// <summary>
        /// Sample Code to search for documents of a Patient and/or other query parameters
        /// </summary>
        public static void SearchDocuments(string patientId)
        {
            try
            {
                //Advance Care Directive Custodian Record For sample demo only
                string classCode = "100.16696";
                var testClassCode = new ClassCode() { CodeSystem = "NCTIS Data Components", Code = classCode };

                //Replace patientId and documentId with actual values
                var response = SampleClient.SearchDocuments(patientId, new SearchQuery { ClassCode = new List<ClassCode> { testClassCode }, Status = Status.Current }).Result;

                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        #endregion

        #region Medicare Information
        /// <summary>
        /// Sample Code to get a list of Medicare Benefits Scheme (PBS) Items 
        /// </summary>
        public static void GetMbsItems(string patientId)
        {
            try
            {
                var response = SampleClient.GetMbsItems(patientId, DateTime.Now.AddMonths(-24), DateTime.Now).Result;

                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /// <summary>
        /// Sample Code to get a list of Pharmaceutical Benefits Scheme (PBS) Items 
        /// </summary>
        public static void GetPbsItems(string patientId)
        {
            try
            {
                var response = SampleClient.GetPbsItems(patientId, DateTime.Now.AddMonths(-24), DateTime.Now).Result;
                OutputResult(response);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }


        }

        #endregion
    }
}
