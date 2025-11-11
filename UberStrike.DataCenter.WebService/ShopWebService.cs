using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class ShopWebService : IShopWebService
    {
        public UberStrikeItemShopClientView GetShop(string applicationVersion)
        {
            return UberstrikeShop.GetClientShop(applicationVersion);
        }

        public int BuyItem(int itemId, int buyerCmid, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            return UberstrikeShop.BuyItem(itemId, buyerCmid, currencyType, durationType, itemType, marketLocation, recommendationType);
        }

        public int BuyPack(int itemId, int buyerCmid, PackType packType, UberStrikeCurrencyType currencyType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            return UberstrikeShop.BuyPack(itemId, buyerCmid, packType, currencyType, itemType, marketLocation, recommendationType);
        }

        public List<BundleView> GetBundles(ChannelType channel)
        {
            return CmuneBundle.GetAllBundlesOnSaleView(channel);
        }

        public bool BuyMasBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            return CmuneEconomy.BuyMasBundle(cmid, bundleId, hashedReceipt, applicationId);
        }

        public bool BuyiPadBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            return CmuneEconomy.BuyiPadBundle(cmid, bundleId, hashedReceipt, applicationId);
        }

        public bool BuyiPhoneBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            return CmuneEconomy.BuyiPhoneBundle(cmid, bundleId, hashedReceipt, applicationId);
        }

        public bool UseConsumableItem(int cmid, int itemId)
        {
            return CmuneEconomy.UseConsumableItem(cmid, itemId);
        }

        public List<MysteryBoxUnityView> GetAllMysteryBoxs()
        {
            return CmuneMysteryBoxService.GetAllMysteryBoxUnitysFilteredAtExposeItem();
        }

        public List<MysteryBoxUnityView> GetAllMysteryBoxs(BundleCategoryType bundleCategoryType)
        {
            return CmuneMysteryBoxService.GetAllMysteryBoxUnitysFilteredAtExposeItem(bundleCategoryType);
        }

        public MysteryBoxUnityView GetMysteryBox(int mysteryBoxId)
        {
            return CmuneMysteryBoxService.GetMysteryBoxUnity(mysteryBoxId);
        }

        public List<MysteryBoxWonItemUnityView> RollMysteryBox(int cmid, int mysteryBoxId, ChannelType channel)
        {
            return CmuneMysteryBoxService.ProcessMysteryBoxRoll(cmid, mysteryBoxId, channel);
        }

        public List<LuckyDrawUnityView> GetAllLuckyDraws()
        {
            return CmuneLuckyDrawService.GetLuckyDraws(false, true).ToLuckyDrawUnityViews();
        }

        public List<LuckyDrawUnityView> GetAllLuckyDraws(BundleCategoryType bundleCategoryType)
        {
            return CmuneLuckyDrawService.GetLuckyDraws(true).Where(d => d.Category == bundleCategoryType).ToLuckyDrawUnityViews();
        }

        public LuckyDrawUnityView GetLuckyDraw(int luckyDrawId)
        {
            return CmuneLuckyDrawService.GetLuckyDrawUnity(luckyDrawId);
        }

        public int RollLuckyDraw(int cmid, int luckDrawId, ChannelType channel)
        {
            return CmuneLuckyDrawService.ProcessLuckyDrawRoll(cmid, luckDrawId, channel);
        }
    }
}