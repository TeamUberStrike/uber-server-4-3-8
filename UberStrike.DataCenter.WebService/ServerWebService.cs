using UberStrike.DataCenter.Utils;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class ServerWebService : IServerWebService
    {
        public bool InvalidateCache(string cacheName, string secret)
        {
            return UberStrikeCacheInvalidation.InvalidateLocalCache(cacheName, secret);
        }

        public string IsAlive()
        {
            return "yes";
        }
    }
}
