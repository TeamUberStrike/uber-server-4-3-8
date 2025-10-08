using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeGearConfigView
    {
        #region Properties

        public int ArmorPoints { get; set; }

        public int ArmorAbsorptionPercent { get; set; }

        public int ArmorWeight { get; set; }

        public int LevelRequired { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            StringBuilder configDisplay = new StringBuilder();

            configDisplay.Append("[UberstrikeGearConfigView: [ArmorPoints: ");
            configDisplay.Append(this.ArmorPoints);
            configDisplay.Append("][ArmorAbsorptionPercent: ");
            configDisplay.Append(this.ArmorAbsorptionPercent);
            configDisplay.Append("][ArmorWeight: ");
            configDisplay.Append(this.ArmorWeight);
            configDisplay.Append("]]");

            return configDisplay.ToString();
        }

        #endregion Methods
    }
}