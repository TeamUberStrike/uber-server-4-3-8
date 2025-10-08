using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrike.Core.Models;

namespace UberStrike.Realtime.Server
{
    public abstract class BasePeer : PeerBase, ICmunePeer, ICompressionContext
    {
        private BaseRoom _room;
        private Dictionary<byte, byte[]> _cache;

        public int Cmid { get; private set; }
        public ActorInfo Info { get; protected set; }

        protected BasePeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmid, ChannelType channel)
            : base(protocol, photonPeer)
        {
            Cmid = cmid;

            _cache = new Dictionary<byte, byte[]>();
        }

        protected abstract void OnOperation(byte id, byte[] data);

        public void PublishEvent(IEventData eventData, bool reliable = true)
        {
            this.RequestFiber.Enqueue(
                () =>
                {
                    try
                    {
                        SendEvent(eventData, new SendParameters() { Unreliable = !reliable });
                    }
                    catch (Exception e)
                    {
                        //CmuneDebug.LogError(e);
                    }
                });
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            try
            {
                object data;
                if (_room != null && operationRequest.Parameters.TryGetValue(0, out data))
                {
                    //if this is a room operation we redirect it to the room thread
                    _room.EnqueueOperation((byte)operationRequest.OperationCode, this, (byte[])operationRequest.Parameters[0]);
                }
                //server requests have an entry on key '1'
                else if (operationRequest.Parameters.TryGetValue(1, out data))
                {
                    //execute server request directly in the peer thread
                    OnOperation((byte)operationRequest.OperationCode, (byte[])data);
                }
                else
                {
                    SendOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 1, DebugMessage = "Room does not exist!" }, sendParameters);
                }
            }
            catch (Exception e)
            {
                SendOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 1, DebugMessage = e.Message }, sendParameters);
            }
        }

        protected override void OnDisconnect()
        {
            if (_room != null) _room.LeaveRoom(this.Cmid);
            _room = null;
        }

        public byte[] ReadCache(byte id)
        {
            byte[] baseData;
            if (!_cache.TryGetValue(id, out baseData))
            {
                baseData = new byte[0];
            }
            return baseData;
        }

        public void WriteCache(byte id, byte[] data)
        {
            _cache[id] = data;
        }
    }
}