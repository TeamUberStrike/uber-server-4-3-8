using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Common.Entities;

namespace Cmune.Instrumentation.Monitoring.Business
{
    public class PhotonMonitoring
    {
        public static void SetInstanceState(string versionNumber, string instanceName, string ip, int port, int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount)
        {
            versionNumber = StandardizeVersionNumber(versionNumber);
            instanceName = StandardizeInstanceName(instanceName);

            if (!versionNumber.IsNullOrFullyEmpty() && !instanceName.IsNullOrFullyEmpty())
            {
                using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
                {
                    PhotonMonitoringInstance instance = GetInstance(versionNumber, ip, port, db);

                    if (instance == null)
                    {
                        instance = CreateInstance(versionNumber, instanceName, ip, port, db);
                    }
                    else if (instance.InstanceName != instanceName)
                    {
                        instance.InstanceName = instanceName;
                        db.SubmitChanges();
                    }

                    SetMonitoringDetails(cpuUtilization, ramUtilization, instanceRamMb, peerCount, instance.PhotonMonitoringInstanceId, db);
                }
            }
        }

        private static PhotonMonitoringInstance GetInstance(string version, string ip, int port, CmuneMonitoringDataContext db)
        {
            PhotonMonitoringInstance instance = null;

            if (db != null)
            {
                instance = db.PhotonMonitoringInstances.SingleOrDefault(i => i.VersionNumber == version && i.InstanceIp == TextUtilities.InetAToN(ip) && i.InstancePort == port);
            }

            return instance;
        }

        private static PhotonMonitoringInstance CreateInstance(string versionNumber, string instanceName, string ip, int port, CmuneMonitoringDataContext db)
        {
            PhotonMonitoringInstance instance = null;

            if (db != null)
            {
                instance = new PhotonMonitoringInstance();
                
                instance.InstanceIp = TextUtilities.InetAToN(ip);
                instance.InstanceName = instanceName;
                instance.InstancePort = port;
                instance.VersionNumber = versionNumber;

                db.PhotonMonitoringInstances.InsertOnSubmit(instance);
                db.SubmitChanges();
            }

            return instance;
        }

        private static void SetMonitoringDetails(int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount, int instanceId, CmuneMonitoringDataContext db)
        {
            if (db != null)
            {
                DateTime statDate = DateTime.Now.RoundToQuarter();
                if (!IsRecorded(statDate, instanceId, db))
                {
                    PhotonMonitoringDetail monitoringDetail = new PhotonMonitoringDetail();
                    monitoringDetail.CpuUtilization = cpuUtilization;
                    monitoringDetail.InstanceId = instanceId;
                    monitoringDetail.InstanceRamMb = instanceRamMb;
                    monitoringDetail.PeerCount = peerCount;
                    monitoringDetail.RamUtilization = ramUtilization;
                    monitoringDetail.StatDate = statDate;

                    db.PhotonMonitoringDetails.InsertOnSubmit(monitoringDetail);
                    db.SubmitChanges();
                }
            }
        }

        private static bool IsRecorded(DateTime statDate, int instanceId, CmuneMonitoringDataContext db)
        {
            bool isRecorded = true;

            if (db != null)
            {
                PhotonMonitoringDetail monitoringDetail = db.PhotonMonitoringDetails.SingleOrDefault(m => m.StatDate == statDate && m.InstanceId == instanceId);

                if (monitoringDetail == null)
                {
                    isRecorded = false;
                }
            }

            return isRecorded;
        }

        public static List<string> GetVersionNumbers(int versionsCount)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                List<string> versions = new List<string>();

                var query = (from i in db.PhotonMonitoringInstances
                            orderby i.PhotonMonitoringInstanceId descending
                            group i by i.VersionNumber into i2
                             select new { Version = i2.Key }).Take(versionsCount);

