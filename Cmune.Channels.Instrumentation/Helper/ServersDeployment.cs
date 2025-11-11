using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Cmune.Channels.Instrumentation.Utils.Extensions;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Business;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class ServersDeployment
    {
        public static List<SelectListItem> GetRegionItems(this HtmlHelper html, ListItem[] regionList, string selectValue)
        {
            var photonRegionTypeList = regionList;
            List<SelectListItem> photonsRegionTypeList = new List<SelectListItem>();
            foreach (var photoRegion in photonRegionTypeList)
            {
                SelectListItem listItem = new SelectListItem() { Text = photoRegion.Text, Value = photoRegion.Value };
                photonsRegionTypeList.Add(listItem);
            }
            photonsRegionTypeList.SetSelectValue(selectValue);
            return photonsRegionTypeList;
        }

        public static List<SelectListItem> GetManagedServersIPItems(this HtmlHelper html, List<ManagedServerModel> managedServersList, string selectValue = "")
        {
            managedServersList = managedServersList.OrderBy(d => d.Region).ThenBy(d => d.PublicIp).ToList();
            List<SelectListItem> managedServersItemList = new List<SelectListItem>();
            foreach (var managedServer in managedServersList)
            {
                SelectListItem listItem = new SelectListItem() { Text = ((RegionType)managedServer.Region).ToString() + " -- " + managedServer.PublicIp, Value = managedServer.PublicIp };
                managedServersItemList.Add(listItem);
            }
            if(selectValue != string.Empty)
                managedServersItemList.SetSelectValue(selectValue);
            return managedServersItemList;
        }

        public static List<SelectListItem> GetSelectListItemView(this HtmlHelper html, List<int> itemIds, ItemCache itemCache, string selectValue = "")
        {
            itemIds = itemIds.OrderBy(d => d).ToList();
            List<SelectListItem> selectListOfItem = new List<SelectListItem>();

            foreach (var itemId in itemIds)
            {
                SelectListItem listItem = new SelectListItem() { Text = String.Format("{0} - {1}", itemId, itemCache.GetItemName(itemId)), Value = itemId.ToString() };
                selectListOfItem.Add(listItem);
            }

            if (selectValue != String.Empty)
                selectListOfItem.SetSelectValue(selectValue);

            return selectListOfItem;
        }
    }
}