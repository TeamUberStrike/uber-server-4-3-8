
using System;
using System.Collections.Generic;
using System.Text;
using Cmune.Realtime.Common.IO;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// 
    /// </summary>
    public struct ServerLoadData : IByteArray
    {
        private ServerLoadData(int d)
            : this()
        {
            Latency = 0;
            TimeStamp = DateTime.MinValue;
            State = Status.None;

            MaxPlayerCount = 250;
            PeersConnected = 0;
            PlayersConnected = 0;
            RoomsCreated = 0;
        }

        public ServerLoadData(byte[] data)
            : this(0)
        {
            FromBytes(data, 0);
        }

        public ServerLoadData(byte[] data, ref int idx)
            : this(0)
        {
            idx = FromBytes(data, idx);
        }

        public static ServerLoadData Empty
        {
            get { return new ServerLoadData(0); }
        }

        #region IByteArray Members

        public int FromBytes(byte[] bytes, int idx)
        {
            PlayersConnected = DefaultByteConverter.ToInt(bytes, ref idx);
            PeersConnected = DefaultByteConverter.ToInt(bytes, ref idx);
            RoomsCreated = DefaultByteConverter.ToInt(bytes, ref idx);
            MaxPlayerCount = DefaultByteConverter.ToFloat(bytes, ref idx);

            return idx;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(12);

            DefaultByteConverter.FromInt(PlayersConnected, ref bytes);
            DefaultByteConverter.FromInt(PeersConnected, ref bytes);
            DefaultByteConverter.FromInt(RoomsCreated, ref bytes);
            DefaultByteConverter.FromFloat(MaxPlayerCount, ref bytes);

            return bytes.ToArray();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("PlayersConnected: {0}\n", PlayersConnected);
            b.AppendFormat("PeersConnected: {0}\n", PeersConnected);
            b.AppendFormat("RoomsCreated: {0}\n", RoomsCreated);
            b.AppendFormat("MaxPlayerCount: {0}\n", MaxPlayerCount);
            b.AppendFormat("Ping: {0}\n", Latency);

            return b.ToString();
        }

        #region Fields

        public int PeersConnected { get; set; }
        public int PlayersConnected { get; set; }
        public int RoomsCreated { get; set; }

        public float MaxPlayerCount { get; set; }

        public int Latency;
        public DateTime TimeStamp;
        public Status State;

        #endregion

        public enum Status
        {
            None,
            Alive,
            NotReachable,
        }
    }
}
