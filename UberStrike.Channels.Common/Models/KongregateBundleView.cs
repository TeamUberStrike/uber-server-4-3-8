using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Models
{
    [System.Serializable]
    public class KongregateBundleView : BundleView
    {
        public string kongregateMetadata { get; set; }
    }
}