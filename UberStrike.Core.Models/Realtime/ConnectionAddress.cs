
namespace UberStrike.Core.Models
{
    [System.Serializable]
    public class ConnectionAddress
    {
        public int Ipv4 { get; set; }
        public short Port { get; set; }

        public string ConnectionString { get { return string.Format("{0}:{1}", ToString(Ipv4), Port); } }

        public static string ToString(int ipv4)
        {
            return string.Format("{0}.{1}.{2}.{3}", (ipv4 >> 24) & 0xFF, (ipv4 >> 16) & 0xFF, (ipv4 >> 8) & 0xFF, ipv4 & 0xFF);

        }

        public static int ToInteger(string ipAddress)
        {
            int ipv4 = 0;
            var token = ipAddress.Split('.');
            if (token.Length == 4)
            {
                for (int i = 0; i < token.Length; i++)
                    ipv4 |= int.Parse(token[i]) << ((3 - i) * 8);
            }
            return ipv4;
        }
    }
}