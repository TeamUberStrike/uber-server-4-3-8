using System;
using Cmune.Realtime.Common;

namespace Cmune.Realtime.Photon.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RoomMessageAttribute : Attribute, IAttributeID<int>
    {
        public RoomMessageAttribute(int id)
        {
            _id = id;
        }

        public bool HasID
        {
            get { return _id.HasValue; }
        }

        public int ID
        {
            get { return _id.HasValue ? _id.Value : (byte)0; }
        }

        private int? _id;
    }

}
