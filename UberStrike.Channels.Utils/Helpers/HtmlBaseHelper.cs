using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UberStrike.Channels.Utils.Helpers
{
    public class HtmlBaseHelper
    {
        public static string JsBegin()
        {
            return "<script type=\"text/javascript\">";
        }

        public static string JsEnd()
        {
            return "</script>";
        }
    }
}
