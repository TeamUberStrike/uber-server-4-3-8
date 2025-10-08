using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class ListProxy<T>
    {
        public delegate void Serializer<U>(Stream stream, U instance);
        public delegate U Deserializer<U>(Stream stream);

        public static void Serialize(Stream bytes, ICollection<T> instance, Serializer<T> serialization)
        {
            UShortProxy.Serialize(bytes, (ushort)instance.Count);
            foreach (var element in instance)
            {
                serialization(bytes, element);
            }
        }

        public static List<T> Deserialize(Stream bytes, Deserializer<T> serialization)
        {
            ushort count = UShortProxy.Deserialize(bytes);
            List<T> list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(serialization(bytes));
            }
            return list;
        }
    }
}
