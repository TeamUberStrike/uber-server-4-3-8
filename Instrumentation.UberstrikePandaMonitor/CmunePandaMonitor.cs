using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Instrumentation.Monitoring.ServerMonitor.ProxyModel;
using System.Threading.Tasks;
using Cmune.DataCenter.Utils;

namespace Instrumentation.Monitoring.ServerMonitor
{
    public partial class CmunePandaMonitor : ServiceBase
    {
        public static ManualResetEvent DataLock = new ManualResetEvent(false);
        public static ManualResetEvent GetServerLock = new ManualResetEvent(false);
        public static ManualResetEvent MainLock = new ManualResetEvent(false);

        public static List<ManagedServerIpProxyModel> ListOfNewServers = new List<ManagedServerIpProxyModel>();
        public static List<ManagedServerIpProxyModel> ListOfNewServersToMonitor = new List<ManagedServerIpProxyModel>();
        public static List<ManagedServerIpProxyModel> ListOfServersOnMonitoring = new List<ManagedServerIpProxyModel>();

        public static int FiveMinutes = 300000;
        public static string LogLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../Log/";

        public static string LogFileName()
        {
            return "Server Monitoring [" + DateTime.Now.ToString("yyyy-MM-dd") + "]";
        }

        private static string ToReadableIps(List<ManagedServerIpProxyModel> listOfServers)
        {
            StringBuilder splattedServers = new StringBuilder();
            listOfServers.ForEach(d => splattedServers.Append(d.Ip + " | "));
            return splattedServers.ToString();
        }

        public CmunePandaMonitor()
        {
            InitializeComponent();
            AutoLog = true;
            if (!System.Diagnostics.EventLog.SourceExists("MyCmunePandaMonitorSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("MyCmunePandaMonitorSource", "MyCmunePandaMonitorNewLog");
            }
            eventLog1.Source = "MyCmunePandaMonitorSource";
            eventLog1.Log = "MyCmunePandaMonitorNewLog";

            var cmunePandaMonitorTask = new Task(() =>
            {
                List<Task> ListOfTasks = new List<Task>();

                ListOfNewServersToMonitor = ServerMonitoringWrapper.GetServerList();
                CmuneLog.LogInfo("PandaMonitor " + ConfigurationUtilities.ReadConfigurationManager("PublishVersion") + " ! Pandaaaaaa !!!", LogFileName());
                CmuneLog.LogInfo("Initialization of the monitoring tasks", LogFileName());

                var task = new Task(() =>
                {
                    // set Program.ListOfNewServers
                    ServerMonitoringWrapper.GetServerListRoutine();
                });

                task.Start();
                int waitTime = 1;

                while (!CmunePandaMonitor.MainLock.WaitOne(waitTime))
                {
                    ListOfNewServersToMonitor.AddRange(ListOfNewServers.Where(d => !ListOfServersOnMonitoring.Select(e => e.Ip).Contains(d.Ip)));
                    ListOfNewServers.Clear();

                    CmuneLog.LogInfo("New servers to monitor " + ToReadableIps(ListOfNewServersToMonitor) + "\n" + "Servers on monitoring " + ToReadableIps(ListOfServersOnMonitoring), LogFileName());

                    foreach (var managedServer in ListOfNewServersToMonitor)
                    {
                        ListOfServersOnMonitoring.Add(managedServer);
                        var serverMonitoringTask = new Task((obj)
                                        =>
                        {
                            var monitoringTask = new MonitoringTask();
                            var proxyIpModel = (ManagedServerIpProxyModel)obj;

                            CmuneLog.LogInfo(proxyIpModel.Ip + " - STARTED", LogFileName());

                            try
                            {
                                monitoringTask.Run(proxyIpModel.Id, proxyIpModel.Ip);
                            }
                            catch (Exception e)
                            {
                                CmuneLog.LogInfo(proxyIpModel.Ip + " - " + e.Message, LogFileName());
                                CmuneLog.LogInfo(proxyIpModel.Ip + " - " + e.Message, "IP " + proxyIpModel.Ip.Replace(".", "-"));
                            }
                            finally
                            {
                                ListOfServersOnMonitoring.RemoveAll(d => d.Id == proxyIpModel.Id);
                            }

                            CmuneLog.LogInfo(proxyIpModel.Ip + " - FINISHED", LogFileName());

                        }, managedServer, TaskCreationOptions.LongRunning);
                        serverMonitoringTask.Start();
                    }
                    ListOfNewServersToMonitor.Clear();

                    waitTime = CmunePandaMonitor.FiveMinutes;
                }
            });
            cmunePandaMonitorTask.Start();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop");
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue");
        }
    }
}
