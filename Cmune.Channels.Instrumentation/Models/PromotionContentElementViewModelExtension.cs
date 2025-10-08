using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UberStrike.Core.ViewModel;
using Cmune.DataCenter.Common.Entities;
using System.Web.Mvc;

namespace Cmune.Channels.Instrumentation.Models
{
    public static class PromotionContentElementViewModelExtension
    {
        public static bool IsModelValid(this PromotionContentElementViewModel promotionContentElementViewModel, ModelStateDictionary modelStateDictionary)
        {
            bool isValid = false;
            modelStateDictionary.Clear();

            if (String.IsNullOrEmpty(promotionContentElementViewModel.FilenameTitle))
            {
                modelStateDictionary.AddModelError("FilenameTitle", "FilenameTitle Required");
            }
            if (promotionContentElementViewModel.PromotionContentId == 0)
            {
                modelStateDictionary.AddModelError("PromotionContentId", "PromotionContentId Required");
            }

            isValid = !(modelStateDictionary.SelectMany(ms => ms.Value.Errors).Count() > 0);
            return isValid;
        }
    }
}