using System;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class MemberWalletView
    {
        #region Properties

        public int Cmid { get; set; }

        public int Credits { get; set; }

        public int Points { get; set; }

        public DateTime CreditsExpiration { get; set; }

        public DateTime PointsExpiration { get; set; }

        #endregion Properties

        #region Constructors

        public MemberWalletView()
        {
            this.CreditsExpiration = DateTime.Today;
            this.PointsExpiration = DateTime.Today;
        }

        public MemberWalletView(int cmid, int? credits, int? points, DateTime? creditsExpiration, DateTime? pointsExpiration)
        {
            if (credits == null)
            {
                credits = 0;
            }
            if (points == null)
            {
                points = 0;
            }
            if (creditsExpiration == null)
            {
                creditsExpiration = DateTime.MinValue;
            }
            if (pointsExpiration == null)
            {
                pointsExpiration = DateTime.MinValue;
            }

            SetMemberWallet(cmid, (int)credits, (int)points, (DateTime)creditsExpiration, (DateTime)pointsExpiration);
        }

        public MemberWalletView(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
        {
            SetMemberWallet(cmid, credits, points, creditsExpiration, pointsExpiration);
        }

        #endregion

        #region Methods

        private void SetMemberWallet(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
        {
            this.Cmid = cmid;
            this.Credits = credits;
            this.Points = points;
            this.CreditsExpiration = creditsExpiration;
            this.PointsExpiration = pointsExpiration;
        }

        public override string ToString()
        {
            string walletDisplay = "[Wallet: ";

            walletDisplay += "[CMID:" + this.Cmid + "][Credits:" + this.Credits + "][Credits Expiration:" + this.CreditsExpiration + "][Points:" + this.Points + "][Points Expiration:" + this.PointsExpiration + "]";

            walletDisplay += "]";

            return walletDisplay;
        }

        #endregion
    }
}