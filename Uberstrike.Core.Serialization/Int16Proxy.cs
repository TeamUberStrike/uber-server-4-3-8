using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class Int16Proxy
    {
        public static void Serialize(Stream bytes, short instance)
        {
            var array = BitConverter.GetBytes(instance);
            bytes.Write(array, 0, array.Length);
        }

        public static short Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[2];
            bytes.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }
    }
}