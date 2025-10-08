using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class PackageDisplay
    {
        public int Amount { get; set; }
        public decimal Percentage { get; set; }

        public PackageDisplay(int amount, decimal percentage)
        {
            this.Amount = amount;
            this.Percentage = percentage;
        }
    }

}