using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Helpers;

namespace UberStrike.Channels.Common.Helpers
{
    public static class CommonMediaHelper
    {
        public static HtmlString CommonCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.Action("CacheContent", "Cache") + "/CommonCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString CommonFacebookFrameCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.Action("CacheContent", "Cache") + "/CommonFacebookFrameCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString CommonPortalFrameCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.Action("CacheContent", "Cache") + "/CommonPortalFrameCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString CommonKongregateFrameCssFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.CssTagStart() + url.Action("CacheContent", "Cache") + "/CommonKongregateFrameCss/" + MediaHelper.GetPublishVersion() + "/css" + html.CssTagEnd());
        }

        public static HtmlString CommonChannelJavaScriptFile(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(html.JavaScriptTagStart() + url.Action("CacheContent", "Cache")  + "/CommonJavaScript/" + MediaHelper.GetPublishVersion() + "/javascript" + html.JavaScriptTagEnd());
        }
    }
}