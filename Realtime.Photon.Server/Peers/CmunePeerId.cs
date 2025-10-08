
namespace Cmune.Realtime.Photon.Server
{
    public static class CmunePeerId
    {
        private static int _peerId = 0;
        private static object _lock = new object();

        public static int Next
        {
            get
            {
                lock (_lock)
                {
                    if (_peerId == int.MaxValue) _peerId = 0;
                    return ++_peerId;
                }
            }
        }
    }
}
