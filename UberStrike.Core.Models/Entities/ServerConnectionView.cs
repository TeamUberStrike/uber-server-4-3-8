using System;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [Serializable]
    public class ServerConnectionView
    {
        public int ApiVersion { get; set; }
        public int Cmid { get; set; }
        public ChannelType Channel { get; set; }
    }
}