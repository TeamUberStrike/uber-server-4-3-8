using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenFlashChart;
using Cmune.Channels.Instrumentation.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class UserActivityChart : StatisticChart
    {
        #region Properties

        protected Dictionary<DateTime, float> DailyPlayRate;

        /* user activity tab */
        protected Dictionary<DateTime, int> DailyActiveUsers; /* used in user activity tab too */
        protected Dictionary<DateTime, int> MonthlyActiveUsers; /* used in user activity tab too */
        protected Dictionary<ChannelType, Dictionary<DateTime, int>> DailyActiveUsersByChannels;
        protected Dictionary<ChannelType, Dictionary<DateTime, int>> MonthlyActiveUsersByChannels;
        protected Dictionary<DateTime, IntPair> NewVersusReturning;

        // Install Flow related
        protected Dictionary<DateTime, int> TrackingInitialState;
        protected Dictionary<DateTime, int> TrackingClickDownload;
        protected Dictionary<DateTime, int> TrackingUnityInitialized;
        protected Dictionary<DateTime, int> TrackingUnityInstalled;
        protected Dictionary<DateTime, int> TrackingFullGameLoaded;
        protected Dictionary<DateTime, int> TrackingAccountCreated;
        protected Dictionary<DateTime, int> TrackingClickCancel;

        public Dictionary<DateTime, Dictionary<TutorialStepType, int>> TutorialSteps { get; private set; }

        public Dictionary<DateTime, int> AverageFriendsCount { get; private set; }
        public Dictionary<DateTime, int> AverageActiveFriendsCount { get; private set; }
        public Dictionary<DateTime, int> MedianFriendsCount { get; private set; }
        public Dictionary<DateTime, int> MedianActiveFriendsCount { get; private set; }

        public Dictionary<DateTime, int> MapOneUsage { get; private set; }
        public Dictionary<DateTime, int> MapTwoUsage { get; private set; }
        public Dictionary<DateTime, int> MapThreeUsage { get; private set; }
        public Dictionary<int, string> MapNames { get; private set; }

        #endregion

        #region Constructor

        public UserActivityChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitDauchart()
        {
            DailyActiveUsers = AdminCache.LoadDailyActiveUsers(FromDate, ToDate);
            DailyActiveUsersByChannels = AdminCache.LoadDailyActiveUsersByChannels(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart()
        {
            InitDauchart();

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<LineDotValue> lineTotalValues = new List<LineDotValue>();
            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in DailyActiveUsersByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            int max = 0;

            // Total

            foreach (KeyValuePair<DateTime, int> kvp in DailyActiveUsers)
            {
                lineTotalValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("All Channels: {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            areaChart.Values = lineTotalValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, int> kvp in DailyActiveUsersByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("{0}: {1} Users ({2})", channel, kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart dauChart = ChartUtil.CreateDefaultChart("Daily Active Users", (double)max);
            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dauChart.AddElement(areaChart);

            foreach (ChannelType channel in values.Keys)
            {
                dauChart.AddElement(charts[channel]);
            }

            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        protected void InitDrawMauChart()
        {
            MonthlyActiveUsers = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate);
            MonthlyActiveUsersByChannels = AdminCache.LoadMonthlyActiveUsersByChannels(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawMauChart()
        {
            InitDrawMauChart();

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<LineDotValue> lineTotalValues = new List<LineDotValue>();
            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in MonthlyActiveUsersByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            int max = 0;

            // Total

            foreach (KeyValuePair<DateTime, int> kvp in MonthlyActiveUsers)
            {
                lineTotalValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("All Channels: {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            areaChart.Values = lineTotalValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, int> kvp in MonthlyActiveUsersByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("{0}: {1} Users ({2})", channel, kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart mauObject = ChartUtil.CreateDefaultChart("Monthly Active Users", max);
            mauObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            mauObject.AddElement(areaChart);

            foreach (ChannelType channel in values.Keys)
            {
                mauObject.AddElement(charts[channel]);
            }

            mauObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return mauObject;
        }

        protected void InitNewVsReturningChart()
        {
            NewVersusReturning = AdminCache.LoadNewVsReturningUsers(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawNewVsReturningChart()
        {
            InitNewVsReturningChart();

            return DrawNewVersusReturningChart(NewVersusReturning);
        }

        protected void InitDailyPlayRateChart()
        {
            DailyPlayRate = AdminCache.LoadDailyPlayRate(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyPlayRateChart()
        {
            InitDailyPlayRateChart();

            return DrawDailyPlayRateChart(DailyPlayRate);
        }

        protected void InitializeInstallFlowData(bool hasUnity)
        {
            if (hasUnity)
            {
                TrackingInitialState = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.HasUnity, hasUnity);
            }
            else
            {
                TrackingInitialState = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.NoUnity, hasUnity);
            }

            TrackingClickDownload = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.ClickDownload, hasUnity);
            TrackingUnityInstalled = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.UnityInstalled, hasUnity);
            TrackingUnityInitialized = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.UnityInitialized, hasUnity);
            TrackingFullGameLoaded = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.FullGameLoaded, hasUnity);
            TrackingAccountCreated = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.AccountCreated, hasUnity);
            TrackingClickCancel = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, UserInstallStepType.ClickCancel, hasUnity);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowChart(bool hasUnity)
        {
            InitializeInstallFlowData(hasUnity);

            return DrawInstallFlowChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowLineChart(bool hasUnity)
        {
            InitializeInstallFlowData(hasUnity);

            return DrawInstallFlowLineChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        protected void InitializeTutorialData()
        {
            TutorialSteps = AdminCache.LoadTutorialStepsCount(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawTutorialChart()
        {
            InitializeTutorialData();

            return DrawTutorialChart(TutorialSteps);
        }

        public OpenFlashChart.OpenFlashChart DrawTutorialLineChart()
        {
            InitializeTutorialData();

            return DrawTutorialLineChart(TutorialSteps);
        }

        protected void InitializeFriendsCountData(ChannelType channel)
        {
            AverageFriendsCount = AdminCache.LoadAverageFriendsCount(FromDate, ToDate, channel);
            MedianFriendsCount = AdminCache.LoadMedianFriendsCount(FromDate, ToDate, channel);
        }

        public OpenFlashChart.OpenFlashChart DrawFriendsCountChart(ChannelType channel)
        {
            InitializeFriendsCountData(channel);

            string averageCountColor = "#f16406";
            string medianCountColor = "#0f19e1";

            List<OpenFlashChart.LineDotValue> averageValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> medianValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in AverageFriendsCount)
            {
                averageValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("Average friends: {0} ({1})", kvp.Value, kvp.Key.ToChartTooltip()), averageCountColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area averageLineChart = ChartUtil.CreateDefaultAreaChart(averageCountColor);
            averageLineChart.Values = averageValues;

            foreach (KeyValuePair<DateTime, int> kvp in MedianFriendsCount)
            {
                medianValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("Median friends: {0} ({1})", kvp.Value, kvp.Key.ToChartTooltip()), medianCountColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area medianLineChart = ChartUtil.CreateDefaultAreaChart(averageCountColor);
            medianLineChart.Values = medianValues;

            OpenFlashChart.OpenFlashChart friendsChart = ChartUtil.CreateDefaultChart(String.Format("Friends on {0}", channel), max);
            friendsChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            friendsChart.AddElement(averageLineChart);
            friendsChart.AddElement(medianLineChart);

            friendsChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            int ySteps = 1;

            if (max > 15)
            {
                ySteps = 5;
            }
            else if (max > 5)
            {
                ySteps = 3;
            }

            friendsChart.Y_Axis.Steps = ySteps;

            return friendsChart;
        }

        protected void InitializeActiveFriendsCountData(ChannelType channel)
        {
            AverageActiveFriendsCount = AdminCache.LoadAverageActiveFriendsCount(FromDate, ToDate, channel);
            MedianActiveFriendsCount = AdminCache.LoadMedianActiveFriendsCount(FromDate, ToDate, channel);
        }

        public OpenFlashChart.OpenFlashChart DrawActiveFriendsCountChart(ChannelType channel)
        {
            InitializeActiveFriendsCountData(channel);

            string averageCountColor = "#f16406";
            string medianCountColor = "#0f19e1";

            List<OpenFlashChart.LineDotValue> averageValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> medianValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in AverageActiveFriendsCount)
            {
                averageValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("Average active friends: {0} ({1})", kvp.Value, kvp.Key.ToChartTooltip()), averageCountColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area averageLineChart = ChartUtil.CreateDefaultAreaChart(averageCountColor);
            averageLineChart.Values = averageValues;

            foreach (KeyValuePair<DateTime, int> kvp in MedianActiveFriendsCount)
            {
                medianValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("Median active friends: {0} ({1})", kvp.Value, kvp.Key.ToChartTooltip()), medianCountColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area medianLineChart = ChartUtil.CreateDefaultAreaChart(averageCountColor);
            medianLineChart.Values = medianValues;

            OpenFlashChart.OpenFlashChart friendsChart = ChartUtil.CreateDefaultChart(String.Format("Active Friends on {0}", channel), max);
            friendsChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            friendsChart.AddElement(averageLineChart);
            friendsChart.AddElement(medianLineChart);

            friendsChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            int ySteps = 1;

            if (max > 15)
            {
                ySteps = 5;
            }
            else if (max > 5)
            {
                ySteps = 3;
            }

            friendsChart.Y_Axis.Steps = ySteps;

            return friendsChart;
        }

        protected void InitMapUsageChart(int mapIdOne, int mapIdTwo, int mapIdThree, GameModeType gameModeId)
        {
            MapOneUsage = new Dictionary<DateTime, int>();
            MapTwoUsage = new Dictionary<DateTime, int>();
            MapThreeUsage = new Dictionary<DateTime, int>();

            if (mapIdOne >= 0)
            {
                MapOneUsage = AdminCache.LoadMapUsage(FromDate, ToDate, mapIdOne, gameModeId);
            }

            if (mapIdTwo >= 0)
            {
                MapTwoUsage = AdminCache.LoadMapUsage(FromDate, ToDate, mapIdTwo, gameModeId);
            }

            if (mapIdThree >= 0)
            {
                MapThreeUsage = AdminCache.LoadMapUsage(FromDate, ToDate, mapIdThree, gameModeId);
            }

            MapNames = AdminCache.LoadMapNames();
        }

        public OpenFlashChart.OpenFlashChart DrawMapUsageChart(int mapIdOne, int mapIdTwo, int mapIdThree, GameModeType gameModeId)
        {
            InitMapUsageChart(mapIdOne, mapIdTwo, mapIdThree, gameModeId);

            string dotColorMapOne = "#F06F30";
            string dotColorMapTwo = "#63BE6C";
            string dotColorMapThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineMapOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineMapTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineMapThreeValues = new List<OpenFlashChart.LineDotValue>();

            string mapOneName = "None";
            string mapTwoName = "None";
            string mapThreeName = "None";
            MapNames.TryGetValue(mapIdOne, out mapOneName);
            MapNames.TryGetValue(mapIdTwo, out mapTwoName);
            MapNames.TryGetValue(mapIdThree, out mapThreeName);

            decimal max = 0;

            // Map One

            foreach (KeyValuePair<DateTime, int> kvp in MapOneUsage)
            {
                lineMapOneValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} players total ({2})", mapOneName, kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorMapOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartMapOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorMapOne);
            areaChartMapOne.Values = lineMapOneValues;

            // Region Two

            foreach (KeyValuePair<DateTime, int> kvp in MapTwoUsage)
            {
                lineMapTwoValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} players total ({2})", mapTwoName, kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorMapTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartMapTwo = ChartUtil.CreateDefaultAreaChart(dotColorMapTwo);
            areaChartMapTwo.Values = lineMapTwoValues;

            // Region Three

            foreach (KeyValuePair<DateTime, int> kvp in MapThreeUsage)
            {
                lineMapThreeValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} players total ({2})", mapThreeName, kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorMapThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartMapThree = ChartUtil.CreateDefaultAreaChart(dotColorMapThree);
            areaChartMapThree.Values = lineMapThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart mapUsageChart = ChartUtil.CreateDefaultChart("Map Usage", (double) max);

            mapUsageChart.Y_Axis.Steps = 1;

            mapUsageChart.AddElement(ChartUtil.CreateMilestoneChart((int)max, ApplicationMilestones));

            if (mapIdOne > 0)
            {
                mapUsageChart.AddElement(areaChartMapOne);
            }

            if (mapIdTwo > 0)
            {
                mapUsageChart.AddElement(areaChartMapTwo);
            }

            if (mapIdThree > 0)
            {
                mapUsageChart.AddElement(areaChartMapThree);
            }

            mapUsageChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return mapUsageChart;
        }

        #endregion
    }
}