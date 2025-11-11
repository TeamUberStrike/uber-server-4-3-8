using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Business;
using System.Text;

namespace Cmune.Channels.Callback
{
    public partial class SuperRewardsHandler : System.Web.UI.Page
    {
        public const string ResultCodeSuccess = "1";
        public const string ResultCodeError = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool callbackSucceed = false;
            StringBuilder logData = new StringBuilder();
            SuperRewardsRequest superRewardsRequest = null;
            string infoLogFileName = PaymentProviderType.SuperRewards.ToString() + " " + DateTime.Now.Date.ToString("yyyy-MM-dd");
            StringBuilder logInfo = new StringBuilder();
            bool canLogInfo = true;

            try
            {
                bool isFormValid = false;
                superRewardsRequest = new SuperRewardsRequest(Request, ref logData, out isFormValid);

                string serverIP = Request.UserHostAddress;
                string serversIPs = ConfigurationUtilities.ReadConfigurationManager("SuperRewardsServersIPs");

                logData.Append("isRequestValid:[");
                logData.Append(isFormValid);
                logData.Append("] Super Rewards server IP:[");
                logData.Append(serverIP);
                logData.Append("]");

                if (isFormValid && !serverIP.IsNullOrFullyEmpty() && serversIPs.Contains(serverIP))
                {
                    logData.Append("payment duplicate:[");

                    if (!CmuneEconomy.IsTransactionExecuted(PaymentProviderType.SuperRewards, superRewardsRequest.TransactionId))
                    {
                        bool isAdminAction = false;

                        logData.Append("No]");

                        int cmid = superRewardsRequest.Cmid;
                        
                        string testCmidsConfig = ConfigurationUtilities.ReadConfigurationManager("SuperRewardsPaymentTesters", false);
                        List<int> testCmids = testCmidsConfig.Split('|').ToList().ConvertAll(u => Convert.ToInt32(u));

                        logData.Append("testMode:[");

                        if (testCmids.Contains(cmid))
                        {
                            logData.Append("Yes]");
                            isAdminAction = true;
                        }
                        else
                        {
                            logData.Append("No]");
                            isAdminAction = false;
                        }

                        int creditDepositId = 0;

                        var SuperRewardBundlesConfig = ConfigurationUtilities.ReadConfigurationManager("SuperRewardBundles");
                        Dictionary<int, int> SuperRewardBundles = new Dictionary<int, int>();

                        foreach (var row in SuperRewardBundlesConfig.Split(','))
                        {
                            var rowContent = row.Split('|');
                            SuperRewardBundles.Add(int.Parse(rowContent[0]), int.Parse(rowContent[1]));
                        }

                        decimal cash = 0;

                        if (SuperRewardBundles.ContainsValue(superRewardsRequest.NewCredits))
                        {
                            cash = SuperRewardBundles.Where(d => d.Value == superRewardsRequest.NewCredits).First().Key;
                        }
                        else
                        {
                            // We need to convert the credits to Usd for stats purpose
                            cash = (decimal)superRewardsRequest.NewCredits / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd];
                        }

                        CmuneEconomy.ProcessCreditAttribution(cmid, cash, CurrencyType.Usd, isAdminAction, PaymentProviderType.SuperRewards, superRewardsRequest.TransactionId, superRewardsRequest.ApplicationId, superRewardsRequest.Channel, null, out creditDepositId, true, superRewardsRequest.NewCredits);

                        logData.Append("creditDepositId:[");
                        logData.Append(creditDepositId);
                        logData.Append("]");

                        callbackSucceed = true;
                    }
                    else
                    {
                        logData.Append("Yes]");

                        callbackSucceed = true;
                    }
                }
                else
                {
                    if (!isFormValid)
                    {
                        logData.Append("[invalid post]");
                    }
                    else
                    {
                        logData.Append("[invalid IP]");
                    }
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Empty);
            }
            finally
            {
                try
                {
                    string response = ResultCodeError;

                    if (callbackSucceed)
                    {
                        response = ResultCodeSuccess;
                    }
                    else
                    {
                        response = ResultCodeError;
                    }

                    logData.Append("response:[");
                    logData.Append(response);
                    logData.Append("][callback data: ");
                    logData.Append(superRewardsRequest);
                    logData.Append("]");

                    bool logSuperRewards = ConfigurationUtilities.ReadConfigurationManagerBool("SuperRewardsActivateFullLog");

                    if (logSuperRewards)
                    {
                        CmuneLog.LogUnexpectedReturn(logData, "Full log");
                    }

                    if (canLogInfo)
                    {
                        logInfo.Append(logData);
                        CmuneLog.LogInfo(logInfo.ToString(), infoLogFileName);
                    }

                    if (!callbackSucceed)
                    {
                        CmuneLog.LogUnexpectedReturn(logData, "Invalid Super Rewards callback");
                    }

                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    Response.ContentType = "plain/text";
                    Response.Write(response);
                }
                catch (Exception ex)
                {
                    CmuneLog.LogException(ex, superRewardsRequest != null ? superRewardsRequest.ToString() : "");
                }
            }
        }
    }

    public class SuperRewardsRequest
    {
        #region Fields

        private int _newCredits;
        private int _totalCredits;
        private string _userId;
        private int _offerId;
        private int _applicationId;
        private ChannelType _channel;
        private int _bundleId; //useless in superreward transaction
        private int _cmid;

        #endregion Fields

        #region Properties

        public string TransactionId { get; set; }
        public int NewCredits
        {
            get { return _newCredits; }
            set { _newCredits = value; }
        }
        public int TotalCredits
        {
            get { return _totalCredits; }
            set { _totalCredits = value; }
        }
        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        public int OfferId
        {
            get { return _offerId; }
            set { _offerId = value; }
        }
        public string Signature { get; set; }
        public int ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }
        public ChannelType Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        public int Cmid
        {
            get { return _cmid; }
            set { _cmid = value; }
        }

        #endregion Properties

        #region Constructors

        public SuperRewardsRequest(HttpRequest request, ref StringBuilder logData, out bool isValid)
        {
            isValid = false;

            logData.Append("[SuperRewardsRequest]");

            NameValueCollection callbackData = new NameValueCollection();

            if (request.HttpMethod.Equals("GET"))
            {
                callbackData = request.QueryString;
                logData.Append(String.Format("[{0}]", request.QueryString));
            }
            else if (request.HttpMethod.Equals("POST"))
            {
                callbackData = request.Form;
                logData.Append(String.Format("[{0}]", request.Form));
            }

            if (callbackData.Count > 0)
            {
                TransactionId = callbackData["id"];
                bool isNewCreditsParsed = Int32.TryParse(callbackData["new"], out _newCredits);
                bool isTotalCreditsParsed = Int32.TryParse(callbackData["total"], out _totalCredits);
                UserId = callbackData["uid"];
                bool isOfferIdParsed = Int32.TryParse(callbackData["oid"], out _offerId);
                Signature = callbackData["sig"];
                string encodedChannelAndApplication = callbackData["custom_d"];
                bool isCmidParsed = Int32.TryParse(callbackData["custom_c"], out _cmid);
                string customHash = callbackData["custom_h"];

                // First we check all the fields that shouldn't be null or empty

                if (!this.TransactionId.IsNullOrFullyEmpty() && isNewCreditsParsed && NewCredits > 0 && isTotalCreditsParsed && TotalCredits >= NewCredits &&
                        !UserId.IsNullOrFullyEmpty() && isOfferIdParsed && OfferId > 0 && !Signature.IsNullOrFullyEmpty() &&
                        !encodedChannelAndApplication.IsNullOrFullyEmpty() && !customHash.IsNullOrFullyEmpty() && isCmidParsed && Cmid > 0)
                {
                    // We check the integrity of the custom data

                    string generatedCmuneHash = CmuneEconomy.SuperRewardsGenerateCmuneHash(UserId, encodedChannelAndApplication, Cmid);

                    if (customHash.Equals(generatedCmuneHash))
                    {
                        // Nobody modified our custom data, it's time to extract it!

                        CmuneEconomy.PaymentReadDeveloperId(encodedChannelAndApplication, out _applicationId, out _channel, out _bundleId);

                        // Check the Super Rewards hash

                        string channelSecret = ConfigurationUtilities.ReadConfigurationManager("SuperRewardsSecret");
                        string generatedHash = CmuneEconomy.SuperRewardsGenerateCallbackHash(TransactionId, NewCredits, UserId, channelSecret);

                        if (Signature.Equals(generatedHash))
                        {
                            isValid = true;
                        }
                        else
                        {
                            logData.Append("Signature mismatch");
                        }
                    }
                    else
                    {
                        logData.Append("Custom hash mismatch");
                    }
                }
                else
                {
                    logData.Append("Parse fail");
                }
            }
            else
            {
                logData.Append("Empty");
            }

            logData.Append("[/SuperRewardsRequest]");
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[SuperRewardsRequest: ");
            display.Append("[id: ");
            display.Append(this.TransactionId);
            display.Append("][new: ");
            display.Append(this.NewCredits);
            display.Append("][total: ");
            display.Append(this.TotalCredits);
            display.Append("][uid: ");
            display.Append(this.UserId);
            display.Append("][oid: ");
            display.Append(this.OfferId);
            display.Append("][sig: ");
            display.Append(this.Signature);
            display.Append("][ApplicationId: ");
            display.Append(this.ApplicationId);
            display.Append("][Channel: ");
            display.Append(this.Channel);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}