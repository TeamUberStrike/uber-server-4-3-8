
namespace Cmune.Realtime.Common.Utils
{
    public static class GuidFactory
    {
        public static System.Guid Create()
        {
            byte[] data = new byte[16];  // 16 bytes = 128 bits
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(data);
            return new System.Guid(data);
        }
    }
}
