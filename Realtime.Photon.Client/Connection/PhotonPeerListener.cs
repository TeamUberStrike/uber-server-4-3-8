using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Security;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using ExitGames.Client.Photon;
using UnityEngine;
using Cmune.Realtime.Photon.Client.Network.Utils;

namespace Cmune.Realtime.Photon.Client
{
    /// <summary>
    /// In this class we handle ALL SERVER BASED Messaging and redirect it to the CmuneNetworkInterface
    /// </summary>
    public class PhotonPeerListener : IPhotonPeerListener//, IDisposable
    {
        #region Properties

        public string Server
        {
            get { return _server; }
        }
        public int Latency
        {
            get { return _peer != null ? Mathf.RoundToInt(_peer.RoundTripTime * 0.5f) : 0; }
        }
        public bool IsConnecting
        {
            get { return ConnectionState == NetworkState.STATE_CONNECTING; }
        }
        public bool IsConnectedToServer
        {
            get { return PeerState == PeerStateValue.Connected; }
        }
        public bool IsDisconnecting
        {
            get { return ConnectionState == NetworkState.STATE_DISCONNECTING; }
        }
        public bool IsJoining
        {
            get { return ConnectionState == NetworkState.STATE_JOINING; }
        }
        public bool IsLeaving
        {
            get { return ConnectionState == NetworkState.STATE_LEAVING; }
        }
        public bool HasJoinedRoom
        {
            get { return _isInRoom; }
            private set { _isInRoom = value; }
        }
        public NetworkState ConnectionState
        {
            get { return _state; }
            private set { _state = value; }
        }
        public PeerStateValue PeerState
        {
            get { return _peer != null ? _peer.PeerState : PeerStateValue.Disconnected; }
        }
        public string PeerInfo
        {
            get { return _peer != null ? _peer.RoundTripTime + "/" + _peer.RoundTripTimeVariance + ", <" + _peer.BytesIn + ", >" + _peer.BytesOut : "<Peer Null>"; }
        }
        public string PingInfo
        {
            get { return _peer != null ? string.Format("{0} ms/{1} ms", _peer.RoundTripTime, _peer.RoundTripTimeVariance) : string.Empty; }
        }
        public string DataIOInfo
        {
            get { return _peer != null ? string.Format("{0} kb/{1} kb", Mathf.RoundToInt(ConvertBytes.ToKiloBytes(_peer.BytesIn)), Mathf.RoundToInt(ConvertBytes.ToKiloBytes(_peer.BytesOut))) : string.Empty; }
        }

        public float TargetSendPerSecond
        {
            get { return _sendPerSecond; }
            set { _sendPerSecond = value; }
        }
        public float TargetGetPerSecond
        {
            get { return _getPerSecond; }
            set { _getPerSecond = value; }
        }
        public int SessionID
        {
            get { return _sessionID; }
            private set { _sessionID = value; }
        }
        #endregion

        #region Fields

        private bool _doDisconnect = false;
        public bool DoDisconnect
        {
            get { return _doDisconnect; }
            set
            {
                _doDisconnect = value;

                //if (_doDisconnect)
                //    CmuneDebug.Log("Register for PhotonPeer.Disconnect()");
                //else
                //    CmuneDebug.Log("Call of PhotonPeer.Disconnect()");
            }
        }
        private string _server;

        private ConnectionEventType _lastEventEnqueued = ConnectionEventType.None;

        public delegate void RecieveMessages(short networkID, byte functionID, object[] args);

        private Queue<ConnectionEvent> _connectionEvents = new Queue<ConnectionEvent>();
        private Action<ConnectionEvent> _castConnectionEvents;
        private RecieveMessages _recieveMessages;
        private PhotonPeer _peer;

