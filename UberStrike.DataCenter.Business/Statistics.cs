using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Utils.Caching;
using UberStrike.DataCenter.Business.Views;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Utils;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using System.Diagnostics;

namespace UberStrike.DataCenter.Business
{
    public static class Statistics
    {
        #region Ranking

        /// <summary>
        /// Get all total stats of members
        /// </summary>
        /// <param name="listPlayingIdsCMID"></param>
        /// <returns></returns>
        public static List<AllTimeTotalRanking> GetStats(List<int> listPlayingIdsCMID)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                List<AllTimeTotalRanking> fbRank = null;
                fbRank = (from tab in ppDB.AllTimeTotalRankings where listPlayingIdsCMID.Contains(tab.CMID) select tab).ToList();
                return fbRank;
            }
        }

        /// <summary>
        /// Get the top 10 total stats
        /// </summary>
        /// <returns></returns>
        public static List<AllTimeTotalRanking> GetTop10MiniverseStatsForPP()
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                List<AllTimeTotalRanking> top10Rank = null;
                Cache<List<AllTimeTotalRanking>> cachedTop10Rank = new Cache<List<AllTimeTotalRanking>>("Cmune.Datacenter.Apps.Shooter.GetTop10MiniverseStatsForPP");
                if (cachedTop10Rank.IsInCache)
                {
                    top10Rank = cachedTop10Rank.Val;
                }
                else
                {
                    top10Rank = (from tab in ppDB.AllTimeTotalRankings where tab.Ranknum <= 10 select tab).ToList();
                    if (top10Rank != null) cachedTop10Rank.InsertHour(top10Rank);
                }

                return top10Rank;
            }
        }

        /// <summary>
        /// Get the first hundred users
        /// </summary>
        /// <returns></returns>
        public static List<AllTimeTotalRanking> GetAllTimeRanking(int page, int countPerPage)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                if (page < 1)
                {
                    page = 1;
                }

                if (countPerPage < 1)
                {
                    countPerPage = 1;
                }

                int lowerLimit = (page - 1) * countPerPage + 1;
                int upperLimit = page * countPerPage;

                List<AllTimeTotalRanking> top100 = (from r in uberStrikeDb.AllTimeTotalRankings where r.Ranknum >= lowerLimit && r.Ranknum <= upperLimit select r).OrderBy(r => r.Ranknum).ToList();

                return top100;
            }
        }

        /// <summary>
        /// Get the first hundred users over the last week
        /// </summary>
        /// <returns></returns>
        public static List<WeeklyTotalRanking> GetTopWeekly100Users()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<WeeklyTotalRanking> top100 = (from tab in uberStrikeDb.WeeklyTotalRankings where tab.Ranknum <= 100 && tab.WeekId == 1 select tab).OrderBy(r => r.Ranknum).ToList();

                return top100;
            }
        }

        public static List<DailyRanking> GetDailyRanking(int page, int countPerPage, out DateTime rankingDate)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                rankingDate = DateTime.Now.AddDays(-1);

                if (rankingDate.Hour == 0 && rankingDate.Minute <= 30)
                {
                    rankingDate = rankingDate.AddDays(-1);
                }

                int rankingDay = rankingDate.Day;

                if (page < 1)
                {
                    page = 1;
                }

                if (countPerPage < 1)
                {
                    countPerPage = 1;
                }

                int lowerLimit = (page - 1) * countPerPage + 1;
                int upperLimit = page * countPerPage;

                List<DailyRanking> ranking = (from r in uberStrikeDb.DailyRankings where r.DailyRankingIndex >= lowerLimit && r.DailyRankingIndex <= upperLimit && r.Day == rankingDay select r).OrderBy(r => r.DailyRankingIndex).ToList();

                return ranking;
            }
        }

        public static int GetUserRank(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                int rank = 0;

                var query = from r in uberStrikeDb.AllTimeTotalRankings
                           where r.CMID == cmid
                           select new { Rank = r.Ranknum };

                foreach (var row in query)
                {
                    rank = row.Rank;
                }

                return rank;
            }
        }

        public static List<AllTimeTotalRanking> GetNearbyUsers(int cmid, int usersCount)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<AllTimeTotalRanking> rankings = new List<AllTimeTotalRanking>();

                int userRank = GetUserRank(cmid);

                if (userRank != 0)
                {
                    int initialRank = userRank - usersCount / 2;

                    rankings = (from r in uberStrikeDb.AllTimeTotalRankings
                                where r.Ranknum >= initialRank && r.Ranknum < initialRank + usersCount
                                select r).Take(usersCount).OrderBy(r => r.Ranknum).ToList();
                }

                return rankings;
            }
        }

        #endregion

        private static void CreateMapUsage(MapUsageView mapUsageView)
        {
            if (mapUsageView != null)
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    MapUsage mapUsage = new MapUsage();

                    mapUsage.GameModeId = (int) mapUsageView.GameModeId;
                    mapUsage.MapId = mapUsageView.MapId;
                    mapUsage.PlayDate = mapUsageView.PlayDate;
                    mapUsage.PlayerLimit = mapUsageView.PlayerLimit;
                    mapUsage.PlayersCompleted = mapUsageView.PlayersCompleted;
                    mapUsage.PlayersTotal = mapUsageView.PlayersTotal;
                    mapUsage.TimeLimit = mapUsageView.TimeLimit;

                    uberStrikeDb.MapUsages.InsertOnSubmit(mapUsage);
                    uberStrikeDb.SubmitChanges();
                }
            }
        }

        public static void SetMatchScore(MatchView matchView)
        {
            if (matchView != null)
            {
                foreach (PlayerStatisticsView statisticView in matchView.PlayersCompleted)
                {
                    SetPlayerScore(statisticView, true);
                }

                foreach (PlayerStatisticsView statisticView in matchView.PlayersNonCompleted)
                {
                    SetPlayerScore(statisticView, false);
                }

                MapUsageView mapUsage = new MapUsageView(DateTime.Now, matchView.MapId,
                                                            matchView.GameModeId,
                                                            matchView.TimeLimit,
                                                            matchView.PlayersLimit,
                                                            matchView.PlayersCompleted.Count + matchView.PlayersNonCompleted.Count,
                                                            matchView.PlayersCompleted.Count);

                CreateMapUsage(mapUsage);
            }
        }

        /// <summary>
        /// Updates the statistic of a user
        /// </summary>
        /// <param name="statisticView"></param>
        /// <param name="computeXP"></param>
        /// <returns></returns>
        private static bool SetPlayerScore(PlayerStatisticsView statisticView, bool computeXP)
        {
            bool isScoreUpdated = false;

            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                try
                {
                    User currentUser = paradiseDB.Users.SingleOrDefault(u => u.CMID == statisticView.Cmid);

                    if (currentUser != null && statisticView != null && statisticView.PersonalRecord != null && statisticView.WeaponStatistics != null)
                    {
                        int points = 0;

                        if (computeXP)
                        {
                            Dictionary<int, PlayerXPEventView> xpEventsViewOrdered = new Dictionary<int, PlayerXPEventView>();

                            if (HttpRuntime.Cache[UberStrikeCacheKeys.XPEvents] != null)
                            {
                                xpEventsViewOrdered = (Dictionary<int, PlayerXPEventView>)HttpRuntime.Cache[UberStrikeCacheKeys.XPEvents];
                            }
                            else
                            {
                                xpEventsViewOrdered = Statistics.GetXPEventsViewOrdered();
                                HttpRuntime.Cache.Add(UberStrikeCacheKeys.XPEvents, xpEventsViewOrdered, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                            }

                            List<PlayerLevelCapView> levelCapsView = new List<PlayerLevelCapView>();

                            if (HttpRuntime.Cache[UberStrikeCacheKeys.LevelCaps] != null)
                            {
                                levelCapsView = (List<PlayerLevelCapView>)HttpRuntime.Cache[UberStrikeCacheKeys.LevelCaps];
                            }
                            else
                            {
                                levelCapsView = Statistics.GetLevelCapsView();
                                HttpRuntime.Cache.Add(UberStrikeCacheKeys.LevelCaps, levelCapsView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                            }

                            int gainedXP = LevelingUtilities.ComputeXP(statisticView, xpEventsViewOrdered);

                            AttributeXp(currentUser, levelCapsView, gainedXP);

                            points = statisticView.Points;
                            currentUser.Points += points;
                        }

                        currentUser.Hits += statisticView.Hits;
                        currentUser.Shots += statisticView.Shots;
                        currentUser.Splats += statisticView.Splats;
                        currentUser.Splatted += statisticView.Splatted;
                        currentUser.Headshots += statisticView.Headshots;
                        currentUser.Nutshots += statisticView.Nutshots;
                        currentUser.TimeSpentInGame += statisticView.TimeSpentInGame;

                        // Weapons part

                        currentUser.CannonTotalDamageDone += statisticView.WeaponStatistics.CannonTotalDamageDone;
                        currentUser.CannonTotalShotsFired += statisticView.WeaponStatistics.CannonTotalShotsFired;
                        currentUser.CannonTotalShotsHit += statisticView.WeaponStatistics.CannonTotalShotsHit;
                        currentUser.CannonTotalSplats += statisticView.WeaponStatistics.CannonTotalSplats;
                        currentUser.HandgunTotalDamageDone += statisticView.WeaponStatistics.HandgunTotalDamageDone;
                        currentUser.HandgunTotalShotsFired += statisticView.WeaponStatistics.HandgunTotalShotsFired;
                        currentUser.HandgunTotalShotsHit += statisticView.WeaponStatistics.HandgunTotalShotsHit;
                        currentUser.HandgunTotalSplats += statisticView.WeaponStatistics.HandgunTotalSplats;
                        currentUser.LauncherTotalDamageDone += statisticView.WeaponStatistics.LauncherTotalDamageDone;
                        currentUser.LauncherTotalShotsFired += statisticView.WeaponStatistics.LauncherTotalShotsFired;
                        currentUser.LauncherTotalShotsHit += statisticView.WeaponStatistics.LauncherTotalShotsHit;
                        currentUser.LauncherTotalSplats += statisticView.WeaponStatistics.LauncherTotalSplats;
                        currentUser.MachineGunTotalDamageDone += statisticView.WeaponStatistics.MachineGunTotalDamageDone;
                        currentUser.MachineGunTotalShotsFired += statisticView.WeaponStatistics.MachineGunTotalShotsFired;
                        currentUser.MachineGunTotalShotsHit += statisticView.WeaponStatistics.MachineGunTotalShotsHit;
                        currentUser.MachineGunTotalSplats += statisticView.WeaponStatistics.MachineGunTotalSplats;
                        currentUser.MeleeTotalDamageDone += statisticView.WeaponStatistics.MeleeTotalDamageDone;
                        currentUser.MeleeTotalShotsFired += statisticView.WeaponStatistics.MeleeTotalShotsFired;
                        currentUser.MeleeTotalShotsHit += statisticView.WeaponStatistics.MeleeTotalShotsHit;
                        currentUser.MeleeTotalSplats += statisticView.WeaponStatistics.MeleeTotalSplats;
                        currentUser.ShotgunTotalDamageDone += statisticView.WeaponStatistics.ShotgunTotalDamageDone;
                        currentUser.ShotgunTotalShotsFired += statisticView.WeaponStatistics.ShotgunTotalShotsFired;
                        currentUser.ShotgunTotalShotsHit += statisticView.WeaponStatistics.ShotgunTotalShotsHit;
                        currentUser.ShotgunTotalSplats += statisticView.WeaponStatistics.ShotgunTotalSplats;
                        currentUser.SniperTotalDamageDone += statisticView.WeaponStatistics.SniperTotalDamageDone;
                        currentUser.SniperTotalShotsFired += statisticView.WeaponStatistics.SniperTotalShotsFired;
                        currentUser.SniperTotalShotsHit += statisticView.WeaponStatistics.SniperTotalShotsHit;
                        currentUser.SniperTotalSplats += statisticView.WeaponStatistics.SniperTotalSplats;
                        currentUser.SplattergunTotalDamageDone += statisticView.WeaponStatistics.SplattergunTotalDamageDone;
                        currentUser.SplattergunTotalShotsFired += statisticView.WeaponStatistics.SplattergunTotalShotsFired;
                        currentUser.SplattergunTotalShotsHit += statisticView.WeaponStatistics.SplattergunTotalShotsHit;
                        currentUser.SplattergunTotalSplats += statisticView.WeaponStatistics.SplattergunTotalSplats;

                        // Personal record

                        if (statisticView.PersonalRecord.MostArmorPickedUp > currentUser.MostArmorPickedUp)
                        {
                            currentUser.MostArmorPickedUp = statisticView.PersonalRecord.MostArmorPickedUp;
                        }

                        if (statisticView.PersonalRecord.MostCannonSplats > currentUser.MostCannonSplats)
                        {
                            currentUser.MostCannonSplats = statisticView.PersonalRecord.MostCannonSplats;
                        }

                        if (statisticView.PersonalRecord.MostConsecutiveSnipes > currentUser.MostConsecutiveSnipes)
                        {
                            currentUser.MostConsecutiveSnipes = statisticView.PersonalRecord.MostConsecutiveSnipes;
                        }

                        if (statisticView.PersonalRecord.MostDamageDealt > currentUser.MostDamageDealt)
                        {
                            currentUser.MostDamageDealt = statisticView.PersonalRecord.MostDamageDealt;
                        }

                        if (statisticView.PersonalRecord.MostDamageReceived > currentUser.MostDamageReceived)
                        {
                            currentUser.MostDamageReceived = statisticView.PersonalRecord.MostDamageReceived;
                        }

                        if (statisticView.PersonalRecord.MostHandgunSplats > currentUser.MostHandgunSplats)
                        {
                            currentUser.MostHandgunSplats = statisticView.PersonalRecord.MostHandgunSplats;
                        }

                        if (statisticView.PersonalRecord.MostHeadshots > currentUser.MostHeadshots)
                        {
                            currentUser.MostHeadshots = statisticView.PersonalRecord.MostHeadshots;
                        }

                        if (statisticView.PersonalRecord.MostHealthPickedUp > currentUser.MostHealthPickedUp)
                        {
                            currentUser.MostHealthPickedUp = statisticView.PersonalRecord.MostHealthPickedUp;
                        }

                        if (statisticView.PersonalRecord.MostLauncherSplats > currentUser.MostLauncherSplats)
                        {
                            currentUser.MostLauncherSplats = statisticView.PersonalRecord.MostLauncherSplats;
                        }

                        if (statisticView.PersonalRecord.MostMachinegunSplats > currentUser.MostMachinegunSplats)
                        {
                            currentUser.MostMachinegunSplats = statisticView.PersonalRecord.MostMachinegunSplats;
                        }

                        if (statisticView.PersonalRecord.MostMeleeSplats > currentUser.MostMeleeSplats)
                        {
                            currentUser.MostMeleeSplats = statisticView.PersonalRecord.MostMeleeSplats;
                        }

                        if (statisticView.PersonalRecord.MostNutshots > currentUser.MostNutshots)
                        {
                            currentUser.MostNutshots = statisticView.PersonalRecord.MostNutshots;
                        }

                        if (statisticView.PersonalRecord.MostShotgunSplats > currentUser.MostShotgunSplats)
                        {
                            currentUser.MostShotgunSplats = statisticView.PersonalRecord.MostShotgunSplats;
                        }

                        if (statisticView.PersonalRecord.MostSniperSplats > currentUser.MostSniperSplats)
                        {
                            currentUser.MostSniperSplats = statisticView.PersonalRecord.MostSniperSplats;
                        }

                        if (statisticView.PersonalRecord.MostSplats > currentUser.MostSplats)
                        {
                            currentUser.MostSplats = statisticView.PersonalRecord.MostSplats;
                        }

                        if (statisticView.PersonalRecord.MostSplattergunSplats > currentUser.MostSplattergunSplats)
                        {
                            currentUser.MostSplattergunSplats = statisticView.PersonalRecord.MostSplattergunSplats;
                        }

                        if (statisticView.PersonalRecord.MostXPEarned > currentUser.MostXPEarned)
                        {
                            currentUser.MostXPEarned = statisticView.PersonalRecord.MostXPEarned;
                        }

                        paradiseDB.SubmitChanges();

                        if (points > 0)
                        {
                            CmuneEconomy.AttributePoints(statisticView.Cmid, points, false, PointsDepositType.Game);
                        }

                        isScoreUpdated = true;
                    }
                }
                catch (Exception ex)
                {
                    XmlSerializer s = new XmlSerializer(typeof(PlayerStatisticsView));
                    StringWriter sw = new StringWriter();
                    s.Serialize(sw, statisticView);
                    string serializedStats = sw.ToString();

                    serializedStats = serializedStats.Replace(Environment.NewLine, String.Empty);
                    int position = serializedStats.IndexOf('>', 0);

                    if (serializedStats.Length > position)
                    {
                        position = serializedStats.IndexOf('>', position + 1);
                    }

                    string serializedXmlBeginning = serializedStats.Substring(0, position + 1);

                    if (serializedStats.Length > position)
                    {
                        serializedStats = serializedXmlBeginning + serializedStats.Substring(position + 1).Replace(" ", String.Empty);
                    }

                    serializedStats = serializedStats.Base64Encode();

                    StringBuilder conflictedFields = new StringBuilder();

                    if (ex.GetType().FullName == "System.Data.Linq.ChangeConflictException")
                    {
                        ex = (ChangeConflictException)ex;

                        foreach (ObjectChangeConflict occ in paradiseDB.ChangeConflicts)
                        {
                            foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                            {
                                object currVal = mcc.CurrentValue;
                                object origVal = mcc.OriginalValue;
                                object databaseVal = mcc.DatabaseValue;
                                MemberInfo mi = mcc.Member;

                                conflictedFields.Append("[Field:");
                                conflictedFields.Append(mi.Name);
                                conflictedFields.Append("][Current:");
                                conflictedFields.Append(currVal);
                                conflictedFields.Append("][Original:");
                                conflictedFields.Append(origVal);
                                conflictedFields.Append("][Database:");
                                conflictedFields.Append(databaseVal);
                                conflictedFields.Append("]");
                            }
                        }
                    }

                    CmuneLog.LogException(ex, "[statisticView][" + serializedStats + "][computeXP][" + computeXP.ToString() + "][Conflicted][" + conflictedFields.ToString() + "]");
                }
            }

            return isScoreUpdated;
        }

        private static void AttributeXp(User user, List<PlayerLevelCapView> levelCapsView, int xp)
        {
            if (user.Level <= UberStrikeCommonConfig.LevelCap)
            {
                LevelingUtilities levelingUtilities = new LevelingUtilities(levelCapsView);
                user.Level = levelingUtilities.LevelUp(user.Level, user.XP + xp);
                user.XP += xp;
            }
        }

        public static void AttributeXp(int cmid, int xp)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                User user = Users.GetUser(cmid, uberStrikeDb);

                if (user != null)
                {
                    List<PlayerLevelCapView> levelCapsView = GetLevelCapsView();
                    AttributeXp(user, levelCapsView, xp);
                    uberStrikeDb.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Gets the 5 last days of stats for a specific member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<DailyRanking> GetDailyStatisticsHistory(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<DailyRanking> statisticsHistory = uberStrikeDb.DailyRankings.Where(r => r.Cmid == cmid).OrderByDescending(r => r.DailyRankingId).ToList();

                return statisticsHistory;
            }
        }

        /// <summary>
        /// Return the stats (UserName, Splats, Splatted...) of the user plus the top ten
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<PlayerCardView> GetUserAndTopStats(int cmid)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                List<PlayerCard> players = new List<PlayerCard>();

                if (cmid < 1)
                {
                    throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
                }

                // First we get the top ten
                List<AllTimeTotalRanking> rankingTopUsers = Statistics.GetTop10MiniverseStatsForPP();
                foreach (AllTimeTotalRanking topPlayer in rankingTopUsers)
                {
                    PlayerCard friend = new PlayerCard(topPlayer);
                    players.Add(friend);
                }

                players.Sort();
                // Now we get the player stats and add them at the top
                PlayerCard statsUser = new PlayerCard(cmid, true);
                if (statsUser != null)
                {
                    players.Insert(0, statsUser);
                }

                List<PlayerCardView> playersViews = new List<PlayerCardView>();
                foreach (PlayerCard playerCard in players)
                {
                    playersViews.Add(new PlayerCardView(playerCard.Name, playerCard.Splats, playerCard.Splatted, playerCard.Precision, playerCard.Ranking, playerCard.TagName));
                }

                return playersViews;
            }
        }

        /// <summary>
        /// Retrieves the player card view of a member. This is a cached data.
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static PlayerCardView GetStatisticsView(int cmid)
        {
            PlayerCardView playerCardView = null;
            
            AllTimeTotalRanking allTimeRanking = GetStatistics(cmid);

            if (allTimeRanking != null)
            {
                playerCardView = new PlayerCardView(allTimeRanking.CMID, allTimeRanking.Name, (int)allTimeRanking.Splats, (int)allTimeRanking.Splatted, allTimeRanking.PrecisionDisplay, allTimeRanking.Ranknum, (int)allTimeRanking.Shots, (int)allTimeRanking.Hits, allTimeRanking.TagName);
            }

            return playerCardView;
        }

        /// <summary>
        /// Retrieves the player card view of a member. This is real time data => does not contain ranking, name, precision and clan tag
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static PlayerCardView GetRealTimeStatisticsView(int cmid)
        {
            PlayerCardView playerCardView = null;

            User uberstrikeMember = Users.GetUser(cmid);

            if (uberstrikeMember != null)
            {
                playerCardView = new PlayerCardView(cmid, uberstrikeMember.Splats, uberstrikeMember.Splatted, uberstrikeMember.Shots, uberstrikeMember.Hits);
            }

            return playerCardView;
        }

        /// <summary>
        /// Retrieves the all time ranking of a member. This is a cached data.
        /// READ ONLY
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static AllTimeTotalRanking GetStatistics(int cmid)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                AllTimeTotalRanking allTimeRanking = GetStatistics(cmid, paradiseDB);

                return allTimeRanking;
            }
        }

        /// <summary>
        /// Retrieves the all time ranking of a member. This is a cached data.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static AllTimeTotalRanking GetStatistics(int cmid, UberstrikeDataContext paradiseDB)
        {
            AllTimeTotalRanking allTimeRanking = null;

            if (cmid > 0 && paradiseDB != null)
            {
                allTimeRanking = paradiseDB.AllTimeTotalRankings.SingleOrDefault(aT => aT.CMID == cmid);
            }

            return allTimeRanking;
        }

        /// <summary>
        /// Get a Paradise Paintball user view (complete statistics)
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static PlayerStatisticsView GetCompleteStatisticsView(int cmid)
        {
            PlayerStatisticsView playerStatisticsView = null;

            User user = GetCompleteStatistics(cmid);

            if (user != null)
            {
                playerStatisticsView = GetCompleteStatisticsView(user);
            }

            return playerStatisticsView;
        }

        /// <summary>
        /// Get a Paradise Paintball user view (complete statistics)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static PlayerStatisticsView GetCompleteStatisticsView(User user)
        {
            PlayerStatisticsView statisticsView = null;

            if (user != null)
            {
                PlayerPersonalRecordStatisticsView personalRecord = new PlayerPersonalRecordStatisticsView(user.MostHeadshots, user.MostNutshots, user.MostConsecutiveSnipes, user.MostXPEarned, user.MostSplats, user.MostDamageDealt, user.MostDamageReceived, user.MostArmorPickedUp, user.MostHealthPickedUp, user.MostMeleeSplats, user.MostHandgunSplats, user.MostMachinegunSplats, user.MostShotgunSplats, user.MostSniperSplats, user.MostSplattergunSplats, user.MostCannonSplats, user.MostLauncherSplats);
                PlayerWeaponStatisticsView weaponStatisticsView = new PlayerWeaponStatisticsView(user.MeleeTotalSplats, user.HandgunTotalSplats, user.MachineGunTotalSplats, user.ShotgunTotalSplats, user.SniperTotalSplats, user.SplattergunTotalSplats, user.CannonTotalSplats, user.LauncherTotalSplats, user.MeleeTotalShotsFired, user.MeleeTotalShotsHit, user.MeleeTotalDamageDone, user.HandgunTotalShotsFired, user.HandgunTotalShotsHit, user.HandgunTotalDamageDone, user.MachineGunTotalShotsFired, user.MachineGunTotalShotsHit, user.MachineGunTotalDamageDone, user.ShotgunTotalShotsFired, user.ShotgunTotalShotsHit, user.ShotgunTotalDamageDone, user.SniperTotalShotsFired, user.SniperTotalShotsHit, user.SniperTotalDamageDone, user.SplattergunTotalShotsFired, user.SplattergunTotalShotsHit, user.SplattergunTotalDamageDone, user.CannonTotalShotsFired, user.CannonTotalShotsHit, user.CannonTotalDamageDone, user.LauncherTotalShotsFired, user.LauncherTotalShotsHit, user.LauncherTotalDamageDone);

                statisticsView = new PlayerStatisticsView(user.CMID, user.Splats, user.Splatted, user.Shots, user.Hits, user.Headshots, user.Nutshots, user.XP, user.Level, personalRecord, weaponStatisticsView, user.Points);
            }

            return statisticsView;
        }

        /// <summary>
        /// Get a Paradise Paintball user (complete statistics)
        /// READ ONLY
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static User GetCompleteStatistics(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                return GetCompleteStatistics(cmid, uberStrikeDb);
            }
        }

        /// <summary>
        /// Get a Paradise Paintball user (complete statistics)
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static User GetCompleteStatistics(int cmid, UberstrikeDataContext paradiseDB)
        {
            User user = null;

            if (cmid > 0 && paradiseDB != null)
            {
                user = paradiseDB.Users.SingleOrDefault(u => u.CMID == cmid);
            }

            return user;
        }

        /// <summary>
        /// Get all the referrers for a specific channel and referrer
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="referrerId"></param>
        /// <returns></returns>
        public static List<ReferrerSource> GetListReferrerSourceByChannelIDAndName(ChannelType channel, ReferrerPartnerType referrerId)
        {
            UberstrikeDataContext ppDB = new UberstrikeDataContext();
            List<ReferrerSource> listReferrers = new List<ReferrerSource>();
            listReferrers = (from tab in ppDB.ReferrerSources where tab.ChannelID == (int) channel && tab.ReferrerPartnerId == (int) referrerId select tab).ToList();
            return listReferrers;
        }

        #region XP

        /// <summary>
        /// Get all the level caps
        /// </summary>
        /// <returns></returns>
        public static List<PlayerLevelCap> GetLevelCaps()
        {
            List<PlayerLevelCap> levelCaps = new List<PlayerLevelCap>();

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                levelCaps = uberStrikeDb.PlayerLevelCaps.OrderBy(l => l.Level).ToList();
            }

            return levelCaps;
        }

        /// <summary>
        /// Get all the level caps view
        /// </summary>
        /// <returns></returns>
        public static List<PlayerLevelCapView> GetLevelCapsView()
        {
            List<PlayerLevelCap> levelCaps = GetLevelCaps();
            List<PlayerLevelCapView> levelCapsView = levelCaps.ConvertAll(new Converter<PlayerLevelCap, PlayerLevelCapView>(l => new PlayerLevelCapView(l.PlayerLevelCapId, l.Level, l.XPRequired)));

            return levelCapsView;
        }

        /// <summary>
        /// Get all the level caps view ordered by levels
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, PlayerLevelCapView> GetLevelCapsViewOrdered()
        {
            Dictionary<int, PlayerLevelCapView> levelCapsOrdered = new Dictionary<int, PlayerLevelCapView>();

            List<PlayerLevelCapView> levelCaps = GetLevelCapsView();
            levelCapsOrdered = levelCaps.ToDictionary(l => l.Level, l => l);

            return levelCapsOrdered;
        }

        public static bool EditLevelCaps(List<PlayerLevelCapView> levelCapsView)
        {
            bool areEdited = false;

            // TODO Sanitize input

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<PlayerLevelCap> levelCaps = (from x in uberStrikeDb.PlayerLevelCaps
                                                  where levelCapsView.Select(e => e.PlayerLevelCapId).Contains(x.PlayerLevelCapId)
                                                select x).ToList();

                Dictionary<int, PlayerLevelCapView> levelCapsViewOrdered = levelCapsView.ToDictionary(e => e.PlayerLevelCapId, e => e);

                foreach (PlayerLevelCap levelCap in levelCaps)
                {
                    levelCap.XPRequired = levelCapsViewOrdered[levelCap.PlayerLevelCapId].XPRequired;
                }

                uberStrikeDb.SubmitChanges();

                areEdited = true;
            }

            return areEdited;
        }

        /// <summary>
        /// Get all the XP Events
        /// </summary>
        /// <returns></returns>
        public static List<PlayerXPEvent> GetXPEvents()
        {
            List<PlayerXPEvent> xpEvents = new List<PlayerXPEvent>();

            using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
            {
                xpEvents = uberStrikeDB.PlayerXPEvents.ToList();
            }

            return xpEvents;
        }

        public static List<PlayerXPEventView> GetXPEventsView()
        {
            List<PlayerXPEvent> xpEvents = GetXPEvents();
            List<PlayerXPEventView> xpEventsView = xpEvents.ConvertAll(new Converter<PlayerXPEvent, PlayerXPEventView>(x => new PlayerXPEventView(x.PlayerXPEventId, x.Name, x.XPMultiplier)));

            return xpEventsView;
        }

        /// <summary>
        /// Get all the XP Events Views ordered
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, PlayerXPEventView> GetXPEventsViewOrdered()
        {
            Dictionary<int, PlayerXPEventView> xpEventsOrdered = new Dictionary<int, PlayerXPEventView>();
            List<PlayerXPEvent> xpEvents = GetXPEvents();

            foreach (PlayerXPEvent xpEvent in xpEvents)
            {
                xpEventsOrdered.Add(xpEvent.PlayerXPEventId, new PlayerXPEventView(xpEvent.Name, xpEvent.XPMultiplier));
            }

            return xpEventsOrdered;
        }

        public static bool EditXPEvents(List<PlayerXPEventView> eventsView)
        {
            bool areEdited = false;

            // TODO Sanitize input

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<PlayerXPEvent> xpEvents = (from x in uberStrikeDb.PlayerXPEvents
                                                where eventsView.Select(e => e.PlayerXPEventId).Contains(x.PlayerXPEventId)
                                                select x).ToList();

                Dictionary<int, PlayerXPEventView> eventsViewOrdered = eventsView.ToDictionary(e => e.PlayerXPEventId, e => e);

                foreach (PlayerXPEvent xpEvent in xpEvents)
                {
                    xpEvent.Name = eventsViewOrdered[xpEvent.PlayerXPEventId].Name;
                    xpEvent.XPMultiplier = eventsViewOrdered[xpEvent.PlayerXPEventId].XPMultiplier;
                }

                uberStrikeDb.SubmitChanges();

                areEdited = true;
            }

            return areEdited;
        }

        /// <summary>
        /// Sets the level of a player and set the equivalent Xp
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="newLevel"></param>
        /// <param name="newXp"></param>
        /// <returns></returns>
        public static bool SetLevel(int cmid, int newLevel, out int newXp)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isSet = false;
                newXp = 0;

                PlayerLevelCap levelCap = uberStrikeDb.PlayerLevelCaps.SingleOrDefault(l => l.Level == newLevel);

                if (levelCap != null)
                {
                    User currentUser = Users.GetUser(cmid, uberStrikeDb);

                    if (currentUser != null)
                    {
                        currentUser.Level = newLevel;
                        currentUser.XP = levelCap.XPRequired + 1;

                        uberStrikeDb.SubmitChanges();
                        
                        isSet = true;
                        newXp = currentUser.XP;
                    }
                }

                return isSet;
            }
        }

        /// <summary>
        /// Ensure that levels and XP are matching by modifying the players' level
        /// </summary>
        /// <param name="debug"></param>
        public static string ResynchroniseLevelsBasedOnXp(bool debug)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            StringBuilder queries = new StringBuilder();
            StringBuilder debugInfo = new StringBuilder();

            List<PlayerLevelCapView> levelCaps = GetLevelCapsView();
            string sqlUpdateTemplate = "UPDATE [MvParadisePaintball].[dbo].[Users] SET [Level] = {0} WHERE [XP] >= {1} AND [XP] < {2} AND [CMID] <> 0";
            int level = 0;
            int startXp = 0;
            int endXp = 0;

            for (int i = 0; i < levelCaps.Count; i++)
            {
                startXp = levelCaps[i].XPRequired;
                endXp = startXp;

                if (i + 1 < levelCaps.Count)
                {
                    endXp = levelCaps[i + 1].XPRequired;
                }
                else
                {
                    endXp = startXp + 1;
                }

                level = levelCaps[i].Level;

                string query = String.Format(sqlUpdateTemplate, level, startXp, endXp);

                if (debug)
                {
                    debugInfo.AppendLine(query);
                }

                queries.Append(query);
                queries.AppendLine("<br />");
            }

            stopWatch.Stop();

            if (debug)
            {
                debugInfo.Append("[Duration: ");
                debugInfo.Append(CmuneLog.DisplayForLog((int) stopWatch.ElapsedMilliseconds / 1000, 5));
                debugInfo.Append("s]");

                CmuneLog.CustomLogToDefaultPath("ResynchroniseLevelsAndXP.log", debugInfo.ToString());
            }

            return queries.ToString();
        }

        /// <summary>
        /// Ensure that levels and XP are matching by modifying the players' xp
        /// </summary>
        /// <param name="debug"></param>
        public static string ResynchroniseXpBasedOnLevel(bool debug)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            StringBuilder debugInfo = new StringBuilder();
            StringBuilder queries = new StringBuilder();

            List<PlayerLevelCapView> levelCaps = GetLevelCapsView();
            levelCaps = levelCaps.Where(q => q.Level <= UberStrikeCommonConfig.LevelCap).ToList();

            string sqlUpdateTemplate = "UPDATE [MvParadisePaintball].[dbo].[Users] SET [XP] = {0} WHERE [Level] = {1} AND [CMID] <> 0";
            int level = 0;
            int xp = 0;


            for (int i = 0; i < levelCaps.Count; i++)
            {
                if (i + 1 < levelCaps.Count)
                {
                    xp = levelCaps[i + 1].XPRequired - 1;
                }
                else
                {
                    xp = levelCaps[i].XPRequired;
                }

                level = levelCaps[i].Level;

                string query = String.Format(sqlUpdateTemplate, xp, level);

                if (debug)
                {
                    debugInfo.AppendLine(query);
                }

                queries.Append(query);
                queries.AppendLine("<br />");
            }

            if (debug)
            {
                debugInfo.Append("[Duration: ");
                debugInfo.Append(CmuneLog.DisplayForLog((int)stopWatch.ElapsedMilliseconds / 1000, 5));
                debugInfo.Append("s]");

                CmuneLog.CustomLogToDefaultPath("ResynchroniseLevelsAndXP.log", debugInfo.ToString());
            }

            return queries.ToString();
        }

        #endregion

        #region Client download

        // TODO -> This region should move to a class stats

        /// <summary>
        /// Records the client download
        /// </summary>
        /// <param name="version"></param>
        /// <param name="channelType"></param>
        /// <param name="url"></param>
        public static void RecordClientDownload(string version, ChannelType channelType, string url)
        {
            DateTime today = DateTime.Now;
            today = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
            version = version.ToLower().Trim();

            using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
            {
                ClientDownloadStat downloadStat = GetClientDownloadStat(today, version, channelType, uberStrikeDB);

                if (downloadStat != null)
                {
                    // We already have a download stat for today for this channel type and version, in this case we need to increase the counter by one

                    downloadStat.DownloadCount += 1;
                }
                else
                {
                    // We need to create a download stat for today for this channel type and version

                    downloadStat = new ClientDownloadStat();

                    downloadStat.Channel = (int)channelType;
                    downloadStat.DownloadCount = 1;
                    downloadStat.DownloadDate = today;
                    downloadStat.FileName = url;
                    downloadStat.Version = version;

                    uberStrikeDB.ClientDownloadStats.InsertOnSubmit(downloadStat);
                }

                uberStrikeDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Get the Client download stats of the day for a specific channel and version
        /// </summary>
        /// <param name="date"></param>
        /// <param name="version"></param>
        /// <param name="channelType"></param>
        /// <param name="uberStrikeDB"></param>
        /// <returns></returns>
        public static ClientDownloadStat GetClientDownloadStat(DateTime date, string version, ChannelType channelType, UberstrikeDataContext uberStrikeDB)
        {
            ClientDownloadStat downloadStat = null;

            if (uberStrikeDB != null)
            {
                date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                version = version.ToLower().Trim();

                downloadStat = uberStrikeDB.ClientDownloadStats.SingleOrDefault(c => c.Channel == (int)channelType && c.DownloadDate == date && c.Version == version);
            }

            return downloadStat;
        }

        #endregion Client download

        #region Facebook

        /// <summary>
        /// Retrieves the basic statistics used to create the social carrousel
        /// </summary>
        /// <param name="facebookUsers">The viewer should be the 1st element of the list</param>
        /// <param name="friendsDisplayedCount"></param>
        /// <returns></returns>
        public static List<FacebookBasicStatisticView> GetFacebookBasicStatistics(List<FacebookBasicStatisticView> facebookUsers, int friendsDisplayedCount)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<FacebookBasicStatisticView> views = new List<FacebookBasicStatisticView>();

                List<int> cmids = CmuneMember.GetCmidByFacebookId(facebookUsers.Select(q => q.FacebookId).ToList());

                var stats = from u in uberStrikeDb.Users where cmids.Contains(u.CMID) select new { Cmid = u.CMID, u.Level, u.XP };
                Dictionary<int, string> names = CmuneMember.GetMembersNames(cmids);

                foreach (var facebookUser in facebookUsers)
                {
                    var stat = stats.SingleOrDefault(v => v.Cmid == facebookUser.Cmid);
                    FacebookBasicStatisticView view = null;

                    if (stat != null)
                    {
                        if (!names.ContainsKey(stat.Cmid))
                        {
                            names[stat.Cmid] = "Unknown";
                        }
                        view = new FacebookBasicStatisticView(facebookUser.FacebookId, facebookUser.FirstName, facebookUser.PicturePath, names[stat.Cmid], stat.XP, stat.Level, stat.Cmid);
                    }
                    else
                    {
                        view = new FacebookBasicStatisticView(facebookUser.FacebookId, facebookUser.FirstName, facebookUser.PicturePath);
                    }

                    views.Add(view);
                }

                views = FacebookBasicStatisticView.Rank(views, friendsDisplayedCount);

                return views;
            }
        }

        #endregion Facebook

        #region Cyworld

        /// <summary>
        /// Retrieves the basic statistics used to create the social carrousel
        /// </summary>
        /// <param name="cyworldIds">The viewer should be the 1st element of the list</param>
        /// <param name="friendsDisplayedCount"></param>
        /// <returns></returns>
        public static List<CyworldBasicStatisticView> GetCyworldBasicStatistics(List<int> cyworldIds, int friendsDisplayedCount)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<CyworldBasicStatisticView> views = new List<CyworldBasicStatisticView>();

                var stats = from cyworlds in uberStrikeDb.Cyworlds join users in uberStrikeDb.Users on cyworlds.UserId equals users.UserID where cyworldIds.Contains(cyworlds.CyworldId) && users.CMID != 0 select new { Cmid = users.CMID, users.Level, users.XP, CyworldId = cyworlds.CyworldId };
                List<int> cmids = stats.Select(s => s.Cmid).ToList();
                Dictionary<int, string> names = CmuneMember.GetMembersNames(cmids);

                foreach (int cyworldId in cyworldIds)
                {
                    var stat = stats.SingleOrDefault(v => v.CyworldId == cyworldId);
                    CyworldBasicStatisticView view = null;

                    if (stat != null)
                    {
                        view = new CyworldBasicStatisticView(cyworldId, names[stat.Cmid], stat.XP, stat.Level, stat.Cmid);
                    }
                    else
                    {
                        view = new CyworldBasicStatisticView(cyworldId);
                    }

                    views.Add(view);
                }

                views = CyworldBasicStatisticView.Rank(views, friendsDisplayedCount);

                return views;
            }
        }

        #endregion Cyworld
    }
}