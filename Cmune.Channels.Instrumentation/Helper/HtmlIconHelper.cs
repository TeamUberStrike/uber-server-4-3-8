using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class HtmlIconHelper
    {
        private static string setAdditionalAttributes(string alt = "", string title = "")
        {
            var attributes = string.Empty;

            if (!string.IsNullOrEmpty(alt))
            {
                attributes += " alt=\"" + alt + "\" ";
            }
            if (!string.IsNullOrEmpty(title))
            {
                attributes += " title=\"" + title + "\" ";
            }

            return attributes;
        }

        public static HtmlString TrueNotif(this HtmlHelper htmlHelper, UrlHelper url, string alt = "", string title = "")
        {
            return new HtmlString("<img src=\"" + url.Content("~/Content/img/s_success.png") + "\" " + setAdditionalAttributes(alt, title) + "/>");
        }

        public static HtmlString FalseNotif(this HtmlHelper htmlHelper, UrlHelper url, string alt = "", string title = "")
        {
            return new HtmlString("<img src=\"" + url.Content("~/Content/img/b_false.png") + "\"" + setAdditionalAttributes(alt, title) + "/>");
        }

        public static HtmlString WarningNotif(this HtmlHelper htmlHelper, UrlHelper url, string alt = "", string title = "")
        {
            return new HtmlString("<img src=\"" + url.Content("~/Content/img/s_notice.png") + "\" " + setAdditionalAttributes(alt, title) + "/>");
        }

        public static HtmlString EditImage(this HtmlHelper htmlHelper, UrlHelper url, string alt = "", string title = "")
        {
            return new HtmlString("<img src=\"" + url.Content("~/Content/img/b_edit.png") + "\" " + setAdditionalAttributes(alt, title) + "/>");
        }

        public static HtmlString DeleteImage(this HtmlHelper htmlHelper, UrlHelper url, string alt = "", string title = "")
        {
            return new HtmlString("<img src=\"" + url.Content("~/Content/img/b_drop.png") + "\" " + setAdditionalAttributes(alt, title) + "/>");
        }

        public static HtmlString IconNotif(this HtmlHelper htmlHelper, UrlHelper url, bool value, string alt = "", string title = "")
        {
            if (value)
                return htmlHelper.TrueNotif(url);
            return htmlHelper.FalseNotif(url);
        }
    }
}