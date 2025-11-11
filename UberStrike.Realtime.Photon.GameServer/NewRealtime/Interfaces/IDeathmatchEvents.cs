using UberStrike.Realtime.Server.Attributes;

namespace UberStrike.Realtime.GameServer
{
    [RoomEvents]
    public interface IDeathmatchEvents
    {
        void PlayerKilled(int playerId);
        void PlayerShot(int playerId);
    }
}