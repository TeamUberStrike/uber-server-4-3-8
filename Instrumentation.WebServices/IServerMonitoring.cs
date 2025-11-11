using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Cmune.Instrumentation.Monitoring.Common.Entities;

namespace Cmune.Instrumentation.WebServices
{
    [ServiceContract]
    public interface IServerMonitoring
    {
        //(IsOneWay = true)
        [OperationContract]
        [WebGet(UriTemplate = "RegisterManagedServerReport?managedServerId={managedServerId}&cpuUsage={cpuUsage}&ramUsage={ramUsage}&bandwidthUsage={bandwidthUsage}&diskUsage={diskUsage}")]
        bool RegisterManagedServerReport(int managedServerId, int cpuUsage, int ramUsage, decimal bandwidthUsage, decimal diskUsage);

        [OperationContract]
        [WebGet(UriTemplate = "GetServerList")]
        ManagedServerIpCollectionModel GetServerList();

        [OperationContract]
        [WebGet(UriTemplate = "TestModel")]
        string TestModel();
    }
}
