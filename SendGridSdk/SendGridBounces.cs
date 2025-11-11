// -----------------------------------------------------------------------
// <copyright file="SendGridBounces.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SendGridSdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SendGridSdk.Helpers;
    using System.Globalization;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// http://docs.sendgrid.com/documentation/api/web-api/webapibounces/#get
    /// </summary>
    public static class SendGridBounces
    {
        private const string Endpoint = "bounces";

        /// <summary>
        /// Note that if you want the bounces for a specific day you should use the same date for startDate and endDate
        /// Only the date is taken into account (no time)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IList<string> GetBounces(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} (endDate) should be bigger than {1} startDate", endDate, startDate));

            List<string> bouncedEmailAddresses = new List<string>();

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parametersTemplate = "&start_date={0}&end_date={1}";
            string parameters = String.Format(CultureInfo.InvariantCulture, parametersTemplate, startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            JArray response = JArray.Parse(WebApiHelper.QueryApi(url + parameters));

            foreach (var row in response.Children())
            {
                bouncedEmailAddresses.Add(row["email"].ToString());
            }

            return bouncedEmailAddresses;
        }

        public static bool DidEmailAddressBounce(string emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException("emailAddress");

            bool didBounce = false;

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parametersTemplate = "&email={0}";
            string parameters = String.Format(CultureInfo.InvariantCulture, parametersTemplate, emailAddress);

            JArray response = JArray.Parse(WebApiHelper.QueryApi(url + parameters));

            didBounce = response.Count > 0;

            return didBounce;
        }
    }
}
