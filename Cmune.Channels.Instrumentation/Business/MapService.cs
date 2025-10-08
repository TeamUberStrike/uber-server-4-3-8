using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.DataCenter.Business;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class MapService
    {
        public static List<SelectListItem> GetMapApplicationVersions(string selectedAppVersion = "")
        {
            List<string> applicationVersions = Games.GetMapClusterVersions().OrderByDescending(q => q).ToList();

            List<SelectListItem> applicationVersionsDropDownList = new List<SelectListItem>();

            foreach (string applicationVersion in applicationVersions)
            {
                SelectListItem listItem = new SelectListItem() { Text = applicationVersion, Value = applicationVersion, Selected = applicationVersion == selectedAppVersion };
                applicationVersionsDropDownList.Add(listItem);
            }

            // If the selected application version is empty we select the first one
            if (String.IsNullOrEmpty(selectedAppVersion) && applicationVersionsDropDownList.Count > 0)
            {
                applicationVersionsDropDownList.First().Selected = true;
            }

            return applicationVersionsDropDownList;
        }
    }
}