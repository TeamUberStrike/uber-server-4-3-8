using System.Collections.Generic;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class ContactGroupView
    {
        #region Properties

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<PublicProfileView> Contacts { get; set; }

        #endregion Properties

        #region Constructors

        public ContactGroupView()
        {
            this.Contacts = new List<PublicProfileView>(0);
            this.GroupName = string.Empty;
        }

        public ContactGroupView(int groupID, string groupName, List<PublicProfileView> contacts)
        {
            this.GroupId = groupID;
            this.GroupName = groupName;
            this.Contacts = contacts;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string contactToGroup = "[Contact group: [Group ID: " + this.GroupId + "][Group Name :" + this.GroupName + "][Contacts: ";

            foreach (PublicProfileView contact in this.Contacts)
            {
                contactToGroup += "[Contact: " + contact.ToString() + "]";
            }
            contactToGroup += "]]";

            return contactToGroup;
        }

        #endregion Methods
    }
}
