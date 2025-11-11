using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.Channels.Instrumentation.Business.Chart;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class CoreChart : StatisticChart
    {
        #region Properties

        protected Dictionary<DateTime, int> DailyActiveUsers;
        protected Dictionary<DateTime, int> MonthlyActiveUsers;
        protected Dictionary<DateTime, float> DailyPlayRate;
        protected Dictionary<DateTime, IntPair> NewVsReturning;
        protected Dictionary<DateTime, DecimalPair> DailyAverageRevenuePerUser;
        protected Dictionary<DateTime, decimal> DailyRevenue;
        protected Dictionary<DateTime, IntPair> DailyTransactions;
        protected Dictionary<DateTime, FloatPair> DailyConversionToPaying;

        #endregion

        #region Constructors

        public CoreChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitDauChart()
        {
            DailyActiveUsers = AdminCache.LoadDailyActiveUsers(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart()
        {
            InitDauChart();
            string dotColor = "#0078a4";

            // Setup the Line Point Values
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();
            int max = 0;
            foreach (KeyValuePair<DateTime, int> kvp in DailyActiveUsers)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
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

        protected void InitMauChart()
        {
            MonthlyActiveUsers = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawMauChart()
        {
            InitMauChart();
            string dotColor = "#0078a4";
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;
            foreach (KeyValuePair<DateTime, int> kvp in MonthlyActiveUsers)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            areaChart.Values = lineDotValues;

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Monthly Active Users", max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            chartObject.AddElement(areaChart);
            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return chartObject;
        }

        protected void InitNewVsReturningChart()
        {
            NewVsReturning = AdminCache.LoadNewVsReturningUsers(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawNewVsReturningChart()
        {
            InitNewVsReturningChart();
            string dotColor = "#0078a4";
            string dotColor2 = "#ffbd16";
            OpenFlashChart.Area nvsrLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            OpenFlashChart.Area nvsrLineChart2 = ChartUtil.CreateDefaultAreaChart(dotColor2);
            List<OpenFlashChart.LineDotValue> nvsrLineChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> nvsrLineChartValues2 = new List<OpenFlashChart.LineDotValue>();

            int max = 0;
            foreach (KeyValuePair<DateTime, IntPair> kvp in NewVsReturning)
            {
                float newPercent = ((kvp.Value.Int2 > 0) ? ((float)kvp.Value.Int1 / (float)kvp.Value.Int2) : 0.0f);
                float retPercent = (kvp.Value.Int2 > 0) ? (1 - ((float)kvp.Value.Int1 / (float)kvp.Value.Int2)) : 0.0f;
                nvsrLineChartValues.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int1, string.Format("{0} New Users ({1}) {2}", kvp.Value.Int1.ToString("N0"), newPercent.ToString("P2"), kvp.Key.ToChartTooltip()), dotColor));
                nvsrLineChartValues2.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int2, string.Format("{0} Returning Users ({1}) {2}", kvp.Value.Int2.ToString("N0"), retPercent.ToString("P2"), kvp.Key.ToChartTooltip()), dotColor2));
                if (kvp.Value.Int2 > max) max = kvp.Value.Int2;
            }

            nvsrLineChart.Values = nvsrLineChartValues;
            nvsrLineChart2.Values = nvsrLineChartValues2;

            OpenFlashChart.OpenFlashChart nvsrChart = ChartUtil.CreateDefaultChart("New vs Returning Users", max);
            nvsrChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            nvsrChart.AddElement(nvsrLineChart2);
            nvsrChart.AddElement(nvsrLineChart);
            nvsrChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return nvsrChart;
        }

        protected void InitPlayRateChartChart()
        {
            DailyPlayRate = AdminCache.LoadDailyPlayRate(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawPlayRateChart()
        {
            InitPlayRateChartChart();
            string dotColor = "#f16406";
            List<OpenFlashChart.LineDotValue> drLineChartValues = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            foreach (KeyValuePair<DateTime, float> kvp in DailyPlayRate)
            {
                float playRate = kvp.Value * 100;
                drLineChartValues.Add(new OpenFlashChart.LineDotValue(playRate, string.Format("{0} ({1})", kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColor));
                if (playRate > max) max = playRate;
            }

            OpenFlashChart.Area drLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            drLineChart.Values = drLineChartValues;

            OpenFlashChart.OpenFlashChart drChart = ChartUtil.CreateDefaultChart("Play Rate", max);
            drChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            drChart.AddElement(drLineChart);
            drChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            drChart.Y_Axis.Steps = 20;

            return drChart;
        }

        protected void InitDailyAverageRevenuePerUserChart()
        {
            DailyAverageRevenuePerUser = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyAverageRevenuePerUserChart()
        {
            InitDailyAverageRevenuePerUserChart();
            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> darpuLineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DailyAverageRevenuePerUser)
            {
                decimal darpu = kvp.Value.Decimal2 * 100;
                darpuLineChartValues.Add(new OpenFlashChart.LineDotValue((double) darpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (darpu > max) max = darpu;
            }

            OpenFlashChart.Area darpuLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            darpuLineChart.Values = darpuLineChartValues;

            OpenFlashChart.OpenFlashChart darpuChart = ChartUtil.CreateDefaultChart("DARPU (Daily Average Revenue Per User)", (double)max);
            darpuChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            darpuChart.AddElement(darpuLineChart);
            darpuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return darpuChart;
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
            string dotColor = "#0078a4";
            string dotColor2 = "#ffbd16";
            OpenFlashChart.Area dailyConvLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            OpenFlashChart.Area dailyConvLineChart2 = ChartUtil.CreateDefaultAreaChart(dotColor2);
            List<OpenFlashChart.LineDotValue> dailyConvLineChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> dailyConvLineChartValues2 = new List<OpenFlashChart.LineDotValue>();

            int max = 0;
            foreach (KeyValuePair<DateTime, IntPair> kvp in DailyTransactions)
            {
                dailyConvLineChartValues.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int1, string.Format("{0} Unique ({1})", kvp.Value.Int1.ToString("N0"), kvp.Key.ToShortDateString()), dotColor));
                dailyConvLineChartValues2.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int2, string.Format("{0} Total ({1})", kvp.Value.Int2.ToString("N0"), kvp.Key.ToShortDateString()), dotColor2));
                if (kvp.Value.Int1 > max) max = kvp.Value.Int1;
                if (kvp.Value.Int2 > max) max = kvp.Value.Int2;
            }
            dailyConvLineChart.Values = dailyConvLineChartValues;
            dailyConvLineChart2.Values = dailyConvLineChartValues2;

            OpenFlashChart.OpenFlashChart dailyRevChart = ChartUtil.CreateDefaultChart("Daily Transactions", max);
            dailyRevChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            dailyRevChart.AddElement(dailyConvLineChart2);
            dailyRevChart.AddElement(dailyConvLineChart);
            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            //dailyRevChart.Y_Axis.Steps = 10;

            return dailyRevChart;
        }

        protected void InitDailyConversionToPayingChart()
        {
            DailyAverageRevenuePerUser = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate);
            DailyConversionToPaying = AdminCache.LoadDailyConversionToPaying(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyConversionToPayingChart()
        {
            InitDailyConversionToPayingChart();

            string dotColorUnique = "#7bc90b";
            string dotColorTotal = "#ffbd16";

            OpenFlashChart.Area areaChartUnique = ChartUtil.CreateDefaultAreaChart(dotColorUnique);
            OpenFlashChart.Area areaChartTotal = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            List<OpenFlashChart.LineDotValue> lineChartValuesUnique = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineChartValuesTotal = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            foreach (KeyValuePair<DateTime, FloatPair> kvp in DailyConversionToPaying)
            {
                float dailyConvertionUnique = kvp.Value.Float1 * 1000;
                float dailyConvertionTotal = kvp.Value.Float2 * 1000;
                lineChartValuesUnique.Add(new OpenFlashChart.LineDotValue(dailyConvertionUnique, string.Format("{0} Unique ({1})", kvp.Value.Float1.ToString("P3"), kvp.Key.ToChartTooltip()), dotColorUnique));
                lineChartValuesTotal.Add(new OpenFlashChart.LineDotValue(dailyConvertionTotal, string.Format("{0} Total ({1})", kvp.Value.Float2.ToString("P3"), kvp.Key.ToChartTooltip()), dotColorTotal));

                if (dailyConvertionTotal > max)
                {
                    max = dailyConvertionTotal;
                }
            }

            areaChartUnique.Values = lineChartValuesUnique;
            areaChartTotal.Values = lineChartValuesTotal;

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Daily Conversion To Paying", max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            chartObject.AddElement(areaChartTotal);
            chartObject.AddElement(areaChartUnique);
            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = 1;

            return chartObject;
        }

        #endregion
    }
}