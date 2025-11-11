// -----------------------------------------------------------------------
// <copyright file="SendGridStatistics.cs" company="">
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
    using Newtonsoft.Json;
    using SendGridSdk.Models;

    /// <summary>
    /// http://docs.sendgrid.com/documentation/api/web-api/webapistatistics/#get
    /// </summary>
    public static class SendGridStatistics
    {
        private const string Endpoint = "stats";

        /// <summary>
        /// Note that if you want the stats for a specific day you should use the same date for startDate and endDate
        /// Only the date is taken into account (no time)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IList<DeliverabilityStatistics> GetStats(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} (endDate) should be bigger than {1} startDate", endDate, startDate));

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parametersTemplate = "&start_date={0}&end_date={1}";
            string parameters = String.Format(CultureInfo.InvariantCulture, parametersTemplate, startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            List<DeliverabilityStatistics> stats = JsonConvert.DeserializeObject<List<DeliverabilityStatistics>>(WebApiHelper.QueryApi(url + parameters));

            return stats;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static DeliverabilityStatistics GetStats()
        {
            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parameters = "&aggregate=1";

            DeliverabilityStatistics stats = JsonConvert.DeserializeObject<DeliverabilityStatistics>(WebApiHelper.QueryApi(url + parameters));

            return stats;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetCategories()
        {
            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parameters = "&list=true";

            List<string> categories = new List<string>();
            JArray response = JArray.Parse(WebApiHelper.QueryApi(url + parameters));

            foreach (var row in response.Children())
            {
                categories.Add(row["category"].ToString());
            }

            return categories;
        }

        /// <summary>
        /// Note that if you want the stats for a specific day you should use the same date for startDate and endDate
        /// Only the date is taken into account (no time)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static IList<DeliverabilityStatistics> GetCategoriesStats(DateTime startDate, DateTime endDate, IList<string> categories)
        {
            if (endDate < startDate)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.InvariantCulture, "{0} (endDate) should be bigger than {1} startDate", endDate, startDate));
            if (categories == null)
                throw new ArgumentNullException("categories");
            if (categories.Count == 0)
                throw new ArgumentOutOfRangeException("categories");

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parametersTemplate = "&start_date={0}&end_date={1}";
            string parameters = String.Format(CultureInfo.InvariantCulture, parametersTemplate, startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            string categoryParameters = String.Empty;

            if (categories.Count == 1)
            {
                categoryParameters = String.Format(CultureInfo.InvariantCulture, "&category={0}", categories[0]);
            }
            else
            {
                StringBuilder categoryParametersBuilder = new StringBuilder();

                foreach (string category in categories)
                {
                    categoryParametersBuilder.Append("&category[]=");
                    categoryParametersBuilder.Append(category);
                }

                categoryParameters = categoryParametersBuilder.ToString();
            }

            List<DeliverabilityStatistics> stats = JsonConvert.DeserializeObject<List<DeliverabilityStatistics>>(WebApiHelper.QueryApi(url + parameters + categoryParameters));

            return stats;
        }

        /// <summary>
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static IList<DeliverabilityStatistics> GetCategoriesStats(IList<string> categories)
        {
            if (categories == null)
                throw new ArgumentNullException("categories");
            if (categories.Count == 0)
                throw new ArgumentOutOfRangeException("categories");

            string url = WebApiHelper.BuildBaseUrl(Endpoint, "get");
            string parameters = "&aggregate=1";
            string categoryParameters = String.Empty;

            if (categories.Count == 1)
            {
                categoryParameters = String.Format(CultureInfo.InvariantCulture, "&category={0}", categories[0]);
            }
            else
            {
                StringBuilder categoryParametersBuilder = new StringBuilder();

                foreach (string category in categories)
                {
                    categoryParametersBuilder.Append("&category[]=");
                    categoryParametersBuilder.Append(category);
                }

                categoryParameters = categoryParametersBuilder.ToString();
            }

            List<DeliverabilityStatistics> stats = JsonConvert.DeserializeObject<List<DeliverabilityStatistics>>(WebApiHelper.QueryApi(url + parameters + categoryParameters));

            return stats;
        }
    }
}
