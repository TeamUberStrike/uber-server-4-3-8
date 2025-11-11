
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Models
{
    [System.Serializable]
    public class ActorInfo
    {
        public int Cmid { get; set; }
        public string PlayerName { get; set; }
        public string ClanTag { get; set; }
        public MemberAccessLevel AccessLevel { get; set; }
        public ChannelType Channel { get; set; }
        public CmuneRoomID CurrentRoom { get; set; }
        public ushort Ping { get; set; }
    }
}