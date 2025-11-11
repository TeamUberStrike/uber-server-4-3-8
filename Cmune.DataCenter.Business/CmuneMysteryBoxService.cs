using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;

namespace Cmune.DataCenter.Business
{
    public static class CmuneMysteryBoxService
    {
        #region MysteryBox

        public static MysteryBoxView ToMysteryBoxView(this MysteryBox mysteryBox)
        {
            if (mysteryBox == null)
                return null;
            var mysteryBoxView = new MysteryBoxView();
            mysteryBoxView.Category = (BundleCategoryType)mysteryBox.Category;
            mysteryBoxView.CreditsAttributed = mysteryBox.CreditsAttributed;
            mysteryBoxView.CreditsAttributedWeight = mysteryBox.CreditsAttributedWeight;
            mysteryBoxView.Description = mysteryBox.Description;
            mysteryBoxView.ExposeItemsToPlayers = mysteryBox.ExposeItemsToPlayers;
            mysteryBoxView.IconUrl = mysteryBox.IconUrl;
            mysteryBoxView.Id = mysteryBox.Id;
            mysteryBoxView.ImageUrl = mysteryBox.ImageUrl;
            mysteryBoxView.IsAvailableInShop = mysteryBox.IsAvailableInShop;
            mysteryBoxView.ItemsAttributed = mysteryBox.ItemsAttributed;
            mysteryBoxView.Name = mysteryBox.Name;
            mysteryBoxView.PointsAttributed = mysteryBox.PointsAttributed;
            mysteryBoxView.PointsAttributedWeight = mysteryBox.PointsAttributedWeight;
            if (mysteryBox.PriceInCredits > 0)
            {
                mysteryBoxView.Price = mysteryBox.PriceInCredits;
                mysteryBoxView.UberStrikeCurrencyType = UberStrikeCurrencyType.Credits;
            }
            else if (mysteryBox.PriceInPoints > 0)
            {
                mysteryBoxView.Price = mysteryBox.PriceInPoints;
                mysteryBoxView.UberStrikeCurrencyType = UberStrikeCurrencyType.Points;
            }
            return mysteryBoxView;
        }

        public static MysteryBox ToMysteryBox(this MysteryBoxView mysteryBoxView)
        {
            if (mysteryBoxView == null)
                return null;

            var mysteryBox = new MysteryBox();

            mysteryBox.CopyFromMysteryBoxView(mysteryBoxView);
            mysteryBox.Id = mysteryBoxView.Id;

            return mysteryBox;
        }

        public static void CopyFromMysteryBoxView(this MysteryBox mysteryBox, MysteryBoxView mysteryBoxView)
        {
            if (mysteryBoxView == null)
                return;
            mysteryBox.Category = (int)mysteryBoxView.Category;
            mysteryBox.CreditsAttributed = mysteryBoxView.CreditsAttributed;
            mysteryBox.CreditsAttributedWeight = mysteryBoxView.CreditsAttributedWeight;
            mysteryBox.Description = mysteryBoxView.Description == null ? "" : mysteryBoxView.Description;
            mysteryBox.ExposeItemsToPlayers = mysteryBoxView.ExposeItemsToPlayers;
            mysteryBox.IconUrl = mysteryBoxView.IconUrl == null ? "" : mysteryBoxView.IconUrl;
            mysteryBox.ImageUrl = mysteryBoxView.ImageUrl == null ? "" : mysteryBoxView.ImageUrl;
            mysteryBox.IsAvailableInShop = mysteryBoxView.IsAvailableInShop;
            mysteryBox.ItemsAttributed = mysteryBoxView.ItemsAttributed;
            mysteryBox.Name = mysteryBoxView.Name == null ? "" : mysteryBoxView.Name;
            mysteryBox.PointsAttributed = mysteryBoxView.PointsAttributed;
            mysteryBox.PointsAttributedWeight = mysteryBoxView.PointsAttributedWeight;
            if (mysteryBoxView.UberStrikeCurrencyType == UberStrikeCurrencyType.Points)
            {
                mysteryBox.PriceInPoints = mysteryBoxView.Price;
                mysteryBox.PriceInCredits = 0;
            }
            if (mysteryBoxView.UberStrikeCurrencyType == UberStrikeCurrencyType.Credits)
            {
                mysteryBox.PriceInCredits = mysteryBoxView.Price;
                mysteryBox.PriceInPoints = 0;
            }
        }

