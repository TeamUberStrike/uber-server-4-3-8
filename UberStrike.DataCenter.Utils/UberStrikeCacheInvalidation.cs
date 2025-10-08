using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Common.Entities;
using System.Threading;
using System.Linq;

namespace UberStrike.DataCenter.Utils
{
    /// <summary>
    /// Invalidates the Uberstrike caches in the Web Services and websites
    /// </summary>
    public class UberStrikeCacheInvalidation : CmuneCacheInvalidation
    {
        /// <summary>
        /// Cache key: shop
        /// </summary>
        public const string CacheNameItemShop = "itemshop";
        /// <summary>
        /// Cache key: Deprecated application version
        /// </summary>
        public const string CacheNameDeprecatedApplicationVersion = "deprecatedapplicationversion";
        /// <summary>
        /// Cache key: application version
        /// </summary>
        public const string CacheNameApplicationVersion = "applicationversion";
        /// <summary>
        /// Cache key: Scoring
        /// </summary>
        public const string CacheNameScoring = "scoring";
        /// <summary>
        /// Cache key: referrer source
        /// </summary>
        public const string CacheNameReferrerSource = "referrer";
        /// <summary>
        /// Cache key: Credit packs
        /// </summary>
        public const string CacheNameBundles = "bundles";

        private const string UrlWcfDev100 = "http://ws.dev.uberstrike.com/1.0.0/" + UrlGeneratedWcfSuffix;
        private const string UrlWcfQa100 = "http://ws.qa.uberstrike.com/1.0.0/" + UrlGeneratedWcfSuffix;
        private const string UrlWcfProd437 = "http://ws.uberstrike.cmune.com/4.3.7/" + UrlGeneratedWcfSuffix;
        private const string UrlWcfProd100 = "http://ws.uberstrike.cmune.com/1.0.0/" + UrlGeneratedWcfSuffix;

        private const string UrlCommonDevPortal = "http://dev.uberstrike.com/CommonChannel/" + UrlWebSiteMvcSuffix;
        private const string UrlCommonStagingPortal = "http://qa.uberstrike.com/CommonChannel/" + UrlWebSiteMvcSuffix;
        private const string UrlCommonProdPortal = "http://uberstrike.cmune.com/CommonChannel/" + UrlWebSiteMvcSuffix;

        private const string UrlCommonDevFacebook = "http://fb.dev.uberstrike.com/CommonChannel/" + UrlWebSiteMvcSuffix;
        private const string UrlCommonStagingFacebook = "http://fb.qa.uberstrike.com/CommonChannel/" + UrlWebSiteMvcSuffix;
        private const string UrlCommonProdFacebook = "http://fb.uberstrike.com/CommonChannel/" + UrlWebSiteMvcSuffix;

        private const string UrlPortalDev = "http://dev.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlPortalStaging = "http://qa.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlPortalProd = "http://uberstrike.cmune.com/" + UrlWebSiteMvcSuffix;

        private static readonly string UrlFacebookDev = "http://fb.dev.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlFacebookStaging = "http://fb.qa.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlFacebookProd = "http://fb.uberstrike.com/" + UrlWebSiteMvcSuffix;

        private static string UrlKongregateDev = "http://kg.dev.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private static string UrlKongregateStaging = "http://kg.qa.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private static string UrlKongregateProd = "http://kg.uberstrike.com/" + UrlWebSiteMvcSuffix;

        private const int WebsiteCommon = 0;
        private const int WebsitePortal = 1;
        private const int WebsiteFacebook = 2;
        private const int WebsiteKongregate = 3;
        private const int Wcf = 4;

