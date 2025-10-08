using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using System;
using log4net.Config;
using UberStrike.Realtime.Common.IO;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Photon.Server.Diagnostics;
using System.IO;
using PhotonHostRuntimeInterfaces;
using Photon.SocketServer;

namespace UberStrike.Realtime.Photon.CommServer
{
    public class CommServerApplication : ApplicationBase
    {
        protected override void Setup()
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo("bin\\log4net.config"));

                //setup delegates
                AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

                CmuneDebug.AddDebugChannel(new DefaultDebug());
                CmuneDebug.AddDebugChannel(new PhotonDebug());

                //save instance name
                ServerSettings.InstanceName = base.PhotonInstanceName;

                ApplicationConfiguration.ReadCmuneConfigFile();
                ConfigurationReader.ReadSocketServerConfigFile();

                //setup serialization 
                RealtimeSerialization.Converter = new UberStrikeByteConverter();

                //mount application center
                ApplicationCenter.Instance = new ApplicationCenter();

                //extend network class factory to support game modes
                ServerNetworkClassManager.Factory = new CommServerNetworkClassFactory();

                CmunePerformanceCounter.Instance.StartPerformanceCounter();

                CmuneDebug.LogInfo("CommServer running as instance " + base.PhotonInstanceName);
            }
            catch (Exception e)
            {
                throw CmuneDebug.Exception("An Exception happend when starting up CommServerApplication with Message: {0}", e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photonPeer"></param>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            CommPeer peer = null;
            int cmid;
            if (int.TryParse(initRequest.ApplicationId, out cmid))
            {
                peer = new CommPeer(initRequest.Protocol, initRequest.PhotonPeer, cmid);

                //if (SecurityManager.IsPeerBanned(peer))
                //    SecurityManager.DisconnectPeer(peer);
            }
            //else
            //{
            //    initRequest.PhotonPeer.DisconnectClient();
            //}

            return peer;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void TearDown()
        { }

        /// <summary>
        /// The photon peer_ send buffer full.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        private static void PhotonPeer_SendBufferFull(IPhotonPeer peer)
        {
            if (CmuneDebug.IsWarningEnabled)
                CmuneDebug.LogWarning("disconnecting peer {0}: Send buffer full", peer.GetConnectionID());

            peer.DisconnectClient();
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.
        /// </param>
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