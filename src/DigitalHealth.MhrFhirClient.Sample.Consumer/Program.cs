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
using System.Configuration;
using System.Linq;
using System.Net;
using DigitalHealth.MhrFhirClient.Factory;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Sample.Shared;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Serilog;

namespace DigitalHealth.MhrFhirClient.Sample.Consumer
{
    class Program
    {
        private const string IhiSystem = "http://ns.electronichealth.net.au/id/hi/ihi/1.0";

        static void Main(string[] args)
        {

            if (args != null && args.Any())
            {
                if (args[0] == "-h")
                {
                    Console.WriteLine(@"*******************************************************");
                    Console.WriteLine(@"Following switches are available to use with application");
                    Console.WriteLine(@"\t\t Use --enable-log to enable verbose logging");
                    Console.WriteLine(@"*******************************************************");
                    return;
                }

                if (args[0] == "--enable-log")
                {
                    Console.WriteLine(@"Logging to console enabled");
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.ColoredConsole()
                        .MinimumLevel.Debug()
                        .CreateLogger();

                    SampleBase.IsLoggingEnabled = true;
                }
            }


            Version currentVersion = typeof(Program).Assembly.GetName().Version;
            bool run = true;
            string currentPatientIhi = null;
            var endpoint = ConfigurationManager.AppSettings["endpoint"];
            var token = ConfigurationManager.AppSettings["token"];
            var clientId = ConfigurationManager.AppSettings["clientId"];
            var appVersion = ConfigurationManager.AppSettings["appVersion"];

            if (string.IsNullOrEmpty(endpoint) || !Uri.IsWellFormedUriString(endpoint, UriKind.RelativeOrAbsolute))
            {
                Console.WriteLine(@"Invalid endpoint");
                Console.ReadLine();
                Environment.Exit(1);
            }

            var endpointUrl = new Uri(endpoint);

            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine(@"Invalid client id");
                Console.ReadLine();
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(appVersion))
            {
                Console.WriteLine(@"Invalid app version");
                Console.ReadLine();
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Please enter token");
                token = Console.ReadLine();

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is required");
                    Environment.Exit(1);

                }
            }

            Console.WriteLine(@"MHR FHIR Consumer client:");
            Console.WriteLine($"Version {currentVersion}");
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(@"Parameters set:");
            Console.WriteLine(@"	Endpoint : " + endpointUrl);
            Console.WriteLine(@"	Token : " + token);
            Console.WriteLine(@"	Client Id : " + clientId);
            Console.WriteLine(@"	App Version : " + appVersion);

            SampleBase.SampleClient = MhrFhirConsumerClientFactory.Create(endpointUrl, token, clientId, appVersion);

