using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class SummaryHelper
    {
        public static string ShadowSummary(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"shadowSummarySuccessMessage\" style=\"display:none;\"></div><div id=\"shadowSummaryErrorMessage\" style=\"display:none;\"></div>");
            return sb.ToString();
        }

        public static string IndicatorLight(this HtmlHelper helper)
        {
            return "<div id=\"GreenLight\" style=\"display:none;\"></div><div id=\"RedLight\" style=\"display:none;\"></div>";
        }
    }
}
