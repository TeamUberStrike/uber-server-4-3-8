using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Models
{
    public class ComparisonViewModel
    {
        public UberstrikeItemShopView UberstrikeItemShopView { get; set; }
        public UberstrikeItemShopView UberstrikeItemShopViewToCompare { get; set; }
    }
}