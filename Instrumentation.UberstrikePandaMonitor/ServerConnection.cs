using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using Cmune.DataCenter.Utils;

namespace Instrumentation.Monitoring.ServerMonitor
{
    internal class ServerConnection
    {
        public ManagementScope ConnectTo(string serverIP)
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Username = ConfigurationUtilities.ReadConfigurationManager("RemoteAccountName");
            options.Password = ConfigurationUtilities.ReadConfigurationManager("RemoteAccountPassword");

            //try
            //{
                ManagementScope scope = new ManagementScope("\\\\" + serverIP + "\\root\\cimv2", options);
                scope.Connect();
                return scope;
            //}
            //catch (Exception e) {
            //    if (e.Message.Contains("Access is denied."))
            //        throw e;

            //    CmuneLog.LogInfo(serverIP + " - " + e.Message, Program.LogFileName());
            //    return null;
            //}
        }
    }
}
