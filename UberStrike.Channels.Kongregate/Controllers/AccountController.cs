using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Kongregate.Models;
using UberStrike.Channels.Utils.Models;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Channels.Utils;
using Cmune.Instrumentation.Business;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Utils;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeletedAccount()
        {
            return View();
        }

        public ActionResult ConnectWith(string kongregateId)
        {
            string kongregateIdHash = Crypto.fncRijndaelEncrypt(kongregateId, ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptPassPhrase"), ConfigurationUtilities.ReadConfigurationManager("AuthenticationEncryptInitVector"));

            ViewBag.Hash = kongregateIdHash;
            return View();
        }

        public ActionResult CreationError()
        {
            return View();
        }

        public ActionResult AccountCompletion()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Complete(CompleteAccountModel model)
        {
            bool success = false;
            string message = String.Empty;

            var userProfile = (UserProfileModel)ViewBag.UserProfile;
            Member currentMember = CmuneMember.GetMember(userProfile.Member.CMID);

            if (currentMember == null)
            {
                message = "Hmm. Something appears to have gone wrong. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
            }
            else if (!ValidationUtilities.IsValidEmailAddress(model.NewEmail))
            {
                message = "Your email is incorrect";
            }
            else if (String.IsNullOrEmpty(model.Password))
            {
                message = "Your password is empty";
            }
            else if (model.Password != model.PasswordConfirmation || !ValidationUtilities.IsValidPassword(model.Password))
            {
                message = "Your confirmation password doesn't match or your password is invalid";
            }

            if (!success && message == String.Empty) // no error
            {
                string newEmail = model.NewEmail;

                MemberOperationResult result = UsersBusiness.ChangeMemberEmail(userProfile.Member.CMID, newEmail, Request.UserHostAddress);

                if (result.Equals(MemberOperationResult.Ok))
                {
                    success = true;
                    ID3 memberId3 = CmuneMember.GetId3(userProfile.Member.CMID, new CmuneDataContext());
                    UberStrikeMail.SendEmailActivateMember(newEmail, memberId3.Name, CmuneMember.GenerateValidationUrl(userProfile.Member.CMID), (EmailAddressStatus)userProfile.Member.EmailAddressState);
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                }
                else
                {
                    message = result.GetMemberOperationErrorMessage();
                }
            }

            if (success) // if username is changed
            {
                if (!Users.ChangePassword(userProfile.Member.CMID, model.Password))
                {
                    success = false;
                    message = "Your password is invalid";
                }
            }

            return new JsonResult() { Data = new { errorMessage = message } };
        }
    }
}
