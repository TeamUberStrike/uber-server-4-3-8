using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Business;
using System.Text;
using Cmune.Channels.Instrumentation.Utils;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class SignInRecordHelper
    {
        public static string GetLastRecords(this HtmlHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            var recordsList = SignInRecord.GetNext20Records();
            sb.Append("<h3>Last logins</h3><br/>");
            sb.Append("<table class=\"searchResultTable\">");
            sb.Append("<tr>");
            sb.Append(helper.TableTh("Name"));
            sb.Append(helper.TableTh("IP"));
            sb.Append(helper.TableTh("Connection Time"));
            sb.Append("</tr>");

            foreach (var item in recordsList)
            {
                sb.Append("<tr>");
                var elements = item.Split(';');
                sb.Append(helper.TableTd(elements[0]));
                sb.Append(helper.TableTd(elements[1]));
                sb.Append(helper.TableTd(elements[2]));
                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return sb.ToString();
        }
    }
}