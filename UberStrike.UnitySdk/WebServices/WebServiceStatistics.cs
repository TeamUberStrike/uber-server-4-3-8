using System;
using System.Collections.Generic;
using Cmune.Util;

namespace UberStrike.WebService.Unity
{
    public static class WebServiceStatistics
    {
        public static bool IsEnabled = true;
        public static readonly Dictionary<string, Statistics> Data = new Dictionary<string, Statistics>();

        public static long TotalBytesIn { get; private set; }
        public static long TotalBytesOut { get; private set; }

        public static void RecordWebServiceBegin(string method, int bytes)
        {
            Statistics stats = GetStatistics(method);

            stats.Counter++;
            stats.OutgoingBytes += bytes;
            TotalBytesOut += bytes;
            stats.LastCall = DateTime.Now;
        }

        public static void RecordWebServiceEnd(string method, int bytes, bool success)
        {
            Statistics stats = GetStatistics(method);

            stats.IncomingBytes += bytes;
            TotalBytesIn += bytes;
            if (!success) stats.FailCounter++;
            stats.Time = (float)DateTime.Now.Subtract(stats.LastCall).TotalSeconds;
        }

        private static Statistics GetStatistics(string method)
        {
            Statistics stats;
            if (!Data.TryGetValue(method, out stats))
            {
                stats = new Statistics();
                Data[method] = stats;
            }
            return stats;
        }

        public class Statistics
        {
            public Statistics()
            {
                LastCall = DateTime.Now;
            }

            public int Counter { get; set; }
            public int IncomingBytes { get; set; }
            public int OutgoingBytes { get; set; }
            public int FailCounter { get; set; }
            public float Time { get; set; }
            internal DateTime LastCall { get; set; }

            public override string ToString()
            {
                return string.Format("\tcount:{0}({1}) | time:{2:N2} | data:{3:F0}/{4:F0}", Counter, FailCounter, Time, ConvertBytes.ToKiloBytes(IncomingBytes), ConvertBytes.ToKiloBytes(OutgoingBytes));
            }
        }
    }
}
