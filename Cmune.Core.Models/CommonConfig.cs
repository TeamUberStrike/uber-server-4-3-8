using System;
using System.Collections.Generic;

namespace Cmune.DataCenter.Common.Entities
{
    public struct CommonConfig
    {
        // TODO we should probably include other structs inside or break this one in more than one struct

        public const string ContactGroupDefaultName = "Default";
        public const int MemberMergePointsPenalizeDefault = 50;
        public const string SmtpIP = "67.228.44.4";
        public const string SmtpUser = "cmuneMailer";
        public const string SmtpPassword = "cmune$1";
        public static readonly string[] DefaultContactGroupToBeCreated = new string[] { "Default" };
        public const string CmuneSupportEmail = "support@cmune.com";
        public const string CmuneSupportEmailName = "The Cmune Team";
        public const string CmuneNoReplyEmail = "noreply@cmune.com";
        public const string CmuneNoReplyEmailName = "The Cmune Team";
        public const string CmuneSupportCenterUrl = "http://support.uberstrike.com/";
        public const string CmuneDevteamEmail = "devteam@cmune.com";
        public const string CmuneDevteamEmailName = "The Cmune Devteam";
        public const int IdentityValidationLifetimeInDays = 30;
        public const int AdminCmid = 767;
        public const int CommunityManagerCmid = 598916;
        public const int PointsAttributedOnRegistration = 1500;
        public const int PointsAttributedOnEmailValidation = 1000;

        #region Validation

        public const int GroupMottoMinLength = 1;
        public const int GroupMottoMaxLength = 25;
        public const int GroupDescriptionMinLength = 1;
        public const int GroupDescriptionMaxLength = 200;
        public const int ItemCategoryNameMinLength = 3; // The min length is included
        public const int ItemCategoryNameMaxLength = 20; // The max length is included
        public const int MemberNameMinLength = 3; // The min length is included
        public const int MemberNameMaxLength = 18; // The max length is included
        public const int MemberEmailMaxLength = 100;
        public const int MemberPasswordMinLength = 3;
        public const int MemberPasswordMaxLength = 64;
        public const int ContacGroupNameMinLength = 3; // The min length is included
        public const int ContactGroupNameMaxLength = 15; // The max length is included
        public const int GroupNameMinLength = 3; // The min length is included
        public const int GroupNameMaxLength = 25; // The max length is included
        public const int GroupTagMinLength = 2; // The min length is included
        public const int GroupTagMaxLenght = 5; // The max length is included
        public const int PhotonsGroupNameMinLenght = 3; // The min length is included
        public const int PhotonsGroupNameMaxLenght = 50; // The max length is included
        public const int PhotonsServerNameMinLenght = 3; // The min length is included
        public const int PhotonsServerNameMaxLenght = 255; // The max length is included
        public const int PhotonsGroupDescriptionMaxLength = 100; // The max length is included
        public const int MemberReportReasonMaxLength = 500; // The max length is included
        public const int MemberReportContextMaxLength = 4000; // The max length is included
        public const int ManagedServerNameMinLenght = 3; // The min length is included
        public const int ManagedServerNameMaxLenght = 50; // The max length is included
        public const int ManagedServerTestNameMinLenght = 3; // The min length is included
        public const int ManagedServerTestNameMaxLenght = 50; // The max length is included
        public const int RotationMemberNameMinLenght = 3; // The min length is included
        public const int RotationMemberNameMaxLenght = 50; // The max length is included
        public const int PortMinNumber = 1;
        public const int PortMaxNumber = 65535;

        #endregion Validation

        #region Application

        public static readonly Dictionary<int, string> ApplicationsName = new Dictionary<int, string> { { ApplicationIdUberstrike, "Uberstrike" } };
        public static readonly Dictionary<long, int> FacebookApplicationsId = new Dictionary<long, int> { { 24509077139, ApplicationIdUberstrike } };
        public const int ApplicationIdUberstrike = 1;
        public static readonly List<int> ApplicationsHavingPayingClient = new List<int> { ApplicationIdUberstrike };
        public static readonly List<ChannelType> ActiveChannels = new List<ChannelType> { ChannelType.MacAppStore, ChannelType.OSXStandalone, ChannelType.WebFacebook, ChannelType.WebPortal, ChannelType.WindowsStandalone, ChannelType.Kongregate, ChannelType.IPad, ChannelType.IPhone, ChannelType.Android };
        public static readonly List<ChannelType> WebChannels = new List<ChannelType> { ChannelType.WebFacebook, ChannelType.WebPortal, ChannelType.Kongregate };
        public static readonly List<ChannelType> StandaloneChannels = new List<ChannelType> { ChannelType.WindowsStandalone };

        #endregion Application

        public static readonly Dictionary<EsnsType, string> EsnsProfilesUrl = new Dictionary<EsnsType, string> { { EsnsType.Aol, String.Empty }, { EsnsType.Facebook, "http://www.facebook.com/profile.php?id=" }, { EsnsType.Gmail, String.Empty }, { EsnsType.LinkedIn, String.Empty }, { EsnsType.MySpace, "http://www.myspace.com/" }, { EsnsType.None, String.Empty }, { EsnsType.WindowsLive, String.Empty }, { EsnsType.Yahoo, String.Empty }, {EsnsType.Cyworld, String.Empty} };

        public const int StoredProcedureSuccess = 0;
        public const int StoredProcedureFailure = 1;

        #region Economy

