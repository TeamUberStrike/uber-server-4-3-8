using Photon.SocketServer;
using Uberstrike.Realtime.Server.Peers;
using Uberstrike.Realtime.Server.Peers.Interfaces;

namespace Uberstrike.Realtime.Server.Base
{
    public abstract class RoomServiceBase
    {
        public int RoomId { get; private set; }
        public int PeerCount { get { return peerCollection.Count; } }

        protected CmunePeerCollection peerCollection;

        protected RoomServiceBase(int roomId)
        {
            RoomId = roomId;

            peerCollection = new CmunePeerCollection();
        }

        public void AddPeerToRoom(ICmunePeer peer)
        {
            peerCollection.Add(peer);

            OnPeerEntered(peer);
        }

        public void RemovePeerFromRoom(int peerId)
        {
            if (peerCollection.Remove(peerId))
            {
                OnPeerLeft(peerId);
            }
        }

        protected abstract void OnPeerEntered(ICmunePeer peer);

        protected abstract void OnPeerLeft(int peerId);

        public bool TryGetPeerWithId(int peerId, out ICmunePeer peer)
        {
            return peerCollection.TryGetByPeerId(peerId, out peer);
        }

        protected void PublishEventToPeer(int peerId, IEventData eventData)
        {
            peerCollection[peerId].PublishEvent(eventData);
        }

        protected void PublishEventToAllPeersExcept(int peerId, IEventData eventData)
        {
            foreach (var p in peerCollection)
            {
                if (p.Id != peerId)
                    p.PublishEvent(eventData);
            }
        }

        protected void PublishEventToAllPeers(IEventData eventData)
        {
            foreach (var p in peerCollection)
            {
                p.PublishEvent(eventData);
            }
        }
    }
}
