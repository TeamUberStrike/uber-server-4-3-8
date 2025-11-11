using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using UberStrike.Channels.Facebook.Models;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Channels.Facebook.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using Facebook.Web.Mvc;
using Facebook.Web;
using Newtonsoft.Json;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Utils.Models;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Facebook;
using Cmune.DataCenter.Common.Utils.Cryptography;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class BaseController : Controller
    {
        public string Environment;
        public string DefaultCulture;
        public string UrlReferrerQueryString;
        protected int RefPartnerId;
        protected string ApplicationPath;
        // initalized at OnActionExecuting
        protected long FacebookApplicationId;
        protected string SignedRequest;
        protected FacebookUserModel FbUserModel;
        protected CmuneCookie CmuneCookie;

        protected FacebookSdkWrapper FacebookSDKWrapper;

        // page display setting, by default not displayed
        protected bool IsLikeButtonEnable;
        protected bool IsUberstrikeMenuEnable;
        protected bool IsFriendsCarouselEnable;
        protected bool IsApplifierPromoEnable;
        protected bool IsUnityEmbedEnable;
        protected Menu.MainTab CurrentTab;

        bool isProdEnv()
        {
            return Environment.ToLower() == "prod";
        }

        bool isDevEnv()
        {
            return Environment.ToLower() == "dev";
        }

        public BaseController()
        {
            Environment = ConfigurationUtilities.ReadConfigurationManager("CmuneAPIKey", true);
            DefaultCulture = ConfigurationUtilities.ReadConfigurationManager("UnityDefaultCulture");
            FacebookSDKWrapper = new FacebookSdkWrapper();
            CurrentTab = Menu.MainTab.Home;
        }

        void configurePageDisplay()
        {
            if (FbUserModel == null)
                throw new ArgumentNullException("FbUserModel is not initalized");

            if (ConfigurationUtilities.ReadConfigurationManagerBool("IsApplifierIsEnabled"))
            {
                IsApplifierPromoEnable = true;
            }
        }

        bool AutoAuthentication(ActionExecutingContext filterContext)
        {
            if (FbUserModel == null)
                throw new ArgumentNullException("FbUserModel is not initalized");

            // Creates the user if he doesn't exist

            int cmid = CmuneMember.GetCmidByFacebookId(FbUserModel.FacebookId);

            if (cmid == 0)
            {
                var userHostAddress = TextUtilities.InetAToN(HttpContextFactory.Current.Request.UserHostAddress);

                MemberRegistrationResult ret = Users.CreateUserFromFacebook(FbUserModel.FacebookId, HttpContextFactory.GetReferrer(), FbUserModel.Email, FbUserModel.FirstName, FbUserModel.LastName, userHostAddress, LocaleIdentifier.EnUS, out cmid);

                if (ret == MemberRegistrationResult.DuplicateEmail)
                {
                    filterContext.Result = new RedirectResult("~/Account/EmailDuplication?email=" + FbUserModel.Email + "&facebookId=" + FbUserModel.FacebookId);
                    return false;
                }
                else if (ret != MemberRegistrationResult.Ok)
                {
                    filterContext.Result = new RedirectResult("~/Account/CreationError?error=" + ret.ToString());
                    return false;
                }

                // clear all cookie to prevent from old value, by the way unauthenticate the user
                HttpContextFactory.Current.Request.Cookies.Clear();
            }
            else
            {
                // complete the old fb accounts
                var member = CmuneMember.GetMember(cmid);
                if (member != null && String.IsNullOrEmpty(member.Login) && !String.IsNullOrEmpty(FbUserModel.Email))
                {
                    CmuneMember.RegisterEsnsMember(cmid, FbUserModel.Email, RandomPassword.Generate(6, 8));
                }
            }

            FbUserModel.Cmid = cmid;
            FbUserModel.IsAccountComplete = CmuneMember.IsAccountComplete(FbUserModel.Cmid);

            /*** authentication by cookie ***/
            CmuneCookie = AuthenticationCookieManagement.ValidateAuthCookie(HttpContextFactory.Current.Request, HttpContextFactory.Current.Response, ChannelType.WebFacebook);

            // user is authenticated
            if (CmuneCookie != null && CmuneCookie.IsFullyValid(FbUserModel.FacebookId.ToString()))
            {
                // set the cmid thats mean the model is authenticated
                FbUserModel.Id3PlayerName = CmuneMember.GetUserName(FbUserModel.Cmid);
            }
            else
            {
                // Authentication : try to check if user exists in database, occured first login and when user is not the owner
                Member memberPlaying = CmuneMember.GetMemberByFacebookId(FbUserModel.FacebookId);
                if (memberPlaying != null) // yes user is authenticated
                {
                    CmuneCookie = AuthenticationCookieManagement.SetAuthCookie(Request, HttpContextFactory.Current.Response, cmid, ChannelType.WebFacebook, FbUserModel.FacebookId.ToString());
                    FbUserModel.Id3PlayerName = CmuneMember.GetUserName(FbUserModel.Cmid);
                }
                else
                {
                    // if user is deleted then reset the cookie
                    CmuneCookie = AuthenticationCookieManagement.UnsetAuthCookie(Request, HttpContextFactory.Current.Response, ChannelType.WebFacebook);
                }
            }

            return FbUserModel.IsAuthenticated;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(Request.Params["signed_request"]))
                throw new FacebookOAuthException();
            SignedRequest = Request.Params["signed_request"];

            FacebookApplicationId = FacebookSDKWrapper.GetFacebookApplicationId();
            FbUserModel = FacebookSDKWrapper.GetFacebookUserModel();
            RefPartnerId = 0;
            IsLikeButtonEnable = true;
            IsUberstrikeMenuEnable = true;
            IsFriendsCarouselEnable = false;
            IsUnityEmbedEnable = true;
            if (Request.UrlReferrer != null)
            {
                UrlReferrerQueryString = new Uri(Request.UrlReferrer.AbsoluteUri).Query;
                if (UrlReferrerQueryString.StartsWith("?"))
                    UrlReferrerQueryString = UrlReferrerQueryString.Substring(1, UrlReferrerQueryString.Length - 1);
            }
            ApplicationPath = HttpContext.Request.ApplicationPath == "/" ? "" : HttpContext.Request.ApplicationPath;
            // must be called at the end after initalisation of variable
            AutoAuthentication(filterContext);
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.IsDevEnv = isDevEnv();
            ViewBag.DefaultCulture = DefaultCulture;
            ViewBag.FacebookApplicationId = FacebookApplicationId;
            ViewBag.FacebookApplicationUrl = FacebookSDKWrapper.GetFacebookApplicationUrl();
            ViewBag.CurrentTab = CurrentTab;
            configurePageDisplay();
            ViewBag.RefPartnerId = RefPartnerId;
            //display
            ViewBag.IsLikeButtonEnable = IsLikeButtonEnable;
            ViewBag.IsUberstrikeMenuEnable = IsUberstrikeMenuEnable;
            ViewBag.IsFriendsCarouselEnable = IsFriendsCarouselEnable;
            ViewBag.IsApplifierPromoEnable = IsApplifierPromoEnable;
            ViewBag.IsUnityEmbedEnable = IsUnityEmbedEnable;
            // provide model
            ViewBag.FacebookUserModel = FbUserModel;
            ViewBag.SignedRequest = SignedRequest;
            ViewBag.UrlReferrerQueryString = UrlReferrerQueryString;
            ViewBag.ApplicationPath = ApplicationPath;
            base.OnActionExecuted(filterContext);
        }
    }
}