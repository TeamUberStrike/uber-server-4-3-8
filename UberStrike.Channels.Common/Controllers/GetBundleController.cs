using System;
using System.Linq;
using System.Web.Mvc;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Channels.Common.Business;
using UberStrike.Channels.Common.Models;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Common.Controllers
{
    public class GetBundleController : Controller
    {

        PaymentProviderType PaymentProviderId { get; set; }
        int PaymentType { get; set; }

        bool GetUserOrigin(ChannelUserViewModel channelUserViewModel)
        {
            if (channelUserViewModel.Cmid == 0)
                return false;
            return true;
        }

        //
        // GET: /GetBundle/
        public ActionResult Index(ChannelType channel, int cmid, string facebookThirdPartId = "", int contentWidth = 0, int contentHeight = 0)
        {
            var channelUser = new ChannelUserViewModel() { Channel = channel, Cmid = cmid, FacebookThirdPartId = facebookThirdPartId }; 

            ViewBag.ChannelType = channel;
            ViewBag.CurrentStep = BuyBundleStepsEnum.SelectBundle;
            ViewBag.ContentWidth = contentWidth;
            ViewBag.ContentHeight = contentHeight;
            return View(channelUser);
        }

        public ActionResult LoadSelectBundleStep(ChannelUserViewModel channelUserViewModel)
        {
            if (!GetUserOrigin(channelUserViewModel))
                return null;
            var bundles = CommonCache.LoadItemAndPointBundles(channelUserViewModel.Channel);
            ViewBag.ChannelType = channelUserViewModel.Channel;
            ViewBag.Cmid = channelUserViewModel.Cmid;
            ViewBag.SelectedBundleId = CmuneBundle.GetDefaultBundleId(false);
            ViewBag.ItemCache = new ItemCache(CommonConfig.ApplicationIdUberstrike, true);
            ViewBag.Bundles = bundles;
            return PartialView("Partial/SelectBundleStep", channelUserViewModel);
        }

        public ActionResult LoadCompleteBundleTransactionStep(ChannelUserViewModel channelUserViewModel, int bundleId)
        {
            var bundle = CommonCache.LoadItemAndPointBundles(channelUserViewModel.Channel).FirstOrDefault(d => d.Id == bundleId);
            if (bundle == null)
                return null;
            if (!GetUserOrigin(channelUserViewModel))
                return null;

            if (PaymentProviderId == PaymentProviderType.PlaySpan)
            {
                //if (!GetPlayspanParams())
                //    return null;
            }
            ViewBag.ChannelType = channelUserViewModel.Channel;
            ViewBag.Cmid = channelUserViewModel.Cmid;
            ViewBag.PaymentProviderType = PaymentProviderId;
            ViewBag.PaymentType = PaymentType;

            ViewBag.Amount = String.Format("{0:0.00}", bundle.USDPrice);

            var merchtrans = CmuneEconomy.PlaySpanGenerateMerchtrans(channelUserViewModel.Cmid);
            ViewBag.PlayspanMerchtrans = merchtrans;
            ViewBag.PlayspanHash = CmuneEconomy.PlaySpanGenerateClientHash(channelUserViewModel.Cmid, bundle.USDPrice, "USD", UberStrikeCommonConfig.ApplicationId, channelUserViewModel.Channel, bundleId, merchtrans);
            ViewBag.DeveloperId = CmuneEconomy.PaymentWriteDeveloperId(UberStrikeCommonConfig.ApplicationId, channelUserViewModel.Channel, bundleId);
            ViewBag.SelectedBundleId = CmuneBundle.GetDefaultBundleId(false);
            return PartialView("Partial/CompleteBundleTransactionStep");
        }
    }
}
