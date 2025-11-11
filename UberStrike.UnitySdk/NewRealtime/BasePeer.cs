using System;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using ExitGames.Client.Photon;
using UberStrike.Core.Serialization;
using UberStrike.Core.ViewModel;
using UnityEngine;

namespace UberStrike.Realtime.Client
{
    public abstract class BasePeer
    {
        private float nextUpdateTime;
        private PhotonPeerListener listener;

        public PhotonPeer Peer { get; private set; }
        public float SyncFrequency { get; set; }

        public bool IsConnected
        {
            get { return Peer.PeerState == ExitGames.Client.Photon.PeerStateValue.Connected; }
        }

        protected BasePeer(int syncFrequency)
        {
            this.listener = new PhotonPeerListener();
            this.Peer = new PhotonPeer(listener);
            this.SyncFrequency = syncFrequency / 1000f;
        }

        public void Connect(string endpointAddress, int cmuneId, ChannelType channel)
        {
            var connection = new ServerConnectionView()
            {
                ApiVersion = 0,
                Cmid = cmuneId,
                Channel = channel,
            };

            using (var stream = new MemoryStream())
            {
                ServerConnectionViewProxy.Serialize(stream, connection);
                bool success = Peer.Connect(endpointAddress, Convert.ToBase64String(stream.ToArray()));
                Debug.Log("connect to " + endpointAddress + ": " + success);
            }
        }

        public void Disconnect()
        {
            Peer.Disconnect();
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup > nextUpdateTime)
            {
                nextUpdateTime = Time.realtimeSinceStartup + SyncFrequency;
                if (Peer.PeerState != PeerStateValue.Disconnected)
                {
                    Peer.Service();
                }
            }
        }

        public void AddRoomLogic(IRoomLogic roomLogic)
        {
            AddRoomLogic(roomLogic, roomLogic.Operations);
        }

        protected void AddRoomLogic(IEventDispatcher evDispatcher, IOperationSender opSender)
        {
            opSender.SendOperation += Peer.OpCustom;
            listener.EventDispatcher += evDispatcher.OnEvent;
        }

        public void RemoveRoomLogic(IRoomLogic roomLogic)
        {
            roomLogic.Operations.SendOperation -= Peer.OpCustom;
            listener.EventDispatcher -= roomLogic.OnEvent;
        }

        public override string ToString()
        {
            return Peer.PeerState.ToString();
        }
    }
}