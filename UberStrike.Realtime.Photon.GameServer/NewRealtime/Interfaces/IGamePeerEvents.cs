using System.Collections.Generic;
using UberStrike.Realtime.Server.Attributes;
using UberStrike.Core.ViewModel;

namespace UberStrike.Realtime.GameServer
{
    [PeerEvents]
    public interface IGameServerPeerEvents
    {
        void GameServerData(ServerConnectionView data);
        void JoinedGameRoom(int roomId);
        void LeftGameRoom(int roomId);
        void FullGameList(List<int> gameList);
        void GameListUpdate(Dictionary<int, int> updatedGames);
        void GamesDeleted(List<int> deletedGameIds);
    }
}