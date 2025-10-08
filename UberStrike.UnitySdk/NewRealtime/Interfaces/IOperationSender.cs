using System;
using System.Collections.Generic;

namespace UberStrike.Realtime.Client
{
    public interface IOperationSender
    {
        event Func<byte, Dictionary<byte, object>, bool, bool> SendOperation;
    }
}