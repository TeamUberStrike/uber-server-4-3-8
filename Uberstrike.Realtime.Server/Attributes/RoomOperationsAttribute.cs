using System;

namespace UberStrike.Realtime.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RoomOperationsAttribute : Attribute
    {
        public RoomOperationsAttribute() { }
    }
}
