using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Business;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsRetentionController : BaseController
    {
        #region properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public StatisticsRetentionController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Statistics;

            DateTime now = DateTime.Now;

            FromDate = now.AddDays(-30).ToDateOnly();
            ToDate = now.AddDays(-1).ToDateOnly();

            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["DisplayCalendar"] = true;

            UseProdDataContext();
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

        #region Methods

        public string GetRetentionCohorts()
        {
            return new Business.Chart.AttritionChart(FromDate, ToDate).DrawRetentionCohortsChart().ToPrettyString();
        }

        public string GetRetentionCohortsByChannel(int channel)
        {
            return new Business.Chart.AttritionChart(FromDate, ToDate).DrawRetentionCohortsChart((ChannelType)channel).ToPrettyString();
        }

        public string GetRetentionCohortsByRegion(int regionId)
        {
            return new Business.Chart.AttritionChart(FromDate, ToDate).DrawRetentionCohortsChart(regionId).ToPrettyString();
        }

        public string GetRetentionCohortsByReferrer(int referrerId)
        {
            return new Business.Chart.AttritionChart(FromDate, ToDate).DrawRetentionCohortsChart((ReferrerPartnerType)referrerId).ToPrettyString();
        }

        #endregion
    }
}