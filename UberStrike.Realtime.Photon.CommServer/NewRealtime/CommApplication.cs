using System;
using System.IO;
using Photon.SocketServer;
using UberStrike.Core.Serialization;
using System.Net;

namespace UberStrike.Realtime.CommServer
{
    public class CommApplication : ApplicationBase
    {
        private static Version serverVersion = new Version(4, 3, 2); // Need to get the native server version

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var connection = ServerConnectionViewProxy.Deserialize(new MemoryStream(Convert.FromBase64String(initRequest.ApplicationId)));

            //only allow connections if API is compatible
            if (connection.ApiVersion == 0)
            {
                return new CommServerPeer(initRequest.Protocol, initRequest.PhotonPeer, connection.Cmid, connection.Channel);
            }
            else
            {
                initRequest.PhotonPeer.DisconnectClient();
                return null;
            }
        }

        protected override void Setup()
        {
            string sHostName = Dns.GetHostName();
            IPHostEntry ipE = Dns.GetHostByName(sHostName);
            IPAddress[] IpA = ipE.AddressList;
            for (int i = 0; i < IpA.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine("IP Address {0}: {1} ", i, IpA[i].ToString());
            }

            System.Diagnostics.Debug.WriteLine("Starting Game Server Application.");
        }

        protected override void TearDown()
        { }
    }
}
