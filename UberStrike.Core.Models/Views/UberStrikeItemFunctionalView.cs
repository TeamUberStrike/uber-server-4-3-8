using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Models.Views
{
    [System.Serializable]
    public class UberStrikeItemFunctionalView : BaseUberStrikeItemView
    {
        public override UberstrikeItemType ItemType { get { return UberstrikeItemType.Functional; } }
    }
}