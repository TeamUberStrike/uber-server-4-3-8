using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class FacebookReversalDisplay
    {
        #region Properties

        public long TransactionId { get; private set; }
        public long FacebookId { get; private set; }
        /// <summary>
        /// In Facebook credits
        /// </summary>
        public decimal Amount { get; private set; }

        #endregion

        #region Constructors

        public FacebookReversalDisplay(long transactionId, decimal amount, long facebookId = 0)
        {
            TransactionId = transactionId;
            FacebookId = facebookId;
            Amount = amount;
        }

        #endregion
    }

    public class FacebookTransactionDisplay
    {
        #region Properties

        public long Id { get; private set; }
        public long From { get; private set; }
        public long To { get; private set; }
        public int Amount { get; private set; }
        public string Status { get; private set; }
        public string Country { get; private set; }
        public string CreatedTime { get; private set; }
        public string UpdatedTime { get; private set; }

        #endregion

        #region Constructors

        public FacebookTransactionDisplay(long id, long from, long to, int amount, string status, string country, string createdTime, string updatedTime)
        {
            Id = id;
            From = from;
            To = to;
            Amount = amount;
            Status = status;
            Country = country;
            CreatedTime = createdTime;
            UpdatedTime = updatedTime;
        }

        #endregion
    }
}