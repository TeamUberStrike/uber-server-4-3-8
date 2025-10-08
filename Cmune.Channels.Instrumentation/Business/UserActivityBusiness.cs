using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Instrumentation.DataAccess;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Models.Display;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business
{
    class PlayerLevelDistribution
    {
        #region Properties

        public int Level { get; private set; }
        public int Count { get; private set; }

        #endregion
    }

    public static class UserActivityBusiness
    {
        #region Daily Active Users

        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                    from m in db.ChannelDaus
                    where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly()
                    select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily active users by referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the daily active users by referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId, int regionId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId && m.Region == regionId
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.ChannelId == (int)channelId
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any referrers together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    if (result.ContainsKey(day))
                        result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the DAU per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, int>> GetDailyActiveUsersByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> result = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly()
                   group m by new { m.ChannelId, m.ActiveDate } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, ActiveDate = m2.Key.ActiveDate, ActiveCount = m2.Sum(m => m.ActiveCount) };

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, int> timeRange = new Dictionary<DateTime, int>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.ActiveDate] = row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    if (result.ContainsKey(day))
                        result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<DateTime, int> GetDailyActiveUsers(DateTime fromDate, DateTime toDate, int regionId, ChannelType channel)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId && m.ChannelId == (int) channel
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    if (result.ContainsKey(day))
                        result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> GetDailyActiveUsersByChannels(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> result = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelDaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId
                   group m by new { m.ChannelId, m.ActiveDate } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, ActiveDate = m2.Key.ActiveDate, ActiveCount = m2.Sum(m => m.ActiveCount) }; ;

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, int> timeRange = new Dictionary<DateTime, int>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.ActiveDate] = row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<int, int> GetDailyActiveUsersByCountries(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, int> dauByCountries = new Dictionary<int, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from cD in instrumentationDb.ChannelDaus
                            where cD.ActiveDate >= fromDate && cD.ActiveDate < toDate
                            group cD by cD.Region into cD2
                            select new
                            {
                                Region = cD2.Key,
                                ActiveCount = cD2.Sum(t => t.ActiveCount)
                            };

                dauByCountries = query.ToDictionary(t => t.Region, t => t.ActiveCount);
            }

            return dauByCountries;
        }

        #endregion

        #region Monthly Active Users

        public static Dictionary<DateTime, int> GetMonthlyActiveUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly()
                   select new { m.ActiveDate, m.ActiveCount };

                // Add the MAU for each day
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the monthly active users by referrer Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetMonthlyActiveUsers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrerId
                   select new { m.ActiveDate, m.ActiveCount };

                // Add the MAU for each day
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }

            }
            return result;
        }

        /// <summary>
        /// Get the monthly active users by channel Id
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetMonthlyActiveUsers(DateTime fromDate, DateTime toDate, ChannelType channelId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.ChannelId == (int)channelId
                   select new { m.ActiveDate, m.ActiveCount };

                // Add the DAU for each day
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any referrers together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the MAU per channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<ChannelType, Dictionary<DateTime, int>> GetMonthlyActiveUsersByChannels(DateTime fromDate, DateTime toDate)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> result = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly()
                   group m by new { m.ChannelId, m.ActiveDate } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, ActiveDate = m2.Key.ActiveDate, ActiveCount = m2.Sum(m => m.ActiveCount) };

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, int> timeRange = new Dictionary<DateTime, int>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.ActiveDate] = row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<DateTime, int> GetMonthlyActiveUsers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    if (result.ContainsKey(day))
                        result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<DateTime, int> GetMonthlyActiveUsers(DateTime fromDate, DateTime toDate, int regionId, ChannelType channel)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId && m.ChannelId == (int)channel
                   select new { m.ActiveDate, m.ActiveCount };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.ActiveDate.ToDateOnly();

                    if (result.ContainsKey(day))
                        result[day] += row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<ChannelType, Dictionary<DateTime, int>> GetMonthlyActiveUsersByChannels(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<ChannelType, Dictionary<DateTime, int>> result = new Dictionary<ChannelType, Dictionary<DateTime, int>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelMaus
                   where m.ActiveDate >= fromDate.ToDateOnly() && m.ActiveDate <= toDate.ToDateOnly() && m.Region == (int)regionId
                   group m by new { m.ChannelId, m.ActiveDate } into m2
                   select new { ChannelId = (ChannelType)m2.Key.ChannelId, ActiveDate = m2.Key.ActiveDate, ActiveCount = m2.Sum(m => m.ActiveCount) };

                foreach (var row in query)
                {
                    if (!result.ContainsKey(row.ChannelId))
                    {
                        // Make sure all days in the range appear in the result
                        Dictionary<DateTime, int> timeRange = new Dictionary<DateTime, int>();

                        foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                        {
                            timeRange.Add(day, 0);
                        }

                        result.Add(row.ChannelId, timeRange);
                    }

                    result[row.ChannelId][row.ActiveDate] = row.ActiveCount;
                }
            }

            return result;
        }

        public static Dictionary<int, int> GetMonthlyActiveUsersByCountries(DateTime toDate)
        {
            Dictionary<int, int> mauByCountries = new Dictionary<int, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from cD in instrumentationDb.ChannelMaus
                            where cD.ActiveDate == toDate
                            group cD by cD.Region into cD2
                            select new
                            {
                                Region = cD2.Key,
                                ActiveCount = cD2.Sum(t => t.ActiveCount)
                            };

                mauByCountries = query.ToDictionary(t => t.Region, t => t.ActiveCount);
            }

            return mauByCountries;
        }

        public static Dictionary<int, int> GetMonthlyActiveUsersByCountries(DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<int, int> mauByCountries = new Dictionary<int, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from cD in instrumentationDb.ReferrerMaus
                            where cD.ActiveDate == toDate && cD.ReferrerId == (int) referrer
                            group cD by cD.Region into cD2
                            select new
                            {
                                Region = cD2.Key,
                                ActiveCount = cD2.Sum(t => t.ActiveCount)
                            };

                mauByCountries = query.ToDictionary(t => t.Region, t => t.ActiveCount);
            }

            return mauByCountries;
        }

        #endregion

        #region New members

        public static Dictionary<DateTime, int> GetNewMembers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly()
                   select new { m.RegistrationDate, m.RegistrationCount };

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group any referrers together
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate.ToDateOnly();

                    result[day] += row.RegistrationCount;
                }

            }
            return result;
        }

        public static Dictionary<DateTime, int> GetNewMembers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly() && m.ChannelId == (int) channel
                   select new { m.RegistrationDate, m.RegistrationCount };

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate.ToDateOnly();

                    result[day] += row.RegistrationCount;
                }

            }
            return result;
        }

        public static Dictionary<DateTime, int> GetNewMembers(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ChannelNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly() && m.Region == regionId
                   select new { m.RegistrationDate, m.RegistrationCount };

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate.ToDateOnly();

                    result[day] += row.RegistrationCount;
                }

            }
            return result;
        }

        public static Dictionary<DateTime, int> GetNewMembers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrer
                   select new { m.RegistrationDate, m.RegistrationCount };

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate.ToDateOnly();

                    result[day] += row.RegistrationCount;
                }

            }
            return result;
        }

        public static Dictionary<DateTime, int> GetNewMembers(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer, int regionId)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrer && m.Region == regionId
                   select new { m.RegistrationDate, m.RegistrationCount };

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    result.Add(day, 0);
                }

                // Group rows together
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate.ToDateOnly();

                    result[day] += row.RegistrationCount;
                }

            }
            return result;
        }

        public static Dictionary<int, int> GetNewMembersCountByCountries(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                   from m in db.ReferrerNewMembers
                   where m.RegistrationDate >= fromDate.ToDateOnly() && m.RegistrationDate <= toDate.ToDateOnly() && m.ReferrerId == (int)referrer
                   group m by m.Region into m2
                   select new { RegionId =  m2.Key, RegistrationCount = m2.Sum(m => m.RegistrationCount) };

                foreach (var row in query)
                {
                    int regionId = row.RegionId;

                    result.Add(regionId, row.RegistrationCount);
                }

            }

            return result;
        }

        #endregion

        public static Dictionary<DateTime, IntPair> GetNewVsReturningUsers(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> dailyActiveUsers = GetDailyActiveUsers(fromDate, toDate);
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                // Get all created accounts for this timeframe
                var query =
                    from m in db.ChannelNewMembers
                    where m.RegistrationDate >= fromDate && m.RegistrationDate <= toDate
                    group m by m.RegistrationDate into m2
                    select new { RegistrationDate = m2.Key, RegistrationCount = m2.Sum(m => m.RegistrationCount) };

                // Add the DAU for each day
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int dau = 0;
                    dailyActiveUsers.TryGetValue(day, out dau);
                    result.Add(day, new IntPair(0, dau));
                }

                // Go through all created accounts and add the created accounts, and subtstract the created accounts from the dau
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate;

                    // Check if this day is contained in the result
                    if (result.ContainsKey(day))
                    {
                        // If there is no DAU, then just ignore the day (since DAU counts new and returning in total
                        if (result[day].Int2 > 0)
                        {
                            result[day] = new IntPair(row.RegistrationCount, result[day].Int2 - row.RegistrationCount);
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<DateTime, IntPair> GetNewVsReturningUsers(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> dailyActiveUsers = GetDailyActiveUsers(fromDate, toDate, channel);
            Dictionary<DateTime, IntPair> result = new Dictionary<DateTime, IntPair>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                // Get all created accounts for this timeframe
                var query =
                    from m in db.ChannelNewMembers
                    where m.RegistrationDate >= fromDate && m.RegistrationDate <= toDate && m.ChannelId == (int) channel
                    group m by m.RegistrationDate into m2
                    select new { RegistrationDate = m2.Key, RegistrationCount = m2.Sum(m => m.RegistrationCount) };

                // Add the DAU for each day
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int dau = 0;
                    dailyActiveUsers.TryGetValue(day, out dau);
                    result.Add(day, new IntPair(0, dau));
                }

                // Go through all created accounts and add the created accounts, and subtstract the created accounts from the dau
                foreach (var row in query)
                {
                    DateTime day = row.RegistrationDate;

                    // Check if this day is contained in the result
                    if (result.ContainsKey(day))
                    {
                        // If there is no DAU, then just ignore the day (since DAU counts new and returning in total
                        if (result[day].Int2 > 0)
                        {
                            result[day] = new IntPair(row.RegistrationCount, result[day].Int2 - row.RegistrationCount);
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<DateTime, float> GetPlayRate(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> dailyActiveUsers = GetDailyActiveUsers(fromDate, toDate);
            Dictionary<DateTime, int> monthlyActiveUsers = GetMonthlyActiveUsers(fromDate, toDate);

            Dictionary<DateTime, float> result = new Dictionary<DateTime, float>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int mau = 0;
                int dau = 0;
                monthlyActiveUsers.TryGetValue(day, out mau);
                dailyActiveUsers.TryGetValue(day, out dau);

                if (mau > 0 && dau > 0)
                    result.Add(day, (float)dau / (float)mau);
                else
                    result.Add(day, 0);
            }

            return result;
        }

        public static Dictionary<DateTime, float> GetPlayRate(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> dailyActiveUsers = AdminCache.LoadDailyActiveUsers(fromDate, toDate, channel);
            Dictionary<DateTime, int> monthlyActiveUsers = AdminCache.LoadMonthlyActiveUsers(fromDate, toDate, channel);

            Dictionary<DateTime, float> result = new Dictionary<DateTime, float>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int mau = 0;
                int dau = 0;
                monthlyActiveUsers.TryGetValue(day, out mau);
                dailyActiveUsers.TryGetValue(day, out dau);

                if (mau > 0 && dau > 0)
                    result.Add(day, (float)dau / (float)mau);
                else
                    result.Add(day, 0);
            }

            return result;
        }

        public static Dictionary<DateTime, float> GetPlayRate(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrer)
        {
            Dictionary<DateTime, int> dailyActiveUsers = AdminCache.LoadDailyActiveUsers(fromDate, toDate, referrer);
            Dictionary<DateTime, int> monthlyActiveUsers = AdminCache.LoadMonthlyActiveUsers(fromDate, toDate, referrer);

            Dictionary<DateTime, float> result = new Dictionary<DateTime, float>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
            {
                int mau = 0;
                int dau = 0;
                monthlyActiveUsers.TryGetValue(day, out mau);
                dailyActiveUsers.TryGetValue(day, out dau);

                if (mau > 0 && dau > 0)
                    result.Add(day, (float)dau / (float)mau);
                else
                    result.Add(day, 0);
            }

            return result;
        }

        /// <summary>
        /// Get the player level distribution
        /// </summary>
        /// <param name="statDate"></param>
        /// <param name="getActivePlayersOnly"></param>
        /// <returns></returns>
        public static Dictionary<int, int> GetPlayerLevelDistribution(DateTime statDate, bool getActivePlayersOnly)
        {
            Dictionary<int, int> playerLevelDistribution = new Dictionary<int, int>();

            int minLevel = UberStrikeCommonConfig.DefaultLevel;
            int maxLevel = UberStrikeCommonConfig.LevelCap;

            for (int i = minLevel; i <= maxLevel; i++)
            {
                playerLevelDistribution.Add(i, 0);
            }

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                if (getActivePlayersOnly)
                {
                    var query = from p in instrumentationDb.PlayersLevelDistributions
                                where p.StatDate == statDate.ToDateOnly()
                                select new { Level = p.PlayerLevel, Count = p.ActivePlayerCount };

                    foreach (var row in query)
                    {
                        playerLevelDistribution[row.Level] = row.Count;
                    }
                }
                else
                {
                    var query = from p in instrumentationDb.PlayersLevelDistributions
                                where p.StatDate == statDate.ToDateOnly()
                                select new { Level = p.PlayerLevel, Count = p.PlayerCount };

                    foreach (var row in query)
                    {
                        playerLevelDistribution[row.Level] = row.Count;
                    }
                }
            }

            return playerLevelDistribution;
        }

        /// <summary>
        /// Get the maximum Cmid
        /// </summary>
        /// <returns></returns>
        public static int GetMaxCmid()
        {
            int maxCmid = 0;

            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                maxCmid = cmuneDb.Members.Select(m => m.CMID).Max();
            }

            return maxCmid;
        }

        /// <summary>
        /// Get the total members count
        /// </summary>
        /// <returns></returns>
        public static int GetMembersCount()
        {
            int membersCount = 0;

            var cmuneDb = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                membersCount = cmuneDb.Members.Count();
            }

            return membersCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOriginDisplay GetMemberOrigin(int cmid)
        {
            using (InstrumentationDataContext instrumentationDb = new InstrumentationDataContext())
            {
                MemberOriginDisplay memberOriginDisplay = null;

                MemberOriginStat memberOrigin = instrumentationDb.MemberOriginStats.SingleOrDefault(m => m.Cmid == cmid);

                if (memberOrigin != null)
                {
                    Dictionary<int, string> countriesName = AdminCache.LoadCountriesName();

                    memberOriginDisplay = new MemberOriginDisplay(memberOrigin.Cmid,
                                                                    (ChannelType)memberOrigin.Channel,
                                                                    (ReferrerPartnerType)memberOrigin.ReferrerId,
                                                                    memberOrigin.Region,
                                                                    countriesName[memberOrigin.Region],
                                                                    memberOrigin.FriendsCount,
                                                                    memberOrigin.ActiveFriendsCount);
                }

                return memberOriginDisplay;
            }
        }

        #region Retention

        public static Dictionary<DateTime, Dictionary<int, decimal>> GetRetentionCohorts(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from r in db.ChannelRetentionCohortsNews
                            where r.RetentionDate >= fromDate && r.RetentionDate <= toDate
                            group r by r.RetentionDate into r2
                            select new { RetentionDate = r2.Key, OneDayRetention = r2.Sum(r => r.OneDayRetention), ThreeDaysRetention = r2.Sum(r => r.ThreeDaysRetention), SevenDaysRetention = r2.Sum(r => r.SevenDaysRetention) };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    cohorts.Add(day, new Dictionary<int, decimal> { {1, 0}, {3, 0}, {7, 0} });
                }

                Dictionary<DateTime, int> newMembers = AdminCache.LoadNewMembers(fromDate, toDate);

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.RetentionDate.ToDateOnly();

                    if (newMembers[day] != 0)
                    {
                        cohorts[day][1] = (decimal) row.OneDayRetention / newMembers[day];
                        cohorts[day][3] = (decimal) row.ThreeDaysRetention / newMembers[day];
                        cohorts[day][7] = (decimal) row.SevenDaysRetention / newMembers[day];
                    }
                }
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> GetRetentionCohorts(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from r in db.ChannelRetentionCohortsNews
                            where r.RetentionDate >= fromDate && r.RetentionDate <= toDate && r.ChannelId == (int) channel
                            group r by r.RetentionDate into r2
                            select new { RetentionDate = r2.Key, OneDayRetention = r2.Sum(r => r.OneDayRetention), ThreeDaysRetention = r2.Sum(r => r.ThreeDaysRetention), SevenDaysRetention = r2.Sum(r => r.SevenDaysRetention) };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    cohorts.Add(day, new Dictionary<int, decimal> { { 1, 0 }, { 3, 0 }, { 7, 0 } });
                }

                Dictionary<DateTime, int> newMembers = AdminCache.LoadNewMembers(fromDate, toDate, channel);

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.RetentionDate.ToDateOnly();

                    if (newMembers[day] != 0)
                    {
                        cohorts[day][1] = (decimal) row.OneDayRetention / newMembers[day];
                        cohorts[day][3] = (decimal) row.ThreeDaysRetention / newMembers[day];
                        cohorts[day][7] = (decimal) row.SevenDaysRetention / newMembers[day];
                    }
                }
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> GetRetentionCohorts(DateTime fromDate, DateTime toDate, int regionId)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from r in db.ChannelRetentionCohortsNews
                            where r.RetentionDate >= fromDate && r.RetentionDate <= toDate && r.Region == regionId
                            group r by r.RetentionDate into r2
                            select new { RetentionDate = r2.Key, OneDayRetention = r2.Sum(r => r.OneDayRetention), ThreeDaysRetention = r2.Sum(r => r.ThreeDaysRetention), SevenDaysRetention = r2.Sum(r => r.SevenDaysRetention) };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    cohorts.Add(day, new Dictionary<int, decimal> { { 1, 0 }, { 3, 0 }, { 7, 0 } });
                }

                Dictionary<DateTime, int> newMembers = AdminCache.LoadNewMembers(fromDate, toDate, regionId);

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.RetentionDate.ToDateOnly();

                    if (newMembers[day] != 0)
                    {
                        cohorts[day][1] = (decimal) row.OneDayRetention / newMembers[day];
                        cohorts[day][3] = (decimal) row.ThreeDaysRetention / newMembers[day];
                        cohorts[day][7] = (decimal) row.SevenDaysRetention / newMembers[day];
                    }
                }
            }

            return cohorts;
        }

        public static Dictionary<DateTime, Dictionary<int, decimal>> GetRetentionCohorts(DateTime fromDate, DateTime toDate, ReferrerPartnerType referrerId)
        {
            Dictionary<DateTime, Dictionary<int, decimal>> cohorts = new Dictionary<DateTime, Dictionary<int, decimal>>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from r in db.ReferrerRetentionCohortsNews
                            where r.RetentionDate >= fromDate && r.RetentionDate <= toDate && r.ReferrerId == (int)referrerId
                            group r by r.RetentionDate into r2
                            select new { RetentionDate = r2.Key, OneDayRetention = r2.Sum(r => r.OneDayRetention), ThreeDaysRetention = r2.Sum(r => r.ThreeDaysRetention), SevenDaysRetention = r2.Sum(r => r.SevenDaysRetention) };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    cohorts.Add(day, new Dictionary<int, decimal> { { 1, 0 }, { 3, 0 }, { 7, 0 } });
                }

                Dictionary<DateTime, int> newMembers = AdminCache.LoadNewMembers(fromDate, toDate, referrerId);

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    DateTime day = row.RetentionDate.ToDateOnly();

                    if (newMembers[day] != 0)
                    {
                        cohorts[day][1] = (decimal) row.OneDayRetention / newMembers[day];
                        cohorts[day][3] = (decimal) row.ThreeDaysRetention / newMembers[day];
                        cohorts[day][7] = (decimal) row.SevenDaysRetention / newMembers[day];
                    }
                }
            }

            return cohorts;
        }

        #endregion

        #region Friends

        public static Dictionary<DateTime, int> GetAverageFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> averageFriends = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerFriends
                            where p.StatDate >= fromDate && p.StatDate <= toDate && p.ChannelId == (int)channel
                            select new { p.StatDate, p.AverageFriendsCount };

                Dictionary<DateTime, int> averageFriendsTmp = query.ToDictionary(u => u.StatDate, u => u.AverageFriendsCount);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int averageFriendsCount = 0;
                    averageFriendsTmp.TryGetValue(day, out averageFriendsCount);

                    averageFriends.Add(day, averageFriendsCount);
                }                
            }

            return averageFriends;
        }

        public static Dictionary<DateTime, int> GetAverageActiveFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> averageActiveFriends = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerFriends
                            where p.StatDate >= fromDate && p.StatDate <= toDate && p.ChannelId == (int)channel
                            select new { p.StatDate, p.AverageActiveFriendsCount };

                Dictionary<DateTime, int> averageActiveFriendsTmp = query.ToDictionary(u => u.StatDate, u => u.AverageActiveFriendsCount);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int averageActiveFriendsCount = 0;
                    averageActiveFriendsTmp.TryGetValue(day, out averageActiveFriendsCount);

                    averageActiveFriends.Add(day, averageActiveFriendsCount);
                }
            }

            return averageActiveFriends;
        }

        public static Dictionary<DateTime, int> GetMedianFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> medianFriends = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerFriends
                            where p.StatDate >= fromDate && p.StatDate <= toDate && p.ChannelId == (int)channel
                            select new { p.StatDate, p.MedianFriendsCount };

                Dictionary<DateTime, int>  medianFriendsTmp = query.ToDictionary(u => u.StatDate, u => u.MedianFriendsCount);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int medianFriendsCount = 0;
                    medianFriendsTmp.TryGetValue(day, out medianFriendsCount);

                    medianFriends.Add(day, medianFriendsCount);
                }
            }

            return medianFriends;
        }

        public static Dictionary<DateTime, int> GetMedianActiveFriendsCount(DateTime fromDate, DateTime toDate, ChannelType channel)
        {
            Dictionary<DateTime, int> medianActiveFriends = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from p in db.PlayerFriends
                            where p.StatDate >= fromDate && p.StatDate <= toDate && p.ChannelId == (int)channel
                            select new { p.StatDate, p.MedianActiveFriendsCount };

                Dictionary<DateTime, int> medianActiveFriendsTmp = query.ToDictionary(u => u.StatDate, u => u.MedianActiveFriendsCount);

                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    int medianActiveFriendsCount = 0;
                    medianActiveFriendsTmp.TryGetValue(day, out medianActiveFriendsCount);

                    medianActiveFriends.Add(day, medianActiveFriendsCount);
                }
            }

            return medianActiveFriends;
        }

        #endregion

        #region Maps

        /// <summary>
        /// Get the maps Id ordered by usage
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, int> GetMapUsage(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, int> mapUsage = new Dictionary<int, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from m in instrumentationDb.MapUsageStats
                            where m.StatDate >= fromDate && m.StatDate <= toDate
                            group m by m.MapId into m2
                            select new { MapId = m2.Key, PlayersTotal = m2.Sum(m => m.PlayersTotal) };

                mapUsage = query.OrderByDescending(m => m.PlayersTotal).ToDictionary(m => m.MapId, m => m.PlayersTotal);
            }

            return mapUsage;
        }

        /// <summary>
        /// Get the map usage
        /// </summary>
        /// <param name="fromDate">included</param>
        /// <param name="toDate">included</param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetMapUsage(DateTime fromDate, DateTime toDate, int mapId, GameModeType gameModeId)
        {
            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, int> mapUsage = new Dictionary<DateTime, int>();

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    mapUsage.Add(day, 0);
                }

                if (gameModeId == GameModeType.None)
                {
                    var query = from m in instrumentationDb.MapUsageStats
                                where m.MapId == mapId && m.StatDate >= fromDate && m.StatDate <= toDate
                                group m by m.StatDate into m2
                                select new { StatDate = m2.Key, PlayersTotal = m2.Sum(m => m.PlayersTotal) };

                    foreach (var row in query)
                    {
                        mapUsage[row.StatDate] += row.PlayersTotal;
                    }
                }
                else
                {
                    var query = from m in instrumentationDb.MapUsageStats
                                where m.MapId == mapId && m.StatDate >= fromDate && m.StatDate <= toDate && m.GameModeId == (int) gameModeId
                                group m by m.StatDate into m2
                                select new { StatDate = m2.Key, PlayersTotal = m2.Sum(m => m.PlayersTotal) };

                    foreach (var row in query)
                    {
                        mapUsage[row.StatDate] += row.PlayersTotal;
                    }
                }

                return mapUsage;
            }
        }

        #endregion
    }
}