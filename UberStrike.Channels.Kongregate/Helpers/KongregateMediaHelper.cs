using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Helpers;

namespace UberStrike.Channels.Kongregate.Helpers
{
    public static class KongregateMediaHelper
    {
        public static HtmlString KongregateChannelCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/KongregateCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString KongregateChannelJavaScriptFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.JavaScriptTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/KongregateJavaScript/" + MediaHelper.GetPublishVersion() + "/javascript" + html.JavaScriptTagEnd());
        }
    }
}