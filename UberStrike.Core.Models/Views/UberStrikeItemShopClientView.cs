namespace UberStrike.Core.Models.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [System.Serializable]
    public class UberStrikeItemShopClientView
    {
        // We can't deserialize the base class
        public List<UberStrikeItemFunctionalView> FunctionalItems { get; set; }
        public List<UberStrikeItemGearView> GearItems { get; set; }
        public List<UberStrikeItemQuickView> QuickItems { get; set; }
        public List<UberStrikeItemWeaponView> WeaponItems { get; set; }
        public Dictionary<int, int> ItemsRecommendationPerMap { get; set; }
    }
}
