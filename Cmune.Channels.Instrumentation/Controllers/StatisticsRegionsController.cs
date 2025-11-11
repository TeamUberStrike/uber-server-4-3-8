using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Business.Chart;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsRegionsController : BaseController
    {
        #region Properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public StatisticsRegionsController()
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

        public string GetDailyRevenue(int? regionOneDailyRevenue, int? regionTwoDailyRevenue, int? regionThreeDailyRevenue)
        {
            int regionOne = -1;
            int regionTwo = -1;
            int regionThree = -1;

            if (regionOneDailyRevenue.HasValue)
            {
                regionOne = regionOneDailyRevenue.Value;
            }

            if (regionTwoDailyRevenue.HasValue)
            {
                regionTwo = regionTwoDailyRevenue.Value;
            }

            if (regionThreeDailyRevenue.HasValue)
            {
                regionThree = regionThreeDailyRevenue.Value;
            }

            return new Business.Chart.RegionsChart(FromDate, ToDate).DrawDailyRevenueChart(regionOne, regionTwo, regionThree).ToPrettyString();
        }

        public JsonResult GetCountriesByRevenueDropDownList()
        {
            return Json(AdminCache.GenerateCountriesRevenueDropDownListItems(FromDate, ToDate));
        }

        public JsonResult GetCountriesByRevenueList()
        {
            Dictionary<int, decimal> revenueByCountry = AdminCache.LoadRevenueByCountriesOrdered(FromDate, ToDate);
            Dictionary<int, string> countriesName = AdminCache.LoadCountriesName();
            var countries = from r in revenueByCountry select new { ColumnOne = countriesName[r.Key], ColumnTwo = r.Value.ToString("N2") };

            return Json(countries);
        }

        public JsonResult GetCountriesByMauDropDownList()
        {
            return Json(AdminCache.GenerateCountriesMauDropDownListItems(ToDate, false, false));
        }

        public JsonResult GetCountriesByMauList()
        {
            Dictionary<int, int> mauByCountry = AdminCache.LoadMonthlyActiveUsersByCountriesOrdered(ToDate);
            Dictionary<int, string> countriesName = AdminCache.LoadCountriesName();
            var countries = from r in mauByCountry select new { ColumnOne = countriesName[r.Key], ColumnTwo = r.Value.ToString("N0") };

            return Json(countries);
        }

        public string GetMau(int? regionMau)
        {
            int region = -1;

            if (regionMau.HasValue)
            {
                region = regionMau.Value;
            }

            return new RegionsChart(FromDate, ToDate).DrawMauChart(region).ToPrettyString();
        }

        public string GetDau(int? regionDau)
        {
            int region = -1;

            if (regionDau.HasValue)
            {
                region = regionDau.Value;
            }

            return new Business.Chart.RegionsChart(FromDate, ToDate).DrawDauChart(region).ToPrettyString();
        }

        public string GetDailyPlayRate(int? regionDailyPlayRate)
        {
            int region = -1;

            if (regionDailyPlayRate.HasValue)
            {
                region = regionDailyPlayRate.Value;
            }

            return new Business.Chart.RegionsChart(FromDate, ToDate).DrawDailyPlayRateChart(region).ToPrettyString();
        }

        public string GetDarpu(int? regionDarpu)
        {
            int region = -1;

            if (regionDarpu.HasValue)
            {
                region = regionDarpu.Value;
            }

            return new Business.Chart.RegionsChart(FromDate, ToDate).DrawDarpuChart(region).ToPrettyString();
        }

        #endregion
    }
}