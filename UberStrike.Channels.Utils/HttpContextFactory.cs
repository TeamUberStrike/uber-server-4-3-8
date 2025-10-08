using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace UberStrike.Channels.Utils
{
    public class HttpContextFactory
    {
        private static HttpContextBase m_context;

        public static HttpContextBase Current
        {
            get
            {
                if (m_context != null)
                    return m_context;
                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext is not available");
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            m_context = context;
        }

        public static void ClearSession(string sessionKey)
        {
            Current.Session[sessionKey] = null;
        }

        public static void SetSession(string sessionKey, object instance)
        {
            Current.Session[sessionKey] = instance;
        }

        public static object GetSession(string sessionKey)
        {
            return Current.Session[sessionKey];
        }

        public static string GetReferrer()
        {
            var urlReferrer = string.Empty;
            if (Current.Request.UrlReferrer != null)
            {
                urlReferrer = Current.Request.UrlReferrer.AbsoluteUri;
            }
            return urlReferrer;
        }

        public static void SetCookieForShortDuration(string key, string value)
        {
            var httpCookie = new HttpCookie(key, value);
            //httpCookie["expirationDate"] = DateTime.Now.AddMinutes(15).ToString("G", CultureInfo.CreateSpecificCulture("en-US"));
            Current.Response.Cookies.Set(httpCookie);
        }

        public static string GetCookie(string key)
        {
            if (Current.Request.Cookies[key] == null || Current.Request.Cookies[key].Value == null)
                return null;
            return Current.Request.Cookies[key].Value.ToString();
        }
    }
}