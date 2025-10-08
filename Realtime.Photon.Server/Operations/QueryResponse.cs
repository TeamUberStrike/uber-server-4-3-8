using Cmune.Realtime.Common;
using Photon.SocketServer.Rpc;

namespace Cmune.Realtime.Photon.Server
{
    public class QueryResponse
    {
        public QueryResponse(short invocId)
        {
            InvocationId = invocId;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.Data)]
        public byte[] Bytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.InvocationId)]
        public short InvocationId { get; set; }
    }
}
