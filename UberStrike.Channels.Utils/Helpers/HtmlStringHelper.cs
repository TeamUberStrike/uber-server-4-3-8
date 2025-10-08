using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class HtmlStringHelper
    {
        public static HtmlString BoldText(this HtmlHelper html, string text)
        {
            return new HtmlString("<span class=\"bold\">" + text + "</span>");
        }


    }
}
