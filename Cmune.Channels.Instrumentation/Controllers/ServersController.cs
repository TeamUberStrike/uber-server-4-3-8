using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Instrumentation.Monitoring.Business;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.Channels.Instrumentation.Business;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ServersController : BaseController
    {
        public ServersController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Deployment;
        }

        [HttpGet]
        public ActionResult Index(int managedServerId = 0)
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Deployment;
            ViewBag.SubActiveTab = CmuneMenu.DeploymentSubTabs.Servers;
            ViewData["ManagedServersListView"] = ManagedServerService.GetManagedServers();

            ManagedServerModel selectedServer = ManagedServerService.GetManagedServer(managedServerId);
            return View();
        }

        [HttpPost]
        public JsonResult DeleteManagedServer(int managedServerId = 0)
        {
            bool response;
            response = ManagedServerService.DeleteManagedServer(managedServerId);
            return new JsonResult() { Data = new { isError = !response } };
        }

        [HttpPost]
        public ActionResult EnableOrDisableManagedServer(string actionType, int managedServerId)
        {
            bool isEnabledOrDisabled = false;
            string message = string.Empty;

            switch (actionType)
            {
                case "enable":
                    ManagedServerService.EnableManagedServer(managedServerId);
                    isEnabledOrDisabled = true;
                    break;
                case "disable":
                    ManagedServerService.DisableManagedServer(managedServerId);
                    isEnabledOrDisabled = true;
                    break;
            }
            message = isEnabledOrDisabled ? "Operation sucessfully done" : "Error occured";
            var json = new JsonResult();
            json.Data = new { message = message, isEnabledOrDisabled = isEnabledOrDisabled };
            return json;
        }


        [HttpPost]
        public ActionResult LoadAddOrEditManagedServerForm(int managedServerId = 0)
        {
            ManagedServerModel managedServerModel = new ManagedServerModel();

            if (managedServerId > 0)
            {
                managedServerModel = ManagedServerService.GetManagedServer(managedServerId);
            }
            ViewData["PhotonRegionTypeListItems"] = AdminCache.GenerateRegionTypeDropDownListItems();
            return PartialView("Partial/Form/AddEditManagedServerForm", managedServerModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddOrEditManagedServer(ManagedServerModel managedServer)
        {
            bool isAddOrEdit = false;
            string message = string.Empty;
            MonitoringOperationResult result = MonitoringOperationResult.Ok;
            bool isActionDone = false;

            if (ModelState.IsValid)
            {
                if (managedServer.ManagedServerId2 > 0)
                {
                    managedServer.NextPollTime = DateTime.Now;
                    result = ManagedServerService.EditManagedServer(managedServer);
                    isActionDone = true;
                }
                else
                {
                    managedServer.NextPollTime = DateTime.Now;
                    result = ManagedServerService.CreateManagedServer(managedServer);
                    isActionDone = true;
                }

                if (isActionDone == true)
                    CheckOperationResult.CheckMonitoringOperationResult(result, ref message, ref isAddOrEdit);
            }
            else
            {
                isAddOrEdit = false;
                foreach (var ele in ModelState.Values.Where(d => d.Errors.Count > 0))
                {
                    //.Select(d2=>d2.Errors.First().ErrorMessage + "<br/>")
                    message += ele.Errors.First().ErrorMessage + "<br/>";
                }
            }
            var json = new JsonResult();
            json.Data = new { message = message, isAddOrEdit = isAddOrEdit };
            return json;
        }

    }
}
