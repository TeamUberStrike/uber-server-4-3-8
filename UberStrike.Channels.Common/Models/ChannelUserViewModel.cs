using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Channels.Common.Models
{
    public class ChannelUserViewModel
    {
        public ChannelType Channel { get; set; }

        public int Cmid { get; set; }

        public string FacebookThirdPartId { get; set; }

        public PaymentProviderType PaymentProviderType { get; set; }

        public string PaymentType { get; set; }

    }
}