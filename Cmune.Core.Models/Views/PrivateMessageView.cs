using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class PrivateMessageView
    {
        #region Properties

        public int PrivateMessageId { get; set; }

        public int FromCmid { get; set; }

        public string FromName { get; set; }

        public int ToCmid { get; set; }

        //public string ToName { get; set; }

        public DateTime DateSent { get; set; }

        public string ContentText { get; set; }

        public bool IsRead { get; set; }

        public bool HasAttachment { get; set; }

        public bool IsDeletedBySender { get; set; }

        public bool IsDeletedByReceiver { get; set; }

        #endregion

        //#region Constructors

        //public PrivateMessageView()   { }

        //public PrivateMessageView(int privateMessageID, int fromCmid, string fromName, int toCmid, string toName, DateTime dateSent, string subjectText, string contentText, bool isRead, bool hasAttachment, bool isDeletedBySender, bool isDeletedByReceiver)
        //{
        //    SetPrivateMessage(privateMessageID, fromCmid, fromName, toCmid, toName, dateSent, subjectText, contentText, isRead, hasAttachment, isDeletedBySender, isDeletedByReceiver);
        //}

        //public PrivateMessageView(int fromCmid, int toCmid, string subjectText, string contentText)
        //{
        //    this.FromCmid = fromCmid;
        //    this.ToCmid = toCmid;
        //    this.SubjectText = subjectText;
        //    this.ContentText = contentText;
        //}

        //#endregion

        //#region Methods

        //private void SetPrivateMessage(int privateMessageID, int fromCmid, string fromName, int toCmid, string toName, DateTime dateSent, string subjectText, string contentText, bool isRead, bool hasAttachment, bool isDeletedBySender, bool isDeletedByReceiver)
        //{
        //    this.PrivateMessageId = privateMessageID;
        //    this.FromCmid = fromCmid;
        //    this.FromName = fromName;
        //    this.ToCmid = toCmid;
        //    this.ToName = toName;
        //    this.DateSent = dateSent;
        //    this.SubjectText = subjectText;
        //    this.ContentText = contentText;
        //    this.IsRead = isRead;
        //    this.HasAttachment = hasAttachment;
        //    this.IsDeletedBySender = isDeletedBySender;
        //    this.IsDeletedByReceiver = isDeletedByReceiver;
        //}

        //#endregion

        public override string ToString()
        {
            string privateMessageDisplay = "[Private Message: ";

            privateMessageDisplay += "[ID:" + this.PrivateMessageId + "][From:" + this.FromCmid + "][To:" + this.ToCmid + "][Date:" + this.DateSent + "][";
            privateMessageDisplay += "[Content:" + this.ContentText + "][Is Read:" + this.IsRead + "][Has attachment:" + this.HasAttachment + "][Is deleted by sender:" + this.IsDeletedBySender + "][Is deleted by receiver:" + this.IsDeletedByReceiver + "]";

            privateMessageDisplay += "]";

            return privateMessageDisplay;
        }
    }
}