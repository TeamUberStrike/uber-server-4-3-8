using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.Channels.Instrumentation.Business;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsReferrersController : BaseController
    {
        #region properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        protected ReferrerPartnerType _referrerOne;
        public ReferrerPartnerType ReferrerOne
        {
            get { return _referrerOne; }
            protected set { _referrerOne = value; }
        }
        protected ReferrerPartnerType _referrerTwo;
        public ReferrerPartnerType ReferrerTwo
        {
            get { return _referrerTwo; }
            protected set { _referrerTwo = value; }
        }
        protected ReferrerPartnerType _referrerThree;
        public ReferrerPartnerType ReferrerThree
        {
            get { return _referrerThree; }
            protected set { _referrerThree = value; }
        }

        #endregion

        #region Constructors

        public StatisticsReferrersController()
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

            ReferrerOne = ReferrerPartnerType.None;
            ReferrerTwo = ReferrerPartnerType.None;
            ReferrerThree = ReferrerPartnerType.None;

            if (HttpContext.Request.Params != null)
            {
                if (HttpContext.Request.Params["referrerOne"] != null)
                {
                    EnumUtilities.TryParseEnumByValue(HttpContext.Request.Params["referrerOne"], out _referrerOne);
                }

                if (HttpContext.Request.Params["referrerTwo"] != null)
                {
                    EnumUtilities.TryParseEnumByValue(HttpContext.Request.Params["referrerTwo"], out _referrerTwo);
                }

                if (HttpContext.Request.Params["referrerThree"] != null)
                {
                    EnumUtilities.TryParseEnumByValue(HttpContext.Request.Params["referrerThree"], out _referrerThree);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        #region Methods

        public string GetDailyRevenue()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawDailyRevenueChart().ToPrettyString();
        }

        public string GetDau()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawDauChart().ToPrettyString();
        }

        public string GetMau()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawMauChart().ToPrettyString();
        }

        public string GetDailyPlayRate()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawDailyPlayRateChart().ToPrettyString();
        }

        public string GetReferrerContribution()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawReferrerContributionChart().ToPrettyString();
        }

        public string GetNewMembers()
        {
            return new Business.Chart.ReferrersChart(FromDate, ToDate, ReferrerOne, ReferrerTwo, ReferrerThree).DrawNewMembersChart().ToPrettyString();
        }

        #endregion
    }
}