using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using System.Web.Caching;

namespace UberStrike.Channels.Common.Business
{
    public class ItemCache
    {
        #region Properties

        Dictionary<int, string> ItemsName { get; set; }

        #endregion

        #region Constructors

        public ItemCache(int applicationId, bool getDeprecatedItems = false)
        {
            ItemsName = new Dictionary<int, string>();

            string cacheName = String.Format("CmuneItem.GetItemNames.{0}.{1}", applicationId, getDeprecatedItems);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                ItemsName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                ItemsName = CmuneItem.GetItemNames(applicationId, getDeprecatedItems);
                HttpRuntime.Cache.Add(cacheName, ItemsName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
        }

        public string GetItemName(int itemId)
        {
            string itemName = "Non existing item";

            if (ItemsName != null && ItemsName.ContainsKey(itemId))
            {
                itemName = ItemsName[itemId];
            }

            return itemName;
        }

        #endregion
    }
}
