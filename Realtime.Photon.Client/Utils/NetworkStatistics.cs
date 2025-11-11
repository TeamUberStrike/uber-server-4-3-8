using System.Collections.Generic;
using Cmune.Realtime.Common;

namespace Cmune.Realtime.Photon.Client.Network.Utils
{
    public static class NetworkStatistics
    {
        public static bool IsEnabled = true;

        public static readonly Dictionary<string, Statistics> Incoming = new Dictionary<string, Statistics>();
        public static readonly Dictionary<string, Statistics> Outgoing = new Dictionary<string, Statistics>();

        public static long TotalBytesIn { get; private set; }
        public static long TotalBytesOut { get; private set; }

        public static void RecordOutgoingCall(string method, int bytes)
        {
            Statistics stats = GetStatistics(Outgoing, method);

            stats.Counter++;
            TotalBytesOut += bytes;
            stats.Bytes += bytes;
        }

        public static void RecordIncomingCall(string method, int bytes)
        {
            Statistics stats = GetStatistics(Incoming, method);

            stats.Counter++;
            TotalBytesIn += bytes;
            stats.Bytes += bytes;
        }

        private static Statistics GetStatistics(Dictionary<string, Statistics> dict, string method)
        {
            Statistics stats;
            if (!dict.TryGetValue(method, out stats))
            {
                stats = new Statistics();
                dict[method] = stats;
            }
            return stats;
        }

        public class Statistics
        {
            public int Counter { get; set; }
            public int Bytes { get; set; }

            public override string ToString()
            {
                return string.Format("\tcount:{0} | bytes:{1}", Counter, Bytes);
            }
        }

        internal static void RecordOutgoingCall(ExitGames.Client.Photon.OperationRequest request)
        {
            object instanceId, methodId, bytes;
            if (request.Parameters.TryGetValue(ParameterKeys.InstanceId, out instanceId) &&
                request.Parameters.TryGetValue(ParameterKeys.MethodId, out methodId) &&
                request.Parameters.TryGetValue(ParameterKeys.Bytes, out bytes))
            {
                RecordOutgoingCall(instanceId + " " + methodId, ((byte[])bytes).Length);
            }
        }
    }
}