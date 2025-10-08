using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class ChangeFieldHelper
    {
        static string template = "<div class=\"greyDiv\">";
        static string templateEnd = "</div>";

        public static string BeginGreyDiv(this HtmlHelper helper)
        {
            return template;
        }

        public static string EndGreyDiv(this HtmlHelper helper)
        {
            return templateEnd;
        }

        /// <summary>
        /// Display a panel to change the field
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="panelId"></param>
        /// <param name="fieldName"></param>
        /// <param name="displayed"></param>
        /// <param name="onclick"></param>
        /// <returns></returns>
        public static string ChangeFieldTogglePanel(this HtmlHelper helper, string panelId, string fieldName, bool displayed, string onclick, string editImageUrl)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<a href=\"javascript:void(0);\" id=\"editTagLink\" onclick=\"$('#" + panelId + "').show();return false;\">");
            sb.Append("<img src=\"" + editImageUrl + "\" alt=\"" + fieldName + "\" />");
            sb.Append("</a>");
            sb.Append("<div id=\"" + panelId + "\" class=\"editBox\" " + (displayed == false ? "style=\"display:none;\"" : "") + ">");
            sb.Append("<span class=\"errorMessage\"></span>");
            sb.Append("New " + fieldName + ":<br />");
            sb.Append("<input type=\"text\" name=\"new" + fieldName + "\" id=\"new" + fieldName + "\" value=\"\"/><br />");
            sb.Append("Action description:<br />");
            sb.Append("<textarea id=\"explanationForNew" + fieldName + "\" name=\"explanationForNew" + fieldName + "\"></textarea><br />");
            sb.Append("Notify leader: <input type=\"checkbox\" checked=\"checked\" name=\"notifyForNew" + fieldName +"\" /><br />");
            sb.Append("<input type=\"submit\" id=\"change" + fieldName + "Button\" onclick=\"" + onclick + "\" value=\"Modify\" />");
            sb.Append("<input type=\"button\" onclick=\"$('#" + panelId + "').hide()\" value=\"Cancel\" />");
            sb.Append("</div>");

            return sb.ToString();
        }
    }
}