using Cmune.DataCenter.Common.Entities;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using System;
using System.Linq;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Utils;

namespace Cmune.Instrumentation.Monitoring.Business
{
    public class UnityExceptionsService
    {
        public static void RecordException(string exceptionType, string exceptionMessage, string stacktrace, BuildType build, ChannelType channel, string buildNumber, int cmid, string exceptionData)
        {
            exceptionType = StandardizeExceptionType(exceptionType);
            exceptionMessage = StandardizeExceptionMessage(exceptionMessage);
            stacktrace = StandardizeStacktrace(stacktrace);

            string faultiveFunction = string.Empty;

            if (!stacktrace.IsNullOrFullyEmpty())
            {
                int faultiveFunctionEndIndex = stacktrace.IndexOf('(');

                if (faultiveFunctionEndIndex > 0)
                {
                    string faultiveFunctionTmp = stacktrace.ShortenText(faultiveFunctionEndIndex);
                    int faultiveFunctionStartIndex = faultiveFunctionTmp.LastIndexOf('.');

                    if (faultiveFunctionStartIndex > 0 && faultiveFunctionEndIndex > faultiveFunctionStartIndex)
                    {
                        faultiveFunction = faultiveFunctionTmp.Substring(faultiveFunctionStartIndex + 1);
                        faultiveFunction = faultiveFunction.Trim();
                    }
                }
            }

            string stacktraceHash = Crypto.fncSHA256Encrypt(stacktrace + exceptionMessage);
            DateTime exceptionTime = DateTime.Now;
            exceptionData = StandardizeExceptionData(exceptionData);
            buildNumber = StandardizeBuildNumber(buildNumber);

            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                UnityException exception = new UnityException();
                exception.Build = (int)build;
                exception.BuildNumber = buildNumber;
                exception.Channel = (int)channel;
                exception.Cmid = cmid;
                exception.ExceptionData = exceptionData;
                exception.ExceptionMessage = exceptionMessage;
                exception.ExceptionTime = exceptionTime;
                exception.ExceptionType = exceptionType;
                exception.FaultiveFunction = faultiveFunction;
                exception.Stacktrace = stacktrace;
                exception.StacktraceHash = stacktraceHash;

                db.UnityExceptions.InsertOnSubmit(exception);
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        public static List<UnityExceptionGroupSummaryView> GetExceptionGroups(DateTime fromDate, DateTime toDate)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                //var query = (from u in db.UnityExceptions
                //            where u.ExceptionTime >= fromDate && u.ExceptionTime < toDate
                //             group u by new { StacktraceHash = u.StacktraceHash, ExceptionType = u.ExceptionType, ExceptionMessage = u.ExceptionMessage, FaultiveFunction = u.FaultiveFunction } into u2
                //             select new { StacktraceHash = u2.Key.StacktraceHash, ExceptionType = u2.Key.ExceptionType, ExceptionMessage = u2.Key.ExceptionMessage, FaultiveFunction = u2.Key.FaultiveFunction, ExceptionCount = u2.Count(), LatestException = u2.Max(u => u.ExceptionTime) });

                var query1 = (from u1 in db.UnityExceptions
                             group u1 by new { StacktraceHash = u1.StacktraceHash } into ug1
                             select new {StacktraceHash = ug1.Key.StacktraceHash, NumberOfExceptions = ug1.Count(), UnityExceptionId = ug1.Max(d=>d.UnityExceptionId)});

                var query = (from u2 in db.UnityExceptions
                              join u3 in query1 on new { u2.StacktraceHash, u2.UnityExceptionId } equals new { u3.StacktraceHash, u3.UnityExceptionId }
                             select new { StacktraceHash = u2.StacktraceHash, ExceptionType = u2.ExceptionType, ExceptionMessage = u2.ExceptionMessage, FaultiveFunction = u2.FaultiveFunction, ExceptionCount = u3.NumberOfExceptions, LatestException = u2.ExceptionTime });



                List<UnityExceptionGroupSummaryView> exceptions = new List<UnityExceptionGroupSummaryView>();

                foreach (var row in query)
                {
                    exceptions.Add(new UnityExceptionGroupSummaryView(row.ExceptionType, row.ExceptionMessage, row.FaultiveFunction, row.ExceptionCount, row.StacktraceHash, row.LatestException));
                }

                return exceptions.OrderByDescending(q => q.LatestException).ToList();
            }
        }

