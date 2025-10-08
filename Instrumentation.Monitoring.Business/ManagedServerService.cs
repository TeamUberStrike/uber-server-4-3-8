using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.Instrumentation.Monitoring.Common.Entities;

namespace Cmune.Instrumentation.Monitoring.Business
{
    /// <summary>
    /// Allows us to monitor our servers
    /// </summary>
    public static class ManagedServerService
    {
        public static ManagedServerModel ToManagedServerModel(this ManagedServer managedServer)
        {
            var managedServerModel = new ManagedServerModel();

            managedServerModel.ManagedServerId2 = managedServer.ManagedServerID;
            managedServerModel.ServerName = managedServer.ServerName;
            managedServerModel.PublicIp = managedServer.PublicIP;
            managedServerModel.PrivateIp = managedServer.PrivateIP;
            managedServerModel.Role = managedServer.Role;
            managedServerModel.DiskSpace = managedServer.DiskSpace;
            managedServerModel.NextPollTime = managedServer.NextPollTime;
            managedServerModel.DeploymentTime = managedServer.DeploymentTime;
            managedServerModel.IsDisable = managedServer.IsDisable;
            managedServerModel.ServerIDC = managedServer.ServerIDC;
            managedServerModel.Region = managedServer.Region;
            managedServerModel.City = managedServer.City;
            managedServerModel.CPUModel = managedServer.CPUModel;
            managedServerModel.CPUSpeed = managedServer.CPUSpeed;
            managedServerModel.CPUCore = managedServer.CPUCore;
            managedServerModel.CPUs = managedServer.CPUs;
            managedServerModel.RAM = managedServer.RAM;
            managedServerModel.AllowedBandwidth = managedServer.AllowedBandwidth;
            managedServerModel.Note = managedServer.Note;
            managedServerModel.Price = managedServer.Price;
            return managedServerModel;
        }

        public static ManagedServer ToManagedServer(this ManagedServerModel managedServerModel)
        {
            var managedServer = new ManagedServer();

            managedServer.ManagedServerID = managedServerModel.ManagedServerId2;
            managedServer.ServerName = managedServerModel.ServerName;
            managedServer.PublicIP = managedServerModel.PublicIp;
            managedServer.PrivateIP = managedServerModel.PrivateIp;
            managedServer.Role = managedServerModel.Role;
            managedServer.DiskSpace = managedServerModel.DiskSpace;
            managedServer.NextPollTime = managedServerModel.NextPollTime;
            managedServer.DeploymentTime = managedServerModel.DeploymentTime;
            managedServer.IsDisable = managedServerModel.IsDisable;
            managedServer.ServerIDC = managedServerModel.ServerIDC;
            managedServer.Region = managedServerModel.Region;
            managedServer.City = managedServerModel.City;
            managedServer.CPUModel = managedServerModel.CPUModel;
            managedServer.CPUSpeed = managedServerModel.CPUSpeed;
            managedServer.CPUCore = managedServerModel.CPUCore;
            managedServer.CPUs = managedServer.CPUs;
            managedServer.RAM = managedServerModel.RAM;
            managedServer.AllowedBandwidth = managedServerModel.AllowedBandwidth;
            managedServer.Note = managedServerModel.Note;
            managedServer.Price = managedServerModel.Price;
            return managedServer;
        }

        public static void CopyFromManagedServerModel(this ManagedServer managedServer, ManagedServerModel managedServerModel)
        {
            managedServer.ServerName = ValidationUtilities.StandardizeManagedServerName(managedServerModel.ServerName);
            managedServer.PublicIP = managedServerModel.PublicIp;
            managedServer.PrivateIP = managedServerModel.PrivateIp;
            managedServer.Role = managedServerModel.Role;
            managedServer.DiskSpace = managedServerModel.DiskSpace;
            managedServer.NextPollTime = managedServerModel.NextPollTime;
            managedServer.DeploymentTime = managedServerModel.DeploymentTime;
            managedServer.IsDisable = managedServerModel.IsDisable;
            managedServer.ServerIDC = managedServerModel.ServerIDC;
            managedServer.City = managedServerModel.City;
            managedServer.Region = managedServerModel.Region;
            managedServer.CPUModel = managedServerModel.CPUModel;
            managedServer.CPUSpeed = managedServerModel.CPUSpeed;
            managedServer.CPUCore = managedServerModel.CPUCore;
            managedServer.CPUs = managedServerModel.CPUs;
            managedServer.RAM = managedServerModel.RAM;
            managedServer.AllowedBandwidth = managedServerModel.AllowedBandwidth;
            managedServer.Note = managedServerModel.Note;
            managedServer.Price = managedServerModel.Price;
        }

