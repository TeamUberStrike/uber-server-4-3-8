using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Utils.Helpers;

namespace UberStrike.Channels.Facebook.Helpers
{
    public static class FacebookMediaHelper
    {

        public static HtmlString FacebookChannelCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/FacebookCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public  static HtmlString FacebookChannelJavaScriptFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.JavaScriptTagStart() + url.CommonChannelUrl() + "Cache/CacheContent/FacebookJavaScript/" + MediaHelper.GetPublishVersion() + "/javascript" + html.JavaScriptTagEnd());
        }
    }
}