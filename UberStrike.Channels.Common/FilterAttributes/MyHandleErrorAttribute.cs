using System.Web.Mvc;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Common.FilterAttributes
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if(filterContext.Exception.Message.Contains("The parameters dictionary contains a null entry for parameter 'channel' of non-nullable")
            || filterContext.Exception.Message.Contains("The parameters dictionary contains a null entry for parameter 'ch' of non-nullable type")
                || filterContext.Exception.Message.Contains("The parameters dictionary contains a null entry for parameter 'cmid' of non-nullable type 'System.Int32'")
            )
            {
                return;
            }
            
            if (filterContext.Exception.Message.Contains("0x80070040") || filterContext.Exception.Message.Contains("0x80070057")
            || filterContext.Exception.Message.Contains("0x800703E3") || filterContext.Exception.Message.Contains("0x800704CD")) // avoid to log a exception when the user left le page
            {
                return;
            }
            CmuneLog.LogException(filterContext.Exception, "Ip: " + filterContext.HttpContext.Request.UserHostAddress);
        }
    }
}