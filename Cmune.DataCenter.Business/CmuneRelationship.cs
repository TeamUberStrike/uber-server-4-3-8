using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages the relations between Cmune members
    /// </summary>
    public static class CmuneRelationship
    {
        /// <summary>
        /// Send a contact request
        /// </summary>
        /// <param name="initiatorCmid"></param>
        /// <param name="receiverCmid"></param>
        /// <param name="initiatorMessage"></param>
        /// <returns></returns>
        public static bool SendContactRequest(int initiatorCmid, int receiverCmid, string initiatorMessage)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isRequestSent = false;
                RequestContact contactRequest = cmuneDB.RequestContacts.SingleOrDefault(f => (f.InitiatorCMID == initiatorCmid && f.ReceiverCMID == receiverCmid) || (f.InitiatorCMID == receiverCmid && f.ReceiverCMID == initiatorCmid));

                if (contactRequest == null && !initiatorCmid.Equals(receiverCmid))
                {
                    // We need to check if the two members are already friends
                    List<MembersRelationship> membersRelationships = cmuneDB.MembersRelationships.Where(mR => (mR.MemberCMID == initiatorCmid && mR.FriendCMID == receiverCmid) || (mR.MemberCMID == receiverCmid && mR.FriendCMID == initiatorCmid)).ToList();

                    if (membersRelationships.Count == 0)
                    {
                        contactRequest = new RequestContact();
                        contactRequest.SentDate = DateTime.Now;
                        contactRequest.Status = (int)ContactRequestStatus.Pending;
                        contactRequest.InitiatorCMID = initiatorCmid;
                        contactRequest.ReceiverCMID = receiverCmid;
                        contactRequest.Message = initiatorMessage;

                        cmuneDB.RequestContacts.InsertOnSubmit(contactRequest);
                        cmuneDB.SubmitChanges();

                        isRequestSent = true;
                    }
                    else
                    {
                        // Those two fellas are already contacts
                    }
                }
                else if (contactRequest != null && contactRequest.ReceiverCMID.Equals(initiatorCmid))
                {
                    // If there is already a contact request initiated by the receiver, we accept it directly instead of sending a friend request
                    isRequestSent = AcceptContactRequest(contactRequest, cmuneDB);
                }
                else
                {
                    // Already sent a contact request to this member
                }

                return isRequestSent;
            }
        }

        /// <summary>
        /// Decline a contact request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="inviteeCmid"></param>
        /// <returns></returns>
        public static ContactRequestDeclineView DeclineContactRequest(int requestId, int inviteeCmid)
        {
            ContactRequestDeclineView result = new ContactRequestDeclineView() { ActionResult = 1, RequestId = requestId };

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                RequestContact contactRequest = cmuneDB.RequestContacts.SingleOrDefault(cR => cR.RequestID == requestId);

                if (contactRequest != null && contactRequest.ReceiverCMID == inviteeCmid)
                {
                    if (RefuseContactRequest(contactRequest, cmuneDB))
                    {
                        result.ActionResult = 0;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Accept a contact request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="inviteeCmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static ContactRequestAcceptView AcceptContactRequest(int requestId, int inviteeCmid, int applicationId)
        {
            ContactRequestAcceptView result = new ContactRequestAcceptView() { ActionResult = 1, RequestId = requestId };

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                RequestContact contactRequest = cmuneDB.RequestContacts.SingleOrDefault(cR => cR.RequestID == requestId);

                if (contactRequest != null && contactRequest.ReceiverCMID == inviteeCmid)
                {
                    if (AcceptContactRequest(contactRequest, cmuneDB))
                    {
                        result.ActionResult = 0;
                        result.Contact = CmuneMember.GetPublicProfile(contactRequest.InitiatorCMID, applicationId);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Respond to a contact request
        /// </summary>
        /// <param name="contactRequestId"></param>
        /// <param name="isContactRequestAccepted"></param>
        /// <returns></returns>
        public static bool RespondContactRequest(int contactRequestId, bool isContactRequestAccepted)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isContactRequestUpdated = false;
                RequestContact contactRequest = cmuneDB.RequestContacts.SingleOrDefault(cR => cR.RequestID == contactRequestId);

                if (contactRequest != null)
                {
                    if (isContactRequestAccepted)
                    {
                        isContactRequestUpdated = AcceptContactRequest(contactRequest, cmuneDB);
                    }
                    else
                    {
                        isContactRequestUpdated = RefuseContactRequest(contactRequest, cmuneDB);
                    }
                }
                else
                {
                    //ContactRequestNotFound
                }

                return isContactRequestUpdated;
            }
        }

        /// <summary>
        /// Accept a contact Request
        /// </summary>
        /// <param name="contactRequest"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        private static bool AcceptContactRequest(RequestContact contactRequest, CmuneDataContext cmuneDB)
        {
            bool isContactRequestAccepted = false;

            if (contactRequest != null && cmuneDB != null)
            {
                ContactGroup initiatorDefaultGroup = cmuneDB.ContactGroups.SingleOrDefault<ContactGroup>(cG => cG.OwnerCmid == contactRequest.InitiatorCMID && cG.Name == CommonConfig.ContactGroupDefaultName);
                ContactGroup receiverDefaultGroup = cmuneDB.ContactGroups.SingleOrDefault<ContactGroup>(cG => cG.OwnerCmid == contactRequest.ReceiverCMID && cG.Name == CommonConfig.ContactGroupDefaultName);

                if (initiatorDefaultGroup != null && receiverDefaultGroup != null)
                {
                    AnalyticalRelationship analyticsRelationship = new AnalyticalRelationship();
                    analyticsRelationship.Created = DateTime.Now;
                    cmuneDB.AnalyticalRelationships.InsertOnSubmit(analyticsRelationship);
                    cmuneDB.SubmitChanges();

                    MembersRelationship memberRelationship = new MembersRelationship();
                    memberRelationship.ARID = analyticsRelationship.ARID;
                    memberRelationship.MemberCMID = contactRequest.InitiatorCMID;
                    memberRelationship.FriendCMID = contactRequest.ReceiverCMID;
                    cmuneDB.MembersRelationships.InsertOnSubmit(memberRelationship);

                    // We need to add the receiver in the member default group but we also need to add the sender to the receiver default group
                    ContactToGroup newInitiatorContactTogroup = new ContactToGroup();
                    newInitiatorContactTogroup.ContactCmid = contactRequest.ReceiverCMID;
                    newInitiatorContactTogroup.ContactGroupID = initiatorDefaultGroup.ContactGroupID;
                    cmuneDB.ContactToGroups.InsertOnSubmit(newInitiatorContactTogroup);

                    ContactToGroup newReceiverContactTogroup = new ContactToGroup();
                    newReceiverContactTogroup.ContactCmid = contactRequest.InitiatorCMID;
                    newReceiverContactTogroup.ContactGroupID = receiverDefaultGroup.ContactGroupID;
                    cmuneDB.ContactToGroups.InsertOnSubmit(newReceiverContactTogroup);

                    cmuneDB.RequestContacts.DeleteOnSubmit(contactRequest);
                    cmuneDB.SubmitChanges();

                    isContactRequestAccepted = true;
                }
            }

            return isContactRequestAccepted;
        }

        /// <summary>
        /// Refuse a contact request
        /// </summary>
        /// <param name="contactRequest"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        private static bool RefuseContactRequest(RequestContact contactRequest, CmuneDataContext cmuneDB)
        {
            bool isContactRequestRefused = false;

            if (contactRequest != null && cmuneDB != null)
            {
                contactRequest.Status = (int)ContactRequestStatus.Refused;
                cmuneDB.SubmitChanges();

                isContactRequestRefused = true;
            }

            return isContactRequestRefused;
        }

        /// <summary>
        /// Gets all the pending contact requests for a specific member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<RequestContact> GetContactRequests(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return (from tab in cmuneDb.RequestContacts where tab.ReceiverCMID == cmid && tab.Status == (int)ContactRequestStatus.Pending orderby tab.RequestID descending select tab).ToList();
            }
        }

        /// <summary>
        /// Gets all the pending contact requests view for a specific member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ContactRequestView> GetContactRequestsView(int cmid)
        {
            List<ContactRequestView> contactRequestsView = new List<ContactRequestView>();

            List<RequestContact> contactRequests = GetContactRequests(cmid);
            List<int> initiatorsCmid = contactRequests.Select(rC => rC.InitiatorCMID).ToList();

            Dictionary<int, string> initiatorsName = CmuneMember.GetMembersNames(initiatorsCmid);

            foreach (RequestContact contactRequest in contactRequests)
            {
                contactRequestsView.Add(new ContactRequestView(contactRequest.RequestID, contactRequest.InitiatorCMID, initiatorsName[contactRequest.InitiatorCMID], contactRequest.ReceiverCMID, contactRequest.Message, (ContactRequestStatus)contactRequest.Status, contactRequest.SentDate));
            }

            return contactRequestsView;
        }

        /// <summary>
        /// Get list of MembersRelationships of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<MembersRelationship> GetMemberRelationships(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<MembersRelationship> listFriends = (from tab in cmuneDB.MembersRelationships where tab.FriendCMID == cmid || tab.MemberCMID == cmid orderby tab.RID descending select tab).ToList();
                return listFriends;
            }
        }

        /// <summary>
        /// Deletes all the the member contacts
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteAllContacts(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                // We need to deleted the Contact To Group of this member and all the Contact to Group where he is present as a friend
                DeleteContactGroups(cmid);

                // We also need to delete the MembersRelationships as there are 2 FK referencing Member.
                cmuneDB.MembersRelationships.DeleteAllOnSubmit(cmuneDB.MembersRelationships.Where<MembersRelationship>(m => m.MemberCMID == cmid || m.FriendCMID == cmid));
                cmuneDB.SubmitChanges();
                ret = MemberOperationResult.Ok;

                return ret;
            }
        }

        /// <summary>
        /// Delete one contact of a specific member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="contactCmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteContact(int cmid, int contactCmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                cmuneDB.ContactToGroups.DeleteAllOnSubmit(cmuneDB.ContactToGroups.Where<ContactToGroup>(cTG => (cmuneDB.ContactGroups.Where<ContactGroup>(cG => cG.OwnerCmid == cmid || cG.OwnerCmid == contactCmid).Select<ContactGroup, int>(cG => cG.ContactGroupID).ToList()).Contains(cTG.ContactGroupID) && (cTG.ContactCmid == cmid || cTG.ContactCmid == contactCmid)));

                cmuneDB.MembersRelationships.DeleteAllOnSubmit(cmuneDB.MembersRelationships.Where<MembersRelationship>(m => (m.MemberCMID == cmid && m.FriendCMID == contactCmid) || (m.FriendCMID == cmid && m.MemberCMID == contactCmid)));
                cmuneDB.SubmitChanges();
                ret = MemberOperationResult.Ok;

                return ret;
            }
        }

        /// <summary>
        /// Deletes all contact requests
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteAllContactRequests(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                cmuneDB.RequestContacts.DeleteAllOnSubmit(cmuneDB.RequestContacts.Where(rC => rC.InitiatorCMID == cmid || rC.ReceiverCMID == cmid));
                cmuneDB.SubmitChanges();

                ret = MemberOperationResult.Ok;

                return ret;
            }
        }

        /// <summary>
        /// Deletes all the login IPs for a specific member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteAllLoginIps(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                cmuneDB.LoginIps.DeleteAllOnSubmit(cmuneDB.LoginIps.Where(loGin => loGin.Cmid == cmid));
                cmuneDB.SubmitChanges();

                ret = MemberOperationResult.Ok;

                return ret;
            }
        }

        /// <summary>
        /// Count the contacts of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static int CountContacts(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int contactsCount = (from tab in cmuneDB.MembersRelationships where tab.FriendCMID == cmid || tab.MemberCMID == cmid orderby tab.RID descending select tab).Count();

                return contactsCount;
            }
        }

        #region Group

        /// <summary>
        /// Moves a contact from a group to another
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="contactCmid"></param>
        /// <param name="previousGroupID"></param>
        /// <param name="newGroupID"></param>
        public static void MoveContactToGroup(int cmid, int contactCmid, int previousGroupID, int newGroupID)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                ContactToGroup oldContactToGroup = cmuneDB.ContactToGroups.SingleOrDefault<ContactToGroup>(cG => cG.ContactGroupID == previousGroupID && cG.ContactCmid == contactCmid);

                List<ContactGroup> memberGroups = cmuneDB.ContactGroups.Where(cG => (cG.ContactGroupID == previousGroupID || cG.ContactGroupID == newGroupID) && cG.OwnerCmid == cmid).ToList();

                if (oldContactToGroup != null && memberGroups.Count == 2)
                {
                    oldContactToGroup.ContactGroupID = newGroupID;
                    cmuneDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Get all the contacts of a member ordered by groups
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ContactGroupView> GetContactsByGroups(int cmid, int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<ContactGroupView> contactsGroups = new List<ContactGroupView>();

                List<ContactGroup> groups = cmuneDb.ContactGroups.Where<ContactGroup>(cG => cG.OwnerCmid == cmid).OrderBy<ContactGroup, int>(cG => cG.ContactGroupID).ToList();

                List<int> groupContacts = new List<int>();
                List<PublicProfileView> contactsProfiles = new List<PublicProfileView>();

                foreach (ContactGroup group in groups)
                {
                    groupContacts = cmuneDb.ContactToGroups.Where<ContactToGroup>(cTG => cTG.ContactGroupID == group.ContactGroupID).Select<ContactToGroup, int>(cTG => cTG.ContactCmid).ToList();
                    contactsProfiles = CmuneMember.GetPublicProfiles(groupContacts, applicationId);

                    contactsGroups.Add(new ContactGroupView(group.ContactGroupID, group.Name, contactsProfiles));
                }

                return contactsGroups;
            }
        }

        /// <summary>
        /// Create a contact group for a specific member
        /// </summary>
        /// <param name="ownerCmid"></param>
        /// <param name="groupName"></param>
        /// <param name="checkConstraints"></param>
        /// <returns></returns>
        private static ContactGroupOperationResult CreateContactGroup(int ownerCmid, string groupName, bool checkConstraints)
        {
            ContactGroupOperationResult ret = ContactGroupOperationResult.InvalidName;

            bool isValidContactGroupName = false;
            if (checkConstraints)
            {
                isValidContactGroupName = ValidationUtilities.IsValidContactGroupName(groupName);
            }
            else
            {
                isValidContactGroupName = ValidationUtilities.IsValidContactGroupName(groupName, false);
            }

            if (isValidContactGroupName)
            {
                if (!IsDuplicateContactGroupName(ownerCmid, groupName))
                {
                    using (CmuneDataContext cmuneDB = new CmuneDataContext())
                    {
                        ContactGroup contactGroup = new ContactGroup();
                        contactGroup.Name = ValidationUtilities.StandardizeContactGroupName(groupName);
                        contactGroup.OwnerCmid = ownerCmid;
                        cmuneDB.ContactGroups.InsertOnSubmit(contactGroup);
                        cmuneDB.SubmitChanges();

                        ret = ContactGroupOperationResult.Ok;
                    }
                }
                else
                {
                    ret = ContactGroupOperationResult.DuplicateName;
                }
            }

            return ret;
        }

        /// <summary>
        /// Create default contact groups when creatin a member
        /// </summary>
        /// <param name="ownerCmid"></param>
        /// <returns></returns>
        public static ContactGroupOperationResult CreateContactGroups(int ownerCmid)
        {
            ContactGroupOperationResult ret = ContactGroupOperationResult.Ok;

            foreach (string groupName in CommonConfig.DefaultContactGroupToBeCreated)
            {
                ContactGroupOperationResult groupCreationResult = CreateContactGroup(ownerCmid, groupName, false);

                if (!groupCreationResult.Equals(ContactGroupOperationResult.Ok))
                {
                    CmuneLog.LogUnexpectedReturn(groupCreationResult, "ownerCmid=" + ownerCmid + "&groupName=" + groupName + "&checkConstraints=" + false);
                }
            }

            return ret;
        }

        /// <summary>
        /// Deletes all the contact groups of a member (called on all contacts deletion)
        /// Removes this CMID from the contact to group of his contact
        /// </summary>
        /// <param name="cmid"></param>
        private static void DeleteContactGroups(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<ContactGroup> contactGroups = cmuneDB.ContactGroups.Where<ContactGroup>(cG => cG.OwnerCmid == cmid).ToList();

                List<int> contactGroupsIDs = contactGroups.Select<ContactGroup, int>(cG => cG.ContactGroupID).ToList();

                cmuneDB.ContactToGroups.DeleteAllOnSubmit(cmuneDB.ContactToGroups.Where(cTG => contactGroupsIDs.Contains(cTG.ContactGroupID)));
                cmuneDB.ContactToGroups.DeleteAllOnSubmit(cmuneDB.ContactToGroups.Where(cTG => cTG.ContactCmid == cmid));
                cmuneDB.SubmitChanges();

                cmuneDB.ContactGroups.DeleteAllOnSubmit(contactGroups);
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Checks wether a member is already having a contact group with the same name
        /// </summary>
        /// <param name="ownerCmid"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool IsDuplicateContactGroupName(int ownerCmid, string groupName)
        {
            bool isContactGroupNameDuplicated = false;

            if (groupName != null)
            {
                groupName = ValidationUtilities.StandardizeContactGroupName(groupName);

                if (!groupName.IsNullOrFullyEmpty())
                {
                    using (CmuneDataContext cmuneDB = new CmuneDataContext())
                    {
                        List<ContactGroup> contactGroup = cmuneDB.ContactGroups.Where<ContactGroup>(cG => cG.Name == groupName && cG.OwnerCmid == ownerCmid).ToList();
                        if (contactGroup.Count > 0)
                        {
                            isContactGroupNameDuplicated = true;
                        }
                    }
                }
            }

            return isContactGroupNameDuplicated;
        }

        #endregion Group
    }
}