using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Entities;
using System.Web.Security;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Channels.Utils;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using System.Text;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Channels.Utils.Models;
using Newtonsoft.Json;
using UberStrike.Channels.Kongregate.Helpers;

namespace UberStrike.Channels.Kongregate.ActionFilters
{
    public class KongregateAuthentication
    {
        public bool success;
        public string user_id;
        public string username;
        public int error;
        public string error_description;
    }

    public class SessionActionFilter : ActionFilterAttribute
    {
        private KongregateAuthentication AuthenticateOnKongregate(string userId, string gameAuthToken, string apiKey, string random = null)
        {
            string authenticationUrl = "http://www.kongregate.com/api/authenticate.json?";
            authenticationUrl += "user_id=" + userId;
            authenticationUrl += "&game_auth_token=" + gameAuthToken;
            authenticationUrl += "&api_key=" + apiKey;

            var webGetRequest = new WebGetRequest(authenticationUrl);
            KongregateAuthentication kongregateAuth = (KongregateAuthentication)JsonConvert.DeserializeObject(webGetRequest.GetResponse(), typeof(KongregateAuthentication));

            return kongregateAuth;
        }

        private string GenerateKongregateEmail(string kongregateId)
        {
            return kongregateId + "@kongregate.fake.com";
        }

        //private bool KongregateGetUserInformation()
        //{
        //    string url = "";
        //    return true;
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var kongregateUserId = HttpContextFactory.Current.Request.Params["kongregate_user_id"];
            var kongregateUsername = HttpContextFactory.Current.Request.Params["kongregate_username"];
            var kongregateGameAuthToken = HttpContextFactory.Current.Request.Params["kongregate_game_auth_token"];
            var random = HttpContextFactory.Current.Request.Params["random"];
            int cmid = 0;

            if (!kongregateUserId.IsNullOrFullyEmpty() && !kongregateGameAuthToken.IsNullOrFullyEmpty())
            {
                var authenticationOnKongregate = AuthenticateOnKongregate(kongregateUserId, kongregateGameAuthToken, ConfigurationUtilities.ReadConfigurationManager("APIKey"));
                if (authenticationOnKongregate.success)
                {

                    // check user against session, find out new user
                    UserProfileModel userProfilTmp = (UserProfileModel)HttpContextFactory.GetSession("KongregateChannelUserProfile");
                    if (userProfilTmp != null && userProfilTmp.KongregateUserModel.KongregateId != kongregateUserId)
                    {
                        HttpContextFactory.ClearSession("KongregateChannelUserProfile");
                    }

                    cmid = CmuneMember.GetCmidByKongregateId(kongregateUserId);
                    if (cmid == 0)
                    {
                        HttpContextFactory.ClearSession("KongregateChannelUserProfile");
                        var kongregateAuthentication = AuthenticateOnKongregate(kongregateUserId, kongregateGameAuthToken, ConfigurationUtilities.ReadConfigurationManager("APIKey"), random);
                        if (kongregateAuthentication.success)
                        {
                            MemberRegistrationResult ret = Users.CreateUserFromKongregate(kongregateUserId, HttpContextFactory.GetReferrer(), GenerateKongregateEmail(kongregateUserId), "", "", "", TextUtilities.InetAToN(HttpContextFactory.Current.Request.UserHostAddress), LocaleIdentifier.EnUS, out cmid);
                            if (ret != MemberRegistrationResult.Ok)
                            {
                                filterContext.Result = new RedirectResult("~/Account/CreationError?error=" + ret.ToString());
                                return;
                            }
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult("~/Account/CreationError?error=" + "not valid Kongregate account");
                            return;
                        }
                    }
                    //&& !string.IsNullOrEmpty(HttpContextFactory.Current.Request["callBack"])
                    //else if (cmid == 0)
                    //{
                    //    filterContext.Result = new RedirectResult("~/Account/ConnectWith?kongregateId=" + kongregateUserId);
                    //    return;
                    //}

                    if (cmid > 0)
                    {
                        if (HttpContextFactory.GetSession("KongregateChannelUserProfile") == null)
                        {
                            var member = CmuneMember.GetMember(cmid);
                            var id3 = CmuneMember.GetId3(cmid);
                            var userProfile = new UserProfileModel();
                            filterContext.Controller.ViewBag.KongregateId = kongregateUserId;

                            if (member != null)
                            {
                                userProfile.Member = member;
                                userProfile.User = Users.GetUser(cmid);
                        userProfile.ChannelType = ChannelType.Kongregate;
                                userProfile.UserName = id3.Name;
                                userProfile.KongregateUserModel = new KongregateUserModel() { KongregateId = kongregateUserId };
                            }
                            HttpContextFactory.SetSession("KongregateChannelUserProfile", userProfile);
                        }
                        filterContext.Controller.ViewBag.UserProfile = HttpContextFactory.GetSession("KongregateChannelUserProfile");
                    }
                }
                else
                {
                    //filterContext.Result = new RedirectResult("~/Home/GetGuestPage");
                    throw new Exception("Not authenticated on Kongregate: ERROR:" + authenticationOnKongregate.error + " ERROR_DESCRIPTION:" + authenticationOnKongregate.error_description);
                }
            }
            else
            {
                throw new Exception("Authentication credentials are missing: KGUserId:" + kongregateUserId + " KGGameAuthToken:" + kongregateGameAuthToken);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}