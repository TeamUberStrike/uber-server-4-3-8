using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PlayerLevelCapView
    {
        #region Properties

        public int PlayerLevelCapId { get; set; }
        public int Level { get; set; }
        public int XPRequired { get; set; }

        #endregion Properties

        #region Constructors

        public PlayerLevelCapView()
        {
        }

        public PlayerLevelCapView(int level, int xpRequired)
        {
            this.Level = level;
            this.XPRequired = xpRequired;
        }

        public PlayerLevelCapView(int playerLevelCapId, int level, int xpRequired)
            : this(level, xpRequired)
        {
            PlayerLevelCapId = playerLevelCapId;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[PlayerLevelCapView: ");
            display.Append("[PlayerLevelCapId: ");
            display.Append(this.PlayerLevelCapId);
            display.Append("][Level: ");
            display.Append(this.Level);
            display.Append("][XPRequired: ");
            display.Append(this.XPRequired);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}