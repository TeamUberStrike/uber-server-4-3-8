using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Business;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Business.Views;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Utils;
using PPUsers = UberStrike.DataCenter.Business.Users;
using Cmune.Channels.Instrumentation.Extensions;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class MemberController : BaseController
    {
        #region Properties

        private int _applicationId = 1;
        private int _cmid = 0;

        public int ApplicationId
        {
            get { return _applicationId; }
            private set { _applicationId = value; }
        }

        public int Cmid
        {
            get { return _cmid; }
            private set { _cmid = value; }
        }

        #region (for create test account)

        const string TestAccountNameTemplate = "Test Account ";
        const string TestAccountEmailTemplate = "testaccount@@@INDEXER@@@@yopmail.com";
        static readonly List<int> DefaultGears = new List<int> { 1090, 1089, 1088, 1087, 1086, 1085, 1084 };
        static readonly List<int> DefaultWeapons = new List<int> { 1007, 1006, 1005, 1004, 1003, 1002, 1001, 1000 };
        const int DefaultCredits = 1000000;
        const int DefaultPoints = 1000000;
        const int DefaultLevel = 20;
        static readonly LoadoutView DefaultLoadout = new LoadoutView(0, 0, 1089, 0, 1085, 0, 0, 0, 1086, 1084, 1088, 1000, 0, 0, 0, AvatarType.LutzRavinoff, 1087, 1001, 0, 0, 0, 1002, 0, 0, 0, 0, 0, 0, 0, 0, "c69c6d");

        #endregion

        #endregion

        public MemberController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Members;
            ViewBag.PublishVersion = ConfigurationUtilities.ReadConfigurationManager("PublishVersion");

            UseProdDataContext();
        }

        #region Actions

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ActionResult ManageMemberAccess()
        {
            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.ManageMemberAccess;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.ManageMemberAccess);

            List<MemberAccess> membersAccess = CmuneMember.GetNonDefaultMemberAccess(CommonConfig.ApplicationIdUberstrike);
            List<int> cmids = membersAccess.Select(mA => mA.Cmid).ToList();
            Dictionary<int, string> names = CmuneMember.GetMembersNames(cmids);
            List<MemberAccessDisplay> accessLevelsDisplay = new List<MemberAccessDisplay>();

            foreach (MemberAccess memberAccess in membersAccess)
            {
                MemberAccessDisplay accessLevelDisplay = new MemberAccessDisplay(memberAccess.Cmid, names[memberAccess.Cmid], (MemberAccessLevel)memberAccess.AccessLevel);
                accessLevelsDisplay.Add(accessLevelDisplay);
            }

            ViewData["memberAccessLevelsDisplay"] = accessLevelsDisplay;

            List<SelectListItem> accessLevelList = new List<SelectListItem>();
            foreach (var item in AdminCache.GenerateMemberAccessLevelDropDownListItems())
                accessLevelList.Add(new SelectListItem() { Text = item.Text, Value = item.Value });
            ViewData["accessLevelList"] = accessLevelList;
            return View();
        }

        public ActionResult Search()
        {
            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.Search;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.Search);

            ViewData["esnsType"] = AdminCache.GenerateEsnsDropDownListItems();

            return View();
        }

        public ActionResult See(string cmid)
        {
            Stopwatch watch = Stopwatch.StartNew();
            long seeStart = watch.ElapsedMilliseconds;
            long getNameStart = 0;
            long getNameEnd = 0;
            long getIsMemberEsnsOnlyStart = 0;
            long getIsMemberEsnsOnlyEnd = 0;
            long getMemberAccessStart = 0;
            long getMemberAccessEnd = 0;
            long getCreditStart = 0;
            long getCreditEnd = 0;
            long getEsnsStart = 0;
            long getEsnsEnd = 0;
            long getRankingStart = 0;
            long getRankingEnd = 0;
            long getGroupStart = 0;
            long getGroupEnd = 0;
            long getGroupMemberStart = 0;
            long getGroupMemberEnd = 0;
            long getPreviousEmailsStart = 0;
            long getPreviousEmailsEnd = 0;
            long getNotesStart = 0;
            long getNotesEnd = 0;

            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.See;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.See);
            ApplicationId = 1;

            bool isCmidParsed = false;
            bool isMemberExisting = false;

            isCmidParsed = Int32.TryParse(cmid, out _cmid);

            Member member = null;
            ID3 id3 = null;

            if (isCmidParsed)
            {
                member = CmuneMember.GetMember(this.Cmid);

                getNameStart = watch.ElapsedMilliseconds;

                id3 = CmuneMember.GetId3(this.Cmid);

                getNameEnd = watch.ElapsedMilliseconds;

                isMemberExisting = member != null && id3 != null;
            }

            if (isMemberExisting)
            {
                MemberOriginDisplay memberOrigin = UserActivityBusiness.GetMemberOrigin(this.Cmid);
                ViewData["MemberOrigin"] = memberOrigin;

                getIsMemberEsnsOnlyStart = watch.ElapsedMilliseconds;

                bool isMemberEsnsOnly = CmuneMember.IsMemberEsnsOnly(member);

                getIsMemberEsnsOnlyEnd = watch.ElapsedMilliseconds;

                #region Basic information

                ViewData["memberCmid"] = this.Cmid;
                ViewData["memberName"] = id3.Name;
                ViewBag.EmailAddressState = member.EmailAddressState;
                ViewData["memberEmail"] = member.Login;
                ViewData["isEmailPasswordVisible"] = ViewData["isUsernameExplainationVisible"] = ViewData["isUsernameCheckBoxVisible"] = true;

                if (isMemberEsnsOnly == true) // means only sign up in social network
                {
                    ViewData["isEmailPasswordVisible"] = false;
                    ViewData["isUsernameExplainationVisible"] = false;
                    ViewData["isUsernameCheckBoxVisible"] = false;
                    ViewData["isCompleteUserLinkButtonVisibile"] = true;
                }

                getMemberAccessStart = watch.ElapsedMilliseconds;

                MemberAccess memberAccess = CmuneMember.GetMemberAccess(this.Cmid, ApplicationId);

                getMemberAccessEnd = watch.ElapsedMilliseconds;

                ViewData["memberAccessIsAccountDisabled"] = memberAccess.IsAccountDisabled;
                ViewData["memberAccessIsAccoundDisabledBanStatus"] = GetBanStatusText((BanMode)memberAccess.IsAccountDisabled, memberAccess.AccountDisabledUntil);
                ViewData["memberAccessIsChatDisabled"] = memberAccess.IsChatDisabled;
                ViewData["memberAccessIsChatDisabledBanStatus"] = GetBanStatusText((BanMode)memberAccess.IsChatDisabled, memberAccess.ChatDisabledUntil);

                ViewBag.AccountCreationDate = member.ResLastSyncDate;
                ViewBag.LastLoginDate = member.LastAliveAck ?? DateTime.MinValue;

                List<SelectListItem> MemberAccessDropDownList = new List<SelectListItem>();

                foreach (var item in AdminCache.GenerateMemberAccessLevelDropDownListItems())
                {
                    MemberAccessDropDownList.Add(new SelectListItem() { Text = item.Text, Value = item.Value, Selected = (item.Value == memberAccess.AccessLevel.ToString()) });
                }

                ViewData["MemberAccessDropDownList"] = MemberAccessDropDownList;

                getCreditStart = watch.ElapsedMilliseconds;

                Credit memberToCharge = CmuneEconomy.GetCreditByMemberID(this.Cmid);

                getCreditEnd = watch.ElapsedMilliseconds;

                ViewData["memberCredits"] = (memberToCharge.NbCredits ?? 0).ToString();
                ViewData["memberPoints"] = (memberToCharge.NbPoints ?? 0).ToString();

                #endregion

                #region Esns handles

                getEsnsStart = watch.ElapsedMilliseconds;

                List<ESNSIdentity> esnsIdentityMember = CmuneMember.GetMemberEsns(this.Cmid);

                getEsnsEnd = watch.ElapsedMilliseconds;

                List<EsnsHandleDisplay> esnsIdentitiesDisplay = esnsIdentityMember.ConvertAll<EsnsHandleDisplay>(new Converter<ESNSIdentity, EsnsHandleDisplay>(eH => new EsnsHandleDisplay((EsnsType)eH.Type, eH.Handle)));
                ViewData["esnsIdentitiesDisplay"] = esnsIdentitiesDisplay;

                #endregion

                #region Stats history

                getRankingStart = watch.ElapsedMilliseconds;

                GetRanking(this.Cmid);

                getRankingEnd = watch.ElapsedMilliseconds;

                #endregion

                #region Clan

                getGroupStart = watch.ElapsedMilliseconds;

                Group group = CmuneGroups.GetGroup(this.Cmid, UberStrikeCommonConfig.ApplicationId);

                getGroupEnd = watch.ElapsedMilliseconds;

                if (group != null)
                {
                    getGroupMemberStart = watch.ElapsedMilliseconds;

                    GroupMember groupMember = CmuneGroups.GetGroupMember(group.GID, this.Cmid);

                    getGroupMemberEnd = watch.ElapsedMilliseconds;

                    ClanMemberDisplay clanMemberDisplay = null;

                    if (groupMember != null)
                    {
                        clanMemberDisplay = new ClanMemberDisplay((GroupPosition)groupMember.Position, group.TagName, group.Name, group.GID, groupMember.DateJoined);
                    }

                    ViewBag.ClanMember = clanMemberDisplay;
                }

                #endregion

                #region Previous emails

                getPreviousEmailsStart = watch.ElapsedMilliseconds;

                List<PreviousEmail> previousEmails = CmuneMember.GetPreviousEmails(this.Cmid);

                getPreviousEmailsEnd = watch.ElapsedMilliseconds;

                List<MemberPreviousEmailDisplay> previousEmailsDisplay = previousEmails.ConvertAll(new Converter<PreviousEmail, MemberPreviousEmailDisplay>(e => new MemberPreviousEmailDisplay(e.PreviousEmailAddress, e.ChangeDate, e.SourceIp)));
                ViewBag.PreviousEmails = previousEmailsDisplay;

                #endregion

                #region Previous names

                List<PreviousName> previousNames = CmuneMember.GetPreviousNames(this.Cmid);
                List<MemberPreviousNameDisplay> previousNamesDisplay = previousNames.ConvertAll(new Converter<PreviousName, MemberPreviousNameDisplay>(e => new MemberPreviousNameDisplay(e.PreviousUserName, e.ChangeDate, e.SourceIp)));
                ViewBag.PreviousNames = previousNamesDisplay;

                #endregion

                #region Member notes

                getNotesStart = watch.ElapsedMilliseconds;

                ViewBag.MemberNotes = CmuneMember.GetMemberNotesView(this.Cmid);

                getNotesEnd = watch.ElapsedMilliseconds;

                List<SelectListItem> addMemberNoteDropDownList = new List<SelectListItem>();

                addMemberNoteDropDownList.Add(new SelectListItem() { Text = ModerationActionType.Note.ToString(), Value = ((int)ModerationActionType.Note).ToString(), Selected = true });
                addMemberNoteDropDownList.Add(new SelectListItem() { Text = ModerationActionType.Warning.ToString(), Value = ((int)ModerationActionType.Warning).ToString(), Selected = false });
                addMemberNoteDropDownList.Add(new SelectListItem() { Text = ModerationActionType.ItemExchange.ToString(), Value = ((int)ModerationActionType.ItemExchange).ToString(), Selected = false });
                addMemberNoteDropDownList.Add(new SelectListItem() { Text = ModerationActionType.RescueFromAccountStealing.ToString(), Value = ((int)ModerationActionType.RescueFromAccountStealing).ToString(), Selected = false });

                ViewData["addNoteType"] = addMemberNoteDropDownList;

                ViewData["addNoteAdminNames"] = AdminCache.GenerateAdminDropDownListItems();

                ViewData["editMemberNameAdminNames"] = AdminCache.GenerateAdminDropDownListItems();
                ViewData["editMemberEmailAdminNames"] = AdminCache.GenerateAdminDropDownListItems();

                ViewData["moderationActionTypes"] = AdminCache.GenerateModerationActionTypeDropDownListItems();

                List<SelectListItem> editNameDropDownList = new List<SelectListItem>();

                editNameDropDownList.Add(new SelectListItem() { Text = ModerationActionType.InvalidNameChange.ToString(), Value = ((int)ModerationActionType.InvalidNameChange).ToString(), Selected = true });
                editNameDropDownList.Add(new SelectListItem() { Text = ModerationActionType.AccountNameChange.ToString(), Value = ((int)ModerationActionType.AccountNameChange).ToString(), Selected = false });

                ViewData["editMemberNameType"] = editNameDropDownList;

                List<SelectListItem> editEmailDropDownList = new List<SelectListItem>();

                editEmailDropDownList.Add(new SelectListItem() { Text = ModerationActionType.AccountEmailChange.ToString(), Value = ((int)ModerationActionType.AccountEmailChange).ToString(), Selected = true });

                ViewData["editMemberEmailType"] = editEmailDropDownList;

                #endregion
            }

            watch.Stop();

            long totalDuration = watch.ElapsedMilliseconds;
            long getNameDuration = getNameEnd - getNameStart;
            long getIsMemberEsnsOnlyDuration = getIsMemberEsnsOnlyEnd - getIsMemberEsnsOnlyStart;
            long getMemberAccessDuration = getMemberAccessEnd - getMemberAccessStart;
            long getCreditDuration = getCreditEnd - getCreditStart;
            long getEsnsDuration = getEsnsEnd - getEsnsStart;
            long getRankingDuration = getRankingEnd - getRankingStart;
            long getGroupDuration = getGroupEnd - getGroupStart;
            long getGroupMemberDuration = getGroupMemberEnd - getGroupMemberStart;
            long getPreviousEmailsDuration = getPreviousEmailsEnd - getPreviousEmailsStart;
            long getNotesDuration = getNotesEnd - getNotesStart;

            string debugTemplate = "{0}{1}ms{2} - +{3}ms";
            string debugHeaderTemplate = "{0}{1}ms{2} - {3}ms";

            if (totalDuration == 0)
            {
                totalDuration = 1;
            }

            StringBuilder debugLog = new StringBuilder();
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugHeaderTemplate, "Total duration:".PadRight(20), totalDuration.ToString().PadLeft(6), ((decimal)totalDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), seeStart.ToString().PadLeft(5)));
            debugLog.AppendLine();
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get name:".PadRight(20), getNameDuration.ToString().PadLeft(6), ((decimal)getNameDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getNameStart - seeStart).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Is Esns only:".PadRight(20), getIsMemberEsnsOnlyDuration.ToString().PadLeft(6), ((decimal)getIsMemberEsnsOnlyDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getIsMemberEsnsOnlyStart - getNameEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get member access:".PadRight(20), getMemberAccessDuration.ToString().PadLeft(6), ((decimal)getMemberAccessDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getMemberAccessStart - getIsMemberEsnsOnlyEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get credit:".PadRight(20), getCreditDuration.ToString().PadLeft(6), ((decimal)getCreditDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getCreditStart - getMemberAccessEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get Esns:".PadRight(20), getEsnsDuration.ToString().PadLeft(6), ((decimal)getEsnsDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getEsnsStart - getCreditEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get ranking:".PadRight(20), getRankingDuration.ToString().PadLeft(6), ((decimal)getRankingDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getRankingStart - getEsnsEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get clan:".PadRight(20), getGroupDuration.ToString().PadLeft(6), ((decimal)getGroupDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getGroupStart - getRankingEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get clan members:".PadRight(20), getGroupMemberDuration.ToString().PadLeft(6), ((decimal)getGroupMemberDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), ((getGroupMemberStart != 0) ? getGroupMemberStart - getGroupEnd : 0).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get previous emails:".PadRight(20), getPreviousEmailsDuration.ToString().PadLeft(6), ((decimal)getPreviousEmailsDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getPreviousEmailsStart - ((getGroupMemberEnd != 0) ? getGroupMemberEnd : getGroupEnd)).ToString().PadLeft(4)));
            debugLog.AppendLine();
            debugLog.Append(String.Format(debugTemplate, "Get notes:".PadRight(20), getNotesDuration.ToString().PadLeft(6), ((decimal)getNotesDuration / (decimal)totalDuration).ToString("P0").PadLeft(7), (getNotesStart - getPreviousEmailsEnd).ToString().PadLeft(4)));
            debugLog.AppendLine();

            ViewBag.DebugInfo = debugLog.ToString();
            ViewBag.IsMemberExisting = isMemberExisting;

            return View();
        }

        public ActionResult DoCustomActions()
        {
            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.DoCustomActions;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.DoCustomActions);

            List<SelectListItem> DurationDropDownList = new List<SelectListItem>();
            List<BuyingDurationType> buyingDurationTypes = EnumUtilities.IterateEnum<BuyingDurationType>();

            foreach (BuyingDurationType buyingDurationType in buyingDurationTypes)
            {
                SelectListItem listItem = new SelectListItem() { Text = CommonConfig.BuyingDurationName[buyingDurationType], Value = ((int)buyingDurationType).ToString() };
                DurationDropDownList.Add(listItem);
            }

            ViewData["DurationDropDownList"] = DurationDropDownList;

            ViewData["attributeCurrencyEsnsType"] = AdminCache.GenerateEsnsDropDownListItems(true);

            List<SelectListItem> currencyItems = new List<SelectListItem> { new SelectListItem { Text = "Points", Value = "0", Selected = true }, new SelectListItem { Text = "Credits", Value = "1", Selected = false } };
            ViewData["attributeCurrencyCurrencyType"] = currencyItems;

            ViewData["UsdDepositsThresholdData"] = 10;

            return View();
        }

        public ActionResult CustomQueries()
        {
            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.CustomQueries;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.CustomQueries);

            ViewData["whoBoughtItemDuration"] = AdminCache.GenerateBuyingDurationSelectItems();

            ViewData["membersCountEsns"] = AdminCache.GenerateEsnsDropDownListItems();

            ViewData["xpFarmerDayOfMonth"] = DateTime.Now.AddDays(-1).Day.ToString();
            ViewData["xpFarmerKills"] = 400;
            ViewData["xpFarmerKdr"] = 8;

            return View();
        }

        public ActionResult SeeBannedIps()
        {
            ViewBag.SubActiveTab = CmuneMenu.MemberSubTabs.BannedIps;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.MemberSubTabs.BannedIps);

            return View();
        }

        #endregion

        #region See member

        [HttpPost]
        public ActionResult ResetPassword(FormCollection form)
        {
            bool isPasswordReset = false;
            string message = "The password was not reset";

            string cmidForm = form["memberCmid"];
            int cmid;
            bool isCmidParsed = Int32.TryParse(cmidForm, out cmid);

            if (isCmidParsed)
            {
                isPasswordReset = UsersBusiness.ResetPassword(cmid);

                if (isPasswordReset)
                {
                    message = "The password was reset.";
                }
            }

            var json = new JsonResult() { Data = new { isModified = isPasswordReset, message = message } };
            return json;
        }

        /// <summary>
        /// Display the add / remove credits / points popup
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoadCreditsAndPoints(string cmid)
        {
            ViewData["adminNames"] = AdminCache.GenerateAdminDropDownListItems();

            List<SelectListItem> actionTypeDropDownList = new List<SelectListItem>();

            actionTypeDropDownList.Add(new SelectListItem() { Text = ModerationActionType.Refund.ToString(), Value = ((int)ModerationActionType.Refund).ToString(), Selected = true });
            actionTypeDropDownList.Add(new SelectListItem() { Text = ModerationActionType.Note.ToString(), Value = ((int)ModerationActionType.Note).ToString(), Selected = false });

            ViewData["actionType"] = actionTypeDropDownList;

            return View("Partial/Form/CreditsAndPointsForm");
        }

        [ValidateInput(false)]
        public ActionResult EditMemberPointsAndCredits(FormCollection form)
        {
            int points = 0;
            int credits = 0;
            string message = string.Empty;
            bool isModified = false;
            bool isMailSent = false;
            Int32.TryParse(form["pointsToAdd"], out points);
            Int32.TryParse(form["creditsToAdd"], out credits);

            string currentName = form["memberName"];
            string currentEmail = form["memberEmail"];
            EmailAddressStatus emailStatus = (EmailAddressStatus)Convert.ToInt32(form["memberEmailStatus"]);
            Cmid = Int32.Parse(form["memberCmid"]);
            int sourceCmid = 0;
            Int32.TryParse(form["adminNames"], out sourceCmid);
            ModerationActionType actionType;
            EnumUtilities.TryParseEnumByValue(form["actionType"], out actionType);

            if (points != 0 || credits != 0)
            {
                CmuneMember.UpdateMemberBalance(points, credits, Cmid, sourceCmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, actionType, form["reasonForAdding"]);
                isModified = true;

                if (isModified)
                {
                    isMailSent = CmuneMail.PointsCreditsModified(currentName, currentEmail, points, credits, form["reasonForAdding"], emailStatus);
                    message = "Operation successfully done" + (isMailSent ? " and a mail is sent to member" : " but no mail is sent to member");
                    Credit memberToCharge = CmuneEconomy.GetCreditByMemberID(Cmid);
                    points = memberToCharge.NbPoints ?? 0;
                    credits = memberToCharge.NbCredits ?? 0;
                }
                else
                    message = "Error occured";
            }
            else
            {
                message = "None value is provided";
            }

            var json = new JsonResult() { Data = new { isModified = isModified, message = message, newPoints = points, newCredits = credits } };

            return json;
        }

        [HttpPost]
        public ActionResult GetContacts(int cmid, int selectedPage)
        {
            int contactPerPage = 9;

            List<ContactDisplay> contactsDisplay = new List<ContactDisplay>();
            List<MembersRelationship> contacts = CmuneRelationship.GetMemberRelationships(cmid);

            if (contacts.Count > 0)
            {
                List<int> contactsCmid = new List<int>();

                for (int i = 0; i < contacts.Count; i++)
                {
                    if (contacts[i].FriendCMID.Equals(cmid))
                    {
                        contactsCmid.Add(contacts[i].MemberCMID);
                    }
                    else
                    {
                        contactsCmid.Add(contacts[i].FriendCMID);
                    }
                }

                Dictionary<int, AllTimeTotalRanking> contactsRanking = Statistics.GetStats(contactsCmid).ToDictionary(r => r.CMID);
                Dictionary<int, string> contactsName = CmuneMember.GetMembersNames(contactsCmid);

                contactsDisplay = contactsCmid.ConvertAll(new Converter<int, ContactDisplay>(c => new ContactDisplay { Cmid = c, Name = contactsName[c], Kills = 0, Ranking = 0 }));

                for (int j = 0; j < contactsDisplay.Count; j++)
                {
                    if (contactsRanking.ContainsKey(contactsDisplay[j].Cmid))
                    {
                        contactsDisplay[j].Ranking = contactsDisplay[j].Ranking;
                        contactsDisplay[j].Kills = contactsDisplay[j].Kills;
                    }
                }
            }

            var paginationModel = new PaginationModel(contactsDisplay.Count, selectedPage, "Contacts", contactPerPage);

            ViewBag.ContactsPaginationModel = paginationModel;

            return View("Partial/Contacts", paginationModel.GetCurrentPage(contactsDisplay));
        }

        public ActionResult GetDailyStatistics(int cmid)
        {
            Stopwatch watch = Stopwatch.StartNew();

            List<DailyRanking> statisticsHistory = AdminCache.LoadDailyStatisticsHistory(cmid);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get daily stats: {0}ms", watch.ElapsedMilliseconds);

            List<StatsHistoryDisplay> statisticsHistoryDisplay = statisticsHistory.ConvertAll<StatsHistoryDisplay>(new Converter<DailyRanking, StatsHistoryDisplay>(r => new StatsHistoryDisplay(r.Kills, r.Deaths, r.Shots, r.Hits)));

            return View("Partial/DailyStatistics", statisticsHistoryDisplay);
        }

        [HttpPost]
        public ActionResult GetItems(int cmid)
        {
            Cmid = cmid;

            DateTime currentTime = DateTime.Now;

            Stopwatch watch = Stopwatch.StartNew();

            List<ItemInventory> inventory = CmuneMember.GetCurrentItemInventory(cmid);

            ItemCache itemCache = new ItemCache(CommonConfig.ApplicationIdUberstrike, true);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get items: {0}ms", watch.ElapsedMilliseconds);

            List<ItemInventoryDisplay> inventoryDisplay = inventory.ConvertAll(new Converter<ItemInventory, ItemInventoryDisplay>(cI => new ItemInventoryDisplay(cI.ItemId, itemCache.GetItemName(cI.ItemId), cI.ExpirationDate, currentTime, cI.AmountRemaining)));

            ViewBag.IsParadisePaintballAccount = false;

            if (inventoryDisplay.SingleOrDefault(i => i.ItemId == CommonConfig.PrivateerUberStrikeLicenseId) == null)
            {
                Member member = CmuneMember.GetMember(cmid);

                if (member != null && member.ResLastSyncDate <= CommonConfig.UberStrikeStartingDate)
                {
                    ViewBag.IsParadisePaintballAccount = true;
                }
            }

            return View("Partial/Items", inventoryDisplay);
        }

        [HttpPost]
        public ActionResult UpdateAccountToUberStrike(int cmid)
        {
            bool isUpdated = Users.UpdateAccountToUberStrike(cmid);

            var json = new JsonResult() { Data = new { IsUpdated = isUpdated } };
            return json;
        }

        public ActionResult GetPointsDeposits(int cmid, int selectedPage)
        {
            int pointsDepositsPerPage = 30;

            Stopwatch watch = Stopwatch.StartNew();

            List<PointDepositView> pointsDeposits = AdminCache.LoadPointDepositView(cmid, selectedPage, pointsDepositsPerPage);
            int pointsDepositsTotalCount = AdminCache.LoadPointDepositViewCount(cmid);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get points deposits: {0}ms", watch.ElapsedMilliseconds);

            var paginationModel = new PaginationModel(pointsDepositsTotalCount, selectedPage, "PointsDeposits", pointsDepositsPerPage);
            ViewBag.PointsDepositsPaginationModel = paginationModel;

            return View("Partial/PointsDeposits", pointsDeposits);
        }

        public ActionResult GetItemTransactions(int cmid, int selectedPage)
        {
            int transactionsPerPage = 30;

            Stopwatch watch = Stopwatch.StartNew();

            List<ItemTransactionView> transactions = AdminCache.LoadItemTransactionsView(cmid, selectedPage, transactionsPerPage);
            int transactionsTotalCount = AdminCache.LoadItemTransactionsViewCount(cmid);

            var paginationModel = new PaginationModel(transactionsTotalCount, selectedPage, "ItemTransactions", transactionsPerPage);
            ViewBag.ItemTransactionsPaginationModel = paginationModel;

            ViewBag.ItemCache = new ItemCache(CommonConfig.ApplicationIdUberstrike, true);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get item transactions: {0}ms", watch.ElapsedMilliseconds);

            return View("Partial/ItemTransactions", transactions);
        }

        public ActionResult GetCurrencyDeposits(int cmid, int selectedPage)
        {
            int currencyDepositsPerPage = 30;

            Stopwatch watch = Stopwatch.StartNew();

            List<CurrencyDepositView> currencyDeposits = AdminCache.LoadCurrencyDepositsView(cmid, selectedPage, currencyDepositsPerPage);
            int currencyDepositsTotalCount = AdminCache.LoadCurrencyDepositsViewCount(cmid);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get currency deposits: {0}ms", watch.ElapsedMilliseconds);

            var paginationModel = new PaginationModel(currencyDepositsTotalCount, selectedPage, "CurrencyDeposits", currencyDepositsPerPage);
            ViewBag.CurrencyDepositsPaginationModel = paginationModel;

            return View("Partial/CurrencyDeposits", currencyDeposits);
        }

        public string GetTotalCurrencyDeposits(int cmid)
        {
            decimal totalCurrencyDeposits = AdminCache.LoadTotalCurrencyDeposits(cmid);

            return totalCurrencyDeposits.ToString("C2");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection form)
        {
            string message = String.Empty;
            bool isModified = false;
            string currentName = form["memberName"];
            string currentEmail = form["memberEmail"];
            EmailAddressStatus emailStatus = (EmailAddressStatus)Convert.ToInt32(form["memberEmailStatus"]);

            Cmid = Int32.Parse(form["memberCmid"]);
            int sourceCmid = 0;
            ModerationActionType actionType = ModerationActionType.AccountEmailChange;
            MemberOperationResult memberOperationResult = MemberOperationResult.InvalidData;

            switch (form["action"])
            {
                case "changeEmailForCompletion":

                    string emailAddress = form["newMemberEmail"].Trim();
                    MemberOperationResult ret = UsersBusiness.RegisterEsnsMember(Cmid, emailAddress, true, form["explanationMemberEmail"]);

                    if (ret == MemberOperationResult.Ok)
                    {
                        isModified = true;
                        message = "The email was added";
                    }
                    else
                    {
                        message = ret.ToString();
                    }

                    break;
                case "changeEmail":
                    bool notifyMemberEmailCheckBox = false;
                    if (!form["notifyMemberEmailCheckBox"].IsNullOrFullyEmpty())
                    {
                        notifyMemberEmailCheckBox = form["notifyMemberEmailCheckBox"].ToString().ToLower() == "on";
                    }
                    string newMemberEmail = form["newMemberEmail"].Trim();

                    Int32.TryParse(form["editMemberEmailAdminNames"], out sourceCmid);
                    EnumUtilities.TryParseEnumByValue(form["editMemberEmailType"], out actionType);

                    memberOperationResult = UsersBusiness.ChangeMemberEmail(sourceCmid, Cmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, actionType, form["explanationMemberEmail"], newMemberEmail, currentEmail);

                    if (memberOperationResult == MemberOperationResult.Ok)
                    {
                        isModified = true;
                        message = "The email was changed";

                        if (notifyMemberEmailCheckBox)
                        {
                            CmuneMail.SendEmailNotification(EmailNotificationType.ChangeMemberEmail, form["explanationMemberEmail"], currentEmail, currentName, emailStatus);
                        }
                    }
                    else
                    {
                        message = memberOperationResult.ToString();
                    }

                    break;
                case "changeName":
                    bool notifyMemberNameCheckBox = false;
                    if (!form["notifyMemberEmailCheckBox"].IsNullOrFullyEmpty())
                    {
                        notifyMemberNameCheckBox = form["notifyMemberEmailCheckBox"].ToString().ToLower() == "on";
                    }
                    string newMemberName = form["newMemberName"].Trim();

                    Int32.TryParse(form["editMemberNameAdminNames"], out sourceCmid);
                    EnumUtilities.TryParseEnumByValue(form["editMemberNameType"], out actionType);

                    memberOperationResult = UsersBusiness.ChangeMemberName(sourceCmid, Cmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, actionType, form["explanationMemberName"], newMemberName, currentName, LocaleIdentifier.EnUS);

                    if (memberOperationResult == MemberOperationResult.Ok)
                    {
                        isModified = true;
                        message = "The name was changed";

                        if (notifyMemberNameCheckBox)
                        {
                            CmuneMail.SendEmailNotification(EmailNotificationType.ChangeMemberName, form["explanationMemberName"], currentEmail, currentName, emailStatus);
                        }
                    }
                    else
                    {
                        message = memberOperationResult.ToString();
                    }

                    break;
                case "changePassword":
                    if (form["pwd"] == form["pwd2"] && !String.IsNullOrEmpty(form["pwd"]) && !String.IsNullOrEmpty(form["pwd2"]))
                    {
                        if (Users.ChangePassword(_cmid, form["pwd"]))
                        {
                            isModified = true;
                            message = "The password was changed";
                            CmuneMail.SendEmailNotification(EmailNotificationType.ChangeMemberPassword, form["pwd"], currentEmail, currentName, emailStatus);
                        }
                        else
                        {
                            message = "The password was not changed";
                        }
                    }
                    else if (String.IsNullOrEmpty(form["pwd"]) || String.IsNullOrEmpty(form["pwd2"]))
                    {
                        message = "Passwords are not filled";
                    }
                    else
                    {
                        message = "Passwords don't match";
                    }
                    break;
                default:
                    ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException("action");
                    ex.Data.Add("action", form["action"]);
                    throw ex;
            }

            var json = new JsonResult() { Data = new { isModified = isModified, message = message, action = form["action"], newValue = "" } };
            return json;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetUserCmid(string userName, string userEmail, string facebookId)
        {
            int cmid = 0;

            if (!userName.IsNullOrFullyEmpty())
            {
                cmid = CmuneMember.GetCmidByName(userName);
            }
            else if (!userEmail.IsNullOrFullyEmpty())
            {
                cmid = CmuneMember.GetCmidByEmail(userEmail);
            }
            else if (!facebookId.IsNullOrFullyEmpty())
            {
                cmid = CmuneMember.GetCmidByEsnsId(facebookId, EsnsType.Facebook);
            }

            var json = new JsonResult() { Data = new { MemberCmid = cmid } };
            return json;
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult LoadDelete(string memberCmid, string memberEmail, string memberName)
        {
            // Retrieve email and name to enforce the security(todo)
            ViewData["memberCmid"] = memberCmid;
            ViewData["memberEmail"] = memberEmail;
            ViewData["memberName"] = memberName;

            return View("Partial/Form/DeleteForm");
        }

        [HttpGet]
        public ActionResult LoadPrivateMessageSender(int memberCmid)
        {
            ViewData["memberCmid"] = memberCmid;

            return View("Partial/Form/SendPrivateMessageForm");
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendPrivateMessage(int memberCmid, string subjectTextBox, string contentTextArea)
        {
            bool isMessageSent = CmunePrivateMessages.SendAdminMessage(memberCmid, subjectTextBox, contentTextArea);

            var json = new JsonResult() { Data = new { IsSent = isMessageSent } };
            return json;
        }

        public ActionResult LoadIpsAndOtherAccounts(int cmid)
        {
            return View("Partial/IpsAndOtherAccounts", Users.GetLinkedAccounts(cmid));
        }

        [HttpPost]
        public JsonResult SetAccessLevel(int cmid, int accessLevel)
        {
            bool isModified = false;
            string message = String.Empty;

            MemberAccessLevel memberAccessLevel;
            bool isMemberAccesLevelParsed = EnumUtilities.TryParseEnumByValue(accessLevel, out memberAccessLevel);

            if (isMemberAccesLevelParsed)
            {
                if (CmuneMember.SetMemberAccess(cmid, CommonConfig.ApplicationIdUberstrike, memberAccessLevel))
                {
                    message = "Modified the access level.";
                    isModified = true;
                }
                else
                {
                    message = "Unable to modfiy the access level";
                }
            }
            else
            {
                message = "Unknown access level.";
            }

            var json = new JsonResult() { Data = new { IsModified = isModified, Message = message } };
            return json;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMemberAccess(FormCollection form)
        {
            // TODO: needs to be rewritten

            string action = string.Empty;
            string message = string.Empty;
            string banUntil = string.Empty;
            BanMode banMode = BanMode.No;
            BanMode oldBanMode;

            string currentName = form["memberName"];
            string currentEmail = form["memberEmail"];
            EmailAddressStatus emailStatus = (EmailAddressStatus)Convert.ToInt32(form["memberEmailStatus"]);
            Cmid = Int32.Parse(form["memberCmid"]);

            int sourceCmid = 0;
            Int32.TryParse(form["adminNames"], out sourceCmid);

            bool isModified = false;

            try
            {
                switch (form["action"])
                {
                    case "changeIsAccountDisabled":

                        banMode = (BanMode)Int32.Parse(form["newMemberAccessIsAccountDisabled"]);
                        oldBanMode = (BanMode)Int32.Parse(form["memberAccessIsAccountDisabled"]);

                        switch (banMode)
                        {
                            case BanMode.No:

                                if (banMode != oldBanMode)
                                {
                                    isModified = CmuneMember.UnbanAccount(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"]);
                                    banUntil = GetBanStatusText(banMode);

                                    if (isModified)
                                    {
                                        message = "The account was unbanned.";
                                        CmuneMail.SendEmailNotification(EmailNotificationType.UnbanMember, form["explanationTextBox"], form["memberEmail"], form["memberName"], emailStatus);
                                    }
                                    else
                                    {
                                        message = "The account was not unbanned.";
                                    }
                                }
                                else
                                {
                                    message = "Already unbanned.";
                                }

                                break;
                            case BanMode.Temporary:
                                DateTime disableUntil = DateTime.Now.AddDays(Convert.ToInt32(form["disablingDuration"]));
                                isModified = CmuneMember.BanAccount(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"], disableUntil);
                                banUntil = GetBanStatusText(BanMode.Temporary, disableUntil);

                                if (isModified)
                                {
                                    message = "The account was temporary bannd.";
                                    CmuneMail.SendEmailNotification(EmailNotificationType.BanMemberTemporary, form["explanationTextBox"], currentEmail, currentName, emailStatus);
                                }
                                else
                                {
                                    message = "The account was not banned.";
                                }

                                break;
                            case BanMode.Permanent:

                                if (banMode != oldBanMode)
                                {
                                    isModified = CmuneMember.BanAccount(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"], null);
                                    banUntil = GetBanStatusText(banMode);

                                    if (isModified)
                                    {
                                        message = "The account was permanently banned.";
                                        CmuneMail.SendEmailNotification(EmailNotificationType.BanMemberPermanent, form["explanationTextBox"], currentEmail, currentName, emailStatus);
                                    }
                                    else
                                    {
                                        message = "The account was not banned.";
                                    }
                                }
                                else
                                {
                                    message = "Already permanently banned.";
                                }

                                break;
                        }
                        break;
                    case "changeIsChatDisabled":

                        banMode = (BanMode)Int32.Parse(form["newMemberAccessIsChatDisabled"]);
                        oldBanMode = (BanMode)Int32.Parse(form["memberAccessIsChatDisabled"]);

                        switch (banMode)
                        {
                            case BanMode.No:

                                if (banMode != oldBanMode)
                                {
                                    isModified = CmuneMember.UnbanFromChat(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"]);
                                    banUntil = GetBanStatusText(BanMode.No);

                                    if (isModified)
                                    {
                                        message = "The member was unbanned from the chat.";
                                        CmuneMail.SendEmailNotification(EmailNotificationType.UnbanMemberChat, form["explanationTextBox"], currentEmail, currentName, emailStatus);
                                    }
                                    else
                                    {
                                        message = "The member was not unbanned from the chat.";
                                    }
                                }
                                else
                                {
                                    message = "Already unbanned from chat.";
                                }

                                break;
                            case BanMode.Temporary:
                                DateTime disableUntil = DateTime.Now.AddDays(Convert.ToInt32(form["disablingDuration"]));
                                banUntil = GetBanStatusText(BanMode.Temporary, disableUntil);
                                isModified = CmuneMember.BanFromChat(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"], disableUntil);

                                if (isModified)
                                {
                                    message = "The member was banned temporary from the chat.";
                                    CmuneMail.SendEmailNotification(EmailNotificationType.BanMemberChatTemporary, form["explanationTextBox"], currentEmail, currentName, emailStatus);
                                }
                                else
                                {
                                    message = "The member was not banned temporary from the chat.";
                                }

                                break;
                            case BanMode.Permanent:

                                if (banMode != oldBanMode)
                                {
                                    isModified = CmuneMember.BanFromChat(sourceCmid, Cmid, ApplicationId, Request.UserHostAddress, form["explanationTextBox"], null);
                                    banUntil = GetBanStatusText(BanMode.Permanent);

                                    if (isModified)
                                    {
                                        message = "The member was banned permanently from the chat.";
                                        CmuneMail.SendEmailNotification(EmailNotificationType.BanMemberChatPermanent, form["explanationTextBox"], currentEmail, currentName, emailStatus);
                                    }
                                    else
                                    {
                                        message = "The member was not banned permanently from the chat.";
                                    }
                                }
                                else
                                {
                                    message = "Already permanently banned from the chat.";
                                }

                                break;
                        }
                        break;
                    default:
                        throw new ArgumentException(String.Format("Unexpected action value: {0}", form["action"]));
                }
            }
            catch (Exception)
            {
                message = "Error occured";
            }

            var json = new JsonResult() { Data = new { isModified = isModified, message = message, action = form["action"], newValue = ((int)banMode).ToString(), banUntil = banUntil, errorCss = !(banMode == BanMode.No) } };
            return json;
        }

        [HttpPost]
        public JsonResult SetLevel(int cmid, int level)
        {
            int newXp = 0;
            bool isModified = Statistics.SetLevel(cmid, level, out newXp);

            var json = new JsonResult() { Data = new { IsModified = isModified, NewXp = newXp } };
            return json;
        }

        #region Ban

        [HttpGet]
        public ActionResult LoadBan(string cmid, string memberAccessIsAccountDisabled, string banUtil)
        {
            ViewData["memberAccessIsAccountDisabled"] = int.Parse(memberAccessIsAccountDisabled);

            var listDisablingDuration = new List<SelectListItem>();

            for (int i = 0; i < 30; i++)
            {
                listDisablingDuration.Add(new SelectListItem() { Value = (i + 1).ToString(), Text = (i + 1).ToString() });
            }

            ViewData["disablingDuration"] = listDisablingDuration;
            ViewData["DisableInfoLiteral"] = banUtil;

            ViewData["adminNames"] = AdminCache.GenerateAdminDropDownListItems();

            return View("Partial/Form/BanForm");
        }

        [HttpGet]
        public ActionResult LoadChatBan(string cmid, string memberAccessIsChatDisabled, string banUtil)
        {
            ViewData["memberAccessIsChatDisabled"] = (BanMode)int.Parse(memberAccessIsChatDisabled);

            var listDisablingDuration = new List<SelectListItem>();

            for (int i = 0; i < 30; i++)
            {
                listDisablingDuration.Add(new SelectListItem() { Value = (i + 1).ToString(), Text = (i + 1).ToString() });
            }

            ViewData["disablingDuration"] = listDisablingDuration;
            ViewData["DisableInfoLiteral"] = banUtil;

            ViewData["adminNames"] = AdminCache.GenerateAdminDropDownListItems();

            return View("Partial/Form/ChatBanForm");
        }

        #endregion

        [HttpPost]
        public ActionResult GetLoadout(int cmid)
        {
            Cmid = cmid;

            DateTime currentTime = DateTime.Now;
            Stopwatch watch = Stopwatch.StartNew();

            LoadoutView loadout = Users.GetLoadoutView(cmid);

            watch.Stop();

            ViewBag.ElapsedMilliseconds = String.Format("Get loadout: {0}ms", watch.ElapsedMilliseconds);
            ViewBag.ItemCache = new ItemCache(CommonConfig.ApplicationIdUberstrike, true);

            return View("Partial/Loadout", loadout);
        }

        [HttpPost]
        public ActionResult GetPreviousClans(int cmid)
        {
            List<GroupMember> previousGroupMembership = CmuneGroups.GetPreviousGroups(cmid, 5);
            List<PreviousClanDisplay> previousGroupsDisplay = new List<PreviousClanDisplay>();

            if (previousGroupMembership.Count > 0)
            {
                Dictionary<int, Group> previousGroups = CmuneGroups.GetGroups(previousGroupMembership.Select(g => g.GroupId).ToList()).ToDictionary(g => g.GID);

                previousGroupsDisplay = previousGroupMembership.ConvertAll(new Converter<GroupMember, PreviousClanDisplay>(g => new PreviousClanDisplay { DateJoined = g.DateJoined, DateQuit = (DateTime)g.DateQuit, Id = g.GroupId, Name = previousGroups[g.GroupId].Name, Tag = previousGroups[g.GroupId].TagName }));
            }

            return View("Partial/PreviousClans", previousGroupsDisplay);
        }

        #endregion

        #region Do custom actions

        [HttpPost]
        public string AttributeCurrency(string currency, string attributeCurrencyCurrencyType, string membersHandle, string attributeCurrencyEsnsType, string allMembers)
        {
            EsnsType esns;
            bool isEsnsParsed = EnumUtilities.TryParseEnumByValue(attributeCurrencyEsnsType, out esns);
            int currencyValue;
            bool isCurrencyParsed = Int32.TryParse(currency, out currencyValue);

            if (isEsnsParsed && isCurrencyParsed)
            {
                if (allMembers == "on")
                {
                    bool isCredits = false;

                    if (attributeCurrencyCurrencyType.Equals("1"))
                    {
                        isCredits = true;
                    }

                    CmuneEconomy.AttributeCurrencyToMembers(currencyValue, isCredits, esns);
                }
                else
                {
                    // Let's remove any kind of whitespace or invisible separator
                    membersHandle = System.Text.RegularExpressions.Regex.Replace(membersHandle, @"\p{Z}", String.Empty);

                    List<int> cmids;
                    List<string> membersHandleList = membersHandle.Split(',').ToList();

                    if (esns != EsnsType.None)
                    {
                        cmids = CmuneMember.GetCmidListByEsnsId(membersHandleList, esns).Values.ToList();
                    }
                    else
                    {
                        cmids = membersHandleList.ConvertAll(u => Convert.ToInt32(u)).ToList();
                    }

                    if (attributeCurrencyCurrencyType.Equals("0"))
                    {
                        CmuneEconomy.AttributeCurrencyToMembers(currencyValue, false, cmids);
                    }
                    else if (attributeCurrencyCurrencyType.Equals("1"))
                    {
                        CmuneEconomy.AttributeCurrencyToMembers(currencyValue, true, cmids);
                    }
                }
            }

            return String.Empty;
        }

        [HttpPost]
        public ActionResult BanCheaters(string cmidsData, string usdDepositsThresholdData)
        {
            decimal threshold = 0;
            List<int> cmids = new List<int>();
            Dictionary<int, decimal> cmidsToWarn = new Dictionary<int, decimal>();
            List<int> notBanned = new List<int>();
            List<int> bannedUsers = new List<int>();

            if (!cmidsData.IsNullOrFullyEmpty())
            {
                cmidsData = cmidsData.Trim();

                List<string> cmidsDataTmp = cmidsData.Split(',').ToList();

                foreach (string cmidData in cmidsDataTmp)
                {
                    int cmidTmp;

                    if (Int32.TryParse(cmidData.Trim(), out cmidTmp))
                    {
                        cmids.Add(cmidTmp);
                    }
                }
            }

            if (cmids.Count > 0 && Decimal.TryParse(usdDepositsThresholdData.Trim(), out threshold))
            {
                Dictionary<int, decimal> userUsdDeposited = CmuneEconomy.GetTotalCurrencyDeposits(cmids);

                List<int> cmidsToBan = userUsdDeposited.Where(q => q.Value < threshold).Select(q => q.Key).ToList();
                cmidsToWarn = userUsdDeposited.Where(q => q.Value >= threshold).ToDictionary(q => q.Key, q => q.Value);

                foreach (int cmid in cmidsToBan)
                {
                    bool isBanned = CmuneMember.BanAccount(CommonConfig.CommunityManagerCmid, cmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, "Mod in game banning", null);

                    if (!isBanned)
                    {
                        notBanned.Add(cmid);
                    }
                    else
                    {
                        bannedUsers.Add(cmid);
                    }
                }

                CmuneMember.NotifyUserOfPermanentBan(bannedUsers, "Cheating");
            }

            ViewBag.AllCmids = cmids;
            ViewBag.BannedCmids = bannedUsers;
            ViewBag.NotBannedCmids = notBanned;
            ViewBag.Threshold = threshold;

            return PartialView("Partial/PermanentlyBannedCheaters", cmidsToWarn);
        }

        #endregion

        #region Custom queries

        [HttpPost]
        public string WhoBoughtItem(string itemId, string from, string to, string whoBoughtItemDuration)
        {
            List<int> cmids = new List<int>();

            int itemIdValue = 0;
            bool isItemIdParsed = Int32.TryParse(itemId, out itemIdValue);
            DateTime fromDate;
            bool isFromDateParsed = DateTime.TryParseExact(from, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fromDate);
            DateTime toDate;
            bool isToDateParsed = DateTime.TryParseExact(to, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out toDate);
            BuyingDurationType buyingDurationType;
            bool isDurationParsed = EnumUtilities.TryParseEnumByValue(whoBoughtItemDuration, out buyingDurationType);

            if (isItemIdParsed && isFromDateParsed && isToDateParsed && isDurationParsed)
            {
                cmids = ItemEconomyBusiness.GetItemBuyers(itemIdValue, fromDate, toDate, buyingDurationType);
            }

            return String.Join(", ", cmids);
        }

        [HttpPost]
        public string RetrieveMembersCount()
        {
            int maxCmid = UserActivityBusiness.GetMaxCmid();

            return maxCmid.ToString();
        }

        [HttpPost]
        public string RetrieveEsnsMembersCount(int membersCountEsns)
        {
            int membersCount = CmuneMember.GetEsnsCount((EsnsType)membersCountEsns);

            return membersCount.ToString();
        }

        [HttpPost]
        public ActionResult GetXpFarmers(string xpFarmerDayOfMonth, string xpFarmerKills, string xpFarmerKdr)
        {
            DateTime now = DateTime.Now;
            int dayOfMonth = 0;

            if (Int32.TryParse(xpFarmerDayOfMonth, out dayOfMonth))
            {
                if (now.Day - dayOfMonth < 1)
                {
                    now = now.AddMonths(-1);
                    now = now.AddDays(dayOfMonth - now.Day);
                }
                else
                {
                    now = now.AddDays(-(now.Day - dayOfMonth));
                }
            }
            else
            {
                now = now.AddDays(-1);
            }

            Dictionary<RankingView, List<RankingView>> xpFarmers = new Dictionary<RankingView, List<RankingView>>();
            int kills = 0;
            int kdr = 0;

            if (Int32.TryParse(xpFarmerKills, out kills) && Int32.TryParse(xpFarmerKdr, out kdr))
            {
                xpFarmers = Users.GetXpFarmers(now, kills, kdr);
            }
            else
            {
                xpFarmers = Users.GetXpFarmers(now);
            }

            return View("Partial/XpFarmers", xpFarmers);
        }

        [HttpPost]
        public string ConvertIp(string ip)
        {
            string convertedIp = String.Empty;

            if (ip.Contains("."))
            {
                convertedIp = TextUtilities.InetAToN(ip).ToString();
            }
            else
            {
                long networkIp = 0;

                if (Int64.TryParse(ip, out networkIp))
                {
                    convertedIp = TextUtilities.InetNToA(networkIp);
                }
            }

            return convertedIp;
        }

        #endregion

        #region Manage Member Access

        [HttpPost]
        public ActionResult ManageMemberAccess(FormCollection form)
        {
            List<MemberAccessDisplay> membersAccessDisplay = new List<MemberAccessDisplay>();
            int cmid = 0;
            MemberAccessLevel memberAccessLevel;

            int numberOfMembers = Int32.Parse(form["numberOfMembers"]);
            for (int i = 0; i < numberOfMembers; i++)
            {
                string accessLevelList = "accessLevelList" + i.ToString();
                string cmidHiddenField = "cmidHiddenField" + i.ToString();
                string accessLevelHiddenField = "accessLevelHiddenField" + i.ToString();

                bool isCmidParsed = Int32.TryParse(form[cmidHiddenField], out cmid);
                bool isMemberAccessParsed = EnumUtilities.TryParseEnumByValue(form[accessLevelList], out memberAccessLevel);

                if (isCmidParsed && isMemberAccessParsed && form[accessLevelHiddenField] != ((int)memberAccessLevel).ToString())
                {
                    MemberAccessDisplay memberAccessDisplay = new MemberAccessDisplay(cmid, String.Empty, memberAccessLevel);
                    membersAccessDisplay.Add(memberAccessDisplay);
                }
            }

            List<int> cmids = membersAccessDisplay.Select(mA => mA.Cmid).ToList();

            if (cmids.Count > 0)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    List<MemberAccess> membersAccess = CmuneMember.GetGetMembersAccess(cmids, CommonConfig.ApplicationIdUberstrike, cmuneDb);
                    Dictionary<int, MemberAccess> membersAccessOrdered = membersAccess.ToDictionary(mA => mA.Cmid);

                    foreach (MemberAccessDisplay memberAccessDisplay in membersAccessDisplay)
                    {
                        if (membersAccessOrdered.ContainsKey(memberAccessDisplay.Cmid))
                            membersAccessOrdered[memberAccessDisplay.Cmid].AccessLevel = (int)memberAccessDisplay.AccessLevel;
                    }

                    cmuneDb.SubmitChanges();
                }
            }

            return RedirectToAction("ManageMemberAccess");
        }

        #endregion

        #region Search

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchUsers(string memberEmail, string memberName, string cmid, string memberHandle, string esnsType, bool byExactName, int selectedPage = 1)
        {
            List<int> membersCmid = new List<int>();
            List<UserV> matchingUsers = new List<UserV>();

            if (cmid.IsNullOrFullyEmpty())
            {
                EsnsType esns;
                EnumUtilities.TryParseEnumByValue(esnsType, out esns);
                membersCmid = CmuneMember.GetCmidFromInfo(memberEmail, memberName, memberHandle, esns, byExactName);
            }
            else
            {
                int cmidTmp = 0;

                if (Int32.TryParse(cmid, out cmidTmp))
                {
                    membersCmid.Add(cmidTmp);
                }
            }

            var PaginationModel = new PaginationModel(membersCmid.Count, selectedPage, "Users", 50);
            var currentElements = PaginationModel.GetCurrentPage(membersCmid);

            if (currentElements.Count > 0)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    var query = from m in cmuneDb.Members
                                join i in cmuneDb.ID3s on m.CMID equals i.CMID
                                where currentElements.Contains(m.CMID)
                                orderby m.LastAliveAck descending
                                select new { Cmid = m.CMID, Name = i.Name, Email = m.Login, LastLogin = m.LastAliveAck };

                    foreach (var row in query)
                    {
                        matchingUsers.Add(new UserV(row.Email, row.Name, (DateTime)row.LastLogin, String.Empty, String.Empty, row.Cmid.ToString()));
                    }
                }
            }

            if (matchingUsers.Count == 1)
            {
                return new JsonResult() { Data = new { Redirect = true, Cmid = matchingUsers[0].UserID } };
            }

            ViewData["usersListToSend"] = matchingUsers;
            ViewBag.PaginationModel = PaginationModel;

            return View("Partial/SearchUsers");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchUsersByPreviousEmail(string previousEmail)
        {
            List<PreviousEmail> previousAccounts = CmuneMember.GetAccountsLinkedToPreviousEmail(previousEmail);
            List<int> cmids = previousAccounts.Select(a => a.Cmid).ToList();
            Dictionary<int, string> accountsName = CmuneMember.GetMembersNames(cmids);

            List<AccountPreviousEmailDisplay> previousAccountDisplay = new List<AccountPreviousEmailDisplay>();

            previousAccountDisplay = previousAccounts.ConvertAll(new Converter<PreviousEmail, AccountPreviousEmailDisplay>(a =>
                new AccountPreviousEmailDisplay(a.PreviousEmailAddress, a.ChangeDate, a.SourceIp, a.Cmid, accountsName[a.Cmid])));

            if (accountsName.Count == 1)
            {
                return new JsonResult() { Data = new { Redirect = true, Cmid = previousAccounts[0].Cmid } };
            }

            return View("Partial/PreviousAccountsByEmail", previousAccountDisplay);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchUsersByPreviousName(string previousName)
        {
            List<PreviousName> previousNames = CmuneMember.GetAccountsLinkedToPreviousName(previousName);
            List<int> cmids = previousNames.Select(a => a.Cmid).ToList();
            Dictionary<int, string> accountsName = CmuneMember.GetMembersNames(cmids);

            List<AccountPreviousNameDisplay> previousAccountDisplay = new List<AccountPreviousNameDisplay>();

            previousAccountDisplay = previousNames.ConvertAll(new Converter<PreviousName, AccountPreviousNameDisplay>(a =>
                new AccountPreviousNameDisplay(a.PreviousUserName, a.ChangeDate, a.SourceIp, a.Cmid, accountsName[a.Cmid])));

            if (accountsName.Count == 1)
            {
                return new JsonResult() { Data = new { Redirect = true, Cmid = previousNames[0].Cmid } };
            }

            return View("Partial/PreviousAccountsByName", previousAccountDisplay);
        }

        #endregion

        protected void GetRanking(int cmid)
        {
            User uberstrikeMember = PPUsers.GetUser(cmid);
            PlayerCard playerCard = new PlayerCard(cmid, true);

            ViewBag.Ranking = "-";
            ViewBag.IsUserExisting = false;

            if (playerCard != null)
            {
                ViewBag.Ranking = playerCard.Ranking.ToString();
            }

            if (uberstrikeMember != null)
            {
                ViewBag.IsUserExisting = true;

                // Overall stats

                ViewBag.Level = uberstrikeMember.Level.ToString();
                ViewBag.Xp = uberstrikeMember.XP.ToString();
                ViewBag.Kills = uberstrikeMember.Splats;
                ViewBag.Deaths = uberstrikeMember.Splatted;

                decimal kdr = 0;

                if (uberstrikeMember.Splatted != 0)
                {
                    kdr = (decimal)uberstrikeMember.Splats / (decimal)uberstrikeMember.Splatted;
                }

                ViewBag.Kdr = kdr.ToString("F2");

                decimal accuracy = 0;

                if (uberstrikeMember.Shots != 0)
                {
                    accuracy = (decimal)uberstrikeMember.Hits / (decimal)uberstrikeMember.Shots;
                }

                ViewBag.Accuracy = accuracy.ToString("P2");

                ViewBag.Shots = uberstrikeMember.Shots;
                ViewBag.Hits = uberstrikeMember.Hits;
                ViewBag.Headshots = uberstrikeMember.Headshots;
                ViewBag.Nutshots = uberstrikeMember.Nutshots;

                // Per life stats

                ViewBag.MostKills = uberstrikeMember.MostSplats;
                ViewBag.MostXpEarned = uberstrikeMember.MostXPEarned;
                ViewBag.MostDamageReceived = uberstrikeMember.MostDamageReceived;
                ViewBag.MostDamageDealt = uberstrikeMember.MostDamageDealt;
                ViewBag.MostHealthPickedUp = uberstrikeMember.MostHealthPickedUp;
                ViewBag.MostArmorPickedUp = uberstrikeMember.MostArmorPickedUp;
                ViewBag.MostHeadshots = uberstrikeMember.MostHeadshots;
                ViewBag.MostNutshots = uberstrikeMember.MostNutshots;
                ViewBag.MostConsecutiveSnipes = uberstrikeMember.MostConsecutiveSnipes;

                // Weapons stats

                ViewBag.MostMeleeKills = uberstrikeMember.MostMeleeSplats;
                ViewBag.TotalMeleeKills = uberstrikeMember.MeleeTotalSplats;

                decimal meleeAccuracy = 0;

                if (uberstrikeMember.MeleeTotalShotsFired != 0)
                {
                    meleeAccuracy = (decimal)uberstrikeMember.MeleeTotalShotsHit / (decimal)uberstrikeMember.MeleeTotalShotsFired;
                }

                ViewBag.MeleeAccuracy = meleeAccuracy.ToString("P2");

                ViewBag.MostHandgunKills = uberstrikeMember.MostHandgunSplats;
                ViewBag.TotalHandgunKills = uberstrikeMember.HandgunTotalSplats;

                decimal handgunAccuracy = 0;

                if (uberstrikeMember.HandgunTotalShotsFired != 0)
                {
                    handgunAccuracy = (decimal)uberstrikeMember.HandgunTotalShotsHit / (decimal)uberstrikeMember.HandgunTotalShotsFired;
                }

                ViewBag.HandgunAccuracy = handgunAccuracy.ToString("P2");

                ViewBag.MostMachineGunKills = uberstrikeMember.MostMachinegunSplats;
                ViewBag.TotalMachineGunKills = uberstrikeMember.MachineGunTotalSplats;

                decimal machineGunAccuracy = 0;

                if (uberstrikeMember.MachineGunTotalShotsFired != 0)
                {
                    machineGunAccuracy = (decimal)uberstrikeMember.MachineGunTotalShotsHit / (decimal)uberstrikeMember.MachineGunTotalShotsFired;
                }

                ViewBag.MachineGunAccuracy = machineGunAccuracy.ToString("P2");

                ViewBag.MostShotgunKills = uberstrikeMember.MostShotgunSplats;
                ViewBag.TotalShotgunKills = uberstrikeMember.ShotgunTotalSplats;

                decimal shotgunAccuracy = 0;

                if (uberstrikeMember.ShotgunTotalShotsFired != 0)
                {
                    shotgunAccuracy = (decimal)uberstrikeMember.ShotgunTotalShotsHit / (decimal)uberstrikeMember.ShotgunTotalShotsFired;
                }

                ViewBag.ShotgunAccuracy = shotgunAccuracy.ToString("P2");

                ViewBag.MostSniperKills = uberstrikeMember.MostSniperSplats;
                ViewBag.TotalSniperKills = uberstrikeMember.SniperTotalSplats;

                decimal sniperAccuracy = 0;

                if (uberstrikeMember.SniperTotalShotsFired != 0)
                {
                    sniperAccuracy = (decimal)uberstrikeMember.SniperTotalShotsHit / (decimal)uberstrikeMember.SniperTotalShotsFired;
                }

                ViewBag.SniperAccuracy = sniperAccuracy.ToString("P2");

                ViewBag.MostSplattergunKills = uberstrikeMember.MostSplattergunSplats;
                ViewBag.TotalSplattergunKills = uberstrikeMember.SplattergunTotalSplats;

                decimal splattergunAccuracy = 0;

                if (uberstrikeMember.SplattergunTotalShotsFired != 0)
                {
                    splattergunAccuracy = (decimal)uberstrikeMember.SplattergunTotalShotsHit / (decimal)uberstrikeMember.SplattergunTotalShotsFired;
                }

                ViewBag.SplattergunAccuracy = splattergunAccuracy.ToString("P2");

                ViewBag.MostCannonKills = uberstrikeMember.MostCannonSplats;
                ViewBag.TotalCannonKills = uberstrikeMember.CannonTotalSplats;

                decimal cannonAccuracy = 0;

                if (uberstrikeMember.CannonTotalShotsFired != 0)
                {
                    cannonAccuracy = (decimal)uberstrikeMember.CannonTotalShotsHit / (decimal)uberstrikeMember.CannonTotalShotsFired;
                }

                ViewBag.CannonAccuracy = cannonAccuracy.ToString("P2");

                ViewBag.MostLauncherKills = uberstrikeMember.MostLauncherSplats;
                ViewBag.TotalLauncherKills = uberstrikeMember.LauncherTotalSplats;

                decimal launcherAccuracy = 0;

                if (uberstrikeMember.LauncherTotalShotsFired != 0)
                {
                    launcherAccuracy = (decimal)uberstrikeMember.LauncherTotalShotsHit / (decimal)uberstrikeMember.LauncherTotalShotsFired;
                }

                ViewBag.LauncherAccuracy = launcherAccuracy.ToString("P2");
            }
        }

        private string GetBanStatusText(BanMode banMode, DateTime? bannedUntil = null)
        {
            string banText = String.Empty;

            if (banMode.Equals(BanMode.No))
            {
                banText = "Not banned";
            }
            else if (banMode.Equals(BanMode.Temporary))
            {
                string banningDateDisplay = "date should not be NULL";

                if (bannedUntil.HasValue)
                {
                    banningDateDisplay = ((DateTime)bannedUntil).ToShortDateString();
                }

                banText = String.Format("Temporary banned until {0}", banningDateDisplay);
            }
            else if (banMode.Equals(BanMode.Permanent))
            {
                banText = "Permanently banned";
            }

            return banText;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Delete(FormCollection form)
        {
            bool isDeleted = false;
            string message = string.Empty;

            string currentName = form["memberName"];
            string currentEmail = form["memberEmail"];
            EmailAddressStatus emailStatus = (EmailAddressStatus)Convert.ToInt32(form["memberEmailStatus"]);
            Cmid = Int32.Parse(form["memberCmid"]);

            String explanationMessage = form["deleteMemberExplanation"];

            MemberOperationResult ret = UsersBusiness.DeleteMember(Cmid);
            CheckOperationResult.CheckMemberOperationResult(ret, ref isDeleted, ref message);
            if (ret.Equals(MemberOperationResult.Ok))
            {
                CmuneMail.SendEmailNotification(EmailNotificationType.DeleteMember, explanationMessage, currentEmail, currentName, emailStatus);
                message = "Please delete the forum user matching with this email address:<br /><br />" + form["memberEmail"];
            }
            var json = new JsonResult() { Data = new { isDeleted = isDeleted, message = message } };
            return json;
        }

        [HttpPost]
        public ActionResult DeleteMemberItem(int cmid, int itemId)
        {
            MemberOperationResult memberOperationResult;
            string message = string.Empty;
            bool isModified = false;

            if (CmuneEconomy.RemoveItem(cmid, itemId))
            {
                memberOperationResult = MemberOperationResult.Ok;
                CheckOperationResult.CheckMemberOperationResult(memberOperationResult, ref isModified, ref message);
            }
            else
            {
                message = "Operation failed";
            }

            var json = new JsonResult() { Data = new { IsModified = isModified, Message = message } };
            return json;
        }

        [ValidateInput(false)]
        public ActionResult LoadCompleteMemberForm(string cmid, string memberEmail, string memberName)
        {
            Cmid = Int32.Parse(cmid);
            return View("Partial/Form/CompleteMemberForm");
        }

        #region Moderation notes

        public ActionResult GetMemberNotes(int memberCmid)
        {
            return View("Partial/MemberNotes", CmuneMember.GetMemberNotesView(memberCmid));
        }

        [ValidateInput(false)]
        public string AddMemberNote(int memberCmid, int addNoteType, string addNoteDescription, int addNoteAdminNames)
        {
            CmuneMember.RecordModerationAction(addNoteAdminNames, memberCmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, DateTime.Now, (ModerationActionType)addNoteType, addNoteDescription);

            return String.Empty;
        }

        public string DeleteMemberNote(int noteId)
        {
            CmuneMember.DeleteMemberNote(noteId);

            return String.Empty;
        }

        public string EditMemberNote(int noteId, int sourceCmid, int actionType, string reason)
        {
            CmuneMember.EditMemberNote(noteId, sourceCmid, (ModerationActionType)actionType, reason);

            return String.Empty;
        }

        #endregion

        #region IP ban

        [HttpGet]
        public ActionResult LoadIpBan()
        {
            ViewData["ipBanAdminNames"] = AdminCache.GenerateAdminDropDownListItems();

            List<SelectListItem> ipBanDurationDropDownList = new List<SelectListItem>();

            for (int i = 1; i < 31; i++)
            {
                bool isSelected = false;

                if (i == 1)
                {
                    isSelected = true;
                }

                ipBanDurationDropDownList.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = isSelected });
            }

            ipBanDurationDropDownList.Add(new SelectListItem() { Text = "Permanent", Value = "0", Selected = false });

            ViewData["ipBanBanUntil"] = ipBanDurationDropDownList;

            return View("Partial/Form/IpBanForm");
        }

        [ValidateInput(false)]
        public string BanIp(int ipBanTargetCmid, string ipBanIpToBan, int ipBanBanUntil, string ipBanReason, int ipBanAdminNames, string unbanIpCheckbox)
        {
            bool isUnban = false;

            if (unbanIpCheckbox == "on")
            {
                isUnban = true;
            }

            if (isUnban)
            {
                CmuneMember.UnbanIp(ipBanIpToBan, ipBanAdminNames, Request.UserHostAddress, ipBanReason);
            }
            else
            {
                DateTime? banUntil = null;

                if (ipBanBanUntil != 0)
                {
                    banUntil = DateTime.Now.AddDays(ipBanBanUntil);
                }

                CmuneMember.BanIp(ipBanAdminNames, ipBanTargetCmid, CommonConfig.ApplicationIdUberstrike, Request.UserHostAddress, ipBanIpToBan, ipBanReason, banUntil);
            }

            return String.Empty;
        }

        [ValidateInput(false)]
        public ActionResult LoadBannedIps(string searchIpTextBox, string loadPermanentBanOnly, int selectedPage)
        {
            int elementPerPage = 50;
            bool displayPermanentBanOnly = false;

            if (loadPermanentBanOnly == "on")
            {
                displayPermanentBanOnly = true;
            }

            List<BannedIpView> bannedIps = new List<BannedIpView>();
            int totalCount = 0;

            if (searchIpTextBox.IsNullOrFullyEmpty())
            {
                bannedIps = CmuneMember.GetBannedIpsViews(selectedPage, elementPerPage, displayPermanentBanOnly);
                totalCount = CmuneMember.GetBannedIpsViewsCount(displayPermanentBanOnly);
            }
            else
            {
                BannedIpView bannedIp = CmuneMember.GetBannedIpView(searchIpTextBox);

                if (bannedIp != null)
                {
                    bannedIps.Add(bannedIp);
                    totalCount = 1;
                }
            }

            var paginationModel = new PaginationModel(totalCount, selectedPage, "BannedIps", elementPerPage);
            ViewBag.PaginationModel = paginationModel;

            return View("Partial/BannedIps", bannedIps);
        }

        #endregion
    }
}