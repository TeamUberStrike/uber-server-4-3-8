using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Core.Models.Views;

namespace UberStrike.DataCenter.Business
{
    /// <summary>
    /// Manages the Shop
    /// </summary>
    public static class UberstrikeShop
    {
        public static UberstrikeItemShopView ConfigureItems(UberstrikeDataContext uberStrikeDb, List<ItemView> items)
        {
            UberstrikeItemShopView shopView = null;

            Dictionary<int, ItemFunctionalConfig> functionalItemsConfigOrdered = GetItemFunctionalConfig(uberStrikeDb);
            Dictionary<int, ItemGearConfig> gearItemsConfigOrdered = GetItemGearConfig(uberStrikeDb);
            Dictionary<int, ItemQuickUseConfig> quickUseItemsConfigOrdered = GetItemQuickUseConfig(uberStrikeDb);
            Dictionary<int, ItemSpecialConfig> specialItemsConfigOrdered = GetItemSpecialConfig(uberStrikeDb);
            Dictionary<int, ItemWeaponConfig> weaponItemsConfigOrdered = GetItemWeaponConfig(uberStrikeDb);
            Dictionary<int, ItemWeaponModConfig> weaponModItemsConfigOrdered = GetItemWeaponModConfig(uberStrikeDb);

            List<UberstrikeItemFunctionalView> functionalItems = new List<UberstrikeItemFunctionalView>();
            List<UberstrikeItemGearView> gearItems = new List<UberstrikeItemGearView>();
            List<UberstrikeItemQuickUseView> quickUseItems = new List<UberstrikeItemQuickUseView>();
            List<UberstrikeItemSpecialView> specialItems = new List<UberstrikeItemSpecialView>();
            List<UberstrikeItemWeaponModView> weaponModItems = new List<UberstrikeItemWeaponModView>();
            List<UberstrikeItemWeaponView> weaponItems = new List<UberstrikeItemWeaponView>();

            foreach (ItemView item in items)
            {
                ShopItemType itemType = (ShopItemType)item.TypeId;

                switch (itemType)
                {
                    case ShopItemType.UberstrikeFunctional:

                        if (functionalItemsConfigOrdered.ContainsKey(item.ItemId))
                        {
                            functionalItems.Add(new UberstrikeItemFunctionalView(item, functionalItemsConfigOrdered[item.ItemId].LevelRequired, functionalItemsConfigOrdered[item.ItemId].ToItemFunctionalConfigViewOrInitialObject()));
                        }

                        break;
                    case ShopItemType.UberstrikeGear:

                        if (gearItemsConfigOrdered.ContainsKey(item.ItemId))
                        {
                            gearItems.Add(new UberstrikeItemGearView(item, gearItemsConfigOrdered[item.ItemId].LevelRequired, gearItemsConfigOrdered[item.ItemId].ToItemGearConfigViewOrInitialObject()));
                        }

                        break;
                    case ShopItemType.UberstrikeQuickUse:

                        ItemQuickUseConfig config;
                        if (quickUseItemsConfigOrdered.TryGetValue(item.ItemId, out config))
                        {
                            quickUseItems.Add(new UberstrikeItemQuickUseView(item, config.LevelRequired, config.ToItemQuickUseConfigViewOrInitialObject()));
                        }

                        break;
                    case ShopItemType.UberstrikeSpecial:

                        if (specialItemsConfigOrdered.ContainsKey(item.ItemId))
                        {
                            specialItems.Add(new UberstrikeItemSpecialView(item, specialItemsConfigOrdered[item.ItemId].LevelRequired, specialItemsConfigOrdered[item.ItemId].ToItemSpecialConfigViewOrInitialObject()));
                        }

                        break;
                    case ShopItemType.UberstrikeWeapon:

                        if (weaponItemsConfigOrdered.ContainsKey(item.ItemId))
                        {
                            weaponItems.Add(new UberstrikeItemWeaponView(item, weaponItemsConfigOrdered[item.ItemId].LevelRequired, weaponItemsConfigOrdered[item.ItemId].ToItemWeaponConfigViewOrInitialObject()));
                        }

                        break;
                    case ShopItemType.UberstrikeWeaponMod:

                        if (weaponModItemsConfigOrdered.ContainsKey(item.ItemId))
                        {
                            weaponModItems.Add(new UberstrikeItemWeaponModView(item, weaponModItemsConfigOrdered[item.ItemId].LevelRequired, weaponModItemsConfigOrdered[item.ItemId].ToItemWeaponModConfigViewOrInitialObject()));
                        }

                        break;
                    default:
                        CmuneLog.LogUnexpectedReturn(itemType, String.Empty);
                        break;
                }
            }

            int discountPointsSevenDays = CommonConfig.DiscountPointsSevenDays;
            int discountPointsThirtyDays = CommonConfig.DiscountPointsThirtyDays;
            int discountPointsNinetyDays = CommonConfig.DiscountPointsNinetyDays;

            int discountCreditsSevenDays = CommonConfig.DiscountCreditsSevenDays;
            int discountCreditsThirtyDays = CommonConfig.DiscountCreditsThirtyDays;
            int discountCreditsNinetyDays = CommonConfig.DiscountCreditsNinetyDays;

            shopView = new UberstrikeItemShopView(functionalItems, gearItems, quickUseItems, specialItems, weaponItems, weaponModItems, discountPointsSevenDays, discountPointsThirtyDays, discountPointsNinetyDays, discountCreditsSevenDays, discountCreditsThirtyDays, discountCreditsNinetyDays);
            return shopView;
        }

        /// <summary>
        /// Get all the uberstrike items 
        /// </summary>
        /// <param name="cmuneDb"></param>
        /// <param name="uberStrikeDb"></param>
        /// <returns></returns>
        public static UberstrikeItemShopView GetAllItems(CmuneDataContext cmuneDb, UberstrikeDataContext uberStrikeDb)
        {
            UberstrikeItemShopView shopView = null;

            List<ItemView> items = CmuneItem.GetItems(UberStrikeCommonConfig.ApplicationId, cmuneDb);

            shopView = ConfigureItems(uberStrikeDb, items);

            return shopView;
        }

