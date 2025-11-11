using System.IO;
using System.Text;

namespace UberStrike.Core.Serialization
{
    public static class StringProxy
    {
        public static void Serialize(Stream bytes, string instance)
        {
            if (string.IsNullOrEmpty(instance))
            {
                UShortProxy.Serialize(bytes, 0);
            }
            else
            {
                UShortProxy.Serialize(bytes, (ushort)instance.Length);
                var array = Encoding.Unicode.GetBytes(instance);
                bytes.Write(array, 0, array.Length);
            }
        }

        public static string Deserialize(Stream bytes)
        {
            ushort len = UShortProxy.Deserialize(bytes);
            if (len > 0)
            {
                byte[] buffer = new byte[len * 2];
                bytes.Read(buffer, 0, buffer.Length);
                return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}