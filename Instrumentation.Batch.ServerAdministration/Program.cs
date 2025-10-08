using System;
using System.Collections.Generic;
using System.Text;
using Cmune.DataCenter.Utils;

namespace Cmune.Instrumentation.Batch.ServerAdministration
{
    public class Program
    {
        static void Main(string[] args)
        {
            int argsCount = args.Length;
            bool debugMode = ConfigurationUtilities.ReadConfigurationManagerBool("DebugBatch");

            if (argsCount == 1)
            {
                switch (args[0])
                {
                    //case "log":
                    //    if (debugMode) { Console.WriteLine("Archive logs"); }
                    //    ServerUtils.ArchiveLogs(true);
                    //    break;
                    case "disk":
                        if (debugMode) { Console.WriteLine("Analyze disk space"); }
                        ServerUtils.AnalyzeDiskSpace(true);
                        break;
                    case "help":
                        Console.WriteLine("This is the options you can provide:");
                        Console.WriteLine("\thelp To display help");
                        //Console.WriteLine("\tlog To archive the logs");
                        Console.WriteLine("\tdisk To check the available space on the system disk");
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
    }
}