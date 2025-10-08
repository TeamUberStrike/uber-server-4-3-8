using System;
using System.Collections.Generic;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class ClanView : BasicClanView
    {
        #region Properties

        public List<ClanMemberView> Members { get; set; }

        #endregion Properties

        #region Constructors

        public ClanView()
            : base()
        {
            this.Members = new List<ClanMemberView>();
        }

        public ClanView(int groupId, int membersCount, string description, string name, string motto, string address, DateTime foundingDate, string picture, GroupType type, DateTime lastUpdated, string tag, int membersLimit, GroupColor colorStyle, GroupFontStyle fontStyle, int applicationId, int ownerCmid, string ownerName, List<ClanMemberView> members)
            : base (groupId, membersCount, description, name, motto, address, foundingDate, picture, type, lastUpdated, tag, membersLimit, colorStyle, fontStyle, applicationId, ownerCmid, ownerName)
        {
            this.Members = members;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string clanContent = "[Clan: " + base.ToString();
            clanContent += "[Members:";

            foreach (ClanMemberView member in this.Members)
            {
                clanContent += member.ToString();
            }
            
            clanContent += "]";

            return clanContent;
        }

        #endregion Methods
    }
}