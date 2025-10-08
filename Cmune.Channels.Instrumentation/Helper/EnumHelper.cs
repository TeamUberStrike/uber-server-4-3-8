using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class EnumHelper
    {
        public static string PrepareEnumForDisplay(this HtmlHelper helper, string enumStr)
        {
            return EnumUtils.PrepareEnumForDisplay(enumStr);
        }

    }
}