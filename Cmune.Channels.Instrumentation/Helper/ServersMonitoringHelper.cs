using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class ServersMonitoringHelper
    {
        public static List<SelectListItem> ManagedServersList(this HtmlHelper helper, List<ManagedServerModel> monitoredServers, int selectedManagedServerId = 0)
        {
            List<SelectListItem> managedServersTestList = new List<SelectListItem>();

            foreach (ManagedServerModel monitoredServer in monitoredServers)
            {
                SelectListItem listItem = new SelectListItem() { Text = monitoredServer.ServerIDC + " -- " + ((RegionType) monitoredServer.Region).ToString() + " -- " + monitoredServer.PublicIp + " -- " + monitoredServer.ServerName, Value = monitoredServer.ManagedServerId2.ToString(), Selected = monitoredServer.ManagedServerId2 == selectedManagedServerId };
                managedServersTestList.Add(listItem);
            }
            if (selectedManagedServerId == 0)
                managedServersTestList.First().Selected = true;
            return managedServersTestList;
        }


    }
}