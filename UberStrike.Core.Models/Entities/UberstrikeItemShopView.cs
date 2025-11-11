using System.Collections.Generic;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeItemShopView
    {
        #region Properties

        public List<UberstrikeItemFunctionalView> FunctionalItems { get; set; }

        public List<UberstrikeItemGearView> GearItems { get; set; }

        public List<UberstrikeItemQuickUseView> QuickUseItems { get; set; }

        public List<UberstrikeItemSpecialView> SpecialItems { get; set; }

        public List<UberstrikeItemWeaponModView> WeaponModItems { get; set; }

        public List<UberstrikeItemWeaponView> WeaponItems { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountPointsSevenDays { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountPointsThirtyDays { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountPointsNinetyDays { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountCreditsSevenDays { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountCreditsThirtyDays { get; set; }

        /// <summary>
        /// Discount in percentage (ie 25 means 25% discount)
        /// </summary>
        public int DiscountCreditsNinetyDays { get; set; }

        public Dictionary<int, int> ItemsRecommendationPerMap { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeItemShopView()
        {
            this.FunctionalItems = new List<UberstrikeItemFunctionalView>();
            this.GearItems = new List<UberstrikeItemGearView>();
            this.QuickUseItems = new List<UberstrikeItemQuickUseView>();
            this.SpecialItems = new List<UberstrikeItemSpecialView>();
            this.WeaponItems = new List<UberstrikeItemWeaponView>();
            this.WeaponModItems = new List<UberstrikeItemWeaponModView>();
        }

        public UberstrikeItemShopView(List<UberstrikeItemFunctionalView> functionalItems, List<UberstrikeItemGearView> gearItems,
                                                List<UberstrikeItemQuickUseView> quickUseItems, List<UberstrikeItemSpecialView> specialItems,
                                                List<UberstrikeItemWeaponView> weaponItems, List<UberstrikeItemWeaponModView> weaponModItems,
                                                int discoutPointsSevenDays, int discountPointsThirtyDays, int discountPointsNinetyDays,
                                                int discountCreditsSevenDays, int discountCreditsThirtyDays, int discountCreditsNinetyDays)
        {
            this.FunctionalItems = functionalItems;
            this.GearItems = gearItems;
            this.QuickUseItems = quickUseItems;
            this.SpecialItems = specialItems;
            this.WeaponItems = weaponItems;
            this.WeaponModItems = weaponModItems;
            this.DiscountPointsSevenDays = discoutPointsSevenDays;
            this.DiscountPointsThirtyDays = discountPointsThirtyDays;
            this.DiscountPointsNinetyDays = discountPointsNinetyDays;
            this.DiscountCreditsSevenDays = discountCreditsSevenDays;
            this.DiscountCreditsThirtyDays = discountCreditsThirtyDays;
            this.DiscountCreditsNinetyDays = discountCreditsNinetyDays;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder itemDisplay = new StringBuilder();

            itemDisplay.Append("[UberstrikeItemShopView: ");
            itemDisplay.Append("[FunctionalItems: ");

            if (this.FunctionalItems != null && this.FunctionalItems.Count > 0)
            {
                foreach (UberstrikeItemFunctionalView functionalItemView in this.FunctionalItems)
                {
                    itemDisplay.Append(functionalItemView);
                }
            }

            itemDisplay.Append("][GearItems: ");

            if (this.GearItems != null && this.GearItems.Count > 0)
            {
                foreach (UberstrikeItemGearView gearItemView in this.GearItems)
                {
                    itemDisplay.Append(gearItemView);
                }
            }

            itemDisplay.Append("][QuickUseItems: ");

            if (this.QuickUseItems != null && this.QuickUseItems.Count > 0)
            {
                foreach (UberstrikeItemQuickUseView quickUseItemView in this.QuickUseItems)
                {
                    itemDisplay.Append(quickUseItemView);
                }
            }

            itemDisplay.Append("][SpecialItems: ");

            if (this.SpecialItems != null && this.SpecialItems.Count > 0)
            {
                foreach (UberstrikeItemSpecialView specialItemView in this.SpecialItems)
                {
                    itemDisplay.Append(specialItemView);
                }
            }

            itemDisplay.Append("][WeaponItems: ");

            if (this.WeaponItems != null && this.WeaponItems.Count > 0)
            {
                foreach (UberstrikeItemWeaponView weaponItemView in this.WeaponItems)
                {
                    itemDisplay.Append(weaponItemView);
                }
            }

            itemDisplay.Append("][WeaponModItems: ");

            if (this.WeaponModItems != null && this.WeaponModItems.Count > 0)
            {
                foreach (UberstrikeItemWeaponModView weaponModItemView in this.WeaponModItems)
                {
                    itemDisplay.Append(weaponModItemView);
                }
            }

            itemDisplay.Append("[DiscountPointsSevenDays: ");
            itemDisplay.Append(this.DiscountPointsSevenDays);
            itemDisplay.Append("%][DiscountPointsThirtyDays: ");
            itemDisplay.Append(this.DiscountPointsThirtyDays);
            itemDisplay.Append("%][DiscountPointsNinetyDays: ");
            itemDisplay.Append(this.DiscountPointsNinetyDays);
            itemDisplay.Append("%][DiscountCreditsSevenDays: ");
            itemDisplay.Append(this.DiscountCreditsSevenDays);
            itemDisplay.Append("%][DiscountCreditsThirtyDays: ");
            itemDisplay.Append(this.DiscountCreditsThirtyDays);
            itemDisplay.Append("%][DiscountCreditsNinetyDays: ");
            itemDisplay.Append(this.DiscountCreditsNinetyDays);
            itemDisplay.Append("]]");

            return itemDisplay.ToString();
        }

        #endregion Methods
    }
}