        public static List<MysteryBoxView> ToMysteryBoxViews(this List<MysteryBox> mysteryBoxs)
        {
            if (mysteryBoxs == null)
                return null;
            return (from toto in mysteryBoxs select toto.ToMysteryBoxView()).ToList();
        }

        #endregion

        #region MysteryBoxItem

        public static MysteryBoxItemView ToMysteryBoxItemView(this MysteryBoxItem mysteryBoxItem)
        {
            if (mysteryBoxItem == null)
                return null;
            var mysteryBoxItemV = new MysteryBoxItemView();

            mysteryBoxItemV.DurationType = (BuyingDurationType)mysteryBoxItem.DurationType;
            mysteryBoxItemV.Amount = mysteryBoxItem.Amount;
            mysteryBoxItemV.Id = mysteryBoxItem.Id;
            mysteryBoxItemV.ItemId = mysteryBoxItem.ItemId;
            mysteryBoxItemV.ItemWeight = mysteryBoxItem.ItemWeight;
            mysteryBoxItemV.MysteryBoxId = mysteryBoxItem.MysteryBoxId;
            return mysteryBoxItemV;
        }

        public static MysteryBoxItem ToMysteryBoxItem(this MysteryBoxItemView mysteryBoxItemV)
        {
            if (mysteryBoxItemV == null)
                return null;
            var mysteryBoxItem = new MysteryBoxItem();
            mysteryBoxItem.CopyFromMysteryBoxItemView(mysteryBoxItemV);

            mysteryBoxItem.Id = mysteryBoxItemV.Id;

            return mysteryBoxItem;
        }

        public static void CopyFromMysteryBoxItemView(this MysteryBoxItem mysteryBoxItem, MysteryBoxItemView mysteryBoxItemView)
        {
            mysteryBoxItem.DurationType = (int)mysteryBoxItemView.DurationType;
            mysteryBoxItem.Amount = mysteryBoxItemView.Amount;
            mysteryBoxItem.ItemId = mysteryBoxItemView.ItemId;
            mysteryBoxItem.ItemWeight = mysteryBoxItemView.ItemWeight;
            mysteryBoxItem.MysteryBoxId = mysteryBoxItemView.MysteryBoxId;
        }

        public static List<MysteryBoxItemView> ToMysteryBoxItemViews(this List<MysteryBoxItem> mysteryBoxItems)
        {
            if (mysteryBoxItems == null)
                return null;
            return (from mbi in mysteryBoxItems select mbi.ToMysteryBoxItemView()).ToList();
        }

        #endregion

        #region MysteryBoxUnityView

        public static MysteryBoxUnityView ToMysteryBoxUnityView(this MysteryBoxView mysteryBoxView)
        {
            if (mysteryBoxView == null)
                return null;

            return new MysteryBoxUnityView()
            {
                Category = (BundleCategoryType)mysteryBoxView.Category,
                CreditsAttributed = mysteryBoxView.CreditsAttributed,
                CreditsAttributedWeight = mysteryBoxView.CreditsAttributedWeight,
                Description = mysteryBoxView.Description,
                ExposeItemsToPlayers = mysteryBoxView.ExposeItemsToPlayers,
                IconUrl = mysteryBoxView.IconUrl,
                Id = mysteryBoxView.Id,
                ImageUrl = mysteryBoxView.ImageUrl,
                IsAvailableInShop = mysteryBoxView.IsAvailableInShop,
                ItemsAttributed = mysteryBoxView.ItemsAttributed,
                Name = mysteryBoxView.Name,
                PointsAttributed = mysteryBoxView.PointsAttributed,
                PointsAttributedWeight = mysteryBoxView.PointsAttributedWeight,
                Price = mysteryBoxView.Price,
                UberStrikeCurrencyType = mysteryBoxView.UberStrikeCurrencyType,
                MysteryBoxItems = mysteryBoxView.MysteryBoxItems.ToMysteryBoxItemUnityViews(),
            };
        }

