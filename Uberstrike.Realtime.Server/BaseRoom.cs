using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using ExitGames.Concurrency.Fibers;

namespace UberStrike.Realtime.Server
{
    public abstract class BaseRoom : ICompressionContext
    {
        protected CmunePeerCollection _peerCollection;
        protected PoolFiber _fiber;
        public Dictionary<byte, byte[]> _deltaCache;

        public int Id { get; private set; }

        public bool IsDisposed { get; private set; }

        public int PeerCount
        {
            get
            {
                lock (_peerCollection)
                {
                    return _peerCollection.Count;
                }
            }
        }

        public IEnumerable<ICmunePeer> Peers { get { return _peerCollection.All; } }

        protected BaseRoom(int roomId)
        {
            _peerCollection = new CmunePeerCollection();
            _fiber = new PoolFiber();
            _deltaCache = new Dictionary<byte, byte[]>();

            Id = roomId;

            _fiber.Start();
        }

        ~BaseRoom()
        {
            this.Dispose(false);
        }

        protected abstract void OnOperation(byte operationId, ICmunePeer peer, byte[] data);

        // Public

        public bool JoinRoom(ICmunePeer peer)
        {
            try
            {
                _fiber.Enqueue(() =>
                    {
                        if (_peerCollection.Add(peer))
                        {
                            OnPeerEntered(peer);
                        }
                    });
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public void LeaveRoom(int cmid)
        {
            try
            {
                _fiber.Enqueue(() =>
                    {
                        if (_peerCollection.Contains(cmid))
                        {
                            OnPeerLeft(cmid);
                            _peerCollection.Remove(cmid);
                        }
                    });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void EnqueueOperation(byte opCode, ICmunePeer peer, byte[] bytes)
        {
            _fiber.Enqueue(() =>
            {
                try
                {
                    OnOperation(opCode, peer, bytes);
                }
                catch (SerializationException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new Exception("EnqueueOperation failed with " + e.GetType(), e);
                }
            });
        }

        public bool TryGetPeer(int peerId, out ICmunePeer peer)
        {
            return _peerCollection.TryGetByPeerId(peerId, out peer) && peer != null;
        }

        // Protected

        protected bool IsEmpty
        {
            get { return this.PeerCount == 0; }
        }

        protected abstract void OnPeerEntered(ICmunePeer peer);

        protected abstract void OnPeerLeft(int peerId);


        #region IDispoable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            this.IsDisposed = true;

            if (dispose)
            {
                _fiber.Dispose();
            }
        }

        #endregion

        public byte[] ReadCache(byte id)
        {
            byte[] baseData;
            if (!_deltaCache.TryGetValue(id, out baseData))
            {
                baseData = new byte[0];
            }
            return baseData;
        }

        public void WriteCache(byte id, byte[] data)
        {
            _deltaCache[id] = data;
        }
    }
}