
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
    public class JoinResponse
    {
        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.ActorNr)]
        public int ActorNr { get; set; }

        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.ServerTicks)]
        public long ServerTicks { get; set; }

        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.InitRoom)]
        public bool InitializeRoom { get; set; }

        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.GameId)]
        public byte[] RoomID { get; set; }

        /// <summary>
        /// <i>Return Value</i> Actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKeys.Actors)]
        public int PlayerCount { get; set; }
    }
}
