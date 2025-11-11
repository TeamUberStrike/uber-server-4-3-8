using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cmune.Core.Types;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Security;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class CmuneRoom : Room, IComparable<CmuneRoom>
    {
        public CmuneRoom(int roomNumber)
            : this(new RoomMetaData(roomNumber, "NoName", ServerSettings.ConnectionString), true, true)
        { }

        public CmuneRoom(RoomMetaData data)
            : this(data, true, true)
        { }

        public CmuneRoom(RoomMetaData data, bool autoDestroy, bool sendLobbyEvents)
        {
            RoomData = data;

            _autoDestroyWhenEmpty = autoDestroy;
            _sendLobbyEvents = sendLobbyEvents;

            _myType = this.GetType();
            _rmi = new RemoteMethodInterface();
            Actors = new ActorCollection();
            _myMethods = new Dictionary<int, MethodInfo>(20);
            _syncCenter = new ServerSynchronizationCenter(_rmi, this);

            _bannedCmids = new Dictionary<int, DateTime>(20);
            _lockBannedCmids = new ReaderWriterLockSlim();

            InitMyInternalMethods();

            SubscribeToRoomMessages(ProcessMessages);

            _updateBannedCmids = ExecutionFiber.ScheduleOnInterval(UpdateBannedCmids, 10 * 60 * 1000, 10 * 60 * 1000);
        }

        /// <summary>
        /// Remove banned Cmids from list if ban time passed.
        /// It's not neccessary for the ban check but only to remove deprecated data
        /// </summary>
        private void UpdateBannedCmids()
        {
            _lockBannedCmids.EnterWriteLock();
            try
            {
                List<int> cmids = new List<int>(_bannedCmids.Keys);
                foreach (int i in cmids)
                {
                    DateTime time;
                    if (_bannedCmids.TryGetValue(i, out time) && time.CompareTo(DateTime.Now) < 0)
                    {
                        _bannedCmids.Remove(i);
                    }
                }
            }
            finally
            {
                _lockBannedCmids.ExitWriteLock();
            }
        }

        public bool IsBannedFromRoom(int cmid)
        {
            _lockBannedCmids.EnterReadLock();
            try
            {
                DateTime time;
                if (_bannedCmids.TryGetValue(cmid, out time) && time.CompareTo(DateTime.Now) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                _lockBannedCmids.ExitReadLock();
            }
        }

        public void BanPlayerFromRoom(int cmid, int durationInMinutes)
        {
            PublishRoomMessage(new RoomMessage(RoomMessageType.KickPlayer, cmid, durationInMinutes));
        }

        [RoomMessage(RoomMessageType.KickPlayer)]
        protected void OnKickPlayer(int cmid, int durationInMinutes)
        {
            //perform the longterm ban
            if (durationInMinutes > 0)
            {
                _lockBannedCmids.EnterWriteLock();
                try
                {
                    _bannedCmids[cmid] = DateTime.Now.AddMinutes(durationInMinutes);
                }
                finally
                {
                    _lockBannedCmids.ExitWriteLock();
                }
            }
        }

        public void SubscribeToRoomMessages(Action<IMessage> reciever)
        {
            _internalMessageChannel += reciever;
        }

        public void UnsubscribeToRoomMessages(Action<IMessage> reciever)
        {
            if (_internalMessageChannel != null)
                _internalMessageChannel -= reciever;
        }

        public void PublishRoomMessage(RoomMessage message)
        {
            if (_isDisposed) return;

            ExecutionFiber.Enqueue(() => ProcessInternalMessages(message));
        }

        public IDisposable ScheduleRoomMessage(RoomMessage message, long time)
        {
            if (_isDisposed) return null;

            return ExecutionFiber.Schedule(() => ProcessInternalMessages(message), time);
        }

        /// <summary>
        /// Every game instance (or process) has a queue of incoming operations to execute. Per game
        /// ExecuteOperation() is calles in order, serial and in one thread.
        /// </summary>
        /// <param name="operation">operation to execute.</param>
        protected override void ExecuteOperation(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case CmuneOperationCodes.MessageToServer:
                    {
                        HandleMessageToServer(peer, operationRequest);
                        break;
                    }
                case CmuneOperationCodes.MessageToAll:
                    {
                        HandleMessageToAll(peer, operationRequest, sendParameters);
                        break;
                    }
                case CmuneOperationCodes.MessageToPlayer:
                    {
                        HandleMessageToPlayer(peer, operationRequest, sendParameters);
                        break;
                    }
                case CmuneOperationCodes.MessageToOthers:
                    {
                        HandleMessageToOthers(peer, operationRequest, sendParameters);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameJoin:
                    {
                        HandleJoin(peer, operationRequest, sendParameters);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameLeave:
                    {
                        HandleLeave(peer, operationRequest, sendParameters);
                        break;
                    }

                default:
                    {
                        CmuneDebug.LogError("Recieved unrecognized op code: {0}", operationRequest.OperationCode);
                        break;
                    }
            }
        }

        protected void HandleMessageToServer(CmunePeer peer, OperationRequest operationRequest)
        {
            try
            {
                MessageToServer msg = new MessageToServer(peer.Protocol, operationRequest);
                object[] param;
                if (msg.TryGetParameters(peer, out param))
                {
                    _rmi.RecieveMessage(msg.NetworkID, msg.LocalAddress, param);
                }
            }
            catch (Exception e)
            {
                CmuneDebug.Exception("HandleMessageToServer-'{0}' because: {1}\n{2}", e.GetType(), e.Message, e.StackTrace);
                CmuneOperations.DebugRequest(peer, operationRequest);
            }
        }

        protected void HandleMessageToPlayer(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            MessageToPlayer msg = new MessageToPlayer(peer.Protocol, operationRequest);

            CmunePeer reciever;
            if (Actors.TryGetPeerByActorID(msg.ActorId, out reciever))
            {
                this.PublishEvent(new EventData(CmuneEventCodes.Standard, operationRequest.Parameters), reciever, sendParameters);
            }
        }

        protected void HandleMessageToAll(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.PublishEvent(new EventData(CmuneEventCodes.Standard, operationRequest.Parameters), Actors.GetAllPeers(), sendParameters);
        }

        protected void HandleMessageToOthers(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.PublishEvent(new EventData(CmuneEventCodes.Standard, operationRequest.Parameters), Actors.GetAllPeers(), peer, sendParameters);
        }

        protected virtual void HandleJoin(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            JoinOperation joinOperation = new JoinOperation(peer.Protocol, operationRequest);

            int actorId = Actors.Add(peer);

            //if (CmuneDebug.IsDebugEnabled)
            //    CmuneDebug.LogFormat("HandleJoin of peer {0} with ActorID {1} in room {2}", peer.SessionId, actorId, this.ID);

            //update room data
            RoomData.ConnectedPlayers = Actors.Count;

            // set comand return values and publish the response
            peer.PublishOperationResponse(joinOperation.CreateOperationResponse(actorId, Actors.Count, false, RoomData.RoomID), sendParameters);

            PublishRoomMessage(new RoomMessage(RoomMessageType.AddedPeerToGame, actorId));

            UpdateRoomInLobby();
        }

        public void UpdateRoomInLobby()
        {
            if (_sendLobbyEvents)
                LobbyServerRoom.Instance.PublishRoomMessage(new RoomMessage(RoomMessageType.GameUpdated, RoomData));
        }

        protected int HandleLeave(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            int actorID = RemovePeerFromGame(peer);

            peer.PublishOperationResponse(new OperationResponse(operationRequest.OperationCode), sendParameters);

            return actorID;
        }

        public void LeaveGame(CmunePeer peer)
        {
            if (_isDisposed) return;

            ExecutionFiber.Enqueue(() => RemovePeerFromGame(peer));
        }

        protected virtual int RemovePeerFromGame(CmunePeer peer)
        {
            int actorID = Actors.Remove(peer);

            RoomData.ConnectedPlayers = Actors.Count;

            if (actorID >= 0)
            {
                PublishRoomMessage(new RoomMessage(RoomMessageType.RemovePeerFromGame, actorID));
            }

            //if (CmuneDebug.IsDebugEnabled)
            //    CmuneDebug.LogFormat("RemovePeer {0} FromGame {1} with ActorId {2} from {3} actors, sendEvents {4}, autoDestroy {5}", peer.SessionId, this.ID, actorID, _actors.Count, _sendLobbyEvents, _autoDestroyWhenEmpty);

            //Update Roomdata InLobby if the room is not empty or is a permanet room
            if (Actors.Count > 0 || !_autoDestroyWhenEmpty)
            {
                UpdateRoomInLobby();
            }
            else if (_autoDestroyWhenEmpty)
            {
                Dispose();
            }

            return actorID;
        }

        public void NewMessageToPeer(CmunePeer peer, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            EventData data = CreateEvent(true, classID, methodID, sendParameters, args);

            this.PublishEvent(data, peer, sendParameters);
        }

        public void NewMessageToPeers(IEnumerable<CmunePeer> peers, bool reliable, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            EventData data = CreateEvent(reliable, classID, methodID, sendParameters, args);

            this.PublishEvent(data, peers, sendParameters);
        }

        public void NewMessageToPeersExceptActor(int actorID, IEnumerable<CmunePeer> peers, bool reliable, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            EventData data = CreateEvent(reliable, classID, methodID, sendParameters, args);

            CmunePeer peer;
            if (Actors.TryGetPeerByActorID(actorID, out peer))
            {
                this.PublishEvent(data, peers, peer, sendParameters);
            }
            else
            {
                this.PublishEvent(data, peers, sendParameters);
            }
        }

        public void NewMessageToAll(bool reliable, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            EventData data = CreateEvent(reliable, classID, methodID, sendParameters, args);

            this.PublishEvent(data, Actors.GetAllPeers(), sendParameters);
        }

        public void NewMessageToActor(int actorID, bool reliable, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            CmunePeer peer;
            if (Actors.TryGetPeerByActorID(actorID, out peer))
            {
                EventData data = CreateEvent(reliable, classID, methodID, sendParameters, args);

                this.PublishEvent(data, peer, sendParameters);
            }
        }

        public void NewMessageToAllExceptActor(int actorID, bool reliable, short classID, byte methodID, SendParameters sendParameters, params object[] args)
        {
            EventData data = CreateEvent(reliable, classID, methodID, sendParameters, args);

            CmunePeer peer;
            if (Actors.TryGetPeerByActorID(actorID, out peer))
            {
                this.PublishEvent(data, Actors.GetAllPeers(), peer, sendParameters);
            }
            else
            {
                this.PublishEvent(data, Actors.GetAllPeers(), sendParameters);
            }
        }

        private static EventData CreateEvent(bool reliable, short classId, byte methodId, SendParameters sendParameters, params object[] args)
        {
            Dictionary<byte, object> data = new Dictionary<byte, object>
            {
                {ParameterKeys.InstanceId, classId},
                {ParameterKeys.MethodId, methodId},
                {ParameterKeys.Bytes, RealtimeSerialization.ToBytes(args).ToArray()}
            };

            return new EventData(CmuneEventCodes.Standard, data);
        }

        public virtual void UpdateSpaceAssetID(int assetID)
        {
            _roomIsValid = true;

            //TODO: find a solution for updating the mapID of a game
            //RoomData.MapID = assetID;
        }

        //public string GetPlayerIP(int playerID)
        //{
        //    string ip = "[0.0.0.0]";

        //    CmunePeer p;
        //    if (_actors.TryGetPeerByActorID(playerID, out p) && p!=null)
        //    {
        //            ip = p.PhotonPeer.NativePeer.GetRemoteIP();
        //    }

        //    return ip;
        //}

        public bool TryGetPeer(int actorID, out CmunePeer peer)
        {
            return Actors.TryGetPeerByActorID(actorID, out peer);
        }

        public bool TryGetActor(CmunePeer peer, out int actorID)
        {
            return Actors.TryGetActorIDByPeer(peer, out actorID);
        }

        public void KickPeer(CmunePeer peer)
        {
            int id;
            if (Actors.TryGetActorIDByPeer(peer, out id))
            {
                NewMessageToActor(id, true, NetworkClassID.ClientSyncCenter, ClientSyncCenterRPC.KickPlayer, new SendParameters(), "You were kicked out!");
            }
            else
            {
                CmuneDebug.LogError("Couldn't kick player because actorID not found!");
            }
        }

        public virtual void CloseRoom()
        {
            IsRoomOpen = false;
        }

        /// <summary>
        /// Remove the game from game cache when game is disposed.
        /// </summary>
        /// <param name="dispose"></param>
        protected override void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                RoomCache.Instance.Remove(this.RoomData.RoomNumber);

                if (_sendLobbyEvents)
                    LobbyServerRoom.Instance.PublishRoomMessage(new RoomMessage(RoomMessageType.GameRemoved, RoomData.RoomID));

                if (_updateBannedCmids != null) _updateBannedCmids.Dispose();

                _internalMessageChannel = null;

                _lockBannedCmids.Dispose();
                _syncCenter.Dispose();
            }

            base.Dispose(dispose);
        }

        #region INTERNAL MESSAGING

        private void InitMyInternalMethods()
        {
            List<MemberInfoMethod<RoomMessageAttribute>> info = AttributeFinder.GetMethods<RoomMessageAttribute>(_myType);
            foreach (MemberInfoMethod<RoomMessageAttribute> p in info)
            {
                _myMethods[p.Attribute.ID] = p.Method;
            }
        }

        private void ProcessInternalMessages(IMessage message)
        {
            if (_internalMessageChannel != null)
                _internalMessageChannel(message);
        }

        private void ProcessMessages(IMessage message)
        {
            CallInternalMethod(message.MessageID, message.Arguments);
        }

        private void CallInternalMethod(int internalID, params object[] args)
        {
            //bool isCalled = false;
            MethodInfo info;

            if (_myMethods.TryGetValue(internalID, out info))
            {
                //isCalled = true;

                try
                {
                    //CmuneDebug.LogFormat("{0}:ProcessMessages with ID {1} executed!", this.GetType().Name, internalID);
                    _myType.InvokeMember(info.Name, _flags, null, this, args);
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("Exception when calling internal function {0}:{1}() by reflection: {2}", _myType.Name, info.Name, e.Message);

                    if (args != null)
                        CmuneDebug.LogError("Call with {0} Arguments: {1}", args.Length, CmunePrint.Types(args));
                    else
                        CmuneDebug.LogError("Call with NULL Argument");
                }
            }
            //else
            //{
            //    CmuneDebug.LogErrorFormat("No internal method known with ID {0} in {1}", internalID, _myType.Name);
            //}

            //return isCalled;
        }

        #endregion

        #region Fields

        protected RemoteMethodInterface _rmi;
        protected ServerSynchronizationCenter _syncCenter;
        protected DateTime _creationDate = DateTime.Now;
        protected bool _autoDestroyWhenEmpty = true;
        protected bool _isRoomEmpty = true;
        protected bool _sendLobbyEvents = true;
        protected bool _roomIsValid = false;
        protected bool _roomIsOpen = true;

        public ActorCollection Actors { get; private set; }
        private Action<IMessage> _internalMessageChannel;
        private Dictionary<int, DateTime> _bannedCmids;

        protected ReaderWriterLockSlim _lockBannedCmids;
        private IDisposable _updateBannedCmids;

        protected static readonly BindingFlags _flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private Dictionary<int, MethodInfo> _myMethods;
        private Type _myType;

        #endregion

        #region COMPARISON

        public int CompareTo(CmuneRoom other)
        {
            if (!ReferenceEquals(other, null))
                return this.ID.CompareTo(other.ID);
            else
                return 0;
        }

        public static bool operator ==(CmuneRoom a, CmuneRoom b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.ID == b.ID;
        }

        public static bool operator !=(CmuneRoom a, CmuneRoom b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == obj as CmuneRoom;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        #endregion

        #region PROPERTIES

        public RoomMetaData RoomData { get; protected set; }

        public new string Name { get { return RoomData.RoomName; } }

        public string ID { get { return RoomData.RoomID.ID; } }

        public int Number { get { return RoomData.RoomNumber; } }

        public TimeSpan LifeTime
        {
            get { return (DateTime.Now - _creationDate); }
        }

        public bool IsRoomValid
        {
            get { return _roomIsValid && _roomIsOpen && Actors.Count > 0; }
            protected set { _roomIsValid = value; }
        }

        public bool IsRoomOpen
        {
            get { return _roomIsOpen; }
            protected set { _roomIsOpen = value; }
        }

        public virtual bool IsRoomFull
        {
            get
            {
                return RoomData.IsFull;
            }
        }

        #endregion

        public override string ToString()
        {
            return RoomData != null ? RoomData.RoomID.ToString() : "Unkown";
        }

        protected EventData CreateEventData(byte eventCode, Hashtable data)
        {
            return new EventData(eventCode, new OperationEvent() { Data = data });
        }

        private class OperationEvent
        {
            /// <summary>
            /// Gets or sets the event data.
            /// </summary>
            /// <value>The event data.</value>
            [DataMember(Code = (byte)ParameterKeys.Data)]
            public Hashtable Data { get; set; }
        }
    }
}