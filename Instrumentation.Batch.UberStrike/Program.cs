using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;

namespace Cmune.Instrumentation.Batch.Apps.Shooter
{
    class Program
    {
        public const string ModeHelp = "help";
        public const string ModeLoadout = "loadout";
        public const string ModeUnsuccessfulRegistration = "emailReg";

        static void Main(string[] args)
        {
            int argumentsCount = args.Length;
            bool debugMode = ConfigurationUtilities.ReadConfigurationManagerBool("DebugBatch");

            if (argumentsCount == 1)
            {
                switch (args[0])
                {
                    case ModeLoadout:
                        if (debugMode) { Console.WriteLine("Loadout expiration"); }
                        LoadoutExpiration avatarsSync = new LoadoutExpiration();
                        avatarsSync.ExpireLoadouts(debugMode);
                        break;
                    case ModeUnsuccessfulRegistration:
                        if (debugMode) { Console.WriteLine("Email unsuccessful registration"); }
                        Users.EmailUnfinishedRegistration();
                        break;
                    case ModeHelp:
                        Console.WriteLine("This is the options you can provide:");
                        Console.WriteLine(String.Format("\t{0}: To display help", ModeHelp));
                        Console.WriteLine(String.Format("\t{0}: To remove expired items from the equipped items", ModeLoadout));
                        Console.WriteLine(String.Format("\t{0}: To email unsuccessful registration", ModeUnsuccessfulRegistration));
                        break;
                    default:
                        if (debugMode) { Console.WriteLine(String.Format("This argument is not recognized, provide {0} as argument to receive assistance.", ModeHelp)); }
                        break;
                }
            }
            else
            {
                if (debugMode) { Console.WriteLine(String.Format("You should provide only one argument. Provide {0} as argument to receive assistance.", ModeHelp)); }
            }

            if (debugMode)
            {
                Console.WriteLine("Type a key to exit...");
                Console.ReadKey();
            }
        }
    }
}