                return query.Select(i => i.Version).ToList();
            }
        }

        public static List<int> GetInstancesId(string versionNumber)
        {
            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                var query = from i in db.PhotonMonitoringInstances
                             where i.VersionNumber == versionNumber
                             select new { InstanceId = i.PhotonMonitoringInstanceId };

                return query.Select(i => i.InstanceId).ToList();
            }
        }

        public static Dictionary<DateTime, int> GetConcurrentUsers(string version, DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, int> concurrentUsers = new Dictionary<DateTime, int>();

            List<DateTime> tempDates = GetPhotonIntervalList(fromDate, toDate);

            foreach (DateTime tempDate in tempDates)
            {
                concurrentUsers.Add(tempDate, 0);
            }

            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                var query = from d in db.PhotonMonitoringDetails
                            join i in db.PhotonMonitoringInstances on d.InstanceId equals i.PhotonMonitoringInstanceId
                            where i.VersionNumber == version && d.StatDate >= fromDate && d.StatDate <= toDate
                            group d by d.StatDate into d2
                            select new { StatDate = d2.Key, Ccu = d2.Sum(d => d.PeerCount) };

                Dictionary<DateTime, int> concurrentUsersTmp = query.ToDictionary(u => u.StatDate, u => u.Ccu);

                foreach (DateTime key in concurrentUsersTmp.Keys)
                {
                    concurrentUsers[key] = concurrentUsersTmp[key];
                }
            }

            return concurrentUsers;
        }

        public static Dictionary<DateTime, PhotonMonitoringDetailView> GetPhotonHealth(int instanceId, DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, PhotonMonitoringDetailView> photonHealth = new Dictionary<DateTime, PhotonMonitoringDetailView>();

            List<DateTime> tempDates = GetPhotonIntervalList(fromDate, toDate);

            foreach (DateTime tempDate in tempDates)
            {
                photonHealth.Add(tempDate, new PhotonMonitoringDetailView(tempDate, instanceId));
            }

            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                var monitoringDetails = (from d in db.PhotonMonitoringDetails
                                         join i in db.PhotonMonitoringInstances on d.InstanceId equals i.PhotonMonitoringInstanceId
                                         where d.InstanceId == instanceId && d.StatDate >= fromDate && d.StatDate <= toDate
                                         select new { PhotonMonitoringDetailId = d.PhotonMonitoringDetailId, InstanceId = d.InstanceId, CpuUtilization = d.CpuUtilization, RamUtilization = d.RamUtilization, InstanceRamMb = d.InstanceRamMb, PeerCount = d.PeerCount, StatDate = d.StatDate, InstanceName = i.InstanceName }).ToList();

                foreach (var monitoringDetail in monitoringDetails)
                {
                    photonHealth[monitoringDetail.StatDate] = new PhotonMonitoringDetailView(monitoringDetail.PhotonMonitoringDetailId,
                                                                                                monitoringDetail.CpuUtilization,
                                                                                                monitoringDetail.RamUtilization,
                                                                                                monitoringDetail.InstanceRamMb,
                                                                                                monitoringDetail.PeerCount,
                                                                                                monitoringDetail.StatDate,
                                                                                                monitoringDetail.InstanceId,
                                                                                                monitoringDetail.InstanceName);
                }
            }

            return photonHealth;
        }

        public static Dictionary<int, string> GetInstancesName(string version)
        {
            Dictionary<int, string> instancesName = new Dictionary<int, string>();

            using (CmuneMonitoringDataContext db = new CmuneMonitoringDataContext())
            {
                var query = from i in db.PhotonMonitoringInstances
                            where i.VersionNumber == version
                            select new { InstanceId = i.PhotonMonitoringInstanceId, InstanceName = i.InstanceName, InstancePort = i.InstancePort, InstanceIp = TextUtilities.InetNToA(i.InstanceIp) };

                instancesName = query.ToDictionary(i => i.InstanceId, i => i.InstanceName +  " - " + i.InstanceIp + ":" + i.InstancePort);
            }

            return instancesName;
        }

        public static string StandardizeInstanceName(string instanceName)
        {
            return instanceName.Trim().ShortenText(30);
        }

        public static string StandardizeVersionNumber(string versionNumber)
        {
            return versionNumber.Trim().ShortenText(10);
        }

        public static List<DateTime> GetPhotonIntervalList(DateTime fromDate, DateTime toDate, int interval = 15)
        {
            List<DateTime> result = new List<DateTime>();

            DateTime tempDate = fromDate;

            while (tempDate <= toDate)
            {
                result.Add(tempDate);

                tempDate = tempDate.AddMinutes(interval);
            }

            return result;
        }
    }
}