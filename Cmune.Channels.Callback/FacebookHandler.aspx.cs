using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Utils;
using Newtonsoft.Json;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using System.Net;
using System.IO;
using UberStrike.DataCenter.Business;
using Facebook;

namespace Cmune.Channels.Callback
{
    public partial class FacebookHandler : System.Web.UI.Page
    {
        protected enum PaymentStatus
        {
            Placed,
            Settled,
            Canceled,
            Refunded,
            Disputed
        }

        #region Facebook Data, this how facebook data is organized, i created these class to serialize and unserialize facebook data more easily

        protected class AgeData
        {
            public int min;
        }

        protected class UserData
        {
            public string country;
            public string locale;
            public AgeData age;
        }

        /// <summary>
        /// data provided in the 1st callback request 
        /// </summary>
        protected class OrderInfoData
        {
            public string userId;
            public string currency;
            public decimal amount;
            public int bundleId;
        }

        protected class CreditsData
        {
            public long buyer;
            public long receiver;
            public long order_id;
            public string order_info;
            public OrderInfoData orderInfo;
            public bool test_mode;
            public string status;
            public string order_details;
            public OrderDetailsData orderDetails;
        }

        /// <summary>
        /// data provided in the 3rd callback request 
        /// </summary>
        protected class OrderDetailsData
        {
            public long order_id;
            public long buyer;
            public long app;
            public long receiver;
            public int amount;
            public long update_time;
            public long time_placed;
            public string data;
            public ItemData[] items;
        }

        /// <summary>
        /// trunk of facebook incoming data
        /// </summary>
        protected class FacebookData
        {
            public string algorithm;
            public CreditsData credits;
            public long expires;
            public long issued_at;
            public string oauth_token;
            public UserData user;
            public string user_id;
        }

        #endregion

        #region the facebook data to return

        /// <summary>
        /// data to return at first callback request
        /// </summary>
        protected class ItemData
        {
            public string title;
            public int price;
            public string description;
            public string image_url;
            public string product_url;
            public int item_id;
            public string data;
        }

        /// <summary>
        /// data to return at second and third call back request
        /// </summary>
        protected class PaymentData
        {
            public long order_id;
            public string status;
        }

        protected class FacebookDataToReturn
        {
            public string method;
            public dynamic content;
        }

        #endregion

        string infoLogFileName;

        protected string ToReadableFbData(string serializedFbData, bool isHtml = false)
        {
            string readableFbData = string.Empty;

            readableFbData = serializedFbData.Replace("\\", "").Replace("{", "").Replace("}", "").Replace(",", "\n");

            if (isHtml)
            {
                readableFbData = readableFbData.Replace("\n", "<br/>");
            }

            return readableFbData;
        }

