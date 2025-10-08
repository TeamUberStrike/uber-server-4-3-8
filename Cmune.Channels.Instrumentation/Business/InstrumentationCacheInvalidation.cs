using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Business
{
    /// <summary>
    /// Invalidates the Instrumentation caches in the Web Services and websites
    /// </summary>
    public class InstrumentationCacheInvalidation : CmuneCacheInvalidation
    {
        private const string UrlWcfDev100 = "http://ws.instrumentation.dev.uberstrike.com/1.0.0/" + UrlWcfSuffix;
        private const string UrlWcfStaging100 = "http://ws.instrumentation.qa.uberstrike.com/1.0.0/" + UrlWcfSuffix;
        private const string UrlWcfProd437 = "http://ws.admin.cmune.com/4.3.7/" + UrlWcfSuffix;
        private const string UrlWcfProd100 = "http://ws.admin.cmune.com/1.0.0/" + UrlWcfSuffix;

        private const string UrlInstrumentationDev = "http://instrumentation.dev.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlInstrumentationStaging = "http://instrumentation.qa.uberstrike.com/" + UrlWebSiteMvcSuffix;
        private const string UrlInstrumentationProd = "http://instrumentation.cmune.com/" + UrlWebSiteMvcSuffix;

        private const int WebsiteInstrumentation = 0;
        private const int Wcf = 1;

        /// <summary>
        /// Services URLs
        /// </summary>
        public static readonly Dictionary<int, Dictionary<BuildType, List<string>>> InstrumentationServicesUrls = new Dictionary<int, Dictionary<BuildType, List<string>>>
        {
            { WebsiteInstrumentation , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string> { UrlInstrumentationDev}}, {BuildType.Staging, new List<string> { UrlInstrumentationStaging}}, {BuildType.Prod, new List<string> { UrlInstrumentationProd}} }}, 
            { Wcf , new Dictionary<BuildType, List<string>> { {BuildType.Dev, new List<string> { UrlWcfDev100}}, {BuildType.Staging, new List<string> { UrlWcfStaging100}}, {BuildType.Prod, new List<string> { UrlWcfProd437, UrlWcfProd100}} }}
        };

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="build"></param>
        /// <param name="cacheName"></param>
        /// <param name="servicesUrls"></param>
        public InstrumentationCacheInvalidation(BuildType build, string cacheName, Dictionary<int, Dictionary<BuildType, List<string>>> servicesUrls)
            : base(build, cacheName, servicesUrls)
        {
        }
    }
}