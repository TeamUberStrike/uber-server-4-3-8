using System;
using System.Collections.Generic;

namespace UberStrike.Realtime.Client
{
    public interface IEventDispatcher
    {
        void OnEvent(byte id, byte[] data);
    }
}