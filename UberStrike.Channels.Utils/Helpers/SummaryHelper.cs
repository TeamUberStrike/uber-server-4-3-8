using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class SummaryHelper
    {
        public static MvcHtmlString UberstrikeValidationSummary(this HtmlHelper html)
        {
            return new MvcHtmlString("<div id=\"uberstrikeValidationSummary\"></div>");
        }
    }
}
