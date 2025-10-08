using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Helpers;

namespace UberStrike.Channels.Portal.Extensions
{
    public static class PortalMediaHelper
    {

        public static HtmlString PortalChannelCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/PortalCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString PortalChannelJavaScriptFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.JavaScriptTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/PortalJavaScript/" + MediaHelper.GetPublishVersion() + "/javascript" + html.JavaScriptTagEnd());
        }
    }
}
