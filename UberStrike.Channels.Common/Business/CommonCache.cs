using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Newtonsoft.Json;
using UberStrike.Channels.Common.Models;

namespace UberStrike.Channels.Common.Business
{
    static class CommonCache
    {

        public class KongregateItemData
        {
            public string api_key { get; set; }
        }

        public class KongregateItemDataReturn
        {
            public bool success;
            public string error;
            public string error_description;
            public KongregateItem kongregateItem;
        }

        public class KongregateItem
        {
            public int id;
            public string identifier;
            public string name;
            public string description;
            public int remaining_uses;
            public string data;
        }

        public static void CopyParentProperties(object parent, object child)
        {
            foreach (var parentProp in parent.GetType().GetProperties())
            {
                var cprop = child.GetType().GetProperties().Where(d => d.Name == parentProp.Name).FirstOrDefault();
                if (cprop != null)
                {
                    cprop.SetValue(child, parentProp.GetValue(parent, null), null);
                }
            }
        }

        public static List<KongregateBundleView> LoadKongregateCreditBundles()
        {
            var kongregateCreditBundles = new List<KongregateBundleView>();
            string itemsUrl = "http://www.kongregate.com/api/items.json";

            var kongregateItemData = new KongregateItemData();
            kongregateItemData.api_key = ConfigurationUtilities.ReadConfigurationManager("KongregateAppAPIKey");
            var webGetRequest = new WebGetRequest(itemsUrl, kongregateItemData);
            KongregateItemDataReturn kongregateItems = (KongregateItemDataReturn)JsonConvert.DeserializeObject(webGetRequest.GetResponse(), typeof(KongregateItemDataReturn));
            if (kongregateItems.success)
            {
                foreach (var bundle in LoadAllBundlesOnSales(ChannelType.Kongregate).Where(d => d.Credits > 0).ToList())
                {
                    var kongregateBundle = new KongregateBundleView();
                    CopyParentProperties(bundle, kongregateBundle);
                    kongregateCreditBundles.Add(kongregateBundle);
                }
            }

            return kongregateCreditBundles;
        }

        public static List<BundleView> LoadCreditBundles(ChannelType channel)
        {
            var allBundles = LoadAllBundlesOnSales(channel).Where(d => d.Credits > 0).ToList();
            return allBundles;
        }

        public static List<BundleView> LoadItemAndPointBundles(ChannelType channel)
        {
            var allBundles = LoadAllBundlesOnSales(channel).Where(d => d.BundleItemViews.Count > 0 || d.Points > 0).ToList();
            return allBundles;
        }

        private static List<BundleView> LoadAllBundlesOnSales(ChannelType channel)
        {
            string cacheKey = String.Format("{0}.{1}", UberStrikeCacheKeys.Bundles, (int)channel);

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<BundleView>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                var allBundles = CmuneBundle.GetAllBundlesOnSaleView(channel);
                HttpRuntime.Cache.Add(cacheKey, allBundles, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                return allBundles;
            }
        }

        //public static Dictionary<int, string> LoadItemNames()
        //{
        //    string cacheKey = String.Format("CmuneItem.GetItemNames");

        //    if (HttpRuntime.Cache[cacheKey] != null)
        //    {
        //        return (Dictionary<int, string>)HttpRuntime.Cache[cacheKey];
        //    }
        //    else
        //    {
        //        var itemNames = CmuneItem.GetItemNames(UberStrikeCommonConfig.ApplicationId);
        //        HttpRuntime.Cache.Add(cacheKey, itemNames, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
        //        return itemNames;
        //    }
        //}
    }
}