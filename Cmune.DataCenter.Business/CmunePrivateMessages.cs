using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using System.Text;
using Cmune.DataCenter.Common.Utils;
using System.Configuration;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages the communication between Cmune members
    /// </summary>
    public static class CmunePrivateMessages
    {
        static int page_size = ConfigurationManager.AppSettings.AllKeys.Contains("MessageThreadsPageSize") ? Int32.Parse(ConfigurationManager.AppSettings["MessageThreadsPageSize"]) : 5;

        /// <summary>
        /// Get list of all threads for user with cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static List<MessageThreadView> GetAllMessageThreadsForUser(int cmid, int page)
        {
            //get list of all threads for user with cmid
            Dictionary<int, List<PrivateMessage>> threads = GetPagedThreadsForUser(cmid, page);
            Dictionary<int, MessageThreadView> threadViews = new Dictionary<int, MessageThreadView>(threads.Count);

            Dictionary<int, string> names = CmuneMember.GetMembersNames(threads.Keys);

            foreach (var t in threads)
            {
                if (!threadViews.ContainsKey(t.Key))
                {
                    var lastMessage = t.Value.OrderByDescending(y => y.DateSent).First();
                    threadViews.Add(t.Key, new MessageThreadView()
                    {
                        ThreadId = t.Key,
                        ThreadName = names[t.Key],
                        MessageCount = t.Value.Count(),
                        LastMessagePreview = TextUtilities.ShortenText(lastMessage.ContentText, 25, true),
                        HasNewMessages = t.Value.Exists(m => m.ToCmid == cmid && !m.IsRead),
                        LastUpdate = lastMessage.DateSent,
                    });
                }
            }


            return threadViews.Values.ToList();
        }

        /// <summary>
        /// Get a paged set of threads for a user.
        /// Very similar to GetAllMessagesBetween...
        /// Note: This is zero-indexed.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal static Dictionary<int, List<PrivateMessage>> GetPagedThreadsForUser(int cmid, int page)
        {
            int skip = page * page_size;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.PrivateMessages.Where<PrivateMessage>(pm => (pm.FromCmid == cmid && pm.IsDeletedBySender == false) || (pm.ToCmid == cmid && pm.IsDeletedByReceiver == false)).GroupBy(m => (m.FromCmid == cmid ? m.ToCmid : m.FromCmid)).OrderByDescending(g => g.Max(m => m.DateSent)).Skip(skip).Take(page_size).ToDictionary(k => k.Key, v => v.ToList());
            }
        }
        /// <summary>
        /// Retrieves a subset of the private messages exchanged by two members
        /// </summary>
        /// <param name="cmid1"></param>
        /// <param name="cmid2"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static List<PrivateMessage> GetAllMessagesBewteen(int cmid1, int cmid2, int range)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<PrivateMessage> threadMessages = new List<PrivateMessage>();
                int subsetSize = CommonConfig.PrivateMessagesThreadPageSize;
                int nbMessagesToSkip = range * subsetSize;

                threadMessages = cmuneDb.PrivateMessages.Where<PrivateMessage>(pm => (pm.FromCmid == cmid1 && pm.ToCmid == cmid2 && pm.IsDeletedBySender == false) || (pm.FromCmid == cmid2 && pm.ToCmid == cmid1 && pm.IsDeletedByReceiver == false)).OrderByDescending(pm => pm.DateSent).Skip(nbMessagesToSkip).Take(subsetSize).ToList<PrivateMessage>();

                return threadMessages;
            }
        }

        /// <summary>
        /// Retrieves a subset of the private messages exchanged by two members
        /// </summary>
        /// <param name="threadViewerCmid"></param>
        /// <param name="otherCmid"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<PrivateMessageView> GetThreadMessages(int threadViewerCmid, int otherCmid, int range)
        {
            List<PrivateMessage> threadMessages = GetAllMessagesBewteen(threadViewerCmid, otherCmid, range);
            return CreatePrivateMessageViews(threadMessages);
        }

        /// <summary>
        /// Get a private message
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public static PrivateMessageView GetMessageWithId(int messageId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                PrivateMessage msg = cmuneDb.PrivateMessages.SingleOrDefault(pm => pm.PrivateMessageID == messageId);

                PrivateMessageView messageView = null;

                if (msg != null)
                {
                    messageView = CreatePrivateMessageView(msg, CmuneMember.GetUserName(msg.FromCmid));
                }

                return messageView;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadMessages"></param>
        /// <returns></returns>
        internal static List<PrivateMessageView> CreatePrivateMessageViews(List<PrivateMessage> threadMessages)
        {
            List<PrivateMessageView> threadView = new List<PrivateMessageView>();

            foreach (PrivateMessage privateMessage in threadMessages)
            {
                threadView.Add(CreatePrivateMessageView(privateMessage));
            }

            return threadView;
        }

        /// <summary>
        /// Creates a Private Message view
        /// </summary>
        /// <param name="privateMessage"></param>
        /// <param name="senderName"></param>
        /// <returns></returns>
        private static PrivateMessageView CreatePrivateMessageView(PrivateMessage privateMessage, string senderName = "")
        {
            return new PrivateMessageView()
                {
                    PrivateMessageId = privateMessage.PrivateMessageID,
                    FromCmid = privateMessage.FromCmid,
                    FromName = senderName,
                    ToCmid = privateMessage.ToCmid,
                    DateSent = privateMessage.DateSent,
                    ContentText = privateMessage.ContentText,
                    IsRead = privateMessage.IsRead,
                    HasAttachment = privateMessage.HasAttachment,
                    IsDeletedBySender = privateMessage.IsDeletedBySender,
                    IsDeletedByReceiver = privateMessage.IsDeletedByReceiver
                };
        }

        /// <summary>
        /// Gets all latest messages to a member with Cmid that arrived before the sentDate 'startDate'
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        internal static List<PrivateMessage> GetAllPrivateMessagesTo(int cmid, DateTime startDate)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.PrivateMessages.Where<PrivateMessage>(pm => (pm.ToCmid == cmid && pm.DateSent > startDate.ToDateOnly() && pm.IsDeletedBySender == false && pm.IsDeletedByReceiver == false)).OrderByDescending(pm => pm.DateSent).ToList<PrivateMessage>();
            }
        }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="fromCmid"></param>
        /// <param name="toCmid"></param>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static PrivateMessageView SendMessage(int fromCmid, int toCmid, string contentText)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                if (!contentText.IsNullOrFullyEmpty())
                {
                    PrivateMessage privateMessage = new PrivateMessage();
                    privateMessage.FromCmid = fromCmid;
                    privateMessage.ToCmid = toCmid;
                    privateMessage.DateSent = DateTime.Now;
                    privateMessage.SubjectText = string.Empty;
                    privateMessage.ContentText = contentText;

                    cmuneDB.PrivateMessages.InsertOnSubmit(privateMessage);
                    cmuneDB.SubmitChanges();

                    return CreatePrivateMessageView(privateMessage);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Sends a PM from the admin account to all users that loged in within the last 30 days
        /// </summary>
        /// <param name="toCmid"></param>
        /// <param name="subjectText"></param>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static int SendAdminMessageToAll(string subjectText, string contentText)
        {
            int counter = 0;
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                if (!contentText.IsNullOrFullyEmpty())
                {
                    DateTime lastLogin = DateTime.Now.Subtract(TimeSpan.FromDays(30));
                    foreach (var user in cmuneDB.Members.Where(m => m.LastAliveAck.HasValue && m.LastAliveAck.Value > lastLogin).Select(m => m.CMID))
                    {
                        cmuneDB.PrivateMessages.InsertOnSubmit(new PrivateMessage()
                        {
                            FromCmid = CommonConfig.AdminCmid,
                            ToCmid = user,
                            DateSent = DateTime.Now,
                            SubjectText = subjectText,
                            ContentText = contentText
                        });
                        counter++;
                    }

                    cmuneDB.SubmitChanges();
                }
            }
            return counter;
        }

        /// <summary>
        /// Sends a PM from the admin account (to send a warning to a player that doesn't have a valid email address for example)
        /// </summary>
        /// <param name="toCmid"></param>
        /// <param name="subjectText"></param>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static bool SendAdminMessage(int toCmid, string subjectText, string contentText)
        {
            StringBuilder messageFooter = new StringBuilder();
            messageFooter.AppendLine();
            messageFooter.AppendLine();
            messageFooter.Append("Do not reply to this PM, direct all questions to ");
            messageFooter.Append(CommonConfig.CmuneSupportEmail);
            messageFooter.Append(".");

            SendMessage(CommonConfig.AdminCmid, toCmid, contentText + messageFooter.ToString());
            return true;
        }

        /// <summary>
        /// Marks a private message as read
        /// </summary>
        /// <param name="privateMessageID"></param>
        public static bool MarkAsRead(int privateMessageID)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isMarkedRead = false;

                PrivateMessage readMessage = cmuneDB.PrivateMessages.SingleOrDefault<PrivateMessage>(pm => pm.PrivateMessageID == privateMessageID);

                if (readMessage != null)
                {
                    readMessage.IsRead = true;
                    cmuneDB.SubmitChanges();
                    isMarkedRead = true;
                }

                return isMarkedRead;
            }
        }

        /// <summary>
        /// Set all unread messages bewteen two users to IsRead = true
        /// </summary>
        /// <param name="otherCmid"></param>
        /// <param name="threadViewerCmid"></param>
        public static void MarkThreadAsRead(int threadViewerCmid, int otherCmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                var allUnreadMessages = cmuneDB.PrivateMessages.Where(pm => pm.ToCmid == threadViewerCmid && pm.FromCmid == otherCmid && !pm.IsRead).ToList();
                foreach (var pm in allUnreadMessages)
                    pm.IsRead = true;
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Set all unread messages bewteen two users to IsRead = true
        /// </summary>
        /// <param name="otherCmid"></param>
        /// <param name="threadViewerCmid"></param>
        public static void DeleteThread(int threadViewerCmid, int otherCmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<PrivateMessage> threadMessages = new List<PrivateMessage>();

                threadMessages = cmuneDb.PrivateMessages.Where<PrivateMessage>(pm => (pm.FromCmid == threadViewerCmid && pm.ToCmid == otherCmid && pm.IsDeletedBySender == false) || (pm.FromCmid == otherCmid && pm.ToCmid == threadViewerCmid && pm.IsDeletedByReceiver == false)).ToList<PrivateMessage>();

                foreach (var pm in threadMessages)
                {
                    if (pm.FromCmid == threadViewerCmid)
                        pm.IsDeletedBySender = true;
                    else if (pm.ToCmid == threadViewerCmid)
                        pm.IsDeletedByReceiver = true;
                }
                cmuneDb.SubmitChanges();
            }
        }
    }
}