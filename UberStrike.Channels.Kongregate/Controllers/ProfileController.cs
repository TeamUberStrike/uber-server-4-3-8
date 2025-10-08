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
using System.Web.Caching;
using UberStrike.Channels.Utils.Models;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class ProfileController : BaseController
    {
        public ActionResult Index(int cmid = 0)
        {
            var userProfile = (UserProfileModel)ViewBag.UserProfile;
            ViewBag.Cmid = userProfile.Member.CMID;
            return View();
        }
    }
}