        public static List<MysteryBoxUnityView> ToMysteryBoxUnityViews(this List<MysteryBoxView> mysteryBoxs)
        {
            if (mysteryBoxs == null)
                return null;
            return (from toto in mysteryBoxs select toto.ToMysteryBoxUnityView()).ToList();
        }

        #endregion

        #region MysteryBoxItemUnityView

        public static BundleItemView ToBundleItemView(this MysteryBoxItemView mysteryBoxItem)
        {
            if (mysteryBoxItem == null)
                return null;

            return new BundleItemView()
            {
                ItemId = mysteryBoxItem.ItemId,
                Duration = mysteryBoxItem.DurationType,
                Amount = mysteryBoxItem.Amount,
                BundleId = mysteryBoxItem.MysteryBoxId,
            };
        }

        public static List<BundleItemView> ToMysteryBoxItemUnityViews(this List<MysteryBoxItemView> mysteryBoxItems)
        {
            if (mysteryBoxItems == null)
                return null;
            return (from mbi in mysteryBoxItems select mbi.ToBundleItemView()).ToList();
        }

        #endregion

        public static MysteryBoxView GetMysteryBox(int mysteryBoxId, bool checkExposeItem = false)
        {
            MysteryBoxView mysteryBoxView = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                mysteryBoxView = cmuneDb.MysteryBoxes.First(d => d.Id == mysteryBoxId).ToMysteryBoxView();
                mysteryBoxView.MysteryBoxItems = cmuneDb.MysteryBoxItems.Where(d => d.MysteryBoxId == mysteryBoxId).ToList().ToMysteryBoxItemViews();

            }
            return mysteryBoxView;
        }

        public static MysteryBoxUnityView GetMysteryBoxUnity(int mysteryBoxId)
        {
            MysteryBoxUnityView mysteryBoxUnityView = null;

            var mysteryBoxView = GetMysteryBox(mysteryBoxId, true);
            if (mysteryBoxView != null)
            {
                mysteryBoxUnityView = mysteryBoxView.ToMysteryBoxUnityView();
                if (!mysteryBoxUnityView.ExposeItemsToPlayers)
                {
                    mysteryBoxUnityView.CreditsAttributed = 0;
                    mysteryBoxUnityView.PointsAttributed = 0;
                }
            }
            return mysteryBoxUnityView;
        }

