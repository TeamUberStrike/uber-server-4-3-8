
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// This class implements the Join operation.'
    /// </summary>
    public class GameListRemovalOperation : Operation
    {
        public GameListRemovalOperation(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            //PopulateParameters();

            if (Bytes != null)
            {
                DeletedRooms = (List<CmuneRoomID>)RealtimeSerialization.ToObject(Bytes);
            }
            else
            {
                DeletedRooms = new List<CmuneRoomID>();
            }
        }

        [DataMember(Code = ParameterKeys.LobbyRoomDelete, IsOptional = true)]
        public byte[] Bytes { get; protected set; }

        public List<CmuneRoomID> DeletedRooms { get; protected set; }
    }
}
