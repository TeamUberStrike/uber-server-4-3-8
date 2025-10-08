using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Facebook.FilterAttributes
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if (filterContext.Exception.Message.Contains("Invalid signed request") || filterContext.Exception.GetType().Name == "FacebookOAuthException")
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.Redirect(urlHelper.Action("Reload", "Error"));
                return;
            }
            if (filterContext.Exception.Message.Contains("0x800703E3")) // avoid to log the exception : The I/O operation has been aborted because of either a thread exit or an application request. (Exception from HRESULT: 0x800703E3)
            {
                return;
            }
            CmuneLog.LogException(filterContext.Exception, "Ip: " + filterContext.HttpContext.Request.UserHostAddress);
        }
    }
}