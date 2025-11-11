
using System.Collections.Generic;
using System.Linq;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public static class AchievementHelper
    {
        public static KeyValuePair<int, ushort> SelectWinnerOfAchievement(AchievementType type, List<PlayerStatistics> players)
        {
            PlayerStatistics[] winner = null;
            int cmid = 0;
            ushort value = 0, value1 = 0, value2 = 0;

            if (players.Count > 1)
            {
                switch (type)
                {
                    //highest KDR
                    case AchievementType.MostValuable:
                        winner = players.OrderByDescending(a => a.TotalStats.GetKdr()).Take(2).ToArray();
                        //value1 = (ushort)(winner[0].TotalStats.GetKdr() * 10);
                        //value2 = (ushort)(winner[1].TotalStats.GetKdr() * 10);
                        //if (value1 != value2 && value1 > 0)
                        //{
                        //    cmid = winner[0].Cmid;
                        //    value = value1;
                        //}

                        float v1 = winner[0].TotalStats.GetKdr();
                        float v2 = winner[1].TotalStats.GetKdr();
                        if (v1 != v2 && v1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = (ushort)(v1 * 10);
                        }
                        break;

                    //highest kill count
                    case AchievementType.MostAggressive:
                        winner = players.OrderByDescending(a => a.TotalStats.GetKills()).Take(2).ToArray();
                        value1 = (ushort)(winner[0].TotalStats.GetKills());
                        value2 = (ushort)(winner[1].TotalStats.GetKills());
                        if (value1 != value2 && value1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = value1;
                        }
                        break;

                    //highest head+nut shots
                    case AchievementType.SharpestShooter:
                        winner = players.OrderByDescending(a => a.TotalStats.Nutshots + a.TotalStats.Headshots).Take(2).ToArray();
                        value1 = (ushort)(winner[0].TotalStats.Nutshots + winner[0].TotalStats.Headshots);
                        value2 = (ushort)(winner[1].TotalStats.Nutshots + winner[1].TotalStats.Headshots);
                        if (value1 != value2 && value1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = value1;
                        }
                        break;

                    //Highest kills per life
                    case AchievementType.TriggerHappy:
                        winner = players.OrderByDescending(a => a.BestLifeStats.GetKills()).Take(2).ToArray();
                        value1 = (ushort)(winner[0].BestLifeStats.GetKills());
                        value2 = (ushort)(winner[1].BestLifeStats.GetKills());
                        if (value1 != value2 && value1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = value1;
                        }
                        break;

                    //highest damage
                    case AchievementType.HardestHitter:
                        winner = players.OrderByDescending(a => a.TotalStats.GetDamageDealt()).Take(2).ToArray();
                        value1 = (ushort)(winner[0].TotalStats.GetDamageDealt());
                        value2 = (ushort)(winner[1].TotalStats.GetDamageDealt());
                        if (value1 != value2 && value1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = value1;
                        }
                        break;

                    //highest accuracy
                    case AchievementType.CostEffective:
                        winner = players.OrderByDescending(a => a.TotalStats.GetAccuracy()).Take(2).ToArray();
                        value1 = (ushort)(winner[0].TotalStats.GetAccuracy() * 1000);
                        value2 = (ushort)(winner[1].TotalStats.GetAccuracy() * 1000);
                        if (value1 != value2 && value1 > 0)
                        {
                            cmid = winner[0].Cmid;
                            value = value1;
                        }
                        break;
                }
            }

            return new KeyValuePair<int, ushort>(cmid, value);
        }

        public static int CreateBitmask(int cmid, Dictionary<AchievementType, int> achievers)
        {
            int mask = 0;
            foreach (var v in achievers)
            {
                if (v.Value == cmid) mask |= (int)v.Key;
            }
            return mask;
        }
    }
}