using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class HtmButtonsHelper
    {
        public static MvcHtmlString SubmitButton(this HtmlHelper html, string id, string value, string cssClass = "", string onClickEvent = "")
        {
            return new MvcHtmlString("<input type=\"submit\" id=\"" + id + "\" value=\"" + value + "\" class=\"" + cssClass + "\" onclick=\"" + onClickEvent + "\" />");
        }

        public static MvcHtmlString Button(this HtmlHelper html, string id, string value, string cssClass = "", string onClickEvent = "")
        {
            return new MvcHtmlString("<input type=\"button\" id=\"" + id + "\" value=\"" + value + "\" class=\"" + cssClass + "\" onclick=\"" + onClickEvent + "\" />");
        }

    }
}
