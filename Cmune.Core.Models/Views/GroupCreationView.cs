using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class GroupCreationView
    {
        #region Properties

        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string Motto { get; set; }
        
        public string Address { get; set; }
        
        public bool HasPicture { get; set; }
        
        public int ApplicationId { get; set; }
        
        public int Cmid { get; set; }
        
        public string Tag { get; set; }
        
        public string Locale { get; set; }

        #endregion Properties

        #region Constructors

        public GroupCreationView()
        {
        }

        public GroupCreationView(string name, string description, string motto, string address, bool hasPicture, int applicationId, int cmid, string tag, string locale)
        {
            this.Name = name;
            this.Description = description;
            this.Motto = motto;
            this.Address = address;
            this.HasPicture = hasPicture;
            this.ApplicationId = applicationId;
            this.Cmid = cmid;
            this.Tag = tag;
            this.Locale = locale;
        }

        public GroupCreationView(string name, string motto, int applicationId, int cmid, string tag, string locale)
        {
            this.Name = name;
            this.Description = String.Empty;
            this.Motto = motto;
            this.Address = String.Empty;
            this.HasPicture = false;
            this.ApplicationId = applicationId;
            this.Cmid = cmid;
            this.Tag = tag;
            this.Locale = locale;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string display = "[GroupCreationView: [name:" + this.Name + "][description:" + this.Description + "][Motto:" + this.Motto + "]";
            display += "[Address:" + this.Address + "][Has picture:" + this.HasPicture + "][Application Id:" + this.ApplicationId + "][Cmid:" + this.Cmid + "]";
            display += "[Tag:" + this.Tag + "][Locale:" + this.Locale + "]";

            return display;
        }

        #endregion Methods
    }
}
