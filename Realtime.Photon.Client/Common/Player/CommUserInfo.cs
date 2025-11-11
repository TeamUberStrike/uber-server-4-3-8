using System;
using System.Text;
using Cmune.Core.Types.Attributes;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// 12 + n(~0)
    /// </summary>
    public class CommActorInfo : ActorInfo
    {
        public CommActorInfo() { }

        public CommActorInfo(SyncObject obj)
        {
            ReadSyncData(obj);
        }

        public CommActorInfo(string name, int actorId, ChannelType channel)
            : base(name, actorId, channel)
        {
            this.PlayerName = name;
            this.ActorId = actorId;
            this.Channel = channel;
        }

        public CommActorInfo(string name, int actorId, int cmuneId, string clanTag, ChannelType channel)
            : this(name, actorId, channel)
        {
            Cmid = cmuneId;
            ClanTag = clanTag;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("Name: " + PlayerName);
            b.AppendLine("ActorID: " + ActorId);
            b.AppendLine("Cmid: " + Cmid);
            b.AppendLine("Flag: " + _moderationFlag);
            b.AppendLine("Room: " + CurrentRoom);
            return b.ToString();
        }

        public bool IsInGame
        {
            get { return CurrentRoom.Number != StaticRoomID.CommCenter; }
        }

        #region SYNC FIELDS
        [CMUNESYNC(FieldTag.ModInformation)]
        private string _modInfo = string.Empty;
        [CMUNESYNC(FieldTag.ModerationFlag)]
        private byte _moderationFlag = 0;
        #endregion

        #region PROPERTIES
        public byte ModerationFlag
        {
            get { return _moderationFlag; }
            set { _moderationFlag = value; }
        }
        public string ModInformation
        {
            get { return _modInfo; }
            set { _modInfo = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }
        #endregion

        [ExtendableEnumBounds(BIT_FLAGS.BIT_10, BIT_FLAGS.BIT_31)]
        public new class FieldTag : CmuneDeltaSync.FieldTag
        {
            public const int ModInformation = BIT_FLAGS.BIT_10;
            public const int ModerationFlag = BIT_FLAGS.BIT_11;
        }

        /// <summary>
        /// BYTE (8 states)
        /// </summary>
        [Flags]
        public enum ModerationTag
        {
            None = BIT_FLAGS.BIT_NONE,
            Muted = BIT_FLAGS.BIT_01,
            Ghosted = BIT_FLAGS.BIT_02,
            Banned = BIT_FLAGS.BIT_03,

            Speedhacking = BIT_FLAGS.BIT_04,
            Spamming = BIT_FLAGS.BIT_05,
            Language = BIT_FLAGS.BIT_06,
            Name = BIT_FLAGS.BIT_07,
        }
    }
}