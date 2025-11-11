// -----------------------------------------------------------------------
// <copyright file="EmailMarketing.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Cmune.DataCenter.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SendGridSdk;
    using Cmune.DataCenter.DataAccess;
    using Cmune.DataCenter.Common.Entities;
    using Cmune.DataCenter.Common.Utils.Cryptography;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class EmailMarketing
    {
        /// <summary>
        /// Collects all bounces, spam reports... from our email provider and update our email status
        /// If you want the data for one day only, you'll have to use the dame date for startDate and endDate
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void UpdateEmailStatusFromProvider(DateTime startDate, DateTime endDate)
        {
            int batchLimit = 100;

            IList<string> unsuscribedAndSpamReportsEmails = SendGridUnsubscribes.GetUnsuscribed(startDate, endDate);
            IList<string> spamReportEmails = SendGridSpamReports.GetSpamReports(startDate, endDate);

            foreach (string spamReportEmail in spamReportEmails)
            {
                unsuscribedAndSpamReportsEmails.Add(spamReportEmail);
            }

            for (int i = 0; i < unsuscribedAndSpamReportsEmails.Count; i += batchLimit)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    List<Member> members = CmuneMember.GetMembers(unsuscribedAndSpamReportsEmails.Skip(i).Take(batchLimit).ToList(), cmuneDb);

                    foreach (Member member in members)
                    {
                        member.MarketingSubscriptionState = (byte)MarketingSubscriptionStatus.Unsubscribed;
                        member.EmailAddressState = (byte)EmailAddressStatus.Verified;
                    }

                    cmuneDb.SubmitChanges();
                }
            }

            IList<string> bouncedAndInvalidEmails = SendGridBounces.GetBounces(startDate, endDate);
            IList<string> invalidEmails = SendGridInvalidEmails.GetInvalidEmails(startDate, endDate);

            foreach (string invalidEmail in invalidEmails)
            {
                bouncedAndInvalidEmails.Add(invalidEmail);
            }

            for (int i = 0; i < bouncedAndInvalidEmails.Count; i += batchLimit)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    List<Member> members = CmuneMember.GetMembers(bouncedAndInvalidEmails.Skip(i).Take(batchLimit).ToList(), cmuneDb);

                    foreach (Member member in members)
                    {
                        member.EmailAddressState = (byte)EmailAddressStatus.Invalid;
                    }

                    cmuneDb.SubmitChanges();
                }
            }

            IList<string> blockedEmails = SendGridBlocks.GetBlocks(startDate, endDate);

            for (int i = 0; i < blockedEmails.Count; i += batchLimit)
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    List<Member> members = CmuneMember.GetMembers(blockedEmails.Skip(i).Take(batchLimit).ToList(), cmuneDb);

                    foreach (Member member in members)
                    {
                        member.MarketingSubscriptionState = (byte)MarketingSubscriptionStatus.Unsubscribed;
                    }

                    cmuneDb.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool UnsubscribeFromEmailMarketing(int cmid, string hash)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isMemberExisting = false;
                Member member = cmuneDb.Members.SingleOrDefault(m => m.CMID == cmid);

                if (member != null)
                {
                    string hashedRegistrationDate = Crypto.fncSHA256Encrypt(member.ResLastSyncDate.ToString(CultureInfo.InvariantCulture));

                    if (hashedRegistrationDate == hash)
                    {
                        member.MarketingSubscriptionState = (byte)MarketingSubscriptionStatus.Unsubscribed;

                        cmuneDb.SubmitChanges();
                        isMemberExisting = true;
                    }
                }

                return isMemberExisting;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void GenerateUnsubscribeLinks(string filePath)
        {
            FileStream fileStreamReader = null;
            StreamReader streamReader = null;
            FileStream fileStreamWriter = null;
            StreamWriter streamWriter = null;

            int extensionBeginning = filePath.LastIndexOf('.');
            string resultFilePath = filePath.Substring(0, extensionBeginning) + "-sex.csv";
            List<int> cmids = new List<int>();

            using (fileStreamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (streamReader = new StreamReader(fileStreamReader, Encoding.UTF8, false, 16384))
            {
                while (streamReader.Peek() >= 0)
                {
                    string currentLine = streamReader.ReadLine();
                    int cmidEndingIndex = currentLine.IndexOf(',');
                    string cmidFromFile = currentLine.Substring(0, cmidEndingIndex);

                    int cmid = 0;

                    if (Int32.TryParse(cmidFromFile, out cmid))
                    {
                        cmids.Add(cmid);
                    }
                }
            }

            int batchLimit = 500;
            Dictionary<int, string> hashesPerCmid = new Dictionary<int, string>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                for (int i = 0; i < cmids.Count; i += batchLimit)
                {
                    var members = from m in cmuneDb.Members
                                    where cmids.Skip(i).Take(batchLimit).Contains(m.CMID)
                                    select new { Cmid = m.CMID, Hash = Crypto.fncSHA256Encrypt(m.ResLastSyncDate.ToString(CultureInfo.InvariantCulture)) };

                    foreach (var member in members)
                    {
                        hashesPerCmid.Add(member.Cmid, member.Hash);
                    }
                }
            }

            using (fileStreamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (streamReader = new StreamReader(fileStreamReader, Encoding.UTF8, false, 16384))
            using (fileStreamWriter = new FileStream(resultFilePath, FileMode.Append, FileAccess.Write))
            using (streamWriter = new StreamWriter(fileStreamWriter))
            {
                while (streamReader.Peek() >= 0)
                {
                    string currentLine = streamReader.ReadLine();

                    int cmidEndingIndex = currentLine.IndexOf(',');
                    string cmidFromFile = currentLine.Substring(0, cmidEndingIndex);

                    int cmid = 0;

                    if (Int32.TryParse(cmidFromFile, out cmid))
                    {
                        currentLine += String.Format(CultureInfo.InvariantCulture, ",\"http://uberstrike.cmune.com/Account/Unsubscribe?c={0}&h={1}\"", cmidFromFile, hashesPerCmid[cmid]);
                        streamWriter.WriteLine(currentLine);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="isOkForMarketing"></param>
        /// <returns></returns>
        public static bool SetEmailStatus(int cmid, bool isOkForMarketing)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isSet = false;
                Member member = CmuneMember.GetMember(cmid, cmuneDb);

                if (member != null)
                {
                    if (isOkForMarketing)
                    {
                        member.MarketingSubscriptionState = (byte)MarketingSubscriptionStatus.BasicSubscription;
                    }
                    else
                    {
                        member.MarketingSubscriptionState = (byte)MarketingSubscriptionStatus.Unsubscribed;
                    }

                    cmuneDb.SubmitChanges();

                    isSet = true;
                }

                return isSet;
            }
        }
    }
}
