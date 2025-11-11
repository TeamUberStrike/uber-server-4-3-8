
using System;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Util;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class JoinOperation : Operation
    {
        public JoinOperation(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            try
            {
                Room = RealtimeSerialization.ToObject(Bytes) as RoomMetaData;
            }
            catch (System.NullReferenceException ex)
            {
                CmuneDebug.Exception("PhotonGameJoin Failed because: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public OperationResponse CreateOperationResponse(int actorId, int playerCount, bool initRoom, CmuneRoomID roomId)
        {
            JoinResponse response = new JoinResponse()
            {
                ActorNr = actorId,
                PlayerCount = playerCount,
                ServerTicks = DateTime.Now.Ticks,
                InitializeRoom = initRoom,
                RoomID = roomId.GetBytes()
            };

            return new OperationResponse(CmuneOperationCodes.PhotonGameJoin, response);
        }

        [DataMember(Code = ParameterKeys.Bytes)]
        public byte[] Bytes { get; protected set; }

        [DataMember(Code = ParameterKeys.AccessLevel)]
        public int AccessLevel { get; protected set; }

        [DataMember(Code = ParameterKeys.Cmid)]
        public int Cmid { get; protected set; }

        public RoomMetaData Room { get; protected set; }

        ///// <summary>
        ///// <i>Return Value</i> Actor number for the joined player.
        ///// </summary>
        //[ResponseParameter(Code = (byte)ParameterKeys.ActorNr)]
        //public int ActorNr { get; set; }

        ///// <summary>
        ///// <i>Return Value</i> Actor number for the joined player.
        ///// </summary>
        //[ResponseParameter(Code = (byte)ParameterKeys.ServerTicks)]
        //public long ServerTicks { get; set; }

        ///// <summary>
        ///// <i>Return Value</i> Actor number for the joined player.
        ///// </summary>
        //[ResponseParameter(Code = (byte)ParameterKeys.InitRoom)]
        //public bool InitializeRoom { get; set; }

        ///// <summary>
        ///// <i>Return Value</i> Actor number for the joined player.
        ///// </summary>
        //[ResponseParameter(Code = (byte)ParameterKeys.GameId)]
        //public byte[] RoomID { get; set; }

        ///// <summary>
        ///// <i>Return Value</i> Actor number for the joined player.
        ///// </summary>
        //[ResponseParameter(Code = (byte)ParameterKeys.Actors)]
        //public int PlayerCount { get; set; }
    }
}
