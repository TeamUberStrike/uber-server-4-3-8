using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using System;

namespace UberStrike.Core.ViewModel
{
    [Serializable]
    public class CurrencyDepositsViewModel
    {
        public List<CurrencyDepositView> CurrencyDeposits { get; set; }
        public int TotalCount { get; set; }
    }
}
