
namespace UberStrike.Realtime.Common
{
    public static class GameConstants
    {
        private const int Minute = 60;

        public static readonly Limits DefaultDeathMatchLimit = new Limits(new Bounds(1 * Minute, 15 * Minute, 5 * Minute), new Bounds(1, 200, 20), new Bounds(2, 16, 8));
        public static readonly Limits DefaultTeamDeathMatchLimit = new Limits(new Bounds(1 * Minute, 15 * Minute, 8 * Minute), new Bounds(10, 200, 40), new Bounds(2, 16, 12));
        public static readonly Limits DefaultTeamEliminationLimit = new Limits(new Bounds(1 * Minute, 5 * Minute, 2 * Minute), new Bounds(1, 20, 5), new Bounds(2, 16, 12));

        public static readonly Limits SpaceStationAlphaLimit = new Limits(new Bounds(1 * Minute, 15 * Minute, 5 * Minute), new Bounds(1, 200, 20), new Bounds(2, 12, 8));

        public static void CheckGameData(short mode, GameMetaData data)
        {
            // fix me!!
            // special case for level SpaceStationAlpha
            if (data.MapID == 10)
            {
                data.RoundTime = CheckValueLimit(data.RoundTime, SpaceStationAlphaLimit.Time);
                data.SplatLimit = CheckValueLimit(data.SplatLimit, SpaceStationAlphaLimit.Kills);
                data.MaxPlayers = CheckValueLimit(data.MaxPlayers, SpaceStationAlphaLimit.Players);
            }
            else
            {
                switch (mode)
                {
                    case GameModeID.DeathMatch:
                        data.RoundTime = CheckValueLimit(data.RoundTime, DefaultDeathMatchLimit.Time);
                        data.SplatLimit = CheckValueLimit(data.SplatLimit, DefaultDeathMatchLimit.Kills);
                        data.MaxPlayers = CheckValueLimit(data.MaxPlayers, DefaultDeathMatchLimit.Players);
                        break;

                    case GameModeID.TeamDeathMatch:
                        data.RoundTime = CheckValueLimit(data.RoundTime, DefaultTeamDeathMatchLimit.Time);
                        data.SplatLimit = CheckValueLimit(data.SplatLimit, DefaultTeamDeathMatchLimit.Kills);
                        data.MaxPlayers = CheckValueLimit(data.MaxPlayers, DefaultTeamDeathMatchLimit.Players);
                        break;

                    case GameModeID.EliminationMode:
                        data.RoundTime = CheckValueLimit(data.RoundTime, DefaultTeamEliminationLimit.Time);
                        data.SplatLimit = CheckValueLimit(data.SplatLimit, DefaultTeamEliminationLimit.Kills);
                        data.MaxPlayers = CheckValueLimit(data.MaxPlayers, DefaultTeamEliminationLimit.Players);
                        break;
                }
            }
        }

        private static int CheckValueLimit(int value, int fallback, int min, int max)
        {
            return (value < min || value > max) ? fallback : value;
        }

        private static int CheckValueLimit(int value, Bounds bounds)
        {
            return (value < bounds.Min || value > bounds.Max) ? bounds.Default : value;
        }

        public class Limits
        {
            public Bounds Time { get; private set; }
            public Bounds Kills { get; private set; }
            public Bounds Players { get; private set; }

            public Limits(Bounds time, Bounds splats, Bounds playerCount)
            {
                Time = time;
                Kills = splats;
                Players = playerCount;
            }
        }

        public class Bounds
        {
            public int Min { get; private set; }
            public int Max { get; private set; }
            public int Default { get; private set; }

            public Bounds(int min, int max, int def)
            {
                Min = min;
                Max = max;
                Default = def;
            }
        }
    }
}