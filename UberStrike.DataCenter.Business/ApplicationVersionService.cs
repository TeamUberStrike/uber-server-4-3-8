using System.Collections.Generic;
using System.Linq;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using System;

namespace UberStrike.DataCenter.Business
{
    public static class ApplicationVersionService
    {
        public static ApplicationVersionViewModel ToApplicationVersionModel(this ApplicationVersion applicationVersion)
        {
            var applicationVersionModel = new ApplicationVersionViewModel();

            applicationVersionModel.ApplicationVersionId = applicationVersion.ApplicationVersionId;
            applicationVersionModel.Version = applicationVersion.Version;
            applicationVersionModel.WebPlayerFileName = applicationVersion.WebPlayerFileName;
            applicationVersionModel.Channel = (ChannelType)applicationVersion.Channel;
            applicationVersionModel.ModificationDate = applicationVersion.ModificationDate;
            applicationVersionModel.IsEnabled = applicationVersion.IsEnabled;
            applicationVersionModel.WarnPlayer = applicationVersion.WarnPlayer;
            applicationVersionModel.PhotonClusterId = applicationVersion.PhotonClusterId;

            return applicationVersionModel;
        }

        public static ApplicationVersion ToApplicationVersion(this ApplicationVersionViewModel applicationVersionModel)
        {
            var applicationVersion = new ApplicationVersion();

            applicationVersion.ApplicationVersionId = applicationVersionModel.ApplicationVersionId;
            applicationVersion.Version = applicationVersionModel.Version;
            applicationVersion.WebPlayerFileName = applicationVersionModel.WebPlayerFileName;
            applicationVersion.Channel = (int)applicationVersionModel.Channel;
            applicationVersion.ModificationDate = applicationVersionModel.ModificationDate;
            applicationVersion.IsEnabled = applicationVersionModel.IsEnabled;
            applicationVersion.WarnPlayer = applicationVersionModel.WarnPlayer;
            applicationVersion.PhotonClusterId = applicationVersionModel.PhotonClusterId;

            return applicationVersion;
        }

        public static void CopyFromApplicationVersionModel(this ApplicationVersion applicationVersion, ApplicationVersionViewModel applicationVersionModel)
        {
            applicationVersion.Version = applicationVersionModel.Version;
            applicationVersion.WebPlayerFileName = applicationVersionModel.WebPlayerFileName;
            applicationVersion.Channel = (int)applicationVersionModel.Channel;
            applicationVersion.ModificationDate = applicationVersionModel.ModificationDate;
            applicationVersion.IsEnabled = applicationVersionModel.IsEnabled;
            applicationVersion.WarnPlayer = applicationVersionModel.WarnPlayer;
            applicationVersion.PhotonClusterId = applicationVersionModel.PhotonClusterId;
        }

        public static IQueryable<ApplicationVersionViewModel> ToQueryableApplicationVersionModel(this IQueryable<ApplicationVersion> applicationVersionList)
        {
            var applicationVersionModelList = from applicationVersion in applicationVersionList select applicationVersion.ToApplicationVersionModel();
            return applicationVersionModelList;
        }

        public static ApplicationView ToApplicationView(ApplicationVersion applicationVersion)
        {
            ApplicationView applicationView = null;

            if (applicationVersion != null)
            {
                applicationView = new ApplicationView(applicationVersion.ApplicationVersionId,
                                                        applicationVersion.Version,
                                                        BuildType.Prod,
                                                        (ChannelType)applicationVersion.Channel,
                                                        applicationVersion.WebPlayerFileName,
                                                        applicationVersion.ModificationDate,
                                                        applicationVersion.ModificationDate,
                                                        applicationVersion.IsEnabled,
                                                        string.Empty,
                                                        applicationVersion.PhotonClusterId,
                                                        new List<PhotonView>());
            }

            return applicationView;
        }

