using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Business;
using System.Web.Caching;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class ItemService
    {
        /// <summary>
        /// Loads the item types
        /// </summary>
        /// <returns></returns>
        public static List<ItemType> LoadItemType()
        {
            List<ItemType> itemType = new List<ItemType>();

            if (HttpRuntime.Cache[ConfigurationUtilities.ReadConfigurationManager("AspCacheItemType")] != null)
            {
                itemType = (List<ItemType>)HttpRuntime.Cache[ConfigurationUtilities.ReadConfigurationManager("AspCacheItemType")];
            }
            else
            {
                itemType = CmuneItem.GetItemType();
                HttpRuntime.Cache.Add(ConfigurationUtilities.ReadConfigurationManager("AspCacheItemType"), itemType, null, Cache.NoAbsoluteExpiration, new TimeSpan(365, 0, 0, 0, 0), CacheItemPriority.NotRemovable, null);
            }

            return itemType;
        }

        /// <summary>
        /// Loads the item class
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ItemClass>> LoadItemClassOrdered()
        {
            Dictionary<int, List<ItemClass>> itemClassOrdered = new Dictionary<int, List<ItemClass>>();

            if (HttpRuntime.Cache[ConfigurationUtilities.ReadConfigurationManager("AspCacheItemClass")] != null)
            {
                itemClassOrdered = (Dictionary<int, List<ItemClass>>)HttpRuntime.Cache[ConfigurationUtilities.ReadConfigurationManager("AspCacheItemClass")];
            }
            else
            {
                itemClassOrdered = CmuneItem.GetItemClassOrdered();
                HttpRuntime.Cache.Add(ConfigurationUtilities.ReadConfigurationManager("AspCacheItemClass"), itemClassOrdered, null, Cache.NoAbsoluteExpiration, new TimeSpan(365, 0, 0, 0, 0), CacheItemPriority.NotRemovable, null);
            }

            return itemClassOrdered;
        }

        /// <summary>
        /// Loads the item class
        /// </summary>
        /// <returns></returns>
        public static List<ItemClass> LoadItemClass()
        {
            List<ItemClass> itemClass = new List<ItemClass>();

            string cacheName = String.Format("CmuneItem.GetItemClass");

            if (HttpRuntime.Cache[cacheName] != null)
            {
                itemClass = (List<ItemClass>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                itemClass = CmuneItem.GetItemClass();
                HttpRuntime.Cache.Add(cacheName, itemClass, null, Cache.NoAbsoluteExpiration, new TimeSpan(365, 0, 0, 0, 0), CacheItemPriority.NotRemovable, null);
            }

            return itemClass;
        }
    }
}