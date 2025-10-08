using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Channels.Instrumentation.Utils.Extensions;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticController : BaseController
    {
        #region Properties

        public DateTime FromDate;
        public DateTime ToDate;

        public string LastQuery = "Empty";

        #endregion

        #region Construtors

        public StatisticController()
            : base()
        {
            // Init vars
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Statistics;

            

            DateTime now = DateTime.Now;

            FromDate = now.AddDays(-30).ToDateOnly();
            ToDate = now.AddDays(-1).ToDateOnly();

            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["DisplayCalendar"] = true;

            UseProdDataContext();
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // User selected dates?
            if (HttpContext.Request.Params != null && HttpContext.Request.Params["startdate"] != null)
                FromDate = Convert.ToDateTime(HttpContext.Request.Params["startdate"]);

            if (HttpContext.Request.Params != null && HttpContext.Request.Params["enddate"] != null)
                ToDate = Convert.ToDateTime(HttpContext.Request.Params["enddate"]);

            base.OnActionExecuting(filterContext);
        }

        #region Actions

        /// <summary>
        /// First Sub Tab Dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Dashboard;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Dashboard);
            return View();
        }

        public ActionResult UserActivity()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.User_Activity;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.User_Activity);

            // Map usage

            List<SelectListItem> mapOneUsageSelect = AdminCache.GenerateMapUsageSelectItems(FromDate, ToDate, true);
            List<SelectListItem> mapDefaultSelect = AdminCache.GenerateMapUsageSelectItems(FromDate, ToDate);

            ViewData["mapOneUsage"] = mapOneUsageSelect;
            ViewData["mapTwoUsage"] = mapDefaultSelect;
            ViewData["mapThreeUsage"] = mapDefaultSelect;

            string selectedMapOneUsage = "-1";

            if (mapOneUsageSelect.Count > 1)
            {
                selectedMapOneUsage = mapOneUsageSelect[1].Value;
            }

            ViewBag.SelectedMapOneUsage = selectedMapOneUsage;


            return View();
        }

        public ActionResult Revenue()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Revenue;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Revenue);
            return View();
        }

        public ActionResult Regions()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Regions;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Regions);

            // Daily revenue

            List<SelectListItem> regionOneDailyRevenueList = AdminCache.GenerateCountriesRevenueDropDownListItems(FromDate, ToDate, true);

            ViewData["regionOneDailyRevenue"] = regionOneDailyRevenueList;
            ViewData["regionTwoDailyRevenue"] = AdminCache.GenerateCountriesRevenueDropDownListItems(FromDate, ToDate);
            ViewData["regionThreeDailyRevenue"] = AdminCache.GenerateCountriesRevenueDropDownListItems(FromDate, ToDate);

            string selectedRegionOneDailyRevenue = "-1";

            if (regionOneDailyRevenueList.Count > 1)
            {
                selectedRegionOneDailyRevenue = regionOneDailyRevenueList[1].Value;
            }

            ViewBag.SelectedRegionOneDailyRevenue = selectedRegionOneDailyRevenue;

            // Dau
            // Mau
            // Daily Play Rate
            // Darpu

            List<SelectListItem> regionMauList = AdminCache.GenerateCountriesMauDropDownListItems(ToDate, true, false);

            ViewData["regionDau"] = regionMauList;
            ViewData["regionMau"] = regionMauList;
            ViewData["regionDailyPlayRate"] = regionMauList;
            ViewData["regionDarpu"] = regionMauList;

            string selectedRegionDau = "0";
            string selectedRegionMau = "0";
            string selectedRegionDailyPlayRate = "0";
            string selectedRegionDarpu = "0";

            if (regionMauList.Count > 0)
            {
                selectedRegionDau = regionMauList[0].Value;
                selectedRegionMau = regionMauList[0].Value;
                selectedRegionDailyPlayRate = regionMauList[0].Value;
                selectedRegionDarpu = regionMauList[0].Value;
            }

            ViewBag.SelectedDauRegion = selectedRegionDau;
            ViewBag.SelectedMauRegion = selectedRegionMau;
            ViewBag.SelectedDailyPlayRateRegion = selectedRegionDailyPlayRate;
            ViewBag.SelectedDarpuRegion = selectedRegionDarpu;

            return View();
        }

        public ActionResult Channels(int? channel)
        {
            ChannelType channelId = ChannelType.WebPortal;
            ChannelType parsedChannel = ChannelType.WebPortal;

            if (channel.HasValue && EnumUtilities.TryParseEnumByValue((int)channel, out parsedChannel))
            {
                channelId = parsedChannel;
            }

            ViewBag.Channel = (int) channelId;
            ViewBag.Title += String.Format(" | {0} | {1}", CmuneMenu.StatisticSubTabs.Channels, channelId);
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Channels;
            return View();
        }

        public ActionResult Referrers()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Referrers;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Referrers);

            List<SelectListItem> referrers = AdminCache.GenerateReferrerDropDownListItems();
            List<SelectListItem> referrersWithNone = AdminCache.GenerateReferrerDropDownListItems(true);

            string selectedReferrerOne = ((int)ReferrerPartnerType.Applifier).ToString();
            string selectedReferrerTwo = ((int)ReferrerPartnerType.None).ToString();

            ViewData["referrerOne"] = referrers.SetSelectValue(selectedReferrerOne);
            ViewData["referrerTwo"] = referrersWithNone.ConvertAll(new Converter<SelectListItem,SelectListItem>(t => new SelectListItem { Selected = t.Selected, Text = t.Text, Value = t.Value})).SetSelectValue(selectedReferrerTwo);
            ViewData["referrerThree"] = referrersWithNone;

            ViewBag.SelectedReferrerOne = selectedReferrerOne;
            ViewBag.SelectedReferrerTwo = selectedReferrerTwo;

            return View();
        }

        public ActionResult ItemEconomy()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Item_Economy;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Item_Economy);

            // Credit items contribution

            List<SelectListItem> itemOneCreditsContributionList = AdminCache.GenerateItemCreditSaleDropDownListItems(CommonConfig.ApplicationIdUberstrike, FromDate, ToDate, true);
            List<SelectListItem> itemDefaulCreditsContributiontList = AdminCache.GenerateItemCreditSaleDropDownListItems(CommonConfig.ApplicationIdUberstrike, FromDate, ToDate);

            ViewData["itemOneCreditsContribution"] = itemOneCreditsContributionList;
            ViewData["itemTwoCreditsContribution"] = itemDefaulCreditsContributiontList;
            ViewData["itemThreeCreditsContribution"] = itemDefaulCreditsContributiontList;

            string selectedItemOneCreditsContribution = "-1";

            if (itemOneCreditsContributionList.Count > 1)
            {
                selectedItemOneCreditsContribution = itemOneCreditsContributionList[1].Value;
            }

            ViewBag.SelectedItemOneCreditsContribution = selectedItemOneCreditsContribution;

            return View();
        }

        public ActionResult Social()
        {
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Social;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Social);

            return View();
        }

        public ActionResult PaymentProviders(int? providerId)
        {
            PaymentProviderType paymentProviderId = PaymentProviderType.PlaySpan;
            string paymentProviderName = CommonConfig.PaymentProviderName[paymentProviderId];
            bool showPackageContribution = true;

            if (providerId.HasValue && EnumUtilities.TryParseEnumByValue((int)providerId, out paymentProviderId))
            {
                paymentProviderName = CommonConfig.PaymentProviderName[paymentProviderId];

                if (paymentProviderId == PaymentProviderType.Dotori || paymentProviderId == PaymentProviderType.SuperRewards)
                {
                    showPackageContribution = false;
                }
            }

            ViewBag.PaymentProviderId = (int) paymentProviderId;
            ViewBag.PaymentProviderName = paymentProviderName;
            ViewBag.ShowPackageContribution = showPackageContribution;
            ViewBag.Title += String.Format(" | {0} | {1}", CmuneMenu.StatisticSubTabs.Payment, paymentProviderName);
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Payment;

            return View();
        }

        public ActionResult Retention()
        {
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Retention);
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Retention;

            List<SelectListItem> regionMauList = AdminCache.GenerateCountriesMauDropDownListItems(ToDate, true, false);

            ViewData["regionRetentionCohorts"] = regionMauList;
            ViewBag.BiggestCountry = "0";

            if (regionMauList.Count > 0)
            {
                ViewBag.BiggestCountry = regionMauList[0].Value;
            }

            return View();
        }

        public ActionResult UnityConversionFunnel()
        {
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.UnityInstallation);
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.UnityInstallation;

            var facebookBrowsersDdl = new List<SelectListItem>();
            facebookBrowsersDdl.Add(new SelectListItem{Text = "Windows / IE 9", Value = "Windows|Explorer|9", Selected = true});
            facebookBrowsersDdl.Add(new SelectListItem { Text = "Windows / IE 8", Value = "Windows|Explorer|8", Selected = false });
            facebookBrowsersDdl.Add(new SelectListItem { Text = "Windows / IE 7", Value = "Windows|Explorer|7", Selected = false });
            facebookBrowsersDdl.Add(new SelectListItem { Text = "Windows / Chrome", Value = "Windows|Chrome|", Selected = false });
            facebookBrowsersDdl.Add(new SelectListItem { Text = "Windows / Firefox", Value = "Windows|Firefox|", Selected = false });

            ViewData["facebookBrowsers"] = facebookBrowsersDdl;
            ViewBag.SelectedFacebookBrowser = "Windows|Explorer|9";

            var portalBrowsersDdl = new List<SelectListItem>();
            portalBrowsersDdl.Add(new SelectListItem { Text = "Windows / IE 9", Value = "Windows|Explorer|9", Selected = true });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Windows / IE 8", Value = "Windows|Explorer|8", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Windows / IE 7", Value = "Windows|Explorer|7", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Windows / Chrome", Value = "Windows|Chrome|", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Windows / Firefox", Value = "Windows|Firefox|", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Mac / Safari", Value = "Mac|Safari|", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Mac / Firefox", Value = "Mac|Firefox|", Selected = false });
            portalBrowsersDdl.Add(new SelectListItem { Text = "Mac / Chrome", Value = "Mac|Chrome|", Selected = false });

            ViewData["portalBrowsers"] = portalBrowsersDdl;
            ViewBag.SelectedPortalBrowser = "Windows|Explorer|9";

            // Custom query

            var customQueryChannel = AdminCache.GenerateChannelDropDownListItems(true);
            customQueryChannel.RemoveAll(c => (new List<string> { ((int)ChannelType.MacAppStore).ToString(), ((int)ChannelType.WebCyworld).ToString(), ((int)ChannelType.OSXStandalone).ToString(), ((int)ChannelType.IPhone).ToString(), ((int)ChannelType.IPad).ToString(), ((int)ChannelType.Android).ToString() }).Contains(c.Value));
            ViewData["customQueryChannel"] = customQueryChannel;

            var customQueryOsName = AdminCache.GenerateOsNameDropDownListItems(FromDate, ToDate, null);
            ViewData["customQueryOsName"] = customQueryOsName;

            var customQueryOsVersion = new List<SelectListItem>();

            customQueryOsVersion.Add(new SelectListItem { Text = "All", Value = "-1", Selected = true });

            ViewData["customQueryOsVersion"] = customQueryOsVersion;

            var customQueryBrowserName = AdminCache.GenerateBrowserNameDropDownListItems(FromDate, ToDate, null, "-1", "-1", true);
            ViewData["customQueryBrowserName"] = customQueryBrowserName;

            var customQuerybrowserVersion = new List<SelectListItem>();

            customQuerybrowserVersion.Add(new SelectListItem { Text = "All", Value = "-1", Selected = true });

            ViewData["customQuerybrowserVersion"] = customQuerybrowserVersion;

            var customQueryReferrer = AdminCache.GenerateReferrerDropDownListItems(true);
            ViewData["customQueryReferrer"] = customQueryReferrer;

            var customQueryJava = new List<SelectListItem>();

            customQueryJava.Add(new SelectListItem { Text = "All", Value = "-1", Selected = true });
            customQueryJava.Add(new SelectListItem { Text = "JavaInstallIsEnabled", Value = "1", Selected = false });
            customQueryJava.Add(new SelectListItem { Text = "JavaInstallIsNotEnabled", Value = "0", Selected = false });

            ViewData["customQueryJava"] = customQueryJava;

            ViewBag.CustomQueryChartBaseUrl = String.Format("?customQueryChannel=-1&customQueryOsName=-1&customQueryOsVersion=-1&customQueryBrowserName=-1&customQueryBrowserVersion=-1&customQueryReferrer=-1&customQueryJava=-1");

            // We try to read the tracking cookie do diplay the matching steps (makes tester life easier :))

            string trackingCookieName = "uberstrike_install";
            string trackingStateCookieName = "uberstrike_install_step";

            ViewBag.TrackingCookieName = trackingCookieName;
            ViewBag.TrackingStateCookieName = trackingStateCookieName;

            return View();
        }

        public ActionResult TutorialConversionFunnel()
        {
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.StatisticSubTabs.Tutorial);
            ViewData["SubTitle"] = ViewBag.SubActiveTab = CmuneMenu.StatisticSubTabs.Tutorial;

            return View();
        }

        #endregion

        /// <summary>
        /// Computes the revenue
        /// </summary>
        /// <param name="date"></param>
        /// <param name="creditDeposit"></param>
        /// <param name="payingMembers"></param>
        /// <param name="payments"></param>
        void ComputeRevenue(DateTime date, CreditDeposit creditDeposit, ref Dictionary<int, int> payingMembers, ref Dictionary<DateTime, RevenueStorage> payments)
        {
            RevenueStorage revenueStorage = payments[date];
            revenueStorage.Revenue += (int)creditDeposit.NbCash;
            revenueStorage.TransactionsCount += 1;

            if (!payingMembers.ContainsKey(creditDeposit.UserId))
            {
                payingMembers.Add(creditDeposit.UserId, creditDeposit.UserId);
                revenueStorage.PayingMembersCount += 1;
            }

            payments[date] = revenueStorage;
        }

        // TODO: Can probably be replaced by a constructor and a ConvertAll
        /// <summary>
        /// Transfers the revenue to display
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public List<RevenueDisplay> TransferRevenueToDisplay(Dictionary<DateTime, RevenueStorage> payments)
        {
            List<RevenueDisplay> revenueDisplay = new List<RevenueDisplay>();

            foreach (DateTime payment in payments.Keys)
            {
                decimal arppu = 0;

                if (payments[payment].PayingMembersCount > 0)
                {
                    arppu = (decimal)payments[payment].Revenue / (decimal)payments[payment].PayingMembersCount;
                }

                revenueDisplay.Add(new RevenueDisplay(payment, payments[payment].Revenue, arppu, payments[payment].TransactionsCount));
            }

            return revenueDisplay;
        }

        #region Methods

        #region Dashboard Tab

        /// <summary>
        /// Called by SWF to get data to display
        /// </summary>
        /// <returns></returns>
        public string GetDauMau()
        {
            return new Cmune.Channels.Instrumentation.Business.Chart.DashboardChart(FromDate, ToDate).DrawDauMauChart().ToPrettyString();
        }

        /// <summary>
        /// called by swf to get data to display in daily revenue graph
        /// </summary>
        /// <returns></returns>
        public string GetDailyRevenue()
        {
            return new Cmune.Channels.Instrumentation.Business.Chart.DashboardChart(FromDate, ToDate).DrawDailyRevenueChart().ToPrettyString();
        }

        #endregion

        #region User Activity Tab

        public string GetDau()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawDauChart().ToPrettyString();
        }

        public string GetNewVsReturning()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawNewVsReturningChart().ToPrettyString();
        }

        public string GetMau()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawMauChart().ToPrettyString();
        }

        public string GetPlayRate()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawDailyPlayRateChart().ToPrettyString();
        }

        public string GetInstallFlow(string hasUnity)
        {
            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawInstallFlowChart(hasUnityTmp).ToPrettyString();
        }

        public string GetInstallFlowLine(string hasUnity)
        {
            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawInstallFlowLineChart(hasUnityTmp).ToPrettyString();
        }

        public JsonResult GetMapUsageSelect()
        {
            return Json(AdminCache.GenerateMapUsageSelectItems(FromDate, ToDate));
        }

        public string GetMapsUsage(int? mapOneUsage, int? mapTwoUsage, int? mapThreeUsage, int? mapGroupModeUsage)
        {
            int mapOne = 0;
            int mapTwo = 0;
            int mapThree = 0;
            GameModeType gameModeId = GameModeType.None;

            if (mapOneUsage.HasValue)
            {
                mapOne = mapOneUsage.Value;
            }

            if (mapTwoUsage.HasValue)
            {
                mapTwo = mapTwoUsage.Value;
            }

            if (mapThreeUsage.HasValue)
            {
                mapThree = mapThreeUsage.Value;
            }

            if (mapGroupModeUsage.HasValue)
            {
                EnumUtilities.TryParseEnumByValue(mapGroupModeUsage.Value, GameModeType.None, out gameModeId);
            }

            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawMapUsageChart(mapOne, mapTwo, mapThree, gameModeId).ToPrettyString();
        }

        #endregion

        #region Revenue Tab

        public string GetDailyRevenueByPaymentProvider()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyRevenueByPaymentProvider().ToPrettyString();
        }

        public string GetDailyAverageRevenuePerUser()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyAverageRevenuePerUserChart().ToPrettyString();
        }

        public string GetDailyAverageRevenuePerPayingUser()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyAverageRevenuePerPayingUserChart().ToPrettyString();
        }

        public string GetDailyConversionToPaying()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyConversionToPayingChart().ToPrettyString();
        }

        public string GetDailyTransactions()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyTransactionsChart().ToPrettyString();
        }

        public string GetDailyRevenueByChannel()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawDailyRevenueByChannel().ToPrettyString();
        }

        public string GetPackageContributionByRevenue()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawPackageContributionByRevenueChart(true).ToPrettyString();
        }

        public string GetPackageContributionByVolume()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawPackageContributionByVolumeChart(true).ToPrettyString();
        }

        public string GetBundleContributionByRevenue()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawPackageContributionByRevenueChart(false).ToPrettyString();
        }

        public string GetBundleContributionByVolume()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawPackageContributionByVolumeChart(false).ToPrettyString();
        }

        public string GetPaymentProviderContribution()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawPaymentPoviderContributionChart().ToPrettyString();
        }

        public string GetChannelContribution()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawChannelContributionChart().ToPrettyString();
        }

        public string GetCreditsSales()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawCreditsSalesChart().ToPrettyString();
        }

        public string GetBundlesSales()
        {
            return new Business.Chart.RevenueChart(FromDate, ToDate).DrawBundlesSalesChart().ToPrettyString();
        }

        public string GetTotalRevenue()
        {
            return AdminCache.LoadTotalRevenue(FromDate, ToDate).ToString("N2");
        }

        #endregion

        #region Social Tab

        public string GetPlayerLevelDistribution(string activeOnly)
        {
            bool getActivePlayersOnly = false;

            if (!activeOnly.IsNullOrFullyEmpty())
            {
                bool isParsed = Boolean.TryParse(activeOnly, out getActivePlayersOnly);
            }

            DateTime statDate = ToDate.ToDateOnly();

            return new Business.Chart.SocialChart(FromDate, ToDate).DrawPlayerLevelDistributionChart(statDate, getActivePlayersOnly).ToPrettyString();
        }

        public string GetFriendsCount(int channelId)
        {
            ChannelType channel;
            EnumUtilities.TryParseEnumByValue(channelId, out channel);

            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawFriendsCountChart(channel).ToPrettyString();
        }

        public string GetActiveFriendsCount(int channelId)
        {
            ChannelType channel;
            EnumUtilities.TryParseEnumByValue(channelId, out channel);

            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawActiveFriendsCountChart(channel).ToPrettyString();
        }

        #endregion      

        #region Payment Tab

        public string GetPackagesChart(int partnerId)
        {
            return new Business.Chart.PaymentProviderChart(FromDate, ToDate).DrawPackagesChart((PaymentProviderType) partnerId).ToPrettyString();
        }

        #endregion

        #region Tutorial Conversion Funnel

        public PartialViewResult GetTutorialSteps(int cmid)
        {
            List<TutorialStepView> steps = Tracking.GetTutorialSteps(cmid);

            return PartialView("Partial/TutorialSteps", steps);
        }

        public string GetTutorialStepsChart()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawTutorialChart().ToPrettyString();
        }

        public string GetTutorialStepsLineChart()
        {
            return new Business.Chart.UserActivityChart(FromDate, ToDate).DrawTutorialLineChart().ToPrettyString();
        }

        #endregion

        #endregion
    }
}