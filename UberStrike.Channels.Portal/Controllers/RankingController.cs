using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using System.Web.Caching;
using UberStrike.Channels.Portal.Models;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Portal.Controllers
{
    public class RankingController : Controller
    {
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}