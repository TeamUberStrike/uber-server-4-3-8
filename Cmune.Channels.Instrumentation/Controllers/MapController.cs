using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.Business;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Utils;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class MapController : BaseController
    {
        public MapController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Management;
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.MapItem;
        }

        public ActionResult MapItem()
        {
            ViewBag.ApplicationVersions = MapService.GetMapApplicationVersions();
            return View();
        }

        [HttpPost]
        public PartialViewResult GetMapItems(string applicationVersion)
        {
            MapClusterView mapsCluster = Games.GetMapCluster(applicationVersion);

            ViewBag.ItemCache = new ItemCache(UberStrikeCommonConfig.ApplicationId, true);
            ViewBag.ApplicationVersion = applicationVersion;
            return PartialView("Partial/GetMapItems", mapsCluster);
        }

        public PartialViewResult LoadEditMapItem(string applicationVersion, int mapId)
        {
            var mapView = Games.GetMapView(applicationVersion, mapId);

            ViewBag.ItemCache = new ItemCache(UberStrikeCommonConfig.ApplicationId, true);
            ViewBag.RecommendableItems = CmuneItem.GetRecommendableItemsForMaps(UberStrikeCommonConfig.ApplicationId);
            ViewBag.ApplicationVersion = applicationVersion;
            return PartialView("Partial/MapItemForm", mapView);
        }

        public JsonResult SaveMapItem(string applicationVersion, int mapId, int recommendedItemId)
        {
            bool success = false;
            string message = string.Empty;

            var map = Games.GetMapView(applicationVersion, mapId);
            // TODO should go into business logic
            var recommendedItems = CmuneItem.GetRecommendableItemsForMaps(UberStrikeCommonConfig.ApplicationId);

            if (recommendedItems.Contains(recommendedItemId))
            {
                map.RecommendedItemId = recommendedItemId;
                var operationResult = Games.UpdateMapRecommendedItem(applicationVersion, mapId, recommendedItemId);
                if (operationResult != MapOperationResult.Ok)
                {
                    message = "Error in db occured";
                }
                else
                {
                    success = true;
                    message = "Successfully updated";
                }
                UberStrikeCacheInvalidation.InvalidateOtherCache(BuildType, "UberStrikeMapItemRecommendation");

            }
            else
            {
                message = "Recommended Item not found";
            }

            return new JsonResult { Data = new { success = success, message = message } };
        }


    }
}
