using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class DateTimeProxy
    {
        public static void Serialize(Stream bytes, DateTime instance)
        {
            var array = BitConverter.GetBytes(instance.Ticks);
            bytes.Write(array, 0, array.Length);
        }

        public static DateTime Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[8];
            bytes.Read(buffer, 0, 8);
            return new DateTime(BitConverter.ToInt64(buffer, 0));
        }
    }
}