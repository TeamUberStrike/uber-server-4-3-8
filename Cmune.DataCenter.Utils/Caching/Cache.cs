using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Caching;

namespace Cmune.DataCenter.Utils.Caching
{
    public class Cache<T>
    {
        /// <summary>
        /// Key to identify the cache
        /// </summary>
        private string key;

        /// <summary>
        /// Cache value
        /// </summary>
        private T val;

        /// <summary>
        /// Is the value persistant after the httpcontext
        /// </summary>
        private bool persist;

        /// <summary>
        /// Initializes a new instance of the Cache class.
        /// </summary>
        /// <param name="key">Clef du cache (Par convention on prendre NomDeLObjet.NomDeLaMethode_valeurparametre1_valeurparametre2_...)</param>
        public Cache(string key)
        {
            this.InitCache(key, true);
        }

        /// <summary>
        /// Initializes a new instance of the Cache class
        /// </summary>
        /// <param name="key">Cache key (ObjName.FunctionName.Parameter1.Parameter2...)</param>
        /// <param name="persist">Is the data persistant after the http context</param>
        public Cache(string key, bool persist)
        {
            this.InitCache(key, persist);
        }

        /// <summary>
        /// Gets renvois la valeur du cache
        /// </summary>
        public T Val
        {
            get { return this.val; }
        }

        /// <summary>
        /// Is the value still in the cache?
        /// </summary>
        public bool IsInCache
        {
            get { return System.Collections.Comparer.Default.Compare(this.val, default(T)) != 0; }
        }

        /// <summary>
        /// Is the value persistant after the httpcontext
        /// </summary>
        public bool Persist
        {
            get { return this.persist; }
        }

        /// <summary>
        /// 
        /// </summary>
        private IDictionary ItemsObj
        {
            get { return HttpContext.Current == null ? null : HttpContext.Current.Items; }
        }

        /// <summary>
        /// Gets the cache obj
        /// </summary>
        private Cache CacheObj
        {
            get { return CacheInit.Cache; }
        }

        /// <summary>
        /// Insert a value with a predefined expiration datetime
        /// If non persistant data, it will expire after the http connection
        /// </summary>
        /// <param name="val">Value to insert</param>
        /// <param name="absoluteExpiration">Expiration Date</param>
        public void Insert(T val, DateTime absoluteExpiration)
        {
            if (ConfigurationManager.AppSettings["ActivateCache"] != null && ConfigurationManager.AppSettings["ActivateCache"].Equals("1"))
            {
                this.val = val;
                if (this.persist)
                {
                    if (this.CacheObj != null && System.Collections.Comparer.Default.Compare(val, default(T)) != 0)
                    {
                        this.CacheObj.Insert(this.key, val, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }
                else
                {
                    if (this.ItemsObj != null && System.Collections.Comparer.Default.Compare(val, default(T)) != 0)
                    {
                        this.ItemsObj[this.key] = val;
                    }
                }
            }
        }

        /// <summary>
        /// Insert a value with a predefined expiration time
        /// If non persistant data, it will expire after the http connection
        /// </summary>
        /// <param name="val">Value to insert</param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void Insert(T val, int days, int hours, int minutes, int seconds)
        {
            this.Insert(val, DateTime.Now.AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds));
        }

        /// <summary>
        /// Insert a value into the cache, by default 1 day
        /// If non persistant data, it will expire after the http connection
        /// </summary>
        /// <param name="val">Value to insert</param>
        public void Insert(T val)
        {
            this.Insert(val, DateTime.Now.AddDays(1));
        }

        /// <summary>
        /// Insert a value into the cache, by default 1 hour
        /// If non persistant data, it will expire after the http connection
        /// </summary>
        /// <param name="val">Value to insert</param>
        public void InsertHour(T val)
        {
            this.Insert(val, DateTime.Now.AddHours(1));
        }

        /// <summary>
        /// Delete the value from the cache
        /// </summary>
        public void Remove()
        {
            if (this.persist)
            {
                if (this.CacheObj != null)
                {
                    this.CacheObj.Remove(this.key);
                }
            }
            else
            {
                if (this.ItemsObj != null)
                {
                    this.ItemsObj.Remove(this.key);
                }
            }

            this.val = default(T);
        }


        public static void RemoveAll()
        {
            IDictionaryEnumerator removeCache = CacheInit.Cache.GetEnumerator();
            while(removeCache.MoveNext())
            {
                CacheInit.Cache.Remove(removeCache.Key.ToString());
            }
        }

        /// <summary>
        /// Init the cache Obj
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="persists">is cache still alive after the http connection?</param>
        private void InitCache(string key, bool persists)
        {
            this.key = key;
            this.persist = persists;
            if (this.persist)
            {
                if (this.CacheObj != null && this.CacheObj[key] != null)
                {
                    this.val = (T)this.CacheObj[key];
                }
                else
                {
                    this.val = default(T);
                }
            }
            else
            {
                if (this.ItemsObj != null && this.ItemsObj[key] != null)
                {
                    this.val = (T)this.ItemsObj[key];
                }
                else
                {
                    this.val = default(T);
                }
            }
        }
    }
}
