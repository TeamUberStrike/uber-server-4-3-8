using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;
using Facebook;
using Facebook.Web;
using System.Configuration;
using System.Collections.Specialized;
using UberStrike.Channels.Facebook.Models;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Channels.Facebook.Utils;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using System.Text;
using System.Globalization;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class HomeController : BaseController
    {
        #region Properties and Parameters

       

        #endregion

        #region Constructors

        public HomeController() : base()
        {
           
        }

        #endregion

        public ActionResult RefreshMenu()
        {
            return PartialView("_Menu", FbUserModel);
        }

        public ActionResult Index()
        {
            if (FbUserModel.FacebookId <= 0)
            {
                throw new ArgumentOutOfRangeException("FacebookId can't be 0");
            }

            PrepareGame(base.CmuneCookie);
            /**** end ****/

            return View("Play", FbUserModel);
        }

        void PrepareGame(CmuneCookie cmuneCookie)
        {
            string developerId = CmuneEconomy.PaymentWriteDeveloperId(UberStrikeCommonConfig.ApplicationId, ChannelType.WebFacebook, 0);

            int unityBuildTypeIndex = ConfigurationUtilities.ReadConfigurationManagerInt("UnityBuild");
            string cultureName = ConfigurationUtilities.ReadConfigurationManager("UnityDefaultCulture");

            if (!Request.QueryString["lang"].IsNullOrFullyEmpty())
            {
                cultureName = Request.QueryString["lang"];
            }

            string initialUnityArguments = UnityArgsBuilder.BuildInitialUnityArguments(ChannelType.WebFacebook, cultureName);
            StringBuilder gameUrlArguments = new StringBuilder();

            // If the cookie belongs to the currentUser
            if (cmuneCookie != null && FbUserModel.IsAuthenticated)
            {
                gameUrlArguments.Append("&cmid=");
                gameUrlArguments.Append(cmuneCookie.Cmid.ToString());
                gameUrlArguments.Append("&time=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.ExpirationTime.ToString("G", CultureInfo.CreateSpecificCulture(cultureName)).Base64Encode()));
                gameUrlArguments.Append("&content=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.CookieEncryptedContent));
                gameUrlArguments.Append("&hash=");
                gameUrlArguments.Append(Uri.EscapeDataString(cmuneCookie.CookieHash));
                gameUrlArguments.Append("&esnsMemberId=");
                gameUrlArguments.Append(FbUserModel.FacebookId);
            }
            else
            {
                gameUrlArguments.Append("&loginmode=facebookrego&esnsMemberId=");
                gameUrlArguments.Append(FbUserModel.FacebookId.ToString());
            }

            ViewBag.DeveloperId = developerId;
            ViewBag.UnityArguments = initialUnityArguments + gameUrlArguments.ToString();
        }

        public ActionResult About()
        {

            return View();
        }
    }
}
