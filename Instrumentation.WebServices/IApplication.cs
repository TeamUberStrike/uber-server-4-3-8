using System.ServiceModel;
using System.ServiceModel.Web;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace Cmune.Instrumentation.WebServices
{
    [ServiceContract]
    public interface IApplication
    {
        /// <summary>
        /// Check whether the WS are pingable
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "IsAlive")]
        string IsAlive();

        /// <summary>
        /// Set a photon instance state
        /// </summary>
        /// <param name="versionNumber"></param>
        /// <param name="instanceName"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="cpuUtilization"></param>
        /// <param name="ramUtilization"></param>
        /// <param name="instanceRamMb"></param>
        /// <param name="peerCount"></param>
        [OperationContract(IsOneWay = true)]
        [WebGet(UriTemplate = "SetInstanceState?versionNumber={versionNumber}&instanceName={instanceName}&ip={ip}&port={port}&cpuUtilization={cpuUtilization}&ramUtilization={ramUtilization}&instanceRamMb={instanceRamMb}&peerCount={peerCount}")]
        void SetInstanceState(string versionNumber, string instanceName, string ip, int port, int cpuUtilization, int ramUtilization, int instanceRamMb, int peerCount);

        [OperationContract]
        [WebGet(UriTemplate = "UpdateMapVersion?mapId={mapId}&fileName={fileName}&appVersion={appVersion}&mapType={mapType}", ResponseFormat = WebMessageFormat.Json)]
        bool UpdateMapVersion(int mapId, string fileName, string appVersion, MapType mapType);

        [OperationContract]
        [WebGet(UriTemplate = "UpdateFileName?versionName={versionName}&fileName={fileName}", ResponseFormat = WebMessageFormat.Json)]
        bool UpdateFileName(string versionName, string fileName);

        [OperationContract]
        [WebGet(UriTemplate = "UpdateUnityClientUrl?versionName={versionName}&clientUrl={clientUrl}&channel={channel}", ResponseFormat = WebMessageFormat.Json)]
        bool UpdateUnityClientUrl(string versionName, string clientUrl, string channel);
    }
}