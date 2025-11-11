using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using UberStrike.Channels.Common.Models;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Business;
using UberStrike.Channels.Utils.Models;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Common.Utils;
using Facebook.Web;

namespace UberStrike.Channels.Common.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        private string GenerateKongregateEmail(string kongregateId)
        {
            return kongregateId + "@kongregate.fake.com";
        }

        public ActionResult ConnectWith(ChannelType channel, string h)
        {
            ViewBag.Hash = h;
            ViewBag.ChannelType = channel;
            return View();
        }

        public ActionResult ConnectWithCmune(ChannelType channel, string h)
        {
            string kgIdFromHash = Crypto.fncRijndaelDecrypt(h, ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptPassPhrase"), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptInitVector"));

            var signInModel = new SignInModel();
            signInModel.KongregateId = kgIdFromHash;
            signInModel.ChannelType = channel;
            signInModel.Hash = h;
            ViewBag.ChannelType = channel;

            return View(signInModel);
        }

        public ActionResult ConnectWithCmuneGo(SignInModel signUpModel)
        {
            Member member = null;
            bool isSuccess = false;
            MemberAuthenticationResult memberAuth = CmuneMember.CmuneLoginEmail(signUpModel.Email, signUpModel.Password, UberStrikeCommonConfig.ApplicationId, out member);
            string message = string.Empty;

            if (memberAuth.Equals(MemberAuthenticationResult.Ok))
            {
                CmuneMember.AttachKongregateAccount(member.CMID, long.Parse(signUpModel.KongregateId));
                message = "You are successfully connected";
                isSuccess = true;
            }
            else
            {
                if (memberAuth == MemberAuthenticationResult.IsBanned)
                {
                    MemberAccess memberAccess = CmuneMember.GetMemberAccess(member.CMID, UberStrikeCommonConfig.ApplicationId);
                    if (memberAccess != null)
                    {
                        BanMode banMode = (BanMode)memberAccess.IsAccountDisabled;
                        message = banMode.GetBanModeErrorMessage(((DateTime)memberAccess.AccountDisabledUntil).AddDays(1).ToString("MMMM dd"));
                    }
                }
                else
                {
                    message = memberAuth.GetMemberRegistrationErrorMessage();
                }
            }

            ViewBag.ChannelType = signUpModel.ChannelType;
            ViewBag.IsSuccess = isSuccess;
            ViewBag.Message = message;
            return View("ConnectWithCmune", signUpModel);
        }

        public ActionResult ConnectWithFacebookGo(ChannelType channel, string h)
        {
            string kongregateIdFromHash = Crypto.fncRijndaelDecrypt(h, ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptPassPhrase"), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptInitVector"));
            long kongregateId = 0;
            long.TryParse(kongregateIdFromHash, out kongregateId);
            bool isSuccess = false;
            string message = string.Empty;

            if (FacebookWebContext.Current.IsAuthenticated())
            {
                var facebookSDKWrapper = new FacebookSdkWrapper();
                var fbUserModel = facebookSDKWrapper.GetFacebookUserModel();
                int cmid = CmuneMember.GetCmidByFacebookId(fbUserModel.FacebookId);
                if (cmid > 0)
                {
                    CmuneMember.AttachKongregateAccount(cmid, kongregateId);
                    message = "You are successfully connected";
                    isSuccess = true;
                }
            }

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Message = message;
            ViewBag.ChannelType = channel;
            var signInModel = new SignInModel();
            signInModel.ChannelType = channel;
            signInModel.Hash = h;
            return View("ConnectWithFacebook", signInModel);
        }

    }
}