        /// <summary>
        /// Deletes all the exceptions with the same stacktrace
        /// </summary>
        /// <param name="stacktraceHash"></param>
        public static void DeleteExceptionGroup(string stacktraceHash)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                string sqlTemplate = "DELETE FROM [CmuneMonitoring].[dbo].[UnityExceptions] WHERE [StacktraceHash] LIKE '{0}'";
                string sql = String.Format(sqlTemplate, TextUtilities.ProtectSqlField(stacktraceHash));

                db.ExecuteCommand(sql);
            }
        }

        public static UnityExceptionGroupDetailedView GetExceptionGroup(string stacktraceHash)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                var groupDetailQuery = (from u in db.UnityExceptions
                                        where u.StacktraceHash == stacktraceHash
                                        orderby u.UnityExceptionId descending
                                        select new { Stacktrace = u.Stacktrace, ExceptionType = u.ExceptionType, ExceptionMessage = u.ExceptionMessage, FaultiveFunction = u.FaultiveFunction }).Take(1);

                UnityExceptionGroupDetailedView exceptionGroup = null;

                if (groupDetailQuery.Count() > 0)
                {
                    var exceptionsQuery = (from u in db.UnityExceptions
                                          where u.StacktraceHash == stacktraceHash
                                          orderby u.UnityExceptionId descending
                                          select new { UnityExceptionId = u.UnityExceptionId, BuildNumber = u.BuildNumber, ExceptionTime = u.ExceptionTime, Cmid = u.Cmid, Channel = u.Channel }).Take(50);

                    List<UnityExceptionBasicView> exceptions = new List<UnityExceptionBasicView>();

                    foreach (var row in exceptionsQuery)
                    {
                        exceptions.Add(new UnityExceptionBasicView(row.BuildNumber, row.ExceptionTime, row.Cmid, (ChannelType)row.Channel, row.UnityExceptionId));
                    }

                    string stacktrace = String.Empty;
                    UnityExceptionGroupSummaryView exceptionGroupSummary = null;

                    foreach (var row in groupDetailQuery)
                    {
                        stacktrace = row.Stacktrace;

                        exceptionGroupSummary = new UnityExceptionGroupSummaryView(row.ExceptionType, row.ExceptionMessage, row.FaultiveFunction, stacktraceHash);
                    }

                    exceptionGroup = new UnityExceptionGroupDetailedView(stacktrace, exceptionGroupSummary, exceptions);
                }

                return exceptionGroup;
            }
        }

        public static UnityExceptionView GetException(int unityExceptionId)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                UnityException exception = db.UnityExceptions.SingleOrDefault(u => u.UnityExceptionId == unityExceptionId);

                UnityExceptionView exceptionView = null;

                if (exception != null)
                {
                    exceptionView = new UnityExceptionView(exception.UnityExceptionId,
                                                            exception.ExceptionType,
                                                            exception.ExceptionMessage,
                                                            exception.Cmid,
                                                            (ChannelType)exception.Channel,
                                                            (BuildType)exception.Build,
                                                            exception.ExceptionTime,
                                                            exception.Stacktrace,
                                                            exception.StacktraceHash,
                                                            exception.FaultiveFunction,
                                                            exception.ExceptionData,
                                                            exception.BuildNumber);
                }

                return exceptionView;
            }
        }

        public static string StandardizeExceptionType(string exceptionType)
        {
            return exceptionType.Trim().ShortenText(50);
        }

        public static string StandardizeExceptionMessage(string exceptionMessage)
        {
            return exceptionMessage.Trim().ShortenText(600);
        }

        public static string StandardizeStacktrace(string stacktrace)
        {
            return stacktrace.Trim();
        }

        public static string StandardizeExceptionData(string exceptionData)
        {
            return exceptionData.Trim();
        }

        public static string StandardizeBuildNumber(string buildNumber)
        {
            return buildNumber.Trim().ShortenText(50);
        }

        public static void DeleteAllExceptions()
        {
            using (CmuneMonitoringDataContext monitoringDb = new CmuneMonitoringDataContext())
            {
                monitoringDb.ExecuteCommand("TRUNCATE TABLE [UnityExceptions]");
            }
        }
    }
}