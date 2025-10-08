using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class RevenueStorage
    {
        public int Revenue { get; set; }
        public int PayingMembersCount { get; set; }
        public int TransactionsCount { get; set; }
    }
}