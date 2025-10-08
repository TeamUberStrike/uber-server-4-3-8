using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models
{
    public class ItemDeprecationModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int DailyPrice { get; set; }
        public int PermanentPrice { get; set; }
        public int NumberOfUsers { get; set; }
    }
}