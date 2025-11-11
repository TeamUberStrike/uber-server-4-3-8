using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Business;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class DeploymentController : BaseController
    {
        #region Properties and Parameters

        const int PHOTONSERVERS_NUMBER = 15;

        #endregion

        #region Constructors

        public DeploymentController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Deployment;
        }

        #endregion

        #region Action

        public ActionResult Index()
        {
            return RedirectToAction("ManageApplicationVersions");
        }

        [HttpGet]
        public ActionResult ManageApplicationVersions()
        {
            ViewBag.SubActiveTab = CmuneMenu.DeploymentSubTabs.ManageApplicationVersions;
            ViewBag.ApplicationVersions = ApplicationVersionService.GetCurrentApplicationVersions();
            ViewBag.WebChannels = CommonConfig.WebChannels;
            ViewBag.StandaloneChannels = CommonConfig.StandaloneChannels;
            return View();
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None)]
        public ActionResult ManagePhotons(int? photonGroupId)
        {
            ViewBag.ActiveTab = CmuneMenu.MainTabs.Deployment;
            ViewBag.SubActiveTab = CmuneMenu.DeploymentSubTabs.Photons;
            ViewBag.Title = String.Format("{0} | {1}", CmuneMenu.MainTabs.Deployment, CmuneMenu.DeploymentSubTabs.Photons);

            ViewData["SelectPhotonsGroupDropDownListData"] = GetPhotonGroupsList((photonGroupId.HasValue) ? photonGroupId.Value : 0);
            return View();
        }

        [HttpGet]
        public ActionResult ManageMaps()
        {
            ViewBag.ActiveTab = CmuneMenu.MainTabs.Deployment;
            ViewBag.SubActiveTab = CmuneMenu.DeploymentSubTabs.Maps;
            ViewBag.Title = String.Format("{0} | {1}", CmuneMenu.MainTabs.Deployment, CmuneMenu.DeploymentSubTabs.Maps);

            ViewData["applicationVersions"] = MapService.GetMapApplicationVersions();

            return View();
        }

        [HttpGet]
        public ActionResult CreateApplicationMilestones()
        {
            ViewBag.SubActiveTab = CmuneMenu.DeploymentSubTabs.Milestones;
            return View();
        }

        #endregion

        #region Methods

        #region Application versions

        [HttpPost]
        public ActionResult LoadApplicationVersionAddForm()
        {
            ViewBag.PhotonGroupsList = Games.GetPhotonGroupsOrderedByModificationDate();
            return PartialView("Partial/Form/ApplicationVersionForm", new ApplicationVersionViewModel());
        }

        [HttpPost]
        public ActionResult GetApplicationVersions()
        {
            return PartialView("Partial/ApplicationVersions", ApplicationVersionService.GetCurrentApplicationVersions());
        }

        [HttpPost]
        public ActionResult LoadApplicationVersionEditForm(int applicationVersionId)
        {
            var applicationVersionViewModel = ApplicationVersionService.GetApplicationVersion(applicationVersionId);
            ViewBag.PhotonGroupsList = Games.GetPhotonGroupsOrderedByModificationDate();
            return PartialView("Partial/Form/ApplicationVersionForm", applicationVersionViewModel);
        }

        [HttpPost]
        public JsonResult SaveApplicationVersion(ApplicationVersionViewModel applicationVersionModel)
        {
            string invalidStates = "";

            ChannelType channelType;

            if (EnumUtilities.TryParseEnumByValue(Request.Params["ChannelType"], out channelType))
            {
                if (applicationVersionModel.ApplicationVersionId == 0)
                {
                    applicationVersionModel.ModificationDate = DateTime.Now;
                    applicationVersionModel.Channel = channelType;

                    if (applicationVersionModel.IsValid(out invalidStates))
                    {
                        if (CommonConfig.WebChannels.Contains(applicationVersionModel.Channel) && applicationVersionModel.IsEnabled)
                        {
                            ApplicationVersionService.DisableAllApplicationVersionsByChannelType(applicationVersionModel.Channel);
                        }

                        ApplicationVersionService.AddApplicationVersion(applicationVersionModel);

                        UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
                    }
                }
                else if (applicationVersionModel.ApplicationVersionId > 0)
                {
                    applicationVersionModel.ModificationDate = DateTime.Now;
                    applicationVersionModel.Channel = channelType;

                    if (applicationVersionModel.IsValid(out invalidStates))
                    {
                        var applicationVersionModelDb = ApplicationVersionService.GetApplicationVersion(applicationVersionModel.ApplicationVersionId);

                        if (CommonConfig.WebChannels.Contains(applicationVersionModel.Channel) && applicationVersionModel.IsEnabled)
                        {
                            ApplicationVersionService.DisableAllApplicationVersionsByChannelType(applicationVersionModel.Channel);
                        }

                        ApplicationVersionService.EditApplicationVersion(applicationVersionModel);

                        UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
                    }
                }
            }
            else
            {
                invalidStates = "Couldn't parse channel";
            }

            return new JsonResult() { Data = new { errorMessage = invalidStates } };
        }

        [HttpPost]
        public JsonResult DeleteApplicationVersion(int applicationVersionId)
        {
            if (applicationVersionId > 0)
            {
                ApplicationVersionService.DeleteApplicationVersion(applicationVersionId);

                UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
            }

            return new JsonResult() { Data = new { } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateApplicationVersionIsEnabled(int applicationVersionId, bool isEnabled)
        {
            bool isUpdated = ApplicationVersionService.UpdateApplicationVersionIsEnabled(applicationVersionId, isEnabled);

            if (isUpdated)
            {
                UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
            }

            return new JsonResult { Data = new { IsOk = isUpdated } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateApplicationVersionWarnPlayer(int applicationVersionId, bool warnPlayer)
        {
            bool isUpdated = ApplicationVersionService.UpdateApplicationVersionWarnPlayer(applicationVersionId, warnPlayer);

            if (isUpdated)
            {
                UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
            }

            return new JsonResult { Data = new { IsOk = isUpdated } };
        }

        #endregion

        #region Photon clusters

        List<SelectListItem> GetPhotonGroupsList(int selectedGroup = 0)
        {
            List<PhotonsGroup> photonsGroups = Games.GetPhotonGroupsOrderedByModificationDate();
            List<SelectListItem> photonsGroupDropDownList = new List<SelectListItem>();

            foreach (PhotonsGroup photonsGroup in photonsGroups)
            {
                SelectListItem listItem = new SelectListItem() { Text = photonsGroup.Name, Value = photonsGroup.PhotonsGroupID.ToString(), Selected = photonsGroup.PhotonsGroupID == selectedGroup };
                photonsGroupDropDownList.Add(listItem);
            }

            // means that user havent select anything
            if (selectedGroup == 0)
            {
                photonsGroupDropDownList.First().Selected = true;
            }

            return photonsGroupDropDownList;
        }

        List<SelectListItem> GetPhotonUsageTypeList()
        {
            var photonUsageTypeList = AdminCache.GeneratePhotonUsageTypeDropDownListItems();
            List<SelectListItem> PhotonsUsageTypeList = new List<SelectListItem>();
            foreach (var photoUsage in photonUsageTypeList)
            {
                SelectListItem listItem = new SelectListItem() { Text = photoUsage.Text, Value = photoUsage.Value };
                PhotonsUsageTypeList.Add(listItem);
            }
            return PhotonsUsageTypeList;
        }

        [HttpGet]
        public ActionResult GetPhotonsCluster(int photonGroupId)
        {
            PhotonsClusterView photonsCluster = Games.GetPhotonClusterView(photonGroupId);

            List<ApplicationVersionViewModel> linkedApplications = new List<ApplicationVersionViewModel>();
            List<ApplicationVersionViewModel> currentApplicationsLinked = new List<ApplicationVersionViewModel>();
            List<ApplicationVersionViewModel> previousApplicationsLinked = new List<ApplicationVersionViewModel>();

            if (photonsCluster != null)
            {
                linkedApplications = ApplicationVersionService.GetApplicationVersionsByPhotonClusterId(photonGroupId);

                foreach (ApplicationVersionViewModel linkedApplication in linkedApplications)
                {
                    if (linkedApplication.IsEnabled)
                    {
                        currentApplicationsLinked.Add(linkedApplication);
                    }
                    else
                    {
                        previousApplicationsLinked.Add(linkedApplication);
                    }
                }
            }
            else
            {
                // The group is not existing
            }

            // init the current linked version
            ViewData["CurrentLinkedVersionsListView"] = currentApplicationsLinked;

            // init the previous linked version
            ViewData["PreviousLinkedVersionsListView"] = previousApplicationsLinked;

            // init photo dropdownlist
            // TODO init on page load only
            ViewData["PhotonRegionTypeListItems"] = AdminCache.GenerateRegionTypeDropDownListItems();
            ViewData["PhotonUsageTypeListData"] = GetPhotonUsageTypeList();
            ViewBag.ManagedServersList = ManagedServerService.GetManagedServers();
            return View("Partial/PhotonsCluster", photonsCluster);
        }

        public JsonResult GetPhotonClustersDropDownList()
        {
            return Json(GetPhotonGroupsList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePhotonCluster(FormCollection form)
        {
            bool isSaved = false;
            string message = String.Empty;

            PhotonGroupOperationResult operationResult = PhotonGroupOperationResult.Ok;
            int photonsGroupId = Int32.Parse(form["photonsGroupId"]);

            // We update an existing group
            List<PhotonView> photonsToModify = new List<PhotonView>();
            List<int> photonsToDelete = new List<int>();
            int currentPhotonServersCount = Int32.Parse(form["currentPhotonServersCount"]);
            ParsePhotonsToModifyAndDelete(form, currentPhotonServersCount, out photonsToModify, out photonsToDelete);
            List<PhotonView> photonsToAdd = ParsePhotonsToAdd(form);

            operationResult = Games.UpdatePhotonGroup(photonsGroupId, form["NameTextBox"].ToString(), String.Empty, photonsToDelete, photonsToModify, photonsToAdd);

            if (operationResult.Equals(PhotonGroupOperationResult.Ok))
            {
                isSaved = true;
                UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
            }

            // Display error 
            if (!operationResult.Equals(PhotonGroupOperationResult.Ok))
            {
                switch (operationResult)
                {
                    case PhotonGroupOperationResult.DuplicateName:
                        message = "The name that you entered is duplicated.";
                        break;
                    case PhotonGroupOperationResult.InvalidName:
                        if (!form["NameTextBox"].IsNullOrFullyEmpty())
                        {
                            message = "The name that you entered is invalid.";
                        }
                        else
                        {
                            message = "The Photons cluster name can't be empty.";
                        }
                        break;
                    case PhotonGroupOperationResult.GroupNotFound:
                        message = "This photons group is not existing.";
                        break;
                    default:
                        message = "Ooops! This error has been logged in.";
                        break;
                }
            }
            else
            {
                message = "The Photons cluster was saved successfully.";
            }

            var json = new JsonResult();
            json.Data = new { IsSaved = isSaved, Message = message };
            return json;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SavePhotonClusterAs(FormCollection form)
        {
            bool isSaved = false;
            string message = String.Empty;

            PhotonGroupOperationResult operationResult = PhotonGroupOperationResult.Ok;

            int photonGroupId = 0;

            List<PhotonView> newPhotons = ParsePhotonsToAdd(form);

            // TODO: Maybe we should parse only the photons to copy accross (and not the one to delete)
            List<int> photonsToDelete = new List<int>();
            List<PhotonView> photonsToModify = new List<PhotonView>();
            int numberOfCurrentPhotonServers = Int32.Parse(form["currentPhotonServersCount"]);
            ParsePhotonsToModifyAndDelete(form, numberOfCurrentPhotonServers, out photonsToModify, out photonsToDelete);

            newPhotons.AddRange(photonsToModify);

            operationResult = Games.CreatePhotonGroup(form["NameTextBox"], String.Empty, newPhotons, out photonGroupId);

            if (operationResult.Equals(PhotonGroupOperationResult.Ok))
            {
                UberStrikeCacheInvalidation.InvalidateApplicationVersion(BuildType);
                isSaved = true;
            }

            // Display error message
            if (!operationResult.Equals(PhotonGroupOperationResult.Ok))
            {
                switch (operationResult)
                {
                    case PhotonGroupOperationResult.DuplicateName:
                        message = "The name that you entered is already taken.";
                        break;
                    case PhotonGroupOperationResult.InvalidName:
                        if (!form["NameTextBox"].IsNullOrFullyEmpty())
                        {
                            message = "The name that you entered is invalid.";
                        }
                        else
                        {
                            message = "The Photons cluster name can't be empty.";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format("PhotonGroupOperationResult not found: {0}", operationResult));
                }
            }
            else
            {
                message = "The Photons cluster was saved successfully.";
            }

            var json = new JsonResult();
            json.Data = new { IsSaved = isSaved, Message = message, PhotonClusterId = photonGroupId };
            return json;
        }

        [HttpPost]
        public ActionResult DeletePhotonCluster(int photonsClusterId)
        {
            bool isSuccess = false;
            string message = string.Empty;

            if (photonsClusterId > 0)
            {
                var returnCode = Games.DeletePhotonGroup(photonsClusterId);
                if (returnCode == PhotonGroupOperationResult.Ok)
                    isSuccess = true;
            }

            return new JsonResult() { Data = new { isSuccess = isSuccess, message = message } };
        }

        [HttpGet]
        public ActionResult LoadSavePhotonsClusterAs()
        {
            return View("Partial/Form/SavePhotonsClusterAsForm");
        }

        protected void ParsePhotonsToModifyAndDelete(FormCollection form, int photonsServerCount, out List<PhotonView> photonsToModify, out List<int> photonsToDelete)
        {
            photonsToModify = new List<PhotonView>();
            photonsToDelete = new List<int>();

            int photonId = 0;
            string photonIp = string.Empty;
            int photonPort = 0;
            string photonName = string.Empty;
            PhotonUsageType photonUsageType = PhotonUsageType.None;
            RegionType photonRegionType = RegionType.EuWest;
            var managedServerList = ManagedServerService.GetManagedServers();

            for (int i = 1; i <= photonsServerCount; i++)
            {
                photonId = Int32.Parse(form["IDHiddenField" + i]);
                photonIp = form["IPTextBox" + i];
                photonName = form["NameTextBox" + i];
                Int32.TryParse(form["PortTextBox" + i], out photonPort);
                photonUsageType = (PhotonUsageType)Int32.Parse(form["UsageDropDownList" + i]);
                photonRegionType = (RegionType)managedServerList.First(d => photonIp == d.PublicIp).Region;


                if (ValidationUtilities.IsValidIPAddress(photonIp) && ValidationUtilities.IsValidPortNumber(photonPort) && ValidationUtilities.IsValidPhotonServerName(photonName))
                {
                    var photon = new PhotonView()
                    {
                        PhotonId = photonId,
                        IP = photonIp,
                        Name = photonName,
                        Region = photonRegionType,
                        Port = photonPort,
                        UsageType = photonUsageType,
                    };
                    photonsToModify.Add(photon);
                }
                else if (photonIp == String.Empty || photonName == String.Empty)
                {
                    photonsToDelete.Add(photonId);
                }
            }
        }

        [ValidateInput(false)]
        protected List<PhotonView> ParsePhotonsToAdd(FormCollection form)
        {
            List<PhotonView> newPhotons = new List<PhotonView>();

            string photonIp = string.Empty;
            int photonPort = 0;
            string photonName = string.Empty;
            PhotonUsageType photonUsageType = PhotonUsageType.None;
            bool isPhotonUsageTypeParsed = false;
            var managedServerList = ManagedServerService.GetManagedServers();

            for (int i = 1; i <= PHOTONSERVERS_NUMBER; i++)
            {
                photonIp = form["NewIPTextBox" + i];
                photonName = form["NewNameTextBox" + i];
                Int32.TryParse(form["NewPortTextBox" + i], out photonPort);
                isPhotonUsageTypeParsed = EnumUtilities.TryParseEnumByValue(form["NewUsageDropDownList" + i], out photonUsageType);

                if (ValidationUtilities.IsValidIPAddress(photonIp) && ValidationUtilities.IsValidPortNumber(photonPort) && ValidationUtilities.IsValidPhotonServerName(photonName))
                {
                    if (isPhotonUsageTypeParsed)
                    {
                        var photonRegionType = (RegionType)managedServerList.First(d => photonIp == d.PublicIp).Region;
                        var photon = new PhotonView()
                        {
                            IP = photonIp,
                            Name = photonName,
                            Region = photonRegionType,
                            Port = photonPort,
                            UsageType = photonUsageType,
                        };
                        newPhotons.Add(photon);
                    }
                }
            }

            return newPhotons;
        }

        #endregion

        #region Maps


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetMapApplicationVersionsSelect(string selectedAppVersion = "")
        {
            return Json(MapService.GetMapApplicationVersions(selectedAppVersion));
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult GetMapsCluster(string applicationVersion)
        {
            MapClusterView mapsCluster = Games.GetMapCluster(applicationVersion);

            List<SelectListItem> mapTypes = new List<SelectListItem>();

            int i = 0;
            foreach (MapType mapType in EnumUtilities.IterateEnum<MapType>())
            {
                if (mapType != MapType.None)
                {
                    SelectListItem listItem = new SelectListItem() { Text = mapType.ToString(), Value = ((int)mapType).ToString(), Selected = i == 0 };
                    mapTypes.Add(listItem);
                    i++;
                }
            }

            ViewBag.MapTypes = mapTypes;
            return PartialView("Partial/MapsCluster", mapsCluster);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateMapVersion(int mapId, string fileName, string mapAppVersion, MapType mapType)
        {
            bool isUpdated = Games.UpdateMapVersion(mapId, fileName, mapAppVersion, mapType);

            return new JsonResult { Data = new { IsUpdated = isUpdated } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult LoadAddMap(string applicationVersion)
        {
            MapView mapView = null;

            ViewBag.ApplicationVersion = applicationVersion;

            return PartialView("Partial/Form/AddOrEditMapForm", mapView);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddMap(string appVersion, int mapId, string displayName, string description, string sceneName, bool isBlueBox)
        {
            MapOperationResult result = Games.CreateMap(mapId, appVersion, displayName, description, sceneName, isBlueBox);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString(), AppVersion = appVersion } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult LoadEditMap(string applicationVersion, int mapId)
        {
            MapView mapView = Games.GetMapView(applicationVersion, mapId);

            ViewBag.ApplicationVersion = applicationVersion;

            return PartialView("Partial/Form/AddOrEditMapForm", mapView);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditMap(string appVersion, int mapId, string displayName, string description, string sceneName)
        {
            MapOperationResult result = Games.UpdateMap(mapId, appVersion, displayName, description, sceneName);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString(), AppVersion = appVersion } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateMapInUse(string appVersion, int mapId, bool isInUse)
        {
            MapOperationResult result = Games.UpdateMapInUse(appVersion, mapId, isInUse);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString() } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateIsBlueBox(string appVersion, int mapId, bool isBlueBox)
        {
            MapOperationResult result = Games.UpdateIsBlueBox(appVersion, mapId, isBlueBox);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString() } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DeleteMap(string appVersion, int mapId)
        {
            MapOperationResult result = Games.DeleteMap(appVersion, mapId);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString() } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveAsMapCluster(string appVersion, string clusterName)
        {
            MapOperationResult result = Games.SaveAsMapCluster(appVersion, clusterName);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString() } };
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DeleteMaps(string appVersion)
        {
            MapOperationResult result = Games.DeleteMaps(appVersion);

            return new JsonResult { Data = new { IsOk = result == MapOperationResult.Ok, StatusMessage = result.ToString() } };
        }

        #endregion

        #region Milestones

        public ActionResult GetApplicationMilestones()
        {
            List<ApplicationMilestoneView> milestones = ApplicationDeployment.GetApplicationMilestonesView(CommonConfig.ApplicationIdUberstrike);
            return View("Partial/ApplicationMilestones", milestones);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateApplicationMilestone(string milestoneDateTextBox, string milestoneDescriptionTextBox)
        {
            string message = string.Empty;

            DateTime creationDate = DateTime.Now;
            TryParseMilestoneCreationDate(milestoneDateTextBox, out creationDate);

            bool isMilestoneCreated = ApplicationDeployment.CreateApplicationMilestone(CommonConfig.ApplicationIdUberstrike, milestoneDescriptionTextBox, creationDate);

            if (isMilestoneCreated)
            {
                message = "The milestone was created succesfully";
            }
            else
            {
                message = "This description is already used by the same application";
            }

            var json = new JsonResult();
            json.Data = new { IsMilestoneCreated = isMilestoneCreated, Message = message };
            return json;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditApplicationMilestone(int milestoneId, string description, string creationDate)
        {
            string message = string.Empty;

            DateTime creationDateArgument = DateTime.Now;
            TryParseMilestoneCreationDate(creationDate, out creationDateArgument);

            bool isMilestoneEdited = ApplicationDeployment.EditApplicationMilestone(milestoneId, description, creationDateArgument);

            if (isMilestoneEdited)
            {
                message = "The milestone was created succesfully";
            }
            else
            {
                message = "This description is already used by the same application";
            }

            var json = new JsonResult();
            json.Data = new { IsMilestoneEdited = isMilestoneEdited, Message = message };
            return json;
        }

        public ActionResult DeleteApplicationMilestone(int milestoneId)
        {
            string message = string.Empty;

            bool isMilestoneDeleted = ApplicationDeployment.DeleteApplicationMilestone(milestoneId);

            if (isMilestoneDeleted)
            {
                message = "The milestone was deleted succesfully.";
            }
            else
            {
                message = "This milestone couldn't be found.";
            }

            var json = new JsonResult();
            json.Data = new { IsMilestoneDeleted = isMilestoneDeleted, Message = message };
            return json;
        }

        bool TryParseMilestoneCreationDate(string formEntry, out DateTime creationDate)
        {
            bool isDateParsed = false;
            creationDate = DateTime.Now;

            if (!formEntry.IsNullOrFullyEmpty())
            {
                DateTime tmpDate;
                string creationDateForm = formEntry;

                if (creationDateForm.Length > 10)
                {
                    if (DateTime.TryParseExact(formEntry, "yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out tmpDate))
                    {
                        creationDate = tmpDate;
                        isDateParsed = true;
                    }
                }
                else if (DateTime.TryParseExact(formEntry, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out tmpDate))
                {
                    creationDate = tmpDate;
                    isDateParsed = true;
                }
            }

            return isDateParsed;
        }

        #endregion

        #endregion
    }
}