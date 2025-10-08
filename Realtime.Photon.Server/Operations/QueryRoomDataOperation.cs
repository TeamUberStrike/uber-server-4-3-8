
//using Cmune.Realtime.Common;
//using Photon.SocketServer;
//using Photon.SocketServer.Rpc;

//namespace Cmune.Realtime.Photon.Server
//{
//    public class QueryRoomDataOperation : Operation
//    {
//        public QueryRoomDataOperation(OperationRequest operationRequest)
//            : base(operationRequest)
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        [RequestParameter(Code = ParameterKeys.Data)]
//        public int RoomNumber { get; protected set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        [ResponseParameter(Code = (byte)ParameterKeys.Bytes)]
//        public byte[] RoomData { get; set; }
//    }
//}
