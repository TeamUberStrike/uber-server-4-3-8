using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using System.Collections.Generic;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class RelationshipWebService : IRelationshipWebService
    {
        public int SendContactRequest(int initiatorCmid, int receiverCmid, string message)
        {
             CmuneRelationship.SendContactRequest(initiatorCmid, receiverCmid, message);
             return receiverCmid;
        }

        public List<ContactRequestView> GetContactRequests(int cmid)
        {
            return CmuneRelationship.GetContactRequestsView(cmid);
        }

        public MemberOperationResult DeleteContact(int cmid, int contactCmid)
        {
            return CmuneRelationship.DeleteContact(cmid, contactCmid);
        }

        public MemberOperationResult MoveContactToGroup(int cmid, int contactCmid, int previousGroupId, int newGroupId)
        {
            CmuneRelationship.MoveContactToGroup(cmid, contactCmid, previousGroupId, newGroupId);

            return MemberOperationResult.Ok;
        }

        public List<ContactGroupView> GetContactsByGroups(int cmid, int applicationId)
        {
            return CmuneRelationship.GetContactsByGroups(cmid, applicationId);
        }

        public ContactRequestAcceptView AcceptContactRequest(int contactRequestId, int cmid, int applicationId)
        {
            return CmuneRelationship.AcceptContactRequest(contactRequestId, cmid, applicationId);
        }

        public ContactRequestDeclineView DeclineContactRequest(int contactRequestId, int cmid)
        {
            return CmuneRelationship.DeclineContactRequest(contactRequestId, cmid);
        }
    }
}