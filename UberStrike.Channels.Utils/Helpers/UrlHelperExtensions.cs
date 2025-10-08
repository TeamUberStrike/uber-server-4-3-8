using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string AppBaseUrl(this UrlHelper url)
        {
            return string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, url.Content("~"));
        }

        public static string DomainUrl(this UrlHelper url)
        {
            return string.Format("{0}", HttpContext.Current.Request.Url.Authority);
        }

        public static string Decode(this UrlHelper url, string text)
        {
            return HttpUtility.UrlDecode(text);
        }
    }
}
