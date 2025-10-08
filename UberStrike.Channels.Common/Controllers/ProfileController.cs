using System.Web.Mvc;
using System;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Business;
using UberStrike.DataCenter.Business.Views;
using UberStrike.DataCenter.Business;
using System.Web;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Channels.Common.Models;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/

        public ActionResult Index(int cmid = 0, ChannelType channelType = ChannelType.WebFacebook, string appCallbackUrl = "")
        {
            string errorMessage = String.Empty;
            string warningMessage = string.Empty;
            var userProfileView = new UserProfileViewModel();
            ViewBag.Cmid = cmid;
            if (appCallbackUrl == "")
            {
                appCallbackUrl = ConfigurationUtilities.ReadConfigurationManager("PortalChannel") + "/Profile";
            }
                
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ID3 memberId3 = CmuneMember.GetId3(cmid, cmuneDb);

                try
                {
                    if (memberId3 != null)
                    {
                        Member member = memberId3.Member;
                        var memberAccess = CmuneMember.GetMemberAccess(cmid, CommonConfig.ApplicationIdUberstrike);

                        PlayerCard statsUser = new PlayerCard(cmid, true);

                        userProfileView.UserName = memberId3.Name;

                        string clanTag = String.Empty;

                        if (!member.TagName.IsNullOrFullyEmpty())
                        {
                            clanTag = "[" + member.TagName + "]";
                        }

                        userProfileView.ClanTag = clanTag;
                        userProfileView.JoinedDateTime = member.ResLastSyncDate;
                        userProfileView.GlobalRank = (statsUser != null && statsUser.Ranking != 0) ? statsUser.Ranking : 0;
                        if (memberAccess.IsAccountDisabled == (int)BanMode.No)
                        {
                            var loadoutView = Users.GetLoadoutView(cmid);
                            UberStrike.DataCenter.DataAccess.User uberstrikeMember = Users.GetUser(cmid);

                            if (loadoutView != null && uberstrikeMember != null)
                            {
                                userProfileView.IsUberStrikeUser = true;

                                userProfileView.Loadout = loadoutView.ToUserLoadoutModel();

                                userProfileView.PersonalBestsPerLife = new PersonalBestsModel()
                                {
                                    MostKills = uberstrikeMember.MostSplats,
                                    MostXpEarned = uberstrikeMember.MostXPEarned,
                                    MostDamageReceived = uberstrikeMember.MostDamageReceived,
                                    MostDamageDealt = uberstrikeMember.MostDamageDealt,
                                    MostHealthPickedUp = uberstrikeMember.MostHealthPickedUp,
                                    MostArmorPickedUp = uberstrikeMember.MostArmorPickedUp,
                                    MostHeadShots = uberstrikeMember.MostHeadshots,
                                    MostNutShots = uberstrikeMember.MostNutshots,
                                    MostConsecutiveSnipes = uberstrikeMember.MostConsecutiveSnipes,
                                };

                                userProfileView.AllTimesStats = new AllTimeStatsModel()
                                {
                                    Deaths = uberstrikeMember.Splatted,
                                    Headshots = uberstrikeMember.Headshots,
                                    Hits = uberstrikeMember.Hits,
                                    Kills = uberstrikeMember.Splats,
                                    Nutshots = uberstrikeMember.Nutshots,
                                    Shots = uberstrikeMember.Shots
                                };

                                userProfileView.Level = uberstrikeMember.Level;
                                userProfileView.Xp = uberstrikeMember.XP;

                                userProfileView.WeaponsStats = new WeaponsStatsModel()
                                {
                                    CannonHits = uberstrikeMember.CannonTotalShotsHit,
                                    CannonShots = uberstrikeMember.CannonTotalShotsFired,
                                    HandGunHits = uberstrikeMember.HandgunTotalShotsHit,
                                    HandGunShots = uberstrikeMember.HandgunTotalShotsFired,
                                    LauncherHits = uberstrikeMember.LauncherTotalShotsHit,
                                    LauncherShots = uberstrikeMember.LauncherTotalShotsFired,
                                    MachineGunHits = uberstrikeMember.MachineGunTotalShotsHit,
                                    MachineGunShots = uberstrikeMember.MachineGunTotalShotsFired,
                                    MeleeHits = uberstrikeMember.MeleeTotalShotsHit,
                                    MeleeShots = uberstrikeMember.MeleeTotalShotsFired,
                                    MostCannonKills = uberstrikeMember.MostCannonSplats,
                                    MostHandGunKills = uberstrikeMember.MostHandgunSplats,
                                    MostLauncherKills = uberstrikeMember.MostLauncherSplats,
                                    MostMachineGunKills = uberstrikeMember.MostMachinegunSplats,
                                    MostMeleeKills = uberstrikeMember.MostMeleeSplats,
                                    MostShotGunKills = uberstrikeMember.MostShotgunSplats,
                                    MostSniperKills = uberstrikeMember.MostSniperSplats,
                                    MostSplatterGunKills = uberstrikeMember.MostSplattergunSplats,
                                    ShotGunHits = uberstrikeMember.ShotgunTotalShotsHit,
                                    ShotGunShots = uberstrikeMember.ShotgunTotalShotsFired,
                                    SniperHits = uberstrikeMember.SniperTotalShotsHit,
                                    SniperShots = uberstrikeMember.SniperTotalShotsFired,
                                    SplatterGunHits = uberstrikeMember.SplattergunTotalShotsHit,
                                    SplatterGunShots = uberstrikeMember.SplattergunTotalShotsFired,
                                    TotalMeleeKills = uberstrikeMember.MeleeTotalSplats,
                                    TotalHandGunKills = uberstrikeMember.HandgunTotalSplats,
                                    TotalMachineGunKills = uberstrikeMember.MachineGunTotalSplats,
                                    TotalShotGunKills = uberstrikeMember.ShotgunTotalSplats,
                                    TotalSniperKills = uberstrikeMember.SniperTotalSplats,
                                    TotalSplatterGunKills = uberstrikeMember.SplattergunTotalSplats,
                                    TotalCannonKills = uberstrikeMember.CannonTotalSplats,
                                    TotalLauncherKills = uberstrikeMember.LauncherTotalSplats
                                };
                            }
                            else if (uberstrikeMember != null)
                            {
                                userProfileView.IsUberStrikeUser = false;
                            }
                            else
                            {
                                CmuneLog.LogUnexpectedReturn(cmid, "User is missing.");
                                errorMessage = "Hey! You've found a Cmid that doesn't exist.";
                            }

                            //if (Request.IsAuthenticated && ((UserProfileModel)ViewBag.UserProfile).Member != null && ((UserProfileModel)ViewBag.UserProfile).Member.CMID == cmid)
                            //{
                            var nearbyUsers = Statistics.GetNearbyUsers(cmid, 11).ConvertAll(new Converter<AllTimeTotalRanking, RankingView>(r => new RankingView { Cmid = r.CMID, ClanTag = r.TagName, Deaths = r.Splatted.Value, Kills = r.Splats.Value, Level = 0, Name = r.Name, Rank = r.Ranknum, Xp = 0 }));

                            if (nearbyUsers.Exists(d => d.Cmid == cmid))
                                ViewBag.NearbyUsers = nearbyUsers;
                            //}
                        }
                        else
                        {
                            warningMessage = "This profile is banned";
                        }
                    }
                    else
                    {
                        errorMessage = "Hey! You've found a Cmid that doesn't exist.";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Hmm. Something appears to have gone wrong. Our team have been notified and will be on the case soon!";
                    CmuneLog.LogException(ex, "cmid=" + cmid.ToString());
                }
            }

            ViewBag.ErrorMessage = errorMessage;
            ViewBag.WarningMessage = warningMessage;
            ViewBag.UserProfileView = userProfileView;
            ViewBag.ChannelType = channelType;
            ViewBag.AppCallbackUrl = HttpUtility.UrlDecode(appCallbackUrl);
            return View();
        }

    }
}
