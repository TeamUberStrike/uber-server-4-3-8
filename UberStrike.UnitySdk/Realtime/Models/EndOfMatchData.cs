using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;

namespace UberStrike.Realtime.Common
{
    [System.Serializable]
    public class EndOfMatchData : IByteArray
    {
        public int RoundNumber { get; set; }

        public List<StatsSummary> MostValuablePlayers { get; set; }
        public int MostEffecientWeaponId { get; set; }

        public StatsCollection PlayerStatsTotal { get; set; }
        public StatsCollection PlayerStatsBestPerLife { get; set; }
        public Dictionary<byte, ushort> PlayerXpEarned { get; set; }

        public EndOfMatchData()
        { }

        public EndOfMatchData(byte[] bytes, ref int idx)
        {
            idx = FromBytes(bytes, idx);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            DefaultByteConverter.FromInt(MostEffecientWeaponId, ref bytes);
            DefaultByteConverter.FromByte((byte)MostValuablePlayers.Count, ref bytes);
            foreach (var mvp in MostValuablePlayers)
            {
                DefaultByteConverter.FromString(mvp.Name, ref bytes);
                DefaultByteConverter.FromInt(mvp.Kills, ref bytes);
                DefaultByteConverter.FromInt(mvp.Deaths, ref bytes);
                DefaultByteConverter.FromByte((byte)mvp.Level, ref bytes);
                DefaultByteConverter.FromByte((byte)mvp.Team, ref bytes);
                DefaultByteConverter.FromInt(mvp.Cmid, ref bytes);
                DefaultByteConverter.FromByteCollection(mvp.Achievements.Keys, ref bytes);
                DefaultByteConverter.FromUShortCollection(mvp.Achievements.Values, ref bytes);
            }

            bytes.AddRange(PlayerStatsTotal.GetBytes());
            bytes.AddRange(PlayerStatsBestPerLife.GetBytes());

            DefaultByteConverter.FromByteCollection(PlayerXpEarned.Keys, ref bytes);
            DefaultByteConverter.FromUShortCollection(PlayerXpEarned.Values, ref bytes);

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            MostEffecientWeaponId = DefaultByteConverter.ToInt(bytes, ref idx);
            int count = DefaultByteConverter.ToByte(bytes, ref idx);
            MostValuablePlayers = new List<StatsSummary>(count);
            for (int i = 0; i < count; i++)
            {
                var stats = new StatsSummary();
                stats.Name = DefaultByteConverter.ToString(bytes, ref idx);
                stats.Kills = DefaultByteConverter.ToInt(bytes, ref idx);
                stats.Deaths = DefaultByteConverter.ToInt(bytes, ref idx);
                stats.Level = DefaultByteConverter.ToByte(bytes, ref idx);
                stats.Team = (TeamID)(int)DefaultByteConverter.ToByte(bytes, ref idx);
                stats.Cmid = DefaultByteConverter.ToInt(bytes, ref idx);
                List<byte> akeys = DefaultByteConverter.ToByteCollection(bytes, ref idx);
                List<ushort> avals = DefaultByteConverter.ToUShortCollection(bytes, ref idx);
                stats.Achievements = new Dictionary<byte, ushort>();
                for (int j = 0; j < akeys.Count && j < avals.Count; j++)
                {
                    stats.Achievements.Add(akeys[j], avals[j]);
                }

                MostValuablePlayers.Add(stats);
            }

            PlayerStatsTotal = new StatsCollection(bytes, ref idx);
            PlayerStatsBestPerLife = new StatsCollection(bytes, ref idx);

            List<byte> pkeys = DefaultByteConverter.ToByteCollection(bytes, ref idx);
            List<ushort> pvals = DefaultByteConverter.ToUShortCollection(bytes, ref idx);
            PlayerXpEarned = new Dictionary<byte, ushort>();
            for (int i = 0; i < pkeys.Count && i < pvals.Count; i++)
            {
                PlayerXpEarned.Add(pkeys[i], pvals[i]);
            }
            return idx;
        }
    }
}