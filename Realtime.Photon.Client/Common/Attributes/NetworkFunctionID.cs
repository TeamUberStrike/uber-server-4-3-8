using System;

namespace Cmune.Realtime.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NetworkMethodAttribute : Attribute, IAttributeID<byte>
    {
        public NetworkMethodAttribute()
        {
        }

        public NetworkMethodAttribute(byte id)
        {
            _id = id;
        }

        public bool HasID
        {
            get { return _id.HasValue; }
        }

        public byte ID
        {
            get { return _id.HasValue ? _id.Value : (byte)0; }
        }

        private byte? _id;
    }

}
