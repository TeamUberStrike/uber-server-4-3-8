using System;
using System.Collections;
using Cmune.Realtime.Common;
using UnityEngine;
using Cmune.Util;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Realtime.Photon.Client
{
    public class PhotonClient// : IDisposable
    {
        public PhotonClient(MonoBehaviour mono, bool monitorMemory = false)
        {
            _mono = mono;

            _peerListener = new PhotonPeerListener(monitorMemory);

            _messenger = new NetworkMessenger(_peerListener);

            _rmi = new RemoteMethodInterface(_messenger);

            _peerListener.SubscribeToEvents(EventCallback);

            _peerListener.SetMessageCallback(_rmi.RecieveMessage);
        }

        public Coroutine ConnectToRoom(RoomMetaData room, int cmid, MemberAccessLevel accessLevel)
        {
            if (PeerListener.IsConnectedToServer && PeerListener.Server == room.ServerConnection && !PeerListener.HasJoinedRoom)
            {
                return _mono.StartCoroutine(StartJoiningRoom(room, cmid, (int)accessLevel));
            }
            else
            {
                return _mono.StartCoroutine(StartConnection(room.ServerConnection, room, cmid, (int)accessLevel));
            }
        }

        public Coroutine ConnectToServer(string server, int cmid, MemberAccessLevel accessLevel)
        {
            return _mono.StartCoroutine(StartConnection(server, null, cmid, (int)accessLevel));
        }

        public Coroutine Disconnect()
        {
            return _mono.StartCoroutine(StopConnection());
        }

        private bool _isShutdown = false;
        public void ShutDown()
        {
            if (_isShutdown) return;
            _isShutdown = true;

            _peerListener.UnsubscribeToEvents(EventCallback);
            _peerListener.ShutDown();
        }

        //#region IDisposable

        //public void Dispose()
        //{
        //    _peerListener.Dispose();

        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected bool _isDisposed = false;
        //protected virtual void Dispose(bool dispose)
        //{
        //    if (_isDisposed) return;

        //    if (dispose)
        //    {
        //        _peerListener.UnsubscribeToEvents(EventCallback);
        //        _peerListener.Dispose();
        //    }

        //    _isDisposed = true;
        //}

        //#endregion

        public void Update()
        {
            if (!_isUpdateCalled) _isUpdateCalled = true;

            _peerListener.Update();
        }

        private IEnumerator StartConnection(string server, RoomMetaData room, int cmid, int accessLevel)
        {
            ConnectionAddress address = new ConnectionAddress(server);

            //CmuneDebug.Log(address + " " + server);
            //CmuneDebug.Log("StartConnection " + !_isStartingConnection + " && " + IsPhotonEnabled + " && " + address.IsValid);

            if (!_isStartingConnection && PhotonClient.IsPhotonEnabled && address.IsValid)
            {
                _isStartingConnection = true;

                //wait if currently Disconnecting
                while (_isStoppingConnection)
                    yield return new WaitForEndOfFrame();

                //only ever connect when the connection is stopped
                if (ConnectionState == ConnectionStatus.STOPPED)
                {
                    ConnectionState = ConnectionStatus.POLICY;
                    yield return _mono.StartCoroutine(CrossdomainPolicy.CheckDomain(address.ServerIP));

                    if (CrossdomainPolicy.HasValidPolicy(address.ServerIP))
                    {
                        //connect to server
                        if (_peerListener.Connect(address.ConnectionString, cmid))
                        {
                            ConnectionState = ConnectionStatus.STARTING;
                            _connectionTime = 0;

                            //wait until connection established
                            float timeout = Time.time + 10;
                            while (_peerListener.IsConnecting && Time.time < timeout && ConnectionState == ConnectionStatus.STARTING)
                            {
                                yield return new WaitForEndOfFrame();
                                _connectionTime += Time.deltaTime;
                                if (!_isUpdateCalled)
                                {
                                    timeout = Time.time;
                                    CmuneDebug.LogError("Please call PhotonClient.Update() in an Update loop!");
                                }
                            }

                            if (_peerListener.IsConnectedToServer && ConnectionState == ConnectionStatus.STARTING)
                            {
                                if (room != null)
                                {
                                    //join the room
                                    _peerListener.JoinRoom(room, cmid, accessLevel);

                                    //wait until room entered
                                    timeout = Time.time + 30;
                                    while (_peerListener.IsJoining && Time.time < timeout && PhotonClient.IsPhotonEnabled)
                                    {
                                        yield return new WaitForEndOfFrame();
                                        _connectionTime += Time.deltaTime;
                                    }

                                    if (!_peerListener.HasJoinedRoom || !PhotonClient.IsPhotonEnabled)
                                    {
                                        ConnectionState = ConnectionStatus.STOPPING;
                                        _peerListener.Disconnect(true);

                                        while (_peerListener.PeerState > 0)
                                        {
                                            yield return new WaitForEndOfFrame();
                                        }
                                    }

                                    //state will be handled in callback function if join is successfull
                                }
                                else
                                {
                                    ConnectionState = ConnectionStatus.RUNNING;
                                }
                            }
                            else if (ConnectionState != ConnectionStatus.STOPPED)
                            {
                                ConnectionState = ConnectionStatus.STOPPING;
                                _peerListener.Disconnect(true);
                                while (_peerListener.PeerState > 0)
                                {
                                    yield return new WaitForEndOfFrame();
                                }
                            }
                        }
                        else
                        {
                            ConnectionState = ConnectionStatus.STOPPED;
                            CmuneDebug.LogError("Server has Crossdomain Policy but not reachable: " + address.ConnectionString);
                        }
                    }
                    else
                    {
                        CmuneDebug.LogError("No Crossdomain Policy hosted on port 843 on : " + address.ServerIP);
                        EventCallback(new PhotonPeerListener.ConnectionEvent(PhotonPeerListener.ConnectionEventType.Disconnected, CmuneRoomID.Empty, 0));
                    }
                }
                else
                {
                    CmuneDebug.LogWarning("Please first stop Connection before reconnecting! " + _peerListener.ConnectionState);
                }

                if (ConnectionState == ConnectionStatus.STOPPING)
                {
                    _peerListener.Disconnect(true);
                    while (_peerListener.PeerState > 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                _isStartingConnection = false;
            }
            else
            {
                if (_isStartingConnection)
                    CmuneDebug.LogWarning("Ignored StartConnection because already running: " + _peerListener.ConnectionState);
                else if (!address.IsValid)
                    CmuneDebug.LogWarning("Ignored StartConnection because address is not valid: " + address.ConnectionString);
            }

        }

        private IEnumerator StartJoiningRoom(RoomMetaData room, int cmid, int accessLevel)
        {
            if (!_isStartingConnection && IsPhotonEnabled)
            {
                _isStartingConnection = true;

                //only ever connect when the connection is stopped
                if (ConnectionState == ConnectionStatus.RUNNING)
                {
                    if (_peerListener.IsConnectedToServer)
                    {
                        //join the room
                        _peerListener.JoinRoom(room, cmid, accessLevel);

                        //wait until room entered
                        while (_peerListener.IsJoining)
                        {
                            _connectionTime += Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                else
                {
                    CmuneDebug.LogWarning("Please first start a Connection before joining a room! " + _peerListener.ConnectionState);
                }

                _isStartingConnection = false;
            }
            else
            {
                CmuneDebug.LogError("Error: failed to join room: " + room.RoomName + " " + _isStartingConnection + " " + IsPhotonEnabled);
                //throw new System.Exception();
            }
        }

        private IEnumerator StopConnection()
        {
            if (!_isStoppingConnection)
            {
                _isStoppingConnection = true;

                if (ConnectionState != ConnectionStatus.STOPPED)
                {
                    //update connection state
                    ConnectionState = ConnectionStatus.STOPPING;

                    if (_isStartingConnection)
                    {
                        //ignore
                    }
                    else
                    {
                        //leave room if  inside
                        if (_peerListener.HasJoinedRoom)
                        {
                            _peerListener.LeaveCurrentRoom();

                            //wait until left
                            while (_peerListener.IsLeaving)
                                yield return new WaitForEndOfFrame();
                        }

                        //disconnect from server
                        _peerListener.Disconnect();

                        //wait until disconnected
                        while (_peerListener.PeerState > 0)
                            yield return new WaitForEndOfFrame();
                    }
                }
                _isStoppingConnection = false;
            }
        }

        private void EventCallback(PhotonPeerListener.ConnectionEvent ev)
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("EventCallback " + ev.Type);

            switch (ev.Type)
            {
                case PhotonPeerListener.ConnectionEventType.Disconnected:
                    {
                        ConnectionState = ConnectionStatus.STOPPED;

                        _rmi.UnregisterAllClasses();

                        break;
                    }
                case PhotonPeerListener.ConnectionEventType.JoinedRoom:
                    {
                        if (ConnectionState == ConnectionStatus.STOPPING)
                        {
                            _peerListener.Disconnect();
                        }
                        else
                        {
                            ConnectionState = ConnectionStatus.RUNNING;
                            _rmi.RegisterAllClasses();
                        }
                        break;
                    }
            }
        }

        #region PROPERTIES

        public string CurrentConnection
        {
            get { return PeerListener.Server; }
        }

        public string MessagingState
        {
            get { return string.Format("{0} {1}", _peerListener.CurrentRoom.Server, _peerListener.ConnectionState); }
        }

        public ConnectionStatus ConnectionState { get; private set; }

        public float ConnectionTime
        {
            get { return _connectionTime; }
        }

        public RemoteMethodInterface Rmi
        {
            get { return _rmi; }
        }

        public PhotonPeerListener PeerListener
        {
            get { return _peerListener; }
        }

        public bool IsActive
        {
            get { return _peerListener.PeerState > 0; }
        }

        public bool IsConnected
        {
            get { return ConnectionState == ConnectionStatus.STARTING || ConnectionState == ConnectionStatus.RUNNING; }
        }

        public int Latency
        {
            get { return _peerListener.Latency; }
        }

        public string Debug
        {
            get { return string.Format("Start {0}/Stop {1}, ConState {2}, PeerState {3}/{4}", _isStartingConnection, _isStoppingConnection, ConnectionState, _peerListener.ConnectionState, _peerListener.PeerState); }
        }

        #endregion

        #region FIELDS
        private bool _isUpdateCalled = false;
        public static bool IsPhotonEnabled = true;

        private bool _isStoppingConnection = false;
        private bool _isStartingConnection = false;

        private float _connectionTime = 0;

        private MonoBehaviour _mono;
        private PhotonPeerListener _peerListener;
        private RemoteMethodInterface _rmi;
        private NetworkMessenger _messenger;
        #endregion

        public enum ConnectionStatus
        {
            STOPPED = 0,
            RUNNING = 1,
            STARTING = 2,
            STOPPING = 4,
            POLICY = 5,
        }
    }
}