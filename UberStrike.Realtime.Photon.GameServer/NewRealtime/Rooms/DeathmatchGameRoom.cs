using System;
using UberStrike.Realtime.Server;

namespace UberStrike.Realtime.GameServer
{
    public class DeathmatchGameRoom : BaseDeathmatchRoom
    {
        public DeathmatchGameRoom(int roomId)
            : base(roomId)
        { }

        protected override void OnKillPlayer(ICmunePeer peer, int killedPeerId)
        {
            Events.SendPlayerKilled(1).ToAll();

            throw new NotImplementedException();
        }

        protected override void OnShootPlayer(ICmunePeer peer, int shotPeerId)
        {
            throw new NotImplementedException();
        }

        protected override void OnPeerEntered(ICmunePeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnPeerLeft(int peerId)
        {
            throw new NotImplementedException();
        }
    }
}