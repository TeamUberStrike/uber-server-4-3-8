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
using System.Data.Linq;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsItemsController : BaseController
    {
        #region properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        #endregion

        #region Constructors

        public StatisticsItemsController()
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

        public string GetItemPointContribution()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawItemPointContributionChart().ToPrettyString();
        }

        public string GetCreditItemSale(int? itemOneCreditsContribution, int? itemTwoCreditsContribution, int? itemThreeCreditsContribution)
        {
            int itemOne = 0;
            int itemTwo = 0;
            int itemThree = 0;

            if (itemOneCreditsContribution.HasValue)
            {
                itemOne = itemOneCreditsContribution.Value;
            }

            if (itemTwoCreditsContribution.HasValue)
            {
                itemTwo = itemTwoCreditsContribution.Value;
            }

            if (itemThreeCreditsContribution.HasValue)
            {
                itemThree = itemThreeCreditsContribution.Value;
            }

            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawItemCreditsContributionChart(itemOne, itemTwo, itemThree).ToPrettyString();
        }

        public JsonResult GetItemCreditSaleDropDownList()
        {
            return Json(AdminCache.GenerateItemCreditSaleDropDownListItems(CommonConfig.ApplicationIdUberstrike, FromDate, ToDate));
        }

        public JsonResult GetItemPricingAvailability(int itemId)
        {
            ItemView itemView = null;

            if (itemId != 0)
            {
                itemView = AdminCache.LoadItemView(itemId);
            }

            if (itemView != null)
            {
                var itemPricing = new
                {
                    ShopDailyCredits = itemView.CreditsPerDay,
                    ShopPermanentCredits = itemView.PermanentCredits,
                    IsAvailableForOneDay = itemView.Enable1Day,
                    IsAvailableForSevenDays = itemView.Enable7Days,
                    IsAvailableForThirtyDays = itemView.Enable30Days,
                    IsAvailableForNinetyDays = itemView.Enable90Days,

                    //TODO: remove the deprecated fields
                    IsAvailableInShop = true,
                    IsAvailableInUnderground = true,
                    UndergroundDailyCredits = 0,
                    UndergroundPermamentCredits = 0,
                };

                return Json(itemPricing);
            }

            return Json("null");
        }

        public string GetAveragePointsBalance()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawAveragePointsBalanceChart().ToPrettyString();
        }

        public string GetPointsMovement()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawPointsMovementChart().ToPrettyString();
        }

        public string GetAverageCreditsBalance()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawAverageCreditsBalanceChart().ToPrettyString();
        }

        public string GetCreditsMovement()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawCreditsMovementChart().ToPrettyString();
        }

        public string GetPointDepositTypesDistribution()
        {
            return new Business.Chart.ItemEconomyChart(FromDate, ToDate).DrawPointDepositTypeDistributionChart().ToPrettyString();
        }

        #endregion
    }
}