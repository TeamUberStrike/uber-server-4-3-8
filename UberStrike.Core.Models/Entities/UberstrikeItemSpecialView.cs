using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemSpecialView : UberstrikeItemView
    {
        #region Properties

        public UberstrikeSpecialConfigView Config { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemSpecialView()
            : base()
        {
        }

        public UberstrikeItemSpecialView(ItemView item, int levelRequired, UberstrikeSpecialConfigView config)
            : base(item, levelRequired)
        {
            this.Config = config;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeSpecialView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append(this.Config);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}