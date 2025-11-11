using System;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.Channels.Portal.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {
            ViewBag.WindowsStandAloneUrl = BuildWindowsStandaloneUrl();
            ViewBag.IsWindows = Request.UserAgent.Contains("Windows");
            ViewBag.MacAppStoreLink = ConfigurationUtilities.ReadConfigurationManager("MacAppStoreLink");

            return View();
        }

        private static string BuildWindowsStandaloneUrl()
        {
            StringBuilder url = new StringBuilder();

            if (!ConfigurationUtilities.ReadConfigurationManager("WindowsStandaloneUrl").IsNullOrFullyEmpty())
            {
                // We can override the application version from the DB with the Web.config
                url.Append(ConfigurationUtilities.ReadConfigurationManager("WindowsStandaloneUrl"));
            }
            else
            {
                string cacheName = String.Format("{0}{1}{2}", UberStrikeCacheKeys.CheckApplicationVersionViewParameters, CmuneCacheKeys.Separator, (int)ChannelType.WindowsStandalone);
                ApplicationVersion buildVersion = null;

                if (HttpRuntime.Cache[cacheName] != null)
                {
                    buildVersion = (ApplicationVersion)HttpRuntime.Cache[cacheName];
                }
                else
                {
                    buildVersion = Games.GetCurrentApplicationVersion(ChannelType.WindowsStandalone);
                    HttpRuntime.Cache.Add(cacheName, buildVersion, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }

                if (buildVersion != null)
                {
                    url.Append(buildVersion.WebPlayerFileName);
                }
                else
                {
                    throw new NullReferenceException("No version is existing for channel: " + ChannelType.WindowsStandalone);
                }
            }

            return url.ToString();
        }

    }
}
