using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.Channels.Instrumentation.Business;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class StatisticsUnityConversionFunnelController : BaseController
    {
        #region Fields

        protected ChannelType _channel;

        #endregion

        #region properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        
        public ChannelType Channel
        {
            get { return _channel; }
            protected set { _channel = value; }
        }

        #endregion

        #region Constructors

        public StatisticsUnityConversionFunnelController()
            : base()
        {
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

            Channel = ChannelType.WebPortal;

            if (HttpContext.Request.Params != null)
            {
                if (HttpContext.Request.Params["channel"] != null)
                {
                    EnumUtilities.TryParseEnumByValue(HttpContext.Request.Params["channel"], out _channel);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        #region Methods

        public string GetOsDistribution()
        {
            return new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawOsContributionChart(Channel).ToPrettyString();
        }

        [ValidateInput(false)]
        public string GetBrowserDistribution(string os)
        {
            return new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawBrowserContributionChart(Channel, os).ToPrettyString();
        }

        [ValidateInput(false)]
        public string GetInstallFlow(string selectedBrowser, string hasUnity)
        {
            string chartData = String.Empty;
            string[] data = selectedBrowser.Split('|');
            
            if (data.Length == 3)
            {
                string os = data[0];
                string browserName = data[1];
                string browserVersion = data[2];

                bool hasUnityTmp = false;

                if (hasUnity == "0")
                {
                    hasUnityTmp = false;
                }
                else if (hasUnity == "1")
                {
                    hasUnityTmp = true;
                }

                chartData = new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawInstallFlowChart(Channel, os, browserName, browserVersion, hasUnityTmp).ToPrettyString();
            }

            return chartData;
        }

        [ValidateInput(false)]
        public string GetInstallFlowLine(string selectedBrowser, string hasUnity)
        {
            string chartData = String.Empty;
            string[] data = selectedBrowser.Split('|');

            if (data.Length == 3)
            {
                string os = data[0];
                string browserName = data[1];
                string browserVersion = data[2];

                bool hasUnityTmp = false;

                if (hasUnity == "0")
                {
                    hasUnityTmp = false;
                }
                else if (hasUnity == "1")
                {
                    hasUnityTmp = true;
                }

                chartData = new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawInstallFlowLineChart(Channel, os, browserName, browserVersion, hasUnityTmp).ToPrettyString();
            }

            return chartData;
        }

        [HttpPost]
        public ActionResult GetSteps(string trackingId)
        {
            trackingId = trackingId.Trim();
            return PartialView("~/Views/Statistic/Partial/UnityInstallationSteps.cshtml", InstallTrackingBusiness.GetSteps(trackingId));
        }

        public JsonResult GetOsNameDropDownList(string customQueryChannel)
        {
            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            return Json(AdminCache.GenerateOsNameDropDownListItems(FromDate, ToDate, channel));
        }

        [ValidateInput(false)]
        public JsonResult GetOsVersionDropDownList(string customQueryChannel, string customQueryOsName)
        {
            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            return Json(AdminCache.GenerateOsVersionDropDownListItems(FromDate, ToDate, channel, customQueryOsName, true));
        }

        [ValidateInput(false)]
        public JsonResult GetBrowserNameDropDownList(string customQueryChannel, string customQueryOsName, string customQueryOsVersion)
        {
            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            return Json(AdminCache.GenerateBrowserNameDropDownListItems(FromDate, ToDate, channel, customQueryOsName, customQueryOsVersion, true));
        }

        [ValidateInput(false)]
        public JsonResult GetBrowserVersionDropDownList(string customQueryChannel, string customQueryOsName, string customQueryOsVersion, string customQueryBrowserName)
        {
            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            return Json(AdminCache.GenerateBrowserVersionDropDownListItems(FromDate, ToDate, channel, customQueryOsName, customQueryOsVersion, customQueryBrowserName, true));
        }

        [ValidateInput(false)]
        public string GetCustomInstallFlow(string customQueryChannel, string customQueryOsName, string customQueryOsVersion, string customQueryBrowserName, string customQueryBrowserVersion, string customQueryReferrer, string hasUnity, string customQueryJava)
        {
            string chartData = String.Empty;

            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            string osName = String.Empty;

            if (customQueryOsName != "-1")
            {
                osName = customQueryOsName;
            }

            string osVersion = String.Empty;

            if (customQueryOsVersion != "-1")
            {
                osVersion = customQueryOsVersion;
            }

            string browserName = String.Empty;

            if (customQueryBrowserName != "-1")
            {
                browserName = customQueryBrowserName;
            }

            string browserVersion = String.Empty;

            if (customQueryBrowserVersion != "-1")
            {
                browserVersion = customQueryBrowserVersion;
            }

            ReferrerPartnerType? referrerId = null;

            if (customQueryReferrer != "0")
            {
                ReferrerPartnerType tmpReferrerId;

                if (EnumUtilities.TryParseEnumByValue(customQueryReferrer, out tmpReferrerId))
                {
                    referrerId = tmpReferrerId;
                }
            }

            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            bool? isJavaInstallEnabled = null;

            if (customQueryJava == "1")
            {
                isJavaInstallEnabled = true;
            }
            else if (customQueryJava == "0")
            {
                isJavaInstallEnabled = false;
            }

            chartData = new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawInstallFlowChart(channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnityTmp, isJavaInstallEnabled).ToPrettyString();

            return chartData;
        }

        [ValidateInput(false)]
        public string GetCustomInstallFlowLine(string customQueryChannel, string customQueryOsName, string customQueryOsVersion, string customQueryBrowserName, string customQueryBrowserVersion, string customQueryReferrer, string hasUnity, string customQueryJava)
        {
            string chartData = String.Empty;

            ChannelType? channel = null;

            if (customQueryChannel != "-1")
            {
                ChannelType tmpChannel;

                if (EnumUtilities.TryParseEnumByValue(customQueryChannel, out tmpChannel))
                {
                    channel = tmpChannel;
                }
            }

            string osName = String.Empty;

            if (customQueryOsName != "-1")
            {
                osName = customQueryOsName;
            }

            string osVersion = String.Empty;

            if (customQueryOsVersion != "-1")
            {
                osVersion = customQueryOsVersion;
            }

            string browserName = String.Empty;

            if (customQueryBrowserName != "-1")
            {
                browserName = customQueryBrowserName;
            }

            string browserVersion = String.Empty;

            if (customQueryBrowserVersion != "-1")
            {
                browserVersion = customQueryBrowserVersion;
            }

            ReferrerPartnerType? referrerId = null;

            if (customQueryReferrer != "0")
            {
                ReferrerPartnerType tmpReferrerId;

                if (EnumUtilities.TryParseEnumByValue(customQueryReferrer, out tmpReferrerId))
                {
                    referrerId = tmpReferrerId;
                }
            }

            bool hasUnityTmp = false;

            if (hasUnity == "0")
            {
                hasUnityTmp = false;
            }
            else if (hasUnity == "1")
            {
                hasUnityTmp = true;
            }

            bool? isJavaInstallEnabled = null;

            if (customQueryJava == "1")
            {
                isJavaInstallEnabled = true;
            }
            else if (customQueryJava == "0")
            {
                isJavaInstallEnabled = false;
            }

            chartData = new Business.Chart.UnityConversionFunnelChart(FromDate, ToDate).DrawInstallFlowLineChart(channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnityTmp, isJavaInstallEnabled).ToPrettyString();

            return chartData;
        }

        #endregion
    }
}