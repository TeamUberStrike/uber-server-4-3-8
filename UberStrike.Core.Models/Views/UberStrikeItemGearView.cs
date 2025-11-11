using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Models.Views
{
    [System.Serializable]
    public class UberStrikeItemGearView : BaseUberStrikeItemView
    {
        public override UberstrikeItemType ItemType { get { return UberstrikeItemType.Gear; } }

        public int ArmorPoints { get; set; }

        public int ArmorAbsorptionPercent { get; set; }

        public int ArmorWeight { get; set; }
    }
}