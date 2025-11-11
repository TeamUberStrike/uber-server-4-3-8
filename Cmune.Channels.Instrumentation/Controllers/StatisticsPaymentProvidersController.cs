using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Business;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsPaymentProvidersController : BaseController
    {
        #region properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public StatisticsPaymentProvidersController()
            : base()
        {
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

        public string GetDailyRevenue(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawDailyRevenueChart((PaymentProviderType) providerId).ToPrettyString();
        }

        public string GetDailyTransactions(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawDailyTransactionsChart(providerId).ToPrettyString();
        }

        public string GetPackagesContributionByRevenue(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawPackageContributionByRevenueChart(providerId, true).ToPrettyString();
        }

        public string GetPackagesContributionByVolume(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawPackageContributionByVolumeChart(providerId, true).ToPrettyString();
        }

        public string GetBundleContributionByRevenue(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawPackageContributionByRevenueChart(providerId, false).ToPrettyString();
        }

        public string GetBundleContributionByVolume(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawPackageContributionByVolumeChart(providerId, false).ToPrettyString();
        }

        public string GetCreditsSales(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawCreditsSalesChart(providerId).ToPrettyString();
        }

        public string GetBundlesSales(int providerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawBundlesSalesChart(providerId).ToPrettyString();
        }

        public ActionResult GetLatestPayments(int providerId, int paymentsCount)
        {
            return View("~/Views/Statistic/Partial/LatestPayments.cshtml", RevenueBusiness.GetLatestDeposits(providerId, paymentsCount));
        }

        public ActionResult GetCreditDeposit(int providerId, string transactionId)
        {
            return View("~/Views/Statistic/Partial/CreditDeposit.cshtml", RevenueBusiness.GetCreditDeposit(providerId, transactionId));
        }

        public string GetTotalRevenue(int providerId)
        {
            return AdminCache.LoadTotalRevenue(FromDate, ToDate, providerId).ToString("N2");
        }

        #endregion
    }
}