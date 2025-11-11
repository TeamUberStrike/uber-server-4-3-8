using Cmune.DataCenter.Common.Entities;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrike.Core.Models;

namespace UberStrike.Realtime.CommServer
{
    public class CommServerPeer : BaseCommPeer
    {
        public CommServerPeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmid, ChannelType channel)
            : base(protocol, photonPeer, cmid, channel)
        {
            Info = new CommActorInfo()
            {
                Cmid = cmid,
                Channel = channel,
            };
        }

        protected override void OnGetServerLoad()
        {
            //Events.SendGameServerData(new RealtimeServerInfoModel()
            //{
            //    DebugTimeStamp = DateTime.Now,
            //    CurrentGameCount = 0,
            //    CurrentPlayerCount = 0,
            //    MaxGameCount = 100,
            //    MaxPlayerCount = 100
            //});
        }

        protected override void OnEnterLobby()
        {
            if (LobbyRoom.Instance.JoinRoom(this))
            {
                Events.SendLobbyEntered();
            }
        }

        protected override void OnLeaveLobby()
        {
            //do something
            LobbyRoom.Instance.LeaveRoom(Cmid);
        }
    }
}