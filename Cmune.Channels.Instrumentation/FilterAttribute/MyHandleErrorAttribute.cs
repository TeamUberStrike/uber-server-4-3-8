using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Utils;
using System.Text;
using Cmune.Channels.Instrumentation.Utils;
using System.Configuration;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.FilterAttribute
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            StringBuilder sb = new StringBuilder();
            sb.Append("Message ");
            sb.AppendLine(filterContext.Exception.Message);
            sb.AppendLine();
            sb.Append("Source ");
            sb.AppendLine(filterContext.Exception.Source);
            sb.AppendLine();
            sb.Append("TargetSite ");
            sb.AppendLine(filterContext.Exception.TargetSite != null ? filterContext.Exception.TargetSite.ToString() : null);
            sb.AppendLine();
            sb.Append("Data ");
            sb.AppendLine(filterContext.Exception.Data != null ? filterContext.Exception.Data.ToString() : null);
            sb.AppendLine();
            sb.Append("StackTrace ");
            sb.AppendLine(filterContext.Exception.StackTrace.ToString());
            sb.AppendLine();
            sb.Append("InnerException ");
            sb.AppendLine(filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.ToString() : null);
            sb.AppendLine();
            sb.Append("HelpLink ");
            sb.AppendLine(filterContext.Exception.HelpLink);
            sb.AppendLine();
            CmuneLog.LogException(filterContext.Exception, String.Empty);
        }
    }
}