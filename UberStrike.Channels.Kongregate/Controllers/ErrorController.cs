using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
           return View("Error");
        }

        public ActionResult NotFound()
        {
            return View("NotFound");
        }

    }
}
