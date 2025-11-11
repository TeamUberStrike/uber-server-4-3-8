using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils;
using Facebook;

namespace UberStrike.Channels.Facebook.Helpers
{
    public static class FacebookUrlHelper
    {
        public static MvcHtmlString CanvasAction(this UrlHelper helper, HtmlHelper html, string actionName, string controllerName)
        {
            var UrlReferrerQueryString = (!String.IsNullOrEmpty((string)html.ViewContext.Controller.ViewBag.UrlReferrerQueryString) ? "&"+ html.ViewContext.Controller.ViewBag.UrlReferrerQueryString : "");
            return new MvcHtmlString(helper.Action(actionName, controllerName) + "?signed_request=" + html.ViewContext.Controller.ViewBag.SignedRequest + UrlReferrerQueryString);
        }

        public static string AbsoluteCanvasInsideUrls(this UrlHelper helper, HtmlHelper html, string actionName, string controllerName)
        {
            return FacebookApplication.Current.CanvasUrl+controllerName+"/"+actionName + "?signed_request=" + html.ViewContext.Controller.ViewBag.SignedRequest;
        }

        public static string DealSpotIframeUrl(this UrlHelper helper, long facebookApplicationId, string facebookThirdPartId)
        {
            return HttpContextFactory.Current.Request.Url.Scheme + "://assets.tp-cdn.com/static3/swf/dealspot.swf?app_id=" + facebookApplicationId.ToString() + "&mode=fbpayments&sid=" + facebookThirdPartId;
        }
    }
}