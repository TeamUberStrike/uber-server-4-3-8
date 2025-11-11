using System;
using System.IO;
using Photon.SocketServer;
using UberStrike.Core.Serialization;

namespace UberStrike.Realtime.GameServer
{
    public class GameApplication : ApplicationBase
    {
        private static Version serverVersion = new Version(4, 3, 2); // Need to get the native server version

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var connection = ServerConnectionViewProxy.Deserialize(new MemoryStream(Convert.FromBase64String(initRequest.ApplicationId)));

            //only allow connections from a peer of type GAME
            if (connection.ApiVersion == 0)
            {
                return new GameServerPeer(initRequest.Protocol, initRequest.PhotonPeer, connection.Cmid, connection.Channel);
            }
            else
            {
                initRequest.PhotonPeer.DisconnectClient();
                return null;
            }
        }

        protected override void Setup()
        {
            System.Diagnostics.Debug.WriteLine("Starting Game Server Application.");
        }

        protected override void TearDown()
        { }
    }
}
