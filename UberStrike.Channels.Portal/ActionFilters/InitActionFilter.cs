using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;

namespace UberStrike.Channels.Portal.ActionFilters
{
    public class InitActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            filterContext.Controller.ViewBag.HeaderBannerFilename = "";
            filterContext.Controller.ViewBag.HeaderBannerFilenameTitle = "";
            filterContext.Controller.ViewBag.HeaderBannerAnchorLink = "";
            filterContext.Controller.ViewBag.RightPromotionFilename = "";
            filterContext.Controller.ViewBag.RightPromotionFilenameTitle = "";
            filterContext.Controller.ViewBag.RightPromotionAnchorLink = "";

            var promotionContent = PromotionContentService.GetLastPromotionContent();
            if (promotionContent != null)
            {
                var elementBanner = PromotionContentService.GetPortalPromotionElement(ChannelElement.Banner, promotionContent.PromotionContentElements);
                var elementRightPromotion = PromotionContentService.GetPortalPromotionElement(ChannelElement.RightPromotion, promotionContent.PromotionContentElements);

                if (elementBanner != null)
                {
                    filterContext.Controller.ViewBag.HeaderBannerFilename = elementBanner.Filename;
                    filterContext.Controller.ViewBag.HeaderBannerFilenameTitle = elementBanner.FilenameTitle;
                    filterContext.Controller.ViewBag.HeaderBannerAnchorLink = elementBanner.AnchorLink;
                }

                if (elementRightPromotion != null)
                {
                    filterContext.Controller.ViewBag.RightPromotionFilename = elementRightPromotion.Filename;
                    filterContext.Controller.ViewBag.RightPromotionFilenameTitle = elementRightPromotion.FilenameTitle;
                    filterContext.Controller.ViewBag.RightPromotionAnchorLink = elementRightPromotion.AnchorLink;
                }
            }
        }
    }
}