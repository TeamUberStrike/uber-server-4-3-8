// -----------------------------------------------------------------------
// <copyright file="SendGridSpamReports.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SendGridSdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using SendGridSdk.Helpers;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SendGridSpamReports
    {
        private const string Endpoint = "spamreports";

        /// <summary>
        /// Note that if you want the spam reports addresses for a specific day you should use the same date for startDate and endDate
        /// Only the date is taken into account (no time)
        /// http://docs.sendgrid.com/documentation/api/web-api/webapispamreports/#get
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IList<string> GetSpamReports(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} (endDate) should be bigger than {1} startDate", endDate, startDate));

            List<string> invalideEmailAddresses = new List<string>();

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parametersTemplate = "&start_date={0}&end_date={1}";
            string parameters = String.Format(CultureInfo.InvariantCulture, parametersTemplate, startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            JArray response = JArray.Parse(WebApiHelper.QueryApi(url + parameters));

            foreach (var row in response.Children())
            {
                invalideEmailAddresses.Add(row["email"].ToString());
            }

            return invalideEmailAddresses;
        }
    }
}
