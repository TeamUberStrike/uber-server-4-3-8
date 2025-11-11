using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using UberStrike.Channels.Utils;
using Facebook.Web;
using UberStrike.Channels.Utils.Models;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Utils
{
    public static class AuthenticationManager
    {
        public static void Login(int cmid, ChannelType channelType)
        {
            FormsAuthentication.SetAuthCookie(cmid.ToString() + SessionVariables.AuthCookieSeparator + channelType, true);
            HttpContextFactory.ClearSession(SessionVariables.UserProfile);
        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
            FacebookWebContext.Current.DeleteAuthCookie();
            HttpContextFactory.ClearSession(SessionVariables.UserProfile);
        }
    }
}