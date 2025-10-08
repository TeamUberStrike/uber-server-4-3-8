using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Channels.Common.Models;
using UberStrike.Channels.Common.Business;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Utils;
using UberStrike.DataCenter.Common.Entities;
using System.Web.UI;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Common.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, Duration = 0)]
    public class GetCreditBundleController : Controller
    {
        bool GetUserOrigin(ChannelUserViewModel channelUserViewModel)
        {
            if (channelUserViewModel.Cmid == 0)
                return false;
            return true;
        }

        bool GetUserPaymentSelection(ChannelUserViewModel channelUserViewModel)
        {
            if (channelUserViewModel.PaymentProviderType == null || channelUserViewModel.PaymentType.IsNullOrFullyEmpty())
                return false;
            return true;
        }

        public GetCreditBundleController()
        {
            ViewBag.Environment = Cmune.DataCenter.Utils.ConfigurationUtilities.ReadConfigurationManager("Environment", false);
        }

        // do not forget to rewrite parameter
        public ActionResult Index(ChannelType channel, int cmid, string facebookThirdPartId = "", int contentWidth = 0, int contentHeight = 0)
        {
            bool noTreeflow = false;
            bool skipSelectPayment = false;
            var initStep = PaymentStepsEnum.SelectPayment;
            if (channel == ChannelType.WebFacebook || channel == ChannelType.Kongregate)
            {
                noTreeflow = true;
            }
            
            if (channel == ChannelType.WebFacebook || channel == ChannelType.Kongregate)
            {
                skipSelectPayment = true;
                initStep = PaymentStepsEnum.SelectPack;
            }

            var channelUser = new ChannelUserViewModel() { Channel = channel, Cmid = cmid, FacebookThirdPartId = facebookThirdPartId }; 

            ViewBag.ChannelType = channel;
           

            ViewBag.SkipSelectPayment = skipSelectPayment;
            ViewBag.CurrentStep = initStep;
            ViewBag.ContentWidth = contentWidth;
            ViewBag.ContentHeight = contentHeight;
            ViewBag.NoTreeflow = noTreeflow;
            return View(channelUser);
        }

        public ActionResult LoadSelectPaymentStep(ChannelUserViewModel channelUserViewModel)
        {
            if (!GetUserOrigin(channelUserViewModel))
                return null;
            ViewBag.SuperRewardsUserId = channelUserViewModel.Channel == ChannelType.WebFacebook ? channelUserViewModel.FacebookThirdPartId : channelUserViewModel.Cmid.ToString();
            return PartialView("Partial/SelectPaymentStep", channelUserViewModel);
        }

        public ActionResult LoadThankYouForShoppingWithUs(ChannelUserViewModel channelUserViewModel)
        {
            if (!GetUserOrigin(channelUserViewModel))
                return null;
            ViewBag.ChannelType = channelUserViewModel.Channel;
            ViewBag.Cmid = channelUserViewModel.Cmid;
            ViewBag.FacebookThirdPartId = channelUserViewModel.FacebookThirdPartId;
            ViewBag.SuperRewardsUserId = channelUserViewModel.Channel == ChannelType.WebFacebook ? channelUserViewModel.FacebookThirdPartId : channelUserViewModel.Cmid.ToString();
            return PartialView("Partial/ThankYouForShoppingWithUs");
        }

        public ActionResult LoadSelectCreditBundleStep(ChannelUserViewModel channelUserViewModel)
        {
            if (!GetUserOrigin(channelUserViewModel))
                return null;
            ViewBag.ChannelType = channelUserViewModel.Channel;
            ViewBag.SelectedBundleId = CmuneBundle.GetDefaultBundleId(true);
            List<BundleView> bundles;
            if (channelUserViewModel.Channel == ChannelType.Kongregate)
            {
                bundles = new List<BundleView>();
                CommonCache.LoadKongregateCreditBundles().ForEach(d => bundles.Add(d));
            }
            else
            {
                bundles = CommonCache.LoadCreditBundles(channelUserViewModel.Channel);
            }
            ViewBag.Bundles = bundles;
            return PartialView("Partial/SelectCreditBundleStep", channelUserViewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadCompleteTransactionStep(ChannelUserViewModel channelUserViewModel, int bundleId)
        {
            var bundle = CommonCache.LoadCreditBundles(channelUserViewModel.Channel).FirstOrDefault(d => d.Id == bundleId);
            if (bundle == null)
                return null;
            if (!GetUserOrigin(channelUserViewModel))
                return null;
            if (!GetUserPaymentSelection(channelUserViewModel))
                return null;

            ViewBag.Amount = String.Format("{0:0.00}", bundle.USDPrice);

            var merchtrans = CmuneEconomy.PlaySpanGenerateMerchtrans(channelUserViewModel.Cmid);
            ViewBag.PlayspanMerchtrans = merchtrans;
            ViewBag.PlayspanHash = CmuneEconomy.PlaySpanGenerateClientHash(channelUserViewModel.Cmid, bundle.USDPrice, "USD", UberStrikeCommonConfig.ApplicationId, channelUserViewModel.Channel, bundleId, merchtrans);
            ViewBag.DeveloperId = CmuneEconomy.PaymentWriteDeveloperId(UberStrikeCommonConfig.ApplicationId, channelUserViewModel.Channel, bundleId);
            return PartialView("Partial/CompleteTransactionStep", channelUserViewModel);
        }

        public ActionResult About()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SuperRewards(string id, ChannelType ch, int cm)
        {
            string userId = id;
            int cmid = cm;
            ChannelType channelType = ch;
            string developerId = String.Empty;
            string hash = String.Empty;
            string name = String.Empty;
            string superRewardsApplicationId = string.Empty;
            if (ch == ChannelType.WebFacebook)
                superRewardsApplicationId = HttpUtility.UrlDecode(ConfigurationUtilities.ReadConfigurationManager("SuperRewardsFacebookApplicationId"));
            else
                superRewardsApplicationId = HttpUtility.UrlDecode(ConfigurationUtilities.ReadConfigurationManager("SuperRewardsPortalApplicationId"));

            bool hasError = true;
            // Parsing URL

            if (cmid > 0)
            {
                ID3 memberId3 = CmuneMember.GetId3(cmid);

                if (memberId3 != null && superRewardsApplicationId != string.Empty)
                {
                    ViewBag.ChannelType = channelType;
                    ViewBag.SuperRewardsApplicationId = superRewardsApplicationId;
                    ViewBag.Name = HttpUtility.HtmlEncode(memberId3.Name);
                    ViewBag.UserId = userId;
                    ViewBag.Cmid = cmid;
                    ViewBag.DeveloperId = developerId = CmuneEconomy.PaymentWriteDeveloperId(UberStrikeCommonConfig.ApplicationId, channelType, 0);
                    ViewBag.Hash = CmuneEconomy.SuperRewardsGenerateCmuneHash(userId, developerId, cmid);
                    hasError = false;
                }
            }
            ViewBag.HasError = hasError;
            return View();
        }

        public ActionResult PlaySpanXD()
        {
            return View();
        }
    }
}

