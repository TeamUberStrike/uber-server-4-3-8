using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class DashboardChart : StatisticChart
    {
        #region Properties

        /* dashboard tab */
        protected Dictionary<DateTime, int> DailyActiveUsers; /* used in user activity tab too */
        protected Dictionary<DateTime, int> MonthlyActiveUsers; /* used in user activity tab too */
        protected Dictionary<DateTime, float> DailyPlayRate;
        protected Dictionary<DateTime, decimal> DailyRevenue;

        #endregion
        
        #region Constructors

        public DashboardChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion
        
        #region Methods

        protected void InitDauMauChart()
        {
            DailyActiveUsers = AdminCache.LoadDailyActiveUsers(FromDate, ToDate);
            MonthlyActiveUsers = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate);
            DailyPlayRate = AdminCache.LoadDailyPlayRate(FromDate, ToDate);
        }

        /// <summary>
        /// Get the chart for Dau Mau
        /// </summary>
        /// <returns></returns>
        public OpenFlashChart.OpenFlashChart DrawDauMauChart()
        {
            InitDauMauChart();

            string dotColor = "#0078a4";
            string dotColor3 = "#f16406";
            string dotColor2 = "#ffbd16";

            // Setup the Line Point Values
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineDotValues2 = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineDotValues3 = new List<OpenFlashChart.LineDotValue>();

            int max = 0;
            foreach (KeyValuePair<DateTime, int> kvp in DailyActiveUsers)
            {
                lineDotValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("DAU {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            foreach (KeyValuePair<DateTime, int> kvp in MonthlyActiveUsers)
            {
                lineDotValues2.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("MAU {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColor2));
                if (kvp.Value > max) max = kvp.Value;
            }

            foreach (KeyValuePair<DateTime, float> kvp in DailyPlayRate)
            {
                lineDotValues3.Add(new OpenFlashChart.LineDotValue(kvp.Value * max, string.Format("Play Rate {0} ({1})", kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColor3));
            }

            // Create the Line Chart
            OpenFlashChart.Area areaChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor);
            OpenFlashChart.Area areaChart2 = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor2);
            OpenFlashChart.Line areaChart3 = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultLineChart(dotColor3);
            areaChart.Values = lineDotValues;
            areaChart2.Values = lineDotValues2;
            areaChart3.Values = lineDotValues3;

            // Create the Chart
            OpenFlashChart.OpenFlashChart dauChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Daily User Activity", max);
            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            dauChart.AddElement(areaChart2);
            dauChart.AddElement(areaChart);
            dauChart.AddElement(areaChart3);

            //SetXAxis(dauChart);

            IList<string> caca = ChartUtil.GetXAxisLabels(FromDate, ToDate);
            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dauChart.X_Axis.Steps = caca.Count / 3;
            //dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        protected void InitDailyRevenueChart()
        {
            DailyRevenue = AdminCache.LoadDailyRevenue(FromDate, ToDate);
        }

        /// <summary>
        /// Create revenue chart
        /// </summary>
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

            OpenFlashChart.Area dailyRevLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = dailyRevLineChartValues;

            OpenFlashChart.OpenFlashChart dailyRevChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultChart("Daily Revenue", (double)max);
            dailyRevChart.AddElement(Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            dailyRevChart.AddElement(dailyRevLineChart);
            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dailyRevChart.Y_Axis.Steps = 0;

            return dailyRevChart;
        }

        #endregion
    }
}