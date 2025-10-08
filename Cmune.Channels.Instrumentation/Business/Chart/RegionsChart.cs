using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Extensions;
using OpenFlashChart;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class RegionsChart : StatisticChart
    {
        #region Properties

        public Dictionary<DateTime, decimal> DailyRevenueRegionOne { get; private set; }
        public Dictionary<DateTime, decimal> DailyRevenueRegionTwo { get; private set; }
        public Dictionary<DateTime, decimal> DailyRevenueRegionThree { get; private set; }
        public Dictionary<int, string> CountriesName { get; private set; }
        public Dictionary<DateTime, int> DauTotal { get; private set; }
        public Dictionary<ChannelType, Dictionary<DateTime, int>> DauByChannels { get; private set; }
        public Dictionary<DateTime, int> MauTotal { get; private set; }
        public Dictionary<ChannelType, Dictionary<DateTime, int>> MauByChannels { get; private set; }
        public Dictionary<DateTime, DecimalPair> Darpu { get; private set; }

        #endregion

        #region Constructors

        public RegionsChart(DateTime FromDate, DateTime ToDate)
                : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitDailyRevenueChart(int countryIdOne, int countryIdTwo, int countryIdThree)
        {
            DailyRevenueRegionOne = new Dictionary<DateTime, decimal>();
            DailyRevenueRegionTwo = new Dictionary<DateTime, decimal>();
            DailyRevenueRegionThree = new Dictionary<DateTime, decimal>();

            if (countryIdOne >= 0)
            {
                DailyRevenueRegionOne = AdminCache.LoadDailyRevenueByCountry(FromDate, ToDate, countryIdOne);
            }

            if (countryIdTwo >= 0)
            {
                DailyRevenueRegionTwo = AdminCache.LoadDailyRevenueByCountry(FromDate, ToDate, countryIdTwo);
            }

            if (countryIdThree >= 0)
            {
                DailyRevenueRegionThree = AdminCache.LoadDailyRevenueByCountry(FromDate, ToDate, countryIdThree);
            }

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueChart(int countryIdOne, int countryIdTwo, int countryIdThree)
        {
            InitDailyRevenueChart(countryIdOne, countryIdTwo, countryIdThree);

            string dotColorRegionOne = "#F06F30";
            string dotColorRegionTwo = "#63BE6C";
            string dotColorRegionThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineRegionOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineRegionTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineRegionThreeValues = new List<OpenFlashChart.LineDotValue>();

            string countryOneName = "None";
            string countryTwoName = "None";
            string countryThreeName = "None";
            CountriesName.TryGetValue(countryIdOne, out countryOneName);
            CountriesName.TryGetValue(countryIdTwo, out countryTwoName);
            CountriesName.TryGetValue(countryIdThree, out countryThreeName);

            decimal max = 0;

            // Region One

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueRegionOne)
            {
                lineRegionOneValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} ({2})", countryOneName, kvp.Value.ToString("C2"), kvp.Key.ToChartTooltip()), dotColorRegionOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartRegionOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorRegionOne);
            areaChartRegionOne.Values = lineRegionOneValues;

            // Region Two

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueRegionTwo)
            {
                lineRegionTwoValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value, string.Format("{0}: {1} ({2})", countryTwoName, kvp.Value.ToString("C2"), kvp.Key.ToChartTooltip()), dotColorRegionTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartRegionTwo = ChartUtil.CreateDefaultAreaChart(dotColorRegionTwo);
            areaChartRegionTwo.Values = lineRegionTwoValues;

            // Region Three

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueRegionThree)
            {
                lineRegionThreeValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} ({2})", countryThreeName, kvp.Value.ToString("C3"), kvp.Key.ToChartTooltip()), dotColorRegionThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartRegionThree = ChartUtil.CreateDefaultAreaChart(dotColorRegionThree);
            areaChartRegionThree.Values = lineRegionThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart dailyRevenueChart = ChartUtil.CreateDefaultChart("Daily revenue", (double) max);

            dailyRevenueChart.Y_Axis.Steps = 1;

            dailyRevenueChart.AddElement(ChartUtil.CreateMilestoneChart((int) max, ApplicationMilestones));

            if (countryIdOne >= 0)
            {
                dailyRevenueChart.AddElement(areaChartRegionOne);
            }

            if (countryIdTwo >= 0)
            {
                dailyRevenueChart.AddElement(areaChartRegionTwo);
            }

            if (countryIdThree >= 0)
            {
                dailyRevenueChart.AddElement(areaChartRegionThree);
            }

            dailyRevenueChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dailyRevenueChart;
        }

        protected void InitDauChart(int regionId)
        {
            DauTotal = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, regionId);
            DauByChannels = AdminCache.LoadDailyActiveUsersByChannels(FromDate, ToDate, regionId);

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart(int regionId)
        {
            InitDauChart(regionId);

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<LineDotValue> lineTotalValues = new List<LineDotValue>();
            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in DauByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            int max = 0;

            // Total

            foreach (KeyValuePair<DateTime, int> kvp in DauTotal)
            {
                lineTotalValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("All Channels: {0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            areaChart.Values = lineTotalValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, int> kvp in DauByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("{0}: {1} Users ({2})", channel, kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart dauChart = ChartUtil.CreateDefaultChart(String.Format("Daily Active Users for {0}", CountriesName[regionId]), max);
            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dauChart.AddElement(areaChart);

            foreach (ChannelType channel in values.Keys)
            {
                dauChart.AddElement(charts[channel]);
            }

            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        protected void InitMauChart(int regionId)
        {
            MauTotal = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate, regionId);
            MauByChannels = AdminCache.LoadMonthlyActiveUsersByChannels(FromDate, ToDate, regionId);

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawMauChart(int regionId)
        {
            InitMauChart(regionId);

            string dotColorTotal = AdminConfig.ChartColorChannelAll;

            List<LineDotValue> lineTotalValues = new List<LineDotValue>();
            Dictionary<ChannelType, List<LineDotValue>> values = new Dictionary<ChannelType, List<LineDotValue>>();
            Dictionary<ChannelType, Area> charts = new Dictionary<ChannelType, Area>();

            foreach (ChannelType channel in MauByChannels.Keys)
            {
                values.Add(channel, new List<OpenFlashChart.LineDotValue>());
                charts.Add(channel, ChartUtil.CreateDefaultAreaChart(AdminConfig.GetChannelColor(channel)));
            }

            int max = 0;

            // Total

            foreach (KeyValuePair<DateTime, int> kvp in MauTotal)
            {
                lineTotalValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0} Users ({1})", kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColorTotal);
            areaChart.Values = lineTotalValues;

            // Channels

            foreach (ChannelType channel in values.Keys)
            {
                foreach (KeyValuePair<DateTime, int> kvp in MauByChannels[channel])
                {
                    values[channel].Add(new OpenFlashChart.LineDotValue(kvp.Value, String.Format("{0}: {1} Users ({2})", channel, kvp.Value.ToString("N0"), kvp.Key.ToChartTooltip()), AdminConfig.GetChannelColor(channel)));
                }

                charts[channel].Values = values[channel];
            }

            OpenFlashChart.OpenFlashChart mauChart = ChartUtil.CreateDefaultChart(String.Format("Monthly Active Users for {0}", CountriesName[regionId]), max);
            mauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            mauChart.AddElement(areaChart);

            foreach (ChannelType channel in values.Keys)
            {
                mauChart.AddElement(charts[channel]);
            }

            mauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return mauChart;
        }

        protected void InitDailyPlayRateChart(int regionId)
        {
            InitDauChart(regionId);
            InitMauChart(regionId);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyPlayRateChart(int regionId)
        {
            InitDailyPlayRateChart(regionId);
            string dotColor = "#f16406";

            // Setup the Line Point Values
            List<OpenFlashChart.LineDotValue> lineDotValues = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            foreach (KeyValuePair<DateTime, int> day in DauTotal)
            {
                int val;
                float t = 0;
                MauTotal.TryGetValue(day.Key, out val);

                if (val > 0)
                    t = ((float)day.Value / (float)val);

                // Change to %
                t = t * 100;

                lineDotValues.Add(new OpenFlashChart.LineDotValue(t, string.Format("Daily Play Rate {0}% ({1})", t.ToString("N2"), day.Key.ToChartTooltip()), dotColor));
                if (t > max) max = t;
            }

            // Create the Line Chart
            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            areaChart.Values = lineDotValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart newChart = ChartUtil.CreateDefaultChart(String.Format("Daily Play Rate for {0}", CountriesName[regionId]), max);
            newChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            newChart.AddElement(areaChart);
            newChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            newChart.Y_Axis.Steps = 10;

            return newChart;
        }

        void InitDarpuChart(int regionId)
        {
            Darpu = AdminCache.LoadDailyAverageRevenuePerUserPerCountry(FromDate, ToDate, regionId);

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawDarpuChart(int regionId)
        {
            InitDarpuChart(regionId);

            return DrawDarpuChart(Darpu, String.Format("DARPU for {0}", CountriesName[regionId]));
        }

        #endregion
    }
}