        private SecureMemory<int> _actorId;
        public CmuneRoomID CurrentRoom { get; private set; }
        public int ActorId { get { return _actorId.ReadData(false); } }
        public int ActorIdSecure { get { return _actorId.ReadData(true); } }
        public ushort Ping { get; private set; }
        public long IncomingBytes { get { return _peer.BytesIn; } }
        public long OutgoingBytes { get { return _peer.BytesOut; } }

        private NetworkState _state = NetworkState.STATE_DISCONNECTED;

        private int _sessionID = -1;
        private float _sendPerSecond = 40;
        private float _getPerSecond = 40;
        private bool _isInRoom = false;

        private float _nextPingUpdate = 0;
        #endregion

        public PhotonPeerListener(bool monitorMemory = false)
        {
            _actorId = new SecureMemory<int>(0, monitorMemory);
            _peer = new PhotonPeer(this, false);

            _peer.DebugOut = DebugLevel.ERROR;
            _peer.TimePingInterval = 1000;
            _peer.SentCountAllowance = 5;
            _peer.CommandBufferSize = 500;

            ConnectionState = NetworkState.STATE_DISCONNECTED;
        }

        public int ServerTimeTicks
        {
            get { return _peer.ServerTimeInMilliSeconds & Int32.MaxValue; }
        }

        public void FetchServerTime()
        {
            _peer.FetchServerTimestamp();
        }

        public void EnableNetworkSimulation(bool enabled, int incomingLag, int outgoingLag)
        {
            Debug.LogError("PhotonPeerListener: Setting up Network Simulation");
            _peer.NetworkSimulationSettings.IsSimulationEnabled = enabled;
            _peer.NetworkSimulationSettings.IncomingJitter = 0;
            _peer.NetworkSimulationSettings.IncomingLag = incomingLag;
            _peer.NetworkSimulationSettings.IncomingLossPercentage = 0;
            _peer.NetworkSimulationSettings.OutgoingJitter = 0;
            _peer.NetworkSimulationSettings.OutgoingLag = outgoingLag;
            _peer.NetworkSimulationSettings.OutgoingLossPercentage = 0;
        }

        public void SubscribeToEvents(Action<ConnectionEvent> process)
        {
            _castConnectionEvents += process;
        }

        public void UnsubscribeToEvents(Action<ConnectionEvent> process)
        {
            _castConnectionEvents -= process;
        }

        public void SetMessageCallback(RecieveMessages callback)
        {
            _recieveMessages = callback;
        }

        public void UpdateServerTime()
        {
            if (_peer != null)
                _peer.FetchServerTimestamp();
        }

        public void Update()
        {
            if (PeerState > 0)
            {
                while (_peer.DispatchIncomingCommands()) ;

                SendOutgoingCommands();

                if (Time.time > _nextPingUpdate)
                {
                    _nextPingUpdate = Time.time + 5;
                    Ping = (ushort)(_peer.RoundTripTime * 0.5f);
                }
            }

            while (_connectionEvents.Count > 0)
                _castConnectionEvents(_connectionEvents.Dequeue());
        }

        private void SendOutgoingCommands()
        {
            if (DoDisconnect)
            {
                DoDisconnect = false;

                //if (CmuneDebug.IsDebugEnabled)
                //    CmuneDebug.Log("Call of PhotonPeer.Disconnect()");

                _peer.Disconnect();
                ConnectionState = NetworkState.STATE_DISCONNECTING;
            }
            else if (ConnectionState == NetworkState.STATE_DISCONNECTED)
            {
                OnDisconnect();
            }

            _peer.SendOutgoingCommands();
        }

        public void LeaveCurrentRoom()
        {
            //CmuneDebug.LogWarning("Leave Room " + Info.RoomID);

            if (!CurrentRoom.IsEmpty)
            {
                OperationRequest request = new OperationRequest()
                {
                    Parameters = OperationFactory.Create(CmuneOperationCodes.PhotonGameLeave, CurrentRoom.GetBytes()),
                    OperationCode = CmuneOperationCodes.PhotonGameLeave
                };

                SendOperationToServer(request, true);

                ConnectionState = NetworkState.STATE_LEAVING;
            }
        }

