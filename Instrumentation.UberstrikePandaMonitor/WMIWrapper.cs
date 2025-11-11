using System;
using System.Management;

namespace Instrumentation.Monitoring.ServerMonitor
{
    public class WMIWrapper
    {
        private int byteToMByte(long bytes)
        {
            return (int) (bytes/ (1024 * 1024));
        }

        private int byteToKByte(long bytes)
        {
            return (int)(bytes / (1024));
        }

        private int bitToMbit(long bits)
        {
            return (int) (bits / (1000000));
        }

        public ManagementScope Scope { get; set; }

        public WMIWrapper(ManagementScope scope)
        {
            Scope = scope;
        }

        public int GetPercentProcessUsed()
        {
            int cpuUsage = 0;

            ObjectQuery query = new ObjectQuery("SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                cpuUsage = (Int32.Parse(m["PercentProcessorTime"].ToString()));
            }
            return cpuUsage;
        }

        public int GetPercentMemoryUsed()
        {
            int memoryCapacity = 0;
            int availaibleMemory = 0;

            ObjectQuery memoryCapacityRequest = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectCollection queryCollection = new ManagementObjectSearcher(Scope, memoryCapacityRequest).Get();
            foreach (ManagementObject m in queryCollection)
            {
                memoryCapacity += byteToMByte(Int64.Parse(m["Capacity"].ToString()));
            }


            ObjectQuery availableMBytesRequest = new ObjectQuery("SELECT AvailableMBytes FROM Win32_PerfFormattedData_PerfOS_Memory");
            ManagementObjectCollection queryCollection2 = new ManagementObjectSearcher(Scope, availableMBytesRequest).Get();
            foreach (ManagementObject m in queryCollection2)
            {
                availaibleMemory = (Int32.Parse(m["AvailableMBytes"].ToString()));
            }

            return (int)Math.Round(100 * (((decimal)memoryCapacity - availaibleMemory) / (decimal)memoryCapacity));
        }

        public int GetSystemDiskSpaceUsage()
        {
            int diskSpaceUsage = 0;

            ObjectQuery query = new ObjectQuery("SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                if (m["DeviceID"].ToString() == "C:")
                {
                    var totalSpace = long.Parse(m["Size"].ToString());
                    var occupiedSpace = totalSpace - long.Parse(m["FreeSpace"].ToString());
                    diskSpaceUsage = (int)Math.Round(100 * (((decimal)occupiedSpace) / (decimal) totalSpace));
                }
                
            }
            return diskSpaceUsage;
        }

        public int GetBandwidthUsage()
        {
            int bandWidthUsage = 0;

            ObjectQuery query = new ObjectQuery("SELECT BytesTotalPersec FROM Win32_PerfFormattedData_Tcpip_NetworkInterface");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                if (m["BytesTotalPersec"] != null)
                {
                    bandWidthUsage += byteToKByte(int.Parse(m["BytesTotalPersec"].ToString()));
                }
            }

            return bandWidthUsage;
        }
    }
}
