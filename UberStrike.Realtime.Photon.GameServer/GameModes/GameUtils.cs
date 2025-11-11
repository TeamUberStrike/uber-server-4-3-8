
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public static class GameUtils
    {
        public static UberStrike.Core.Types.GameModeType GetGameModeType(int gameModeId)
        {
            switch (gameModeId)
            {
                case GameModeID.DeathMatch: return UberStrike.Core.Types.GameModeType.DeathMatch;
                case GameModeID.EliminationMode: return UberStrike.Core.Types.GameModeType.EliminationMode;
                case GameModeID.TeamDeathMatch: return UberStrike.Core.Types.GameModeType.TeamDeathMatch;
                default: return UberStrike.Core.Types.GameModeType.None;
            }
        }

    }
}
