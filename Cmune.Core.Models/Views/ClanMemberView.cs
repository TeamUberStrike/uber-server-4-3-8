using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class ClanMemberView
    {
        #region Properties

        
        public string Name { get; set; }
        
        public int Cmid { get; set; }
        
        public GroupPosition Position { get; set; }
        
        public DateTime JoiningDate { get; set; }
        
        public DateTime Lastlogin { get; set; }

        #endregion Properties

        #region Constructors

        public ClanMemberView()
        {
        }

        public ClanMemberView(string name, int cmid, GroupPosition position, DateTime joiningDate, DateTime lastLogin)
        {
            this.Cmid = cmid;
            this.Name = name;
            this.Position = position;
            this.JoiningDate = joiningDate;
            this.Lastlogin = lastLogin;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string playerDisplay = "[Clan member: [Name: " + this.Name + "][Cmid: " + this.Cmid + "][Position: " + this.Position + "][JoiningDate: " + this.JoiningDate + "][Lastlogin: " + this.Lastlogin + "]]";

            return playerDisplay;
        }

        #endregion Methods
    }
}