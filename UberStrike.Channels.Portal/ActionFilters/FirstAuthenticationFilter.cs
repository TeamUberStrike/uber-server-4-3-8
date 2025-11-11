using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;

namespace UberStrike.Channels.Portal.ActionFilters
{
    public class FirstAuthenticationFilter : ActionFilterAttribute
    {
        public static string HashedCookieIdentifier = Crypto.fncSHA256Encrypt(ConfigurationUtilities.ReadConfigurationManager("BetaLogin") + ConfigurationUtilities.ReadConfigurationManager("BetaPassword"));
        public string firstAuthCookieName = ConfigurationUtilities.ReadConfigurationManager("UberstrikeStagingAccessCookie");

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller.GetType().Name != "FirstAuthenticationController")
            {
                if (HttpContextFactory.Current.Request.Cookies[firstAuthCookieName] == null
                    || (HttpContextFactory.Current.Request.Cookies[firstAuthCookieName] != null && HttpContextFactory.Current.Request.Cookies[firstAuthCookieName].Value != HashedCookieIdentifier))
                {
                        filterContext.Result = new RedirectResult("~/FirstAuthentication");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}