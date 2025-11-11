using System;

namespace UberStrike.Realtime.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class PeerOperationsAttribute : Attribute
    {
        public PeerOperationsAttribute() { }
    }
}