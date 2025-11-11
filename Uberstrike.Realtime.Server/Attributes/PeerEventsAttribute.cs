using System;

namespace UberStrike.Realtime.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class PeerEventsAttribute : Attribute
    {
        public PeerEventsAttribute() { }
    }
}
