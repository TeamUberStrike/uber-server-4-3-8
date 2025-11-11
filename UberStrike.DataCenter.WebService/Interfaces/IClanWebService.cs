using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IClanWebService
    {
        bool IsMemberPartOfGroup(int cmuneId, int groupId);
        bool IsMemberPartOfAnyGroup(int cmuneId, int applicationId);
        ClanView GetClan(int groupId);
        int UpdateMemberPosition(MemberPositionUpdateView updateMemberPositionData);
        int InviteMemberToJoinAGroup(int clanId, int inviterCmid, int inviteeCmid, string message);

        ClanRequestAcceptView AcceptClanInvitation(int clanInvitationId, int cmid);
        ClanRequestDeclineView DeclineClanInvitation(int clanInvitationId, int cmid);

        int KickMemberFromClan(int groupId, int cmidTakingAction, int cmidToKick);
        int DisbandGroup(int groupId, int cmidTakingAction);
        int LeaveAClan(int groupId, int cmid);
        int GetMyClanId(int cmid, int applicationId);
        int CancelInvitation(int groupInvitationId, int cmid);
        int CancelRequest(int groupRequestId);
        List<GroupInvitationView> GetAllGroupInvitations(int inviteeCmid, int applicationId);
        List<GroupInvitationView> GetPendingGroupInvitations(int groupId);
        ClanCreationReturnView CreateClan(GroupCreationView createClanData);
        int TransferOwnership(int groupId, int previousLeaderCmid, int newLeaderCmid);
        int CanOwnAClan(int cmid);
        int test();
    }
}

