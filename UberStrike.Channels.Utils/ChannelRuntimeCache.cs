using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Cmune.DataCenter.Utils;
using System.Web.Caching;

namespace UberStrike.Channels.Utils
{
    public static class ChannelRuntimeCache
    {
        public static bool RetrieveFromCache<T>(string cacheKey, ref T anObject)
        {
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                try
                {
                    anObject = (T)HttpRuntime.Cache[cacheKey];
                    return true;
                }
                catch (FormatException e)
                {
                    CmuneLog.LogException(e, "cached item can not be converted to the given type");
                }
            }
            return false;
        }

        public static void AddToCache<T>(string cacheKey, T anObject, DateTime expirationDate)
        {
            HttpRuntime.Cache.Add(cacheKey, anObject, null, expirationDate, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }
    }
}
