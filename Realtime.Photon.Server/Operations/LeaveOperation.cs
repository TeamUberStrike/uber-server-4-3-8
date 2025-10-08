
using Cmune.Realtime.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// This class implements the Leave operation.
    /// </summary>
    public class LeaveOperation : Operation
    {
        public LeaveOperation(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            try
            {
                GameId = new CmuneRoomID(Bytes);
            }
            catch (System.NullReferenceException ex)
            {
                CmuneDebug.Exception("PhotonGameLeave Failed because: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        [DataMember(Code = ParameterKeys.RoomId)]
        public byte[] Bytes { get; protected set; }

        public CmuneRoomID GameId { get; protected set; }
    }
}
