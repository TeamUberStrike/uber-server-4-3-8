using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Utils;
using UberStrike.DataCenter.Common.Entities;
using OpenFlashChart;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class ChannelChart : StatisticChart
    {
        #region Properties

        public ChannelType Channel { get; private set; }

        public Dictionary<DateTime, int> Dau { get; private set; }
        public Dictionary<DateTime, int> Mau { get; private set; }
        public Dictionary<DateTime, float> DailyPlayRate { get; private set; }
        public Dictionary<DateTime, int> TrackingInitialState { get; private set; }
        public Dictionary<DateTime, int> TrackingClickDownload { get; private set; }
        public Dictionary<DateTime, int> TrackingUnityInstalled { get; private set; }
        public Dictionary<DateTime, int> TrackingUnityInitialized { get; private set; }
        public Dictionary<DateTime, int> TrackingFullGameLoaded { get; private set; }
        public Dictionary<DateTime, int> TrackingAccountCreated { get; private set; }
        public Dictionary<DateTime, int> TrackingClickCancel { get; private set; }
        public Dictionary<DateTime, decimal> DailyRevenue { get; private set; }
        public Dictionary<DateTime, DecimalPair> Darpu { get; private set; }
        public Dictionary<DateTime, IntPair> DailyTransactions { get; private set; }
        public Dictionary<DateTime, FloatPair> DailyConversionToPaying { get; private set; }
        public Dictionary<DateTime, DecimalPair> Darppu { get; private set; }
        public Dictionary<DateTime, IntPair> NewVersusReturning { get; private set; }
        public Dictionary<int, decimal> PackagesContributionbyRevenue { get; private set; }
        public Dictionary<int, int> PackagesContributionByVolume { get; private set; }
        public Dictionary<DateTime, Dictionary<int, int>> CreditsSales { get; private set; }
        public Dictionary<DateTime, Dictionary<int, int>> BundlesSales { get; private set; }

        #endregion

        #region Constructors

        public ChannelChart(DateTime FromDate, DateTime ToDate, ChannelType channel)
                : base(FromDate, ToDate)
        {
            Channel = channel;
        }

        #endregion

        #region Methods

        protected void InitDauChart()
        {
            Dau = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart()
        {
            InitDauChart();
            string dotColor = "#0078a4";

            // Setup the Line Point Values
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in Dau)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("DAU {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            // Create the Line Chart
            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            areaChart.Values = lineDotValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart dauChart = ChartUtil.CreateDefaultChart("Daily Active Users", max);
            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dauChart.AddElement(areaChart);
            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        void InitMauChart()
        {
            Mau = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawMauChart()
        {
            InitMauChart();
            string dotColor = "#0078a4";

            // Setup the Line Point Values
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in Mau)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("MAU {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            // Create the Line Chart
            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            areaChart.Values = lineDotValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart mauChart = ChartUtil.CreateDefaultChart("Monthy Active Users", max);
            mauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            mauChart.AddElement(areaChart);
            mauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return mauChart;
        }

        protected void InitNewVersusReturningChart()
        {
            NewVersusReturning = AdminCache.LoadNewVsReturningUsers(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawNewVersusReturningChart()
        {
            InitNewVersusReturningChart();

            return DrawNewVersusReturningChart(NewVersusReturning);
        }

        void InitDailyPlayRateChart()
        {
            DailyPlayRate = AdminCache.LoadDailyPlayRate(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyPlayRateChart()
        {
            InitDailyPlayRateChart();

            return DrawDailyPlayRateChart(DailyPlayRate);
        }

        void InitInstallTrackingChart(bool hasUnity)
        {
            if (hasUnity)
            {
                TrackingInitialState = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.HasUnity, hasUnity);
            }
            else
            {
                TrackingInitialState = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.NoUnity, hasUnity);
            }

            TrackingClickDownload = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.ClickDownload, hasUnity);
            TrackingUnityInstalled = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.UnityInstalled, hasUnity);
            TrackingUnityInitialized = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.UnityInitialized, hasUnity);
            TrackingFullGameLoaded = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.FullGameLoaded, hasUnity);
            TrackingAccountCreated = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.AccountCreated, hasUnity);
            TrackingClickCancel = AdminCache.LoadInstallTrackingTotals(FromDate, ToDate, Channel, UserInstallStepType.ClickCancel, hasUnity);
        }

        /// <summary>
        /// Installation Convertion Funnel
        /// </summary>
        public OpenFlashChart.OpenFlashChart DrawInstallTrackingChart(bool hasUnity)
        {
            InitInstallTrackingChart(hasUnity);

            return DrawInstallFlowChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        /// <summary>
        /// Installation Conversion Funnel - Step Ratio
        /// </summary>
        public OpenFlashChart.OpenFlashChart DrawInstallTrackingLineChart(bool hasUnity)
        {
            InitInstallTrackingChart(hasUnity);

            return DrawInstallFlowLineChart(TrackingInitialState, TrackingClickDownload, TrackingUnityInstalled, TrackingUnityInitialized, TrackingFullGameLoaded, TrackingAccountCreated, TrackingClickCancel, hasUnity);
        }

        void InitRevenueChart()
        {
            DailyRevenue = AdminCache.LoadDailyRevenue(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawRevenueChart()
        {
            InitRevenueChart();
            string dotColor = "#7bc90b";

            List<OpenFlashChart.LineDotValue> lineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenue)
            {
                lineChartValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value, String.Format("US${0} ({1})", kvp.Value.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area dailyRevLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = lineChartValues;

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Daily Revenue", (double) max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            chartObject.AddElement(dailyRevLineChart);
            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = (int)((max * 1.1m) / 10);

            return chartObject;
        }

        void InitDarpuChart()
        {
            Darpu = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDarpuChart()
        {
            InitDarpuChart();

            return DrawDarpuChart(Darpu);
        }

        void InitDailyTransactionsChart()
        {
            DailyTransactions = AdminCache.LoadDailyTransactions(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyTransactionsChart()
        {
            InitDailyTransactionsChart();

            return DrawDailyTransactionsChart(DailyTransactions);
        }

        void InitDailyConversionToPayingChart()
        {
            DailyConversionToPaying = AdminCache.LoadDailyConversionToPaying(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyConversionToPayingChart()
        {
            InitDailyConversionToPayingChart();

            return DrawDailyConversionToPayingChart(DailyConversionToPaying);
        }

        void InitDarppuChart()
        {
            Darppu = AdminCache.LoadDailyAverageRevenuePerPayingUser(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawDarppuChart()
        {
            InitDarppuChart();
            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> darppuLineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in Darppu)
            {
                darppuLineChartValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value.Decimal2, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N2"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value.Decimal2 > max) max = kvp.Value.Decimal2;
            }

            OpenFlashChart.Area darppuLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            darppuLineChart.Values = darppuLineChartValues;

            OpenFlashChart.OpenFlashChart darppuChart = ChartUtil.CreateDefaultChart("DARPPU (Daily Average Revenue Per Paying User)", (double) max);
            darppuChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            darppuChart.AddElement(darppuLineChart);
            darppuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            darppuChart.Y_Axis.Steps = 10;
            darppuChart.Y_Legend = new OpenFlashChart.Legend("US$");
            darppuChart.Y_Legend.Style = "{color:#555555; font-family:Verdana,Arial,Helvetica,sans-serif; font-size:12px; font-weight:bold; font background-color: #FFFFFF}";
            return darppuChart;
        }

        void InitPackagesContributionByRevenueChart(bool areCreditsBundles)
        {
            PackagesContributionbyRevenue = AdminCache.LoadPackageContributionByRevenue(FromDate, ToDate, Channel, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackagesContributionByRevenueChart(bool areCreditsBundles)
        {
            InitPackagesContributionByRevenueChart(areCreditsBundles);

            return DrawPackageContributionByRevenueChart(PackagesContributionbyRevenue, areCreditsBundles);
        }

        void InitPackagesContributionByVolumeChart(bool areCreditsBundles)
        {
            PackagesContributionByVolume = AdminCache.LoadPackageContributionByVolume(FromDate, ToDate, Channel, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackagesContributionByVolumeChart(bool areCreditsBundles)
        {
            InitPackagesContributionByVolumeChart(areCreditsBundles);

            return DrawPackageContributionByVolumeChart(PackagesContributionByVolume, areCreditsBundles);
        }

        protected void InitDrawCreditsSalesChart()
        {
            CreditsSales = AdminCache.LoadCreditSales(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawCreditsSalesChart()
        {
            InitDrawCreditsSalesChart();

            return DrawBundlesSalesChart(CreditsSales, true);
        }

        protected void InitDrawBundlesSalesChart()
        {
            BundlesSales = AdminCache.LoadBundlesSales(FromDate, ToDate, Channel);
        }

        public OpenFlashChart.OpenFlashChart DrawBundlesSalesChart()
        {
            InitDrawBundlesSalesChart();

            return DrawBundlesSalesChart(BundlesSales, false);
        }

        #endregion
    }
}