using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class UberStrikeValidationUtilities
    {
        #region Maps

        /// <summary>
        /// Checks wether a map display name is compliant with our rules
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static bool IsValidMapDisplayName(string displayName)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(displayName))
            {
                displayName = displayName.Trim();

                if (displayName.Equals(TextUtilities.CompleteTrim(displayName)))
                {
                    isValid = (displayName.Length >= UberStrikeCommonConfig.MapDisplayNameMinLength && displayName.Length <= UberStrikeCommonConfig.MapDisplayNameMaxLength);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a map display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static string StandardizeMapDisplayName(string displayName)
        {
            string cleanName = TextUtilities.CompleteTrim(displayName);

            return cleanName;
        }

        /// <summary>
        /// Checks wether a map description is compliant with our rules
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static bool IsValidMapDescription(string description)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(description))
            {
                description = description.Trim();

                if (description.Equals(TextUtilities.CompleteTrim(description)))
                {
                    isValid = (description.Length >= UberStrikeCommonConfig.MapDescriptionMinLength && description.Length <= UberStrikeCommonConfig.MapDescriptionMaxLength);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a map description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string StandardizeMapDescription(string description)
        {
            string cleanName = TextUtilities.CompleteTrim(description);

            return cleanName;
        }

        /// <summary>
        /// Checks wether a map Id is compliant with our rules
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static bool IsValidMapId(int mapId)
        {
            return mapId > 0;
        }

        /// <summary>
        /// Checks wether a map scene name is compliant with our rules
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool IsValidMapSceneName(string sceneName)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(sceneName))
            {
                sceneName = sceneName.Trim();

                if (sceneName.Equals(TextUtilities.CompleteTrim(sceneName)))
                {
                    isValid = (sceneName.Length >= UberStrikeCommonConfig.MapSceneNameMinLength && sceneName.Length <= UberStrikeCommonConfig.MapSceneNameMaxLength);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a map scene name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static string StandardizeMapSceneName(string sceneName)
        {
            string cleanName = TextUtilities.CompleteTrim(sceneName);

            return cleanName;
        }

        #endregion

        /// <summary>
        /// Checks wether a application version is compliant with our rules
        /// </summary>
        /// <param name="appVersion"></param>
        /// <returns></returns>
        public static bool IsValidApplicationVersion(string appVersion)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(appVersion))
            {
                appVersion = appVersion.Trim();

                if (appVersion.Equals(TextUtilities.CompleteTrim(appVersion)))
                {
                    // TODO: Regexp to validate app version: x.x.x

                    isValid = true;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes an application version
        /// </summary>
        /// <param name="appVersion"></param>
        /// <returns></returns>
        public static string StandardizeApplicationVersion(string appVersion)
        {
            string cleanName = TextUtilities.CompleteTrim(appVersion);

            return cleanName;
        }
    }
}