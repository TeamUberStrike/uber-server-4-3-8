using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.DataAccess;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Utils;
using System.Diagnostics;

namespace Cmune.Instrumentation.Batch.Apps.Shooter
{
    public class LoadoutExpiration
    {
        private Dictionary<String, ItemInventory> _itemInventoriesOrdered;
        private DateTime _currentDate;

        public LoadoutExpiration()
        {
        }

        /// <summary>
        /// Expires the items
        /// </summary>
        public void ExpireLoadouts(bool debugMode)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                _currentDate = DateTime.Now;
                int itemsRemovedCount = 0;
                StringBuilder debugInfo = new StringBuilder();

                int loadoutLimit = ConfigurationUtilities.ReadConfigurationManagerInt("LoadoutLimit");
                int currentLoadoutCount = 0;
                int loadoutTotalCount = 0;
                int minCmid = 0;
                int maxCmid = 0;
                int itemInventoriesTotalCount = 0;
                int submitDuration = 0;
                int j = 1;

                do
                {
                    using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
                    using (CmuneDataContext cmuneDB = new CmuneDataContext())
                    {
                        maxCmid = loadoutLimit * j;
                        minCmid = maxCmid - loadoutLimit;

                        List<Loadout> loadouts = paradiseDB.Loadouts.Where(l => l.Cmid <= maxCmid && l.Cmid > minCmid).ToList();
                        currentLoadoutCount = loadouts.Count;
                        loadoutTotalCount += currentLoadoutCount;
                        List<ItemInventory> itemInventories = cmuneDB.ItemInventories.Where(iI => (iI.ExpirationDate > _currentDate || iI.ExpirationDate == null) && iI.Cmid <= maxCmid && iI.Cmid > minCmid).ToList();
                        itemInventoriesTotalCount += itemInventories.Count;

                        _itemInventoriesOrdered = new Dictionary<string, ItemInventory>();

                        foreach (ItemInventory itemInventory in itemInventories)
                        {
                            if (!_itemInventoriesOrdered.ContainsKey(GenerateKey(itemInventory.Cmid, itemInventory.ItemId)))
                            {
                                _itemInventoriesOrdered.Add(GenerateKey(itemInventory.Cmid, itemInventory.ItemId), itemInventory);
                            }
                        }

                        bool isSlotToEmpty = false;

                        int i = 0;

                        foreach (Loadout loadout in loadouts)
                        {
                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Backpack);

                            if (isSlotToEmpty)
                            {
                                loadout.Backpack = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Boots);

                            if (isSlotToEmpty)
                            {
                                loadout.Boots = UberStrikeCommonConfig.DefaultBoots;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Face);

                            if (isSlotToEmpty)
                            {
                                loadout.Face = UberStrikeCommonConfig.DefaultFace;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.FunctionalItem1);

                            if (isSlotToEmpty)
                            {
                                loadout.FunctionalItem1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.FunctionalItem2);

                            if (isSlotToEmpty)
                            {
                                loadout.FunctionalItem2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.FunctionalItem3);

                            if (isSlotToEmpty)
                            {
                                loadout.FunctionalItem3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Gloves);

                            if (isSlotToEmpty)
                            {
                                loadout.Gloves = UberStrikeCommonConfig.DefaultGloves;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Head);

                            if (isSlotToEmpty)
                            {
                                loadout.Head = UberStrikeCommonConfig.DefaultHead;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.LowerBody);

                            if (isSlotToEmpty)
                            {
                                loadout.LowerBody = UberStrikeCommonConfig.DefaultLowerBody;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.MeleeWeapon);

                            if (isSlotToEmpty)
                            {
                                loadout.MeleeWeapon = UberStrikeCommonConfig.DefaultMeleeWeapon;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.QuickItem1);

                            if (isSlotToEmpty)
                            {
                                loadout.QuickItem1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.QuickItem2);

                            if (isSlotToEmpty)
                            {
                                loadout.QuickItem2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.QuickItem3);

                            if (isSlotToEmpty)
                            {
                                loadout.QuickItem3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.UpperBody);

                            if (isSlotToEmpty)
                            {
                                loadout.UpperBody = UberStrikeCommonConfig.DefaultUpperBody;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon1);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon1Mod1);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon1Mod1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon1Mod2);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon1Mod2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon1Mod3);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon1Mod3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon2);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon2Mod1);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon2Mod1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon2Mod2);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon2Mod2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon2Mod3);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon2Mod3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon3);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon3Mod1);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon3Mod1 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon3Mod2);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon3Mod2 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Weapon3Mod3);

                            if (isSlotToEmpty)
                            {
                                loadout.Weapon3Mod3 = 0;
                                itemsRemovedCount++;
                            }

                            isSlotToEmpty = IsSlotToEmpty(loadout.Cmid, loadout.Webbing);

                            if (isSlotToEmpty)
                            {
                                loadout.Webbing = 0;
                                itemsRemovedCount++;
                            }

                            if (loadout.Weapon1 == 0 && loadout.Weapon2 == 0 && loadout.Weapon3 == 0)
                            {
                                loadout.Weapon1 = UberStrikeCommonConfig.DefaultWeapon;
                            }

                            if (debugMode && i % 50000 == 0)
                            {
                                Console.WriteLine("[Inventories processed: " + i.ToString() + "]");
                            }

                            i++;
                        }

                        if (debugMode)
                        {
                            Console.WriteLine("[Submit started]");
                        }

                        int submitStart = (int)watch.ElapsedMilliseconds;
                        paradiseDB.SubmitChanges();
                        int submitEnd = (int)watch.ElapsedMilliseconds;
                        submitDuration += (int)(submitEnd - submitStart) / 1000;

                        if (debugMode)
                        {
                            Console.WriteLine("[Submit ended]");
                        }

                        j++;
                        _itemInventoriesOrdered = null;
                        loadouts = null;
                        itemInventories = null;
                    }

                } while (currentLoadoutCount > 0);

                watch.Stop();

                int scriptDuration = (int)watch.ElapsedMilliseconds / 1000;

                debugInfo.Append("[Loadouts: ");
                debugInfo.Append(CmuneLog.DisplayForLog(loadoutTotalCount, 8));
                debugInfo.Append("]");
                debugInfo.Append("[Inventories: ");
                debugInfo.Append(CmuneLog.DisplayForLog(itemInventoriesTotalCount, 9));
                debugInfo.Append("]");

                if (debugMode)
                {
                    Console.WriteLine("[Loadouts: " + loadoutTotalCount+"]");
                    Console.WriteLine("[Inventories: " + itemInventoriesTotalCount + "]");
                }

                debugInfo.Append("[Items removed: ");
                debugInfo.Append(CmuneLog.DisplayForLog(itemsRemovedCount, 8));
                debugInfo.Append("]");
                debugInfo.Append("[Script duration: ");
                debugInfo.Append(CmuneLog.DisplayForLog(scriptDuration, 5));
                debugInfo.Append("s]");
                debugInfo.Append("[Submit duration: ");
                debugInfo.Append(CmuneLog.DisplayForLog(submitDuration, 5));
                debugInfo.Append("s]");

                if (debugMode)
                {
                    Console.WriteLine("[Items removed: " + itemsRemovedCount.ToString() + "]");
                    Console.WriteLine("[Script duration: " + scriptDuration.ToString() + "s]");
                    Console.WriteLine("[Submit duration: " + submitDuration.ToString() + "s]");
                }

                CmuneLog.CustomLogToDefaultPath("loadout.expiration.log", debugInfo.ToString());
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, "LoadoutExpiration ended unexpectedly!");
                throw;
            }
        }

        /// <summary>
        /// Generates a uniq key to know if an item is own by a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private string GenerateKey(int cmid, int itemId)
        {
            string key = cmid.ToString() + "|" + itemId.ToString();

            return key;
        }

        /// <summary>
        /// Checks wether we need to empty a slot
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private bool IsSlotToEmpty(int cmid, int itemId)
        {
            bool deleteEquippedItem = false;

            if (itemId != 0)
            {
                string currentKey = GenerateKey(cmid, itemId);

                if (!_itemInventoriesOrdered.ContainsKey(currentKey))
                {
                    deleteEquippedItem = true;
                }
            }

            return deleteEquippedItem;
        }
    }
}