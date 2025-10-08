using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class MysteryBoxController : BaseController
    {
        public MysteryBoxController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Management;
        }

        public ActionResult Index()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.MysteryBox;
            ViewBag.Title = CmuneMenu.ManagementSubTabs.MysteryBox;
            var mysteryBoxs = CmuneMysteryBoxService.GetMysteryBoxs();

            return View(mysteryBoxs);
        }

        public ActionResult GetMysteryBoxForm(int mysteryBoxId = 0)
        {
            MysteryBoxView mysteryBoxView = CmuneMysteryBoxService.InstantiateDefaultMysteryBoxView();
            var itemViews = CmuneItem.GetItemsWithMinimalPrice(CommonConfig.ApplicationIdUberstrike);

            if (mysteryBoxId > 0)
            {
                mysteryBoxView = CmuneMysteryBoxService.GetMysteryBox(mysteryBoxId);
            }

            itemViews.Sort((i, j) => i.Name.CompareTo(j.Name));
            ViewBag.ItemViews = itemViews;
            return PartialView("Partial/Form/MysteryBoxForm", mysteryBoxView);
        }

        public ActionResult SaveMysteryBoxForm(MysteryBoxView mysteryBoxView)
        {
            string message = "";
            bool success = false;

            foreach (var item in mysteryBoxView.MysteryBoxItems)
            {
                item.Amount = Math.Max(item.Amount, 0);
                if (item.Amount > 0)
                {
                    item.DurationType = BuyingDurationType.None;
                }
            }

            if (mysteryBoxView.Id == 0)
            {
                success = CmuneMysteryBoxService.AddMysteryBox(mysteryBoxView);
            }
            else if (mysteryBoxView.Id > 0)
            {
                success = CmuneMysteryBoxService.EditMysteryBox(mysteryBoxView);
            }

            if (success)
                message = "successfully saved";
            else
                message = "an error occured";

            return new JsonResult() { Data = new { message = message, success = success } };
        }

        public ActionResult TestMysteryBox(MysteryBoxView mysteryBoxView)
        {
            int numberOfRoll = 1000;
            List<RollElementSummary> summaries = new List<RollElementSummary>();
            var allPrizeElements = CmuneMysteryBoxService.RollMysterBox(mysteryBoxView, numberOfRoll);

            foreach (var prizeElements in allPrizeElements)
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
                        rollElementSummary.NumberOfRoll = 1;
                        summaries.Add(rollElementSummary);
                    }
                }
            }

            int itemId;
            UberStrikeCurrencyType currency;
            foreach (var summary in summaries)
            {
                if (int.TryParse(summary.PrizeElement.Id, out itemId))
                {
                    var mysteryBoxItem = mysteryBoxView.MysteryBoxItems.Where(d => d.ItemId == itemId).First();
                    summary.PrizeElement.Name = CmuneItem.GetItem(itemId).Name;
                    summary.Price += CmuneItem.ComputePrice(mysteryBoxItem.DurationType, mysteryBoxItem.ItemId) * summary.NumberOfRoll;
                }
                else if (Enum.TryParse(summary.PrizeElement.Id, out currency))
                {
                    summary.PrizeElement.Name = currency.ToString();
                    if (currency == UberStrikeCurrencyType.Credits)
                    {
                        summary.Price = mysteryBoxView.Price * summary.NumberOfRoll;
                    }
                    else if (currency == UberStrikeCurrencyType.Points)
                    {
                        summary.PointPrice = mysteryBoxView.Price * summary.NumberOfRoll;
                    }
                }
            }

            ViewBag.NumberOfRoll = numberOfRoll;
            ViewBag.SpentCredits = numberOfRoll * mysteryBoxView.Price;
            ViewBag.PriceCurrency = mysteryBoxView.UberStrikeCurrencyType;
            return PartialView("Partial/TestMysteryBoxResult", summaries);
        }
    }
}