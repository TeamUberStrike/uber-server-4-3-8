using System.Collections.Generic;
using Cmune.Realtime.Common;

namespace UberStrike.Realtime.Common
{
    public class StatsInfo : IByteArray
    {
        public short Kills { get; set; }
        public short Deaths { get; set; }
        public ushort XP { get; set; }
        public ushort Points { get; set; }

        public StatsInfo()
        { }

        public StatsInfo(byte[] bytes, ref int index)
            : this()
        {
            index = FromBytes(bytes, index);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(16);
            UberStrike.Realtime.Common.IO.UberStrikeByteConverter.FromShort(Kills, ref bytes);
            UberStrike.Realtime.Common.IO.UberStrikeByteConverter.FromShort(Deaths, ref bytes);
            UberStrike.Realtime.Common.IO.UberStrikeByteConverter.FromUShort(XP, ref bytes);
            UberStrike.Realtime.Common.IO.UberStrikeByteConverter.FromUShort(Points, ref bytes);
            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            Kills = UberStrike.Realtime.Common.IO.UberStrikeByteConverter.ToShort(bytes, ref idx);
            Deaths = UberStrike.Realtime.Common.IO.UberStrikeByteConverter.ToShort(bytes, ref idx);
            XP = UberStrike.Realtime.Common.IO.UberStrikeByteConverter.ToUShort(bytes, ref idx);
            Points = UberStrike.Realtime.Common.IO.UberStrikeByteConverter.ToUShort(bytes, ref idx);
            return idx;
        }

        public override int GetHashCode()
        {
            return Kills + Deaths + XP + Points;
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                return this.GetHashCode() == obj.GetHashCode();
            }
            else { return false; }
        }

        public override string ToString()
        {
            return string.Format("{0}/{1} {2}/{3}%", Kills, Deaths, XP, Points);
        }
    }
}