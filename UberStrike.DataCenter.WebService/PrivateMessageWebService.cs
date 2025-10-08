using System.Collections.Generic;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class PrivateMessageWebService : IPrivateMessageWebService
    {
        public List<MessageThreadView> GetAllMessageThreadsForUser(int cmid)
        {
            return CmunePrivateMessages.GetAllMessageThreadsForUser(cmid, 0);
        }

        public List<MessageThreadView> GetAllMessageThreadsForUser(int cmid, int page)
        {
            return CmunePrivateMessages.GetAllMessageThreadsForUser(cmid, page);
        }

        public List<PrivateMessageView> GetThreadMessages(int threadViewerCmid, int otherCmid, int pageNumber)
        {
            return CmunePrivateMessages.GetThreadMessages(threadViewerCmid, otherCmid, pageNumber);
        }

        public PrivateMessageView GetMessageWithId(int messageId, int requesterCmid)
        {
            return CmunePrivateMessages.GetMessageWithId(messageId);
        }

        public void MarkThreadAsRead(int threadViewerCmid, int otherCmid)
        {
            CmunePrivateMessages.MarkThreadAsRead(threadViewerCmid, otherCmid);
        }

        public void DeleteThread(int threadViewerCmid, int otherCmid)
        {
            CmunePrivateMessages.DeleteThread(threadViewerCmid, otherCmid);
        }

        public PrivateMessageView SendMessage(int senderCmid, int receiverCmid, string content)
        {
            return CmunePrivateMessages.SendMessage(senderCmid, receiverCmid, content);
        }
    }
}