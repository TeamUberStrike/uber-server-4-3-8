using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class Int32Proxy
    {
        public static void Serialize(Stream bytes, int instance)
        {
            var array = BitConverter.GetBytes(instance);
            bytes.Write(array, 0, array.Length);
        }

        public static int Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[4];
            bytes.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}