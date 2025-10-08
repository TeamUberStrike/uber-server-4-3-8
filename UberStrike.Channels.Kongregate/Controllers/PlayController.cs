using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Models;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Business;
using UberStrike.Channels.Utils;
using System.Text;
using System.Globalization;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class PlayController : BaseController
    {
        public ActionResult Index()
        {
            PrepareGame();

            return View();
        }

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

        private void PrepareGame()
        {
            var userProfileModel = (UserProfileModel)ViewBag.UserProfile;
            var initialUnityArguments = PlayBuildInitialUnityArgments(ChannelType.Kongregate);

            var cmuneCookie = AuthenticationCookieManagement.SetAuthCookie(HttpContextFactory.Current.Request, HttpContextFactory.Current.Response, userProfileModel.Member != null ? userProfileModel.Member.CMID : 0, ChannelType.Kongregate, userProfileModel.Member != null ? userProfileModel.Member.CMID.ToString() : 0.ToString());

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
            //else if (userProfileModel.UserProfileType == UserProfileType.Facebook)
            //{
            //    var facebookSDKWrapper = new FacebookSdkWrapper();
            //    gameUrlArguments.Append("&loginmode=facebookrego&esnsMemberId=");
            //    gameUrlArguments.Append(facebookSDKWrapper.GetFacebookUserModel().FacebookId.ToString());
            //}
            ViewBag.IsAccountComplete = CmuneMember.IsAccountComplete(userProfileModel.Member.CMID);
            ViewBag.unityArgs = initialUnityArguments + gameUrlArguments;

        }
    }
}
