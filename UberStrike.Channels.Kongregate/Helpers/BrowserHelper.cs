using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Kongregate.Helpers
{
    public static class BrowserHelper
    {
        public static bool BrowserIsSafari
        {
            get { return HttpContextFactory.Current.Request.Browser.Browser.ToLower().IndexOf("safari") >= 0; }
        }
    }
}