        /// <summary>
        /// Adds the Photon servers to the ApplicationVersion to make an ApplicationView
        /// </summary>
        /// <param name="applicationVersionVersion"></param>
        /// <returns></returns>
        public static AuthenticateApplicationView GetAuthenticationApplicationView(ApplicationVersionViewModel applicationVersionVersion)
        {
            AuthenticateApplicationView authenticationApplicationView = new AuthenticateApplicationView { GameServers = new List<PhotonView>() }; ;

            if (applicationVersionVersion != null)
            {
                List<PhotonServer> photonServers = Games.GetPhotonServerList(applicationVersionVersion.PhotonClusterId);
                List<PhotonView> photonServersViews = new List<PhotonView>(photonServers.Count);

                foreach (PhotonServer p in photonServers)
                {
                    PhotonView currentServer = new PhotonView()
                    {
                        PhotonId = p.PhotonServerID,
                        IP = p.IP,
                        Name = p.Name,
                        Region = (RegionType)p.Region,
                        Port = p.Port,
                        UsageType = (PhotonUsageType)p.UsageType,
                        MinLatency = p.MinLatency,
                    };
                    photonServersViews.Add(currentServer);
                }

                foreach (var s in photonServersViews)
                {
                    if (s.UsageType == PhotonUsageType.CommServer)
                        authenticationApplicationView.CommServer = s;
                    else if (s.UsageType != PhotonUsageType.None)
                        authenticationApplicationView.GameServers.Add(s);
                }

                authenticationApplicationView.IsEnabled = applicationVersionVersion.IsEnabled;
                authenticationApplicationView.WarnPlayer = applicationVersionVersion.WarnPlayer;
            }

            return authenticationApplicationView;
        }

        /// <summary>
        /// get an application version
        /// </summary>
        /// <param name="applicationVersionId"></param>
        /// <returns></returns>
        public static ApplicationVersionViewModel GetApplicationVersion(int applicationVersionId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var appVersion = GetApplicationVersion(applicationVersionId, uberStrikeDb);
                if (appVersion == null)
                    return null;
                return appVersion.ToApplicationVersionModel();
            }
        }

        private static ApplicationVersion GetApplicationVersion(int applicationId, UberstrikeDataContext uberStrikeDb)
        {
            ApplicationVersion version = null;

            if (uberStrikeDb != null)
            {
                version = uberStrikeDb.ApplicationVersions.FirstOrDefault(d => d.ApplicationVersionId == applicationId);
            }

            return version;
        }

