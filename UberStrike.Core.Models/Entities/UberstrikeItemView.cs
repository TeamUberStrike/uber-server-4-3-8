using System.Text;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemView : ItemView
    {
        #region Properties

        public int LevelRequired { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemView()
            : base()
        { }

        public UberstrikeItemView(ItemView item, int levelRequired)
            : base(item)
        {
            LevelRequired = levelRequired;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeItemView: ");
            itemDisplay.Append(base.ToString());
            itemDisplay.Append("[LevelRequired: ");
            itemDisplay.Append(this.LevelRequired);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}