using System.Collections.Generic;
using System.Linq;
using System;

namespace UberStrike.Realtime.Photon.GameServer
{
    public static class CheatDetection
    {
        /// <summary>
        /// Detects Xp farming when people are scoring
        /// </summary>
        /// <param name="matchView"></param>
        /// <param name="farmerKillThreshold"></param>
        /// <param name="farmerKdrThreshold"></param>
        /// <param name="plantDeathThreshold"></param>
        /// <param name="plantKdrThreshold"></param>
        public static HashSet<int> DetectXpFarmers(ICollection<PlayerStatistics> allPlayers, int farmerKillThreshold = 30, float farmerKdrThreshold = 8, int plantDeathThreshold = 15, float plantKdrThreshold = 0.3f)
        {
            var cheaters = new HashSet<int>();

            foreach (var statsView in allPlayers)
            {
                var stats = statsView.TotalStats;
                if (stats.GetKills() >= farmerKillThreshold)
                {
                    float kdr = stats.GetKills() / (float)Math.Max(stats.Deaths, 1);
                    if (kdr >= farmerKdrThreshold)
                    {
                        cheaters.Add(statsView.Cmid);
                    }
                }
            }

            int plantCount = 0;
            foreach (var statsView in allPlayers)
            {
                var stats = statsView.TotalStats;
                if (stats.Deaths >= plantDeathThreshold)
                {
                    float kdr = kdr = stats.GetKills() / (float)stats.Deaths;
                    if (kdr <= plantKdrThreshold)
                    {
                        plantCount++;
                        cheaters.Add(statsView.Cmid);
                    }
                }
            }

            //if more than half of the players are considered to be plants: there is something wrong
            if (plantCount > 0 && plantCount * 2 >= allPlayers.Count)
            {
                return cheaters;
            }
            else
            {
                return new HashSet<int>();
            }
        }
    }
}
