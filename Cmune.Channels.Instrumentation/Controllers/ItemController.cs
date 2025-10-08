using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Helper;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ItemController : BaseController
    {
        #region Properties and Parameters
        HtmlHelper html;

        public int ItemId { get; set; }
        private char _iDsSeparator = ',';

        #endregion

        #region Construtors

        public ItemController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Items;
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            html = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "omg"), new ViewDataDictionary(), new TempDataDictionary(), new System.IO.StringWriter()), new ViewPage());
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            ViewBag.SubActiveTab = CmuneMenu.ItemSubTabs.Catalog;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.ItemSubTabs.Catalog);

            int initType = 0;
            int initClass = 0;
            int applicationId = CommonConfig.ApplicationIdUberstrike;

            ViewData["applicationList"] = GetApplicationList(applicationId);
            ViewData["itemTypeList"] = html.GetItemTypeList(initType);
            ViewData["itemClassList"] = html.GetItemClassList(initType, initClass);

            List<SelectListItem> searchModesDdl = new List<SelectListItem>();
            List<ItemSearchMode> searchModes = EnumUtilities.IterateEnum<ItemSearchMode>();

            foreach (ItemSearchMode searchMode in searchModes)
            {
                string text = searchMode.ToString();

                if (searchMode == ItemSearchMode.IsFeatured)
                {
                    text = "Sale";
                }
                else if (searchMode == ItemSearchMode.IsPopular)
                {
                    text = "Hot";
                }

                searchModesDdl.Add(new SelectListItem { Value = ((int)searchMode).ToString(), Text = text, Selected = (searchMode == ItemSearchMode.None) });
            }

            ViewData["itemStatus"] = searchModesDdl;

            GetShopItemType();

            return View();
        }

        public ActionResult ItemManagement()
        {
            ViewBag.SubActiveTab = CmuneMenu.ItemSubTabs.Management;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.ItemSubTabs.Management);

            #region Synchronize items

            bool isSynchronizable = false;
            string databaseConfiguration = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSource);

            // We can't sync from prod
            if (!databaseConfiguration.Equals(DatabaseDeployment.Prod))
            {
                if (databaseConfiguration.Equals(DatabaseDeployment.Dev))
                {
                    // if the configuration is dev, we push database to staging
                    ViewData["synchronizationType"] = DatabaseDeployment.Staging.ToUpper();
                }
                else if (databaseConfiguration.Equals(DatabaseDeployment.Staging))
                {
                    //  otherwise we put the database to Prod
                    ViewData["synchronizationType"] = DatabaseDeployment.Prod.ToUpper();
                }

                isSynchronizable = true;
            }

            ViewData["isSynchronizable"] = isSynchronizable;

            #endregion

            return View();
        }

        public ActionResult Comparison(int itemClassId = 0)
        {
            ViewBag.SubActiveTab = CmuneMenu.ItemSubTabs.Comparison;
            ViewBag.Title += String.Format(" | {0}", CmuneMenu.ItemSubTabs.Comparison);


            ViewBag.ItemTypeList = html.GetItemTypeList(0, false);

            return View();
        }

        public ActionResult ComparisonPerType(int itemTypeId)
        {
            var cmuneDataContext = new CmuneDataContext(ConfigurationUtilities.ReadConfigurationManager("ItemsSyncCmuneConnectionStringProd"));
            var uberstrikeDataContext = new UberstrikeDataContext(ConfigurationUtilities.ReadConfigurationManager("ItemsSyncUberStrikeConnectionStringProd"));

            var uberstrikeDevItems = UberstrikeShop.GetAllItemsByType(new CmuneDataContext(), new UberstrikeDataContext(), itemTypeId);
            var uberstrikeProdItems = UberstrikeShop.GetAllItemsByType(cmuneDataContext, uberstrikeDataContext, itemTypeId);

            ViewBag.ItemTypeId = itemTypeId;
            return PartialView("Partial/ComparisonTable", new ComparisonViewModel() { UberstrikeItemShopView = uberstrikeDevItems, UberstrikeItemShopViewToCompare = uberstrikeProdItems });
        }

        #endregion

        #region Methods

        List<SelectListItem> GetApplicationList(int applicationId, bool withAllSelection = true)
        {
            List<SelectListItem> applicationList = new List<SelectListItem>();
            List<Application> applications = ApplicationDeployment.GetApplications();

            if (withAllSelection == true)
            {
                SelectListItem listItemAllApplications = new SelectListItem() { Text = "All applications", Value = "0", Selected = applicationId.ToString() == "0" };
                applicationList.Add(listItemAllApplications);
            }

            foreach (Application application in applications)
            {
                SelectListItem listItem = new SelectListItem() { Text = application.Name, Value = application.ApplicationId.ToString(), Selected = applicationId.ToString() == application.ApplicationId.ToString() };
                applicationList.Add(listItem);
            }

            return applicationList;
        }

        List<ItemView> GetItemList(int applicationId, int itemId, string itemName, int typeId, int classId, ItemSearchMode searchMode)
        {
            List<ItemView> items = new List<ItemView>();

            if (itemId != 0)
            {
                ItemView item = CmuneItem.GetItemView(itemId);

                if (item != null)
                {
                    items.Add(item);
                }
            }
            else if (!itemName.IsNullOrFullyEmpty())
            {
                items = CmuneItem.GetItemByName(applicationId, itemName);
            }
            else
            {
                if (classId != 0)
                {
                    items = CmuneItem.GetItemByClass(applicationId, classId);
                }
                else if (typeId != 0)
                {
                    items = CmuneItem.GetItemByType(applicationId, typeId);
                }
                else
                {
                    switch (searchMode)
                    {
                        case ItemSearchMode.None:
                            items = CmuneItem.GetItems(applicationId);
                            break;
                        case ItemSearchMode.IsFeatured:
                            items = CmuneItem.SearchItemByIsFeatured(applicationId);
                            break;
                        case ItemSearchMode.IsNew:
                            items = CmuneItem.SearchItemByIsNew(applicationId);
                            break;
                        case ItemSearchMode.IsPopular:
                            items = CmuneItem.SearchItemByIsPopular(applicationId);
                            break;
                        default:
                            throw new ArgumentException(String.Format("This ItemSearchMode ({0}) is not implemented yet", searchMode), "searchMode");
                    }
                }
            }

            return items;
        }

        public void GetShopItemType()
        {
            ViewData["ShopItemTypeFunctional"] = (int)ShopItemType.UberstrikeFunctional;
            ViewData["ShopItemTypeGear"] = (int)ShopItemType.UberstrikeGear;
            ViewData["ShopItemTypeQuickUse"] = (int)ShopItemType.UberstrikeQuickUse;
            ViewData["ShopItemTypeSpecial"] = (int)ShopItemType.UberstrikeSpecial;
            ViewData["ShopItemTypeWeapon"] = (int)ShopItemType.UberstrikeWeapon;
            ViewData["ShopItemTypeWeaponMod"] = (int)ShopItemType.UberstrikeWeaponMod;
        }

        protected void InvalidateItemShop()
        {
            // TODO: very dirty (my bad), we need a cleaner way to determine the environment
            string buildIndex = ConfigurationUtilities.ReadConfigurationManager("NLogBuild");
            BuildType buildType;
            EnumUtilities.TryParseEnumByValue(buildIndex, out buildType);
            UberStrikeCacheInvalidation.InvalidateItemShop(buildType, InstrumentationCacheInvalidation.InstrumentationServicesUrls);
            UberStrikeCacheInvalidation.InvalidateItemShop(buildType);
        }

        #region Display Data

        void DisplayItem(ItemView item)
        {
            // if it isn't a creation
            ShopItemType itemType = (ShopItemType)item.TypeId;
            UberstrikeGearConfigView gearConfig = new UberstrikeGearConfigView();
            UberstrikeWeaponConfigView weaponConfig = new UberstrikeWeaponConfigView();
            ItemQuickUseConfigView quickUseConfig = new ItemQuickUseConfigView();
            UberstrikeWeaponModConfigView weaponModConfig = new UberstrikeWeaponModConfigView();
            UberstrikeFunctionalConfigView functionalConfig = new UberstrikeFunctionalConfigView();
            UberstrikeSpecialConfigView specialConfig = new UberstrikeSpecialConfigView();
            if (item.ItemId != 0)
            {
                switch (itemType)
                {
                    case ShopItemType.UberstrikeFunctional:
                        functionalConfig = UberstrikeShop.GetFunctionalConfig(item.ItemId, false);
                        break;
                    case ShopItemType.UberstrikeGear:
                        gearConfig = UberstrikeShop.GetGearConfig(item.ItemId, false);
                        if (gearConfig == null)
                            gearConfig = new UberstrikeGearConfigView();
                        break;
                    case ShopItemType.UberstrikeQuickUse:
                        quickUseConfig = UberstrikeShop.GetQuickUseConfig(item.ItemId, false);
                        break;
                    case ShopItemType.UberstrikeSpecial:
                        specialConfig = UberstrikeShop.GetSpecialConfig(item.ItemId, false);
                        break;
                    case ShopItemType.UberstrikeWeapon:
                        weaponConfig = UberstrikeShop.GetWeaponConfig(item.ItemId, false);
                        break;
                    case ShopItemType.UberstrikeWeaponMod:
                        weaponModConfig = UberstrikeShop.GetWeaponModConfig(item.ItemId, false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format("Unexpected item type: {0}", item.TypeId));
                }
            }
            ViewBag.ItemGearConfigViewModel = gearConfig;
            ViewBag.ItemQuickUseConfigViewModel = quickUseConfig;
            ViewBag.ItemWeaponConfigViewModel = weaponConfig;
        }

        protected string DisplayField(int value)
        {
            string result = String.Empty;

            if (value != CommonConfig.ItemMallFieldDisable)
            {
                result = value.ToString();
            }

            return result;
        }


        #endregion

        #region Read data

        protected int ReadField(string value)
        {
            int result = CommonConfig.ItemMallFieldDisable;

            if (!value.IsNullOrFullyEmpty() && value.IsNumeric())
            {
                int tmpResult = CommonConfig.ItemMallFieldDisable;
                Int32.TryParse(value, out tmpResult);

                if (tmpResult >= 0)
                {
                    result = tmpResult;
                }
            }

            return result;
        }


        #endregion Read data

        [HttpGet]
        public ActionResult HttpGetItemClassList(string typeIdstr, bool withAllSelection)
        {
            var typeId = Int32.Parse(typeIdstr);
            var itemClassList = html.GetItemClassList(typeId, 0, withAllSelection);
            var json = new JsonResult();
            json.Data = new { itemClassList = itemClassList };
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpPost]
        public JsonResult SynchronizeGo()
        {
            string message = string.Empty;
            bool isFullSynchronized = false;
            var itemProcess = new ItemProcessBusiness();

            string databaseConfiguration = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSource);

            // We can't sync from prod
            if (!databaseConfiguration.Equals(DatabaseDeployment.Prod))
            {
                if (databaseConfiguration.Equals(DatabaseDeployment.Dev))
                {
                    // if the configuration is dev, we push database to staging
                    isFullSynchronized = itemProcess.SyncItems(ConfigurationUtilities.ReadConfigurationManager("ItemsSyncCmuneConnectionStringStaging"), ConfigurationUtilities.ReadConfigurationManager("ItemsSyncUberStrikeConnectionStringStaging"));
                    UberStrikeCacheInvalidation.InvalidateItemShop(BuildType.Staging);
                    UberStrikeCacheInvalidation.InvalidateItemShop(BuildType.Staging, InstrumentationCacheInvalidation.InstrumentationServicesUrls);
                }
                else if (databaseConfiguration.Equals(DatabaseDeployment.Staging))
                {
                    //  otherwise we put the database to Prod
                    isFullSynchronized = itemProcess.SyncItems(ConfigurationUtilities.ReadConfigurationManager("ItemsSyncCmuneConnectionStringProd"), ConfigurationUtilities.ReadConfigurationManager("ItemsSyncUberStrikeConnectionStringProd"));
                    UberStrikeCacheInvalidation.InvalidateItemShop(BuildType.Prod);
                    UberStrikeCacheInvalidation.InvalidateItemShop(BuildType.Prod, InstrumentationCacheInvalidation.InstrumentationServicesUrls);
                }
            }

            message = isFullSynchronized ? "Items are fully synchronized" : "Something wrong happened: ask a dev to investigate the issue and ensure data integrity.";

            var json = new JsonResult() { Data = new { isFullSynchronized = isFullSynchronized, message = message } };
            return json;
        }

        [HttpPost]
        public JsonResult AttributeItemsToMembers(FormCollection form)
        {
            string message = "";
            bool isAttributed = false;
            DateTime now = DateTime.Now;
            List<ItemView> itemsView = new List<ItemView>();
            List<int> canBeAttributeItemIds = new List<int>();
            List<int> nonExistingItemsIds = new List<int>();
            List<Member> members = new List<Member>();
            List<int> nonExistingMembersCmids = new List<int>();

            List<int> itemsIds = ExtractIds(form["ItemsIDTextBox"]);
            List<int> cmids = ExtractIds(form["CmidsTextBox"]);
            string selectedBuyingDurationType = form["DurationDropDownList"];
            BuyingDurationType buyingDurationType = BuyingDurationType.OneDay;
            bool isDurationEnum = EnumUtilities.TryParseEnumByValue(selectedBuyingDurationType, out buyingDurationType);

            // We need to check that the items and the members are existing
            foreach (int itemId in itemsIds)
            {
                ItemView currentItemView = CmuneItem.GetItemView(itemId);

                if (currentItemView != null)
                {
                    canBeAttributeItemIds.Add(itemId);
                    itemsView.Add(currentItemView);
                }
                else
                    nonExistingItemsIds.Add(itemId);
            }

            foreach (int cmid in cmids)
            {
                Member currentMember = CmuneMember.GetMember(cmid);

                if (currentMember != null)
                    members.Add(currentMember);
                else
                    nonExistingMembersCmids.Add(cmid);
            }

            foreach (Member member in members)
            {
                // TODO: We can use the return to display feedback
                if (!buyingDurationType.Equals(BuyingDurationType.Permanent))
                {
                    CmuneEconomy.AddItemsToInventory(member.CMID, itemsView, buyingDurationType, now, false);
                    isAttributed = true;
                }
                else
                {
                    CmuneEconomy.AddItemsToInventoryPermanently(member.CMID, canBeAttributeItemIds, now);
                    isAttributed = true;
                }
            }

            if (itemsView.Count > 0 && members.Count > 0)
            {
                // We bought at least one item for one player
                message = "<p>The items have been attributed.</p>";

                if (nonExistingItemsIds.Count > 0 || nonExistingMembersCmids.Count > 0)
                {
                    message += "<ul style=\"margin:0 0 15px 0; text-align: left;\">";
                    if (nonExistingItemsIds.Count > 0)
                        message += "<li>The following items ID are not existing: " + TextUtilities.Join(", ", nonExistingItemsIds) + "</li>";

                    if (nonExistingMembersCmids.Count > 0)
                        message += "<li>The following CMIDs are not existing: " + TextUtilities.Join(", ", nonExistingMembersCmids) + "</li>";
                    message += "</ul>";
                }
            }
            else
            {
                message = "<p style=\"margin:0;\">We couldn't attribute items for the following reasons:</p><ul style=\"margin:0 0 15px 0; text-align: left;\">";
                if (itemsView.Count == 0)
                    message += "<li>There is no item matching with what you entered</li>";
                if (members.Count == 0)
                    message += "<li>There is no member matching with what you entered</li>";
                message += "</ul>";
            }

            var json = new JsonResult() { Data = new { isAttributed = isAttributed, message = message } };
            return json;
        }

        List<int> ExtractIds(string inputValue)
        {
            List<int> ids = new List<int>();

            inputValue = System.Text.RegularExpressions.Regex.Replace(inputValue, @"\p{Z}", String.Empty);

            string[] idsString = inputValue.Split(_iDsSeparator);

            for (int i = 0; i < idsString.Length; i++)
            {
                int id = 0;
                bool isIdInt = Int32.TryParse(idsString[i], out id);
                if (isIdInt)
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        public Dictionary<string, string> ReadCustomProperties(FormCollection form)
        {
            int i = 0;
            Dictionary<string, string> customProperties = new Dictionary<string, string>();
            while (!string.IsNullOrEmpty(form["CustomPropertyName" + i]))
            {
                if (!string.IsNullOrEmpty(form["CustomPropertyValue" + i]))
                    customProperties.Add(form["CustomPropertyName" + i], form["CustomPropertyValue" + i]);
                i++;
            }

            return customProperties;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(FormCollection form, ItemView itemView, UberstrikeGearConfigView itemGearConfigView, UberstrikeWeaponConfigView itemWeaponConfigView, ItemQuickUseConfigView itemQuickUseConfigView)
        {
            string message = "";
            bool isModified = false;

            int previousTypeId = 0;
            Int32.TryParse(form["CurrentItemTypeId"], out previousTypeId);
            ShopItemType previousItemType = (ShopItemType)previousTypeId;

            if (itemView.TypeId == 0) // that mean dropdownlist is disabled and value are not modified
                itemView.TypeId = previousTypeId;

            bool isTypeChanged = false;
            if (itemView.TypeId != previousTypeId)
            {
                isTypeChanged = true;
            }

            ShopItemType itemType = (ShopItemType)itemView.TypeId;
            itemView.CustomProperties = ReadCustomProperties(form);
            switch (itemType)
            {
                case ShopItemType.UberstrikeWeapon:
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToWeapon(itemView, previousItemType, itemWeaponConfigView.LevelRequired, itemWeaponConfigView);
                    else
                        UberstrikeShop.ModifyWeaponItem(itemView, itemWeaponConfigView.LevelRequired, itemWeaponConfigView);
                    isModified = true;
                    break;
                case ShopItemType.UberstrikeGear:
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToGear(itemView, previousItemType, itemGearConfigView.LevelRequired, itemGearConfigView);
                    else
                        UberstrikeShop.ModifyGearItem(itemView, itemGearConfigView.LevelRequired, itemGearConfigView);
                    isModified = true;
                    break;
                case ShopItemType.UberstrikeFunctional:
                    UberstrikeFunctionalConfigView functionalConfigView = new UberstrikeFunctionalConfigView();
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToFunctional(itemView, previousItemType, functionalConfigView.LevelRequired, functionalConfigView);
                    else
                        UberstrikeShop.ModifyFunctionalItem(itemView, functionalConfigView.LevelRequired, functionalConfigView);
                    isModified = true;
                    break;
                case ShopItemType.UberstrikeQuickUse:
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToQuickUse(itemView, previousItemType, itemQuickUseConfigView.LevelRequired, itemQuickUseConfigView);
                    else
                        UberstrikeShop.ModifyQuickUseItem(itemView, itemQuickUseConfigView.LevelRequired, itemQuickUseConfigView);
                    isModified = true;
                    break;
                case ShopItemType.UberstrikeSpecial:
                    UberstrikeSpecialConfigView specialConfigView = new UberstrikeSpecialConfigView();
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToSpecial(itemView, previousItemType, specialConfigView.LevelRequired, specialConfigView);
                    else
                        UberstrikeShop.ModifySpecialItem(itemView, specialConfigView.LevelRequired, specialConfigView);
                    isModified = true;
                    break;
                case ShopItemType.UberstrikeWeaponMod:
                    UberstrikeWeaponModConfigView weaponModConfigView = new UberstrikeWeaponModConfigView();
                    if (isTypeChanged)
                        UberstrikeShop.ChangeTypeToWeaponMod(itemView, previousItemType, weaponModConfigView.LevelRequired, weaponModConfigView);
                    else
                        UberstrikeShop.ModifyWeaponModItem(itemView, weaponModConfigView.LevelRequired, weaponModConfigView);
                    isModified = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Unexpected item type: {0}", itemType));
            }

            InvalidateItemShop();

            message = isModified ? "Item has been modified" : "Error occured";

            var json = new JsonResult()
            {
                Data = new { isModified = isModified, message = message }
            };

            return json;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Add(FormCollection form, ItemView itemView, UberstrikeGearConfigView uberstrikeGearConfigView, UberstrikeWeaponConfigView uberstrikeWeaponConfigView, ItemQuickUseConfigView itemQuickUseConfigView)
        {
            string message = "";
            bool isInserted = false;

            itemView.CustomProperties = ReadCustomProperties(form);
            if (base.CmuneEnvironnement == "dev")
            {
                switch ((ShopItemType)itemView.TypeId)
                {
                    case ShopItemType.UberstrikeWeapon:
                        UberstrikeShop.CreateWeaponItem(itemView, uberstrikeWeaponConfigView.LevelRequired, uberstrikeWeaponConfigView);
                        isInserted = true;
                        break;
                    case ShopItemType.UberstrikeGear:
                        UberstrikeShop.CreateGearItem(itemView, uberstrikeGearConfigView.LevelRequired, uberstrikeGearConfigView);
                        isInserted = true;
                        break;
                    case ShopItemType.UberstrikeFunctional:
                        UberstrikeFunctionalConfigView functionalConfigView = new UberstrikeFunctionalConfigView();
                        UberstrikeShop.CreateFunctionalItem(itemView, functionalConfigView.LevelRequired, functionalConfigView);
                        isInserted = true;
                        break;
                    case ShopItemType.UberstrikeQuickUse:
                        UberstrikeShop.CreateQuickUseItem(itemView, itemQuickUseConfigView.LevelRequired, itemQuickUseConfigView);
                        isInserted = true;
                        break;
                    case ShopItemType.UberstrikeSpecial:
                        UberstrikeSpecialConfigView specialConfigView = new UberstrikeSpecialConfigView();
                        UberstrikeShop.CreateSpecialItem(itemView, specialConfigView.LevelRequired, specialConfigView);
                        isInserted = true;
                        break;
                    case ShopItemType.UberstrikeWeaponMod:
                        UberstrikeWeaponModConfigView weaponModConfigView = new UberstrikeWeaponModConfigView();
                        UberstrikeShop.CreateWeaponModItem(itemView, weaponModConfigView.LevelRequired, weaponModConfigView);
                        isInserted = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format("Unexpected item type: {0}", itemView.TypeId));
                }

                InvalidateItemShop();
            }

            message = isInserted ? "Item has been created" : "Error occured";
            var json = new JsonResult()
            {
                Data = new { isInserted = isInserted, message = message }
            };
            return json;
        }

        public ActionResult LoadAddOrEditForm(string actionTodo, string itemId)
        {
            ViewData["MaximumDurationLiteral"] = CommonConfig.ItemMaximumDurationInDays.ToString();
            ItemId = Int32.Parse(itemId);
            ItemView item = new ItemView();
            switch (actionTodo)
            {
                case "add":
                    ViewData["ControlTitleLiteral"] = "Add an item";
                    ViewData["DisableItemTypeDropDownList"] = false;
                    ViewData["Add"] = true;
                    item = CmuneItem.CreateDefaultInstance();
                    DisplayItem(item);
                    break;
                case "edit":
                    ViewData["ControlTitleLiteral"] = "Add an item";
                    ViewData["DisableItemTypeDropDownList"] = true;
                    ViewData["Add"] = false;
                    item = CmuneItem.GetItem(ItemId).ToItemView();
                    DisplayItem(item);
                    break;
            }
            ViewBag.ItemView = item;
            ViewBag.ItemViewCustomProperties = CmuneItem.ConvertCustomPropertiesToString(item.CustomProperties);
            return View("Partial/Form/AddOrEditForm");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetItems(FormCollection form)
        {
            int applicationid = Int32.Parse(form["ApplicationDropDownList"]);
            int itemId = form["itemIdTextBox"].IsNullOrFullyEmpty() ? 0 : Int32.Parse(form["itemIdTextBox"]);
            string itemName = form["itemNameTextbox"];
            int typeId = form["TypeDropDownList"].IsNullOrFullyEmpty() ? 0 : Int32.Parse(form["TypeDropDownList"]);
            int classId = form["ClassDropDownList"].IsNullOrFullyEmpty() ? 0 : Int32.Parse(form["ClassDropDownList"]);

            ItemSearchMode searchMode = ItemSearchMode.None;
            EnumUtilities.TryParseEnumByValue(form["itemStatus"], ItemSearchMode.None, out searchMode);

            // get pagination optional
            int pageIndex;
            pageIndex = Int32.TryParse(form["selectedPage"], out pageIndex) == true ? pageIndex : 1;

            ViewBag.ItemTypesName = ItemService.LoadItemType().ToDictionary(i => i.ItemTypeId, i => i.Name);
            ViewBag.ItemClassName = ItemService.LoadItemClass().ToDictionary(i => i.ItemClassId, i => i.Name);

            List<ItemView> itemsView = GetItemList(applicationid, itemId, itemName, typeId, classId, searchMode);

            PaginationModel paginationModel = new PaginationModel(itemsView.Count, pageIndex);
            ViewBag.PaginationModel = paginationModel;

            return PartialView("Partial/Items", paginationModel.GetCurrentPage(itemsView));
        }

        #region Catalog

        #endregion

        #region Management

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetItemId(string itemName)
        {
            int itemId = CmuneItem.GetItemIdByName(itemName);

            var json = new JsonResult() { Data = new { ItemId = itemId } };
            return json;
        }

        [HttpPost]
        public JsonResult ApplyGlobalDiscount(int discount)
        {
            bool isApplied = false;

            if (discount > 0 && discount < 100)
            {
                isApplied = CmuneItem.ApplyGlobalDiscount(CommonConfig.ApplicationIdUberstrike, discount);

                if (isApplied)
                {
                    InvalidateItemShop();
                }
            }

            var json = new JsonResult()
            {
                Data = new { IsApplied = isApplied }
            };

            return json;
        }

        #endregion

        #region XLXS export

        protected void WriteUberstrikeItemViewToCsv(UberstrikeItemView item, ref CmuneCsv csv)
        {
            //remove the deprecated fields (hardcoded to true)
            csv.AddRowToCsv(item.ItemId);
            csv.AddRowToCsv(item.Name);
            csv.AddRowToCsv(item.CreditsPerDay);
            csv.AddRowToCsv(item.PointsPerDay);
            csv.AddRowToCsv(((UberstrikeItemType)item.TypeId).ToString());
            csv.AddRowToCsv(true);
            csv.AddRowToCsv(item.IsForSale);
            csv.AddRowToCsv(true);
            csv.AddRowToCsv(((UberstrikeItemClass)item.ClassId).ToString());
            csv.AddRowToCsv(true);
            csv.AddRowToCsv(item.PermanentCredits);
            csv.AddRowToCsv(true);
            csv.AddRowToCsv(item.Enable1Day);
            csv.AddRowToCsv(item.Enable7Days);
            csv.AddRowToCsv(item.Enable30Days);
            csv.AddRowToCsv(item.Enable90Days);
            csv.AddRowToCsv(item.LevelRequired);
            csv.AppendLine();
        }

        string GetShopsContent()
        {
            UberstrikeItemShopView shopView = UberstrikeShop.GetShop();
            CmuneCsv csv = new CmuneCsv(new List<string>{"Item Id",
                "Name",
                "Shop credits per day",
                "Shop points per day",
                "Type",
                "Is enable in shop",
                "Is for sale",
                "Is enabled in undergroud",
                "Class",
                "Underground credits per day",
                "Shop permanent credits",
                "Undergrounds permanents credits",
                "Enable 1 day",
                "Enable 7 day",
                "Enable 30 day",
                "Enable 90 day",
                "Level required"});

            foreach (UberstrikeItemFunctionalView item in shopView.FunctionalItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            foreach (UberstrikeItemGearView item in shopView.GearItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            foreach (UberstrikeItemQuickUseView item in shopView.QuickUseItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            foreach (UberstrikeItemSpecialView item in shopView.SpecialItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            foreach (UberstrikeItemWeaponView item in shopView.WeaponItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            foreach (UberstrikeItemWeaponModView item in shopView.WeaponModItems)
            {
                WriteUberstrikeItemViewToCsv(item, ref csv);
            }

            return csv.ToString();
        }

        public void GetShop()
        {
            string name = "shop.csv";

            var content = GetShopsContent();
            Response.AddHeader("Content-Type", "text/csv");
            Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
            Response.Write(content);
        }

        #endregion

        #endregion
    }
}