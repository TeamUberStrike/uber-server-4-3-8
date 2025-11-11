using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UberStrike.Core.ViewModel;
using System.Web.Mvc;

namespace Cmune.Channels.Instrumentation.Models
{
    public static class PromotionContentViewModelExtension
    {
        public static bool IsModelValid(this PromotionContentViewModel promotionContentViewModel, ModelStateDictionary modelStateDictionary)
        {
            bool isValid = false;
            modelStateDictionary.Clear();

            if (String.IsNullOrEmpty(promotionContentViewModel.Name))
            {
                modelStateDictionary.AddModelError("Name", "Name Required");
            }
            if (promotionContentViewModel.StartDate == DateTime.Now)
            {
                modelStateDictionary.AddModelError("StartDate", "StartDate Required");
            }
            if (promotionContentViewModel.EndDate == DateTime.Now)
            {
                modelStateDictionary.AddModelError("EndDate", "EndDate Required");
            }
            isValid = !(modelStateDictionary.SelectMany(ms => ms.Value.Errors).Count() > 0);

            return isValid;
        }

    }
}