using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemQuickUseView : UberstrikeItemView
    {
        #region Properties

        public ItemQuickUseConfigView Config { get; set; }

        public QuickItemLogic Logic { get; set; }

        #endregion Properties

        public UberstrikeItemQuickUseView()
            : base()
        { }

        public UberstrikeItemQuickUseView(ItemView item, int levelRequired)
            : base(item, levelRequired)
        {
        }

        public UberstrikeItemQuickUseView(ItemView item, int levelRequired, ItemQuickUseConfigView Config)
            : base(item, levelRequired)
        {
            this.Config = Config;
        }
    }
}