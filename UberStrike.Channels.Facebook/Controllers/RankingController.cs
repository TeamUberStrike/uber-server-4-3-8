using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UberStrike.Channels.Facebook.Controllers
{
    public class RankingController : Controller
    {
        //
        // GET: /Ranking/

        public ActionResult Index()
        {
            return PartialView();
        }

    }
}
