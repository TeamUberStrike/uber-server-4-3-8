using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Instrumentation.Monitoring.Business;
using Cmune.Channels.Instrumentation.Extensions;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ServersMonitoringController : BaseController
    {
        #region Properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public ServersMonitoringController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Monitoring;
            ViewBag.SubActiveTab = CmuneMenu.MonitoringSubTabs.ServersMonitoring;
            ViewBag.DisplayCalendar = true;

            DateTime now = DateTime.Now;

            //decimal quarters = (decimal)now.Minute / (decimal)15;
            //now = now.Date + new TimeSpan(0, now.Hour, (int)quarters * 15, 0, 0);

            ToDate = DateTime.Now.AddDays(1);
            FromDate = DateTime.Now.AddDays(-1);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // User selected dates?
            if (HttpContext.Request.Params != null && HttpContext.Request.Params["startdate"] != null)
                FromDate = Convert.ToDateTime(HttpContext.Request.Params["startdate"]);

            if (HttpContext.Request.Params != null && HttpContext.Request.Params["enddate"] != null)
                ToDate = Convert.ToDateTime(HttpContext.Request.Params["enddate"]);

            base.OnActionExecuting(filterContext);
        }

        //
        // GET: /ServersMonitoring/

        public ActionResult Index(int selectedManagedServer = 0)
        {
            // for dropdownList
            var monitoredServers = ManagedServerService.GetManagedServers();
            ViewBag.ManagedServers = monitoredServers;

            var monitoredServer = (selectedManagedServer == 0) ? monitoredServers.First() : monitoredServers.Single(d=>d.ManagedServerId2 == selectedManagedServer);
            ViewBag.ManagedServerId = monitoredServer.ManagedServerId2;

            return View();
        }

        public ActionResult GetManagedServerServices(int managedServerId)
        {
            var monitoredServer = ManagedServerService.GetManagedServer(managedServerId);
            var photonGroup = Games.GetPhotonGroupsOrderedByModificationDate().FirstOrDefault();
            var photonServers = Games.GetPhotonServerList(photonGroup.PhotonsGroupID).Where(d => d.IP == monitoredServer.PublicIp).ToList();

            return PartialView("Partial/ManagedServerServices", photonServers);
        }

        public string GetCPUUsage(int managedServerId)
        {
            ViewBag.ManagedServerId = managedServerId;
            return new Business.Chart.ServersMonitoringChart(FromDate, ToDate).DrawCpuUsage(managedServerId).ToPrettyString();
        }

        public string GetRamUsage(int managedServerId)
        {
            ViewBag.ManagedServerId = managedServerId;
            return new Business.Chart.ServersMonitoringChart(FromDate, ToDate).DrawRamUsage(managedServerId).ToPrettyString();
        }

        public string GetBandwidthUsage(int managedServerId)
        {
            ViewBag.ManagedServerId = managedServerId;
            return new Business.Chart.ServersMonitoringChart(FromDate, ToDate).DrawBandwidthUsage(managedServerId).ToPrettyString();
        }

        public string GetDiskSpaceUsage(int managedServerId)
        {
            ViewBag.ManagedServerId = managedServerId;
            return new Business.Chart.ServersMonitoringChart(FromDate, ToDate).DrawDiskSpaceUsage(managedServerId).ToPrettyString();
        }
    }
}