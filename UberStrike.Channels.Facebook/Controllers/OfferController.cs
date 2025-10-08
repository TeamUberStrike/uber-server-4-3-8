using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook.Web.Mvc;
using System.Configuration;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Channels.Utils;

namespace UberStrike.Channels.Facebook.Controllers
{
    [CanvasAuthorize(Permissions = "publish_stream,email,publish_actions")]
    public class OfferController : BaseController
    {
        #region Properties and Parameters

        /// <summary>
        /// Hash used to identify the internal application Id and the channel
        /// </summary>
        public string DeveloperId = String.Empty;
        /// <summary>
        /// Hash to ensure that our custom fields are not modified
        /// </summary>
        public string Hash = String.Empty;
        /// <summary>
        /// Name of the member loading this page
        /// </summary>
        public string Name = String.Empty;
        /// <summary>
        /// Super Rewards application Id as we have to use a different iframe depending on the channel
        /// </summary>
        public string SuperRewardsApplicationId = String.Empty;

        bool displayError;
        /// <summary>
        /// Cmid
        /// </summary>
        public int Cmid = 0;

        #endregion

        #region Constructors

        public OfferController()
            : base()
        {
            base.CurrentTab = Utils.Menu.MainTab.Credit;
        }

        #endregion

        private void InitIframeValues()
        {
            displayError = true;
            if (FbUserModel.IsAuthenticated)
            {
                ID3 memberId3 = CmuneMember.GetId3(FbUserModel.Cmid);

                if (memberId3 != null)
                {
                    bool isInCommon = ConfigurationUtilities.ReadConfigurationManagerBool("IsSuperRewardsInCommon");

                    if (isInCommon)
                    {
                        this.SuperRewardsApplicationId = UnityArgsBuilder.GetSuperRewardsApplicationId(ChannelType.WebFacebook);
                    }
                    else
                    {
                        this.SuperRewardsApplicationId = ConfigurationUtilities.ReadConfigurationManager("SuperRewardsApplication");
                    }

                    ViewBag.SuperRewardsApplicationId = SuperRewardsApplicationId;
                    ViewBag.Name = HttpUtility.HtmlEncode(memberId3.Name);
                    ViewBag.FacebookThirdPartyId = FbUserModel.FacebookThirdPartyId;
                    ViewBag.Cmid = FbUserModel.Cmid;
                    ViewBag.DeveloperId = DeveloperId = CmuneEconomy.PaymentWriteDeveloperId(UberStrikeCommonConfig.ApplicationId, ChannelType.WebFacebook, 0);
                    ViewBag.Hash = CmuneEconomy.SuperRewardsGenerateCmuneHash(FbUserModel.FacebookThirdPartyId.ToString(), DeveloperId, FbUserModel.Cmid);
                    displayError = false;
                }
            }
        }

        public ActionResult Index()
        {
            InitIframeValues();
            
            ViewBag.HasError = displayError;
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }


        public ActionResult FacebookCredits()
        {
            ViewBag.Cmid = FbUserModel.Cmid;
            ViewBag.HasError = false;
            if (Request.IsAjaxRequest())
                return PartialView(FbUserModel);
            return View(FbUserModel);
        }

        public ActionResult Bundle()
        {
            ViewBag.Cmid = FbUserModel.Cmid;
            ViewBag.HasError = false;
            return PartialView(FbUserModel);
        }

        public ActionResult EarnFacebookCredits()
        {
            ViewBag.Cmid = FbUserModel.Cmid;
            ViewBag.HasError = false;
            if (Request.IsAjaxRequest())
                return PartialView(FbUserModel);
            return View(FbUserModel);
        }

        public ActionResult RedeemEpin()
        {
            ViewBag.UberStrikePortal = ConfigurationUtilities.ReadConfigurationManager("UberStrikePortal");
            ViewBag.UberStrikePortalEpinPage = ConfigurationUtilities.ReadConfigurationManager("UberStrikePortalEpinPage");
            return PartialView(FbUserModel);
        }
    }
}
