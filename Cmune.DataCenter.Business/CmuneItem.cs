using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Utils.Caching;
using System.Text;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// 
    /// </summary>
    public static class CmuneItem
    {

        /// <summary>
        /// convert item model to item viewmodel
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ItemView ToItemView(this Item item)
        {
            var itemView = new ItemView();

            itemView.ItemId = item.ItemId;
            itemView.TypeId = item.TypeId;
            itemView.ClassId = item.ClassId;
            itemView.Name = item.Name;
            itemView.PurchaseType = (PurchaseType)item.PurchaseType;
            itemView.Description = item.Description;
            itemView.IsForSale = item.IsForSale;
            itemView.MaximumDurationDays = item.MaximumDurationDays;
            itemView.IsFeatured = item.IsFeatured;
            itemView.IsNew = item.IsNew;
            itemView.IsPopular = item.IsPopular;
            itemView.CreditsPerDay = item.CreditsPerDay;
            itemView.PointsPerDay = item.PointsPerDay;
            itemView.PermanentCredits = item.PermanentCredits;
            itemView.PermanentPoints = item.PermanentPoints;
            itemView.AmountRemaining = item.AmountRemaining;
            itemView.PackOneAmount = item.PackOneAmount;
            itemView.PackTwoAmount = item.PackTwoAmount;
            itemView.PackThreeAmount = item.PackThreeAmount;
            itemView.MaximumOwnableAmount = item.MaximumOwnableAmount;
            itemView.Enable1Day = item.Enable1Day;
            itemView.Enable7Days = item.Enable7Days;
            itemView.Enable30Days = item.Enable30Days;
            itemView.Enable90Days = item.Enable90Days;
            itemView.CustomProperties = ConvertStringToProperties(item.CustomProperties);
            itemView.IsDisable = item.IsDisabled;

            return itemView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemView"></param>
        /// <returns></returns>
        public static Item ToItem(this ItemView itemView)
        {
            var item = new Item();

            item.ItemId = itemView.ItemId;
            item.CopyFromItemView(itemView);
            return item;
        }

        public static void CopyFromItemView(this Item item, ItemView itemView)
        {
            if (itemView.Name == null)
                itemView.Name = string.Empty;
            if (itemView.Description == null)
                itemView.Description = string.Empty;

            item.TypeId = itemView.TypeId;
            item.ClassId = itemView.ClassId;
            item.Name = itemView.Name;
            item.PurchaseType = (int)itemView.PurchaseType;
            item.Description = itemView.Description;
            item.IsForSale = itemView.IsForSale;
            item.MaximumDurationDays = itemView.MaximumDurationDays;
            item.IsFeatured = itemView.IsFeatured;
            item.IsNew = itemView.IsNew;
            item.IsPopular = itemView.IsPopular;
            item.CreditsPerDay = itemView.CreditsPerDay;
            item.PointsPerDay = itemView.PointsPerDay;
            item.PermanentCredits = itemView.PermanentCredits;
            item.PermanentPoints = itemView.PermanentPoints;
            item.AmountRemaining = CommonConfig.ItemMallFieldDisable;
            item.PackOneAmount = itemView.PackOneAmount;
            item.PackTwoAmount = itemView.PackTwoAmount;
            item.PackThreeAmount = itemView.PackThreeAmount;
            item.MaximumOwnableAmount = itemView.MaximumOwnableAmount;
            item.Enable1Day = itemView.Enable1Day;
            item.Enable7Days = itemView.Enable7Days;
            item.Enable30Days = itemView.Enable30Days;
            item.Enable90Days = itemView.Enable90Days;
            item.CustomProperties = ConvertCustomPropertiesToString(itemView.CustomProperties);
            item.IsDisabled = itemView.IsDisable;
        }


        #region Search

        /// <summary>
        /// Search items by type
        /// </summary>
        /// <param name="applicationId">0 = All applications</param>
        /// <param name="typeId">0 = All Types</param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<ItemView> GetItemByType(int applicationId, int typeId, CmuneDataContext cmuneDb = null)
        {
            if (cmuneDb == null)
                cmuneDb = new CmuneDataContext();

            using (cmuneDb)
            {
                List<int> listIdItem = new List<int>();
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    if (typeId == 0)
                    {
                        items = cmuneDb.Items.Where(i => i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                    }
                    else
                    {
                        items = cmuneDb.Items.Where(i => i.TypeId == typeId && i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                    }
                }
                else
                {
                    listIdItem = GetItemIds(applicationId, cmuneDb);

                    if (typeId == 0)
                    {
                        items = cmuneDb.Items.Where(i => listIdItem.Contains(i.ItemId)).OrderByDescending(i => i.ItemId).ToList();
                    }
                    else
                    {
                        var q = cmuneDb.Items.Where(i => listIdItem.Contains(i.ItemId) && i.TypeId == typeId).OrderByDescending(i => i.ItemId);
                        items = q.ToList();
                    }
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Search items by class
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public static List<ItemView> GetItemByClass(int applicationId, int classId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<int> listIdItem = new List<int>();
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    if (classId == 0)
                    {
                        items = cmuneDb.Items.Where(i => i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                    }
                    else
                    {
                        items = cmuneDb.Items.Where(i => i.ClassId == classId && i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                    }
                }
                else
                {
                    listIdItem = GetItemIds(applicationId, cmuneDb);

                    if (classId == 0)
                    {
                        items = cmuneDb.Items.Where(i => listIdItem.Contains(i.ItemId)).OrderByDescending(i => i.ItemId).ToList();
                    }
                    else
                    {
                        items = cmuneDb.Items.Where(i => listIdItem.Contains(i.ItemId) && i.ClassId == classId).OrderByDescending(i => i.ItemId).ToList();
                    }
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Search items by name
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<ItemView> GetItemByName(int applicationId, string name)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = new List<Item>();

                if (!name.IsNullOrFullyEmpty())
                {
                    name = name.Trim();

                    if (applicationId == 0)
                    {
                        items = cmuneDb.Items.Where(i => SqlMethods.Like(i.Name, "%" + name + "%")).OrderByDescending(i => i.ItemId).ToList();
                    }
                    else
                    {
                        List<int> itemIds = GetItemIds(applicationId, cmuneDb);
                        items = cmuneDb.Items.Where(i => itemIds.Contains(i.ItemId) && SqlMethods.Like(i.Name, "%" + name + "%")).OrderByDescending(i => i.ItemId).ToList();
                    }
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get all the items where IsNew is set to true
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ItemView> SearchItemByIsNew(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    items = cmuneDb.Items.Where(i => i.IsNew == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                }
                else
                {
                    var query = from i in cmuneDb.Items
                                join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                                where a.ApplicationId == applicationId && i.IsNew == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                                orderby i.ItemId descending
                                select i;

                    items = query.ToList();
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get all the items where IsFeatured is set to true
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ItemView> SearchItemByIsFeatured(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    items = cmuneDb.Items.Where(i => i.IsFeatured == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                }
                else
                {
                    var query = from i in cmuneDb.Items
                                join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                                where a.ApplicationId == applicationId && i.IsFeatured == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                                orderby i.ItemId descending
                                select i;

                    items = query.ToList();
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get all the items where IsPopular is set to true
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ItemView> SearchItemByIsPopular(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    items = cmuneDb.Items.Where(i => i.IsPopular == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                }
                else
                {
                    var query = from i in cmuneDb.Items
                                join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                                where a.ApplicationId == applicationId && i.IsPopular == true && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                                orderby i.ItemId descending
                                select i;

                    items = query.ToList();
                }

                return ToItemView(items);
            }
        }

        #endregion Search

        #region Getters

        /// <summary>
        /// Get all the Item Ids linked to an application
        /// Needs caching
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<int> GetItemIds(int applicationId, CmuneDataContext cmuneDb)
        {
            List<int> itemIds = new List<int>();

            if (cmuneDb != null)
            {
                itemIds = cmuneDb.ItemToApplications.Where(iTA => iTA.ApplicationId == applicationId && iTA.ItemId >= CommonConfig.NewItemMallItemIdStart).Select(iTA => iTA.ItemId).ToList();
            }

            return itemIds;
        }

        /// <summary>
        /// Exact match
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetItemIdByName(string name)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int itemId = 0;

                name = name.Trim();

                var query = (from i in cmuneDb.Items
                             where i.Name == name
                             select new { ItemId = i.ItemId }).Take(1);

                foreach (var row in query)
                {
                    itemId = row.ItemId;
                }

                return itemId;
            }
        }

        /// <summary>
        /// Get an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Item GetItem(int itemId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetItem(itemId, cmuneDB);
            }
        }

        /// <summary>
        /// Gets an Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static Item GetItem(int itemId, CmuneDataContext cmuneDb)
        {
            Item item = null;

            if (cmuneDb != null)
            {
                Cache<Item> cachedItem = new Cache<Item>("Cmune.DataCenter.Business.GetItemByID." + itemId.ToString());

                if (cachedItem.IsInCache)
                {
                    item = cachedItem.Val;
                }
                else
                {
                    item = cmuneDb.Items.SingleOrDefault<Item>(f => f.ItemId == itemId);
                    if (item != null) { cachedItem.InsertHour((item)); }
                }
            }

            return item;
        }

        /// <summary>
        /// Get Item View
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static ItemView GetItemView(int itemId)
        {
            Item item = GetItem(itemId);
            return item.ToItemView();
        }

        /// <summary>
        /// Get the item types
        /// </summary>
        /// <returns></returns>
        public static List<ItemType> GetItemType()
        {
            List<ItemType> itemTypes = new List<ItemType>();

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                itemTypes = cmuneDB.ItemTypes.ToList();
            }

            return itemTypes;
        }

        #region Get item Class

        /// <summary>
        /// Gets all the Item Class
        /// </summary>
        /// <returns></returns>
        public static List<ItemClass> GetItemClass()
        {
            List<ItemClass> itemClass = new List<ItemClass>();

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                itemClass = cmuneDB.ItemClasses.ToList();
            }

            return itemClass;
        }

        /// <summary>
        /// Get the ItemClass ordered by ItemTypeId
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ItemClass>> GetItemClassOrdered()
        {
            Dictionary<int, List<ItemClass>> itemClassOrdered = new Dictionary<int, List<ItemClass>>();

            List<ItemClass> itemClasses = GetItemClass();

            foreach (ItemClass itemClass in itemClasses)
            {
                if (!itemClassOrdered.ContainsKey(itemClass.ItemTypeId))
                {
                    itemClassOrdered[itemClass.ItemTypeId] = new List<ItemClass>();
                }

                itemClassOrdered[itemClass.ItemTypeId].Add(itemClass);
            }

            return itemClassOrdered;
        }

        #endregion Get item Class

        /// <summary>
        /// Get all the items of an application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ItemView> GetShop(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = (from i in cmuneDb.Items
                                    join iA in cmuneDb.ItemToApplications on i.ItemId equals iA.ItemId
                                    where i.ItemId >= CommonConfig.NewItemMallItemIdStart && !i.IsDisabled && iA.ApplicationId == applicationId
                                    select i).ToList();

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get all the items of an application or all the items if applicationId is set to 0
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<ItemView> GetItems(int applicationId, CmuneDataContext cmuneDb = null)
        {
            if (cmuneDb == null)
                cmuneDb = new CmuneDataContext();

            using (cmuneDb)
            {
                List<Item> items = new List<Item>();

                if (applicationId == 0)
                {
                    items = cmuneDb.Items.Where(i => i.ItemId >= CommonConfig.NewItemMallItemIdStart).OrderByDescending(i => i.ItemId).ToList();
                }
                else
                {
                    var query = from i in cmuneDb.Items
                                join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                                where a.ApplicationId == applicationId && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                                orderby i.ItemId descending
                                select i;

                    items = query.ToList();
                }

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get a list of recommandable items
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<int> GetRecommendableItemsForMaps(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = from i in cmuneDb.Items
                            join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                            where a.ApplicationId == applicationId && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                            && !i.IsDisabled && i.IsForSale
                            orderby i.ItemId descending
                            select i.ItemId;

                return query.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ItemView> GetItemsWithMinimalPrice(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = from i in cmuneDb.Items
                            join a in cmuneDb.ItemToApplications on i.ItemId equals a.ItemId
                            where a.ApplicationId == applicationId && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                            && !i.IsDisabled
                            orderby i.ItemId descending
                            select i.ToItemView();

                return query.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="durationType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static int ComputePrice(BuyingDurationType durationType, int itemId)
        {
            var item = CmuneItem.GetItem(itemId);
            if (durationType == BuyingDurationType.Permanent)
            {
                if (item.PermanentCredits > 0)
                    return item.PermanentCredits;
            }
            else
            {
                if (item.CreditsPerDay > 0)
                    return CmuneEconomy.ComputeTemporaryPrice(durationType, item.CreditsPerDay);
            }
            return 0;
        }

        /// <summary>
        /// Get items based on their Ids
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        public static List<ItemView> GetItems(List<int> itemIds)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<Item> items = cmuneDB.Items.Where(i => itemIds.Contains(i.ItemId)).ToList();

                return ToItemView(items);
            }
        }

        /// <summary>
        /// Get the names of all the items of an application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="getDeprecatedItems"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetItemNames(int applicationId, bool getDeprecatedItems = false)
        {
            Dictionary<int, string> itemNames = new Dictionary<int, string>();

            using (var cmuneDb = new CmuneDataContext())
            {
                if (getDeprecatedItems)
                {
                    itemNames = (from i in cmuneDb.Items
                                 from iA in cmuneDb.ItemToApplications
                                 where i.ItemId == iA.ItemId && iA.ApplicationId == applicationId
                                 select new { ItemId = i.ItemId, Name = i.Name }).ToDictionary(i => i.ItemId, i => i.Name);
                }
                else
                {
                    itemNames = (from i in cmuneDb.Items
                                 from iA in cmuneDb.ItemToApplications
                                 where i.ItemId == iA.ItemId && iA.ApplicationId == applicationId && i.ItemId >= CommonConfig.NewItemMallItemIdStart
                                 select new { ItemId = i.ItemId, Name = i.Name }).ToDictionary(i => i.ItemId, i => i.Name);
                }
            }

            return itemNames;
        }

        /// <summary>
        /// Get names
        /// </summary>
        /// <param name="itemsId"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetItemNames(List<int> itemsId)
        {
            using (var cmuneDb = new CmuneDataContext())
            {
                Dictionary<int, string> itemNames = (from i in cmuneDb.Items
                                                     where itemsId.Contains(i.ItemId)
                                                     select new { ItemId = i.ItemId, Name = i.Name }).ToDictionary(i => i.ItemId, i => i.Name);
                return itemNames;
            }
        }

        #endregion Getters


        /// <summary>
        /// Converts a List of Item
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<ItemView> ToItemView(List<Item> items)
        {
            List<ItemView> itemsView = new List<ItemView>();

            if (items != null)
            {
                itemsView = items.ConvertAll(new Converter<Item, ItemView>(i => i.ToItemView()));
            }

            return itemsView;
        }

        /// <summary>
        /// Update an item
        /// </summary>
        /// <param name="itemView"></param>
        /// <returns></returns>
        public static bool ModifyItem(ItemView itemView)
        {
            bool isUpdated = false;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                if (itemView != null)
                {
                    Item itemToUpdate = cmuneDB.Items.SingleOrDefault<Item>(f => f.ItemId == itemView.ItemId);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.CopyFromItemView(itemView);

                        cmuneDB.SubmitChanges();

                        isUpdated = true;
                    }
                }
            }

            return isUpdated;
        }

        /// <summary>
        /// convert a dictionary of custom properties to string
        /// </summary>
        /// <param name="customProperties"></param>
        /// <returns></returns>
        public static string ConvertCustomPropertiesToString(Dictionary<string, string> customProperties)
        {
            StringBuilder str = new StringBuilder();

            int i = 0;
            foreach (var prop in customProperties)
            {
                str.Append(prop.Key);
                str.Append("=");
                str.Append(prop.Value);
                i++;
                if (i < customProperties.Count)
                {
                    str.Append("&");
                }
            }
            return str.ToString();
        }

        static Dictionary<string, string> ConvertStringToProperties(string customProperties)
        {
            Dictionary<string, string> customPropertiesToReturn = new Dictionary<string, string>();

            if (customProperties != null)
            {
                var splittedProperties = customProperties.Split('&');
                if (!String.IsNullOrEmpty(splittedProperties[0]) && splittedProperties.Count() > 0)
                {
                    foreach (var prop in splittedProperties)
                    {
                        var splittedProp = prop.Split('=');
                        if (!String.IsNullOrEmpty(splittedProp[0]) && splittedProp.Count() > 0)
                            customPropertiesToReturn.Add(splittedProp[0], splittedProp[1]);
                    }
                }
            }
            return customPropertiesToReturn;
        }

        /// <summary>
        /// Create a default instance of ItemView
        /// </summary>
        /// <returns></returns>
        public static ItemView CreateDefaultInstance()
        {
            return new ItemView()
            {
                ItemId = 0,
                TypeId = (int)ShopItemType.UberstrikeWeapon,

                // name description
                Name = string.Empty,
                Description = string.Empty,

                // type setting
                IsForSale = true,
                MaximumDurationDays = CommonConfig.ItemMallFieldDisable,

                // feature setting
                IsFeatured = false,
                IsNew = false,
                IsPopular = false,

                // shop setting
                CreditsPerDay = CommonConfig.ItemMallFieldDisable,
                PointsPerDay = CommonConfig.ItemMallFieldDisable,
                PermanentCredits = CommonConfig.ItemMallFieldDisable,
                PermanentPoints = CommonConfig.ItemMallFieldDisable,
                AmountRemaining = CommonConfig.ItemMallFieldDisable,

                // pack setting
                PackOneAmount = CommonConfig.ItemMallFieldDisable,
                PackTwoAmount = CommonConfig.ItemMallFieldDisable,
                PackThreeAmount = CommonConfig.ItemMallFieldDisable,
                MaximumOwnableAmount = CommonConfig.ItemMallFieldDisable,

                // day setting
                Enable1Day = true,
                Enable7Days = true,
                Enable30Days = true,
                Enable90Days = true,

                CustomProperties = new Dictionary<string, string>(),
            };
        }


        /// <summary>
        /// Create a new Item
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item CreateItem(int applicationId, ItemView item)
        {
            Item newItem = null;

            if (item != null)
            {
                using (CmuneDataContext CmuneDbCenter = new CmuneDataContext())
                {
                    newItem = item.ToItem();
                    newItem.AmountRemaining = CommonConfig.ItemMallFieldDisable;
                    newItem.IsDisabled = false;

                    if (newItem.MaximumDurationDays != CommonConfig.ItemMallFieldDisable && (newItem.MaximumDurationDays < CommonConfig.ItemMinimumDurationInDays || newItem.MaximumDurationDays > CommonConfig.ItemMinimumDurationInDays))
                    {
                        newItem.MaximumDurationDays = CommonConfig.ItemMallFieldDisable;
                    }

                    CmuneDbCenter.Items.InsertOnSubmit(newItem);
                    CmuneDbCenter.SubmitChanges();

                    ItemToApplication newItemMiniverse = new ItemToApplication();
                    newItemMiniverse.ItemId = newItem.ItemId;
                    newItemMiniverse.ApplicationId = applicationId;

                    CmuneDbCenter.ItemToApplications.InsertOnSubmit(newItemMiniverse);
                    CmuneDbCenter.SubmitChanges();
                }
            }

            return newItem;
        }

        /// <summary>
        /// Apply the same discount to all the items that are in sale. Only one day and permanent duration are available
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="discount">in %: 30 means a 30% discount</param>
        /// <returns></returns>
        public static bool ApplyGlobalDiscount(int applicationId, int discount)
        {
            bool isDiscountApplied = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Item> items = (from i in cmuneDb.Items
                                    join iA in cmuneDb.ItemToApplications on i.ItemId equals iA.ItemId
                                    where iA.ApplicationId == applicationId && i.IsForSale && i.ItemId > CommonConfig.NewItemMallItemIdStart
                                    select i).ToList();

                foreach (Item item in items)
                {

                    if (item.PointsPerDay != CommonConfig.ItemMallFieldDisable)
                    {
                        item.PointsPerDay = (int)((decimal)item.PointsPerDay - (decimal)item.PointsPerDay * (decimal)discount / 100m);
                    }

                    if (item.CreditsPerDay != CommonConfig.ItemMallFieldDisable)
                    {
                        item.CreditsPerDay = (int)((decimal)item.CreditsPerDay - (decimal)item.CreditsPerDay * (decimal)discount / 100m);
                    }

                    if (item.PermanentPoints != CommonConfig.ItemMallFieldDisable)
                    {
                        item.PermanentPoints = (int)((decimal)item.PermanentPoints - (decimal)item.PermanentPoints * (decimal)discount / 100m);
                    }

                    if (item.PermanentCredits != CommonConfig.ItemMallFieldDisable)
                    {
                        item.PermanentCredits = (int)((decimal)item.PermanentCredits - (decimal)item.PermanentCredits * (decimal)discount / 100m);
                    }

                    item.Enable1Day = true;
                    item.Enable7Days = false;
                    item.Enable30Days = false;
                    item.Enable90Days = false;
                    item.MaximumDurationDays = 1;
                }

                cmuneDb.SubmitChanges();
                isDiscountApplied = true;
            }

            return isDiscountApplied;
        }

        /// <summary>
        /// The replacement item has to be from the same type and class than the deprecated item
        /// </summary>
        /// <param name="itemIdToDisable">This item will be deprecated</param>
        /// <param name="replacementItemId">This item will replace the deprecated one in the member inventory</param>
        /// <returns></returns>
        public static bool DisableItem(int itemIdToDisable, int replacementItemId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isDisable = false;

                Item itemToDisable = GetItem(itemIdToDisable, cmuneDb);

                var replacementItem = (from i in cmuneDb.Items
                                       where i.ItemId == replacementItemId
                                       select new { Class = i.ClassId, @Type = i.TypeId }).SingleOrDefault();

                if (itemToDisable != null && replacementItem != null &&
                    itemToDisable.ClassId == replacementItem.Class &&
                    itemToDisable.TypeId == replacementItem.Type)
                {
                    string sqlTemplate = "UPDATE [Cmune].[dbo].[ItemInventory] SET [ItemId] = {0} WHERE [ItemId] = {1}";
                    cmuneDb.ExecuteCommand(sqlTemplate, replacementItemId, itemIdToDisable);

                    itemToDisable.IsDisabled = true;
                    cmuneDb.SubmitChanges();

                    isDisable = true;
                }

                return isDisable;
            }
        }

        /// <summary>
        /// delete the item from all inventories and delete the item
        /// </summary>
        /// <param name="itemIdToDisable"></param>
        /// <returns></returns>
        public static bool DeprecateItem(int itemIdToDisable)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isDisable = false;

                Item itemToDisable = GetItem(itemIdToDisable, cmuneDb);

                string sqlTemplate = "DELETE [Cmune].[dbo].[ItemInventory] WHERE [ItemId] = {0}";
                cmuneDb.ExecuteCommand(sqlTemplate, itemIdToDisable);

                itemToDisable.IsDisabled = true;
                cmuneDb.SubmitChanges();

                isDisable = true;

                return isDisable;
            }
        }

        /// <summary>
        /// Returns only the Item Ids that are existing
        /// </summary>
        /// <param name="itemsId"></param>
        /// <returns></returns>
        public static List<int> GetExistingItemsId(List<int> itemsId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<int> existingItemIds = cmuneDb.Items.Where(i => itemsId.Contains(i.ItemId)).Select(i => i.ItemId).ToList();

                return existingItemIds;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool IsItemOnSale(int itemId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isOnSale = false;

                Item item = GetItem(itemId, cmuneDb);

                if (item != null)
                {
                    isOnSale = !item.IsDisabled && item.IsForSale;
                }

                return isOnSale;
            }
        }
    }
}