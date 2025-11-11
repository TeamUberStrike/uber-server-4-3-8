using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Models;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult GetGuestPage()
        {
            return PartialView();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult GetTopMenu()
        {
            var userProfile = (UserProfileModel)ViewBag.UserProfile;

            var isFakeEmail = userProfile.Member.Login.Contains("@kongregate.fake.com");
            ViewBag.IsFakeEmail = isFakeEmail;
            return PartialView();
        }

        private Dictionary<string, string> GetI18n()
        {
            var I18nText = new Dictionary<string, string>();
            I18nText.Add("windowTitle", "Install Unity3D");
            I18nText.Add("welcomeSentence", "Welcome to UberStrike! You're few clicks away from over 6 millions players!");
            I18nText.Add("continueInstall", "Click <strong>Continue</strong> to setup Unity3D");
            I18nText.Add("continueInstallError", "If your download didn't start please ");
            I18nText.Add("clickHere", "click here");
            I18nText.Add("supportLink", "Need Help? ");
            return I18nText;
        }

        [HttpPost]
        public ActionResult ProcessUnityInstallFlow(string unityObjectUAInstallUrl, string unityObjectUAInstallonclick, bool unityObjectUAJava,
            bool unityObjectUACo, bool unityObjectUAWin, bool unityObjectUAMac, string unityObjectUAInstallNavPlatform)
        {
            ViewBag.I18nText = GetI18n();
            ViewBag.UnityObjectUAInstallUrl = unityObjectUAInstallUrl;
            ViewBag.UnityObjectUAInstallonclick = unityObjectUAInstallonclick;
            ViewBag.UnityObjectUAJava = unityObjectUAJava;
            ViewBag.UnityObjectUACo = unityObjectUACo;
            ViewBag.UnityObjectUAWin = unityObjectUAWin;
            ViewBag.UnityObjectUAMac = unityObjectUAMac;
            ViewBag.UnityObjectUAInstallNavPlatform = unityObjectUAInstallNavPlatform;
            return PartialView();
        }

    }
}
