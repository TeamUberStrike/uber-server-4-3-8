using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class RevenueDisplay
    {
        public DateTime Date { get; set; }
        public int Revenue { get; set; }
        public decimal Arppu { get; set; }
        public int TransactionsCount { get; set; }

        public RevenueDisplay(DateTime date, int revenue, decimal arppu, int transationsCount)
        {
            this.Date = date;
            this.Revenue = revenue;
            this.Arppu = arppu;
            this.TransactionsCount = transationsCount;
        }
    }
}