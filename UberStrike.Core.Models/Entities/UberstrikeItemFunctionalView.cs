using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemFunctionalView : UberstrikeItemView
    {
        #region Properties

        public UberstrikeFunctionalConfigView Config { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemFunctionalView()
            : base()
        {
        }

        public UberstrikeItemFunctionalView(ItemView item, int levelRequired, UberstrikeFunctionalConfigView config)
            : base(item, levelRequired)
        {
            this.Config = config;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeFunctionalView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append(this.Config);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}
