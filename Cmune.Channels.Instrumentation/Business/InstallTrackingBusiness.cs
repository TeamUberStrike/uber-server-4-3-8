using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.DataAccess;
using UberStrike.Core.Types;
using UberStrike.DataCenter.DataAccess;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class InstallTrackingBusiness
    {
        /// <summary>
        /// Get the steps count for all the stepsId accross the time range
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, Dictionary<TutorialStepType, int>> GetTutorialStepsCount(DateTime fromDate, DateTime toDate)
        {
            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, Dictionary<TutorialStepType, int>> result = new Dictionary<DateTime, Dictionary<TutorialStepType, int>>();
                List<TutorialStepType> stepsIds = EnumUtilities.IterateEnum<TutorialStepType>();
                List<DateTime> dates = StatisticsBusiness.GetDaysList(fromDate, toDate);

                foreach (DateTime date in dates)
                {
                    Dictionary<TutorialStepType, int> steps = new Dictionary<TutorialStepType, int>();

                    foreach (TutorialStepType stepId in stepsIds)
                    {
                        steps.Add(stepId, 0);
                    }

                    result.Add(date, steps);
                }

                var query = from s in db.ChannelTutorialConversionFunnels
                            where s.StepDate >= fromDate.ToDateOnly()
                                    && s.StepDate <= toDate.ToDateOnly()
                            group s by new { StepDate = s.StepDate, StepId = s.StepId } into s2
                            select new { StepDate = s2.Key.StepDate, StepId = s2.Key.StepId, StepCount = s2.Sum(s => s.StepCount) };

                foreach (var row in query)
                {
                    result[row.StepDate][(TutorialStepType) row.StepId] = row.StepCount;
                }

                return result;
            }
        }

        /// <summary>
        /// Get a Unity install flow step overall
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="stepType"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetInstallTrackingTotals(DateTime fromDate, DateTime toDate, UserInstallStepType stepType, bool hasUnity)
        {
            return GetInstallTrackingTotals(fromDate, toDate, null, stepType, hasUnity);
        }

        /// <summary>
        /// Get a Unity install flow step for a specific referrer
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="referrerPartnerId"></param>
        /// <param name="stepType"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetInstallTrackingTotals(DateTime fromDate, DateTime toDate, ReferrerPartnerType? referrerPartnerId, UserInstallStepType stepType, bool hasUnity)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, int> temp = new Dictionary<DateTime, int>();

                if (referrerPartnerId != null)
                {
                    var query1 =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.ReferrerId == (int)referrerPartnerId
                            && t.StepType == (int)stepType
                            && t.HasUnity == hasUnity
                        select new { t.StepDate, t.StepCount };

                    foreach (var row in query1)
                    {
                        if (!temp.ContainsKey(row.StepDate))
                        {
                            temp.Add(row.StepDate, 0);
                        }

                        temp[row.StepDate] += row.StepCount;
                    }
                }
                else
                {
                    var query2 =
                      from t in db.UnityConversionFunnels
                      where t.StepDate >= fromDate.ToDateOnly()
                         && t.StepDate <= toDate.ToDateOnly()
                         && t.StepType == (int)stepType
                         && t.HasUnity == hasUnity
                      select new { t.StepDate, t.StepCount };

                    foreach (var row in query2)
                    {
                        if (!temp.ContainsKey(row.StepDate))
                        {
                            temp.Add(row.StepDate, 0);
                        }

                        temp[row.StepDate] += row.StepCount;
                    }
                }

                foreach (KeyValuePair<DateTime, int> kvp in temp)
                {
                    int x;
                    DateTime key = Convert.ToDateTime(kvp.Key.ToShortDateString());

                    // Get current value
                    if (result.TryGetValue(key, out x))
                    {
                        result[key] += (int)kvp.Value;
                    }
                    else
                    {
                        result.Add(key, (int)kvp.Value);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get a Unity install flow step for a specific channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="stepType"></param>
        /// <param name="hasUnity"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetInstallTrackingTotals(DateTime fromDate, DateTime toDate, ChannelType channel, UserInstallStepType stepType, bool hasUnity)
        {
            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                Dictionary<DateTime, int> temp = new Dictionary<DateTime, int>();

                var query =
                    from t in db.UnityConversionFunnels
                    where t.StepDate >= fromDate.ToDateOnly()
                        && t.StepDate <= toDate.ToDateOnly()
                        && t.ChannelId == (int)channel
                        && t.StepType == (int)stepType
                        && t.HasUnity == hasUnity
                    select new { t.StepDate, t.StepCount };

                foreach (var row in query)
                {
                    if (!temp.ContainsKey(row.StepDate))
                    {
                        temp.Add(row.StepDate, 0);
                    }

                    temp[row.StepDate] += row.StepCount;
                }

                foreach (KeyValuePair<DateTime, int> kvp in temp)
                {
                    int x;
                    DateTime key = Convert.ToDateTime(kvp.Key.ToShortDateString());

                    // Get current value
                    if (result.TryGetValue(key, out x))
                    {
                        result[key] += (int)kvp.Value;
                    }
                    else
                    {
                        result.Add(key, (int)kvp.Value);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get the Os distribution for the step no unity for a specific channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetOsNameDistribution(DateTime fromDate, DateTime toDate, ChannelType? channel)
        {
            Dictionary<string, int> osDistribution = new Dictionary<string, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                if (channel.HasValue)
                {
                    var query =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.ChannelId == (int)channel
                            && t.StepType == (int)UserInstallStepType.NoUnity
                            && t.StepDate >= fromDate
                            && t.StepDate <= toDate
                        group t by t.OsName into t2
                        orderby t2.Count() descending
                        select new { OsName = t2.Key, Count = t2.Sum(t => t.StepCount) };

                    osDistribution = query.ToDictionary(t => t.OsName, t => t.Count);
                }
                else
                {
                    var query =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.StepType == (int)UserInstallStepType.NoUnity
                            && t.StepDate >= fromDate
                            && t.StepDate <= toDate
                        group t by t.OsName into t2
                        orderby t2.Count() descending
                        select new { OsName = t2.Key, Count = t2.Sum(t => t.StepCount) };

                    osDistribution = query.ToDictionary(t => t.OsName, t => t.Count);
                }
            }

            return osDistribution;
        }

        /// <summary>
        /// Get the Browser distribution for the step no unity for a specific channel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="channel"></param>
        /// <param name="operatingSystem"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetBrowserDistribution(DateTime fromDate, DateTime toDate, ChannelType channel, string operatingSystem)
        {
            Dictionary<string, int> osDistribution = new Dictionary<string, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query =
                    from t in db.UnityConversionFunnels
                    where t.StepDate >= fromDate.ToDateOnly()
                        && t.StepDate <= toDate.ToDateOnly()
                        && t.ChannelId == (int)channel
                        && t.StepType == (int)UserInstallStepType.NoUnity
                        && t.OsName == operatingSystem
                        && t.BrowserName != "Explorer"
                    group t by t.BrowserName into t2
                    select new { BrowserName = t2.Key, Count = t2.Sum(t => t.StepCount) };

                osDistribution = query.ToDictionary(t => t.BrowserName, t => t.Count);

                if (operatingSystem == "Windows")
                {
                    var query2 =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.ChannelId == (int)channel
                            && t.StepType == (int)UserInstallStepType.NoUnity
                            && t.OsName == operatingSystem
                            && t.BrowserName == "Explorer"
                        group t by t.BrowserVersion into t2
                        select new { BrowserName = "IE " + t2.Key, Count = t2.Sum(t => t.StepCount) };

                    osDistribution = osDistribution.Concat(query2.ToDictionary(t => t.BrowserName, t => t.Count)).ToDictionary(u => u.Key, u => u.Value);
                }
            }

            return osDistribution;
        }

        public static Dictionary<DateTime, int> GetStep(DateTime fromDate, DateTime toDate, UserInstallStepType stepType, ChannelType channel, string operatingSystem, string browserName, string browserVersion, bool hasUnity)
        {
            Dictionary<DateTime, int> steps = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                if (browserVersion.IsNullOrFullyEmpty())
                {
                    var query =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.ChannelId == (int)channel
                            && t.StepType == (int)stepType
                            && t.OsName == operatingSystem
                            && t.BrowserName == browserName
                            && t.HasUnity == hasUnity
                        group t by t.StepDate into t2
                        select new { StepDate = t2.Key, Count = t2.Sum(t => t.StepCount) };

                    steps = query.ToDictionary(t => t.StepDate, t => t.Count);
                }
                else
                {
                    var query =
                        from t in db.UnityConversionFunnels
                        where t.StepDate >= fromDate.ToDateOnly()
                            && t.StepDate <= toDate.ToDateOnly()
                            && t.ChannelId == (int)channel
                            && t.StepType == (int)stepType
                            && t.OsName == operatingSystem
                            && t.BrowserName == browserName
                            && t.BrowserVersion == browserVersion
                            && t.HasUnity == hasUnity
                        group t by t.StepDate into t2
                        select new { StepDate = t2.Key, Count = t2.Sum(t => t.StepCount) };

                    steps = query.ToDictionary(t => t.StepDate, t => t.Count);
                }
            }

            return steps;
        }

        public static Dictionary<DateTime, int> GetStep(DateTime fromDate, DateTime toDate, UserInstallStepType stepType, ChannelType? channel, string osName, string osVersion, string browserName, string browserVersion, ReferrerPartnerType? referrerId, bool hasUnity, bool? isJavaInstallEnabled)
        {
            Dictionary<DateTime, int> steps = new Dictionary<DateTime, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append("SELECT [StepDate] AS StatDate, SUM([StepCount]) AS StepCount FROM [Instrumentation].[dbo].[UnityConversionFunnel] WHERE [StepDate] >= '");
                sqlQuery.Append(fromDate.ToDateOnly());
                sqlQuery.Append("' AND [StepDate] <= '");
                sqlQuery.Append(toDate.ToDateOnly());
                sqlQuery.Append("' AND [StepType] = ");
                sqlQuery.Append((int)stepType);

                if (channel.HasValue)
                {
                    sqlQuery.Append(" AND [ChannelId] = ");
                    sqlQuery.Append((int)channel.Value);
                }

                if (!osName.IsNullOrFullyEmpty())
                {
                    sqlQuery.Append(" AND [OsName] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osName));
                    sqlQuery.Append("'");
                }

                if (!osVersion.IsNullOrFullyEmpty())
                {
                    sqlQuery.Append(" AND [OsVersion] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osVersion));
                    sqlQuery.Append("'");
                }

                if (!browserName.IsNullOrFullyEmpty())
                {
                    sqlQuery.Append(" AND [BrowserName] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(browserName));
                    sqlQuery.Append("'");
                }

                if (!browserVersion.IsNullOrFullyEmpty())
                {
                    sqlQuery.Append(" AND [BrowserVersion] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(browserVersion));
                    sqlQuery.Append("'");
                }

                if (referrerId.HasValue)
                {
                    sqlQuery.Append(" AND [ReferrerId] = ");
                    sqlQuery.Append((int) referrerId);
                }

                sqlQuery.Append(" AND [HasUnity] = ");
                sqlQuery.Append((hasUnity == true) ? "1" : "0");

                if (isJavaInstallEnabled.HasValue)
                {
                    sqlQuery.Append(" AND [IsJavaInstallEnabled] = ");
                    sqlQuery.Append((isJavaInstallEnabled.Value == true) ? "1" : "0");
                }

                sqlQuery.Append(" GROUP BY [StepDate]");

                //CmuneLog.CustomLogToDefaultPath("generated-sql.log", sqlQuery.ToString());

                IEnumerable<CustomQueryStep> customSteps = db.ExecuteQuery<CustomQueryStep>(sqlQuery.ToString());
                steps = customSteps.ToDictionary(u => u.StatDate, u => u.StepCount);
            }

            return steps;
        }

        private class CustomQueryStep
        {
            #region Properties

            public DateTime StatDate { get; private set; }
            public int StepCount { get; private set; }

            #endregion

            #region Constructors

            public CustomQueryStep()
            {
            }

            #endregion
        }

        public static UnityInstallationDisplay GetSteps(string trackingId)
        {
            UnityInstallationDisplay steps = null;

            using (UberstrikeDataContext uberstrikeDb = new UberstrikeDataContext())
            {
                var query =
                    from u in uberstrikeDb.UserInstalls
                    join s in uberstrikeDb.UserInstallSteps on u.UserInstallId equals s.UserInstallId
                    where u.TrackingId == trackingId
                    orderby s.StepDate
                    select new { InstallId = u.UserInstallId, TrackingId = u.TrackingId, Channel = u.ChannelId, StepType = s.StepType, StepDate = s.StepDate, OsName = u.OsName, OsVersion = u.OsVersion, BrowserName = u.BrowserName, BrowserVersion = u.BrowserVersion, IsJavaInstallEnabled = u.IsJavaInstallEnabled, Ip = u.Ip, ReferrerId = u.ReferrerId, UserAgent = u.UserAgent, HasUnity = u.HasUnity };

                int i = 0;
                List<UnityInstallationStepDisplay> allSteps = new List<UnityInstallationStepDisplay>();

                foreach (var row in query)
                {
                    allSteps.Add(new UnityInstallationStepDisplay(row.InstallId, (UserInstallStepType)row.StepType, row.StepDate));

                    if (i == 0)
                    {
                        steps = new UnityInstallationDisplay(row.InstallId,
                                                                row.TrackingId,
                                                                (ChannelType)row.Channel,
                                                                (ReferrerPartnerType)row.ReferrerId,
                                                                allSteps,
                                                                row.OsName,
                                                                row.OsVersion,
                                                                row.BrowserName,
                                                                row.BrowserVersion,
                                                                row.IsJavaInstallEnabled,
                                                                TextUtilities.InetNToA(row.Ip),
                                                                row.UserAgent,
                                                                row.HasUnity);
                    }

                    i++;
                }
            }

            return steps;
        }

        public static Dictionary<string, int> GetBrowserNamesOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion)
        {
            Dictionary<string, int> browserNames = new Dictionary<string, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append("SELECT [BrowserName] AS Name, SUM([StepCount]) AS StepCount FROM [Instrumentation].[dbo].[UnityConversionFunnel] WHERE [StepDate] >= '");
                sqlQuery.Append(fromDate.ToDateOnly());
                sqlQuery.Append("' AND [StepDate] <= '");
                sqlQuery.Append(toDate.ToDateOnly());
                sqlQuery.Append("' AND [StepType] = ");
                sqlQuery.Append((int)UserInstallStepType.NoUnity);

                if (channel.HasValue)
                {
                    sqlQuery.Append(" AND [ChannelId] = ");
                    sqlQuery.Append((int)channel.Value);
                }

                if (osName != "-1")
                {
                    sqlQuery.Append(" AND [OsName] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osName));
                    sqlQuery.Append("'");
                }

                if (osVersion != "-1")
                {
                    sqlQuery.Append(" AND [OsVersion] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osVersion));
                    sqlQuery.Append("'");
                }

                sqlQuery.Append(" GROUP BY [BrowserName] ORDER BY [StepCount] DESC");

                //CmuneLog.CustomLogToDefaultPath("generated-sql.log", sqlQuery.ToString());

                IEnumerable<CustomQueryParameter> customParameters = db.ExecuteQuery<CustomQueryParameter>(sqlQuery.ToString());
                browserNames = customParameters.ToDictionary(u => u.Name, u => u.StepCount);
            }

            return browserNames;
        }

        private class CustomQueryParameter
        {
            #region Properties

            public string Name { get; private set; }
            public int StepCount { get; private set; }

            #endregion

            #region Constructors

            public CustomQueryParameter()
            {
            }

            #endregion
        }

        public static Dictionary<string, int> GetOsVersionsOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName)
        {
            Dictionary<string, int> osVersions = new Dictionary<string, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                if (channel.HasValue && osName != "-1")
                {
                    var query = from u in db.UnityConversionFunnels
                                where u.StepDate >= fromDate
                                    && u.StepDate <= toDate
                                    && u.OsName == osName
                                    && u.ChannelId == (int) channel.Value
                                    && u.StepType == (int)UserInstallStepType.NoUnity
                                group u by u.OsVersion into t2
                                orderby t2.Count() descending
                                select new { OsVersion = t2.Key, Count = t2.Count() };

                    osVersions = query.ToDictionary(t => t.OsVersion, t => t.Count);
                }
                else if (channel.HasValue)
                {
                    var query = from u in db.UnityConversionFunnels
                                where u.StepDate >= fromDate
                                    && u.StepDate <= toDate
                                    && u.ChannelId == (int)channel.Value
                                    && u.StepType == (int)UserInstallStepType.NoUnity
                                group u by u.OsVersion into t2
                                orderby t2.Count() descending
                                select new { OsVersion = t2.Key, Count = t2.Count() };

                    osVersions = query.ToDictionary(t => t.OsVersion, t => t.Count);
                }
                else if (osName != "-1")
                {
                    var query = from u in db.UnityConversionFunnels
                                where u.StepDate >= fromDate
                                    && u.StepDate <= toDate
                                    && u.OsName == osName
                                    && u.StepType == (int)UserInstallStepType.NoUnity
                                group u by u.OsVersion into t2
                                orderby t2.Count() descending
                                select new { OsVersion = t2.Key, Count = t2.Count() };

                    osVersions = query.ToDictionary(t => t.OsVersion, t => t.Count);
                }
            }

            return osVersions;
        }

        public static Dictionary<string, int> GetBrowserVersionsOrderedByCount(DateTime fromDate, DateTime toDate, ChannelType? channel, string osName, string osVersion, string browserName)
        {
            Dictionary<string, int> browserVersions = new Dictionary<string, int>();

            var db = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append("SELECT [BrowserVersion] AS Name, SUM([StepCount]) AS StepCount FROM [Instrumentation].[dbo].[UnityConversionFunnel] WHERE [StepDate] >= '");
                sqlQuery.Append(fromDate.ToDateOnly());
                sqlQuery.Append("' AND [StepDate] <= '");
                sqlQuery.Append(toDate.ToDateOnly());
                sqlQuery.Append("' AND [StepType] = ");
                sqlQuery.Append((int)UserInstallStepType.NoUnity);

                if (channel.HasValue)
                {
                    sqlQuery.Append(" AND [ChannelId] = ");
                    sqlQuery.Append((int)channel.Value);
                }

                if (osName != "-1")
                {
                    sqlQuery.Append(" AND [OsName] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osName));
                    sqlQuery.Append("'");
                }

                if (osVersion != "-1")
                {
                    sqlQuery.Append(" AND [OsVersion] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(osVersion));
                    sqlQuery.Append("'");
                }

                if (browserName != "-1")
                {
                    sqlQuery.Append(" AND [BrowserName] = '");
                    sqlQuery.Append(TextUtilities.ProtectSqlField(browserName));
                    sqlQuery.Append("'");
                }

                sqlQuery.Append(" GROUP BY [BrowserVersion] ORDER BY [StepCount] DESC");

                //CmuneLog.CustomLogToDefaultPath("generated-sql.log", sqlQuery.ToString());

                IEnumerable<CustomQueryParameter> customParameters = db.ExecuteQuery<CustomQueryParameter>(sqlQuery.ToString());
                browserVersions = customParameters.ToDictionary(u => u.Name, u => u.StepCount);
            }

            return browserVersions;
        }
    }
}