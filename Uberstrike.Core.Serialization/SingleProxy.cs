using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class SingleProxy
    {
        public static void Serialize(Stream bytes, float instance)
        {
            var array = BitConverter.GetBytes(instance);
            bytes.Write(array, 0, array.Length);
        }

        public static float Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[4];
            bytes.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }
    }
}