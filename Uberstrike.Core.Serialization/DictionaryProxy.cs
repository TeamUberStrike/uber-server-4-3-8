using System.Collections.Generic;
using System.IO;

namespace UberStrike.Core.Serialization
{
    public static class DictionaryProxy<S, T>
    {
        public delegate void Serializer<U>(Stream stream, U instance);
        public delegate U Deserializer<U>(Stream stream);

        public static void Serialize(Stream bytes, Dictionary<S, T> instance, Serializer<S> keySerialization, Serializer<T> valueSerialization)
        {
            Int32Proxy.Serialize(bytes, instance.Count);
            foreach (var element in instance)
            {
                keySerialization(bytes, element.Key);
                valueSerialization(bytes, element.Value);
            }
        }

        public static Dictionary<S, T> Deserialize(Stream bytes, Deserializer<S> keySerialization, Deserializer<T> valueSerialization)
        {
            int count = Int32Proxy.Deserialize(bytes);
            Dictionary<S, T> list = new Dictionary<S, T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(keySerialization(bytes), valueSerialization(bytes));
            }
            return list;
        }
    }
}