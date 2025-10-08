using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IUserWebService
    {
        MemberOperationResult ChangeMemberName(int cmid, string name, string locale, string machineId);

        bool IsDuplicateMemberName(string username);
        List<string> GenerateNonDuplicatedMemberNames(string username);
        //MemberView GetMember(int cmid, int applicationId);
        PublicProfileView GetPublicProfile(int cmid, int applicationId);
        MemberWalletView GetMemberWallet(int cmid);
        List<ItemInventoryView> GetInventory(int cmid);
        bool ReportMember(MemberReportView memberReport);
        Dictionary<int, string> FindMembers(string name, int maxResults);
        CurrencyDepositsViewModel GetCurrencyDeposits(int cmid, int pageIndex, int elementPerPage);
        ItemTransactionsViewModel GetItemTransactions(int cmid, int pageIndex, int elementPerPage);
        PointDepositsViewModel GetPointsDeposits(int cmid, int pageIndex, int elementPerPage);

        List<PlayerCardView> GetUserAndTopStats(int cmid);
        PlayerCardView GetRealTimeStatistics(int cmid);
        void SetScore(MatchView scoringView);
        UberstrikeUserViewModel GetMember(int cmid);
        PlayerCardView GetStatistics(int cmid);
        LoadoutView GetLoadout(int cmid);
        MemberOperationResult SetLoadout(LoadoutView loadoutView);
        Dictionary<int, PlayerXPEventView> GetXPEventsView();
        List<PlayerLevelCapView> GetLevelCapsView();
    }
}

