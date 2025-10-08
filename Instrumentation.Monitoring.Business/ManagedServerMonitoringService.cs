using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.Instrumentation.Monitoring.DataAccess;

namespace Cmune.Instrumentation.Monitoring.Business
{
    public static class ManagedServerMonitoringService
    {
        public static ManagedServerMonitoringModel ToManagedServerMonitoringModel(this ManagedServerMonitoring managedServerMonitoring)
        {
            var managedServerMonitoringModel = new ManagedServerMonitoringModel();

            managedServerMonitoringModel.ManagedServerMonitoringId = managedServerMonitoring.ManagedServerMonitoringId;
            managedServerMonitoringModel.CPUUsage = managedServerMonitoring.CPUUsage;
            managedServerMonitoringModel.RamUsage = managedServerMonitoring.RamUsage;
            managedServerMonitoringModel.ReportTime = managedServerMonitoring.ReportTime;
            managedServerMonitoringModel.BandwidthUsage = managedServerMonitoring.BandwidthUsage;
            managedServerMonitoringModel.DiskUsage = managedServerMonitoring.DiskUsage;
            managedServerMonitoringModel.ManagedServerId = managedServerMonitoring.ManagedServerId;

            return managedServerMonitoringModel;
        }

        public static ManagedServerMonitoring ToManagedServerMonitoring(this ManagedServerMonitoringModel managedServerMonitoringModel)
        {
            var managedServerMonitoring = new ManagedServerMonitoring();

            managedServerMonitoring.ManagedServerMonitoringId = managedServerMonitoringModel.ManagedServerMonitoringId;
            managedServerMonitoring.CPUUsage = managedServerMonitoringModel.CPUUsage;
            managedServerMonitoring.RamUsage = managedServerMonitoringModel.RamUsage;
            managedServerMonitoring.ReportTime = managedServerMonitoringModel.ReportTime;
            managedServerMonitoring.BandwidthUsage = managedServerMonitoringModel.BandwidthUsage;
            managedServerMonitoring.DiskUsage = managedServerMonitoringModel.DiskUsage;
            managedServerMonitoring.ManagedServerId = managedServerMonitoringModel.ManagedServerId;
            return managedServerMonitoring;
        }

        public static void CopyFromManagedServerModel(this ManagedServerMonitoring managedServerMonitoring, ManagedServerMonitoringModel managedServerMonitoringModel)
        {
            managedServerMonitoring.CPUUsage = managedServerMonitoringModel.CPUUsage;
            managedServerMonitoring.RamUsage = managedServerMonitoringModel.RamUsage;
            managedServerMonitoring.ReportTime = managedServerMonitoringModel.ReportTime;
            managedServerMonitoring.BandwidthUsage = managedServerMonitoringModel.BandwidthUsage;
            managedServerMonitoring.DiskUsage = managedServerMonitoringModel.DiskUsage;
            managedServerMonitoring.ManagedServerId = managedServerMonitoringModel.ManagedServerId;
        }

        public static IQueryable<ManagedServerMonitoringModel> ToManagedServerMonitoringModelQueryable(this IQueryable<ManagedServerMonitoring> managedServerMonitoringList)
        {
            var managedserverModelList = from managedServerMonitoring in managedServerMonitoringList select managedServerMonitoring.ToManagedServerMonitoringModel();
            return managedserverModelList;
        }

        /// <summary>
        /// Get a specific managed server monitoring
        /// </summary>
        /// <param name="managedServerMonitoringId"></param>
        /// <returns></returns>
        public static ManagedServerMonitoringModel GetManagedServerMonitoring(int managedServerMonitoringId)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServerMonitoring managedServerMonitoring = monitoringDB.ManagedServerMonitorings.SingleOrDefault(mS => mS.ManagedServerMonitoringId == managedServerMonitoringId);
                if (managedServerMonitoring != null)
                    return managedServerMonitoring.ToManagedServerMonitoringModel();
                return null;
            }
        }

        /// <summary>
        /// Creates a new managed server monitoring
        /// </summary>
        /// <param name="managedServerMonitoringModel"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateManagedServerMonitoring(ManagedServerMonitoringModel managedServerMonitoringModel)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                DateTime startDate = DateTime.Now.RoundToMinutesDivisiblesByFive();

                if (!IsRecorded(startDate, managedServerMonitoringModel.ManagedServerId, monitoringDB))
                {
                    managedServerMonitoringModel.ReportTime = startDate;
                    monitoringDB.ManagedServerMonitorings.InsertOnSubmit(managedServerMonitoringModel.ToManagedServerMonitoring());
                    monitoringDB.SubmitChanges();
                    return MonitoringOperationResult.Ok;
                }
                return MonitoringOperationResult.DuplicateReport;
            }
        }

        private static bool IsRecorded(DateTime reportDate, int managedServerId, CmuneMonitoringDataContext db)
        {
            bool isRecorded = true;

            if (db != null)
            {
                ManagedServerMonitoring managedServerMonitoring = db.ManagedServerMonitorings.SingleOrDefault(m => m.ReportTime == reportDate && m.ManagedServerId == managedServerId);

                if (managedServerMonitoring == null)
                {
                    isRecorded = false;
                }
            }

            return isRecorded;
        }

        /// <summary>
        /// Creates a new managed server monitoring
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <param name="cpuUsage"></param>
        /// <param name="ramUsage"></param>
        /// <param name="bandwidthUsage"></param>
        /// <param name="diskUsage"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateManagedServerMonitoring(int managedServerId, int cpuUsage, int ramUsage, decimal bandwidthUsage, decimal diskUsage)
        {
            var managedServerMonitoringModel = new ManagedServerMonitoringModel();

            managedServerMonitoringModel.ManagedServerId = managedServerId;
            managedServerMonitoringModel.CPUUsage = cpuUsage;
            managedServerMonitoringModel.RamUsage = ramUsage;
            managedServerMonitoringModel.BandwidthUsage = bandwidthUsage;
            managedServerMonitoringModel.DiskUsage = diskUsage;

            var result = CreateManagedServerMonitoring(managedServerMonitoringModel);
            return result;
        }

        public static bool DeleteManagedServerMonitoring(int managedServerId)
        {
            bool isSuccessful = false;

            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                var listOfManaedServerMonitoring = monitoringDB.ManagedServerMonitorings.Where(d=>d.ManagedServerId == managedServerId);

                monitoringDB.ManagedServerMonitorings.DeleteAllOnSubmit(listOfManaedServerMonitoring);
                monitoringDB.SubmitChanges();
                isSuccessful = true;
            }
            return isSuccessful;
        }
    }
}