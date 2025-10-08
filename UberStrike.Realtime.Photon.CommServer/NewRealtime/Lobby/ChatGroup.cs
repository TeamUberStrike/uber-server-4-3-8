using System.Collections.Generic;
using System.Linq;
using UberStrike.Realtime.Server;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Realtime.CommServer
{
    class ChatGroup
    {
        private Dictionary<int, ICmunePeer> _peersByCmid;
        private LobbyRoomEventFactory _events;

        public ChatGroup()
        {
            _peersByCmid = new Dictionary<int, ICmunePeer>(16);
            _events = new LobbyRoomEventFactory(null);
        }

        public void AddUser(ICmunePeer peer)
        {
            _peersByCmid[peer.Cmid] = peer;

            var ev = _events.SendUpdateIngameGroup(_peersByCmid.Keys.ToList());
            foreach (var p in _peersByCmid.Values)
            {
                p.PublishEvent(ev.Data, false);
            }
        }

        public void RemoveUser(ICmunePeer peer)
        {
            _peersByCmid.Remove(peer.Cmid);

            var ev = _events.SendUpdateIngameGroup(_peersByCmid.Keys.ToList());
            foreach (var p in _peersByCmid.Values)
            {
                p.PublishEvent(ev.Data, false);
            }

            //CommServerRoom.Instance.NewMessageToPeers(_peersByCmid.Values, true, NetworkClassID.CommCenter, CommRPC.UpdateIngameGroup, new SendParameters() { Unreliable = true }, (object)_peersByCmid.Keys);
        }

        public void SendMessageToGroup(int cmid, int actorId, string name, string msg, MemberAccessLevel accessLevel, byte context)
        {
            var ev = _events.SendInGameChatMessage(cmid, name, msg, accessLevel, context);
            foreach (var p in _peersByCmid.Values)
            {
                p.PublishEvent(ev.Data, false);
            }

            //try
            //{
            //    CommServerRoom.Instance.NewMessageToPeersExceptActor(actorId, _peersByCmid.Values, true, NetworkClassID.CommCenter, CommRPC.ChatMessageInGame, new SendParameters() { Unreliable = true }, cmid, actorId, name, msg, accessLevel, context);
            //}
            //catch (InvalidOperationException e)
            //{
            //    CmuneDebug.LogError("InvalidOperationException at SendMessageToGroup where group length '{0}': {1}", _peersByCmid.Count, e.Message);
            //}
        }

        public bool IsEmpty
        {
            get { return _peersByCmid.Count == 0; }
        }
    }
}
