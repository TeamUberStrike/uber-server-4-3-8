using System.Threading;
using System.Web;
using System.Web.Caching;

namespace Cmune.DataCenter.Utils.Caching
{
    internal static class CacheInit
    {
        /// <summary>
        /// Current Http runtime
        /// </summary>
        private static HttpRuntime runtime;

        /// <summary>
        /// Cache Obj
        /// </summary>
        private static Cache cache;

        /// <summary>
        /// Initializes static members of the CacheInit class. 
        /// </summary>
        static CacheInit()
        {
            CacheInit.EnsureHttpRuntime();
        }

        /// <summary>
        /// Gets objet cache
        /// </summary>
        public static Cache Cache
        {
            get { return cache; }
        }

        /// <summary>
        /// Obj Cache Init
        /// </summary>
        private static void EnsureHttpRuntime()
        {
            //if (Config.activeCache)
            //{
                if (HttpContext.Current != null)
                {
                    cache = HttpContext.Current.Cache;
                }
                else if (null == cache)
                {
                    try
                    {
                        Monitor.Enter(typeof(CacheInit));
                        if (null == cache)
                        {
                            // Create an Http Content to give us access to the cache.
                            runtime = new HttpRuntime();
                            cache = HttpRuntime.Cache;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(typeof(CacheInit));
                    }
                }
            //}
        }
    }
}
