using System.IO;

namespace UberStrike.Core.Serialization.Utils
{
    public class DeltaCompression
    {
        public static byte[] Deflate(byte[] baseData, byte[] newData)
        {
            using (var stream = new MemoryStream())
            {
                byte count = 0;
                for (int i = 0; i < newData.Length; i++)
                {
                    if (i < baseData.Length)
                    {
                        if (baseData[i] == newData[i])
                        {
                            count++;
                        }
                        else
                        {
                            stream.WriteByte(count);
                            stream.WriteByte(newData[i]);
                            count = 0;
                        }
                    }
                    else
                    {
                        stream.WriteByte(newData[i]);
                    }
                }

                return stream.ToArray();
            }
        }

        public static byte[] Inflate(byte[] baseData, byte[] delta)
        {
            if (delta.Length == 0)
            {
                return baseData;
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    int k = 0;
                    for (int i = 0; i < delta.Length; )
                    {
                        if (k < baseData.Length)
                        {
                            for (int j = 0; j < delta[i]; j++, k++)
                                stream.WriteByte(baseData[k]);
                            stream.WriteByte(delta[i + 1]);
                            k++;
                            i += 2;
                        }
                        else
                        {
                            stream.WriteByte(delta[i]);
                            i++;
                        }
                    }
                    return stream.ToArray();
                }
            }
        }
    }
}