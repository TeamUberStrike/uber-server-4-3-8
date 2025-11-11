using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public class ManagedServerMonitoringModel
    {
        public int ManagedServerMonitoringId { get; set; }

        public int CPUUsage { get; set; }

        public int RamUsage { get; set; }

        public DateTime ReportTime  { get; set; }

        public decimal BandwidthUsage { get; set; }

        public decimal DiskUsage { get; set; }

        public int ManagedServerId { get; set; }
    }
}
