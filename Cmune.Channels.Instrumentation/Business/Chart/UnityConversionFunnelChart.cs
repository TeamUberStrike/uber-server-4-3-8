using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class UnityConversionFunnelChart : StatisticChart
    {
        #region Properties

        public Dictionary<string, int> OsDistribution { get; private set; }
        public Dictionary<string, int> BrowserDistribution { get; private set; }

        // Install Flow related
        protected Dictionary<DateTime, int> TrackingInitialState;
        protected Dictionary<DateTime, int> TrackingClickDownload;
        protected Dictionary<DateTime, int> TrackingUnityInitialized;
        protected Dictionary<DateTime, int> TrackingUnityInstalled;
        protected Dictionary<DateTime, int> TrackingFullGameLoaded;
        protected Dictionary<DateTime, int> TrackingAccountCreated;
        protected Dictionary<DateTime, int> TrackingClickCancel;

        #endregion

        #region Constructors

        public UnityConversionFunnelChart(DateTime FromDate, DateTime ToDate)
                : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitOsContributionChart(ChannelType channel)
        {
            OsDistribution = AdminCache.LoadOsNameDistribution(FromDate, ToDate, channel);
        }

        public OpenFlashChart.OpenFlashChart DrawOsContributionChart(ChannelType channel)
        {
            InitOsContributionChart(channel);

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<string, int> kvp in OsDistribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, kvp.Key.ToString()));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "#val# of #total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title("Amongst No Unity:");
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected void InitBrowserContributionChart(ChannelType channel, string operatingSystem)
        {
            BrowserDistribution = AdminCache.LoadBrowserDistribution(FromDate, ToDate, channel, operatingSystem);
        }

        public OpenFlashChart.OpenFlashChart DrawBrowserContributionChart(ChannelType channel, string operatingSystem)
        {
            InitBrowserContributionChart(channel, operatingSystem);

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<string, int> kvp in BrowserDistribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, kvp.Key.ToString()));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "#val# of #total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title(String.Format("On {0}", operatingSystem));
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected void InitializeInstallFlowData(ChannelType channel, string operatingSystem, string browserName, string browserVersion, bool hasUnity)
        {
            if (hasUnity)
            {
                TrackingInitialState = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.HasUnity, channel, operatingSystem, browserName, browserVersion, hasUnity);
            }
            else
            {
                TrackingInitialState = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.NoUnity, channel, operatingSystem, browserName, browserVersion, hasUnity);
            }

            TrackingClickDownload = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.ClickDownload, channel, operatingSystem, browserName, browserVersion, hasUnity);
            TrackingUnityInstalled = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.UnityInstalled, channel, operatingSystem, browserName, browserVersion, hasUnity);
            TrackingUnityInitialized = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.UnityInitialized, channel, operatingSystem, browserName, browserVersion, hasUnity);
            TrackingFullGameLoaded = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.FullGameLoaded, channel, operatingSystem, browserName, browserVersion, hasUnity);
            TrackingAccountCreated = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.AccountCreated, channel, operatingSystem, browserName, browserVersion, hasUnity);
            TrackingClickCancel = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.ClickCancel, channel, operatingSystem, browserName, browserVersion, hasUnity);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowChart(ChannelType channel, string operatingSystem, string browserName, string browserVersion, bool hasUnity)
        {
            InitializeInstallFlowData(channel, operatingSystem, browserName, browserVersion, hasUnity);

            return DrawInstallFlowChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowLineChart(ChannelType channel, string operatingSystem, string browserName, string browserVersion, bool hasUnity)
        {
            InitializeInstallFlowData(channel, operatingSystem, browserName, browserVersion, hasUnity);

            return DrawInstallFlowLineChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        protected void InitializeInstallFlowData(ChannelType? channel, string osName, string osVersion, string browserName, string browserVersion, ReferrerPartnerType? referrerId, bool hasUnity, bool? isJavaInstallEnabled)
        {
            if (hasUnity)
            {
                TrackingInitialState = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.HasUnity, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            }
            else
            {
                TrackingInitialState = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.NoUnity, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            }

            TrackingClickDownload = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.ClickDownload, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            TrackingUnityInstalled = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.UnityInstalled, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            TrackingUnityInitialized = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.UnityInitialized, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            TrackingFullGameLoaded = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.FullGameLoaded, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            TrackingAccountCreated = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.AccountCreated, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
            TrackingClickCancel = AdminCache.LoadStep(FromDate, ToDate, UserInstallStepType.ClickCancel, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowChart(ChannelType? channel, string osName, string osVersion, string browserName, string browserVersion, ReferrerPartnerType? referrerId, bool hasUnity, bool? isJavaInstallEnabled)
        {
            InitializeInstallFlowData(channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);

            return DrawInstallFlowChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowLineChart(ChannelType? channel, string osName, string osVersion, string browserName, string browserVersion, ReferrerPartnerType? referrerId, bool hasUnity, bool? isJavaInstallEnabled)
        {
            InitializeInstallFlowData(channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);

            return DrawInstallFlowLineChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        #endregion
    }
}