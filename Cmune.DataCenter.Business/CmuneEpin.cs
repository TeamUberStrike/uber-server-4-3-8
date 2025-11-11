using System;
using Cmune.DataCenter.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils.Cryptography;
using System.Collections.ObjectModel;
using Cmune.DataCenter.Utils;
using System.Data.Linq.SqlClient;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages E Pin
    /// </summary>
    public static class CmuneEpin
    {
        #region Generation

        /// <summary>
        /// Generates a batch of epins
        /// </summary>
        /// <param name="applicationid"></param>
        /// <param name="totalCount"></param>
        /// <param name="creditAmount"></param>
        /// <param name="epinProvider"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static bool GenerateBatch(int applicationid, int totalCount, int creditAmount, PaymentProviderType epinProvider, bool isAdmin)
        {
            bool isGenerationSuccesful = false;

            List<string> epinCodes = GenerateNonExistingEpinsCode(totalCount);

            int batchId = InsertBatch(applicationid, epinProvider, totalCount, creditAmount, isAdmin);

            if (batchId != 0)
            {
                InsertEpins(batchId, epinCodes);
                isGenerationSuccesful = true;
            }

            return isGenerationSuccesful;
        }

        /// <summary>
        /// Insert the Epins from a batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="epinCodes"></param>
        private static void InsertEpins(int batchId, List<string> epinCodes)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Epin> epins = new List<Epin>();

                foreach (string epinCode in epinCodes)
                {
                    epins.Add(new Epin { BatchId = batchId, IsRedeemed = false, Pin = epinCode });
                }

                cmuneDb.Epins.InsertAllOnSubmit(epins);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Creates a batch
        /// </summary>
        /// <param name="applicationid"></param>
        /// <param name="epinProvider"></param>
        /// <param name="totalCount"></param>
        /// <param name="creditAmount"></param>
        /// <param name="isAdmin"></param>
        /// <returns>0 if insert fails</returns>
        private static int InsertBatch(int applicationid, PaymentProviderType epinProvider, int totalCount, int creditAmount, bool isAdmin)
        {
            int batchId = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (epinProvider == PaymentProviderType.Cmune)
                {
                    isAdmin = true;
                }

                EpinBatch batch = new EpinBatch();

                batch.ApplicationId = applicationid;
                batch.BatchDate = DateTime.Now;
                batch.Amount = totalCount;
                batch.CreditAmount = creditAmount;
                batch.EpinProvider = (int)epinProvider;
                batch.IsAdmin = isAdmin;

                cmuneDb.EpinBatches.InsertOnSubmit(batch);
                cmuneDb.SubmitChanges();

                batchId = batch.BatchId;
            }

            return batchId;
        }

        /// <summary>
        /// Generates totalCount distinct E-Pin codes that are not stored in the db
        /// </summary>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private static List<string> GenerateNonExistingEpinsCode(int totalCount)
        {
            Dictionary<string, string> epins = new Dictionary<string, string>();
            int attemptsCount = 0;
            int attempsLimit = 5;

            while (epins.Count < totalCount)
            {
                List<string> newEpins = GenerateEpinCodes(totalCount);
                List<string> epinsToBeAdded = new List<string>();
                attemptsCount++;

                foreach (string newEpin in newEpins)
                {
                    if (!epins.ContainsKey(newEpin))
                    {
                        epinsToBeAdded.Add(newEpin);
                    }
                }

                epinsToBeAdded = GetNonDuplicate(epinsToBeAdded);

                foreach (string epinToBeAdded in epinsToBeAdded)
                {
                    epins.Add(epinToBeAdded, epinToBeAdded);
                }

                if (attemptsCount > attempsLimit)
                {
                    throw new InvalidOperationException(String.Format("Coudn't generate {0} distincts and non existing pins after {1} attempts.", totalCount, attempsLimit));
                }
            }

            return epins.Keys.ToList();
        }

        /// <summary>
        /// Returns only the epins that are not stored in the database
        /// </summary>
        /// <param name="epinsCodes"></param>
        /// <returns></returns>
        private static List<string> GetNonDuplicate(List<string> epinsCodes)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<string> distinctEpins = new List<string>();

                Dictionary<string, string> duplicatedEpins = (from e in cmuneDb.Epins
                                                              where epinsCodes.Contains(e.Pin)
                                                              select new { Epin = e.Pin }).ToDictionary(e => e.Epin, e => e.Epin);

                if (duplicatedEpins.Count > 0)
                {
                    foreach (string epinCode in epinsCodes)
                    {
                        if (!duplicatedEpins.ContainsKey(epinCode))
                        {
                            distinctEpins.Add(epinCode);
                        }
                    }
                }
                else
                {
                    distinctEpins = epinsCodes;
                }

                return distinctEpins;
            }
        }

        /// <summary>
        /// Generates totalCount distinct E-Pin codes
        /// </summary>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private static List<string> GenerateEpinCodes(int totalCount)
        {
            Dictionary<string, string> epins = new Dictionary<string, string>();
            int attemptsCount = 0;
            int attempsLimit = 3 * totalCount;

            for (int i = 0; i < totalCount; )
            {
                string newEpinCode = GenerateEpinCode();
                attemptsCount++;

                if (!epins.ContainsKey(newEpinCode))
                {
                    epins.Add(newEpinCode, newEpinCode);
                    i++;
                }
                else if (attemptsCount > attempsLimit)
                {
                    throw new InvalidOperationException(String.Format("Coudn't generate {0} distinct pins after {1} attempts.", totalCount, attempsLimit));
                }
            }

            return epins.Keys.ToList();
        }

        /// <summary>
        /// Generates a pin code
        /// </summary>
        /// <returns></returns>
        private static string GenerateEpinCode()
        {
            return RandomPassword.Generate(20);
        }

        #endregion

        #region Get

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="elementsPerPage"></param>
        /// <returns></returns>
        public static List<EpinBatchView> GetBatches(int pageIndex, int elementsPerPage)
        {
            List<EpinBatchView> epinBatchesView = new List<EpinBatchView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int elementsToSkip = 0;

                if (pageIndex > 1)
                {
                    elementsToSkip = (pageIndex - 1) * elementsPerPage;
                }

                List<EpinBatch> batches = (from i in cmuneDb.EpinBatches
                                           orderby i.BatchId descending
                                           select i).Skip(elementsToSkip).Take(elementsPerPage).ToList();

                epinBatchesView = ToEpinBatchView(batches);
            }

            return epinBatchesView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetBatchesCount()
        {
            int count = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                count = (from i in cmuneDb.EpinBatches select i.BatchId).Count();
            }

            return count;
        }

        /// <summary>
        /// Get a batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="getEpins"></param>
        /// <returns></returns>
        public static EpinBatchView GetBatch(int batchId, bool getEpins = true)
        {
            EpinBatchView batchView = null;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                EpinBatch batch = cmuneDb.EpinBatches.SingleOrDefault(b => b.BatchId == batchId);

                if (batch != null)
                {
                    if (getEpins)
                    {
                        List<Epin> epins = cmuneDb.Epins.Where(e => e.BatchId == batchId).ToList();
                        batchView = ToEpinBatchView(batch, epins);
                    }
                    else
                    {
                        batchView = ToEpinBatchView(batch);
                    }
                }
            }

            return batchView;
        }

        /// <summary>
        /// Get batches
        /// </summary>
        /// <param name="batchIds"></param>
        /// <param name="getEpins"></param>
        /// <returns></returns>
        public static List<EpinBatchView> GetBatches(List<int> batchIds, bool getEpins = true)
        {
            List<EpinBatchView> batchesView = new List<EpinBatchView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<EpinBatch> batches = cmuneDb.EpinBatches.Where(b => batchIds.Contains(b.BatchId)).ToList();

                if (batches != null && batches.Count > 0)
                {
                    if (getEpins)
                    {
                        Dictionary<int, List<Epin>> epinsPerBatch = new Dictionary<int, List<Epin>>();
                        List<Epin> epins = cmuneDb.Epins.Where(e => batches.Select(b => b.BatchId).Contains(e.BatchId)).ToList();

                        foreach (Epin epin in epins)
                        {
                            if (epinsPerBatch.ContainsKey(epin.BatchId))
                            {
                                epinsPerBatch[epin.BatchId].Add(epin);
                            }
                            else
                            {
                                epinsPerBatch[epin.BatchId] = new List<Epin> { epin };
                            }
                        }

                        foreach (EpinBatch batch in batches)
                        {
                            batchesView.Add(ToEpinBatchView(batch, epinsPerBatch[batch.BatchId]));
                        }
                    }
                    else
                    {
                        batchesView = ToEpinBatchView(batches);
                    }
                }
            }

            return batchesView;
        }

        /// <summary>
        /// Get the Cmids that redeemded the Epins (if they were redeemed)
        /// </summary>
        /// <param name="epinViews"></param>
        /// <returns></returns>
        public static Dictionary<int, int> GetCmidThatRedeemedPins(List<EpinView> epinViews)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Dictionary<int, int> cmids = new Dictionary<int, int>();

                List<string> redeemedEpinIds = epinViews.Where(e => e.IsRedeemed).Select(e => e.EpinId).ToList().ConvertAll(new Converter<int, string>(e => e.ToString())).ToList();

                cmids = (from c in cmuneDb.CreditDeposits
                         where redeemedEpinIds.Contains(c.TransactionKey)
                         select new { EpinId = Convert.ToInt32(c.TransactionKey), Cmid = c.UserId }).ToDictionary(e => e.EpinId, e => e.Cmid);

                return cmids;
            }
        }

        #endregion

        #region Search

        /// <summary>
        /// Search E-Pins by Ids
        /// </summary>
        /// <param name="epinIds"></param>
        /// <returns></returns>
        public static List<EpinView> SearchEpins(List<int> epinIds)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<EpinView> epinViews = new List<EpinView>();

                List<Epin> epins = cmuneDb.Epins.Where(e => epinIds.Contains(e.EpinId)).ToList();
                epinViews = ToEpinView(epins);

                return epinViews;
            }
        }

        /// <summary>
        /// Search E-Pins by range of Ids
        /// </summary>
        /// <param name="epinIdStart"></param>
        /// <param name="epinIdEnd"></param>
        /// <returns></returns>
        public static List<EpinView> SearchEpins(int epinIdStart, int epinIdEnd)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<EpinView> epinViews = new List<EpinView>();

                List<Epin> epins = cmuneDb.Epins.Where(e => e.EpinId >= epinIdStart && e.EpinId <= epinIdEnd).ToList();
                epinViews = ToEpinView(epins);

                return epinViews;
            }
        }

        /// <summary>
        /// Search E-Pins by Pins
        /// </summary>
        /// <param name="pins"></param>
        /// <returns></returns>
        public static List<EpinView> SearchEpins(List<string> pins)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<EpinView> epinViews = new List<EpinView>();

                List<Epin> epins = cmuneDb.Epins.Where(e => pins.Contains(e.Pin)).ToList();
                epinViews = ToEpinView(epins);

                return epinViews;
            }
        }

        #endregion

        #region Retire

        /// <summary>
        /// Retires or unretire epins
        /// </summary>
        /// <param name="epinId"></param>
        /// <returns></returns>
        public static bool ChangeEpinRetirementStatus(int epinId)
        {
            bool isStatusChanged = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Epin epin = cmuneDb.Epins.SingleOrDefault(b => b.EpinId == epinId);

                if (epin != null)
                {
                    epin.IsRetired = !epin.IsRetired;
                    cmuneDb.SubmitChanges();

                    isStatusChanged = true;
                }
            }

            return isStatusChanged;
        }

        /// <summary>
        /// Retires multiple epins
        /// </summary>
        /// <param name="epinIds"></param>
        /// <returns></returns>
        public static bool RetireEpins(List<int> epinIds)
        {
            bool areEpinsRetired = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Epin> epins = cmuneDb.Epins.Where(e => epinIds.Contains(e.EpinId)).ToList();

                if (epins != null && epins.Count > 0)
                {
                    foreach (Epin epin in epins)
                    {
                        epin.IsRetired = true;
                    }

                    cmuneDb.SubmitChanges();
                    areEpinsRetired = true;
                }
            }

            return areEpinsRetired;
        }

        /// <summary>
        /// Retires or unretire a batch
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static bool ChangeBatchRetirementStatus(int batchId)
        {
            bool isStatusChanged = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                EpinBatch epinBatch = cmuneDb.EpinBatches.SingleOrDefault(b => b.BatchId == batchId);

                if (epinBatch != null)
                {
                    epinBatch.IsRetired = !epinBatch.IsRetired;
                    cmuneDb.SubmitChanges();

                    isStatusChanged = true;
                }
            }

            return isStatusChanged;
        }

        #endregion

        #region Redeem

        /// <summary>
        /// Redeem a PIN
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="cmid"></param>
        /// <param name="pin"></param>
        /// <param name="channel"></param>
        /// <param name="creditAmountAttributed"></param>
        /// <returns></returns>
        public static EpinTransactionResult Redeem(int applicationId, int cmid, string pin, ChannelType channel, out int creditAmountAttributed)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                EpinTransactionResult result = EpinTransactionResult.Error;
                int creditsDepositId = 0;
                bool isEpinValid = false;
                creditAmountAttributed = 0;

                if (IsRedeemDataValid(applicationId, cmid, pin, channel))
                {
                    Epin epin = cmuneDb.Epins.SingleOrDefault(e => e.Pin == pin);

                    if (epin != null && epin.EpinBatch != null)
                    {
                        if (!epin.IsRedeemed && !epin.EpinBatch.IsRetired && !epin.IsRetired && applicationId == epin.EpinBatch.ApplicationId)
                        {
                            isEpinValid = true;
                        }
                        else if (epin.IsRedeemed)
                        {
                            result = EpinTransactionResult.AlreadyRedeemed;
                        }
                        else if (epin.EpinBatch.IsRetired || epin.IsRetired)
                        {
                            result = EpinTransactionResult.Retired;
                        }
                        else if (applicationId != epin.EpinBatch.ApplicationId)
                        {
                            result = EpinTransactionResult.InvalidApplication;
                        }
                        else
                        {
                            result = EpinTransactionResult.Error;

                            CmuneLog.LogUnexpectedReturn(result, String.Format("Epin in DB is invalid somehow: applicationId={0}&cmid={1}&pin={2}&channel={3}", applicationId, cmid, pin, channel));
                        }
                    }
                    else
                    {
                        result = EpinTransactionResult.InvalidPin;
                    }

                    if (isEpinValid)
                    {
                        decimal cash = (decimal)epin.EpinBatch.CreditAmount / CommonConfig.CurrenciesToCreditsConversionRate[CurrencyType.Usd];

                        bool areCreditsAttributed = CmuneEconomy.ProcessCreditAttribution(cmid, cash, CurrencyType.Usd, epin.EpinBatch.IsAdmin, (PaymentProviderType)epin.EpinBatch.EpinProvider, epin.EpinId.ToString(), epin.EpinBatch.ApplicationId, channel, null, out creditsDepositId, true, epin.EpinBatch.CreditAmount);

                        if (areCreditsAttributed)
                        {
                            epin.IsRedeemed = true;
                            cmuneDb.SubmitChanges();

                            result = EpinTransactionResult.Ok;
                            creditAmountAttributed = epin.EpinBatch.CreditAmount;
                        }
                        else
                        {
                            result = EpinTransactionResult.Error;

                            CmuneLog.LogUnexpectedReturn(result, String.Format("Credits attribution failed: applicationId={0}&cmid={1}&pin={2}&channel={3}", applicationId, cmid, pin, channel));
                        }
                    }
                }
                else
                {
                    result = EpinTransactionResult.InvalidData;
                }

                return result;
            }
        }

        private static bool IsRedeemDataValid(int applicationId, int cmid, string pin, ChannelType channel)
        {
            bool isDataValid = true;

            if (!CommonConfig.ApplicationsName.ContainsKey(applicationId))
            {
                isDataValid = false;
            }

            if (isDataValid && cmid < 1)
            {
                isDataValid = false;
            }

            // Might code a pin validation function

            if (isDataValid && pin.IsNullOrFullyEmpty())
            {
                isDataValid = false;
            }

            return isDataValid;
        }

        #endregion

        #region To

        private static List<EpinView> ToEpinView(List<Epin> epins)
        {
            List<EpinView> epinViews = new List<EpinView>();

            if (epins != null && epins.Count > 0)
            {
                epinViews = epins.ConvertAll(new Converter<Epin, EpinView>(e => ToEpinView(e)));
            }

            return epinViews;
        }

        private static EpinView ToEpinView(Epin epin)
        {
            EpinView epinView = null;

            if (epin != null)
            {
                epinView = new EpinView(epin.EpinId, epin.Pin, epin.IsRedeemed, epin.BatchId, epin.IsRetired);
            }

            return epinView;
        }

        private static List<EpinBatchView> ToEpinBatchView(List<EpinBatch> epinBatches)
        {
            List<EpinBatchView> epinBatchViews = new List<EpinBatchView>();

            if (epinBatches != null && epinBatches.Count > 0)
            {
                epinBatchViews = epinBatches.ConvertAll(new Converter<EpinBatch, EpinBatchView>(e => ToEpinBatchView(e)));
            }

            return epinBatchViews;
        }

        private static EpinBatchView ToEpinBatchView(EpinBatch epinBatch)
        {
            return ToEpinBatchView(epinBatch, new List<Epin>());
        }

        private static EpinBatchView ToEpinBatchView(EpinBatch epinBatch, List<Epin> epins)
        {
            EpinBatchView epinBatchView = null;

            if (epinBatch != null)
            {
                epinBatchView = new EpinBatchView(epinBatch.BatchId,
                                                    epinBatch.ApplicationId,
                                                    (PaymentProviderType)epinBatch.EpinProvider,
                                                    epinBatch.Amount,
                                                    epinBatch.CreditAmount,
                                                    epinBatch.BatchDate,
                                                    epinBatch.IsAdmin,
                                                    epinBatch.IsRetired,
                                                    ToEpinView(epins));
            }

            return epinBatchView;
        }

        #endregion

        /// <summary>
        /// Detect duplicates (to be deleted most likely)
        /// </summary>
        /// <returns></returns>
        public static bool DetectDuplicates()
        {
            using (var db = new CmuneDataContext())
            {
                var query =
                   from m in db.Epins
                   group m by m.Pin into g
                   where g.Count() > 1
                   select g.Key;

                foreach (var row in query)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the actual ip can redeem
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool CanRedeem(long ip, int cmid)
        {
            int numberOfFailedAttempts = 0;

            using (var cmuneDb = new CmuneDataContext())
            {
                var query = from epinFailedAttempts in cmuneDb.EpinFailedAttempts
                            where (epinFailedAttempts.Ip == ip && epinFailedAttempts.Cmid == cmid) && (DateTime.Now - epinFailedAttempts.AttemptDate).TotalMinutes < 30
                            select epinFailedAttempts;
                numberOfFailedAttempts = query.ToList().Count(); 
            }
            if (numberOfFailedAttempts >= 5)
                return false;
            return true;
        }

        /// <summary>
        /// Record a failed attempt
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="ip"></param>
        /// <param name="pin"></param>
        public static void RecordFailedAttempt(int cmid, long ip, string pin)
        {
            using (var cmuneDb = new CmuneDataContext())
            {
                var epinFailedAttempt = new EpinFailedAttempt();

                epinFailedAttempt.Cmid = cmid;
                epinFailedAttempt.Ip = ip;
                epinFailedAttempt.Pin = pin;
                epinFailedAttempt.AttemptDate = DateTime.Now;

                cmuneDb.EpinFailedAttempts.InsertOnSubmit(epinFailedAttempt);
                cmuneDb.SubmitChanges();
            }
        }
    }
}
