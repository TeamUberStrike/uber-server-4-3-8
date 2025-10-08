using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsChannelsController : BaseController
    {
        #region properties

        public DateTime FromDate;
        public DateTime ToDate;

        #endregion

        #region Constructors

        public StatisticsChannelsController()
            : base()
        {
            // Init vars
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

        #region Actions

        public string GetDau(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDauChart().ToPrettyString();
        }

        public string GetMau(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawMauChart().ToPrettyString();
        }

        public string GetNewVersusReturning(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawNewVersusReturningChart().ToPrettyString();
        }

        public string GetDailyPlayRate(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDailyPlayRateChart().ToPrettyString();
        }

        public string GetInstallTracking(int channel, string hasUnity)
        {
            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawInstallTrackingChart(hasUnityTmp).ToPrettyString();
        }

        public string GetInstallTrackingLine(int channel, string hasUnity)
        {
            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawInstallTrackingLineChart(hasUnityTmp).ToPrettyString();
        }

        public string GetRevenue(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawRevenueChart().ToPrettyString();
        }

        public string GetDarpu(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDarpuChart().ToPrettyString();
        }

        public string GetDailyTransactions(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDailyTransactionsChart().ToPrettyString();
        }

        public string GetDailyConversionToPaying(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDailyConversionToPayingChart().ToPrettyString();
        }

        public string GetDarppu(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawDarppuChart().ToPrettyString();
        }

        public string GetPackagesContributionByRevenue(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawPackagesContributionByRevenueChart(true).ToPrettyString();
        }

        public string GetPackagesContributionByVolume(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawPackagesContributionByVolumeChart(true).ToPrettyString();
        }

        public string GetBundleContributionByRevenue(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawPackagesContributionByRevenueChart(false).ToPrettyString();
        }

        public string GetBundleContributionByVolume(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawPackagesContributionByVolumeChart(false).ToPrettyString();
        }

        public string GetCreditsSales(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawCreditsSalesChart().ToPrettyString();
        }

        public string GetBundlesSales(int channel)
        {
            return new Business.Chart.ChannelChart(FromDate, ToDate, (ChannelType)channel).DrawBundlesSalesChart().ToPrettyString();
        }

        #endregion
    }
}