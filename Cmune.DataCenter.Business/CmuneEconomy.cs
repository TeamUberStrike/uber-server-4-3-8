using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// This class manages the credits/points of a member
    /// </summary>
    public static class CmuneEconomy
    {
        #region Getters

        /// <summary>
        /// Get the Credit of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static Credit GetCreditByMemberID(int cmid)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                Credit memberCredit = GetMemberWallet(cmid, cmuneDbCenter);
                return memberCredit;
            }
        }

        /// <summary>
        /// Get the credits of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Credit GetCredit(int cmid, CmuneDataContext cmuneDB)
        {
            return GetMemberWallet(cmid, cmuneDB);
        }

        /// <summary>
        /// Gets the wallet of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Credit GetMemberWallet(int cmid, CmuneDataContext cmuneDB)
        {
            Credit memberWallet = null;

            if (cmuneDB != null)
            {
                memberWallet = cmuneDB.Credits.SingleOrDefault(f => f.UserId == cmid);
            }

            return memberWallet;
        }

        /// <summary>
        /// Gets the wallet of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberWalletView GetMemberWallet(int cmid)
        {
            MemberWalletView memberWallet = null;

            Credit memberCredit = GetCreditByMemberID(cmid);

            if (memberCredit != null)
            {
                memberWallet = new MemberWalletView(memberCredit.UserId, memberCredit.NbCredits, memberCredit.NbPoints, memberCredit.ExpDateCredit, memberCredit.ExpDatePoint);
            }

            return memberWallet;
        }

        private static DateTime GetLowerDateLimitForTransactions()
        {
            return DateTime.Now.AddMonths(-3);
        }

        #region Currency deposits

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="elementsPerPage"></param>
        /// <param name="getOldDeposits">if false will get only the most recent deposits</param>
        /// <returns></returns>
        public static List<CurrencyDepositView> GetCurrencyDepositsView(int cmid, int pageIndex, int elementsPerPage, bool getOldDeposits = false)
        {
            List<CurrencyDepositView> currencyDepositsView = new List<CurrencyDepositView>();
            List<CreditDeposit> currencyDeposits = GetCurrencyDeposits(cmid, pageIndex, elementsPerPage, getOldDeposits);
            List<BundleView> bundles = CmuneBundle.GetAllBundlesView();
            Dictionary<int, BundleView> bundlesOrdered = bundles.ToDictionary(u => u.Id, u => u);

            foreach (CreditDeposit currencyDeposit in currencyDeposits)
            {
                int bundlePoints = 0;
                string bundleName = "-";

                if (currencyDeposit.BundleId.HasValue)
                {
                    int bundleId = currencyDeposit.BundleId.Value;

                    if (bundlesOrdered.ContainsKey(bundleId))
                    {
                        bundleName = bundlesOrdered[bundleId].Name;
                        bundlePoints = bundlesOrdered[bundleId].Points;
                    }
                    else
                    {
                        bundleName = "Deleted bundle";
                    }
                }

                CurrencyDepositView currencyDepositView = new CurrencyDepositView(currencyDeposit.id, currencyDeposit.DepositDate, currencyDeposit.NbCredit, bundlePoints, currencyDeposit.NbCash, currencyDeposit.CurrencyLabel, currencyDeposit.UserId, currencyDeposit.isAdminAction, (PaymentProviderType)currencyDeposit.PartnerID, currencyDeposit.TransactionKey, currencyDeposit.ApplicationID, (ChannelType)currencyDeposit.ChannelID, currencyDeposit.UsdAmount, currencyDeposit.BundleId, bundleName);
                currencyDepositsView.Add(currencyDepositView);
            }

            return currencyDepositsView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="elementsPerPage"></param>
        /// <param name="getOldDeposits">if false will get only the most recent deposits</param>
        /// <returns></returns>
        public static List<CreditDeposit> GetCurrencyDeposits(int cmid, int pageIndex, int elementsPerPage, bool getOldDeposits = false)
        {
            List<CreditDeposit> currencyDeposits = new List<CreditDeposit>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int elementsToSkip = 0;

                if (pageIndex > 1)
                {
                    elementsToSkip = (pageIndex - 1) * elementsPerPage;
                }


                if (getOldDeposits)
                {
                    currencyDeposits = (from i in cmuneDb.CreditDeposits
                                        where i.UserId == cmid
                                        orderby i.id descending
                                        select i).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
                else
                {
                    currencyDeposits = (from i in cmuneDb.CreditDeposits
                                        where i.UserId == cmid && i.DepositDate >= GetLowerDateLimitForTransactions()
                                        orderby i.id descending
                                        select i).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
            }

            return currencyDeposits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="getOldDeposits">if false will count only the most recent deposits</param>
        /// <returns></returns>
        public static int GetCurrencyDepositsCount(int cmid, bool getOldDeposits = false)
        {
            int count = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (getOldDeposits)
                {
                    count = (from i in cmuneDb.CreditDeposits
                             where i.UserId == cmid
                             select i.id).Count();
                }
                else
                {
                    count = (from i in cmuneDb.CreditDeposits
                             where i.UserId == cmid && i.DepositDate >= GetLowerDateLimitForTransactions()
                             select i.id).Count();
                }
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="getOldDeposits">if false will count only the most recent deposits</param>
        /// <returns></returns>
        public static int GetCurrencyDepositsViewCount(int cmid, bool getOldDeposits = false)
        {
            return GetCurrencyDepositsCount(cmid, getOldDeposits);
        }

        /// <summary>
        /// Get the sum of USD spent by a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static decimal GetTotalCurrencyDeposits(int cmid)
        {
            decimal totalCurrencyDeposits = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                decimal? totalCurrencyDepositsTmp = (from c in cmuneDb.CreditDeposits
                                                     where c.UserId == cmid && !c.isAdminAction
                                                     select c.UsdAmount).Sum(i => (decimal?)i);

                if (totalCurrencyDepositsTmp.HasValue)
                {
                    totalCurrencyDeposits = totalCurrencyDepositsTmp.Value;
                }
            }

            return totalCurrencyDeposits;
        }

        /// <summary>
        /// Get USD spent by users
        /// </summary>
        /// <param name="cmids"></param>
        /// <returns></returns>
        public static Dictionary<int, decimal> GetTotalCurrencyDeposits(List<int> cmids)
        {
            Dictionary<int, decimal> userUsdDeposited = cmids.ToDictionary(c => c, c => 0m);

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Dictionary<int, decimal> userUsdDepositedTmp = (from c in cmuneDb.CreditDeposits
                                                                where cmids.Contains(c.UserId) && !c.isAdminAction
                                                                group c by c.UserId into c2
                                                                select new { Cmid = c2.Key, UsdDeposited = c2.Sum(c => c.UsdAmount) }).ToDictionary(q => q.Cmid, q => q.UsdDeposited);

                foreach (int cmid in userUsdDepositedTmp.Keys)
                {
                    userUsdDeposited[cmid] = userUsdDepositedTmp[cmid];
                }
            }

            return userUsdDeposited;
        }

        /// <summary>
        /// Gets the credit deposits for a specific partner
        /// </summary>
        /// <param name="parterId"></param>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns></returns>
        public static List<CreditDeposit> GetCreditDeposits(PaymentProviderType parterId, DateTime startingDate, DateTime endingDate)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<CreditDeposit> creditDeposits = new List<CreditDeposit>();

                creditDeposits = cmuneDB.CreditDeposits.Where(c => c.PartnerID == (int)parterId && c.DepositDate >= startingDate && c.DepositDate <= endingDate && c.isAdminAction == false).ToList();

                return creditDeposits;
            }
        }

        #endregion

        #region Points deposits

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="elementsPerPage"></param>
        /// <param name="getOldPointDeposits">if false will get only the most recent point deposits</param>
        /// <returns></returns>
        public static List<PointDepositView> GetPointDepositView(int cmid, int pageIndex, int elementsPerPage, bool getOldPointDeposits = false)
        {
            List<PointDepositView> pointDepositsView = new List<PointDepositView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int elementsToSkip = 0;

                if (pageIndex > 1)
                {
                    elementsToSkip = (pageIndex - 1) * elementsPerPage;
                }

                if (getOldPointDeposits)
                {
                    pointDepositsView = (from p in cmuneDb.PointDeposits
                                         where p.UserId == cmid
                                         orderby p.id descending
                                         select new PointDepositView(p.id, p.DepositDate, p.NbPoints, p.UserId, p.isAdminAction, (PointsDepositType)p.DepositType)).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
                else
                {
                    pointDepositsView = (from p in cmuneDb.PointDeposits
                                         where p.UserId == cmid && p.DepositDate >= GetLowerDateLimitForTransactions()
                                         orderby p.DepositDate descending
                                         select new PointDepositView(p.id, p.DepositDate, p.NbPoints, p.UserId, p.isAdminAction, (PointsDepositType)p.DepositType)).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
            }

            return pointDepositsView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="getOldPointDeposits">if false will count only the most recent point deposits</param>
        /// <returns></returns>
        public static int GetPointDepositViewCount(int cmid, bool getOldPointDeposits = false)
        {
            int count = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (getOldPointDeposits)
                {
                    count = (from p in cmuneDb.PointDeposits
                             where p.UserId == cmid
                             select p.id).Count();
                }
                else
                {
                    count = (from p in cmuneDb.PointDeposits
                             where p.UserId == cmid && p.DepositDate >= GetLowerDateLimitForTransactions()
                             select p.id).Count();
                }
            }

            return count;
        }

        #endregion

        #region Item transactions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="elementsPerPage"></param>
        /// <param name="getOldTransactions">if false will get only the most recent withdrawals</param>
        /// <returns></returns>
        public static List<ItemTransactionView> GetItemTransactionsView(int cmid, int pageIndex, int elementsPerPage, bool getOldTransactions = false)
        {
            List<ItemTransactionView> transactionsView = new List<ItemTransactionView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int elementsToSkip = 0;

                if (pageIndex > 1)
                {
                    elementsToSkip = (pageIndex - 1) * elementsPerPage;
                }

                if (getOldTransactions)
                {
                    transactionsView = (from w in cmuneDb.Withdrawals
                                        join i in cmuneDb.ItemTransactions on w.WithdrawalId equals i.WithdrawalId
                                        where w.Cmid == cmid
                                        orderby w.WithdrawalId descending
                                        select new ItemTransactionView(w.WithdrawalId, w.WithdrawalDate, w.Points, w.Credits, w.Cmid, w.IsAdminAction, i.ItemId, (BuyingDurationType)i.DurationType)).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
                else
                {
                    transactionsView = (from w in cmuneDb.Withdrawals
                                        join i in cmuneDb.ItemTransactions on w.WithdrawalId equals i.WithdrawalId
                                        where w.Cmid == cmid && w.WithdrawalDate >= GetLowerDateLimitForTransactions()
                                        orderby w.WithdrawalId descending
                                        select new ItemTransactionView(w.WithdrawalId, w.WithdrawalDate, w.Points, w.Credits, w.Cmid, w.IsAdminAction, i.ItemId, (BuyingDurationType)i.DurationType)).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
            }

            return transactionsView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="getOldTransactions">if false will count only the most recent withdrawals</param>
        /// <returns></returns>
        public static int GetItemTransactionsViewCount(int cmid, bool getOldTransactions = false)
        {
            int count = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (getOldTransactions)
                {
                    count = (from w in cmuneDb.Withdrawals
                             where w.Cmid == cmid
                             select w.WithdrawalId).Count();
                }
                else
                {
                    count = (from w in cmuneDb.Withdrawals
                             where w.Cmid == cmid && w.WithdrawalDate >= GetLowerDateLimitForTransactions()
                             select w.WithdrawalId).Count();
                }
            }

            return count;
        }

        #endregion

        #endregion

        #region Attribute points / credits

        /// <summary>
        /// Attributes points when winning a Lucky Draw / Mystery Box
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="points"></param>
        public static void AttributePointsWhenWinningLuckyDrawMysteryBox(int cmid, int points)
        {
            AttributePoints(cmid, points, true, PointsDepositType.LuckyDrawMysteryBoxPrize);
        }

        /// <summary>
        /// Add Points
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="points"></param>
        /// <param name="isAdminAction"></param>
        /// <param name="depositType"></param>
        public static void AttributePoints(int cmid, int points, bool isAdminAction, PointsDepositType depositType)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                try
                {
                    if (points > 0)
                    {
                        PointDeposit pointsDeposit = new PointDeposit();
                        pointsDeposit.DepositDate = DateTime.Now;
                        pointsDeposit.NbPoints = points;
                        pointsDeposit.UserId = cmid;
                        pointsDeposit.isAdminAction = isAdminAction;
                        pointsDeposit.DepositType = (int)depositType;

                        cmuneDb.PointDeposits.InsertOnSubmit(pointsDeposit);

                        Credit memberWallet = cmuneDb.Credits.SingleOrDefault(f => f.UserId == cmid);

                        memberWallet.NbPoints += points;
                        memberWallet.ExpDatePoint = DateTime.Now.AddYears(1);

                        cmuneDb.SubmitChanges();
                    }
                }
                catch (ChangeConflictException ex)
                {
                    ex.Data.Add("Message", "[Cmid: " + cmid + "][Nb points to attribute: " + points + "][Is admin action: " + isAdminAction + "@@@" + ex.SerializeForLog(cmuneDb) + "@@@" + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Remove Points
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="points"></param>
        /// <param name="isAdminAction"></param>
        public static void RemovePoints(int cmid, int points, bool isAdminAction)
        {
            using (CmuneDataContext cmuneDbCenter = new CmuneDataContext())
            {
                Withdrawal attPoints = new Withdrawal();
                attPoints.WithdrawalDate = DateTime.Now;
                attPoints.Points = points * (-1);
                attPoints.Cmid = cmid;
                attPoints.IsAdminAction = isAdminAction;
                cmuneDbCenter.Withdrawals.InsertOnSubmit(attPoints);

                Credit userCredit = cmuneDbCenter.Credits.SingleOrDefault<Credit>(f => f.UserId == cmid);
                if ((int)userCredit.NbPoints < (points * (-1))) userCredit.NbPoints = 0;
                else userCredit.NbPoints -= points * (-1);
                userCredit.ExpDatePoint = DateTime.Now.AddYears(1);
                cmuneDbCenter.SubmitChanges();
            }
        }

        static decimal GetNaturalLogarithmicA(int dailyPrice)
        {
            int highestPrice = 0;
            decimal alpha = 0;

            highestPrice = CmuneEconomy.ComputeHighestPrice(dailyPrice);
            if (highestPrice > 0)
            {
                alpha = MathematicNaturalLogarithm.GetNaturalLogarithmicAlpha(highestPrice, GetHighestDurationDay());
            }
            return alpha;
        }

        static List<ItemInventoryView> GetAllInventoryWithItem(CmuneDataContext cmuneDb, int itemId)
        {
            var query = from globalInventory in cmuneDb.ItemInventories
                        where globalInventory.ItemId == itemId
                        select new ItemInventoryView(globalInventory.ItemId, globalInventory.ExpirationDate, globalInventory.AmountRemaining, globalInventory.Cmid);
            return query.ToList();
        }

        /// <summary>
        /// Get the number of users using the item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static int GetNumberOfUsersUsingItem(int itemId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = from globalInventory in cmuneDb.ItemInventories
                            where globalInventory.ItemId == itemId
                            group globalInventory by globalInventory.Cmid into colgroup
                            select new { numberOfUsersUsingItem = colgroup.Count() };
                return query.ToList().Count();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amountOfRemaning"></param>
        /// <param name="expirationDate"></param>
        /// <param name="dailyPrice"></param>
        /// <param name="permanentPrice"></param>
        /// <returns></returns>
        static int CalculateNumberOfCreditsToRefund(int amountOfRemaning, DateTime? expirationDate, int dailyPrice, int permanentPrice)
        {
            decimal refundRate = 1.25M;

            if (expirationDate == null)
            {
                return (int)(permanentPrice * refundRate);
            }
            else if (expirationDate != null)
            {
                if (expirationDate.Value.Subtract(DateTime.Now).TotalDays == 1)
                {
                    return (int)(dailyPrice * refundRate);
                }
                else if (expirationDate.Value.Subtract(DateTime.Now).TotalDays > 1)
                {
                    return (int)((decimal)MathematicNaturalLogarithm.CalculateY(GetNaturalLogarithmicA(dailyPrice), (int)expirationDate.Value.Subtract(DateTime.Now).TotalDays) * refundRate);
                }
            }
            return 0;
        }

        /// <summary>
        ///  deprecate Item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dailyPrice"></param>
        /// <param name="permanentPrice"></param>
        public static void DeprecateItem(ItemView item, int dailyPrice, int permanentPrice)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int lastMember = 0;

                foreach (var memberItem in GetAllInventoryWithItem(cmuneDb, item.ItemId))
                {
                    if (lastMember != memberItem.Cmid)
                    {
                        var creditsToRefund = CalculateNumberOfCreditsToRefund(memberItem.AmountRemaining, memberItem.ExpirationDate, dailyPrice, permanentPrice);
                        if (creditsToRefund > 0)
                        {
                            //CmuneEconomy.AttributeCredits(memberItem.Cmid, creditsToRefund);
                            var cash = (decimal)creditsToRefund / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd];
                            int creditDepositId;
                            CmuneEconomy.ProcessCreditAttribution(memberItem.Cmid, cash, CurrencyType.Usd, true, PaymentProviderType.Cmune, string.Empty, CommonConfig.ApplicationIdUberstrike, ChannelType.WebPortal, null, out creditDepositId);
                            string messageText = string.Format("The item {0} was retired from the game. We thank you for your original purchase and credit you with {1} UberStrike Credits.", item.Name, creditsToRefund);
                            string messageSubject = string.Format("You received {0} UberStrike Credits", creditsToRefund);
                            CmunePrivateMessages.SendAdminMessage(memberItem.Cmid, messageSubject, messageText);
                        }
                    }
                    lastMember = memberItem.Cmid;
                }

                CmuneItem.DeprecateItem(item.ItemId);
            }
        }

        /// <summary>
        /// Attributes credits when winning a Mystery Box / Lucky Draw
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="credits"></param>
        /// <param name="channel"></param>
        public static void AttributeCreditsWhenWinningLuckyDrawMysteryBox(int cmid, int credits, ChannelType channel)
        {
            int creditDepositId = 0;
            CmuneEconomy.ProcessCreditAttribution(cmid, (decimal)credits / (decimal)CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd], CurrencyType.Usd, true, PaymentProviderType.Cmune, String.Empty, CommonConfig.ApplicationIdUberstrike, channel, null, out creditDepositId, true, credits);
        }

        /// <summary>
        /// Admin attribution of Cmune credits
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="credits"></param>
        public static void AttributeCredits(int cmid, int credits)
        {
            if (credits > 0)
            {
                using (CmuneDataContext cmuneDB = new CmuneDataContext())
                {
                    CreditDeposit creditDeposit = new CreditDeposit();
                    creditDeposit.DepositDate = DateTime.Now;
                    creditDeposit.NbCredit = credits;
                    creditDeposit.UserId = cmid;
                    creditDeposit.isAdminAction = true;
                    creditDeposit.ConversionRate = 1;
                    creditDeposit.PartnerID = (int)PaymentProviderType.Cmune;
                    creditDeposit.TransactionKey = String.Empty;
                    creditDeposit.ChannelID = (int)ChannelType.WebPortal;
                    creditDeposit.ApplicationID = CommonConfig.ApplicationIdUberstrike;

                    cmuneDB.CreditDeposits.InsertOnSubmit(creditDeposit);

                    Credit memberCredit = GetCredit(cmid, cmuneDB);

                    memberCredit.NbCredits += credits;
                    memberCredit.ExpDateCredit = DateTime.Now.AddYears(1);

                    cmuneDB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Removes credits to a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="credits">This number should be positive</param>
        /// <param name="isAdminAction"></param>
        public static void RemoveCreditsToMember(int cmid, int credits, bool isAdminAction)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int realNbCredits = credits;

                Credit memberCredit = cmuneDB.Credits.SingleOrDefault(f => f.UserId == cmid);

                if ((int)memberCredit.NbCredits < credits)
                {
                    realNbCredits = (int)memberCredit.NbCredits;
                    memberCredit.NbCredits = 0;
                }
                else
                {
                    memberCredit.NbCredits -= credits;
                }

                memberCredit.ExpDateCredit = DateTime.Now.AddYears(1);
                cmuneDB.SubmitChanges();

                // TODO: Should the withdraw be equals to nbCredits or the real number of credits that we remove from the member credits?
                Withdrawal withdraw = new Withdrawal();
                withdraw.WithdrawalDate = DateTime.Now;
                withdraw.Credits = realNbCredits;
                withdraw.Cmid = cmid;
                withdraw.IsAdminAction = isAdminAction;
                cmuneDB.Withdrawals.InsertOnSubmit(withdraw);
            }
        }

        /// <summary>
        /// Admin attribution of points / credits to either ALL our members or all our member on a Esns
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="isCredits"></param>
        /// <param name="esns"></param>
        public static void AttributeCurrencyToMembers(int currency, bool isCredits, EsnsType esns)
        {
            DateTime depositsDate = DateTime.Now;

            StringBuilder sqlUpdateCurrency = new StringBuilder();
            sqlUpdateCurrency.Append("UPDATE [Cmune].[dbo].[Credits] SET ");

            if (isCredits)
            {
                sqlUpdateCurrency.Append("[NbCredits]");
            }
            else
            {
                sqlUpdateCurrency.Append("[NbPoints]");
            }

            sqlUpdateCurrency.Append(" += ");
            sqlUpdateCurrency.Append(currency.ToString());

            if (esns != EsnsType.None)
            {
                sqlUpdateCurrency.Append(" WHERE [UserId] IN (SELECT [CMID] FROM [ESNSIdentities] WHERE [Type] = ");
                sqlUpdateCurrency.Append((int)esns);
                sqlUpdateCurrency.Append(")");
            }

            StringBuilder sqlRecordDeposits = new StringBuilder();

            string depositDateFormatted = depositsDate.ToString("yyyy-MM-dd HH:mm:ss");

            if (isCredits)
            {
                sqlRecordDeposits.Append("INSERT INTO [Cmune].[dbo].[CreditDeposit] ([DepositDate], [NbCredit], [NbCash], [UserId], [ConversionRate], [isAdminAction], [CurrencyLabel], [PartnerID], [TransactionKey], [ApplicationID], [ChannelID], [UsdAmount]) SELECT '");
                sqlRecordDeposits.Append(depositDateFormatted);
                sqlRecordDeposits.Append("', ");
                sqlRecordDeposits.Append(currency);
                sqlRecordDeposits.Append(", 0, [CMID], 650, 1, 'USD', 1, '', 1, 0, 0 FROM ");
            }
            else
            {
                sqlRecordDeposits.Append("INSERT INTO [Cmune].[dbo].[PointDeposit] ([DepositDate], [NbPoints], [UserId], [isAdminAction], [DepositType]) SELECT '");
                sqlRecordDeposits.Append(depositDateFormatted);
                sqlRecordDeposits.Append("', ");
                sqlRecordDeposits.Append(currency);
                sqlRecordDeposits.Append(", [CMID], 1, 0 FROM ");
            }

            if (esns != EsnsType.None)
            {
                sqlRecordDeposits.Append("[Cmune].[dbo].[ESNSIdentities] WHERE [Type] = ");
                sqlRecordDeposits.Append((int)esns);
            }
            else
            {
                sqlRecordDeposits.Append("[Cmune].[dbo].[Members]");
            }

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                // TODO We should perform only one transaction
                cmuneDb.ExecuteCommand(sqlUpdateCurrency.ToString());
                cmuneDb.ExecuteCommand(sqlRecordDeposits.ToString());
            }
        }

        /// <summary>
        /// Admin attribution of points / credits to a list of members
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="isCredits"></param>
        /// <param name="cmids"></param>
        public static void AttributeCurrencyToMembers(int currency, bool isCredits, List<int> cmids)
        {
            DateTime depositsDate = DateTime.Now;

            StringBuilder sqlUpdateCurrecny = new StringBuilder();
            sqlUpdateCurrecny.Append("UPDATE [Cmune].[dbo].[Credits] SET ");

            if (isCredits)
            {
                sqlUpdateCurrecny.Append("[NbCredits]");
            }
            else
            {
                sqlUpdateCurrecny.Append("[NbPoints]");
            }

            sqlUpdateCurrecny.Append(" += ");
            sqlUpdateCurrecny.Append(currency.ToString());
            sqlUpdateCurrecny.Append(" WHERE [UserId] IN (");
            sqlUpdateCurrecny.Append(String.Join(", ", cmids));
            sqlUpdateCurrecny.Append(")");

            StringBuilder sqlRecordDeposits = new StringBuilder();

            string depositDateFormatted = depositsDate.ToString("yyyy-MM-dd HH:mm:ss");

            if (isCredits)
            {
                sqlRecordDeposits.Append("INSERT INTO [Cmune].[dbo].[CreditDeposit] ([DepositDate], [NbCredit], [NbCash], [UserId], [ConversionRate], [isAdminAction], [CurrencyLabel], [PartnerID], [TransactionKey], [ApplicationID], [ChannelID], [UsdAmount]) SELECT '");
                sqlRecordDeposits.Append(depositDateFormatted);
                sqlRecordDeposits.Append("', ");
                sqlRecordDeposits.Append(currency);
                sqlRecordDeposits.Append(", 0, [CMID], 650, 1, 'USD', 1, '', 1, 0, 0 FROM ");
            }
            else
            {
                sqlRecordDeposits.Append("INSERT INTO [Cmune].[dbo].[PointDeposit] ([DepositDate], [NbPoints], [UserId], [isAdminAction], [DepositType]) SELECT '");
                sqlRecordDeposits.Append(depositDateFormatted);
                sqlRecordDeposits.Append("', ");
                sqlRecordDeposits.Append(currency);
                sqlRecordDeposits.Append(", [CMID], 1, 0 FROM ");
            }

            sqlRecordDeposits.Append("[Cmune].[dbo].[Members] WHERE [CMID] IN (");
            sqlRecordDeposits.Append(String.Join(", ", cmids));
            sqlRecordDeposits.Append(")");

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                // TODO We should perform only one transaction
                cmuneDb.ExecuteCommand(sqlUpdateCurrecny.ToString());
                cmuneDb.ExecuteCommand(sqlRecordDeposits.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="bundleId"></param>
        /// <param name="currencyLabel"></param>
        /// <param name="isAdminAction"></param>
        /// <param name="partnerId"></param>
        /// <param name="transactionKey"></param>
        /// <param name="applicationId"></param>
        /// <param name="channel"></param>
        /// <param name="creditDepositId"></param>
        /// <returns></returns>
        public static bool ProcessBundleAttribution(int cmid, int bundleId, string currencyLabel, bool isAdminAction, PaymentProviderType partnerId, string transactionKey, int applicationId, ChannelType channel, out int creditDepositId)
        {
            creditDepositId = 0;

            if (currencyLabel != CurrencyType.Usd)
            {
                throw new NotImplementedException("The package payment only support USD for the moment");
            }

            BundleView selectedBundle = CmuneBundle.GetBundleOnSaleView(bundleId, channel);

            // if the bundle exists
            if (selectedBundle != null)
            {
                if (!ProcessCreditAttribution(cmid, selectedBundle.USDPrice, CurrencyType.Usd, isAdminAction, partnerId, transactionKey, applicationId, channel, selectedBundle.Id, out creditDepositId, true, selectedBundle.Credits))
                    return false;

                List<BundleItemView> bundleItemViews = selectedBundle.BundleItemViews;

                if (bundleItemViews.Count > 0)
                {
                    List<ItemView> itemViews = CmuneItem.GetItems(bundleItemViews.Select(b => b.ItemId).ToList());

                    CmuneEconomy.AddItemsToInventory(cmid, itemViews.Where(i => bundleItemViews.Where(b => b.Duration == BuyingDurationType.OneDay).Select(b => b.ItemId).Contains(i.ItemId)).ToList(), BuyingDurationType.OneDay, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, itemViews.Where(i => bundleItemViews.Where(b => b.Duration == BuyingDurationType.SevenDays).Select(b => b.ItemId).Contains(i.ItemId)).ToList(), BuyingDurationType.SevenDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, itemViews.Where(i => bundleItemViews.Where(b => b.Duration == BuyingDurationType.ThirtyDays).Select(b => b.ItemId).Contains(i.ItemId)).ToList(), BuyingDurationType.ThirtyDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventory(cmid, itemViews.Where(i => bundleItemViews.Where(b => b.Duration == BuyingDurationType.NinetyDays).Select(b => b.ItemId).Contains(i.ItemId)).ToList(), BuyingDurationType.NinetyDays, DateTime.Now, false);
                    CmuneEconomy.AddItemsToInventoryPermanently(cmid, bundleItemViews.Where(d => d.Duration == BuyingDurationType.Permanent).Select(d => d.ItemId).ToList(), DateTime.Now);
                    foreach (var item in bundleItemViews.Where(d => d.Duration == BuyingDurationType.None))
                    {
                        CmuneEconomy.AddConsumableItemToInventory(cmid, item.ItemId, item.Amount, DateTime.Now);
                    }
                }
                if (selectedBundle.Points > 0)
                {
                    AttributePoints(cmid, selectedBundle.Points, isAdminAction, PointsDepositType.PointPurchase);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cash"></param>
        /// <param name="currencyLabel"></param>
        /// <param name="isAdminAction"></param>
        /// <param name="partnerId"></param>
        /// <param name="transactionKey"></param>
        /// <param name="applicationId"></param>
        /// <param name="channel"></param>
        /// <param name="bundleId"></param>
        /// <param name="creditDepositId"></param>
        /// <param name="attributeTotalCredit"></param>
        /// <param name="totalCredit"></param>
        /// <returns></returns>
        public static bool ProcessCreditAttribution(int cmid, decimal cash, string currencyLabel, bool isAdminAction, PaymentProviderType partnerId, string transactionKey, int applicationId, ChannelType channel, int? bundleId, out int creditDepositId, bool attributeTotalCredit = false, int totalCredit = 0)
        {
            //decimal cash, decimal conversionRate, 
            creditDepositId = 0;
            if (!CommonConfig.AcceptedCurrencies.ContainsKey(currencyLabel) && !CommonConfig.CurrenciesToCreditsConversionRate.ContainsKey(currencyLabel))
            {
                throw new NotImplementedException(String.Format("The payment only support {0} for the moment", String.Join(", ", CommonConfig.AcceptedCurrencies)));
            }

            int credits = 0;
            if (attributeTotalCredit == true)
                credits = totalCredit;
            else
                credits = (int)(CommonConfig.CurrenciesToCreditsConversionRate[currencyLabel] * cash);

            if (cash > 0)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    DateTime now = DateTime.Now;

                    decimal usdAmount = cash;

                    if (cash != 0 && currencyLabel != CurrencyType.Usd)
                    {
                        usdAmount = ConvertToUsd(currencyLabel, cash, cmuneDb);
                    }

                    CreditDeposit creditDeposit = new CreditDeposit();
                    creditDeposit.DepositDate = now;
                    creditDeposit.NbCredit = credits;
                    creditDeposit.NbCash = cash;
                    creditDeposit.UserId = cmid;
                    creditDeposit.isAdminAction = isAdminAction;
                    creditDeposit.ConversionRate = CommonConfig.CurrenciesToCreditsConversionRate[currencyLabel];
                    creditDeposit.CurrencyLabel = currencyLabel;
                    creditDeposit.PartnerID = (int)partnerId;
                    creditDeposit.TransactionKey = transactionKey;
                    creditDeposit.ApplicationID = applicationId;
                    creditDeposit.ChannelID = (int)channel;
                    creditDeposit.UsdAmount = usdAmount;
                    creditDeposit.BundleId = bundleId;

                    Credit memberCredit = GetCredit(cmid, cmuneDb);

                    if (memberCredit != null)
                    {
                        memberCredit.NbCredits += credits;
                        memberCredit.ExpDateCredit = now.AddYears(1);
                        cmuneDb.CreditDeposits.InsertOnSubmit(creditDeposit);
                        cmuneDb.SubmitChanges();
                    }

                    creditDepositId = creditDeposit.id;

                    if (partnerId == PaymentProviderType.PlaySpan)
                    {
                        PlaySpanRecordTransaction(transactionKey, creditDepositId, now, PlaySpanTransactionType.Payment);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether a payment is already validated
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="transactionKey"></param>
        /// <returns></returns>
        public static bool IsTransactionExecuted(PaymentProviderType partnerId, string transactionKey)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<CreditDeposit> deposits = cmuneDB.CreditDeposits.Where(f => f.PartnerID == (int)partnerId && f.TransactionKey == transactionKey).ToList();

                if (deposits.Count > 0)
                {
                    if (deposits.Count > 1)
                    {
                        CmuneLog.LogUnexpectedReturn(deposits.Count, String.Format("The transaction {0} was recorderded {1} times for the payment provider {2} ({3})", transactionKey, deposits.Count, partnerId, (int)partnerId));
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Allows to rent all kind of items EXCEPT packs
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="itemId"></param>
        /// <param name="buyerCmid"></param>
        /// <param name="currencyType"></param>
        /// <param name="duration"></param>
        /// <param name="marketLocation"></param>
        /// <param name="recommendationType"></param>
        /// <param name="isAdminAction"></param>
        /// <returns></returns>
        public static int BuyItem(int applicationId, int itemId, int buyerCmid, UberStrikeCurrencyType currencyType, BuyingDurationType duration, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType, bool isAdminAction = false)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int ret = BuyItemResult.Ok;
                int price = 0;

                ItemView itemView = CmuneItem.GetItemView(itemId);

                if (itemView != null)
                {
                    BuyingType buyingType = duration == BuyingDurationType.Permanent ? BuyingType.Permanent : BuyingType.Rent;

                    CheckBuyingArguments(itemView, currencyType, buyingType, ref ret);

                    if (ret == BuyItemResult.Ok)
                    {
                        switch (duration)
                        {
                            case BuyingDurationType.OneDay:
                                if (itemView.Enable1Day == false) ret = BuyItemResult.DurationDisabled;
                                break;
                            case BuyingDurationType.SevenDays:
                                if (itemView.Enable7Days == false) ret = BuyItemResult.DurationDisabled;
                                break;
                            case BuyingDurationType.ThirtyDays:
                                if (itemView.Enable30Days == false) ret = BuyItemResult.DurationDisabled;
                                break;
                            case BuyingDurationType.NinetyDays:
                                if (itemView.Enable90Days == false) ret = BuyItemResult.DurationDisabled;
                                break;
                            case BuyingDurationType.Permanent:
                                if (itemView.EnablePermanent == false) ret = BuyItemResult.DurationDisabled;
                                break;
                        }
                    }

                    if (ret == BuyItemResult.Ok)
                    {
                        price = ComputeItemRentPrice(itemView, currencyType, duration);

                        DateTime buyingTime = DateTime.Now;

                        Withdrawal withdrawal = null;

                        if (!isAdminAction)
                        {
                            withdrawal = Withdraw(buyerCmid, price, currencyType, buyingTime, cmuneDb, out ret);
                        }

                        ItemInventory itemInventory = null;

                        if (ret == BuyItemResult.Ok)
                        {
                            if (duration != BuyingDurationType.Permanent)
                            {
                                itemInventory = AddItemToInventory(buyerCmid, itemView, duration, buyingTime, cmuneDb, out ret);
                            }
                            else
                            {
                                itemInventory = AddItemToInventoryPermanently(buyerCmid, itemView.ItemId, buyingTime, cmuneDb, out ret, itemView);
                            }
                        }

                        if (!isAdminAction && ret == BuyItemResult.Ok)
                        {
                            DoItemTransaction(buyingTime, itemInventory.ExpirationDate, buyerCmid, itemId, withdrawal, duration, marketLocation, recommendationType, cmuneDb, out ret);
                        }
                    }
                }
                else
                {
                    ret = BuyItemResult.InvalidData;
                }

                return ret;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="itemId"></param>
        /// <param name="buyerCmid"></param>
        /// <param name="packType"></param>
        /// <param name="currencyType"></param>
        /// <param name="marketLocation"></param>
        /// <param name="recommendationType"></param>
        /// <param name="isAdminAction"></param>
        /// <returns></returns>
        public static int BuyPack(int applicationId, int itemId, int buyerCmid, PackType packType, UberStrikeCurrencyType currencyType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType, bool isAdminAction = false)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int ret = BuyItemResult.Ok;
                int price = 0;

                ItemView itemView = CmuneItem.GetItemView(itemId);

                if (itemView != null)
                {
                    CheckBuyingArguments(itemView, currencyType, BuyingType.Permanent, ref ret);

                    if (ret == BuyItemResult.Ok)
                    {
                        switch (packType)
                        {
                            case PackType.One:
                                if (itemView.PackOneAmount == CommonConfig.ItemMallFieldDisable) ret = BuyItemResult.PackDisabled;
                                break;
                            case PackType.Two:
                                if (itemView.PackTwoAmount == CommonConfig.ItemMallFieldDisable) ret = BuyItemResult.PackDisabled;
                                break;
                            case PackType.Three:
                                if (itemView.PackThreeAmount == CommonConfig.ItemMallFieldDisable) ret = BuyItemResult.PackDisabled;
                                break;
                        }
                    }

                    if (ret == BuyItemResult.Ok)
                    {
                        price = ComputeItemConsumablePrice(itemView, packType, currencyType);

                        DateTime buyingTime = DateTime.Now;

                        Withdrawal withdrawal = null;

                        if (!isAdminAction)
                        {
                            withdrawal = Withdraw(buyerCmid, price, currencyType, buyingTime, cmuneDb, out ret);
                        }

                        ItemInventory itemInventory = null;

                        if (ret == BuyItemResult.Ok)
                        {
                            var maxAmount = itemView.MaximumOwnableAmount != CommonConfig.ItemMallFieldDisable ? itemView.MaximumOwnableAmount : CommonConfig.ItemMaximumOwnableAmount;
                            itemInventory = AddPackToInventory(buyerCmid, itemView.ItemId, maxAmount, buyingTime, GetPackAmount(itemView, packType), cmuneDb, out ret);
                        }

                        if (itemInventory != null && !isAdminAction && ret == BuyItemResult.Ok)
                        {
                            DoItemTransaction(buyingTime, itemInventory.ExpirationDate, buyerCmid, itemId, withdrawal, CommonConfig.PackDuration, marketLocation, recommendationType, cmuneDb, out ret);
                        }
                    }
                }
                else
                {
                    ret = BuyItemResult.InvalidData;
                }

                return ret;
            }
        }

        /// <summary>
        /// Checks the arguments of the buying functions
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="currencyType"></param>
        /// <param name="buyingType"></param>
        /// <param name="result"></param>
        private static void CheckBuyingArguments(ItemView itemView, UberStrikeCurrencyType currencyType, BuyingType buyingType, ref int result)
        {
            if (itemView != null)
            {
                if (itemView.IsForSale == false)
                {
                    result = BuyItemResult.IsNotForSale;
                }
                else if (buyingType.Equals(BuyingType.Rent))
                {
                    if (currencyType == UberStrikeCurrencyType.Points &&
                        itemView.PointsPerDay == CommonConfig.ItemMallFieldDisable)
                    {
                        result = BuyItemResult.DisableForRent;
                    }

                    if (currencyType == UberStrikeCurrencyType.Credits &&
                        itemView.CreditsPerDay == CommonConfig.ItemMallFieldDisable)
                    {
                        result = BuyItemResult.DisableForRent;
                    }
                }
                else if (buyingType.Equals(BuyingType.Permanent))
                {
                    if (currencyType == UberStrikeCurrencyType.Points &&
                        itemView.PermanentPoints == CommonConfig.ItemMallFieldDisable)
                    {
                        result = BuyItemResult.DisableForPermanent;
                    }

                    if (currencyType == UberStrikeCurrencyType.Credits &&
                        itemView.PermanentCredits == CommonConfig.ItemMallFieldDisable)
                    {
                        result = BuyItemResult.DisableForPermanent;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="packType"></param>
        /// <param name="currencyType"></param>
        public static int ComputeItemConsumablePrice(ItemView itemView, PackType packType, UberStrikeCurrencyType currencyType)
        {
            int price = CommonConfig.ItemMallFieldDisable;
            int discount = 0;
            int amount = itemView.PackOneAmount;

            if (packType.Equals(PackType.Two))
            {
                amount = itemView.PackTwoAmount;
            }
            else if (packType.Equals(PackType.Three))
            {
                discount = CommonConfig.DiscountPackThree;
                amount = itemView.PackThreeAmount;
            }

            if (currencyType == UberStrikeCurrencyType.Points)
            {
                price = ComputePricePack(amount, discount, itemView.PermanentPoints);
            }
            else if (currencyType == UberStrikeCurrencyType.Credits)
            {
                price = ComputePricePack(amount, discount, itemView.PermanentCredits);
            }
            else
            {
                throw new System.NotSupportedException("Currency type not supported: " + currencyType);
            }

            if (price < 0)
            {
                throw new ArgumentOutOfRangeException("Price shouldn't be negative");
            }

            return price;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="currencyType"></param>
        /// <param name="duration"></param>
        /// <param name="price"></param>
        public static int ComputeItemRentPrice(ItemView itemView, UberStrikeCurrencyType currencyType, BuyingDurationType duration)
        {
            int discount = 0;
            int price = CommonConfig.ItemMallFieldDisable;

            if (duration.Equals(BuyingDurationType.SevenDays))
            {
                if (currencyType == UberStrikeCurrencyType.Credits)
                {
                    discount = CommonConfig.DiscountCreditsSevenDays;
                }
                else if (currencyType == UberStrikeCurrencyType.Points)
                {
                    discount = CommonConfig.DiscountPointsSevenDays;
                }
            }
            else if (duration.Equals(BuyingDurationType.ThirtyDays))
            {
                if (currencyType == UberStrikeCurrencyType.Credits)
                {
                    discount = CommonConfig.DiscountCreditsThirtyDays;
                }
                else if (currencyType == UberStrikeCurrencyType.Points)
                {
                    discount = CommonConfig.DiscountPointsThirtyDays;
                }
            }
            else if (duration.Equals(BuyingDurationType.NinetyDays))
            {
                if (currencyType == UberStrikeCurrencyType.Credits)
                {
                    discount = CommonConfig.DiscountCreditsNinetyDays;
                }
                else if (currencyType == UberStrikeCurrencyType.Points)
                {
                    discount = CommonConfig.DiscountPointsNinetyDays;
                }
            }

            if (currencyType == UberStrikeCurrencyType.Points)
            {
                if (duration != BuyingDurationType.Permanent)
                {
                    price = ComputePriceRent(GetDurationInDays(duration), discount, itemView.PointsPerDay);
                }
                else
                {
                    price = itemView.PermanentPoints;
                }
            }
            else if (currencyType == UberStrikeCurrencyType.Credits)
            {
                if (duration != BuyingDurationType.Permanent)
                {
                    price = ComputePriceRent(GetDurationInDays(duration), discount, itemView.CreditsPerDay);
                }
                else
                {
                    price = itemView.PermanentCredits;
                }
            }
            else
            {
                throw new System.NotSupportedException("Currency type not supported: " + currencyType);
            }

            if (price < 0)
            {
                throw new ArgumentOutOfRangeException("Price shouldn't be negative");
            }

            return price;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="currencyAmount"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        public static bool Withdraw(int cmid, int currencyAmount, UberStrikeCurrencyType currencyType)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isSuccessful = false;
                DateTime widthdrawalTime = DateTime.Now;
                int result = BuyItemResult.InvalidData;

                Withdrawal withdraw = Withdraw(cmid, currencyAmount, currencyType, widthdrawalTime, cmuneDb, out result);

                if (result == BuyItemResult.Ok && withdraw != null)
                {
                    isSuccessful = true;
                    cmuneDb.SubmitChanges();
                }

                return isSuccessful;
            }
        }

        /// <summary>
        /// Withdraws points or credits when buying an item
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="currencyAmount"></param>
        /// <param name="currencyType"></param>
        /// <param name="widthdrawalTime"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static Withdrawal Withdraw(int cmid, int currencyAmount, UberStrikeCurrencyType currencyType, DateTime widthdrawalTime, CmuneDataContext cmuneDB, out int result)
        {
            // TODO should we check the points > 0 && credits > 0?

            Withdrawal withdrawal = null;
            result = BuyItemResult.Ok;

            int points = 0;
            int credits = 0;

            if (currencyType == UberStrikeCurrencyType.Credits)
            {
                credits = currencyAmount;
            }
            else if (currencyType == UberStrikeCurrencyType.Points)
            {
                points = currencyAmount;
            }

            if (cmuneDB != null)
            {
                Credit memberWallet = CmuneEconomy.GetMemberWallet(cmid, cmuneDB);

                if (memberWallet != null)
                {
                    if (memberWallet.NbCredits < credits || memberWallet.NbPoints < points)
                    {
                        result = BuyItemResult.NotEnoughCurrency;
                    }
                    else
                    {
                        memberWallet.NbCredits -= credits;
                        memberWallet.NbPoints -= points;

                        withdrawal = new Withdrawal();

                        withdrawal.Cmid = cmid;
                        withdrawal.Credits = credits;
                        withdrawal.IsAdminAction = false;
                        withdrawal.Points = points;
                        withdrawal.WithdrawalDate = widthdrawalTime;

                        cmuneDB.Withdrawals.InsertOnSubmit(withdrawal);
                        // We'll submit changes only at the end of the transaction
                    }
                }
                else
                {
                    result = BuyItemResult.InvalidMember;
                }
            }

            return withdrawal;
        }

        /// <summary>
        /// Adds an item to the inventory (not permanently)
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemView"></param>
        /// <param name="duration"></param>
        /// <param name="buyingTime"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static ItemInventory AddItemToInventory(int cmid, ItemView itemView, BuyingDurationType duration, DateTime buyingTime, CmuneDataContext cmuneDB, out int result)
        {
            return AddItemToInventory(cmid, itemView, duration, buyingTime, cmuneDB, true, out result);
        }

        /// <summary>
        /// Adds items to the inventory of a member (not permanently)
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemViews"></param>
        /// <param name="duration"></param>
        /// <param name="buyingTime"></param>
        /// <param name="throwErrorOnTooLongDuration">Allow to add an item up to the max duration (CommonConfig.ItemMaximumDurationInDays OR itemView.MaximumDurationDays) even if the duration is bigger.</param>
        /// <returns></returns>
        public static Dictionary<int, int> AddItemsToInventory(int cmid, List<ItemView> itemViews, BuyingDurationType duration, DateTime buyingTime, bool throwErrorOnTooLongDuration)
        {
            Dictionary<int, int> results = new Dictionary<int, int>();

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                foreach (ItemView itemView in itemViews)
                {
                    int result = BuyItemResult.Ok;

                    AddItemToInventory(cmid, itemView, duration, buyingTime, cmuneDB, throwErrorOnTooLongDuration, out result);

                    if (!results.ContainsKey(itemView.ItemId))
                    {
                        results.Add(itemView.ItemId, result);
                    }
                    else
                    {
                        throw new ArgumentException("Duplicate item Id: " + itemView.ItemId);
                    }
                }

                cmuneDB.SubmitChanges();
            }

            return results;
        }

        /// <summary>
        /// Adds an item to the inventory (not permanently)
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemView"></param>
        /// <param name="duration"></param>
        /// <param name="buyingTime"></param>
        /// <param name="cmuneDb"></param>
        /// <param name="throwErrorOnTooLongDuration">Allow to add an item up to the max duration (CommonConfig.ItemMaximumDurationInDays) even if duration is bigger</param>
        /// <param name="result"></param>
        private static ItemInventory AddItemToInventory(int cmid, ItemView itemView, BuyingDurationType duration, DateTime buyingTime, CmuneDataContext cmuneDb, bool throwErrorOnTooLongDuration, out int result)
        {
            result = BuyItemResult.Ok;
            ItemInventory itemInventory = null;

            if (duration.Equals(BuyingDurationType.None) || duration.Equals(BuyingDurationType.Permanent))
            {
                result = BuyItemResult.InvalidExpirationDate;
            }

            if (cmuneDb != null && itemView != null && result.Equals(BuyItemResult.Ok))
            {
                itemInventory = CmuneMember.GetItemInventory(cmid, itemView.ItemId, cmuneDb);

                // We need to compute the new expiration date (this item might be in the inventory already)

                int durationInDays = GetDurationInDays(duration);
                DateTime expirationTime = buyingTime;

                if (itemInventory != null)
                {
                    if (itemInventory.ExpirationDate == null)
                    {
                        result = BuyItemResult.AlreadyInInventory;
                    }
                    else
                    {
                        expirationTime = (DateTime)itemInventory.ExpirationDate;

                        if (expirationTime < buyingTime)
                        {
                            // If the item already expired, we should take into account the new buying date and not the previous expiration date
                            expirationTime = buyingTime;
                        }
                    }
                }

                DateTime newExpirationTime = expirationTime.AddDays(durationInDays);

                if (result.Equals(BuyItemResult.Ok))
                {
                    // Now we check whether the duration is too long or not

                    if (newExpirationTime.Subtract(buyingTime).TotalDays > CommonConfig.ItemMaximumDurationInDays || itemView.MaximumDurationDays != CommonConfig.ItemMallFieldDisable && newExpirationTime.Subtract(buyingTime).TotalDays > itemView.MaximumDurationDays)
                    {
                        if (throwErrorOnTooLongDuration)
                        {
                            result = BuyItemResult.InvalidExpirationDate;
                        }
                        else
                        {
                            // We need to override the duration in the case of the duration could be more than 90 days

                            if (newExpirationTime.Subtract(buyingTime).TotalDays > CommonConfig.ItemMaximumDurationInDays)
                            {
                                newExpirationTime = buyingTime.AddDays(CommonConfig.ItemMaximumDurationInDays);
                            }

                            //if (itemView.MaximumDurationDays != CommonConfig.ItemMallFieldDisable)
                            //{
                            //    newExpirationTime = buyingTime.AddDays(itemView.MaximumDurationDays);
                            //}
                            //else
                            //{
                            //    newExpirationTime = buyingTime.AddDays(CommonConfig.ItemMaximumDurationInDays);
                            //}

                            result = BuyItemResult.Ok;
                        }
                    }
                    else if (itemInventory != null)
                    {
                        // All good
                        result = BuyItemResult.Ok;
                    }

                    if (itemInventory == null)
                    {
                        itemInventory = new ItemInventory();

                        itemInventory.AmountRemaining = CommonConfig.ItemMallFieldDisable;
                        itemInventory.Cmid = cmid;
                        itemInventory.ExpirationDate = newExpirationTime;
                        itemInventory.ItemId = itemView.ItemId;

                        cmuneDb.ItemInventories.InsertOnSubmit(itemInventory);
                    }
                    else if (result.Equals(BuyItemResult.Ok))
                    {
                        itemInventory.ExpirationDate = newExpirationTime;
                    }
                }
            }

            return itemInventory;
        }

        /// <summary>
        /// Adds an item permanently to the inventory
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <param name="buyingTime"></param>
        /// <returns></returns>
        public static int AddItemToInventoryPermanently(int cmid, int itemId, DateTime buyingTime)
        {
            int result = BuyItemResult.Ok;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                AddItemToInventoryPermanently(cmid, itemId, buyingTime, cmuneDB, out result);

                if (result.Equals(BuyItemResult.Ok))
                {
                    cmuneDB.SubmitChanges();
                }
            }

            return result;
        }

        /// <summary>
        /// Adds an item permanently to the inventory
        /// Needs a SubmitChanges() afterwards
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <param name="buyingTime"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="result"></param>
        /// <param name="itemView">For perfomance reason, if provided avoid an extra SQL query</param>
        private static ItemInventory AddItemToInventoryPermanently(int cmid, int itemId, DateTime buyingTime, CmuneDataContext cmuneDB, out int result, ItemView itemView = null)
        {
            result = BuyItemResult.Ok;
            ItemInventory itemInventory = null;

            if (cmuneDB != null && itemId != 0)
            {
                itemInventory = CmuneMember.GetItemInventory(cmid, itemId, cmuneDB);

                if (itemView == null)
                {
                    itemView = CmuneItem.GetItemView(itemId);
                }

                if (result == BuyItemResult.Ok)
                {
                    if (itemInventory != null && itemInventory.ExpirationDate == null)
                    {
                        result = BuyItemResult.AlreadyInInventory;
                    }
                    else
                    {
                        if (itemInventory == null)
                        {
                            itemInventory = new ItemInventory();
                            cmuneDB.ItemInventories.InsertOnSubmit(itemInventory);
                            itemInventory.AmountRemaining = CommonConfig.ItemMallFieldDisable;
                        }

                        itemInventory.Cmid = cmid;
                        itemInventory.ExpirationDate = null;
                        itemInventory.ItemId = itemId;

                        // We'll submit changes only at the end of the transaction
                    }
                }
            }

            return itemInventory;
        }

        /// <summary>
        /// Adds items permanently to the inventory of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemIds"></param>
        /// <param name="buyingTime"></param>
        /// <returns></returns>
        public static Dictionary<int, int> AddItemsToInventoryPermanently(int cmid, List<int> itemIds, DateTime buyingTime)
        {
            Dictionary<int, int> results = new Dictionary<int, int>();

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                AddItemsToInventoryPermanently(cmid, itemIds, buyingTime, cmuneDB, out results);
                cmuneDB.SubmitChanges();
            }

            return results;
        }

        public static void AddConsumableItemToInventory(int cmid, int itemId, int amount, DateTime buyingTime)
        {
            int result;
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                AddPackToInventory(cmid, itemId, CommonConfig.ItemMallFieldDisable, buyingTime, amount, cmuneDB, out result);
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Adds items permanently to the inventory of a member
        /// Needs a SubmitChanges() afterwards
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemIds"></param>
        /// <param name="buyingTime"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private static List<ItemInventory> AddItemsToInventoryPermanently(int cmid, List<int> itemIds, DateTime buyingTime, CmuneDataContext cmuneDB, out Dictionary<int, int> results)
        {
            results = new Dictionary<int, int>();
            List<ItemInventory> itemsInventory = new List<ItemInventory>();

            if (cmuneDB != null)
            {
                // First we get all the items that are already in the inventory
                List<ItemInventory> currentInventory = CmuneMember.GetItemsInventory(cmid, itemIds, cmuneDB);
                Dictionary<int, ItemInventory> currentInventoryOrdered = currentInventory.ToDictionary(i => i.ItemId, i => i);

                foreach (int itemId in itemIds)
                {
                    int result = BuyItemResult.Ok;

                    AddItemToInventoryPermanently(cmid, itemId, buyingTime, cmuneDB, out result);

                    if (!results.ContainsKey(itemId))
                    {
                        results.Add(itemId, result);
                    }
                    else
                    {
                        throw new ArgumentException("Duplicate item Id: " + itemId);
                    }
                }
            }

            cmuneDB.ItemInventories.InsertAllOnSubmit(itemsInventory);

            return itemsInventory;
        }

        /// <summary>
        /// Bypass all duration and availability check
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemIds"></param>
        public static void AttributeItemsOnAccountCompletion(int cmid, Dictionary<int, int> itemIds)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                DateTime now = DateTime.Now;
                List<ItemInventory> inventory = new List<ItemInventory>();

                foreach (int itemId in itemIds.Keys)
                {
                    ItemInventory item = new ItemInventory();

                    item.AmountRemaining = CommonConfig.ItemMallFieldDisable;
                    item.Cmid = cmid;

                    if (itemIds[itemId] == 0)
                    {
                        item.ExpirationDate = null;
                    }
                    else
                    {
                        item.ExpirationDate = now.AddDays(itemIds[itemId]);
                    }

                    item.ItemId = itemId;

                    inventory.Add(item);
                }

                cmuneDb.ItemInventories.InsertAllOnSubmit(inventory);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Adds a pack to the inventory
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemView"></param>
        /// <param name="buyingTime"></param>
        /// <param name="packType"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="result"></param>
        private static ItemInventory AddPackToInventory(int cmid, int itemId, int maxAmount, DateTime buyingTime, int packAmount, CmuneDataContext cmuneDB, out int result)
        {
            result = BuyItemResult.Ok;
            ItemInventory itemInventory = null;

            if (cmuneDB != null)
            {
                itemInventory = CmuneMember.GetItemInventory(cmid, itemId, cmuneDB);

                if (itemInventory != null)
                {
                    if (maxAmount != CommonConfig.ItemMallFieldDisable && maxAmount < (itemInventory.AmountRemaining + packAmount))
                    {
                        result = BuyItemResult.InvalidAmount;
                    }
                    else
                    {
                        itemInventory.AmountRemaining += packAmount;
                    }
                }
                else
                {
                    if (maxAmount != CommonConfig.ItemMallFieldDisable && maxAmount < packAmount)
                    {
                        result = BuyItemResult.InvalidAmount;
                    }
                    else
                    {
                        itemInventory = new ItemInventory();

                        itemInventory.AmountRemaining = packAmount;
                        itemInventory.Cmid = cmid;
                        itemInventory.ExpirationDate = null;
                        itemInventory.ItemId = itemId;

                        cmuneDB.ItemInventories.InsertOnSubmit(itemInventory);
                    }
                }
            }

            return itemInventory;
        }

        /// <summary>
        /// Gets the amount of a pack
        /// </summary>
        /// <param name="itemView"></param>
        /// <param name="packType"></param>
        /// <returns></returns>
        public static int GetPackAmount(ItemView itemView, PackType packType)
        {
            int packAmount = 0;

            if (packType.Equals(PackType.One) && itemView.PackOneAmount != CommonConfig.ItemMallFieldDisable)
            {
                packAmount = itemView.PackOneAmount;
            }
            else if (packType.Equals(PackType.Two) && itemView.PackTwoAmount != CommonConfig.ItemMallFieldDisable)
            {
                packAmount = itemView.PackTwoAmount;
            }
            else if (packType.Equals(PackType.Three) && itemView.PackThreeAmount != CommonConfig.ItemMallFieldDisable)
            {
                packAmount = itemView.PackThreeAmount;
            }

            return packAmount;
        }

        /// <summary>
        /// Converts the BuyingDurationType in number of days
        /// Do NOT convert permanent duration
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static int GetDurationInDays(BuyingDurationType duration)
        {
            int durationInDays = 0;

            if (duration.Equals(BuyingDurationType.None))
            {
                durationInDays = 0;
            }
            else if (duration.Equals(BuyingDurationType.OneDay))
            {
                durationInDays = 1;
            }
            else if (duration.Equals(BuyingDurationType.SevenDays))
            {
                durationInDays = 7;
            }
            else if (duration.Equals(BuyingDurationType.ThirtyDays))
            {
                durationInDays = 30;
            }
            else if (duration.Equals(BuyingDurationType.NinetyDays))
            {
                durationInDays = 90;
            }
            else
            {
                NotImplementedException exception = new NotImplementedException("Unexpected duration");
                exception.Data.Add("duration", duration.ToString());
                throw exception;
            }

            return durationInDays;
        }

        /// <summary>
        /// Get expiration date
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static DateTime? GetExpirationDate(BuyingDurationType duration)
        {
            return GetExpirationDate(duration, DateTime.Now);
        }

        /// <summary>
        /// Get expiration date
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="currentExpirationDate"></param>
        /// <returns></returns>
        public static DateTime? GetExpirationDate(BuyingDurationType duration, DateTime? currentExpirationDate)
        {
            DateTime? newExpirationDate = currentExpirationDate;

            if (duration == BuyingDurationType.None)
            {
                throw new ArgumentException("Duration can't be set to None");
            }
            else if (duration == BuyingDurationType.Permanent)
            {
                newExpirationDate = null;
            }
            else
            {
                int durationInDays = GetDurationInDays(duration);

                if (currentExpirationDate.HasValue)
                {
                    newExpirationDate = currentExpirationDate.Value.AddDays(durationInDays);
                }
            }

            return newExpirationDate;
        }

        /// <summary>
        /// Computes the price
        /// </summary>
        /// <param name="durationInDays"></param>
        /// <param name="discount"></param>
        /// <param name="dailyPrice"></param>
        /// <returns></returns>
        private static int ComputePriceRent(int durationInDays, int discount, int dailyPrice)
        {
            decimal price = durationInDays * dailyPrice;

            if (discount > 0)
            {
                price = price - (price * discount / (decimal)100);
            }

            return (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Compute Highest Price
        /// </summary>
        /// <param name="dailyPrice"></param>
        /// <returns></returns>
        public static int ComputeHighestPrice(int dailyPrice)
        {
            return ComputePriceRent(GetHighestDurationDay(), GetHighestDiscount(), dailyPrice);
        }

        /// <summary>
        /// Compute Temporary Price
        /// </summary>
        /// <param name="durationType"></param>
        /// <param name="dailyPrice"></param>
        /// <returns></returns>
        public static int ComputeTemporaryPrice(BuyingDurationType durationType, int dailyPrice)
        {
            int discount = 0;

            switch (durationType)
            {
                case BuyingDurationType.OneDay:
                    discount = 0;
                    break;
                case BuyingDurationType.SevenDays:
                    discount = CommonConfig.DiscountCreditsSevenDays;
                    break;
                case BuyingDurationType.ThirtyDays:
                    discount = CommonConfig.DiscountCreditsThirtyDays;
                    break;
                case BuyingDurationType.NinetyDays:
                    discount = CommonConfig.DiscountCreditsNinetyDays;
                    break;
                default:
                    break;
            }
            var durationInDays = GetDurationInDays(durationType);

            return ComputePriceRent(durationInDays, discount, dailyPrice);
        }

        /// <summary>
        /// Get the hight buying duration day
        /// </summary>
        /// <returns></returns>
        static int GetHighestDurationDay()
        {
            return (int)GetDurationInDays(BuyingDurationType.NinetyDays);
        }

        static int GetHighestDiscount()
        {
            return CommonConfig.DiscountCreditsNinetyDays;
        }

        /// <summary>
        /// Computes the prices of a pack
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="discount"></param>
        /// <param name="unitPermanentPrice"></param>
        /// <returns></returns>
        private static int ComputePricePack(int quantity, int discount, int unitPermanentPrice)
        {
            decimal price = quantity * unitPermanentPrice;

            if (discount > 0)
            {
                price = price - (price * discount / (decimal)100);
            }

            return (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyingTime"></param>
        /// <param name="expirationTime"></param>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <param name="withdrawal"></param>
        /// <param name="durationType"></param>
        /// <param name="locationType"></param>
        /// <param name="recommendationType"></param>
        /// <param name="cmuneDB"></param>
        /// <param name="result"></param>
        private static void DoItemTransaction(DateTime buyingTime, DateTime? expirationTime, int cmid, int itemId, Withdrawal withdrawal, BuyingDurationType durationType, BuyingLocationType locationType, BuyingRecommendationType recommendationType, CmuneDataContext cmuneDB, out int result)
        {
            result = BuyItemResult.Ok;
            bool toCommit = false;
            bool decreaseStock = false;

            if (cmuneDB != null && withdrawal != null)
            {
                try
                {
                    Item item = CmuneItem.GetItem(itemId, cmuneDB);

                    if (item.AmountRemaining == CommonConfig.ItemMallFieldDisable)
                    {
                        toCommit = true;
                    }
                    else if (item.AmountRemaining > 0)
                    {
                        toCommit = true;
                        decreaseStock = true;
                    }
                    else
                    {
                        result = BuyItemResult.NoStockRemaining;
                    }

                    if (toCommit)
                    {
                        if (decreaseStock)
                        {
                            item.AmountRemaining -= 1;
                        }

                        cmuneDB.SubmitChanges();

                        ItemTransaction itemTransaction = new ItemTransaction();

                        itemTransaction.Cmid = cmid;
                        itemTransaction.ItemExpirationDate = expirationTime;
                        itemTransaction.ItemId = itemId;
                        itemTransaction.TransactionDate = buyingTime;
                        itemTransaction.WithdrawalId = withdrawal.WithdrawalId;
                        itemTransaction.DurationType = (int)durationType;
                        itemTransaction.MarketType = (int)locationType;
                        itemTransaction.MarketLocation = (int)BuyingMarketType.Shop;
                        itemTransaction.RecommendationType = (int)recommendationType;

                        cmuneDB.ItemTransactions.InsertOnSubmit(itemTransaction);

                        cmuneDB.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder conflictedFields = new StringBuilder();

                    if (ex.GetType().FullName == "System.Data.Linq.ChangeConflictException")
                    {
                        ex = (ChangeConflictException)ex;

                        foreach (ObjectChangeConflict occ in cmuneDB.ChangeConflicts)
                        {
                            foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                            {
                                object currVal = mcc.CurrentValue;
                                object origVal = mcc.OriginalValue;
                                object databaseVal = mcc.DatabaseValue;
                                MemberInfo mi = mcc.Member;

                                conflictedFields.Append("[Field:");
                                conflictedFields.Append(mi.Name);
                                conflictedFields.Append("][Current:");
                                conflictedFields.Append(currVal);
                                conflictedFields.Append("][Original:");
                                conflictedFields.Append(origVal);
                                conflictedFields.Append("][Database:");
                                conflictedFields.Append(databaseVal);
                                conflictedFields.Append("]");
                            }
                        }
                    }

                    CmuneLog.LogUnexpectedReturn(ex, "[Cmid:" + cmid.ToString() + "][Credits:" + withdrawal.Credits.ToString() + "][Points:" + withdrawal.Points.ToString() + "][Withdrawal Id:" + withdrawal.WithdrawalId + "][Time:" + withdrawal.WithdrawalDate.ToString("dd HH:mm:ss.fff") + "][Conflicted][" + conflictedFields.ToString() + "]");
                    throw;
                }
            }
        }

        /// <summary>
        /// Writes the developerid
        /// Useful to serialize our data in the URL of a payment provider
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="channelType"></param>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public static string PaymentWriteDeveloperId(int applicationId, ChannelType channelType, int bundleId)
        {
            string developerId = (applicationId.ToString() + CommonConfig.PlaySpanDelimiter + ((int)channelType).ToString() + CommonConfig.PlaySpanDelimiter + bundleId.ToString()).UrlBase64Encode();

            return developerId;
        }

        /// <summary>
        /// Reads the developerid
        /// </summary>
        /// <param name="developerId"></param>
        /// <param name="applicationId"></param>
        /// <param name="channelType"></param>
        /// <param name="bundleId"></param>
        public static void PaymentReadDeveloperId(string developerId, out int applicationId, out ChannelType channelType, out int bundleId)
        {
            applicationId = 0;
            bundleId = 0;
            channelType = ChannelType.WebPortal;

            developerId = developerId.UrlBase64Decode();
            string[] arguments = developerId.Split(CommonConfig.PlaySpanDelimiter);

            if (arguments.Length == 3)
            {
                int applicationIdTmp;
                Int32.TryParse(arguments[0], out applicationIdTmp);

                if (CommonConfig.ApplicationsName.ContainsKey(applicationIdTmp))
                {
                    applicationId = applicationIdTmp;
                }

                EnumUtilities.TryParseEnumByValue(arguments[1], ChannelType.WebPortal, out channelType);
                Int32.TryParse(arguments[2], out bundleId);
            }
        }

        #region PlaySpan

        /// <summary>
        /// Checks whether a pbctrans is already stored in our database
        /// We need a specific function for PlaySpan as Forced Reversal and Admin reversal will also generate a transaction
        /// </summary>
        /// <param name="pbctrans"></param>
        /// <returns></returns>
        public static bool PlaySpanIsTransactionExecuted(string pbctrans)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isPlaySpanTransactionExecuted = false;

                PlaySpanTransaction playSpanTransaction = cmuneDB.PlaySpanTransactions.SingleOrDefault(p => p.Pbctrans.Equals(pbctrans));

                if (playSpanTransaction != null)
                {
                    isPlaySpanTransactionExecuted = true;
                }

                return isPlaySpanTransactionExecuted;
            }
        }

        /// <summary>
        /// Records a pbctrans. You should check first that this transaction was not stored before
        /// </summary>
        /// <param name="pbctrans"></param>
        /// <param name="creditDepositId"></param>
        /// <param name="transactionTime"></param>
        /// <param name="transactionType"></param>
        public static void PlaySpanRecordTransaction(string pbctrans, int creditDepositId, DateTime transactionTime, PlaySpanTransactionType transactionType)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                PlaySpanTransaction playSpanTransaction = new PlaySpanTransaction();
                playSpanTransaction.CreditDepositId = creditDepositId;
                playSpanTransaction.Pbctrans = pbctrans;
                playSpanTransaction.TransactionTime = transactionTime;
                playSpanTransaction.TransactionType = (int)transactionType;

                cmuneDB.PlaySpanTransactions.InsertOnSubmit(playSpanTransaction);
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Generates the MD5 has required on the PlaySpan popup
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <param name="applicationId"></param>
        /// <param name="channelType"></param>
        /// <param name="bundleId"></param>
        /// <param name="merchtrans">A unique transaction Id used by PlaySpan</param>
        /// <returns></returns>
        public static string PlaySpanGenerateClientHash(int cmid, decimal amount, string currency, int applicationId, ChannelType channelType, int bundleId, string merchtrans)
        {
            string hash = String.Empty;

            StringBuilder hashContent = new StringBuilder();
            hashContent.Append(cmid);
            hashContent.Append(ConfigurationUtilities.ReadConfigurationManager("PlaySpanAdminPassword"));
            hashContent.Append(ConfigurationUtilities.ReadConfigurationManager("PlaySpanSecretHashPassPhrase"));
            hashContent.Append(currency);
            hashContent.Append(amount.ToString("F2"));
            hashContent.Append(merchtrans);
            hashContent.Append(PaymentWriteDeveloperId(applicationId, channelType, bundleId));

            hash = CryptographyUtilities.Md5Hash(hashContent.ToString());

            return hash;
        }

        /// <summary>
        /// Generates PlaySpan callback hash
        /// </summary>
        /// <param name="dtDateTime"></param>
        /// <param name="login"></param>
        /// <param name="adminPassword"></param>
        /// <param name="secretHashPassPhrase"></param>
        /// <param name="userId"></param>
        /// <param name="communicationType"></param>
        /// <param name="settlementAmount"></param>
        /// <param name="amount"></param>
        /// <param name="sepAmount"></param>
        /// <param name="currency"></param>
        /// <param name="sn"></param>
        /// <param name="mirror"></param>
        /// <param name="pbctrans"></param>
        /// <param name="developerId"></param>
        /// <param name="applicationId"></param>
        /// <param name="virtualAmount"></param>
        /// <param name="virtualCurrency"></param>
        /// <returns></returns>
        public static string PlaySpanGenerateCallbackHash(string dtDateTime, string login, string adminPassword, string secretHashPassPhrase, int userId, string communicationType, decimal settlementAmount, decimal amount, decimal sepAmount, string currency, string sn, string mirror, string pbctrans, string developerId, string applicationId, int virtualAmount, string virtualCurrency)
        {
            string hash = String.Empty;

            StringBuilder hashContent = new StringBuilder(dtDateTime);
            hashContent.Append(login);
            hashContent.Append(adminPassword);
            hashContent.Append(secretHashPassPhrase);
            hashContent.Append(userId);
            hashContent.Append(communicationType);
            hashContent.Append(settlementAmount.ToString("F2"));
            hashContent.Append(amount.ToString("F2"));
            hashContent.Append(sepAmount.ToString("F2"));
            hashContent.Append(currency);
            hashContent.Append(sn);
            hashContent.Append(mirror);
            hashContent.Append(pbctrans);
            hashContent.Append(developerId);
            hashContent.Append(applicationId);

            // The following fields should be added to the hash only when non empty and non zero

            if (virtualAmount > 0)
            {
                hashContent.Append(virtualAmount);
            }

            hashContent.Append(virtualCurrency);

            hash = CryptographyUtilities.Md5Hash(hashContent.ToString());

            return hash;
        }

        /// <summary>
        /// Generate the merchtrans that should be unique for each transaction
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static string PlaySpanGenerateMerchtrans(int cmid)
        {
            return String.Format("{0}|{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), cmid);
        }

        #endregion PlaySpan

        #region Super Rewards

        /// <summary>
        /// Generates Super Rewards callback hash
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="newCredits"></param>
        /// <param name="userId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string SuperRewardsGenerateCallbackHash(string transactionId, int newCredits, string userId, string secret)
        {
            string hash = String.Empty;
            string separator = ":";

            StringBuilder hashContent = new StringBuilder();
            hashContent.Append(transactionId);
            hashContent.Append(separator);
            hashContent.Append(newCredits);
            hashContent.Append(separator);
            hashContent.Append(userId);
            hashContent.Append(separator);
            hashContent.Append(secret);

            hash = CryptographyUtilities.Md5Hash(hashContent.ToString());

            return hash;
        }

        /// <summary>
        /// Generates the hash to protect Cmune custom data
        /// We add the user id to avoid to generate the same hash everytime
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="developerId"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static string SuperRewardsGenerateCmuneHash(string userId, string developerId, int cmid)
        {
            string hash = String.Empty;

            StringBuilder hashContent = new StringBuilder();
            hashContent.Append(userId);
            hashContent.Append(developerId);
            hashContent.Append(cmid);
            hashContent.Append(ConfigurationUtilities.ReadConfigurationManager("SuperRewardsCmuneHash"));

            hash = Crypto.fncSHA256Encrypt(hashContent.ToString());

            return hash;
        }

        #endregion Super Rewards

        #region 6waves

        /// <summary>
        /// Generates 6waves callback hash
        /// </summary>
        /// <param name="facebookApplicationId"></param>
        /// <param name="facebookId"></param>
        /// <param name="points"></param>
        /// <param name="txnId"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static string SixWavesGenerateCallbackHash(long facebookApplicationId, long facebookId, string points, string txnId, string secretKey)
        {
            string hash = String.Empty;

            StringBuilder hashContent = new StringBuilder(facebookApplicationId.ToString());
            hashContent.Append(facebookId.ToString());
            hashContent.Append(points);
            hashContent.Append(txnId);
            hashContent.Append(secretKey);

            hash = CryptographyUtilities.Md5Hash(hashContent.ToString());

            return hash;
        }

        #endregion 6waves

        #region Dotori

        /// <summary>
        /// Generates Dotori callback hash
        /// </summary>
        /// <param name="status"></param>
        /// <param name="paymentKey"></param>
        /// <param name="passthrough"></param>
        /// <param name="userId"></param>
        /// <param name="appsNo"></param>
        /// <param name="itemId"></param>
        /// <param name="itemType"></param>
        /// <param name="itemName"></param>
        /// <param name="itemDotori"></param>
        /// <param name="sharedSecret"></param>
        /// <returns></returns>
        public static string DotoriGenerateCallbackHash(string status, string paymentKey, string passthrough, int userId, int appsNo, int itemId, string itemType, string itemName, int itemDotori, string sharedSecret)
        {
            string hash = String.Empty;

            StringBuilder hashContent = new StringBuilder();
            hashContent.Append("payment_key=");
            hashContent.Append(paymentKey);
            hashContent.Append("&user_id=");
            hashContent.Append(userId);
            hashContent.Append("&apps_no=");
            hashContent.Append(appsNo);
            hashContent.Append("&item_id=");
            hashContent.Append(itemId);
            hashContent.Append("&item_type=");
            hashContent.Append(itemType);
            hashContent.Append("&item_name=");
            hashContent.Append(Uri.EscapeDataString(itemName));
            hashContent.Append("&item_dotori=");
            hashContent.Append(itemDotori);
            hashContent.Append("&status=");
            hashContent.Append(status);
            hashContent.Append("&passthrough=");
            hashContent.Append(passthrough);

            hash = CryptographyUtilities.HmacMd5(hashContent.ToString(), sharedSecret);

            return hash;
        }

        /// <summary>
        /// Generate the passthrough for a Dotori transaction and write it to the database
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cyworldId"></param>
        /// <returns></returns>
        public static string DotoriGeneratePassThrough(int applicationId, int cyworldId)
        {
            DateTime now = DateTime.Now;
            string passThrough = DotoriGeneratePassThrough(applicationId, cyworldId, now);

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                CyworldPassThrough cyworldPassThrough = new CyworldPassThrough();
                cyworldPassThrough.ApplicationId = applicationId;
                cyworldPassThrough.CyworldId = cyworldId;
                cyworldPassThrough.GenerationDate = now;
                cyworldPassThrough.PassThrough = passThrough;

                cmuneDb.CyworldPassThroughs.InsertOnSubmit(cyworldPassThrough);
                cmuneDb.SubmitChanges();
            }

            return passThrough;
        }

        /// <summary>
        /// Generate the passthrough for a Dotori transaction
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cyworlId"></param>
        /// <param name="generationTime"></param>
        /// <returns></returns>
        private static string DotoriGeneratePassThrough(int applicationId, int cyworlId, DateTime generationTime)
        {
            StringBuilder passThrough = new StringBuilder();

            passThrough.Append(applicationId);
            passThrough.Append(RandomPassword.Generate(3));
            passThrough.Append(cyworlId);
            passThrough.Append(RandomPassword.Generate(5));
            passThrough.Append(generationTime.ToString("yyyyMMddHHmmssfff"));

            string hashedPassTrough = Crypto.fncSHA256Encrypt(passThrough.ToString());

            return hashedPassTrough;
        }

        #endregion Dotori

        #region Mac App Store

        /// <summary>
        /// Buy a MAS bundle
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="bundleId"></param>
        /// <param name="hashedReceipt"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool BuyMasBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            bool isSuccessful = false;

            if (!IsTransactionExecuted(PaymentProviderType.Apple, hashedReceipt))
            {
                int transactionId = 0;
                isSuccessful = ProcessBundleAttribution(cmid, bundleId, CurrencyType.Usd, false, PaymentProviderType.Apple, hashedReceipt, applicationId, ChannelType.MacAppStore, out transactionId);
            }

            return isSuccessful;
        }

        #endregion

        #region iOS App Store

        /// <summary>
        /// Buy an iPad Bundle
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="bundleId"></param>
        /// <param name="hashedReceipt"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool BuyiPadBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            bool isSuccessful = false;

            if (!IsTransactionExecuted(PaymentProviderType.iOS, hashedReceipt))
            {
                int transactionId = 0;
                isSuccessful = ProcessBundleAttribution(cmid, bundleId, CurrencyType.Usd, false, PaymentProviderType.iOS, hashedReceipt, applicationId, ChannelType.IPad, out transactionId);
            }

            return isSuccessful;
        }

        /// <summary>
        /// Buy an iPhone Bundle
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="bundleId"></param>
        /// <param name="hashedReceipt"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool BuyiPhoneBundle(int cmid, int bundleId, string hashedReceipt, int applicationId)
        {
            bool isSuccessful = false;

            if (!IsTransactionExecuted(PaymentProviderType.iOS, hashedReceipt))
            {
                int transactionId = 0;
                isSuccessful = ProcessBundleAttribution(cmid, bundleId, CurrencyType.Usd, false, PaymentProviderType.iOS, hashedReceipt, applicationId, ChannelType.IPhone, out transactionId);
            }

            return isSuccessful;
        }

        #endregion

        #region Exchange rates

        /// <summary>
        /// Updates a specific exchange rate
        /// </summary>
        /// <param name="currency"></param>
        private static void UpdateExchangeRate(string currency)
        {
            // We need to retrieve the current exchange rate

            // http://download.finance.yahoo.com/d/quotes.json?s=EURUSD=X&f=l1&e=.cs (man you suck balls, ony 4 real decimals)
            // http://www.google.com/ig/calculator?hl=en&q=1{0}=?USD

            string response = String.Empty;
            string urlTemplate = " http://www.google.com/ig/calculator?hl=en&q=1{0}=?USD";
            decimal newExchangeRate = 0;
            bool isExchangeRateParsed = false;

            string url = String.Format(urlTemplate, currency);

            try
            {
                using (WebClient client = new WebClient())
                {
                    response = client.DownloadString(url);
                    response = response.Trim();

                    // {lhs: "1 South Korean won",rhs: "0.000948 U.S. dollars",error: "",icc: true}

                    int currencyExchangeStartIndex = response.IndexOf("rhs: \"");

                    if (currencyExchangeStartIndex != -1)
                    {
                        currencyExchangeStartIndex += "rhs: \"".Length;
                        int currencyExchangeEndIndex = response.IndexOf(" ", currencyExchangeStartIndex);

                        if (currencyExchangeEndIndex != -1)
                        {
                            string exchangeRate = response.Substring(currencyExchangeStartIndex, currencyExchangeEndIndex - currencyExchangeStartIndex);
                            isExchangeRateParsed = Decimal.TryParse(exchangeRate, out newExchangeRate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, url);
            }

            if (isExchangeRateParsed && newExchangeRate > 0)
            {
                // We need to update our current value

                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    ExchangeRate exchangeRate = GetExchangeRate(currency, cmuneDb);

                    if (exchangeRate != null)
                    {
                        // We should check for big fluctuations to avoid bad surprises

                        decimal fluctuation = newExchangeRate - exchangeRate.ConversionRate;
                        decimal fluctuationPercent = fluctuation / exchangeRate.ConversionRate * 100;

                        if (fluctuationPercent > 5 || fluctuationPercent < -5)
                        {
                            CmuneLog.LogUnexpectedReturn(fluctuationPercent, String.Format("currency={0}&oldExchangeRate={1}&newExchangeRate={2}&fluctuationPercent={3}", currency, exchangeRate.ConversionRate, newExchangeRate, fluctuationPercent));
                        }
                    }
                    else
                    {
                        exchangeRate = new ExchangeRate();
                        exchangeRate.Currency = currency;
                        cmuneDb.ExchangeRates.InsertOnSubmit(exchangeRate);
                    }

                    exchangeRate.ConversionRate = newExchangeRate;
                    exchangeRate.LastUpdatedDate = DateTime.Now;

                    cmuneDb.SubmitChanges();
                }
            }
            else
            {
                CmuneLog.LogUnexpectedReturn(response, String.Format("Impossible to parse the new exchange rate:currency={0}&url={1}&response={2}", currency, url, response));
            }
        }

        /// <summary>
        /// Updates all the supported exchange rates
        /// </summary>
        public static void UpdateExchangeRates()
        {
            List<string> exchangeRatesToGet = CommonConfig.AcceptedCurrencies.Keys.ToList();
            exchangeRatesToGet.Remove(CurrencyType.Usd);

            foreach (string exchangeRateToGet in exchangeRatesToGet)
            {
                UpdateExchangeRate(exchangeRateToGet);
            }
        }

        private static decimal ConvertToUsd(string currency, decimal amount, CmuneDataContext cmuneDb)
        {
            decimal usdAmount = 0;

            if (cmuneDb != null)
            {
                List<string> supportedCurrencies = CommonConfig.AcceptedCurrencies.Keys.ToList();
                supportedCurrencies.Remove(CurrencyType.Usd);

                if (supportedCurrencies.Contains(currency))
                {
                    ExchangeRate exchangeRate = GetExchangeRate(currency, cmuneDb);

                    if (exchangeRate != null)
                    {
                        usdAmount = amount * exchangeRate.ConversionRate;
                    }
                }
                else
                {
                    throw new ArgumentException(String.Format("The conversion only support {0} for the moment", String.Join(", ", supportedCurrencies)), "currency");
                }
            }

            return usdAmount;
        }

        private static ExchangeRate GetExchangeRate(string currency, CmuneDataContext cmuneDb)
        {
            ExchangeRate exchangeRate = null;

            if (cmuneDb != null)
            {
                exchangeRate = cmuneDb.ExchangeRates.SingleOrDefault(e => e.Currency == currency);
            }

            return exchangeRate;
        }

        #endregion Exchange rates

        /// <summary>
        /// Remove an item from a member inventory
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool RemoveItem(int cmid, int itemId)
        {
            bool isItemRemoved = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ItemInventory itemInventory = CmuneMember.GetItemInventory(cmid, itemId, cmuneDb);

                if (itemInventory != null)
                {
                    cmuneDb.ItemInventories.DeleteOnSubmit(itemInventory);
                    cmuneDb.SubmitChanges();

                    isItemRemoved = true;
                }
            }

            return isItemRemoved;
        }

        /// <summary>
        /// Use a consumable item
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool UseConsumableItem(int cmid, int itemId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isItemConsumed = false;
                ItemInventory inventory = cmuneDb.ItemInventories.SingleOrDefault(i => i.Cmid == cmid && i.ItemId == itemId);

                if (inventory != null)
                {
                    if (inventory.AmountRemaining > 0)
                    {
                        if (inventory.AmountRemaining == 1)
                        {
                            cmuneDb.ItemInventories.DeleteOnSubmit(inventory);
                        }
                        else
                        {
                            inventory.AmountRemaining -= 1;
                        }

                        cmuneDb.SubmitChanges();
                        isItemConsumed = true;
                    }
                }

                return isItemConsumed;
            }
        }

        /// <summary>
        /// Checks if a member can use a consumable item
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool CanUseConsumableItem(int cmid, int itemId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool canUseItem = cmuneDb.ItemInventories.Where(i => i.Cmid == cmid && i.ItemId == itemId && i.AmountRemaining > 0).Select(i => itemId).Count() > 0;

                return canUseItem;
            }
        }
    }
}