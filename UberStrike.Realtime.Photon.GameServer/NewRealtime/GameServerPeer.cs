using System;
using Cmune.DataCenter.Common.Entities;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrike.Realtime.Server;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.GameServer
{
    public class GameServerPeer : BaseGameServerPeer
    {
        public GameServerPeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmuneId, ChannelType channel)
            : base(protocol, photonPeer, cmuneId, channel)
        {
        }

        // Game Server Operations

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

        protected override void OnJoinGameRoom(int roomId)
        {
            BaseRoom room;
            if (RoomManager.TryGetRoom(roomId, out room))
            {
                if (room.JoinRoom(this))
                {
                    Events.SendJoinedGameRoom(room.Id);
                }
            }
        }

        protected override void OnCreateGameRoom(GameModeType type)
        {
            BaseRoom room = null;
            switch (type)
            {
                case GameModeType.DeathMatch:
                    room = RoomManager.CreateRoom<DeathmatchGameRoom>();
                    break;
            }

            if (room != null)
            {
                if (room.JoinRoom(this))
                {
                    Events.SendJoinedGameRoom(room.Id);
                }
            }
        }

        protected override void OnLeaveGameRoom(int roomId)
        {
            Events.SendLeftGameRoom(roomId);
        }

        protected override void OnRegisterToGameLobby()
        {
            throw new NotImplementedException();
        }
    }
}