using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class ItemTransactionsViewModel
    {
        public List<ItemTransactionView> ItemTransactions { get; set; }
        public int TotalCount { get; set; }
    }
}
