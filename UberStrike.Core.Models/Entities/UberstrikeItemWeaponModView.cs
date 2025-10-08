using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemWeaponModView : UberstrikeItemView
    {
        #region Properties

        public UberstrikeWeaponModConfigView Config { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemWeaponModView()
            : base()
        {
        }

        public UberstrikeItemWeaponModView(ItemView item, int level, UberstrikeWeaponModConfigView config)
            : base(item, level)
        {
            this.Config = config;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeWeaponModView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append(this.Config);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}