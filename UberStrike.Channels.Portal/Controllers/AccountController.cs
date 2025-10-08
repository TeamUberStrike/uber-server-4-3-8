using System;
using System.Linq;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Business;
using Facebook.Web;
using UberStrike.Channels.Portal.Extensions;
using UberStrike.Channels.Portal.Models;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Utils.Helpers;
using UberStrike.Channels.Utils.Models;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Utils;

namespace UberStrike.Channels.Portal.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                var userProfile = ((UserProfileModel)ViewBag.UserProfile);

                if (userProfile != null && userProfile.Member != null)
                {
                    ID3 memberId3 = CmuneMember.GetId3(((UserProfileModel)ViewBag.UserProfile).Member.CMID, cmuneDB);
                    ViewBag.MemberUserName = memberId3.Name;
                }
                else
                {
                    ViewBag.MemberUserName = "";
                }
            }

            return View();
        }

        public ActionResult ValidateEmail(int? cmid, string hash)
        {
            string pointsAwarded = String.Empty;
            string validationMessage = "Hmm. Something appears to have gone wrong. Please try the original link again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
            bool isValidated = false;

            if (cmid.HasValue)
            {
                bool hasAlreadyValidatedEmail = CmuneMember.HasAlreadyValidatedEmail(cmid.Value);

                if (Users.ValidateMemberEmail(cmid.Value, hash))
                {

                    isValidated = true;

                    if (!hasAlreadyValidatedEmail)
                    {
                        pointsAwarded = " We've added <b>" + CommonConfig.PointsAttributedOnEmailValidation + " points to your account.</b>";
                    }

                    validationMessage = "<p>Congratulations! Your account has been successfully activated.<br />" + pointsAwarded + "!</p>";

                    string linkTemplate = "<p style=\"padding-top:30px;\"><a href=\"{0}\" style=\"color:#FEC42C\">Click here to start the game!</a></p>";
                    string redirectLink = String.Empty;

                    if (Request.IsAuthenticated)
                    {
                        redirectLink = String.Format(linkTemplate, Url.Play());
                    }
                    else
                    {
                        redirectLink = String.Format(linkTemplate, Url.Play());
                    }

                    ViewBag.RedirectLink = redirectLink;
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                }
                else
                {
                    if (CmuneMember.GetEmailAddressStatus(cmid.Value) == EmailAddressStatus.Verified)
                    {
                        validationMessage = "<p>Congratulations! Your account has been successfully activated.</p>";
                    }
                    else
                    {
                        isValidated = false;
                        validationMessage = "<p>Hmm. Something appears to have gone wrong. Please try the original link again and if the problem continues visit our <a href=\"" + CommonConfig.CmuneSupportCenterUrl + "\">support center</a>.</p>";
                    }
                }
            }

            ViewBag.IsValidated = isValidated;
            ViewBag.ValidationMessage = validationMessage;
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult SendVerificationEmail()
        {
            var userProfile = (UserProfileModel)ViewBag.UserProfile;
            string validationUrl = CmuneMember.GenerateValidationUrl(userProfile.Member.CMID);
            string errorMessage = String.Empty;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ID3 memberId3 = CmuneMember.GetId3(userProfile.Member.CMID, cmuneDb);
                Member member = memberId3.Member;

                if (!validationUrl.IsNullOrFullyEmpty())
                {
                    UberStrikeMail.SendEmailActivateMember(member.Login, memberId3.Name, validationUrl, (EmailAddressStatus) member.EmailAddressState);
                }
                else
                {
                    errorMessage = "Hmm. Something appears to have gone wrong. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
                }
            }

            return new JsonResult() { Data = new { errorMessage = errorMessage } };
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ChangeEmail(ChangeEmailModel model)
        {
            bool isDataCorrect = true;
            string errorMessage = String.Empty;

            var userProfile = (UserProfileModel)ViewBag.UserProfile;
            Member currentMember = CmuneMember.GetMember(userProfile.Member.CMID);

            if (currentMember != null)
            {
                if (model.Password.IsNullOrFullyEmpty() || currentMember.Password != CryptographyUtilities.HashPassword(model.Password))
                {
                    isDataCorrect = false;
                    errorMessage = "Password incorrect.";
                }
            }
            else
            {
                isDataCorrect = false;
                errorMessage = "Hmm. Something appears to have gone wrong. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
            }

            if (isDataCorrect)
            {
                string newEmail = model.NewEmail;

                MemberOperationResult result = UsersBusiness.ChangeMemberEmail(userProfile.Member.CMID, newEmail, Request.UserHostAddress);

                if (result.Equals(MemberOperationResult.Ok))
                {
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                }
                else {
                    isDataCorrect = false;
                    errorMessage = result.GetMemberOperationErrorMessage();
                }
            }
            return new JsonResult() { Data = new { errorMessage = errorMessage } };
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            bool isDataCorrect = true;
            string errorMessage = String.Empty;

            var userProfile = (UserProfileModel)ViewBag.UserProfile;
            var currentMember = userProfile.Member;

            if (currentMember != null)
            {
                if (model.CurrentPassword.IsNullOrFullyEmpty() || currentMember.Password != CryptographyUtilities.HashPassword(model.CurrentPassword))
                {
                    isDataCorrect = false;
                    errorMessage = "Password incorrect.";
                }
            }
            else
            {
                isDataCorrect = false;
                errorMessage = "Hmm. Something appears to have gone wrong. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
            }

            if (isDataCorrect && (!model.NewPassword.IsNullOrFullyEmpty() && !model.VerifyPassword.IsNullOrFullyEmpty() && !model.NewPassword.Equals(model.VerifyPassword)))
            {
                isDataCorrect = false;
                errorMessage = "Hey, they don't match!";
            }

            if (isDataCorrect)
            {
                bool result = Users.ChangePassword(((UserProfileModel)ViewBag.UserProfile).Member.CMID, model.NewPassword);

                if (result)
                {
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                }
                else
                {
                    isDataCorrect = false;
                    errorMessage = "That doesn't look like a valid password to me!";
                }
            }

            return new JsonResult() { Data = new { errorMessage = errorMessage } };
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DoRecoverPassword(string email)
        {
            bool isError = false;
            string message = string.Empty;

            if (email.IsNullOrFullyEmpty())
            {
                isError = true;
                message = "That doesn't work. Please enter an e-mail address first!";
            }

            if (!isError)
            {
                MemberOperationResult resetPasswordResult = CmuneMember.RequestNewPassword(email, CommonConfig.ApplicationsName[UberStrikeCommonConfig.ApplicationId], Url.AppBaseUrl() + "Account/RecoverPasswordValidation?cmid={0}&hash={1}", Url.AppBaseUrl() + "Account", Request.UserHostAddress);

                if (resetPasswordResult.Equals(MemberOperationResult.Ok))
                {
                    message = "Check your inbox for further instructions!";
                }
                else
                {
                    isError = true;
                    message = resetPasswordResult.GetMemberOperationErrorMessage();
                }
            }

            return new JsonResult() { Data = new { isError = isError, message = message } };
        }

        public ActionResult RecoverPasswordValidation(int? cmid, string hash)
        {
            bool isPasswordReseted = false;

            if (!hash.IsNullOrFullyEmpty() && cmid.HasValue && cmid.Value != 0)
            {
                isPasswordReseted = Cmune.Instrumentation.Business.UsersBusiness.ResetPassword(cmid.Value, hash);
            }

            ViewBag.IsPasswordReseted = isPasswordReseted;
            return View("RecoverPassword");
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        public JsonResult AccountCompletion(CompleteAccountModel completeAccountModel)
        {
            var message = string.Empty;
            bool isError = false;
            var userProfileModel = (UserProfileModel)ViewBag.UserProfile;

            if (!ValidationUtilities.IsValidEmailAddress(completeAccountModel.Email))
            {
                isError = true;
                message = "That doesn't look like a valid e-mail address to me!";
            }

            if (!isError && (completeAccountModel.Password.IsNullOrFullyEmpty() || completeAccountModel.VerifyPassword.IsNullOrFullyEmpty()))
            {
                isError = true;
                message = "You need to enter a password first!";
            }

            if (!isError && completeAccountModel.Password != completeAccountModel.VerifyPassword)
            {
                isError = true;
                message = "Hey, they don't match!";
            }

            var userProfile = (UserProfileModel)ViewBag.UserProfile;

            if (!isError)
            {
                MemberOperationResult memberOperationResult = UsersBusiness.RegisterEsnsMember(userProfile.Member.CMID, completeAccountModel.Email, completeAccountModel.Password, false, "");

                isError = (memberOperationResult != MemberOperationResult.Ok);
                message = memberOperationResult.GetMemberOperationErrorMessage();

                if (!isError)
                {
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                    message = "Congratulations! Your account has been successfully completed.";
                }
            }

            return new JsonResult() { Data = new { message = message, isError = isError } };
        }

        public ActionResult FBLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult SignUp()
        {
            ViewBag.NumberOfPlayers = CmuneMember.GetLastMemberId();
            ViewBag.NotBlack = true;
            return View("SignUp");
        }

        [ValidateInput(false)]
        public ActionResult SignUpGo(CompleteAccountModel completeAccount)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;
            var cmid = 0;
            string name = String.Empty;
            string successMessage = string.Empty;

            if (!ModelState.IsValid)
            {
                return SignUp();
            }
            if (completeAccount.Password != completeAccount.VerifyPassword)
            {
                ModelState.AddModelError("VerifyPassword", "doesn't match");
            }
            else if (completeAccount.Password.Length < 6)
            {
                ModelState.AddModelError("VerifyPassword", "password should have at least 6 characters");
            }
            // check duplication here because we want that password verification and email verification can be displayed at the same time

            if (!ValidationUtilities.IsValidEmailAddress(completeAccount.Email))
            {
                ModelState.AddModelError("Email", "the email format is incorrect");
            }
            else if (!completeAccount.Email.IsNullOrFullyEmpty() && CmuneMember.IsDuplicateUserEmail(completeAccount.Email))
            {
                ModelState.AddModelError("Email", "address already in use");
            }

            if (ModelState.Where(d => d.Value.Errors.Count > 0).Count() == 0)
            {
                var referrerLink = HttpContextFactory.GetCookie("Referrer");
                var urlReferrer = referrerLink != null ? referrerLink : string.Empty;

                ret = Users.CreateUser(completeAccount.Email, completeAccount.Password, ChannelType.WebPortal, TextUtilities.InetAToN(HttpContextFactory.Current.Request.UserHostAddress), ConfigurationUtilities.ReadConfigurationManager("DefaultCulture"), out cmid, urlReferrer);
                if (ret != MemberRegistrationResult.Ok)
                {
                    ModelState.AddModelError("Email", "A error occured, please contact our support for help");
                }
                else
                {
                    AuthenticationManager.Login(cmid, ChannelType.WebPortal);
                    return RedirectToAction("", "Play");
                }
            }
            return SignUp();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Login(string email, string password, string returnUrl)
        {
            string message = string.Empty;

            bool isSuccessfull = Login(email, password, ChannelType.WebPortal, out message);

            return new JsonResult() { Data = new { isSuccessfull = isSuccessfull, message = message } };
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.Logout();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EmailDuplication(string email, long facebookId)
        {
            ViewBag.Email = email;
            ViewBag.FacebookId = facebookId;
            FacebookSdkWrapper facebookSdkWrapper = new FacebookSdkWrapper();
            ViewBag.FacebookApplicationUrl = facebookSdkWrapper.GetFacebookApplicationUrl();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult LinkFacebookAccount(string linkingEmail, string linkingPassword, long linkingFacebookId)
        {
            var message = "";
            var isError = false;

            var ret = CmuneMember.AttachFacebookAccount(linkingEmail, linkingPassword, linkingFacebookId, CommonConfig.ApplicationIdUberstrike);
            if (ret == MemberRegistrationResult.Ok)
            {
                var cmid = CmuneMember.GetCmidByEmail(linkingEmail);
                AuthenticationManager.Login(cmid, ChannelType.WebFacebook);
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

        public ActionResult Unsubscribe(int c, string h)
        {
            bool isUnsubscribed = EmailMarketing.UnsubscribeFromEmailMarketing(c, h);

            ViewBag.IsUnsubscribed = isUnsubscribed;
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult SetMarketing(bool status)
        {
            bool isSet = EmailMarketing.SetEmailStatus(((UserProfileModel)ViewBag.UserProfile).Member.CMID, status);

            if (isSet)
            {
                HttpContextFactory.ClearSession(SessionVariables.UserProfile);
            }

            return new JsonResult() { Data = new { IsSet = isSet } };
        }

        [ValidateInput(false)]
        public ActionResult ExternalLogin(string channel, string email, string returnUrl, string lang)
        {
            if (!HandleExternalLogin(channel, returnUrl, email))
            {
                return RedirectToAction("Index", "Home");
            }

            if (lang == "tr-TR")
            {
                ViewBag.ReturnUrl = "../Epin?lang=" + lang;
                return View("turkishExternalLogin");
            }

            return View();
        }

        [ValidateInput(false)]
        public ActionResult FacebookLogin(string channel, string returnUrl)
        {
            if (!HandleExternalLogin(channel, returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private bool HandleExternalLogin(string channel, string returnUrl, string email = "")
        {
            ChannelType channelType = ChannelType.WebPortal;

            if (!Enum.TryParse(channel, out channelType))
            {
                return false;
            }

            ViewBag.Channel = (int) channelType;
            ViewBag.ReturnUrl = returnUrl;

            if (!email.IsNullOrFullyEmpty())
            {
                ViewBag.EmailAddress = email;
            }

            if (Request.IsAuthenticated)
            {
                // TODO: should move to SessionActionFilter probably
                AuthenticationManager.Logout();
            }

            return true;
        }

        public ActionResult NoAccount()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UpdateAccount()
        {
            if (FacebookWebContext.Current.IsAuthenticated())
            {
                var facebookWrapper = new FacebookSdkWrapper();
                var facebookUserModel = facebookWrapper.GetFacebookUserModel();

                string emailAddress = facebookUserModel.Email;

                // If the user already has an email address in his email account we'll select this one over his Facebook email address

                Member member = CmuneMember.GetMemberByFacebookId(FacebookWebContext.Current.UserId);

                if (member != null)
                {
                    emailAddress = member.Login;
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.EmailAddress = emailAddress;
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateAccount(string email, string password, string confirmPassword)
        {
            string errorMessage = String.Empty;

            if (FacebookWebContext.Current.IsAuthenticated())
            {
                long facebookId = FacebookWebContext.Current.UserId;

                if (password != confirmPassword)
                {
                    errorMessage = "The passwords don't match.";
                }
                else if (!ValidationUtilities.IsValidPassword(password))
                {
                    errorMessage = "That doesn't look like a valid password to me!";
                }

                if (!ValidationUtilities.IsValidEmailAddress(email))
                {
                    if (!errorMessage.IsNullOrFullyEmpty())
                    {
                        errorMessage += "<br />";
                    }

                    errorMessage += "That doesn't look like a valid e-mail address to me!";
                }

                if (errorMessage.IsNullOrFullyEmpty())
                {
                    // Now we need to test if the user already has an email address and password

                    Member member = CmuneMember.GetMemberByFacebookId(facebookId);

                    if (member != null)
                    {
                        if (member.Login.IsNullOrFullyEmpty())
                        {
                            // We need to complete the member

                            MemberOperationResult userCompletionResult = Users.CompleteEsnsUser(member.CMID, email, password, true);

                            if (!userCompletionResult.Equals(MemberOperationResult.Ok))
                            {
                                errorMessage = userCompletionResult.GetMemberOperationErrorMessage();
                            }
                        }
                        else
                        {
                            if (!member.Login.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                            {
                                // The user modified his email address

                                MemberOperationResult emailModificationresult = UsersBusiness.ChangeMemberEmail(member.CMID, email, Request.UserHostAddress);

                                if (!emailModificationresult.Equals(MemberOperationResult.Ok))
                                {
                                    errorMessage = emailModificationresult.GetMemberOperationErrorMessage();
                                }                                    
                            }

                            if (member.Password != CryptographyUtilities.HashPassword(password))
                            {
                                bool passwordModificationResult = Users.ChangePassword(member.CMID, password);

                                if (!passwordModificationResult)
                                {
                                    errorMessage = "That doesn't look like a valid password to me!";
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Hmm. Something appears to have gone wrong. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
                        CmuneLog.LogUnexpectedReturn(facebookId, "This member is missing");
                    }
                }
            }

            return new JsonResult() { Data = new { ErrorMessage = errorMessage } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoginFromExternal(string email, string password, string returnUrl, ChannelType channel)
        {
            string message = String.Empty;
            bool isSuccessfull = Login(email, password, channel, out message);

            if (isSuccessfull)
            {
                if (channel == ChannelType.WindowsStandalone)
                {
                    if (returnUrl != null)
                    {
                        return new JsonResult() { Data = new { IsSuccessfull = isSuccessfull, Url = Url.Action("Index", "Epin", new { @lang = "tr-TR" }) } };
                    }
                    return new JsonResult() { Data = new { IsSuccessfull = isSuccessfull, Url = Url.Action("Index", "Bundle", new { @TransactionType = "Credit" }) } };
                }
            }

            return new JsonResult() { Data = new { IsSuccessfull = isSuccessfull, ErrorMessage = message } };
        }

        private bool Login(string email, string password, ChannelType channel, out string message)
        {
            bool isSuccessfull = false;
            message = String.Empty;
            Member member = null;
            MemberAuthenticationResult memberAuthResult = CmuneMember.CmuneLoginEmail(email, password, UberStrikeCommonConfig.ApplicationId, out member);

            if (memberAuthResult.Equals(MemberAuthenticationResult.Ok))
            {
                UberStrike.DataCenter.DataAccess.User uberstrikeMember = Users.GetUser(member.CMID);

                if (uberstrikeMember != null)
                {
                    AuthenticationManager.Login(member.CMID, channel);
                    isSuccessfull = true;
                    message = "You are successfully connected";
                }
                else
                {
                    CmuneLog.LogUnexpectedReturn(member.CMID, "Missing user in UberStrike Users table");
                    message = "An error occured with you account, please visit " + CommonConfig.CmuneSupportCenterUrl;
                }
            }
            else
            {
                if (memberAuthResult == MemberAuthenticationResult.IsBanned)
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
                    message = memberAuthResult.GetMemberRegistrationErrorMessage();
                }
            }

            return isSuccessfull;
        }
    }
}