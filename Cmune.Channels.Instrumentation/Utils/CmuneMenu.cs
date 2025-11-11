using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Utils
{
    public class CmuneMenu
    {
        public enum MainTabs
        {
            Dashboard,
            Statistics,
            Deployment,
            Members,
            Items,
            Monitoring,
            Utilities,
            Management,
            Clans,
            SixWaves,
            Unity
        }

        public enum StatisticSubTabs
        {
            Dashboard,
            User_Activity,
            Revenue,
            Item_Economy,
            Social,
            Payment,
            Channels,
            Regions,
            Retention,
            Referrers,
            UnityInstallation,
            Tutorial,
        }

        public enum MemberSubTabs
        {
            See,
            Search,
            ManageMemberAccess,
            DoCustomActions,
            CustomQueries,
            BannedIps
        }

        public enum ItemSubTabs
        {
            Catalog,
            Management,
            Comparison
        }

        public enum DeploymentSubTabs
        {
            ManageApplicationVersions,
            Servers,
            Photons,
            Milestones,
            Maps
        }

        public enum MonitoringSubTabs
        {
            ServersMonitoring,
            PhotonsHealth,
            UnityExceptions
        }

        public enum ManagementSubTabs
        {
            LiveFeeds,
            EPins,
            Messaging,
            Promotion,
            Bundle,
            FacebookTransactions,
            ManageXp,
            WeeklySpecials,
            MapItem,
            LuckyDraw,
            MysteryBox
        }

        public enum UtilitiesSubTabs
        {
            Email,
            AdminMember
        }

        public enum SixWavesSubTabs
        {
            Dashboard,
            CheckMember
        }
    }
}