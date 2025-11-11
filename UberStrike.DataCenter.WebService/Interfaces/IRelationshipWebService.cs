using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IRelationshipWebService
    {
        int SendContactRequest(int initiatorCmid, int receiverCmid, string message);
        List<ContactRequestView> GetContactRequests(int cmid);

        ContactRequestAcceptView AcceptContactRequest(int contactRequestId, int cmid, int applicationId);
        ContactRequestDeclineView DeclineContactRequest(int contactRequestId, int cmid);

        MemberOperationResult DeleteContact(int cmid, int contactCmid);
        MemberOperationResult MoveContactToGroup(int cmid, int contactCmid, int previousGroupId, int newGroupId);
        List<ContactGroupView> GetContactsByGroups(int cmid, int applicationId);
    }
}