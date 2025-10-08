using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.Channels.Instrumentation.Models;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.DataAccess;

namespace Cmune.Channels.Instrumentation.Business
{
    public class ItemProcessBusiness
    {
        protected static readonly int ItemsVersion4IdStart = 1000;

        protected string WriteItemInsertToSql(List<Item> items)
        {
            StringBuilder itemsToSql = new StringBuilder();

            if (items != null && items.Count > 0)
            {
                itemsToSql.Append("SET IDENTITY_INSERT [Items] ON<br />");

                foreach (Item item in items)
                {
                    itemsToSql.Append(WriteItemInsertToSql(item));
                }

                itemsToSql.Append("SET IDENTITY_INSERT [Items] OFF<br />");
            }

            return itemsToSql.ToString();
        }

        protected StringBuilder WriteItemInsertToSql(Item item)
        {
            StringBuilder itemToSql = new StringBuilder();

            if (item != null)
            {
                itemToSql.Append(@"SET IDENTITY_INSERT [Items] ON;
                                    INSERT INTO [Items] 
                                        ([ItemId], [Name], [Description], [CreditsPerDayShop], [PointsPerDayShop], [TypeId], [ClassId], [IsForSale], 
                                         [AmountRemainingInShop], [IsFeatured], [PurchaseType], [IsNew], [IsPopular], [PermanentCreditsShop], 
                                         [PackOneAmount], [PackTwoAmount], [PackThreeAmount], [MaximumOwnableAmount], [Enable1Day], 
                                         [Enable7Days], [Enable30Days], [Enable90Days], [MaximumDurationDays], [IsEnabledInShop], 
                                         [CreditsPerDayUnderground], [PermanentCreditsUnderground], [IsEnabledInUnderground], [AmountRemainingInUnderground]) VALUES (");
                itemToSql.Append(item.ItemId);
                itemToSql.Append(", '");
                itemToSql.Append(TextUtilities.ProtectSqlField(item.Name));
                itemToSql.Append("', '");
                itemToSql.Append(TextUtilities.ProtectSqlField(item.Description));
                itemToSql.Append("', ");
                itemToSql.Append(item.CreditsPerDay);
                itemToSql.Append(", ");
                itemToSql.Append(item.PointsPerDay);
                itemToSql.Append(", ");
                itemToSql.Append(item.TypeId);
                itemToSql.Append(", ");
                itemToSql.Append(item.ClassId);
                itemToSql.Append(", '");
                itemToSql.Append(item.IsForSale);
                itemToSql.Append("', ");
                itemToSql.Append(item.AmountRemaining);
                itemToSql.Append(", '");
                itemToSql.Append(item.IsFeatured);
                itemToSql.Append("', ");
                itemToSql.Append(item.PurchaseType);
                itemToSql.Append(", '");
                itemToSql.Append(item.IsNew);
                itemToSql.Append("', '");
                itemToSql.Append(item.IsPopular);
                itemToSql.Append("', ");
                itemToSql.Append(item.PermanentCredits);
                itemToSql.Append(", ");
                itemToSql.Append(item.PackOneAmount);
                itemToSql.Append(", ");
                itemToSql.Append(item.PackTwoAmount);
                itemToSql.Append(", ");
                itemToSql.Append(item.PackThreeAmount);
                itemToSql.Append(", ");
                itemToSql.Append(item.MaximumOwnableAmount);
                itemToSql.Append(", '");
                itemToSql.Append(item.Enable1Day);
                itemToSql.Append("', '");
                itemToSql.Append(item.Enable7Days);
                itemToSql.Append("', '");
                itemToSql.Append(item.Enable30Days);
                itemToSql.Append("', '");
                itemToSql.Append(item.Enable90Days);
                itemToSql.Append("', ");
                itemToSql.Append(item.MaximumDurationDays);
                itemToSql.Append(", '");
                itemToSql.Append(item.IsEnabledInShop);
                itemToSql.Append("', ");
                itemToSql.Append(item.CreditsPerDayUnderground);
                itemToSql.Append(", ");
                itemToSql.Append(item.PermanentCreditsUnderground);
                itemToSql.Append(",'");
                itemToSql.Append(item.IsEnabledInUnderground);
                itemToSql.Append("',");
                itemToSql.Append(item.AmountRemainingInUnderground);
                itemToSql.Append(");");

                itemToSql.Append("INSERT INTO [ItemToApplication] ([ItemId], [ApplicationId]) VALUES (");
                itemToSql.Append(item.ItemId);
                itemToSql.Append(", 1);SET IDENTITY_INSERT [Items] OFF;");

            }

            return itemToSql;

        }

        /// <summary>
        /// Synchronize items from dev to staging or staging to prod depending on the environment it's running
        /// </summary>
        /// <param name="dbCmuneConnectionString"></param>
        /// <param name="dbUberstrikeConnectionString"></param>
        /// <returns></returns>
        public bool SyncItems(string dbCmuneConnectionString, string dbUberstrikeConnectionString)
        {
            using (CmuneDataContext dbCmuneSource = new CmuneDataContext())
            using (UberstrikeDataContext dbUberstrikeSource = new UberstrikeDataContext())
            using (CmuneDataContext dbCmuneTarget = new CmuneDataContext(dbCmuneConnectionString))
            using (UberstrikeDataContext dbUberstrikeTarget = new UberstrikeDataContext(dbUberstrikeConnectionString))
            {
                // We need to get the last sync item
                int dbTargetMaxId = dbCmuneTarget.Items.Max(i => i.ItemId);

                Dictionary<int, Item> sourceItemsById = dbCmuneSource.Items.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<Item> targetItems = dbCmuneTarget.Items.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<Item> sourceItems = dbCmuneSource.Items.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemFunctionalConfig> itemsFunctionalConfigToUpdateDev = dbUberstrikeSource.ItemFunctionalConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<ItemFunctionalConfig> itemsFunctionalConfigToUpdateDBToSync = dbUberstrikeTarget.ItemFunctionalConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemFunctionalConfig> itemsFunctionalConfigToInsertDev = dbUberstrikeSource.ItemFunctionalConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemGearConfig> itemsGearConfigToUpdateDev = dbUberstrikeSource.ItemGearConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<ItemGearConfig> itemsGearConfigToUpdateDBToSync = dbUberstrikeTarget.ItemGearConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemGearConfig> itemsGearConfigToInsertDev = dbUberstrikeSource.ItemGearConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemQuickUseConfig> itemsQuickUseConfigToUpdateDev = dbUberstrikeSource.ItemQuickUseConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<ItemQuickUseConfig> itemsQuickUseConfigToUpdateDBToSync = dbUberstrikeTarget.ItemQuickUseConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemQuickUseConfig> itemsQuickUseConfigToInsertDev = dbUberstrikeSource.ItemQuickUseConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemSpecialConfig> itemsSpecialConfigUpdateDev = dbUberstrikeSource.ItemSpecialConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<ItemSpecialConfig> itemsSpecialConfigUpdateDBToSync = dbUberstrikeTarget.ItemSpecialConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemSpecialConfig> itemsSpecialConfigToInsertDev = dbUberstrikeSource.ItemSpecialConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemWeaponConfig> itemsWeaponConfigToUpdateDev = dbUberstrikeSource.ItemWeaponConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId);
                List<ItemWeaponConfig> itemsWeaponConfigToUpdateDBToSync = dbUberstrikeTarget.ItemWeaponConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemWeaponConfig> itemsWeaponConfigToInsertDev = dbUberstrikeSource.ItemWeaponConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                Dictionary<int, ItemWeaponModConfig> itemsWeaponModConfigToUpdateDev = dbUberstrikeSource.ItemWeaponModConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToDictionary(i => i.ItemId); ;
                List<ItemWeaponModConfig> itemsWeaponModConfigToUpdateDBToSync = dbUberstrikeTarget.ItemWeaponModConfigs.Where(i => i.ItemId >= ItemsVersion4IdStart && i.ItemId <= dbTargetMaxId).ToList();
                List<ItemWeaponModConfig> itemsWeaponModConfigToInsertDev = dbUberstrikeSource.ItemWeaponModConfigs.Where(i => i.ItemId > dbTargetMaxId).ToList();

                foreach (Item dbTargetItem in targetItems)
                {
                    Item dbSourceItem;
                    if (sourceItemsById.TryGetValue(dbTargetItem.ItemId, out dbSourceItem))
                    {
                        dbTargetItem.Name = dbSourceItem.Name;
                        dbTargetItem.Description = dbSourceItem.Description;
                        dbTargetItem.PermanentCredits = dbSourceItem.PermanentCredits;
                        dbTargetItem.PermanentPoints = dbSourceItem.PermanentPoints;
                        dbTargetItem.CreditsPerDay = dbSourceItem.CreditsPerDay;
                        dbTargetItem.PointsPerDay = dbSourceItem.PointsPerDay;
                        dbTargetItem.ClassId = dbSourceItem.ClassId;
                        dbTargetItem.TypeId = dbSourceItem.TypeId;
                        dbTargetItem.PurchaseType = dbSourceItem.PurchaseType;
                        dbTargetItem.IsForSale = dbSourceItem.IsForSale;
                        dbTargetItem.IsFeatured = dbSourceItem.IsFeatured;
                        dbTargetItem.IsNew = dbSourceItem.IsNew;
                        dbTargetItem.IsPopular = dbSourceItem.IsPopular;
                        dbTargetItem.CustomProperties = dbSourceItem.CustomProperties;

                        dbTargetItem.Enable1Day = dbSourceItem.Enable1Day;
                        dbTargetItem.Enable30Days = dbSourceItem.Enable30Days;
                        dbTargetItem.Enable7Days = dbSourceItem.Enable7Days;
                        dbTargetItem.Enable90Days = dbSourceItem.Enable90Days;
                        dbTargetItem.MaximumDurationDays = dbSourceItem.MaximumDurationDays;

                        dbTargetItem.AmountRemaining = dbSourceItem.AmountRemaining;
                        dbTargetItem.MaximumOwnableAmount = dbSourceItem.MaximumOwnableAmount;
                        dbTargetItem.PackOneAmount = dbSourceItem.PackOneAmount;
                        dbTargetItem.PackThreeAmount = dbSourceItem.PackThreeAmount;
                        dbTargetItem.PackTwoAmount = dbSourceItem.PackTwoAmount;
                    }
                }

                foreach (ItemFunctionalConfig dbToSyncConfig in itemsFunctionalConfigToUpdateDBToSync)
                {
                    if (itemsFunctionalConfigToUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemFunctionalConfig devConfig = itemsFunctionalConfigToUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                    }
                }

                foreach (ItemGearConfig dbToSyncConfig in itemsGearConfigToUpdateDBToSync)
                {
                    if (itemsGearConfigToUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemGearConfig devConfig = itemsGearConfigToUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                        dbToSyncConfig.ArmorAbsorptionPercent = devConfig.ArmorAbsorptionPercent;
                        dbToSyncConfig.ArmorPoints = devConfig.ArmorPoints;
                        dbToSyncConfig.ArmorWeight = devConfig.ArmorWeight;
                    }
                }

                foreach (ItemQuickUseConfig dbToSyncConfig in itemsQuickUseConfigToUpdateDBToSync)
                {
                    if (itemsQuickUseConfigToUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemQuickUseConfig devConfig = itemsQuickUseConfigToUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                        dbToSyncConfig.BehaviourType = devConfig.BehaviourType;
                        dbToSyncConfig.CoolDownTime = devConfig.CoolDownTime;
                        dbToSyncConfig.UsesPerGame = devConfig.UsesPerGame;
                        dbToSyncConfig.UsesPerLife = devConfig.UsesPerLife;
                        dbToSyncConfig.UsesPerRound = devConfig.UsesPerRound;
                        dbToSyncConfig.WarmUpTime = devConfig.WarmUpTime;
                    }
                }

                foreach (ItemSpecialConfig dbToSyncConfig in itemsSpecialConfigUpdateDBToSync)
                {
                    if (itemsSpecialConfigUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemSpecialConfig devConfig = itemsSpecialConfigUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                    }
                }

                foreach (ItemWeaponConfig dbToSyncConfig in itemsWeaponConfigToUpdateDBToSync)
                {
                    if (itemsWeaponConfigToUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemWeaponConfig devConfig = itemsWeaponConfigToUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                        dbToSyncConfig.RateOfFire = devConfig.RateOfFire;
                        dbToSyncConfig.ProjectilesPerShot = devConfig.ProjectilesPerShot;
                        dbToSyncConfig.DamagePerProjectile = devConfig.DamagePerProjectile;
                        dbToSyncConfig.AccuracySpread = devConfig.AccuracySpread;
                        dbToSyncConfig.DamageKnockback = devConfig.DamageKnockback;
                        dbToSyncConfig.RecoilKickback = devConfig.RecoilKickback;
                        dbToSyncConfig.RecoilMovement = devConfig.RecoilMovement;
                        dbToSyncConfig.StartAmmo = devConfig.StartAmmo;
                        dbToSyncConfig.MaxAmmo = devConfig.MaxAmmo;
                        dbToSyncConfig.MissileBounciness = devConfig.MissileBounciness;
                        dbToSyncConfig.MissileForceImpulse = devConfig.MissileForceImpulse;
                        dbToSyncConfig.MissileTimeToDetonate = devConfig.MissileTimeToDetonate;
                        dbToSyncConfig.SplashRadius = devConfig.SplashRadius;
                        dbToSyncConfig.ProjectileSpeed = devConfig.ProjectileSpeed;
                    }
                }

                foreach (ItemWeaponModConfig dbToSyncConfig in itemsWeaponModConfigToUpdateDBToSync)
                {
                    if (itemsWeaponModConfigToUpdateDev.ContainsKey(dbToSyncConfig.ItemId))
                    {
                        ItemWeaponModConfig devConfig = itemsWeaponModConfigToUpdateDev[dbToSyncConfig.ItemId];

                        dbToSyncConfig.LevelRequired = devConfig.LevelRequired;
                    }
                }
                dbCmuneTarget.SubmitChanges();
                dbUberstrikeTarget.SubmitChanges();

                foreach (Item itemToInsert in sourceItems)
                {
                    dbCmuneTarget.ExecuteCommand(WriteItemInsertToSql(itemToInsert).ToString());
                }


                // FUNC CONFIG
                var itemsFunctionalConfigToInsertDBToSync = new List<ItemFunctionalConfig>();
                foreach (ItemFunctionalConfig configToInsert in itemsFunctionalConfigToInsertDev)
                {
                    ItemFunctionalConfig configToInsertDBToSync = new ItemFunctionalConfig();

                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;

                    itemsFunctionalConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemFunctionalConfigs.InsertAllOnSubmit(itemsFunctionalConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();


                // GEAR CONFIG
                var itemsGearConfigToInsertDBToSync = new List<ItemGearConfig>();
                foreach (ItemGearConfig configToInsert in itemsGearConfigToInsertDev)
                {
                    ItemGearConfig configToInsertDBToSync = new ItemGearConfig();

                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;
                    configToInsertDBToSync.ArmorAbsorptionPercent = configToInsert.ArmorAbsorptionPercent;
                    configToInsertDBToSync.ArmorPoints = configToInsert.ArmorPoints;
                    configToInsertDBToSync.ArmorWeight = configToInsert.ArmorWeight;

                    itemsGearConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemGearConfigs.InsertAllOnSubmit(itemsGearConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();


                // QUICK USE CONFIG
                var itemsQuickUseConfigToInsertDBToSync = new List<ItemQuickUseConfig>();
                foreach (ItemQuickUseConfig configToInsert in itemsQuickUseConfigToInsertDev)
                {
                    ItemQuickUseConfig configToInsertDBToSync = new ItemQuickUseConfig();

                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;

                    configToInsertDBToSync.BehaviourType = configToInsert.BehaviourType;
                    configToInsertDBToSync.CoolDownTime = configToInsert.CoolDownTime;
                    configToInsertDBToSync.UsesPerGame = configToInsert.UsesPerGame;
                    configToInsertDBToSync.UsesPerLife = configToInsert.UsesPerLife;
                    configToInsertDBToSync.UsesPerRound = configToInsert.UsesPerRound;
                    configToInsertDBToSync.WarmUpTime = configToInsert.WarmUpTime;

                    itemsQuickUseConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemQuickUseConfigs.InsertAllOnSubmit(itemsQuickUseConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();


                // SPECIAL CONFIG
                var itemsSpecialConfigToInsertDBToSync = new List<ItemSpecialConfig>();
                foreach (ItemSpecialConfig configToInsert in itemsSpecialConfigToInsertDev)
                {
                    ItemSpecialConfig configToInsertDBToSync = new ItemSpecialConfig();

                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;

                    itemsSpecialConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemSpecialConfigs.InsertAllOnSubmit(itemsSpecialConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();


                // WEAPONS CONFIG
                var itemsWeaponConfigToInsertDBToSync = new List<ItemWeaponConfig>();
                foreach (ItemWeaponConfig configToInsert in itemsWeaponConfigToInsertDev)
                {
                    ItemWeaponConfig configToInsertDBToSync = new ItemWeaponConfig();

                    configToInsertDBToSync.AccuracySpread = configToInsert.AccuracySpread;
                    configToInsertDBToSync.DamageKnockback = configToInsert.DamageKnockback;
                    configToInsertDBToSync.DamagePerProjectile = configToInsert.DamagePerProjectile;
                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;
                    configToInsertDBToSync.MaxAmmo = configToInsert.MaxAmmo;
                    configToInsertDBToSync.MissileBounciness = configToInsert.MissileBounciness;
                    configToInsertDBToSync.MissileForceImpulse = configToInsert.MissileForceImpulse;
                    configToInsertDBToSync.MissileTimeToDetonate = configToInsert.MissileTimeToDetonate;
                    configToInsertDBToSync.ProjectileSpeed = configToInsert.ProjectileSpeed;
                    configToInsertDBToSync.ProjectilesPerShot = configToInsert.ProjectilesPerShot;
                    configToInsertDBToSync.RateOfFire = configToInsert.RateOfFire;
                    configToInsertDBToSync.RecoilKickback = configToInsert.RecoilKickback;
                    configToInsertDBToSync.RecoilMovement = configToInsert.RecoilMovement;
                    configToInsertDBToSync.SplashRadius = configToInsert.SplashRadius;
                    configToInsertDBToSync.StartAmmo = configToInsert.StartAmmo;

                    itemsWeaponConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemWeaponConfigs.InsertAllOnSubmit(itemsWeaponConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();


                // WEAPON MOD CONFIG
                var itemsWeaponModConfigToInsertDBToSync = new List<ItemWeaponModConfig>();
                foreach (ItemWeaponModConfig configToInsert in itemsWeaponModConfigToInsertDev)
                {
                    ItemWeaponModConfig configToInsertDBToSync = new ItemWeaponModConfig();

                    configToInsertDBToSync.ItemId = configToInsert.ItemId;
                    configToInsertDBToSync.LevelRequired = configToInsert.LevelRequired;

                    itemsWeaponModConfigToInsertDBToSync.Add(configToInsertDBToSync);
                }
                dbUberstrikeTarget.ItemWeaponModConfigs.InsertAllOnSubmit(itemsWeaponModConfigToInsertDBToSync);
                dbUberstrikeTarget.SubmitChanges();

                return true;
            }
        }

        /// <summary>
        /// Retrieves the difference between the prod and the dev environment
        /// </summary>
        /// <param name="prodCmuneDbConnectionString"></param>
        /// <param name="prodUberStrikeDbConnectionString"></param>
        /// <returns></returns>
        public static ItemSyncModel SyncItemsReverse(string prodCmuneDbConnectionString, string prodUberStrikeDbConnectionString)
        {
            using (CmuneDataContext cmuneDevDb = new CmuneDataContext())
            using (UberstrikeDataContext uberStrikeDevDb = new UberstrikeDataContext())
            using (CmuneDataContext cmuneProdDb = new CmuneDataContext(prodCmuneDbConnectionString))
            using (UberstrikeDataContext uberStrikeProdDb = new UberstrikeDataContext(prodUberStrikeDbConnectionString))
            {
                Dictionary<int, Item> devItems = cmuneDevDb.Items.Where(i => i.ItemId > CommonConfig.NewItemMallItemIdStart).ToDictionary(i => i.ItemId, i => i);
                Dictionary<int, Item> prodItems = cmuneProdDb.Items.Where(i => i.ItemId > CommonConfig.NewItemMallItemIdStart).ToDictionary(i => i.ItemId, i => i);

                Dictionary<int, ItemFunctionalConfig> devFunctional = uberStrikeDevDb.ItemFunctionalConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
                Dictionary<int, ItemGearConfig> devGear = uberStrikeDevDb.ItemGearConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
                Dictionary<int, ItemQuickUseConfig> devQuickUse = uberStrikeDevDb.ItemQuickUseConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
                Dictionary<int, ItemSpecialConfig> devSpecial = uberStrikeDevDb.ItemSpecialConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
                Dictionary<int, ItemWeaponConfig> devWeapon = uberStrikeDevDb.ItemWeaponConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
                Dictionary<int, ItemWeaponModConfig> devWeaponMod = uberStrikeDevDb.ItemWeaponModConfigs.Where(c => c.ItemId >= CommonConfig.NewItemMallItemIdStart).ToDictionary(c => c.ItemId, c => c);
            }

            return null;
        }
    }
}