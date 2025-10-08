using System;
using System.Diagnostics;

namespace Cmune.Realtime.Common.Utils
{
    public static class ProcessData
    {
        private const float ByteToMb = 1048576f;

        public static float PhysicalMemory
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.WorkingSet64 / ByteToMb;
            }
        }

        public static float VirtualMemory
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.VirtualMemorySize64 / ByteToMb;
            }
        }

        public static string Name
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.ProcessName;
            }
        }

        public static float PrivateMemory
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.PrivateMemorySize64 / ByteToMb;
            }
        }

        public static int ID
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.Id;
            }
        }

        public static float ProcessorTime
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return (float)p.TotalProcessorTime.TotalSeconds;
            }
        }

        public static DateTime StartTime
        {
            get
            {
                Process p = System.Diagnostics.Process.GetCurrentProcess();
                return p.StartTime;
            }
        }
    }
}
