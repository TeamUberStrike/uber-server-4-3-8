using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cmune.DataCenter.Utils;
using Newtonsoft.Json;
using Cmune.DataCenter.Common.Entities;
using System.Text;
using Cmune.DataCenter.Business;
using System.Collections;

namespace Cmune.Channels.Callback
{
    public partial class Kongregate : System.Web.UI.Page
    {
        protected class KongregateData
        {
            public string @event;
            public string api_key;
            public string time;
            public KongregateDataUserInventoryChanged kongregateDataUser;
        }

        protected class KongregateDataUserInventoryChanged
        {
            public string user_id;
            public string username;
            public string game_auth_token;
        }

        protected class KongregateInventoryData
        {
            public string api_key { get; set; }
            public string user_id { get; set; }
        }

        protected class KongregateInventoryReturnData
        {
            public bool success;
            public string error;
            public string error_description;
            public List<KongregateItem> items;
        }

        protected class KongregateItem
        {
            public int id;
            public string identifier;
            public string name;
            public string description;
            public int remaining_uses;
            public string data;
        }

        protected class KongregateUseItemData
        {
            public string api_key { get; set; }
            public string game_auth_token { get; set; }
            public string user_id { get; set; }
            public int id { get; set; }
        }

        protected class KongregateUseItemReturnData
        {
            public bool success;
            public bool error;
            public string error_description;
            public string remaining_uses;
            public long usage_record_id;
        }

        protected class KongregateConsumeItemResult
        {
            public bool Success { get; set; }
            public string Identifier { get; set; }
            public long UsageRecordId { get; set; }
        }

        protected KongregateData ParseRequest(HttpRequest httpRequest)
        {
            var kongregateData = new KongregateData();
            if (String.IsNullOrEmpty(httpRequest.Params["api_key"]) || String.IsNullOrEmpty(httpRequest.Params["event"])
                || String.IsNullOrEmpty(httpRequest.Params["time"]))
            {
                throw new ApplicationException(String.Format("Unknown request"));
            }
            if (!(httpRequest.Params["api_key"] == ConfigurationUtilities.ReadConfigurationManager("KongregateAppAPIKey")))
            {
                throw new ApplicationException(String.Format("Permission denied"));
            }

            kongregateData.api_key = httpRequest.Params["api_key"];
            kongregateData.@event = httpRequest.Params["event"];
            kongregateData.time = httpRequest.Params["time"];
            if (!String.IsNullOrEmpty(httpRequest.Params["user_id"]) && !String.IsNullOrEmpty(httpRequest.Params["username"]) && !String.IsNullOrEmpty(httpRequest.Params["game_auth_token"]))
            {
                var kongregateDataUserInventoryChanged = new KongregateDataUserInventoryChanged();

                kongregateDataUserInventoryChanged.user_id = httpRequest.Params["user_id"];
                kongregateDataUserInventoryChanged.username = httpRequest.Params["username"];
                kongregateDataUserInventoryChanged.game_auth_token = httpRequest.Params["game_auth_token"];
                kongregateData.kongregateDataUser = kongregateDataUserInventoryChanged;
            }

            return kongregateData;
        }

        protected List<KongregateItem> KongregateGetInventory(KongregateData kongregateData)
        {
            var inventoryUrl = "http://www.kongregate.com/api/user_items.json";

            var kongregateInventoryData = new KongregateInventoryData();
            kongregateInventoryData.api_key = kongregateData.api_key;
            kongregateInventoryData.user_id = kongregateData.kongregateDataUser.user_id;

            var webGetRequest = new WebGetRequest(inventoryUrl, kongregateInventoryData);
            KongregateInventoryReturnData kongregateInventory = (KongregateInventoryReturnData)JsonConvert.DeserializeObject(webGetRequest.GetResponse(), typeof(KongregateInventoryReturnData));
            if (kongregateInventory.success)
            {
                return kongregateInventory.items;
            }
            return null;
        }

