
using System;
using System.Text;
using Cmune.Core.Types.Attributes;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// Serialization Size: 21 byte + n(~20)
    /// </summary>
    public class ActorInfo : CmuneDeltaSync
    {
        protected ActorInfo()
        { }

        public ActorInfo(string name, int playerID, ChannelType platform)
        {
            this.PlayerName = name;
            this.ActorId = playerID;
            this.Channel = platform;
        }

        public bool IsLoggedIn
        {
            get { return _cmuneId > 0; }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("ActorId: " + ActorId);
            b.AppendLine("Cmid: " + _cmuneId);
            b.AppendLine("Name: " + _name);
            b.AppendLine("Channel: " + _channel);
            b.AppendLine("Room: " + _currentRoom);
            return b.ToString();
        }

        #region SYNC FIELDS
        [CMUNESYNC(FieldTag.Cmid)]
        private int _cmuneId = 0;
        [CMUNESYNC(FieldTag.PlayerName)]
        private string _name = string.Empty;
        [CMUNESYNC(FieldTag.ClanTag)]
        private string _clanTag = string.Empty;
        [CMUNESYNC(FieldTag.AccessLevel)]
        private byte _accessLevel = 0;
        [CMUNESYNC(FieldTag.Channel)]
        private byte _channel = 0;
        [CMUNESYNC(FieldTag.CurrentRoom)]
        private CmuneRoomID _currentRoom = CmuneRoomID.Empty;
        [CMUNESYNC(FieldTag.Ping)]
        private ushort _ping = 0;
        #endregion

        #region PROPERTIES
        public CmuneRoomID CurrentRoom
        {
            get { return _currentRoom; }
            set { _currentRoom = value; }
        }
        public int ActorId
        {
            get { return InstanceId; }
            set { InstanceId = value; }
        }
        public ushort Ping
        {
            get { return _ping; }
            set { _ping = value; }
        }
        public int Cmid
        {
            get { return _cmuneId; }
            set { _cmuneId = value; }
        }
        public string PlayerName
        {
            get { return _name; }
            set { _name = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }
        public ChannelType Channel
        {
            get { return (ChannelType)_channel; }
            set { _channel = (byte)value; }
        }
        public string ClanTag
        {
            get { return _clanTag; }
            set { _clanTag = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }
        public int AccessLevel
        {
            get { return _accessLevel; }
            set { _accessLevel = (byte)Math.Min(value, 10); }
        }
        #endregion

        [ExtendableEnumBounds(BIT_FLAGS.BIT_01, BIT_FLAGS.BIT_07)]
        public new class FieldTag : CmuneDeltaSync.FieldTag
        {
            public const int Cmid = BIT_FLAGS.BIT_01;
            public const int PlayerName = BIT_FLAGS.BIT_02;
            public const int Channel = BIT_FLAGS.BIT_03;
            public const int CurrentRoom = BIT_FLAGS.BIT_04;
            public const int Ping = BIT_FLAGS.BIT_05;
            public const int ClanTag = BIT_FLAGS.BIT_06;
            public const int AccessLevel = BIT_FLAGS.BIT_07;
        }
    }
}