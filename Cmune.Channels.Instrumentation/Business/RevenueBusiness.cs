using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.Channels.Instrumentation.Utils;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using Cmune.Instrumentation.DataAccess;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class RevenueBusiness
    {
        #region Payment contribution

        public static Dictionary<PaymentProviderType, decimal> GetPaymentProviderContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PaymentProviderType, decimal> result = new Dictionary<PaymentProviderType, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   select new { m.PaymentProviderId, m.CashDeposited };

                foreach (var row in query)
                {
                    if (result.ContainsKey((PaymentProviderType)row.PaymentProviderId))
                    {
                        result[(PaymentProviderType)row.PaymentProviderId] += row.CashDeposited;
                    }
                    else
                    {
                        result.Add((PaymentProviderType)row.PaymentProviderId, row.CashDeposited);
                    }
                }
            }

            return result;
        }

        public static Dictionary<ChannelType, decimal> GetChannelContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, decimal> result = new Dictionary<ChannelType, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                result =
                   (from m in db.ChannelCreditDeposits
                    where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                    group m by m.ChannelId into m2
                    select new { ChannelId = m2.Key, CashDeposited = m2.Sum(m => m.CashDeposited) }).ToDictionary(q => (ChannelType)q.ChannelId, q => q.CashDeposited);
            }

            return result;
        }

        public static Dictionary<ReferrerPartnerType, decimal> GetReferrerContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ReferrerPartnerType, decimal> result = new Dictionary<ReferrerPartnerType, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ReferrerId != (int)ReferrerPartnerType.None
                   group m by m.ReferrerId into m2
                   select new { ReferrerId = m2.Key, CashDeposited = m2.Sum(m => m.CashDeposited) };

                // Make sure that the resultset contains all the channels

                result.Add(ReferrerPartnerType.SixWaves, 0);
                result.Add(ReferrerPartnerType.Applifier, 0);

                foreach (var row in query)
                {
                    result[(ReferrerPartnerType)row.ReferrerId] += row.CashDeposited;
                }
            }

            return result;
        }

        #endregion

        #region Bundles

        public static Dictionary<int, decimal> GetPackageContributionByRevenue(DateTime fromDate, DateTime toDate, bool areCreditsBundles)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && b.Description == null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
                else
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && b.Description != null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
            }

            return result;
        }

        public static Dictionary<int, decimal> GetPackageContributionByRevenue(DateTime fromDate, DateTime toDate, int paymentProviderId, bool areCreditsBundles)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.PartnerID == paymentProviderId && b.Description == null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
                else
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.PartnerID == paymentProviderId && b.Description != null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
            }

            return result;
        }

        public static Dictionary<int, decimal> GetPackageContributionByRevenue(DateTime fromDate, DateTime toDate, ChannelType channel, bool areCreditsBundles)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.ChannelID == (int)channel && b.Description == null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
                else
                {
                    var query =
                       from c in db.CreditDeposits
                       join b in db.Bundles on c.BundleId equals b.Id
                       where c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.ChannelID == (int)channel && b.Description != null
                       group c by c.BundleId into c2
                       select new { BundleId = c2.Key, Revenue = c2.Sum(m => m.UsdAmount) };

                    result = query.ToDictionary(m => (int)m.BundleId, m => m.Revenue);
                }
            }

            return result;
        }

        public static Dictionary<int, int> GetPackageContributionByVolume(DateTime fromDate, DateTime toDate, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && b.Description == null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
                else
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && b.Description != null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
            }

            return packagesContribution;
        }

        public static Dictionary<int, int> GetPackageContributionByVolume(DateTime fromDate, DateTime toDate, ChannelType channel, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && c.ChannelID == (int)channel && b.Description == null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
                else
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && c.ChannelID == (int)channel && b.Description != null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
            }

            return packagesContribution;
        }

        public static Dictionary<int, int> GetPackageContributionByVolume(DateTime fromDate, DateTime toDate, int paymentProviderId, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                if (areCreditsBundles)
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && c.PartnerID == paymentProviderId && b.Description == null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
                else
                {
                    var query = from c in cmuneDb.CreditDeposits
                                join b in cmuneDb.Bundles on c.BundleId equals b.Id
                                where c.DepositDate >= fromDate && c.DepositDate <= toDate && c.isAdminAction == false && c.PartnerID == paymentProviderId && b.Description != null
                                group c by c.BundleId into c2
                                select new { BundleId = c2.Key, Volume = c2.Count() };

                    packagesContribution = query.ToDictionary(m => (int)m.BundleId, m => m.Volume);
                }
            }

            return packagesContribution;
        }

        /// <summary>
        /// Get bundle sales (points or items bundles)
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, Dictionary<int, int>> GetBundlesSales(DateTime fromDate, DateTime toDate)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> bundleSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    bundleSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description != null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    bundleSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return bundleSales;
            }
        }

        public static Dictionary<DateTime, Dictionary<int, int>> GetBundlesSales(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> bundleSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    bundleSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description != null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.ChannelID == (int)channel
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    bundleSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return bundleSales;
            }
        }

        public static Dictionary<DateTime, Dictionary<int, int>> GetBundlesSales(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> bundleSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    bundleSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description != null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.PartnerID == paymentProviderId
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    bundleSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return bundleSales;
            }
        }

        /// <summary>
        /// Get credits sales (credits only bundles)
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, Dictionary<int, int>> GetCreditSales(DateTime fromDate, DateTime toDate)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> creditsSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    creditsSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description == null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    creditsSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return creditsSales;
            }
        }

        public static Dictionary<DateTime, Dictionary<int, int>> GetCreditSales(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> creditsSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    creditsSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description == null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.ChannelID == (int)channel
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    creditsSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return creditsSales;
            }
        }

        public static Dictionary<DateTime, Dictionary<int, int>> GetCreditSales(DateTime fromDate, DateTime toDate, int paymentPartnerId)
        {
            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<int, int>> creditsSales = new Dictionary<DateTime, Dictionary<int, int>>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    creditsSales.Add(day, new Dictionary<int, int>());
                }

                toDate = toDate.AddDays(1);

                var query = from c in cmuneDb.CreditDeposits
                            join b in cmuneDb.Bundles on c.BundleId equals b.Id
                            where b.Description == null && c.DepositDate >= fromDate && c.DepositDate <= toDate && !c.isAdminAction && c.PartnerID == paymentPartnerId
                            group c by new { c.DepositDate.Date, c.BundleId } into c2
                            select new { StatDate = c2.Key.Date, BundleId = c2.Key.BundleId, BundleCount = c2.Count() };

                foreach (var row in query)
                {
                    creditsSales[row.StatDate].Add((int)row.BundleId, row.BundleCount);
                }

                return creditsSales;
            }
        }

        #endregion

        #region Daily Average Revenue Per User

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();
            result = GetDailyAverageRevenuePerUser(fromDate, toDate, null);
            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType? referrerId)
        {
            // DAU
            Dictionary<DateTime, int> dailyActiveUsers;
            if (referrerId == null)
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate);
            else
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, referrerId.GetValueOrDefault());


            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue;

            if (referrerId == null)
                dailyRevenue = GetDailyRevenue(fromDate, toDate);
            else
                dailyRevenue = GetDailyRevenue(fromDate, toDate, (ReferrerPartnerType)referrerId);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int dau = 0;
                decimal dailyRev = 0;

                dailyActiveUsers.TryGetValue(day, out dau);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (dau > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)dau));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }

            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType? referrerId, int regionId)
        {
            // DAU
            Dictionary<DateTime, int> dailyActiveUsers;

            if (referrerId == null)
            {
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, regionId);
            }
            else
            {
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, referrerId.GetValueOrDefault(), regionId);
            }

            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue;

            if (referrerId == null)
            {
                dailyRevenue = GetDailyRevenue(fromDate, toDate, regionId);
            }
            else
            {
                dailyRevenue = GetDailyRevenue(fromDate, toDate, (ReferrerPartnerType)referrerId, regionId);
            }

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int dau = 0;
                decimal dailyRev = 0;

                dailyActiveUsers.TryGetValue(day, out dau);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (dau > 0)
                {
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)dau));
                }
                else
                {
                    result.Add(day, new DecimalPair(0, 0));
                }
            }

            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            // DAU
            Dictionary<DateTime, int> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, channelId);

            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue = GetDailyRevenue(fromDate, toDate, channelId);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int dau = 0;
                decimal dailyRev = 0;

                dailyActiveUsers.TryGetValue(day, out dau);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (dau > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)dau));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }

            return result;
        }

        /// <summary>
        /// Get the DARPU per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> GetDailyAverageRevenuePerUserByChannels(DateTime fromDate, DateTime toDate)
        {
            // DAU
            Dictionary<ChannelType, Dictionary<DateTime, int>> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsersByChannels(fromDate, toDate);

            // Get the Total daily rev for the day
            Dictionary<ChannelType, Dictionary<DateTime, decimal>> dailyRevenue = AdminCache.LoadDailyRevenueByChannels(fromDate, toDate);

            Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> result = new Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (ChannelType channel in dailyActiveUsers.Keys)
            {
                Dictionary<DateTime, DecimalPair> timeRange = new Dictionary<DateTime, DecimalPair>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    timeRange.Add(day, new DecimalPair(0, 0));
                }

                result.Add(channel, timeRange);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int dau = 0;
                    decimal dailyRev = 0;

                    dailyActiveUsers[channel].TryGetValue(day, out dau);

                    if (dailyRevenue.ContainsKey(channel))
                    {
                        dailyRevenue[channel].TryGetValue(day, out dailyRev);
                    }

                    if (dau > 0)
                    {
                        result[channel][day] = new DecimalPair(dailyRev, dailyRev / (decimal)dau);
                    }
                }
            }

            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerUserPerCountry(DateTime fromDate, DateTime toDate, int regionId)
        {
            // DAU
            Dictionary<DateTime, int> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, regionId);

            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue = GetDailyRevenueByCountry(fromDate, toDate, regionId);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int dau = 0;
                decimal dailyRev = 0;

                dailyActiveUsers.TryGetValue(day, out dau);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (dau > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)dau));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }
            return result;
        }

        #endregion

        #region Daily Average Revenue Per Paying User

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate)
        {
            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue = GetDailyRevenue(fromDate, toDate);
            Dictionary<DateTime, IntPair> dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the DAU to get the DARPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                IntPair transactions = new IntPair(0, 0);
                decimal dailyRev = 0;

                dailyTransactions.TryGetValue(day, out transactions);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (transactions.Int1 > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)transactions.Int1));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }

            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType? referrerId)
        {
            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue;
            Dictionary<DateTime, IntPair> dailyTransactions = GetDailyTransactions(fromDate, toDate, referrerId);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();


            if (referrerId == null)
                dailyRevenue = GetDailyRevenue(fromDate, toDate);
            else
                dailyRevenue = GetDailyRevenue(fromDate, toDate, (ReferrerPartnerType)referrerId);



            // Divide the Total Daily Rev by the (unique?) transactions to get the DARPPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                IntPair transactions = new IntPair(0, 0);
                decimal dailyRev = 0;

                dailyTransactions.TryGetValue(day, out transactions);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (transactions.Int1 > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)transactions.Int1));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }

            return result;
        }

        public static Dictionary<DateTime, DecimalPair> GetDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            // Get the Total daily rev for the day
            Dictionary<DateTime, decimal> dailyRevenue = GetDailyRevenue(fromDate, toDate, channelId);
            Dictionary<DateTime, IntPair> dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate, channelId);

            Dictionary<DateTime, DecimalPair> result = new Dictionary<DateTime, DecimalPair>();

            // Divide the Total Daily Rev by the (unique?) transactions to get the DARPPU
            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                IntPair transactions = new IntPair(0, 0);
                decimal dailyRev = 0;

                dailyTransactions.TryGetValue(day, out transactions);
                dailyRevenue.TryGetValue(day, out dailyRev);

                if (transactions.Int1 > 0)
                    result.Add(day, new DecimalPair(dailyRev, dailyRev / (decimal)transactions.Int1));
                else
                    result.Add(day, new DecimalPair(0, 0));
            }

            return result;
        }

        /// <summary>
        /// Get the DARPPU per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> GetDailyAverageRevenuePerPayingUserByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, decimal>> dailyRevenue = AdminCache.LoadDailyRevenueByChannels(fromDate, toDate);
            Dictionary<ChannelType, Dictionary<DateTime, IntPair>> dailyTransactions = RevenueBusiness.GetDailyTransactionsByChannels(fromDate, toDate);

            Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> result = new Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>();

            // Divide the Total Daily Rev by the (unique?) transactions to get the DARPPU
            foreach (ChannelType channel in dailyRevenue.Keys)
            {
                Dictionary<DateTime, DecimalPair> timeRange = new Dictionary<DateTime, DecimalPair>();

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    timeRange.Add(day, new DecimalPair(0, 0));
                }

                result.Add(channel, timeRange);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    IntPair transactions = new IntPair(0, 0);
                    decimal dailyRev = 0;

                    dailyRevenue[channel].TryGetValue(day, out dailyRev);
                    dailyTransactions[channel].TryGetValue(day, out transactions);

                    if (transactions.Int1 > 0)
                    {
                        result[channel][day] = new DecimalPair(dailyRev, dailyRev / (decimal)transactions.Int1);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Daily Revenue

        public static Dictionary<DateTime, decimal> GetDailyRevenue(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   select new { m.DepositsDate, m.CashDeposited };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.DepositsDate))
                        temp.Add(row.DepositsDate, 0);

                    temp[row.DepositsDate] += row.CashDeposited;
                }

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    decimal i = 0;
                    if (temp.TryGetValue(day, out i))
                        result.Add(day, i);
                    else
                        result.Add(day, 0);
                }
            }

            return result;
        }

        public static Dictionary<DateTime, decimal> GetDailyRevenue(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.Region == regionId
                   select new { m.DepositsDate, m.CashDeposited };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.DepositsDate))
                        temp.Add(row.DepositsDate, 0);

                    temp[row.DepositsDate] += row.CashDeposited;
                }

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    decimal i = 0;
                    if (temp.TryGetValue(day, out i))
                        result.Add(day, i);
                    else
                        result.Add(day, 0);
                }
            }

            return result;
        }

        public static Dictionary<DateTime, decimal> GetDailyRevenue(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ChannelId == (int)channelId
                   select new { m.DepositsDate, m.CashDeposited };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.DepositsDate))
                        temp.Add(row.DepositsDate, 0);

                    temp[row.DepositsDate] += row.CashDeposited;
                }

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    decimal i = 0;
                    if (temp.TryGetValue(day, out i))
                        result.Add(day, i);
                    else
                        result.Add(day, 0);
                }
            }

            return result;
        }

        /// <summary>
        /// Get daily revenue per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, decimal>> GetDailyRevenueByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, decimal>> result = new Dictionary<ChannelType, Dictionary<DateTime, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   group m by new { m.ChannelId, m.DepositsDate } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, m2.Key.DepositsDate, CashDeposited = m2.Sum(m => m.CashDeposited) };

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, decimal> timeRange = new Dictionary<DateTime, decimal>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.DepositsDate] = row.CashDeposited;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the revenue by referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> GetDailyRevenue(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId
                   select new { m.DepositsDate, m.CreditsDeposited, m.CashDeposited };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group the accounts together
                foreach (var row in query)
                {
                    DateTime day = row.DepositsDate.ToDateOnly();

                    decimal amountDeposited = row.CashDeposited;

                    // If we have this day already increment, otherwise create an entry
                    if (result.ContainsKey(day))
                        result[day] += amountDeposited;
                    else
                        result.Add(day, amountDeposited);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the revenue by referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> GetDailyRevenue(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, int regionId)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId && m.Region == regionId
                   select new { m.DepositsDate, m.CreditsDeposited, m.CashDeposited };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group the accounts together
                foreach (var row in query)
                {
                    DateTime day = row.DepositsDate.ToDateOnly();

                    decimal amountDeposited = row.CashDeposited;

                    // If we have this day already increment, otherwise create an entry
                    if (result.ContainsKey(day))
                        result[day] += amountDeposited;
                    else
                        result.Add(day, amountDeposited);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="paymentProviderId">PaymentPartnerType</param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> GetDailyRevenueByPaymentProvider(DateTime fromDate, DateTime toDate, PaymentProviderType paymentProviderId)
        {
            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.PaymentProviderId == (int)paymentProviderId
                   select new { m.DepositsDate, m.CashDeposited };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.DepositsDate))
                    {
                        temp.Add(row.DepositsDate, 0);
                    }

                    temp[row.DepositsDate] += row.CashDeposited;
                }

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    decimal i = 0;

                    if (temp.TryGetValue(day, out i))
                    {
                        result.Add(day, i);
                    }
                    else
                    {
                        result.Add(day, 0);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>> GetDailyRevenueByPaymentProviders(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>> result = new Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   group m by new { m.PaymentProviderId, m.DepositsDate } into m2
                   select new { PaymentProviderId = (PaymentProviderType) m2.Key.PaymentProviderId, m2.Key.DepositsDate, CashDeposited = m2.Sum(m => m.CashDeposited) };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.PaymentProviderId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, decimal> timeRange = new Dictionary<DateTime, decimal>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.PaymentProviderId, timeRange);
                    }

                    result[row.PaymentProviderId][row.DepositsDate] = row.CashDeposited;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily revenue for a specific region
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> GetDailyRevenueByCountry(DateTime fromDate, DateTime toDate, int countryId)
        {
            Dictionary<DateTime, decimal> dailyRevenue = new Dictionary<DateTime, decimal>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.Region == countryId
                   select new { m.DepositsDate, m.CashDeposited };

                Dictionary<DateTime, decimal> temp = new Dictionary<DateTime, decimal>();

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.DepositsDate))
                        temp.Add(row.DepositsDate, 0);

                    temp[row.DepositsDate] += row.CashDeposited;
                }

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    decimal i = 0;
                    if (temp.TryGetValue(day, out i))
                        dailyRevenue.Add(day, i);
                    else
                        dailyRevenue.Add(day, 0);
                }
            }

            return dailyRevenue;
        }

        #endregion

        #region Daily Transactions

        /// <returns>DateTime, Unique Transactions, Total Transactions</returns>
        public static Dictionary<DateTime, IntPair> GetDailyTransactions(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   group m by m.DepositsDate into m2
                   select new
                   {
                       StatDate = m2.Key,
                       TotalTransactions = m2.Sum(t => t.TotalTransactions),
                       UniqueTransactions = m2.Sum(t => t.UniqueTransactions)
                   };

                Dictionary<DateTime, int> tempUnique = query.ToDictionary(t => t.StatDate, t => t.UniqueTransactions);
                Dictionary<DateTime, int> tempTotal = query.ToDictionary(t => t.StatDate, t => t.TotalTransactions);

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int unique;
                    int total;
                    tempUnique.TryGetValue(day, out unique);
                    tempTotal.TryGetValue(day, out total);

                    result.Add(day, new IntPair(unique, total));
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily transactions for a referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> GetDailyTransactions(DateTime fromDate, DateTime toDate, ReferrerPartnerType? referrerId)
        {
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                if (!result.ContainsKey(day))
                {
                    result.Add(day, new IntPair(0, 0));
                }
            }

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId
                   group m by m.DepositsDate into m2
                   select new { DepositsDate = m2.Key, UniqueTransactions = m2.Sum(m => m.UniqueTransactions), TotalTransactions = m2.Sum(m => m.TotalTransactions) };

                Dictionary<DateTime, int> tempU = query.ToDictionary(t => t.DepositsDate, t => t.UniqueTransactions);
                Dictionary<DateTime, int> tempT = query.ToDictionary(t => t.DepositsDate, t => t.TotalTransactions);

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int u;
                    int t;
                    tempU.TryGetValue(day, out u);
                    tempT.TryGetValue(day, out t);

                    if (result.ContainsKey(day))
                        result.Remove(day);
                    result.Add(day, new IntPair(u, t));
                }
            }

            return result;
        }

        public static Dictionary<DateTime, IntPair> GetDailyTransactions(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                if (!result.ContainsKey(day))
                {
                    result.Add(day, new IntPair(0, 0));
                }
            }

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.ChannelId == (int)channelId
                   select new { m.DepositsDate, m.UniqueTransactions, m.TotalTransactions };


                foreach (var row in query)
                {
                    int u = 0;
                    int t = 0;

                    if (result.ContainsKey(row.DepositsDate))
                    {
                        u = result[row.DepositsDate].Int1;
                        t = result[row.DepositsDate].Int2;
                        result.Remove(row.DepositsDate);
                    }

                    result.Add(row.DepositsDate, new IntPair(row.UniqueTransactions + u, row.TotalTransactions + t));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, IntPair>> GetDailyTransactionsByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, IntPair>> result = new Dictionary<ChannelType, Dictionary<DateTime, IntPair>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly()
                   group m by new { m.DepositsDate, m.ChannelId } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, m2.Key.DepositsDate, UniqueTransactions = m2.Sum(m => m.UniqueTransactions), TotalTransactions = m2.Sum(m => m.TotalTransactions) };

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, IntPair> timeRange = new Dictionary<DateTime, IntPair>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, new IntPair(0, 0));
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.DepositsDate] = new IntPair(row.UniqueTransactions, row.TotalTransactions);
                }
            }

            return result;
        }

        public static Dictionary<DateTime, IntPair> GetDailyTransactions(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                if (!result.ContainsKey(day))
                {
                    result.Add(day, new IntPair(0, 0));
                }
            }

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate.ToDateOnly() && m.DepositsDate <= toDate.ToDateOnly() && m.PaymentProviderId == paymentProviderId
                   select new { m.DepositsDate, m.UniqueTransactions, m.TotalTransactions };

                foreach (var row in query)
                {
                    int u = 0;
                    int t = 0;

                    if (result.ContainsKey(row.DepositsDate))
                    {
                        u = result[row.DepositsDate].Int1;
                        t = result[row.DepositsDate].Int2;
                        result.Remove(row.DepositsDate);
                    }

                    result.Add(row.DepositsDate, new IntPair(row.UniqueTransactions + u, row.TotalTransactions + t));
                }
            }

            return result;
        }

        #endregion

        #region Daily conversion to paying

        public static Dictionary<DateTime, FloatPair> GetDailyConversionToPaying(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate);
            Dictionary<DateTime, IntPair> dailyTransactions = GetDailyTransactions(fromDate, toDate);
            Dictionary<DateTime, FloatPair> result = new Dictionary<DateTime, FloatPair>();

            // Divide the Daily Unique Transactions by the DAU
            foreach (KeyValuePair<DateTime, int> kvp in dailyActiveUsers)
            {
                // Try to find the days transactions in for the same day in dau
                IntPair outTrans = new IntPair(0, 0);

                if (dailyTransactions.TryGetValue(kvp.Key, out outTrans))
                {
                    // If we had 0 DAU, don't divide by zero
                    if (kvp.Value > 0)
                    {
                        result.Add(kvp.Key, new FloatPair((float)outTrans.Int1 / kvp.Value, (float)outTrans.Int2 / kvp.Value));
                    }
                    else
                    {
                        result.Add(kvp.Key, new FloatPair(0.0f, 0.0f));
                    }
                }
                else
                {
                    result.Add(kvp.Key, new FloatPair(0.0f, 0.0f));
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily conversion to paying per referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, FloatPair> GetDailyConversionToPaying(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, int> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, referrerId);
            Dictionary<DateTime, IntPair> dailyTransactions = GetDailyTransactions(fromDate, toDate, referrerId);
            Dictionary<DateTime, FloatPair> result = new Dictionary<DateTime, FloatPair>();

            // Divide the Daily Unique Transactions by the DAU
            foreach (KeyValuePair<DateTime, int> kvp in dailyActiveUsers)
            {
                // Try to find the days transactions in for the same day in dau
                IntPair outTrans = new IntPair(0, 0);
                if (dailyTransactions.TryGetValue(kvp.Key, out outTrans))
                {
                    // If we had 0 DAU, don't divide by zero
                    if (kvp.Value > 0)
                        result.Add(kvp.Key, new FloatPair((float)outTrans.Int1 / kvp.Value, (float)outTrans.Int2 / kvp.Value));
                    else
                        result.Add(kvp.Key, new FloatPair(0.0f, 0.0f));
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily conversion to paying per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, FloatPair> GetDailyConversionToPaying(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, channel);
            Dictionary<DateTime, IntPair> dailyTransactions = GetDailyTransactions(fromDate, toDate, channel);
            Dictionary<DateTime, FloatPair> result = new Dictionary<DateTime, FloatPair>();

            // Divide the Daily Unique Transactions by the DAU
            foreach (KeyValuePair<DateTime, int> kvp in dailyActiveUsers)
            {
                // Try to find the days transactions in for the same day in dau
                IntPair outTrans = new IntPair(0, 0);
                if (dailyTransactions.TryGetValue(kvp.Key, out outTrans))
                {
                    // If we had 0 DAU, don't divide by zero
                    if (kvp.Value > 0)
                        result.Add(kvp.Key, new FloatPair((float)outTrans.Int1 / kvp.Value, (float)outTrans.Int2 / kvp.Value));
                    else
                        result.Add(kvp.Key, new FloatPair(0.0f, 0.0f));
                }
            }

            return result;
        }

        #endregion

        #region Countries

        /// <summary>
        /// Get the revenue per country
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> GetRevenueByCountries(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, decimal> countriesRevenue = new Dictionary<int, decimal>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from cD in instrumentationDb.ChannelCreditDeposits
                            where cD.DepositsDate >= fromDate && cD.DepositsDate < toDate
                            group cD by cD.Region into cD2
                            select new
                            {
                                Region = cD2.Key,
                                Revenue = cD2.Sum(t => t.CashDeposited)
                            };

                countriesRevenue = query.ToDictionary(t => t.Region, t => t.Revenue);
            }

            return countriesRevenue;
        }

        #endregion

        public static List<CurrencyDepositView> GetLatestDeposits(int paymentProviderId, int paymentsCount)
        {
            List<CurrencyDepositView> depositsView = new List<CurrencyDepositView>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                List<CreditDeposit> deposits = db.CreditDeposits.Where(c => c.PartnerID == paymentProviderId).OrderByDescending(c => c.id).Take(paymentsCount).ToList();
                depositsView = deposits.ConvertAll(new Converter<CreditDeposit, CurrencyDepositView>(c => new CurrencyDepositView(
                                        c.id,
                                        c.DepositDate,
                                        c.NbCredit,
                                        0,
                                        c.NbCash,
                                        c.CurrencyLabel,
                                        c.UserId,
                                        c.isAdminAction,
                                        (PaymentProviderType)c.PartnerID,
                                        c.TransactionKey,
                                        c.ApplicationID,
                                        (ChannelType)c.ChannelID,
                                        c.UsdAmount,
                                        c.BundleId,
                                        String.Empty)));
            }

            return depositsView;
        }

        public static CurrencyDepositView GetCreditDeposit(int paymentProviderId, string transactionKey)
        {
            CurrencyDepositView depositView = null;

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                transactionKey = transactionKey.Trim();

                CreditDeposit deposit = db.CreditDeposits.SingleOrDefault(c => c.PartnerID == paymentProviderId && c.TransactionKey == transactionKey);

                if (deposit != null)
                {
                    depositView = new CurrencyDepositView(
                                            deposit.id,
                                            deposit.DepositDate,
                                            deposit.NbCredit,
                                            0,
                                            deposit.NbCash,
                                            deposit.CurrencyLabel,
                                            deposit.UserId,
                                            deposit.isAdminAction,
                                            (PaymentProviderType)deposit.PartnerID,
                                            deposit.TransactionKey,
                                            deposit.ApplicationID,
                                            (ChannelType)deposit.ChannelID,
                                            deposit.UsdAmount,
                                            deposit.BundleId,
                                            String.Empty);
                }
            }

            return depositView;
        }

        public static decimal GetTotalRevenue(DateTime fromDate, DateTime toDate)
        {
            decimal totalRevenue = 0;

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate && m.DepositsDate <= toDate
                   select new { CashDeposited = m.CashDeposited };

                totalRevenue = query.ToList().Sum(m => m.CashDeposited);
            }

            return totalRevenue;
        }

        public static decimal GetTotalRevenue(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            decimal totalRevenue = 0;

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelCreditDeposits
                   where m.DepositsDate >= fromDate && m.DepositsDate <= toDate && m.PaymentProviderId == paymentProviderId
                   select new { CashDeposited = m.CashDeposited };

                totalRevenue = query.ToList().Sum(m => m.CashDeposited);
            }

            return totalRevenue;
        }

        public static Dictionary<DateTime, int> GetAveragePointBalance(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> averagePointsBalance = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.AveragePointsBalance };

                Dictionary<DateTime, int> averagePointsBalanceResult = query.ToDictionary(u => u.StatDate, u => u.AveragePointsBalance);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int pointsBalance = 0;

                    averagePointsBalanceResult.TryGetValue(day, out pointsBalance);
                    averagePointsBalance.Add(day, pointsBalance);
                }
            }

            return averagePointsBalance;
        }

        public static Dictionary<DateTime, int> GetAveragePointsEarned(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> averagePointsEarned = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.AveragePointsEarned };

                Dictionary<DateTime, int> averagePointsEarnedResult = query.ToDictionary(u => u.StatDate, u => u.AveragePointsEarned);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int pointsEarned = 0;

                    averagePointsEarnedResult.TryGetValue(day, out pointsEarned);
                    averagePointsEarned.Add(day, pointsEarned);
                }
            }

            return averagePointsEarned;
        }

        public static Dictionary<DateTime, int> GetAveragePointsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> averagePointsSpent = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.AveragePointsSpent };

                Dictionary<DateTime, int> averagePointsSpentResult = query.ToDictionary(u => u.StatDate, u => u.AveragePointsSpent);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int pointsSpent = 0;

                    averagePointsSpentResult.TryGetValue(day, out pointsSpent);
                    averagePointsSpent.Add(day, pointsSpent);
                }
            }

            return averagePointsSpent;
        }

        public static Dictionary<DateTime, int> GetMedianPointsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> medianPointsSpent = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.MedianPointsSpent };

                Dictionary<DateTime, int> medianPointsSpentResult = query.ToDictionary(u => u.StatDate, u => u.MedianPointsSpent);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int pointsSpent = 0;

                    medianPointsSpentResult.TryGetValue(day, out pointsSpent);
                    medianPointsSpent.Add(day, pointsSpent);
                }
            }

            return medianPointsSpent;
        }

        public static Dictionary<DateTime, int> GetAverageCreditsBalance(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> averageCreditsBalance = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.AverageCreditsBalance };

                Dictionary<DateTime, int> averageCreditsBalanceResult = query.ToDictionary(u => u.StatDate, u => u.AverageCreditsBalance);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int creditsBalance = 0;

                    averageCreditsBalanceResult.TryGetValue(day, out creditsBalance);
                    averageCreditsBalance.Add(day, creditsBalance);
                }
            }

            return averageCreditsBalance;
        }

        public static Dictionary<DateTime, int> GetAverageCreditsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> averageCreditsSpent = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.AverageCreditsSpent };

                Dictionary<DateTime, int> averageCreditsSpentResult = query.ToDictionary(u => u.StatDate, u => u.AverageCreditsSpent);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int creditsSpent = 0;

                    averageCreditsSpentResult.TryGetValue(day, out creditsSpent);
                    averageCreditsSpent.Add(day, creditsSpent);
                }
            }

            return averageCreditsSpent;
        }

        public static Dictionary<DateTime, int> GetMedianCreditsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> medianCreditsSpent = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerEconomies
                            where p.StatDate >= fromDate && p.StatDate <= toDate
                            select new { p.StatDate, p.MedianCreditsSpent };

                Dictionary<DateTime, int> medianCreditsSpentResult = query.ToDictionary(u => u.StatDate, u => u.MedianCreditsSpent);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int creditsSpent = 0;

                    medianCreditsSpentResult.TryGetValue(day, out creditsSpent);
                    medianCreditsSpent.Add(day, creditsSpent);
                }
            }

            return medianCreditsSpent;
        }

        public static Dictionary<PointsDepositType, long> GetPointDepositsDistribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PointsDepositType, long> pointsDepositDistribution = new Dictionary<PointsDepositType, long>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PointsEarnedDistributions
                            where p.StatDate >= fromDate && p.StatDate <= toDate && (p.DepositType == (int)PointsDepositType.Game || p.DepositType == (int)PointsDepositType.Login)
                            group p by p.DepositType into p2
                            select new { DepositType = p2.Key, Points = p2.Sum(p => (long)p.Points) };

                pointsDepositDistribution = query.ToDictionary(u => (PointsDepositType)u.DepositType, u => u.Points);
            }

            return pointsDepositDistribution;
        }
    }
}