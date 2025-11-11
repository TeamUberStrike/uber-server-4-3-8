using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ClanController : BaseController
    {
        #region Properties and Parameters

        int _clansCount = 30;
        public int ClanId { get; set; }

        #endregion

        #region Constructors

        public ClanController() :base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Clans;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            ViewBag.Title += " | Search";

            return View();
        }

        public ActionResult See(int clanId)
        {
            ViewBag.Title += " | See";
            bool isClanLeaderNotifiable = true;

            ClanView clan = CmuneGroups.GetGroupView(clanId);

            if (clan != null)
            {
                ViewData["memberCmid"] = clan.OwnerCmid.ToString();
                ViewData["memberEmail"] = String.Empty;
                ViewData["memberName"] = String.Empty;
                ViewData["clanName"] = clan.Name;
                ViewData["clanTag"] = clan.Tag;
                ViewData["clanMotto"] = clan.Motto;
                ViewData["clanId"] = clan.GroupId;

                Member clanLeader = CmuneMember.GetMember(clan.OwnerCmid);

                if (clanLeader != null)
                {
                    if (CmuneMember.IsMemberEsnsOnly(clanLeader))
                    {
                        isClanLeaderNotifiable = false;
                    }
                    else
                    {
                        ViewData["ownerEmailHiddenField"] = clanLeader.Login;
                        ViewData["ownerNameHiddenField"] = clan.OwnerName;
                    }
                }

                ViewData["notifiable"] = isClanLeaderNotifiable;
                ViewData["clanMembers"] = clan.Members;

                ViewData["IsClanExisting"] = true;
            }
            else
            {
                ViewData["IsClanExisting"] = false;
            }

            return View();
        }

        #endregion

        #region Methods

        #region Search

        [ValidateInput(false)]
        public ActionResult GetClans(FormCollection form)
        {
            string clanName = form["ClanNameTextBox"];
            string clanTag = form["ClanTagTextBox"];
            string clanMemberName = form["ClanMemberNameTextBox"];

            List<BasicClanView> clans = CmuneGroups.GetBasicClansView(UberStrikeCommonConfig.ApplicationId, 0, _clansCount, clanName, clanTag, clanMemberName);

            return PartialView("Partial/GetClans", clans);
        }

        #endregion

        #region See

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection form)
        {
            string action = string.Empty;
            string message = string.Empty;
            bool isModified = false;
            string newValue = string.Empty;

            action = form["action"];

            ClanId = Int32.Parse(form["clanId"]);
            
            int groupOperationResult = 0;
            bool notify = false;
            string explanation = string.Empty;

            bool actionDone = false;
            switch (form["action"])
            {
                case "changeTag":
                    if (form["newClanTag"] != form["clanTag"] && !String.IsNullOrEmpty(form["newClanTag"]))
                    {
                        groupOperationResult = UberStrikeGroups.SetGroupTag(ClanId, form["newClanTag"], LocaleIdentifier.EnUS);

                        if (form["notifyForNewClanTag"] != null && form["notifyForNewClanTag"].ToString().ToLower() == "on")
                            notify = true;
                        explanation = form["explanationForNewClanTag"];
                        actionDone = true;
                    }
                    break;
                case "changeName":
                    if (form["newClanName"] != form["clanName"] && !String.IsNullOrEmpty(form["newClanName"]))
                    {
                        groupOperationResult = CmuneGroups.SetGroupName(ClanId, form["newClanName"], LocaleIdentifier.EnUS);
                        if (form["notifyForNewClanName"] != null && form["notifyForNewClanName"].ToString().ToLower() == "on")
                            notify = true;
                        explanation = form["explanationForNewClanName"];
                        actionDone = true;
                    }
                    break;
                case "changeMotto":
                    if (form["newClanMotto"] != form["clanMotto"] && !String.IsNullOrEmpty(form["newClanMotto"]))
                    {
                        groupOperationResult = CmuneGroups.SetGroupMotto(ClanId, form["newClanMotto"]);
                        if (form["notifyForNewClanMotto"] != null && form["notifyForNewClanMotto"].ToString().ToLower() == "on")
                            notify = true;
                        explanation = form["explanationForNewClanMotto"];
                        actionDone = true;
                    }
                    break;
            }

            if (actionDone == true)
            {
                CheckOperationResult.CheckGroupOperationResult(groupOperationResult, ref isModified, ref message);
                if (notify == true && groupOperationResult == GroupOperationResult.Ok)
                {
                    CmuneMail.SendEmailNotification(EmailNotificationType.ChangeClanMotto, explanation, form["memberEmail"], form["memberName"], EmailAddressStatus.Invalid);
                }
            }

            var json = new JsonResult() { Data = new { isModified = isModified, message = message, action = form["action"], newValue = newValue } };
            return json;
        }

        [HttpPost]
        public ActionResult Delete(int clanId, int cmid)
        {
            bool isModified = false;
            string message = string.Empty;

            CmuneGroups.DisbandGroup(clanId, cmid);
            isModified = true;
            message = "The clan has been disbanded, reloading the page...";

            var json = new JsonResult() { Data = new { IsModified = isModified, Message = message, ReturnUrl = ApplicationPath + "Clan/Search"} };
            return json;
        }

        [HttpPost]
        public JsonResult MakeLeader(int clandId, int oldLeaderCmid, int newLeaderCmid)
        {
            int result = CmuneGroups.TransferOwnership(clandId, oldLeaderCmid, newLeaderCmid);

            bool isOk = result == GroupOperationResult.Ok;

            JsonResult ret = new JsonResult { Data = new { IsOk = isOk } };
            return ret;
        }

        [HttpPost]
        public JsonResult KickFromClan(int clandId, int cmid)
        {
            int result = CmuneGroups.LeaveAClan(clandId, cmid);

            bool isOk = result == GroupOperationResult.Ok;

            JsonResult ret = new JsonResult { Data = new { IsOk = isOk } };
            return ret;
        }

        #endregion

        #endregion
    }
}