
using System;
using Cmune.Realtime.Common;
using Cmune.Util;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrike.Realtime.Server;

namespace Cmune.Realtime.Photon.Server
{
    public abstract class CmunePeer : PeerBase, IDisposable, ICmunePeer
    {
        public int Cmid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LitePeer"/> class.
        /// </summary>
        /// <param name="photonPeer">
        /// The photon peer.
        /// </param>
        protected CmunePeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmid)
            : base(protocol, photonPeer)
        {
            Cmid = cmid;

            _sendFiber = new PoolFiber();

            _address = new ConnectionAddress(photonPeer.GetRemoteIP(), (short)photonPeer.GetLocalPort());

            _photonPeer = photonPeer;

            _sessionID = CmunePeerId.Next;

            _sendFiber.Start();
        }

        /// <summary>
        /// Gets PhotonPeer.
        /// </summary>
        public IPhotonPeer PhotonPeer
        {
            get
            {
                return this._photonPeer;
            }
        }

        /// <summary>
        /// Gets SessionId.
        /// </summary>
        public int SessionId
        {
            get
            {
                return _sessionID;
            }
        }

        public string IpAddress
        {
            get
            {
                return (PhotonPeer != null) ? PhotonPeer.GetRemoteIP() : "127.0.0.1";
            }
        }

        /// <summary>
        /// The publish event.
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void PublishEvent(EventData eventData, SendParameters sendParameters)
        {
            if (_isDisposed) return;

            this._sendFiber.Enqueue(
                () =>
                {
                    try
                    {
                        SendEvent(eventData, sendParameters);
                    }
                    catch (Exception e)
                    {
                        CmuneDebug.LogError(e);
                    }
                });
        }

        /// <summary>
        /// The publish operation response.
        /// </summary>
        /// <param name="operationResponse">
        /// The operation response.
        /// </param>
        public void PublishOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            if (_isDisposed) return;

            this._sendFiber.Enqueue(
                () =>
                {
                    try
                    {
                        SendOperationResponse(operationResponse, sendParameters);
                    }
                    catch (Exception e)
                    {
                        CmuneDebug.LogError(e);
                    }
                });
        }

        /// <summary>
        /// The on disconnect.
        /// </summary>
        protected override void OnDisconnect()
        {
            //lock (Counter.Players)
            //{
            //    Counter.Players.Decrement();
            //}

            Dispose();
        }

        /// <summary>
        /// The on operation request.
        /// </summary>
        /// <param name="operationRequest">
        /// The operation Request.
        /// </param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case CmuneOperationCodes.MessageToApplication:
                    {
                        //if (CmuneDebug.IsDebugEnabled)
                        //    CmuneDebug.LogFormat("MessageToApplication {0}/{1} by peer {2}", operationRequest.OperationCode, operationRequest.InvocationId, SessionId);

                        ApplicationCenter.RecieveMessage(this, new MessageToApplication(Protocol, operationRequest));
                        break;
                    }
                default:
                    {
                        if (PeerType != Cmune.Realtime.Common.PeerType.None)
                        {
                            CmuneDebug.LogError("Recieved OperationCode {0} by peer {1} but operation not supported on this server!", operationRequest.OperationCode, PeerType);
                            CmuneOperations.DebugRequest(this, operationRequest);
                        }

                        PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 1, DebugMessage = "Operation not supported on Server!" }, sendParameters);
                        break;
                    }
            }
        }

        #region Properties

        public ConnectionAddress Address
        {
            get { return _address; }
            internal set { _address = value; }
        }

        public Cmune.Realtime.Common.PeerType PeerType
        {
            get { return _peerType; }
            internal set { _peerType = value; }
        }

        //public int Cmid
        //{
        //    get { return _cmid; }
        //    set { _cmid = value; }
        //}

        //public IEnumerable<int> FriendCmids
        //{
        //    get { return _friendsCmids; }
        //    set { _friendsCmids.Clear(); _friendsCmids.AddRange(value); }
        //}

        #endregion

        #region Fields

        private Cmune.Realtime.Common.PeerType _peerType = Cmune.Realtime.Common.PeerType.None;

        //private int _cmid;

        //private List<int> _friendsCmids;

        /// <summary>
        /// The photon peer.
        /// </summary>
        private readonly IPhotonPeer _photonPeer;

        /// <summary>
        /// The send fiber.
        /// </summary>
        private readonly IFiber _sendFiber;

        /// <summary>
        /// The cached IP Address and Port
        /// </summary>
        private ConnectionAddress _address;

        /// <summary>
        /// The cached IP Address and Port
        /// </summary>
        private int _sessionID;

        #endregion

        #region IDisposable

        private bool _isDisposed = false;

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);

            if (_isDisposed) return;

            if (dispose)
            {
                _sendFiber.Dispose();
            }

            _isDisposed = true;
        }

        #endregion


        public UberStrike.Core.Models.ActorInfo Info
        {
            get { throw new NotImplementedException(); }
        }

        public void PublishEvent(IEventData eventData, bool reliable = true)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadCache(byte id)
        {
            throw new NotImplementedException();
        }

        public void WriteCache(byte id, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}