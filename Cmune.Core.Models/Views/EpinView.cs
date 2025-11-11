using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
    public class EpinBatchView
    {
        #region Properties

        public int BatchId { get; private set; }
        public int ApplicationId { get; private set; }
        public PaymentProviderType EpinProvider { get; private set; }
        public int Amount { get; private set; }
        public int CreditAmount { get; private set; }
        public DateTime BatchDate { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsRetired { get; private set; }
        public List<EpinView> Epins { get; private set; }

        #endregion

        #region Constructors

        public EpinBatchView(int batchId, int applicationId, PaymentProviderType epinProvider, int amount, int creditAmount, DateTime batchDate, bool isAdmin, bool isRetired, List<EpinView> epins)
        {
            BatchId = batchId;
            ApplicationId = applicationId;
            EpinProvider = epinProvider;
            Amount = amount;
            CreditAmount = creditAmount;
            BatchDate = batchDate;
            IsAdmin = isAdmin;
            Epins = epins;
            IsRetired = isRetired;
        }

        #endregion
    }

    public class EpinView
    {
        #region Properties

        public int EpinId { get; private set; }
        public string Pin { get; private set; }
        public bool IsRedeemed { get; private set; }
        public int BatchId { get; private set; }
        public bool IsRetired { get; private set; }

        #endregion

        #region Constructors

        public EpinView(int epinId, string pin, bool isRedeemed, int batchId, bool isRetired)
        {
            EpinId = epinId;
            Pin = pin;
            IsRedeemed = isRedeemed;
            BatchId = batchId;
            IsRetired = isRetired;
        }

        #endregion
    }
}