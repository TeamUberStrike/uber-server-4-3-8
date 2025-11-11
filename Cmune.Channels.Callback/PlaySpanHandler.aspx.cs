using System;
using System.Collections.Generic;
using System.Web;
using Cmune.DataCenter.Utils;
using System.Collections.Specialized;
using System.Text;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Channels.Callback
{
    public partial class PlaySpanHandler : System.Web.UI.Page
    {
        public const string CommTypePayment = "PAYMENT";
        public const string CommTypeForcedRevearsal = "FORCED_REVERSAL";
        public const string CommTypeAdminReversal = "ADMIN_REVERSAL";
        public static readonly Dictionary<string, string> CommTypes = new Dictionary<string, string> { { CommTypePayment, CommTypePayment }, { CommTypeForcedRevearsal, CommTypeForcedRevearsal }, { CommTypeAdminReversal, CommTypeAdminReversal } };
        public const string ResultCodeSuccess = "[OK]";
        public const string ResultCodeError = "[Error]";
        public const string ResponseDelimiter = "|";
        public const string ReasonDelimiter = "~";
        public const string DateTimeFormat = "yyyyMMddHHmmss";
        public const string ReasonCodeSuccess = "[N/A]";
        public const string ReasonCodeErrorInvalidPost = "Invalid_Post";
        public const string ReasonCodeErrorInvalidAmount = "Invalid_Amount";
        public const string ReasonCodeErrorInvalidApplicationId = "Invalid_Application";
        public const string ReasonCodeErrorInvalidDeveloperId = "Invalid_Developer";

        protected void Page_Load(object sender, EventArgs e)
        {
            bool callbackSucceed = false;
            string resultCode = ResultCodeError;
            string reasonCode = String.Empty;
            string transactionId = String.Empty; // By default takes the pbctrans
            PlaySpanRequest playSpanRequest = null;
            string dateTime = String.Empty;
            StringBuilder logData = new StringBuilder();
            string infoLogFileName = PaymentProviderType.PlaySpan.ToString() + " " + DateTime.Now.Date.ToString("yyyy-MM-dd");
            StringBuilder logInfo = new StringBuilder();
            bool canLogInfo = true;

            try
            {
                DateTime communicationReceptionTime = DateTime.Now;
                dateTime = communicationReceptionTime.ToString(DateTimeFormat);
                int creditDepositId = 0;
                bool isFormValid = false;
                playSpanRequest = new PlaySpanRequest(Request, out isFormValid);

                string serversIPs = ConfigurationUtilities.ReadConfigurationManager("PlaySpanServersIPs");
                string serverIP = Request.UserHostAddress;

                logData.Append("isRequestValid:[");
                logData.Append(isFormValid);
                logData.Append("] serverIP:[");
                logData.Append(serverIP);
                logData.Append("]");

                if (isFormValid && !serverIP.IsNullOrFullyEmpty() && serversIPs.Contains(serverIP))
                {
                    logData.Append("payment duplicate:[");
                    if (!CmuneEconomy.PlaySpanIsTransactionExecuted(playSpanRequest.Pbctrans))
                    {
                        logData.Append("no]");

                        PlaySpanTransactionType playspanTransactionType = PlaySpanTransactionType.Payment;

                        // Optionally, the following name-value pairs can be returned at your request: quoted_amount, 
                        // quoted_currency, pay_name, pay_logo_url, order_number, language, country, 
                        // full_name, support_url, hold_period.

                        logData.Append("transactionType:[");

                        if (playSpanRequest.CommunicationType.Equals(CommTypePayment))
                        {
                            logData.Append("Payment]");

                            bool isDeveloperIdValid = true;

                            int applicationId = 0;
                            ChannelType channelType = ChannelType.WebPortal;
                            int bundleId = 0;

                            CmuneEconomy.PaymentReadDeveloperId(playSpanRequest.DeveloperId, out applicationId, out channelType, out bundleId);

                            if (!CommonConfig.ApplicationsName.ContainsKey(applicationId))
                            {
                                isDeveloperIdValid = false;
                                reasonCode = BuildReasonCode(reasonCode, ReasonCodeErrorInvalidApplicationId);
                                logData.Append("[Application Id is invalid]");
                                CmuneLog.LogUnexpectedReturn(applicationId, "This application Id is not recognized by our system. [" + playSpanRequest.ToString() + "]", infoLogFileName);
                            }

                            if (isDeveloperIdValid)
                            {
                                playspanTransactionType = PlaySpanTransactionType.Payment;

                                // We should use SepAmount as we're selling digital goods
                                decimal cash = playSpanRequest.SepAmount; // TODO in currency (USD)???? We might need to convert from other currency

                                bool isAdminAction = false;

                                logData.Append("testMode:[");

                                // If we're in test mode, we should attribute the credits as an admin.
                                if (playSpanRequest.Sn.Equals(ConfigurationUtilities.ReadConfigurationManager("PlaySpanMerchantCode")))
                                {
                                    logData.Append("No]");
                                    isAdminAction = false;
                                }
                                else if (playSpanRequest.Sn.Equals(ConfigurationUtilities.ReadConfigurationManager("PlaySpanMerchantTestCode")))
                                {
                                    logData.Append("Yes]");
                                    isAdminAction = true;
                                }

                                CmuneEconomy.ProcessBundleAttribution(playSpanRequest.UserId, bundleId, playSpanRequest.Currency, isAdminAction, PaymentProviderType.PlaySpan, playSpanRequest.Pbctrans, applicationId, channelType, out creditDepositId);

                                logData.Append("creditDepositId:[");
                                logData.Append(creditDepositId);
                                logData.Append("]");

                                callbackSucceed = true;

                                // These parameters will be passed with commtype=PAYMENT:  
                                // login, adminpwd, commtype, detail, userid, accountname, dtdatetime, currency, amount, sepamount, set_amount, 
                                // paymentid, pkgid, pbctrans, merchtrans, sn, developerid, appid, mirror, rescode, virtualamount, 
                                // virtualcurrency, gwtid and hash.

                                transactionId = creditDepositId.ToString();
                            }
                            else
                            {
                                CmuneLog.LogUnexpectedReturn(playSpanRequest.DeveloperId, "This developerId is invalid. [" + playSpanRequest.ToString() + "]", infoLogFileName);
                            }
                        }
                        else if (playSpanRequest.CommunicationType.Equals(CommTypeForcedRevearsal))
                        {
                            logData.Append("ForcedReversal]");

                            playspanTransactionType = PlaySpanTransactionType.ForcedReversal;

                            // These parameters will be passed with commtype=FORCED_REVERSAL: A FORCED_REVERSAL shall close and block the customer’s account, denying him or her access to whatever 
                            // goods or services may have been purchased with this payment, until UltimatePay sends another “payment” 
                            // communication.  If UltimatePay sends a PAYMENT communication for this userid, it means that UltimatePay has 
                            // collected the bad debt incurred by the customer when their previous payment was reversed. 
                            // login, adminpwd, commtype, detail, userid, accountname, dtdatetime, currency, set_amount, amount, sepamount, 
                            // pkgid, pbctrans, merchtrans, sn, developerid, appid, mirror, virtualamount, virtualcurrency and hash.

                            CmuneMail.SendEmail(CommonConfig.CmuneNoReplyEmail, CommonConfig.CmuneNoReplyEmailName, CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, "PlaySpan FORCED_REVERSAL", playSpanRequest.ToString(), playSpanRequest.ToString());

                            creditDepositId = 0;
                            transactionId = playSpanRequest.Pbctrans;
                            CmuneEconomy.PlaySpanRecordTransaction(playSpanRequest.Pbctrans, creditDepositId, communicationReceptionTime, playspanTransactionType);
                            callbackSucceed = true;
                        }
                        else if (playSpanRequest.CommunicationType.Equals(CommTypeAdminReversal))
                        {
                            playspanTransactionType = PlaySpanTransactionType.AdminReversal;

                            logData.Append("AdminReversal]");

                            // These parameters will be passed with commtype=ADMIN_REVERSAL:  
                            // login, adminpwd, commtype, detail, userid, accountname, dtdatetime, currency, set_amount, amount, sepamount, pkgid, pbctrans, 
                            // merchtrans, sn, developerid, appid, mirror, virtualamount, virtualcurrency and hash.

                            CmuneMail.SendEmail(CommonConfig.CmuneNoReplyEmail, CommonConfig.CmuneNoReplyEmailName, CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, "PlaySpan ADMIN_REVERSAL", playSpanRequest.ToString(), playSpanRequest.ToString());

                            transactionId = playSpanRequest.Pbctrans;
                            creditDepositId = 0;
                            CmuneEconomy.PlaySpanRecordTransaction(playSpanRequest.Pbctrans, creditDepositId, communicationReceptionTime, playspanTransactionType);
                            callbackSucceed = true;
                        }
                        else
                        {
                            CmuneLog.LogUnexpectedReturn(playSpanRequest, "Unknown playSpanRequest.CommunicationType(" + playSpanRequest.CommunicationType.ToString() + ")", infoLogFileName);
                        }
                    }
                    else
                    {
                        logData.Append("yes]");

                        callbackSucceed = true;
                        transactionId = playSpanRequest.Pbctrans;
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

                    reasonCode = BuildReasonCode(reasonCode, ReasonCodeErrorInvalidPost);
                    CmuneLog.LogUnexpectedReturn(playSpanRequest, "Invalid POST (Server IP: [" + serverIP + "].", infoLogFileName);
                }
            }
            catch (Exception ex)
            {
                string requestContent = String.Empty;

                if (playSpanRequest != null)
                {
                    requestContent = playSpanRequest.ToString();
                }

                CmuneLog.LogException(ex, requestContent, infoLogFileName);
            }
            finally
            {
                try
                {
                    if (callbackSucceed)
                    {
                        resultCode = ResultCodeSuccess;
                        reasonCode = ReasonCodeSuccess;
                    }
                    else
                    {
                        resultCode = ResultCodeError;
                        transactionId = playSpanRequest.Pbctrans;
                    }

                    string[] responseFields = new string[] { resultCode, dateTime, transactionId, reasonCode };
                    string response = String.Join(ResponseDelimiter, responseFields);
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                    Response.ContentType = "plain/text";

                    logData.Append("response:[");
                    logData.Append(response);
                    logData.Append("][callback data: ");
                    logData.Append(playSpanRequest);
                    logData.Append("]");

                    bool logPlaySpan = ConfigurationUtilities.ReadConfigurationManagerBool("PlaySpanActivateFullLog");

                    if (logPlaySpan)
                    {
                        CmuneLog.LogUnexpectedReturn(logData, "playspan", infoLogFileName);
                    }
                    if (canLogInfo)
                    {
                        logInfo.Append(logData);
                        CmuneLog.LogInfo(logInfo.ToString(), infoLogFileName);
                    }
                    Response.Write(response);
                }
                catch (Exception ex)
                {
                    CmuneLog.LogException(ex, playSpanRequest != null ? playSpanRequest.ToString() : "", infoLogFileName);
                }
            }
        }

        /// <summary>
        /// Builds the reason code returned to PlaySpan, allows us to stack reason codes
        /// </summary>
        /// <param name="currentReasonCode"></param>
        /// <param name="reasonCodeToAdd"></param>
        /// <returns></returns>
        protected string BuildReasonCode(string currentReasonCode, string reasonCodeToAdd)
        {
            if (!currentReasonCode.IsNullOrFullyEmpty())
            {
                currentReasonCode = currentReasonCode + ReasonDelimiter;
            }

            currentReasonCode = currentReasonCode + reasonCodeToAdd;

            return currentReasonCode;
        }
    }

    public class PlaySpanRequest
    {
        #region Fields

        private string _login;
        private string _adminPassword;
        private string _communicationType;
        private string _detail;
        private int _userId; // Internal user Id
        private string _accountName;
        private string _dtDateTime;
        private string _currency;
        private decimal _amount; // For subscriptions
        private decimal _sepAmount; // For digital goods
        private decimal _settlementAmount; // Amount we get paid
        private string _paymentId;
        private string _pkgId;
        private string _pbctrans; // UltimatePay transaction Id
        private string _merchtrans;
        private string _sn; // Merchant code
        private string _developerId; // Internal developer Id
        private string _applicationId; // Internal application Id
        private string _mirror;
        private string _resCode;
        private int _virtualAmount;
        private string _virtualCurrency;
        private string _gwtId;
        private string _hash;

        #endregion Fields

        #region Properties

        public string Login
        {
            get { return _login; }
            private set { _login = value; }
        }

        public string AdminPassword
        {
            get { return _adminPassword; }
            private set { _adminPassword = value; }
        }

        public string CommunicationType
        {
            get { return _communicationType; }
            private set { _communicationType = value; }
        }

        public string Detail
        {
            get { return _detail; }
            private set { _detail = value; }
        }

        public int UserId
        {
            get { return _userId; }
            private set { _userId = value; }
        }

        public string AccountName
        {
            get { return _accountName; }
            private set { _accountName = value; }
        }

        public string DtDateTime
        {
            get { return _dtDateTime; }
            private set { _dtDateTime = value; }
        }

        public string Currency
        {
            get { return _currency; }
            private set { _currency = value; }
        }

        public decimal Amount
        {
            get { return _amount; }
            private set { _amount = value; }
        }

        public decimal SepAmount
        {
            get { return _sepAmount; }
            private set { _sepAmount = value; }
        }

        public decimal SettlementAmount
        {
            get { return _settlementAmount; }
            private set { _settlementAmount = value; }
        }

        public string PaymentId
        {
            get
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a paymentid only for a commtype of type PAYMENT.");
                }

                return _paymentId;
            }
            private set
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a paymentid only for a commtype of type PAYMENT.");
                }

                _paymentId = value;
            }
        }

        public string PkgId
        {
            get { return _pkgId; }
            private set { _pkgId = value; }
        }

        public string Pbctrans
        {
            get { return _pbctrans; }
            private set { _pbctrans = value; }
        }

        public string Merchtrans
        {
            get { return _merchtrans; }
            private set { _merchtrans = value; }
        }

        public string Sn
        {
            get { return _sn; }
            private set { _sn = value; }
        }

        public string DeveloperId
        {
            get { return _developerId; }
            private set { _developerId = value; }
        }

        public string ApplicationId
        {
            get { return _applicationId; }
            private set { _applicationId = value; }
        }

        public string Mirror
        {
            get { return _mirror; }
            private set { _mirror = value; }
        }

        public string ResCode
        {
            get
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a rescode only for a commtype of type PAYMENT.");
                }

                return _resCode;
            }
            private set
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a rescode only for a commtype of type PAYMENT.");
                }

                _resCode = value;
            }
        }

        public int VirtualAmount
        {
            get { return _virtualAmount; }
            private set { _virtualAmount = value; }
        }

        public string VirtualCurrency
        {
            get { return _virtualCurrency; }
            private set { _virtualCurrency = value; }
        }

        public string GwtId
        {
            get
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a gwtid only for a commtype of type PAYMENT.");
                }

                return _gwtId;
            }
            private set
            {
                if (!this.CommunicationType.Equals(PlaySpanHandler.CommTypePayment))
                {
                    throw new NotImplementedException("There is a gwtid only for a commtype of type PAYMENT.");
                }

                _gwtId = value;
            }
        }

        public string Hash
        {
            get { return _hash; }
            private set { _hash = value; }
        }

        #endregion Properties

        #region Constructors

        public PlaySpanRequest(HttpRequest request, out bool isValid)
        {
            isValid = false;

            NameValueCollection callbackData = new NameValueCollection();

            if (request.HttpMethod.Equals("GET"))
            {
                callbackData = request.QueryString;
            }
            else if (request.HttpMethod.Equals("POST"))
            {
                callbackData = request.Form;
            }
            // TODO We could add extra logging here: neither POST or GET

            if (callbackData.Count > 0)
            {
                this._accountName = callbackData["accountname"];
                this._adminPassword = callbackData["adminpwd"];
                bool isAmountParsed = Decimal.TryParse(callbackData["amount"], out this._amount);
                this._applicationId = callbackData["appid"];
                this._communicationType = callbackData["commtype"];
                this._currency = callbackData["currency"];
                this._detail = callbackData["detail"];
                this._developerId = callbackData["developerid"];
                this._dtDateTime = callbackData["dtdatetime"];
                this._gwtId = callbackData["gwtid"];
                this._hash = callbackData["hash"];
                this._login = callbackData["login"];
                this._merchtrans = callbackData["merchtrans"];
                this._mirror = callbackData["mirror"];
                this._paymentId = callbackData["paymentid"];
                this._pbctrans = callbackData["pbctrans"];
                this._pkgId = callbackData["pkgid"];
                this._resCode = callbackData["rescode"];
                bool isSepAmountParsed = Decimal.TryParse(callbackData["sepamount"], out this._sepAmount);
                bool isSetAmountParsed = Decimal.TryParse(callbackData["set_amount"], out this._settlementAmount);
                this._sn = callbackData["sn"];
                bool isUserIdParsed = Int32.TryParse(callbackData["userid"], out this._userId);
                bool isVirtualAmountParsed = Int32.TryParse(callbackData["virtualamount"], out this._virtualAmount);
                this._virtualCurrency = callbackData["virtualcurrency"];

                // First we check all the fields that shouldn't be null or empty

                if (!this._dtDateTime.IsNullOrFullyEmpty() && !this._login.IsNullOrFullyEmpty() && !this._adminPassword.IsNullOrFullyEmpty() && isSetAmountParsed && isAmountParsed
                    && isUserIdParsed && this._userId > 0 && !this._currency.IsNullOrFullyEmpty() && !this._sn.IsNullOrFullyEmpty() && !this._pbctrans.IsNullOrFullyEmpty()
                    && PlaySpanHandler.CommTypes.ContainsKey(this.CommunicationType) && isSepAmountParsed)
                {
                    // Match required parameters

                    string merchantCodeConfig = ConfigurationUtilities.ReadConfigurationManager("PlaySpanMerchantCode");
                    string merchantCodeTestConfig = ConfigurationUtilities.ReadConfigurationManager("PlaySpanMerchantTestCode");
                    string loginConfig = ConfigurationUtilities.ReadConfigurationManager("PlaySpanLogin");
                    string adminPasswordConfig = ConfigurationUtilities.ReadConfigurationManager("PlaySpanAdminPassword");

                    if ((merchantCodeConfig.Equals(this._sn) || merchantCodeTestConfig.Equals(this._sn)) && loginConfig.Equals(this._login) && adminPasswordConfig.Equals(this._adminPassword))
                    {
                        // Check the hash

                        string secretHashPassPhrase = ConfigurationUtilities.ReadConfigurationManager("PlaySpanSecretHashPassPhrase");

                        string generatedHash = CmuneEconomy.PlaySpanGenerateCallbackHash(this._dtDateTime, this._login, this._adminPassword, secretHashPassPhrase, this._userId, this._communicationType, this._settlementAmount, this._amount, this._sepAmount, this._currency, this._sn, this._mirror, this._pbctrans, this._developerId, this._applicationId, this._virtualAmount, this._virtualCurrency);

                        if (generatedHash.Equals(this._hash))
                        {
                            isValid = true;
                        }
                    }
                }
            }
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[PlaySpanRequest: ");
            display.Append("[login: ");
            display.Append(this._login);
            display.Append("][adminpwd: ");
            display.Append(this._adminPassword);
            display.Append("][commtype: ");
            display.Append(this._communicationType);
            display.Append("][detail: ");
            display.Append(this._detail);
            display.Append("][userid: ");
            display.Append(this._userId);
            display.Append("][accountname: ");
            display.Append(this._accountName);
            display.Append("][dtdatetime: ");
            display.Append(this._dtDateTime);
            display.Append("][currency: ");
            display.Append(this._currency);
            display.Append("][amount: ");
            display.Append(this._amount);
            display.Append("][sepamount: ");
            display.Append(this._sepAmount);
            display.Append("][set_amount: ");
            display.Append(this._settlementAmount);
            display.Append("][paymentid: ");
            display.Append(this._paymentId);
            display.Append("][pkgid: ");
            display.Append(this._pkgId);
            display.Append("][pbctrans: ");
            display.Append(this._pbctrans);
            display.Append("][merchtrans: ");
            display.Append(this._merchtrans);
            display.Append("][sn: ");
            display.Append(this._sn);
            display.Append("][developerid: ");
            display.Append(this._developerId);
            display.Append("][appid: ");
            display.Append(this._applicationId);
            display.Append("][mirror: ");
            display.Append(this._mirror);
            display.Append("][rescode: ");
            display.Append(this._resCode);
            display.Append("][virtualamount: ");
            display.Append(this._virtualAmount);
            display.Append("][virtualcurrency: ");
            display.Append(this._virtualCurrency);
            display.Append("][gwtid: ");
            display.Append(this._gwtId);
            display.Append("][hash: ");
            display.Append(this._hash);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}