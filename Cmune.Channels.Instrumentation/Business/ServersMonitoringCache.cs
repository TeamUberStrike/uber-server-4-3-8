using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Cmune.Channels.Instrumentation.Business
{
    public class ServersMonitoringCache
    {
        /// <summary>
        /// Loads the daily CPU Usage
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> LoadCPUUsageOverTime(DateTime fromDate, DateTime toDate, int managedServerId)
        {
            Dictionary<DateTime, int> cpuUsage = new Dictionary<DateTime, int>();

            string cacheName = "ServersMonitoring.CpuUsage." + fromDate.ToString() + "." + toDate.ToString() + "." + managedServerId;

            //if (HttpRuntime.Cache[cacheName] != null)
            //{
            //    dailyCPUUsage = (Dictionary<DateTime, int>)HttpRuntime.Cache[cacheName];
            //}
            //else
            //{
            cpuUsage = ServersMonitoringBusiness.GetCPUUsage(fromDate, toDate, managedServerId);
            //    HttpRuntime.Cache.Add(cacheName, dailyCPUUsage, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            //}

            return cpuUsage;
        }

        public static Dictionary<DateTime, int> LoadRamUsageOverTime(DateTime fromDate, DateTime toDate, int managedServerId)
        {
            Dictionary<DateTime, int> ramUsage = new Dictionary<DateTime, int>();

            string cacheName = "ServersMonitoring.RamUsage." + fromDate.ToString() + "." + toDate.ToString() + "." + managedServerId;
            ramUsage = ServersMonitoringBusiness.GetRamUsage(fromDate, toDate, managedServerId);
            return ramUsage;
        }

        public static Dictionary<DateTime, int> LoadBandwidthUsageOverTime(DateTime fromDate, DateTime toDate, int managedServerId)
        {
            Dictionary<DateTime, int> bandwidthUsage = new Dictionary<DateTime, int>();

            string cacheName = "ServersMonitoring.BandwidthUsage." + fromDate.ToString() + "." + toDate.ToString() + "." + managedServerId;
            bandwidthUsage = ServersMonitoringBusiness.GetBandwidthUsage(fromDate, toDate, managedServerId);
            return bandwidthUsage;
        }

        public static Dictionary<DateTime, int> LoadDiskSpaceUsageOverTime(DateTime fromDate, DateTime toDate, int managedServerId)
        {
            Dictionary<DateTime, int> diskSpaceUsage = new Dictionary<DateTime, int>();

            string cacheName = "ServersMonitoring.DiskSpaceUsage." + fromDate.ToString() + "." + toDate.ToString() + "." + managedServerId;
            diskSpaceUsage = ServersMonitoringBusiness.GetDiskSpaceUsage(fromDate, toDate, managedServerId);
            return diskSpaceUsage;
        }
    }
}