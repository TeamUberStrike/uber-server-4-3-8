using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Cmune.Channels.Instrumentation.Business;
using Cmune.Channels.Instrumentation.Models;
using Cmune.Channels.Instrumentation.Models.Display;
using Cmune.Channels.Instrumentation.Models.Enums;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Forum.Business.Utils;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Utils;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Controllers
{
    [Authorize(Roles = MembershipRoles.Administrator)]
    public class ManagementController : BaseController
    {
        #region Properties

        public DateTime BatchFromDate { get; private set; }
        public DateTime BatchToDate { get; private set; }
        public DateTime UsedFromDate { get; private set; }
        public DateTime UsedToDate { get; private set; }
        public List<EpinView> EpinsList = new List<EpinView>();

        #endregion

        #region Constructors

        public ManagementController()
            : base()
        {
            ViewBag.Title = ViewBag.ActiveTab = CmuneMenu.MainTabs.Management;

            BatchFromDate = DateTime.Now.AddDays(-30);
            BatchToDate = DateTime.Now.AddDays(-1);

            UsedFromDate = DateTime.Now.AddDays(-30);
            UsedToDate = DateTime.Now.AddDays(-1);
        }

        #endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // User selected dates?
            if (Request.Params["batch_startdate"] != null)
                BatchFromDate = Convert.ToDateTime(Request.Params["batch_startdate"]);
            if (Request.Params["batch_enddate"] != null)
                BatchToDate = Convert.ToDateTime(Request.Params["batch_enddate"]);
            if (Request.Params["used_startdate"] != null)
                UsedFromDate = Convert.ToDateTime(Request.Params["used_startdate"]);
            if (Request.Params["used_enddate"] != null)
                UsedToDate = Convert.ToDateTime(Request.Params["used_enddate"]);
            base.OnActionExecuting(filterContext);
        }

        #region Actions

        public ActionResult Index()
        {
            return RedirectToAction("LiveFeeds");
        }

        public ActionResult EPins()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.EPins;

            ViewData["BatchFromDate"] = BatchFromDate;
            ViewData["BatchToDate"] = BatchToDate;
            ViewData["UsedFromDate"] = UsedFromDate;
            ViewData["UsedToDate"] = UsedToDate;

            List<SelectListItem> providersList = AdminCache.GenerateEpinProviderDropDownListItems(false);
            ViewData["ProvidersList"] = providersList;

            return View();
        }

        public ActionResult Messaging()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.Messaging;

            ViewData["Message"] = "";

            return View();
        }

        public ActionResult Bundles()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.Bundle;

            ViewBag.ItemCache = new ItemCache(UberStrikeCommonConfig.ApplicationId);
            ViewBag.Bundles = CmuneBundle.GetAllBundlesOnSaleView();

            return View();
        }

        public ActionResult FacebookTransactions()
        {
            ViewBag.Title = String.Format("{0} | {1}", CmuneMenu.MainTabs.Management, CmuneMenu.ManagementSubTabs.FacebookTransactions);
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.FacebookTransactions;

            List<SelectListItem> actionTypes = new List<SelectListItem> { new SelectListItem { Text = "Refund", Value = "refunded", Selected = true }, new SelectListItem { Text = "Settle", Value = "settled", Selected = true } };
            ViewData["DisputedTransactionActionType"] = actionTypes;

            return View();
        }

        public ActionResult ManageXp()
        {
            ViewBag.Title = String.Format("{0} | {1}", CmuneMenu.MainTabs.Management, CmuneMenu.ManagementSubTabs.ManageXp);
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.ManageXp;

            ViewBag.XpAttributedOnTutorialCompletion = UberStrikeCommonConfig.XpAttributedOnTutorialCompletion;

            return View();
        }

        public ActionResult WeeklySpecials()
        {
            ViewBag.Title = String.Format("{0} | {1}", CmuneMenu.MainTabs.Management, CmuneMenu.ManagementSubTabs.WeeklySpecials);
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.WeeklySpecials;

            return View();
        }

        #endregion

        #region Methods

        protected List<SelectListItem> GetPriorityDropDownList(int selectedValue)
        {
            List<SelectListItem> priorities = new List<SelectListItem>();

            foreach (int priorityKey in UberStrikeCommonConfig.LiveFeedPriorityNames.Keys.ToList())
            {
                priorities.Add(new SelectListItem() { Text = UberStrikeCommonConfig.LiveFeedPriorityNames[priorityKey], Value = priorityKey.ToString(), Selected = priorityKey == selectedValue });
            }

            return priorities;
        }

        #region Live feeds

        public ActionResult LiveFeeds()
        {
            ViewBag.SubActiveTab = CmuneMenu.ManagementSubTabs.LiveFeeds;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoadAddEditLiveFeedForm(int liveFeedId, string description, string url, int priority)
        {
            ViewData["isCreating"] = liveFeedId == 0;
            ViewData["LiveFeedIdHiddenField"] = liveFeedId;
            ViewData["DescriptionTextBox"] = description;
            ViewData["UrlTextBox"] = url;
            ViewData["PriorityDropDownListData"] = GetPriorityDropDownList(priority);
            return View("Partial/Form/AddEditLiveFeedForm");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddEditLiveFeed(FormCollection form)
        {
            bool isAddEdit = false;
            string message = String.Empty;
            string successMessage = String.Empty;

            int liveFeedId = Int32.Parse(form["LiveFeedIdHiddenField"]);
            string description = form["DescriptionTextBox"];
            string url = form["UrlTextBox"];
            int priority = Int32.Parse(form["PriorityDropDownList"]);

            if (liveFeedId == 0)
            {
                isAddEdit = Games.CreateLiveFeed(priority, description, url);
                successMessage = "The live feed was added successfly";
            }
            else if (liveFeedId > 0)
            {
                isAddEdit = Games.UpdateLiveFeed(liveFeedId, priority, description, url);
                successMessage = "The live feed was edited successfly";
            }

            message = isAddEdit ? successMessage : "Error occured";

            var json = new JsonResult();
            json.Data = new { Message = message, IsModified = isAddEdit };
            return json;
        }

        [HttpPost]
        public ActionResult DeleteLiveFeed(int liveFeedId)
        {
            string message = string.Empty;

            bool isDeleted = Games.DeleteLiveFeed(liveFeedId);
            message = isDeleted ? "The live feed was deleted" : "Error occured";

            var json = new JsonResult();
            json.Data = new { Message = message, IsDeleted = isDeleted };
            return json;
        }

        [HttpPost]
        public ActionResult GetLiveFeeds()
        {
            List<LiveFeedView> liveFeeds = Games.GetLiveFeedView();

            return PartialView("Partial/LiveFeeds", liveFeeds);
        }

        #endregion

        [HttpPost]
        public JsonResult SendMessageToAllUsers(string message)
        {
            int messagesSent = CmunePrivateMessages.SendAdminMessageToAll("", message);

            return new JsonResult() { Data = messagesSent };
        }

        #region E Pins

        [HttpPost]
        public JsonResult GenerateEpins(int epinProvider, int epinAmount, int creditAmount, bool isAdmin)
        {
            bool isGenerated = false;
            string message = string.Empty;

            if (epinAmount > 0 && creditAmount > 0)
            {
                isGenerated = CmuneEpin.GenerateBatch(CommonConfig.ApplicationIdUberstrike, epinAmount, creditAmount, (PaymentProviderType)epinProvider, isAdmin);

                if (isGenerated)
                {
                    message = "The batch was generated, loading it now...";
                }
            }
            else
            {
                message = "Amount and credits should be greater than 0.";
            }

            var json = new JsonResult();
            json.Data = new { Message = message, IsGenerated = isGenerated };
            return json;
        }

        [HttpPost]
        public ActionResult GetEpinBatches(int selectedPage)
        {
            int batchesPerPage = 30;

            List<EpinBatchView> batches = CmuneEpin.GetBatches(selectedPage, batchesPerPage);
            int batchesTotalCount = CmuneEpin.GetBatchesCount();

            var paginationModel = new PaginationModel(batchesTotalCount, selectedPage, "EpinBatches", batchesPerPage);
            ViewBag.EpinBatchesPaginationModel = paginationModel;

            return View("Partial/EpinBatches", batches);
        }

        public void ExportBatch(int batchId, string exportMode)
        {
            switch (exportMode)
            {
                case "xlxs":

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        EpinBatchView batch = CmuneEpin.GetBatch(batchId);

                        if (batch != null)
                        {
                            string fileName = String.Format("{0}-{1}.xlsx", CommonConfig.PaymentProviderName[batch.EpinProvider], batch.BatchId);

                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(String.Format("{0} UberStrike Credits", batch.CreditAmount));

                            int pinCurrencyValue = batch.CreditAmount / CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd];

                            ws.Cells["A1:B1"].Merge = true;
                            ws.Cells[1, 1].Value = String.Format("{0}: {1} E-Pins of {2} UberStrike credits ({3}) each (total value: {4})", CommonConfig.PaymentProviderName[batch.EpinProvider], batch.Amount, batch.CreditAmount, pinCurrencyValue.ToString("C0"), (pinCurrencyValue * batch.Amount).ToString("C0"));

                            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 1].Style.Border.Top.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Cells[1, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 1].Style.Border.Bottom.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Cells[1, 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 1].Style.Border.Left.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Cells[1, 2].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 2].Style.Border.Right.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Cells[1, 2].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 2].Style.Border.Top.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Cells[1, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            ws.Cells[1, 2].Style.Border.Bottom.Color.SetColor(Color.FromArgb(0, 0, 0));

                            ws.Column(1).Width = 30;
                            ws.Column(2).Width = 60;
                            ws.Row(1).Height = 30;

                            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[3, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[3, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 123, 251));
                            ws.Cells[3, 1].Style.Font.Color.SetColor(Color.White);
                            ws.Cells[3, 1].Style.Font.Bold = true;

                            ws.Cells[3, 1].Value = "Epin Id";

                            ws.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells[3, 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 123, 251));
                            ws.Cells[3, 2].Style.Font.Color.SetColor(Color.White);
                            ws.Cells[3, 2].Style.Font.Bold = true;

                            ws.Cells[3, 2].Value = "Pin";

                            int i = 4;

                            foreach (EpinView epin in batch.Epins)
                            {
                                ws.Cells[i, 1].Value = epin.EpinId;
                                ws.Cells[i, 2].Value = epin.Pin;
                                ws.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                i++;
                            }

                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", fileName));
                            Response.BinaryWrite(pck.GetAsByteArray());
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        [HttpPost]
        public JsonResult ChangeBatchRetirementStatus(int batchId)
        {
            bool isStatusChanged = CmuneEpin.ChangeBatchRetirementStatus(batchId);

            var json = new JsonResult();
            json.Data = new { IsStatusChanged = isStatusChanged };
            return json;
        }

        [HttpPost]
        public ActionResult GetEpins(string epinsSearch)
        {
            List<EpinView> epins = new List<EpinView>();
            Dictionary<int, EpinBatchView> epinsBatch = new Dictionary<int, EpinBatchView>();
            Dictionary<int, int> cmids = new Dictionary<int, int>();

            if (epinsSearch.Contains('-'))
            {
                List<string> range = epinsSearch.Split('-').ToList();

                if (range != null && range.Count == 2)
                {
                    int epinIdStart;
                    int epinIdEnd;

                    if (Int32.TryParse(range[0], out epinIdStart) && Int32.TryParse(range[1], out epinIdEnd))
                    {
                        epins = CmuneEpin.SearchEpins(epinIdStart, epinIdEnd);
                    }
                }
            }
            else
            {
                List<string> ids = epinsSearch.Split(',').ToList();

                if (ids.Count > 0)
                {
                    int tmp;

                    if (Int32.TryParse(ids[0], out tmp))
                    {
                        List<int> epinIds = new List<int>();

                        foreach (string id in ids)
                        {
                            if (Int32.TryParse(id, out tmp))
                            {
                                epinIds.Add(tmp);
                            }
                        }

                        epins = CmuneEpin.SearchEpins(epinIds);
                    }
                    else
                    {
                        epins = CmuneEpin.SearchEpins(ids);
                    }
                }
            }

            if (epins.Count > 0)
            {
                List<int> distinctBatchIds = epins.Select(e => e.BatchId).Distinct().ToList();
                epinsBatch = CmuneEpin.GetBatches(distinctBatchIds, false).ToDictionary(b => b.BatchId);

                cmids = CmuneEpin.GetCmidThatRedeemedPins(epins);
            }

            ViewData["Batches"] = epinsBatch;
            ViewData["Cmids"] = cmids;

            return View("Partial/Epins", epins);
        }

        [HttpPost]
        public JsonResult ChangeEpinRetirementStatus(int epinId)
        {
            bool isStatusChanged = CmuneEpin.ChangeEpinRetirementStatus(epinId);

            var json = new JsonResult();
            json.Data = new { IsStatusChanged = isStatusChanged };
            return json;
        }

        [HttpPost]
        public JsonResult RetireEpins(string epinIdsToRetire)
        {
            bool isStatusChanged = false;

            if (!epinIdsToRetire.IsNullOrFullyEmpty())
            {
                List<string> epinIdsData = epinIdsToRetire.Split(',').ToList();
                List<int> epinIds = epinIdsData.ConvertAll(new Converter<string, int>(s => Convert.ToInt32(s)));

                isStatusChanged = CmuneEpin.RetireEpins(epinIds);
            }

            var json = new JsonResult();
            json.Data = new { IsStatusChanged = isStatusChanged };
            return json;
        }

        #endregion

        #region Bundles

        public ActionResult LoadAddEditBundleForm(int bundleId = 0)
        {
            var jsSerializer = new JavaScriptSerializer();
            ViewBag.BundleCategorySelect = AdminCache.GenerateBundleCategorySelectItems();
            ViewBag.BuyingDurationTypeList = AdminCache.GenerateBuyingDurationSelectItems(BuyingDurationType.Permanent);
            ViewBag.BuyingDurationTypeJsonList = jsSerializer.Serialize(AdminCache.GenerateBuyingDurationSelectItems(BuyingDurationType.Permanent));
            BundleView bundleView = null;

            if (bundleId > 0)
            {
                bundleView = CmuneBundle.GetBundleView(bundleId);
            }

            return PartialView("Partial/Form/AddEditBundleForm", bundleView);
        }


        // TODO: Refactor this.
        [ValidateInput(false)]
        public ActionResult AddEditBundle(BundleView bundle)
        {
            string message = String.Empty;
            BundleOperationResult result = BundleOperationResult.Error;
            int bundleId = bundle.Id;
            List<BundleItemView> bundleItemViewsToCreate = new List<BundleItemView>();

            if (bundle.Id > 0)
            {
                bundle.BundleItemViews = CmuneBundle.GetBundleItems(bundle.Id);
                foreach (var item in bundle.BundleItemViews)
                {
                    int itemId;
                    int duration;
                    int amount;
                    // to modify
                    if (Request.Params["OwnedItem" + item.ItemId] != null && Request.Params["OwnedDuration" + item.ItemId] != null && Request.Params["OwnedAmount" + item.ItemId] != null)
                    {
                        Int32.TryParse(Request.Params["OwnedItem" + item.ItemId], out itemId);
                        Int32.TryParse(Request.Params["OwnedDuration" + item.ItemId], out duration);
                        Int32.TryParse(Request.Params["OwnedAmount" + item.ItemId], out amount);
                        var bundleItemV = new BundleItemView()
                        {
                            BundleId = bundle.Id,
                            ItemId = itemId,
                            Amount = Math.Max(amount, 0),
                            Duration = (BuyingDurationType)duration
                        };
                        if (amount > 0)
                        {
                            bundleItemV.Duration = BuyingDurationType.None;
                        }
                        bundleItemViewsToCreate.Add(bundleItemV);
                    }
                }
            }

            for (int i = 0; i < 30; i++)
            {
                if (Request.Params["NewItem" + i] != null && Request.Params["NewDuration" + i] != null)
                {
                    int itemId;
                    int duration;
                    int amount;
                    Int32.TryParse(Request.Params["NewItem" + i], out itemId);
                    Int32.TryParse(Request.Params["NewDuration" + i], out duration);
                    Int32.TryParse(Request.Params["NewAmount" + i], out amount);
                    if (itemId != 0 && duration != 0)
                    {
                        var bundleItem = new BundleItemView()
                        {
                            ItemId = itemId,
                            BundleId = bundle.Id,
                            Amount = amount,
                            Duration = (BuyingDurationType)duration
                        };
                        bundleItemViewsToCreate.Add(bundleItem);
                    }
                }
            }

            if (!(bundle.Credits > 0 && bundleItemViewsToCreate.Count > 0))
            {
                if (bundle.Id == 0)
                    result = CmuneBundle.CreateBundle(bundle);
                else
                    result = CmuneBundle.UpdateBundle(bundle);
            }
            else
                result = BundleOperationResult.InvalidComposition;

            // Add Delete Items
            if (result == BundleOperationResult.Ok)
            {


                CmuneBundle.DeleteBundleItems(bundle.Id);
                CmuneBundle.AddBundleItems(bundle.Id, bundleItemViewsToCreate);

                List<ChannelType> availableChannels = new List<ChannelType>();

                foreach (ChannelType activeChannel in CommonConfig.ActiveChannels)
                {
                    if (Request.Form[String.Format("ActiveOnChannel{0}", (int)activeChannel)].Contains("true"))
                    {
                        availableChannels.Add(activeChannel);
                    }
                }

                CmuneBundle.DefineAvailableChannels(bundle.Id, availableChannels);

                if (bundle.IsDefault == true)
                    CmuneBundle.UnsetIsDefaultForBundles(bundle.Id);

                UberStrikeCacheInvalidation.InvalidateBundles(BuildType);
            }

            switch (result)
            {
                case BundleOperationResult.InvalidComposition:
                    message = "Can't create bundle with credits and items";
                    break;
                case BundleOperationResult.DuplicateUniqueId:
                    message = "Duplicate UniqueId";
                    break;
                case BundleOperationResult.Ok:
                    message = "Bundle updated";
                    break;
                default:
                    CmuneLog.LogUnexpectedReturn(bundle, "Error adding / editing bundle");
                    message = "Error, was logged";
                    break;
            }

            var json = new JsonResult() { Data = new { isAddOrEdit = result == BundleOperationResult.Ok, message = message } };
            return json;
        }

        public ActionResult DeleteBundle(int bundleId)
        {
            var isDeleted = CmuneBundle.DeleteBundle(bundleId);
            UberStrikeCacheInvalidation.InvalidateBundles(BuildType);
            var json = new JsonResult() { Data = new { isDeleted = isDeleted } };
            return json;
        }

        public PartialViewResult GetBundles(bool onlyOnSale, bool onlyBundle, bool onlyPack)
        {
            List<BundleView> bundles = new List<BundleView>();

            if (onlyOnSale)
            {
                bundles = CmuneBundle.GetAllBundlesOnSaleView();
            }
            else
            {
                bundles = CmuneBundle.GetAllBundlesView();
            }

            if (onlyBundle)
            {
                bundles = bundles.Where(b => b.Credits == 0).ToList();
            }

            if (onlyPack)
            {
                bundles = bundles.Where(b => b.Credits > 0).ToList();
            }

            ViewBag.ItemCache = new ItemCache(UberStrikeCommonConfig.ApplicationId);

            return PartialView("Partial/Bundles", bundles);
        }

        #endregion

        #region Facebook transactions

        public ActionResult GetFacebookReversals()
        {
            List<FacebookReversalDisplay> reversals = new List<FacebookReversalDisplay>();

            DateTime facebookCreditsStart = new DateTime(2011, 06, 27, 0, 0, 0);
            long facebookCreditsStartTimeStamp = Php.ConvertToTimestamp(facebookCreditsStart);
            DateTime now = DateTime.Now;
            long starting = Php.ConvertToTimestamp(now.AddDays(-1));
            long ending = Php.ConvertToTimestamp(now);

            string urlTemplate = "https://graph.facebook.com/24509077139/payments?status=refunded&since={0}&until={1}&access_token=" + ConfigurationUtilities.ReadConfigurationManager("FacebookAccessToken");

            while (ending >= facebookCreditsStartTimeStamp)
            {
                using (WebClient client = new WebClient())
                {
                    JObject response = JObject.Parse(client.DownloadString(String.Format(urlTemplate, starting, ending)));

                    foreach (var row in response["data"].Children())
                    {
                        long transactionId;
                        Int64.TryParse(row["id"].ToString().Replace("\"", ""), out transactionId);

                        long facebookId = 0;
                        Int64.TryParse(row["from"].ToString().Replace("\"", ""), out facebookId);

                        decimal amount;
                        Decimal.TryParse(row["amount"].ToString().Replace("\"", ""), out amount);

                        reversals.Add(new FacebookReversalDisplay(transactionId, amount, facebookId));
                    }
                }

                ending = starting;
                starting -= 86400;
            }

            return View("Partial/FacebookReversals", reversals);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult RefundTransaction(long orderId, string comment, string disputedTransactionActionType)
        {
            bool isTransactionRefunded = false;

            try
            {
                WebPostRequest request = new WebPostRequest(String.Format("https://graph.facebook.com/{0}", orderId));
                request.Add("access_token", ConfigurationUtilities.ReadConfigurationManager("FacebookAccessToken"));
                request.Add("status", disputedTransactionActionType);
                request.Add("message", comment);
                request.Add("params", String.Format("{{'comment' => '{0}'}}", comment.Replace('\'', '"')));
                string response = request.GetResponse();

                if (response == "true")
                {
                    isTransactionRefunded = true;
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Format("orderId={0}&comment={1}&disputedTransactionActionType={2}", orderId, comment, disputedTransactionActionType));
            }

            var json = new JsonResult();
            json.Data = new { IsRefunded = isTransactionRefunded };
            return json;
        }

        [HttpPost]
        public ActionResult GetTransaction(long orderId)
        {
            FacebookTransactionDisplay display = null;

            try
            {
                WebGetRequest request = new WebGetRequest(String.Format("https://graph.facebook.com/{0}&access_token={1}", orderId, ConfigurationUtilities.ReadConfigurationManager("FacebookAccessToken")));
                JObject response = JObject.Parse(request.GetResponse());

                long id;
                Int64.TryParse(response["id"].ToString(), out id);
                long from;
                Int64.TryParse(response["from"].ToString(), out from);
                long to;
                Int64.TryParse(response["to"].ToString(), out to);
                int amount;
                Int32.TryParse(response["amount"].ToString(), out amount);
                string status = response["status"].ToString();
                string country = response["country"].ToString();
                string createdTime = response["created_time"].ToString();
                string updatedTime = response["updated_time"].ToString();

                display = new FacebookTransactionDisplay(id, from, to, amount, status, country, createdTime, updatedTime);
            }
            catch
            {
            }

            return PartialView("Partial/FacebookTransaction", display);
        }

        #endregion

        #region XP and tutorial management

        [HttpPost]
        public ActionResult GetXpEvents()
        {
            List<PlayerXPEventView> xpEvents = Statistics.GetXPEventsView();

            return View("Partial/XpEvents", xpEvents);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditXpEvents(FormCollection form)
        {
            bool areModified = false;

            string idsData = form["xpEventsId"];
            List<int> ids = idsData.Split(',').ToList().ConvertAll(new Converter<string, int>(x => Convert.ToInt32(x)));
            List<PlayerXPEventView> events = new List<PlayerXPEventView>();

            foreach (int id in ids)
            {
                events.Add(new PlayerXPEventView(id, form[String.Format("name{0}", id)], Convert.ToDecimal(form[String.Format("multiplier{0}", id)])));
            }

            areModified = Statistics.EditXPEvents(events);

            var json = new JsonResult();
            json.Data = new { AreModified = areModified };
            return json;
        }

        [HttpPost]
        public ActionResult GetLevelCaps()
        {
            List<PlayerLevelCapView> levelCaps = Statistics.GetLevelCapsView();

            return View("Partial/LevelCaps", levelCaps);
        }

        [HttpPost]
        public JsonResult EditLevelCaps(FormCollection form)
        {
            bool areModified = false;

            string idsData = form["levelCapsId"];
            List<int> ids = idsData.Split(',').ToList().ConvertAll(new Converter<string, int>(x => Convert.ToInt32(x)));
            List<PlayerLevelCapView> levelCaps = new List<PlayerLevelCapView>();

            foreach (int id in ids)
            {
                levelCaps.Add(new PlayerLevelCapView(id, 0, Convert.ToInt32(form[String.Format("xp{0}", id)])));
            }

            areModified = Statistics.EditLevelCaps(levelCaps);

            var json = new JsonResult();
            json.Data = new { AreModified = areModified };
            return json;
        }

        [HttpPost]
        public ActionResult GetItemsAttributed()
        {
            Dictionary<int, int> itemsAttributed = Users.GetItemsAttributedOnTutorial();

            ViewBag.ItemMaximumDurationInDays = CommonConfig.ItemMaximumDurationInDays;
            ViewBag.FirstLoadoutWeaponItemIds = UberStrikeCommonConfig.FirstLoadoutWeaponItemIds;

            return View("Partial/ItemsAttributed", itemsAttributed);
        }

        [HttpPost]
        public JsonResult SetItemsAttributed(FormCollection form)
        {
            bool areModified = false;

            int maxIndex;

            if (Int32.TryParse(form["maxIndex"], out maxIndex))
            {
                Dictionary<int, int> itemsAttributed = new Dictionary<int, int>();

                for (int i = 0; i < maxIndex; i++)
                {
                    int itemId;
                    int duration;

                    if (Int32.TryParse(form[String.Format("itemId{0}", i)], out itemId) &&
                        Int32.TryParse(form[String.Format("itemDuration{0}", i)], out duration))
                    {
                        if (itemId >= CommonConfig.NewItemMallItemIdStart && duration >= 0 && duration <= CommonConfig.ItemMaximumDurationInDays)
                        {
                            if (!itemsAttributed.ContainsKey(itemId))
                            {
                                itemsAttributed.Add(itemId, duration);
                            }
                        }
                    }
                }

                Users.SetItemsAttributedOnTutorial(itemsAttributed);
                areModified = true;
            }

            var json = new JsonResult();
            json.Data = new { AreModified = areModified };
            return json;
        }

        [HttpPost]
        public JsonResult ResynchroniseLevelsBasedOnXp()
        {
            string queries = Statistics.ResynchroniseLevelsBasedOnXp(true);

            return new JsonResult() { Data = new { Queries = queries } };
        }

        [HttpPost]
        public JsonResult ResynchroniseXpBasedOnLevel()
        {
            string queries = Statistics.ResynchroniseXpBasedOnLevel(true);

            return new JsonResult() { Data = new { Queries = queries } };
        }

        #endregion

        #region Weekly Specials

        [HttpPost]
        public ActionResult GetWeeklySpecials()
        {
            List<WeeklySpecialView> weeklySpecials = WeeklySpecialService.GetRecentWeeklySpecials();
            ViewBag.ItemCache = new ItemCache(UberStrikeCommonConfig.ApplicationId);

            return View("Partial/WeeklySpecials", weeklySpecials);
        }

        [HttpPost]
        public ActionResult LoadAddEditWeeklySpecialForm(int id)
        {
            WeeklySpecialView weeklySpecial = null;

            if (id != 0)
            {
                weeklySpecial = WeeklySpecialService.GetWeeklySpecial(id);

                if (weeklySpecial == null)
                {
                    return new JsonResult() { Data = new { HasError = true } };
                }
            }

            return View("Partial/Form/AddEditWeeklySpecialForm", weeklySpecial);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddEditWeeklySpecial(WeeklySpecialView weeklySpecial)
        {
            WeeklySpecialOperationResult result = WeeklySpecialOperationResult.Error;

            if (weeklySpecial.Id == 0)
            {
                result = WeeklySpecialService.CreateWeeklySpecial(weeklySpecial);
            }
            else
            {
                result = WeeklySpecialService.EditWeeklySpecial(weeklySpecial);
            }

            return new JsonResult() { Data = new { IsModified = result == WeeklySpecialOperationResult.Ok, ErrorMessage = result.ToString() } };
        }

        [HttpPost]
        public JsonResult EndWeeklySpecial(int id)
        {
            bool result = WeeklySpecialService.EndWeeklySpecial(id);

            return new JsonResult() { Data = new { IsModified = result } };
        }

        #endregion Weekly Specials

        #endregion
    }
}