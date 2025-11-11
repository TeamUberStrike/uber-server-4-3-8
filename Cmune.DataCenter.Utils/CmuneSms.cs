using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Relying on http://www.smsglobal.com.au/
    /// </summary>
    public class CmuneSms
    {
        public string To { get; private set; }
        public string Text { get; private set; }
        public List<int> SmsIntervalls { get; private set; }

        public const string ActionSend = "sendsms";
        public static readonly List<string> ValidActions = new List<string> { ActionSend };

        public const string StatusCodeOk = "OK: 0;";
        public const string StatusCodeError = "Error: ";

        /// <summary>
        /// Sends a SMS
        /// </summary>
        /// <param name="action"></param>
        /// <param name="from">Do not use + before the country code</param>
        /// <param name="to">Do not use + before the country code</param>
        /// <param name="text"></param>
        /// <param name="maxSplit">Enables splitting of message if text exceeds 160 characters. Specifies number of times allowed to split.</param>
        /// <param name="scheduleDateTime"></param>
        /// <param name="statusCode"></param>
        /// <returns>If the message was sent (more details provided in statusCode)</returns>
        public static bool SendSms(string action, string from, string to, string text, int? maxSplit, DateTime? scheduleDateTime, out string statusCode)
        {
            bool isSend = false;
            statusCode = String.Empty;

            if (!ValidActions.Contains(action))
            {
                throw new ArgumentException("This action is not valid", "action");
            }

            // We'll do minimal validations as it's very complicated to validate a phone number for the whole world
            // You want to make sure this number is valid? Send an activation SMS ;)

            if (!from.IsNullOrFullyEmpty())
            {
                if (from.StartsWith("+"))
                {
                    from = from.Remove(0, 1);
                }
            }
            else
            {
                throw new ArgumentException("Empty from number", "from");
            }

            if (!to.IsNullOrFullyEmpty())
            {
                if (to.StartsWith("+"))
                {
                    to = to.Remove(0, 1);
                }
            }
            else
            {
                throw new ArgumentException("Empty to number", "to");
            }

            if (maxSplit.HasValue && ((int) maxSplit  < 0))
            {
                maxSplit = 0;
            }

            DateTime now = DateTime.Now;

            if (scheduleDateTime.HasValue && ((DateTime)scheduleDateTime < now))
            {
                // We add 3 minutes if the send date is in the past
                scheduleDateTime = now.AddSeconds(180);
            }

            string user = ConfigurationUtilities.ReadConfigurationManager("SmsGlobalUser");
            string password = ConfigurationUtilities.ReadConfigurationManager("SmsGlobalPassword");

            string url = String.Empty;

            try
            {
                using (WebClient client = new WebClient())
                {
                    string urlTemplate = "http://www.smsglobal.com.au/http-api.php?action={0}&user={1}&password={2}&from={3}&to={4}&text={5}&maxsplit={6}&scheduledatetime={7}";

                    string maxSplitText = String.Empty;
                    string scheduleDateTimeText = String.Empty;

                    if (maxSplit.HasValue)
                    {
                        maxSplitText = ((int)maxSplit).ToString();
                    }

                    if (scheduleDateTime.HasValue)
                    {
                        // Ahah not even an offset for the time zone (as we'll send in GMT and they're based in Australia I'll add + 8 hours manually)
                        scheduleDateTime = ((DateTime)scheduleDateTime).AddHours(8);
                        scheduleDateTimeText = ((DateTime)scheduleDateTime).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    action = Uri.EscapeDataString(action);
                    user = Uri.EscapeDataString(user);
                    password = Uri.EscapeDataString(password);
                    from = Uri.EscapeDataString(from);
                    to = Uri.EscapeDataString(to);
                    text = Uri.EscapeDataString(text);
                    maxSplitText = Uri.EscapeDataString(maxSplitText);
                    scheduleDateTimeText = Uri.EscapeDataString(scheduleDateTimeText);

                    url = String.Format(urlTemplate, action, user, password, from, to, text, maxSplitText, scheduleDateTimeText);
                    string response = client.DownloadString(url);

                    if (response.StartsWith(StatusCodeOk))
                    {
                        statusCode = response;
                        isSend = true;
                    }
                    else if (response.StartsWith(StatusCodeError))
                    {
                        statusCode = response;
                    }
                    else
                    {
                        statusCode = response;
                        CmuneLog.LogUnexpectedReturn(response, url);
                    }
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, url);
                isSend = false;
            }

            return isSend;
        }

        /// <summary>
        /// Sends a SMS
        /// </summary>
        /// <param name="from">Do not use + before the country code</param>
        /// <param name="to">Do not use + before the country code</param>
        /// <param name="text"></param>
        /// <param name="statusCode"></param>
        /// <returns>If the message was sent (more details provided in statusCode)</returns>
        public static bool SendSms(string from, string to, string text, out string statusCode)
        {
            return SendSms(ActionSend, from, to, text, null, null, out statusCode);
        }

        /// <summary>
        /// Sends a monitoring alert SMS
        /// </summary>
        /// <param name="to"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool SendMonitoringAlertSms(string to, string text)
        {
            bool isSend = false;
            string statusCode = String.Empty;
            string from = ConfigurationUtilities.ReadConfigurationManager("SmsAlertNumberFrom");

            if (!from.IsNullOrFullyEmpty())
            {
                isSend = SendSms(from, to, text, out statusCode);
            }

            return isSend;
        }

        /// <summary>
        /// Sends SMS at different time intervalls
        /// </summary>
        /// <param name="to"></param>
        /// <param name="text"></param>
        /// <param name="smsIntervalls">in mili seconds</param>
        public void SendMonitoringAlertSms(string to, string text, List<int> smsIntervalls)
        {
            this.To = to;
            this.Text = text;
            this.SmsIntervalls = smsIntervalls;

            Thread senderThread = new Thread(SenderLoop);
            senderThread.Name = "AlertSmsSendingThread";
            senderThread.Start();
        }

        /// <summary>
        /// Thread that is sending SMS in loop respecting the time intervalls defined in _smsIntervalls
        /// </summary>
        public void SenderLoop()
        {
            foreach (int intervall in this.SmsIntervalls)
            {
                Thread.Sleep(intervall);

                SendMonitoringAlertSms(this.To, this.Text);
            }
        }
    }
}