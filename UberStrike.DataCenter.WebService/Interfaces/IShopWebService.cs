using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IShopWebService
    {
        UberStrikeItemShopClientView GetShop(string applicationVersion);
        int BuyItem(int itemId, int buyerCmid, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType);
        int BuyPack(int itemId, int buyerCmid, PackType packType, UberStrikeCurrencyType currencyType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType);
        List<BundleView> GetBundles(ChannelType channel);
        bool BuyMasBundle(int cmid, int bundleId, string hashedReceipt, int applicationId);
        bool BuyiPadBundle(int cmid, int bundleId, string hashedReceipt, int applicationId);
        bool BuyiPhoneBundle(int cmid, int bundleId, string hashedReceipt, int applicationId);
        bool UseConsumableItem(int cmid, int itemId);
        List<MysteryBoxUnityView> GetAllMysteryBoxs();
        List<MysteryBoxUnityView> GetAllMysteryBoxs(BundleCategoryType bundleCategoryType);
        MysteryBoxUnityView GetMysteryBox(int mysteryBoxId);
        List<MysteryBoxWonItemUnityView> RollMysteryBox(int cmid, int mysteryBoxId, ChannelType channel);
        List<LuckyDrawUnityView> GetAllLuckyDraws();
        List<LuckyDrawUnityView> GetAllLuckyDraws(BundleCategoryType bundleCategoryType);
        LuckyDrawUnityView GetLuckyDraw(int luckyDrawId);
        int RollLuckyDraw(int cmid, int luckDrawId, ChannelType channel);
    }
}