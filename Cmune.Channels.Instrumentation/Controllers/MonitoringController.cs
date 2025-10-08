using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Instrumentation.Monitoring.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.Instrumentation.Monitoring.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class MonitoringController : BaseController
    {
        #region Properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public MonitoringController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Monitoring;

            DateTime now = DateTime.Now;

            decimal quarters = (decimal)now.Minute / (decimal)15;
            now = now.Date + new TimeSpan(0, now.Hour, (int)quarters * 15, 0, 0);

            ToDate = now;
            FromDate = ToDate.AddDays(-1);
        }

        #endregion

        #region Actions

        [HttpGet]
        public ActionResult SeePhotonsHealth()
        {
            ViewBag.SubActiveTab = CmuneMenu.MonitoringSubTabs.PhotonsHealth;
            ViewBag.Versions = AdminCache.GenerateVersionsDropDownListItems(5);

            return View();
        }

        [HttpGet]
        public ActionResult UnityExceptions()
        {
            ViewBag.SubActiveTab = CmuneMenu.MonitoringSubTabs.UnityExceptions;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MonitoringSubTabs.UnityExceptions); 

            return View();
        }

        [HttpGet]
        public ActionResult ExceptionGroup(string stacktraceHash)
        {
            ViewBag.SubActiveTab = CmuneMenu.MonitoringSubTabs.UnityExceptions;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MonitoringSubTabs.UnityExceptions);

            UnityExceptionGroupDetailedView exceptionGroup = UnityExceptionsService.GetExceptionGroup(stacktraceHash);

            return View(exceptionGroup);
        }

        [HttpGet]
        public ActionResult Exception(int exceptionId)
        {
            ViewBag.SubActiveTab = CmuneMenu.MonitoringSubTabs.UnityExceptions;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MonitoringSubTabs.UnityExceptions);

            UnityExceptionView exception = UnityExceptionsService.GetException(exceptionId);

            return View(exception);
        }

        #endregion

        #region Methods

        #region Unity exceptions

        [HttpPost]
        public ActionResult GetExceptionGroups()
        {
            DateTime toDate = DateTime.Now;
            DateTime fromDate = toDate.AddDays(-7);

            List<UnityExceptionGroupSummaryView> exceptions = UnityExceptionsService.GetExceptionGroups(fromDate, toDate);

            return View("Partial/UnityExceptionGroups", exceptions);
        }

        [HttpPost]
        public JsonResult DeleteExceptionGroup(string stacktraceHash)
        {
            UnityExceptionsService.DeleteExceptionGroup(stacktraceHash);

            return new JsonResult() { Data = new { IsDeleted = true } };
        }

        [HttpPost]
        public JsonResult DeleteAllExceptions()
        {
            UnityExceptionsService.DeleteAllExceptions();

            return new JsonResult() { Data = new { IsDeleted = true } };
        }

        #endregion

        #region Photon health

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GetPhotonsHealthCharts(string version)
        {
            ViewBag.InstancesId = AdminCache.LoadInstancesId(version);
            ViewBag.VersionNumber = version;

            return View("Partial/PhotonsHealthCharts");
        }

        [ValidateInput(false)]
        public string GetConcurrentUsers(string version)
        {
            return new Business.Chart.PhotonsChart(FromDate, ToDate).DrawCcuChart(version).ToPrettyString();
        }

        [ValidateInput(false)]
        public string GetPhotonHealth(int instanceId, string version)
        {
            return new Business.Chart.PhotonsChart(FromDate, ToDate).DrawPhotonHealthChart(instanceId, version).ToPrettyString();
        }

        #endregion

        #endregion
    }
}