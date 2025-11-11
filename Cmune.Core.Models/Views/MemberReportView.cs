
namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class MemberReportView
    {
        #region Properties

        public int SourceCmid { get; set; }

        public int TargetCmid { get; set; }

        public MemberReportType ReportType { get; set; }

        public string Reason { get; set; }

        public string Context { get; set; }

        public int ApplicationId { get; set; }

        public string IP { get; set; }

        #endregion Properties

        #region Constructors

        public MemberReportView()
        {
        }

        public MemberReportView(int sourceCmid, int targetCmid, MemberReportType reportType, string reason, string context, int applicationID, string ip)
        {
            this.SourceCmid = sourceCmid;
            this.TargetCmid = targetCmid;
            this.ReportType = reportType;
            this.Reason = reason;
            this.Context = context;
            this.ApplicationId = applicationID;
            this.IP = ip;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string memberReportDisplay = "[Member report: [Source CMID: " + this.SourceCmid + "][Target CMID: " + this.TargetCmid + "][Type: " + this.ReportType + "][Reason: " + this.Reason + "][Context: " + this.Context + "][Application ID: " + this.ApplicationId + "][IP: " + this.IP + "]]";

            return memberReportDisplay;
        }

        #endregion Methods
    }
}