        /// <summary>
        /// Services Urls
        /// </summary>
        public static readonly Dictionary<int, Dictionary<BuildType, List<string>>> UberStrikeServicesUrls = new Dictionary<int, Dictionary<BuildType, List<string>>>
        {
            { WebsiteCommon , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string>{ UrlCommonDevPortal, UrlCommonDevFacebook} }, {BuildType.Staging, new List<string>{UrlCommonStagingPortal, UrlCommonStagingFacebook}}, {BuildType.Prod, new List<string>{UrlCommonProdPortal, UrlCommonProdFacebook}} }}, 
            { WebsitePortal , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string>{ UrlPortalDev}}, {BuildType.Staging, new List<string>{ UrlPortalStaging}}, {BuildType.Prod, new List<string>{ UrlPortalProd}} }}, 
            { WebsiteFacebook , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string>{ UrlFacebookDev}}, {BuildType.Staging, new List<string>{ UrlFacebookStaging}}, {BuildType.Prod, new List<string>{ UrlFacebookProd}} }}, 
            { WebsiteKongregate, new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string>{ UrlKongregateDev}}, {BuildType.Staging, new List<string>{ UrlKongregateStaging}}, {BuildType.Prod, new List<string>{ UrlKongregateProd}} }}, 
            { Wcf , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string>{ UrlWcfDev100}}, {BuildType.Staging, new List<string>{ UrlWcfQa100}}, {BuildType.Prod, new List<string>{ UrlWcfProd437, UrlWcfProd100}} }}
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="build"></param>
        /// <param name="cacheName"></param>
        /// <param name="servicesUrl"></param>
        public UberStrikeCacheInvalidation(BuildType build, string cacheName, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrl)
            : base(build, cacheName, servicesUrl)
        {
        }

        /// <summary>
        /// Invalidate the cache on the local machine
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static bool InvalidateLocalCache(string cacheName, string secret)
        {
            bool isCacheInvalidated = false;

            if (!cacheName.IsNullOrFullyEmpty() && !secret.IsNullOrFullyEmpty() && secret.Equals(CmuneCacheInvalidation.Secret))
            {
                cacheName = cacheName.ToLower();
                int source = ConfigurationUtilities.ReadConfigurationManagerInt("NLogSource");

                switch (cacheName)
                {
                    case UberStrikeCacheInvalidation.CacheNameItemShop:
                        List<string> shopKeysToRemove = new List<string>();

                        IDictionaryEnumerator shopCacheEnumerator = HttpRuntime.Cache.GetEnumerator();
                        shopCacheEnumerator.Reset();

                        while (shopCacheEnumerator.MoveNext())
                        {
                            string key = (string)shopCacheEnumerator.Key;

                            if (key.StartsWith(UberStrikeCacheKeys.ShopFunctionalConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopGearConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopQuickUseConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopSpecialConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopWeaponModConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopWeaponConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.ShopWeaponConfigParameters))
                            {
                                shopKeysToRemove.Add(key);
                            }
                        }

                        foreach (string key in shopKeysToRemove)
                        {
                            HttpRuntime.Cache.Remove(key);
                        }

                        HttpRuntime.Cache.Remove(UberStrikeCacheKeys.ShopView);

                        isCacheInvalidated = true;

                        CmuneLog.CustomLogToDefaultPath("CacheInvalidation.US.Shop." + source.ToString() + ".log", "[Expirated keys:" + CmuneLog.DisplayForLog(shopKeysToRemove.Count, 3) + "]");
                        break;
                    case UberStrikeCacheInvalidation.CacheNameDeprecatedApplicationVersion:
                        // This one is more tricky as there is more than one value in our cache

                        List<string> deprecatedApplicationVersionKeysToRemove = new List<string>();

                        IDictionaryEnumerator deprecatedApplicationVersionCacheEnumerator = HttpRuntime.Cache.GetEnumerator();
                        deprecatedApplicationVersionCacheEnumerator.Reset();

                        while (deprecatedApplicationVersionCacheEnumerator.MoveNext())
                        {
                            string key = (string)deprecatedApplicationVersionCacheEnumerator.Key;

                            if (key.StartsWith(UberStrikeCacheKeys.CheckDeprecatedApplicationVersionViewParameters))
                            {
                                deprecatedApplicationVersionKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.GetDeprecatedPhotonServersViewParameters))
                            {
                                deprecatedApplicationVersionKeysToRemove.Add(key);
                            }
                        }

                        foreach (string key in deprecatedApplicationVersionKeysToRemove)
                        {
                            HttpRuntime.Cache.Remove(key);
                        }

                        CmuneLog.CustomLogToDefaultPath("CacheInvalidation.US.DeprecatedApplicationVersion." + source.ToString() + ".log", "[Expirated keys:" + CmuneLog.DisplayForLog(deprecatedApplicationVersionKeysToRemove.Count, 3) + "]");

                        isCacheInvalidated = true;
                        break;
                    case UberStrikeCacheInvalidation.CacheNameApplicationVersion:
                        // This one is more tricky as there is more than one value in our cache

                        List<string> applicationVersionKeysToRemove = new List<string>();

                        IDictionaryEnumerator applicationVersionCacheEnumerator = HttpRuntime.Cache.GetEnumerator();
                        applicationVersionCacheEnumerator.Reset();

                        while (applicationVersionCacheEnumerator.MoveNext())
                        {
                            string key = (string)applicationVersionCacheEnumerator.Key;

                            if (key.StartsWith(UberStrikeCacheKeys.CheckApplicationVersionViewParameters))
                            {
                                applicationVersionKeysToRemove.Add(key);
                            }
                            else if (key.StartsWith(UberStrikeCacheKeys.GetPhotonServersViewParameters))
                            {
                                applicationVersionKeysToRemove.Add(key);
                            }
                        }

                        foreach (string key in applicationVersionKeysToRemove)
                        {
                            HttpRuntime.Cache.Remove(key);
                        }

                        CmuneLog.CustomLogToDefaultPath("CacheInvalidation.US.ApplicationVersion." + source.ToString() + ".log", "[Expirated keys:" + CmuneLog.DisplayForLog(applicationVersionKeysToRemove.Count, 3) + "]");

                        isCacheInvalidated = true;
                        break;
                    case UberStrikeCacheInvalidation.CacheNameScoring:
                        HttpRuntime.Cache.Remove(UberStrikeCacheKeys.XPEvents);
                        HttpRuntime.Cache.Remove(UberStrikeCacheKeys.LevelCaps);

                        isCacheInvalidated = true;

                        CmuneLog.CustomLogToDefaultPath("CacheInvalidation.US.Scoring." + source.ToString() + ".log", String.Empty);
                        break;
                    case UberStrikeCacheInvalidation.CacheNameReferrerSource:
                        HttpRuntime.Cache.Remove(UberStrikeCacheKeys.ReferrerSource);

                        isCacheInvalidated = true;

                        CmuneLog.CustomLogToDefaultPath("CacheInvalidation.US.Referrers." + source.ToString() + ".log", String.Empty);
                        break;

                    case UberStrikeCacheInvalidation.CacheNameBundles:

                        List<string> bundleKeysToRemove = new List<string>();

                        IDictionaryEnumerator bundleCacheEnumerator = HttpRuntime.Cache.GetEnumerator();
                        bundleCacheEnumerator.Reset();

                        while (bundleCacheEnumerator.MoveNext())
                        {
                            string key = (string)bundleCacheEnumerator.Key;

                            if (key.StartsWith(UberStrikeCacheKeys.Bundles))
                            {
                                bundleKeysToRemove.Add(key);
                            }
                        }

                        foreach (string key in bundleKeysToRemove)
                        {
                            HttpRuntime.Cache.Remove(key);
                        }

                        CmuneLog.CustomLogToDefaultPath(String.Format("CacheInvalidation.US.Bundles.{0}.log", source), String.Format("[Expirated keys:{0}]", bundleKeysToRemove.Count.ToString().PadLeft(3)));

                        isCacheInvalidated = true;
                        break;
                    default:
                        List<string> keysToRemove = new List<string>();

                        IDictionaryEnumerator keysToRemoveEnumerator = HttpRuntime.Cache.GetEnumerator();
                        keysToRemoveEnumerator.Reset();

                        while (keysToRemoveEnumerator.MoveNext())
                        {
                            string key = (string)keysToRemoveEnumerator.Key;

                            if (key.StartsWith(cacheName))
                            {
                                keysToRemove.Add(key);
                            }
                        }

                        foreach (string key in keysToRemove)
                        {
                            HttpRuntime.Cache.Remove(key);
                        }

                        CmuneLog.CustomLogToDefaultPath(String.Format("CacheInvalidation.US.OtherKeys.{0}.log", source), String.Format("[Expirated keys:{0}]", keysToRemove.Count.ToString().PadLeft(3)));

                        isCacheInvalidated = true;
                        break;
                }
            }

            return isCacheInvalidated;
        }

        /// <summary>
        /// Invalidates the referrer sources for all the Web Services and websites
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        public static void InvalidateReferrers(BuildType build)
        {
            InvalidateHelper(build, CacheNameReferrerSource, "US.ReferrersInvalidationThread", UberStrikeServicesUrls);
        }

        /// <summary>
        /// Invalidates the referrer sources for all the Web Services and websites
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        /// <param name="servicesUrls"></param>
        public static void InvalidateReferrers(BuildType build, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrls)
        {
            InvalidateHelper(build, CacheNameReferrerSource, "US.ReferrersInvalidationThread", servicesUrls);
        }

        /// <summary>
        /// Invalidates the XP events and level caps for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        public static void InvalidateScoring(BuildType build)
        {
            InvalidateHelper(build, CacheNameScoring, "US.ScoringInvalidationThread", UberStrikeServicesUrls);
        }

        /// <summary>
        /// Invalidates the XP events and level caps for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        /// <param name="servicesUrls"></param>
        public static void InvalidateScoring(BuildType build, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrls)
        {
            InvalidateHelper(build, CacheNameScoring, "US.ScoringInvalidationThread", servicesUrls);
        }

        /// <summary>
        /// Invalidates the shop for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        public static void InvalidateItemShop(BuildType build)
        {
            InvalidateHelper(build, CacheNameItemShop, "US.ItemShopInvalidationThread", UberStrikeServicesUrls);
        }

        /// <summary>
        /// Invalidates the shop for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        /// <param name="servicesUrls"></param>
        public static void InvalidateItemShop(BuildType build, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrls)
        {
            InvalidateHelper(build, CacheNameItemShop, "US.ItemShopInvalidationThread", servicesUrls);
        }

        /// <summary>
        /// Invalidates the application for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        public static void InvalidateApplicationVersion(BuildType build)
        {
            InvalidateHelper(build, CacheNameApplicationVersion, "US.ApplicationVersionInvalidationThread", UberStrikeServicesUrls);
            InvalidateHelper(build, CacheNameDeprecatedApplicationVersion, "US.DeprecatedApplicationVersionInvalidationThread", UberStrikeServicesUrls);
        }

        /// <summary>
        /// Invalidates the application for all the Web Services and channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        /// <param name="servicesUrls"></param>
        public static void InvalidateApplicationVersion(BuildType build, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrls)
        {
            InvalidateHelper(build, CacheNameApplicationVersion, "US.ApplicationVersionInvalidationThread", servicesUrls);
            InvalidateHelper(build, CacheNameDeprecatedApplicationVersion, "US.DeprecatedApplicationVersionInvalidationThread", servicesUrls);
        }

        /// <summary>
        /// Invalidates the credit packs and bundles for all channels
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        public static void InvalidateBundles(BuildType build)
        {
            InvalidateHelper(build, CacheNameBundles, "US.BundleInvalidationThread", UberStrikeServicesUrls);
        }

        /// <summary>
        /// Invalidates the other cache
        /// Launch a thread
        /// </summary>
        /// <param name="build"></param>
        /// <param name="cacheName"></param>
        public static void InvalidateOtherCache(BuildType build, string cacheName)
        {
            InvalidateHelper(build, cacheName, "US.OtherCacheInvalidationThread", UberStrikeServicesUrls);
        }
    }
}