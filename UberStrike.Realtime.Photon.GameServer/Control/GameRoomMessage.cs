
using Cmune.Core.Types.Attributes;
using Cmune.Realtime.Photon.Server;

namespace UberStrike.Realtime.Photon.GameServer
{
    [ExtendableEnumBounds(51, 100)]
    public class GameRoomMessage : RoomMessageType
    {
        public const int RoundTimeoutEvent = 52;
        public const int NewRoundEvent = 53;
        public const int SittingDuckEvent = 54;
        public const int PlayerStats = 55;
        public const int PauseBeforeRoundEvent = 56;
        public const int RespawnPowerUpEvent = 57;
        public const int PauseBeforeGameEvent = 58;

        public const int Score = 59;
        public const int Stats = 60;
    }
}
