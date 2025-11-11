using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class ReferrersChart : StatisticChart
    {
        #region Properties

        public ReferrerPartnerType ReferrerOne { get; private set; }
        public ReferrerPartnerType ReferrerTwo { get; private set; }
        public ReferrerPartnerType ReferrerThree { get; private set; }

        public Dictionary<DateTime, decimal> DailyRevenueReferrerOne { get; private set; }
        public Dictionary<DateTime, decimal> DailyRevenueReferrerTwo { get; private set; }
        public Dictionary<DateTime, decimal> DailyRevenueReferrerThree { get; private set; }

        public Dictionary<DateTime, int> DauReferrerOne { get; private set; }
        public Dictionary<DateTime, int> DauReferrerTwo { get; private set; }
        public Dictionary<DateTime, int> DauReferrerThree { get; private set; }

        public Dictionary<DateTime, int> MauReferrerOne { get; private set; }
        public Dictionary<DateTime, int> MauReferrerTwo { get; private set; }
        public Dictionary<DateTime, int> MauReferrerThree { get; private set; }

        public Dictionary<DateTime, float> DailyPlayRateReferrerOne { get; private set; }
        public Dictionary<DateTime, float> DailyPlayRateReferrerTwo { get; private set; }
        public Dictionary<DateTime, float> DailyPlayRateReferrerThree { get; private set; }

        public Dictionary<ReferrerPartnerType, decimal> ReferrerContribution { get; private set; }

        public Dictionary<DateTime, int> NewMembersReferrerOne { get; private set; }
        public Dictionary<DateTime, int> NewMembersReferrerTwo { get; private set; }
        public Dictionary<DateTime, int> NewMembersReferrerThree { get; private set; }

        public Dictionary<int, string> CountriesName { get; private set; }

        public Dictionary<DateTime, DecimalPair> DarpuReferrerOne { get; private set; }
        public Dictionary<DateTime, DecimalPair> DarpuReferrerTwo { get; private set; }
        public Dictionary<DateTime, DecimalPair> DarpuReferrerThree { get; private set; }

        public Dictionary<DateTime, int> NewMembersRegionReferrerOne { get; private set; }
        public Dictionary<DateTime, int> NewMembersRegionReferrerTwo { get; private set; }
        public Dictionary<DateTime, int> NewMembersRegionReferrerThree { get; private set; }

        #endregion

        #region Constructors

        public ReferrersChart(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerOne, ReferrerPartnerType referrerTwo, ReferrerPartnerType referrerThree)
                : base(fromDate, toDate)
        {
            ReferrerOne = referrerOne;
            ReferrerTwo = referrerTwo;
            ReferrerThree = referrerThree;
        }

        #endregion

        #region Methods

        protected void InitDailyRevenueChart()
        {
            DailyRevenueReferrerOne = new Dictionary<DateTime, decimal>();
            DailyRevenueReferrerTwo = new Dictionary<DateTime, decimal>();
            DailyRevenueReferrerThree = new Dictionary<DateTime, decimal>();

            if (ReferrerOne > 0)
            {
                DailyRevenueReferrerOne = AdminCache.LoadDailyRevenue(FromDate, ToDate, ReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                DailyRevenueReferrerTwo = AdminCache.LoadDailyRevenue(FromDate, ToDate, ReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                DailyRevenueReferrerThree = AdminCache.LoadDailyRevenue(FromDate, ToDate, ReferrerThree);
            }
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueChart()
        {
            InitDailyRevenueChart();

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString("C2"), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue((double) kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString("C2"), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenueReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString("C3"), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart dailyRevenueChart = ChartUtil.CreateDefaultChart("Daily revenue", (double) max);
            dailyRevenueChart.AddElement(ChartUtil.CreateMilestoneChart((double) max, ApplicationMilestones));
            dailyRevenueChart.Y_Axis.Steps = (int) max / 5;

            if (ReferrerOne > 0)
            {
                dailyRevenueChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                dailyRevenueChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                dailyRevenueChart.AddElement(areaChartReferrerThree);
            }

            dailyRevenueChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dailyRevenueChart;
        }

        protected void InitDailyAverageRevenuePerUserChart(int regionId)
        {
            DarpuReferrerOne = new Dictionary<DateTime, DecimalPair>();
            DarpuReferrerTwo = new Dictionary<DateTime, DecimalPair>();
            DarpuReferrerThree = new Dictionary<DateTime, DecimalPair>();

            if (ReferrerOne > 0)
            {
                DarpuReferrerOne = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate, ReferrerOne, regionId);
            }

            if (ReferrerTwo > 0)
            {
                DarpuReferrerTwo = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate, ReferrerTwo, regionId);
            }

            if (ReferrerThree > 0)
            {
                DarpuReferrerThree = AdminCache.LoadDailyAverageRevenuePerUser(FromDate, ToDate, ReferrerThree, regionId);
            }

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawDailyAverageRevenuePerUserChart(int regionId)
        {
            InitDailyAverageRevenuePerUserChart(regionId);

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DarpuReferrerOne)
            {
                decimal darpu = kvp.Value.Decimal2 * 100;
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue((double)darpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (darpu > max) max = darpu;
            }

            OpenFlashChart.Area areaChartReferrerOne = ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DarpuReferrerTwo)
            {
                decimal darpu = kvp.Value.Decimal2 * 100;
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue((double)darpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (darpu > max) max = darpu;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in DarpuReferrerThree)
            {
                decimal darpu = kvp.Value.Decimal2 * 100;
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue((double)darpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (darpu > max) max = darpu;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the chart

            OpenFlashChart.OpenFlashChart darpuChart = ChartUtil.CreateDefaultChart(String.Format("DARPU for {0}", CountriesName[regionId]), (double)max);
            darpuChart.AddElement(ChartUtil.CreateMilestoneChart((double)max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                darpuChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                darpuChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                darpuChart.AddElement(areaChartReferrerThree);
            }

            darpuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return darpuChart;
        }

        protected void InitDauChart()
        {
            DauReferrerOne = new Dictionary<DateTime, int>();
            DauReferrerTwo = new Dictionary<DateTime, int>();
            DauReferrerThree = new Dictionary<DateTime, int>();

            if (ReferrerOne > 0)
            {
                DauReferrerOne = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                DauReferrerTwo = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                DauReferrerThree = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerThree);
            }
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart()
        {
            InitDauChart();

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart dauChart = ChartUtil.CreateDefaultChart("DAU", max);

            dauChart.Y_Axis.Steps = 1;

            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                dauChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                dauChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                dauChart.AddElement(areaChartReferrerThree);
            }

            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        protected void InitDauChart(int regionId)
        {
            DauReferrerOne = new Dictionary<DateTime, int>();
            DauReferrerTwo = new Dictionary<DateTime, int>();
            DauReferrerThree = new Dictionary<DateTime, int>();

            if (ReferrerOne > 0)
            {
                DauReferrerOne = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerOne, regionId);
            }

            if (ReferrerTwo > 0)
            {
                DauReferrerTwo = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerTwo, regionId);
            }

            if (ReferrerThree > 0)
            {
                DauReferrerThree = AdminCache.LoadDailyActiveUsers(FromDate, ToDate, ReferrerThree, regionId);
            }

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawDauChart(int regionId)
        {
            InitDauChart(regionId);

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, int> kvp in DauReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart dauChart = ChartUtil.CreateDefaultChart(String.Format("Daily Active Users for {0}", CountriesName[regionId]), max);
            dauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dauChart.AddElement(areaChartReferrerOne);
            dauChart.AddElement(areaChartReferrerTwo);
            dauChart.AddElement(areaChartReferrerThree);

            dauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dauChart;
        }

        protected void InitMauChart()
        {
            MauReferrerOne = new Dictionary<DateTime, int>();
            MauReferrerTwo = new Dictionary<DateTime, int>();
            MauReferrerThree = new Dictionary<DateTime, int>();

            if (ReferrerOne > 0)
            {
                MauReferrerOne = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate, ReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                MauReferrerTwo = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate, ReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                MauReferrerThree = AdminCache.LoadMonthlyActiveUsers(FromDate, ToDate, ReferrerThree);
            }
        }

        public OpenFlashChart.OpenFlashChart DrawMauChart()
        {
            InitMauChart();

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, int> kvp in MauReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, int> kvp in MauReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, int> kvp in MauReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart mauChart = ChartUtil.CreateDefaultChart("MAU", max);

            mauChart.Y_Axis.Steps = 1;

            mauChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                mauChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                mauChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                mauChart.AddElement(areaChartReferrerThree);
            }

            mauChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return mauChart;
        }

        protected void InitDailyPlayRateChart()
        {
            DailyPlayRateReferrerOne = new Dictionary<DateTime, float>();
            DailyPlayRateReferrerTwo = new Dictionary<DateTime, float>();
            DailyPlayRateReferrerThree = new Dictionary<DateTime, float>();

            if (ReferrerOne > 0)
            {
                DailyPlayRateReferrerOne = AdminCache.LoadDailyPlayRate(FromDate, ToDate, ReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                DailyPlayRateReferrerTwo = AdminCache.LoadDailyPlayRate(FromDate, ToDate, ReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                DailyPlayRateReferrerThree = AdminCache.LoadDailyPlayRate(FromDate, ToDate, ReferrerThree);
            }
        }

        public OpenFlashChart.OpenFlashChart DrawDailyPlayRateChart()
        {
            InitDailyPlayRateChart();

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, float> kvp in DailyPlayRateReferrerOne)
            {
                float playRate = kvp.Value * 100;
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(playRate, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (playRate > max) max = playRate;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, float> kvp in DailyPlayRateReferrerTwo)
            {
                float playRate = kvp.Value * 100;
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(playRate, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (playRate > max) max = playRate;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, float> kvp in DailyPlayRateReferrerThree)
            {
                float playRate = kvp.Value * 100;
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(playRate, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (playRate > max) max = playRate;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart dailyPlayRateChart = ChartUtil.CreateDefaultChart("Daily Play Rate", (int)(max));

            dailyPlayRateChart.Y_Axis.Steps = 5;

            dailyPlayRateChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                dailyPlayRateChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                dailyPlayRateChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                dailyPlayRateChart.AddElement(areaChartReferrerThree);
            }

            dailyPlayRateChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return dailyPlayRateChart;
        }

        protected void InitNewMembersChart()
        {
            NewMembersReferrerOne = new Dictionary<DateTime, int>();
            NewMembersReferrerTwo = new Dictionary<DateTime, int>();
            NewMembersReferrerThree = new Dictionary<DateTime, int>();

            if (ReferrerOne > 0)
            {
                NewMembersReferrerOne = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                NewMembersReferrerTwo = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                NewMembersReferrerThree = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerThree);
            }
        }

        public OpenFlashChart.OpenFlashChart DrawNewMembersChart()
        {
            InitNewMembersChart();

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart newMembersChart = ChartUtil.CreateDefaultChart("New members", max);

            newMembersChart.Y_Axis.Steps = 1;

            newMembersChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                newMembersChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                newMembersChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                newMembersChart.AddElement(areaChartReferrerThree);
            }

            newMembersChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return newMembersChart;
        }

        protected void InitReferrerContributionChart()
        {
            ReferrerContribution = AdminCache.LoadReferrerContribution(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawReferrerContributionChart()
        {
            InitReferrerContributionChart();

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (KeyValuePair<ReferrerPartnerType, decimal> kvp in ReferrerContribution)
            {
                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, CommonConfig.ReferrerPartnerName[kvp.Key]));
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
            objectChart.Title = new OpenFlashChart.Title("Referrer Revenue Distribution");
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected void InitNewMemberByRegionChart(int regionId)
        {
            NewMembersRegionReferrerOne = new Dictionary<DateTime, int>();
            NewMembersRegionReferrerTwo = new Dictionary<DateTime, int>();
            NewMembersRegionReferrerThree = new Dictionary<DateTime, int>();

            if (ReferrerOne > 0)
            {
                NewMembersRegionReferrerOne = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerOne, regionId);
            }

            if (ReferrerTwo > 0)
            {
                NewMembersRegionReferrerTwo = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerTwo, regionId);
            }

            if (ReferrerThree > 0)
            {
                NewMembersRegionReferrerThree = AdminCache.LoadNewMembers(FromDate, ToDate, ReferrerThree, regionId);
            }

            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawNewMemberByRegionChart(int regionId)
        {
            InitNewMemberByRegionChart(regionId);

            string dotColorReferrerOne = "#F06F30";
            string dotColorReferrerTwo = "#63BE6C";
            string dotColorReferrerThree = "#78B3E5";

            List<OpenFlashChart.LineDotValue> lineReferrerOneValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerTwoValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineReferrerThreeValues = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            // Referrer One

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersRegionReferrerOne)
            {
                lineReferrerOneValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerOne], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerOne));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerOne = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColorReferrerOne);
            areaChartReferrerOne.Values = lineReferrerOneValues;

            // Referrer Two

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersRegionReferrerTwo)
            {
                lineReferrerTwoValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerTwo], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerTwo));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerTwo = ChartUtil.CreateDefaultAreaChart(dotColorReferrerTwo);
            areaChartReferrerTwo.Values = lineReferrerTwoValues;

            // Referrer Three

            foreach (KeyValuePair<DateTime, int> kvp in NewMembersRegionReferrerThree)
            {
                lineReferrerThreeValues.Add(new OpenFlashChart.LineDotValue(kvp.Value, string.Format("{0}: {1} ({2})", CommonConfig.ReferrerPartnerName[ReferrerThree], kvp.Value.ToString(), kvp.Key.ToChartTooltip()), dotColorReferrerThree));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChartReferrerThree = ChartUtil.CreateDefaultAreaChart(dotColorReferrerThree);
            areaChartReferrerThree.Values = lineReferrerThreeValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart newMembersChart = ChartUtil.CreateDefaultChart(String.Format("New members for {0}", CountriesName[regionId]), max);
            newMembersChart.Y_Axis.Steps = max / 8;
            newMembersChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            if (ReferrerOne > 0)
            {
                newMembersChart.AddElement(areaChartReferrerOne);
            }

            if (ReferrerTwo > 0)
            {
                newMembersChart.AddElement(areaChartReferrerTwo);
            }

            if (ReferrerThree > 0)
            {
                newMembersChart.AddElement(areaChartReferrerThree);
            }

            newMembersChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return newMembersChart;
        }

        #endregion
    }
}