        public static IQueryable<ManagedServerModel> ToManagedServerModelQueryable(this IQueryable<ManagedServer> managedServerList)
        {
            var managedserverModelList = from managedServerModel in managedServerList select managedServerModel.ToManagedServerModel();
            return managedserverModelList;
        }

        /// <summary>
        /// Get a specific managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static ManagedServerModel GetManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);
                if(managedServer != null)
                    return managedServer.ToManagedServerModel();
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <returns></returns>
        public static bool DeleteManagedServer(int managedServerId)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                var managedServer = monitoringDB.ManagedServers.SingleOrDefault(d => d.ManagedServerID == managedServerId);
                if (managedServer != null)
                {
                    monitoringDB.ManagedServers.DeleteOnSubmit(managedServer);

                    ManagedServerMonitoringService.DeleteManagedServerMonitoring(managedServerId);

                    monitoringDB.SubmitChanges();

                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// get a dictionnary of managed Servers' Ip
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServerIpModel> GetManagedServersIPs()
        {
            var managedServers = GetManagedServers();
            var managedServersInfos = new List<ManagedServerIpModel>();
            managedServers.ForEach(d=> managedServersInfos.Add(new ManagedServerIpModel(){ Id = d.ManagedServerId2, Ip =  d.PublicIp}));

            return managedServersInfos;
        }

        /// <summary>
        /// Get all the servers managed by this system
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServerModel> GetManagedServers()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServerModel> managedServerModelList = monitoringDB.ManagedServers.ToManagedServerModelQueryable().ToList().OrderBy(d=>d.ServerIDC).ThenBy(d=>d.ServerName).ToList();

                return managedServerModelList;
            }
        }

        /// <summary>
        /// Get the servers that are enabled
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServer> GetMonitoredServers()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServer> managedServers = monitoringDB.ManagedServers.Where(mS => mS.IsDisable == false).ToList();

                return managedServers;
            }
        }

        /// <summary>
        /// Get the servers to test now
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServer> GetManagedServersToTest()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServer> managedServersToTest = monitoringDB.ManagedServers.Where(mS => mS.NextPollTime < DateTime.Now && mS.IsDisable == false).ToList();

                return managedServersToTest;
            }
        }

        /// <summary>
        /// Disable a managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        public static void DisableManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);

                if (managedServer != null)
                {
                    if (!managedServer.IsDisable)
                    {
                        managedServer.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        public static void EnableManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);

                if (managedServer != null)
                {
                    if (managedServer.IsDisable)
                    {
                        managedServer.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the name is duplicated
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static bool IsManagedServerNameDuplicated(string serverName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerName(serverName))
                {
                    serverName = ValidationUtilities.StandardizeManagedServerName(serverName);

                    int duplicatedNames = monitoringDB.ManagedServers.Count(mS => mS.ServerName == serverName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether the name is duplicated except for a specific server
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static bool IsManagedServerNameDuplicated(string serverName, int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerName(serverName))
                {
                    serverName = ValidationUtilities.StandardizeManagedServerName(serverName);

                    int duplicatedNames = monitoringDB.ManagedServers.Count(mS => mS.ServerName == serverName && mS.ManagedServerID != managedServerID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Creates a new managed server
        /// </summary>
        /// <param name="managedServerModel"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateManagedServer(ManagedServerModel managedServerModel)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerName(managedServerModel.ServerName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidIPAddress(managedServerModel.PublicIp))
                {
                    ret = MonitoringOperationResult.InvalidIP;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsManagedServerNameDuplicated(managedServerModel.ServerName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    managedServerModel.NextPollTime = DateTime.Now;
                    managedServerModel.PublicIp = managedServerModel.PublicIp.Trim();
                    managedServerModel.ServerName = ValidationUtilities.StandardizeManagedServerName(managedServerModel.ServerName);

                    monitoringDB.ManagedServers.InsertOnSubmit(managedServerModel.ToManagedServer());
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a managed server
        /// </summary>
        /// <param name="managedServerModel"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditManagedServer(ManagedServerModel managedServerModel)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidIPAddress(managedServerModel.PublicIp))
                {
                    ret = MonitoringOperationResult.InvalidIP;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsManagedServerNameDuplicated(managedServerModel.ServerName, managedServerModel.ManagedServerId2))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerModel.ManagedServerId2);

                    if (managedServer != null)
                    {
                        managedServer.CopyFromManagedServerModel(managedServerModel);
                        managedServer.PublicIP = managedServer.PublicIP.Trim();
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.ServerNotFound;
                    }
                }

                return ret;
            }
        }
    }
}