using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Utils.Models;
using UberStrike.DataCenter.Business;

namespace UberStrike.Channels.Portal.Controllers
{
    public class PlayController : Controller
    {
        private string PlayBuildInitialUnityArgments(ChannelType channelType = ChannelType.WebPortal)
        {
            this.ViewBag.LittleTopMenuDisplay = true;
            this.ViewBag.BannerDisplay = false;
            this.ViewBag.TopMenuDisplay = false; 
            int unityBuildTypeIndex = ConfigurationUtilities.ReadConfigurationManagerInt("UnityBuild");
            int unityObjectWidth = 989; 

            string cultureName = ConfigurationUtilities.ReadConfigurationManager("DefaultCulture");
            string embedType = String.Empty;

            if (!Request.Params["lang"].IsNullOrFullyEmpty())
            {
                cultureName = Request.Params["lang"];
            }

            if (!Request.Params["embedtype"].IsNullOrFullyEmpty())
            {
                embedType = Request.Params["embedtype"];

                if (embedType.Equals("1"))
                {
                    unityObjectWidth = 928;
                }
            }

            ViewBag.UnityObjectWidth = unityObjectWidth;
            string initialUnityArguments = UnityArgsBuilder.BuildInitialUnityArguments(channelType, cultureName, embedType);
            return initialUnityArguments;
        }

        [Authorize]
        public ActionResult Index()
        {
            var userProfileModel = (UserProfileModel)ViewBag.UserProfile;
            var initialUnityArguments = PlayBuildInitialUnityArgments(ChannelType.WebPortal);

            var cmuneCookie = AuthenticationCookieManagement.SetAuthCookie(HttpContextFactory.Current.Request, HttpContextFactory.Current.Response, userProfileModel.Member != null ? userProfileModel.Member.CMID : 0, ChannelType.WebPortal, userProfileModel.Member != null ? userProfileModel.Member.CMID.ToString() : 0.ToString());

            StringBuilder gameUrlArguments = new StringBuilder();
            StringBuilder gameUrlArgumentsWithoutRegistration = new StringBuilder();
            if (cmuneCookie != null && cmuneCookie.IsValid)
            {
                gameUrlArguments.Append("&cmid=");
                gameUrlArguments.Append(cmuneCookie.Cmid);
                gameUrlArguments.Append("&time=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.ExpirationTime.ToString("G", CultureInfo.CreateSpecificCulture("en-US")).Base64Encode()));
                gameUrlArguments.Append("&content=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.CookieEncryptedContent));
                gameUrlArguments.Append("&hash=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.CookieHash));
            }

            ViewBag.IsAccountComplete = CmuneMember.IsAccountComplete(userProfileModel.Member.CMID);
            ViewBag.unityArgs = initialUnityArguments + gameUrlArguments;
            return View();
        }
    }
}
