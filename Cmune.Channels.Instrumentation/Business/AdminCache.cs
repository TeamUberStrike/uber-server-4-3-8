using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.DataAccess;
using System.Web.Caching;
using System.Configuration;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using System.Web.UI.WebControls;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Business;
using System.Web.Mvc;
using Cmune.Instrumentation.Monitoring.Business;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Business;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business
{
    /// <summary>
    /// Manages the cache
    /// </summary>
    class AdminCache
    {
        /// <summary>
        /// Loads the application milestones
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, string> LoadApplicationMilestones(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, string> applicationMilestones = new Dictionary<DateTime, string>();

            string cacheName = "Statistics.GetApplicationMilestones." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                applicationMilestones = (Dictionary<DateTime, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Grab the App Milestones so we can compare important dates and data changes
                applicationMilestones = StatisticsBusiness.GetApplicationMilestones(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, applicationMilestones, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return applicationMilestones;
        }

        /// <summary>
        /// Loads all the countries name
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> LoadCountriesName()
        {
            Dictionary<int, string> countriesName = new Dictionary<int, string>();

            string cacheName = "CmuneMember.GetAllCountriesName";

            if (HttpRuntime.Cache[cacheName] != null)
            {
                countriesName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                countriesName = CmuneMember.GetAllCountriesName();
                countriesName.Add(0, "Undetermined");
                HttpRuntime.Cache.Add(cacheName, countriesName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return countriesName;
        }

        /// <summary>
        /// Loads all the items name for a specific application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="getDeprecatedItems"></param>
        /// <returns></returns>
        public static Dictionary<int, string> LoadItemNames(int applicationId, bool getDeprecatedItems)
        {
            Dictionary<int, string> itemsName = new Dictionary<int, string>();

            string cacheName = String.Format("CmuneItem.GetItemNames.{0}.{1}", applicationId, getDeprecatedItems);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                itemsName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                itemsName = CmuneItem.GetItemNames(applicationId, getDeprecatedItems);
                HttpRuntime.Cache.Add(cacheName, itemsName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return itemsName;
        }

        public static ItemView LoadItemView(int itemId)
        {
            ItemView itemView = null;

            string cacheName = String.Format("CmuneItem.GetItemView.{0}", itemId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                itemView = (ItemView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                itemView = CmuneItem.GetItemView(itemId);
                HttpRuntime.Cache.Add(cacheName, itemView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return itemView;
        }

        #region Revenue

        /// <summary>
        /// Loads the revenue per country
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> LoadRevenueByCountries(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, decimal> countriesByRevenue = new Dictionary<int, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetRevenueByCountries.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                countriesByRevenue = (Dictionary<int, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                countriesByRevenue = RevenueBusiness.GetRevenueByCountries(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, countriesByRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return countriesByRevenue;
        }

        /// <summary>
        /// Loads the revenue per country (the order is by revenue not country name)
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> LoadRevenueByCountriesOrdered(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, decimal> countriesByRevenue = LoadRevenueByCountries(fromDate, toDate);
            Dictionary<int, decimal> countriesByRevenueOrderded = countriesByRevenue.OrderByDescending(t => t.Value).ToDictionary(x => x.Key, x => x.Value);

            return countriesByRevenueOrderded;
        }

        public static decimal LoadTotalRevenue(DateTime fromDate, DateTime toDate)
        {
            decimal totalRevenue = 0;

            string cacheName = String.Format("RevenueBusiness.GetTotalRevenue.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                totalRevenue = (decimal)HttpRuntime.Cache[cacheName];
            }
            else
            {
                totalRevenue = RevenueBusiness.GetTotalRevenue(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, totalRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return totalRevenue;
        }

        public static decimal LoadTotalRevenue(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            decimal totalRevenue = 0;

            string cacheName = String.Format("RevenueBusiness.GetTotalRevenue.{0}.{1}.{2}", fromDate, toDate, paymentProviderId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                totalRevenue = (decimal)HttpRuntime.Cache[cacheName];
            }
            else
            {
                totalRevenue = RevenueBusiness.GetTotalRevenue(fromDate, toDate, paymentProviderId);
                HttpRuntime.Cache.Add(cacheName, totalRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return totalRevenue;
        }

        #endregion

        #region Daily Active Users

        /// <summary>
        /// Loads the daily active users
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> dailyActiveUsers = new Dictionary<DateTime, int>();

            string cacheName = "UserActivity.GetDailyActiveUsers." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Grab the DAU
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsers;
        }

        /// <summary>
        /// Loads the daily active users by referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, int> dailyActiveUsers = new Dictionary<DateTime, int>();

            string cacheName = "UserActivity.GetDailyActiveUsers.referrer." + (int)referrerId + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Grab the DAU
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsers;
        }

        /// <summary>
        /// Loads the daily active users by referrer and region
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, int regionId)
        {
            Dictionary<DateTime, int> dailyActiveUsers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivity.GetDailyActiveUsers.referrer.region.{0}.{1}.{2}.{3}", fromDate.ToString(), toDate.ToString(), referrerId, regionId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsers = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, referrerId, regionId);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsers;
        }

        /// <summary>
        /// Loads the daily active users by channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> dailyActiveUsersByChannel = new Dictionary<DateTime, int>();

            string cacheName = "UserActivityBusiness.GetDailyActiveUsers.channel." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByChannel = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Grab the DAU by Channel
                dailyActiveUsersByChannel = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByChannel;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> LoadDailyActiveUsersByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> dailyActiveUsersByChannel = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            string cacheName = String.Format("UserActivityBusiness.GetDailyActiveUsersByChannels.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByChannel = (Dictionary<ChannelType, Dictionary<DateTime, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByChannel = UserActivityBusiness.GetDailyActiveUsersByChannels(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByChannel;
        }

        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> dailyActiveUsersByRegion = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetDailyActiveUsers.region.{0}.{1}.{2}", regionId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByRegion = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByRegion = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, regionId);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByRegion, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByRegion;
        }

        public static Dictionary<DateTime, int> LoadDailyActiveUsers(DateTime fromDate, DateTime toDate, int regionId, ChannelType channel)
        {
            Dictionary<DateTime, int> dailyActiveUsersByRegionAndChannel = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetDailyActiveUsers.region.channel.{0}.{1}.{2}.{3}", regionId, channel, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByRegionAndChannel = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByRegionAndChannel = UserActivityBusiness.GetDailyActiveUsers(fromDate, toDate, regionId, channel);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByRegionAndChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByRegionAndChannel;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> LoadDailyActiveUsersByChannels(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> dailyActiveUsersByRegionAndChannel = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            string cacheName = String.Format("UserActivityBusiness.GetDailyActiveUsersByChannels.region.{0}.{1}.{2}", regionId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByRegionAndChannel = (Dictionary<ChannelType, Dictionary<DateTime, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByRegionAndChannel = UserActivityBusiness.GetDailyActiveUsersByChannels(fromDate, toDate, regionId);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByRegionAndChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByRegionAndChannel;
        }

        #endregion

        #region Monthly Active Users

        /// <summary>
        /// Loads the monthly active users
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadMonthlyActiveUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> monthlyActiveUsers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsers.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsers = UserActivityBusiness.GetMonthlyActiveUsers(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsers;
        }

        /// <summary>
        /// Loads the monthly active users by referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadMonthlyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, int> monthlyActiveUsers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivity.GetMonthlyActiveUsers.referrer.{0}.{1}.{2}", (int)referrerId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Grab the MAU
                monthlyActiveUsers = UserActivityBusiness.GetMonthlyActiveUsers(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsers;
        }

        /// <summary>
        /// Loads the monthly active users by channels
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadMonthlyActiveUsers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> monthlyActiveUsersByChannel = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsers.channel.{0}.{1}.{2}", channel, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsersByChannel = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsersByChannel = UserActivityBusiness.GetMonthlyActiveUsers(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsersByChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsersByChannel;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> LoadMonthlyActiveUsersByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> monthlyActiveUsersByChannels = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsersByChannels.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsersByChannels = (Dictionary<ChannelType, Dictionary<DateTime, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsersByChannels = UserActivityBusiness.GetMonthlyActiveUsersByChannels(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsersByChannels, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsersByChannels;
        }

        public static Dictionary<DateTime, int> LoadMonthlyActiveUsers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> monthlyActiveUsersByRegion = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsers.region.{0}.{1}.{2}", regionId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsersByRegion = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsersByRegion = UserActivityBusiness.GetMonthlyActiveUsers(fromDate, toDate, regionId);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsersByRegion, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsersByRegion;
        }

        public static Dictionary<DateTime, int> LoadMonthlyActiveUsers(DateTime fromDate, DateTime toDate, int regionId, ChannelType channel)
        {
            Dictionary<DateTime, int> monthlyActiveUsersByRegionAndChannel = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsers.region.channel.{0}.{1}.{2}.{3}", regionId, channel, fromDate, toDate.ToString());

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsersByRegionAndChannel = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsersByRegionAndChannel = UserActivityBusiness.GetMonthlyActiveUsers(fromDate, toDate, regionId, channel);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsersByRegionAndChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsersByRegionAndChannel;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> LoadMonthlyActiveUsersByChannels(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> monthlyActiveUsersByRegionAndChannel = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsersByChannels.region.{0}.{1}.{2}", regionId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                monthlyActiveUsersByRegionAndChannel = (Dictionary<ChannelType, Dictionary<DateTime, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                monthlyActiveUsersByRegionAndChannel = UserActivityBusiness.GetMonthlyActiveUsersByChannels(fromDate, toDate, regionId);
                HttpRuntime.Cache.Add(cacheName, monthlyActiveUsersByRegionAndChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return monthlyActiveUsersByRegionAndChannel;
        }

        public static Dictionary<int, int> LoadMonthlyActiveUsersByCountries(DateTime toDate)
        {
            Dictionary<int, int> dailyActiveUsersByCountries = new Dictionary<int, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsersByCountries.{0}", toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByCountries = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByCountries = UserActivityBusiness.GetMonthlyActiveUsersByCountries(toDate);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByCountries, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByCountries;
        }

        public static Dictionary<int, int> LoadMonthlyActiveUsersByCountriesOrdered(DateTime toDate)
        {
            Dictionary<int, int> countriesByMau = LoadMonthlyActiveUsersByCountries(toDate);
            Dictionary<int, int> countriesByMauOrderded = countriesByMau.OrderByDescending(t => t.Value).ToDictionary(x => x.Key, x => x.Value);

            return countriesByMauOrderded;
        }

        public static Dictionary<int, int> LoadMonthlyActiveUsersByCountries(DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<int, int> dailyActiveUsersByCountries = new Dictionary<int, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMonthlyActiveUsersByCountries.Referrer.{0}.{1}", toDate, referrer);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyActiveUsersByCountries = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyActiveUsersByCountries = UserActivityBusiness.GetMonthlyActiveUsersByCountries(toDate, referrer);
                HttpRuntime.Cache.Add(cacheName, dailyActiveUsersByCountries, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyActiveUsersByCountries;
        }

        public static Dictionary<int, int> LoadMonthlyActiveUsersByCountriesOrdered(DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<int, int> countriesByMau = LoadMonthlyActiveUsersByCountries(toDate, referrer);
            Dictionary<int, int> countriesByMauOrderded = countriesByMau.OrderByDescending(t => t.Value).ToDictionary(x => x.Key, x => x.Value);

            return countriesByMauOrderded;
        }

        #endregion

        #region Daily play rate

        /// <summary>
        /// Loads the daily play rate
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, float> LoadDailyPlayRate(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, float> dailyPlayRate = new Dictionary<DateTime, float>();

            string cacheName = String.Format("UserActivityBusiness.GetPlayRate.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyPlayRate = (Dictionary<DateTime, float>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                // Calculate Daily Retention
                dailyPlayRate = UserActivityBusiness.GetPlayRate(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyPlayRate, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyPlayRate;
        }

        /// <summary>
        /// Loads the daily play rate
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrer"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, float> LoadDailyPlayRate(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<DateTime, float> dailyPlayRate = new Dictionary<DateTime, float>();

            string cacheName = String.Format("UserActivityBusiness.GetPlayRate.Referrer.{0}.{1}.{2}", (int)referrer, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyPlayRate = (Dictionary<DateTime, float>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyPlayRate = UserActivityBusiness.GetPlayRate(fromDate, toDate, referrer);
                HttpRuntime.Cache.Add(cacheName, dailyPlayRate, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyPlayRate;
        }

        public static Dictionary<DateTime, float> LoadDailyPlayRate(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, float> dailyPlayRate = new Dictionary<DateTime, float>();

            string cacheName = String.Format("UserActivityBusiness.GetPlayRate.Channel.{0}.{1}.{2}", (int)channel, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyPlayRate = (Dictionary<DateTime, float>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyPlayRate = UserActivityBusiness.GetPlayRate(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyPlayRate, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyPlayRate;
        }

        #endregion

        #region Daily revenue

        /// <summary>
        /// Loads the daily revenue
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> LoadDailyRevenue(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, decimal> dailyRevenue = new Dictionary<DateTime, decimal>();

            string cacheName = "RevenueBusiness.GetDailyRevenue." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyRevenue = (Dictionary<DateTime, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyRevenue = RevenueBusiness.GetDailyRevenue(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyRevenue;
        }

        public static Dictionary<DateTime, decimal> LoadDailyRevenue(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, decimal> dailyRevenue = new Dictionary<DateTime, decimal>();

            string cacheName = "RevenueBusiness.GetRevenue." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyRevenue = (Dictionary<DateTime, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyRevenue = RevenueBusiness.GetDailyRevenue(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyRevenue;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, decimal>> LoadDailyRevenueByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, decimal>> dailyRevenue = new Dictionary<ChannelType, Dictionary<DateTime, decimal>>();

            string cacheName = String.Format("RevenueBusiness.GetDailyRevenueByChannels.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyRevenue = (Dictionary<ChannelType, Dictionary<DateTime, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyRevenue = RevenueBusiness.GetDailyRevenueByChannels(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyRevenue;
        }

        /// <summary>
        /// Load the daily revenue for a specific region
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> LoadDailyRevenueByCountry(DateTime fromDate, DateTime toDate, int countryId)
        {
            Dictionary<DateTime, decimal> dailyRevenue = new Dictionary<DateTime, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetDailyRevenueByCountry.{0}.{1}.{2}", countryId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyRevenue = (Dictionary<DateTime, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyRevenue = RevenueBusiness.GetDailyRevenueByCountry(fromDate, toDate, countryId);
                HttpRuntime.Cache.Add(cacheName, dailyRevenue, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyRevenue;
        }

        /// <summary>
        /// Loads the revenue
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> LoadDailyRevenue(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, decimal> revenueByReferrer = new Dictionary<DateTime, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetDailyRevenue.{0}.{1}.{2}", (int)referrerId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                revenueByReferrer = (Dictionary<DateTime, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                revenueByReferrer = RevenueBusiness.GetDailyRevenue(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, revenueByReferrer, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return revenueByReferrer;
        }

        /// <summary>
        /// Loads the revenue per payment provider
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="paymentProviderId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, decimal> LoadDailyRevenueByPaymentProvider(DateTime fromDate, DateTime toDate, PaymentProviderType paymentProviderId)
        {
            Dictionary<DateTime, decimal> revenueByPaymentProvider = new Dictionary<DateTime, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetDailyRevenueByPaymentProvider.{0}.{1}.{2}", paymentProviderId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                revenueByPaymentProvider = (Dictionary<DateTime, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                revenueByPaymentProvider = RevenueBusiness.GetDailyRevenueByPaymentProvider(fromDate, toDate, paymentProviderId);
                HttpRuntime.Cache.Add(cacheName, revenueByPaymentProvider, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return revenueByPaymentProvider;
        }

        /// <summary>
        /// Loads the revenue per payment provider
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>> LoadDailyRevenueByPaymentProviders(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>> revenueByPaymentProvider = new Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>>();

            string cacheName = String.Format("RevenueBusiness.LoadDailyRevenueByPaymentProviders.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                revenueByPaymentProvider = (Dictionary<PaymentProviderType, Dictionary<DateTime, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                revenueByPaymentProvider = RevenueBusiness.GetDailyRevenueByPaymentProviders(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, revenueByPaymentProvider, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return revenueByPaymentProvider;
        }

        #endregion

        #region New members

        public static Dictionary<DateTime, int> LoadNewMembers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> newMembers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembers.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembers(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        public static Dictionary<DateTime, int> LoadNewMembers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<DateTime, int> newMembers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembers.Referrer.{0}.{1}.{2}", referrer, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembers(fromDate, toDate, referrer);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        public static Dictionary<DateTime, int> LoadNewMembers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer, int regionId)
        {
            Dictionary<DateTime, int> newMembers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembers.Referrer.{0}.{1}.{2}.{3}", referrer, fromDate, toDate, regionId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembers(fromDate, toDate, referrer, regionId);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        public static Dictionary<DateTime, int> LoadNewMembers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> newMembers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembers.Channel.{0}.{1}.{2}", channel, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembers(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        public static Dictionary<DateTime, int> LoadNewMembers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> newMembers = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembers.Region.{0}.{1}.{2}", regionId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembers(fromDate, toDate, regionId);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        public static Dictionary<int, int> LoadNewMembersCountByCountries(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<int, int> newMembers = new Dictionary<int, int>();

            string cacheName = String.Format("UserActivityBusiness.GetNewMembersCountByCountries.Referrer.{0}.{1}.{2}", fromDate, toDate, referrer);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newMembers = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newMembers = UserActivityBusiness.GetNewMembersCountByCountries(fromDate, toDate, referrer);
                HttpRuntime.Cache.Add(cacheName, newMembers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newMembers;
        }

        #endregion

        #region New versus returning

        /// <summary>
        /// Loads the new versus returning users
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadNewVsReturningUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, IntPair> newVsReturningUsers = new Dictionary<DateTime, IntPair>();

            string cacheName = "UserActivityBusiness.GetNewVsReturningUsers." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newVsReturningUsers = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newVsReturningUsers = UserActivityBusiness.GetNewVsReturningUsers(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, newVsReturningUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newVsReturningUsers;
        }

        /// <summary>
        /// Loads the new versus returning users for a specific channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadNewVsReturningUsers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, IntPair> newVsReturningUsers = new Dictionary<DateTime, IntPair>();

            string cacheName = "UserActivityBusiness.GetNewVsReturningUsers.Channel." + ((int)channel).ToString() + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                newVsReturningUsers = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                newVsReturningUsers = UserActivityBusiness.GetNewVsReturningUsers(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, newVsReturningUsers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return newVsReturningUsers;
        }

        #endregion

        #region Darpu

        /// <summary>
        /// Loads the daily average revenue per user
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerUser." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUser(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        /// <summary>
        /// Loads the daily average revenue per user per referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerUser.referrer" + (int)referrerId + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUser(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        /// <summary>
        /// Loads the daily average revenue per user per referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, int regionId)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = String.Format("RevenueBusiness.GetDailyAverageRevenuePerUser.referrer.{0}.{1}.{2}.{3}", fromDate, toDate, referrerId, regionId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUser(fromDate, toDate, referrerId, regionId);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        /// <summary>
        /// Loads the daily average revenue per user per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerUser(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerUser.channel." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUser(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        /// <summary>
        /// Loads the daily average revenue per user per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> LoadDailyAverageRevenuePerUserByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> dailyAverageRevenuePerUser = new Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>();

            string cacheName = String.Format("RevenueBusiness.LoadDailyAverageRevenuePerUserByChannels.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUserByChannels(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        /// <summary>
        /// Loads the daily average revenue per user per country
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerUserPerCountry(DateTime fromDate, DateTime toDate, int countryId)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = String.Format("RevenueBusiness.LoadDailyAverageRevenuePerUserPerCountry.{0}.{1}.{2}", countryId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerUser = RevenueBusiness.GetDailyAverageRevenuePerUserPerCountry(fromDate, toDate, countryId);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerUser;
        }

        #endregion

        #region Darppu

        /// <summary>
        /// Loads the daily average revenue per paying user
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerPayingUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerPayingUser." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerPayingUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerPayingUser = RevenueBusiness.GetDailyAverageRevenuePerPayingUser(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerPayingUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerPayingUser;
        }

        /// <summary>
        /// Loads the daily average revenue per paying user per referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerPayingUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerPayingUser.referrer." + (int)referrerId + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerPayingUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerPayingUser = RevenueBusiness.GetDailyAverageRevenuePerPayingUser(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerPayingUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerPayingUser;
        }

        /// <summary>
        /// Loads the daily average revenue per paying user per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, DecimalPair> LoadDailyAverageRevenuePerPayingUser(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, DecimalPair> dailyAverageRevenuePerPayingUser = new Dictionary<DateTime, DecimalPair>();

            string cacheName = "RevenueBusiness.GetDailyAverageRevenuePerPayingUser.channel." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerPayingUser = (Dictionary<DateTime, DecimalPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerPayingUser = RevenueBusiness.GetDailyAverageRevenuePerPayingUser(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerPayingUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerPayingUser;
        }

        /// <summary>
        /// Loads the daily average revenue per paying user per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> LoadDailyAverageRevenuePerPayingUserByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>> dailyAverageRevenuePerPayingUser = new Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>();

            string cacheName = String.Format("RevenueBusiness.LoadDailyAverageRevenuePerPayingUserByChannels.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyAverageRevenuePerPayingUser = (Dictionary<ChannelType, Dictionary<DateTime, DecimalPair>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyAverageRevenuePerPayingUser = RevenueBusiness.GetDailyAverageRevenuePerPayingUserByChannels(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyAverageRevenuePerPayingUser, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyAverageRevenuePerPayingUser;
        }

        #endregion

        #region Daily transactions

        /// <summary>
        /// Loads the daily transaction
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadDailyTransactions(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, IntPair> dailyTransactions = new Dictionary<DateTime, IntPair>();

            string cacheName = "RevenueBusiness.GetDailyTransactions." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyTransactions = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyTransactions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyTransactions;
        }

        /// <summary>
        /// Loads the daily transaction per referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadDailyTransactions(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, IntPair> dailyTransactions = new Dictionary<DateTime, IntPair>();

            string cacheName = "RevenueBusiness.GetDailyTransactions.Referrer." + (int)referrerId + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyTransactions = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, dailyTransactions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyTransactions;
        }

        /// <summary>
        /// Loads the daily transaction per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadDailyTransactions(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, IntPair> dailyTransactions = new Dictionary<DateTime, IntPair>();

            string cacheName = "RevenueBusiness.GetDailyTransactions.Channel." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyTransactions = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyTransactions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyTransactions;
        }

        /// <summary>
        /// Loads the daily transaction per referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="paymenProviderId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, IntPair> LoadDailyTransactions(DateTime fromDate, DateTime toDate, int paymenProviderId)
        {
            Dictionary<DateTime, IntPair> dailyTransactions = new Dictionary<DateTime, IntPair>();

            string cacheName = String.Format("RevenueBusiness.GetDailyTransactions.PaymentProvider.{0}.{1}.{2}", fromDate, toDate, paymenProviderId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyTransactions = (Dictionary<DateTime, IntPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyTransactions = RevenueBusiness.GetDailyTransactions(fromDate, toDate, paymenProviderId);
                HttpRuntime.Cache.Add(cacheName, dailyTransactions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyTransactions;
        }

        #endregion

        #region Daily conversion to paying

        /// <summary>
        /// Loads the daily conversion to paying
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, FloatPair> LoadDailyConversionToPaying(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, FloatPair> dailyConversionToPaying = new Dictionary<DateTime, FloatPair>();

            string cacheName = "RevenueBusiness.GetDailyConversionToPaying." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyConversionToPaying = (Dictionary<DateTime, FloatPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyConversionToPaying = RevenueBusiness.GetDailyConversionToPaying(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, dailyConversionToPaying, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyConversionToPaying;
        }

        /// <summary>
        /// Loads the daily conversion to paying by referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, FloatPair> LoadDailyConversionToPaying(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, FloatPair> dailyConversionToPaying = new Dictionary<DateTime, FloatPair>();

            string cacheName = "RevenueBusiness.GetDailyConversionToPaying.Referrer." + (int)referrerId + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyConversionToPaying = (Dictionary<DateTime, FloatPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyConversionToPaying = RevenueBusiness.GetDailyConversionToPaying(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, dailyConversionToPaying, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyConversionToPaying;
        }

        /// <summary>
        /// Loads the daily conversion to paying by channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, FloatPair> LoadDailyConversionToPaying(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, FloatPair> dailyConversionToPaying = new Dictionary<DateTime, FloatPair>();

            string cacheName = "RevenueBusiness.GetDailyConversionToPaying.Channel." + (int)channel + "." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                dailyConversionToPaying = (Dictionary<DateTime, FloatPair>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                dailyConversionToPaying = RevenueBusiness.GetDailyConversionToPaying(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, dailyConversionToPaying, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return dailyConversionToPaying;
        }

        #endregion

        #region Bundles

        /// <summary>
        /// Loads the package contribution
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="areCreditsBundles"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> LoadPackageContributionByRevenue(DateTime fromDate, DateTime toDate, bool areCreditsBundles)
        {
            Dictionary<int, decimal> packagesContribution = new Dictionary<int, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByRevenue.{0}.{1}.{2}", fromDate, toDate, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByRevenue(fromDate, toDate, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, decimal> LoadPackageContributionByRevenue(DateTime fromDate, DateTime toDate, int paymentProviderId, bool areCreditsBundles)
        {
            Dictionary<int, decimal> packagesContribution = new Dictionary<int, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByRevenue.paymentProvider.{0}.{1}.{2}.{3}", fromDate, toDate, paymentProviderId, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByRevenue(fromDate, toDate, paymentProviderId, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, decimal> LoadPackageContributionByRevenue(DateTime fromDate, DateTime toDate, ChannelType channel, bool areCreditsBundles)
        {
            Dictionary<int, decimal> packagesContribution = new Dictionary<int, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByRevenue.channel.{0}.{1}.{2}.{3}", fromDate, toDate, channel, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByRevenue(fromDate, toDate, channel, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, int> LoadPackageContributionByVolume(DateTime fromDate, DateTime toDate, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByVolume.{0}.{1}.{2}", fromDate, toDate, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByVolume(fromDate, toDate, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, int> LoadPackageContributionByVolume(DateTime fromDate, DateTime toDate, ChannelType channel, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByVolume.channel.{0}.{1}.{2}.{3}", (int)channel, fromDate, toDate, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByVolume(fromDate, toDate, channel, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, int> LoadPackageContributionByVolume(DateTime fromDate, DateTime toDate, int paymentProviderId, bool areCreditsBundles)
        {
            Dictionary<int, int> packagesContribution = new Dictionary<int, int>();

            string cacheName = String.Format("RevenueBusiness.GetPackageContributionByVolume.paymentProvider.{0}.{1}.{2}.{3}", (int)paymentProviderId, fromDate, toDate, areCreditsBundles);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                packagesContribution = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                packagesContribution = RevenueBusiness.GetPackageContributionByVolume(fromDate, toDate, paymentProviderId, areCreditsBundles);
                HttpRuntime.Cache.Add(cacheName, packagesContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return packagesContribution;
        }

        public static Dictionary<int, string> LoadBundlesName()
        {
            Dictionary<int, string> bundlesName = new Dictionary<int, string>();

            string cacheName = String.Format("CmuneBundle.GetBundlesName");

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesName = CmuneBundle.GetBundlesName();
                HttpRuntime.Cache.Add(cacheName, bundlesName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesName;
        }

        public static Dictionary<int, decimal> LoadBundlesUsdPrice()
        {
            Dictionary<int, decimal> bundlesName = new Dictionary<int, decimal>();

            string cacheName = String.Format("CmuneBundle.GetBundlesUsdPrice");

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesName = (Dictionary<int, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesName = CmuneBundle.GetBundlesUsdPrice();
                HttpRuntime.Cache.Add(cacheName, bundlesName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesName;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadBundlesSales(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetBundlesSales.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetBundlesSales(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadBundlesSales(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetBundlesSales.channel.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetBundlesSales(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadBundlesSales(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetBundlesSales.paymentProvider.{0}.{1}.{2}", fromDate, toDate, (int)paymentProviderId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetBundlesSales(fromDate, toDate, paymentProviderId);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadCreditSales(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetCreditSales.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetCreditSales(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadCreditSales(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetCreditSales.channel.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetCreditSales(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        public static Dictionary<DateTime, Dictionary<int, int>> LoadCreditSales(DateTime fromDate, DateTime toDate, int paymentProviderId)
        {
            Dictionary<DateTime, Dictionary<int, int>> bundlesSales = new Dictionary<DateTime, Dictionary<int, int>>();

            string cacheName = String.Format("RevenueBusiness.GetCreditSales.paymentProvider.{0}.{1}.{2}", fromDate, toDate, paymentProviderId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bundlesSales = (Dictionary<DateTime, Dictionary<int, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bundlesSales = RevenueBusiness.GetCreditSales(fromDate, toDate, paymentProviderId);
                HttpRuntime.Cache.Add(cacheName, bundlesSales, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bundlesSales;
        }

        #endregion

        /// <summary>
        /// Loads the payment partner contribution
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<PaymentProviderType, decimal> LoadPaymentProviderContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PaymentProviderType, decimal> paymentPartnerContribution = new Dictionary<PaymentProviderType, decimal>();

            string cacheName = "RevenueBusiness.GetPaymentProviderContribution." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                paymentPartnerContribution = (Dictionary<PaymentProviderType, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                paymentPartnerContribution = RevenueBusiness.GetPaymentProviderContribution(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, paymentPartnerContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return paymentPartnerContribution;
        }

        /// <summary>
        /// Loads the channel contribution
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, decimal> LoadChannelContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, decimal> channelContribution = new Dictionary<ChannelType, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetChannelContribution.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                channelContribution = (Dictionary<ChannelType, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                channelContribution = RevenueBusiness.GetChannelContribution(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, channelContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return channelContribution;
        }

        /// <summary>
        /// Loads the referrer contribution
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ReferrerPartnerType, decimal> LoadReferrerContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ReferrerPartnerType, decimal> referrerContribution = new Dictionary<ReferrerPartnerType, decimal>();

            string cacheName = String.Format("RevenueBusiness.GetReferrerContribution.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                referrerContribution = (Dictionary<ReferrerPartnerType, decimal>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                referrerContribution = RevenueBusiness.GetReferrerContribution(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, referrerContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return referrerContribution;
        }

        /// <summary>
        /// Loads the item credit contribution
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<string, int> LoadItemPointContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, int> itemPointContribution = new Dictionary<string, int>();

            string cacheName = "ItemEconomy.GetItemPointContribution." + fromDate.ToString() + "." + toDate.ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                itemPointContribution = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                itemPointContribution = ItemEconomyBusiness.GetItemPointContribution(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, itemPointContribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return itemPointContribution;
        }

        #region Installation conversion funnel

        /// <summary>
        /// Loads the count of install step
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="stepId"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadInstallTrackingTotals(DateTime fromDate, DateTime toDate, UserInstallStepType stepId, bool hasUnity)
        {
            Dictionary<DateTime, int> installStepsCount = new Dictionary<DateTime, int>();

            string cacheName = String.Format("InstallTracking.GetInstallTrackingTotals.{0}.{1}.{2}.{3}", fromDate, toDate, (int)stepId, hasUnity);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                installStepsCount = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                installStepsCount = InstallTrackingBusiness.GetInstallTrackingTotals(fromDate, toDate, stepId, hasUnity);
                HttpRuntime.Cache.Add(cacheName, installStepsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return installStepsCount;
        }

        /// <summary>
        /// Loads the count of install step by referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <param name="stepId"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadInstallTrackingTotals(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, UserInstallStepType stepId, bool hasUnity)
        {
            Dictionary<DateTime, int> revenueByReferrer = new Dictionary<DateTime, int>();

            string cacheName = String.Format("InstallTracking.GetInstallTrackingTotals.Referrer.{0}.{1}.{2}.{3}.{4}", fromDate, toDate, (int)referrerId, (int)stepId, hasUnity);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                revenueByReferrer = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                revenueByReferrer = InstallTrackingBusiness.GetInstallTrackingTotals(fromDate, toDate, referrerId, stepId, hasUnity);
                HttpRuntime.Cache.Add(cacheName, revenueByReferrer, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return revenueByReferrer;
        }

        /// <summary>
        /// Loads the count of install step by channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="stepId"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadInstallTrackingTotals(DateTime fromDate, DateTime toDate, ChannelType channel, UserInstallStepType stepId, bool hasUnity)
        {
            Dictionary<DateTime, int> installTrackingTotalByChannel = new Dictionary<DateTime, int>();

            string cacheName = String.Format("InstallTracking.GetInstallTrackingTotals.Channel.{0}.{1}.{2}.{3}.{4}", fromDate, toDate, (int)channel, (int)stepId, hasUnity);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                installTrackingTotalByChannel = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                installTrackingTotalByChannel = InstallTrackingBusiness.GetInstallTrackingTotals(fromDate, toDate, channel, stepId, hasUnity);
                HttpRuntime.Cache.Add(cacheName, installTrackingTotalByChannel, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return installTrackingTotalByChannel;
        }

        public static Dictionary<string, int> LoadOsNameDistribution(DateTime fromDate, DateTime toDate, ChannelType? channel)
        {
            Dictionary<string, int> osDistribution = new Dictionary<string, int>();

            int channelId = -1;

            if (channel.HasValue)
            {
                channelId = (int)channel;
            }

            string cacheName = String.Format("InstallTrackingBusiness.GetOsNameDistribution.Channel.{0}.{1}.{2}", channelId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                osDistribution = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                osDistribution = InstallTrackingBusiness.GetOsNameDistribution(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, osDistribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return osDistribution;
        }

        public static Dictionary<string, int> LoadBrowserDistribution(DateTime fromDate, DateTime toDate, ChannelType channel, string operatingSystem)
        {
            Dictionary<string, int> browserDistribution = new Dictionary<string, int>();

            string cacheName = String.Format("InstallTrackingBusiness.GetBrowserDistribution.Channel.{0}.{1}.{2}.{3}", (int)channel, fromDate, toDate, operatingSystem);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                browserDistribution = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                browserDistribution = InstallTrackingBusiness.GetBrowserDistribution(fromDate, toDate, channel, operatingSystem);
                HttpRuntime.Cache.Add(cacheName, browserDistribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return browserDistribution;
        }

        public static Dictionary<DateTime, int> LoadStep(DateTime fromDate, DateTime toDate, UserInstallStepType stepType, ChannelType channel, string operatingSystem, string browserName, string browserVersion, bool hasUnity)
        {
            Dictionary<DateTime, int> steps = new Dictionary<DateTime, int>();

            string cacheName = String.Format("InstallTrackingBusiness.GetStep.{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}", fromDate, toDate, (int)stepType, (int)channel, operatingSystem, browserName, browserVersion, hasUnity);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                steps = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                steps = InstallTrackingBusiness.GetStep(fromDate, toDate, stepType, channel, operatingSystem, browserName, browserVersion, hasUnity);
                HttpRuntime.Cache.Add(cacheName, steps, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return steps;
        }

        public static Dictionary<DateTime, int> LoadStep(DateTime fromDate, DateTime toDate, UserInstallStepType stepType, ChannelType? channel, string osName, string osVersion, string browserName, string browserVersion, ReferrerPartnerType? referrerId, bool hasUnity, bool? isJavaInstallEnabled)
        {
            Dictionary<DateTime, int> steps = new Dictionary<DateTime, int>();

            string cacheName = String.Format("InstallTrackingBusiness.GetStep.{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.{8}.{9}.{10}", fromDate, toDate, stepType, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                steps = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                steps = InstallTrackingBusiness.GetStep(fromDate, toDate, stepType, channel, osName, osVersion, browserName, browserVersion, referrerId, hasUnity, isJavaInstallEnabled);
                HttpRuntime.Cache.Add(cacheName, steps, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return steps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> LoadBrowserNamesOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion)
        {
            Dictionary<string, int> browserNames = new Dictionary<string, int>();

            int channelId = -1;

            if (channel.HasValue)
            {
                channelId = (int)channel;
            }

            string cacheName = String.Format("InstallTrackingBusiness.GetBrowserNamesOrderedByCount.{0}.{1}.{2}.{3}.{4}", fromDate, toDate, channelId, osName, osVersion);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                browserNames = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                browserNames = InstallTrackingBusiness.GetBrowserNamesOrderedByCount(fromDate, toDate, channel, osName, osVersion);
                HttpRuntime.Cache.Add(cacheName, browserNames, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return browserNames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="osName"></param>
        /// <returns></returns>
        public static Dictionary<string, int> LoadOsVersionsOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName)
        {
            Dictionary<string, int> osVersions = new Dictionary<string, int>();

            int channelId = -1;

            if (channel.HasValue)
            {
                channelId = (int)channel.Value;
            }

            string cacheName = String.Format("InstallTrackingBusiness.GetOsVersionsOrderedByCount.{0}.{1}.{2}.{3}", fromDate, toDate, channelId, osName);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                osVersions = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                osVersions = InstallTrackingBusiness.GetOsVersionsOrderedByCount(fromDate, toDate, channel, osName);
                HttpRuntime.Cache.Add(cacheName, osVersions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return osVersions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="osName"></param>
        /// <param name="osVersion"></param>
        /// <param name="browserName"></param>
        /// <returns></returns>
        public static Dictionary<string, int> LoadBrowserVersionsOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion, string browserName)
        {
            Dictionary<string, int> browsersVersions = new Dictionary<string, int>();

            int channelId = -1;

            if (channel.HasValue)
            {
                channelId = (int)channel;
            }

            string cacheName = String.Format("InstallTrackingBusiness.GetBrowserVersionsOrderedByCount.{0}.{1}.{2}.{3}.{4}.{5}", fromDate, toDate, channel, osName, osVersion, browserName);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                browsersVersions = (Dictionary<string, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                browsersVersions = InstallTrackingBusiness.GetBrowserVersionsOrderedByCount(fromDate, toDate, channel, osName, osVersion, browserName);
                HttpRuntime.Cache.Add(cacheName, browsersVersions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return browsersVersions;
        }

        #endregion

        #region Tutorial

        /// <summary>
        /// Loads the steps count for all the stepsId accross the time range
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, Dictionary<TutorialStepType, int>> LoadTutorialStepsCount(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, Dictionary<TutorialStepType, int>> tutorialStepsCount = new Dictionary<DateTime, Dictionary<TutorialStepType, int>>();

            string cacheName = String.Format("InstallTrackingBusiness.GetTutorialStepsCount.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                tutorialStepsCount = (Dictionary<DateTime, Dictionary<TutorialStepType, int>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                tutorialStepsCount = InstallTrackingBusiness.GetTutorialStepsCount(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, tutorialStepsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return tutorialStepsCount;
        }

        #endregion

        /// <summary>
        /// Loads the player level distribution
        /// </summary>
        /// <param name="statDate"></param>
        /// <param name="getActivePlayersOnly"></param>
        /// <returns></returns>
        public static Dictionary<int, int> LoadPlayerLevelDistribution(DateTime statDate, bool getActivePlayersOnly)
        {
            Dictionary<int, int> playerLevelDistribution = new Dictionary<int, int>();

            string cacheName = String.Format("UserActivityBusiness.GetPlayerLevelDistribution.{0}.{1}", statDate.ToDateOnly(), getActivePlayersOnly);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                playerLevelDistribution = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                playerLevelDistribution = UserActivityBusiness.GetPlayerLevelDistribution(statDate, getActivePlayersOnly);
                HttpRuntime.Cache.Add(cacheName, playerLevelDistribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return playerLevelDistribution;
        }

        /// <summary>
        /// Loads credits sale per item
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, int> LoadCreditSaleByItems(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, int> creditSaleByItem = new Dictionary<int, int>();

            string cacheName = String.Format("ItemEconomyBusiness.GetCreditSaleByItems.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                creditSaleByItem = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                creditSaleByItem = ItemEconomyBusiness.GetCreditSaleByItems(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, creditSaleByItem, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return creditSaleByItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadCreditItemSale(DateTime fromDate, DateTime toDate, int itemId)
        {
            Dictionary<DateTime, int> creditItemSale = new Dictionary<DateTime, int>();

            string cacheName = String.Format("ItemEconomyBusiness.GetCreditItemSale.{0}.{1}.{2}", itemId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                creditItemSale = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                creditItemSale = ItemEconomyBusiness.GetCreditItemSale(fromDate, toDate, itemId);
                HttpRuntime.Cache.Add(cacheName, creditItemSale, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return creditItemSale;
        }

        public static Dictionary<int, string> LoadNonDefaultMembersNames(int applicationId, MemberAccessLevel accessLevel)
        {
            Dictionary<int, string> memberNames = new Dictionary<int, string>();

            string cacheName = String.Format("CmuneMember.GetNonDefaultMembersNames.{0}.{1}", applicationId, (int)accessLevel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                memberNames = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                memberNames = CmuneMember.GetNonDefaultMembersNames(applicationId, accessLevel);
                HttpRuntime.Cache.Add(cacheName, memberNames, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return memberNames;
        }

        #region Retention

        public static Dictionary<DateTime, Dictionary<int, decimal>> LoadRetentionCohorts(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            string cacheName = String.Format("UserActivityBusiness.GetRetentionCohorts.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                cohorts = (Dictionary<DateTime, Dictionary<int, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                cohorts = UserActivityBusiness.GetRetentionCohorts(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, cohorts, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> LoadRetentionCohorts(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            string cacheName = String.Format("UserActivityBusiness.GetRetentionCohorts.Channel.{0}.{1}.{2}", fromDate, toDate, channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                cohorts = (Dictionary<DateTime, Dictionary<int, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                cohorts = UserActivityBusiness.GetRetentionCohorts(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, cohorts, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> LoadRetentionCohorts(DateTime fromDate, DateTime toDate, int region)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            string cacheName = String.Format("UserActivityBusiness.GetRetentionCohorts.Region.{0}.{1}.{2}", fromDate, toDate, region);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                cohorts = (Dictionary<DateTime, Dictionary<int, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                cohorts = UserActivityBusiness.GetRetentionCohorts(fromDate, toDate, region);
                HttpRuntime.Cache.Add(cacheName, cohorts, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> LoadRetentionCohorts(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            string cacheName = String.Format("UserActivityBusiness.GetRetentionCohorts.Referrer.{0}.{1}.{2}", fromDate, toDate, referrerId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                cohorts = (Dictionary<DateTime, Dictionary<int, decimal>>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                cohorts = UserActivityBusiness.GetRetentionCohorts(fromDate, toDate, referrerId);
                HttpRuntime.Cache.Add(cacheName, cohorts, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return cohorts;
        }

        #endregion

        #region Points / Credits economy

        public static Dictionary<DateTime, int> LoadAveragePointBalance(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> pointsBalance = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetAveragePointBalance.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsBalance = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsBalance = RevenueBusiness.GetAveragePointBalance(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, pointsBalance, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsBalance;
        }

        public static Dictionary<DateTime, int> LoadAveragePointsEarned(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> pointsEarned = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetAveragePointsEarned.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsEarned = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsEarned = RevenueBusiness.GetAveragePointsEarned(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, pointsEarned, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsEarned;
        }

        public static Dictionary<DateTime, int> LoadAveragePointsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> pointsSpent = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetAveragePointsSpent.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsSpent = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsSpent = RevenueBusiness.GetAveragePointsSpent(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, pointsSpent, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsSpent;
        }

        public static Dictionary<DateTime, int> LoadMedianPointsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> pointsSpent = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetMedianPointsSpent.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsSpent = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsSpent = RevenueBusiness.GetMedianPointsSpent(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, pointsSpent, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsSpent;
        }

        public static Dictionary<DateTime, int> LoadAverageCreditsBalance(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> creditsBalance = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetAverageCreditsBalance.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                creditsBalance = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                creditsBalance = RevenueBusiness.GetAverageCreditsBalance(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, creditsBalance, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return creditsBalance;
        }

        public static Dictionary<DateTime, int> LoadAverageCreditsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> creditsSpent = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetAverageCreditsSpent.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                creditsSpent = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                creditsSpent = RevenueBusiness.GetAverageCreditsSpent(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, creditsSpent, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return creditsSpent;
        }

        public static Dictionary<DateTime, int> LoadMedianCreditsSpent(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> creditsSpent = new Dictionary<DateTime, int>();

            string cacheName = String.Format("RevenueBusiness.GetMedianCreditsSpent.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                creditsSpent = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                creditsSpent = RevenueBusiness.GetMedianCreditsSpent(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, creditsSpent, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return creditsSpent;
        }

        public static Dictionary<PointsDepositType, long> LoadPointDepositsDistribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<PointsDepositType, long> pointsDepositsDistribution = new Dictionary<PointsDepositType, long>();

            string cacheName = String.Format("RevenueBusiness.GetPointDepositsDistribution.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsDepositsDistribution = (Dictionary<PointsDepositType, long>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsDepositsDistribution = RevenueBusiness.GetPointDepositsDistribution(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, pointsDepositsDistribution, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsDepositsDistribution;
        }

        #endregion

        #region Friends

        public static Dictionary<DateTime, int> LoadAverageFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> averageFriendsCount = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetAverageFriendsCount.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                averageFriendsCount = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                averageFriendsCount = UserActivityBusiness.GetAverageFriendsCount(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, averageFriendsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return averageFriendsCount;
        }

        public static Dictionary<DateTime, int> LoadAverageActiveFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> averageActiveFriendsCount = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetAverageActiveFriendsCount.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                averageActiveFriendsCount = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                averageActiveFriendsCount = UserActivityBusiness.GetAverageActiveFriendsCount(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, averageActiveFriendsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return averageActiveFriendsCount;
        }

        public static Dictionary<DateTime, int> LoadMedianFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> medianFriendsCount = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMedianFriendsCount.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                medianFriendsCount = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                medianFriendsCount = UserActivityBusiness.GetMedianFriendsCount(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, medianFriendsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return medianFriendsCount;
        }

        public static Dictionary<DateTime, int> LoadMedianActiveFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> medianActiveFriendsCount = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMedianActiveFriendsCount.{0}.{1}.{2}", fromDate, toDate, (int)channel);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                medianActiveFriendsCount = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                medianActiveFriendsCount = UserActivityBusiness.GetMedianActiveFriendsCount(fromDate, toDate, channel);
                HttpRuntime.Cache.Add(cacheName, medianActiveFriendsCount, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return medianActiveFriendsCount;
        }

        #endregion

        #region Generate Selects

        /// <summary>
        /// Generates the purchase type DropDownList ListItem[]
        /// </summary>
        /// <returns></returns>
        public static ListItem[] GeneratePurchaseTypeDropDownListItems()
        {
            List<PurchaseType> purchaseTypes = EnumUtilities.IterateEnum<PurchaseType>();

            ListItem[] listContent = new ListItem[purchaseTypes.Count];

            int i = 0;

            foreach (PurchaseType purchaseType in purchaseTypes)
            {
                listContent[i] = new ListItem(purchaseType.ToString(), purchaseType.ToString());
                i++;
            }

            return listContent;
        }

        /// <summary>
        /// Generates the Photon usage type DropDownList ListItem[]
        /// </summary>
        /// <returns></returns>
        public static ListItem[] GeneratePhotonUsageTypeDropDownListItems()
        {
            List<PhotonUsageType> photonUsageTypes = EnumUtilities.IterateEnum<PhotonUsageType>();

            ListItem[] listContent = new ListItem[photonUsageTypes.Count];

            int i = 0;

            foreach (PhotonUsageType photonUsageType in photonUsageTypes)
            {
                listContent[i] = new ListItem(photonUsageType.ToString(), ((int)photonUsageType).ToString());
                i++;
            }

            return listContent;
        }

        /// <summary>
        /// Generates the region type DropDownList ListItem[]
        /// </summary>
        /// <returns></returns>
        public static ListItem[] GenerateRegionTypeDropDownListItems()
        {
            List<RegionType> regionTypes = EnumUtilities.IterateEnum<RegionType>();

            ListItem[] listContent = new ListItem[regionTypes.Count];

            int i = 0;

            foreach (RegionType regionType in regionTypes)
            {
                listContent[i] = new ListItem(regionType.ToString(), ((int)regionType).ToString());
                i++;
            }

            return listContent;
        }

        /// <summary>
        /// Generates the member access level DropDownList ListItem[]
        /// </summary>
        /// <returns></returns>
        public static ListItem[] GenerateMemberAccessLevelDropDownListItems()
        {
            List<MemberAccessLevel> memberAccessLevels = EnumUtilities.IterateEnum<MemberAccessLevel>();

            ListItem[] listContent = new ListItem[memberAccessLevels.Count];

            int i = 0;

            foreach (MemberAccessLevel memberAccessLevel in memberAccessLevels)
            {
                listContent[i] = new ListItem(memberAccessLevel.ToString(), ((int)memberAccessLevel).ToString());
                i++;
            }

            return listContent;
        }

        /// <summary>
        /// Generates the Esns DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateEsnsDropDownListItems()
        {
            return GenerateEsnsDropDownListItems(false);
        }

        /// <summary>
        /// Generates the Esns DropDownList List SelectListItem
        /// </summary>
        /// <param name="keepNone"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateEsnsDropDownListItems(bool keepNone)
        {
            List<EsnsType> esnses = EnumUtilities.IterateEnum<EsnsType>();

            var esnsDdl = new List<SelectListItem>();

            foreach (EsnsType esns in esnses)
            {
                if (EnumUtilities.IsCurrentEsns(esns) && (!esns.Equals(EsnsType.None) || keepNone))
                {
                    SelectListItem item = new SelectListItem();

                    item.Text = esns.ToString();
                    item.Value = ((int)esns).ToString();
                    item.Selected = false;

                    if (esns.Equals(EsnsType.Facebook))
                    {
                        item.Selected = true;
                    }

                    esnsDdl.Add(item);
                }
            }

            return esnsDdl;
        }

        /// <summary>
        /// Generates the Referrer DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateReferrerDropDownListItems()
        {
            return GenerateReferrerDropDownListItems(false);
        }

        /// <summary>
        /// Generates the Referrer DropDownList List SelectListItem
        /// </summary>
        /// <param name="keepNone"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateReferrerDropDownListItems(bool keepNone)
        {
            List<ReferrerPartnerType> referrers = EnumUtilities.IterateEnum<ReferrerPartnerType>();

            var esnsDdl = new List<SelectListItem>();

            foreach (ReferrerPartnerType referrer in referrers)
            {
                if (EnumUtilities.IsCurrentReferrer(referrer) && (!referrer.Equals(ReferrerPartnerType.None) || keepNone))
                {
                    SelectListItem item = new SelectListItem();

                    item.Text = referrer.ToString();
                    item.Value = ((int)referrer).ToString();
                    item.Selected = false;

                    if (keepNone && referrer.Equals(ReferrerPartnerType.None))
                    {
                        item.Selected = true;
                    }
                    else if (!keepNone && referrer.Equals(ReferrerPartnerType.Applifier))
                    {
                        item.Selected = true;
                    }

                    esnsDdl.Add(item);
                }
            }

            return esnsDdl;
        }

        /// <summary>
        /// Generates the BuyingDurationType DropDownList List SelectListItem
        /// </summary>
        /// <param name="keepNone"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateBuyingDurationSelectItems(BuyingDurationType selectedDuration = BuyingDurationType.OneDay)
        {
            List<BuyingDurationType> buyingDurationTypes = EnumUtilities.IterateEnum<BuyingDurationType>();

            var buyingDurationTypeDdl = new List<SelectListItem>();

            foreach (BuyingDurationType buyingDurationType in buyingDurationTypes)
            {
                SelectListItem item = new SelectListItem();

                item.Text = buyingDurationType.ToString();
                item.Value = ((int)buyingDurationType).ToString();
                item.Selected = false;

                if (buyingDurationType.Equals(selectedDuration))
                {
                    item.Selected = true;
                }

                buyingDurationTypeDdl.Add(item);
            }

            return buyingDurationTypeDdl;
        }

        /// <summary>
        /// Generates the countries revenue DropDownList
        /// Contains two list in one: 1) countriesToBeDisplayedAtTheTopCount ordered by revenue descending (the top countries by revenue basically) 2) Then totalCountriesToBeDisplayed countries by alphabetical order
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="countriesToBeDisplayedAtTheTopCount"></param>
        /// <param name="totalCountriesToBeDisplayed"></param>
        /// <param name="selectTopCountry"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateCountriesRevenueDropDownListItems(DateTime fromDate, DateTime toDate, int countriesToBeDisplayedAtTheTopCount, int totalCountriesToBeDisplayed, bool selectTopCountry)
        {
            Dictionary<int, string> countriesName = LoadCountriesName();
            Dictionary<int, decimal> countriesRevenue = LoadRevenueByCountriesOrdered(fromDate, toDate);
            Dictionary<int, decimal> countriesToBeDisplayed = new Dictionary<int, decimal>();

            var revenueByCountriesDdl = new List<SelectListItem>();

            countriesToBeDisplayed = countriesRevenue.Take(countriesToBeDisplayedAtTheTopCount).ToDictionary(t => t.Key, t => t.Value);
            var remaininCountries = countriesRevenue.Skip(countriesToBeDisplayedAtTheTopCount).Take(totalCountriesToBeDisplayed).ToDictionary(t => t.Key, t => t.Value);
            countriesToBeDisplayed = countriesToBeDisplayed.Concat(remaininCountries.OrderBy(t => countriesName[t.Key]).ToDictionary(t => t.Key, t => t.Value)).ToDictionary(t => t.Key, t => t.Value);

            revenueByCountriesDdl.Add(new SelectListItem { Text = "None", Value = "-1", Selected = !selectTopCountry });

            int i = 0;

            foreach (int countryId in countriesToBeDisplayed.Keys)
            {
                SelectListItem item = new SelectListItem();

                item.Text = String.Format("{0}{1}", countriesName[countryId], countriesToBeDisplayed[countryId].ToString("C2").PadLeft(40 - countriesName[countryId].Length, '\xA0'));
                item.Value = countryId.ToString();
                item.Selected = false;

                if (i == 0 && selectTopCountry)
                {
                    item.Selected = true;
                }

                revenueByCountriesDdl.Add(item);

                i++;
            }

            return revenueByCountriesDdl;
        }

        /// <summary>
        /// Generates the countries revenue DropDownList
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateCountriesRevenueDropDownListItems(DateTime fromDate, DateTime toDate)
        {
            return GenerateCountriesRevenueDropDownListItems(fromDate, toDate, false);
        }

        /// <summary>
        /// Generates the countries revenue DropDownList
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="selectTopCountry"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateCountriesRevenueDropDownListItems(DateTime fromDate, DateTime toDate, bool selectTopCountry)
        {
            return GenerateCountriesRevenueDropDownListItems(fromDate, toDate, 10, 250, selectTopCountry);
        }

        public static List<SelectListItem> GenerateCountriesMauDropDownListItems(Dictionary<int, int> countriesMau, int countriesToBeDisplayedAtTheTopCount, int totalCountriesToBeDisplayed, bool selectTopCountry, bool includeNone)
        {
            Dictionary<int, string> countriesName = LoadCountriesName();
            Dictionary<int, int> countriesToBeDisplayed = new Dictionary<int, int>();

            var revenueByCountriesDdl = new List<SelectListItem>();

            countriesToBeDisplayed = countriesMau.Take(countriesToBeDisplayedAtTheTopCount).ToDictionary(t => t.Key, t => t.Value);
            var remaininCountries = countriesMau.Skip(countriesToBeDisplayedAtTheTopCount).Take(totalCountriesToBeDisplayed).ToDictionary(t => t.Key, t => t.Value);
            countriesToBeDisplayed = countriesToBeDisplayed.Concat(remaininCountries.OrderBy(t => countriesName[t.Key]).ToDictionary(t => t.Key, t => t.Value)).ToDictionary(t => t.Key, t => t.Value);

            if (includeNone)
            {
                revenueByCountriesDdl.Add(new SelectListItem { Text = "None", Value = "-1", Selected = !selectTopCountry });
            }

            int i = 0;

            foreach (int countryId in countriesToBeDisplayed.Keys)
            {
                SelectListItem item = new SelectListItem();

                item.Text = String.Format("{0}{1}", countriesName[countryId], countriesToBeDisplayed[countryId].ToString("N0").PadLeft(45 - countriesName[countryId].Length, '\xA0'));
                item.Value = countryId.ToString();
                item.Selected = false;

                if (i == 0 && (selectTopCountry || !includeNone))
                {
                    item.Selected = true;
                }

                revenueByCountriesDdl.Add(item);

                i++;
            }

            return revenueByCountriesDdl;
        }

        public static List<SelectListItem> GenerateCountriesMauDropDownListItems(DateTime toDate)
        {
            return GenerateCountriesMauDropDownListItems(toDate, false, true);
        }

        public static List<SelectListItem> GenerateCountriesMauDropDownListItems(DateTime toDate, bool selectTopCountry, bool includeNone)
        {
            Dictionary<int, int> countriesMau = LoadMonthlyActiveUsersByCountriesOrdered(toDate);
            return GenerateCountriesMauDropDownListItems(countriesMau, 10, 250, selectTopCountry, includeNone);
        }

        public static List<SelectListItem> GenerateCountriesMauDropDownListItems(DateTime toDate, ReferrerPartnerType referrerId, bool selectTopCountry = false, bool includeNone = true)
        {
            Dictionary<int, int> countriesMau = LoadMonthlyActiveUsersByCountriesOrdered(toDate, referrerId);
            return GenerateCountriesMauDropDownListItems(countriesMau, 10, 250, selectTopCountry, includeNone);
        }

        public static List<SelectListItem> GenerateItemCreditSaleDropDownListItems(int applicationId, DateTime fromDate, DateTime toDate, int itemsToBeDisplayedAtTheTopCount, int totalItemsToBeDisplayed, bool selectTopItem)
        {
            ItemCache itemCache = new ItemCache(applicationId, true);
            Dictionary<int, int> itemsSale = LoadCreditSaleByItems(fromDate, toDate);
            Dictionary<int, int> itemsSaleToBeDisplayed = new Dictionary<int, int>();

            var itemsSaleDdl = new List<SelectListItem>();

            itemsSaleToBeDisplayed = itemsSale.Take(itemsToBeDisplayedAtTheTopCount).ToDictionary(t => t.Key, t => t.Value);
            var remainingItemSales = itemsSale.Skip(itemsToBeDisplayedAtTheTopCount).Take(totalItemsToBeDisplayed).ToDictionary(t => t.Key, t => t.Value);
            itemsSaleToBeDisplayed = itemsSaleToBeDisplayed.Concat(remainingItemSales.OrderBy(t => itemCache.GetItemName(t.Key)).ToDictionary(t => t.Key, t => t.Value)).ToDictionary(t => t.Key, t => t.Value);

            itemsSaleDdl.Add(new SelectListItem { Text = "None", Value = "0", Selected = !selectTopItem });

            int i = 0;

            foreach (int itemId in itemsSaleToBeDisplayed.Keys)
            {
                SelectListItem item = new SelectListItem();

                string usdValue = ((decimal)itemsSaleToBeDisplayed[itemId] / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd]).ToString("C2");

                item.Text = String.Format("{0}{1} credits - {2}", itemCache.GetItemName(itemId), itemsSaleToBeDisplayed[itemId].ToString().PadLeft(42 - itemCache.GetItemName(itemId).Length, '\xA0'), usdValue.PadLeft(9, '\xA0'));
                item.Value = itemId.ToString();
                item.Selected = false;

                if (i == 0 && selectTopItem)
                {
                    item.Selected = true;
                }

                itemsSaleDdl.Add(item);

                i++;
            }

            return itemsSaleDdl;
        }

        public static List<SelectListItem> GenerateItemCreditSaleDropDownListItems(int applicationId, DateTime fromDate, DateTime toDate)
        {
            return GenerateItemCreditSaleDropDownListItems(applicationId, fromDate, toDate, false);
        }

        public static List<SelectListItem> GenerateItemCreditSaleDropDownListItems(int applicationId, DateTime fromDate, DateTime toDate, bool selectTopCountry)
        {
            return GenerateItemCreditSaleDropDownListItems(applicationId, fromDate, toDate, 10, 250, selectTopCountry);
        }

        /// <summary>
        /// Generates the countries revenue DropDownList
        /// Contains two list in one: 1) countriesToBeDisplayedAtTheTopCount ordered by revenue descending (the top countries by revenue basically) 2) Then totalCountriesToBeDisplayed countries by alphabetical order
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="countriesToBeDisplayedAtTheTopCount"></param>
        /// <param name="totalCountriesToBeDisplayed"></param>
        /// <param name="selectTopCountry"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateCountriesReferrerNewMembersDropDownListItems(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, bool selectTopCountry = false, bool includeNone = false, int countriesToBeDisplayedAtTheTopCount = 10, int totalCountriesToBeDisplayed = 250)
        {
            Dictionary<int, string> countriesName = LoadCountriesName();
            Dictionary<int, int> countriesNewMembers = LoadNewMembersCountByCountries(fromDate, toDate, referrerId).OrderByDescending(u => u.Value).ToDictionary(u => u.Key, u => u.Value);
            Dictionary<int, int> countriesToBeDisplayed = new Dictionary<int, int>();

            var newMembersCountriesDdl = new List<SelectListItem>();

            countriesToBeDisplayed = countriesNewMembers.Take(countriesToBeDisplayedAtTheTopCount).ToDictionary(t => t.Key, t => t.Value);
            var remaininCountries = countriesNewMembers.Skip(countriesToBeDisplayedAtTheTopCount).Take(totalCountriesToBeDisplayed).ToDictionary(t => t.Key, t => t.Value);
            countriesToBeDisplayed = countriesToBeDisplayed.Concat(remaininCountries.OrderBy(t => countriesName[t.Key]).ToDictionary(t => t.Key, t => t.Value)).ToDictionary(t => t.Key, t => t.Value);

            if (includeNone)
            {
                newMembersCountriesDdl.Add(new SelectListItem { Text = "None", Value = "-1", Selected = !selectTopCountry });
            }

            int newMembersCount = countriesToBeDisplayed.Sum(u => u.Value);
            int i = 0;

            foreach (int countryId in countriesToBeDisplayed.Keys)
            {
                SelectListItem item = new SelectListItem();

                string newMembersCountDisplay = countriesToBeDisplayed[countryId].ToString("N0").PadLeft(40 - countriesName[countryId].Length, '\xA0');
                decimal newMembersPercent = 0;

                if (newMembersCount > 0)
                {
                    newMembersPercent = (decimal)countriesToBeDisplayed[countryId] / (decimal)newMembersCount;
                }

                item.Text = String.Format("{0}{1} - {2}", countriesName[countryId], newMembersCountDisplay, newMembersPercent.ToString("P2").PadLeft(5, '\xA0'));
                item.Value = countryId.ToString();
                item.Selected = false;

                if (i == 0 && (selectTopCountry || !includeNone))
                {
                    item.Selected = true;
                }

                newMembersCountriesDdl.Add(item);

                i++;
            }

            return newMembersCountriesDdl;
        }

        /// <summary>
        /// Generates the Channel DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateChannelDropDownListItems()
        {
            return GenerateChannelDropDownListItems(false);
        }

        /// <summary>
        /// Generates the Channel DropDownList List SelectListItem
        /// </summary>
        /// <param name="displayAll"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateChannelDropDownListItems(bool displayAll)
        {
            List<ChannelType> channels = EnumUtilities.IterateEnum<ChannelType>();

            var channelDdl = new List<SelectListItem>();

            if (displayAll)
            {
                channelDdl.Add(new SelectListItem { Text = "All channels", Selected = true, Value = "-1" });
            }

            foreach (ChannelType channel in channels)
            {
                if (EnumUtilities.IsCurrentChannel(channel))
                {
                    SelectListItem item = new SelectListItem();

                    item.Text = channel.ToString();
                    item.Value = ((int)channel).ToString();
                    item.Selected = false;

                    if (channel.Equals(ChannelType.WebFacebook) && !displayAll)
                    {
                        item.Selected = true;
                    }

                    channelDdl.Add(item);
                }
            }

            return channelDdl;
        }

        /// <summary>
        /// Generates the Channel DropDownList List SelectListItem
        /// </summary>
        /// <param name="displayAll"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateEpinProviderDropDownListItems(bool displayAll)
        {
            List<PaymentProviderType> paymentProviders = EnumUtilities.IterateEnum<PaymentProviderType>();

            var paymentProvidersDdl = new List<SelectListItem>();

            if (displayAll)
            {
                paymentProvidersDdl.Add(new SelectListItem { Text = "All Epin providers", Selected = true, Value = "-1" });
            }

            foreach (PaymentProviderType paymentProvider in paymentProviders)
            {
                if (EnumUtilities.IsEpinProvider(paymentProvider))
                {
                    SelectListItem item = new SelectListItem();

                    item.Text = paymentProvider.ToString();
                    item.Value = ((int)paymentProvider).ToString();
                    item.Selected = false;

                    if (paymentProvider.Equals(PaymentProviderType.GameSultan) && !displayAll)
                    {
                        item.Selected = true;
                    }

                    paymentProvidersDdl.Add(item);
                }
            }

            return paymentProvidersDdl;
        }

        #region Unity Conversion funnel

        /// <summary>
        /// Generates the Os name DropDownList List SelectListItem
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateOsNameDropDownListItems(DateTime fromDate, DateTime toDate, ChannelType? channel)
        {
            var osNameDdl = new List<SelectListItem>();

            Dictionary<string, int> osNames = AdminCache.LoadOsNameDistribution(fromDate, toDate, channel);

            osNameDdl.Add(new SelectListItem { Text = "All OS", Value = "-1", Selected = true });

            int totalCount = 0;

            foreach (string osName in osNames.Keys)
            {
                totalCount += osNames[osName];
            }

            foreach (string osName in osNames.Keys)
            {
                decimal marketShare = 0;

                if (totalCount != 0)
                {
                    marketShare = (decimal)osNames[osName] / (decimal)totalCount;
                }

                string osNameDisplay = osName.Trim();

                if (osName == "0")
                {
                    osNameDisplay = "Undetermined";
                }

                osNameDdl.Add(new SelectListItem { Text = String.Format("{0} - {1} ({2})", osNameDisplay, osNames[osName], marketShare.ToString("P2")), Value = osName.Trim(), Selected = false });
            }

            return osNameDdl;
        }

        /// <summary>
        /// Generates the Os versions DropDownList List SelectListItem
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="osName"></param>
        /// <param name="displayAll"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateOsVersionDropDownListItems(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, bool displayAll)
        {
            Dictionary<string, int> osVersions = AdminCache.LoadOsVersionsOrderedByCount(fromDate, toDate, channel, osName);

            var osVersionDdl = new List<SelectListItem>();

            if (displayAll)
            {
                osVersionDdl.Add(new SelectListItem { Text = "All versions", Selected = true, Value = "-1" });
            }

            int i = 0;

            int totalCount = 0;

            foreach (string osVersion in osVersions.Keys)
            {
                totalCount += osVersions[osVersion];
            }

            foreach (string osVersion in osVersions.Keys)
            {
                SelectListItem item = new SelectListItem();

                string displayName = osVersion.Trim();

                if (String.Equals("0", displayName))
                {
                    displayName = "Undefined";
                }

                switch (displayName)
                {
                    case "4.0":
                        displayName = String.Format("{0} (NT {1})", "95", displayName);
                        break;
                    case "5.0":
                        displayName = String.Format("{0} (NT {1})", "2000", displayName);
                        break;
                    case "5.1":
                        displayName = String.Format("{0} (NT {1})", "XP", displayName);
                        break;
                    case "5.2":
                        displayName = String.Format("{0} (NT {1})", "XP 64 bits", displayName);
                        break;
                    case "6.0":
                        displayName = String.Format("{0} (NT {1})", "Vista", displayName);
                        break;
                    case "6.1":
                        displayName = String.Format("{0} (NT {1})", "7", displayName);
                        break;
                    case "6.2":
                        displayName = String.Format("{0} (NT {1})", "8", displayName);
                        break;
                }

                decimal marketShare = 0;

                if (totalCount != 0)
                {
                    marketShare = (decimal)osVersions[osVersion] / (decimal)totalCount;
                }

                displayName = String.Format("{0} - {1} ({2})", displayName, osVersions[osVersion], marketShare.ToString("P2"));

                item.Text = displayName;
                item.Value = osVersion.Trim();
                item.Selected = false;

                if (i == 0 && !displayAll)
                {
                    item.Selected = true;
                }

                osVersionDdl.Add(item);

                i++;
            }

            return osVersionDdl;
        }

        /// <summary>
        /// Generates the Os name DropDownList List SelectListItem
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="osName"></param>
        /// <param name="osVersion"></param>
        /// <param name="displayAll"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateBrowserNameDropDownListItems(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion, bool displayAll)
        {
            Dictionary<string, int> browserNames = AdminCache.LoadBrowserNamesOrderedByCount(fromDate, toDate, channel, osName, osVersion);

            var browserNamesDdl = new List<SelectListItem>();

            if (displayAll)
            {
                browserNamesDdl.Add(new SelectListItem { Text = "All browsers", Selected = true, Value = "-1" });
            }

            int totalCount = 0;

            foreach (string browserName in browserNames.Keys)
            {
                totalCount += browserNames[browserName];
            }

            int i = 0;

            foreach (string browserName in browserNames.Keys)
            {
                SelectListItem item = new SelectListItem();

                string displayName = browserName.Trim();

                if (String.Equals("Explorer", displayName))
                {
                    displayName = "IE";
                }
                else if (String.Equals("0", displayName))
                {
                    displayName = "Undefined";
                }

                decimal marketShare = 0;

                if (totalCount != 0)
                {
                    marketShare = (decimal)browserNames[browserName] / (decimal)totalCount;
                }

                item.Text = String.Format("{0} - {1} ({2})", displayName, browserNames[browserName], marketShare.ToString("P2"));
                item.Value = browserName.Trim();
                item.Selected = false;

                if (i == 0 && !displayAll)
                {
                    item.Selected = true;
                }

                browserNamesDdl.Add(item);

                i++;
            }

            return browserNamesDdl;
        }

        /// <summary>
        /// Generates the browser versions DropDownList List SelectListItem
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="osName"></param>
        /// <param name="osVersion"></param>
        /// <param name="browserName"></param>
        /// <param name="displayAll"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateBrowserVersionDropDownListItems(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion, string browserName, bool displayAll)
        {
            Dictionary<string, int> browsersVersions = AdminCache.LoadBrowserVersionsOrderedByCount(fromDate, toDate, channel, osName, osVersion, browserName);

            var browserVersionDdl = new List<SelectListItem>();

            if (displayAll)
            {
                browserVersionDdl.Add(new SelectListItem { Text = "All versions", Selected = true, Value = "-1" });
            }

            int totalCount = 0;

            foreach (string browserVersion in browsersVersions.Keys)
            {
                totalCount += browsersVersions[browserVersion];
            }

            int i = 0;

            foreach (string browserVersion in browsersVersions.Keys)
            {
                SelectListItem item = new SelectListItem();

                string displayName = browserVersion.Trim();

                if (String.Equals("0", displayName))
                {
                    displayName = "Undefined";
                }

                decimal marketShare = 0;

                if (totalCount != 0)
                {
                    marketShare = (decimal)browsersVersions[browserVersion] / (decimal)totalCount;
                }

                item.Text = String.Format("{0} - {1} ({2})", displayName, browsersVersions[browserVersion], marketShare.ToString("P2"));
                item.Value = browserVersion.Trim();
                item.Selected = false;

                if (i == 0 && !displayAll)
                {
                    item.Selected = true;
                }

                browserVersionDdl.Add(item);

                i++;
            }

            return browserVersionDdl;
        }

        #endregion

        /// <summary>
        /// Generates the admin DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateAdminDropDownListItems()
        {
            Dictionary<int, string> adminNames = AdminCache.LoadNonDefaultMembersNames(CommonConfig.ApplicationIdUberstrike, MemberAccessLevel.Admin);

            var adminNamesDdl = new List<SelectListItem>();

            int i = 0;

            foreach (KeyValuePair<int, string> adminName in adminNames)
            {
                SelectListItem item = new SelectListItem();

                item.Text = adminName.Value;
                item.Value = adminName.Key.ToString();
                item.Selected = false;

                if (i == 0)
                {
                    item.Selected = true;
                }

                adminNamesDdl.Add(item);

                i++;
            }

            return adminNamesDdl;
        }

        /// <summary>
        /// Generates the action type DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateModerationActionTypeDropDownListItems()
        {
            List<ModerationActionType> actionTypes = EnumUtilities.IterateEnum<ModerationActionType>();

            var actionTypeDdl = new List<SelectListItem>();

            foreach (ModerationActionType actionType in actionTypes)
            {
                SelectListItem item = new SelectListItem();

                item.Text = actionType.ToString();
                item.Value = ((int)actionType).ToString();
                item.Selected = false;

                if (actionType.Equals(ModerationActionType.Note))
                {
                    item.Selected = true;
                }

                actionTypeDdl.Add(item);
            }

            return actionTypeDdl;
        }

        /// <summary>
        /// Generates the versions DropDownList List SelectListItem
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GenerateVersionsDropDownListItems(int versionsCount)
        {
            List<string> versions = AdminCache.LoadVersionNumbers(versionsCount);

            var versionsDdl = new List<SelectListItem>();

            for (int i = 0; i < versions.Count; i++)
            {
                SelectListItem item = new SelectListItem();

                item.Text = versions[i].ToString();
                item.Value = versions[i].ToString();
                item.Selected = false;

                if (i == 0)
                {
                    item.Selected = true;
                }

                versionsDdl.Add(item);
            }

            return versionsDdl;
        }

        public static List<SelectListItem> GenerateMapUsageSelectItems(DateTime fromDate, DateTime toDate, bool selectTopMap = false)
        {
            Dictionary<int, string> mapsName = LoadMapNames();
            Dictionary<int, int> mapUsage = LoadMapUsage(fromDate, toDate);
            var mapUsageSelect = new List<SelectListItem>();

            mapUsageSelect.Add(new SelectListItem { Text = "None", Value = "0", Selected = !selectTopMap });

            int i = 0;

            foreach (int mapId in mapUsage.Keys)
            {
                SelectListItem item = new SelectListItem();

                string mapName = "Deleted map";

                if (mapsName.ContainsKey(mapId))
                {
                    mapName = mapsName[mapId];
                }

                item.Text = String.Format("{0}{1} players", mapName, mapUsage[mapId].ToString().PadLeft(36 - mapName.Length, '\xA0'));
                item.Value = mapId.ToString();
                item.Selected = false;

                if (i == 0 && selectTopMap)
                {
                    item.Selected = true;
                }

                mapUsageSelect.Add(item);

                i++;
            }

            return mapUsageSelect;
        }

        /// <summary>
        /// Generates the bundle categories select
        /// </summary>
        /// <param name="selectedCategory"></param>
        /// <returns></returns>
        public static List<SelectListItem> GenerateBundleCategorySelectItems(BundleCategoryType selectedCategory = BundleCategoryType.None)
        {
            List<BundleCategoryType> categories = EnumUtilities.IterateEnum<BundleCategoryType>();

            var categoriesSelect = new List<SelectListItem>();

            foreach (BundleCategoryType category in categories)
            {
                SelectListItem item = new SelectListItem();

                item.Text = category.ToString();
                item.Value = category.ToString();
                item.Selected = false;

                if (category.Equals(selectedCategory))
                {
                    item.Selected = true;
                }

                categoriesSelect.Add(item);
            }

            return categoriesSelect;
        }

        #endregion Generate DropDownLists

        #region Members

        #region Banned Ips

        public static List<BannedIpView> LoadBannedIps(int pageIndex, int elementsPerPage, bool displayPermanentBanOnly = false)
        {
            List<BannedIpView> bannedIps = new List<BannedIpView>();

            string cacheName = String.Format("CmuneMember.GetBannedIps.{0}.{1}.{2}", pageIndex, elementsPerPage, displayPermanentBanOnly);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bannedIps = (List<BannedIpView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bannedIps = CmuneMember.GetBannedIpsViews(pageIndex, elementsPerPage, displayPermanentBanOnly);
                HttpRuntime.Cache.Add(cacheName, bannedIps, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bannedIps;
        }

        public static int LoadBannedIpsCount(bool displayPermanentBanOnly = false)
        {
            int bannedIpsCount = 0;

            string cacheName = String.Format("CmuneMember.GetBannedIpsCount.{0}", displayPermanentBanOnly);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                bannedIpsCount = (int)HttpRuntime.Cache[cacheName];
            }
            else
            {
                bannedIpsCount = CmuneMember.GetBannedIpsViewsCount(displayPermanentBanOnly);
                HttpRuntime.Cache.Add(cacheName, bannedIpsCount, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return bannedIpsCount;
        }

        #endregion

        #region Currency deposits

        public static List<CurrencyDepositView> LoadCurrencyDepositsView(int cmid, int pageIndex, int elementsPerPage)
        {
            List<CurrencyDepositView> currencyDeposits = new List<CurrencyDepositView>();

            bool getOldDeposits = true;
            string cacheName = String.Format("CmuneEconomy.GetCurrencyDepositsView.{0}.{1}.{2}.{3}", cmid, pageIndex, elementsPerPage, getOldDeposits);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                currencyDeposits = (List<CurrencyDepositView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                currencyDeposits = CmuneEconomy.GetCurrencyDepositsView(cmid, pageIndex, elementsPerPage, getOldDeposits);
                HttpRuntime.Cache.Add(cacheName, currencyDeposits, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return currencyDeposits;
        }

        public static int LoadCurrencyDepositsViewCount(int cmid)
        {
            int currencyDepositsCount = 0;

            bool getOldDeposits = true;
            string cacheName = String.Format("CmuneEconomy.GetCurrencyDepositsViewCount.{0}.{1}", cmid, getOldDeposits);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                currencyDepositsCount = (int)HttpRuntime.Cache[cacheName];
            }
            else
            {
                currencyDepositsCount = CmuneEconomy.GetCurrencyDepositsViewCount(cmid, getOldDeposits);
                HttpRuntime.Cache.Add(cacheName, currencyDepositsCount, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return currencyDepositsCount;
        }

        public static decimal LoadTotalCurrencyDeposits(int cmid)
        {
            decimal totalCurrencyDeposits = 0;

            string cacheName = String.Format("CmuneEconomy.GetTotalCurrencyDeposits.{0}", cmid);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                totalCurrencyDeposits = (decimal)HttpRuntime.Cache[cacheName];
            }
            else
            {
                totalCurrencyDeposits = CmuneEconomy.GetTotalCurrencyDeposits(cmid);
                HttpRuntime.Cache.Add(cacheName, totalCurrencyDeposits, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return totalCurrencyDeposits;
        }

        #endregion

        #region Withdrwals

        public static List<ItemTransactionView> LoadItemTransactionsView(int cmid, int pageIndex, int elementsPerPage)
        {
            List<ItemTransactionView> withdrawals = new List<ItemTransactionView>();

            bool getOldTransactions = true;
            string cacheName = String.Format("CmuneEconomy.GetItemTransactionsView.{0}.{1}.{2}.{3}", cmid, pageIndex, elementsPerPage, getOldTransactions);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                withdrawals = (List<ItemTransactionView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                withdrawals = CmuneEconomy.GetItemTransactionsView(cmid, pageIndex, elementsPerPage, getOldTransactions);
                HttpRuntime.Cache.Add(cacheName, withdrawals, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return withdrawals;
        }

        public static int LoadItemTransactionsViewCount(int cmid)
        {
            int withdrawalsCount = 0;

            bool getOldTransactions = true;
            string cacheName = String.Format("CmuneEconomy.GetItemTransactionsViewCount.{0}.{1}", cmid, getOldTransactions);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                withdrawalsCount = (int)HttpRuntime.Cache[cacheName];
            }
            else
            {
                withdrawalsCount = CmuneEconomy.GetItemTransactionsViewCount(cmid, getOldTransactions);
                HttpRuntime.Cache.Add(cacheName, withdrawalsCount, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return withdrawalsCount;
        }

        #endregion

        #region Points deposits

        public static List<PointDepositView> LoadPointDepositView(int cmid, int pageIndex, int elementsPerPage)
        {
            List<PointDepositView> pointsDeposits = new List<PointDepositView>();

            bool getOldPointsDeposits = true;
            string cacheName = String.Format("CmuneEconomy.GetPointDepositView.{0}.{1}.{2}.{3}", cmid, pageIndex, elementsPerPage, getOldPointsDeposits);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsDeposits = (List<PointDepositView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsDeposits = CmuneEconomy.GetPointDepositView(cmid, pageIndex, elementsPerPage, getOldPointsDeposits);
                HttpRuntime.Cache.Add(cacheName, pointsDeposits, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsDeposits;
        }

        public static int LoadPointDepositViewCount(int cmid)
        {
            int pointsDepositsCount = 0;

            bool getOldPointsDeposits = true;
            string cacheName = String.Format("CmuneEconomy.GetPointDepositViewCount.{0}.{1}", cmid, getOldPointsDeposits);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                pointsDepositsCount = (int)HttpRuntime.Cache[cacheName];
            }
            else
            {
                pointsDepositsCount = CmuneEconomy.GetPointDepositViewCount(cmid, getOldPointsDeposits);
                HttpRuntime.Cache.Add(cacheName, pointsDepositsCount, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return pointsDepositsCount;
        }

        #endregion

        #region Stats

        public static List<DailyRanking> LoadDailyStatisticsHistory(int cmid)
        {
            List<DailyRanking> stats = new List<DailyRanking>();

            string cacheName = String.Format("Statistics.GetDailyStatisticsHistory.{0}", cmid);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                stats = (List<DailyRanking>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                stats = Statistics.GetDailyStatisticsHistory(cmid);
                HttpRuntime.Cache.Add(cacheName, stats, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return stats;
        }

        #endregion

        #endregion

        #region Monitoring

        public static List<string> LoadVersionNumbers(int versionsCount)
        {
            List<string> versions = new List<string>();

            string cacheName = String.Format("PhotonMonitoring.GetVersionNumbers.{0}", versionsCount);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                versions = (List<string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                versions = PhotonMonitoring.GetVersionNumbers(versionsCount);
                HttpRuntime.Cache.Add(cacheName, versions, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return versions;
        }

        public static List<int> LoadInstancesId(string versionNumber)
        {
            List<int> instancesId = new List<int>();

            string cacheName = String.Format("PhotonMonitoring.GetInstancesId.{0}", versionNumber);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                instancesId = (List<int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                instancesId = PhotonMonitoring.GetInstancesId(versionNumber);
                HttpRuntime.Cache.Add(cacheName, instancesId, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return instancesId;
        }

        public static Dictionary<DateTime, int> LoadConcurrentUsers(string version, DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> ccu = new Dictionary<DateTime, int>();

            string cacheName = String.Format("PhotonMonitoring.GetInstancesId.{0}.{1}.{2}", version, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                ccu = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                ccu = PhotonMonitoring.GetConcurrentUsers(version, fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, ccu, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return ccu;
        }

        public static Dictionary<DateTime, PhotonMonitoringDetailView> LoadPhotonHealth(int instanceId, DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, PhotonMonitoringDetailView> photonHealth = new Dictionary<DateTime, PhotonMonitoringDetailView>();

            string cacheName = String.Format("PhotonMonitoring.GetPhotonHealth.{0}.{1}.{2}", instanceId, fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                photonHealth = (Dictionary<DateTime, PhotonMonitoringDetailView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                photonHealth = PhotonMonitoring.GetPhotonHealth(instanceId, fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, photonHealth, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return photonHealth;
        }

        public static Dictionary<int, string> LoadInstancesName(string versionNumber)
        {
            Dictionary<int, string> instancesName = new Dictionary<int, string>();

            string cacheName = String.Format("PhotonMonitoring.GetInstancesName.{0}", versionNumber);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                instancesName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                instancesName = PhotonMonitoring.GetInstancesName(versionNumber);
                HttpRuntime.Cache.Add(cacheName, instancesName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return instancesName;
        }

        #endregion

        #region Maps

        /// <summary>
        /// Loads all the map names
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> LoadMapNames()
        {
            Dictionary<int, string> mapsName = new Dictionary<int, string>();

            string cacheName = String.Format("Games.GetMapsName");

            if (HttpRuntime.Cache[cacheName] != null)
            {
                mapsName = (Dictionary<int, string>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                mapsName = Games.GetMapsName();
                HttpRuntime.Cache.Add(cacheName, mapsName, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return mapsName;
        }

        /// <summary>
        /// Loads map usage
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, int> LoadMapUsage(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, int> mapUsage = new Dictionary<int, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMapUsage.{0}.{1}", fromDate, toDate);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                mapUsage = (Dictionary<int, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                mapUsage = UserActivityBusiness.GetMapUsage(fromDate, toDate);
                HttpRuntime.Cache.Add(cacheName, mapUsage, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return mapUsage;
        }

        public static Dictionary<DateTime, int> LoadMapUsage(DateTime fromDate, DateTime toDate, int mapId, GameModeType gameModeId)
        {
            Dictionary<DateTime, int> mapUsage = new Dictionary<DateTime, int>();

            string cacheName = String.Format("UserActivityBusiness.GetMapUsage.{0}.{1}.{2}.{3}", fromDate, toDate, mapId, gameModeId);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                mapUsage = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                mapUsage = UserActivityBusiness.GetMapUsage(fromDate, toDate, mapId, gameModeId);
                HttpRuntime.Cache.Add(cacheName, mapUsage, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return mapUsage;
        }

        #endregion
    }
}