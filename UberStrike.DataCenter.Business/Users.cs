using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Forum.Business;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using PPUser = UberStrike.DataCenter.DataAccess.User;

namespace UberStrike.DataCenter.Business
{
    public static class Users
    {
        #region Tracking

        // TODO Tracking related functions should not be in Users class but in the tracking class

        /// <summary>
        /// Retrieves all the IPs (network address) used by a member ordered by date desc
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<LoginIp> GetMemberIps(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<LoginIp> memberIps = cmuneDb.LoginIps.Where(lI => lI.Cmid == cmid && lI.LoginDate >= DateTime.Now.AddDays(-30).ToDateOnly()).OrderByDescending(lI => lI.LoginIpId).ToList();
                Dictionary<long, LoginIp> memberIpsIndexedByIp = new Dictionary<long, LoginIp>();

                foreach (LoginIp memberIp in memberIps)
                {
                    if (!memberIpsIndexedByIp.ContainsKey(memberIp.Ip))
                    {
                        memberIpsIndexedByIp.Add(memberIp.Ip, memberIp);
                    }
                }

                return memberIpsIndexedByIp.Values.ToList();
            }
        }

        public static List<LastIpView> GetLinkedAccounts(int cmid)
        {
            List<LastIpView> lastIps = new List<LastIpView>();

            try
            {
                List<LoginIp> memberIps = GetMemberIps(cmid);

                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    Dictionary<int, long> otherCmids = new Dictionary<int, long>();

                    List<LoginIp> loginIps = cmuneDb.LoginIps.Where(lI => memberIps.Select(u => u.Ip).ToList().Contains(lI.Ip) && lI.Cmid != cmid && lI.LoginDate >= DateTime.Now.AddDays(-30).ToDateOnly()).ToList();

                    Dictionary<int, string> linkedNames = CmuneMember.GetMembersNames(loginIps.GroupBy(p => p.Cmid).Select(g => g.First()).Select(u => u.Cmid).ToList());

                    Dictionary<long, Dictionary<int, int>> cmidsLinkedToIp = new Dictionary<long, Dictionary<int, int>>();

                    foreach (LoginIp loginIp in loginIps)
                    {
                        if (!cmidsLinkedToIp.ContainsKey(loginIp.Ip))
                        {
                            cmidsLinkedToIp.Add(loginIp.Ip, new Dictionary<int, int> { { loginIp.Cmid, loginIp.Cmid } });
                        }
                        else
                        {
                            if (!cmidsLinkedToIp[loginIp.Ip].ContainsKey(loginIp.Cmid))
                            {
                                cmidsLinkedToIp[loginIp.Ip].Add(loginIp.Cmid, loginIp.Cmid);
                            }
                        }
                    }

                    Dictionary<long, LoginIp> memberIpsByIp = memberIps.ToDictionary(u => u.Ip, u => u);

                    Dictionary<long, BannedIpView> bannedIpViews = CmuneMember.GetBannedIpsViews(memberIpsByIp.Keys.ToList());

                    foreach (long ip in memberIps.Select(u => u.Ip).ToList())
                    {
                        List<LinkedMemberView> linkedMembers = new List<LinkedMemberView>();

                        if (cmidsLinkedToIp.ContainsKey(ip))
                        {
                            foreach (int otherAccountCmid in cmidsLinkedToIp[ip].Keys)
                            {
                                linkedMembers.Add(new LinkedMemberView(otherAccountCmid, linkedNames[otherAccountCmid]));
                            }
                        }

                        BannedIpView bannedIpView = null;
                        BannedIpView bannedIpViewTmp = null;

                        if (bannedIpViews.TryGetValue(ip, out bannedIpViewTmp))
                        {
                            bannedIpView = bannedIpViewTmp;
                        }

                        LastIpView lastIp = new LastIpView(ip, memberIpsByIp[ip].LoginDate, linkedMembers, bannedIpView);
                        lastIps.Add(lastIp);
                    }
                }
            }
            catch (SqlException ex)
            {
                CmuneLog.LogUnexpectedReturn(ex, String.Format("cmid={0}", cmid));
                throw;
            }

