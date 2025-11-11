using System;

namespace Cmune.Realtime.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NetworkClassAttribute : Attribute, IAttributeID<short>
    {
        public NetworkClassAttribute(short id)
        {
            _id = id;
        }

        public bool HasID
        {
            get { return _id.HasValue; }
        }

        public short ID
        {
            get { return _id.HasValue ? _id.Value : (short)-1; }
        }

        private short? _id;
    }
}
