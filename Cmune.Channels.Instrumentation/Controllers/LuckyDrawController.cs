using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using System;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class LuckyDrawController : BaseController
    {
        public LuckyDrawController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Management;
        }

        public ActionResult Index()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.LuckyDraw;
            ViewBag.Title = CmuneMenu.ManagementSubTabs.LuckyDraw;
            var allLuckyDraws = CmuneLuckyDrawService.GetLuckyDraws();

            return View(allLuckyDraws);
        }

        public ActionResult GetLuckyDrawForm(int luckyDrawId = 0)
        {
            LuckyDrawView luckyDrawView = CmuneLuckyDrawService.InstantiateDefaultLuckyDraw();
            var itemViews = CmuneItem.GetItemsWithMinimalPrice(CommonConfig.ApplicationIdUberstrike);
            if (luckyDrawId > 0)
            {
                luckyDrawView = CmuneLuckyDrawService.GetLuckyDraw(luckyDrawId);
            }
            itemViews.Sort((i, j) => i.Name.CompareTo(j.Name));
            ViewBag.ItemViews = itemViews;
            return PartialView("Partial/Form/LuckyDrawForm", luckyDrawView);
        }

        public ActionResult SaveLuckyDrawForm(LuckyDrawView luckyDrawView)
        {
            string message = "";
            bool success = false;

            foreach (var sets in luckyDrawView.LuckyDrawSets)
                if (sets.LuckyDrawSetItems != null)
                {
                    foreach (var item in sets.LuckyDrawSetItems)
                    {
                        item.Amount = Math.Max(item.Amount, 0);
                        if (item.Amount > 0)
                        {
                            item.DurationType = BuyingDurationType.None;
                        }
                    }
                }

            if (luckyDrawView.Id == 0)
            {
                success = CmuneLuckyDrawService.AddLuckyDraw(luckyDrawView);
            }
            else if (luckyDrawView.Id > 0)
            {
                success = CmuneLuckyDrawService.EditLuckyDraw(luckyDrawView);
            }

            if (success)
                message = "successfully saved";
            else
                message = "an error occured";

            var itemViews = CmuneItem.GetItemsWithMinimalPrice(CommonConfig.ApplicationIdUberstrike);
            itemViews.Sort((i, j) => i.Name.CompareTo(j.Name));
            ViewBag.ItemViews = itemViews;
            return new JsonResult() { Data = new { message = message, success = success } };
        }


        public ActionResult TestLuckyDraw(LuckyDrawView luckyDrawView)
        {
            int numberOfRoll = 1000;
            List<RollElementSummary> summaries = new List<RollElementSummary>();

            var prizeElements = CmuneLuckyDrawService.RollLuckyDraw(luckyDrawView, numberOfRoll);
            if (prizeElements != null)
            {
                foreach (var prizeEle in prizeElements)
                {
                    var first = summaries.Where(d => (d.PrizeElement != null) && d.PrizeElement.Id == prizeEle.Id).FirstOrDefault();
                    if (first != null)
                    {
                        first.NumberOfRoll++; // update
                    }
                    else
                    { // add
                        var rollElementSummary = new RollElementSummary();
                        rollElementSummary.PrizeElement = prizeEle;
                        rollElementSummary.Price = 0;
                        rollElementSummary.PointPrice = 0;
                        rollElementSummary.NumberOfRoll = 1;
                        summaries.Add(rollElementSummary);
                    }
                }
            }

            int luckyDrawSetId;
            foreach (var summary in summaries)
            {
                if (int.TryParse(summary.PrizeElement.Id, out luckyDrawSetId))
                {
                    var luckyDrawSet = luckyDrawView.LuckyDrawSets.First(d => d.Id == luckyDrawSetId);
                    if (luckyDrawSet.LuckyDrawSetItems != null)
                    {
                        foreach (var luckyDrawSetItem in luckyDrawSet.LuckyDrawSetItems)
                        {
                            if (luckyDrawSetItem != null && luckyDrawSetItem.ItemId > 0)
                                summary.Price += CmuneItem.ComputePrice(luckyDrawSetItem.DurationType, luckyDrawSetItem.ItemId) * summary.NumberOfRoll;
                        }
                    }
                    if (luckyDrawSet.CreditsAttributed > 0)
                        summary.Price += luckyDrawSet.CreditsAttributed * summary.NumberOfRoll;
                    if (luckyDrawSet.PointsAttributed > 0)
                        summary.PointPrice += luckyDrawSet.PointsAttributed * summary.NumberOfRoll;
                }
            }

            ViewBag.NumberOfRoll = numberOfRoll;
            ViewBag.SpentCredits = numberOfRoll * luckyDrawView.Price;
            ViewBag.PriceCurrency = luckyDrawView.UberStrikeCurrencyType;

            return PartialView("Partial/TestLuckyDrawResult", summaries);
        }
    }
}
