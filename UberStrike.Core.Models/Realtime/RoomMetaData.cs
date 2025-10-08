using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Models
{
    public class RoomMetaData
    {
        public string RoomName { get; set; }
        public CmuneRoomID ID { get; set; }
        public virtual int ConnectedPlayers { get; set; }
        public virtual int MaxPlayers { get; set; }
        public virtual string Password { get; set; }
        public PhotonUsageType Tag { get; set; }

        public virtual bool IsPublic
        {
            get { return string.IsNullOrEmpty(Password); }
        }

        public bool IsFull
        {
            get { return ConnectedPlayers >= MaxPlayers; }
        }
    }
}