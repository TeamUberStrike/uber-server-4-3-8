using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Channels.Utils
{
    /// <summary>
    /// Utils used in Uberstrike channels
    /// </summary>
    public static class UnityArgsBuilder
    {
        /// <summary>
        /// Write the first arguments of the Unity file to load
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public static string BuildInitialUnityArguments(ChannelType channelType, string cultureName)
        {
            return BuildInitialUnityArguments(channelType, cultureName, String.Empty);
        }

        /// <summary>
        /// Write the first arguments of the Unity file to load
        /// </summary>
        /// <param name="channelType"></param>
        /// <param name="cultureName"></param>
        /// <param name="embedType"></param>
        /// <returns></returns>
        public static string BuildInitialUnityArguments(ChannelType channelType, string cultureName, string embedType)
        {
            StringBuilder initialUnityArguments = new StringBuilder();

            #region Unity file name

            if (!ConfigurationUtilities.ReadConfigurationManager("UnityWebPlayerUrl").IsNullOrFullyEmpty())
            {
                // We can override the application version from the DB with the Web.config
                initialUnityArguments.Append(ConfigurationUtilities.ReadConfigurationManager("UnityWebPlayerUrl"));
            }
            else
            {
                string cacheName = String.Format("{0}{1}{2}", UberStrikeCacheKeys.CheckApplicationVersionViewParameters, CmuneCacheKeys.Separator, (int)channelType);
                ApplicationVersion buildVersion = null;

                if (HttpRuntime.Cache[cacheName] != null)
                {
                    buildVersion = (ApplicationVersion)HttpRuntime.Cache[cacheName];
                }
                else
                {
                    buildVersion = Games.GetCurrentApplicationVersion(channelType);
                    HttpRuntime.Cache.Add(cacheName, buildVersion, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }

                if (buildVersion != null)
                {
                    initialUnityArguments.Append(buildVersion.WebPlayerFileName);
                }
                else
                {
                    CmuneLog.LogUnexpectedReturn(channelType, "No current version on " + channelType.ToString() + ".");
                    throw new NullReferenceException("No version is existing for channel: " + channelType);
                }
            }

            #endregion Unity file name

            #region Channel type

            initialUnityArguments.Append("?channeltype=");
            initialUnityArguments.Append(((int)channelType));

            #endregion Channel type

            #region Locale

            initialUnityArguments.Append("&lang=");
            initialUnityArguments.Append(Uri.EscapeDataString(cultureName));

            #endregion Locale

            if (!embedType.IsNullOrFullyEmpty())
            {
                initialUnityArguments.Append("&embedtype=");
                initialUnityArguments.Append(Uri.EscapeDataString(embedType));
            }

            return initialUnityArguments.ToString();
        }

        /// <summary>
        /// Get the SuperRewards application Id according to the type of channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string GetSuperRewardsApplicationId(ChannelType channel)
        {
            string applicationId = String.Empty;
            Dictionary<string, string> secrets = new Dictionary<string, string>();

            string applicationIdsFromConfig = ConfigurationUtilities.ReadConfigurationManager("SuperRewardsApplicationIds");
            string[] applicationIdsPerChannel = applicationIdsFromConfig.Split('|');

            foreach (string applicationIdPerChannel in applicationIdsPerChannel)
            {
                string[] channelApplicationId = applicationIdPerChannel.Split(',');

                if (channelApplicationId.Length == 2)
                {
                    if (!secrets.ContainsKey(channelApplicationId[0]))
                    {
                        secrets.Add(channelApplicationId[0], channelApplicationId[1]);
                    }
                    else
                    {
                        CmuneLog.LogUnexpectedReturn(applicationIdsFromConfig, "We have a duplicate key in \"SuperRewardsApplicationIds\": [" + channelApplicationId[0] + "]");
                    }
                }
            }

            secrets.TryGetValue(((int)channel).ToString(), out applicationId);

            return applicationId;
        }
    }
}