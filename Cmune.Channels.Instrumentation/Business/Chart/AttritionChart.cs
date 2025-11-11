using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class AttritionChart : StatisticChart
    {
        #region Properties

        public int AttritionDay { get; private set; }

        public Dictionary<DateTime, Dictionary<int, decimal>> RetentionCohorts { get; private set; }
        public Dictionary<int, string> CountriesName { get; private set; }

        #endregion

        #region Constructors

        public AttritionChart(DateTime FromDate, DateTime ToDate, int attritionDay = 0)
                : base(FromDate, ToDate)
        {
            AttritionDay = attritionDay;
        }

        #endregion

        #region Methods

        protected void InitRetentionCohortsChart()
        {
            RetentionCohorts = AdminCache.LoadRetentionCohorts(FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawRetentionCohortsChart()
        {
            InitRetentionCohortsChart();

            return DrawRetentionCohortsChart(RetentionCohorts, "Retention");
        }

        protected void InitRetentionCohortsChart(ChannelType channel)
        {
            RetentionCohorts = AdminCache.LoadRetentionCohorts(FromDate, ToDate, channel);
        }

        public OpenFlashChart.OpenFlashChart DrawRetentionCohortsChart(ChannelType channel)
        {
            InitRetentionCohortsChart(channel);

            return DrawRetentionCohortsChart(RetentionCohorts, String.Format("{0} Retention", channel));
        }

        protected void InitRetentionCohortsChart(ReferrerPartnerType referrerId)
        {
            RetentionCohorts = AdminCache.LoadRetentionCohorts(FromDate, ToDate, referrerId);
        }

        public OpenFlashChart.OpenFlashChart DrawRetentionCohortsChart(ReferrerPartnerType referrerId)
        {
            InitRetentionCohortsChart(referrerId);

            return DrawRetentionCohortsChart(RetentionCohorts, String.Format("{0} Retention", referrerId));
        }

        protected void InitRetentionCohortsChart(int regionId)
        {
            RetentionCohorts = AdminCache.LoadRetentionCohorts(FromDate, ToDate, regionId);
            CountriesName = AdminCache.LoadCountriesName();
        }

        public OpenFlashChart.OpenFlashChart DrawRetentionCohortsChart(int regionId)
        {
            InitRetentionCohortsChart(regionId);

            string countryName;

            if (!CountriesName.TryGetValue(regionId, out countryName))
            {
                countryName = "Unknown country";
            }

            return DrawRetentionCohortsChart(RetentionCohorts, String.Format("{0} Retention", countryName));
        }

        #endregion
    }
}