using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.Instrumentation.DataAccess;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.Channels.Instrumentation.Extensions;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Business;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class ServersMonitoringBusiness
    {
        public static Dictionary<DateTime, int> InitPhotonInterval(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> interval = new Dictionary<DateTime, int>();

            var internvalList = PhotonMonitoring.GetPhotonIntervalList(fromDate, toDate, 5);
            foreach (var row in internvalList)
            {
                interval.Add(row, 0);
            }
            return interval;
        }

        public static Dictionary<DateTime, decimal> InitPhotonDecimalInterval(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, decimal> interval = new Dictionary<DateTime, decimal>();

            var internvalList = PhotonMonitoring.GetPhotonIntervalList(fromDate, toDate, 5);
            foreach (var row in internvalList)
            {
                interval.Add(row, 0);
            }
            return interval;
        }

        public static Dictionary<DateTime, int> GetCPUUsage(DateTime fromDate, DateTime toDate, int managedServerId)
        {

            Dictionary<DateTime, int> cpuUsage = InitPhotonInterval(fromDate, toDate);

            var db = ContextHelper<CmuneMonitoringDataContext>.GetCurrentContext();
            {
                var query = from t in db.ManagedServerMonitorings
                            where t.ReportTime >= fromDate
                                && t.ReportTime <= toDate && t.ManagedServerId == managedServerId
                            select new { t.ReportTime, t.CPUUsage };
                foreach (var row in query)
                {
                    if (cpuUsage.Keys.Contains(row.ReportTime.RoundToMinutesDivisiblesByFive()))
                        cpuUsage[row.ReportTime.RoundToMinutesDivisiblesByFive()] = row.CPUUsage;
                }

            }

            return cpuUsage;
        }

        public static Dictionary<DateTime, int> GetRamUsage(DateTime fromDate, DateTime toDate, int managedServerId)
        {

            Dictionary<DateTime, int> ramUsage = InitPhotonInterval(fromDate, toDate);

            var db = ContextHelper<CmuneMonitoringDataContext>.GetCurrentContext();
            {
                var query = from t in db.ManagedServerMonitorings
                            where t.ReportTime >= fromDate
                                && t.ReportTime <= toDate && t.ManagedServerId == managedServerId
                            select new { t.ReportTime, t.RamUsage };
                foreach (var row in query)
                {
                    if (ramUsage.Keys.Contains(row.ReportTime.RoundToMinutesDivisiblesByFive()))
                        ramUsage[row.ReportTime.RoundToMinutesDivisiblesByFive()] = row.RamUsage;
                }

            }

            return ramUsage;
        }

        public static Dictionary<DateTime, int> GetBandwidthUsage(DateTime fromDate, DateTime toDate, int managedServerId)
        {

            Dictionary<DateTime, int> bandwidthUsage = InitPhotonInterval(fromDate, toDate);

            var db = ContextHelper<CmuneMonitoringDataContext>.GetCurrentContext();
            {
                var query = from t in db.ManagedServerMonitorings
                            where t.ReportTime >= fromDate
                                && t.ReportTime <= toDate && t.ManagedServerId == managedServerId
                            select new { t.ReportTime, t.BandwidthUsage };
                foreach (var row in query)
                {
                    if (bandwidthUsage.Keys.Contains(row.ReportTime.RoundToMinutesDivisiblesByFive()))
                        bandwidthUsage[row.ReportTime.RoundToMinutesDivisiblesByFive()] = (int) Math.Round(row.BandwidthUsage);
                }

            }

            return bandwidthUsage;
        }

        public static Dictionary<DateTime, int> GetDiskSpaceUsage(DateTime fromDate, DateTime toDate, int managedServerId)
        {

            Dictionary<DateTime, int> diskSpaceUsage = InitPhotonInterval(fromDate, toDate);

            var db = ContextHelper<CmuneMonitoringDataContext>.GetCurrentContext();
            {
                var query = from t in db.ManagedServerMonitorings
                            where t.ReportTime >= fromDate
                                && t.ReportTime <= toDate && t.ManagedServerId == managedServerId
                            select new { t.ReportTime, t.DiskUsage };
                foreach (var row in query)
                {
                    if (diskSpaceUsage.Keys.Contains(row.ReportTime.RoundToMinutesDivisiblesByFive()))
                        diskSpaceUsage[row.ReportTime.RoundToMinutesDivisiblesByFive()] = (int) Math.Round(row.DiskUsage);
                }

            }

            return diskSpaceUsage;
        }


    }
}