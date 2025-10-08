
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class GameListUpdateOperation : Operation
    {
        public GameListUpdateOperation(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            //PopulateParameters();

            if (Bytes != null)
            {
                UpdatedRooms = (List<RoomMetaData>)RealtimeSerialization.ToObject(Bytes);
            }
            else
            {
                UpdatedRooms = new List<RoomMetaData>();
            }
        }

        [DataMember(Code = ParameterKeys.LobbyRoomUpdate, IsOptional = true)]
        public byte[] Bytes { get; protected set; }

        public List<RoomMetaData> UpdatedRooms { get; protected set; }

    }
}
