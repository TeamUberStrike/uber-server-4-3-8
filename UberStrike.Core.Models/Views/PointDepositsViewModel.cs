using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class PointDepositsViewModel
    {
        public List<PointDepositView> PointDeposits { get; set; }
        public int TotalCount { get; set; }
    }
}
