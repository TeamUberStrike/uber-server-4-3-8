
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class GamePeer : CmunePeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CmunePeer"/> class.
        /// </summary>
        /// <param name="photonPeer">
        /// The photon peer.
        /// </param>
        public GamePeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmid)
            : base(protocol, photonPeer, cmid)
        { }

        /// <summary>
        /// The on disconnect.
        /// </summary>
        protected override void OnDisconnect()
        {
            //CmuneDebug.LogInfo("OnDisconnect  " + SessionId);

            if (this.CurrentRoom != null)
            {
                if (this.CurrentRoom.Number != StaticRoomID.LobbyCenter)
                    DecrementPlayerCount();

                this.CurrentRoom.LeaveGame(this);
                this.CurrentRoom = null;
            }

            base.OnDisconnect();
        }

        /// <summary>
        /// The on operation request.
        /// </summary>
        /// <param name="operationRequest">
        /// The operation Request.
        /// </param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //if (CmuneDebug.IsDebugEnabled)
            //    CmuneDebug.LogFormat("OperationRequest: {0}", operationRequest.OperationCode);

            if (PeerType != Cmune.Realtime.Common.PeerType.None)
            {
                switch (operationRequest.OperationCode)
                {
                    case CmuneOperationCodes.MessageToServer:
                    case CmuneOperationCodes.MessageToAll:
                    case CmuneOperationCodes.MessageToPlayer:
                    case CmuneOperationCodes.MessageToOthers:
                        {
                            HandleRoomMessage(operationRequest, sendParameters);
                            break;
                        }

                    case CmuneOperationCodes.PhotonGameJoin:
                        {
                            HandleJoin(operationRequest, sendParameters);
                            break;
                        }

                    case CmuneOperationCodes.PhotonGameLeave:
                        {
                            HandleLeave(operationRequest, sendParameters);
                            break;
                        }

                    default:
                        {
                            base.OnOperationRequest(operationRequest, sendParameters);
                            break;
                        }
                }
            }
            else
            {
                base.OnOperationRequest(operationRequest, sendParameters);
            }
        }

        #region OPERATION HANDLING

        //private void HandleBanPlayerFromRoom(OperationRequest operationRequest)
        //{
        //    //
        //}

        //private void HandleRoomRequest(OperationRequest operationRequest, SendParameters sendParameters)
        //{
        //    QueryRoomDataOperation op = new QueryRoomDataOperation(operationRequest);

        //    CmuneRoom room;
        //    if (CmuneRoomFactory.Instance.TryGetGame(op.RoomNumber, out room))
        //    {
        //        QueryResponse response = new QueryResponse(operationRequest.
        //        op.RoomData = room.RoomData.GetBytes();
        //        PublishOperationResponse(op.GetOperationResponse(0, string.Empty));
        //    }
        //    else
        //    {
        //        PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 1, DebugMessage = "Room not found" }, new SendParameters());
        //    }
        //}

        private void HandleRoomMessage(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (this.CurrentRoom != null)
            {
                this.CurrentRoom.EnqueueOperation(this, operationRequest, sendParameters);
            }
        }

        private void HandleJoin(OperationRequest operationRequest, SendParameters sendParameters)
        {
            JoinOperation joinOperation = new JoinOperation(Protocol, operationRequest);

            //if peer is not connected to another room right now
            if (this.CurrentRoom == null)
            {
                // get game from cache or create if not exists already
                CmuneRoom room = CmuneRoomFactory.GetOrCreateGame(joinOperation.Room);

                if (room == null)
                {
                    PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 1, DebugMessage = "Game doesn't exist anymore!" }, new SendParameters());
                }
                //if Cmid is not set or player is explicitly banned from game
                else if (joinOperation.Cmid == 0 || room.IsBannedFromRoom(joinOperation.Cmid))
                {
                    PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 3, DebugMessage = "You are banned from this game!" }, new SendParameters());
                    room.LeaveGame(this);
                }
                //allow admins to join every game
                else if (joinOperation.AccessLevel < 10 && room.IsRoomFull)
                {
                    PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 2, DebugMessage = "Game is already full!" }, new SendParameters());
                    room.LeaveGame(this);
                }
                //else if (joinOperation.AccessLevel < 10 && room.Number != StaticRoomID.LobbyCenter && Counter.Players.GetNextValue() >= ServerSettings.MaxPlayerCount)
                //{
                //    PublishOperationResponse(new OperationResponse(operationRequest.OperationCode) { ReturnCode = 4, DebugMessage = "The server is currently full!" }, new SendParameters());
                //    room.LeaveGame(this);
                //}
                else
                {
                    this.CurrentRoom = room;

                    if (this.CurrentRoom.Number != StaticRoomID.LobbyCenter)
                        IncrementPlayerCount();

                    this.CurrentRoom.EnqueueOperation(this, operationRequest, sendParameters);
                }
            }
            else
            {
                PublishOperationResponse(new OperationResponse(joinOperation.OperationRequest.OperationCode) { ReturnCode = 4, DebugMessage = "You are already connected to another room!" }, sendParameters);
            }
        }

        private void HandleLeave(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //normally the peer should have the current room cached in his state
            if (this.CurrentRoom != null)
            {
                this.CurrentRoom.EnqueueOperation(this, operationRequest, sendParameters);

                if (this.CurrentRoom.Number != StaticRoomID.LobbyCenter)
                    DecrementPlayerCount();
            }

            this.CurrentRoom = null;
        }
        #endregion

        private static void IncrementPlayerCount()
        {
            lock (Counter.Players)
            {
                Counter.Players.Increment();
            }
        }

        private static void DecrementPlayerCount()
        {
            lock (Counter.Players)
            {
                Counter.Players.Decrement();
            }
        }

        #region PROPERTIES

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        public CmuneRoom CurrentRoom { get; set; }

        #endregion
    }
}