        public const int ItemMallFieldDisable = -1;
        public const BuyingDurationType PackDuration = BuyingDurationType.Permanent;
        public const int ItemMinimumDurationInDays = 1;
        public const int ItemMaximumDurationInDays = 90;
        public const int ItemMaximumOwnableAmount = 1000;
        public static readonly Dictionary<PaymentProviderType, string> PaymentProviderName = new Dictionary<PaymentProviderType, string> { { PaymentProviderType.Cmune, "Cmune" }, { PaymentProviderType.Offerpal, "Offerpal" }, { PaymentProviderType.PlaySpan, "PlaySpan" }, { PaymentProviderType.SixWaves, "Six Waves" }, { PaymentProviderType.Zong, "Zong" }, { PaymentProviderType.SuperRewards, "Super Rewards" }, { PaymentProviderType.Dotori, "Cyworld" }, { PaymentProviderType.FacebookCredits, "Facebook" }, { PaymentProviderType.GameSultan, "Game Sultan" }, { PaymentProviderType.Apple, "Apple"}, { PaymentProviderType.KongregateKreds, "Kongregate Kreds"} };
        public static readonly Dictionary<ReferrerPartnerType, string> ReferrerPartnerName = new Dictionary<ReferrerPartnerType, string> { { ReferrerPartnerType.None, "None" }, { ReferrerPartnerType.AppleWidget, "Apple Widget" }, { ReferrerPartnerType.MySpace, "MySpace" }, { ReferrerPartnerType.SixWaves, "6waves" }, { ReferrerPartnerType.Applifier, "Applifier" } };
        public static readonly Dictionary<BuyingDurationType, string> BuyingDurationName = new Dictionary<BuyingDurationType, string> { { BuyingDurationType.None, "None" }, { BuyingDurationType.OneDay, "1" }, { BuyingDurationType.SevenDays, "7" }, { BuyingDurationType.ThirtyDays, "30" }, { BuyingDurationType.NinetyDays, "90" }, { BuyingDurationType.Permanent, "Permanent" } };
        public static readonly DateTime UberStrikeStartingDate = new DateTime(2010, 09, 15, 0, 0, 0);
        /// <summary>
        /// Used to separate the fields that we store in the "developerid" field of PlaySpan
        /// </summary>
        public const char PlaySpanDelimiter = '|';
        /// <summary>
        /// All the item Ids below are deprecated
        /// </summary>
        public const int NewItemMallItemIdStart = 1000;
        public const int DotoriToSouthKoreanWon = 100;
        public const int UsdToFacebookCredit = 10;
        public const int UsdToKreds = 10;
        public static readonly Dictionary<string, string> AcceptedCurrencies = new Dictionary<string, string> { {CurrencyType.Usd, CurrencyType.Usd} , {CurrencyType.Krw, CurrencyType.Krw} };
        public static readonly Dictionary<string, int> CurrenciesToCreditsConversionRate = new Dictionary<string, int> { { CurrencyType.Usd, 650 }, { CurrencyType.Krw, 1 } };

        public const int NameChangeItem = 1294;

        // Discount in %: 10 means 10% of discount compare to the price for one day
        public const int DiscountPointsSevenDays = 20;
        public const int DiscountPointsThirtyDays = 25;
        public const int DiscountPointsNinetyDays = 30;
        public const int DiscountCreditsSevenDays = 40;
        public const int DiscountCreditsThirtyDays = 75;
        public const int DiscountCreditsNinetyDays = 85;

        // Discount in %: 10 means 10% of discount compare to the price for one unit in the Pack One
        public const int DiscountPackThree = 20;

        public const int LuckyDrawSetCount = 3;
        public const int LuckyDrawSetItemMaxCount = 12;
        public const int MysteryBoxItemMaxCount = 12;

        #endregion Economy

        #region Groups

        public const int GroupMembersLimitCount = 12;

        #endregion Groups

        #region Private messages

        public const int PrivateMessagesInboxPageSize = 30;
        public const int PrivateMessagesThreadPageSize = 30;

        #endregion Private messages

        /// <summary>
        /// This item is owned only by players that created a loadout for the version 4.0 of UberStrike
        /// </summary>
        public const int PrivateerUberStrikeLicenseId = 1094;
        /// <summary>
        /// This item is owned only by members that can own a clan in UberStrike
        /// </summary>
        public const int ClanLeaderUberStrikeLicenseId = 1234;
        /// <summary>
        /// This item is owned only by members that bought UberStrike on the Mac App Store
        /// </summary>
        public const int MacAppStoreUberStrikeLicenseId = 1265;

        public static readonly int LoginMinPointsPerDay = 500;
        public static readonly int LoginMaxPointsPerDay = 1500;
        public static readonly int LoginDailyGrowth = 100;
        public static readonly int LoginDailyDecay = 200;
    }

    /// <summary>
    /// Config name for web/app.config
    /// </summary>
    public struct CommonAppSettings
    {
        public const string PassPhrase = "CmunePassPhrase";
        public const string InitVector = "CmuneInitVector";
        public const string DatabaseDataSourceOverride = "DbAddressCore";
        public const string DatabaseDataSource = "CmuneAPIKey";
        public const string DatabaseForumDataSourceOverride = "DbAddressMvForum";
        public const string DatabaseForumDataSource = "appsAPIKey";
    }

    public struct DatabaseDeployment
    {
        public const string Dev = "dev";
        public const string Staging = "staging";
        public const string Prod = "prod";
    }

    public struct CmuneCacheKeys
    {
        public const string MemberAccessParameters = "CmuneMemberAccess";
        public const string Separator = "_";
    }
}