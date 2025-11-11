using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Facebook.Controllers
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

        public ActionResult Reload(string thing1, string thing2)
        {
            string query = string.Empty;
            if (Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.Query) && !Request.UrlReferrer.Query.Contains("signed_request"))
                query = Request.UrlReferrer.Query;
            ViewBag.Query = query;
            ViewBag.Url = new FacebookSdkWrapper().GetFacebookApplicationUrl();
            return View();
        }
    }
}
