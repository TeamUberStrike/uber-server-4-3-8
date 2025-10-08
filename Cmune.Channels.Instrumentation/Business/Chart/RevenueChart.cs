using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.Channels.Instrumentation.Business.Chart;
using Cmune.Channels.Instrumentation.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;
using OpenFlashChart;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class RevenueChart : StatisticChart
    {
        #region Properties

        protected Dictionary<DateTime, DecimalPair> DailyAverageRevenuePerUser;
        protected Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> DailyAverageRevenuePerUserByChannels;

        protected Dictionary<DateTime, DecimalPair> DailyAverageRevenuePerPayingUser;
        protected Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> DailyAverageRevenuePerPayingUserByChannels;

        protected Dictionary<DateTime, decimal> DailyRevenue;
        protected Dictionary<DateTime, IntPair> DailyTransactions;
        protected Dictionary<DateTime, FloatPair> DailyConversionToPaying;
        protected Dictionary<int, decimal> PackageContribution;
        protected Dictionary<int, int> PackageContributionByVolume;
        protected Dictionary<PaymentProviderType, decimal> PaymentProviderContribution;
        public Dictionary<ChannelType, decimal> ChannelContribution { get; private set; }

        protected Dictionary<ChannelType, Dictionary<DateTime, decimal>> DailyRevenueByChannels;

        public Dictionary<DateTime, Dictionary<int, int>> CreditsSales { get; private set; }
        public Dictionary<DateTime, Dictionary<int, int>> BundlesSales { get; private set; }

        #endregion

        #region Constructors

        public RevenueChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitDailyAverageRevenuePerPayingUserChart()
        {
            DailyAverageRevenuePerPayingUser = AdminCache.LoadDailyAverageRevenuePerPayingUser(FromDate, ToDate);
            DailyAverageRevenuePerPayingUserByChannels = AdminCache.LoadDailyAverageRevenuePerPayingUserByChannels(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyAverageRevenuePerPayingUserChart()
        {
            InitDailyAverageRevenuePerPayingUserChart();

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<OpenFlashChart.LineDotValue> darppuTotalLineChartValues = new List<OpenFlashChart.LineDotValue>();

            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in DailyAverageRevenuePerPayingUserByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            decimal max = 0;

            // Total

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DailyAverageRevenuePerPayingUser)
            {
                darppuTotalLineChartValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value.Decimal2, string.Format("All Channels: US${0} Total: US${1} ({2})", kvp.Value.Decimal2.ToString("N2"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value.Decimal2 > max) max = kvp.Value.Decimal2;
            }

            OpenFlashChart.Area darppuTotalLineChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            darppuTotalLineChart.Values = darppuTotalLineChartValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, DecimalPair> kvp in DailyAverageRevenuePerPayingUserByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue((double)kvp.Value.Decimal2, string.Format("{0}: US${1} Total US${2} ({3})", channel, kvp.Value.Decimal2.ToString("N2"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                    if (kvp.Value.Decimal2 > max) max = kvp.Value.Decimal2;
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart darppuChart = ChartUtil.CreateDefaultChart("DARPPU (Daily Average Revenue Per Paying User)", (double)max);
            darppuChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));

            darppuChart.AddElement(darppuTotalLineChart);

            foreach (ChannelType channel in values.Keys)
            {
                darppuChart.AddElement(charts[channel]);
            }

            darppuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            darppuChart.Y_Axis.Steps = 5;
            darppuChart.Y_Legend = new OpenFlashChart.Legend("US$");
            darppuChart.Y_Legend.Style = "{color:#555555; font-family:Verdana,Arial,Helvetica,sans-serif; font-size:12px; font-weight:bold; font background-color: #FFFFFF}";

           return darppuChart;
        }

        protected void InitDailyAverageRevenuePerUserChart()
        {
            DailyAverageRevenuePerUser = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate);
            DailyAverageRevenuePerUserByChannels = AdminCache.LoadDailyAverageRevenuePerUserByChannels(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyAverageRevenuePerUserChart()
        {
            InitDailyAverageRevenuePerUserChart();

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<LineDotValue> darpuTotalLineChartValues = new List<LineDotValue>();
            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in DailyAverageRevenuePerUserByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            decimal max = 0;

            // Total

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DailyAverageRevenuePerUser)
            {
                decimal darpu = kvp.Value.Decimal2 * 100;
                darpuTotalLineChartValues.Add(new OpenFlashChart.LineDotValue((double) darpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (darpu > max) max = darpu;
            }

            OpenFlashChart.Area darpuTotalLineChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            darpuTotalLineChart.Values = darpuTotalLineChartValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, DecimalPair> kvp in DailyAverageRevenuePerUserByChannels[channel])
                {
                    decimal darpu = kvp.Value.Decimal2 * 100;
                    values[channel].Add(new OpenFlashChart.LineDotValue((double)darpu, string.Format("{0}: US${1} Total US${2} ({3})", channel, kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                    if (darpu > max) max = darpu;
                }

                charts[channel].Values = values[channel];
            }
            
            OpenFlashChart.OpenFlashChart darpuChart = ChartUtil.CreateDefaultChart("DARPU (Daily Average Revenue Per User)", (double)max);
            darpuChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));

            darpuChart.AddElement(darpuTotalLineChart);

            foreach (ChannelType channel in values.Keys)
            {
                darpuChart.AddElement(charts[channel]);
            }

            darpuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            darpuChart.Y_Axis.Steps = 2;

            return darpuChart;
        }

        protected void InitPaymentPartnerContributionChart()
        {
            PaymentProviderContribution = AdminCache.LoadPaymentProviderContribution(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawPaymentPoviderContributionChart()
        {
            InitPaymentPartnerContributionChart();

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<PaymentProviderType, decimal> kvp in PaymentProviderContribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double) kvp.Value, CommonConfig.PaymentProviderName[kvp.Key]));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "US$#val# of US$#total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title("Payment Provider Distribution");
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected void InitChannelContributionChart()
        {
            ChannelContribution = AdminCache.LoadChannelContribution(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawChannelContributionChart()
        {
            InitChannelContributionChart();

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<ChannelType, decimal> kvp in ChannelContribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, kvp.Key.ToString()));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "US$#val# of US$#total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title("Channel Distribution");
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected void InitPackageContributionByRevenueChart(bool areCreditsBundles)
        {
            PackageContribution = AdminCache.LoadPackageContributionByRevenue(FromDate, ToDate, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackageContributionByRevenueChart(bool areCreditsBundles)
        {
            InitPackageContributionByRevenueChart(areCreditsBundles);

            return DrawPackageContributionByRevenueChart(PackageContribution, areCreditsBundles);
        }

        protected void InitPackageContributionByVolumeChart(bool areCreditsBundles)
        {
            PackageContributionByVolume = AdminCache.LoadPackageContributionByVolume(FromDate, ToDate, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackageContributionByVolumeChart(bool areCreditsBundles)
        {
            InitPackageContributionByVolumeChart(areCreditsBundles);

            return DrawPackageContributionByVolumeChart(PackageContributionByVolume, areCreditsBundles);
        }

        protected void InitDailyRevenueByChannel()
        {
            DailyRevenueByChannels = AdminCache.LoadDailyRevenueByChannels(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueByChannel()
        {
            InitDailyRevenueByChannel();

            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in DailyRevenueByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            decimal max = 0;

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: US${1} ({2})", channel, kvp.Value.ToString("N2"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                    if (kvp.Value > max) max = kvp.Value;
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Daily Revenue By Channel", (double)max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));

            foreach (ChannelType channel in values.Keys)
            {
                chartObject.AddElement(charts[channel]);
            }

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = 0;

            return chartObject;
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueByPaymentProvider()
        {
            Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>> dailyRevenue = AdminCache.LoadDailyRevenueByPaymentProviders(FromDate, ToDate);

            Dictionary<PaymentProviderType, List<LineDotValue>> values = new Dictionary<PaymentProviderType, List<LineDotValue>>();
            Dictionary<PaymentProviderType, Area> charts = new Dictionary<PaymentProviderType, Area>();

            foreach (PaymentProviderType paymentProvider in dailyRevenue.Keys)
            {
                values.Add(paymentProvider, new List<OpenFlashChart.LineDotValue>());
                charts.Add(paymentProvider, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetPaymentProviderColor(paymentProvider)));
            }

            decimal max = 0;

            foreach (PaymentProviderType paymentProvider in values.Keys)
            {
                foreach (KeyValuePair<DateTime, decimal> kvp in dailyRevenue[paymentProvider])
                {
                    values[paymentProvider].Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: US${1} ({2})", paymentProvider, kvp.Value.ToString("N2"), kvp.Key.ToChartTooltip()), AdminConfig.GetPaymentProviderColor(paymentProvider)));
                    if (kvp.Value > max) max = kvp.Value;
                }

                charts[paymentProvider].Values = values[paymentProvider];
            }

            OpenFlashChart.OpenFlashChart paymentProviderChart = ChartUtil.CreateDefaultChart("Daily Revenue By Payment Provider", (double)max);
            paymentProviderChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));

            foreach (PaymentProviderType paymentProvider in values.Keys)
            {
                paymentProviderChart.AddElement(charts[paymentProvider]);
            }

            paymentProviderChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            paymentProviderChart.Y_Axis.Steps = 0;

            return paymentProviderChart;
        }

        protected void InitDailyRevenueChart()
        {
            DailyRevenue = AdminCache.LoadDailyRevenue(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueChart()
        {
            InitDailyRevenueChart();
            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> dailyRevLineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenue)
            {
                dailyRevLineChartValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value, String.Format("US${0} ({1})", kvp.Value.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area dailyRevLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = dailyRevLineChartValues;

            OpenFlashChart.OpenFlashChart dailyRevChart = ChartUtil.CreateDefaultChart("Daily Revenue", (double)max);
            dailyRevChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            dailyRevChart.AddElement(dailyRevLineChart);
            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dailyRevChart.Y_Axis.Steps = 0;

            return dailyRevChart;
        }

        protected void InitDailyTransactionsChart()
        {
            DailyTransactions = AdminCache.LoadDailyTransactions(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyTransactionsChart()
        {
            InitDailyTransactionsChart();

            return DrawDailyTransactionsChart(DailyTransactions);
        }

        protected void InitDailyConversionToPayingChart()
        {
            DailyConversionToPaying = AdminCache.LoadDailyConversionToPaying(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyConversionToPayingChart()
        {
            InitDailyConversionToPayingChart();

            return DrawDailyConversionToPayingChart(DailyConversionToPaying);
        }

        protected void InitDrawCreditsSalesChart()
        {
            CreditsSales = AdminCache.LoadCreditSales(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawCreditsSalesChart()
        {
            InitDrawCreditsSalesChart();

            return DrawBundlesSalesChart(CreditsSales, true);
        }

        protected void InitDrawBundlesSalesChart()
        {
            BundlesSales = AdminCache.LoadBundlesSales(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawBundlesSalesChart()
        {
            InitDrawBundlesSalesChart();

            return DrawBundlesSalesChart(BundlesSales, false);
        }

        #endregion
    }
}