
using System;
namespace Cmune.Realtime.Common.Security
{
    public class SecureMemoryMonitor
    {
        public static readonly SecureMemoryMonitor Instance = new SecureMemoryMonitor();

        private SecureMemoryMonitor() { }

        public void PerformCheck()
        {
            if (_sender != null)
                _sender();
        }

        private event Action _sender;
        internal event Action AddToMonitor
        {
            add { _sender += value; }
            remove { _sender -= value; }
        }
    }
}