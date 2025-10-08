using System.Collections.Generic;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IApplicationWebService
    {
        [System.Obsolete("Replaced by AuthenticateApplication")]
        [DontEncryptMethodAttribute]
        RegisterClientApplicationViewModel RegisterClientApplication(int cmuneId, string hashCode, ChannelType channel, int applicationId);
        [System.Obsolete]
        List<PhotonView> GetPhotonServers(ApplicationView applicationView);
        [System.Obsolete]
        string GetMyIP();

        [DontEncryptMethodAttribute]
        AuthenticateApplicationView AuthenticateApplication(string version, ChannelType channel, string publicKey);
        void RecordException(int cmid, BuildType buildType, ChannelType channelType, string buildNumber, string logString, string stackTrace, string exceptionData);
        [DontEncryptMethodAttribute]
        void RecordExceptionUnencrypted(BuildType buildType, ChannelType channelType, string buildNumber, string errorType, string errorMessage);
        void RecordTutorialStep(int cmid, TutorialStepType step);
        bool ReportBug(BugView bugView);
        List<LiveFeedView> GetLiveFeed();
        List<MapView> GetMaps(string appVersion, UberStrike.Core.Types.LocaleType locale, MapType mapType);
        void SetLevelVersion(int id, int version, string md5Hash);
        string GetPhotonServerName(string applicationVersion, string ipAddress, int port);
        string IsAlive();

        //string ExceptionString();
        //void ExceptionVoid();
        //int Sleep(int milliseconds);
    }
}