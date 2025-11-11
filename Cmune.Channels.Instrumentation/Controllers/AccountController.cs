using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.Models.Enums;

namespace Cmune.Channels.Instrumentation.Controllers
{
    public class AccountController : BaseController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        public AccountController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Utilities;
            ViewBag.SubActiveTab = CmuneMenu.UtilitiesSubTabs.Email; ;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Zero()
        {
            throw new DivideByZeroException();
        }

        public ActionResult LogOn(string returnUrl)
        {
            // choose the page where to redirect if authenticated
            if (Request.IsAuthenticated)
            {
                if (IsAdministrator())
                    return RedirectToAction("Index", "Statistic");
            }

            ViewBag.ReturnUrl = returnUrl;

            // automatically log in local
            if (CmuneEnvironnement == "dev")
                return RedirectToAction("LogIn", "Account", new { returnUrl = new UrlHelper(Request.RequestContext).Action("Index", "Statistic") });
            return View();
        }

        public ActionResult LogIn(LogOnModel model, string returnUrl)
        {
            // automatically log in local, this is not set in LogOn to avoid giving credential by client
            if (CmuneEnvironnement == "dev")
            {
                model.UserName = "administrator";
                model.Password = "BurgerBlast99";
                ModelState.Clear();
            }

            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("LogOn", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            ViewBag.ReturnUrl = returnUrl;

            // If we got this far, something failed, redisplay form
            return View("LogOn", model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("LogOn", "Account");
        }

    }
}