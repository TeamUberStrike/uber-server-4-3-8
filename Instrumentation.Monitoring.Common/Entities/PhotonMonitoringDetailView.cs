using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.Instrumentation.Monitoring.Common.Entities
{
    public class PhotonMonitoringDetailView
    {
        #region Properties

        public int PhotonMonitoringDetailId { get; private set; }
        public int CpuUtilization { get; private set; }
        public int RamUtilization { get; private set; }
        public int InstanceRamMb { get; private set; }
        public int PeerCount { get; private set; }
        public DateTime StatDate { get; private set; }
        public int InstanceId { get; private set; }
        public string InstanceName { get; private set; }

        #endregion

        #region Constructors

        public PhotonMonitoringDetailView(DateTime statDate, int instanceId)
        {
            StatDate = statDate;
            InstanceId = instanceId;
        }

        public PhotonMonitoringDetailView(int photonMonitoringDetailId, int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount, DateTime statDate, int instanceId)
            : this (statDate, instanceId)
        {
            PhotonMonitoringDetailId = photonMonitoringDetailId;
            CpuUtilization = cpuUtilization;
            RamUtilization = ramUtilization;
            InstanceRamMb = instanceRamMb;
            PeerCount = peerCount;
        }

        public PhotonMonitoringDetailView(int photonMonitoringDetailId, int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount, DateTime statDate, int instanceId, string instanceName)
            : this (photonMonitoringDetailId, cpuUtilization, ramUtilization, instanceRamMb, peerCount, statDate, instanceId)
        {
            InstanceName = instanceName;
        }

        #endregion
    }
}