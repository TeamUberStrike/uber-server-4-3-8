using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Utils.Models;
using Cmune.DataCenter.Business;

namespace UberStrike.Channels.Kongregate.Controllers
{
    public class OfferController : Controller
    {
        //
        // GET: /Offer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ItemBundles()
        {
            return View();
        }

        public ActionResult UseItem()
        {
            return View();
        }

        public ActionResult GetBundleUniqueId(int bundleId)
        {
            return new JsonResult() { Data = new { bundleUniqueId =  CmuneBundle.GetMacAppStoreUniqueId(bundleId) }};
        }
    }
}