            //Ignore HTTPS invalid cert warn
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            while (run)
            {
                try
                {
                    DisplayMenu();
                    var choice = 0;
                    while (true)
                    {
                        var val = Console.ReadLine();

                        if (val == "quit" || val == "q" || val == "exit")
                            Environment.Exit(1);

                        if (int.TryParse(val, out choice))
                        {
                            if (choice >= 1 && choice <= 20)
                            {
                                break;
                            }

                            SampleBase.SetConsole(ConsoleMode.Error);
                            Console.WriteLine(@"Invalid options chosen");
                            SampleBase.SetConsole();
                        }
                    }

                    try
                    {
                        string patientId = null;

                        if (choice != 2)
                        {
                            patientId = ConfigurationManager.AppSettings["patientId"];

                            if (string.IsNullOrEmpty(patientId))
                            {
                                Console.WriteLine(@"Patient Id is required to continue. Please enter Patient Id in app config file.");
                                break;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(currentPatientIhi))
                            {
                                Console.WriteLine(@"Enter patient IHI");
                                currentPatientIhi = Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine(@"Enter patient IHI or we can use " + currentPatientIhi + @" from the last call to GetPatientDetails (Press enter if you want to use this one.)");
                                var newId = Console.ReadLine();

                                if (!string.IsNullOrEmpty(newId))
                                {
                                    currentPatientIhi = newId;
                                }

                            }

                            if (string.IsNullOrEmpty(currentPatientIhi))
                            {
                                Console.WriteLine(@"IHI is required to continue");

                            }
                        }

                        SampleBase.StartLog();

                        RouteChoice(choice, patientId, ref currentPatientIhi);

                    }
                    catch (Exception ex)
                    {
                        SampleBase.LogException(ex);
                    }

                }
                catch (Exception ex)
                {
                    SampleBase.LogException(ex);
                }
                Console.WriteLine(@"Press enter to continue and type q and press enter to quit");

                var isQuitting = Console.ReadLine();

                if (!string.IsNullOrEmpty(isQuitting) && isQuitting.StartsWith("q"))
                {
                    run = false;
                }
            }
        }

        private static void RouteChoice(int choice, string patientId, ref string currentPatientIhi)
        {
            string phsDocAllergyDocId = null;
            switch (choice)
            {
                case 1:
                    var patient = SampleBase.GetPatientDetails(patientId);

                    if (patient?.Identifier != null && patient.Identifier.Any())
                    {
                        foreach (var i in patient.Identifier)
                        {
                            if (!string.IsNullOrEmpty(i.System) && string.Equals(i.System, IhiSystem) && !string.IsNullOrEmpty(i.Value))
                            {
                                currentPatientIhi = i.Value;
                                break;
                            }
                        }
                    }

                    break;
                case 2:
                    GetRecordList();
                    break;
                case 3:
                    SampleBase.GetPrescriptions(patientId);
                    break;
                case 4:
                    SampleBase.GetDispenses(patientId);
                    break;
                case 5:
                    SampleBase.GetDispensesWithPrescriptions(patientId);
                    break;
                case 6:
                    SampleBase.GetSharedHealthSummaryAllergies(patientId);
                    break;
                case 7:
                    Console.WriteLine(@"Enter document id");
                    var docId = Console.ReadLine();
                    SampleBase.GetDocument(patientId, docId);
                    break;
                case 8:
                    SampleBase.SearchDocuments(patientId);
                    break;
                case 9:
                    SampleBase.GetMbsItems(patientId);
                    break;
                case 10:
                    SampleBase.GetPbsItems(patientId);
                    break;
                case 11:
                    SampleBase.GetPersonalHealthSummaryMedications(patientId);
                    break;
                case 12:
                    string phsDocIdCreate = null;
                    Bundle phsMedicationsCreate;


                    try
                    {
                        phsMedicationsCreate = SampleBase.SampleClient.GetPersonalHealthSummaryMedications(patientId).Result;
                        if (phsMedicationsCreate != null && phsMedicationsCreate.Entry != null &&
                            phsMedicationsCreate.Entry.Any())
                        {
                            phsDocIdCreate = phsMedicationsCreate.Id;
                        }

                        if (string.IsNullOrEmpty(phsDocIdCreate))
                        {
                            CreateOrAddPhsMedications(patientId, null);
                        }
                        else
                        {
                            CreateOrAddPhsMedications(patientId, phsDocIdCreate);
                        }

                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null && e.InnerException is MhrFhirException)
                        {
                            if (((MhrFhirException) e.InnerException).StatusCode == HttpStatusCode.NotFound)
                            {
                                CreateOrAddPhsMedications(patientId, null);
                            }
                        }
                    }


                   
                    break;
                case 13:
                    UpdatePhsMedication(patientId);
                    break;
                case 14:
                    DeletePhsMedications(patientId);
                    break;
                case 15:
                    SampleBase.GetPersonalHealthSummaryAllergies(patientId);
                    break;
                case 16:

                    Bundle existingPhsAllergiesCreate;

                    try
                    {
                        existingPhsAllergiesCreate = SampleBase.SampleClient.GetPersonalHealthSummaryAllergies(patientId).Result;

                        if (existingPhsAllergiesCreate != null && existingPhsAllergiesCreate.Entry != null &&
                            existingPhsAllergiesCreate.Entry.Any())
                        {
                            phsDocAllergyDocId = existingPhsAllergiesCreate.Id;
                        }

                        if (string.IsNullOrWhiteSpace(phsDocAllergyDocId))
                        {
                            CreateOrAddPhsDeletePhsAllergy(patientId, null);
                        }
                        else
                        {
                            CreateOrAddPhsDeletePhsAllergy(patientId, phsDocAllergyDocId);
                        }

                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null && e.InnerException is MhrFhirException)
                        {
                            if (((MhrFhirException)e.InnerException).StatusCode == HttpStatusCode.NotFound)
                            {
                                CreateOrAddPhsDeletePhsAllergy(patientId, null);
                            }
                        }
                    }

                
                   
                    break;
                case 17:
                    UpdatePhsAllergy(patientId);
                    break;
                case 18:
                    DeletePhsAllergy(patientId);
                    break;
                case 19:
                    UpdatePhsMedications(patientId);
                    break;
                default:
                    throw new ArgumentException("Invalid selection");
            }
        }

        private static void UpdatePhsAllergy(string patientId)
        {

            try
            {
                var existingAllergies = SampleBase.SampleClient.GetPersonalHealthSummaryAllergies(patientId).Result;

                var docId = string.Empty;
                if (existingAllergies != null && existingAllergies.Entry != null &&
                    existingAllergies.Entry.Any())
                {
                    docId = existingAllergies.Id;
                }
                else
                {
                    throw new ArgumentException("Alteast one allergy item has to be present to update");
                }

                if (string.IsNullOrWhiteSpace(docId) || existingAllergies.Entry == null || !existingAllergies.Entry.Any())
                {
                    throw new ArgumentException("Alteast one allergy item has to be present to update");
                }

                var allergy = (AllergyIntolerance)existingAllergies.Entry.First().Resource;

                allergy.Reaction = new List<AllergyIntolerance.ReactionComponent>()
                {
                    new AllergyIntolerance.ReactionComponent()
                    {
                        Manifestation = new List<CodeableConcept>()
                        {
                            new CodeableConcept()
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding()
                                    {
                                        Display = "Reaction event"
                                    }
                                },
                                Text = "----------UPDATED-----REACTION--EVENT--" + DateTime.Now.ToLongTimeString()
                            }
                        }
                    }
                };



                var result = ((IMhrFhirConsumerClient)SampleBase.SampleClient).UpdatePersonalHealthSummaryAllergy(allergy,docId).Result;
                SampleBase.OutputResult(result);
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }

          
        }




        private static void UpdatePhsMedication(string patientId)
        {

            try
            {
                var existingMedications = SampleBase.SampleClient.GetPersonalHealthSummaryMedications(patientId).Result;

                var docId = string.Empty;
                if (existingMedications != null && existingMedications.Entry != null &&
                    existingMedications.Entry.Any())
                {
                    docId = existingMedications.Id;
                }
                else
                {
                    throw new ArgumentException("Alteast one medication item has to be present to update");
                }

                if (string.IsNullOrWhiteSpace(docId) || existingMedications.Entry == null || !existingMedications.Entry.Any())
                {
                    throw new ArgumentException("Alteast one medication item has to be present to update");
                }

                var medication = (MedicationStatement)existingMedications.Entry.First().Resource;

                medication.Dosage = new List<MedicationStatement.DosageComponent>()
                {
                    new MedicationStatement.DosageComponent()
                    {
                        Text = "----UPDATE---" + DateTime.Now
                    }
                };



                var result = ((IMhrFhirConsumerClient)SampleBase.SampleClient).UpdatePersonalHealthSummaryMedication(medication, docId).Result;
                SampleBase.OutputResult(result);
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }


        }




        private static void UpdatePhsMedications(string patientId)
        {

            try
            {
                var existingMedications = SampleBase.SampleClient.GetPersonalHealthSummaryMedications(patientId).Result;

                string docId;
                if (existingMedications != null && existingMedications.Entry != null &&
                    existingMedications.Entry.Any())
                {
                    docId = existingMedications.Id;
                }
                else
                {
                    throw new ArgumentException("No document is present to perform udpate");
                }

                if (string.IsNullOrWhiteSpace(docId) || existingMedications.Entry == null || !existingMedications.Entry.Any())
                {
                    throw new ArgumentException("No document is present to perform udpate");
                }


                //Update sample using PUT verb
                var firstMedication = (MedicationStatement)existingMedications.Entry[0].Resource;
                firstMedication.Dosage = new List<MedicationStatement.DosageComponent>()
                {
                    new MedicationStatement.DosageComponent()
                    {
                        Text = "----UPDATE---" + DateTime.Now
                    }
                };
                existingMedications.Entry[0].Resource = firstMedication;
                existingMedications.Entry[0].Request = new Bundle.RequestComponent()
                {
                    Method = Bundle.HTTPVerb.PUT,
                    Url = "MedicationStatement"
                };

                ////Update sample using POST verb
                var secondEntry = GenerateDummyPhsMedication(patientId);
                var entry = new Bundle.EntryComponent
                {
                    Resource = secondEntry,
                    FullUrl = Guid.NewGuid().ToString(),
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.POST,
                        Url = "MedicationStatement"
                    }
                };


                existingMedications.Entry.Add(entry);

                var result = ((IMhrFhirConsumerClient)SampleBase.SampleClient).UpdatePersonalHealthSummaryMedications(existingMedications,docId).Result;
                SampleBase.OutputResult(result);
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }


        }



        private static void DisplayMenu()
        {
            Console.WriteLine(@" Menu: Select one of the following menu options by entering the number or enter q to quit");
            Console.WriteLine(@"_____________________________________________________");
            Console.WriteLine(@"    1  > Get Patient Details");
            Console.WriteLine(@"    2  > Get Records List");
            Console.WriteLine(@"    3  > Get Prescriptions");
            Console.WriteLine(@"    4  > Get Dispenses");
            Console.WriteLine(@"    5  > Get Dispenses With Prescriptions");
            Console.WriteLine(@"    6  > Get Shared Health Summary Allergies");
            Console.WriteLine(@"    ");
            Console.WriteLine(@"    7  > Get Document");
            Console.WriteLine(@"    8  > Search Documents");
            Console.WriteLine(@"    ");
            Console.WriteLine(@"    9  > Get Mbs Items");
            Console.WriteLine(@"    10 > Get Pbs Items");
            Console.WriteLine(@"    ");
            Console.WriteLine(@"    11 > Get Personal HealthSummary Medications");
            Console.WriteLine(@"    12 > Create / Add PHS Medication");
            Console.WriteLine(@"    13 > Update PHS Medication");
            Console.WriteLine(@"    14 > Delete PHS Medication");
            Console.WriteLine(@"    ");
            Console.WriteLine(@"    15 > Get Personal Health Summary Allergies");
            Console.WriteLine(@"    16 > Create / Add PHS Allergy ");
            Console.WriteLine(@"    17 > Update PHS Allergy");
            Console.WriteLine(@"    18 > Delete PHS Allergy");
            Console.WriteLine(@"    ");
            //Console.WriteLine(@"    19 > Sample Update PHS Medication using PUT + POST Verb");
            Console.WriteLine(@"_____________________________________________________");
        }

        /// <summary>
        /// Gets the record list.
        /// </summary>
        static void GetRecordList()
        {
            try
            {
                var response = ((IMhrFhirConsumerClient)SampleBase.SampleClient).GetRecordList().Result;
                SampleBase.OutputResult(response);
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }
        }



        /// <summary>
        /// Deletes the PHS medications.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        private static void DeletePhsMedications(string patientId)
        {
            var bundle = SampleBase.GetPersonalHealthSummaryMedications(patientId);
            if (bundle != null && bundle.Entry != null && bundle.Entry.Any())
            {
                var medStatement = bundle.Entry.FirstOrDefault(x => x.Resource is MedicationStatement);
                if (medStatement != null)
                {
                    var med = (MedicationStatement)medStatement.Resource;

                    var docId = bundle.Id;
                    var medicationId = med.Id;

                    SampleBase.Log("Entries before: " + bundle.Entry.Count);
                    Console.WriteLine($"Removing Document : {docId}, medication id: {medicationId}, patient id {patientId} ");
                    ((IMhrFhirConsumerClient)SampleBase.SampleClient).DeletePersonalHealthSummaryMedication(patientId, docId, medicationId);
                }
                else
                {
                    Console.WriteLine(@"No entries found!");
                }

            }
            else
            {
                Console.WriteLine(@"No entries found!");
            }
        }

        /// <summary>
        /// Creates the or add PHS medications.
        /// If a docId is supplied, it will call the AddMethod or else Create.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="docId">The document identifier.</param>
        static void CreateOrAddPhsMedications(string patientId, string docId)
        {
            try
            {

                var medicationStatement = GenerateDummyPhsMedication(patientId);

                if (!string.IsNullOrEmpty(docId))
                {
                    SampleBase.Log("Performing Add : Medications");
                    var response = ((IMhrFhirConsumerClient)SampleBase.SampleClient).AddPersonalHealthSummaryMedication(medicationStatement, docId).Result;
                    SampleBase.OutputResult(response);
                }
                else
                {
                    var entry = new Bundle.EntryComponent
                    {
                        Resource = medicationStatement,
                        FullUrl = Guid.NewGuid().ToString(),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = "MedicationStatement"
                        }
                    };
                    var medications = new Bundle
                    {
                        Type = Bundle.BundleType.Transaction,
                        Entry = new List<Bundle.EntryComponent>() { entry }
                    };
                    SampleBase.Log("Performing Create : Medications");
                    var response = ((IMhrFhirConsumerClient)SampleBase.SampleClient).CreatePersonalHealthSummaryMedications(medications).Result;
                    SampleBase.OutputResult(response);
                }

            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }
        }

        /// <summary>
        /// Generates a dummy PHS medication.
        /// </summary>
        /// <param name="patientId">The test patient identifier.</param>
        /// <returns></returns>
        static MedicationStatement GenerateDummyPhsMedication(string patientId)
        {
            var m = new MedicationStatement()
            {
                Dosage = new List<MedicationStatement.DosageComponent>()
                 {
                     new MedicationStatement.DosageComponent()
                     {
                         Text = "Sample Dosage"
                     }
                 },
                Status = MedicationStatement.MedicationStatementStatus.Active,
                Patient = new ResourceReference()
                {
                    Reference = "Patient/" + patientId,
                },
                InformationSource = new ResourceReference()
                {
                    Reference = "Patient/" + patientId,
                },
                DateAsserted = DateTime.Today.AddDays(-1).ToString("O"),
                Note = new List<Annotation>() {new Annotation(){
                    Text = "Note 1"
                } },
                Medication = new CodeableConcept() { Text = "Sample text" }
            };
            return m;
        }








        /// <summary>
        /// Deletes the PHS medications.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        private static void DeletePhsAllergy(string patientId)
        {
            var bundle = SampleBase.GetPersonalHealthSummaryAllergies(patientId);
            
            if (bundle != null && bundle.Entry != null && bundle.Entry.Any())
            {
                var medStatement = bundle.Entry.FirstOrDefault(x => x.Resource is AllergyIntolerance);
                if (medStatement != null)
                {
                    SampleBase.Log("Entries Before : " + bundle.Entry.Count);

                    var allergyIntolerance = (AllergyIntolerance)medStatement.Resource;

                    var docId = bundle.Id;
                    var medicationId = allergyIntolerance.Id;

                    Console.WriteLine($"Removing:  Document : {docId}, medication id: {medicationId}, patient id {patientId} ");
                    ((IMhrFhirConsumerClient)SampleBase.SampleClient).DeletePersonalHealthSummaryAllergy(patientId, docId, medicationId);
                }
                else
                {
                    Console.WriteLine(@"No entries found!");
                }

            }
            else
            {
                Console.WriteLine(@"No entries found!");
            }
        }

        /// <summary>
        /// Creates the or add PHS allergies.
        /// If a docId is supplied, it will call the AddMethod or else Create.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="docId">The document identifier.</param>
        static void CreateOrAddPhsDeletePhsAllergy(string patientId, string docId)
        {
            try
            {

                var allergyDummyObject = GenerateDummyDeletePhsAllergy(patientId);

                if (!string.IsNullOrEmpty(docId))
                {
                    SampleBase.Log("Performing Add : Allergy");
                    var response = ((IMhrFhirConsumerClient)SampleBase.SampleClient).AddPersonalHealthSummaryAllergy(allergyDummyObject, docId).Result;
                    SampleBase.OutputResult(response);
                }
                else
                {
                    var entry = new Bundle.EntryComponent
                    {
                        Resource = allergyDummyObject,
                        FullUrl = Guid.NewGuid().ToString(),
                        Request = new Bundle.RequestComponent
                        {
                            Method = Bundle.HTTPVerb.POST,
                            Url = "AllergyIntolerance"
                        }
                    };
                    var allergy = new Bundle
                    {
                        Type = Bundle.BundleType.Transaction,
                        Entry = new List<Bundle.EntryComponent>() { entry }
                    };
                    SampleBase.Log("Performing Create : Allergy");
                    var response = ((IMhrFhirConsumerClient)SampleBase.SampleClient).CreatePersonalHealthSummaryAllergies(allergy).Result;
                    SampleBase.OutputResult(response);
                }

            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }
        }

        /// <summary>
        /// Generates a dummy PHS allergy.
        /// </summary>
        /// <param name="patientId">The test patient identifier.</param>
        /// <returns></returns>
        static AllergyIntolerance GenerateDummyDeletePhsAllergy(string patientId)
        {
            var m = new AllergyIntolerance()
            {
                Patient = new ResourceReference()
                {
                    Reference = "Patient/" + patientId,
                },
                Recorder = new ResourceReference()
                {
                    Reference = "Patient/" + patientId,
                },
                Substance =  new CodeableConcept()
                {
                    Coding = new List<Coding>()
                    {
                        new Coding()
                        {
                            Display = "Adverse Reaction"
                        }
                    },
                    Text = "Substance 1"
                },
                Reaction =  new List<AllergyIntolerance.ReactionComponent>()
                {
                    new AllergyIntolerance.ReactionComponent()
                    {
                        Manifestation = new List<CodeableConcept>()
                        {
                            new CodeableConcept()
                            {
                                Coding =  new List<Coding>()
                                {
                                    new Coding()
                                    {
                                        Display = "Reaction event"
                                    }
                                },
                                Text = "Some text"
                            }
                        }
                    }
                }
              
            };
            
        
        

            return m;
        }
    }


}
