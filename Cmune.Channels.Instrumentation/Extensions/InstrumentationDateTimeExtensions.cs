using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Cmune.Channels.Instrumentation.Extensions
{
    public static class InstrumentationDateTimeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToChartTooltip(this DateTime dateTime)
        {
            return dateTime.ToString("ddd, MMM dd, yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToChartAxisLabel(this DateTime dateTime)
        {
            return dateTime.ToString("MMM dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToChartLabelDateHour(this DateTime dateTime)
        {
            return dateTime.ToString("MM-dd HH", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="includeSeconds"></param>
        /// <returns></returns>
        public static string ToNiceDisplay(this DateTime dateTime, bool includeSeconds = true)
        {
            return dateTime.ToString(String.Format(CultureInfo.InvariantCulture, "HH:mm{0}, MMM dd, yyyy", includeSeconds ? ":ss" : String.Empty), CultureInfo.InvariantCulture);
        }
    }
}