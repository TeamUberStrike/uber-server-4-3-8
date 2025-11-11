using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PlayerXPEventView
    {
        #region Properties

        public int PlayerXPEventId { get; set; }
        public string Name { get; set; }
        public decimal XPMultiplier { get; set; }

        #endregion Properties

        #region Constructors

        public PlayerXPEventView()
        {
        }

        public PlayerXPEventView(string name, decimal xpMultiplier)
        {
            this.Name = name;
            this.XPMultiplier = xpMultiplier;
        }

        public PlayerXPEventView(int playerXPEventId, string name, decimal xpMultiplier)
            : this(name, xpMultiplier)
        {
            this.PlayerXPEventId = playerXPEventId;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[PlayerXPEventView: ");
            display.Append("[PlayerXPEventId: ");
            display.Append(this.PlayerXPEventId);
            display.Append("][Name: ");
            display.Append(this.Name);
            display.Append("][XPMultiplier: ");
            display.Append(this.XPMultiplier);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}