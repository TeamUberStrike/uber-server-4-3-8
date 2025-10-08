using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using SendGridSdk;
using UberStrike.WebService.DotNet;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class UtilityController : BaseController
    {
        #region Constructors

        public UtilityController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Utilities;
            ViewBag.SubActiveTab = CmuneMenu.UtilitiesSubTabs.Email;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Methods

        #region Email sending

        [ValidateInput(false)]
        public JsonResult SendSingleEmail(string fromEmailAddress, string fromName, string toEmailAddress, string toName, string subject, string htmlBody, string textBody)
        {
            bool isSent = true;
            String message = String.Empty;

            SendGridMail.SendSingleEmail(toEmailAddress, toName, fromEmailAddress, fromName, subject, htmlBody, textBody);

            return new JsonResult() { Data = new { IsSent = isSent, Message = message } };
        }

        public JsonResult SendMultipleEmails()
        {
            bool isSent = true;
            String message = String.Empty;

            string subject = @"Welcome to the party, #name#!";
            string htmlBody = @"<html><body><p>Welcome to the party, #name#!</p><p>Click on my <a href=""#link#"">link</a>.</p></body></html>";
            string textBody = @"Welcome to the party, #name#!\n\nClick on my link #link#.";
            List<string> recipients = new List<string> { "lvturner@me.com", "lee@cmune.com"};
            Dictionary<string, List<string>> variables = new Dictionary<string,List<string>> {
                {"#name#", new List<string> { "LeeMe", "LeeCmune"}},
                {"#link#", new List<string> { "http://www.google.com/", "http://www.facebook.com/"}}
            };

            SendGridMail.SendMultipleEmails(CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, subject, htmlBody, textBody, recipients, variables, new List<string>());

            return new JsonResult() { Data = new { IsSent = isSent, Message = message } };
        }

        [HttpGet]
        public void TestSendGrid()
        {
            //IList<String> bounces = SendGridBounces.GetBounces(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02));
            //bool didBounce = SendGridBounces.DidEmailAddressBounce("caca@cmune.com");
            IList<String> blocks = SendGridBlocks.GetBlocks(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02));
            //IList<String> invalidEmailAddresses = SendGridInvalidEmails.GetInvalidEmails(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02));
            //IList<String> spamEmailAddresses = SendGridSpamReports.GetSpamReports(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02));
            //IList<DeliverabilityStatistics> stats = SendGridStatistics.GetStats(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02));
            //DeliverabilityStatistics stats = SendGridStatistics.GetStats();
            //IList<String> categories = SendGridStatistics.GetCategories();
            //IList<DeliverabilityStatistics> categoriesStats = SendGridStatistics.GetCategoriesStats(new DateTime(2012, 03, 02), new DateTime(2012, 03, 02), new List<string> { "Newsletter" });
            //IList<DeliverabilityStatistics> categoriesStats = SendGridStatistics.GetCategoriesStats(new List<string> { "Newsletter" });
        }

        #endregion

        #endregion
    }
}