using System.Collections.Generic;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class MemberView
    {
        #region Properties

        public PublicProfileView PublicProfile { get; set; }

        public MemberWalletView MemberWallet { get; set; }

        public List<int> MemberItems { get; set; }

        #endregion Properties

        #region Constructors

        public MemberView()
        {
            this.PublicProfile = new PublicProfileView();
            this.MemberWallet = new MemberWalletView();
            this.MemberItems = new List<int>(0);
        }

        public MemberView(PublicProfileView publicProfile, MemberWalletView memberWallet, List<int> memberItems)
        {
            this.PublicProfile = publicProfile;
            this.MemberWallet = memberWallet;
            this.MemberItems = memberItems;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder memberViewDisplay = new StringBuilder("[Member view: ");

            if (this.PublicProfile != null && this.MemberWallet != null)
            {
                memberViewDisplay.Append(this.PublicProfile);
                memberViewDisplay.Append(this.MemberWallet);
                memberViewDisplay.Append("[items: ");
                if (this.MemberItems != null && this.MemberItems.Count > 0)
                {
                    int i = this.MemberItems.Count;
                    foreach (int itemID in this.MemberItems)
                    {
                        memberViewDisplay.Append(itemID);
                        if (--i > 0)
                            memberViewDisplay.Append(", ");
                    }
                }
                else
                {
                    memberViewDisplay.Append("No items");
                }
                memberViewDisplay.Append("]");
            }
            else
            {
                memberViewDisplay.Append("No member");
            }

            memberViewDisplay.Append("]");

            return memberViewDisplay.ToString();
        }

        #endregion
    }
}