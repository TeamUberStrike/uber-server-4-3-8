using System;
using System.IO;

namespace Cmune.DataCenter.Common.Utils
{
    public static class ProtocolBuffers
    {
        /// <summary>
        /// Serialize a message (includes calling ProtoBuff)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeMessage<T>(T value)
        {
            string str = string.Empty;

            using (var ret = new MemoryStream())
            {
                if (value != null)
                {
                    //Serializer.Serialize(ret, value);
                }

                str = Convert.ToBase64String(ret.ToArray());
            }

            return str;
        }

        /// <summary>
        /// Deserialize a message (to use before calling ProtoBuff)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Stream DeserializeMessage(string message)
        {
            return new MemoryStream(Convert.FromBase64String(message));
        }
    }
}