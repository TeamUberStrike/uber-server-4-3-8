using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages all our paying stand alone out there
    /// </summary>
    public static class ClientApplication
    {
        /// <summary>
        /// When a user buys one of our application, we can register the sell and execute specific actions
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hashCode"></param>
        /// <param name="channel"></param>
        /// <param name="applicationId"></param>
        /// <param name="itemsAttributed"></param>
        /// <returns></returns>
        public static ApplicationRegistrationResult RegisterClientApplication(int cmid, string hashCode, ChannelType channel, int applicationId, out Dictionary<int, int> itemsAttributed)
        {
            ApplicationRegistrationResult result = ApplicationRegistrationResult.InvalidApplication;
            itemsAttributed = new Dictionary<int, int>();

            // First we need to check if we're selling a standalone for this application

            if (CommonConfig.ApplicationsHavingPayingClient.Contains(applicationId))
            {
                if (!hashCode.IsNullOrFullyEmpty())
                {
                    using (CmuneDataContext cmuneDb = new CmuneDataContext())
                    {
                        // We check if this hash was already used

                        ApplicationRegistration registration = GetApplicationRegistration(hashCode, channel, applicationId, cmuneDb);

                        if (registration == null)
                        {
                            DateTime registrationDate = DateTime.Now;

                            switch (applicationId)
                            {
                                case CommonConfig.ApplicationIdUberstrike:

                                    // In the case of UberStrike we allocate items

                                    string itemIdsConfigs = ConfigurationUtilities.ReadConfigurationManager(String.Format("ApplicationRegistrationItems.{0}", CommonConfig.ApplicationIdUberstrike));
                                    List<int> itemIds = itemIdsConfigs.Split(',').ToList().ConvertAll(u => Convert.ToInt32(u));

                                    itemsAttributed = CmuneEconomy.AddItemsToInventoryPermanently(cmid, itemIds, registrationDate);

                                    result = ApplicationRegistrationResult.Ok;

                                    break;
                                default:
                                    result = ApplicationRegistrationResult.InvalidApplication;
                                    CmuneLog.LogUnexpectedReturn(applicationId, String.Format("This application is not implemented yet?cmid={0}&hashCode={1}&channel={2}&applicationId={3}", cmid, hashCode, channel, applicationId));
                                    break;
                            }

                            if (result.Equals(ApplicationRegistrationResult.Ok))
                            {
                                RecordHashCode(cmid, hashCode, channel, applicationId, registrationDate, cmuneDb);
                            }
                        }
                        else
                        {
                            result = ApplicationRegistrationResult.DuplicateHashCode;
                        }
                    }
                }
                else
                {
                    result = ApplicationRegistrationResult.InvalidHash;
                }
            }

            return result;
        }

        /// <summary>
        /// Get a specific application registration
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="channel"></param>
        /// <param name="applicationId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        private static ApplicationRegistration GetApplicationRegistration(string hashCode, ChannelType channel, int applicationId, CmuneDataContext cmuneDb)
        {
            ApplicationRegistration registration = null;

            if (cmuneDb != null)
            {
                registration = cmuneDb.ApplicationRegistrations.SingleOrDefault(a => a.ApplicationId == applicationId && a.HashCode == hashCode && a.ChannelId == (int)channel);
            }

            return registration;
        }

        /// <summary>
        /// Record a hash
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hashCode"></param>
        /// <param name="channel"></param>
        /// <param name="applicationId"></param>
        /// <param name="registrationDate"></param>
        /// <param name="cmuneDb"></param>
        private static void RecordHashCode(int cmid, string hashCode, ChannelType channel, int applicationId, DateTime registrationDate, CmuneDataContext cmuneDb)
        {
            if (cmuneDb != null)
            {
                ApplicationRegistration registration = new ApplicationRegistration();
                registration.ApplicationId = applicationId;
                registration.ChannelId = (int)channel;
                registration.Cmid = cmid;
                registration.HashCode = hashCode;
                registration.RegistrationDate = registrationDate;

                cmuneDb.ApplicationRegistrations.InsertOnSubmit(registration);
                cmuneDb.SubmitChanges();
            }
        }
    }
}