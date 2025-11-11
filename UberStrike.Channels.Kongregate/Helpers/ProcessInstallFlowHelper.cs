using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace UberStrike.Channels.Kongregate.Helpers
{
    public static class ProcessInstallFlowHelper
    {
        public static bool UnityInstallIsOneClickPluginEnabled(this HtmlHelper htmlHelper)
        {
            return ((bool)htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAJava) || (bool)(htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUACo);
        }

        public static bool UnityInstallIsMacPlatform(this HtmlHelper htmlHelper)
        {
            return (htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAInstallNavPlatform == "MacIntel" || htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAInstallNavPlatform == "MacPPC");
        }

        public static bool UnityInstallIsWindowPlatform(this HtmlHelper htmlHelper)
        {
            return (bool) htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAWin;
        }

        public static string UnityInstallGetUnityDownloadLink(this HtmlHelper htmlHelper)
        {
            if ((bool)htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAWin)
                return "http://webplayer.unity3d.com/download_webplayer-3.x/UnityWebPlayer.exe";
            else if ((bool)htmlHelper.ViewContext.Controller.ViewBag.UnityObjectUAMac)
                return "http://webplayer.unity3d.com/download_webplayer-3.x/webplayer-mini.dmg";
            else
                return "";
        }
    }
}