using System;
using System.Globalization;

namespace Cmune.DataCenter.Utils
{
    public static class DateTimeExtensions
    {
        /// <summary>Get the week number of a certain date
        /// </summary>
        /// <param name="date">Date of interest.</param>
        /// <returns>The week number.</returns>
        public static int WeekOfYear(this DateTime date)
        {
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            int weekNumber = myCal.GetWeekOfYear(date, myCWR, myFirstDOW);
            return weekNumber;
        }

        /// <summary>
        /// Get the first date of the week for a certain date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return date;
            }

            int week = date.WeekOfYear();

            while (week == date.WeekOfYear())
            {
                date = date.AddDays(-1);
            }

            return date.AddDays(1);
        }

        /// <summary>
        /// Get the last date of the week for a certain date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MaxValue)
            {
                return date;
            }

            int week = date.WeekOfYear();
            while (week == date.WeekOfYear())
            {
                date = date.AddDays(1);
            }

            return date.AddDays(-1);
        }

        /// <summary>
        /// Get the first date of the next week for a certain date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDateOfNextWeek(this DateTime date)
        {
            date = date.GetLastDateOfWeek();

            if (date == DateTime.MaxValue)
            {
                return date;
            }

            return date.AddDays(1);
        }

        /// <summary>
        /// Get the first date of the previous week for certain date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDateOfPreviousWeek(this DateTime date)
        {
            date = date.GetFirstDateOfWeek();

            if (date == DateTime.MinValue)
            {
                return date;
            }

            date = date.AddDays(-1);

            return date.GetFirstDateOfWeek();
        }

        /// <summary>
        /// Convert any DateTime to Beijing DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertToBeijingTime(this DateTime date)
        {
            TimeZoneInfo beijingTimeZoneInfo;
            DateTime beijingTime;
            beijingTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            beijingTime = TimeZoneInfo.ConvertTime(date, beijingTimeZoneInfo);
            return beijingTime;
        }

        /// <summary>
        /// Discards time information from DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToDateOnly(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// Return a datetime in date and hour format only
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToDateHourOnly(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        }

        /// <summary>
        /// Returns a string compatible with MS SQL DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToYMDDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToEasyReadableFormat(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm, dd MMM, yyyy");
        }

        /// <summary>
        /// Round a datetime to quarter
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToQuarter(this DateTime dateTime)
        {
            decimal quarters = (decimal)dateTime.Minute / (decimal)15;
            dateTime = dateTime.Date + new TimeSpan(0, dateTime.Hour, (int)quarters * 15, 0, 0);
            return dateTime;
        }

        /// <summary>
        /// Round a datetime to minutes divisibles by 5
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToMinutesDivisiblesByFive(this DateTime dateTime)
        {
            decimal dividente = (decimal)dateTime.Minute / (decimal)5;
            dateTime = dateTime.Date + new TimeSpan(0, dateTime.Hour, (int)Math.Round(dividente, 0, MidpointRounding.AwayFromZero) * 5, 0, 0);
            return dateTime;
        }

    }
}
