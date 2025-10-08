
using System;
using System.Collections.Generic;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    public class ActorCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public ActorCollection()
        {
            _actorsBySessionID = new Dictionary<int, int>();
            _peersByActorID = new Dictionary<int, CmunePeer>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        public int Add(CmunePeer peer)
        {
            int actorId = GetNextActorID();
            try
            {
                _peersByActorID.Add(actorId, peer);
                _actorsBySessionID.Add(peer.SessionId, actorId);
            }
            catch (Exception e)
            {
                if (peer != null)
                {
                    CmuneDebug.LogError("Exception Add(CmunePeer): {0} {1}/{2}", e.Message, peer.SessionId, actorId);
                }
                else
                {
                    CmuneDebug.LogError("Exception Add(CmunePeer): {0}, peer null = {1}", e.Message, (peer != null));
                }
            }

            return actorId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        public int Remove(CmunePeer peer)
        {
            int actorId = -1;

            try
            {
                if (peer != null && _actorsBySessionID.TryGetValue(peer.SessionId, out actorId))
                {
                    bool peerRemoved = _peersByActorID.Remove(actorId);
                    bool actorRemoved = _actorsBySessionID.Remove(peer.SessionId);

                    if (!(peerRemoved && actorRemoved))
                    {
                        CmuneDebug.LogError("Remove Peer with ActorID {0} failed! pba/abs {1}/{2}", actorId, _peersByActorID.Count, _actorsBySessionID.Count);
                    }
                }
                else
                {
                    //CmuneDebug.LogErrorFormat("Failed removing {0} with ID {1} because actor not found! pba/abs {2}/{3}", p.PeerType, p.SessionId, _peersByActorID.Count, _actorsBySessionID.Count);
                }
            }
            catch (Exception e)
            {
                if (peer != null)
                {
                    CmuneDebug.LogError("Exception in Remove(CmunePeer): {0} {1}/{2}", e.Message, peer.SessionId, actorId);
                }
                else
                {
                    CmuneDebug.LogError("Exception in Remove(CmunePeer): {0}, peer null = {1}", e.Message, (peer != null));
                }
            }

            return actorId;
        }

        public bool TryGetActorIDByPeer(CmunePeer p, out int id)
        {
            return _actorsBySessionID.TryGetValue(p.SessionId, out id);
        }

        public bool TryGetPeerByCmuneID(int cmid, out CmunePeer peer)
        {
            peer = null;
            foreach (var p in _peersByActorID.Values)
            {
                if (p.Cmid == cmid)
                {
                    peer = p;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetPeerByActorID(int actorID, out CmunePeer peer)
        {
            return _peersByActorID.TryGetValue(actorID, out peer);
        }

        public ICollection<CmunePeer> GetAllPeers()
        {
            return new List<CmunePeer>(_peersByActorID.Values);
        }

        private int GetNextActorID()
        {
            return ++_actorID;
        }

        #region PROPERTIES
        public int Count
        {
            get
            {
                return _actorsBySessionID.Count;
            }
        }
        #endregion

        #region FIELDS
        protected Dictionary<int, int> _actorsBySessionID;
        protected Dictionary<int, CmunePeer> _peersByActorID;
        private int _actorID = 0;
        #endregion
    }
}
