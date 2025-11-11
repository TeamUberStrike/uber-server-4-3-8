using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Utils.Extensions;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Channels.Instrumentation.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Common.Entities;
using System.Web.Script.Serialization;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class DropDownListHelper
    {
        public static List<SelectListItem> GetDropDownListItems(this HtmlHelper html, List<SelectListItem> selectListItems, string selectValue = "")
        {
            if (selectValue != string.Empty)
                selectListItems.SetSelectValue(selectValue);
            return selectListItems;
        }

        public static List<SelectListItem> GetDropDownListItems(this HtmlHelper html, List<string> listOfItems, List<string> listOfValues)
        {
            var listOfSelectListItem = new List<SelectListItem>();
            if (listOfValues.Count < listOfItems.Count)
                return null;
            for (int i = 0; i < listOfItems.Count; i++)
            {
                listOfSelectListItem.Add(new SelectListItem() { Value = listOfValues[i], Text = listOfItems[i] });
            }
            return listOfSelectListItem;
        }

        public static List<SelectListItem> GetDropDownListItems(this HtmlHelper html, Dictionary<string, string> values)
        {
            var listOfSelectListItem = new List<SelectListItem>();
            if (values.Count < 0)
                return null;
            foreach (var value in values)
            {
                listOfSelectListItem.Add(new SelectListItem() { Value = value.Value, Text = value.Key });
            }
            return listOfSelectListItem;
        }

        public static List<SelectListItem> GetDropDownListYesNoItems(this HtmlHelper html, string selectedValue)
        {
            var listOfSelectListItem = new List<SelectListItem>();
            listOfSelectListItem.Add(new SelectListItem() { Value = "true", Text = "Yes", Selected = "true" == selectedValue.ToLower() });
            listOfSelectListItem.Add(new SelectListItem() { Value = "false", Text = "No", Selected = "false" == selectedValue.ToLower() });
            return listOfSelectListItem;
        }

        public static List<SelectListItem> GetDropDownListQuickItemLogicItems(this HtmlHelper html, string logic)
        {
            var listOfSelectListItem = new List<SelectListItem>();
            foreach (QuickItemLogic p in Enum.GetValues(typeof(QuickItemLogic)))
            {
                listOfSelectListItem.Add(new SelectListItem() { Value = p.ToString(), Text = p.ToString(), Selected = p.ToString() == logic });
            }
            return listOfSelectListItem;
        }

        public static List<SelectListItem> GetItemTypeList(this HtmlHelper html, int itemTypeId, bool withAllSelection = true)
        {
            var itemTypeList = new List<SelectListItem>();

            List<ItemType> itemTypes = ItemService.LoadItemType();

            if (withAllSelection == true)
            {
                SelectListItem listItemAllItems = new SelectListItem() { Text = "All types", Value = "0", Selected = itemTypeId.ToString() == "0" };
                itemTypeList.Add(listItemAllItems);
            }

            foreach (ItemType itemType in itemTypes)
            {
                SelectListItem listItem = new SelectListItem() { Text = itemType.Name, Value = itemType.ItemTypeId.ToString(), Selected = itemTypeId.ToString() == itemType.ItemTypeId.ToString() };
                itemTypeList.Add(listItem);
            }

            return itemTypeList;
        }

        public static List<SelectListItem> GetItemClassList(this HtmlHelper html, int typeId, int classId, bool withAllSelection = true)
        {
            Dictionary<int, string> itemTypesOrdered = new Dictionary<int, string>();
            List<ItemClass> itemClasses = new List<ItemClass>();
            bool displayAllClasses = false;
            var itemClassList = new List<SelectListItem>();

            Dictionary<int, List<ItemClass>> itemClassesOrdered = ItemService.LoadItemClassOrdered();
            if (itemClassesOrdered.ContainsKey(typeId))
            {
                itemClasses = itemClassesOrdered[typeId];
            }
            else
            {
                displayAllClasses = true;

                foreach (KeyValuePair<int, List<ItemClass>> itemClass in itemClassesOrdered)
                {
                    itemClasses.AddRange(itemClass.Value);
                }

                List<ItemType> itemTypes = ItemService.LoadItemType();

                foreach (ItemType itemType in itemTypes)
                {
                    itemTypesOrdered.Add(itemType.ItemTypeId, itemType.Name);
                }
            }

            if (withAllSelection == true)
            {
                SelectListItem listItemAllClasses = new SelectListItem() { Text = "All Classes", Value = "0", Selected = classId.ToString() == "0" };
                itemClassList.Add(listItemAllClasses);
            }

            foreach (ItemClass itemClass in itemClasses)
            {
                string typeName = String.Empty;

                if (displayAllClasses)
                {
                    if (itemTypesOrdered.ContainsKey(itemClass.ItemTypeId))
                    {
                        typeName = itemTypesOrdered[itemClass.ItemTypeId] + " - ";
                    }
                }

                typeName += itemClass.Name;

                SelectListItem listItem = new SelectListItem() { Text = typeName, Value = itemClass.ItemClassId.ToString(), Selected = itemClass.ItemClassId == classId };
                itemClassList.Add(listItem);
            }
            return itemClassList;
        }

        public static List<SelectListItem> GetPurchaseTypeList(this HtmlHelper html, PurchaseType purchaseType)
        {
            var purchaseTypeList = new List<SelectListItem>();
            foreach (var item in AdminCache.GeneratePurchaseTypeDropDownListItems())
            {
                purchaseTypeList.Add(new SelectListItem() { Text = item.Text, Value = item.Value, Selected = item.Value.ToString() == purchaseType.ToString() ? true : false });
            }
            return purchaseTypeList;
        }

        public static List<SelectListItem> GetBuyingDurationType(this HtmlHelper html, BuyingDurationType buyingDurationType)
        {
            var listOfSelectListItem = new List<SelectListItem>();
            foreach (BuyingDurationType p in Enum.GetValues(typeof(BuyingDurationType)))
            {
                listOfSelectListItem.Add(new SelectListItem() { Value = p.ToString(), Text = p.ToString(), Selected = p.ToString() == buyingDurationType.ToString() });
            }
            return listOfSelectListItem;
        }

        public static List<SelectListItem> GetBundleCategoryType(this HtmlHelper html, BundleCategoryType bundleCategoryType)
        {
            var listeOfSelectListItem = new List<SelectListItem>();
            foreach (BundleCategoryType p in Enum.GetValues(typeof(BundleCategoryType)))
            {
                if (p != 0)
                    listeOfSelectListItem.Add(new SelectListItem() { Value = p.ToString(), Text = p.ToString(), Selected = p.ToString() == bundleCategoryType.ToString() });
            }
            return listeOfSelectListItem;
        }

        public static string GetBuyingDurationTypeTypeJson(this HtmlHelper html)
        {
            var jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(html.GetBuyingDurationType(BuyingDurationType.SevenDays));
        }

        public static List<SelectListItem> GetUberStrikeCurrencyType(this HtmlHelper html, UberStrikeCurrencyType uberStrikeCurrencyType)
        {
            var listeOfSelectListItem = new List<SelectListItem>();
            foreach (UberStrikeCurrencyType p in Enum.GetValues(typeof(UberStrikeCurrencyType)))
            {
                listeOfSelectListItem.Add(new SelectListItem() { Value = p.ToString(), Text = p.ToString(), Selected = p.ToString() == uberStrikeCurrencyType.ToString() });
            }
            return listeOfSelectListItem;
        }

        public static List<SelectListItem> GetItems(this HtmlHelper html, List<ItemView> itemViews, string selectedValue = "")
        {
            var listeOfSelectListItem = new List<SelectListItem>();
            listeOfSelectListItem.Add(new SelectListItem() { Value = "0", Text = "-------------" });
            foreach (var itemView in (List<ItemView>)itemViews)
            {
                listeOfSelectListItem.Add(new SelectListItem() { Value = itemView.ItemId.ToString(), Text = itemView.Name.ToString(), Selected = !String.IsNullOrEmpty(selectedValue) && (itemView.ItemId.ToString() == selectedValue) });
            }
            return listeOfSelectListItem;
        }

        public static string GetItemsJson(this HtmlHelper html, List<ItemView> itemViews)
        {
            var jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(html.GetItems(itemViews));
        }
    }
}