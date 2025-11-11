using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using ExitGames.Concurrency.Fibers;
using System.Globalization;
using System.Management;

namespace Cmune.Realtime.Photon.Server.Diagnostics
{
    public class CmunePerformanceCounter
    {
        private CmunePerformanceCounter()
        {
            ExecutionFiber = new PoolFiber();
            _currentLoad = new ServerLoadData();

            _counter = new Dictionary<PerfCounter, PerformanceCounter>();

            _counter.Add(PerfCounter.MemoryAvailable, new PerformanceCounter("Memory", "Available MBytes"));
            _counter.Add(PerfCounter.ProcessorTime, new PerformanceCounter("Processor", "% Processor Time", "_Total", true));
            _counter.Add(PerfCounter.DotNetCurrentThreads, new PerformanceCounter(".Net CLR LocksAndThreads", "# of current logical Threads", ProcessData.Name, true));
            _counter.Add(PerfCounter.DotNetExceptions, new PerformanceCounter(".Net CLR Exceptions", "# of Exceps Thrown", ProcessData.Name, true));
            _counter.Add(PerfCounter.DotNetBytesInAllHeaps, new PerformanceCounter(".Net CLR Memory", "# Bytes in all Heaps", ProcessData.Name, true));
            _counter.Add(PerfCounter.DotNetLOH, new PerformanceCounter(".Net CLR Memory", "Large Object Heap size", ProcessData.Name, true));


            _currentLoad.MaxPlayerCount = ServerSettings.MaxPlayerCount;

            if (PerformanceCounterCategory.Exists("Photon Socket Server"))
            {
                CmuneDebug.LogInfo("Photon Socket Server Counters installed: TRUE");
                try
                {
                    _counter.Add(PerfCounter.PhotonPeers, new PerformanceCounter("Photon Socket Server", "Peers", ServerSettings.InstanceName, true));
                }
                catch (System.Exception e)
                {
                    CmuneDebug.LogError("Failed getting PerfCounter PhotonSocketServer.Peers with Message: " + e.Message);
                }
            }
            else
            {
                CmuneDebug.LogError("Photon Socket Server Counters installed: FALSE");
            }

            ExecutionFiber.Start();
        }

        public static CmunePerformanceCounter Instance
        {
            get
            {
                if (_instance == null) _instance = new CmunePerformanceCounter();
                return _instance;
            }
        }

        public bool IsPerformanceCounterStarted { get; private set; }
        public bool IsMonitorInstanceStarted { get; private set; }

        //run routine to gather performance counters
        public void StartPerformanceCounter()
        {
            if (!IsPerformanceCounterStarted)
            {
                IsPerformanceCounterStarted = true;
                ExecutionFiber.ScheduleOnInterval(UpdateCounters, 2000, 2000);
            }
        }

        //send staistics to the Instrumentation box every 15 minutes
        public void StartMonitorInstance()
        {
            if (!IsMonitorInstanceStarted)
            {
                IsMonitorInstanceStarted = true;
                ExecutionFiber.ScheduleOnInterval(SendInstanceState, 1 * 1 * 1000, 15 * 60 * 1000);
            }
        }

        private void UpdateCounters()
        {
            //_currentLoad.PeersConnected = (int)Counter.Players.GetNextValue();
            _currentLoad.PlayersConnected = (int)Counter.Players.GetNextValue();
            _currentLoad.RoomsCreated = (int)Counter.Games.GetNextValue();
        }

        public List<float> GetCounterSnapshot()
        {
            List<float> allCounters = new List<float>(9);
            try
            {
                allCounters.Add(Counter.Players.GetNextValue());
                allCounters.Add(Counter.Games.GetNextValue());
                allCounters.Add(ReadCounter(PerfCounter.PhotonPeers));
                allCounters.Add(ReadCounter(PerfCounter.ProcessorTime));
                allCounters.Add(ReadCounter(PerfCounter.MemoryAvailable));
                allCounters.Add(ReadCounter(PerfCounter.ProcessPrivateBytes));
                allCounters.Add(ReadCounter(PerfCounter.DotNetBytesInAllHeaps));
                allCounters.Add(ReadCounter(PerfCounter.DotNetCurrentThreads));
                allCounters.Add(ReadCounter(PerfCounter.DotNetExceptions));
            }
            catch (System.Exception e)
            {
                CmuneDebug.LogError("Error getting PerfCounters with Message: ", e.Message);
            }
            return allCounters;
        }

