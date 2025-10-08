using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Facebook.Utils;
using System.Net;
using Facebook.Web;
using System.IO;
using Facebook;
using Facebook.Web.Mvc;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using System.Web.Caching;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class FriendController : BaseController
    {
        #region Properties and Parameters
       
        #endregion

        #region Constructors
        public FriendController() : base()
        {
            base.CurrentTab = Utils.Menu.MainTab.Friend;
        }
        #endregion

        #region Methods
        #region Action

        //
        // GET: /Friend/

        public ActionResult LoadUsingAppFriends()
        {
            // We insert the current user at the 1st place to display him 1st
            //facebookFriendsIdsUsingApp.Insert(0, FacebookWebContext.Current.Session.UserId);

            List<FacebookBasicStatisticView> stats = new List<FacebookBasicStatisticView>();
            var cacheKey = FbUserModel.Cmid + "FacebookFriendLists";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                stats = (List<FacebookBasicStatisticView>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                var facebookFriendsUsingApp = FacebookSDKWrapper.GetAppUsersFriends();
                facebookFriendsUsingApp.Insert(0, new FacebookBasicStatisticView() { FacebookId = FbUserModel.FacebookId, FirstName = FbUserModel.FirstName, PicturePath = FbUserModel.PicturePath });
                stats = Statistics.GetFacebookBasicStatistics(facebookFriendsUsingApp, 7);
                HttpRuntime.Cache.Add(cacheKey, stats, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
             return View(stats);
        }

        public ActionResult Invite()
        {
            return View();
        }

        public ActionResult InviteByRequest2_0()
        {
            return View("InviteByRequest2_0");
        }

        #endregion
        #endregion


    }
}
