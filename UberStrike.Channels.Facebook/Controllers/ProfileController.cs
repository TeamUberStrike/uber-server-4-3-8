using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Business.Views;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Business;
using Facebook.Web.Mvc;
using System.Web.Caching;
using UberStrike.Channels.Facebook.Utils;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class ProfileController : BaseController
    {
        //
        // GET: /Profile/
        public ProfileController()
            : base()
        {
            base.CurrentTab = Utils.Menu.MainTab.Profile;
        }

        public ActionResult Index(int cmid = 0)
        {
            if (cmid == 0)
            {
                cmid = FbUserModel.Cmid;
            }

            ViewBag.Cmid = cmid;
            if (Request.IsAjaxRequest())
                return PartialView();
            // in case its not a Ajax request
            IsUberstrikeMenuEnable = false;
            IsFriendsCarouselEnable = false;
            IsUnityEmbedEnable = false;
            ViewBag.LoadMainTab = Menu.MainTab.Profile;
            base.CurrentTab = Utils.Menu.MainTab.Home;
            return View("LoadPage");
        }
    }
}
