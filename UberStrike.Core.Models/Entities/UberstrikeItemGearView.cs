using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemGearView : UberstrikeItemView
    {
        #region Properties

        public UberstrikeGearConfigView Config { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemGearView()
            : base()
        {
        }

        public UberstrikeItemGearView(ItemView item, int levelRequired, UberstrikeGearConfigView config)
            : base(item, levelRequired)
        {
            this.Config = config;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeGearView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append(this.Config);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}