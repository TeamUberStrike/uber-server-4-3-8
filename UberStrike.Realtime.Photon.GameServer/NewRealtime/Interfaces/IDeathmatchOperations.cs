using UberStrike.Realtime.Server.Attributes;

namespace UberStrike.Realtime.GameServer
{
    [RoomOperations]
    public interface IDeathmatchOperations
    {
        //NOTICE:
        //All operations will automatically have the peerId of the sender as first argument
        
        void KillPlayer(int killedPeerId);
        void ShootPlayer(int shotPeerId);
    }
}