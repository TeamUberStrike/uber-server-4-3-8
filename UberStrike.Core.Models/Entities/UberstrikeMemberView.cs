
namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class UberstrikeMemberView
    {
        #region Properties

        public PlayerCardView PlayerCardView { get; set; }

        public PlayerStatisticsView PlayerStatisticsView { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeMemberView()
        {
        }

        public UberstrikeMemberView(PlayerCardView playerCardView, PlayerStatisticsView playerStatisticsView)
        {
            this.PlayerCardView = playerCardView;
            this.PlayerStatisticsView = playerStatisticsView;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string memberViewDisplay = "[Uberstrike member view: ";

            if (this.PlayerCardView != null)
            {
                memberViewDisplay += this.PlayerCardView.ToString();
            }
            else
            {
                memberViewDisplay += "null";
            }

            if (this.PlayerStatisticsView != null)
            {
                memberViewDisplay += this.PlayerStatisticsView.ToString();
            }
            else
            {
                memberViewDisplay += "null";
            }

            memberViewDisplay += "]";

            return memberViewDisplay;
        }

        #endregion Methods
    }
}