        public static UberstrikeItemShopView GetAllItemsByType(CmuneDataContext cmuneDb, UberstrikeDataContext uberStrikeDb, int itemTypeId)
        {
            UberstrikeItemShopView shopView = null;

            List<ItemView> items = CmuneItem.GetItemByType(UberStrikeCommonConfig.ApplicationId, itemTypeId, cmuneDb);

            shopView = ConfigureItems(uberStrikeDb, items);

            return shopView;
        }

        /// <summary>
        /// Get all the items
        /// </summary>
        /// <returns></returns>
        public static UberstrikeItemShopView GetShop()
        {
            UberstrikeItemShopView shopView = null;

            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<ItemView> items = CmuneItem.GetShop(UberStrikeCommonConfig.ApplicationId);

                shopView = ConfigureItems(uberStrikeDb, items);

            }

            return shopView;
        }

        public static UberStrikeItemShopClientView GetClientShop(string applicationVersion)
        {
            UberStrikeItemShopClientView shopView = null;

            if (HttpRuntime.Cache[UberStrikeCacheKeys.ShopView] != null)
            {
                shopView = (UberStrikeItemShopClientView)HttpRuntime.Cache[UberStrikeCacheKeys.ShopView];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    List<ItemView> items = CmuneItem.GetShop(UberStrikeCommonConfig.ApplicationId);

                    Dictionary<int, ItemFunctionalConfig> functionalItemsConfigOrdered = GetItemFunctionalConfig(uberStrikeDb);
                    Dictionary<int, ItemGearConfig> gearItemsConfigOrdered = GetItemGearConfig(uberStrikeDb);
                    Dictionary<int, ItemQuickUseConfig> quickUseItemsConfigOrdered = GetItemQuickUseConfig(uberStrikeDb);
                    Dictionary<int, ItemWeaponConfig> weaponItemsConfigOrdered = GetItemWeaponConfig(uberStrikeDb);

                    List<UberStrikeItemFunctionalView> functionalItems = new List<UberStrikeItemFunctionalView>();
                    List<UberStrikeItemGearView> gearItems = new List<UberStrikeItemGearView>();
                    List<UberStrikeItemQuickView> quickUseItems = new List<UberStrikeItemQuickView>();
                    List<UberStrikeItemWeaponView> weaponsItems = new List<UberStrikeItemWeaponView>();

                    foreach (var item in items)
                    {
                        // Pending refactoring of the item system
                        switch ((UberstrikeItemType)item.TypeId)
                        {
                            case UberstrikeItemType.Functional:
                                ItemFunctionalConfig functionalConfig = functionalItemsConfigOrdered[item.ItemId];

                                var uberStrikeFunctionalItem = new UberStrikeItemFunctionalView
                                {
                                    Description = item.Description,
                                    ID = item.ItemId,
                                    IsConsumable = item.PurchaseType == PurchaseType.Pack,
                                    ItemClass = (UberstrikeItemClass)item.ClassId,
                                    LevelLock = functionalConfig.LevelRequired,
                                    Name = item.Name,
                                    ShopHighlightType = GetHighlightType(item),
                                    Prices = GetItemPrices(item),
                                    CustomProperties = item.CustomProperties
                                };

                                functionalItems.Add(uberStrikeFunctionalItem);
                                break;
                            case UberstrikeItemType.Gear:
                                ItemGearConfig gearConfig = gearItemsConfigOrdered[item.ItemId];

                                var uberStrikeGearItem = new UberStrikeItemGearView
                                {
                                    ArmorAbsorptionPercent = gearConfig.ArmorAbsorptionPercent,
                                    ArmorPoints = gearConfig.ArmorPoints,
                                    ArmorWeight = gearConfig.ArmorWeight,
                                    Description = item.Description,
                                    ID = item.ItemId,
                                    IsConsumable = item.PurchaseType == PurchaseType.Pack,
                                    ItemClass = (UberstrikeItemClass)item.ClassId,
                                    LevelLock = gearConfig.LevelRequired,
                                    Name = item.Name,
                                    ShopHighlightType = GetHighlightType(item),
                                    Prices = GetItemPrices(item),
                                    CustomProperties = item.CustomProperties
                                };

                                gearItems.Add(uberStrikeGearItem);
                                break;
                            case UberstrikeItemType.QuickUse:
                                ItemQuickUseConfig quickUseConfig = quickUseItemsConfigOrdered[item.ItemId];

                                var uberStrikeQuickItem = new UberStrikeItemQuickView
                                {
                                    BehaviourType = (QuickItemLogic)quickUseConfig.BehaviourType,
                                    CoolDownTime = quickUseConfig.CoolDownTime,
                                    Description = item.Description,
                                    ID = item.ItemId,
                                    IsConsumable = item.PurchaseType == PurchaseType.Pack,
                                    ItemClass = (UberstrikeItemClass)item.ClassId,
                                    LevelLock = quickUseConfig.LevelRequired,
                                    Name = item.Name,
                                    ShopHighlightType = GetHighlightType(item),
                                    Prices = GetItemPrices(item),
                                    UsesPerGame = quickUseConfig.UsesPerGame,
                                    UsesPerLife = quickUseConfig.UsesPerLife,
                                    UsesPerRound = quickUseConfig.UsesPerRound,
                                    WarmUpTime = quickUseConfig.WarmUpTime,
                                    MaxOwnableAmount = item.MaximumOwnableAmount != CommonConfig.ItemMallFieldDisable ? item.MaximumOwnableAmount : CommonConfig.ItemMaximumOwnableAmount,
                                    CustomProperties = item.CustomProperties,
                                };

                                quickUseItems.Add(uberStrikeQuickItem);
                                break;
                            case UberstrikeItemType.Weapon:
                                ItemWeaponConfig weaponConfig = weaponItemsConfigOrdered[item.ItemId];

                                var uberStrikeWeaponItem = new UberStrikeItemWeaponView
                                {
                                    AccuracySpread = weaponConfig.AccuracySpread,
                                    DamageKnockback = weaponConfig.DamageKnockback,
                                    DamagePerProjectile = weaponConfig.DamagePerProjectile,
                                    Description = item.Description,
                                    ID = item.ItemId,
                                    IsConsumable = item.PurchaseType == PurchaseType.Pack,
                                    ItemClass = (UberstrikeItemClass)item.ClassId,
                                    LevelLock = weaponConfig.LevelRequired,
                                    MaxAmmo = weaponConfig.MaxAmmo,
                                    MissileBounciness = weaponConfig.MissileBounciness,
                                    MissileForceImpulse = weaponConfig.MissileForceImpulse,
                                    MissileTimeToDetonate = weaponConfig.MissileTimeToDetonate,
                                    Name = item.Name,
                                    Prices = GetItemPrices(item),
                                    ProjectileSpeed = weaponConfig.ProjectileSpeed,
                                    ProjectilesPerShot = weaponConfig.ProjectilesPerShot,
                                    RateOfFire = weaponConfig.RateOfFire,
                                    RecoilKickback = weaponConfig.RecoilKickback,
                                    RecoilMovement = weaponConfig.RecoilMovement,
                                    ShopHighlightType = GetHighlightType(item),
                                    SplashRadius = weaponConfig.SplashRadius,
                                    StartAmmo = weaponConfig.StartAmmo,
                                    CustomProperties = item.CustomProperties
                                };

                                weaponsItems.Add(uberStrikeWeaponItem);
                                break;
                            default:
                                throw new NotImplementedException(String.Format("Type {0} is not implemented yet", item.TypeId));
                        }
                    }

                    Dictionary<int, int> itemsRecommendationPerMap = new Dictionary<int, int>();

                    foreach (var map in Games.GetMapCluster(applicationVersion).Maps)
                    {
                        itemsRecommendationPerMap.Add(map.MapId, map.ItemId);
                    }

                    shopView = new UberStrikeItemShopClientView()
                    {
                        FunctionalItems = functionalItems,
                        GearItems = gearItems,
                        QuickItems = quickUseItems,
                        WeaponItems = weaponsItems,
                        ItemsRecommendationPerMap = itemsRecommendationPerMap,
                    };

                    HttpRuntime.Cache.Add(UberStrikeCacheKeys.ShopView, shopView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }

            return shopView;
        }

        private static List<ItemPrice> GetItemPrices(ItemView item)
        {
            List<ItemPrice> itemPrices = new List<ItemPrice>();

            if (item.IsForSale)
            {
                if (item.PurchaseType == PurchaseType.Pack)
                {
                    itemPrices = GetConsumableItemPrices(item);
                }
                else if (item.PurchaseType == PurchaseType.Rent)
                {
                    itemPrices = GetRentItemPrices(item);
                }
                else
                {
                    throw new NotImplementedException(String.Format("PurchaseType {0} is not implemented yet", item.PurchaseType));
                }
            }

            return itemPrices;
        }

        private static List<ItemPrice> GetConsumableItemPrices(ItemView item)
        {
            List<ItemPrice> itemPrices = new List<ItemPrice>();

            if (item.PackOneAmount != CommonConfig.ItemMallFieldDisable && item.PermanentPoints != CommonConfig.ItemMallFieldDisable)
            {
                itemPrices.Add(new ItemPrice
                {
                    Amount = item.PackOneAmount,
                    Currency = UberStrikeCurrencyType.Points,
                    PackType = PackType.One,
                    Price = CmuneEconomy.ComputeItemConsumablePrice(item, PackType.One, UberStrikeCurrencyType.Points),
                });
            }

            if (item.PackTwoAmount != CommonConfig.ItemMallFieldDisable && item.PermanentCredits != CommonConfig.ItemMallFieldDisable)
            {
                itemPrices.Add(new ItemPrice
                {
                    Amount = item.PackTwoAmount,
                    Currency = UberStrikeCurrencyType.Credits,
                    PackType = PackType.Two,
                    Price = CmuneEconomy.ComputeItemConsumablePrice(item, PackType.Two, UberStrikeCurrencyType.Credits),
                });
            }

            if (item.PackThreeAmount != CommonConfig.ItemMallFieldDisable && item.PermanentCredits != CommonConfig.ItemMallFieldDisable)
            {
                itemPrices.Add(new ItemPrice
                {
                    Amount = item.PackThreeAmount,
                    Currency = UberStrikeCurrencyType.Credits,
                    Discount = CommonConfig.DiscountPackThree,
                    PackType = PackType.Three,
                    Price = CmuneEconomy.ComputeItemConsumablePrice(item, PackType.Three, UberStrikeCurrencyType.Credits),
                });
            }

            return itemPrices;
        }

        private static List<ItemPrice> GetRentItemPrices(ItemView item)
        {
            List<ItemPrice> itemPrices = new List<ItemPrice>();

            if (item.Enable1Day)
            {
                if (item.PointsPerDay != CommonConfig.ItemMallFieldDisable)
                {
                    itemPrices.Add(new ItemPrice
                    {
                        Currency = UberStrikeCurrencyType.Points,
                        Duration = BuyingDurationType.OneDay,
                        Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Points, BuyingDurationType.OneDay),
                    });
                }

                if (item.CreditsPerDay != CommonConfig.ItemMallFieldDisable)
                {
                    itemPrices.Add(new ItemPrice
                    {
                        Currency = UberStrikeCurrencyType.Credits,
                        Duration = BuyingDurationType.OneDay,
                        Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Credits, BuyingDurationType.OneDay),
                    });
                }
            }

