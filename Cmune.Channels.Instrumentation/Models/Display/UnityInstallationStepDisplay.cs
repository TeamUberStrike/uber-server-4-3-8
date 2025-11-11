using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class UnityInstallationStepDisplay
    {
        #region Properties

        public int UserInstallId { get; private set; }
        public UserInstallStepType StepType { get; private set; }
        public DateTime StepDate { get; private set; }

        #endregion

        #region Constructors

        public UnityInstallationStepDisplay(int installId, UserInstallStepType stepType, DateTime stepDate)
        {
            this.UserInstallId = installId;
            this.StepType = stepType;
            this.StepDate = stepDate;
        }

        #endregion
    }

    public class UnityInstallationDisplay
    {
        #region Properties

        public int UserInstallId { get; private set; }
        public string TrackingId { get; private set; }
        public ChannelType Channel { get; private set; }
        public ReferrerPartnerType ReferrerId { get; private set; }
        public List<UnityInstallationStepDisplay> Steps { get; private set; }
        public string OsName { get; private set; }
        public string OsVersion { get; private set; }
        public string BrowserName { get; private set; }
        public string BrowserVersion { get; private set; }
        public bool IsJavaInstallEnabled { get; private set; }
        public string Ip { get; private set; }
        public string UserAgent { get; private set; }
        public bool HasUnity { get; private set; }

        #endregion

        #region Constructors

        public UnityInstallationDisplay(int userInstallId, string trackingId, ChannelType channel, ReferrerPartnerType referrerId, List<UnityInstallationStepDisplay> steps, string osName, string osVersion, string browserName, string browserVersion, bool isJavaInstallEnabled, string ip, string userAgent, bool hasUnity)
        {
            UserInstallId = userInstallId;
            TrackingId = trackingId;
            Channel = channel;
            ReferrerId = referrerId;
            Steps = steps;
            OsName = osName;
            OsVersion = osVersion;
            BrowserName = browserName;
            BrowserVersion = browserVersion;
            IsJavaInstallEnabled = isJavaInstallEnabled;
            Ip = ip;
            UserAgent = userAgent;
            HasUnity = hasUnity;
        }

        #endregion
    }
}