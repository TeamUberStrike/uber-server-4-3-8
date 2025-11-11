using System.Web.Mvc;
using System;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Business.Views;
using UberStrike.DataCenter.Business;
using System.Web;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Portal.Models;
using System.Collections.Generic;
using System.Web.Security;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.Channels.Portal.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index(int cmid = 0)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                }
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    ID3 memberId3 = CmuneMember.GetId3(cmid, cmuneDb);

                    ViewBag.ProfileName = memberId3.Name;
                }
                ViewBag.Cmid = cmid;
            }
            catch(Exception)
            {
                ViewBag.HasError = true;
            }
            return View();
        }
    }
}