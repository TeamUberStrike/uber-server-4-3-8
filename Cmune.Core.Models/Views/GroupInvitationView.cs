using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class GroupInvitationView
    {
        #region Properties

        
        public string InviterName { get; set; }
        
        public int InviterCmid { get; set; }
        
        public int GroupId { get; set; }
        
        public string GroupName { get; set; }
        
        public string GroupTag { get; set; }
        
        public int GroupInvitationId { get; set; }
        
        public string InviteeName { get; set; }
        
        public int InviteeCmid { get; set; }
        
        public string Message { get; set; }

        #endregion Properties

        #region Constructors

        public GroupInvitationView()
        {
        }

        public GroupInvitationView(int inviterCmid, int groupId, int inviteeCmid, string message)
        {
            this.InviterCmid = inviterCmid;
            this.InviterName = String.Empty;
            this.GroupName = String.Empty;
            this.GroupTag = String.Empty;
            this.GroupId = groupId;
            this.GroupInvitationId = 0;
            this.InviteeCmid = inviteeCmid;
            this.InviteeName = String.Empty;
            this.Message = message;
        }

        public GroupInvitationView(int inviterCmid, string inviterName, string groupName, string groupTag, int groupId, int groupInvitationId, int inviteeCmid, string inviteeName, string message)
        {
            this.InviterCmid = inviterCmid;
            this.InviterName = inviterName;
            this.GroupName = groupName;
            this.GroupTag = groupTag;
            this.GroupId = groupId;
            this.GroupInvitationId = groupInvitationId;
            this.InviteeCmid = inviteeCmid;
            this.InviteeName = inviteeName;
            this.Message = message;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string display = "[GroupInvitationDisplayView: [InviterCmid: " + this.InviterCmid + "][InviterName: " + this.InviterName + "]";
            display += "[GroupName: " + this.GroupName + "][GroupTag: " + this.GroupTag + "][GroupId: " + this.GroupId + "]";
            display += "[GroupInvitationId:" + this.GroupInvitationId + "][InviteeCmid:" + this.InviteeCmid + "][InviteeName:" + this.InviteeName + "]";
            display += "[Message:" + this.Message + "]]";

            return display;
        }

        #endregion Methods
    }
}