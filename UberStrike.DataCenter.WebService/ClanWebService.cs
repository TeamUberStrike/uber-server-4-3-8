using System.Collections.Generic;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Business;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class ClanWebService : IClanWebService
    {
        public bool IsMemberPartOfGroup(int cmid, int groupId)
        {
            return CmuneGroups.IsMemberPartOfGroup(cmid, groupId);
        }

        public bool IsMemberPartOfAnyGroup(int cmid, int applicationId)
        {
            return CmuneGroups.IsMemberPartOfAnyGroup(cmid, applicationId);
        }

        public ClanCreationReturnView CreateClan(GroupCreationView groupCreationView)
        {
            return UberStrikeGroups.CreateGroup(groupCreationView.Name, groupCreationView.Motto, groupCreationView.Cmid, groupCreationView.Tag, groupCreationView.Locale);
        }

        public ClanView GetClan(int groupId)
        {
            return CmuneGroups.GetGroupView(groupId);
        }

        public int UpdateMemberPosition(MemberPositionUpdateView memberPositionUpdateView)
        {
            return CmuneGroups.UpdateMemberPosition(memberPositionUpdateView.GroupId, memberPositionUpdateView.CmidMakingAction, memberPositionUpdateView.MemberCmid, memberPositionUpdateView.Position);
        }

        public int TransferOwnership(int groupId, int previousLeaderCmid, int newLeaderCmid)
        {
            return UberStrikeGroups.TransferOwnership(groupId, previousLeaderCmid, newLeaderCmid);
        }

        public int InviteMemberToJoinAGroup(int clanId, int inviterCmid, int inviteeCmid, string message)
        {
            return CmuneGroups.InviteMemberToJoinAGroup(clanId, inviterCmid, inviteeCmid, message);
        }

        //public ClanInvitationAnswerViewModel AnswerGroupInvitation(int groupInvitationId, bool isInvitationAccepted)
        //{
        //    return new ClanInvitationAnswerViewModel()
        //    {
        //        GroupInvitationId = groupInvitationId,
        //        IsInvitationAccepted = isInvitationAccepted,
        //        ReturnValue = CmuneGroups.AnswerGroupInvitation(groupInvitationId, isInvitationAccepted)
        //    };
        //}

        public ClanRequestAcceptView AcceptClanInvitation(int clanInvitationId, int cmid)
        {
            return CmuneGroups.AnswerGroupInvitation(clanInvitationId, cmid);
        }

        public ClanRequestDeclineView DeclineClanInvitation(int clanInvitationId, int cmid)
        {
            return CmuneGroups.DeclineGroupInvitation(clanInvitationId, cmid);
        }

        public int KickMemberFromClan(int groupId, int cmidTakingAction, int cmidToKick)
        {
            return CmuneGroups.KickMemberFromClan(groupId, cmidTakingAction, cmidToKick);
        }

        public int DisbandGroup(int groupId, int cmidTakingAction)
        {
            return CmuneGroups.DisbandGroup(groupId, cmidTakingAction);
        }

        public int LeaveAClan(int groupId, int cmid)
        {
            return CmuneGroups.LeaveAClan(groupId, cmid);
        }

        public int GetMyClanId(int cmid, int applicationId)
        {
            return CmuneGroups.GetGroupId(cmid, applicationId);
        }

        public int CancelInvitation(int groupInvitationId, int cmid)
        {
            return CmuneGroups.CancelInvitation(groupInvitationId, cmid);
        }

        public int CancelRequest(int groupRequestId)
        {
            return CmuneGroups.CancelRequest(groupRequestId);
        }

        public List<GroupInvitationView> GetAllGroupInvitations(int inviteeCmid, int applicationId)
        {
            return CmuneGroups.GetAllGroupInvitations(inviteeCmid, applicationId);
        }

        public List<GroupInvitationView> GetPendingGroupInvitations(int groupId)
        {
            return CmuneGroups.GetPendingGroupInvitations(groupId);
        }

        public int CanOwnAClan(int cmid)
        {
            return UberStrikeGroups.CanOwnAClan(cmid);
        }

        public int test()
        {
            return 0;
        }
    }
}