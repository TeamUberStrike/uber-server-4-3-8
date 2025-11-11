using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.Instrumentation.Monitoring.Business;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.Instrumentation.WebServices
{
    public partial class WebServicesClass : IAllWebServicesInterfaces
    {
        public bool RegisterManagedServerReport(int managedServerId, int cpuUsage, int ramUsage, decimal bandwidthUsage, decimal diskUsage)
        {
            var res = ManagedServerMonitoringService.CreateManagedServerMonitoring(managedServerId, cpuUsage, ramUsage, bandwidthUsage, diskUsage);
            return res == MonitoringOperationResult.Ok;
            //CmuneLog.CustomLogToDefaultPath("RegisterManagedServerReport", res.ToString());
        }

        public ManagedServerIpCollectionModel GetServerList()
        {
            var managedServers = ManagedServerService.GetManagedServersIPs();
            var managedServersCollection = new ManagedServerIpCollectionModel();
            managedServersCollection.ManagedServerIpList = managedServers;
            return managedServersCollection;
        }

        public string TestModel()
        {
            //var managed = new TestModel();
            //return TestModel();
            return "works";
        }
    }
}
