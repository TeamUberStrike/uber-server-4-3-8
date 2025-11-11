using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class UserWebService : IUserWebService
    {
        public MemberOperationResult ChangeMemberName(int cmid, string name, string locale, string machineId)
        {
            MemberOperationResult result = MemberOperationResult.NameChangeNotInInventory;

            if (CmuneEconomy.CanUseConsumableItem(cmid, CommonConfig.NameChangeItem))
            {
                result = Cmune.Instrumentation.Business.UsersBusiness.ChangeMemberName(cmid, name, locale, TextUtilities.InetNToA(WebServiceUtil.GetCurrentContextNetworkAddress()));

                if (result == MemberOperationResult.Ok)
                {
                    CmuneEconomy.UseConsumableItem(cmid, CommonConfig.NameChangeItem);
                }
            }

            return result;
        }

        public bool IsDuplicateMemberName(string username)
        {
            return CmuneMember.IsDuplicateUserName(username);
        }

        public List<string> GenerateNonDuplicatedMemberNames(string username)
        {
            return CmuneMember.GenerateNonDuplicateUserNames(username, 3);
        }

        public MemberView GetMember(int cmid, int applicationId)
        {
            return CmuneMember.GetMember(cmid, applicationId);
        }

        public PublicProfileView GetPublicProfile(int cmid, int applicationId)
        {
            return CmuneMember.GetPublicProfile(cmid, applicationId);
        }

        public MemberWalletView GetMemberWallet(int cmid)
        {
            return CmuneEconomy.GetMemberWallet(cmid);
        }

        public List<ItemInventoryView> GetInventory(int cmid)
        {
            return CmuneMember.GetItemInventory(cmid).ConvertAll(new System.Converter<ItemInventory, ItemInventoryView>(lI => new ItemInventoryView(lI.ItemId, lI.ExpirationDate, lI.AmountRemaining)));
        }

        public bool ReportMember(MemberReportView memberReport)
        {
            CmuneMember.ReportMember(memberReport.SourceCmid, memberReport.TargetCmid, memberReport.ReportType, memberReport.Reason, memberReport.Context, memberReport.ApplicationId);
            return true;
        }

        public Dictionary<int, string> FindMembers(string name, int maxResults)
        {
            return CmuneMember.FindMembers(name, maxResults);
        }

        public CurrencyDepositsViewModel GetCurrencyDeposits(int cmid, int pageIndex, int elementPerPage)
        {
            bool getOldDeposits = false;
            return new UberStrike.Core.ViewModel.CurrencyDepositsViewModel()
            {
                CurrencyDeposits = CmuneEconomy.GetCurrencyDepositsView(cmid, pageIndex, elementPerPage, getOldDeposits),
                TotalCount = CmuneEconomy.GetCurrencyDepositsViewCount(cmid, getOldDeposits)
            };
        }

        public ItemTransactionsViewModel GetItemTransactions(int cmid, int pageIndex, int elementPerPage)
        {
            bool getOldTransactions = false;
            return new UberStrike.Core.ViewModel.ItemTransactionsViewModel()
            {
                ItemTransactions = CmuneEconomy.GetItemTransactionsView(cmid, pageIndex, elementPerPage, getOldTransactions),
                TotalCount = CmuneEconomy.GetItemTransactionsViewCount(cmid, getOldTransactions)
            };
        }

        public PointDepositsViewModel GetPointsDeposits(int cmid, int pageIndex, int elementPerPage)
        {
            bool getOldDeposits = false;
            return new UberStrike.Core.ViewModel.PointDepositsViewModel()
            {
                PointDeposits = CmuneEconomy.GetPointDepositView(cmid, pageIndex, elementPerPage, getOldDeposits),
                TotalCount = CmuneEconomy.GetPointDepositViewCount(cmid, getOldDeposits)
            };
        }

        public List<PlayerCardView> GetUserAndTopStats(int cmid)
        {
            return Statistics.GetUserAndTopStats(cmid);
        }

        public PlayerCardView GetRealTimeStatistics(int cmid)
        {
            return Statistics.GetRealTimeStatisticsView(cmid);
        }

        public void SetScore(MatchView scoringView)
        {
            Statistics.SetMatchScore(scoringView);
        }

        public UberstrikeUserViewModel GetMember(int cmid)
        {
            return new Core.ViewModel.UberstrikeUserViewModel()
            {
                CmuneMemberView = CmuneMember.GetMember(cmid, UberStrikeCommonConfig.ApplicationId),
                UberstrikeMemberView = Users.GetMember(cmid)
            };
        }

        public PlayerCardView GetStatistics(int cmid)
        {
            return Statistics.GetStatisticsView(cmid);
        }

        public LoadoutView GetLoadout(int cmid)
        {
            return Users.GetLoadoutView(cmid);
        }

        public MemberOperationResult SetLoadout(LoadoutView loadoutView)
        {
            return Users.SetLoadout(loadoutView);
        }

        public Dictionary<int, PlayerXPEventView> GetXPEventsView()
        {
            Dictionary<int, PlayerXPEventView> xpEventsView = new Dictionary<int, PlayerXPEventView>();

            if (HttpRuntime.Cache[UberStrikeCacheKeys.XPEvents] != null)
            {
                xpEventsView = (Dictionary<int, PlayerXPEventView>)HttpRuntime.Cache[UberStrikeCacheKeys.XPEvents];
            }
            else
            {
                xpEventsView = Statistics.GetXPEventsViewOrdered();
                HttpRuntime.Cache.Add(UberStrikeCacheKeys.XPEvents, xpEventsView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return xpEventsView;
        }

        public List<PlayerLevelCapView> GetLevelCapsView()
        {
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

            return levelCapsView;
        }

        public string HelloWorld(int id)
        {
            return "hello " + id.ToString();
        }
    }
}