            return lastIps;
        }

        #endregion Tracking

        #region Detects XP farmer

        private static List<RankingView> DetectHighKdr(int dayOfTheMonth, int killThreshold, int kdrThreshold)
        {
            List<RankingView> rankingView = new List<RankingView>();

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var query = from r in uberStrikeDb.DailyRankings
                            where r.Kills > r.Deaths * kdrThreshold && r.Kills > killThreshold && r.Day == dayOfTheMonth
                            orderby r.Kills descending
                            select new { Kills = r.Kills, Death = r.Deaths, Cmid = r.Cmid, Name = r.Name };

                foreach (var row in query)
                {
                    rankingView.Add(new RankingView { Cmid = row.Cmid, Deaths = row.Death, Kills = row.Kills, Name = row.Name });
                }
            }

            return rankingView;
        }

        private static List<RankingView> DetectLowKdr(int dayOfTheMonth, int deathThreshold, int kdrThreshold)
        {
            List<RankingView> rankingView = new List<RankingView>();

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var query = from r in uberStrikeDb.DailyRankings
                            where r.Deaths > r.Kills * kdrThreshold && r.Deaths > deathThreshold && r.Day == dayOfTheMonth
                            orderby r.Deaths descending
                            select new { Kills = r.Kills, Death = r.Deaths, Cmid = r.Cmid, Name = r.Name };

                foreach (var row in query)
                {
                    rankingView.Add(new RankingView { Cmid = row.Cmid, Deaths = row.Death, Kills = row.Kills, Name = row.Name });
                }
            }

            return rankingView;
        }

        /// <summary>
        /// Detects XP farming using the daily ranking (can only detects accounts linke by IP)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="killThreshold"></param>
        /// <param name="kdrThreshold"></param>
        /// <returns></returns>
        public static Dictionary<RankingView, List<RankingView>> GetXpFarmers(DateTime date, int killThreshold = 400, int kdrThreshold = 8)
        {
            int day = date.Day;

            List<RankingView> highKdrs = DetectHighKdr(day, killThreshold, kdrThreshold);
            List<RankingView> lowKdrs = DetectLowKdr(day, killThreshold, kdrThreshold);
            Dictionary<int, RankingView> lowKdrsByCmid = lowKdrs.ToDictionary(u => u.Cmid);

            Dictionary<int, List<long>> highKdrsIps = GetAccountsIp(highKdrs.Select(u => u.Cmid).ToList(), date);
            Dictionary<long, List<int>> lowKdrsIps = GetAccountsIpByIp(lowKdrs.Select(u => u.Cmid).ToList(), date);

            Dictionary<RankingView, List<RankingView>> xpFarmers = new Dictionary<RankingView, List<RankingView>>();

            foreach (RankingView highKdr in highKdrs)
            {
                if (highKdrsIps.ContainsKey(highKdr.Cmid))
                {
                    List<long> ips = highKdrsIps[highKdr.Cmid];

                    foreach (long ip in ips)
                    {
                        if (lowKdrsIps.ContainsKey(ip))
                        {
                            if (xpFarmers.ContainsKey(highKdr))
                            {
                                foreach (int cmid in lowKdrsIps[ip])
                                {
                                    xpFarmers[highKdr].Add(lowKdrsByCmid[cmid]);
                                }
                            }
                            else
                            {
                                int i = 0;

                                foreach (int cmid in lowKdrsIps[ip])
                                {
                                    if (i == 0)
                                    {
                                        xpFarmers.Add(highKdr, new List<RankingView> { lowKdrsByCmid[cmid] });
                                    }
                                    else
                                    {
                                        xpFarmers[highKdr].Add(lowKdrsByCmid[cmid]);
                                    }

                                    i++;
                                }
                            }
                        }
                    }
                }
            }

            return xpFarmers;
        }

        /// <summary>
        /// Detects Xp farming when people are scoring
        /// </summary>
        /// <param name="matchView"></param>
        /// <param name="farmerKillThreshold"></param>
        /// <param name="farmerKdrThreshold"></param>
        /// <param name="plantDeathThreshold"></param>
        /// <param name="plantKdrThreshold"></param>
        public static void DetectXpFarmers(MatchView matchView, int farmerKillThreshold = 30, decimal farmerKdrThreshold = 8, int plantDeathThreshold = 15, decimal plantKdrThreshold = 0.3m)
        {
            if (matchView != null)
            {
                List<PlayerStatisticsView> allPlayers = new List<PlayerStatisticsView>();
                allPlayers.AddRange(matchView.PlayersCompleted);
                allPlayers.AddRange(matchView.PlayersNonCompleted);

                List<PlayerStatisticsView> potentialFarmers = new List<PlayerStatisticsView>();

                foreach (PlayerStatisticsView stats in allPlayers)
                {
                    decimal kdr = farmerKdrThreshold + 1;

                    if (stats.Splatted != 0)
                    {
                        kdr = (decimal)stats.Splats / (decimal)stats.Splatted;
                    }

                    if (stats.Splats >= farmerKillThreshold && kdr >= farmerKdrThreshold)
                    {
                        potentialFarmers.Add(stats);
                    }
                }

                List<PlayerStatisticsView> potentialPlants = new List<PlayerStatisticsView>();

                foreach (PlayerStatisticsView stats in allPlayers)
                {
                    decimal kdr = plantKdrThreshold + 1;

                    if (stats.Splatted != 0)
                    {
                        kdr = (decimal)stats.Splats / (decimal)stats.Splatted;
                    }

                    if (stats.Splatted >= plantDeathThreshold && kdr <= plantKdrThreshold)
                    {
                        potentialPlants.Add(stats);
                    }
                }

                DateTime reportTime = DateTime.Now;

                string subjectTemplate = "Potential Xp Farmer: Cmid {0}";
                string bodyHtmlTemplate = "<html><body><p>Cmid <a href=\"{1}Member/See?Cmid={0}\">{0}</a> is suspected of farming ({2}):</p><ul><li><b>Kills</b>: {3}</li><li><b>Deaths</b>: {4}</li><li><b>KDR</b>: {5}</li></ul><p>He's being naughty with the potential plants ({11} players in game):</p><ul>{6}</ul><p>Detection performed with the following settings:<ul><li>farmerKillThreshold: {7}</li><li>farmerKdrThreshold: {8}</li><li>plantDeathThreshold: {9}</li><li>plantKdrThreshold: {10}</li></ul>###XpFarmingAutoDetection###</p></body></html>";
                string bodyTextTemplate = "Cmid {0} ({1}Member/See?Cmid={0}) is suspected of farming ({2}):\n\nKills: {3}\nDeaths: {4}\nKDR: {5}\n\nHe's being naughty with the potential plants ({11} players in game):\n\n{6}\n\nDetection performed with the following settings: farmerKillThreshold={7}&farmerKdrThreshold{8}&plantDeathThreshold={9}&plantKdrThreshold={10}\n\n###XpFarmingAutoDetection###";
                string plantHtmlTemplate = "<li>Cmid: <a href=\"{1}Member/See?Cmid={0}\">{0}</a> - Kills: {2} - Deaths: {3} - KDR: {4}</li>";
                string plantTextTemplate = "Cmid: {0} ({1}Member/See?Cmid={0}) - Kills: {2} - Deaths: {3} - KDR: {4}\n";

                StringBuilder plantsHtmlContent = new StringBuilder();
                StringBuilder plantsTextContent = new StringBuilder();

                string instrumentationUrl = ConfigurationUtilities.ReadConfigurationManager("InstrumentationUrl");

                foreach (PlayerStatisticsView stats in potentialPlants)
                {
                    decimal kdr = 0;

                    if (stats.Splatted > 0)
                    {
                        kdr = (decimal)stats.Splats / (decimal)stats.Splatted;
                    }

                    plantsHtmlContent.Append(String.Format(plantHtmlTemplate, stats.Cmid, instrumentationUrl, stats.Splats, stats.Splatted, kdr.ToString("N2")));
                    plantsTextContent.Append(String.Format(plantTextTemplate, stats.Cmid, instrumentationUrl, stats.Splats, stats.Splatted, kdr.ToString("N2")));
                }

                if (potentialPlants.Count > 0 && (decimal)potentialPlants.Count / (decimal)allPlayers.Count >= 0.5m)
                {
                    foreach (PlayerStatisticsView stats in potentialFarmers)
                    {
                        decimal kdr = stats.Splats;

                        if (stats.Splatted > 0)
                        {
                            kdr = (decimal)stats.Splats / (decimal)stats.Splatted;
                        }

                        string subject = String.Format(subjectTemplate, stats.Cmid);
                        string htmlBody = String.Format(bodyHtmlTemplate, stats.Cmid, instrumentationUrl, reportTime.ToString("MM/dd/yyyy HH:mm:ss 'GMT'"), stats.Splats, stats.Splatted, kdr.ToString("N2"), plantsHtmlContent.ToString(), farmerKillThreshold, farmerKdrThreshold, plantDeathThreshold, plantKdrThreshold, allPlayers.Count);
                        string textBody = String.Format(bodyTextTemplate, stats.Cmid, instrumentationUrl, reportTime.ToString("MM/dd/yyyy HH:mm:ss 'GMT'"), stats.Splats, stats.Splatted, kdr.ToString("N2"), plantsTextContent.ToString(), farmerKillThreshold, farmerKdrThreshold, plantDeathThreshold, plantKdrThreshold, allPlayers.Count);

                        CmuneMail.SendEmail(CommonConfig.CmuneDevteamEmail, CommonConfig.CmuneDevteamEmailName, CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, subject, htmlBody, textBody);
                    }
                }
            }
        }

        // TODO: to be renamed
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmids"></param>
        /// <param name="loginDate"></param>
        /// <returns>Cmids as key, IP(s) as value</returns>
        private static Dictionary<int, List<long>> GetAccountsIp(List<int> cmids, DateTime loginDate)
        {
            Dictionary<int, List<long>> accountIps = new Dictionary<int, List<long>>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = (from l in cmuneDb.LoginIps
                             where l.LoginDate.Date == loginDate.Date && cmids.Contains(l.Cmid)
                             select new { Cmid = l.Cmid, Ip = l.Ip }).Distinct();

                foreach (var row in query)
                {
                    if (accountIps.ContainsKey(row.Cmid))
                    {
                        accountIps[row.Cmid].Add(row.Ip);
                    }
                    else
                    {
                        accountIps.Add(row.Cmid, new List<long> { row.Ip });
                    }
                }
            }

            return accountIps;
        }

        private static Dictionary<long, List<int>> GetAccountsIpByIp(List<int> cmids, DateTime loginDate)
        {
            Dictionary<long, List<int>> accountIps = new Dictionary<long, List<int>>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = (from l in cmuneDb.LoginIps
                             where l.LoginDate.Date == loginDate.Date && cmids.Contains(l.Cmid)
                             select new { Cmid = l.Cmid, Ip = l.Ip }).Distinct();

                foreach (var row in query)
                {
                    if (accountIps.ContainsKey(row.Ip))
                    {
                        accountIps[row.Ip].Add(row.Cmid);
                    }
                    else
                    {
                        accountIps.Add(row.Ip, new List<int> { row.Cmid });
                    }
                }
            }

            return accountIps;
        }

        #endregion

        #region Get user

        /// <summary>
        /// Return a READ ONLY Paradise Paintball User thanks to a CMID
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static User GetUser(int cmid)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                return GetUser(cmid, ppDB);
            }
        }

        /// <summary>
        /// Return a Paradise Paintball User thanks to a CMID
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="ppDB"></param>
        /// <returns></returns>
        public static User GetUser(int cmid, UberstrikeDataContext ppDB)
        {
            User currentUser = null;

            if (ppDB != null && cmid > 0)
            {
                currentUser = ppDB.Users.SingleOrDefault<User>(u => u.CMID == cmid);
            }

            return currentUser;
        }

        /// <summary>
        /// Get a MySpace row link to a MySpace Id
        /// READ ONLY
        /// </summary>
        /// <param name="mySpaceId"></param>
        /// <returns></returns>
        public static MySpace GetMySpace(int mySpaceId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                return GetMySpace(mySpaceId, uberStrikeDb);
            }
        }

        /// <summary>
        /// Get a MySpace row link to a MySpace Id
        /// </summary>
        /// <param name="mySpaceId"></param>
        /// <param name="uberStrikeDb"></param>
        /// <returns></returns>
        public static MySpace GetMySpace(int mySpaceId, UberstrikeDataContext uberStrikeDb)
        {
            MySpace mySpace = null;

            if (uberStrikeDb != null)
            {
                mySpace = uberStrikeDb.MySpaces.SingleOrDefault(mS => mS.MySpaceID == mySpaceId);
            }

            return mySpace;
        }

        /// <summary>
        /// Get a Cyworld row link to a Cyworld Id
        /// READ ONLY
        /// </summary>
        /// <param name="cyworldId"></param>
        /// <returns></returns>
        public static Cyworld GetCyworld(int cyworldId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                return GetCyworld(cyworldId, uberStrikeDb);
            }
        }

        /// <summary>
        /// Get a MySpace row link to a MySpace ID
        /// </summary>
        /// <param name="cyworldId"></param>
        /// <param name="uberStrikeDb"></param>
        /// <returns></returns>
        public static Cyworld GetCyworld(int cyworldId, UberstrikeDataContext uberStrikeDb)
        {
            Cyworld cyworld = null;

            if (uberStrikeDb != null)
            {
                cyworld = uberStrikeDb.Cyworlds.SingleOrDefault(c => c.CyworldId == cyworldId);
            }

            return cyworld;
        }

        /// <summary>
        /// Return a Paradise Paintball ID list thanks to a Cmune ID list
        /// </summary>
        /// <param name="cmuneIdList"></param>
        /// <returns></returns>
        public static List<int> GetUsersFromCmuneIdList(List<int> cmuneIdList)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                List<int> userFriends = (from tab in ppDB.Users where cmuneIdList.Contains(tab.CMID) select tab.UserID).ToList();

                return userFriends;
            }
        }

        /// <summary>
        /// Return a READ ONLY Paradise Paintball Users list thanks to a Cmune ID List user
        /// </summary>
        /// <param name="cmuneIdList"></param>
        /// <returns></returns>
        public static List<User> GetUsersFromCmuneIdListUser(List<int> cmuneIdList)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                List<User> userFriends = (from tab in ppDB.Users where cmuneIdList.Contains(tab.CMID) select tab).ToList();

                return userFriends;
            }
        }

        /// <summary>
        /// Return a Paradise Paintball Users list thanks to a Cmune ID List user
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="ppDB"></param>
        /// <returns></returns>
        public static List<User> GetUsers(List<int> cmid, UberstrikeDataContext ppDB)
        {
            List<User> users = (from tab in ppDB.Users where cmid.Contains(tab.CMID) select tab).ToList();

            return users;
        }

        /// <summary>
        /// Gets a Paradise Paintball member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static UberstrikeMemberView GetMember(int cmid)
        {
            UberstrikeMemberView memberView = null;
            PlayerCardView playerCardView = Statistics.GetStatisticsView(cmid);
            PlayerStatisticsView statistics = Statistics.GetCompleteStatisticsView(cmid);

            memberView = new UberstrikeMemberView(playerCardView, statistics);

            return memberView;
        }

        #endregion

        #region Data synchronization

        /// <summary>
        /// Changes the member password
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool ChangePassword(int cmid, string newPassword)
        {
            bool ret = false;

            if (ValidationUtilities.IsValidPassword(newPassword))
            {
                newPassword = CryptographyUtilities.HashPassword(newPassword);

                ret = CmuneMember.ChangeMemberPassword(cmid, newPassword);
                if (ret)
                {
                    ret = ChangePasswordMiniverse(cmid, newPassword);
                }
            }

            return ret;
        }

        /// <summary>
        /// Change the member password by providing a hashed password
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="newHashedPassword"></param>
        /// <returns></returns>
        public static bool ChangeHashedPassword(int cmid, string newHashedPassword)
        {
            bool ret = false;

            ret = CmuneMember.ChangeMemberPassword(cmid, newHashedPassword);
            if (ret)
            {
                ret = ChangePasswordMiniverse(cmid, newHashedPassword);
            }
            return ret;
        }

        /// <summary>
        /// Change the password of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="newMemberPassword"></param>
        /// <returns></returns>
        public static bool ChangePasswordMiniverse(int cmid, string newMemberPassword)
        {
            bool ret = false;

            if (!CmuneMember.IsMemberEsnsOnly(cmid))
            {
                MemberOperationResult forumReturn = ForumMember.ChangePassword(cmid, newMemberPassword);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Change the email in all the part of the miniverse
        /// </summary>
        /// <param name="newEmail"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeEmailMiniverse(String newEmail, int cmid, bool checkDuplicate)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                ret = MemberOperationResult.InvalidCmid;
            }
            else if (!ValidationUtilities.IsValidEmailAddress(newEmail))
            {
                ret = MemberOperationResult.InvalidEmail;
            }

            #endregion

            MemberOperationResult isDuplicateEmail = MemberOperationResult.Ok;
            newEmail = ValidationUtilities.StandardizeEmail(newEmail);

            if (checkDuplicate && CmuneMember.IsDuplicateUserEmail(newEmail))
            {
                isDuplicateEmail = MemberOperationResult.DuplicateEmail;
            }

            if (isDuplicateEmail.Equals(MemberOperationResult.Ok))
            {
                ret = ForumMember.ChangeEmail(newEmail, cmid, checkDuplicate);
            }

            return ret;
        }

        /// <summary>
        /// Change the user name in all the part of the miniverse
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <param name="isMemberEsnsOnly"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeUsernameMiniverse(string newName, int cmid, bool checkDuplicate, bool isMemberEsnsOnly)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
            }
            newName = ValidationUtilities.StandardizeMemberName(newName);
            if (TextUtilities.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException("newUsername", "The new user name should not be NULL or empty and should not contain only forbidden characters.");
            }

            #endregion

            MemberOperationResult isDuplicateUsername = MemberOperationResult.Ok;

            if (checkDuplicate && CmuneMember.IsDuplicateUserName(newName))
            {
                isDuplicateUsername = MemberOperationResult.DuplicateName;
            }

            if (isDuplicateUsername.Equals(MemberOperationResult.Ok))
            {
                ret = MemberOperationResult.Ok;

                PropagateName(newName, cmid);

                if (!isMemberEsnsOnly)
                {
                    ret = ForumMember.ChangeUsername(newName, cmid, checkDuplicate);
                }
            }

            return ret;
        }

        #endregion

        #region Create user

        /// <summary>
        /// Create a UberStrike user
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        private static int CreateUser(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                var uberStrikeUser = new User()
                {
                    CannonTotalDamageDone = 0,
                    CannonTotalShotsFired = 0,
                    CannonTotalShotsHit = 0,
                    CannonTotalSplats = 0,
                    CMID = cmid,
                    HandgunTotalDamageDone = 0,
                    HandgunTotalShotsFired = 0,
                    HandgunTotalShotsHit = 0,
                    HandgunTotalSplats = 0,
                    Headshots = 0,
                    Hits = 0,
                    LauncherTotalDamageDone = 0,
                    LauncherTotalShotsFired = 0,
                    LauncherTotalShotsHit = 0,
                    LauncherTotalSplats = 0,
                    Level = UberStrikeCommonConfig.DefaultLevel,
                    MachineGunTotalDamageDone = 0,
                    MachineGunTotalShotsFired = 0,
                    MachineGunTotalShotsHit = 0,
                    MachineGunTotalSplats = 0,
                    MeleeTotalDamageDone = 0,
                    MeleeTotalShotsFired = 0,
                    MeleeTotalShotsHit = 0,
                    MeleeTotalSplats = 0,
                    MostArmorPickedUp = 0,
                    MostCannonSplats = 0,
                    MostConsecutiveSnipes = 0,
                    MostDamageDealt = 0,
                    MostDamageReceived = 0,
                    MostHandgunSplats = 0,
                    MostHeadshots = 0,
                    MostHealthPickedUp = 0,
                    MostLauncherSplats = 0,
                    MostMachinegunSplats = 0,
                    MostMeleeSplats = 0,
                    MostNutshots = 0,
                    MostShotgunSplats = 0,
                    MostSniperSplats = 0,
                    MostSplats = 0,
                    MostSplattergunSplats = 0,
                    MostXPEarned = 0,
                    Nutshots = 0,
                    Points = 0,
                    ShotgunTotalDamageDone = 0,
                    ShotgunTotalShotsFired = 0,
                    ShotgunTotalShotsHit = 0,
                    ShotgunTotalSplats = 0,
                    Shots = 0,
                    SniperTotalDamageDone = 0,
                    SniperTotalShotsFired = 0,
                    SniperTotalShotsHit = 0,
                    SniperTotalSplats = 0,
                    Splats = 0,
                    Splatted = 0,
                    SplattergunTotalDamageDone = 0,
                    SplattergunTotalShotsFired = 0,
                    SplattergunTotalShotsHit = 0,
                    SplattergunTotalSplats = 0,
                    TimeSpentInGame = 0,
                    XP = 0,
                };

                uberStrikeDb.Users.InsertOnSubmit(uberStrikeUser);
                uberStrikeDb.SubmitChanges();

                return uberStrikeUser.UserID;
            }
        }

        /// <summary>
        /// Create a Portal Referrer
        /// </summary>
        /// <param name="referrer"></param>
        /// <param name="cmid"></param>
        private static void CreatePortalReferrer(string referrer, int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                ReferrerPartnerType referrerAdvertising = GetReferrerSourceByUrlAndChannel(ChannelType.WebPortal, referrer);

                PortalReferrer portalReferrer = new PortalReferrer();
                portalReferrer.Cmid = cmid;
                portalReferrer.Referrer = referrer;
                portalReferrer.CreationDate = DateTime.Now;
                portalReferrer.ReferrerPartnerId = (int)referrerAdvertising;

                uberStrikeDb.PortalReferrers.InsertOnSubmit(portalReferrer);
                uberStrikeDb.SubmitChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="channelType"></param>
        /// <param name="networkAddress"></param>
        /// <param name="locale"></param>
        /// <param name="cmid"></param>
        /// <param name="referrer">Stand alone channels do not have a referrer</param>
        /// <returns>UberstrikeMemberRegistrationResult</returns>
        public static MemberRegistrationResult CreateUser(string emailAddress, string password, ChannelType channelType, long networkAddress, string locale, out int cmid, string referrer = "")
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;
            cmid = 0;
            string name = String.Empty;
            DateTime accountCreationDate;

            ret = CmuneMember.CreateUser(emailAddress, password, UberStrikeCommonConfig.ApplicationId, channelType, networkAddress, locale, UberStrikeCommonConfig.FirstLoadoutItemIds, out cmid, out name, out accountCreationDate);

            if (ret == MemberRegistrationResult.Ok)
            {
                CreateUserCommon(cmid, emailAddress, password, name);

                if (channelType == ChannelType.WebPortal)
                {
                    CreatePortalReferrer(referrer, cmid);
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        private static void CreateUserCommon(int cmid, string emailAddress, string password, string name)
        {
            CreateUser(cmid);

            CreateLoadout(cmid);

            //CmunePrivateMessages.SendAdminMessage(cmid, "Welcome UberStriker", UberStrikeCommonConfig.WelcomeMessage);

            try
            {
                ForumMember.AddUser(emailAddress, password, name, cmid, false);
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, "Failed to add member to forum on registration: cmid=" + cmid.ToString() + "&memberEmail=" + emailAddress + "&memberName=" + name);
            }
        }

        /// <summary>
        /// Create a Facebook Referrer
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="referrer"></param>
        /// <param name="facebookId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        private static void CreateFacebookReferrer(int cmid, string referrer, long facebookId, string firstName, string lastName)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                ReferrerPartnerType referrerPartner = GetReferrerSourceByUrlAndChannel(ChannelType.WebFacebook, referrer);

                FacebookReferrer facebookReferrer = new FacebookReferrer();
                facebookReferrer.FacebookID = facebookId;
                facebookReferrer.Referrer = referrer;
                facebookReferrer.CreationDate = DateTime.Now;
                facebookReferrer.ReferrerPartnerId = (int)referrerPartner;
                facebookReferrer.FirstName = firstName;
                facebookReferrer.LastName = lastName;
                facebookReferrer.Cmid = cmid;

                uberStrikeDb.FacebookReferrers.InsertOnSubmit(facebookReferrer);
                uberStrikeDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Create a kongregate referrer 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="referrer"></param>
        /// <param name="kongregateId"></param>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        private static void CreateKongregateReferrer(int cmid, string referrer, long kongregateId, string userName, string firstName, string lastName)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                ReferrerPartnerType referrerPartner = GetReferrerSourceByUrlAndChannel(ChannelType.Kongregate, referrer);

                KongregateReferrer kongregateReferrer = new KongregateReferrer();
                kongregateReferrer.KongregateId = kongregateId;
                kongregateReferrer.Referrer = (referrer == null) ? string.Empty : referrer;
                kongregateReferrer.CreationDate = DateTime.Now;
                kongregateReferrer.ReferrerPartnerId = (int)EsnsType.Kongregate;
                kongregateReferrer.UserName = userName;
                kongregateReferrer.FirstName = firstName;
                kongregateReferrer.LastName = lastName;
                kongregateReferrer.Cmid = cmid;

                uberStrikeDb.KongregateReferrers.InsertOnSubmit(kongregateReferrer);
                uberStrikeDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Create a Facebook member from a Facebook Id
        /// </summary>
        /// <param name="facebookId"></param>
        /// <param name="referrer"></param>
        /// <param name="emailAddress"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="networkAddress"></param>
        /// <param name="locale"></param>
        /// <param name="cmid">UberstrikeMemberRegistrationResult</param>
        /// <returns></returns>
        public static MemberRegistrationResult CreateUserFromFacebook(long facebookId, string referrer, string emailAddress, string firstName, string lastName, long networkAddress, string locale, out int cmid)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;

            cmid = 0;
            string name = String.Empty;
            string password = String.Empty;
            DateTime accountCreationDate;

            ret = CmuneMember.CreateUserFromEsns(EsnsType.Facebook, facebookId.ToString(), emailAddress, UberStrikeCommonConfig.ApplicationId, ChannelType.WebFacebook, networkAddress, locale, UberStrikeCommonConfig.FirstLoadoutItemIds, out cmid, out name, out password, out accountCreationDate);
            if (ret == MemberRegistrationResult.Ok)
            {
                CreateUserCommon(cmid, emailAddress, password, name);
                CreateFacebookReferrer(cmid, referrer, facebookId, firstName, lastName);
            }

            return ret;
        }

        /// <summary>
        /// Create a Kongregate member from a kongregate Id
        /// </summary>
        /// <param name="kongregateId"></param>
        /// <param name="referrer"></param>
        /// <param name="emailAddress"></param>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="networkAddress"></param>
        /// <param name="locale"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberRegistrationResult CreateUserFromKongregate(string kongregateId, string referrer, string emailAddress, string userName, string firstName, string lastName, long networkAddress, string locale, out int cmid)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;

            cmid = 0;
            string name = String.Empty;
            string password = String.Empty;
            DateTime accountCreationDate;

            ret = CmuneMember.CreateUserFromEsns(EsnsType.Kongregate, kongregateId, emailAddress, UberStrikeCommonConfig.ApplicationId, ChannelType.WebPortal, networkAddress, locale, UberStrikeCommonConfig.FirstLoadoutItemIds, out cmid, out name, out password, out accountCreationDate);
            if (ret == MemberRegistrationResult.Ok)
            {
                CreateUserCommon(cmid, emailAddress, password, name);
                CreateKongregateReferrer(cmid, referrer, long.Parse(kongregateId), userName, firstName, lastName);
            }

            return ret;
        }

        /// <summary>
        /// Add an email and a password to an Esns Member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        public static MemberOperationResult CompleteEsnsUser(int cmid, string emailAddress, string password, bool checkDuplicate)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (!ValidationUtilities.IsValidEmailAddress(emailAddress))
            {
                ret = MemberOperationResult.InvalidEmail;
            }
            if (!ValidationUtilities.IsValidPassword(password))
            {
                ret = MemberOperationResult.InvalidPassword;
            }

            #endregion Check input

            MemberOperationResult isDuplicateEmail = MemberOperationResult.Ok;

            if (checkDuplicate && CmuneMember.IsDuplicateUserEmail(emailAddress))
            {
                isDuplicateEmail = MemberOperationResult.DuplicateEmail;
            }

            if (isDuplicateEmail.Equals(MemberOperationResult.Ok))
            {
                // Now we need to create an account on the forum as the ESNS only don't have an account on it

                ID3 memberToComplete = CmuneMember.GetId3(cmid);

                if (memberToComplete != null)
                {
                    ret = ForumMember.AddUser(emailAddress, password, memberToComplete.Name, cmid, false);
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="name"></param>
        /// <param name="channel"></param>
        /// <param name="locale"></param>
        /// <param name="sourceIp"></param>
        /// <returns></returns>
        public static AccountCompletionResultView CompleteAccount(int cmid, string name, ChannelType channel, string locale, string sourceIp)
        {
            int result = UberStrikeAccountCompletionResult.InvalidData;
            Dictionary<int, int> itemsToAttribute = Users.GetItemsAttributedOnTutorial();

            if (channel == ChannelType.Kongregate)
            {
                AddKongregateItems(itemsToAttribute);
            }

            List<string> nonDuplicateNames = new List<string>();

            if (!ValidationUtilities.IsValidMemberName(name))
            {
                result = UberStrikeAccountCompletionResult.InvalidName;
            }

            if (result == UberStrikeAccountCompletionResult.InvalidData && CmuneMember.IsAccountComplete(cmid))
            {
                result = UberStrikeAccountCompletionResult.AlreadyCompletedAccount;
            }

            if (result == UberStrikeAccountCompletionResult.InvalidData && CmuneMember.IsDuplicateUserName(name))
            {
                result = UberStrikeAccountCompletionResult.DuplicateName;
                nonDuplicateNames = CmuneMember.GenerateNonDuplicateUserNames(name, 3);
            }

            if (cmid > 0 && result == AccountCompletionResult.InvalidData)
            {
                CmuneEconomy.AttributeItemsOnAccountCompletion(cmid, itemsToAttribute);
                CmuneMember.ChangeMemberName(cmid, name, false, locale, sourceIp, false);
                Users.ChangeUsernameMiniverse(name, cmid, false, false);
                Statistics.AttributeXp(cmid, UberStrikeCommonConfig.XpAttributedOnTutorialCompletion);
                CmuneMember.CompleteAccount(cmid);

                string emailAddress = CmuneMember.GetUserEmail(cmid);

                switch (channel)
                {
                    case ChannelType.WebFacebook:

                        if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailRegistrationFacebook"))
                        {
                            string newPassword = CmuneMember.GeneratePassword();

                            if (ChangePassword(cmid, newPassword))
                            {
                                CmuneMail.SendEmailNewFacebookUser(emailAddress, name, newPassword);
                            }
                            else
                            {
                                CmuneLog.LogUnexpectedReturn(false, "Unable to change the password on user completion for Facebook");
                            }
                        }

                        break;
                    case ChannelType.WebPortal:

                        if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailRegistrationPortal"))
                        {
                            string validationUrl = CmuneMember.GenerateValidationUrl(cmid);
                            CmuneMail.SendEmailNewUser(emailAddress, name, validationUrl);
                        }

                        break;
                    case ChannelType.MacAppStore:

                        if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailRegistrationMacApplication"))
                        {
                            string validationUrl = CmuneMember.GenerateValidationUrl(cmid);
                            CmuneMail.SendEmailNewUser(emailAddress, name, validationUrl);
                        }

                        break;
                    case ChannelType.WindowsStandalone:

                        if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailRegistrationWindowsStandalone"))
                        {
                            string validationUrl = CmuneMember.GenerateValidationUrl(cmid);
                            CmuneMail.SendEmailNewUser(emailAddress, name, validationUrl);
                        }

                        break;
                    case ChannelType.OSXStandalone:

                        if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailRegistrationOSXStandalone"))
                        {
                            string validationUrl = CmuneMember.GenerateValidationUrl(cmid);
                            CmuneMail.SendEmailNewUser(emailAddress, name, validationUrl);
                        }

                        break;
                    case ChannelType.Kongregate:
                    case ChannelType.Android:
                    case ChannelType.IPhone:
                    case ChannelType.IPad:
                        // Do nothing as we can't get users email address
                        break;
                }

                Tracking.RecordTutorialStep(cmid, TutorialStepType.NameSelection);

                result = UberStrikeAccountCompletionResult.Ok;
            }

            foreach (int itemId in UberStrikeCommonConfig.FirstLoadoutWeaponItemIds)
            {
                itemsToAttribute.Add(itemId, 0);
            }

            AccountCompletionResultView resultView = new AccountCompletionResultView(result, itemsToAttribute, nonDuplicateNames);

            return resultView;
        }

        private static void AddKongregateItems(Dictionary<int, int> itemsToAttribute)
        {
            string[] items = ConfigurationUtilities.ReadConfigurationManager("KongregateItemsAttributedOnAccountCompletion").Split('|');

            foreach (string item in items)
            {
                string[] splittedConfig = item.Split('-');

                if (splittedConfig.Length != 2)
                {
                    throw new ArgumentException(String.Format("Invalid configuration: {0} configs should be separated by '|' itemId and duration should be separated by '-' duration should be in days (0 being permanent) ie: 1331-7|1288-0 (7 days of Vanquisher Kongregate Edition and permanent Sentinel Power Armor", item));
                }

                int itemId = 0;
                int duration = 0;

                if (Int32.TryParse(splittedConfig[0], out itemId) && Int32.TryParse(splittedConfig[1], out duration))
                {
                    if (duration >= 0 && duration <= CommonConfig.ItemMaximumDurationInDays)
                    {
                        itemsToAttribute.Add(itemId, duration);
                    }
                }
            }
        }

        /// <summary>
        /// For testing purpose only
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool UncompleteAccount(int cmid)
        {
            bool isUncomplete = false;

            try
            {
                Dictionary<int, int> itemsToAttribute = Users.GetItemsAttributedOnTutorial();

                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    List<ItemInventory> inventory = cmuneDb.ItemInventories.Where(i => itemsToAttribute.Keys.Contains(i.ItemId)).ToList();
                    cmuneDb.ItemInventories.DeleteAllOnSubmit(inventory);
                    cmuneDb.SubmitChanges();

                    User user = uberStrikeDb.Users.SingleOrDefault(u => u.CMID == cmid);
                    Member member = cmuneDb.Members.SingleOrDefault(m => m.CMID == cmid);
                    List<TutorialStep> tutorialSteps = uberStrikeDb.TutorialSteps.Where(t => t.Cmid == cmid && t.StepId == (int)TutorialStepType.NameSelection).ToList();

                    uberStrikeDb.TutorialSteps.DeleteAllOnSubmit(tutorialSteps);
                    uberStrikeDb.SubmitChanges();

                    if (user != null && member != null)
                    {
                        user.XP = 0;
                        user.Level = 1;
                        uberStrikeDb.SubmitChanges();

                        member.IsAccountComplete = false;
                        cmuneDb.SubmitChanges();

                        isUncomplete = true;
                    }
                }

                string name = DateTime.Now.ToString("yyMMddHHMMssfff");
                CmuneMember.ChangeMemberName(cmid, name, false, LocaleIdentifier.EnUS, String.Empty);
                Users.ChangeUsernameMiniverse(name, cmid, false, false);
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Format("cmid={0}", cmid));
                throw;
            }

            return isUncomplete;
        }

        /// <summary>
        /// Update an account to PP 4.0 aka UberStrike (creates a loadout for old member)
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool UpdateAccountToUberStrike(int cmid)
        {
            bool isUpdated = false;

            ItemInventory privateerLicence = CmuneMember.GetItemInventory(cmid, UberStrikeCommonConfig.PrivateerLicenseId);

            if (privateerLicence == null)
            {
                LoadoutView currentLoadoutView = GetLoadoutView(cmid);

                if (currentLoadoutView == null)
                {
                    CreateLoadout(cmid);
                }
                else
                {
                    throw new ArgumentException(String.Format("The Cmid {0} already has a loadout", cmid));
                }

                //Dictionary<int, int> itemsToAttribute = Users.GetItemsAttributedOnTutorial();
                //CmuneEconomy.AttributeItemsOnAccountCompletion(cmid, itemsToAttribute);
                CmuneEconomy.AddItemsToInventoryPermanently(cmid, UberStrikeCommonConfig.FirstLoadoutItemIds, DateTime.Now);

                isUpdated = true;
            }
            else
            {
                throw new ArgumentException(String.Format("This Cmid {0} already has a UberStrike account", cmid));
            }

            return isUpdated;
        }

        #endregion

        #region Items attribution on tutorial

        public static Dictionary<int, int> GetItemsAttributedOnTutorial()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Dictionary<int, int> attributedItems = uberStrikeDb.ItemsAttributedOnTutorials.ToDictionary(i => i.ItemId, i => i.Duration);

                return attributedItems;
            }
        }

        public static void SetItemsAttributedOnTutorial(Dictionary<int, int> attributedItems)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<ItemsAttributedOnTutorial> currentAttributedItems = uberStrikeDb.ItemsAttributedOnTutorials.ToList();
                List<ItemsAttributedOnTutorial> itemsToRemove = new List<ItemsAttributedOnTutorial>();
                List<ItemsAttributedOnTutorial> itemsToAdd = new List<ItemsAttributedOnTutorial>();

                foreach (ItemsAttributedOnTutorial currentAttributedItem in currentAttributedItems)
                {
                    if (attributedItems.ContainsKey(currentAttributedItem.ItemId))
                    {
                        currentAttributedItem.Duration = attributedItems[currentAttributedItem.ItemId];
                        attributedItems.Remove(currentAttributedItem.ItemId);
                    }
                    else
                    {
                        itemsToRemove.Add(currentAttributedItem);
                    }
                }

                // Remaining keys in attributedItems are to add

                foreach (int itemId in attributedItems.Keys)
                {
                    itemsToAdd.Add(new ItemsAttributedOnTutorial { ItemId = itemId, Duration = attributedItems[itemId] });
                }

                uberStrikeDb.ItemsAttributedOnTutorials.InsertAllOnSubmit(itemsToAdd);
                uberStrikeDb.ItemsAttributedOnTutorials.DeleteAllOnSubmit(itemsToRemove);

                uberStrikeDb.SubmitChanges();
            }
        }

        #endregion

        #region Referrer

        /// <summary>
        /// Get Referrer source by URL
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ReferrerPartnerType GetReferrerSourceByUrlAndChannel(ChannelType channel, string url)
        {
            string queryString = String.Empty;

            if (!url.IsNullOrFullyEmpty())
            {
                Uri tempUri = new Uri(url);
                queryString = tempUri.Query;
            }

            return GetReferrerSourceByQueryStringAndChannel(channel, queryString);
        }

        /// <summary>
        /// Get Referrer source by query string
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="querystring">This is only the query string, not the full URL</param>
        /// <returns></returns>
        public static ReferrerPartnerType GetReferrerSourceByQueryStringAndChannel(ChannelType channel, string querystring)
        {
            ReferrerPartnerType referrerSource = ReferrerPartnerType.None;
            List<ReferrerSource> listReferrers = new List<ReferrerSource>();

            if (HttpRuntime.Cache[UberStrikeCacheKeys.ReferrerSource] != null)
            {
                listReferrers = (List<ReferrerSource>)HttpRuntime.Cache[UberStrikeCacheKeys.ReferrerSource];
            }
            else
            {
                using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
                {
                    listReferrers = (from tab in ppDB.ReferrerSources where tab.ChannelID == (int)channel select tab).ToList();
                    HttpRuntime.Cache.Add(UberStrikeCacheKeys.ReferrerSource, listReferrers, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }

            NameValueCollection parameters = HttpUtility.ParseQueryString(querystring);

            if (listReferrers != null)
            {
                for (int i = 0; i < listReferrers.Count; i++)
                {
                    if (parameters[listReferrers[i].ParameterName] != null && parameters[listReferrers[i].ParameterName].Equals(listReferrers[i].ParameterValue))
                    {
                        if (listReferrers[i].SecondParameterName != null)
                        {
                            if (parameters[listReferrers[i].SecondParameterName] != null && parameters[listReferrers[i].SecondParameterName].Equals(listReferrers[i].SecondParameterValue))
                            {
                                referrerSource = (ReferrerPartnerType)listReferrers[i].ReferrerPartnerId;
                            }
                        }
                        else
                        {
                            referrerSource = (ReferrerPartnerType)listReferrers[i].ReferrerPartnerId;
                        }
                    }
                }
            }

            return referrerSource;
        }

        /// <summary>
        /// Checks wether a referrer URL is matching with one of our partner
        /// </summary>
        /// <param name="partner"></param>
        /// <param name="channel"></param>
        /// <param name="querystring">This is only the query string, not the full URL</param>
        /// <returns></returns>
        public static bool IsReferredByPartner(ReferrerPartnerType partner, ChannelType channel, string querystring)
        {
            bool isReferredByPartner = false;

            ReferrerPartnerType referrerPartner = GetReferrerSourceByQueryStringAndChannel(channel, querystring);

            if (referrerPartner == partner)
            {
                isReferredByPartner = true;
            }

            return isReferredByPartner;
        }

        #endregion

        #region Loadout

        /// <summary>
        /// Expire a members loadout
        /// </summary>
        /// <param name="cmid"></param>
        public static void ExpireLoadout(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                var loadout = uberStrikeDB.Loadouts.FirstOrDefault(l => l.Cmid == cmid);

                if (loadout == null) return;

                // get all items that are not expired or permanent
                var itemInventoriesOrdered = cmuneDB.ItemInventories.Where(iI => (iI.ExpirationDate > DateTime.Now || iI.ExpirationDate == null) && iI.Cmid == cmid).ToDictionary(i => i.ItemId);

                int[] loadoutArray = { 
                                         loadout.Backpack, loadout.Boots, loadout.Face, loadout.FunctionalItem1,
                                         loadout.FunctionalItem2, loadout.FunctionalItem3, loadout.Gloves,
                                         loadout.Head, loadout.LowerBody, loadout.MeleeWeapon, loadout.QuickItem1,
                                         loadout.QuickItem2, loadout.QuickItem3, loadout.UpperBody, loadout.Weapon1,
                                         loadout.Weapon1Mod1, loadout.Weapon1Mod2, loadout.Weapon1Mod3, loadout.Weapon2,
                                         loadout.Weapon2Mod1, loadout.Weapon2Mod2, loadout.Weapon2Mod3, loadout.Weapon3,
                                         loadout.Weapon3Mod1, loadout.Weapon3Mod2, loadout.Weapon3Mod3, loadout.Webbing
                                     };

                for (int index = 0; index < loadoutArray.Length; index++)
                {
                    if (loadoutArray[index] > 0 && !itemInventoriesOrdered.ContainsKey(loadoutArray[index]))
                    {
                        //TODO: needs improved
                        switch (index)
                        {
                            case 0:
                                loadout.Backpack = 0;
                                break;
                            case 1:
                                loadout.Boots = UberStrikeCommonConfig.DefaultBoots;
                                break;
                            case 2:
                                loadout.Face = UberStrikeCommonConfig.DefaultFace;
                                break;
                            case 3:
                                loadout.FunctionalItem1 = 0;
                                break;
                            case 4:
                                loadout.FunctionalItem2 = 0;
                                break;
                            case 5:
                                loadout.FunctionalItem3 = 3;
                                break;
                            case 6:
                                loadout.Gloves = UberStrikeCommonConfig.DefaultGloves;
                                break;
                            case 7:
                                loadout.Head = UberStrikeCommonConfig.DefaultHead;
                                break;
                            case 8:
                                loadout.LowerBody = UberStrikeCommonConfig.DefaultLowerBody;
                                break;
                            case 9:
                                loadout.MeleeWeapon = UberStrikeCommonConfig.DefaultMeleeWeapon;
                                break;
                            case 10:
                                loadout.QuickItem1 = 0;
                                break;
                            case 11:
                                loadout.QuickItem2 = 0;
                                break;
                            case 12:
                                loadout.QuickItem3 = 0;
                                break;
                            case 13:
                                loadout.UpperBody = UberStrikeCommonConfig.DefaultUpperBody;
                                break;
                            case 14:
                                loadout.Weapon1 = 0;
                                break;
                            case 15:
                                loadout.Weapon1Mod1 = 0;
                                break;
                            case 16:
                                loadout.Weapon1Mod2 = 0;
                                break;
                            case 17:
                                loadout.Weapon1Mod3 = 0;
                                break;
                            case 18:
                                loadout.Weapon2 = 0;
                                break;
                            case 19:
                                loadout.Weapon2Mod1 = 0;
                                break;
                            case 20:
                                loadout.Weapon2Mod2 = 0;
                                break;
                            case 21:
                                loadout.Weapon2Mod3 = 0;
                                break;
                            case 22:
                                loadout.Weapon3 = 0;
                                break;
                            case 23:
                                loadout.Weapon3Mod1 = 0;
                                break;
                            case 24:
                                loadout.Weapon3Mod2 = 0;
                                break;
                            case 25:
                                loadout.Weapon3Mod3 = 0;
                                break;
                            case 26:
                                loadout.Webbing = 0;
                                break;
                        }
                    }
                }

                //if the user has no weapon equipped we equip the default weapon
                if ((loadout.Weapon1 + loadout.Weapon2 + loadout.Weapon3) == 0)
                {
                    loadout.Weapon1 = UberStrikeCommonConfig.DefaultWeapon;
                }

                uberStrikeDB.SubmitChanges();
            }
        }
        /// <summary>
        /// Generates a uniq key to know if an item is own by a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private static string GenerateKey(int cmid, int itemId)
        {
            string key = cmid.ToString() + "|" + itemId.ToString();

            return key;
        }

        /// <summary>
        /// Creates a loadout
        /// </summary>
        /// <param name="cmid"></param>
        private static void CreateLoadout(int cmid)
        {
            try
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    LoadoutView defaultLoadout = UberStrikeCommonConfig.DefaultLoadout;

                    Loadout loadout = new Loadout();

                    loadout.Backpack = defaultLoadout.Backpack;
                    loadout.Boots = defaultLoadout.Boots;
                    loadout.Cmid = cmid;
                    loadout.Face = defaultLoadout.Face;
                    loadout.FunctionalItem1 = defaultLoadout.FunctionalItem1;
                    loadout.FunctionalItem2 = defaultLoadout.FunctionalItem2;
                    loadout.FunctionalItem3 = defaultLoadout.FunctionalItem3;
                    loadout.Gloves = defaultLoadout.Gloves;
                    loadout.Head = defaultLoadout.Head;
                    loadout.LowerBody = defaultLoadout.LowerBody;
                    loadout.MeleeWeapon = defaultLoadout.MeleeWeapon;
                    loadout.QuickItem1 = defaultLoadout.QuickItem1;
                    loadout.QuickItem2 = defaultLoadout.QuickItem2;
                    loadout.QuickItem3 = defaultLoadout.QuickItem3;
                    loadout.SkinColor = defaultLoadout.SkinColor;
                    loadout.Type = (int)defaultLoadout.Type;
                    loadout.UpperBody = defaultLoadout.UpperBody;
                    loadout.Weapon1 = defaultLoadout.Weapon1;
                    loadout.Weapon1Mod1 = defaultLoadout.Weapon1Mod1;
                    loadout.Weapon1Mod2 = defaultLoadout.Weapon1Mod2;
                    loadout.Weapon1Mod3 = defaultLoadout.Weapon1Mod3;
                    loadout.Weapon2 = defaultLoadout.Weapon2;
                    loadout.Weapon2Mod1 = defaultLoadout.Weapon2Mod1;
                    loadout.Weapon2Mod2 = defaultLoadout.Weapon2Mod2;
                    loadout.Weapon2Mod3 = defaultLoadout.Weapon2Mod3;
                    loadout.Weapon3 = defaultLoadout.Weapon3;
                    loadout.Weapon3Mod1 = defaultLoadout.Weapon3Mod1;
                    loadout.Weapon3Mod2 = defaultLoadout.Weapon3Mod2;
                    loadout.Weapon3Mod3 = defaultLoadout.Weapon3Mod3;
                    loadout.Webbing = defaultLoadout.Webbing;

                    uberStrikeDb.Loadouts.InsertOnSubmit(loadout);
                    uberStrikeDb.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, cmid.ToString());
                throw;
            }
        }

        /// <summary>
        /// Deletes a loadout
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        private static MemberOperationResult DeleteLoadout(int cmid)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
            }

            #endregion

            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                Loadout loadout = paradiseDB.Loadouts.SingleOrDefault(l => l.Cmid == cmid);

                if (loadout != null)
                {
                    paradiseDB.Loadouts.DeleteOnSubmit(loadout);
                    paradiseDB.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets a loadout
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static LoadoutView GetLoadoutView(int cmid)
        {
            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
            }

            #endregion

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                Loadout loadout = uberStrikeDb.Loadouts.SingleOrDefault(l => l.Cmid == cmid);
                LoadoutView loadoutView = null;

                if (loadout != null)
                {
                    loadoutView = new LoadoutView(loadout.LoadoutId, loadout.Backpack, loadout.Boots, loadout.Cmid, loadout.Face, loadout.FunctionalItem1, loadout.FunctionalItem2, loadout.FunctionalItem3, loadout.Gloves, loadout.Head, loadout.LowerBody, loadout.MeleeWeapon, loadout.QuickItem1, loadout.QuickItem2, loadout.QuickItem3, (AvatarType)loadout.Type, loadout.UpperBody, loadout.Weapon1, loadout.Weapon1Mod1, loadout.Weapon1Mod2, loadout.Weapon1Mod3, loadout.Weapon2, loadout.Weapon2Mod1, loadout.Weapon2Mod2, loadout.Weapon2Mod3, loadout.Weapon3, loadout.Weapon3Mod1, loadout.Weapon3Mod2, loadout.Weapon3Mod3, loadout.Webbing, loadout.SkinColor);
                }

                return loadoutView;
            }

        }

        /// <summary>
        /// Sets a loadout
        /// </summary>
        /// <param name="loadoutView"></param>
        /// <returns></returns>
        public static MemberOperationResult SetLoadout(LoadoutView loadoutView)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            if (loadoutView != null)
            {
                ret = SetLoadout(loadoutView.Cmid, loadoutView);
            }

            return ret;
        }

        /// <summary>
        /// Sets a loadout
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="loadoutView"></param>
        /// <returns></returns>
        public static MemberOperationResult SetLoadout(int cmid, LoadoutView loadoutView)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                if (loadoutView != null)
                {
                    Loadout loadout = paradiseDB.Loadouts.SingleOrDefault(l => l.Cmid == cmid);

                    if (loadout != null)
                    {
                        loadout.Backpack = loadoutView.Backpack;
                        loadout.Boots = loadoutView.Boots;
                        loadout.Face = loadoutView.Face;
                        loadout.FunctionalItem1 = loadoutView.FunctionalItem1;
                        loadout.FunctionalItem2 = loadoutView.FunctionalItem2;
                        loadout.FunctionalItem3 = loadoutView.FunctionalItem3;
                        loadout.Gloves = loadoutView.Gloves;
                        loadout.Head = loadoutView.Head;
                        loadout.LowerBody = loadoutView.LowerBody;
                        loadout.MeleeWeapon = loadoutView.MeleeWeapon;
                        loadout.QuickItem1 = loadoutView.QuickItem1;
                        loadout.QuickItem2 = loadoutView.QuickItem2;
                        loadout.QuickItem3 = loadoutView.QuickItem3;
                        loadout.Type = (int)loadoutView.Type;
                        loadout.UpperBody = loadoutView.UpperBody;
                        loadout.Weapon1 = loadoutView.Weapon1;
                        loadout.Weapon1Mod1 = loadoutView.Weapon1Mod1;
                        loadout.Weapon1Mod2 = loadoutView.Weapon1Mod2;
                        loadout.Weapon1Mod3 = loadoutView.Weapon1Mod3;
                        loadout.Weapon2 = loadoutView.Weapon2;
                        loadout.Weapon2Mod1 = loadoutView.Weapon2Mod1;
                        loadout.Weapon2Mod2 = loadoutView.Weapon2Mod2;
                        loadout.Weapon2Mod3 = loadoutView.Weapon2Mod3;
                        loadout.Weapon3 = loadoutView.Weapon3;
                        loadout.Weapon3Mod1 = loadoutView.Weapon3Mod1;
                        loadout.Weapon3Mod2 = loadoutView.Weapon3Mod2;
                        loadout.Weapon3Mod3 = loadoutView.Weapon3Mod3;
                        loadout.Webbing = loadoutView.Webbing;
                        loadout.SkinColor = loadoutView.SkinColor;

                        paradiseDB.SubmitChanges();

                        ret = MemberOperationResult.Ok;
                    }
                }

                return ret;
            }
        }

        #endregion Loadout

        #region Delete

        /// <summary>
        /// Deletes a paradise paintball link to a MySpace ID
        /// </summary>
        /// <param name="mySpaceId"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteEmptyMySpace(int mySpaceId)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                MySpace mySpaceAccount = GetMySpace(mySpaceId, paradiseDB);

                if (mySpaceAccount != null)
                {
                    PPUser ppMember = mySpaceAccount.User;

                    paradiseDB.MySpaces.DeleteOnSubmit(mySpaceAccount);
                    paradiseDB.Users.DeleteOnSubmit(mySpaceAccount.User);

                    paradiseDB.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }

                return ret;
            }
        }

        /// <summary>
        /// Deletes a paradise paintball link to a MySpace ID
        /// </summary>
        /// <param name="cyworldId"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteEmptyCyworld(int cyworldId)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                Cyworld cyworldAccount = GetCyworld(cyworldId, uberStrikeDb);

                if (cyworldAccount != null)
                {
                    PPUser ppMember = cyworldAccount.User;

                    uberStrikeDb.Cyworlds.DeleteOnSubmit(cyworldAccount);
                    uberStrikeDb.Users.DeleteOnSubmit(cyworldAccount.User);

                    uberStrikeDb.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }

                return ret;
            }
        }

        ///// <summary>
        ///// Deletes a Cyworld account linked to a player
        ///// </summary>
        ///// <param name="cmid"></param>
        //private static MemberOperationResult DeleteCyworld(int cmid)
        //{
        //    MemberOperationResult ret = MemberOperationResult.MemberNotFound;

        //    #region Check input

        //    if (cmid < 1)
        //    {
        //        throw new ArgumentOutOfRangeException("cmid", cmid, "The Cmid should be superior to 0.");
        //    }

        //    #endregion

        //    using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
        //    {
        //        PPUser currentUser = uberStrikeDb.Users.SingleOrDefault<PPUser>(u => u.CMID == cmid);

        //        if (currentUser != null)
        //        {
        //            if (currentUser.Cyworlds != null)
        //            {
        //                Cyworld cyworldAccountToDelete = currentUser.Cyworlds;
        //                uberStrikeDb.Cyworlds.DeleteOnSubmit(cyworldAccountToDelete);

        //                uberStrikeDb.SubmitChanges();

        //                ret = MemberOperationResult.Ok;
        //            }
        //        }

        //        return ret;
        //    }
        //}

        /// <summary>
        /// Deletes a Paradise Paintball User, deletes also all the table linked to User with a foreign key
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeletePPUser(int cmid)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be superior to 0.");
            }

            #endregion

            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                PPUser userToDelete = ppDB.Users.SingleOrDefault<PPUser>(u => u.CMID == cmid);

                if (userToDelete != null)
                {
                    DeleteLoadout(cmid);

                    ppDB.Users.DeleteOnSubmit(userToDelete);
                    ppDB.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }

                return ret;
            }
        }

        #endregion

        /// <summary>
        /// Validates a member email and gives points to the member if he didn't validate it before
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="validationCode"></param>
        /// <returns></returns>
        public static bool ValidateMemberEmail(int cmid, string validationCode)
        {
            bool isEmailValidated = CmuneMember.ValidateMemberEmail(cmid, validationCode);

            if (isEmailValidated)
            {
                bool hasAlreadyValidatedEmail = CmuneMember.HasAlreadyValidatedEmail(cmid);

                if (!hasAlreadyValidatedEmail)
                {
                    CmuneEconomy.AttributePoints(cmid, CommonConfig.PointsAttributedOnEmailValidation, false, PointsDepositType.IdentityValidation);
                }
            }

            return isEmailValidated;
        }

        #region Cache propagation

        /// <summary>
        /// Propagates a name change to all the cache tables
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool PropagateName(string name, int cmid)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                bool isPropagated = false;

                int res = paradiseDB.Paradise_Propagate_Name(name, cmid);

                if (res == CommonConfig.StoredProcedureSuccess)
                {
                    isPropagated = true;
                }
                else
                {
                    CmuneLog.LogUnexpectedReturn(res, "name=" + name + "&cmid=" + cmid.ToString());
                }

                return isPropagated;
            }
        }

        #endregion Cache propagation

        public static void EmailUnfinishedRegistration()
        {
            DateTime now = DateTime.Now;
            DateTime endingDate = now.AddMinutes(-now.Minute).AddSeconds(-now.Second).AddHours(-1);

            int delayInHours = ConfigurationUtilities.ReadConfigurationManagerInt("EmailUnfinishedRegistrationHours");

            DateTime startingDate = endingDate.AddHours(-delayInHours);


            if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailUnfinishedRegistrationPortal"))
            {
                List<string> emailAddresses = CmuneMember.GetNonFacebookUnfinishedRegistrationEmails(startingDate, endingDate, ChannelType.WebPortal);

                CmuneMail.SendEmailUnsuccessfulPortalRegistration(emailAddresses);
            }

            if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailUnfinishedRegistrationFacebook"))
            {
                Dictionary<string, string> emailAddresses = CmuneMember.GetFacebookUnfinishedRegistration(startingDate, endingDate);

                CmuneMail.SendEmailUnsuccessfulFacebookRegistration(emailAddresses.Keys.ToList(), emailAddresses.Values.ToList());
            }

            if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailUnfinishedRegistrationMacApplication"))
            {
                List<string> emailAddresses = CmuneMember.GetNonFacebookUnfinishedRegistrationEmails(startingDate, endingDate, ChannelType.MacAppStore);

                CmuneMail.SendEmailUnsuccessfulStandaloneRegistration(emailAddresses);
            }

            if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailUnfinishedRegistrationWindowsStandalone"))
            {
                List<string> emailAddresses = CmuneMember.GetNonFacebookUnfinishedRegistrationEmails(startingDate, endingDate, ChannelType.WindowsStandalone);

                CmuneMail.SendEmailUnsuccessfulStandaloneRegistration(emailAddresses);
            }

            if (ConfigurationUtilities.ReadConfigurationManagerBool("EmailUnfinishedRegistrationOSXStandalone"))
            {
                List<string> emailAddresses = CmuneMember.GetNonFacebookUnfinishedRegistrationEmails(startingDate, endingDate, ChannelType.OSXStandalone);

                CmuneMail.SendEmailUnsuccessfulStandaloneRegistration(emailAddresses);
            }
        }
    }
}