        protected FacebookData ParseSignedRequest(string signedRequest, string secret)
        {
            string error = string.Empty;

            var signedValues = signedRequest.Split('.');
            string payload = signedValues[1];

            Dictionary<string, object> requestData = new Dictionary<string, object>();

            string data = payload.UrlBase64Decode();
            FacebookData fbData = (FacebookData)JsonConvert.DeserializeObject(data, typeof(FacebookData));

            if (fbData.credits.order_info != null)
            {
                fbData.credits.orderInfo = (OrderInfoData)JsonConvert.DeserializeObject(fbData.credits.order_info, typeof(OrderInfoData));
            }

            if (fbData.credits.order_details != null)
            {
                fbData.credits.orderDetails = (OrderDetailsData)JsonConvert.DeserializeObject(fbData.credits.order_details, typeof(OrderDetailsData));
                //fbData.credits.orderDetails.item = ((ItemData[])JsonConvert.DeserializeObject(fbData.credits.orderDetails.items, typeof(ItemData[]))).First();
            }

            // verify the hash algorithm 
            if (fbData.algorithm == null || (fbData.algorithm != null && ((string)fbData.algorithm).ToUpper() != "HMAC-SHA256"))
            {
                string algorithmName = (fbData.algorithm != null) ? fbData.algorithm : "null";

                throw new ArgumentOutOfRangeException(String.Format("Unknown algorithm ({0}). Expected HMAC-SHA256", algorithmName));
            }

            // verify the signedRequest to make sure thats come from facebook
            FacebookSignedRequest signedRequestRet;

            if (Facebook.FacebookSignedRequest.TryParse(FacebookApplication.Current, signedRequest, out signedRequestRet) == false)
            {
                throw new ArgumentOutOfRangeException("Intrusion in our payment system, signed request is not matching");
            }

            return fbData;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FacebookData fbData = new FacebookData();
            FacebookDataToReturn fbDataToReturn = new FacebookDataToReturn();
            infoLogFileName = PaymentProviderType.FacebookCredits.ToString() + " " + DateTime.Now.Date.ToString("yyyy-MM-dd");
            StringBuilder logInfo = new StringBuilder();
            bool canLogInfo = false;
            int cmid = 0;

            try
            {
                decimal usdToFacebookCreditsConversionRate = CommonConfig.UsdToFacebookCredit;
                string UberstrikeCreditCurrencyImage = ConfigurationUtilities.ReadConfigurationManager("UberstrikeCreditCurrencyImage");
                string imageRoot = ConfigurationUtilities.ReadConfigurationManager("ImagesRoot");
                string facebookAppId = ConfigurationUtilities.ReadConfigurationManager("FacebookAppId");
                string facebookAppSecret = ConfigurationUtilities.ReadConfigurationManager("FacebookAppSecret");

                if (!Request.Params["signed_request"].IsNullOrFullyEmpty())
                {
                    fbData = ParseSignedRequest(Request.Params["signed_request"], facebookAppSecret);
                    canLogInfo = true;

                    if (Request.Params["method"] != null)
                    {
                        if (Request.Params["method"] == "payments_get_items")
                        {
                            // First callback: retrieves the item details

                            if (fbData.credits.orderInfo.bundleId > 0)
                            {
                                var selectedBundle = CmuneBundle.GetBundleOnSaleWithoutItemView(fbData.credits.orderInfo.bundleId, ChannelType.WebFacebook);

                                if (selectedBundle != null)
                                {
                                    var item = new ItemData();
                                    item.price = (int)(selectedBundle.USDPrice * usdToFacebookCreditsConversionRate);
                                    item.title = selectedBundle.Name;
                                    item.product_url = imageRoot + selectedBundle.IconUrl;
                                    item.image_url = imageRoot + selectedBundle.IconUrl;
                                    item.description = "Uberstrike by Cmune";
                                    item.item_id = selectedBundle.Id;
                                    fbDataToReturn.content = new object[] { item };
                                }
                            }

                            fbDataToReturn.method = Request.Params["method"];
                        }
                        else if (Request.Params["method"] == "payments_status_update")
                        {
                            if (fbData.credits.status == PaymentStatus.Placed.ToString().ToLower())
                            {
                                // Second callback: the user completed the order

                                var paymentUpdate = new PaymentData();
                                paymentUpdate.status = PaymentStatus.Canceled.ToString().ToLower(); // by default canceled

                                cmid = CmuneMember.GetCmidByEsnsId(fbData.user_id, EsnsType.Facebook);

                                bool isAdmin = fbData.credits.test_mode;

                                int creditDepositId;
                                var item = fbData.credits.orderDetails.items.First();

                                if (CmuneEconomy.ProcessBundleAttribution(cmid, item.item_id, CurrencyType.Usd, isAdmin, PaymentProviderType.FacebookCredits, fbData.credits.order_id.ToString(), CommonConfig.ApplicationIdUberstrike, ChannelType.WebFacebook, out creditDepositId))
                                    paymentUpdate.status = PaymentStatus.Settled.ToString().ToLower(); // success

                                paymentUpdate.order_id = fbData.credits.order_id;
                                fbDataToReturn.content = paymentUpdate;
                                fbDataToReturn.method = Request.Params["method"];
                            }
                            else if (fbData.credits.status == PaymentStatus.Settled.ToString().ToLower())
                            {
                                /* Kept for legacy purpose, even though Facebook is not supposed to send a "settled" status anymore they sometimes do it
                                 * "Finally, sometimes we respond with another callback notifying you that the order has been moved to the settled state which requires no action/response from the developer."
                                 * http://developers.facebook.com/blog/post/598/
                                 */
                            }
                            else if (fbData.credits.status == PaymentStatus.Refunded.ToString().ToLower())
                            {
                                // Facebook refunded the transaction, we send an email to the community manager for her to take action

                                CmuneMail.SendEmail(CommonConfig.CmuneNoReplyEmail, CommonConfig.CmuneNoReplyEmailName, CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, "Facebook Transaction Refunded", ToReadableFbData(JsonConvert.SerializeObject(fbData), true), ToReadableFbData(JsonConvert.SerializeObject(fbData)));
                            }
                            else if (fbData.credits.status == PaymentStatus.Disputed.ToString().ToLower())
                            {
                                // A user is disputing a transaction, we send an email to the community manager for her to take action

                                CmuneMail.SendEmail(CommonConfig.CmuneNoReplyEmail, CommonConfig.CmuneNoReplyEmailName, CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, "Facebook Transaction Disputed", ToReadableFbData(JsonConvert.SerializeObject(fbData), true), ToReadableFbData(JsonConvert.SerializeObject(fbData)));
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException("Unknown payment status: " + fbData.credits.status + " / " + JsonConvert.SerializeObject(fbData));
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("Unknown payment method: " + Request.Params["method"] + " / " + JsonConvert.SerializeObject(fbData));
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Missing payment method: " + JsonConvert.SerializeObject(fbData));
                    }

                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(fbDataToReturn));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Missing signed_request: " + Request.Params);
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, "Channel Callback - Ip : " + Request.UserHostAddress);
            }
            finally
            {
                if (canLogInfo)
                {
                    logInfo.AppendLine();
                    logInfo.AppendLine(String.Format("Request: {0}", JsonConvert.SerializeObject(fbData)));
                    logInfo.AppendLine(String.Format("Response: {0}", JsonConvert.SerializeObject(fbDataToReturn)));
                    CmuneLog.LogInfo(logInfo.ToString(), infoLogFileName);
                }
            }
        }
    }
}