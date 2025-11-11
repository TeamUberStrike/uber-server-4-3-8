using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class ItemEconomyChart : StatisticChart
    {
        #region Properties
        
        protected Dictionary<String, IntPair> ItemWeaponCreditContribution;
        protected Dictionary<String, IntPair> ItemGearCreditContribution;
        protected Dictionary<String, int> ItemPointContribution;

        public Dictionary<DateTime, int> ItemOneCreditsContribution { get; private set; }
        public Dictionary<DateTime, int> ItemTwoCreditsContribution { get; private set; }
        public Dictionary<DateTime, int> ItemThreCreditsContribution { get; private set; }
        public ItemCache ItemCache { get; private set; }

        public Dictionary<DateTime, int> AveragePointsBalance { get; private set; }
        public Dictionary<DateTime, int> AveragePointsEarned { get; private set; }
        public Dictionary<DateTime, int> AveragePointsSpent { get; private set; }
        public Dictionary<DateTime, int> MedianPointsSpent { get; private set; }
        public Dictionary<DateTime, int> AverageCreditsBalance { get; private set; }
        public Dictionary<DateTime, int> AverageCreditsSpent { get; private set; }
        public Dictionary<DateTime, int> MedianCreditsSpent { get; private set; }
        public Dictionary<PointsDepositType, long> PointDepositTypesDistribution { get; private set; }

        #endregion

        #region Constructors

        public ItemEconomyChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitItemPointContributionChart()
        {
            ItemPointContribution = AdminCache.LoadItemPointContribution(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawItemPointContributionChart()
        {
            InitItemPointContributionChart();
            string dotColor = "#ffbd16";
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();

            float max = 0;
            foreach (KeyValuePair<String, int> kvp in ItemPointContribution)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("{0} {1} Points", kvp.Key, kvp.Value.ToString("N0")), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area dailyRevLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = lineDotValues;

            OpenFlashChart.OpenFlashChart objectChart = ChartUtil.CreateDefaultChart("Item Points Contribution", max);
            objectChart.AddElement(dailyRevLineChart);
            objectChart.X_Axis.SetLabels(ItemPointContribution.Keys.ToList());
            objectChart.Y_Axis.Labels.Steps = 1;
            objectChart.X_Axis.Labels.Steps = 1;
            objectChart.X_Axis.Labels.Rotate = "-45";

            return objectChart;
        }

        protected void InitItemCreditsContributionChart(int itemIdOne, int itemIdTwo, int itemIdThree)
        {
            ItemOneCreditsContribution = new Dictionary<DateTime, int>();
            ItemTwoCreditsContribution = new Dictionary<DateTime, int>();
            ItemThreCreditsContribution = new Dictionary<DateTime, int>();

            if (itemIdOne >= 0)
            {
                ItemOneCreditsContribution = AdminCache.LoadCreditItemSale(FromDate, ToDate, itemIdOne);
            }

            if (itemIdTwo >= 0)
            {
                ItemTwoCreditsContribution = AdminCache.LoadCreditItemSale(FromDate, ToDate, itemIdTwo);
            }

            if (itemIdThree >= 0)
            {
                ItemThreCreditsContribution = AdminCache.LoadCreditItemSale(FromDate, ToDate, itemIdThree);
            }

            ItemCache = new ItemCache(CommonConfig.ApplicationIdUberstrike);
        }

        public OpenFlashChart.OpenFlashChart DrawItemCreditsContributionChart(int itemIdOne, int itemIdTwo, int itemIdThree)
        {
            InitItemCreditsContributionChart(itemIdOne, itemIdTwo, itemIdThree);

            string dotColorItemOne = "#F06F30";
            string dotColorItemTwo = "#63BE6C";
            string dotColorItemThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineItemOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineItemTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineItemThreeValues = new List<OpenFlashChart.LineDotValue>();

            string itemOneName = ItemCache.GetItemName(itemIdOne);
            string itemTwoName = ItemCache.GetItemName(itemIdTwo);
            string itemThreeName = ItemCache.GetItemName(itemIdThree);

            decimal max = 0;

            // Region One

            foreach (KeyValuePair<DateTime, int> kvp in ItemOneCreditsContribution)
            {
                lineItemOneValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} - {2} ({3})", itemOneName, kvp.Value.ToString(), ((decimal) kvp.Value / (decimal) CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd]).ToString("C2"), kvp.Key.ToChartTooltip()), dotColorItemOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartItemOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorItemOne);
            areaChartItemOne.Values = lineItemOneValues;

            // Region Two

            foreach (KeyValuePair<DateTime, int> kvp in ItemTwoCreditsContribution)
            {
                lineItemTwoValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} - {2} ({3})", itemTwoName, kvp.Value.ToString(), ((decimal)kvp.Value / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd]).ToString("C2"), kvp.Key.ToChartTooltip()), dotColorItemTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartItemTwo = ChartUtil.CreateDefaultAreaChart(dotColorItemTwo);
            areaChartItemTwo.Values = lineItemTwoValues;

            // Region Three

            foreach (KeyValuePair<DateTime, int> kvp in ItemThreCreditsContribution)
            {
                lineItemThreeValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} - {2} ({3})", itemThreeName, kvp.Value.ToString(), ((decimal)kvp.Value / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd]).ToString("C2"), kvp.Key.ToChartTooltip()), dotColorItemThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartItemThree = ChartUtil.CreateDefaultAreaChart(dotColorItemThree);
            areaChartItemThree.Values = lineItemThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart itemCreditsContributionChart = ChartUtil.CreateDefaultChart("Item credits contribution", (double)max);

            itemCreditsContributionChart.Y_Axis.Steps = 1;

            itemCreditsContributionChart.AddElement(ChartUtil.CreateMilestoneChart((int)max, ApplicationMilestones));

            if (itemIdOne > 0)
            {
                itemCreditsContributionChart.AddElement(areaChartItemOne);
            }

            if (itemIdTwo > 0)
            {
                itemCreditsContributionChart.AddElement(areaChartItemTwo);
            }

            if (itemIdThree > 0)
            {
                itemCreditsContributionChart.AddElement(areaChartItemThree);
            }

            itemCreditsContributionChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return itemCreditsContributionChart;
        }

        protected void InitAveragePointsBalanceChart()
        {
            AveragePointsBalance = AdminCache.LoadAveragePointBalance(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawAveragePointsBalanceChart()
        {
            InitAveragePointsBalanceChart();

            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> pointsBalanceChartValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in AveragePointsBalance)
            {
                pointsBalanceChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("{0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area dailyRevLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = pointsBalanceChartValues;

            max = (int) (max * 1.05m);

            OpenFlashChart.OpenFlashChart dailyRevChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Average Points Balance", max);
            dailyRevChart.AddElement(Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            dailyRevChart.AddElement(dailyRevLineChart);
            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dailyRevChart.Y_Axis.Steps = 0;

            return dailyRevChart;
        }

        protected void InitPointsMovementChart()
        {
            AveragePointsEarned = AdminCache.LoadAveragePointsEarned(FromDate, ToDate);
            AveragePointsSpent = AdminCache.LoadAveragePointsSpent(FromDate, ToDate);
            MedianPointsSpent = AdminCache.LoadMedianPointsSpent(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawPointsMovementChart()
        {
            InitPointsMovementChart();

            string averagePointsEarnedDotColor = "#7bc90b";
            string averagePointsSpentDotColor = "#1cccf2";
            string medianPointsSpentDotColor = "#efae13";

            List<OpenFlashChart.LineDotValue> averagePointsEarnedValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> averagePointsSpentValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> medianPointsSpentValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Average points earned

            foreach (KeyValuePair<DateTime, int> kvp in AveragePointsEarned)
            {
                averagePointsEarnedValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("Average points earned: {0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), averagePointsEarnedDotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area averagePointsEarnedLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(averagePointsEarnedDotColor);
            averagePointsEarnedLineChart.Values = averagePointsEarnedValues;

            // Average points spent

            foreach (KeyValuePair<DateTime, int> kvp in AveragePointsSpent)
            {
                averagePointsSpentValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("Average points spent: {0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), averagePointsSpentDotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area averagePointsSpentLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(averagePointsSpentDotColor);
            averagePointsSpentLineChart.Values = averagePointsSpentValues;

            // Median points spent

            foreach (KeyValuePair<DateTime, int> kvp in MedianPointsSpent)
            {
                medianPointsSpentValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("Median points spent: {0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), medianPointsSpentDotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area medianPointsSpentLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(medianPointsSpentDotColor);
            medianPointsSpentLineChart.Values = medianPointsSpentValues;

            OpenFlashChart.OpenFlashChart dailyRevChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Points Movements", max);
            dailyRevChart.AddElement(Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dailyRevChart.AddElement(averagePointsEarnedLineChart);
            dailyRevChart.AddElement(averagePointsSpentLineChart);
            dailyRevChart.AddElement(medianPointsSpentLineChart);

            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dailyRevChart.Y_Axis.Steps = 0;

            return dailyRevChart;
        }

        protected void InitAverageCreditsBalanceChart()
        {
            AverageCreditsBalance = AdminCache.LoadAverageCreditsBalance(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawAverageCreditsBalanceChart()
        {
            InitAverageCreditsBalanceChart();

            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> creditsBalanceChartValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in AverageCreditsBalance)
            {
                creditsBalanceChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("{0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area creditsBalanceLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor);
            creditsBalanceLineChart.Values = creditsBalanceChartValues;

            OpenFlashChart.OpenFlashChart creditsBalanceChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Average Credits Balance", max);
            creditsBalanceChart.AddElement(Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            creditsBalanceChart.AddElement(creditsBalanceLineChart);

            creditsBalanceChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            creditsBalanceChart.Y_Axis.Steps = 0;

            return creditsBalanceChart;
        }

        protected void InitCreditsMovementChart()
        {
            AverageCreditsSpent = AdminCache.LoadAverageCreditsSpent(FromDate, ToDate);
            MedianCreditsSpent = AdminCache.LoadMedianCreditsSpent(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawCreditsMovementChart()
        {
            InitCreditsMovementChart();

            string averageCreditsSpentDotColor = "#1cccf2";
            string medianCreditsSpentDotColor = "#efae13";

            List<OpenFlashChart.LineDotValue> averageCreditsSpentValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> medianCreditsSpentValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Average credits spent

            foreach (KeyValuePair<DateTime, int> kvp in AverageCreditsSpent)
            {
                averageCreditsSpentValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("Average credits spent: {0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), averageCreditsSpentDotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area averageCreditsSpentLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(averageCreditsSpentDotColor);
            averageCreditsSpentLineChart.Values = averageCreditsSpentValues;

            // Median credits spent

            foreach (KeyValuePair<DateTime, int> kvp in MedianCreditsSpent)
            {
                medianCreditsSpentValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("Median credits spent: {0} ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), medianCreditsSpentDotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area medianCreditsSpentLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(medianCreditsSpentDotColor);
            medianCreditsSpentLineChart.Values = medianCreditsSpentValues;

            OpenFlashChart.OpenFlashChart creditsMovementChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Credits Movements", max);
            creditsMovementChart.AddElement(Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            creditsMovementChart.AddElement(averageCreditsSpentLineChart);
            creditsMovementChart.AddElement(medianCreditsSpentLineChart);

            creditsMovementChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            creditsMovementChart.Y_Axis.Steps = 0;

            return creditsMovementChart;
        }

        protected void InitPointDepositTypeDistributionChart()
        {
            PointDepositTypesDistribution = AdminCache.LoadPointDepositsDistribution(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawPointDepositTypeDistributionChart()
        {
            InitPointDepositTypeDistributionChart();

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<PointsDepositType, long> kvp in PointDepositTypesDistribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, String.Format("{0}", kvp.Key)));
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
            objectChart.Title = new OpenFlashChart.Title("Points Deposit Type Distribution");
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        #endregion
    }
}