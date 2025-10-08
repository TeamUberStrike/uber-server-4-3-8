using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class Int64Proxy
    {
        public static void Serialize(Stream bytes, long instance)
        {
            var array = BitConverter.GetBytes(instance);
            bytes.Write(array, 0, array.Length);
        }

        public static long Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[8];
            bytes.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}