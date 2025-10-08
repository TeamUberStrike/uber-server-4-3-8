using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class ClanMemberDisplay
    {
        public GroupPosition Position { get; private set; }
        public string ClanTag { get; private set; }
        public string ClanName { get; private set; }
        public int ClanId { get; private set; }
        public DateTime MemberSince { get; private set; }

        public ClanMemberDisplay(GroupPosition position, string clanTag, string clanName, int clanId, DateTime memberSince)
        {
            this.Position = position;
            this.ClanTag = clanTag;
            this.ClanName = clanName;
            this.ClanId = clanId;
            this.MemberSince = memberSince;
        }
    }
}