        /// <summary>
        /// Get the application version by an id
        /// </summary>
        /// <param name="version"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static ApplicationVersionViewModel GetApplicationVersion(string version, ChannelType channel)
        {
            ApplicationVersionViewModel applicationVersionModel = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                if (uberStrikeDb != null)
                {
                    var applicationVersion = uberStrikeDb.ApplicationVersions.FirstOrDefault(aV => aV.Version == version && aV.Channel == (int)channel && aV.IsEnabled);
                    if (applicationVersion != null)
                    {
                        applicationVersionModel = applicationVersion.ToApplicationVersionModel();
                    }
                }
            }
            return applicationVersionModel;
        }

        /// <summary>
        /// Get all the application versions of a channel
        /// </summary>
        /// <param name="photonClusterId"></param>
        /// <returns></returns>
        public static List<ApplicationVersionViewModel> GetApplicationVersionsByPhotonClusterId(int photonClusterId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var appVersions = uberStrikeDb.ApplicationVersions.Where(d => d.PhotonClusterId == photonClusterId && d.IsEnabled == true).ToQueryableApplicationVersionModel().ToList();
                if (appVersions == null)
                    return null;
                return appVersions;
            }
        }

        /// <summary>
        /// Gets all the current versions
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationVersionViewModel> GetCurrentApplicationVersions()
        {
            List<ApplicationVersionViewModel> currentApplicationVersions = new List<ApplicationVersionViewModel>();
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                if (uberStrikeDb != null)
                {
                    currentApplicationVersions = uberStrikeDb.ApplicationVersions.OrderByDescending(d => d.Channel).ToQueryableApplicationVersionModel().ToList();
                    foreach (var currentApplicationV in currentApplicationVersions)
                    {
                        var photonGroup = Games.GetPhotonGroup(currentApplicationV.PhotonClusterId);
                        if (photonGroup != null)
                        {
                            currentApplicationV.PhotonClusterName = photonGroup.Name;
                        }
                    }
                }
                return currentApplicationVersions;
            }
        }

        /// <summary>
        /// Add an application version
        /// </summary>
        /// <param name="addApplicationVersionModel"></param>
        public static bool AddApplicationVersion(ApplicationVersionViewModel addApplicationVersionModel)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                if (addApplicationVersionModel.ApplicationVersionId == 0)
                {
                    uberStrikeDb.ApplicationVersions.InsertOnSubmit(addApplicationVersionModel.ToApplicationVersion());
                    uberStrikeDb.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Updates the unity client file url for a given channel
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="fileName"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool UpdateUnityClientUrl(string versionName, string fileName, ChannelType channel)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var versions = uberStrikeDb.ApplicationVersions.Where(v => channel == (ChannelType)v.Channel && v.Version == versionName).ToList();

                if (versions.Count > 0)
                {
                    foreach (ApplicationVersion version in versions)
                    {
                        version.WebPlayerFileName = fileName;
                        version.ModificationDate = DateTime.Now;
                    }

                    uberStrikeDb.SubmitChanges();

                    return true;
                }
                else
                {
                    var group = uberStrikeDb.PhotonsGroups.OrderByDescending(g => g.ModificationDate).First();

                    return AddApplicationVersion(new ApplicationVersionViewModel()
                    {
                        Version = versionName,
                        Channel = channel,
                        WebPlayerFileName = fileName,
                        ModificationDate = DateTime.Now,
                        PhotonClusterId = group.PhotonsGroupID,
                        PhotonClusterName = group.Name,
                    });
                }
            }
        }

        /// <summary>
        /// Edit an application version
        /// </summary>
        /// <param name="applicationVersionModel"></param>
        public static bool EditApplicationVersion(ApplicationVersionViewModel applicationVersionModel)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var applicationVersionTmp = uberStrikeDb.ApplicationVersions.FirstOrDefault(d => d.ApplicationVersionId == applicationVersionModel.ApplicationVersionId);
                if (applicationVersionTmp != null)
                {
                    applicationVersionTmp.CopyFromApplicationVersionModel(applicationVersionModel);
                    uberStrikeDb.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Delete an application version
        /// </summary>
        /// <param name="applicationVersionId"></param>
        public static bool DeleteApplicationVersion(int applicationVersionId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var applicationVersionTmp = uberStrikeDb.ApplicationVersions.FirstOrDefault(d => d.ApplicationVersionId == applicationVersionId);
                if (applicationVersionTmp != null)
                {
                    uberStrikeDb.ApplicationVersions.DeleteOnSubmit(applicationVersionTmp);
                    uberStrikeDb.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Disable all application version by channelType
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="applicationVersionId"></param>
        /// <returns></returns>
        public static bool DisableAllApplicationVersionsByChannelType(ChannelType channelType, int applicationVersionId = 0)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isUpdated = false;

                List<ApplicationVersion> applicationVersions = new List<ApplicationVersion>();

                if (applicationVersionId == 0)
                {
                    applicationVersions = uberStrikeDb.ApplicationVersions.Where(d => d.Channel == (int)channelType).ToList();
                }
                else
                {
                    applicationVersions = uberStrikeDb.ApplicationVersions.Where(d => d.Channel == (int)channelType && d.ApplicationVersionId != applicationVersionId).ToList();
                }

                if (applicationVersions != null)
                {
                    foreach (var applicationVersion in applicationVersions)
                    {
                        applicationVersion.IsEnabled = false;
                    }

                    uberStrikeDb.SubmitChanges();
                    isUpdated = true;
                }

                return isUpdated;
            }
        }

        public static bool UpdateApplicationVersionIsEnabled(int applicationVersionId, bool isEnabled)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isUpdated = false;

                ApplicationVersion version = GetApplicationVersion(applicationVersionId, uberStrikeDb);

                if (version != null)
                {
                    ChannelType channel = (ChannelType)version.Channel;

                    if (CommonConfig.WebChannels.Contains(channel) && isEnabled)
                    {
                        ApplicationVersionService.DisableAllApplicationVersionsByChannelType(channel, applicationVersionId);
                    }

                    version.IsEnabled = isEnabled;
                    uberStrikeDb.SubmitChanges();

                    isUpdated = true;
                }

                return isUpdated;
            }
        }

        public static bool UpdateApplicationVersionWarnPlayer(int applicationVersionId, bool warnPlayer)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isUpdated = false;

                ApplicationVersion version = GetApplicationVersion(applicationVersionId, uberStrikeDb);

                if (version != null)
                {
                    version.WarnPlayer = warnPlayer;
                    uberStrikeDb.SubmitChanges();

                    isUpdated = true;
                }

                return isUpdated;
            }
        }
    }
}

