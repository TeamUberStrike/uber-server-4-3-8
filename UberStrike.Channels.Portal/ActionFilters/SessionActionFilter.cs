using System;
using System.Web.Mvc;
using System.Web.Security;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Facebook.Web;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Utils.Models;
using UberStrike.DataCenter.Business;

namespace UberStrike.Channels.Portal.ActionFilters
{
    public class SessionActionFilter : ActionFilterAttribute
    {
        private const string ExtendedPermissions = "publish_stream,email,publish_actions";

        private bool IsAuthenticatedByFacebook(ActionExecutingContext filterContext)
        {
            bool isAuthenticatedByFacebook = false;

            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string[] userData = filterContext.HttpContext.User.Identity.Name.Split(SessionVariables.AuthCookieSeparator);
                int cmid = int.Parse(userData[0]);
                ChannelType channel = (ChannelType)Enum.Parse(typeof(ChannelType), userData[1]);

                if (channel == ChannelType.WebFacebook)
                    isAuthenticatedByFacebook = true;
            }

            return isAuthenticatedByFacebook;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var returnUrl = HttpContextFactory.Current.Request.Params["returnUrl"] != null ? HttpContextFactory.Current.Request.Params["returnUrl"] : string.Empty;

            // Save the referrer information
            if (!HttpContextFactory.GetReferrer().IsNullOrFullyEmpty())
            {
                HttpContextFactory.SetCookieForShortDuration("Referrer", HttpContextFactory.GetReferrer());
            }

            // auto login
            if (filterContext.HttpContext.Request.Params["autoLogin"] != null)
            {
                AuthenticationManager.Logout();
                //filterContext.Result = new RedirectResult("~/?execAutoLogin=true&returnUrl=" + returnUrl);
                filterContext.Result = new RedirectResult("~/Account/FbLogin?returnUrl=" + returnUrl);
                return;
            }

            if (filterContext.HttpContext.Request.Params["execAutoLogin"] != null)
            {
                filterContext.Controller.ViewBag.ExecAutoLogin = true;
                filterContext.Controller.ViewBag.ReturnUrl = returnUrl;
                return;
            }

            // We need to logout members when they access external login (via email or Facebook)
            if (HttpContextFactory.Current.Request.IsAuthenticated
                && (HttpContextFactory.Current.Request.Path.ToLower().Contains("account/externallogin") || HttpContextFactory.Current.Request.Path.ToLower().Contains("account/facebooklogin")))
            {
                AuthenticationManager.Logout();
                return;
            }

            /* If the member:
             *  1) is not authenticated on the portal
             *  2) and he has a valid Facebook session
             *  3) and he was redirected after being authenticated on Facebook
             */
            if ((filterContext.HttpContext.Request.Params["facebookExternalLogin"] != null)
                && FacebookWebContext.Current.IsAuthenticated()
                && !filterContext.HttpContext.Request.IsAuthenticated)
            {
                var facebookWrapper = new FacebookSdkWrapper();
                var facebookUserModel = facebookWrapper.GetFacebookUserModel();
                int cmid = CmuneMember.GetCmidByFacebookId(facebookUserModel.FacebookId);

                // We need to ensure that the member already has a UberStrike account

                if (cmid == 0)
                {
                    filterContext.Result = new RedirectResult("~/Account/NoAccount");
                    return;
                }

                if (!returnUrl.IsNullOrFullyEmpty())
                    filterContext.Result = new RedirectResult(returnUrl);
                else
                    filterContext.Result = new RedirectResult("~/");

                return;
            }

            // facebook authentication part
            if ((filterContext.HttpContext.Request.Params["facebookLogin"] != null) && FacebookWebContext.Current.IsAuthenticated() && !filterContext.HttpContext.Request.IsAuthenticated)
            {
                var facebookSDKWrapper = new FacebookSdkWrapper();
                var fbUserModel = facebookSDKWrapper.GetFacebookUserModel();
                int cmid = CmuneMember.GetCmidByFacebookId(fbUserModel.FacebookId);

                if (cmid == 0)
                {
                    var referrerLink = HttpContextFactory.GetCookie("Referrer");
                    var urlReferrer = referrerLink != null ? referrerLink : string.Empty;
                    var userHostAddress = TextUtilities.InetAToN(HttpContextFactory.Current.Request.UserHostAddress);

                    MemberRegistrationResult ret = Users.CreateUserFromFacebook(fbUserModel.FacebookId, urlReferrer, fbUserModel.Email, fbUserModel.FirstName, fbUserModel.LastName, userHostAddress, LocaleIdentifier.EnUS, out cmid);

                    if (ret == MemberRegistrationResult.DuplicateEmail)
                    {
                        filterContext.Result = new RedirectResult("~/Account/EmailDuplication?email=" + fbUserModel.Email + "&facebookId=" + fbUserModel.FacebookId);
                    }
                    else if (ret != MemberRegistrationResult.Ok)
                    {
                        filterContext.Result = new RedirectResult("~/Account/CreationError?error=" + ret.ToString());
                    }
                }

                if (cmid != 0) // yes user is authenticated
                {
                    fbUserModel.Cmid = cmid;
                    FormsAuthentication.SetAuthCookie(cmid.ToString() + SessionVariables.AuthCookieSeparator + ChannelType.WebFacebook, true);
                    HttpContextFactory.ClearSession(SessionVariables.UserProfile);
                    if (!returnUrl.IsNullOrFullyEmpty())
                        filterContext.Result = new RedirectResult(returnUrl);
                    else
                        filterContext.Result = new RedirectResult("~/");
                }

                return;
            }
            else if (!FacebookWebContext.Current.IsAuthenticated() && IsAuthenticatedByFacebook(filterContext))
            {
                AuthenticationManager.Logout();
                filterContext.Result = new RedirectResult("~/");
                return;
            }

            // authenticated part
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                string[] userData = filterContext.HttpContext.User.Identity.Name.Split(SessionVariables.AuthCookieSeparator);
                int cmid = int.Parse(userData[0]);
                ChannelType channelType = (ChannelType)Enum.Parse(typeof(ChannelType), userData[1]);

                if (HttpContextFactory.GetSession(SessionVariables.UserProfile) == null)
                {
                    var userProfile = new UserProfileModel();

                    if (cmid > 0) // in case when we are connected 
                    {
                        var member = CmuneMember.GetMember(cmid);
                        var id3 = CmuneMember.GetId3(cmid);

                        if (member != null)
                        {
                            userProfile.Member = member;
                            userProfile.User = Users.GetUser(cmid);
                            userProfile.ChannelType = channelType;
                            userProfile.UserName = id3.Name;
                        }
                    }
                    if (IsAuthenticatedByFacebook(filterContext))
                    {
                        var facebookSDKWrapper = new FacebookSdkWrapper();
                        var fbUserModel = facebookSDKWrapper.GetFacebookUserModel();
                        userProfile.FacebookUserModel = fbUserModel;
                    }

                    HttpContextFactory.SetSession(SessionVariables.UserProfile, userProfile);
                }

                filterContext.Controller.ViewBag.ReturnUrl = returnUrl;
                filterContext.Controller.ViewBag.Cmid = cmid;
                filterContext.Controller.ViewBag.UserProfileType = channelType;
                filterContext.Controller.ViewBag.UserProfile = HttpContextFactory.GetSession(SessionVariables.UserProfile);
            }
        }
    }
}