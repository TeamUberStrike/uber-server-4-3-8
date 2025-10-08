
using System;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using log4net.Config;
using Photon.SocketServer;
using UberStrike.Realtime.Common.IO;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class GameServerApplication : ApplicationBase
    {
        protected override void Setup()
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo("bin\\log4net.config"));

                //setup delegates
                AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
                //PhotonPeer.SendBufferFull += PhotonPeer_SendBufferFull;

                CmuneDebug.AddDebugChannel(new DefaultDebug());
                CmuneDebug.AddDebugChannel(new PhotonDebug());

                //save instance name
                ServerSettings.InstanceName = base.PhotonInstanceName;

                ApplicationConfiguration.ReadCmuneConfigFile();
                ConfigurationReader.ReadSocketServerConfigFile();

                //get the server name
                ServerSettings.NickName = UberStrike.WebService.DotNet.ApplicationWebServiceClient.GetPhotonServerName(ServerSettings.AppVersion, ServerSettings.IP, ServerSettings.Port);

                //setup serialization 
                RealtimeSerialization.Converter = new UberStrikeByteConverter();

                //mount application center
                ApplicationCenter.Instance = new GameApplicationCenter();

                //extend network class factory to support game modes
                ServerNetworkClassManager.Factory = new GameServerNetworkClassFactory();

                //run webservice routines to retrieve leveCaps/XP event lists/etc
                GameDataManager.Instance.StartUpdateGameEventData(30);

                CmunePerformanceCounter.Instance.StartPerformanceCounter();
                CmunePerformanceCounter.Instance.StartMonitorInstance();

                //create permanent rooms
                PermanentRoomManager.Instance.CreatePermanentRooms();

                CmuneDebug.LogInfo("GameServer running as instance " + base.PhotonInstanceName + " and NickName" + ServerSettings.NickName);
            }
            catch (Exception e)
            {
                throw CmuneDebug.Exception("An Exception happend when starting up GameServerApplication with Message: {0}", e.Message);
            }
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            int cmid;
            if (int.TryParse(initRequest.ApplicationId, out cmid))
                return new GamePeer(initRequest.Protocol, initRequest.PhotonPeer, cmid);
            else
            {
                initRequest.PhotonPeer.DisconnectClient();
                return null;
            }
        }

        protected override void TearDown()
        { }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CmuneDebug.LogError(e);

            if (e.IsTerminating)
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("Process is terminating.");
            }
        }
    }
}