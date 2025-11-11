using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Create and Manage Bundle
    /// </summary>
    public static class CmuneBundle
    {
        private static BundleView ToBundleView(Bundle bundle)
        {
            var bundleView = new BundleView();
            bundleView.Id = bundle.Id;
            bundleView.Name = bundle.Name;
            bundleView.ImageUrl = bundle.ImageUrl;
            bundleView.IconUrl = bundle.IconUrl;
            bundleView.Description = bundle.Description;
            bundleView.IsOnSale = bundle.IsOnSale;
            bundleView.IsPromoted = bundle.IsPromoted;
            bundleView.USDPrice = bundle.USDPrice;
            bundleView.USDPromoPrice = bundle.USDPromoPrice;
            bundleView.Points = bundle.Points;
            bundleView.Credits = bundle.Credits;
            bundleView.Category = (BundleCategoryType)bundle.Category;
            bundleView.PromotionTag = bundle.PromotionTag;
            bundleView.MacAppStoreUniqueId = bundle.MacAppStoreUniqueId;
            bundleView.IosAppStoreUniqueId = bundle.IosAppStoreUniqueId;
            bundleView.IsDefault = bundle.IsDefault;
            return bundleView;
        }

        /// <summary>
        /// Get all bundle Packs on sale
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<BundleView> GetAllBundlesOnSaleView(ChannelType channel)
        {
            List<BundleView> bundleViews = new List<BundleView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundles = from b in cmuneDb.Bundles
                              join a in cmuneDb.BundlesAvailabilities on b.Id equals a.BundleId
                              where b.IsOnSale && a.Channel == (int)channel
                              select b;

                var bundlesItems = GetBundleItems(bundles.Select(b => b.Id).ToList());

                foreach (var bundle in bundles)
                {
                    var bundleView = ToBundleView(bundle);
                    bundleView.BundleItemViews = bundlesItems.Where(b => b.BundleId == bundle.Id).ToList();
                    bundleViews.Add(bundleView);
                }
            }

            return bundleViews;
        }

        /// <summary>
        /// Get all bundle Packs on sale
        /// </summary>
        /// <returns></returns>
        public static List<BundleView> GetAllBundlesOnSaleView()
        {
            List<BundleView> bundleViews = new List<BundleView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundles = from b in cmuneDb.Bundles
                              where b.IsOnSale
                              select b;

                var bundlesItems = GetBundleItems(bundles.Select(b => b.Id).ToList());
                Dictionary<int, List<ChannelType>> bundlesAvailability = GetBundleAvailability(bundles.Select(b => b.Id).ToList());

                foreach (var bundle in bundles)
                {
                    var bundleView = ToBundleView(bundle);
                    bundleView.BundleItemViews = bundlesItems.Where(b => b.BundleId == bundle.Id).ToList();

                    bundleView.Availability = new List<ChannelType>();

                    if (bundlesAvailability.ContainsKey(bundle.Id))
                    {
                        bundleView.Availability = bundlesAvailability[bundle.Id];
                    }

                    bundleViews.Add(bundleView);
                }
            }

            return bundleViews.OrderBy(d => d.Credits).ToList();
        }

        /// <summary>
        /// Get all bundles
        /// </summary>
        /// <returns></returns>
        public static List<BundleView> GetAllBundlesView()
        {
            List<BundleView> bundleViews = new List<BundleView>();

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundles = cmuneDbCenter.Bundles;
                var bundlesItems = GetAllBundleItems();
                Dictionary<int, List<ChannelType>> bundlesAvailability = GetAllBundleAvailability();

                foreach (var bundle in bundles)
                {
                    var bundleView = ToBundleView(bundle);
                    bundleView.BundleItemViews = bundlesItems.Where(b => b.BundleId == bundle.Id).ToList();
                    bundleView.Availability = new List<ChannelType>();

                    if (bundlesAvailability.ContainsKey(bundle.Id))
                    {
                        bundleView.Availability = bundlesAvailability[bundle.Id];
                    }

                    bundleViews.Add(bundleView);
                }
            }

            return bundleViews.OrderBy(d => d.Credits).ToList();
        }

        /// <summary>
        /// Get bundleId with its MacAppStoreUniqueId
        /// </summary>
        /// <param name="uniqueBundleId"></param>
        /// <returns></returns>
        public static int GetBundleIdByMacAppStoreUniqueId(string uniqueBundleId)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                return cmuneDbCenter.Bundles.Where(d => d.MacAppStoreUniqueId == uniqueBundleId).Select(d => d.Id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get bundleId with its MacAppStoreUniqueId
        /// </summary>
        /// <param name="uniqueBundleId"></param>
        /// <returns></returns>
        public static int GetBundleIdByIosAppStoreUniqueId(string uniqueBundleId)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                return cmuneDbCenter.Bundles.Where(d => d.IosAppStoreUniqueId == uniqueBundleId).Select(d => d.Id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get the bundle's MacAppStoreUniqueId by bundleId
        /// </summary>
        /// <param name="bundleId"></param>
        public static string GetMacAppStoreUniqueId(int bundleId)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                return cmuneDbCenter.Bundles.Where(d => d.Id == bundleId).Select(d => d.MacAppStoreUniqueId).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get the bundle's IosAppStoreUniqueId by bundleId
        /// </summary>
        /// <param name="bundleId"></param>
        public static string GetIosAppStoreUniqueId(int bundleId)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                return cmuneDbCenter.Bundles.Where(d => d.Id == bundleId).Select(d => d.IosAppStoreUniqueId).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static BundleView GetBundleView(int bundleId)
        {
            BundleView bundleView = null;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundle = cmuneDbCenter.Bundles.SingleOrDefault<Bundle>(d => d.Id == bundleId);

                if (bundle != null)
                {
                    bundleView = new BundleView();
                    bundleView = ToBundleView(bundle);
                    bundleView.Availability = GetBundleAvailability(bundleId);
                    bundleView.BundleItemViews = GetBundleItems(bundle.Id);
                }
            }

            return bundleView;
        }

        /// <summary>
        /// Get a default bundle id
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultBundleId(bool isCreditBundle)
        {
            int bundleId = 0;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundles = cmuneDbCenter.Bundles.Where(d => d.IsDefault == true).ToList();
                foreach (var bundle in bundles)
                {
                    if (isCreditBundle && bundle.Credits > 0)
                    {
                        bundleId = bundle.Id;
                    }
                    else if (!isCreditBundle && bundle.Credits == 0)
                    {
                        bundleId = bundle.Id;
                    }
                }

            }
            return bundleId;
        }

        /// <summary>
        /// Get a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static BundleView GetBundleOnSaleWithoutItemView(int bundleId, ChannelType channel)
        {
            BundleView bundleView = null;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundle = (from b in cmuneDbCenter.Bundles
                              join a in cmuneDbCenter.BundlesAvailabilities on b.Id equals a.BundleId
                              where b.Id == bundleId && b.IsOnSale && a.Channel == (int)channel
                              select b).SingleOrDefault();

                if (bundle != null)
                {
                    bundleView = new BundleView();
                    bundleView = ToBundleView(bundle);
                }
            }

            return bundleView;
        }

        /// <summary>
        /// Get a bundle and its items
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static BundleView GetBundleOnSaleView(int bundleId, ChannelType channel)
        {
            BundleView bundleView = null;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundle = (from b in cmuneDbCenter.Bundles
                              join a in cmuneDbCenter.BundlesAvailabilities on b.Id equals a.BundleId
                              where b.Id == bundleId && b.IsOnSale && a.Channel == (int)channel
                              select b).SingleOrDefault();

                if (bundle != null)
                {
                    bundleView = new BundleView();
                    bundleView = ToBundleView(bundle);
                    bundleView.BundleItemViews = GetBundleItems(bundle.Id);
                }
            }

            return bundleView;
        }

        /// <summary>
        /// Create a bundle
        /// </summary>
        /// <param name="bundleView"></param>
        /// <returns></returns>
        public static BundleOperationResult CreateBundle(BundleView bundleView)
        {
            int bundleId;
            var bundleOperationResult = CreateBundle(bundleView.Name, bundleView.ImageUrl, bundleView.IconUrl, bundleView.Description, bundleView.USDPrice, bundleView.IsOnSale, bundleView.IsPromoted, bundleView.USDPromoPrice, bundleView.Points, bundleView.Credits, bundleView.Category, bundleView.PromotionTag, bundleView.MacAppStoreUniqueId, bundleView.IosAppStoreUniqueId, bundleView.IsDefault, out bundleId);
            bundleView.Id = bundleId;
            return bundleOperationResult;
        }

        /// <summary>
        ///  Create a bundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pictureName"></param>
        /// <param name="logoName"></param>
        /// <param name="description"></param>
        /// <param name="usdPrice"></param>
        /// <param name="isOnSale"></param>
        /// <param name="isPromoted"></param>
        /// <param name="usdPromoPrice"></param>
        /// <param name="points"></param>
        /// <param name="credits"></param>
        /// <param name="category"></param>
        /// <param name="promotionTag"></param>
        /// <param name="masUniqueId"></param>
        /// <param name="isDefault"></param>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static BundleOperationResult CreateBundle(string name, string pictureName, string logoName, string description, decimal usdPrice, bool isOnSale, bool isPromoted, decimal usdPromoPrice, int points, int credits, BundleCategoryType category, string promotionTag, string masUniqueId, string iosUniqueId, bool isDefault, out int bundleId)
        {
            BundleOperationResult result = BundleOperationResult.Error;
            bundleId = 0;

            if (IsDuplicateUniqueId(masUniqueId))
            {
                result = BundleOperationResult.DuplicateUniqueId;
            }

            if (result == BundleOperationResult.Error)
            {
                using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
                {
                    Bundle bundle = new Bundle();
                    bundle.Name = name;
                    bundle.IconUrl = (logoName == null) ? "" : logoName;
                    bundle.ImageUrl = (pictureName == null) ? "" : pictureName;
                    bundle.Description = description;
                    bundle.USDPrice = usdPrice;
                    bundle.IsOnSale = isOnSale;
                    bundle.IsPromoted = isPromoted;
                    bundle.USDPromoPrice = usdPromoPrice;
                    bundle.Points = points;
                    bundle.Credits = credits;
                    bundle.Category = (int)category;
                    bundle.PromotionTag = (promotionTag == null) ? "" : promotionTag;
                    bundle.MacAppStoreUniqueId = (masUniqueId == null) ? "" : masUniqueId;
                    bundle.IosAppStoreUniqueId = (iosUniqueId == null) ? "" : iosUniqueId;
                    bundle.IsDefault = isDefault;

                    cmuneDbCenter.Bundles.InsertOnSubmit(bundle);
                    cmuneDbCenter.SubmitChanges();

                    bundleId = bundle.Id;
                    result = BundleOperationResult.Ok;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if a UniqueId is already in use
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="exceptBundleId"></param>
        /// <returns></returns>
        public static bool IsDuplicateUniqueId(string uniqueId, int? exceptBundleId = null)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (exceptBundleId.HasValue)
                    return (cmuneDb.Bundles.Where(b => b.MacAppStoreUniqueId == uniqueId && b.Id != exceptBundleId).ToList().Count() > 0);
                else
                    return (cmuneDb.Bundles.Where(b => b.MacAppStoreUniqueId == uniqueId).ToList().Count() > 0);
            }
        }

        /// <summary>
        /// Delete a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static bool DeleteBundle(int bundleId)
        {
            bool success = false;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                var bundle = cmuneDbCenter.Bundles.Where(d => d.Id == bundleId).FirstOrDefault();

                if (bundle != null)
                {
                    DeleteBundleItems(bundle.Id);
                    DeleteBundleAvailability(bundleId);
                    cmuneDbCenter.Bundles.DeleteOnSubmit(bundle);
                    cmuneDbCenter.SubmitChanges();
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// update a bundle
        /// </summary>
        /// <param name="bundleView"></param>
        /// <returns></returns>
        public static BundleOperationResult UpdateBundle(BundleView bundleView)
        {
            return UpdateBundle(bundleView.Id, bundleView.Name, bundleView.IconUrl, bundleView.ImageUrl, bundleView.Description, bundleView.USDPrice, bundleView.IsOnSale, bundleView.IsPromoted, bundleView.USDPromoPrice, bundleView.Points, bundleView.Credits, bundleView.Category, bundleView.PromotionTag, bundleView.MacAppStoreUniqueId, bundleView.IosAppStoreUniqueId, bundleView.IsDefault);
        }

        /// <summary>
        /// Update a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="name"></param>
        /// <param name="logoName"></param>
        /// <param name="pictureName"></param>
        /// <param name="description"></param>
        /// <param name="usdPrice"></param>
        /// <param name="isOnSale"></param>
        /// <param name="isPromoted"></param>
        /// <param name="usdPromoPrice"></param>
        /// <param name="points"></param>
        /// <param name="credits"></param>
        /// <param name="category"></param>
        /// <param name="promotionTag"></param>
        /// <param name="masUniqueId"></param>
        /// <param name="isDefault"></param>
        /// <param name="iosUniqueId"></param>
        /// <returns></returns>
        public static BundleOperationResult UpdateBundle(int bundleId, string name, string logoName, string pictureName, string description, decimal usdPrice, bool isOnSale, bool isPromoted, decimal usdPromoPrice, int points, int credits, BundleCategoryType category, string promotionTag, string masUniqueId, string iosUniqueId, bool isDefault)
        {
            BundleOperationResult result = BundleOperationResult.Error;

            if (IsDuplicateUniqueId(masUniqueId, bundleId))
            {
                result = BundleOperationResult.DuplicateUniqueId;
            }

            if (result == BundleOperationResult.Error)
            {
                using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
                {
                    var bundle = cmuneDbCenter.Bundles.SingleOrDefault<Bundle>(d => d.Id == bundleId);

                    if (bundle != null)
                    {
                        bundle.Name = name;
                        bundle.IconUrl = (logoName == null) ? "" : logoName;
                        bundle.ImageUrl = (pictureName == null) ? "" : pictureName;
                        bundle.Description = description;
                        bundle.IsOnSale = isOnSale;
                        bundle.IsPromoted = isPromoted;
                        bundle.USDPrice = usdPrice;
                        bundle.USDPromoPrice = usdPromoPrice;
                        bundle.Points = points;
                        bundle.Credits = credits;
                        bundle.Category = (int)category;
                        bundle.PromotionTag = (promotionTag == null) ? "" : promotionTag;
                        bundle.MacAppStoreUniqueId = (masUniqueId == null) ? "" : masUniqueId;
                        bundle.IosAppStoreUniqueId = (iosUniqueId == null) ? "" : iosUniqueId;
                        bundle.IsDefault = isDefault;

                        cmuneDbCenter.SubmitChanges();

                        result = BundleOperationResult.Ok;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// set the default value to false for other bundles except exceptBundleId's one regarding its type
        /// </summary>
        /// <returns></returns>
        public static void UnsetIsDefaultForBundles(int exceptBundleId)
        {
            var bundles = GetAllBundlesView();
            var noEvalBundle = bundles.Where(d => d.Id == exceptBundleId).First();
            bundles.Remove(noEvalBundle);
            if (noEvalBundle.IsDefault) // evaluate the other bundles only if IsDefault is set
            {
                using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
                {
                    bool save = false;
                    bool isCredit = (noEvalBundle.Credits > 0);
                    if (isCredit) // get credit bundle
                        bundles = bundles.Where(d => d.Credits > 0).ToList();
                    else // get item/point bundle
                        bundles = bundles.Where(d => d.Credits == 0).ToList();

                    foreach (var bundleView in bundles)
                    {
                        var bundle = cmuneDbCenter.Bundles.Where(d => d.Id == bundleView.Id).First();
                        bundle.IsDefault = false;
                        save = true;
                    }
                    if (save == true)
                    {
                        cmuneDbCenter.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Add items to a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="bundleItemsView"></param>
        /// <returns></returns>
        public static bool AddBundleItems(int bundleId, List<BundleItemView> bundleItemsView)
        {
            bool success = false;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                if (bundleItemsView != null && bundleItemsView.Count > 0)
                {
                    var listBundleItems = new List<BundleItem>();

                    List<int> existingItemIds = CmuneItem.GetExistingItemsId(bundleItemsView.Select(b => b.ItemId).ToList());

                    foreach (var item in bundleItemsView)
                    {
                        if (existingItemIds.Contains(item.ItemId))
                        {
                            var bundleItem = new BundleItem() { BundleId = bundleId, Duration = (int)item.Duration, ItemId = item.ItemId };
                            listBundleItems.Add(bundleItem);
                        }
                    }

                    if (listBundleItems.Count > 0)
                    {
                        cmuneDbCenter.BundleItems.InsertAllOnSubmit(listBundleItems);
                        cmuneDbCenter.SubmitChanges();
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// delete items in a bundle
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="itemIds">if null => delete all items</param>
        /// <returns></returns>
        public static bool DeleteBundleItems(int bundleId, List<int> itemIds = null)
        {
            bool success = false;

            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                if (bundleId > 0)
                {
                    var bundleItems = new List<BundleItem>();

                    if (itemIds != null)
                    {
                        bundleItems = cmuneDbCenter.BundleItems.Where(d => itemIds.Contains(d.ItemId) && d.BundleId == bundleId).ToList();
                    }
                    else
                    {
                        bundleItems = cmuneDbCenter.BundleItems.Where(d => d.BundleId == bundleId).ToList();
                    }

                    foreach (var bundleItem in bundleItems)
                    {
                        if (bundleItem != null)
                        {
                            cmuneDbCenter.BundleItems.DeleteOnSubmit(bundleItem);
                            cmuneDbCenter.SubmitChanges();
                            success = true;
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Remove a bundle from all channels
        /// </summary>
        /// <param name="bundleId"></param>
        public static void DeleteBundleAvailability(int bundleId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundleAvailability = cmuneDb.BundlesAvailabilities.Where(a => a.BundleId == bundleId);
                cmuneDb.BundlesAvailabilities.DeleteAllOnSubmit(bundleAvailability);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static List<BundleItemView> GetBundleItems(int bundleId)
        {
            var bundleItemViews = GetBundleItems(new List<int> { bundleId });

            return bundleItemViews;
        }

        /// <summary>
        /// Get the items contained by those bundles
        /// </summary>
        /// <param name="bundlesId"></param>
        /// <returns></returns>
        public static List<BundleItemView> GetBundleItems(List<int> bundlesId)
        {
            List<BundleItemView> bundleItemViews = new List<BundleItemView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundleItems = cmuneDb.BundleItems.Where(d => bundlesId.Contains(d.BundleId)).ToList();

                if (bundleItems.Count > 0)
                {
                    bundleItemViews = bundleItems.Select(d => new BundleItemView()
                    {
                        ItemId = d.ItemId,
                        BundleId = d.BundleId,
                        Amount = d.Amount,
                        Duration = (BuyingDurationType)d.Duration,
                    }).ToList();
                }
            }

            return bundleItemViews;
        }

        /// <summary>
        /// Get all the items linked to bundles
        /// </summary>
        /// <returns></returns>
        public static List<BundleItemView> GetAllBundleItems()
        {
            List<BundleItemView> bundleItemViews = new List<BundleItemView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundleItems = cmuneDb.BundleItems.ToList();

                if (bundleItems.Count > 0)
                {
                    bundleItemViews = bundleItems.Select(d => new BundleItemView()
                    {
                        ItemId = d.ItemId,
                        BundleId = d.BundleId,
                        Duration = (BuyingDurationType)d.Duration,
                    }).ToList();
                }
            }

            return bundleItemViews;
        }

        /// <summary>
        /// Get the channels where a bundles is available
        /// </summary>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static List<ChannelType> GetBundleAvailability(int bundleId)
        {
            List<ChannelType> bundleAvailability = new List<ChannelType>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var availabilities = cmuneDb.BundlesAvailabilities.Where(d => d.BundleId == bundleId).ToList();

                foreach (var availability in availabilities)
                {
                    bundleAvailability.Add((ChannelType)availability.Channel);
                }
            }

            return bundleAvailability;
        }

        /// <summary>
        /// Get the channels where bundles are available
        /// </summary>
        /// <param name="bundlesId"></param>
        /// <returns></returns>
        public static Dictionary<int, List<ChannelType>> GetBundleAvailability(List<int> bundlesId)
        {
            Dictionary<int, List<ChannelType>> bundleAvailability = new Dictionary<int, List<ChannelType>>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var availabilities = cmuneDb.BundlesAvailabilities.Where(d => bundlesId.Contains(d.BundleId)).ToList();

                foreach (var availability in availabilities)
                {
                    if (!bundleAvailability.ContainsKey(availability.BundleId))
                    {
                        bundleAvailability.Add(availability.BundleId, new List<ChannelType>());
                    }

                    bundleAvailability[availability.BundleId].Add((ChannelType)availability.Channel);
                }
            }

            return bundleAvailability;
        }

        /// <summary>
        /// Get the available channels for all the bundles
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ChannelType>> GetAllBundleAvailability()
        {
            Dictionary<int, List<ChannelType>> bundleAvailability = new Dictionary<int, List<ChannelType>>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var availabilities = cmuneDb.BundlesAvailabilities;

                foreach (var availability in availabilities)
                {
                    if (!bundleAvailability.ContainsKey(availability.BundleId))
                    {
                        bundleAvailability.Add(availability.BundleId, new List<ChannelType>());
                    }

                    bundleAvailability[availability.BundleId].Add((ChannelType)availability.Channel);
                }
            }

            return bundleAvailability;
        }

        /// <summary>
        /// Set the channels where a bundle is available
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="channels"></param>
        public static void DefineAvailableChannels(int bundleId, List<ChannelType> channels)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<int> newChannels = channels.ConvertAll(new System.Converter<ChannelType, int>(c => (int)c));

                List<BundlesAvailability> bundlesAvailability = cmuneDb.BundlesAvailabilities.Where(a => a.BundleId == bundleId).ToList();
                List<BundlesAvailability> bundlesAvailabilityToDelete = bundlesAvailability.Where(a => !newChannels.Contains(a.Channel)).ToList();
                List<int> availabilitiesToAdd = newChannels.Where(c => !bundlesAvailability.Select(a => a.Channel).Contains(c)).ToList();

                cmuneDb.BundlesAvailabilities.DeleteAllOnSubmit(bundlesAvailabilityToDelete);

                List<BundlesAvailability> bundlesAvailabilityToInsert = new List<BundlesAvailability>();

                foreach (int availabilityToAdd in availabilitiesToAdd)
                {
                    bundlesAvailabilityToInsert.Add(new BundlesAvailability { BundleId = bundleId, Channel = availabilityToAdd });
                }

                cmuneDb.BundlesAvailabilities.InsertAllOnSubmit(bundlesAvailabilityToInsert);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Get the bundle names
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetBundlesName()
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundlesName = from b in cmuneDb.Bundles
                                  select new { BundleId = b.Id, BundleName = b.Name };

                return bundlesName.ToDictionary(b => b.BundleId, b => b.BundleName);
            }
        }

        /// <summary>
        /// Get the bundle USD price
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, decimal> GetBundlesUsdPrice()
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var bundlesUsdPrice = from b in cmuneDb.Bundles
                                      select new { BundleId = b.Id, BundleUsdPrice = b.USDPrice };

                return bundlesUsdPrice.ToDictionary(b => b.BundleId, b => b.BundleUsdPrice);
            }
        }
    }
}