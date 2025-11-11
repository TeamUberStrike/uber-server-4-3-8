using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Enums;
using UberStrike.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ItemDeprecationController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.SubActiveTab = CmuneMenu.ItemSubTabs.Catalog;

            return View();
        }

        [HttpPost]
        public ActionResult GetItemPricing(int ItemId)
        {
            int dailyPrice = 0;
            int permanentPrice = 0;
            var itemDeprecationModel = new ItemDeprecationModel();

            var item = CmuneItem.GetItem(ItemId);
            if (item != null && !item.IsDisabled)
            {
                if (item.CreditsPerDay > 0)
                {
                    dailyPrice = item.CreditsPerDay;
                }
                if (item.PermanentCredits > 0)
                {
                    permanentPrice = item.PermanentCredits;
                }

                itemDeprecationModel.ItemId = item.ItemId;
                itemDeprecationModel.ItemName = item.Name;
                itemDeprecationModel.DailyPrice = dailyPrice;
                itemDeprecationModel.PermanentPrice = permanentPrice;
                itemDeprecationModel.NumberOfUsers = CmuneEconomy.GetNumberOfUsersUsingItem(item.ItemId);
            }
            ViewBag.IsExist = (item != null && !item.IsDisabled);
            return PartialView("Partial/ItemDeprecationPrice", itemDeprecationModel);
        }

        [HttpPost]
        public JsonResult Deprecate(ItemDeprecationModel itemDeprecationModel)
        {
            var item = CmuneItem.GetItemView(itemDeprecationModel.ItemId);
            bool success = false;
            string message = string.Empty;
            if (!UberStrikeCommonConfig.FirstLoadoutItemIds.Contains(item.ItemId) && !UberStrikeCommonConfig.FirstLoadoutWeaponItemIds.Contains(item.ItemId))
            {
                if (itemDeprecationModel.DailyPrice > 0 && itemDeprecationModel.PermanentPrice > 0)
                {
                    if (item != null && !item.IsDisable)
                    {
                        try
                        {
                            CmuneEconomy.DeprecateItem(item, itemDeprecationModel.DailyPrice, itemDeprecationModel.PermanentPrice);
                            success = true;
                            message = "Item deprecated";
                        }
                        catch (Exception e)
                        {
                            message = "Error occured";
                        }
                    }
                    else
                    {
                        message = "Item already disabled or not found";
                    }
                }
                else
                {
                    message = "A daily and permanent price must be informed to deprecate this item";
                }
            }
            else
            {
                message = "Can't deprecate this item";
            }
            return new JsonResult() { Data = new { success = success, message = message } };
        }
    }
}