        public Dictionary<int, float> GetNewCounterSnapshot()
        {
            int pos = 0;
            Dictionary<int, float> allCounters = new Dictionary<int, float>(20);
            try
            {
                pos++; ReadCounter(PerfCounter.PhotonPeers, allCounters);

                pos++; ReadCounter(PerfCounter.CountPlayers, allCounters);
                pos++; ReadCounter(PerfCounter.CountGames, allCounters);

                pos++; ReadCounter(PerfCounter.ProcessorTime, allCounters);
                pos++; ReadCounter(PerfCounter.ProcessRuntime, allCounters);
                pos++; ReadCounter(PerfCounter.ProcessPrivateBytes, allCounters);
                pos++; ReadCounter(PerfCounter.MemoryAvailable, allCounters);
                pos++; ReadCounter(PerfCounter.VirtualBytes, allCounters);

                pos++; ReadCounter(PerfCounter.DotNetBytesInAllHeaps, allCounters);
                pos++; ReadCounter(PerfCounter.DotNetCurrentThreads, allCounters);
                pos++; ReadCounter(PerfCounter.DotNetExceptions, allCounters);
                pos++; ReadCounter(PerfCounter.DotNetLOH, allCounters);
            }
            catch (System.Exception e)
            {
                CmuneDebug.LogError("Error getting PerfCounters at {0} with Message: {1}", pos, e.Message);
            }
            return allCounters;
        }

        public void ReadCounter(PerfCounter c, Dictionary<int, float> d)
        {
            if (!d.ContainsKey((int)c))
                d.Add((int)c, ReadCounter(c));
            else
                CmuneDebug.LogError("PerfCounter of Type {0} is already present in Dictionary", c);
        }

        public float ReadCounter(PerfCounter c)
        {
            switch (c)
            {
                case PerfCounter.CountPlayers: return Counter.Players.GetNextValue();
                case PerfCounter.CountGames: return Counter.Games.GetNextValue();

                case PerfCounter.ProcessPrivateBytes: return ProcessData.PrivateMemory;
                case PerfCounter.ProcessRuntime: return (float)DateTime.Now.Subtract(ProcessData.StartTime).TotalSeconds;
                case PerfCounter.VirtualBytes: return ProcessData.VirtualMemory;

                case PerfCounter.TotalPhysicalMemory: return GetPhysicalMemory();

                default:
                    {
                        PerformanceCounter counter;
                        if (_counter.TryGetValue(c, out counter) && counter != null)
                        {
                            return counter.NextValue();
                        }
                        else
                        {
                            return 0;
                        }
                    }
            }
        }

        public ServerLoadData CurrentLoad
        {
            get
            {
                return _currentLoad;
            }
        }

        /// <summary>
        /// Gets total physical memory
        /// </summary>
        /// <returns></returns>
        public static float GetPhysicalMemory()
        {
            try
            {
                UInt64 bytes = 0;
                ObjectQuery winQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);
                var collection = searcher.Get();
                foreach (ManagementObject item in collection)
                {
                    bytes += Convert.ToUInt64(item.Properties["Capacity"].Value);
                }

                return ConvertBytes.ToMegaBytes(bytes);
            }
            catch (Exception e)
            {
                CmuneDebug.Log("GetPhysicalMemory failed with Exception: " + e.Message);
            }

            return 0;
        }

        #region Fields

        private ServerLoadData _currentLoad;
        public PoolFiber ExecutionFiber { get; private set; }
        private Dictionary<PerfCounter, PerformanceCounter> _counter;
        private static CmunePerformanceCounter _instance;

        #endregion

        public void SendInstanceState()
        {
            try
            {
                string url = ServerSettings.InstrumentationWsBaseUrl + "Cmune.svc/Application/SetInstanceState";
                var request = new Cmune.DataCenter.Utils.WebGetRequest(url);
                request.Add("versionNumber", ServerSettings.AppVersion);
                request.Add("instanceName", ServerSettings.NickName);
                request.Add("ip", ServerSettings.IP);
                request.Add("port", ServerSettings.Port.ToString());
                request.Add("cpuUtilization", ReadCounter(PerfCounter.ProcessorTime).ToString("F0", CultureInfo.InvariantCulture));
                request.Add("ramUtilization", ReadCounter(PerfCounter.ProcessPrivateBytes).ToString("F0", CultureInfo.InvariantCulture));
                request.Add("instanceRamMb", ReadCounter(PerfCounter.TotalPhysicalMemory).ToString("F0", CultureInfo.InvariantCulture));
                request.Add("peerCount", ReadCounter(PerfCounter.CountPlayers).ToString("F0", CultureInfo.InvariantCulture));
                request.GetResponse();
            }
            catch (Exception e)
            {
                CmuneDebug.LogError("SendInstanceState failed with error: " + e.Message);
            }
        }
    }
}