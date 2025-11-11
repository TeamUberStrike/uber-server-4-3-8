using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Utils;
using System.Xml;
using Instrumentation.Monitoring.ServerMonitor.ProxyModel;
using System.IO;

namespace Instrumentation.Monitoring.ServerMonitor
{
    public static class ServerMonitoringWrapper
    {
        public static List<ManagedServerIpProxyModel> GetServerList()
        {
            var webServiceUrl = ConfigurationUtilities.ReadConfigurationManager("InstrumentationWebService_ServerMonitoring");
            var webGet = new WebGetRequest(webServiceUrl + "/GetServerList");
            var serverListXml = webGet.GetResponse();

            var xmlReader = XmlReader.Create(new StringReader(serverListXml));
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ManagedServerIpProxyModel>));
            //xmlSerializer.Deserialize(xmlReader);

            var managedServers = new List<ManagedServerIpProxyModel>();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);
            var managedServerIpModels = xmlDoc.GetElementsByTagName("ManagedServerIpModel");
            for (int i = 0; i < managedServerIpModels.Count; i++)
            {
                var Id = int.Parse(managedServerIpModels[i].ChildNodes[0].InnerText);
                var Ip = managedServerIpModels[i].ChildNodes[1].InnerText;
                managedServers.Add(new ManagedServerIpProxyModel() { Id = Id, Ip = Ip });
            }

            return managedServers;
        }

        public static bool ReportMonitoring(ManagedServerMonitoringProxyModel managedServerMonitoringModel)
        {
            var webServiceUrl = ConfigurationUtilities.ReadConfigurationManager("InstrumentationWebService_ServerMonitoring");
            var registerManagedServerReportUrl = webServiceUrl + "/RegisterManagedServerReport";
            var webPost = new WebGetRequest(registerManagedServerReportUrl);

            webPost.Add("managedServerId", managedServerMonitoringModel.ManagedServerId.ToString());
            webPost.Add("cpuUsage", managedServerMonitoringModel.CPUUsage.ToString());
            webPost.Add("ramUsage", managedServerMonitoringModel.RamUsage.ToString());
            webPost.Add("bandwidthUsage", managedServerMonitoringModel.BandwidthUsage.ToString());
            webPost.Add("diskUsage", managedServerMonitoringModel.DiskUsage.ToString());
            var response = webPost.GetResponse();

            var xmlReader = XmlReader.Create(new StringReader(response));
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlReader);
            var value = xmlDoc.GetElementsByTagName("boolean");
            return bool.Parse(value[0].InnerText);
        }

        public static void GetServerListRoutine()
        {
            int waitTime = CmunePandaMonitor.FiveMinutes;

            while (!CmunePandaMonitor.GetServerLock.WaitOne(waitTime))
            {
                var serverList = GetServerList();
                var listOfServers = new List<ManagedServerIpProxyModel>();
                
                foreach (var server in serverList)
                {
                    if (listOfServers.Where(d => d.Ip == server.Ip).Count() == 0)
                        listOfServers.Add(server);
                }

                CmunePandaMonitor.ListOfNewServers = GetServerList();
                CmuneLog.LogInfo("New servers retrieved", CmunePandaMonitor.LogFileName());
            }
        }
    }
}
