using System;

namespace UberStrike.Realtime.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RoomEventsAttribute : Attribute
    {
        public RoomEventsAttribute() { }
    }
}
