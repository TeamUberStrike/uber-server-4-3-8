using System.Collections.Generic;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Business
{
    public static class Maps
    {
        private const int Minute = 60;

        public static void UpdateMapSettings(MapView map)
        {
            // Per map-type settings
            SetMapLimits(map);

            // Per game-mode settings
            map.Settings = new Dictionary<GameModeType, MapSettings>();
            if ((map.SupportedGameModes & GetGameModeFlag(GameModeType.DeathMatch)) != 0)
            {
                map.Settings.Add(GameModeType.DeathMatch, new MapSettings()
                {
                    KillsMin = 1,
                    KillsMax = 200,
                    KillsCurrent = 20,

                    TimeMin = 1 * Minute,
                    TimeMax = 15 * Minute,
                    TimeCurrent = 5 * Minute,

                    PlayersMin = 2,
                    PlayersMax = map.MaxPlayers,
                    PlayersCurrent = map.MaxPlayers,
                });
            }
            if ((map.SupportedGameModes & GetGameModeFlag(GameModeType.TeamDeathMatch)) != 0)
            {
                map.Settings.Add(GameModeType.TeamDeathMatch, new MapSettings()
                {
                    KillsMin = 1,
                    KillsMax = 200,
                    KillsCurrent = 40,

                    TimeMin = 1 * Minute,
                    TimeMax = 15 * Minute,
                    TimeCurrent = 8 * Minute,

                    PlayersMin = 2,
                    PlayersMax = map.MaxPlayers,
                    PlayersCurrent = map.MaxPlayers,
                });
            }
            if ((map.SupportedGameModes & GetGameModeFlag(GameModeType.EliminationMode)) != 0)
            {
                map.Settings.Add(GameModeType.EliminationMode, new MapSettings()
                {
                    KillsMin = 1,
                    KillsMax = 20,
                    KillsCurrent = 5,

                    TimeMin = 1 * Minute,
                    TimeMax = 5 * Minute,
                    TimeCurrent = 2 * Minute,

                    PlayersMin = 2,
                    PlayersMax = map.MaxPlayers,
                    PlayersCurrent = map.MaxPlayers,
                });
            }
        }

        private static void SetMapLimits(MapView map)
        {
            switch (map.MapId)
            {
                case 3: //The Warehouse
                    map.MaxPlayers = 8;
                    break;
                case 5: //Fort Winter
                    map.MaxPlayers = 12;
                    break;
                case 7: //Sky Garden
                    map.MaxPlayers = 8;
                    break;
                case 10: //Spaceport Alpha
                    map.MaxPlayers = 10;
                    map.SupportedGameModes = (int)GameModeFlag.DeathMatch;
                    break;
                case 13: //The Warehouse 2
                    map.MaxPlayers = 8;
                    break;
                case 16: //Space City
                    map.MaxPlayers = 8;
                    break;
                case 20: //Quake DM6
                    map.MaxPlayers = 8;
                    map.SupportedGameModes = (int)GameModeFlag.DeathMatch;
                    break;
                default:
                    map.MaxPlayers = UberStrikeCommonConfig.MaxPlayers;
                    map.SupportedGameModes = -1;
                    break;
            }
        }

        public static int GetGameModeFlag(GameModeType mode)
        {
            switch (mode)
            {
                case GameModeType.DeathMatch: return (int)GameModeFlag.DeathMatch;
                case GameModeType.TeamDeathMatch: return (int)GameModeFlag.TeamDeathMatch;
                case GameModeType.EliminationMode: return (int)GameModeFlag.EliminationMode;
                default: return (int)GameModeFlag.All;
            }
        }
    }
}