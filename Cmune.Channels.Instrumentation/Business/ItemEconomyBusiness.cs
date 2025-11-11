using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.Channels.Instrumentation.Utils;
using Cmune.Instrumentation.DataAccess;

namespace Cmune.Channels.Instrumentation.Business
{
    public static class ItemEconomyBusiness
    {
        public static Dictionary<String, int> GetItemPointContribution(DateTime fromDate, DateTime toDate)
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                var query =
                   from i in db.ItemTransactions
                   where i.TransactionDate >= fromDate.ToDateOnly() && i.TransactionDate <= toDate.ToDateOnly() && i.Withdrawal.Points > 0
                   select new { i.Item.Name, i.Withdrawal.Points };

                foreach (var row in query.OrderByDescending(t => t.Points))
                {
                    if (result.ContainsKey(row.Name))
                        result[row.Name] = result[row.Name] + row.Points;
                    else
                        result.Add(row.Name, row.Points);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the list of all Cmids that bought a specific item during a certain period for a specific duration
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static List<int> GetItemBuyers(int itemId, DateTime fromDate, DateTime toDate, BuyingDurationType duration)
        {
            List<int> buyersCmid = new List<int>();

            var db = ContextHelper<CmuneDataContext>.GetCurrentContext();
            {
                var query =
                    from i in db.ItemTransactions
                    where i.TransactionDate >= fromDate && i.TransactionDate <= toDate && i.ItemId == itemId && i.DurationType == (int)duration
                    select new { Cmid = i.Cmid };

                buyersCmid = query.Select(i => i.Cmid).ToList();
            }

            return buyersCmid;
        }

        /// <summary>
        /// Get the the credits sale on an item
        /// </summary>
        /// <param name="fromDate">included</param>
        /// <param name="toDate">included</param>
        /// <param name="itemId">included</param>
        /// <returns></returns>
        public static Dictionary<DateTime, int> GetCreditItemSale(DateTime fromDate, DateTime toDate, int itemId)
        {
            Dictionary<DateTime, int> itemSale = new Dictionary<DateTime, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                toDate = toDate.AddDays(1);

                var query = from i in instrumentationDb.ItemSales
                            where i.ItemId == itemId && i.SaleDate >= fromDate && i.SaleDate < toDate
                            group i by i.SaleDate into i2
                            select new { SaleDate = i2.Key, DailyRevenue = i2.Sum(t => t.Credits) };

                // Make sure all days in the range appear in the result
                foreach (DateTime day in StatisticsBusiness.GetDaysList(fromDate, toDate))
                {
                    itemSale.Add(day, 0);
                }

                // Group any multiple rows with the same date together
                foreach (var row in query)
                {
                    itemSale[row.SaleDate] += row.DailyRevenue;
                }

                itemSale.Remove(toDate);
            }

            return itemSale;
        }

        /// <summary>
        /// Get the credits item sale ordered by revenue
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static Dictionary<int, int> GetCreditSaleByItems(DateTime fromDate, DateTime toDate)
        {
            Dictionary<int, int> creditSaleByitems = new Dictionary<int, int>();

            var instrumentationDb = ContextHelper<InstrumentationDataContext>.GetCurrentContext();
            {
                var query = from i in instrumentationDb.ItemSales
                            where i.SaleDate >= fromDate && i.SaleDate <= toDate
                            group i by i.ItemId into i2
                            select new { ItemId = i2.Key, Revenue = i2.Sum(t => t.Credits) };

                creditSaleByitems = query.OrderByDescending(i => i.Revenue).ToDictionary(i => i.ItemId, i => i.Revenue);
            }

            return creditSaleByitems;
        }
    }
}