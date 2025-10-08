using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.Instrumentation.Monitoring.Business;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class ServersService
    {
        public static List<SelectListItem> GetManagedServersSelectList(int managedServerId = 0)
        {
            List<ManagedServerModel> monitoredServers = ManagedServerService.GetManagedServers();
            List<SelectListItem> managedServersTestList = new List<SelectListItem>();

            foreach (ManagedServerModel monitoredServer in monitoredServers)
            {
                SelectListItem listItem = new SelectListItem() { Text = monitoredServer.ServerName, Value = monitoredServer.ManagedServerId2.ToString(), Selected = monitoredServer.ManagedServerId2 == managedServerId };
                managedServersTestList.Add(listItem);
            }
            if (managedServerId == 0)
                managedServersTestList.First().Selected = true;
            return managedServersTestList;
        }
    }
}