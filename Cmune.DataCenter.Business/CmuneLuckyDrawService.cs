using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manage Lucky Draw
    /// </summary>
    public static class CmuneLuckyDrawService
    {
        #region LuckyDraw ViewModel

        public static LuckyDrawView ToLuckyDrawView(this LuckyDraw luckyDraw)
        {
            if (luckyDraw == null)
                return null;
            var luckyDrawView = new LuckyDrawView();

            luckyDrawView.Category = (BundleCategoryType)luckyDraw.Category;
            luckyDrawView.Description = luckyDraw.Description;
            luckyDrawView.IconUrl = luckyDraw.IconUrl;
            luckyDrawView.Id = luckyDraw.Id;
            luckyDrawView.IsAvailableInShop = luckyDraw.IsAvailableInShop;
            luckyDrawView.Name = luckyDraw.Name;
            if (luckyDraw.PriceInCredits > 0)
            {
                luckyDrawView.Price = luckyDraw.PriceInCredits;
                luckyDrawView.UberStrikeCurrencyType = UberStrikeCurrencyType.Credits;
            }

            if (luckyDraw.PriceInPoints > 0)
            {
                luckyDrawView.Price = luckyDraw.PriceInPoints;
                luckyDrawView.UberStrikeCurrencyType = UberStrikeCurrencyType.Points;
            }

            luckyDrawView.Id = luckyDraw.Id;

            return luckyDrawView;
        }

        public static LuckyDraw ToLuckyDraw(this LuckyDrawView luckyDrawView)
        {
            if (luckyDrawView == null)
                return null;
            var luckyDraw = new LuckyDraw();

            luckyDraw.CopyFromLuckyDrawView(luckyDrawView);
            luckyDraw.Id = luckyDrawView.Id;

            return luckyDraw;
        }

        public static void CopyFromLuckyDrawView(this LuckyDraw luckyDraw, LuckyDrawView luckyDrawView)
        {
            luckyDraw.Category = (int)luckyDrawView.Category;
            luckyDraw.Description = luckyDrawView.Description != null ? luckyDrawView.Description : "";
            luckyDraw.IconUrl = luckyDrawView.IconUrl != null ? luckyDrawView.IconUrl : "";
            luckyDraw.Id = luckyDrawView.Id;
            luckyDraw.IsAvailableInShop = luckyDrawView.IsAvailableInShop;
            luckyDraw.Name = luckyDrawView.Name != null ? luckyDrawView.Name : "";
            if (luckyDrawView.UberStrikeCurrencyType == UberStrikeCurrencyType.Points)
            {
                luckyDraw.PriceInPoints = luckyDrawView.Price;
                luckyDraw.PriceInCredits = 0;
            }
            if (luckyDrawView.UberStrikeCurrencyType == UberStrikeCurrencyType.Credits)
            {
                luckyDraw.PriceInCredits = luckyDrawView.Price;
                luckyDraw.PriceInPoints = 0;
            }
        }

        public static List<LuckyDrawView> ToLuckyDrawViews(this IEnumerable<LuckyDraw> luckydraws)
        {
            if (luckydraws == null)
                return null;
            return (from ld in luckydraws select ld.ToLuckyDrawView()).ToList();
        }

        #endregion

        #region LuckyDrawSet ViewModel

        public static LuckyDrawSetView ToLuckyDrawSetView(this LuckyDrawSet luckyDrawSet)
        {
            if (luckyDrawSet == null)
                return null;

            var luckyDrawSetView = new LuckyDrawSetView();
            luckyDrawSetView.CreditsAttributed = luckyDrawSet.CreditsAttributed;
            luckyDrawSetView.ExposeItemsToPlayers = luckyDrawSet.ExposeItemsToPlayers;
            luckyDrawSetView.Id = luckyDrawSet.Id;
            luckyDrawSetView.ImageUrl = luckyDrawSet.ImageUrl;
            luckyDrawSetView.LuckyDrawId = luckyDrawSet.LuckyDrawId;
            luckyDrawSetView.PointsAttributed = luckyDrawSet.PointsAttributed;
            luckyDrawSetView.SetWeight = luckyDrawSet.SetWeight;
            return luckyDrawSetView;
        }

        public static LuckyDrawSet ToLuckyDrawSet(this LuckyDrawSetView luckyDrawSetView)
        {
            if (luckyDrawSetView == null)
                return null;
            var luckyDrawSet = new LuckyDrawSet();
            luckyDrawSet.CopyFromLuckyDrawSetView(luckyDrawSetView);
            luckyDrawSet.Id = luckyDrawSetView.Id;

            return luckyDrawSet;
        }

        public static void CopyFromLuckyDrawSetView(this LuckyDrawSet luckyDrawSet, LuckyDrawSetView luckyDrawSetView)
        {
            luckyDrawSet.CreditsAttributed = luckyDrawSetView.CreditsAttributed;
            luckyDrawSet.ExposeItemsToPlayers = luckyDrawSetView.ExposeItemsToPlayers;
            luckyDrawSet.ImageUrl = luckyDrawSetView.ImageUrl != null ? luckyDrawSetView.ImageUrl : "";
            luckyDrawSet.LuckyDrawId = luckyDrawSetView.LuckyDrawId;
            luckyDrawSet.PointsAttributed = luckyDrawSetView.PointsAttributed;
            luckyDrawSet.SetWeight = luckyDrawSetView.SetWeight;
        }

        public static List<LuckyDrawSet> ToLuckyDrawSets(this IEnumerable<LuckyDrawSetView> luckyDrawSetView)
        {
            if (luckyDrawSetView == null)
                return null;

            var query = from lds in luckyDrawSetView
                        select lds.ToLuckyDrawSet();
            return query.ToList();
        }

        public static List<LuckyDrawSetView> ToLuckyDrawSetViews(this IEnumerable<LuckyDrawSet> luckyDrawSets)
        {
            if (luckyDrawSets == null)
                return null;
            return (from lds in luckyDrawSets select lds.ToLuckyDrawSetView()).ToList();
        }

        #endregion

        #region LuckyDrawSetItem ViewModel

        public static LuckyDrawSetItemView ToLuckyDrawSetItemView(this LuckyDrawSetItem luckyDrawSetItem)
        {
            return new LuckyDrawSetItemView()
            {
                DurationType = (BuyingDurationType)luckyDrawSetItem.DurationType,
                Amount = luckyDrawSetItem.Amount,
                Id = luckyDrawSetItem.Id,
                ItemId = luckyDrawSetItem.ItemId,
                LuckyDrawSetId = luckyDrawSetItem.LuckyDrawSetId,
            };
        }

        public static LuckyDrawSetItem ToLuckyDrawSetItem(this LuckyDrawSetItemView luckyDrawSetItemView)
        {
            var luckyDrawSetItem = new LuckyDrawSetItem();

            luckyDrawSetItem.CopyFromLuckyDrawSetItemView(luckyDrawSetItemView);
            luckyDrawSetItem.Id = luckyDrawSetItemView.Id;

            return luckyDrawSetItem;
        }

        public static void CopyFromLuckyDrawSetItemView(this LuckyDrawSetItem luckyDrawSetItem, LuckyDrawSetItemView luckyDrawSetItemView)
        {
            luckyDrawSetItem.Amount = luckyDrawSetItemView.Amount;
            luckyDrawSetItem.DurationType = (int)luckyDrawSetItemView.DurationType;
            luckyDrawSetItem.LuckyDrawSetId = luckyDrawSetItemView.LuckyDrawSetId;
            luckyDrawSetItem.ItemId = luckyDrawSetItemView.ItemId;
        }

        public static List<LuckyDrawSetItem> ToLuckyDrawSetItems(this IEnumerable<LuckyDrawSetItemView> luckyDrawSetItemView)
        {
            if (luckyDrawSetItemView == null)
                return null;
            return (from ldsi in luckyDrawSetItemView select ldsi.ToLuckyDrawSetItem()).ToList();
        }

        public static List<LuckyDrawSetItemView> ToLuckyDrawSetItemViews(this IEnumerable<LuckyDrawSetItem> luckyDrawSetItems)
        {
            if (luckyDrawSetItems == null)
                return null;
            return (from ldsi in luckyDrawSetItems select ldsi.ToLuckyDrawSetItemView()).ToList();
        }

        #endregion

        #region LuckyDrawUnityView

        public static LuckyDrawUnityView ToLuckyDrawUnityView(this LuckyDrawView luckyDrawView)
        {
            if (luckyDrawView == null)
                return null;

            return new LuckyDrawUnityView()
            {
                Category = (BundleCategoryType)luckyDrawView.Category,
                Description = luckyDrawView.Description,
                IconUrl = luckyDrawView.IconUrl,
                Id = luckyDrawView.Id,
                IsAvailableInShop = luckyDrawView.IsAvailableInShop,
                Name = luckyDrawView.Name,
                Price = luckyDrawView.Price,
                UberStrikeCurrencyType = luckyDrawView.UberStrikeCurrencyType,
                LuckyDrawSets = luckyDrawView.LuckyDrawSets.ToLuckyDrawSetUnityViews(),
            };
        }

        public static LuckyDrawView ToLuckyDrawView(this LuckyDrawUnityView luckyDrawUnityView)
        {
            if (luckyDrawUnityView == null)
                return null;
            var luckyDrawView = new LuckyDrawView();

            luckyDrawView.CopyFromLuckyDrawUnityView(luckyDrawUnityView);
            luckyDrawView.Id = luckyDrawUnityView.Id;
            return luckyDrawView;
        }

        public static void CopyFromLuckyDrawUnityView(this LuckyDrawView luckyDrawView, LuckyDrawUnityView luckyDrawUnityView)
        {
            luckyDrawView.Category = luckyDrawUnityView.Category;
            luckyDrawView.Description = luckyDrawUnityView.Description != null ? luckyDrawUnityView.Description : "";
            luckyDrawView.IconUrl = luckyDrawUnityView.IconUrl != null ? luckyDrawUnityView.IconUrl : "";
            luckyDrawView.Id = luckyDrawUnityView.Id;
            luckyDrawView.IsAvailableInShop = luckyDrawUnityView.IsAvailableInShop;
            luckyDrawView.Name = luckyDrawUnityView.Name != null ? luckyDrawUnityView.Name : "";
        }

        public static List<LuckyDrawUnityView> ToLuckyDrawUnityViews(this IEnumerable<LuckyDrawView> luckyDrawViews)
        {
            if (luckyDrawViews == null)
                return null;
            return (from lds in luckyDrawViews select lds.ToLuckyDrawUnityView()).ToList();
        }

        #endregion

        #region LuckyDrawSetUnityView

        public static LuckyDrawSetUnityView ToLuckyDrawSetUnityView(this LuckyDrawSetView luckyDrawSetView)
        {
            if (luckyDrawSetView == null)
                return null;

            var luckyDrawSetUnityView = new LuckyDrawSetUnityView();
            luckyDrawSetUnityView.CreditsAttributed = luckyDrawSetView.CreditsAttributed;
            luckyDrawSetUnityView.ExposeItemsToPlayers = luckyDrawSetView.ExposeItemsToPlayers;
            luckyDrawSetUnityView.Id = luckyDrawSetView.Id;
            luckyDrawSetUnityView.ImageUrl = luckyDrawSetView.ImageUrl;
            luckyDrawSetUnityView.LuckyDrawId = luckyDrawSetView.LuckyDrawId;
            luckyDrawSetUnityView.PointsAttributed = luckyDrawSetView.PointsAttributed;
            luckyDrawSetUnityView.SetWeight = luckyDrawSetView.SetWeight;
            luckyDrawSetUnityView.LuckyDrawSetItems = luckyDrawSetView.LuckyDrawSetItems.ToBundleItemViews();
            return luckyDrawSetUnityView;
        }

        public static LuckyDrawSetView ToLuckyDrawSet(this LuckyDrawSetUnityView luckyDrawSetUnityView)
        {
            if (luckyDrawSetUnityView == null)
                return null;
            var luckyDrawSetView = new LuckyDrawSetView();
            luckyDrawSetView.CopyFromLuckyDrawSetUnityView(luckyDrawSetUnityView);
            luckyDrawSetView.Id = luckyDrawSetUnityView.Id;

            return luckyDrawSetView;
        }

        public static void CopyFromLuckyDrawSetUnityView(this LuckyDrawSetView luckyDrawSetView, LuckyDrawSetUnityView luckyDrawSetUnityView)
        {
            luckyDrawSetView.CreditsAttributed = luckyDrawSetUnityView.CreditsAttributed;
            luckyDrawSetView.ExposeItemsToPlayers = luckyDrawSetUnityView.ExposeItemsToPlayers;
            luckyDrawSetView.ImageUrl = luckyDrawSetUnityView.ImageUrl != null ? luckyDrawSetUnityView.ImageUrl : "";
            luckyDrawSetView.LuckyDrawId = luckyDrawSetUnityView.LuckyDrawId;
            luckyDrawSetView.PointsAttributed = luckyDrawSetUnityView.PointsAttributed;
            luckyDrawSetView.SetWeight = luckyDrawSetUnityView.SetWeight;
        }

        public static List<LuckyDrawSetUnityView> ToLuckyDrawSetUnityViews(this List<LuckyDrawSetView> luckyDrawSetViews)
        {
            if (luckyDrawSetViews == null)
                return null;
            return (from lds in luckyDrawSetViews select lds.ToLuckyDrawSetUnityView()).ToList();
        }

        public static List<LuckyDrawSetView> ToLuckyDrawSetViews(this List<LuckyDrawSetUnityView> luckyDrawSetUnityViews)
        {
            if (luckyDrawSetUnityViews == null)
                return null;
            return (from lds in luckyDrawSetUnityViews select lds.ToLuckyDrawSet()).ToList();
        }

        #endregion

        #region LuckyDrawSetItemUnityView

        public static BundleItemView ToBundleItemView(this LuckyDrawSetItemView luckyDrawSetItemView)
        {
            return new BundleItemView()
            {
                ItemId = luckyDrawSetItemView.ItemId,
                Duration = luckyDrawSetItemView.DurationType,
                Amount = luckyDrawSetItemView.Amount,
                BundleId = luckyDrawSetItemView.LuckyDrawSetId,
            };
        }

        public static List<BundleItemView> ToBundleItemViews(this List<LuckyDrawSetItemView> luckyDrawSetItemViews)
        {
            if (luckyDrawSetItemViews == null)
                return null;
            return (from ldsi in luckyDrawSetItemViews select ldsi.ToBundleItemView()).ToList();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="luckyDrawId"></param>
        /// <param name="checkExposeItem"></param>
        /// <returns></returns>
        public static LuckyDrawView GetLuckyDraw(int luckyDrawId, bool checkExposeItem = false)
        {
            LuckyDrawView luckyDrawView = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                luckyDrawView = cmuneDb.LuckyDraws.First(d => d.Id == luckyDrawId).ToLuckyDrawView();
                luckyDrawView.LuckyDrawSets = cmuneDb.LuckyDrawSets.Where(d => d.LuckyDrawId == luckyDrawView.Id).ToLuckyDrawSetViews();
                foreach (var luckyDrawSetView in luckyDrawView.LuckyDrawSets)
                {
                    if (!checkExposeItem || luckyDrawSetView.ExposeItemsToPlayers == true)
                    {
                        luckyDrawSetView.LuckyDrawSetItems = cmuneDb.LuckyDrawSetItems.Where(d => d.LuckyDrawSetId == luckyDrawSetView.Id).ToLuckyDrawSetItemViews();
                        foreach (var luckyDrawSetItem in luckyDrawSetView.LuckyDrawSetItems)
                        {
                            luckyDrawSetItem.Name = CmuneItem.GetItem(luckyDrawSetItem.ItemId).Name;
                        }
                    }
                }
            }
            return luckyDrawView;
        }

        public static LuckyDrawUnityView GetLuckyDrawUnity(int luckyDrawId)
        {
            LuckyDrawUnityView luckyDrawUnityView = null;

            var luckyDrawView = GetLuckyDraw(luckyDrawId, true);
            if (luckyDrawView != null)
            {
                luckyDrawUnityView = luckyDrawView.ToLuckyDrawUnityView();
                foreach (var lduv in luckyDrawUnityView.LuckyDrawSets)
                {
                    if (!lduv.ExposeItemsToPlayers)
                    {
                        lduv.CreditsAttributed = 0;
                        lduv.PointsAttributed = 0;
                    }
                }
            }
            return luckyDrawUnityView;
        }

        /// <summary>
        /// get lucky draw set
        /// </summary>
        /// <param name="luckyDrawSetId"></param>
        /// <returns></returns>
        public static LuckyDrawSetView GetLuckyDrawSet(int luckyDrawSetId)
        {
            LuckyDrawSetView luckyDrawSetView = null;

            using (var cmuneDb = new CmuneDataContext())
            {

                luckyDrawSetView = cmuneDb.LuckyDrawSets.Where(d => d.Id == luckyDrawSetId).First().ToLuckyDrawSetView();
                luckyDrawSetView.LuckyDrawSetItems = cmuneDb.LuckyDrawSetItems.Where(d => d.LuckyDrawSetId == luckyDrawSetView.Id).ToLuckyDrawSetItemViews();
                foreach (var luckyDrawSetItem in luckyDrawSetView.LuckyDrawSetItems)
                {
                    luckyDrawSetItem.Name = CmuneItem.GetItem(luckyDrawSetItem.ItemId).Name;
                }
            }

            return luckyDrawSetView;
        }

        public static LuckyDrawSetUnityView GetLuckyDrawSetUnity(int luckyDrawSetId)
        {
            LuckyDrawSetUnityView luckyDrawSetUnityView = null;

            var luckyDrawSetView = CmuneLuckyDrawService.GetLuckyDrawSet(luckyDrawSetId);
            if (luckyDrawSetView != null)
                return luckyDrawSetView.ToLuckyDrawSetUnityView();
            return luckyDrawSetUnityView;
        }

        /// <summary>
        /// get a default instance of lucky 
        /// </summary>
        /// <returns></returns>
        public static LuckyDrawView InstantiateDefaultLuckyDraw()
        {
            LuckyDrawView luckyDrawView = new LuckyDrawView();
            luckyDrawView.LuckyDrawSets = new List<LuckyDrawSetView>();
            luckyDrawView.LuckyDrawSets.Add(new LuckyDrawSetView() { LuckyDrawSetItems = new List<LuckyDrawSetItemView>() });
            luckyDrawView.LuckyDrawSets.Add(new LuckyDrawSetView() { LuckyDrawSetItems = new List<LuckyDrawSetItemView>() });
            luckyDrawView.LuckyDrawSets.Add(new LuckyDrawSetView() { LuckyDrawSetItems = new List<LuckyDrawSetItemView>() });

            return luckyDrawView;
        }

        /// <summary>
        /// get all lucky draw
        /// </summary>
        /// <returns></returns>
        public static List<LuckyDrawView> GetLuckyDraws()
        {
            List<LuckyDrawView> luckyDrawViews = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                luckyDrawViews = cmuneDb.LuckyDraws.ToLuckyDrawViews();
                foreach (var luckyDrawView in luckyDrawViews)
                {
                    luckyDrawView.LuckyDrawSets = cmuneDb.LuckyDrawSets.Where(d => d.LuckyDrawId == luckyDrawView.Id).ToLuckyDrawSetViews();
                    foreach (var luckyDrawSetView in luckyDrawView.LuckyDrawSets)
                    {
                        luckyDrawSetView.LuckyDrawSetItems = cmuneDb.LuckyDrawSetItems.Where(d => d.LuckyDrawSetId == luckyDrawSetView.Id).ToLuckyDrawSetItemViews();
                        foreach (var luckyDrawSetItem in luckyDrawSetView.LuckyDrawSetItems)
                        {
                            luckyDrawSetItem.Name = CmuneItem.GetItem(luckyDrawSetItem.ItemId).Name;
                        }
                    }
                }
            }

            return luckyDrawViews;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkExposeItem"></param>
        /// <returns></returns>
        public static List<LuckyDrawView> GetLuckyDraws(bool checkExposeItem = false, bool onlyInShop = true)
        {
            List<LuckyDrawView> luckyDrawViews = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                if (onlyInShop)
                    luckyDrawViews = cmuneDb.LuckyDraws.Where(d => d.IsAvailableInShop == true).ToLuckyDrawViews();
                else
                    luckyDrawViews = cmuneDb.LuckyDraws.ToLuckyDrawViews();

                foreach (var luckyDrawView in luckyDrawViews)
                {
                    luckyDrawView.LuckyDrawSets = cmuneDb.LuckyDrawSets.Where(d => d.LuckyDrawId == luckyDrawView.Id).ToLuckyDrawSetViews();
                    foreach (var luckyDrawSetView in luckyDrawView.LuckyDrawSets)
                    {
                        luckyDrawSetView.LuckyDrawSetItems = new List<LuckyDrawSetItemView>();
                        if (!checkExposeItem || luckyDrawSetView.ExposeItemsToPlayers == true)
                        {
                            luckyDrawSetView.LuckyDrawSetItems.AddRange(cmuneDb.LuckyDrawSetItems.Where(d => d.LuckyDrawSetId == luckyDrawSetView.Id).ToLuckyDrawSetItemViews());
                            foreach (var luckyDrawSetItem in luckyDrawSetView.LuckyDrawSetItems)
                            {
                                luckyDrawSetItem.Name = CmuneItem.GetItem(luckyDrawSetItem.ItemId).Name;
                            }
                        }
                    }
                }
            }

            return luckyDrawViews;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="luckyDrawV"></param>
        /// <returns></returns>
        static bool ValidateLuckyDrawForAdd(LuckyDrawView luckyDrawV)
        {
            if (!(luckyDrawV.LuckyDrawSets.Count == CommonConfig.LuckyDrawSetCount))
                return false;
            foreach (var luckyDrawSetV in luckyDrawV.LuckyDrawSets)
            {
                if (!(luckyDrawSetV.Id == 0))
                    return false;
                if (luckyDrawSetV.LuckyDrawSetItems != null)
                {
                    if (!(luckyDrawSetV.LuckyDrawSetItems.Count <= CommonConfig.LuckyDrawSetItemMaxCount))
                        return false;
                    foreach (var luckyDrawSetItemV in luckyDrawSetV.LuckyDrawSetItems)
                    {
                        if (!(luckyDrawSetItemV.Id == 0))
                            return false;
                    }
                }
            }
            return true;

        }

        /// <summary>
        /// Add a lucky draw
        /// </summary>
        /// <param name="luckyDrawV"></param>
        /// <returns></returns>
        public static bool AddLuckyDraw(LuckyDrawView luckyDrawV)
        {
            bool success = false;
            bool canInsert = true;

            if (luckyDrawV != null && luckyDrawV.Id == 0)
            {
                canInsert = ValidateLuckyDrawForAdd(luckyDrawV);
                if (canInsert)
                {
                    using (var cmuneDb = new CmuneDataContext())
                    {
                        var luckyDraw = luckyDrawV.ToLuckyDraw();

                        cmuneDb.LuckyDraws.InsertOnSubmit(luckyDraw);
                        cmuneDb.SubmitChanges();
                        luckyDrawV.Id = luckyDraw.Id;
                        luckyDrawV.LuckyDrawSets.ForEach(d => d.LuckyDrawId = luckyDrawV.Id);

                        foreach (var luckyDrawSetV in luckyDrawV.LuckyDrawSets)
                        {
                            var luckyDrawSet = luckyDrawSetV.ToLuckyDrawSet();
                            cmuneDb.LuckyDrawSets.InsertOnSubmit(luckyDrawSet);
                            cmuneDb.SubmitChanges();
                            luckyDrawSetV.Id = luckyDrawSet.Id;
                            if (luckyDrawSetV.LuckyDrawSetItems != null)
                            {
                                luckyDrawSetV.LuckyDrawSetItems.ForEach(d => d.LuckyDrawSetId = luckyDrawSetV.Id);

                                foreach (var luckyDrawSetItemV in luckyDrawSetV.LuckyDrawSetItems)
                                {
                                    var luckyDrawSetItem = luckyDrawSetItemV.ToLuckyDrawSetItem();
                                    if (luckyDrawSetItem.ItemId > 0)
                                    {
                                        cmuneDb.LuckyDrawSetItems.InsertOnSubmit(luckyDrawSetItem);
                                        cmuneDb.SubmitChanges();
                                        luckyDrawSetItemV.Id = luckyDrawSetItem.Id;
                                    }
                                }
                            }
                        }
                        success = true;
                    }
                }

            }

            return success;
        }

        static bool ValidateLuckyDrawForEdit(LuckyDrawView luckyDrawV)
        {
            if (!(luckyDrawV.LuckyDrawSets.Count == CommonConfig.LuckyDrawSetCount))
                return false;
            foreach (var luckyDrawSetV in luckyDrawV.LuckyDrawSets)
            {
                if (luckyDrawSetV.LuckyDrawSetItems != null)
                {
                    if (!(luckyDrawSetV.Id > 0 && (luckyDrawSetV.LuckyDrawSetItems.Count <= CommonConfig.LuckyDrawSetItemMaxCount)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Edit a lucky Draw
        /// </summary>
        /// <param name="luckyDrawV"></param>
        /// <returns></returns>
        public static bool EditLuckyDraw(LuckyDrawView luckyDrawV)
        {
            bool success = false;

            if (luckyDrawV != null && luckyDrawV.Id > 0)
            {
                var canInsert = ValidateLuckyDrawForEdit(luckyDrawV);
                if (canInsert)
                {
                    using (var cmuneDb = new CmuneDataContext())
                    {
                        var luckyDraw = cmuneDb.LuckyDraws.First(d => d.Id == luckyDrawV.Id);
                        luckyDraw.CopyFromLuckyDrawView(luckyDrawV);
                        cmuneDb.SubmitChanges();

                        foreach (var luckyDrawSetV in luckyDrawV.LuckyDrawSets)
                        {
                            var luckyDrawSet = cmuneDb.LuckyDrawSets.First(d => d.Id == luckyDrawSetV.Id);
                            luckyDrawSet.CopyFromLuckyDrawSetView(luckyDrawSetV);
                            cmuneDb.SubmitChanges();

                            if (luckyDrawSetV.LuckyDrawSetItems != null)
                            {
                                foreach (var luckyDrawSetItemV in luckyDrawSetV.LuckyDrawSetItems)
                                {
                                    if (luckyDrawSetItemV.Id == 0) // new
                                    {
                                        if (luckyDrawSetItemV.ItemId > 0)
                                        {
                                            // add
                                            luckyDrawSetItemV.LuckyDrawSetId = luckyDrawSetV.Id;
                                            cmuneDb.LuckyDrawSetItems.InsertOnSubmit(luckyDrawSetItemV.ToLuckyDrawSetItem());
                                            cmuneDb.SubmitChanges();
                                        }
                                    }
                                    if (luckyDrawSetItemV.Id > 0) // old
                                    {
                                        if (luckyDrawSetItemV.ItemId == 0)
                                        {
                                            // delete
                                            var luckyDrawSetItem = cmuneDb.LuckyDrawSetItems.First(d => d.Id == luckyDrawSetItemV.Id);
                                            cmuneDb.LuckyDrawSetItems.DeleteOnSubmit(luckyDrawSetItem);
                                            cmuneDb.SubmitChanges();
                                        }
                                        else
                                        {
                                            // update
                                            var luckyDrawSetItem = cmuneDb.LuckyDrawSetItems.First(d => d.Id == luckyDrawSetItemV.Id);
                                            luckyDrawSetItem.CopyFromLuckyDrawSetItemView(luckyDrawSetItemV);
                                            cmuneDb.SubmitChanges();
                                        }
                                    }
                                }
                            }
                        }
                        success = true;
                    }
                }
            }
            return success;
        }

        public static List<PrizeElementView> RollLuckyDraw(LuckyDrawView luckyDrawView, int numberOfRoll = 1)
        {
            List<PrizeElementView> rollList = new List<PrizeElementView>();
            List<PrizeElementView> rolledElements = new List<PrizeElementView>();

            if (numberOfRoll > 0)
            {
                if (luckyDrawView.LuckyDrawSets.Count > 0)
                {
                    foreach (var luckyDrawSet in luckyDrawView.LuckyDrawSets)
                    {
                        rollList.Add(new PrizeElementView()
                        {
                            Id = luckyDrawSet.Id.ToString(),
                            Name = "Prize " + luckyDrawSet.Id.ToString(),
                            Type = PrizeElementType.LuckyDrawSet.ToString(),
                            Weight = luckyDrawSet.SetWeight
                        });
                    }
                }

                for (int i = 0; i < numberOfRoll; i++)
                    rolledElements.Add(PrizesRoll.PickElements(rollList, 1).FirstOrDefault());
            }

            return rolledElements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="luckyDrawId"></param>
        /// <param name="channel"></param>
        /// <param name="isAdmin"></param>
        /// <returns>The set id that was won</returns>
        public static int ProcessLuckyDrawRoll(int cmid, int luckyDrawId, ChannelType channel, bool isAdmin = false)
        {
            bool success = false;
            int luckyDrawSetId = 0;
            int totalCredit = 0;
            int totalPoint = 0;
            PrizeElementView prizeElement;

            var luckyDrawView = CmuneLuckyDrawService.GetLuckyDraw(luckyDrawId);

            bool hasMemberEnoughCurrency = CmuneEconomy.Withdraw(cmid, luckyDrawView.Price, luckyDrawView.UberStrikeCurrencyType);

            if (hasMemberEnoughCurrency)
            {
                prizeElement = CmuneLuckyDrawService.RollLuckyDraw(luckyDrawView).First();
                luckyDrawSetId = Convert.ToInt32(prizeElement.Id);

                var luckyDrawSet = luckyDrawView.LuckyDrawSets.SingleOrDefault(d => d.Id == luckyDrawSetId);

                if (luckyDrawSet.LuckyDrawSetItems != null && luckyDrawSet.LuckyDrawSetItems.Count > 0)
                {
                    CmuneEconomy.AddItemsToInventory(cmid, CmuneItem.GetItems(luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.OneDay).Select(d => d.ItemId).ToList()), BuyingDurationType.OneDay, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, CmuneItem.GetItems(luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.SevenDays).Select(d => d.ItemId).ToList()), BuyingDurationType.SevenDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, CmuneItem.GetItems(luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.ThirtyDays).Select(d => d.ItemId).ToList()), BuyingDurationType.ThirtyDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, CmuneItem.GetItems(luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.NinetyDays).Select(d => d.ItemId).ToList()), BuyingDurationType.NinetyDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventoryPermanently(cmid, luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.Permanent).Select(d => d.ItemId).ToList(), DateTime.Now);
                    foreach (var item in luckyDrawSet.LuckyDrawSetItems.Where(d => d.DurationType == BuyingDurationType.None))
                    {
                        CmuneEconomy.AddConsumableItemToInventory(cmid, item.ItemId, item.Amount, DateTime.Now);
                    }
                }

                foreach (var luckyDrawSetItem in luckyDrawSet.LuckyDrawSetItems)
                {
                    totalCredit += CmuneItem.ComputePrice(luckyDrawSetItem.DurationType, luckyDrawSetItem.ItemId);
                }

                if (luckyDrawSet.CreditsAttributed > 0)
                {
                    totalCredit += luckyDrawSet.CreditsAttributed;
                    CmuneEconomy.AttributeCreditsWhenWinningLuckyDrawMysteryBox(cmid, luckyDrawSet.CreditsAttributed, channel);
                }

                if (luckyDrawSet.PointsAttributed > 0)
                {
                    totalPoint += luckyDrawSet.PointsAttributed;
                    CmuneEconomy.AttributePointsWhenWinningLuckyDrawMysteryBox(cmid, luckyDrawSet.PointsAttributed);
                }

                var boxTransaction = new BoxTransactionView()
                {
                    Cmid = cmid,
                    BoxType = BoxType.LuckyDraw,
                    BoxId = luckyDrawId,
                    Category = luckyDrawView.Category,
                    IsAdmin = isAdmin,
                    TotalCreditsAttributed = totalCredit,
                    TotalPointsAttributed = totalPoint,
                    TransactionDate = DateTime.Now,
                };

                if (luckyDrawView.UberStrikeCurrencyType == UberStrikeCurrencyType.Credits)
                    boxTransaction.CreditPrice = luckyDrawView.Price;
                else if (luckyDrawView.UberStrikeCurrencyType == UberStrikeCurrencyType.Points)
                    boxTransaction.PointPrice = luckyDrawView.Price;

                success = CmuneBoxTransactionService.AddBoxTransaction(boxTransaction);
            }

            return luckyDrawSetId;
        }
    }
}
