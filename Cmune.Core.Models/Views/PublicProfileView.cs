using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class PublicProfileView
    {
        #region Properties

        public int Cmid { get; set; }

        public string Name { get; set; }

        public bool IsChatDisabled { get; set; }

        public MemberAccessLevel AccessLevel { get; set; }

        public string GroupTag { get; set; }

        public DateTime LastLoginDate { get; set; }

        public EmailAddressStatus EmailAddressStatus { get; set; }

        #endregion Properties

        #region Constructors

        public PublicProfileView()
        {
            this.Cmid = 0;
            this.Name = String.Empty;
            this.IsChatDisabled = false;
            this.AccessLevel = MemberAccessLevel.Default;
            this.GroupTag = String.Empty;
            this.LastLoginDate = DateTime.Now;
            this.EmailAddressStatus = EmailAddressStatus.Unverified;
        }

        public PublicProfileView(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus)
        {
            SetPublicProfile(cmid, name, accesLevel, isChatDisabled, String.Empty, lastLoginDate, emailAddressStatus);
        }

        public PublicProfileView(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, string groupTag, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus)
        {
            SetPublicProfile(cmid, name, accesLevel, isChatDisabled, groupTag, lastLoginDate, emailAddressStatus);
        }

        #endregion Constructors

        #region Methods

        private void SetPublicProfile(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, string groupTag, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus)
        {
            this.Cmid = cmid;
            this.Name = name;
            this.AccessLevel = accesLevel;
            this.IsChatDisabled = isChatDisabled;
            this.GroupTag = groupTag;
            this.LastLoginDate = lastLoginDate;
            this.EmailAddressStatus = emailAddressStatus;
        }

        public override string ToString()
        {
            String publicProfileDisplay = "[Public profile: ";

            publicProfileDisplay += "[Member name:" + this.Name + "][CMID:" + this.Cmid + "][Is banned from chat: " + this.IsChatDisabled + "]";
            publicProfileDisplay += "[Access level:" + this.AccessLevel + "][Group tag: " + this.GroupTag + "][Last login date: " + this.LastLoginDate + "]]";
            publicProfileDisplay += "[EmailAddressStatus:" + this.EmailAddressStatus + "]]";

            return publicProfileDisplay;
        }

        #endregion Methods
    }
}