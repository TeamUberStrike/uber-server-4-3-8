using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Business;
using Cmune.Instrumentation.Monitoring.Business;
using System.Net;

namespace Cmune.Instrumentation.Batch.Core
{
    public class Program
    {
        static void Main(string[] args)
        {
            int nbArgs = args.Length;
            bool debugMode = ConfigurationUtilities.ReadConfigurationManagerBool("DebugBatch");

            try
            {
                if (nbArgs == 1 || nbArgs == 2)
                {
                    switch (args[0])
                    {
                        case "unban":
                            if (debugMode) { Console.WriteLine("Unban"); }
                            CmuneMember.UnbanTemporaryBanAccounts(true);
                            break;
                        case "delete-identity-validation":
                            if (debugMode) { Console.WriteLine("Delete identity validations"); }
                            CmuneMember.DeleteOldValidationCodes(true);
                            break;
                        case "unban-chat":
                            if (debugMode) { Console.WriteLine("Unban from chat"); }
                            CmuneMember.UnbanTemporaryBanFromChat(true);
                            break;
                        case "check-local-cache":
                            if (debugMode) { Console.WriteLine("Check local cache"); }

                            if (nbArgs == 2)
                            {
                                WebFileVersionChecker.CheckFile(args[1], true);
                            }
                            else
                            {
                                if (debugMode) { Console.WriteLine("You should provide the URL of the file to check as argument."); }
                            }

                            break;
                        case "exchange-rates":
                            CmuneEconomy.UpdateExchangeRates();
                            break;
                        case "unban-ip":
                            if (debugMode) { Console.WriteLine("Unban IPs"); }
                            CmuneMember.UnbanTemporaryBanIps(true);
                            break;
                        case "email":
                            if (debugMode) { Console.WriteLine("Email"); }

                            DateTime yesterday = DateTime.Now.ToDateOnly().AddDays(-1);

                            EmailMarketing.UpdateEmailStatusFromProvider(yesterday, yesterday);
                            break;
                        case "email-link":
                            if (debugMode) { Console.WriteLine("Email-link"); }

                            if (nbArgs == 2)
                            {
                                EmailMarketing.GenerateUnsubscribeLinks(args[1]);
                            }
                            else
                            {
                                if (debugMode) { Console.WriteLine("You should provide the filePath of the csv as argument."); }
                            }

                            break;
                        case "help":
                            Console.WriteLine("This is the options you can provide:");
                            Console.WriteLine("\thelp To display help");
                            Console.WriteLine("\tunban To unban member if they are banned until today");
                            Console.WriteLine("\tdelete-identity-validation To delete the old identity validations");
                            Console.WriteLine("\tunban-chat To unban member from chat if they are banned until today");
                            Console.WriteLine("\tcheck-local-cache To check if the UnityObject.js file hosted on Unity servers changed");
                            Console.WriteLine("\texchange-rates To refresh the exchange rates for all our supported currencies");
                            Console.WriteLine("\tunban-ip To unban Ips if they are banned until today");
                            Console.WriteLine("\temail Update marketing status");
                            Console.WriteLine("\temail-link Add unsuscribe links to csv file, Cmid should be the first column, provide the filePath as an argument");
                            break;
                        default:
                            Console.WriteLine("This argument is not recognized, provide help as argument to receive assistance.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("You should provide one or two arguments. Provide help as argument to receive assistance.");
                }

                if (debugMode)
                {
                    Console.WriteLine("Type a key to exit...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                StringBuilder exceptionLog = new StringBuilder();
                exceptionLog.AppendLine();
                exceptionLog.AppendLine(ex.GetType().Name);
                exceptionLog.AppendLine(ex.Message);
                exceptionLog.AppendLine(ex.StackTrace);

                CmuneLog.CustomLogToDefaultPath("batch.core.log", exceptionLog.ToString());
                throw;
            }
        }
    }
}