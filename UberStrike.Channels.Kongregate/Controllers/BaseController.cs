using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils;
using UberStrike.Channels.Utils.Models;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class BaseController : Controller
    {
        public string KongregateUserId { get; set; }
        public string KongregateGameAuthToken { get; set; }
        public string Random { get; set; }
        public UserProfileModel UserProfileModel { get; set; } 

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(HttpContextFactory.Current.Request.Params["kongregate_user_id"]))
            {
                KongregateUserId = Request.Params["kongregate_user_id"];
            }
            if (!string.IsNullOrEmpty(HttpContextFactory.Current.Request.Params["kongregate_game_auth_token"]))
            {
                KongregateGameAuthToken = Request.Params["kongregate_game_auth_token"];
            }
            if (!string.IsNullOrEmpty(HttpContextFactory.Current.Request.Params["random"]))
            {
                Random = Request.Params["random"];
            }

            ViewBag.DefaultCulture = ConfigurationUtilities.ReadConfigurationManager("DefaultCulture");
            ViewBag.IsDevEnv = (ConfigurationUtilities.ReadConfigurationManager("CmuneAPIKey").ToLower() == "dev"); 

            if (filterContext.Controller.ViewBag.UserProfile != null)
            {
                UserProfileModel = (UserProfileModel)filterContext.Controller.ViewBag.UserProfile;
            }
        }
    }
}
