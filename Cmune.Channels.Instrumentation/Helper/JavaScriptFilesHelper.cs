using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class JavaScriptFilesHelper
    {
        public static string ServersMonitoringJsFile(this UrlHelper url)
        {
            return url.Content("~/Scripts/js/serversMonitoring.js");
        }

        public static string MapJsFile(this UrlHelper url)
        {
            return url.Content("~/Scripts/js/map.js");
        }
    }
}