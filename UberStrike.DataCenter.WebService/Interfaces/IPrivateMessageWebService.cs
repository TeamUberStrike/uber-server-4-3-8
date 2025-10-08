using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IPrivateMessageWebService
    {
        [System.Obsolete("Please define a page number!")]
        List<MessageThreadView> GetAllMessageThreadsForUser(int cmid);
        List<MessageThreadView> GetAllMessageThreadsForUser(int cmid, int pageNumber);
        List<PrivateMessageView> GetThreadMessages(int threadViewerCmid, int otherCmid, int pageNumber);
        PrivateMessageView SendMessage(int senderCmid, int receiverCmid, string content);
        PrivateMessageView GetMessageWithId(int messageId, int requesterCmid);
        void MarkThreadAsRead(int threadViewerCmid, int otherCmid);
        void DeleteThread(int threadViewerCmid, int otherCmid);
    }
}

