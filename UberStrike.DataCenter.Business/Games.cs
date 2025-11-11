using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Utils;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business
{
    public static class Games
    {

        /// <summary>
        /// Gets all the Photons groups
        /// </summary>
        /// <returns></returns>
        public static List<PhotonsGroup> GetPhotonGroups()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                List<PhotonsGroup> photonsGroups = paradiseDB.PhotonsGroups.ToList();

                return photonsGroups;
            }
        }

        /// <summary>
        /// Gets all the Photons groups ordered by modification date
        /// </summary>
        /// <returns></returns>
        public static List<PhotonsGroup> GetPhotonGroupsOrderedByModificationDate()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                List<PhotonsGroup> photonsGroups = paradiseDB.PhotonsGroups.OrderByDescending(pG => pG.ModificationDate).ToList();

                return photonsGroups;
            }
        }

        /// <summary>
        /// Gets a specific Photons group
        /// READ-ONLY
        /// </summary>
        /// <param name="applicationVersion"></param>
        /// <returns></returns>
        public static List<PhotonView> GetPhotonServers(string applicationVersion)
        {
            List<PhotonView> photons = new List<PhotonView>();

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var app = uberStrikeDb.ApplicationVersions.SingleOrDefault(aV => aV.Version == applicationVersion && aV.Channel == (int)ChannelType.WebPortal);

                if (app != null)
                {
                    photons.AddRange(app.PhotonsGroup.PhotonServers.ToList().ConvertAll(new Converter<PhotonServer, PhotonView>(p => new PhotonView()
                    {
                        PhotonId = p.PhotonServerID,
                        IP = p.IP,
                        Name = p.Name,
                        Region = (RegionType)p.Region,
                        Port = p.Port,
                        UsageType = (PhotonUsageType)p.UsageType,
                        MinLatency = p.MinLatency,
                    })));
                }
            }

            return photons;
        }

        /// <summary>
        /// Gets a specific Photons group
        /// READ-ONLY
        /// </summary>
        /// <param name="applicationVersion"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string GetPhotonServerName(string applicationVersion, string ip, int port)
        {
            string name = "No Name";

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var app = uberStrikeDb.ApplicationVersions.SingleOrDefault(aV => aV.Version == applicationVersion && aV.Channel == (int)ChannelType.WebPortal);

                if (app != null)
                {
                    var server = app.PhotonsGroup.PhotonServers.FirstOrDefault(p => p.IP == ip && p.Port == port);
                    if (server != null)
                    {
                        name = server.Name;
                    }
                }
            }

            return name;
        }

        /// <summary>
        /// Gets a specific Photons group
        /// READ-ONLY
        /// </summary>
        /// <param name="photonGroupId"></param>
        /// <returns></returns>
        public static PhotonsGroup GetPhotonGroup(int photonGroupId)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetPhotonGroup(photonGroupId, paradiseDB);
            }
        }

        /// <summary>
        /// Gets a specific Photons group
        /// </summary>
        /// <param name="photonGroupId"></param>
        /// <param name="uberStrikeDb"></param>
        /// <returns></returns>
        public static PhotonsGroup GetPhotonGroup(int photonGroupId, UberstrikeDataContext uberStrikeDb)
        {
            PhotonsGroup photonsGroup = null;

            if (uberStrikeDb != null)
            {
                photonsGroup = uberStrikeDb.PhotonsGroups.SingleOrDefault(pG => pG.PhotonsGroupID == photonGroupId);
            }

            return photonsGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photonsClusterId"></param>
        /// <returns></returns>
        public static PhotonsClusterView GetPhotonClusterView(int photonsClusterId)
        {
            PhotonsClusterView photonsCluster = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                PhotonsGroup photonsGroup = uberStrikeDb.PhotonsGroups.SingleOrDefault(pG => pG.PhotonsGroupID == photonsClusterId);

                if (photonsGroup != null)
                {
                    List<PhotonView> photons = photonsGroup.PhotonServers.ToList().ConvertAll(new Converter<PhotonServer, PhotonView>(p => new PhotonView()
                    {
                        PhotonId = p.PhotonServerID,
                        IP = p.IP,
                        Name = p.Name,
                        Region = (RegionType)p.Region,
                        Port = p.Port,
                        UsageType = (PhotonUsageType)p.UsageType,
                        MinLatency = p.MinLatency,
                    }));

                    photonsCluster = new PhotonsClusterView(photonsGroup.PhotonsGroupID, photonsGroup.Name, photonsGroup.Description, photons);
                }
            }

            return photonsCluster;
        }

        /// <summary>
        /// Gets all the Photons servers of a group
        /// </summary>
        /// <param name="photonsGroupID"></param>
        /// <returns></returns>
        public static List<PhotonServer> GetPhotonServerList(int photonsGroupID)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<PhotonServer> photonsGroupServers = uberStrikeDb.PhotonServers.Where(pS => pS.PhotonsGroupID == photonsGroupID).ToList();

                return photonsGroupServers;
            }
        }

        /// <summary>
        /// Checks whether a photons group name is duplicated
        /// </summary>
        /// <param name="photonsGroupName"></param>
        /// <returns></returns>
        public static bool IsPhotonsGroupNameDuplicated(string photonsGroupName)
        {
            bool isPhotonGroupNameDuplicated = false;

            if (!photonsGroupName.IsNullOrFullyEmpty())
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    PhotonsGroup photonsGroup = paradiseDB.PhotonsGroups.SingleOrDefault(pG => pG.Name == ValidationUtilities.StandardizePhotonsGroupName(photonsGroupName));

                    if (photonsGroup != null)
                    {
                        isPhotonGroupNameDuplicated = true;
                    }
                }
            }

            return isPhotonGroupNameDuplicated;
        }

        /// <summary>
        /// Checks whether a photons group name is duplicated
        /// </summary>
        /// <param name="photonsGroupName"></param>
        /// <param name="photonsGroupID"></param>
        /// <returns></returns>
        public static bool IsPhotonsGroupNameDuplicated(string photonsGroupName, int photonsGroupID)
        {
            bool isPhotonGroupNameDuplicated = false;

            if (!photonsGroupName.IsNullOrFullyEmpty() && photonsGroupID > 0)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    PhotonsGroup photonsGroup = paradiseDB.PhotonsGroups.SingleOrDefault(pG => pG.Name == ValidationUtilities.StandardizePhotonsGroupName(photonsGroupName) && pG.PhotonsGroupID != photonsGroupID);

                    if (photonsGroup != null)
                    {
                        isPhotonGroupNameDuplicated = true;
                    }
                }
            }

            return isPhotonGroupNameDuplicated;
        }

        /// <summary>
        /// Creates a new group of Photons servers
        /// Note: Description will be shortened to the max length silently, if one port, IP or name is not valid, the photon server will not be added
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="photonServersViews"></param>
        /// <param name="photonsGroupId"></param>
        /// <returns></returns>
        public static PhotonGroupOperationResult CreatePhotonGroup(string name, string description, List<PhotonView> photonServersViews, out int photonsGroupId)
        {
            PhotonGroupOperationResult ret = PhotonGroupOperationResult.Ok;

            photonsGroupId = 0;

            bool isNameValid = ValidationUtilities.IsValidPhotonsGroupName(name);
            bool isNameDuplicate = IsPhotonsGroupNameDuplicated(name);

            if (isNameValid && !isNameDuplicate)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    DateTime now = DateTime.Now;

                    PhotonsGroup photonsGroup = new PhotonsGroup();
                    photonsGroup.CreationDate = now;
                    photonsGroup.Description = description.ShortenText(CommonConfig.PhotonsGroupDescriptionMaxLength, false);
                    photonsGroup.ModificationDate = now;
                    photonsGroup.Name = ValidationUtilities.StandardizePhotonsGroupName(name);

                    paradiseDB.PhotonsGroups.InsertOnSubmit(photonsGroup);
                    paradiseDB.SubmitChanges();

                    List<PhotonServer> photonServers = new List<PhotonServer>();

                    foreach (PhotonView photonServerView in photonServersViews)
                    {
                        if (ValidationUtilities.IsValidIPAddress(photonServerView.IP) && ValidationUtilities.IsValidPortNumber(photonServerView.Port))
                        {
                            PhotonServer photonServer = new PhotonServer();
                            photonServer.IP = photonServerView.IP;
                            photonServer.Name = ValidationUtilities.StandardizePhotonServerName(photonServerView.Name);
                            photonServer.PhotonsGroupID = photonsGroup.PhotonsGroupID;
                            photonServer.Port = photonServerView.Port;
                            photonServer.Region = (int)photonServerView.Region;
                            photonServer.UsageType = (int)photonServerView.UsageType;

                            photonServers.Add(photonServer);
                        }
                    }

                    paradiseDB.PhotonServers.InsertAllOnSubmit(photonServers);
                    paradiseDB.SubmitChanges();

                    photonsGroupId = photonsGroup.PhotonsGroupID;

                    ret = PhotonGroupOperationResult.Ok;
                }
            }
            else if (!isNameValid)
            {
                ret = PhotonGroupOperationResult.InvalidName;
            }
            else if (isNameDuplicate)
            {
                ret = PhotonGroupOperationResult.DuplicateName;
            }
            else
            {
                throw new ArgumentOutOfRangeException("name=" + name + "&description=" + description + "&isNameValid=" + isNameValid + "&isNameDuplicate=" + isNameDuplicate);
            }

            return ret;
        }

        /// <summary>
        /// Delete a photon group
        /// </summary>
        /// <param name="photonsGroupId"></param>
        /// <returns></returns>
        public static PhotonGroupOperationResult DeletePhotonGroup(int photonsGroupId)
        {
            if (photonsGroupId == 0)
                return PhotonGroupOperationResult.GroupNotFound;
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                var photonServers = paradiseDB.PhotonServers.Where(d => d.PhotonsGroupID == photonsGroupId).ToList();
                var photonGroup = paradiseDB.PhotonsGroups.First(d => d.PhotonsGroupID == photonsGroupId);

                if (photonGroup != null)
                {
                    paradiseDB.PhotonServers.DeleteAllOnSubmit(photonServers);
                    paradiseDB.PhotonsGroups.DeleteOnSubmit(photonGroup);
                    paradiseDB.SubmitChanges();
                    return PhotonGroupOperationResult.Ok;
                }
            }
            return PhotonGroupOperationResult.UnknownError;
        }

        /// <summary>
        /// Modify a Photon servers group
        /// Note: Description will be shortened to the max length silently, if one port, IP or name is not valid, the photon server will not be added
        /// </summary>
        /// <param name="photonGroupId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="photonServersToDelete"></param>
        /// <param name="photonServersToModify"></param>
        /// <param name="photonServersToAdd"></param>
        /// <returns></returns>
        public static PhotonGroupOperationResult UpdatePhotonGroup(int photonGroupId, string name, string description, List<int> photonServersToDelete, List<PhotonView> photonServersToModify, List<PhotonView> photonServersToAdd)
        {
            PhotonGroupOperationResult ret = PhotonGroupOperationResult.Ok;

            bool isNameValid = ValidationUtilities.IsValidPhotonsGroupName(name);
            bool isNameDuplicate = IsPhotonsGroupNameDuplicated(name, photonGroupId);

            if (isNameValid && !isNameDuplicate)
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    PhotonsGroup photonGroup = uberStrikeDb.PhotonsGroups.SingleOrDefault(pG => pG.PhotonsGroupID == photonGroupId);

                    if (photonGroup != null)
                    {
                        photonGroup.Name = ValidationUtilities.StandardizePhotonsGroupName(name);
                        photonGroup.Description = description.ShortenText(CommonConfig.PhotonsGroupDescriptionMaxLength, false);
                        photonGroup.ModificationDate = DateTime.Now;

                        // Photons modification

                        Dictionary<int, PhotonView> modifiedPhotons = new Dictionary<int, PhotonView>();
                        List<int> modifiedPhotonsIds = new List<int>();

                        foreach (PhotonView photonServerToModify in photonServersToModify)
                        {
                            if (ValidationUtilities.IsValidIPAddress(photonServerToModify.IP) && ValidationUtilities.IsValidPortNumber(photonServerToModify.Port) && ValidationUtilities.IsValidPhotonServerName(photonServerToModify.Name))
                            {
                                modifiedPhotons.Add(photonServerToModify.PhotonId, photonServerToModify);
                                modifiedPhotonsIds.Add(photonServerToModify.PhotonId);
                            }
                        }

                        List<PhotonServer> modifiedPhotonServers = uberStrikeDb.PhotonServers.Where(pS => modifiedPhotonsIds.Contains(pS.PhotonServerID)).ToList();

                        foreach (PhotonServer photonServer in modifiedPhotonServers)
                        {
                            if (modifiedPhotons.ContainsKey(photonServer.PhotonServerID))
                            {
                                photonServer.IP = modifiedPhotons[photonServer.PhotonServerID].IP;
                                photonServer.Port = modifiedPhotons[photonServer.PhotonServerID].Port;
                                photonServer.UsageType = (int)modifiedPhotons[photonServer.PhotonServerID].UsageType;
                                photonServer.Name = ValidationUtilities.StandardizePhotonServerName(modifiedPhotons[photonServer.PhotonServerID].Name);
                                photonServer.Region = (int)(int)modifiedPhotons[photonServer.PhotonServerID].Region;
                            }
                            else
                            {
                                // TODO should we delete the Photon?
                            }
                        }

                        // Photons deletion

                        List<PhotonServer> deletedPhotonServers = uberStrikeDb.PhotonServers.Where(pS => photonServersToDelete.Contains(pS.PhotonServerID)).ToList();

                        uberStrikeDb.PhotonServers.DeleteAllOnSubmit(deletedPhotonServers);

                        // Photons adding

                        List<PhotonServer> addedPhotonServers = new List<PhotonServer>();

                        foreach (PhotonView photonServerToAdd in photonServersToAdd)
                        {
                            if (ValidationUtilities.IsValidIPAddress(photonServerToAdd.IP) && ValidationUtilities.IsValidPortNumber(photonServerToAdd.Port))
                            {
                                PhotonServer photonServer = new PhotonServer();
                                photonServer.IP = photonServerToAdd.IP;
                                photonServer.Name = ValidationUtilities.StandardizePhotonServerName(photonServerToAdd.Name);
                                photonServer.PhotonsGroupID = photonGroupId;
                                photonServer.Port = photonServerToAdd.Port;
                                photonServer.Region = (int)photonServerToAdd.Region;
                                photonServer.UsageType = (int)photonServerToAdd.UsageType;

                                addedPhotonServers.Add(photonServer);
                            }
                        }

                        uberStrikeDb.PhotonServers.InsertAllOnSubmit(addedPhotonServers);

                        uberStrikeDb.SubmitChanges();

                        ret = PhotonGroupOperationResult.Ok;
                    }
                    else
                    {
                        ret = PhotonGroupOperationResult.GroupNotFound;
                    }
                }
            }
            else if (!isNameValid)
            {
                ret = PhotonGroupOperationResult.InvalidName;
            }
            else if (isNameDuplicate)
            {
                ret = PhotonGroupOperationResult.DuplicateName;
            }
            else
            {
                throw new ArgumentOutOfRangeException("photonGroupID=" + photonGroupId + "&name=" + name + "&description=" + description + "&isNameValid=" + isNameValid + "&isNameDuplicate=" + isNameDuplicate);
            }

            return ret;
        }

        /// <summary>
        /// Updates the file name for all the current web channels
        /// Will not create version if missing
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool UpdateFileName(string versionName, string fileName)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isUpdated = false;

                List<ApplicationVersion> versions = uberStrikeDb.ApplicationVersions.Where(v => v.IsEnabled && CommonConfig.WebChannels.Contains((ChannelType)v.Channel) && v.Version == versionName).ToList();

                if (versions.Count > 0)
                {
                    foreach (ApplicationVersion version in versions)
                    {
                        version.WebPlayerFileName = fileName;
                        version.ModificationDate = DateTime.Now;
                    }

                    uberStrikeDb.SubmitChanges();

                    isUpdated = true;
                }

                return isUpdated;
            }
        }

        /// <summary>
        /// Get the current version view for a specific channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static ApplicationVersion GetCurrentApplicationVersion(ChannelType channel)
        {
            ApplicationVersion applicationVersion = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                applicationVersion = uberStrikeDb.ApplicationVersions.SingleOrDefault(aV => aV.IsEnabled == true && aV.Channel == (int)channel);
            }

            return applicationVersion;
        }

        /// <summary>
        /// Get the current version view for a specific channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ApplicationView GetApplicationView(ChannelType channel, string version)
        {
            ApplicationVersion applicationVersion = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                applicationVersion = uberStrikeDb.ApplicationVersions.SingleOrDefault(aV => aV.Version == version && aV.Channel == (int)channel);
            }

            return GetApplicationView(applicationVersion);
        }

        /// <summary>
        /// DEPRECATED, UGLY FIX FOR CURRENT PROD
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ApplicationView GetCurrentApplicationView(ChannelType channel, string version)
        {
            ApplicationVersion applicationVersion = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                applicationVersion = uberStrikeDb.ApplicationVersions.SingleOrDefault(aV => aV.Channel == (int)channel && aV.Version == version && aV.IsEnabled);

                if (applicationVersion == null)
                {
                    applicationVersion = uberStrikeDb.ApplicationVersions.FirstOrDefault(aV => aV.Channel == (int)channel && aV.IsEnabled);
                }
            }

            return GetApplicationView(applicationVersion);
        }

        /// <summary>
        /// Adds the Photon servers to the ApplicationVersion to make an ApplicationView
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ApplicationView GetApplicationView(ApplicationVersion version)
        {
            ApplicationView applicationView = null;

            if (version != null)
            {
                List<PhotonServer> photonServers = GetPhotonServerList(version.PhotonClusterId);
                List<PhotonView> photonServersViews = new List<PhotonView>(photonServers.Count);

                foreach (var p in photonServers)
                {
                    var currentServer = new PhotonView()
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

                applicationView = new ApplicationView(version.ApplicationVersionId, version.Version, BuildType.Prod, (ChannelType)version.Channel, version.WebPlayerFileName, version.ModificationDate, version.ModificationDate, version.IsEnabled, "", version.PhotonClusterId, photonServersViews);
            }

            return applicationView;
        }

        /// <summary>
        /// Get all the Photon servers view linked to a version
        /// </summary>
        /// <param name="version"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<PhotonView> GetAllPhotonServerViews(string version, ChannelType channel)
        {
            List<PhotonView> photonsView = new List<PhotonView>();

            string cacheName = UberStrikeCacheKeys.GetDeprecatedPhotonServersViewParameters + CmuneCacheKeys.Separator + version + CmuneCacheKeys.Separator + ((int)channel).ToString();

            if (HttpRuntime.Cache[cacheName] != null)
            {
                photonsView = (List<PhotonView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ApplicationVersion applicationVersion = ApplicationVersionService.GetApplicationVersion(version, channel).ToApplicationVersion();

                    if (applicationVersion != null)
                    {
                        List<PhotonServer> photons = applicationVersion.PhotonsGroup.PhotonServers.ToList();
                        photonsView = photons.ConvertAll(new Converter<PhotonServer, PhotonView>(p => GetPhotonServerView(p)));
                    }

                    HttpRuntime.Cache.Add(cacheName, photonsView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }

            return photonsView;
        }

        /// <summary>
        /// Convert a PhotonServer to a PhotonView
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static PhotonView GetPhotonServerView(PhotonServer server)
        {
            PhotonView photonView = null;

            if (server != null)
            {
                photonView = new PhotonView()
                    {
                        PhotonId = server.PhotonServerID,
                        IP = server.IP,
                        Name = server.Name,
                        Region = (RegionType)server.Region,
                        Port = server.Port,
                        UsageType = (PhotonUsageType)server.UsageType,
                        MinLatency = server.MinLatency,
                    };
            }

            return photonView;
        }

        #region Live feeds

        /// <summary>
        /// Retrieve the Live Feed list from the DB
        /// </summary>
        /// <returns></returns>
        public static List<LiveFeedView> GetLiveFeedView()
        {
            List<LiveFeedView> liveFeedViewList = new List<LiveFeedView>();

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<LiveFeed> liveFeeds = uberStrikeDb.LiveFeeds.ToList();

                liveFeedViewList = liveFeeds.ConvertAll<LiveFeedView>(new Converter<LiveFeed, LiveFeedView>(t => new LiveFeedView(t.Date, t.Priority, t.Description, t.Url, t.LiveFeedId)));
            }

            return liveFeedViewList;
        }

        /// <summary>
        /// Creates a live feed
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <returns>false => empty description</returns>
        public static bool CreateLiveFeed(int priority, string description, string url)
        {
            bool isLivedFeedCreated = false;

            if (!description.IsNullOrFullyEmpty())
            {
                priority = StandardizeLiveFeedPriority(priority);
                description = StandardizeLiveFeedDescription(description);
                url = StandardizeLiveFeedUrl(url);

                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    LiveFeed liveFeed = new LiveFeed();
                    liveFeed.Date = DateTime.Now;
                    liveFeed.Description = description;
                    liveFeed.Priority = byte.Parse(priority.ToString());
                    liveFeed.Url = url;

                    uberStrikeDb.LiveFeeds.InsertOnSubmit(liveFeed);
                    uberStrikeDb.SubmitChanges();

                    isLivedFeedCreated = true;
                }
            }

            return isLivedFeedCreated;
        }

        /// <summary>
        /// Deletes a live feed
        /// </summary>
        /// <param name="liveFeedId"></param>
        /// <returns>false => the live feed is not existing</returns>
        public static bool DeleteLiveFeed(int liveFeedId)
        {
            bool isLiveFeedDeleted = false;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                LiveFeed liveFeed = uberStrikeDb.LiveFeeds.SingleOrDefault(lF => lF.LiveFeedId == liveFeedId);

                if (liveFeed != null)
                {
                    uberStrikeDb.LiveFeeds.DeleteOnSubmit(liveFeed);
                    uberStrikeDb.SubmitChanges();

                    isLiveFeedDeleted = true;
                }
            }

            return isLiveFeedDeleted;
        }

        /// <summary>
        /// Updates a live feed
        /// </summary>
        /// <param name="liveFeedId"></param>
        /// <param name="priority"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <returns>return false => live feed is not existing or description is null or empty</returns>
        public static bool UpdateLiveFeed(int liveFeedId, int priority, string description, string url)
        {
            bool isLiveFeedUpdated = false;

            if (!description.IsNullOrFullyEmpty())
            {
                priority = StandardizeLiveFeedPriority(priority);
                description = StandardizeLiveFeedDescription(description);
                url = StandardizeLiveFeedUrl(url);

                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    LiveFeed liveFeed = uberStrikeDb.LiveFeeds.SingleOrDefault(lF => lF.LiveFeedId == liveFeedId);

                    if (liveFeed != null)
                    {
                        liveFeed.Date = DateTime.Now;
                        liveFeed.Description = description;
                        liveFeed.Priority = byte.Parse(priority.ToString());
                        liveFeed.Url = url;

                        uberStrikeDb.SubmitChanges();

                        isLiveFeedUpdated = true;
                    }
                }
            }

            return isLiveFeedUpdated;
        }

        /// <summary>
        /// Standardize the live feed priority
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static int StandardizeLiveFeedPriority(int priority)
        {
            if (!UberStrikeCommonConfig.LiveFeedPriorityNames.Keys.Contains(priority))
            {
                priority = UberStrikeCommonConfig.LiveFeedNormalPriority;
            }

            return priority;
        }

        /// <summary>
        /// Standardize the live feed description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string StandardizeLiveFeedDescription(string description)
        {
            if (description != null && description.Length > UberStrikeCommonConfig.LiveFeedDescriptionMaxLength)
            {
                description = description.ShortenText(UberStrikeCommonConfig.LiveFeedDescriptionMaxLength);
            }

            return description;
        }

        /// <summary>
        /// Standardize the live feed url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string StandardizeLiveFeedUrl(string url)
        {
            if (url != null && url.Length > UberStrikeCommonConfig.LiveFeedUrlMaxLength)
            {
                url = url.ShortenText(UberStrikeCommonConfig.LiveFeedUrlMaxLength);
            }

            return url;
        }

        #endregion Live feeds

        #region Maps

        public static bool IsDuplicateMapApplicationVersion(string appVersion)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                // For the moment we do nor check the MapVersion

                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
                return uberStrikeDb.Maps.Where(m => m.AppVersion == appVersion).Select(m => m.MapId).Count() > 0;
            }
        }

        public static MapOperationResult SaveAsMapCluster(string oldAppVersion, string newAppVersion)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                MapOperationResult result = MapOperationResult.Error;

                if (result == MapOperationResult.Error && !UberStrikeValidationUtilities.IsValidApplicationVersion(newAppVersion))
                {
                    result = MapOperationResult.InvalidApplicationVersion;
                }

                oldAppVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(oldAppVersion);
                newAppVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(newAppVersion);

                if (result == MapOperationResult.Error && IsDuplicateMapApplicationVersion(newAppVersion))
                {
                    result = MapOperationResult.DuplicateApplicationVersion;
                }

                if (result == MapOperationResult.Error)
                {
                    List<Map> oldMaps = GetMaps(oldAppVersion, uberStrikeDb);
                    List<Map> newMaps = new List<Map>();

                    foreach (Map oldMap in oldMaps)
                    {
                        Map newMap = new Map();

                        newMap.AppVersion = newAppVersion;
                        newMap.Description = oldMap.Description;
                        newMap.DisplayName = oldMap.DisplayName;
                        newMap.InUse = oldMap.InUse;
                        newMap.MapId = oldMap.MapId;
                        newMap.SceneName = oldMap.SceneName;
                        newMap.IsBlueBox = oldMap.IsBlueBox;
                        newMap.RecommendedItemId = oldMap.RecommendedItemId;
                        newMaps.Add(newMap);
                    }

                    uberStrikeDb.Maps.InsertAllOnSubmit(newMaps);

                    uberStrikeDb.SubmitChanges();

                    result = MapOperationResult.Ok;
                }

                return result;
            }
        }

        public static MapClusterView GetMapCluster(string appVersion)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);

                List<Map> maps = GetMaps(appVersion, uberStrikeDb);
                List<MapVersion> mapsVersion = uberStrikeDb.MapVersions.Where(v => v.AppVersion == appVersion).ToList();
                List<MapInfoView> mapsInfo = new List<MapInfoView>();

                foreach (Map map in maps)
                {
                    Dictionary<MapType, MapVersionView> currentMapTypes = mapsVersion.Where(q => q.MapId == map.MapId).ToDictionary(q => (MapType)q.MapType, q => new MapVersionView(q.FileName, q.LastUpdatedDate));

                    int recommendedItemId = 0;
                    if (map.RecommendedItemId.HasValue)
                        recommendedItemId = map.RecommendedItemId.Value;
                    mapsInfo.Add(new MapInfoView(map.MapId, map.DisplayName, map.SceneName, map.Description, map.InUse, map.IsBlueBox, recommendedItemId, currentMapTypes));
                }

                MapClusterView mapCluster = new MapClusterView(appVersion, mapsInfo);

                return mapCluster;
            }
        }

        public static List<string> GetMapClusterVersions()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<string> appVersions = uberStrikeDb.Maps.Select(m => m.AppVersion).Distinct().ToList();

                return appVersions;
            }
        }

        public static MapOperationResult CreateMap(int mapId, string appVersion, string displayName, string description, string sceneName, bool isBlueBox)
        {
            MapOperationResult result = CheckCreateUpdateMapArguments(mapId, appVersion, displayName, description, sceneName);

            appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
            displayName = UberStrikeValidationUtilities.StandardizeMapDisplayName(displayName);
            description = UberStrikeValidationUtilities.StandardizeMapDescription(description);
            sceneName = UberStrikeValidationUtilities.StandardizeMapSceneName(sceneName);

            if (result == MapOperationResult.Error)
            {
                DuplicateMapInfo duplicateMapInfo = AreDuplicateMapInfo(appVersion, mapId, displayName, sceneName, true);
                MapOperationResult duplicateResult = ToMapOperationResult(duplicateMapInfo);

                if (duplicateResult == MapOperationResult.Ok)
                {
                    using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                    {
                        Map map = new Map();

                        map.AppVersion = appVersion;
                        map.Description = description;
                        map.DisplayName = displayName;
                        map.InUse = false;
                        map.MapId = mapId;
                        map.SceneName = sceneName;
                        map.IsBlueBox = isBlueBox;

                        uberStrikeDb.Maps.InsertOnSubmit(map);
                        uberStrikeDb.SubmitChanges();

                        result = MapOperationResult.Ok;
                    }
                }
                else
                {
                    result = duplicateResult;
                }
            }

            return result;
        }

        private static MapOperationResult CheckCreateUpdateMapArguments(int mapId, string appVersion, string displayName, string description, string sceneName)
        {
            MapOperationResult result = MapOperationResult.Error;

            if (!UberStrikeValidationUtilities.IsValidMapId(mapId))
            {
                result = MapOperationResult.InvalidMapId;
            }

            if (result == MapOperationResult.Error && !UberStrikeValidationUtilities.IsValidApplicationVersion(appVersion))
            {
                result = MapOperationResult.InvalidApplicationVersion;
            }

            if (result == MapOperationResult.Error && !UberStrikeValidationUtilities.IsValidMapDisplayName(displayName))
            {
                result = MapOperationResult.InvalidDisplayName;
            }

            if (result == MapOperationResult.Error && !UberStrikeValidationUtilities.IsValidMapDescription(description))
            {
                result = MapOperationResult.InvalidDescription;
            }

            if (result == MapOperationResult.Error && !UberStrikeValidationUtilities.IsValidMapSceneName(sceneName))
            {
                result = MapOperationResult.InvalidDescription;
            }

            return result;
        }

        public static MapOperationResult UpdateMap(int mapId, string appVersion, string displayName, string description, string sceneName)
        {
            MapOperationResult result = CheckCreateUpdateMapArguments(mapId, appVersion, displayName, description, sceneName);

            appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
            displayName = UberStrikeValidationUtilities.StandardizeMapDisplayName(displayName);
            description = UberStrikeValidationUtilities.StandardizeMapDescription(description);
            sceneName = UberStrikeValidationUtilities.StandardizeMapSceneName(sceneName);

            if (result == MapOperationResult.Error)
            {
                DuplicateMapInfo duplicateMapInfo = AreDuplicateMapInfo(appVersion, mapId, displayName, sceneName, false);
                MapOperationResult duplicateResult = ToMapOperationResult(duplicateMapInfo);

                if (duplicateResult == MapOperationResult.Ok)
                {
                    using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                    {
                        Map map = GetMap(appVersion, mapId, uberStrikeDb);

                        if (map != null)
                        {
                            map.Description = description;
                            map.DisplayName = displayName;
                            map.SceneName = sceneName;

                            uberStrikeDb.SubmitChanges();

                            result = MapOperationResult.Ok;
                        }
                        else
                        {
                            result = MapOperationResult.NotFound;
                        }
                    }
                }
                else
                {
                    result = duplicateResult;
                }
            }

            return result;
        }

        public static MapView GetMapView(string appVersion, int mapId)
        {
            return ToMapView(GetMap(appVersion, mapId));
        }

        public static MapView ToMapView(Map map)
        {
            MapView mapView = null;

            if (map != null)
            {
                mapView = new MapView
                {
                    Description = map.Description,
                    DisplayName = map.DisplayName,
                    MapId = map.MapId,
                    SceneName = map.SceneName,
                    IsBlueBox = map.IsBlueBox,
                    RecommendedItemId = map.RecommendedItemId ?? 0,
                };
            }

            return mapView;
        }

        public static Map GetMap(string appVersion, int mapId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                return GetMap(appVersion, mapId, uberStrikeDb);
            }
        }

        private static Map GetMap(string appVersion, int mapId, UberstrikeDataContext uberStrikeDb)
        {
            Map map = null;

            if (uberStrikeDb != null)
            {
                map = uberStrikeDb.Maps.SingleOrDefault(m => m.MapId == mapId && m.AppVersion == appVersion);
            }

            return map;
        }

        private static List<Map> GetMaps(string appVersion, UberstrikeDataContext uberStrikeDb)
        {
            List<Map> maps = new List<Map>();

            if (uberStrikeDb != null)
            {
                maps = uberStrikeDb.Maps.Where(m => m.AppVersion == appVersion).ToList();
            }

            return maps;
        }

        private static MapVersion GetMapVersion(string appVersion, int mapId, MapType mapType, UberstrikeDataContext uberStrikeDb)
        {
            MapVersion mapVersion = null;

            if (uberStrikeDb != null)
            {
                mapVersion = uberStrikeDb.MapVersions.SingleOrDefault(v => v.MapId == mapId && v.AppVersion == appVersion && v.MapType == (int)mapType);
            }

            return mapVersion;
        }

        private static List<MapVersion> GetMapVersions(string appVersion, int mapId, UberstrikeDataContext uberStrikeDb)
        {
            List<MapVersion> mapVersions = new List<MapVersion>();

            if (uberStrikeDb != null)
            {
                mapVersions = uberStrikeDb.MapVersions.Where(v => v.MapId == mapId && v.AppVersion == appVersion).ToList();
            }

            return mapVersions;
        }

        private static List<MapVersion> GetMapsVersion(string appVersion, UberstrikeDataContext uberStrikeDb)
        {
            List<MapVersion> mapsVersion = new List<MapVersion>();

            if (uberStrikeDb != null)
            {
                mapsVersion = uberStrikeDb.MapVersions.Where(v => v.AppVersion == appVersion).ToList();
            }

            return mapsVersion;
        }

        private static MapOperationResult ToMapOperationResult(DuplicateMapInfo duplicateMapInfo)
        {
            MapOperationResult result = MapOperationResult.Ok;

            if (!duplicateMapInfo.IsDuplicateDisplayName && !duplicateMapInfo.IsDuplicateMapId && !duplicateMapInfo.IsDuplicateSceneName)
            {
                result = MapOperationResult.Ok;
            }
            else if (duplicateMapInfo.IsDuplicateDisplayName && duplicateMapInfo.IsDuplicateMapId && duplicateMapInfo.IsDuplicateSceneName)
            {
                result = MapOperationResult.DuplicateMapIdDisplayNameSceneName;
            }
            else if (duplicateMapInfo.IsDuplicateDisplayName && duplicateMapInfo.IsDuplicateMapId)
            {
                result = MapOperationResult.DuplicateMapIdDisplayName;
            }
            else if (duplicateMapInfo.IsDuplicateDisplayName && duplicateMapInfo.IsDuplicateSceneName)
            {
                result = MapOperationResult.DuplicateDisplayNameSceneName;
            }
            else if (duplicateMapInfo.IsDuplicateMapId && duplicateMapInfo.IsDuplicateSceneName)
            {
                result = MapOperationResult.DuplicateMapIdSceneName;
            }
            else if (duplicateMapInfo.IsDuplicateDisplayName)
            {
                result = MapOperationResult.DuplicateDisplayName;
            }
            else if (duplicateMapInfo.IsDuplicateMapId)
            {
                result = MapOperationResult.DuplicateMapId;
            }
            else if (duplicateMapInfo.IsDuplicateSceneName)
            {
                result = MapOperationResult.DuplicateSceneName;
            }
            else
            {
                throw new ArgumentException("else if case missing");
            }

            return result;
        }

        public static DuplicateMapInfo AreDuplicateMapInfo(string appVersion, int mapId, string displayName, string sceneName, bool isCreating)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isDuplicateMapId = false;
                bool isDuplicateDisplayName = false;
                bool isDuplicateSceneName = false;

                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
                displayName = UberStrikeValidationUtilities.StandardizeMapDisplayName(displayName);
                sceneName = UberStrikeValidationUtilities.StandardizeMapSceneName(sceneName);

                if (isCreating)
                {
                    var createQuery = from m in uberStrikeDb.Maps
                                      where m.AppVersion == appVersion && (m.MapId == mapId || m.DisplayName == displayName || m.SceneName == sceneName)
                                      select new { MapId = m.MapId, DisplayName = m.DisplayName, SceneName = m.SceneName };

                    foreach (var row in createQuery)
                    {
                        if (row.MapId == mapId)
                        {
                            isDuplicateMapId = true;
                        }

                        if (row.DisplayName == displayName)
                        {
                            isDuplicateDisplayName = true;
                        }

                        if (row.SceneName == sceneName)
                        {
                            isDuplicateSceneName = true;
                        }
                    }
                }
                else
                {
                    var updateQuery = from m in uberStrikeDb.Maps
                                      where m.AppVersion == appVersion && (m.DisplayName == displayName || m.SceneName == sceneName) && m.MapId != mapId
                                      select new { MapId = m.MapId, DisplayName = m.DisplayName, SceneName = m.SceneName };

                    foreach (var row in updateQuery)
                    {
                        if (row.DisplayName == displayName)
                        {
                            isDuplicateDisplayName = true;
                        }

                        if (row.SceneName == sceneName)
                        {
                            isDuplicateSceneName = true;
                        }
                    }
                }

                DuplicateMapInfo result = new DuplicateMapInfo(isDuplicateMapId, isDuplicateDisplayName, isDuplicateSceneName);

                return result;
            }
        }

        public class DuplicateMapInfo
        {
            #region Properties

            public bool IsDuplicateMapId { get; private set; }
            public bool IsDuplicateDisplayName { get; private set; }
            public bool IsDuplicateSceneName { get; private set; }

            #endregion

            #region Constructors

            public DuplicateMapInfo(bool isDuplicateMapId, bool isDuplicateDisplayName, bool isDuplicateSceneName)
            {
                IsDuplicateMapId = isDuplicateMapId;
                IsDuplicateDisplayName = isDuplicateDisplayName;
                IsDuplicateSceneName = isDuplicateSceneName;
            }

            #endregion
        }

        public static bool IsDuplicateMapId(string appVersion, int mapId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
                return uberStrikeDb.Maps.Where(m => m.MapId == mapId && m.AppVersion == appVersion).Select(m => m.MapId).Count() > 0;
            }
        }

        public static bool IsDuplicateMapDisplayName(string appVersion, string displayName)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
                displayName = UberStrikeValidationUtilities.StandardizeMapDisplayName(displayName);
                return uberStrikeDb.Maps.Where(m => m.DisplayName == displayName && m.AppVersion == appVersion).Select(m => m.MapId).Count() > 0;
            }
        }

        public static bool IsDuplicateMapSceneName(string appVersion, string sceneName)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);
                sceneName = UberStrikeValidationUtilities.StandardizeMapSceneName(sceneName);
                return uberStrikeDb.Maps.Where(m => m.SceneName == sceneName && m.AppVersion == appVersion).Select(m => m.MapId).Count() > 0;
            }
        }

        public static MapOperationResult UpdateMapInUse(string appVersion, int mapId, bool isInUse)
        {
            MapOperationResult result = MapOperationResult.Error;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Map map = GetMap(appVersion, mapId, uberStrikeDb);

                if (map != null)
                {
                    map.InUse = isInUse;
                    uberStrikeDb.SubmitChanges();

                    result = MapOperationResult.Ok;
                }
                else
                {
                    result = MapOperationResult.NotFound;
                }
            }

            return result;
        }

        public static MapOperationResult UpdateIsBlueBox(string appVersion, int mapId, bool isBlueBox)
        {
            MapOperationResult result = MapOperationResult.Error;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Map map = GetMap(appVersion, mapId, uberStrikeDb);

                if (map != null)
                {
                    map.IsBlueBox = isBlueBox;
                    uberStrikeDb.SubmitChanges();

                    result = MapOperationResult.Ok;
                }
                else
                {
                    result = MapOperationResult.NotFound;
                }
            }

            return result;
        }

        public static MapOperationResult UpdateMapRecommendedItem(string appVersion, int mapId, int itemId)
        {
            MapOperationResult result = MapOperationResult.Error;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Map map = GetMap(appVersion, mapId, uberStrikeDb);

                if (map != null)
                {
                    map.RecommendedItemId = itemId;

                    uberStrikeDb.SubmitChanges();
                    result = MapOperationResult.Ok;
                }
                else
                {
                    result = MapOperationResult.NotFound;
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes a a specific map and map version linked to an application version
        /// </summary>
        /// <param name="appVersion"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static MapOperationResult DeleteMap(string appVersion, int mapId)
        {
            MapOperationResult result = MapOperationResult.Error;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);

                Map map = GetMap(appVersion, mapId, uberStrikeDb);
                List<MapVersion> mapVersions = GetMapVersions(appVersion, mapId, uberStrikeDb);

                if (mapVersions.Count > 0)
                {
                    uberStrikeDb.MapVersions.DeleteAllOnSubmit(mapVersions);
                    uberStrikeDb.SubmitChanges();
                }

                if (map != null)
                {
                    uberStrikeDb.Maps.DeleteOnSubmit(map);
                    uberStrikeDb.SubmitChanges();

                    result = MapOperationResult.Ok;
                }
                else
                {
                    result = MapOperationResult.NotFound;
                }
            }

            return result;
        }

        /// <summary>
        /// Delete all maps and maps version linked to an application version
        /// </summary>
        /// <param name="appVersion"></param>
        /// <returns></returns>
        public static MapOperationResult DeleteMaps(string appVersion)
        {
            MapOperationResult result = MapOperationResult.Error;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);

                List<Map> maps = GetMaps(appVersion, uberStrikeDb);
                List<MapVersion> mapsVersion = GetMapsVersion(appVersion, uberStrikeDb);

                if (mapsVersion.Count > 0)
                {
                    uberStrikeDb.MapVersions.DeleteAllOnSubmit(mapsVersion);
                    uberStrikeDb.SubmitChanges();
                }

                if (maps.Count > 0)
                {
                    uberStrikeDb.Maps.DeleteAllOnSubmit(maps);
                    uberStrikeDb.SubmitChanges();

                    result = MapOperationResult.Ok;
                }
                else
                {
                    result = MapOperationResult.NotFound;
                }
            }

            return result;
        }

        public static bool UpdateMapVersion(int mapId, string fileName, string appVersion, MapType mapType)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isMapUpdated = false;

                if (UberStrikeValidationUtilities.IsValidApplicationVersion(appVersion) && !fileName.IsNullOrFullyEmpty())
                {
                    appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);

                    MapVersion mapVersion = GetMapVersion(appVersion, mapId, mapType, uberStrikeDb);

                    if (mapVersion == null)
                    {
                        mapVersion = new MapVersion();

                        mapVersion.AppVersion = appVersion;
                        mapVersion.FileName = fileName;
                        mapVersion.MapId = mapId;
                        mapVersion.LastUpdatedDate = DateTime.Now;
                        mapVersion.MapType = (int)mapType;

                        uberStrikeDb.MapVersions.InsertOnSubmit(mapVersion);
                        uberStrikeDb.SubmitChanges();
                    }
                    else if (fileName != mapVersion.FileName)
                    {
                        mapVersion.FileName = fileName;
                        mapVersion.LastUpdatedDate = DateTime.Now;

                        uberStrikeDb.SubmitChanges();
                    }

                    isMapUpdated = true;
                }

                return isMapUpdated;
            }
        }

        public static List<MapView> GetMaps(string appVersion, MapType mapType)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                appVersion = UberStrikeValidationUtilities.StandardizeApplicationVersion(appVersion);

                var query = from m in uberStrikeDb.Maps
                            join v in uberStrikeDb.MapVersions on m.MapId equals v.MapId
                            where m.AppVersion == appVersion && v.AppVersion == appVersion && m.InUse && v.MapType == (int)mapType
                            select new MapView
                            {
                                MapId = m.MapId,
                                DisplayName = m.DisplayName,
                                Description = m.Description,
                                SceneName = m.SceneName,
                                FileName = v.FileName,
                                IsBlueBox = m.IsBlueBox,
                                RecommendedItemId = m.RecommendedItemId ?? 0,
                                SupportedGameModes = -1, //all
                                MaxPlayers = UberStrikeCommonConfig.MaxPlayers,
                            };
                var listOfMaps = query.ToList();

                return listOfMaps;
            }
        }

        public static Dictionary<int, string> GetMapsName()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Dictionary<int, string> names = (from m in uberStrikeDb.Maps
                                                 orderby m.Id descending
                                                 group m by new { MapId = m.MapId, DisplayName = m.SceneName } into m2
                                                 select new { MapId = m2.Key.MapId, MapName = m2.Key.DisplayName }).ToDictionary(m => m.MapId, m => m.MapName);

                return names;
            }
        }

        #endregion
    }
}