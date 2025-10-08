using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Facebook.Web.Mvc;
using UberStrike.Channels.Utils;
using System.Web.Security;

namespace UberStrike.Channels.Facebook.Controllers
{
   
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmailDuplication(string email, long facebookId)
        {
            ViewBag.Email = email;
            ViewBag.FacebookId = facebookId;
            FacebookSdkWrapper facebookSdkWrapper  = new FacebookSdkWrapper();
            ViewBag.FacebookApplicationUrl = facebookSdkWrapper.GetFacebookApplicationUrl();
            return View();
        }

        [ValidateInput(false)]
        public JsonResult LinkFacebookAccount(string linkingEmail, string linkingPassword, long linkingFacebookId)
        {
            var message = "";
            var isError = false;

            var ret = CmuneMember.AttachFacebookAccount(linkingEmail, linkingPassword, linkingFacebookId, CommonConfig.ApplicationIdUberstrike);
            if (ret == MemberRegistrationResult.Ok)
            {
                message = "Successfully linked, you will be soon redirected to the game page";
            }
            else
            {
                isError = true;
                message = "Password incorrect";
            }

            return new JsonResult() { Data = new { message = message, isError = isError } };
        }

        public ActionResult CreationError(string error)
        {
            ViewBag.CreationError = error;
            return View();
        }
    }
}
