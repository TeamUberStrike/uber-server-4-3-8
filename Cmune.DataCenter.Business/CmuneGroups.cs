using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// This class manages the groups that a user is part of (clans (teams, communities) 
    /// </summary>
    public static class CmuneGroups
    {
        /// <summary>
        /// Create a new clan
        /// </summary>
        /// <param name="name"></param>
        /// <param name="motto"></param>
        /// <param name="applicationId"></param>
        /// <param name="cmid"></param>
        /// <param name="tag"></param>
        /// <param name="locale"></param>
        /// <param name="clanId"></param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int CreateGroup(int cmid, string name, string motto, string tag, string locale, int applicationId, out int clanId)
        {
            clanId = 0;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int ret = GroupOperationResult.MemberNotFound;
                bool isInputValid = true;

                if (isInputValid)
                {
                    int isValidClanName = ValidationUtils.IsGroupNameFullyValid(name, locale);

                    if (isValidClanName != GroupOperationResult.Ok)
                    {
                        ret = isValidClanName;
                        isInputValid = false;
                    }
                }

                if (isInputValid)
                {
                    int isValidClanTag = ValidationUtils.IsGroupTagFullyValid(tag, locale);

                    if (isValidClanTag != GroupOperationResult.Ok)
                    {
                        ret = isValidClanTag;
                        isInputValid = false;
                    }
                }

                if (isInputValid)
                {
                    int isValidClanMotto = ValidationUtils.IsGroupMottoFullyValid(motto);

                    if (isValidClanMotto != GroupOperationResult.Ok)
                    {
                        ret = isValidClanMotto;
                        isInputValid = false;
                    }
                }

                if (isInputValid)
                {
                    int areClanAndTagDuplicated = AreDuplicateNameAndTag(name, tag);

                    if (areClanAndTagDuplicated != GroupOperationResult.Ok)
                    {
                        ret = areClanAndTagDuplicated;
                        isInputValid = false;
                    }
                }

                if (isInputValid)
                {
                    if (IsMemberPartOfAnyGroup(cmid, applicationId))
                    {
                        ret = GroupOperationResult.AlreadyMemberOfAGroup;
                        isInputValid = false;
                    }
                }

                if (isInputValid)
                {
                    DateTime creationDate = DateTime.Now;

                    Group newGroup = new Group();
                    newGroup.DateCreated = creationDate;
                    newGroup.LastUpdated = creationDate;
                    newGroup.Name = ValidationUtilities.StandardizeClanName(name);
                    newGroup.NbMembers = 0;
                    newGroup.TagName = ValidationUtilities.StandardizeClanTag(tag);
                    newGroup.NbMembersLimitation = CommonConfig.GroupMembersLimitCount;
                    newGroup.ApplicationId = applicationId;
                    newGroup.OwnerCmid = cmid;
                    newGroup.ColorStyle = (int)GroupColor.Black;
                    newGroup.FontStyle = (int)GroupFontStyle.Normal;
                    newGroup.Type = (int)GroupType.Clan;
                    newGroup.Motto = motto.Trim();

                    //not in use anymore
                    newGroup.Description = String.Empty;
                    newGroup.Address = String.Empty;
                    newGroup.Picture = String.Empty;

                    cmuneDB.Groups.InsertOnSubmit(newGroup);
                    cmuneDB.SubmitChanges();

                    clanId = newGroup.GID;

                    ret = JoinAClan(newGroup.GID, cmid, GroupPosition.Leader);
                }

                return ret;
            }
        }

        /// <summary>
        /// Transfers the ownership to another member of the clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="previousLeaderCmid"></param>
        /// <param name="newLeaderCmid"></param>
        /// <returns>GroupOperationResult</returns>
        public static int TransferOwnership(int groupId, int previousLeaderCmid, int newLeaderCmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isOwnershipTransferred = GroupOperationResult.GroupNotFound;
                Group group = cmuneDB.Groups.SingleOrDefault<Group>(f => f.GID == groupId);

                if (group != null)
                {
                    if (group.OwnerCmid.Equals(previousLeaderCmid))
                    {
                        GroupMember groupMemberNewLeader = GetGroupMember(groupId, newLeaderCmid, cmuneDB);

                        if (groupMemberNewLeader != null)
                        {
                            GroupMember groupMemberPreviousLeader = GetGroupMember(groupId, previousLeaderCmid, cmuneDB);

                            if (groupMemberPreviousLeader != null)
                            {
                                groupMemberNewLeader.Position = (int)GroupPosition.Leader;
                                group.OwnerCmid = newLeaderCmid;

                                groupMemberPreviousLeader.Position = (int)GroupPosition.Member;

                                cmuneDB.SubmitChanges();

                                isOwnershipTransferred = GroupOperationResult.Ok;
                            }
                            else
                            {
                                isOwnershipTransferred = GroupOperationResult.MemberNotFound;
                            }
                        }
                        else
                        {
                            isOwnershipTransferred = GroupOperationResult.MemberNotFound;
                        }
                    }
                    else
                    {
                        isOwnershipTransferred = GroupOperationResult.IsNotOwner;
                    }
                }
                else
                {
                    isOwnershipTransferred = GroupOperationResult.GroupNotFound;
                }

                return isOwnershipTransferred;
            }
        }

        /// <summary>
        /// Deletes a clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmidTakingAction"></param>
        /// <returns></returns>
        public static int DisbandGroup(int groupId, int cmidTakingAction)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isGroupDeleted = GroupOperationResult.GroupNotFound;
                Group group = GetGroup(groupId, cmuneDB);

                if (group != null && group.OwnerCmid == cmidTakingAction)
                {
                    // We need to update the Tag of the clan members and to resume their requests

                    List<GroupMember> membersList = GetClanMembersList(groupId, cmuneDB);

                    foreach (GroupMember clanMember in membersList)
                    {
                        clanMember.Member.TagName = String.Empty;
                        ResumeMemberRequests(clanMember.CMID);
                    }

                    cmuneDB.Groups.DeleteOnSubmit(group);
                    cmuneDB.SubmitChanges();

                    isGroupDeleted = GroupOperationResult.Ok;
                }
                else
                {
                    if (group == null)
                    {
                        isGroupDeleted = GroupOperationResult.GroupNotFound;
                    }
                    else
                    {
                        isGroupDeleted = GroupOperationResult.IsNotOwner;
                    }
                }

                return isGroupDeleted;
            }
        }

        /// <summary>
        /// Checks whether a member is part of a specific group
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsMemberPartOfGroup(int cmid, int groupId)
        {
            bool isMemberPartOfGroup = false;

            GroupMember groupCurrentMember = GetGroupMember(groupId, cmid);

            if (groupCurrentMember != null)
            {
                isMemberPartOfGroup = true;
            }

            return isMemberPartOfGroup;
        }

        /// <summary>
        /// Checks whether a member is part of any group
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool IsMemberPartOfAnyGroup(int cmid, int applicationId)
        {
            bool isMemberPartOfAnyGroup = false;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                GroupMember groupCurrentMember = cmuneDB.GroupMembers.SingleOrDefault(gM => gM.CMID == cmid && gM.ApplicationId == applicationId && gM.IsCurrent == true);

                if (groupCurrentMember != null)
                {
                    isMemberPartOfAnyGroup = true;
                }
            }

            return isMemberPartOfAnyGroup;
        }

        #region Is duplicate

        /// <summary>
        /// Checks whether a clan name and tag are duplicate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int AreDuplicateNameAndTag(string name, string tag)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isNameAndTagDuplicate = GroupOperationResult.Ok;

                name = ValidationUtilities.StandardizeClanName(name);
                tag = ValidationUtilities.StandardizeClanTag(tag);

                List<Group> groupToQuery = (from tab in cmuneDB.Groups where tab.Name == name || tab.TagName == tag select tab).ToList();

                if (!groupToQuery.Count.Equals(0))
                {
                    if (groupToQuery[0].Name.Equals(name))
                    {
                        isNameAndTagDuplicate = GroupOperationResult.DuplicateName;
                    }
                    else
                    {
                        isNameAndTagDuplicate = GroupOperationResult.DuplicateTag;
                    }
                }

                return isNameAndTagDuplicate;
            }
        }

        /// <summary>
        /// Checks wether a clan tag is duplicate
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool IsDuplicateTag(string tag)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isTagDuplicate = false;

                if (!tag.IsNullOrFullyEmpty())
                {
                    tag = ValidationUtilities.StandardizeClanTag(tag);

                    Group group = cmuneDb.Groups.SingleOrDefault(g => g.TagName == tag);

                    if (group != null)
                    {
                        isTagDuplicate = true;
                    }
                }

                return isTagDuplicate;
            }
        }

        /// <summary>
        /// Checks wether a clan name is duplicate
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsDuplicateName(string name)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isNameDuplicate = false;

                if (!name.IsNullOrFullyEmpty())
                {
                    name = ValidationUtilities.StandardizeClanName(name);

                    Group group = cmuneDb.Groups.SingleOrDefault(g => g.Name == name);

                    if (group != null)
                    {
                        isNameDuplicate = true;
                    }
                }

                return isNameDuplicate;
            }
        }

        #endregion Is duplicate

        /// <summary>
        /// Kick a member from a clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmidTakingAction"></param>
        /// <param name="cmidToKick"></param>
        /// <returns></returns>
        public static int KickMemberFromClan(int groupId, int cmidTakingAction, int cmidToKick)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isMemberKicked = GroupOperationResult.MemberNotFound;

                if (cmidTakingAction != cmidToKick)
                {
                    GroupMember groupMemberBeingKicked = GetGroupMember(groupId, cmidToKick, cmuneDB);
                    GroupMember groupMemberTakingAction = GetGroupMember(groupId, cmidTakingAction, cmuneDB);

                    if (groupMemberBeingKicked != null && groupMemberTakingAction != null)
                    {
                        // Leaders can kick everybody (except themself)
                        // Officers can only kick juniors

                        if ((groupMemberTakingAction.Position == (int)GroupPosition.Leader || groupMemberTakingAction.Position == (int)GroupPosition.Officer) &&
                            !(groupMemberTakingAction.Position == (int)GroupPosition.Officer && groupMemberBeingKicked.Position != (int)GroupPosition.Member))
                        {
                            // Duplicate code with LeaveAClan

                            groupMemberBeingKicked.DateQuit = DateTime.Now;
                            groupMemberBeingKicked.IsCurrent = false;

                            Group group = GetGroup(groupId, cmuneDB);
                            group.NbMembers -= 1;

                            Member memberToJoin = CmuneMember.GetMember(cmidToKick, cmuneDB);

                            if (memberToJoin != null)
                            {
                                memberToJoin.TagName = String.Empty;
                                ResumeMemberRequests(cmidToKick);
                            }

                            cmuneDB.SubmitChanges();

                            isMemberKicked = GroupOperationResult.Ok;
                        }
                        else
                        {
                            isMemberKicked = GroupOperationResult.NotEnoughRight;
                        }
                    }
                    else
                    {
                        isMemberKicked = GroupOperationResult.MemberNotFound;
                    }
                }

                return isMemberKicked;
            }
        }

        /// <summary>
        /// Let a member join a clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmid"></param>
        /// <param name="position"></param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int JoinAClan(int groupId, int cmid, GroupPosition position)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int ret = GroupOperationResult.GroupNotFound;

                Group groupToJoin = cmuneDB.Groups.SingleOrDefault(f => f.GID == groupId);

                if (groupToJoin != null)
                {
                    bool isAlreadyMemberOfAnotherGroup = !GetGroupId(cmid, groupToJoin.ApplicationId).Equals(0);
                    bool isGroupFull = groupToJoin.NbMembers >= groupToJoin.NbMembersLimitation;

                    if (!isAlreadyMemberOfAnotherGroup && !isGroupFull)
                    {
                        Member memberToJoin = cmuneDB.Members.SingleOrDefault(f => f.CMID == cmid);

                        if (memberToJoin != null)
                        {
                            GroupMember groupMemberToAdd = new GroupMember();
                            groupMemberToAdd.DateJoined = DateTime.Now;
                            groupMemberToAdd.CMID = cmid;
                            groupMemberToAdd.GroupId = groupId;
                            groupMemberToAdd.Position = (int)position;
                            groupMemberToAdd.ApplicationId = groupToJoin.ApplicationId;
                            groupMemberToAdd.IsCurrent = true;

                            cmuneDB.GroupMembers.InsertOnSubmit(groupMemberToAdd);

                            memberToJoin.TagName = groupToJoin.TagName;

                            groupToJoin.NbMembers += 1;

                            cmuneDB.SubmitChanges();

                            PutOnHoldMemberRequests(cmid);

                            ret = GroupOperationResult.Ok;
                        }
                        else
                        {
                            ret = GroupOperationResult.MemberNotFound;
                        }
                    }
                    else
                    {
                        if (isAlreadyMemberOfAnotherGroup)
                        {
                            ret = GroupOperationResult.AlreadyMemberOfAGroup;
                        }
                        else if (isGroupFull)
                        {
                            ret = GroupOperationResult.GroupFull;
                        }
                        else
                        {
                            CmuneLog.LogUnexpectedReturn("yo", "Unexpected case: gid=" + groupId + "&cmid=" + cmid + "&position=" + position);
                        }
                    }
                }
                else
                {
                    ret = GroupOperationResult.GroupNotFound;
                }

                return ret;
            }
        }

        /// <summary>
        /// Leave a clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static int LeaveAClan(int groupId, int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int ret = GroupOperationResult.MemberNotFound;

                GroupMember groupMemberLeaving = GetGroupMember(groupId, cmid, cmuneDB);

                if (groupMemberLeaving != null)
                {
                    Group group = GetGroup(groupId, cmuneDB);

                    if (group != null && group.OwnerCmid != cmid)
                    {
                        // Duplicate code with KickMemberFromClan

                        groupMemberLeaving.DateQuit = DateTime.Now;
                        groupMemberLeaving.IsCurrent = false;

                        group.NbMembers -= 1;

                        Member memberLeaving = CmuneMember.GetMember(cmid, cmuneDB);

                        if (memberLeaving != null)
                        {
                            memberLeaving.TagName = String.Empty;
                            ResumeMemberRequests(cmid);
                        }

                        cmuneDB.SubmitChanges();

                        ret = GroupOperationResult.Ok;
                    }
                    else
                    {
                        if (group == null)
                        {
                            ret = GroupOperationResult.GroupNotFound;
                        }
                        else
                        {
                            ret = GroupOperationResult.IsOwner;
                        }
                    }
                }
                else
                {
                    ret = GroupOperationResult.MemberNotFound;
                }

                return ret;
            }
        }

        /// <summary>
        /// Update the position of a clan member
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmidMakingAction"></param>
        /// <param name="memberCmid"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int UpdateMemberPosition(int groupId, int cmidMakingAction, int memberCmid, GroupPosition position)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isPositionUpdated = GroupOperationResult.MemberNotFound;

                GroupMember groupMemberMakingAction = GetGroupMember(groupId, cmidMakingAction, cmuneDB);
                GroupMember groupMember = GetGroupMember(groupId, memberCmid, cmuneDB);

                if (groupMemberMakingAction != null && groupMember != null)
                {
                    if ((GroupPosition)groupMemberMakingAction.Position == GroupPosition.Leader
                        && (position == GroupPosition.Member || position == GroupPosition.Officer))
                    {
                        groupMember.Position = (int)position;
                        cmuneDB.SubmitChanges();

                        isPositionUpdated = GroupOperationResult.Ok;
                    }
                    else
                    {
                        isPositionUpdated = GroupOperationResult.NotEnoughRight;
                    }
                }
                else
                {
                    isPositionUpdated = GroupOperationResult.MemberNotFound;
                }

                return isPositionUpdated;
            }
        }

        #region Request to join a group

        /// <summary>
        /// Imped member requests
        /// </summary>
        /// <param name="cmid"></param>
        public static void PutOnHoldMemberRequests(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                object[] sqlParams = { cmid };

                if (cmid != 0)
                {
                    // TODO What happens if we change this table?
                    string updateQuery = "Update [Cmune].[dbo].[RequestGroup] set IsApproved = " + (int)GroupRequestStatus.OnHold + " where Cmid = {0} and IsApproved = " + (int)GroupRequestStatus.Pending;
                    cmuneDB.ExecuteCommand(updateQuery, sqlParams);
                }
            }
        }

        /// <summary>
        /// Resume Impeded Member Requests
        /// </summary>
        /// <param name="cmid"></param>
        public static void ResumeMemberRequests(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                object[] sqlParams = { cmid };
                if (!cmid.Equals(0))
                {
                    // TODO What happens if we change this table?
                    string updateQuery = "Update [Cmune].[dbo].[RequestGroup] set IsApproved = " + (int)GroupRequestStatus.Pending + " where Cmid = {0} and IsApproved = " + (int)GroupRequestStatus.OnHold;
                    cmuneDB.ExecuteCommand(updateQuery, sqlParams);
                }
            }
        }

        /// <summary>
        /// Cancel request
        /// </summary>
        /// <param name="groupRequestId"></param>
        /// <returns></returns>
        public static int CancelRequest(int groupRequestId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isRequestCanceled = GroupOperationResult.RequestNotFound;

                RequestGroup groupRequest = cmuneDB.RequestGroups.SingleOrDefault(rG => rG.Grid == groupRequestId);

                if (groupRequest != null)
                {
                    cmuneDB.RequestGroups.DeleteOnSubmit(groupRequest);
                    cmuneDB.SubmitChanges();

                    isRequestCanceled = GroupOperationResult.Ok;
                }
                else
                {
                    isRequestCanceled = GroupOperationResult.RequestNotFound;
                }

                return isRequestCanceled;
            }
        }

        #endregion Request to join a group

        #region Invite to join a group

        /// <summary>
        /// Accept a group invitation
        /// </summary>
        /// <param name="groupInvitationId"></param>
        /// <param name="cmid"></param>
        /// <returns>ClanRequestAcceptView</returns>
        public static ClanRequestAcceptView AnswerGroupInvitation(int groupInvitationId, int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                ClanRequestAcceptView result = new ClanRequestAcceptView() { ActionResult = GroupOperationResult.GroupNotFound, ClanRequestId = groupInvitationId };

                GroupInvitation groupInvitation = cmuneDB.GroupInvitations.SingleOrDefault(f => f.GroupInvitationId == groupInvitationId && f.InviteeCmid == cmid);

                if (groupInvitation != null)
                {
                    result.ActionResult = JoinAClan(groupInvitation.Gid, groupInvitation.InviteeCmid, GroupPosition.Member);

                    if (result.ActionResult == GroupOperationResult.Ok)
                    {
                        cmuneDB.GroupInvitations.DeleteOnSubmit(groupInvitation);
                        cmuneDB.SubmitChanges();

                        Group group = GetGroup(groupInvitation.Gid, cmuneDB);
                        if (group != null)
                        {
                            ID3 ownerId3 = CmuneMember.GetId3(group.OwnerCmid);

                            if (ownerId3 != null)
                            {
                                List<ClanMemberView> members = GetClanMembersViewList(group.GID);
                                result.ClanView = new ClanView(group.GID, group.NbMembers, group.Description, group.Name, group.Motto, group.Address, group.DateCreated, group.Picture, (GroupType)group.Type, group.LastUpdated, group.TagName, group.NbMembersLimitation, (GroupColor)group.ColorStyle, (GroupFontStyle)group.FontStyle, group.ApplicationId, group.OwnerCmid, ownerId3.Name, members);
                            }
                        }
                    }
                }
                else
                {
                    result.ActionResult = GroupOperationResult.InvitationNotFound;
                }

                return result;
            }
        }

        /// <summary>
        /// Decline a group invitation
        /// </summary>
        /// <param name="groupInvitationId"></param>
        /// <param name="cmid"></param>
        /// <returns>ClanRequestDeclineView</returns>
        public static ClanRequestDeclineView DeclineGroupInvitation(int groupInvitationId, int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int result = GroupOperationResult.GroupNotFound;

                GroupInvitation groupInvitation = cmuneDB.GroupInvitations.SingleOrDefault(f => f.GroupInvitationId == groupInvitationId);

                if (groupInvitation != null)
                {
                    cmuneDB.GroupInvitations.DeleteOnSubmit(groupInvitation);
                    cmuneDB.SubmitChanges();
                }
                else
                {
                    result = GroupOperationResult.InvitationNotFound;
                }

                return new ClanRequestDeclineView() { ActionResult = result, ClanRequestId = groupInvitationId };
            }
        }

        /// <summary>
        /// Answer a group invitation
        /// </summary>
        /// <param name="groupInvitationId"></param>
        /// <param name="isInvitationAccepted"></param>
        /// <returns>(from GroupOperationResult)</returns>
        public static int AnswerGroupInvitation(int groupInvitationId, bool isInvitationAccepted)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int result = GroupOperationResult.GroupNotFound;

                GroupInvitation groupInvitation = cmuneDB.GroupInvitations.SingleOrDefault(f => f.GroupInvitationId == groupInvitationId);

                if (groupInvitation != null)
                {
                    bool shouldDeleteInvitation = false;

                    if (isInvitationAccepted == true)
                    {
                        result = JoinAClan(groupInvitation.Gid, groupInvitation.InviteeCmid, GroupPosition.Member);

                        if (result.Equals(GroupOperationResult.Ok))
                        {
                            shouldDeleteInvitation = true;
                        }
                    }
                    else
                    {
                        shouldDeleteInvitation = true;
                        result = GroupOperationResult.Ok;
                    }

                    if (shouldDeleteInvitation)
                    {
                        cmuneDB.GroupInvitations.DeleteOnSubmit(groupInvitation);
                        cmuneDB.SubmitChanges();
                    }
                }
                else
                {
                    result = GroupOperationResult.InvitationNotFound;
                }

                return result;
            }
        }

        /// <summary>
        /// Invite member to join a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="inviterCmid"></param>
        /// <param name="inviteeCmid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int InviteMemberToJoinAGroup(int groupId, int inviterCmid, int inviteeCmid, string message)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isMemberInvited = GroupOperationResult.MemberNotFound;

                GroupMember groupMemberInviter = GetGroupMember(groupId, inviterCmid);

                if (groupMemberInviter != null && (groupMemberInviter.Position == (int)GroupPosition.Leader || groupMemberInviter.Position == (int)GroupPosition.Officer))
                {
                    Group group = GetGroup(groupId);

                    if (group != null && group.NbMembers <= group.NbMembersLimitation)
                    {
                        GroupInvitation groupInvitation = GetGroupInvitation(groupId, inviterCmid, inviteeCmid);

                        if (groupInvitation == null) // Ok let's invite!
                        {
                            if (!IsMemberPartOfGroup(inviteeCmid, groupId))
                            {
                                groupInvitation = new GroupInvitation();
                                groupInvitation.Gid = groupId;
                                groupInvitation.InviterCmid = inviterCmid;
                                groupInvitation.InviteeCmid = inviteeCmid;
                                groupInvitation.DateInvitation = DateTime.Now;
                                groupInvitation.Message = message;
                                groupInvitation.ApplicationId = groupMemberInviter.ApplicationId;

                                cmuneDB.GroupInvitations.InsertOnSubmit(groupInvitation);
                                cmuneDB.SubmitChanges();

                                isMemberInvited = GroupOperationResult.Ok;
                            }
                            else
                            {
                                isMemberInvited = GroupOperationResult.AlreadyMemberOfAGroup;
                            }
                        }
                        else
                        {
                            isMemberInvited = GroupOperationResult.AlreadyInvited;
                        }
                    }
                    else
                    {
                        if (group == null)
                        {
                            isMemberInvited = GroupOperationResult.GroupNotFound;
                        }
                        else
                        {
                            isMemberInvited = GroupOperationResult.GroupFull;
                        }
                    }
                }
                else
                {
                    if (groupMemberInviter == null)
                    {
                        isMemberInvited = GroupOperationResult.MemberNotFound;
                    }
                    else
                    {
                        isMemberInvited = GroupOperationResult.NotEnoughRight;
                    }
                }

                return isMemberInvited;
            }
        }

        /// <summary>
        /// Get Group Invitation By Members And GroupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="inviterCmid"></param>
        /// <param name="inviteeCmid"></param>
        /// <returns></returns>
        public static GroupInvitation GetGroupInvitation(int groupId, int inviterCmid, int inviteeCmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                GroupInvitation groupToSelect = cmuneDB.GroupInvitations.SingleOrDefault<GroupInvitation>(f => f.Gid == groupId && f.InviterCmid == inviterCmid && f.InviteeCmid == inviteeCmid);
                return groupToSelect;
            }
        }

        /// <summary>
        /// Get all the group invitations
        /// </summary>
        /// <param name="inviteeCmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<GroupInvitationView> GetAllGroupInvitations(int inviteeCmid, int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<GroupInvitationView> invitationsViewList = new List<GroupInvitationView>();

                List<GroupInvitation> groupInvitationList = (from tab in cmuneDb.GroupInvitations where tab.InviteeCmid == inviteeCmid && tab.ApplicationId == applicationId select tab).ToList();

                if (groupInvitationList != null && groupInvitationList.Count > 0)
                {
                    List<int> invitersCmid = groupInvitationList.Select(gI => gI.InviterCmid).ToList();
                    Dictionary<int, string> invitersName = CmuneMember.GetMembersNames(invitersCmid);

                    string inviteeName = String.Empty;
                    ID3 inviteeId3 = CmuneMember.GetId3(inviteeCmid);

                    if (inviteeId3 != null)
                    {
                        inviteeName = inviteeId3.Name;
                    }

                    // We can have multiple invitations from the same group
                    Dictionary<int, int> groupsId = new Dictionary<int, int>();

                    foreach (GroupInvitation groupInvitation in groupInvitationList)
                    {
                        if (!groupsId.ContainsKey(groupInvitation.Gid))
                        {
                            groupsId.Add(groupInvitation.Gid, groupInvitation.Gid);
                        }
                    }

                    List<Group> groups = GetGroups(groupsId.Keys.ToList());
                    Dictionary<int, Group> groupsOrdered = groups.ToDictionary(g => g.GID);

                    int groupId = 0;
                    int inviterCmid = 0;

                    foreach (GroupInvitation groupInvitation in groupInvitationList)
                    {
                        inviterCmid = groupInvitation.InviterCmid;
                        groupId = groupInvitation.Gid;

                        invitationsViewList.Add(new GroupInvitationView(inviterCmid, invitersName[inviterCmid], groupsOrdered[groupId].Name, groupsOrdered[groupId].TagName, groupId, groupInvitation.GroupInvitationId, inviteeCmid, inviteeName, groupInvitation.Message));
                    }
                }

                return invitationsViewList;
            }
        }

        /// <summary>
        /// Get the pending group invitations
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<GroupInvitationView> GetPendingGroupInvitations(int groupId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<GroupInvitationView> invitationsViewList = new List<GroupInvitationView>();

                List<GroupInvitation> groupInvitationList = (from tab in cmuneDB.GroupInvitations where tab.Gid == groupId && tab.IsApproved == false select tab).ToList();

                if (groupInvitationList != null && groupInvitationList.Count > 0)
                {
                    // We can have multiple invitations from the same member
                    Dictionary<int, int> invitersCmid = new Dictionary<int, int>();

                    foreach (GroupInvitation groupInvitation in groupInvitationList)
                    {
                        if (!invitersCmid.ContainsKey(groupInvitation.InviterCmid))
                        {
                            invitersCmid.Add(groupInvitation.InviterCmid, groupInvitation.InviterCmid);
                        }
                    }

                    Dictionary<int, string> invitersName = CmuneMember.GetMembersNames(invitersCmid.Keys.ToList());

                    // Only from invitation from the same group to the same member
                    List<int> inviteesCmid = groupInvitationList.Select(gI => gI.InviteeCmid).ToList();
                    Dictionary<int, string> inviteesName = CmuneMember.GetMembersNames(inviteesCmid);

                    Group group = GetGroup(groupId);

                    if (group != null)
                    {
                        int inviterCmid = 0;
                        int inviteeCmid = 0;

                        foreach (GroupInvitation groupInvitation in groupInvitationList)
                        {
                            inviterCmid = groupInvitation.InviterCmid;
                            inviteeCmid = groupInvitation.InviteeCmid;

                            invitationsViewList.Add(new GroupInvitationView(inviterCmid, invitersName[inviterCmid], group.Name, group.TagName, groupId, groupInvitation.GroupInvitationId, inviteeCmid, inviteesName[inviteeCmid], groupInvitation.Message));
                        }
                    }
                }

                return invitationsViewList;
            }
        }

        /// <summary>
        /// Cancel invitation
        /// </summary>
        /// <param name="groupInvitationId"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static int CancelInvitation(int groupInvitationId, int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int isInvitationCanceled = GroupOperationResult.InvitationNotFound;

                GroupInvitation groupInvitation = cmuneDB.GroupInvitations.SingleOrDefault(gI => gI.GroupInvitationId == groupInvitationId && gI.IsApproved == false);

                if (groupInvitation != null)
                {
                    GroupMember groupMember = GetGroupMember(groupInvitation.Gid, cmid);

                    if (groupMember != null && (groupMember.Position == (int)GroupPosition.Leader || groupMember.Position == (int)GroupPosition.Officer))
                    {
                        cmuneDB.GroupInvitations.DeleteOnSubmit(groupInvitation);
                        cmuneDB.SubmitChanges();

                        isInvitationCanceled = GroupOperationResult.Ok;
                    }
                    else
                    {
                        if (groupMember == null)
                        {
                            isInvitationCanceled = GroupOperationResult.MemberNotFound;
                        }
                        else
                        {
                            isInvitationCanceled = GroupOperationResult.NotEnoughRight;
                        }
                    }
                }
                else
                {
                    isInvitationCanceled = GroupOperationResult.InvitationNotFound;
                }

                return isInvitationCanceled;
            }
        }

        #endregion Invite to join a group

        #region Search Clans

        /// <summary>
        /// Get the list of groups
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<Group> GetClansList(int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<Group> clansList = (from tab in cmuneDB.Groups where tab.ApplicationId == applicationId select tab).ToList();
                return clansList;
            }
        }

        /// <summary>
        /// Allows us to paginate over the clans
        /// range == 0 => 1st clansCount
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="range"></param>
        /// <param name="clansCount"></param>
        /// <returns></returns>
        public static List<Group> GetClans(int applicationId, int range, int clansCount)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int clansToSkipCount = range * clansCount;

                // If you modify this query, you should also modify CountClans
                List<Group> clansList = cmuneDB.Groups.Where(g => g.ApplicationId == applicationId).OrderByDescending(g => g.DateCreated).Skip(clansToSkipCount).Take(clansCount).ToList();
                return clansList;
            }
        }

        /// <summary>
        /// Gets all the clans with the cmids as members
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cmids"></param>
        /// <returns></returns>
        public static List<Group> GetClansByMembers(int applicationId, List<int> cmids)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<Group> clans = new List<Group>();

                List<GroupMember> clansMembers = cmuneDB.GroupMembers.Where(gM => cmids.Contains(gM.CMID) && gM.IsCurrent == true && gM.ApplicationId == applicationId).ToList();

                foreach (GroupMember clanMember in clansMembers)
                {
                    clans.Add(clanMember.Group);
                }

                return clans;
            }
        }

        /// <summary>
        /// Gets a subset of the clans with the cmids as members
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="range"></param>
        /// <param name="clansCount"></param>
        /// <param name="cmids"></param>
        /// <returns></returns>
        public static List<Group> GetClansByMembers(int applicationId, int range, int clansCount, List<int> cmids)
        {
            int clansToSkipCount = range * clansCount;

            // TODO: This function should also include the SQL in the next line for performance reason
            List<Group> clans = GetClansByMembers(applicationId, cmids);

            clans = clans.OrderByDescending(g => g.DateCreated).Skip(clansToSkipCount).Take(clansCount).ToList();

            return clans;
        }

        /// <summary>
        /// Allows us to paginate over the clans when searching them by clan name and / or a clan tag name in a specific application
        /// range == 0 => 1st clansCount
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="range"></param>
        /// <param name="clansCount"></param>
        /// <param name="clanName"></param>
        /// <param name="clanTag"></param>
        /// <param name="memberName">If used, should be bigger than CommonConfig.MemberNameMinLength</param>
        /// <returns></returns>
        public static List<Group> GetClans(int applicationId, int range, int clansCount, string clanName, string clanTag, string memberName)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<Group> clans = new List<Group>();
                int clansToSkipCount = range * clansCount;

                // If you modify these queries, you should also modify CountClans

                memberName = ValidationUtilities.StandardizeMemberName(memberName);

                if (memberName != null && memberName.Length >= CommonConfig.MemberNameMinLength)
                {
                    // Search by member name

                    List<int> membersCmid = CmuneMember.GetCmidFromInfo(String.Empty, memberName, String.Empty, EsnsType.Facebook, false);
                    clans = GetClansByMembers(applicationId, range, clansCount, membersCmid);
                }
                else
                {
                    // Search by clan name & tag

                    clanTag = ValidationUtilities.StandardizeClanTag(clanTag);
                    clanName = ValidationUtilities.StandardizeClanName(clanName);

                    if (!clanTag.IsNullOrFullyEmpty() && !clanTag.IsNullOrFullyEmpty())
                    {
                        clans = cmuneDB.Groups.Where(g => g.ApplicationId == applicationId && SqlMethods.Like(g.TagName, "%" + clanTag + "%") && SqlMethods.Like(g.Name, "%" + clanName + "%")).OrderByDescending(g => g.DateCreated).Skip(clansToSkipCount).Take(clansCount).ToList();
                    }
                    else if (!clanTag.IsNullOrFullyEmpty())
                    {
                        clans = cmuneDB.Groups.Where(g => g.ApplicationId == applicationId && SqlMethods.Like(g.TagName, "%" + clanTag + "%")).OrderByDescending(g => g.DateCreated).Skip(clansToSkipCount).Take(clansCount).ToList();
                    }
                    else if (!clanName.IsNullOrFullyEmpty())
                    {
                        clans = cmuneDB.Groups.Where(g => g.ApplicationId == applicationId && SqlMethods.Like(g.Name, "%" + clanName + "%")).OrderByDescending(g => g.DateCreated).Skip(clansToSkipCount).Take(clansCount).ToList();
                    }
                    else
                    {
                        clans = GetClans(applicationId, range, clansCount);
                    }
                }

                return clans;
            }
        }

        /// <summary>
        /// Allows us to paginate over the clans when searching them by clan name and / or a clan tag name in a specific application
        /// range == 0 => 1st clansCount
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="range"></param>
        /// <param name="clansCount"></param>
        /// <param name="clanName"></param>
        /// <param name="clanTag"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static List<BasicClanView> GetBasicClansView(int applicationId, int range, int clansCount, string clanName, string clanTag, string memberName)
        {
            List<Group> clans = GetClans(applicationId, range, clansCount, clanName, clanTag, memberName);
            return ConvertToBasicClanView(clans);
        }

        #endregion Search clans

        #region Delete member

        /// <summary>
        /// Deletes the community part of a member
        /// To be called when deleting a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        public static void DeleteCommunityInvolvment(int cmid, int applicationId)
        {
            int groupId = GetGroupId(cmid, applicationId);

            if (groupId != 0)
            {
                bool isClanOwner = IsOwner(cmid, groupId, applicationId);

                if (isClanOwner)
                {
                    List<GroupMember> groupMembers = GetClanMembersList(groupId);

                    if (groupMembers.Count > 1)
                    {
                        int newOwnerCmid = cmid;

                        int i = 0;
                        while (i < groupMembers.Count && newOwnerCmid == cmid)
                        {
                            if (groupMembers[i].CMID != cmid)
                            {
                                newOwnerCmid = groupMembers[i].CMID;
                            }

                            i++;
                        }

                        TransferOwnership(groupId, cmid, newOwnerCmid);
                        LeaveAClan(groupId, cmid);
                    }
                    else
                    {
                        DisbandGroup(groupId, cmid);
                    }
                }
                else
                {
                    LeaveAClan(groupId, cmid);
                }
            }

            DeleteGroupMembers(cmid, applicationId);
            DeleteGroupInvitations(cmid, applicationId);
        }

        /// <summary>
        /// Deletes all the Group Members linked to a specific member
        /// To be called when deleting a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        private static void DeleteGroupMembers(int cmid, int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<GroupMember> groupMembers = cmuneDB.GroupMembers.Where(gM => gM.CMID == cmid && gM.ApplicationId == applicationId).ToList();
                cmuneDB.GroupMembers.DeleteAllOnSubmit(groupMembers);
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Deletes all the Group Invitations linked to a specific member
        /// To be called when deleting a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        private static void DeleteGroupInvitations(int cmid, int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                // TODO only remove from group linked the application
                List<GroupInvitation> groupInvitations = cmuneDB.GroupInvitations.Where(gI => gI.InviteeCmid == cmid || gI.InviterCmid == cmid).ToList();
                cmuneDB.GroupInvitations.DeleteAllOnSubmit(groupInvitations);
                cmuneDB.SubmitChanges();
            }
        }

        #endregion Delete member

        /// <summary>
        /// Checks wether a member is the owner of a group or not
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="groupId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool IsOwner(int cmid, int groupId, int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isOwner = false;

                Group group = cmuneDB.Groups.SingleOrDefault(g => g.GID == groupId && g.OwnerCmid == cmid && g.ApplicationId == applicationId);

                if (group != null)
                {
                    isOwner = true;
                }

                return isOwner;
            }
        }

        /// <summary>
        /// When we merge two accounts, we should try to replace the member to delete by the member in a group if possible
        /// </summary>
        /// <param name="cmidToKeep"></param>
        /// <param name="cmidToDelete"></param>
        /// <param name="applicationId"></param>
        public static void MergeMembers(int cmidToKeep, int cmidToDelete, int applicationId)
        {
            int groupIdOdCmidToDelete = GetGroupId(cmidToDelete, applicationId);

            if (groupIdOdCmidToDelete != 0)
            {
                int groupIdOfCmidToKeep = GetGroupId(cmidToKeep, applicationId);
                bool isCmidToDeleteClanOwner = IsOwner(cmidToDelete, groupIdOdCmidToDelete, applicationId);

                if (isCmidToDeleteClanOwner)
                {
                    if (groupIdOfCmidToKeep == groupIdOdCmidToDelete)
                    {
                        TransferOwnership(groupIdOdCmidToDelete, cmidToDelete, cmidToKeep);
                        LeaveAClan(groupIdOdCmidToDelete, cmidToDelete);
                    }
                    else if (groupIdOfCmidToKeep == 0)
                    {
                        JoinAClan(groupIdOdCmidToDelete, cmidToKeep, GroupPosition.Member);
                        TransferOwnership(groupIdOdCmidToDelete, cmidToDelete, cmidToKeep);
                        LeaveAClan(groupIdOdCmidToDelete, cmidToDelete);
                    }
                }
                else
                {
                    LeaveAClan(groupIdOdCmidToDelete, cmidToDelete);

                    if (groupIdOfCmidToKeep == 0)
                    {
                        JoinAClan(groupIdOdCmidToDelete, cmidToKeep, GroupPosition.Member);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a clan list to a clans view list
        /// </summary>
        /// <param name="clans"></param>
        /// <returns></returns>
        public static List<BasicClanView> ConvertToBasicClanView(List<Group> clans)
        {
            List<BasicClanView> basicClansView = new List<BasicClanView>();

            if (clans != null && clans.Count > 0)
            {
                List<int> ownersCmid = clans.Select(g => g.OwnerCmid).ToList();
                Dictionary<int, string> ownersName = CmuneMember.GetMembersNames(ownersCmid);

                foreach (Group clan in clans)
                {
                    BasicClanView basicClanView = new BasicClanView(clan.GID, clan.NbMembers, clan.Description, clan.Name, clan.Motto, clan.Address, clan.DateCreated, clan.Picture, (GroupType)clan.Type, clan.LastUpdated, clan.TagName, clan.NbMembersLimitation, (GroupColor)clan.ColorStyle, (GroupFontStyle)clan.FontStyle, clan.ApplicationId, clan.OwnerCmid, ownersName[clan.OwnerCmid]);
                    basicClansView.Add(basicClanView);
                }
            }

            return basicClansView;
        }

        #region Getters

        /// <summary>
        /// Get a clan view
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static ClanView GetGroupView(int groupId)
        {
            ClanView clanView = null;

            Group group = GetGroup(groupId);

            if (group != null)
            {
                ID3 ownerId3 = CmuneMember.GetId3(group.OwnerCmid);

                if (ownerId3 != null)
                {
                    List<ClanMemberView> members = GetClanMembersViewList(groupId);
                    clanView = new ClanView(groupId, group.NbMembers, group.Description, group.Name, group.Motto, group.Address, group.DateCreated, group.Picture, (GroupType)group.Type, group.LastUpdated, group.TagName, group.NbMembersLimitation, (GroupColor)group.ColorStyle, (GroupFontStyle)group.FontStyle, group.ApplicationId, group.OwnerCmid, ownerId3.Name, members);
                }
            }

            return clanView;
        }

        /// <summary>
        /// Gets a list of the members view of a clan
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<ClanMemberView> GetClanMembersViewList(int groupId)
        {
            List<ClanMemberView> listMembersView = new List<ClanMemberView>();

            List<GroupMember> listMembers = GetClanMembersList(groupId);

            if (listMembers != null && listMembers.Count > 0)
            {
                List<int> clanMembersCmid = listMembers.Select(g => g.CMID).ToList();

                using (CmuneDataContext cmuneDB = new CmuneDataContext())
                {
                    Dictionary<int, ID3> membersId3 = CmuneMember.GetMembersId3(clanMembersCmid, cmuneDB);
                    Dictionary<int, GroupMember> clanMembers = listMembers.ToDictionary(gM => gM.CMID);

                    if (clanMembers.Count == membersId3.Count)
                    {
                        int cmid = 0;

                        for (int i = 0; i < listMembers.Count; i++)
                        {
                            cmid = listMembers[i].CMID;
                            ClanMemberView clanMember = new ClanMemberView(membersId3[cmid].Name, cmid, (GroupPosition)clanMembers[cmid].Position, clanMembers[cmid].DateJoined, (DateTime)membersId3[cmid].Member.LastAliveAck);
                            listMembersView.Add(clanMember);
                        }
                    }
                }
            }

            return listMembersView;
        }

        /// <summary>
        /// Get the members of a specified group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static List<GroupMember> GetClanMembersList(int groupId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<GroupMember> clanMembersList = GetClanMembersList(groupId, cmuneDb);
                return clanMembersList;
            }
        }

        /// <summary>
        /// Get the members of a specified group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<GroupMember> GetClanMembersList(int groupId, CmuneDataContext cmuneDb)
        {
            List<GroupMember> clanMembersList = new List<GroupMember>();

            if (cmuneDb != null)
            {
                clanMembersList = (from tab in cmuneDb.GroupMembers where tab.GroupId == groupId && tab.IsCurrent == true select tab).ToList();
            }

            return clanMembersList;
        }

        /// <summary>
        /// Get a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static Group GetGroup(int groupId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return GetGroup(groupId, cmuneDb);
            }
        }

        /// <summary>
        /// Get a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static Group GetGroup(int groupId, CmuneDataContext cmuneDb)
        {
            Group group = null;

            if (cmuneDb != null)
            {
                group = cmuneDb.Groups.SingleOrDefault(f => f.GID == groupId);
            }

            return group;
        }

        /// <summary>
        /// Get the clan that a user is part of
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static int GetGroupId(int cmid, int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int groupId = 0;

                var query = from gM in cmuneDb.GroupMembers
                            where gM.CMID == cmid && gM.ApplicationId == applicationId && gM.IsCurrent == true
                            select gM.GroupId;

                foreach (var row in query)
                {
                    groupId = row;
                }

                return groupId;
            }
        }

        /// <summary>
        /// Gets the clan that a member is part of
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static Group GetGroup(int cmid, int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                Group group = null;

                GroupMember groupMember = cmuneDB.GroupMembers.SingleOrDefault<GroupMember>(gM => gM.CMID == cmid && gM.ApplicationId == applicationId && gM.IsCurrent == true);

                if (groupMember != null)
                {
                    group = groupMember.Group;
                }

                return group;
            }
        }

        /// <summary>
        /// Get groups
        /// </summary>
        /// <param name="groupsId"></param>
        /// <returns></returns>
        public static List<Group> GetGroups(List<int> groupsId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Group> groups = cmuneDb.Groups.Where(g => groupsId.Contains(g.GID)).ToList();
                return groups;
            }
        }

        /// <summary>
        /// Get group member
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static GroupMember GetGroupMember(int groupId, int cmid, CmuneDataContext cmuneDB)
        {
            GroupMember groupMember = null;

            if (cmuneDB != null)
            {
                groupMember = cmuneDB.GroupMembers.SingleOrDefault(f => f.CMID == cmid && f.GroupId == groupId && f.IsCurrent == true);
            }

            return groupMember;
        }

        /// <summary>
        /// Get group member
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static GroupMember GetGroupMember(int groupId, int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                GroupMember groupMember = GetGroupMember(groupId, cmid, cmuneDB);

                return groupMember;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<GroupMember> GetPreviousGroups(int cmid, int count)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<GroupMember> previousGroups = new List<GroupMember>();

                previousGroups = cmuneDb.GroupMembers.Where(g => g.CMID == cmid && !g.IsCurrent).OrderByDescending(g => g.GMID).Take(count).ToList();

                return previousGroups;
            }
        }

        #endregion Getters

        #region Setters

        /// <summary>
        /// Change the group tag
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupTag"></param>
        /// <param name="locale"></param>
        /// <param name="memberCmids">All the cmids that saw their tag modified (useful if you want to update other databases)</param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int SetGroupTag(int groupId, string groupTag, string locale, out List<int> memberCmids)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int ret = GroupOperationResult.Ok;
                memberCmids = new List<int>();

                ret = ValidationUtils.IsGroupTagFullyValid(groupTag, locale);

                if (ret.Equals(GroupOperationResult.Ok))
                {
                    bool isDuplicateTag = IsDuplicateTag(groupTag);

                    if (!isDuplicateTag)
                    {
                        Group group = GetGroup(groupId, cmuneDb);

                        if (group != null)
                        {
                            groupTag = ValidationUtilities.StandardizeClanTag(groupTag);
                            group.TagName = groupTag;
                            group.LastUpdated = DateTime.Now;

                            List<GroupMember> membersList = GetClanMembersList(groupId, cmuneDb);

                            foreach (GroupMember clanMember in membersList)
                            {
                                clanMember.Member.TagName = groupTag;
                            }

                            memberCmids.AddRange(membersList.Select(m => m.CMID).ToList());

                            cmuneDb.SubmitChanges();

                            ret = GroupOperationResult.Ok;
                        }
                        else
                        {
                            ret = GroupOperationResult.GroupNotFound;
                        }
                    }
                    else
                    {
                        ret = GroupOperationResult.DuplicateTag;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Change the group name
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <param name="locale"></param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int SetGroupName(int groupId, string groupName, string locale)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int ret = GroupOperationResult.Ok;

                ret = ValidationUtils.IsGroupNameFullyValid(groupName, locale);

                if (ret.Equals(GroupOperationResult.Ok))
                {
                    bool isDuplicateName = IsDuplicateName(groupName);

                    if (!isDuplicateName)
                    {
                        Group group = GetGroup(groupId, cmuneDb);

                        if (group != null)
                        {
                            groupName = ValidationUtilities.StandardizeClanName(groupName);
                            group.Name = groupName;
                            group.LastUpdated = DateTime.Now;

                            cmuneDb.SubmitChanges();

                            ret = GroupOperationResult.Ok;
                        }
                        else
                        {
                            ret = GroupOperationResult.GroupNotFound;
                        }
                    }
                    else
                    {
                        ret = GroupOperationResult.DuplicateName;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Change the group motto
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupMotto"></param>
        /// <returns>from (GroupOperationResult)</returns>
        public static int SetGroupMotto(int groupId, string groupMotto)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int ret = GroupOperationResult.Ok;

                ret = ValidationUtils.IsGroupMottoFullyValid(groupMotto);

                if (ret.Equals(GroupOperationResult.Ok))
                {
                    Group group = GetGroup(groupId, cmuneDb);

                    if (group != null)
                    {
                        group.Motto = groupMotto.Trim();
                        group.LastUpdated = DateTime.Now;

                        cmuneDb.SubmitChanges();

                        ret = GroupOperationResult.Ok;
                    }
                    else
                    {
                        ret = GroupOperationResult.GroupNotFound;
                    }
                }

                return ret;
            }
        }

        #endregion Setters
    }
}