            if (item.Enable7Days)
            {
                if (item.PointsPerDay != CommonConfig.ItemMallFieldDisable)
                {
                    itemPrices.Add(new ItemPrice
                    {
                        Currency = UberStrikeCurrencyType.Points,
                        Discount = CommonConfig.DiscountPointsSevenDays,
                        Duration = BuyingDurationType.SevenDays,
                        Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Points, BuyingDurationType.SevenDays),
                    });
                }

                if (item.CreditsPerDay != CommonConfig.ItemMallFieldDisable)
                {
                    itemPrices.Add(new ItemPrice
                    {
                        Currency = UberStrikeCurrencyType.Credits,
                        Discount = CommonConfig.DiscountCreditsSevenDays,
                        Duration = BuyingDurationType.SevenDays,
                        Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Credits, BuyingDurationType.SevenDays),
                    });
                }
            }

            if (item.PermanentPoints != CommonConfig.ItemMallFieldDisable)
            {
                itemPrices.Add(new ItemPrice
                {
                    Currency = UberStrikeCurrencyType.Points,
                    Duration = BuyingDurationType.Permanent,
                    Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Points, BuyingDurationType.Permanent),
                });
            }

            if (item.PermanentCredits != CommonConfig.ItemMallFieldDisable)
            {
                itemPrices.Add(new ItemPrice
                {
                    Currency = UberStrikeCurrencyType.Credits,
                    Duration = BuyingDurationType.Permanent,
                    Price = CmuneEconomy.ComputeItemRentPrice(item, UberStrikeCurrencyType.Credits, BuyingDurationType.Permanent),
                });
            }

            return itemPrices;
        }

        private static ItemShopHighlightType GetHighlightType(ItemView item)
        {
            ItemShopHighlightType highlightType = ItemShopHighlightType.None;

            if (item.IsFeatured)
            {
                highlightType = ItemShopHighlightType.Featured;
            }
            else if (item.IsNew)
            {
                highlightType = ItemShopHighlightType.New;
            }
            else if (item.IsPopular)
            {
                highlightType = ItemShopHighlightType.Popular;
            }

            return highlightType;
        }

        /// <summary>
        /// Get all the ItemFunctionalConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemFunctionalConfig> GetItemFunctionalConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemFunctionalConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemFunctionalConfig
        /// </summary>
        /// <param name="uberStrikeDb"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemFunctionalConfig> GetItemFunctionalConfig(UberstrikeDataContext uberStrikeDb)
        {
            Dictionary<int, ItemFunctionalConfig> items = new Dictionary<int, ItemFunctionalConfig>();

            if (uberStrikeDb != null)
            {
                items = uberStrikeDb.ItemFunctionalConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get all the ItemGearConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemGearConfig> GetItemGearConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemGearConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemGearConfig
        /// </summary>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemGearConfig> GetItemGearConfig(UberstrikeDataContext paradiseDB)
        {
            Dictionary<int, ItemGearConfig> items = new Dictionary<int, ItemGearConfig>();

            if (paradiseDB != null)
            {
                items = paradiseDB.ItemGearConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get all the ItemQuickUseConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemQuickUseConfig> GetItemQuickUseConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemQuickUseConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemQuickUseConfig
        /// </summary>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemQuickUseConfig> GetItemQuickUseConfig(UberstrikeDataContext paradiseDB)
        {
            Dictionary<int, ItemQuickUseConfig> items = new Dictionary<int, ItemQuickUseConfig>();

            if (paradiseDB != null)
            {
                items = paradiseDB.ItemQuickUseConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get all the ItemSpecialConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemSpecialConfig> GetItemSpecialConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemSpecialConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemSpecialConfig
        /// </summary>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemSpecialConfig> GetItemSpecialConfig(UberstrikeDataContext paradiseDB)
        {
            Dictionary<int, ItemSpecialConfig> items = new Dictionary<int, ItemSpecialConfig>();

            if (paradiseDB != null)
            {
                items = paradiseDB.ItemSpecialConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get all the ItemWeaponConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemWeaponConfig> GetItemWeaponConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemWeaponConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemWeaponConfig
        /// </summary>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemWeaponConfig> GetItemWeaponConfig(UberstrikeDataContext paradiseDB)
        {
            Dictionary<int, ItemWeaponConfig> items = new Dictionary<int, ItemWeaponConfig>();

            if (paradiseDB != null)
            {
                items = paradiseDB.ItemWeaponConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get all the ItemWeaponModConfig
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, ItemWeaponModConfig> GetItemWeaponModConfig()
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                return GetItemWeaponModConfig(paradiseDB);
            }
        }

        /// <summary>
        /// Get all the ItemWeaponModConfig
        /// </summary>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ItemWeaponModConfig> GetItemWeaponModConfig(UberstrikeDataContext paradiseDB)
        {
            Dictionary<int, ItemWeaponModConfig> items = new Dictionary<int, ItemWeaponModConfig>();

            if (paradiseDB != null)
            {
                items = paradiseDB.ItemWeaponModConfigs.ToDictionary(i => i.ItemId);
            }

            return items;
        }

        /// <summary>
        /// Get Functional Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static UberstrikeFunctionalConfigView GetFunctionalConfig(int itemId, bool cache = true)
        {
            UberstrikeFunctionalConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopFunctionalConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (UberstrikeFunctionalConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var functionalConfig = GetFunctionalConfig(itemId, uberStrikeDB);
                    if (functionalConfig != null)
                    {
                        config = functionalConfig.ToItemFunctionalConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Functional Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemFunctionalConfig GetFunctionalConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemFunctionalConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemFunctionalConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }

        /// <summary>
        /// Get Gear Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static UberstrikeGearConfigView GetGearConfig(int itemId, bool cache = true)
        {
            UberstrikeGearConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopGearConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (UberstrikeGearConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var gearConfig = GetGearConfig(itemId, uberStrikeDB);
                    if (gearConfig != null)
                    {
                        config = gearConfig.ToItemGearConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Gear Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemGearConfig GetGearConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemGearConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemGearConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }

        /// <summary>
        /// Get Quick Use Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static ItemQuickUseConfigView GetQuickUseConfig(int itemId, bool cache = true)
        {
            ItemQuickUseConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopQuickUseConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (ItemQuickUseConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var quickUseConfig = GetQuickUseConfig(itemId, uberStrikeDB);
                    if (quickUseConfig != null)
                    {
                        config = quickUseConfig.ToItemQuickUseConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Quick Use Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemQuickUseConfig GetQuickUseConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemQuickUseConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemQuickUseConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }


        /// <summary>
        /// Get Special Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static UberstrikeSpecialConfigView GetSpecialConfig(int itemId, bool cache = true)
        {
            UberstrikeSpecialConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopSpecialConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (UberstrikeSpecialConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var specialConfig = GetSpecialConfig(itemId, uberStrikeDB);
                    if (specialConfig != null)
                    {
                        config = specialConfig.ToItemSpecialConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Special Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemSpecialConfig GetSpecialConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemSpecialConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemSpecialConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }

        /// <summary>
        /// Get Weapon Mod Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static UberstrikeWeaponModConfigView GetWeaponModConfig(int itemId, bool cache = true)
        {
            UberstrikeWeaponModConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopWeaponModConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (UberstrikeWeaponModConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var weaponModConfig = GetWeaponModConfig(itemId, uberStrikeDB);
                    if (weaponModConfig != null)
                    {
                        config = weaponModConfig.ToItemWeaponModConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Weapon Mod Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemWeaponModConfig GetWeaponModConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemWeaponModConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemWeaponModConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }

        /// <summary>
        /// Get Weapon Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static UberstrikeWeaponConfigView GetWeaponConfig(int itemId, bool cache = true)
        {
            UberstrikeWeaponConfigView config = null;

            string cacheName = UberStrikeCacheKeys.ShopWeaponModConfigParameters + CmuneCacheKeys.Separator + itemId.ToString();

            if (HttpRuntime.Cache[cacheName] != null && cache)
            {
                config = (UberstrikeWeaponConfigView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                using (UberstrikeDataContext uberStrikeDB = new UberstrikeDataContext())
                {
                    var weaponConfig = GetWeaponConfig(itemId, uberStrikeDB);
                    if (weaponConfig != null)
                    {
                        config = weaponConfig.ToItemWeaponConfigView();
                        HttpRuntime.Cache.Add(cacheName, config, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Get Weapon Config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="paradiseDB"></param>
        /// <returns></returns>
        public static ItemWeaponConfig GetWeaponConfig(int itemId, UberstrikeDataContext paradiseDB)
        {
            ItemWeaponConfig config = null;

            if (paradiseDB != null)
            {
                config = paradiseDB.ItemWeaponConfigs.SingleOrDefault(c => c.ItemId == itemId);
            }

            return config;
        }

        #region Create Item

        /// <summary>
        /// Creates a new weapon
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateWeaponItem(ItemView itemView, int levelRequired, UberstrikeWeaponConfigView configView)
        {
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            if (item != null)
            {
                CreateWeaponItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Creates the Paradise Paintball part of a weapon
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void CreateWeaponItem(int itemId, int levelRequired, UberstrikeWeaponConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemWeaponConfig config = new ItemWeaponConfig();

                    if (levelRequired < UberStrikeCommonConfig.DefaultLevel)
                    {
                        levelRequired = UberStrikeCommonConfig.DefaultLevel;
                    }

                    config.AccuracySpread = configView.AccuracySpread;
                    config.DamageKnockback = configView.DamageKnockback;
                    config.DamagePerProjectile = configView.DamagePerProjectile;
                    config.ItemId = itemId;
                    config.LevelRequired = levelRequired;
                    config.MaxAmmo = configView.MaxAmmo;
                    config.MissileBounciness = configView.MissileBounciness;
                    config.MissileForceImpulse = configView.MissileForceImpulse;
                    config.MissileTimeToDetonate = configView.MissileTimeToDetonate;
                    config.ProjectileSpeed = configView.ProjectileSpeed;
                    config.ProjectilesPerShot = configView.ProjectilesPerShot;
                    config.RateOfFire = configView.RateOfFire;
                    config.RecoilKickback = configView.RecoilKickback;
                    config.SplashRadius = configView.SplashRadius;
                    config.StartAmmo = configView.StartAmmo;
                    config.RecoilMovement = configView.RecoilMovement;

                    paradiseDB.ItemWeaponConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Creates a new functional item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateFunctionalItem(ItemView itemView, int levelRequired, UberstrikeFunctionalConfigView configView)
        {
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            if (item != null)
            {
                CreateFunctionalItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Creates the Paradise Paintball part of a functional item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void CreateFunctionalItem(int itemId, int levelRequired, UberstrikeFunctionalConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemFunctionalConfig config = new ItemFunctionalConfig();

                    if (levelRequired < UberStrikeCommonConfig.DefaultLevel)
                    {
                        levelRequired = UberStrikeCommonConfig.DefaultLevel;
                    }

                    config.ItemId = itemId;
                    config.LevelRequired = levelRequired;

                    paradiseDB.ItemFunctionalConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Creates a new gear item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateGearItem(ItemView itemView, int levelRequired, UberstrikeGearConfigView configView)
        {
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            if (item != null)
            {
                CreateGearItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Creates the Paradise Paintball part of a gear item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void CreateGearItem(int itemId, int levelRequired, UberstrikeGearConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemGearConfig config = new ItemGearConfig();

                    if (levelRequired < UberStrikeCommonConfig.DefaultLevel)
                    {
                        levelRequired = UberStrikeCommonConfig.DefaultLevel;
                    }

                    config.ArmorAbsorptionPercent = configView.ArmorAbsorptionPercent;
                    config.ArmorPoints = configView.ArmorPoints;
                    config.ArmorWeight = configView.ArmorWeight;
                    config.ItemId = itemId;
                    config.LevelRequired = levelRequired;

                    paradiseDB.ItemGearConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Creates a new quick use item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateQuickUseItem(ItemView itemView, int levelRequired, ItemQuickUseConfigView configView)
        {
            //First insert the item
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            //Now insert the config 
            if (item != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemQuickUseConfig config = configView.ToItemQuickUseConfig();
                    //careful here: use the NEW ItemId that was generated after inserting the item
                    config.ItemId = item.ItemId;
                    config.LevelRequired = levelRequired;

                    paradiseDB.ItemQuickUseConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Creates a new special item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateSpecialItem(ItemView itemView, int levelRequired, UberstrikeSpecialConfigView configView)
        {
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            if (item != null)
            {
                CreateSpecialItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Creates the Paradise Paintball part of a special item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void CreateSpecialItem(int itemId, int levelRequired, UberstrikeSpecialConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemSpecialConfig config = new ItemSpecialConfig();

                    if (levelRequired < UberStrikeCommonConfig.DefaultLevel)
                    {
                        levelRequired = UberStrikeCommonConfig.DefaultLevel;
                    }

                    config.ItemId = itemId;
                    config.LevelRequired = levelRequired;

                    paradiseDB.ItemSpecialConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Creates a new weapon mod item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void CreateWeaponModItem(ItemView itemView, int levelRequired, UberstrikeWeaponModConfigView configView)
        {
            Item item = CmuneItem.CreateItem(UberStrikeCommonConfig.ApplicationId, itemView);

            if (item != null)
            {
                CreateWeaponModItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Creates the Paradise Paintball part of a weapon mod item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void CreateWeaponModItem(int itemId, int levelRequired, UberstrikeWeaponModConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemWeaponModConfig config = new ItemWeaponModConfig();

                    if (levelRequired < UberStrikeCommonConfig.DefaultLevel)
                    {
                        levelRequired = UberStrikeCommonConfig.DefaultLevel;
                    }

                    config.ItemId = itemId;
                    config.LevelRequired = levelRequired;

                    paradiseDB.ItemWeaponModConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        #endregion Create Item

        #region Modify Item

        /// <summary>
        /// Modifies a weapon
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ModifyWeaponItem(ItemView itemView, int levelRequired, UberstrikeWeaponConfigView configView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                ModifyWeaponItem(itemView.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Modifies the Paradise Paintball part of a weapon
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void ModifyWeaponItem(int itemId, int levelRequired, UberstrikeWeaponConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemWeaponConfig config = GetWeaponConfig(itemId, paradiseDB);

                    config.AccuracySpread = configView.AccuracySpread;
                    config.DamageKnockback = configView.DamageKnockback;
                    config.DamagePerProjectile = configView.DamagePerProjectile;
                    config.LevelRequired = levelRequired;
                    config.MaxAmmo = configView.MaxAmmo;
                    config.MissileBounciness = configView.MissileBounciness;
                    config.MissileForceImpulse = configView.MissileForceImpulse;
                    config.MissileTimeToDetonate = configView.MissileTimeToDetonate;
                    config.ProjectileSpeed = configView.ProjectileSpeed;
                    config.ProjectilesPerShot = configView.ProjectilesPerShot;
                    config.RateOfFire = configView.RateOfFire;
                    config.RecoilKickback = configView.RecoilKickback;
                    config.SplashRadius = configView.SplashRadius;
                    config.StartAmmo = configView.StartAmmo;
                    config.RecoilMovement = configView.RecoilMovement;

                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Modifies a functional item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ModifyFunctionalItem(ItemView itemView, int levelRequired, UberstrikeFunctionalConfigView configView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                ModifyFunctionalItem(itemView.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Modifies the Paradise Paintball part of a functional item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void ModifyFunctionalItem(int itemId, int levelRequired, UberstrikeFunctionalConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemFunctionalConfig config = GetFunctionalConfig(itemId, paradiseDB);

                    config.LevelRequired = levelRequired;

                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Modifies a gear item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ModifyGearItem(ItemView itemView, int levelRequired, UberstrikeGearConfigView configView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                ModifyGearItem(itemView.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Modifies the Paradise Paintball part of a gear item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void ModifyGearItem(int itemId, int levelRequired, UberstrikeGearConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemGearConfig config = GetGearConfig(itemId, paradiseDB);

                    config.ArmorAbsorptionPercent = configView.ArmorAbsorptionPercent;
                    config.ArmorPoints = configView.ArmorPoints;
                    config.ArmorWeight = configView.ArmorWeight;
                    config.LevelRequired = levelRequired;

                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Modifies a quick use item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="itemQuickUseConfigView"></param>
        /// <param name="levelRequired"></param>
        public static void ModifyQuickUseItem(ItemView itemView, int levelRequired, ItemQuickUseConfigView itemQuickUseConfigView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    bool insert = false;
                    var config = GetQuickUseConfig(itemView.ItemId, paradiseDB);
                    if (config == null)
                    {
                        config = new ItemQuickUseConfig();
                        insert = true;
                    }
                    config.CopyFromItemQuickUseConfigView(itemQuickUseConfigView);
                    config.ItemId = itemView.ItemId;
                    //overwrite the config properties with the configChanges
                    config.LevelRequired = levelRequired;
                    if (insert)
                        paradiseDB.ItemQuickUseConfigs.InsertOnSubmit(config);
                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Modifies a special item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ModifySpecialItem(ItemView itemView, int levelRequired, UberstrikeSpecialConfigView configView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                ModifySpecialItem(itemView.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Modifies the Paradise Paintball part of a special item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void ModifySpecialItem(int itemId, int levelRequired, UberstrikeSpecialConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemSpecialConfig config = GetSpecialConfig(itemId, paradiseDB);

                    config.LevelRequired = levelRequired;

                    paradiseDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Modifies a weapon mod item
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ModifyWeaponModItem(ItemView itemView, int levelRequired, UberstrikeWeaponModConfigView configView)
        {
            bool isItemModified = CmuneItem.ModifyItem(itemView);

            if (isItemModified)
            {
                ModifyWeaponModItem(itemView.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Modifies the Paradise Paintball part of a weapon mod item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        private static void ModifyWeaponModItem(int itemId, int levelRequired, UberstrikeWeaponModConfigView configView)
        {
            if (configView != null)
            {
                using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                {
                    ItemWeaponModConfig config = GetWeaponModConfig(itemId, paradiseDB);

                    config.LevelRequired = levelRequired;

                    paradiseDB.SubmitChanges();
                }
            }
        }

        #endregion Modify Item

        #region Change Item Type

        /// <summary>
        /// Changes the type of an item to weapon
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousType"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ChangeTypeToWeapon(ItemView item, ShopItemType previousType, int levelRequired, UberstrikeWeaponConfigView configView)
        {
            if (item != null && configView != null)
            {
                CmuneItem.ModifyItem(item);
                DeleteConfig(item.ItemId, previousType);
                CreateWeaponItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Changes the type of an item to functional
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousType"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ChangeTypeToFunctional(ItemView item, ShopItemType previousType, int levelRequired, UberstrikeFunctionalConfigView configView)
        {
            if (item != null && configView != null)
            {
                CmuneItem.ModifyItem(item);
                DeleteConfig(item.ItemId, previousType);
                CreateFunctionalItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Changes the type of an item to gear
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousType"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ChangeTypeToGear(ItemView item, ShopItemType previousType, int levelRequired, UberstrikeGearConfigView configView)
        {
            if (item != null && configView != null)
            {
                CmuneItem.ModifyItem(item);
                DeleteConfig(item.ItemId, previousType);
                CreateGearItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Changes the type of an item to quick use
        /// </summary>
        /// <param name="previousType"></param>
        /// <param name="itemView"></param>
        /// <param name="configView"></param>
        /// <param name="levelRequired"></param>
        public static void ChangeTypeToQuickUse(ItemView itemView, ShopItemType previousType, int levelRequired, ItemQuickUseConfigView configView)
        {
            if (itemView != null)
            {
                CmuneItem.ModifyItem(itemView);
                DeleteConfig(itemView.ItemId, previousType);
                CreateQuickUseItem(itemView, levelRequired, configView);
            }
        }

        /// <summary>
        /// Changes the type of an item to special
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousType"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ChangeTypeToSpecial(ItemView item, ShopItemType previousType, int levelRequired, UberstrikeSpecialConfigView configView)
        {
            if (item != null && configView != null)
            {
                CmuneItem.ModifyItem(item);
                DeleteConfig(item.ItemId, previousType);
                CreateSpecialItem(item.ItemId, levelRequired, configView);
            }
        }

        /// <summary>
        /// Changes the type of an item to special
        /// </summary>
        /// <param name="item"></param>
        /// <param name="previousType"></param>
        /// <param name="levelRequired"></param>
        /// <param name="configView"></param>
        public static void ChangeTypeToWeaponMod(ItemView item, ShopItemType previousType, int levelRequired, UberstrikeWeaponModConfigView configView)
        {
            if (item != null && configView != null)
            {
                CmuneItem.ModifyItem(item);
                DeleteConfig(item.ItemId, previousType);
                CreateWeaponModItem(item.ItemId, levelRequired, configView);
            }
        }

        #endregion Change Item Type

        /// <summary>
        /// Deletes an item config
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="type"></param>
        public static void DeleteConfig(int itemId, ShopItemType type)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                switch (type)
                {
                    case ShopItemType.UberstrikeFunctional:
                        ItemFunctionalConfig functionalConfig = GetFunctionalConfig(itemId, paradiseDB);
                        paradiseDB.ItemFunctionalConfigs.DeleteOnSubmit(functionalConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    case ShopItemType.UberstrikeGear:
                        ItemGearConfig gearConfig = GetGearConfig(itemId, paradiseDB);
                        paradiseDB.ItemGearConfigs.DeleteOnSubmit(gearConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    case ShopItemType.UberstrikeQuickUse:
                        ItemQuickUseConfig quickUseConfig = GetQuickUseConfig(itemId, paradiseDB);
                        paradiseDB.ItemQuickUseConfigs.DeleteOnSubmit(quickUseConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    case ShopItemType.UberstrikeSpecial:
                        ItemSpecialConfig specialConfig = GetSpecialConfig(itemId, paradiseDB);
                        paradiseDB.ItemSpecialConfigs.DeleteOnSubmit(specialConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    case ShopItemType.UberstrikeWeapon:
                        ItemWeaponConfig weaponConfig = GetWeaponConfig(itemId, paradiseDB);
                        paradiseDB.ItemWeaponConfigs.DeleteOnSubmit(weaponConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    case ShopItemType.UberstrikeWeaponMod:
                        ItemWeaponModConfig weaponModConfig = GetWeaponModConfig(itemId, paradiseDB);
                        paradiseDB.ItemWeaponModConfigs.DeleteOnSubmit(weaponModConfig);
                        paradiseDB.SubmitChanges();
                        break;
                    default:
                        CmuneLog.LogUnexpectedReturn(type, String.Empty);
                        throw new ArgumentOutOfRangeException(String.Format("Unexpected ShopItemType: {0}", type.ToString()));
                }
            }
        }

        #region Buy Item

        /// <summary>
        /// Allows to buy all kind of items EXCEPT packs
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="buyerCmid"></param>
        /// <param name="currencyType"></param>
        /// <param name="duration"></param>
        /// <param name="itemType"></param>
        /// <param name="marketLocation"></param>
        /// <param name="recommendationType"></param>
        /// <returns></returns>
        public static int BuyItem(int itemId, int buyerCmid, UberStrikeCurrencyType currencyType, BuyingDurationType duration, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            using (UberstrikeDataContext paradiseDb = new UberstrikeDataContext())
            {
                int ret = UberstrikeBuyItemResult.Ok;

                ret = CheckBuyingArguments(itemId, buyerCmid, itemType);

                //here we ignore the level if we buy for credits
                bool ignoreLevel = ret.Equals(UberstrikeBuyItemResult.InvalidLevel) && currencyType == UberStrikeCurrencyType.Credits;

                if (ret.Equals(UberstrikeBuyItemResult.Ok) || ignoreLevel)
                {
                    ret = CmuneEconomy.BuyItem(UberStrikeCommonConfig.ApplicationId, itemId, buyerCmid, currencyType, duration, marketLocation, recommendationType);
                }

                return ret;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="buyerCmid"></param>
        /// <param name="packType"></param>
        /// <param name="currencyType"></param>
        /// <param name="itemType"></param>
        /// <param name="marketLocation"></param>
        /// <param name="recommendationType"></param>
        /// <returns></returns>
        public static int BuyPack(int itemId, int buyerCmid, PackType packType, UberStrikeCurrencyType currencyType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                int ret = UberstrikeBuyItemResult.Ok;

                ret = CheckBuyingArguments(itemId, buyerCmid, itemType);

                //here we ignore the level if we buy for credits
                bool ignoreLevel = ret.Equals(UberstrikeBuyItemResult.InvalidLevel) && currencyType == UberStrikeCurrencyType.Credits;

                if (ret.Equals(UberstrikeBuyItemResult.Ok) || ignoreLevel)
                {
                    ret = CmuneEconomy.BuyPack(UberStrikeCommonConfig.ApplicationId, itemId, buyerCmid, packType, currencyType, marketLocation, recommendationType);
                }

                return ret;
            }
        }

        /// <summary>
        /// Checks the buying arguments for a functional Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckFunctionalBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            UberstrikeFunctionalConfigView config = GetFunctionalConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks the buying arguments for a gear Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckGearBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            UberstrikeGearConfigView config = GetGearConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks the buying arguments for a quick use Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckQuickUseBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            ItemQuickUseConfigView config = GetQuickUseConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks the buying arguments for a functional Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckSpecialBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            UberstrikeSpecialConfigView config = GetSpecialConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks the buying arguments for a weapon Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckWeaponBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            UberstrikeWeaponConfigView config = GetWeaponConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks the buying arguments for a weapon mod Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="playerStatisticsView"></param>
        /// <returns></returns>
        private static int CheckWeaponModBuyingArguments(int itemId, PlayerStatisticsView playerStatisticsView)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            UberstrikeWeaponModConfigView config = GetWeaponModConfig(itemId);

            if (config != null && playerStatisticsView != null)
            {
                if (IsItemUnlockedForThisLevel(config.LevelRequired, playerStatisticsView.Level))
                {
                    ret = UberstrikeBuyItemResult.Ok;
                }
                else
                {
                    ret = UberstrikeBuyItemResult.InvalidLevel;
                }
            }

            return ret;
        }

        /// <summary>
        /// Checks wether an item is level unlocked for the current member
        /// </summary>
        /// <param name="levelRequired"></param>
        /// <param name="memberLevel"></param>
        /// <returns></returns>
        private static bool IsItemUnlockedForThisLevel(int levelRequired, int memberLevel)
        {
            bool isItemUnlockedForThisLevel = false;

            if (levelRequired == CommonConfig.ItemMallFieldDisable || memberLevel >= levelRequired)
            {
                isItemUnlockedForThisLevel = true;
            }
            else
            {
                isItemUnlockedForThisLevel = false;
            }

            return isItemUnlockedForThisLevel;
        }

        /// <summary>
        /// Checks the buying arguments
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="buyerCmid"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private static int CheckBuyingArguments(int itemId, int buyerCmid, UberstrikeItemType itemType)
        {
            int ret = UberstrikeBuyItemResult.Ok;

            // TODO: we should only get the level
            PlayerStatisticsView statistics = Statistics.GetCompleteStatisticsView(buyerCmid);

            if (statistics != null)
            {
                switch (itemType)
                {
                    case UberstrikeItemType.Functional:
                        ret = CheckFunctionalBuyingArguments(itemId, statistics);
                        break;
                    case UberstrikeItemType.Gear:
                        ret = CheckGearBuyingArguments(itemId, statistics);
                        break;
                    case UberstrikeItemType.QuickUse:
                        ret = CheckQuickUseBuyingArguments(itemId, statistics);
                        break;
                    case UberstrikeItemType.Special:
                        ret = CheckSpecialBuyingArguments(itemId, statistics);
                        break;
                    case UberstrikeItemType.Weapon:
                        ret = CheckWeaponBuyingArguments(itemId, statistics);
                        break;
                    case UberstrikeItemType.WeaponMod:
                        ret = CheckWeaponModBuyingArguments(itemId, statistics);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format("Unexpected UberstrikeItemType: {0}", itemType));
                }
            }

            return ret;
        }

        #endregion Buy Item

        /// <summary>
        /// The replacement item has to be from the same type and class than the deprecated item
        /// </summary>
        /// <param name="itemIdToDisable">This item will be deprecated</param>
        /// <param name="replacementItemId">This item will replace the deprecated one in the member inventory</param>
        /// <returns></returns>
        public static bool DisableItem(int itemIdToDisable, int replacementItemId)
        {
            bool isDisable = CmuneItem.DisableItem(itemIdToDisable, replacementItemId);

            if (isDisable)
            {
                string sqlTemplate = String.Empty;

                Item item = CmuneItem.GetItem(itemIdToDisable);

                if (item != null)
                {
                    ShopItemType itemType = (ShopItemType)item.TypeId;
                    UberstrikeItemClass itemClass = (UberstrikeItemClass)item.ClassId;

                    using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                    {
                        switch (itemType)
                        {
                            case ShopItemType.UberstrikeFunctional:
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [FunctionalItem1] = {0} WHERE [FunctionalItem1] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [FunctionalItem2] = {0} WHERE [FunctionalItem2] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [FunctionalItem3] = {0} WHERE [FunctionalItem3] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                break;
                            case ShopItemType.UberstrikeGear:

                                if (itemClass == UberstrikeItemClass.GearBoots)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Boots] = {0} WHERE [Boots] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearFace)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Face] = {0} WHERE [Face] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearGloves)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Gloves] = {0} WHERE [Gloves] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearHead)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Head] = {0} WHERE [Head] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearHolo)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Webbing] = {0} WHERE [Webbing] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearLowerBody)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [LowerBody] = {0} WHERE [LowerBody] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else if (itemClass == UberstrikeItemClass.GearUpperBody)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [UpperBody] = {0} WHERE [UpperBody] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else
                                {
                                    throw new ArgumentException("No matching class for the type Gear");
                                }

                                break;
                            case ShopItemType.UberstrikeQuickUse:
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [QuickItem1] = {0} WHERE [QuickItem1] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [QuickItem2] = {0} WHERE [QuickItem2] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [QuickItem3] = {0} WHERE [QuickItem3] = {1}";
                                uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                break;
                            case ShopItemType.UberstrikeWeapon:
                                if (itemClass == UberstrikeItemClass.WeaponMelee)
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [MeleeWeapon] = {0} WHERE [MeleeWeapon] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                else
                                {
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Weapon1] = {0} WHERE [Weapon1] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Weapon2] = {0} WHERE [Weapon2] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                    sqlTemplate = "UPDATE [MvParadisePaintball].[dbo].[Loadouts] SET [Weapon3] = {0} WHERE [Weapon3] = {1}";
                                    uberStrikeDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);
                                }
                                break;
                            default:
                                throw new ArgumentException(String.Format("This type ({0}) is not supported yet", itemType));
                        }
                    }
                }
            }

            return isDisable;
        }
    }
}