        private void OnRoomLeft()
        {
            OnRoomLeft(false);
        }

        private void OnRoomLeft(bool reject)
        {
            //CmuneDebug.LogErrorFormat("handleRoomLeft: {0} {1} {2}", reject, DebugType, Channel);

            CurrentRoom = CmuneRoomID.Empty;
            _actorId.WriteData(0);

            if (ConnectionState != NetworkState.STATE_DISCONNECTED)
                ConnectionState = NetworkState.STATE_LEFT;

            HasJoinedRoom = false;
        }

        public void Disconnect(bool force = false)
        {
            if (IsConnectedToServer || force)
            {
                //CmuneDebug.LogWarning("Manual Call of Disconnect, force=" + force);
                DoDisconnect = true;
            }
        }

        private void OnDisconnect()
        {
            if (HasJoinedRoom)
            {
                OnRoomLeft();
            }

            //IsConnectedToServer = false;
        }

        public bool Connect(string server, int cmid)
        {
            //Debug.Log("PhotonPeerListener: Connect(" + server + ")" + " " + app);

            _server = server;

            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("({0}) - Connect TO {1}", SessionID, server);

            ConnectionState = NetworkState.STATE_CONNECTING;

            try
            {
                if (!_peer.Connect(server, cmid.ToString()))
                {
                    ConnectionState = NetworkState.STATE_DISCONNECTED;
                }

                return ConnectionState == NetworkState.STATE_CONNECTING;
            }
            catch (Exception e)
            {
                CmuneDebug.LogError("({0}) - Connect failed with: {1}", SessionID, e.Message);

                ConnectionState = NetworkState.STATE_DISCONNECTED;

                _connectionEvents.Clear();
                _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.Disconnected, CurrentRoom, 0));
                return false;
            }
        }

        public void JoinRoom(RoomMetaData room, int cmid, int accessLevel)
        {
            if (IsConnectedToServer)
            {
                const byte code = CmuneOperationCodes.PhotonGameJoin;

                OperationRequest request = new OperationRequest()
                {
                    Parameters = OperationFactory.Create(code, RealtimeSerialization.ToBytes(room).ToArray(), cmid, accessLevel),
                    OperationCode = code
                };

                int invocID = 0;
                SendOperationToServer(request, true);

                if (CmuneNetworkState.DebugMessaging)
                    CmuneDebug.Log("({0}) - JoinRoom {1} on Server with InvocID: {2}", SessionID, room.RoomID, invocID);

                ConnectionState = NetworkState.STATE_JOINING;
            }
            else
            {
                CmuneDebug.LogError("({0}) - JoinRoom {1} by {2} failed because not connected yet", SessionID, room.RoomID, SessionID);
            }
        }

        private void ReadBinaryLobbyListUpdate(IDictionary data)
        {
            byte[] bytes = OperationUtil.GetGeneralArg<byte[]>(data, ParameterKeys.LobbyRoomUpdate);

            try
            {
                List<RoomMetaData> rooms = RealtimeSerialization.ToObject(bytes) as List<RoomMetaData>;
                if (rooms != null)
                {
                    foreach (RoomMetaData r in rooms)
                    {
                        CmuneNetworkState.AddRoom(r);
                    }
                }
            }
            catch
            {
                CmuneDebug.LogError("({0}) - LobbyList Update failed: Byte[] null = {1}", SessionID, (bytes == null));
            }
        }

        private void ReadBinaryLobbyListRemoval(Hashtable data)
        {
            byte[] bytes = OperationUtil.GetGeneralArg<byte[]>(data, ParameterKeys.LobbyRoomDelete);

            try
            {
                List<CmuneRoomID> rooms = RealtimeSerialization.ToObject(bytes) as List<CmuneRoomID>;
                if (rooms != null)
                {
                    foreach (CmuneRoomID s in rooms)
                    {
                        CmuneNetworkState.RemoveRoom(s);
                    }
                }

            }
            catch
            {
                CmuneDebug.LogError("({0}) - LobbyList Removal failed: Byte[] null = {1}", SessionID, (bytes == null));
            }
        }

        private void OnIncomingMessage(IDictionary<byte, object> data)
        {
            short netID = OperationUtil.GetArg<short>(data, ParameterKeys.InstanceId);
            byte localAddress = OperationUtil.GetArg<byte>(data, ParameterKeys.MethodId);
            byte[] bytes = OperationUtil.GetArg<byte[]>(data, ParameterKeys.Bytes);

            if (NetworkStatistics.IsEnabled)
                NetworkStatistics.RecordIncomingCall(netID + "/" + localAddress, bytes.Length);

            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("({0}) - OnIncomingMessage {1}:{2}", SessionID, netID, localAddress);

            try
            {
                object[] args = RealtimeSerialization.ToObjects(bytes);
                if (_recieveMessages != null)
                    _recieveMessages(netID, localAddress, args);
            }
            catch (Exception ex)
            {
                CmuneDebug.LogError("({0}) - OnIncomingMessage {1}:{2} crashed with: {3}\n{4}", SessionID, netID, localAddress, ex.Message, ex.StackTrace);
            }
        }

        private Dictionary<short, OperationCallback> _operationsWaitingForResponse = new Dictionary<short, OperationCallback>();

        private short _invocationId = 1;

        /// <summary>
        /// This function must only be used to send specific instructions to the server that concern the whole application.
        /// This means that it is not processed room based (as usual) but application based.
        /// Typical usages are for example 'kick a peer from server', 'ban member', 'load query' or 'game id lookup'
        /// </summary>
        /// <param name="code">Operation Code</param>
        /// <param name="args"></param>
        /// <param name="action"></param>
        /// <returns>Operation Invocation ID</returns>
        internal short SendOperationToServerApplication(Action<int, object[]> action, byte appMethodID, params object[] args)
        {
            if (_peer != null)
            {
                OperationRequest request = new OperationRequest()
                {
                    Parameters = OperationFactory.Create(CmuneOperationCodes.MessageToApplication, appMethodID, ++_invocationId, RealtimeSerialization.ToBytes(args).ToArray()),
                    OperationCode = CmuneOperationCodes.MessageToApplication
                };

                _peer.OpCustom(request.OperationCode, request.Parameters, true);

                if (action != null)
                    _operationsWaitingForResponse[_invocationId] = new OperationCallback(action);

                if (CmuneNetworkState.DebugMessaging)
                    CmuneDebug.Log("({0}) - SendOperation: ApplicationMethodId {1} with invocID {2}, has callback: {3}", SessionID, appMethodID, _invocationId, action != null);

                //CmuneDebug.Log("MessageToApplication " + appMethodID + " with InvocID: " + invocID + " @ " + _peer.ServerAddress);

                return _invocationId;
            }
            else
            {
                CmuneDebug.LogError("({0}) - SendOperationToServerApplication failed because peer NULL", SessionID);
                return -1;
            }
        }

        /// <summary>
        /// Use this function to trigger server code on a room based level.
        /// On the server the call will be routed through the RMI layer and executed inside of a room or 
        /// distributed to other peers, depending on the operation code.
        /// </summary>
        /// <param name="code">Operation Code</param>
        /// <param name="args"></param>
        /// <param name="isReliable"></param>
        /// <returns></returns>
        internal bool SendOperationToServer(OperationRequest request, bool isReliable)
        {
            if (_peer != null)
            {
                if (CmuneNetworkState.DebugMessaging)
                    CmuneDebug.Log("({0}) - SendMessage of Type {1}", SessionID, request.OperationCode);

                if (NetworkStatistics.IsEnabled)
                    NetworkStatistics.RecordOutgoingCall(request);

                bool ret = _peer.OpCustom(request.OperationCode, request.Parameters, isReliable);

                return ret;
            }
            else
            {
                CmuneDebug.LogError("({0}) - SendOperationToServer failed because peer NULL", SessionID);
                return false;
            }
        }

        #region INPeerListener Members

        public void OnStatusChanged(StatusCode statusCode)
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("PeerStatusCallback " + statusCode);

            switch (statusCode)
            {
                case StatusCode.Connect:                            // = 1024
                    {
                        ConnectionState = NetworkState.STATE_CONNECTED;
                        //IsConnectedToServer = true;

                        SessionID = CmuneNetworkState.GetNextSessionID();

                        //_connectionEvent = ConnectionEventType.Connected;
                        EnqueueConnectionEvent(ConnectionEventType.Connected);

                        //get peer spec data
                        //Hashtable data = OperationFactory.Create(CmuneOperationCodes.MessageToApplication, ApplicationRPC.PeerSpecification, (object)RealtimeSerialization.ToBytes(new object[] { PeerType.GamePeer, string.Empty }));

                        //the first you want to do is to tell the server that you are a regular peer (and not e.g. a game server)
                        int invocID = SendOperationToServerApplication(null, ApplicationRPC.PeerSpecification, (byte)PeerType.GamePeer, Cmune.Realtime.Common.Protocol.Version);

                        if (CmuneNetworkState.DebugMessaging)
                            CmuneDebug.Log("({0}) - SendOperation: PeerSpecification {1} with invocID {2}", SessionID, PeerType.GamePeer, invocID);
                        break;
                    }
                case StatusCode.DisconnectByServerLogic:             // = 1041
                case StatusCode.DisconnectByServerUserLimit:         // = 1041
                case StatusCode.DisconnectByServer:                  // = 1041
                    {
                        if (CmuneNetworkState.DebugMessaging)
                            CmuneDebug.LogError("({0}) - RETURN DisconnectByServer", SessionID);

                        ConnectionState = NetworkState.STATE_DISCONNECTED;

                        //_connectionEvent = ConnectionEventType.Disconneced;
                        EnqueueConnectionEvent(ConnectionEventType.Disconnected);

                        break;
                    }
                case StatusCode.TimeoutDisconnect:                   // = 1040
                    {
                        if (CmuneNetworkState.DebugMessaging)
                            CmuneDebug.LogError("({0}) - RETURN TimeoutDisconnect", SessionID);

                        ConnectionState = NetworkState.STATE_DISCONNECTED;

                        //_connectionEvent = ConnectionEventType.Disconneced;
                        EnqueueConnectionEvent(ConnectionEventType.Disconnected);

                        break;
                    }
                case StatusCode.Disconnect:                           // = 1025
                    {
                        ConnectionState = NetworkState.STATE_DISCONNECTED;

                        //_connectionEvent = ConnectionEventType.Disconneced;
                        EnqueueConnectionEvent(ConnectionEventType.Disconnected);

                        break;
                    }
                case StatusCode.QueueOutgoingReliableWarning:               // = 1032;
                //case StatusCode.QueueOutgoingReliableError:             // = 1031;
                case StatusCode.QueueOutgoingUnreliableWarning:           // = 1028;
                case StatusCode.QueueOutgoingAcksWarning:         // = 1027;
                case StatusCode.QueueSentWarning:                     // = 1037;
                    {
                        CmuneDebug.LogWarning("({0}) - RETURN <OUT-QUEUE> FILLING UP {1} ", SessionID, statusCode);

                        //_peer.sendOutgoingCommands();
                        //increase sending frequency

                        break;
                    }
                case StatusCode.QueueIncomingReliableWarning:           // = 1034;
                case StatusCode.QueueIncomingUnreliableWarning:         // = 1033;
                    {

                        CmuneDebug.LogWarning("({0}) - RETURN <IN-QUEUE> FILLING UP {1} {2}/{3}", SessionID, statusCode, _peer.QueuedIncomingCommands, _peer.QueuedOutgoingCommands);

                        //while (_peer.dispatchIncomingCommands()) ;
                        //increase dispatching frequency

                        break;
                    }

                case StatusCode.Exception:                            // = 1026;
                case StatusCode.ExceptionOnConnect:                    // = 1023;
                case StatusCode.InternalReceiveException:             // = 1039;
                    {
                        if (CmuneNetworkState.DebugMessaging)
                            CmuneDebug.LogError("({0}) - RETURN Exception: {1}", SessionID, statusCode);

                        //don't process any other command anymore
                        //_processIncomingCommands = false;

                        ConnectionState = NetworkState.STATE_DISCONNECTED;

                        //CmuneEventHandler.Route(new ServerErrorEvent((int)returnCode));

                        //_connectionEvent = ConnectionEventType.Disconneced;
                        EnqueueConnectionEvent(ConnectionEventType.Disconnected);

                        break;
                    }

                case StatusCode.SecurityExceptionOnConnect:
                    {

                        CmuneDebug.LogError("({0}) SecurityExceptionOnConnect - check if PolicyServer is running", SessionID);
                        break;
                    }

                default:
                    {
                        CmuneDebug.LogError("({0}) - UNHANDLED PeerStatusCallback with returnCode: {1}", SessionID, statusCode);

                        break;
                    }
            }
        }

        private void EnqueueConnectionEvent(ConnectionEvent ev)
        {
            _connectionEvents.Enqueue(ev);
            _lastEventEnqueued = ev.Type;
        }

        private void EnqueueConnectionEvent(ConnectionEventType ev)
        {
            if (_connectionEvents.Count == 0 || _lastEventEnqueued != ev)
                _connectionEvents.Enqueue(new ConnectionEvent(ev, CurrentRoom, ActorIdSecure));

            _lastEventEnqueued = ev;
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("({0}) - OperationResult with returnCode: {1},  opCode: {2}", SessionID, operationResponse.ReturnCode, operationResponse.OperationCode);

            switch (operationResponse.OperationCode)
            {
                case CmuneOperationCodes.MessageToApplication:
                    {
                        if (operationResponse.ReturnCode == 0 && OperationUtil.HasArg(operationResponse.Parameters, ParameterKeys.InvocationId))
                        {
                            short invocId = OperationUtil.GetArg<short>(operationResponse.Parameters, ParameterKeys.InvocationId);
                            //CmuneDebug.Log("invocId " + invocId);

                            OperationCallback c;
                            if (_operationsWaitingForResponse.TryGetValue(invocId, out c))
                            {
                                _operationsWaitingForResponse.Remove(invocId);

                                if (OperationUtil.HasArg(operationResponse.Parameters, ParameterKeys.Data))
                                {
                                    try
                                    {
                                        if (c != null && c.Callback != null)
                                            c.Callback(operationResponse.ReturnCode, RealtimeSerialization.ToObjects(OperationUtil.GetArg<byte[]>(operationResponse.Parameters, ParameterKeys.Data)));
                                    }
                                    catch
                                    {
                                        CmuneDebug.LogError("({0}) - Error executing Action after recieving response from MessageToApplication", SessionID);
                                    }
                                }
                                else { CmuneDebug.LogWarning("{0} MessageToApplication return executed no action for invocId {1} because no data attached", SessionID, invocId); }

                            }
                            else { CmuneDebug.LogWarning("{0} MessageToApplication return executed no action for invocId {1}", SessionID, invocId); }
                        }
                        else
                        {
                            CmuneDebug.LogError("{0} MessageToApplication {1} failed with message: {2}", SessionID, operationResponse.ReturnCode, operationResponse.DebugMessage);
                        }
                        //Debug.Log("MessageToApplication " + returnCode + " end " + invocID);
                        break;
                    }
                case CmuneOperationCodes.PhotonGameJoin: //88
                    {
                        ConnectionState = NetworkState.STATE_RECEIVING;

                        if (operationResponse.ReturnCode == 0)
                        {
                            int actorId = OperationUtil.GetActor(operationResponse.Parameters);
                            byte[] roomId = OperationUtil.GetArg<byte[]>(operationResponse.Parameters, ParameterKeys.GameId);
                            bool initializeRoom = OperationUtil.GetArg<bool>(operationResponse.Parameters, ParameterKeys.InitRoom);
                            long serverTicks = OperationUtil.GetArg<long>(operationResponse.Parameters, ParameterKeys.ServerTicks);

                            if (actorId > 0)
                            {
                                _actorId.WriteData(actorId);
                                CurrentRoom = new CmuneRoomID(roomId);

                                HasJoinedRoom = true;

                                _connectionEvents.Clear();

                                if (initializeRoom)
                                    _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.BuildLevel, CurrentRoom, ActorIdSecure));
                                else
                                    _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.ClearLevel, CurrentRoom, ActorIdSecure));

                                _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.JoinedRoom, CurrentRoom, ActorIdSecure));
                            }
                            else
                            {
                                //very rare case
                                CmuneDebug.LogWarning("PhotonGameJoin failed with actorId " + actorId);
                                _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.JoinFailed, CurrentRoom, ActorIdSecure, operationResponse.ReturnCode));
                                _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.LeftRoom, CurrentRoom, ActorIdSecure));
                                DoDisconnect = true;
                            }
                        }
                        else
                        {
                            CmuneDebug.LogError("({0}) - PhotonGameJoin failed with code {1} and message {2}", SessionID, operationResponse.ReturnCode, operationResponse.DebugMessage);

                            _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.JoinFailed, CurrentRoom, ActorIdSecure, operationResponse.ReturnCode));
                            _connectionEvents.Enqueue(new ConnectionEvent(ConnectionEventType.LeftRoom, CurrentRoom, ActorIdSecure));
                            DoDisconnect = true;
                        }

                        break;
                    }
                case CmuneOperationCodes.PhotonGameLeave: //89
                    {
                        EnqueueConnectionEvent(ConnectionEventType.LeftRoom);

                        OnRoomLeft();

                        break;
                    }

                case (byte)CmuneOperationCodes.MessageToServer:
                case (byte)CmuneOperationCodes.MessageToPlayer:
                case (byte)CmuneOperationCodes.MessageToOthers:
                case (byte)CmuneOperationCodes.MessageToAll:
                    {
                        CmuneDebug.LogError("({0}) - Operation '{1}' not allowed on Server and returned with code {2}", SessionID, operationResponse.OperationCode, operationResponse.ReturnCode);
                        break;
                    }

                default:
                    {
                        CmuneDebug.LogError("({0}) - UNHANDLED OperationResult with returnCode: {1},  opCode: {2}", SessionID, operationResponse.ReturnCode, operationResponse.OperationCode);
                        break;
                    }
            }
        }

        public void OnEvent(EventData eventData)
        {
            try
            {
                switch (eventData.Code)
                {
                    case CmuneEventCodes.GameListInit:
                        {
                            CmuneNetworkState.ClearRooms();

                            //Debug.Log("GameListInit " + eventData.ToStringFull());
                            //Debug.Log(OperationUtil.PrintHashtable(eventData.Parameters));

                            Hashtable data = OperationUtil.GetArg<Hashtable>(eventData.Parameters, ParameterKeys.Data);

                            ReadBinaryLobbyListUpdate(data);

                            CmuneEventHandler.Route(new RoomListUpdatedEvent(CmuneNetworkState.AllRooms, true));

                            break;
                        }

                    case CmuneEventCodes.GameListUpdate:
                        {
                            //Debug.Log("GameListUpdate " + eventData.ToStringFull());
                            //Debug.Log(OperationUtil.PrintHashtable(eventData.Parameters));

                            Hashtable data = OperationUtil.GetArg<Hashtable>(eventData.Parameters, ParameterKeys.Data);

                            ReadBinaryLobbyListUpdate(data);

                            CmuneEventHandler.Route(new RoomListUpdatedEvent(CmuneNetworkState.AllRooms));

                            break;
                        }

                    case CmuneEventCodes.GameListRemoval:
                        {
                            //Debug.Log("GameListRemoval " + eventData.ToStringFull());
                            //Debug.Log(OperationUtil.PrintHashtable(eventData.Parameters));

                            Hashtable data = OperationUtil.GetArg<Hashtable>(eventData.Parameters, ParameterKeys.Data);

                            ReadBinaryLobbyListRemoval(data);

                            CmuneEventHandler.Route(new RoomListUpdatedEvent(CmuneNetworkState.AllRooms));
                            break;
                        }

                    case CmuneEventCodes.Standard:
                        {
                            OnIncomingMessage(eventData.Parameters);
                            break;
                        }

                    default:
                        {
                            CmuneDebug.LogError("({0}) - UNHANDLED EventAction with code: {1} and argCount: {2}", SessionID, eventData.Code, eventData.Parameters.Count);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                CmuneDebug.LogError("({0}) - CRASH IN EventAction with eventCode: {1}", SessionID, eventData.Code);
                CmuneDebug.LogError("{0}{1}", e.Message, e.StackTrace);
                CmuneDebug.LogError(OperationUtil.PrintHashtable(eventData.Parameters));
            }
        }

        public void DebugReturn(DebugLevel level, string debug)
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.LogError("({0}) - DebugReturn: {1}", SessionID, debug);
        }

        #endregion

        //#region IDisposable

        //public void Dispose()
        //{
        //    SecureMemory<int>.ReleaseData(_actorId);
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected bool _isDisposed = false;
        //protected virtual void Dispose(bool dispose)
        //{
        //    if (_isDisposed) return;

        //    _recieveMessages = null;
        //    _castConnectionEvents = null;

        //    if (dispose)
        //    {
        //        //send disconnect command
        //        if (_peer.PeerState > 0)
        //        {
        //            _peer.Disconnect();

        //            //clean up local threads
        //            _peer.StopThread();
        //        }
        //    }

        //    _isDisposed = true;
        //}

        //#endregion

        private bool _isShutdown = false;
        public void ShutDown()
        {
            if (_isShutdown) return;
            _isShutdown = true;

            _recieveMessages = null;
            _castConnectionEvents = null;

            //send disconnect command
            if (_peer.PeerState > 0)
            {
                _peer.Disconnect();
                _peer.StopThread();
            }
        }

        public enum ConnectionEventType
        {
            None = 0,

            Connected = 1,
            Disconnected = 2,

            JoinedRoom = 3,
            LeftRoom = 4,

            OtherJoined = 5,
            OtherLeft = 6,

            JoinFailed = 7,

            BuildLevel = 8,
            ClearLevel = 9,
        }

        public class ConnectionEvent
        {
            public ConnectionEvent(ConnectionEventType t, CmuneRoomID id, int actorID)
                : this(t, id, actorID, 0)
            {
            }


            public ConnectionEvent(ConnectionEventType t, CmuneRoomID id, int actorID, int errorCode)
            {
                Type = t;
                Room = id;
                ActorID = actorID;
                ErrorCode = errorCode;
            }

            public ConnectionEventType Type { get; private set; }
            public CmuneRoomID Room { get; private set; }
            public int ActorID { get; private set; }
            public int ErrorCode { get; private set; }
        }

        private class OperationCallback
        {
            public OperationCallback(Action<int, object[]> action) { Callback = action; }
            public readonly Action<int, object[]> Callback;
        }
    }
}