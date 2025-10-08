namespace UberStrike.DataCenter.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cmune.DataCenter.Common.Entities;

    public class LastIpView
    {
        #region Properties

        public long Ip { get; private set; }
        public DateTime LastConnectionDate { get; private set; }
        public List<LinkedMemberView> LinkedMembers { get; private set; }
        public BannedIpView BannedIpView { get; private set; }

        #endregion

        #region Constructors

        public LastIpView(long ip, DateTime lastConnectionDate, List<LinkedMemberView> linkedMembers, BannedIpView bannedIpView)
        {
            this.Ip = ip;
            this.LastConnectionDate = lastConnectionDate;
            this.LinkedMembers = linkedMembers;
            this.BannedIpView = bannedIpView;
        }

        #endregion
    }
}
