
using System;
namespace Cmune.Realtime.Common.Security
{
    public class SecureMemoryMonitor
    {
        public static readonly SecureMemoryMonitor Instance = new SecureMemoryMonitor();

        private SecureMemoryMonitor() { }

        public void PerformCheck()
        {
            if (AddToMonitor != null)
                AddToMonitor();
        }

        internal event Action AddToMonitor;
    }
}