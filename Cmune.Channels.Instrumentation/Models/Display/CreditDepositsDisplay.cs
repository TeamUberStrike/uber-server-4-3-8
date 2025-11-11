using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class CreditDepositsDisplay
    {
        public List<CurrencyDepositView> CreditDeposits { get; set; }
        public decimal TotalCurrency { get; set; }

        public CreditDepositsDisplay(List<CurrencyDepositView> creditDeposits)
        {
            CreditDeposits = new List<CurrencyDepositView>();
            TotalCurrency = 0;

            if (creditDeposits != null)
            {
                CreditDeposits = creditDeposits;

                foreach (CurrencyDepositView creditDeposit in creditDeposits)
                {
                    TotalCurrency += creditDeposit.UsdAmount;
                }
            }
        }
    }
}