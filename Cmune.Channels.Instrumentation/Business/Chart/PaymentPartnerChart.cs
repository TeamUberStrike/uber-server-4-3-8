using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.DataCenter.DataAccess;
using System.Web.Caching;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class PaymentProviderChart : StatisticChart
    {
        #region Properties

        public Dictionary<DateTime, decimal> DailyRevenue { get; private set; }
        public Dictionary<DateTime, IntPair> DailyTransactions { get; private set; }
        public Dictionary<int, decimal> PackageContributionByRevenue { get; private set; }
        public Dictionary<int, int> PackageContributionByVolume { get; private set; }
        public Dictionary<DateTime, Dictionary<int, int>> CreditsSales { get; private set; }
        public Dictionary<DateTime, Dictionary<int, int>> BundlesSales { get; private set; }

        #endregion

        #region Constructors

        public PaymentProviderChart(DateTime fromDate, DateTime toDate)
            : base(fromDate, toDate)
        {
        }

        #endregion

        public OpenFlashChart.OpenFlashChart DrawPackagesChart(PaymentProviderType partnerId)
        {
            Dictionary<decimal, PackageView> packagesView = new Dictionary<decimal, PackageView> { { 9m, new PackageView() { Bonus = 1, Items = new List<int>() { 1 }, Name = "todelete", Price = 1 } } };
            Dictionary<int, int> packagesCount = new Dictionary<int, int>();
            Dictionary<int, PackageDisplay> packages = new Dictionary<int, PackageDisplay>();

            List<CreditDeposit> creditDeposits = new List<CreditDeposit>();
            // We cache this query for one hour (expiration is in one hour so we don't need to store the datetimes in the cache name)
            string partnerCacheName = "Cmune.DataCenter.Business.CmuneEconomy.GetCreditDeposits." + partnerId.ToString();
            DateTime endingDate = DateTime.Now;
            DateTime startingDate = endingDate.AddMonths(-3);
            if (startingDate.Day != 1)
                startingDate.AddDays(-(startingDate.Day - 1));
            if (startingDate < CommonConfig.UberStrikeStartingDate)
                startingDate = CommonConfig.UberStrikeStartingDate;
            DateTime firstDayOfCurrentWeek = endingDate.GetFirstDateOfWeek();
            firstDayOfCurrentWeek = new DateTime(firstDayOfCurrentWeek.Year, firstDayOfCurrentWeek.Month, firstDayOfCurrentWeek.Day, 0, 0, 0);

            foreach (int packageAmount in packagesView.Keys)
                packagesCount.Add(packageAmount, 0);

            if (HttpRuntime.Cache[partnerCacheName] != null)
            {
                creditDeposits = (List<CreditDeposit>)HttpRuntime.Cache[partnerCacheName];
            }
            else
            {
                creditDeposits = CmuneEconomy.GetCreditDeposits(partnerId, startingDate, endingDate);
                HttpRuntime.Cache.Add(partnerCacheName, creditDeposits, null, DateTime.Now.AddMinutes(5), new TimeSpan(0, 0, 0, 0, 0), CacheItemPriority.Default, null);
            }

            creditDeposits = creditDeposits.OrderByDescending(c => c.id).ToList();
            foreach (CreditDeposit creditDeposit in creditDeposits)
            {
                int currentDeposit = (int)creditDeposit.NbCash;
                // We count the number of packages
                if (packagesCount.ContainsKey(currentDeposit))
                {
                    packagesCount[currentDeposit] += 1;
                }
            }

            int paymentCount = creditDeposits.Count;

            foreach (int amount in packagesCount.Keys)
            {
                decimal percentage = 0;

                if (paymentCount != 0)
                {
                    percentage = (decimal)packagesCount[amount] / (decimal)paymentCount;
                }
                if (!packages.ContainsKey(amount))
                {
                    packages.Add(amount, new PackageDisplay(amount, percentage));
                }
            }

            // Packages chart

            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            foreach (int currency in packagesCount.Keys)
            {
                pieValues.Add(new OpenFlashChart.PieValue(packagesCount[currency], currency.ToString("C0") + " - " + packages[currency].Percentage.ToString("P")));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#454141", "#F99506" };
            pie.Tooltip = "#val# of #total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart packagesChart = new OpenFlashChart.OpenFlashChart();
            packagesChart.Title = new OpenFlashChart.Title("Packages distribution");
            packagesChart.Title.Style = "{color:#000000; font-size:14px;}";
            packagesChart.AddElement(pie);
            packagesChart.Bgcolor = "#FFFFFF";

            return packagesChart;
        }

        protected void InitDailyRevenue(PaymentProviderType providerId)
        {
            DailyRevenue = AdminCache.LoadDailyRevenueByPaymentProvider(FromDate, ToDate, providerId);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyRevenueChart(PaymentProviderType providerId)
        {
            InitDailyRevenue(providerId);

            string dotColor = AdminConfig.GetPaymentProviderColor(providerId);

            List<OpenFlashChart.LineDotValue> lineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, decimal> kvp in DailyRevenue)
            {
                lineChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, String.Format("US${0} ({1})", kvp.Value.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area dailyRevLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            dailyRevLineChart.Values = lineChartValues;

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Daily Revenue", (double)max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart((double)max, ApplicationMilestones));

            chartObject.AddElement(dailyRevLineChart);

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = (int) (max / 5m);

            return chartObject;
        }

        protected void InitDailyTransactions(int providerId)
        {
            DailyTransactions = AdminCache.LoadDailyTransactions(FromDate, ToDate, providerId);
        }

        public OpenFlashChart.OpenFlashChart DrawDailyTransactionsChart(int providerId)
        {
            InitDailyTransactions(providerId);

            return DrawDailyTransactionsChart(DailyTransactions);
        }

        protected void InitPackageContributionByRevenueChart(int providerId, bool areCreditsBundles)
        {
            PackageContributionByRevenue = AdminCache.LoadPackageContributionByRevenue(FromDate, ToDate, providerId, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackageContributionByRevenueChart(int providerId, bool areCreditsBundles)
        {
            InitPackageContributionByRevenueChart(providerId, areCreditsBundles);

            return DrawPackageContributionByRevenueChart(PackageContributionByRevenue, areCreditsBundles);
        }

        protected void InitPackageContributionByVolumeChart(int providerId, bool areCreditsBundles)
        {
            PackageContributionByVolume = AdminCache.LoadPackageContributionByVolume(FromDate, ToDate, providerId, areCreditsBundles);
        }

        public OpenFlashChart.OpenFlashChart DrawPackageContributionByVolumeChart(int providerId, bool areCreditsBundles)
        {
            InitPackageContributionByVolumeChart(providerId, areCreditsBundles);

            return DrawPackageContributionByVolumeChart(PackageContributionByVolume, areCreditsBundles);
        }

        protected void InitDrawCreditsSalesChart(int providerId)
        {
            CreditsSales = AdminCache.LoadCreditSales(FromDate, ToDate, providerId);
        }

        public OpenFlashChart.OpenFlashChart DrawCreditsSalesChart(int providerId)
        {
            InitDrawCreditsSalesChart(providerId);

            return DrawBundlesSalesChart(CreditsSales, true);
        }

        protected void InitDrawBundlesSalesChart(int providerId)
        {
            BundlesSales = AdminCache.LoadBundlesSales(FromDate, ToDate, providerId);
        }

        public OpenFlashChart.OpenFlashChart DrawBundlesSalesChart(int providerId)
        {
            InitDrawBundlesSalesChart(providerId);

            return DrawBundlesSalesChart(BundlesSales, false);
        }
    }
}