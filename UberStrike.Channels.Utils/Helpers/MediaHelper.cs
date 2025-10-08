using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class MediaHelper
    {
        public static string RequestProtocole()
        {
            return HttpContextFactory.Current.Request.Url.Scheme;
        }

        public static string GetPublishVersion()
        {
            if (ConfigurationUtilities.ReadConfigurationManager("Environment").ToLower() == "dev")
            {
                return RandomString.GenerateRandomString(8);
            }
            else
                return ConfigurationUtilities.ReadConfigurationManager("PublishVersion");
        }

        public static string CommonChannelUrl(this UrlHelper url)
        {
            return ConfigurationUtilities.ReadConfigurationManager("CommonChannel").Replace("{protocole}", RequestProtocole());
        }

        public static string Image(this UrlHelper url, string localeImgPath = "")
        {
            return ConfigurationUtilities.ReadConfigurationManager("ImagesRoot").Replace("{protocole}", RequestProtocole()) + localeImgPath + (localeImgPath.IsNullOrFullyEmpty() ? "" : "?p=" + GetPublishVersion());
        }

        private static string buildParameter(string paramName, string value)
        {
            if (value.IsNullOrFullyEmpty())
                return string.Empty;
            return " " + paramName + "=\""+value+"\" ";
        }

        public static HtmlString ImageTag(this HtmlHelper html, string url, string title = "", string alt = "", string style = "")
        {
            return new HtmlString(html.ImgStart() + html.imgSrc(url) + buildParameter("title", title) + buildParameter("alt", alt) + buildParameter("style", style) + html.ImgEnd());
        }

        public static HtmlString ImageTag(this HtmlHelper html, UrlHelper url, string img, string title = "", string alt = "", string style = "")
        {
            return new HtmlString(html.ImgStart() + html.imgSrc(url.Image(img)) + buildParameter("title", title) + buildParameter("alt", alt) + buildParameter("style", style) + html.ImgEnd());
        }

        public static string Video(this UrlHelper url, string localeVideoPath = "")
        {
            return ConfigurationUtilities.ReadConfigurationManager("VideoRoot") + localeVideoPath + (localeVideoPath.IsNullOrFullyEmpty() ? "" : "?p=" + GetPublishVersion());
        }

        public static string JavaScript(this UrlHelper url, string jsFileName)
        {
            return url.Content("~/Scripts/js/") + jsFileName + (jsFileName.IsNullOrFullyEmpty() ? "" : "?p=" + GetPublishVersion());
        }

        public static string JavaScriptLib(this UrlHelper url, string jsFileName)
        {
            return url.Content("~/Scripts/") + jsFileName + (jsFileName.IsNullOrFullyEmpty() ? "" : "?p=" + GetPublishVersion());
        }

        private static string imgSrc(this HtmlHelper html, string src)
        {
            return " src=\"" + src + "\" ";
        }   

        public static string ImgStart(this HtmlHelper html)
        {
            return "<img ";
        }

        public static string ImgEnd(this HtmlHelper html)
        {
            return " />";
        }

        public static string JavaScriptTagStart(this HtmlHelper html)
        {
            return "<script type=\"text/javascript\" src=\"";
        }

        public static string JavaScriptTagEnd(this HtmlHelper html)
        {
            return "\" ></script>";
        }

        public static string CssTagStart(this HtmlHelper html)
        {
            return "<link rel=\"stylesheet\" type=\"text/css\" href=\"";
        }

        public static string CssTagEnd(this HtmlHelper html)
        {
            return "\" />";
        }

    }
}