        public static List<MysteryBoxView> GetMysteryBoxs()
        {
            List<MysteryBoxView> mysteryBoxViews = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                mysteryBoxViews = cmuneDb.MysteryBoxes.ToList().ToMysteryBoxViews();
                foreach (var mysteryBoxView in mysteryBoxViews)
                {
                    mysteryBoxView.MysteryBoxItems = cmuneDb.MysteryBoxItems.Where(d => d.MysteryBoxId == mysteryBoxView.Id).ToList().ToMysteryBoxItemViews();
                    foreach (var mysteryBoxItems in mysteryBoxView.MysteryBoxItems)
                    {
                        mysteryBoxItems.Name = CmuneItem.GetItem(mysteryBoxItems.ItemId).Name;
                    }
                }
            }
            return mysteryBoxViews;
        }

        public static List<MysteryBoxView> GetMysteryBoxs(bool checkExposeItem = false)
        {
            List<MysteryBoxView> mysteryBoxViews = null;

            using (var cmuneDb = new CmuneDataContext())
            {
                mysteryBoxViews = cmuneDb.MysteryBoxes.Where(d => d.IsAvailableInShop == true).ToList().ToMysteryBoxViews();
                foreach (var mysteryBoxView in mysteryBoxViews)
                {
                    mysteryBoxView.MysteryBoxItems = new List<MysteryBoxItemView>();
                    if (!checkExposeItem || mysteryBoxView.ExposeItemsToPlayers)
                    {
                        mysteryBoxView.MysteryBoxItems.AddRange(cmuneDb.MysteryBoxItems.Where(d => d.MysteryBoxId == mysteryBoxView.Id).ToList().ToMysteryBoxItemViews());
                        foreach (var mysteryBoxItems in mysteryBoxView.MysteryBoxItems)
                        {
                            mysteryBoxItems.Name = CmuneItem.GetItem(mysteryBoxItems.ItemId).Name;
                        }
                    }
                }
            }

            return mysteryBoxViews;
        }

        public static List<MysteryBoxUnityView> GetAllMysteryBoxUnitysFilteredAtExposeItem(BundleCategoryType bundleCategoryType)
        {
            return CmuneMysteryBoxService.GetMysteryBoxs(true).Where(d => d.Category == bundleCategoryType).ToList().ToMysteryBoxUnityViews();
        }

        public static List<MysteryBoxUnityView> GetAllMysteryBoxUnitysFilteredAtExposeItem()
        {
            return CmuneMysteryBoxService.GetMysteryBoxs(true).ToMysteryBoxUnityViews();
        }

        public static bool ValidateMysteryBoxForAdd(MysteryBoxView mysteryBoxView)
        {
            if (mysteryBoxView.MysteryBoxItems != null)
            {
                if (!(mysteryBoxView.MysteryBoxItems.Count <= CommonConfig.MysteryBoxItemMaxCount))
                    return false;
                foreach (var mysteryBoxItemV in mysteryBoxView.MysteryBoxItems)
                {
                    if (!(mysteryBoxItemV.Id == 0))
                        return false;
                }
            }
            return true;
        }

        public static bool AddMysteryBox(MysteryBoxView mysteryBoxView)
        {
            bool success = false;
            bool canInsert = true;

            if (mysteryBoxView != null && mysteryBoxView.Id == 0)
            {
                canInsert = ValidateMysteryBoxForAdd(mysteryBoxView);
                if (canInsert)
                {
                    using (var cmuneDb = new CmuneDataContext())
                    {
                        var mysteryBox = mysteryBoxView.ToMysteryBox();
                        cmuneDb.MysteryBoxes.InsertOnSubmit(mysteryBox);
                        cmuneDb.SubmitChanges();
                        mysteryBoxView.Id = mysteryBox.Id;
                        if (mysteryBoxView.MysteryBoxItems != null)
                        {
                            mysteryBoxView.MysteryBoxItems.ForEach(d => d.MysteryBoxId = mysteryBoxView.Id);

                            foreach (var mysteryBoxItemV in mysteryBoxView.MysteryBoxItems)
                            {
                                var mysteryBoxItem = mysteryBoxItemV.ToMysteryBoxItem();
                                if (mysteryBoxItem.ItemId > 0)
                                {
                                    cmuneDb.MysteryBoxItems.InsertOnSubmit(mysteryBoxItem);
                                    cmuneDb.SubmitChanges();
                                    mysteryBoxItemV.Id = mysteryBoxItem.Id;
                                }
                            }
                        }
                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool ValidateMysteryBoxForEdit(MysteryBoxView mysteryBoxView)
        {
            if (mysteryBoxView.MysteryBoxItems != null)
            {
                if (!((mysteryBoxView.MysteryBoxItems.Count <= CommonConfig.LuckyDrawSetItemMaxCount)))
                    return false;
            }
            return true;
        }

        public static bool EditMysteryBox(MysteryBoxView mysteryBoxView)
        {
            bool success = false;

            if (mysteryBoxView != null && mysteryBoxView.Id > 0)
            {
                var canEdit = ValidateMysteryBoxForEdit(mysteryBoxView);
                if (canEdit)
                {
                    using (var cmuneDb = new CmuneDataContext())
                    {
                        var mysteryBox = cmuneDb.MysteryBoxes.First(d => d.Id == mysteryBoxView.Id);
                        mysteryBox.CopyFromMysteryBoxView(mysteryBoxView);
                        cmuneDb.SubmitChanges();

                        if (mysteryBoxView.MysteryBoxItems != null)
                        {
                            foreach (var mysteryBoxItemV in mysteryBoxView.MysteryBoxItems)
                            {
                                if (mysteryBoxItemV.Id == 0) // new
                                {
                                    if (mysteryBoxItemV.ItemId > 0)
                                    {
                                        // add
                                        mysteryBoxItemV.MysteryBoxId = mysteryBoxView.Id;
                                        cmuneDb.MysteryBoxItems.InsertOnSubmit(mysteryBoxItemV.ToMysteryBoxItem());
                                        cmuneDb.SubmitChanges();
                                    }
                                }
                                if (mysteryBoxItemV.Id > 0) // old
                                {
                                    if (mysteryBoxItemV.ItemId == 0)
                                    {
                                        // delete
                                        var mysteryBoxItem = cmuneDb.MysteryBoxItems.First(d => d.Id == mysteryBoxItemV.Id);
                                        cmuneDb.MysteryBoxItems.DeleteOnSubmit(mysteryBoxItem);
                                        cmuneDb.SubmitChanges();
                                    }
                                    else if (mysteryBoxItemV.ItemId > 0)
                                    {
                                        // update
                                        var mysteryBoxItem = cmuneDb.MysteryBoxItems.First(d => d.Id == mysteryBoxItemV.Id);
                                        mysteryBoxItem.CopyFromMysteryBoxItemView(mysteryBoxItemV);
                                        cmuneDb.SubmitChanges();
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

        /// <summary>
        /// get a default instance of lucky 
        /// </summary>
        /// <returns></returns>
        public static MysteryBoxView InstantiateDefaultMysteryBoxView()
        {
            MysteryBoxView mysteryBoxView = new MysteryBoxView();
            mysteryBoxView.MysteryBoxItems = new List<MysteryBoxItemView>();

            return mysteryBoxView;
        }

        /// <summary>
        /// Rolls a mysteryBox rollsCount times for testing purpose
        /// </summary>
        /// <param name="mysteryBox"></param>
        /// <param name="rollsCount"></param>
        /// <returns></returns>
        public static List<List<PrizeElementView>> RollMysterBox(MysteryBoxView mysteryBox, int rollsCount)
        {
            List<List<PrizeElementView>> allRolledElements = new List<List<PrizeElementView>>();

            for (int i = 0; i < rollsCount; i++)
            {
                allRolledElements.Add(RollMysterBox(mysteryBox));
            }

            return allRolledElements;
        }

        /// <summary>
        /// Roll a mystery box
        /// </summary>
        /// <param name="mysteryBox"></param>
        /// <returns></returns>
        public static List<PrizeElementView> RollMysterBox(MysteryBoxView mysteryBox)
        {
            List<PrizeElementView> rolledElements = new List<PrizeElementView>();

            if (mysteryBox.ItemsAttributed > 0)
            {
                List<PrizeElementView> rollList = new List<PrizeElementView>();

                foreach (var mysteryBoxItem in mysteryBox.MysteryBoxItems)
                {
                    rollList.Add(new PrizeElementView()
                    {
                        Id = mysteryBoxItem.ItemId.ToString(),
                        Type = PrizeElementType.Item.ToString(),
                        Weight = mysteryBoxItem.ItemWeight
                    });
                }

                if (mysteryBox.PointsAttributedWeight > 0 && mysteryBox.PointsAttributed > 0)
                {
                    rollList.Add(new PrizeElementView()
                    {
                        Id = UberStrikeCurrencyType.Points.ToString(),
                        Type = PrizeElementType.Point.ToString(),
                        Name = PrizeElementType.Point.ToString(),
                        Weight = mysteryBox.PointsAttributedWeight
                    });
                }

                if (mysteryBox.CreditsAttributedWeight > 0 && mysteryBox.CreditsAttributed > 0)
                {
                    rollList.Add(new PrizeElementView()
                    {
                        Id = UberStrikeCurrencyType.Credits.ToString(),
                        Type = PrizeElementType.Credit.ToString(),
                        Name = PrizeElementType.Credit.ToString(),
                        Weight = mysteryBox.CreditsAttributedWeight
                    });
                }

                rolledElements = PrizesRoll.PickElements(rollList, mysteryBox.ItemsAttributed);
            }

            return rolledElements;
        }

        public static List<MysteryBoxWonItemUnityView> ProcessMysteryBoxRoll(int cmid, int mysteryBoxId, ChannelType channel, bool isAdmin = false)
        {
            bool success = false;
            int itemId;
            int totalCredit = 0;
            int totalPoint = 0;
            List<PrizeElementView> prizeElements;
            UberStrikeCurrencyType currency;
            List<MysteryBoxWonItemUnityView> listOfWonItems = new List<MysteryBoxWonItemUnityView>();

            var mysteryBoxView = CmuneMysteryBoxService.GetMysteryBox(mysteryBoxId);

            bool hasMemberEnoughCurrency = CmuneEconomy.Withdraw(cmid, mysteryBoxView.Price, mysteryBoxView.UberStrikeCurrencyType);

            if (hasMemberEnoughCurrency)
            {
                prizeElements = CmuneMysteryBoxService.RollMysterBox(mysteryBoxView);

                foreach (var prizeElement in prizeElements)
                {
                    if (int.TryParse(prizeElement.Id, out itemId))
                    {
                        var mysteryBoxItem = mysteryBoxView.MysteryBoxItems.Where(d => d.ItemId == itemId).First();
                        totalCredit += CmuneItem.ComputePrice(mysteryBoxItem.DurationType, mysteryBoxItem.ItemId);
                        var item = CmuneItem.GetItems(new List<int>() { mysteryBoxItem.ItemId });
                        listOfWonItems.Add(new MysteryBoxWonItemUnityView() { ItemIdWon = itemId });

                        switch (mysteryBoxItem.DurationType)
                        {
                            case BuyingDurationType.None:
                                CmuneEconomy.AddConsumableItemToInventory(cmid, itemId, mysteryBoxItem.Amount, DateTime.Now);
                                break;
                            case BuyingDurationType.Permanent:
                                CmuneEconomy.AddItemsToInventoryPermanently(cmid, new List<int>() { mysteryBoxItem.ItemId }, DateTime.Now);
                                break;
                            default:
                                CmuneEconomy.AddItemsToInventory(cmid, item, mysteryBoxItem.DurationType, DateTime.Now, false);
                                break;
                        }
                    }
                    else if (Enum.TryParse(prizeElement.Id, out currency))
                    {
                        if (currency == UberStrikeCurrencyType.Credits)
                        {
                            totalCredit += mysteryBoxView.CreditsAttributed;
                            CmuneEconomy.AttributeCreditsWhenWinningLuckyDrawMysteryBox(cmid, mysteryBoxView.CreditsAttributed, channel);
                            listOfWonItems.Add(new MysteryBoxWonItemUnityView() { CreditWon = mysteryBoxView.CreditsAttributed });
                        }
                        else if (currency == UberStrikeCurrencyType.Points)
                        {
                            totalPoint += mysteryBoxView.PointsAttributed;
                            CmuneEconomy.AttributePointsWhenWinningLuckyDrawMysteryBox(cmid, mysteryBoxView.PointsAttributed);
                            listOfWonItems.Add(new MysteryBoxWonItemUnityView() { PointWon = mysteryBoxView.PointsAttributed });
                        }
                    }
                }

                var boxTransaction = new BoxTransactionView()
                {
                    Cmid = cmid,
                    BoxType = BoxType.MysteryBox,
                    BoxId = mysteryBoxId,
                    Category = mysteryBoxView.Category,
                    IsAdmin = isAdmin,
                    TotalCreditsAttributed = totalCredit,
                    TotalPointsAttributed = totalPoint,
                    TransactionDate = DateTime.Now,
                };

                if (mysteryBoxView.UberStrikeCurrencyType == UberStrikeCurrencyType.Credits)
                    boxTransaction.CreditPrice = mysteryBoxView.Price;
                else if (mysteryBoxView.UberStrikeCurrencyType == UberStrikeCurrencyType.Points)
                    boxTransaction.PointPrice = mysteryBoxView.Price;

                success = CmuneBoxTransactionService.AddBoxTransaction(boxTransaction);
            }

            return listOfWonItems;
        }
    }
}
