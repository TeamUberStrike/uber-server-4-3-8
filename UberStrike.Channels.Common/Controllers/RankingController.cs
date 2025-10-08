using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.Channels.Common.Models;
using UberStrike.Channels.Utils;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Business;
using System.Web.Caching;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace UberStrike.Channels.Common.Controllers
{
    public class RankingController : Controller
    {
        public const int CountPerPage = 100;

        public ActionResult Index(ChannelType channelType = ChannelType.WebFacebook, string appCallbackUrl = "")
        {
            if (appCallbackUrl == "")
            {
                appCallbackUrl = ConfigurationUtilities.ReadConfigurationManager("PortalChannel") + "/Profile";
            }
            DateTime allTimeRankingDate;
            ViewBag.AllTimeRanking = new RankingDisplay(LoadAllTimeRanking(1, out allTimeRankingDate), new PaginationModel(1000, 1, "AllTimeRanking", 100), "allTime", allTimeRankingDate);

            DateTime dailyRankingDate;
            ViewBag.DailyRanking = new RankingDisplay(LoadDailyRanking(1, out dailyRankingDate), new PaginationModel(1000, 1, "DailyRanking", 100), "daily", dailyRankingDate);
            ViewBag.ChannelType = channelType;
            ViewBag.AppCallbackUrl = HttpUtility.UrlDecode(appCallbackUrl);
            return View();
        }

        static List<RankingView> LoadAllTimeRanking(int page, out DateTime rankingDate)
        {
            rankingDate = DateTime.Now;

            List<RankingView> ranking = new List<RankingView>();

            string cacheName = String.Format("RankingController.AllTimeRanking.{0}", page);

            if (HttpRuntime.Cache[cacheName] != null)
            {
                ranking = (List<RankingView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                List<AllTimeTotalRanking> rankingData = Statistics.GetAllTimeRanking(page, CountPerPage);

                // Dirty hack as the level / XP are not yet in the ranking

                using (var uberStrikeDb = new UberstrikeDataContext())
                {
                    var query = from u in uberStrikeDb.Users
                                where rankingData.Select(r => r.CMID).Contains(u.CMID)
                                select new { Cmid = u.CMID, Level = u.Level, Xp = u.XP };

                    Dictionary<int, int> levels = query.ToDictionary(u => u.Cmid, u => u.Level);
                    Dictionary<int, int> xps = query.ToDictionary(u => u.Cmid, u => u.Xp);

                    foreach (AllTimeTotalRanking currentTop in rankingData)
                    {
                        int level = 0;
                        int xp = 0;

                        levels.TryGetValue(currentTop.CMID, out level);
                        xps.TryGetValue(currentTop.CMID, out xp);

                        ranking.Add(new RankingView { Cmid = currentTop.CMID, ClanTag = currentTop.TagName, Deaths = (int)currentTop.Splatted, Kills = (int)currentTop.Splats, Level = level, Name = currentTop.Name, Rank = currentTop.Ranknum, Xp = xp });
                    }
                }

                HttpRuntime.Cache.Add(cacheName, ranking, null, DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return ranking;
        }

        static List<RankingView> LoadTop100WeeklyUsers(out bool isCached)
        {
            isCached = false;

            List<RankingView> top100 = new List<RankingView>();

            string cacheName = "RankingController.Top100Weekly";

            if (HttpRuntime.Cache[cacheName] != null)
            {
                isCached = true;

                top100 = (List<RankingView>)HttpRuntime.Cache[cacheName];
            }
            else
            {
                List<WeeklyTotalRanking> top100Data = Statistics.GetTopWeekly100Users();

                // Dirty hack as the level / XP are not yet in the ranking

                using (var uberStrikeDb = new UberstrikeDataContext())
                {
                    var query = from u in uberStrikeDb.Users
                                where top100Data.Select(r => r.CMID).Contains(u.CMID)
                                select new { Cmid = u.CMID, Level = u.Level, Xp = u.XP };

                    Dictionary<int, int> levels = query.ToDictionary(u => u.Cmid, u => u.Level);
                    Dictionary<int, int> xps = query.ToDictionary(u => u.Cmid, u => u.Xp);

                    foreach (WeeklyTotalRanking currentTop in top100Data)
                    {
                        int level = 0;
                        int xp = 0;

                        levels.TryGetValue(currentTop.CMID, out level);
                        xps.TryGetValue(currentTop.CMID, out xp);

                        top100.Add(new RankingView { Cmid = currentTop.CMID, ClanTag = currentTop.TagName, Deaths = (int)currentTop.Splatted, Kills = (int)currentTop.Splats, Level = level, Name = currentTop.Name, Rank = currentTop.Ranknum, Xp = xp });
                    }
                }

                HttpRuntime.Cache.Add(cacheName, top100, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return top100;
        }

        static List<RankingView> LoadDailyRanking(int page, out DateTime rankingDate)
        {
            rankingDate = DateTime.Now;

            List<RankingView> ranking = new List<RankingView>();

            string cacheName = String.Format("RankingController.DailyRanking.{0}", page);
            string dateCacheName = String.Format("RankingController.DailyRanking.Date.{0}", page);

            //if (HttpRuntime.Cache[cacheName] != null && HttpRuntime.Cache[dateCacheName] != null)
            //{
            //    ranking = (List<RankingView>)HttpRuntime.Cache[cacheName];
            //    rankingDate = (DateTime)HttpRuntime.Cache[dateCacheName];
            //}
            //else
            //{
                List<DailyRanking> top100Data = Statistics.GetDailyRanking(page, CountPerPage, out rankingDate);
                ranking = top100Data.ConvertAll(new Converter<DailyRanking, RankingView>(r => new RankingView { Cmid = r.Cmid, ClanTag = r.ClanTag, Deaths = r.Deaths, Kills = r.Kills, Level = r.Level, Name = r.Name, Rank = r.DailyRankingIndex, Xp = r.TotalXP }));

            //    HttpRuntime.Cache.Add(cacheName, ranking, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            //    HttpRuntime.Cache.Add(dateCacheName, rankingDate, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            //}

            return ranking;
        }

        public ActionResult GetDailyRanking(int page = 1, string appCallbackUrl = "")
        {
            DateTime uselessField;

            if (page < 1)
            {
                page = 1;
            }
            else if (page > 10)
            {
                page = 10;
            }

            ViewBag.AppCallbackUrl = HttpUtility.UrlDecode(appCallbackUrl);
            return PartialView("_Ranking", new RankingDisplay(LoadDailyRanking(page, out uselessField), new PaginationModel(1000, page, "DailyRanking", 100), "daily", uselessField));
        }

        public ActionResult GetAllTimeRanking(int page, string appCallbackUrl = "")
        {
            DateTime uselessField;

            if (page < 1)
            {
                page = 1;
            }
            else if (page > 10)
            {
                page = 10;
            }
            ViewBag.AppCallbackUrl = HttpUtility.UrlDecode(appCallbackUrl);
            return PartialView("_Ranking", new RankingDisplay(LoadAllTimeRanking(page, out uselessField), new PaginationModel(1000, page, "AllTimeRanking", 100), "allTime", uselessField));
        }
    }


}
