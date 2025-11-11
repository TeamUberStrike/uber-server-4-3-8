using System.Collections;
using System.Collections.Generic;

namespace UberStrike.Realtime.Server
{
    public class CmunePeerCollection : IEnumerable<ICmunePeer>
    {
        private Dictionary<int, ICmunePeer> peersByCmid;

        public CmunePeerCollection()
        {
            this.peersByCmid = new Dictionary<int, ICmunePeer>();
        }

        public int Count
        {
            get
            {
                return this.peersByCmid.Count;
            }
        }

        public ICmunePeer this[int cmid]
        {
            get
            {
                return this.peersByCmid[cmid];
            }
        }

        public bool TryGetByPeerId<T>(int cmid, out T peer) where T : class,  ICmunePeer
        {
            ICmunePeer p;
            if (peersByCmid.TryGetValue(cmid, out p))
            {
                peer = p as T;
                return peer != null;
            }
            else
            {
                peer = null;
                return false;
            }
        }

        public IEnumerable<int> PeerIds
        {
            get { return peersByCmid.Keys; }
        }

        public IEnumerable<ICmunePeer> GetExcludedList(int cmidToExclude)
        {
            List<ICmunePeer> list = new List<ICmunePeer>(this.peersByCmid.Count - 1);
            foreach (var v in this.peersByCmid)
            {
                if (v.Key != cmidToExclude)
                    list.Add(v.Value);
            }
            return list;
        }

        public bool Add(ICmunePeer peer)
        {
            if (!this.peersByCmid.ContainsKey(peer.Cmid))
            {
                this.peersByCmid.Add(peer.Cmid, peer);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(int cmid)
        {
            return this.peersByCmid.Remove(cmid);
        }

        public void Clear()
        {
            this.peersByCmid.Clear();
        }

        public bool Contains(int cmid)
        {
            return this.peersByCmid.ContainsKey(cmid);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerable<ICmunePeer> All
        {
            get { return this.peersByCmid.Values; }
        }

        public IEnumerator<ICmunePeer> GetEnumerator()
        {
            return this.peersByCmid.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.peersByCmid.GetEnumerator();
        }
    }
}
