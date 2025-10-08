using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections;
using Cmune.DataCenter.Utils;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.DataCenter.DataAccess;

namespace Cmune.Channels.Instrumentation.Business
{
    public struct IntPair
    {
        public int Int1;
        public int Int2;

        public IntPair(int int1, int int2)
        {
            Int1 = int1;
            Int2 = int2;
        }
    }

    public struct FloatPair
    {
        public float Float1;
        public float Float2;

        public FloatPair(float float1, float float2)
        {
            Float1 = float1;
            Float2 = float2;
        }
    }

    public struct DecimalPair
    {
        public readonly decimal Decimal1;
        public readonly decimal Decimal2;

        public DecimalPair(decimal decimal1, decimal decimal2)
        {
            Decimal1 = decimal1;
            Decimal2 = decimal2;
        }
    }

    public static class StatisticsBusiness
    {
        public static string LastQuery = string.Empty;

        public static List<DateTime> GetDaysList(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> result = new List<DateTime>();

            int noOfDays = toDate.Subtract(fromDate).Days;

            for (int i = 0; i <= noOfDays; i++)
            {
                result.Add(fromDate.AddDays(i).ToDateOnly());
            }

            return result;
        }



        public static List<DateTime> GetHoursList(DateTime fromDate, DateTime toDate)
        {
            List<DateTime> result = new List<DateTime>();

            TimeSpan timeSpan = toDate.Subtract(fromDate);

            int noOfHours = (int)Math.Round(toDate.Subtract(fromDate).TotalHours);

            for (int i = 0; i <= noOfHours; i++)
            {
                result.Add(fromDate.AddHours(i).ToDateHourOnly());
            }

            return result;
        }


        public static Dictionary<DateTime, string> GetApplicationMilestones(DateTime fromDate, DateTime toDate)
        {
            Dictionary<DateTime, string> result = new Dictionary<DateTime, string>();
            // Dictionary<DateTime, int> dailyActiveUsers = UserActivity.GetDailyActiveUsers(fromDate, toDate);

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            var query =
               from m in db.ApplicationMilestones
               where m.Date > fromDate.ToDateOnly() && m.Date < toDate.ToDateOnly()
               select new { m.Date, m.Description };

            Dictionary<DateTime, string> temp = query.ToDictionary(t => t.Date, t => t.Description);
            Dictionary<DateTime, string> milestonesByShortDate = query.ToDictionary(t => t.Date.ToDateOnly(), t => t.Description);

            foreach (DateTime day in GetDaysList(fromDate, toDate))
            {
                if (milestonesByShortDate.ContainsKey(day))
                    result.Add(day, milestonesByShortDate[day]);
                else
                    result.Add(day, String.Empty);
            }
            return result;
        }
    }
}