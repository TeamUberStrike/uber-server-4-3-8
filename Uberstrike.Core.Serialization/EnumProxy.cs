using System;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class EnumProxy<T>
    {
        public static void Serialize(Stream bytes, T instance)
        {
            var array = BitConverter.GetBytes(Convert.ToInt32(instance));
            bytes.Write(array, 0, array.Length);
        }

        public static T Deserialize(Stream bytes)
        {
            byte[] buffer = new byte[4];
            bytes.Read(buffer, 0, 4);
            return (T)Enum.ToObject(typeof(T), BitConverter.ToInt32(buffer, 0));
        }
    }
}