using System.Web.Mvc;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Kongregate.ActionFilters
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {

        private string fillExceptionWithAllRequestParams(ExceptionContext filterContext)
        {
            string parameters = string.Empty;

            filterContext.Exception.Data.Add("", "the datas below are http request datas");
            foreach (var key in filterContext.HttpContext.Request.Params.Keys)
            {
                if (key != null)
                    filterContext.Exception.Data.Add(key.ToString(), filterContext.HttpContext.Request.Params[key.ToString()].ToString());
            }

            return parameters;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            CmuneLog.LogException(filterContext.Exception, fillExceptionWithAllRequestParams(filterContext));
        }
    }
}