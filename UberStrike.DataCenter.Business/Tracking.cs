using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Utils.Caching;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business
{
    public static class Tracking
    {
        /// <summary>
        /// Record a step during the Unity install flow
        /// </summary>
        /// <param name="trackingId"></param>
        /// <param name="channel"></param>
        /// <param name="stepId"></param>
        /// <param name="referrerId"></param>
        /// <param name="isJavaInstall"></param>
        /// <param name="osName"></param>
        /// <param name="browserName"></param>
        /// <param name="browserVersion"></param>
        /// <param name="ipAddress"></param>
        /// <param name="userAgent"></param>
        public static void UserInstallTracking(string trackingId, ChannelType channel, UserInstallStepType stepId, ReferrerPartnerType referrerId, bool isJavaInstall, string osName, string browserName, string browserVersion, string ipAddress, string userAgent)
        {
            string osVersion = "0";

            try
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    // Get install Id for this user (create if none found and the step Id is the first one)

                    // Let's clean the data

                    if (browserVersion.IsNullOrFullyEmpty() || String.Equals("an unknown version", browserVersion))
                    {
                        browserVersion = "0";
                    }
                    else if (browserVersion.Length > 10)
                    {
                        browserVersion = browserVersion.ShortenText(10);
                    }

                    browserVersion = browserVersion.Trim();

                    if (browserName.IsNullOrFullyEmpty() || String.Equals("An unknown browser", browserName) || String.Equals("undefined", browserName))
                    {
                        browserName = "0";
                    }
                    else if (browserName.Length > 15)
                    {
                        browserName = browserName.ShortenText(15);
                    }

                    browserName = browserName.Trim();

                    if (userAgent != null && userAgent.Length > 500)
                    {
                        userAgent = userAgent.ShortenText(500);
                    }
                    else if (userAgent == null)
                    {
                        userAgent = String.Empty;
                    }

                    userAgent = userAgent.Trim();

                    if (osName.IsNullOrFullyEmpty() || String.Equals("an unknown OS", osName))
                    {
                        osName = "0";
                    }
                    else if (osName.Length > 20)
                    {
                        osName = osName.ShortenText(20);
                    }

                    osName = osName.Trim();

                    int userInstallId = 0;
                    List<int> userInstallIds = uberStrikeDb.UserInstalls.Where(i => i.TrackingId == trackingId).Select(i => i.UserInstallId).ToList();

                    if (userInstallIds.Count > 0)
                    {
                        userInstallId = userInstallIds[0];
                    }

                    if (userInstallId == 0 && (stepId == UserInstallStepType.NoUnity || stepId == UserInstallStepType.HasUnity))
                    {
                        // Create new InstallId now
                        UserInstall userInstallUser = new UserInstall();
                        userInstallUser.TrackingId = trackingId;
                        userInstallUser.ChannelId = (int)channel;
                        userInstallUser.ReferrerId = (int)referrerId;
                        userInstallUser.IsJavaInstallEnabled = isJavaInstall;
                        userInstallUser.OsName = osName;
                        userInstallUser.Ip = TextUtilities.InetAToN(ipAddress);
                        userInstallUser.BrowserName = browserName;
                        userInstallUser.BrowserVersion = browserVersion;

                        bool hasUnity = false;

                        if (stepId == UserInstallStepType.HasUnity)
                        {
                            hasUnity = true;
                        }
                        else if (stepId == UserInstallStepType.NoUnity)
                        {
                            hasUnity = false;
                        }

                        userInstallUser.HasUnity = hasUnity;

                        // we need to extract some information from the userAgent

                        if (!userAgent.IsNullOrFullyEmpty())
                        {
                            if (userAgent.Contains("Windows"))
                            {
                                string osNameInUserAgent = "Windows NT ";
                                string alternateOsNameInUserAgent = "Windows ";

                                int osNameIndex = userAgent.IndexOf(osNameInUserAgent);
                                int versionIndex = -1;

                                if (osNameIndex == -1)
                                {
                                    osNameIndex = userAgent.IndexOf(alternateOsNameInUserAgent);

                                    if (osNameIndex != -1)
                                    {
                                        versionIndex = osNameIndex + alternateOsNameInUserAgent.Length;
                                    }
                                }
                                else
                                {
                                    versionIndex = osNameIndex + osNameInUserAgent.Length;
                                }

                                int endVersionIndex = userAgent.IndexOf(";", versionIndex);

                                if (endVersionIndex == -1)
                                {
                                    endVersionIndex = userAgent.IndexOf(")", versionIndex);
                                }

                                if (versionIndex != -1 && endVersionIndex != -1)
                                {
                                    osVersion = userAgent.Substring(versionIndex, endVersionIndex - versionIndex);

                                    if (osVersion.StartsWith("Phone OS"))
                                    {
                                        osVersion = osVersion.Replace("Phone OS", "Phone");
                                    }

                                    if (osVersion == "4.10")
                                    {
                                        osVersion = "98";
                                    }
                                }
                            }
                            else if (userAgent.Contains("Macintosh"))
                            {
                                string osNameInUserAgent = "OS X ";

                                int osNameIndex = userAgent.IndexOf(osNameInUserAgent);
                                int versionIndex = -1;
                                int endVersionIndex = -1;

                                if (osNameIndex != -1)
                                {
                                    versionIndex = osNameIndex + osNameInUserAgent.Length;

                                    endVersionIndex = userAgent.IndexOf(";", versionIndex);

                                    if (endVersionIndex == -1)
                                    {
                                        endVersionIndex = userAgent.IndexOf(")", versionIndex);
                                    }
                                }

                                if (versionIndex != -1 && endVersionIndex != -1)
                                {
                                    osVersion = userAgent.Substring(versionIndex, endVersionIndex - versionIndex);
                                    osVersion = osVersion.Replace('_', '.');
                                }
                            }
                        }

                        userInstallUser.OsVersion = osVersion;
                        userInstallUser.UserAgent = userAgent;

                        uberStrikeDb.UserInstalls.InsertOnSubmit(userInstallUser);
                        uberStrikeDb.SubmitChanges();

                        userInstallId = userInstallUser.UserInstallId;
                    }

                    if (userInstallId != 0)
                    {
                        // Add the new install step                    

                        UserInstallStep newStep = new UserInstallStep();

                        newStep.UserInstallId = userInstallId;
                        newStep.StepType = byte.Parse(((int)stepId).ToString());
                        newStep.StepDate = DateTime.Now;

                        uberStrikeDb.UserInstallSteps.InsertOnSubmit(newStep);
                        uberStrikeDb.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Format("trackingId={0}&channel={1}&stepId={2}&referrerId={3}&isJavaInstall={4}&osName={5}&browserName={6}&browserVersion={7}&ip={8}&osVersion={9}&userAgent={10}", trackingId, channel, stepId, referrerId, isJavaInstall, osName, browserName, browserVersion, ipAddress, osVersion, userAgent));
                throw;
            }
        }

        #region Tutorial Conversion Funnel

        public static void RecordTutorialStep(int cmid, TutorialStepType step)
        {
            if (cmid != 0)
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    TutorialStep tutorialStep = new TutorialStep();
                    tutorialStep.Cmid = cmid;
                    tutorialStep.StepDateTime = DateTime.Now;
                    tutorialStep.StepId = (int)step;

                    uberStrikeDb.TutorialSteps.InsertOnSubmit(tutorialStep);
                    uberStrikeDb.SubmitChanges();
                }
            }
        }

        public static bool HasCompletedTutorial(int cmid)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool hasCompletedTutorial = uberStrikeDb.TutorialSteps.Where(t => t.Cmid == cmid && t.StepId == (int)TutorialStepType.TutorialComplete).Count() == 1;

                return hasCompletedTutorial;
            }
        }

        public static List<TutorialStepView> GetTutorialSteps(int cmid)
        {
            List<TutorialStepView> stepsView = new List<TutorialStepView>();

            if (cmid != 0)
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    List<TutorialStep> steps = uberStrikeDb.TutorialSteps.Where(s => s.Cmid == cmid).OrderBy(s => s.Id).ToList();

                    stepsView = steps.ConvertAll(new Converter<TutorialStep, TutorialStepView>(s => new TutorialStepView(s.Cmid, (TutorialStepType)s.StepId, s.StepDateTime))); 
                }
            }

            return stepsView;
        }

        #endregion

        /// <summary>
        /// Is this Facebook user coming from 6 Waves
        /// </summary>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        public static bool IsFbUserComingFrom6Waves(long facebookId)
        {
            bool isFbUserComingFrom6Waves = false;
            Cache<bool> c = new Cache<bool>("Cmune.Datacenter.Apps.Shooter.IsFbUserComingFrom6Waves." + facebookId.ToString());
            if (c.IsInCache) return c.Val;

            using (UberstrikeDataContext paradiseDB = new UberstrikeDataContext())
            {
                List<FacebookReferrer> userSourceList = new List<FacebookReferrer>();

                // Timeout issue
                try
                {
                    userSourceList = (from tab in paradiseDB.FacebookReferrers where tab.FacebookID == facebookId && tab.ReferrerPartnerId == (int)ReferrerPartnerType.SixWaves orderby tab.FacebookReferrerID descending select tab).ToList();
                }
                catch (Exception ex)
                {
                    CmuneLog.LogException(ex, String.Empty);
                    throw;
                }
                // End timeout issue

                if (userSourceList.Count > 0)
                {
                    if (userSourceList[0].Referrer != null)
                    {
                        isFbUserComingFrom6Waves = true;
                    }
                }

                c.InsertHour(isFbUserComingFrom6Waves);
                return isFbUserComingFrom6Waves;
            }
        }
    }
}