        protected KongregateConsumeItemResult KongregateConsumeItem(KongregateData kongregateData, KongregateItem kongregateItem)
        {
            var useItemUrl = "http://www.kongregate.com/api/use_item.json";

            var kongregateConsumeItemResult = new KongregateConsumeItemResult();
            kongregateConsumeItemResult.UsageRecordId = 0;
            kongregateConsumeItemResult.Identifier = string.Empty;

            KongregateUseItemData kongregateUseItemData = new KongregateUseItemData();
            kongregateUseItemData.api_key = kongregateData.api_key;
            kongregateUseItemData.game_auth_token = kongregateData.kongregateDataUser.game_auth_token;
            kongregateUseItemData.user_id = kongregateData.kongregateDataUser.user_id;
            kongregateUseItemData.id = kongregateItem.id;

            var webGetRequest = new WebGetRequest(useItemUrl, kongregateUseItemData);
            KongregateUseItemReturnData kongregateUseItemReturn = (KongregateUseItemReturnData)JsonConvert.DeserializeObject(webGetRequest.GetResponse(), typeof(KongregateUseItemReturnData));
            kongregateConsumeItemResult.Success = kongregateUseItemReturn.success;
            if (kongregateUseItemReturn.success)
            {
                kongregateConsumeItemResult.UsageRecordId = kongregateUseItemReturn.usage_record_id;
                kongregateConsumeItemResult.Identifier = kongregateItem.identifier;
            }
            return kongregateConsumeItemResult;
        }

        protected void ConsumeKongregateInventory(KongregateData kongregateData)
        {
            var kongregateInventory = KongregateGetInventory(kongregateData);
            KongregateConsumeItemResult kongregateConsumeItemResult;

            if (kongregateInventory.Count > 0)
            {
                foreach (var item in kongregateInventory)
                {
                    kongregateConsumeItemResult = KongregateConsumeItem(kongregateData, item);

                    if (kongregateConsumeItemResult.Success)
                    {
                        int creditDepositId;
                        int adminId = ConfigurationUtilities.ReadConfigurationManagerInt("KongregateAdminId");
                        var isAdmin = kongregateData.kongregateDataUser.user_id == adminId.ToString();

                        var cmid = CmuneMember.GetCmidByEsnsId(kongregateData.kongregateDataUser.user_id, EsnsType.Kongregate);
                        var bundleId = CmuneBundle.GetBundleIdByMacAppStoreUniqueId(kongregateConsumeItemResult.Identifier);
                        if (bundleId > 0)
                            CmuneEconomy.ProcessBundleAttribution(cmid, bundleId, CurrencyType.Usd, isAdmin, PaymentProviderType.KongregateKreds, kongregateConsumeItemResult.UsageRecordId.ToString(), CommonConfig.ApplicationIdUberstrike, ChannelType.Kongregate, out creditDepositId);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            KongregateData kgData = new KongregateData();
            var infoLogFileName = PaymentProviderType.KongregateKreds.ToString() + " " + DateTime.Now.Date.ToString("yyyy-MM-dd");
            StringBuilder logInfo = new StringBuilder();

            try
            {
                kgData = ParseRequest(HttpContext.Current.Request);
                if (kgData.kongregateDataUser != null)
                {
                    ConsumeKongregateInventory(kgData);
                }
            }
            catch (Exception ex)
            {
                logInfo.AppendLine(String.Format("HttpRequest : {0}", Request.Params));
                CmuneLog.LogException(ex, "Channel Callback - Ip : " + Request.UserHostAddress);
            }
            finally
            {
                //if (canLogInfo)
                //{
                //    logInfo.AppendLine();
                //    logInfo.AppendLine(String.Format("Request: {0}", JsonConvert.SerializeObject(fbData)));
                //    logInfo.AppendLine(String.Format("Response: {0}", JsonConvert.SerializeObject(fbDataToReturn)));
                logInfo.AppendLine(String.Format("Request: {0}", JsonConvert.SerializeObject(kgData)));
                CmuneLog.LogInfo(logInfo.ToString(), infoLogFileName);
                //}
            }
        }
    }
}