using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrumentation.Monitoring.ServerMonitor.ProxyModel
{
    public class ManagedServerMonitoringProxyModel
    {
        public int ManagedServerId { get; set; }
        public int CPUUsage { get; set; }
        public int RamUsage { get; set; }
        public DateTime ReportTime { get; set; }
        public int BandwidthUsage { get; set; }
        public int DiskUsage { get; set; }
    }
}
