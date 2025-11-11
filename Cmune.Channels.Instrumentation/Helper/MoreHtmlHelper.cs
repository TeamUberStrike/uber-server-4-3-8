using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class MoreHtmlHelper
    {
        public static string TableTh(this HtmlHelper helper, string html)
        {
            return "<th>" + html + "</th>";
        }
        public static string TableTd(this HtmlHelper helper, string html)
        {
            return "<td>" + html + "</td>";
        }

        public static string Image(this UrlHelper url, string localeImgPath = "")
        {
            return ConfigurationUtilities.ReadConfigurationManager("CommonImagesRoot") + localeImgPath;
        }
    }
}
