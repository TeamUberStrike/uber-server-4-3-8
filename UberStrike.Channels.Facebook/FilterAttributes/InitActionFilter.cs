using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;

namespace UberStrike.Channels.Facebook.FilterAttributes
{
    public class InitActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            filterContext.Controller.ViewBag.HeaderBannerFilename = "";
            filterContext.Controller.ViewBag.HeaderBannerFilenameTitle = "";
            filterContext.Controller.ViewBag.HeaderBannerAnchorLink = "";

            var promotionContent = PromotionContentService.GetLastPromotionContent();
            if (promotionContent != null)
            {
                var elementBanner = PromotionContentService.GetFacebookPromotionElement(ChannelElement.Banner, promotionContent.PromotionContentElements);

                if (elementBanner != null)
                {
                    filterContext.Controller.ViewBag.HeaderBannerFilename = elementBanner.Filename;
                    filterContext.Controller.ViewBag.HeaderBannerFilenameTitle = elementBanner.FilenameTitle;
                    filterContext.Controller.ViewBag.HeaderBannerAnchorLink = elementBanner.AnchorLink;
                }
            }
        }
    }
}