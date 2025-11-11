
using Cmune.Realtime.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class QueryServerLoadResponse
    {
        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.Bytes)]
        public byte[] Bytes
        {
            get;
            set;
        }
    }
}
