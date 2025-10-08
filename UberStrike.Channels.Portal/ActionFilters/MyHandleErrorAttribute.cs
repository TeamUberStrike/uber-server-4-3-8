using System.Web.Mvc;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Portal.ActionFilters
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {

        private string fillExceptionWithAllRequestParams(ExceptionContext filterContext)
        {
            string parameters = string.Empty;

            filterContext.Exception.Data.Add("", "the datas below are http request datas");
            foreach (var key in filterContext.HttpContext.Request.Params.Keys)
            {
                filterContext.Exception.Data.Add(key.ToString(), filterContext.HttpContext.Request.Params[key.ToString()].ToString());
            }

            return parameters;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            string incomingDatas = string.Empty;
            incomingDatas += "Ip: "+ filterContext.HttpContext.Request.UserHostAddress;
            if (filterContext.Exception.Message.Contains("Invalid signed request") || filterContext.Exception.GetType().Name == "FacebookOAuthException")
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.Redirect(urlHelper.Action("LogOff", "Account"));
                return;
            }

            if (filterContext.Exception.Message.Contains("The input is not a valid Base-64 string as it contains a non-base 64 character") && filterContext.Exception.StackTrace.Contains("FacebookSignedRequest"))
            {
                fillExceptionWithAllRequestParams(filterContext);
            }

            if (filterContext.Exception.Message.Contains("0x800703E3")) // avoid to log the exception : The I/O operation has been aborted because of either a thread exit or an application request. (Exception from HRESULT: 0x800703E3)
            {
                return;
            }

            CmuneLog.LogException(filterContext.Exception, incomingDatas);
        }
    }
}