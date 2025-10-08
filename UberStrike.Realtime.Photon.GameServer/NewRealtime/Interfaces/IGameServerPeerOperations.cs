using UberStrike.Realtime.Server.Attributes;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.GameServer
{
    [PeerOperations]
    public interface IGameServerPeerOperations
    {
        void GetServerLoad();
        void JoinGameRoom(int roomId);
        void CreateGameRoom(GameModeType type);
        void LeaveGameRoom(int roomId);
        void RegisterToGameLobby();
    }
}