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
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class AdminMemberController : BaseController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        public AdminMemberController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Utilities;
            ViewBag.SubActiveTab = CmuneMenu.UtilitiesSubTabs.AdminMember;
            ViewBag.AdminRoles = Roles.Provider.GetAllRoles().ToList();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = MembershipRoles.Administrator)]
        public ActionResult Register()
        {
            ViewBag.Title = "Register";
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View("Index");
        }

        [Authorize(Roles = MembershipRoles.Administrator)]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Roles.Provider.AddUsersToRoles(new String[] { model.UserName }, new String[] { model.Role });
                    ViewBag.SuccessRegistration = true;
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            ViewBag.RegisterModel = model;

            return View("Index");
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Change Password";
            ViewData["MinPasswordLength"] = MembershipService.MinPasswordLength;

            return View("Index");
        }

        [HttpPost]
        public ActionResult ChangePassword(string username, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            ViewBag.Title = "Change Password";

            string statusMessage = String.Empty;
            bool isChanged = false;

            if (Request.IsAuthenticated)
            {
                if (newPassword != string.Empty && newPasswordConfirm != string.Empty)
                {
                    if (newPassword == newPasswordConfirm)
                    {
                        MembershipUser currentUser = Membership.Provider.GetUser(membershipUser.UserName, true /* userIsOnline */);
                        //string newRandomPassword = currentUser.ResetPassword();

                        isChanged = MembershipService.ChangePassword(membershipUser.UserName, oldPassword, newPassword);

                        if (isChanged)
                        {
                            statusMessage = "You changed your password";
                        }
                        else
                        {
                            statusMessage = "Password wasn't changed: either wrong old password or new passord doesn't comply to rules";
                        }
                    }
                    else
                    {
                        statusMessage = "The passwords don't match";
                    }
                }
                else
                {
                    statusMessage = "Both Password and Confirmed password must be provided";
                }
            }
            else
            {
                statusMessage = "You're not authenticated";
            }

            ViewData["MinPasswordLength"] = MembershipService.MinPasswordLength;
            ViewData["IsChanged"] = isChanged;
            ViewData["StatusMessage"] = statusMessage;

            return View("Index");
        }
    }
}
