using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class ContactRequestView
    {
        #region Properties

        public int RequestId { get; set; }

        public int InitiatorCmid { get; set; }

        public string InitiatorName { get; set; }

        public int ReceiverCmid { get; set; }

        public string InitiatorMessage { get; set; }

        public ContactRequestStatus Status { get; set; }

        public DateTime SentDate { get; set; }

        #endregion Properties

        #region Constructors

        public ContactRequestView()
        { }

        public ContactRequestView(int initiatorCmid, int receiverCmid, string initiatorMessage)
        {
            this.InitiatorCmid = initiatorCmid;
            this.ReceiverCmid = receiverCmid;
            this.InitiatorMessage = initiatorMessage;
        }

        public ContactRequestView(int requestID, int initiatorCmid, string initiatorName, int receiverCmid, string initiatorMessage, ContactRequestStatus status, DateTime sentDate)
        {
            SetContactRequest(requestID, initiatorCmid, initiatorName, receiverCmid, initiatorMessage, status, sentDate);
        }

        #endregion Constructors

        #region Methods

        public void SetContactRequest(int requestID, int initiatorCmid, string initiatorName, int receiverCmid, string initiatorMessage, ContactRequestStatus status, DateTime sentDate)
        {
            this.RequestId = requestID;
            this.InitiatorCmid = initiatorCmid;
            this.InitiatorName = initiatorName;
            this.ReceiverCmid = receiverCmid;
            this.InitiatorMessage = initiatorMessage;
            this.Status = status;
            this.SentDate = sentDate;
        }

        public override string ToString()
        {
            string contactRequest = "[Request contact: [Request ID: " + this.RequestId + "][Initiator Cmid :" + this.InitiatorCmid + "][Initiator Name:" + this.InitiatorName + "][Receiver Cmid: " + this.ReceiverCmid + "]";
            contactRequest += "[Initiator Message: " + this.InitiatorMessage + "][Status: " + this.Status + "][Sent Date: " + this.SentDate + "]]";

            return contactRequest;
        }

        #endregion Methods
    }
}