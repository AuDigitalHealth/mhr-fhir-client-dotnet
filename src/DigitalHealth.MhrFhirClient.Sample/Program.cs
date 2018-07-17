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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using DigitalHealth.MhrFhirClient.Enum;
using DigitalHealth.MhrFhirClient.Factory;
using DigitalHealth.MhrFhirClient.Interface;
using DigitalHealth.MhrFhirClient.Model;
using DigitalHealth.MhrFhirClient.Model.OAuth;
using DigitalHealth.MhrFhirClient.Sample.Shared;
using Hl7.Fhir.Serialization;
using Serilog;

namespace DigitalHealth.MhrFhirClient.Sample.Provider
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
                    Console.WriteLine("Logging to console enabled");

                    // Enable Serilog
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.ColoredConsole()
                        .MinimumLevel.Debug()
                        .CreateLogger();

                    SampleBase.IsLoggingEnabled = true;
                }
            }

            bool run = true;
            Version currentVersion = typeof(Program).Assembly.GetName().Version;
            var endpoint = ConfigurationManager.AppSettings["endpoint"];
            var token = ConfigurationManager.AppSettings["token"];
            var clientId = ConfigurationManager.AppSettings["clientId"];
            var appVersion = ConfigurationManager.AppSettings["appVersion"];
            var certPath = ConfigurationManager.AppSettings["certPath"];
            var certPass = ConfigurationManager.AppSettings["certPass"];
            var fullCertPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certPath);
            string ihi = null;

            if (string.IsNullOrEmpty(certPath) || !File.Exists(fullCertPath))
            {
                Console.WriteLine(@"Invalid cert specified.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(endpoint) || !Uri.IsWellFormedUriString(endpoint, UriKind.RelativeOrAbsolute))
            {
                Console.WriteLine(@"Invalid endpoint");
                Console.ReadLine();
                Environment.Exit(1);
            }

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
            var endpointUrl = new Uri(endpoint);

            Console.WriteLine(@"MHR FHIR Provider client:");
            Console.WriteLine($"Version {currentVersion}");
            Console.WriteLine(DateTime.Now);

            Console.WriteLine(@"Parameters set:");
            Console.WriteLine(@"	Endpoint : " + endpointUrl);
            Console.WriteLine(@"	Token : " + token);
            Console.WriteLine(@"	Client Id : " + clientId);
            Console.WriteLine(@"	App Version : " + appVersion);


            Console.WriteLine($"Using {fullCertPath} for certificate to use. Note: file should be placed in the root of this executable directory");


            var cert = new X509Certificate2(fullCertPath, certPass);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No Token found ? Retrieve token ? \"y\" or \"n\"  ");
                var result = Console.ReadKey();
                if (result.KeyChar == 'y' || result.KeyChar == 'Y')
                {
                    SampleBase.StartLog();
                    SampleBase.SetConsole(ConsoleMode.Success);
                    token = GetToken(cert, clientId);
                    SampleBase.SetConsole();
                    SampleBase.EndLog();
                }
                else
                {
                    Console.WriteLine(@"Please enter token");
                    token = Console.ReadLine();

                    if (string.IsNullOrEmpty(token))
                    {
                        Console.WriteLine(@"Token is required");
                        Environment.Exit(1);

                    }
                }
            }

            SampleBase.SampleClient = MhrFhirProviderClientFactory.Create(endpointUrl, token, clientId, appVersion, cert);

            //Ignore HTTPS
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            while (run)
            {
                try
                {
                    Console.WriteLine(@" Menu: Select one of the following menu options by entering the number or enter q to quit");
                    Console.WriteLine(@"_____________________________________________________");
                    Console.WriteLine(@"    1  > Get Patient Details");
                    Console.WriteLine(@"    2  > Verify Patient Exists");
                    Console.WriteLine(@"    3  > Gain Access To Patient Record");
                    Console.WriteLine(@"    4  > Get Prescriptions");
                    Console.WriteLine(@"    5  > Get Dispenses");
                    Console.WriteLine(@"    6  > Get Dispenses With Prescriptions");
                    Console.WriteLine(@"    7  > Get Shared Health Summary Allergies");
                    Console.WriteLine(@"    8  > Get Personal HealthSummary Medications");
                    Console.WriteLine(@"    9  > Get Personal Health Summary Allergies");
                    Console.WriteLine(@"    10 > Get Document");
                    Console.WriteLine(@"    11 > Search Documents");
                    Console.WriteLine(@"    12 > Get Mbs Items");
                    Console.WriteLine(@"    13 > Get Pbs Items");
                    Console.WriteLine(@"_____________________________________________________");
                    var choice = 0;
                    while (true)
                    {
                        var val = Console.ReadLine();

                        if (val == "quit" || val == "q" || val == "exit")
                            Environment.Exit(1);

                        if (int.TryParse(val, out choice))
                        {
                            if (choice >= 1 && choice <= 13)
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
                             patientId = System.Configuration.ConfigurationManager.AppSettings["patientId"];
                             
                            if (string.IsNullOrEmpty(patientId))
                            {
                                Console.WriteLine(@"Patient Id is required to continue");
                                break;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(ihi))
                            {
                                Console.WriteLine(@"Enter patient IHI");
                                patientId = Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine(@"Enter patient IHI or we can use " + ihi + @" from the last call to GetPatientDetails (Press enter if you want to use this one.)");
                                patientId = Console.ReadLine();

                                if (string.IsNullOrEmpty(patientId))
                                {
                                    patientId = ihi;
                                }
                            }

                            if (string.IsNullOrEmpty(patientId))
                            {
                                Console.WriteLine(@"IHI is required to continue");

                            }
                        }

                        SampleBase.StartLog();

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
                                            ihi = i.Value;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                VerifyPatientExists(patientId);
                                break;
                            case 3:
                                GainAccessToPatientRecord(patientId);
                                break;
                            case 4:
                                SampleBase.GetPrescriptions(patientId);
                                break;
                            case 5:
                                SampleBase.GetDispenses(patientId);
                                break;
                            case 6:
                                SampleBase.GetDispensesWithPrescriptions(patientId);
                                break;
                            case 7:
                                SampleBase.GetSharedHealthSummaryAllergies(patientId);
                                break;
                            case 8:
                                SampleBase.GetPersonalHealthSummaryMedications(patientId);
                                break;
                            case 9:
                                SampleBase.GetPersonalHealthSummaryAllergies(patientId);
                                break;
                            case 10:
                                Console.WriteLine(@"Enter document id");
                                var docId = Console.ReadLine();
                                SampleBase.GetDocument(patientId, docId);
                                break;
                            case 11:
                                SampleBase.SearchDocuments(patientId);
                                break;
                            case 12:
                                SampleBase.GetMbsItems(patientId);
                                break;
                            case 13:
                               SampleBase.GetPbsItems(patientId);
                                break;
                            default:
                                throw new ArgumentException("Invalid selection");
                        }

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
         

        static string GetToken(X509Certificate2 cert, string clientId)
        {
            string userIdentifier = ConfigurationManager.AppSettings["userIdentifier"];
            string userName = ConfigurationManager.AppSettings["userName"];
            string tokenEndPoint = ConfigurationManager.AppSettings["tokenEndpoint"];
            string providerClientSecret = ConfigurationManager.AppSettings["clientSecret"];
            string hpio = ConfigurationManager.AppSettings["hpio"];
            string organisationName = ConfigurationManager.AppSettings["organisationName"];
            string deviceModel = ConfigurationManager.AppSettings["deviceModel"];
            string deviceIdentifier = ConfigurationManager.AppSettings["deviceIdentifier"];
            string deviceMake = ConfigurationManager.AppSettings["deviceMake"];
            string callback = ConfigurationManager.AppSettings["redirectUrl"];


            if (string.IsNullOrEmpty(userIdentifier))
            {
                Console.WriteLine(@"Invalid " + nameof(userIdentifier));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine(@"Invalid " + nameof(userName));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(tokenEndPoint))
            {
                Console.WriteLine(@"Invalid " + nameof(tokenEndPoint));
                Environment.Exit(1);
            }


            if (string.IsNullOrEmpty(providerClientSecret))
            {
                Console.WriteLine(@"Invalid " + nameof(providerClientSecret));
                Environment.Exit(1);
            }


            if (string.IsNullOrEmpty(hpio))
            {
                Console.WriteLine(@"Invalid " + nameof(hpio));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(organisationName))
            {
                Console.WriteLine(@"Invalid " + nameof(organisationName));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(deviceModel))
            {
                Console.WriteLine(@"Invalid " + nameof(deviceModel));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(deviceIdentifier))
            {
                Console.WriteLine(@"Invalid " + nameof(deviceIdentifier));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(deviceMake))
            {
                Console.WriteLine(@"Invalid " + nameof(deviceMake));
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(callback))
            {
                Console.WriteLine(@"Invalid " + nameof(callback));
                Environment.Exit(1);
            }

            IProviderOAuthClient providerOAuthClient = ProviderOAuthClientFactory.Create(new ProviderOAuthModel
            {
                Certificate = cert,
                TokenProviderEndpointUrl = new Uri(tokenEndPoint),
                ClientIdentifier = clientId,
                ClientSecret = providerClientSecret,
                RedirectUrl = callback,
                Hpio = hpio,
                OrganisationName = organisationName,
                DeviceModel = deviceModel,
                DeviceIdentifier = deviceIdentifier,
                DeviceMake = deviceMake
            });


            return providerOAuthClient.GetProviderToken(userIdentifier, userName).Result.AccessToken;
        }
               
        /// <summary>
        ///  Sample Code to Verify if Patient exists
        /// </summary>
        static void VerifyPatientExists(string patientId)
        {
            try
            {
                var response =((IMhrFhirProviderClient)SampleBase.SampleClient).VerifyPatientExists(new PatientSearch(patientId)).Result; //Replace patientId with actual value 

                SampleBase.OutputResult(response);
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }
        }

        /// <summary>
        /// Sample Code to Gain Access to Patient Record
        /// </summary>
        static void GainAccessToPatientRecord(string patientid)
        {
            try
            {
                //Replace patientId with actual value
                var response = ((IMhrFhirProviderClient)SampleBase.SampleClient).GainAccessToPatientRecord(patientid, null, AccessType.GeneralAccess).Result;

                SampleBase.EndLog();
                SampleBase.SetConsole(ConsoleMode.Notify);
                Console.WriteLine("====================Start=Result=========================");
                var parsed = FhirSerializer.SerializeResourceToJson(response);
                Console.WriteLine(parsed);
                Console.WriteLine("====================End=Result=========================");
                SampleBase.SetConsole();
                
            }
            catch (Exception ex)
            {
                SampleBase.LogException(ex);
            }
        } 
       
    }
}
