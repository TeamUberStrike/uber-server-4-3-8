using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Utils.Models;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Portal.Controllers
{
    public class EpinController : Controller
    {
        //
        // GET: /Epin/
        [Authorize]
        public ActionResult Index(string lang)
        {
            var userProfile = ((UserProfileModel)ViewBag.UserProfile);

            ViewBag.AuthHash = Url.Encode(Crypto.fncRijndaelEncrypt(userProfile.Member.CMID.ToString(), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptPassPhrase"), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptInitVector")));
            if (userProfile.ChannelType == ChannelType.WebFacebook)
            {
                ViewBag.FacebookProfilePicture = new FacebookSdkWrapper().GetUserPicture(userProfile.FacebookUserModel.FacebookId);
            }
            ViewBag.ChannelType = userProfile.ChannelType;
            ViewBag.Lang = lang;


            return View();
        }
    }
}
