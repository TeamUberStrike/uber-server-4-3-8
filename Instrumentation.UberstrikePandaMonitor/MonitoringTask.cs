using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Instrumentation.Monitoring.ServerMonitor.ProxyModel;
using System.Threading;
using Cmune.DataCenter.Utils;

namespace Instrumentation.Monitoring.ServerMonitor
{
    public class MonitoringTask
    {
        public void Run(int managedServerId, string managedServerIp)
        {
            int waitTime = 1;
            int fiveMinutes = 300000;
            while (!CmunePandaMonitor.DataLock.WaitOne(waitTime))
            {
                var startDate = DateTime.Now;
                var serverConnection = new ServerConnection();
                var scope = serverConnection.ConnectTo(managedServerIp);
                if (scope != null && scope.IsConnected)
                {
                    CmuneLog.LogInfo(managedServerIp + " : Retrieving data", CmunePandaMonitor.LogFileName());

                    var wmiWrapper = new WMIWrapper(scope);
                    var managedServerMonitoringModel = new ManagedServerMonitoringProxyModel();

                    managedServerMonitoringModel.ManagedServerId = managedServerId;
                    managedServerMonitoringModel.CPUUsage = wmiWrapper.GetPercentProcessUsed();
                    managedServerMonitoringModel.RamUsage = wmiWrapper.GetPercentMemoryUsed();
                    managedServerMonitoringModel.DiskUsage = wmiWrapper.GetSystemDiskSpaceUsage();
                    managedServerMonitoringModel.BandwidthUsage = wmiWrapper.GetBandwidthUsage();

                    var response = ServerMonitoringWrapper.ReportMonitoring(managedServerMonitoringModel);
                    if (response == false)
                    {
                        throw new ArgumentOutOfRangeException("Saving data in database failed");
                    }
                }
                var endDate = DateTime.Now;

                var elapsedTime = endDate.Subtract(startDate);
                if (elapsedTime.TotalMilliseconds >= fiveMinutes)
                    waitTime = 1;
                else
                {
                    waitTime = fiveMinutes - (int) elapsedTime.TotalMilliseconds;
                }
            }
        }
    }
}
