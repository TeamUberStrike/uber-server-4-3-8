// -----------------------------------------------------------------------
// <copyright file="WebApiHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SendGridSdk.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SendGridSdk.Models;
    using Newtonsoft.Json.Linq;
    using System.Globalization;
    using System.Net;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class WebApiHelper
    {
        internal static SendGridApiResponse ReadResponseStatus(JObject response)
        {
            bool isSuccess = false;
            List<string> errorMessages = new List<string>();

            string message = response["message"].ToString();

            if (message == "success")
            {
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }

            SendGridApiResponse apiResponse = new SendGridApiResponse(isSuccess, errorMessages);

            return apiResponse;
        }

        internal static string BuildBaseUrl(string endpoint, string method)
        {
            if (String.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException("endpoint");
            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentNullException("method");

            string url = String.Empty;

            SendGridConfigurationSection config = SendGridConfig.GetConfig();

            url = String.Format(CultureInfo.InvariantCulture, SendGridConfig.ApiUrl, endpoint, method, "json", config.ApiUser, config.ApiKey);

            return url;
        }

        /// <summary>
        /// 2XX – The API call was successful.
        /// 4XX – The API call had an error in the parameters. The error will be encoded in the body of the response.
        /// 5XX – The API call was unsuccessful. You should retry later.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static string QueryApi(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url");

            string response = String.Empty;

            using(WebClient client = new WebClient())
            {
                try
                {
                    response = client.DownloadString(url);
                }
                catch (WebException we)
                {
                    HttpWebResponse webResponse = (HttpWebResponse)we.Response;
                    int statusCode = (int) webResponse.StatusCode;

                    if (statusCode >= 400 && statusCode < 500)
                    {
                        string bodyResponse = String.Empty;

                        using (Stream responseStream = webResponse.GetResponseStream())
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            bodyResponse = responseReader.ReadToEnd();
                        }

                        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "One of your parameter was invalid. Error message: {0}", bodyResponse));
                    }
                    else if (statusCode >= 500 && statusCode < 600)
                    {
                        throw new InvalidOperationException("The API call was unsuccessful. You should retry later.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return response;
        }
    }
}
