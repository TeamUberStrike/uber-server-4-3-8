using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class UShortProxy
    {
        public static void Serialize(Stream bytes, ushort instance)
        {
            var array = BitConverter.GetBytes(instance);
            bytes.Write(array, 0, array.Length);
        }

        public static ushort Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[2];
            bytes.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }
    }
}