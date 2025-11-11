using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace Cmune.Channels.Instrumentation.Utils.Extensions
{
    public static class ListSelectListItemExtension {

        public static List<SelectListItem> SetSelectValue(this List<SelectListItem> listSelectListItem, string selectedValue)
        {
            foreach (var item in listSelectListItem)
                item.Selected = item.Value == selectedValue;
            return listSelectListItem;
        }
    }
}
