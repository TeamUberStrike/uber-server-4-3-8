using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Utils;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class SubTabsHelper 
    {
        public static string GetSubTabsPath(this HtmlHelper helper, Enum subTabs)
        {
            string subTabToRender = string.Empty;
            Type type = subTabs.GetType();
            if (type.Name == typeof(CmuneMenu.StatisticSubTabs).Name)
                return "SubMenu/StatsSubMenu";
            else if (type.Name == typeof(CmuneMenu.MemberSubTabs).Name)
                return "SubMenu/MemberSubMenu";
            else if (type.Name == typeof(CmuneMenu.ItemSubTabs).Name)
                return "SubMenu/ItemSubMenu";
            else if (type.Name == typeof(CmuneMenu.DeploymentSubTabs).Name)
                return "SubMenu/DeploymentSubMenu";
            else if (type.Name == typeof(CmuneMenu.MonitoringSubTabs).Name)
                return "SubMenu/MonitoringSubMenu";
            else if (type.Name == typeof(CmuneMenu.ManagementSubTabs).Name)
                return "SubMenu/ManagementSubMenu";
            else if (type.Name == typeof(CmuneMenu.SixWavesSubTabs).Name)
                return "SubMenu/SixWavesSubMenu";
            else if (type.Name == typeof(CmuneMenu.UtilitiesSubTabs).Name)
                return "SubMenu/UtilitiesSubMenu";
            return string.Empty;
        }

    }
}
