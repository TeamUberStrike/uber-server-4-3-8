using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
    public class BannedIpView
    {
        #region Properties

        public int BannedIpId { get; private set; }
        public long IpAddress { get; private set; }
        public DateTime? BannedUntil { get; private set; }
        public DateTime BanningDate { get; private set; }
        public int SourceCmid { get; private set; }
        public string SourceName { get; private set; }
        public int TargetCmid { get; private set; }
        public string TargetName { get; private set; }
        public string Reason { get; set; }

        #endregion

        #region Constructors

        public BannedIpView(int bannedIpId, long ipAddress, DateTime? bannedUntil, DateTime banningDate, int sourceCmid, string sourceName, int targetCmid, string targetName, string reason)
        {
            BannedIpId = bannedIpId;
            IpAddress = ipAddress;
            BannedUntil = bannedUntil;
            BanningDate = banningDate;
            SourceCmid = sourceCmid;
            SourceName = sourceName;
            TargetCmid = targetCmid;
            TargetName = targetName;
            Reason = reason;
        }

        #endregion
    }
}