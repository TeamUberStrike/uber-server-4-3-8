using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.DataAccess;
using UberStrike.Core.ViewModel;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;
using System.Web.Script.Serialization;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class PromotionContentController : BaseController
    {
        //
        // GET: /Promotion/

        public PromotionContentController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Management;
        }

        public ActionResult Index()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.Promotion;
            var promotionContents = PromotionContentService.GetPromotionContents();
            return View(promotionContents);
        }

        public ActionResult LoadAddEditPromotionContentForm(int promotionContentId)
        {
            var promotionContentModel = new PromotionContentViewModel();
            promotionContentModel.StartDate = DateTime.Now;
            promotionContentModel.EndDate = DateTime.Now.AddDays(7);
            var listOfChannelElement = PromotionContentService.GetChannelElementList();
            ViewBag.ListOfChannelElement = listOfChannelElement;


            var listOfChannelType = PromotionContentService.GetChannelTypeList();
            ViewBag.ListOfChannelType = listOfChannelType;


            if (promotionContentId > 0)
            {
                promotionContentModel = PromotionContentService.GetPromotionContent(promotionContentId);
            }

            return View("Partial/Form/AddEditPromotionContentForm", promotionContentModel);
        }

        private PromotionContentElementViewModel MapToPromotionContentElementModel(bool owned, int promotionContentElementId, int promotionContentId)
        {
            int channelElement;
            int channelType;
            string filename;
            string filenameTitle;
            string anchorLink;

            string status = owned ? "Owned" : "New";

            Int32.TryParse(Request.Params[status + "ChannelElement" + promotionContentElementId], out channelElement);
            Int32.TryParse(Request.Params[status + "ChannelType" + promotionContentElementId], out channelType);
            filename = Request.Params[status + "Filename" + promotionContentElementId];
            filenameTitle = Request.Params[status + "FilenameTitle" + promotionContentElementId];
            anchorLink = Request.Params[status + "AnchorLink" + promotionContentElementId];

            var promoContentElementView = new PromotionContentElementViewModel()
            {
                PromotionContentId = promotionContentId,
                ChannelType = (ChannelType)channelType,
                ChannelElement = (ChannelElement)channelElement,
                Filename = filename,
                FilenameTitle = filenameTitle,
                AnchorLink = anchorLink
            };
            return promoContentElementView;
        }

        [ValidateInput(false)]
        public ActionResult AddEditPromotionContent(PromotionContentViewModel promotionContentModel)
        {
            bool isAddOrEdit = false;
            bool isJson = false;
            dynamic message;

            if (promotionContentModel.IsModelValid(ModelState))
            {

                if (promotionContentModel.PromotionContentId == 0)
                {
                    int promotionContentId = PromotionContentService.AddPromotionContent(promotionContentModel);
                    if (promotionContentId > 0)
                    {
                        promotionContentModel.PromotionContentId = promotionContentId;
                        isAddOrEdit = true;
                    }
                }
                else
                {
                    isAddOrEdit = PromotionContentService.EditPromotionContent(promotionContentModel);
                }

                // Add Delete Items
                if (isAddOrEdit == true)
                {
                    promotionContentModel.PromotionContentElements = PromotionContentService.GetPromotionContentElements(promotionContentModel.PromotionContentId);
                    List<PromotionContentElementViewModel> promotionContentElementToCreate = new List<PromotionContentElementViewModel>();

                    foreach (var item in promotionContentModel.PromotionContentElements)
                    {
                        // to modify
                        var promotionContentElementModel = MapToPromotionContentElementModel(true, item.PromotionContentElementId, promotionContentModel.PromotionContentId);

                        if (promotionContentElementModel.IsModelValid(ModelState))
                        {
                            promotionContentElementToCreate.Add(promotionContentElementModel);
                        }
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        var promotionContentElementModel = MapToPromotionContentElementModel(false, i, promotionContentModel.PromotionContentId);
                        if (promotionContentElementModel.IsModelValid(ModelState))
                        {
                            promotionContentElementToCreate.Add(promotionContentElementModel);
                        }
                    }
                    PromotionContentService.DeletePromotionContentElements(promotionContentModel.PromotionContentId);
                    PromotionContentService.AddPromotionContentElements(promotionContentElementToCreate);
                    //UberStrikeCacheInvalidation.InvalidateBundles(BuildType);
                }
                message = isAddOrEdit ? "Promotion updated" : "Error occured";
            }
            else
            {
                message = ModelState.SelectMany(ms => ms.Value.Errors).Select(ms => ms.ErrorMessage);
                isJson = true;
            }

            var json = new JsonResult() { Data = new { isAddOrEdit = isAddOrEdit, message = message, isJson = isJson } };
            return json;
        }

        public ActionResult DeletePromotionContent(int promotionContentId)
        {
            var isDeleted = PromotionContentService.DeletePromotionContent(promotionContentId);
            var json = new JsonResult() { Data = new { isDeleted = isDeleted } };
            return json;
        }
    }
}
