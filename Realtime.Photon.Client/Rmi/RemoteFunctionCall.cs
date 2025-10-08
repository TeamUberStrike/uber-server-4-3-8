using System;
using System.Collections.Generic;

namespace Cmune.Realtime.Photon.Client
{
    public class RemoteFunctionCall
    {
        public byte LocalAddress;
        public object[] Arguments;
        public DateTime Time;

        public RemoteFunctionCall(byte functionAddress, params object[] args)
        {
            LocalAddress = functionAddress;
            Arguments = new List<object>(args).ToArray();
            Time = DateTime.Now;
        }
    }
}
