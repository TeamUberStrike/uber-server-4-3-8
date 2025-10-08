using System;
using System.Net;
using Cmune.DataCenter.Common.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Invalidates the Cmune caches in the Web Services
    /// </summary>
    public class CmuneCacheInvalidation
    {
        public const string Secret = "15cd4a45dxsdcf";

        public const string UrlWcfSuffix = "Cmune.svc/CacheInvalidation/InvalidateCache?name={0}&secret={1}";
        public const string UrlGeneratedWcfSuffix = "Uberstrike.DataCenter.WebService.ServerWebService.svc/InvalidateCache?name={0}&secret={1}";
        public const string UrlWebSiteSuffix = "CacheInvalidation.aspx?name={0}&secret={1}";
        public const string UrlWebSiteMvcSuffix = "CacheInvalidation?name={0}&secret={1}";

        public BuildType Build { get; set; }
        public string CacheName { get; set; }
        public Dictionary<int, Dictionary<BuildType, List<string>>> ServicesUrl { get; set; }

        public CmuneCacheInvalidation(BuildType build, string cacheName, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrl)
        {
            this.Build = build;
            this.CacheName = cacheName;
            this.ServicesUrl = servicesUrl;
        }

        /// <summary>
        /// Performs a HTTP GET on the url to invalidate the cache
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string InvalidateCache(string url)
        {
            string response = String.Empty;

            if (!url.IsNullOrFullyEmpty())
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        response = client.DownloadString(url);
                    }
                }
                catch (Exception ex)
                {
                    CmuneLog.LogException(ex, url);
                }
            }

            return response;
        }

        /// <summary>
        /// Invalidates the specific cache for the all the services (WCF and websites)
        /// </summary>
        public void InvalidateServices()
        {
            List<int> servicesId = ServicesUrl.Keys.ToList();

            foreach (int serviceId in servicesId)
            {
                InvalidateService(serviceId);
            }
        }

        /// <summary>
        /// Invalidates the specific cache for the a service
        /// </summary>
        /// <param name="serviceId"></param>
        public void InvalidateService(int serviceId)
        {
            List<string> urlTemplates = new List<string>();

            bool isServiceExisting = ServicesUrl.ContainsKey(serviceId);

            if (isServiceExisting)
            {
                bool isBuildExisting = ServicesUrl[serviceId].TryGetValue(Build, out urlTemplates);

                if (isBuildExisting)
                {
                    foreach (string urlTemplate in urlTemplates)
                    {
                        string url = GetServicelUrl(urlTemplate);
                        string response = InvalidateCache(url);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the service cache invalidation url
        /// </summary>
        /// <param name="urlTemplate"></param>
        /// <returns></returns>
        public string GetServicelUrl(string urlTemplate)
        {
            urlTemplate = String.Format(urlTemplate, Uri.EscapeDataString(CacheName), Uri.EscapeDataString(CmuneCacheInvalidation.Secret));

            return urlTemplate;
        }

        /// <summary>
        /// Launch a thread and invalidate the cache for all services
        /// </summary>
        /// <param name="build"></param>
        /// <param name="cacheName"></param>
        /// <param name="threadName"></param>
        /// <param name="servicesUrl"></param>
        public static void InvalidateHelper(BuildType build, string cacheName, string threadName, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrl)
        {
            CmuneCacheInvalidation cacheInvalidation = new CmuneCacheInvalidation(build, cacheName, servicesUrl);

            Thread invalidationThread = new Thread(cacheInvalidation.InvalidateServices);
            invalidationThread.Name = threadName;
            invalidationThread.Start();
        }
    }
}