using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Business;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Utils;
using System;

namespace Cmune.Instrumentation.WebServices
{
    public partial class WebServicesClass : IAllWebServicesInterfaces
    {
        /// <summary>
        /// Check whether the WS are pingable
        /// </summary>
        /// <returns></returns>
        public string IsAlive()
        {
            return "yes";
        }

        public void SetInstanceState(string versionNumber, string instanceName, string ip, int port, int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount)
        {
            PhotonMonitoring.SetInstanceState(versionNumber, instanceName, ip, port, cpuUtilization, ramUtilization, instanceRamMb, peerCount);
        }

        public bool UpdateMapVersion(int mapId, string fileName, string appVersion, MapType mapType)
        {
            try
            {
                return Games.UpdateMapVersion(mapId, fileName, appVersion, mapType);
            }
            catch (System.Exception ex)
            {
                CmuneLog.LogException(ex, string.Format("mapId={0}&fileName={1}&appVersion={2}&mapType={3}", mapId, fileName, appVersion, mapType));
                return false;
            }
        }

        public bool UpdateFileName(string versionName, string fileName)
        {
            try
            {
                bool isUpdated = Games.UpdateFileName(versionName, fileName);

                if (isUpdated)
                {
                    string buildIndex = ConfigurationUtilities.ReadConfigurationManager("NLogBuild");
                    BuildType buildType;
                    EnumUtilities.TryParseEnumByValue(buildIndex, out buildType);
                    UberStrikeCacheInvalidation.InvalidateApplicationVersion(buildType);
                }

                return isUpdated;
            }
            catch (System.Exception ex)
            {
                CmuneLog.LogException(ex, string.Format("versionName={0}&fileName={1}", versionName, fileName));
                return false;
            }
        }

        public bool UpdateUnityClientUrl(string versionName, string fileName, string channel)
        {
            ChannelType channelType;
            try
            {
                if (Enum.TryParse<ChannelType>(channel, out channelType))
                {
                    bool isUpdated = ApplicationVersionService.UpdateUnityClientUrl(versionName, fileName, channelType);

                    if (isUpdated)
                    {
                        BuildType buildType;
                        if (EnumUtilities.TryParseEnumByValue(ConfigurationUtilities.ReadConfigurationManager("NLogBuild"), out buildType))
                            UberStrikeCacheInvalidation.InvalidateApplicationVersion(buildType);
                    }

                    return isUpdated;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                CmuneLog.LogException(ex, string.Format("versionName={0}&fileName={1}&channel={2}", versionName, fileName, channel));
                return false;
            }
        }
    }
}