using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Portal.Models;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;

namespace UberStrike.Channels.Portal.Controllers
{
    public class FirstAuthenticationController : Controller
    {
        //
        // GET: /FirstAuthentication/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(FirstAuthModel firstAuth)
        {
            if (firstAuth.FirstAuthLogin == ConfigurationUtilities.ReadConfigurationManager("BetaLogin") && firstAuth.FirstPasswordLogin == ConfigurationUtilities.ReadConfigurationManager("BetaPassword"))
            {
                Response.Cookies.Add(new HttpCookie(ConfigurationUtilities.ReadConfigurationManager("UberstrikeStagingAccessCookie"),
                    Crypto.fncSHA256Encrypt(firstAuth.FirstAuthLogin + firstAuth.FirstPasswordLogin)));
                Response.Redirect("~/");
            }
            else
            {
                ViewBag.FirstLoginFailed = "Wrong credentials !";
            }
            
            return View("Index");
        }
    }
}
