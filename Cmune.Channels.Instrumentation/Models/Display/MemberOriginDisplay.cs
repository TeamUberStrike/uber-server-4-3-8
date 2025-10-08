using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class MemberOriginDisplay
    {
        #region Properties

        public int Cmid { get; private set; }
        public ChannelType Channel { get; private set; }
        public ReferrerPartnerType ReferrerId { get; private set; }
        public int Region { get; private set; }
        public string RegionName { get; private set; }
        public int FriendsCount { get; private set; }
        public int ActiveFriendsCount { get; private set; }

        #endregion

        #region Constructors

        public MemberOriginDisplay(int cmid, ChannelType channel, ReferrerPartnerType referrerId, int region, string regionName, int friendsCount, int activeFriendsCount)
        {
            Cmid = cmid;
            Channel = channel;
            ReferrerId = referrerId;
            Region = region;
            RegionName = regionName;
            FriendsCount = friendsCount;
            ActiveFriendsCount = activeFriendsCount;
        }

        #endregion
    }
}