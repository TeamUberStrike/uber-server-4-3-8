using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemWeaponView : UberstrikeItemView
    {
        #region Properties

        public UberstrikeWeaponConfigView Config { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemWeaponView()
            : base()
        {
        }

        public UberstrikeItemWeaponView(ItemView item, int levelRequired, UberstrikeWeaponConfigView config)
            : base(item, levelRequired)
        {
            this.Config = config;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeWeaponView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append(this.Config);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}