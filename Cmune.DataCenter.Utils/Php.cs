using System;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// This class brings some PHP functionality to .NET
    /// </summary>
    public static class Php
    {
        /// <summary>
        /// Transform a timestamp in a DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertFromTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Transform a DateTime in a timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ConvertToTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Convert.ToInt32(Math.Floor(diff.TotalSeconds));
        }

        /// <summary>
        /// Generates a PHP microtime from a Datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String MicroTime(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            String millisecond = (diff.Milliseconds / 1000.0).ToString();
            while (millisecond.Length < 10)
            {
                millisecond += "0";
            }
            String result = millisecond.ToString() + " " + ConvertToTimestamp(date).ToString();
            return result;
        }
    }
}
