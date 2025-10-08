using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using Photon.SocketServer;
using UberStrike.Realtime.Server;
using UberStrike.Realtime.CommServer;

namespace UberStrike.Realtime.Photon.CommServer
{
    public static class SecurityManager
    {
        private const int ReportDateInterval = 6;
        private const int ShortBanHours = 6;
        private const int LongBanHours = 24;

        public enum ReportType
        {
            HumanReport,
            SpeedHack,
            MemoryHack,
        }

        public static readonly Dictionary<int, List<PlayerReport>> PlayerReports = new Dictionary<int, List<PlayerReport>>();

        private static Dictionary<int, DateTime> _bannedCmids = new Dictionary<int, DateTime>();

        public static IEnumerable<int> BannedCmids { get { return _bannedCmids.Keys; } }

        public static void ReportPlayer(int reporterCmid, ICmunePeer peer, ReportType type, string comment = "")
        {
            if (!PlayerReports.ContainsKey(peer.Cmid))
                PlayerReports[peer.Cmid] = new List<PlayerReport>();

            //remove old reports of the same player
            if (reporterCmid > 0)
                PlayerReports[peer.Cmid].RemoveAll(r => r.Cmid == reporterCmid);
            PlayerReports[peer.Cmid].Add(new PlayerReport()
            {
                Cmid = reporterCmid,
                IpAddress = peer.RemoteIP,
                Type = type,
                Info = comment,
            });

            EvaluatePlayerReportsForPlayer(peer, PlayerReports[peer.Cmid]);
        }

        public static void ReportPlayer(int reporterCmid, int cmid, ReportType type, string comment = "")
        {
            if (!PlayerReports.ContainsKey(cmid))
                PlayerReports[cmid] = new List<PlayerReport>();

            //remove old reports of the same player
            PlayerReports[cmid].RemoveAll(r => r.Cmid == reporterCmid);
            PlayerReports[cmid].Add(new PlayerReport() { Cmid = reporterCmid, IpAddress = string.Empty, Type = type, Info = comment });
        }

        private static void EvaluatePlayerReportsForPlayer(ICmunePeer peer, List<PlayerReport> reports)
        {
            //speedhacker
            if (reports.Exists(r => r.Type == ReportType.SpeedHack && WithinHours(r.Date, ReportDateInterval)))
            {
                _bannedCmids[peer.Cmid] = DateTime.Now.AddHours(ShortBanHours);
                //_bannedIpAddresses[peer.RemoteIP] = DateTime.Now.AddHours(ShortBanHours);
                DisconnectPeer(peer);
            }
            else if (reports.Count(r => WithinHours(r.Date, ReportDateInterval)) > 3)
            {
                _bannedCmids[peer.Cmid] = DateTime.Now.AddHours(ShortBanHours);
                DisconnectPeer(peer);
            }
        }

        private static bool WithinHours(DateTime dateTime, int interval)
        {
            return DateTime.Now.Subtract(dateTime).TotalHours < interval;
        }

        public static void BanPlayerByCmid(int cmid)
        {
            _bannedCmids[cmid] = DateTime.Now.AddHours(LongBanHours);
        }

        public static bool IsPeerBanned(CmunePeer peer)
        {
            DateTime date;
            if (_bannedCmids.TryGetValue(peer.Cmid, out date))
            {
                if (date > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    UnbanPlayer(peer.Cmid);
                }
            }

            return false;
        }

        public static void DisconnectPeer(ICmunePeer peer)
        {
            var f = new LobbyRoomEventFactory(null);
            var ev = f.SendDisconnectAndDisablePhoton("You were disconnected! Please restart UberStrike to log in again.");
            peer.PublishEvent(ev.Data);
        }

        public static void UnbanPlayer(int cmid)
        {
            _bannedCmids.Remove(cmid);
            PlayerReports.Remove(cmid);
        }

        public class PlayerReport
        {
            public PlayerReport()
            {
                Date = DateTime.Now;
            }

            public DateTime Date { get; private set; }
            public ReportType Type { get; set; }
            public string Info { get; set; }
            public string IpAddress { get; set; }
            public int Cmid { get; set; }
        }
    }
}