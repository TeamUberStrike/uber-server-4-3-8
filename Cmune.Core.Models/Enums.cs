using System;

namespace Cmune.DataCenter.Common.Entities
{
    #region Shop

    public enum BoxType
    {
        LuckyDraw = 1,
        MysteryBox = 2
    }

    public enum PurchaseType
    {
        Rent = 0,
        Pack = 1,
    }

    public enum ShopItemType
    {
        UberstrikeWeapon = 1,
        UberstrikeWeaponMod = 2,
        UberstrikeGear = 3,
        UberstrikeQuickUse = 4,
        UberstrikeFunctional = 5,
        UberstrikeSpecial = 6,
    }

    public class BuyItemResult
    {
        public const int Ok = 0;
        public const int DisableInShop = 1;
        public const int DisableForRent = 3;
        public const int DisableForPermanent = 4;
        public const int DurationDisabled = 5;
        public const int PackDisabled = 6;
        public const int IsNotForSale = 7;
        public const int NotEnoughCurrency = 8;
        public const int InvalidMember = 9;
        public const int InvalidExpirationDate = 10;
        public const int AlreadyInInventory = 11;
        public const int InvalidAmount = 12;
        public const int NoStockRemaining = 13;
        public const int InvalidData = 14;
        public const int TooManyUsage = 15;
    }

    public enum BuyingDurationType
    {
        None = 0,
        OneDay = 1,
        SevenDays = 2,
        ThirtyDays = 3,
        NinetyDays = 4,
        Permanent = 5,
    }

    public enum BuyingType
    {
        Rent = 0,
        Permanent = 1,
    }

    public enum BuyingMarketType
    {
        Shop = 0,
        Underground = 1
    }

    public enum BuyingLocationType
    {
        None = 0,
        Shop = 1,
        HomeScreen = 2,
        PreGame = 3,
        DeathScene = 4,
        EndOfRound = 5
    }

    public enum BuyingRecommendationType
    {
        None = 0,
        Manual = 1,
        Behavior = 2
    }

    public enum PackType
    {
        One = 0,
        Two = 1,
        Three = 2,
    }

    public enum PlaySpanTransactionType
    {
        Payment = 0,
        ForcedReversal = 1,
        AdminReversal = 2,
    }

    public enum PaymentProviderType
    {
        Cmune = 1,
        SixWaves = 2,
        Offerpal = 3,
        Zong = 4,
        PlaySpan = 5,
        SuperRewards = 6,
        /// <summary>
        /// It's a nut in Korean
        /// </summary>
        Dotori = 8,
        FacebookCredits = 9,
        GameSultan = 10,
        Apple = 11,
        KongregateKreds = 12,
        iOS = 13
    }

    public enum ReferrerPartnerType
    {
        None = 0,
        AppleWidget = 1,
        MySpace = 2,
        SixWaves = 4,
        Applifier = 5,
    }

    public static class CurrencyType
    {
        public const string Usd = "USD";
        /// <summary>
        /// South Korean Won
        /// </summary>
        public const string Krw = "KRW";
    }

    public enum UberStrikeCurrencyType
    {
        None = 0,
        Credits = 1,
        Points = 2
    }

    public enum PointsDepositType
    {
        Admin = 0,
        Registration = 1,
        IdentityValidation = 2,
        Login = 3,
        Game = 4,
        Invite = 5,
        PointPurchase = 6,
        LuckyDrawMysteryBoxPrize = 7,
    }

    public enum EpinTransactionResult
    {
        Error = 0,
        AlreadyRedeemed = 1,
        Retired = 2,
        InvalidApplication = 3,
        InvalidPin = 4,
        Ok = 5,
        InvalidData = 6
    }

    public enum BundleCategoryType
    {
        None = 0,
        Starter = 1,
        Pro = 2,
        Elite = 3,
        Login = 4,
        Signup = 5,
    }

    public enum BundleOperationResult
    {
        Error = 0,
        Ok = 1,
        DuplicateUniqueId = 2,
        InvalidComposition = 3
    }

    #endregion Shop

    #region Members

    public enum MemberAuthenticationResult
    {
        Ok = 0,
        InvalidData = 1,
        InvalidName = 2,
        InvalidEmail = 3,
        InvalidPassword = 4,
        IsBanned = 5,
        InvalidHandle = 6,
        InvalidEsns = 7,
        InvalidCookie = 8,
        IsIpBanned = 9,
    }

    public enum MemberRegistrationResult
    {
        Ok = 0,
        InvalidEmail = 1,
        InvalidName = 2,
        InvalidPassword = 3,
        DuplicateEmail = 4,
        DuplicateName = 5,
        DuplicateEmailName = 6,
        InvalidData = 7,
        InvalidHandle = 8,
        DuplicateHandle = 9,
        InvalidEsns = 10,
        MemberNotFound = 11,
        OffensiveName = 12,
        IsIpBanned = 13,
        EmailAlreadyLinkedToActualEsns = 14
    }

    [Serializable]
    public enum MemberOperationResult
    {
        Ok = 0,
        DuplicateEmail = 2,
        DuplicateName = 3,
        DuplicateHandle = 4,
        DuplicateEmailName = 5,
        MemberNotFound = 6,
        InvalidData = 9,
        InvalidHandle = 10,
        InvalidEsns = 11,
        InvalidCmid = 12,
        InvalidName = 13,
        InvalidEmail = 14,
        InvalidPassword = 15,
        OffensiveName = 16,
        NameChangeNotInInventory = 17
    }

    public class AccountCompletionResult
    {
        public const int InvalidData = 0;
        public const int Ok = 1;
        public const int DuplicateName = 2;
        /// <summary>
        /// The user already picked up a name
        /// </summary>
        public const int AlreadyCompletedAccount = 3;
        public const int InvalidName = 4;
        public const int IsIpBanned = 5;
    }

    public enum MemberMergeResult
    {
        Ok = 0,
        CmidNotFound = 1,
        CmidAlreadyLinkedToEsns = 3,
        EsnsAlreadyLinkedToCmid = 4,
        InvalidData = 5,
    }

    public enum MergePointsMode
    {
        Add = 0, // Add the points of the current and the previous accounts
        Penalize = 1, // Add only a percentage of the points of the previous account to the new one
        Ignore = 2, // Keep only the points of the current account
    }

    public enum EsnsType
    {
        None = 0,
        WindowsLive = 1,
        LinkedIn = 2,
        Gmail = 3,
        Aol = 4,
        Yahoo = 5,
        Facebook = 6,
        MySpace = 7,
        Cyworld = 8,
        Kongregate = 9
    }

    public enum Id3Type
    {
        RealName = 1,
        Nickname = 2,
        Anonymous = 3
    }

    public enum BanMode
    {
        No = 0,
        Temporary = 1,
        Permanent = 2,
    }

    public enum MemberAccessLevel
    {
        Default = 0,
        ChatModerator = 2,
        JuniorModerator = 4,
        SeniorModerator = 7,
        Admin = 10,
    }

    #region Group

    public class GroupOperationResult
    {
        public const int Ok = 0;
        public const int InvalidName = 1;
        public const int AlreadyMemberOfAGroup = 2;
        public const int DuplicateName = 3;
        public const int InvalidTag = 4;
        public const int MemberNotFound = 5;
        public const int GroupNotFound = 6;
        public const int GroupFull = 7;
        public const int InvalidMotto = 8;
        public const int InvalidDescription = 9;
        public const int DuplicateTag = 10;
        public const int OffensiveName = 13;
        public const int OffensiveTag = 14;
        public const int OffensiveMotto = 15;
        public const int OffensiveDescription = 16;
        public const int IsNotOwner = 17;
        public const int NotEnoughRight = 18;
        public const int IsOwner = 19;
        public const int RequestNotFound = 20;
        public const int ExistingMemberRequest = 21;
        public const int InvitationNotFound = 23;
        public const int AlreadyInvited = 24;
    }

    public enum GroupType
    {
        Clan = 0,
        Work = 1,
        Country = 2,
        School = 3
    }

    public enum GroupPosition
    {
        Leader = 0,
        Member = 2,
        Officer = 6
    }

    public enum GroupFontStyle
    {
        Normal = 0,
        Bold = 1,
        Underline = 2,
        Italic = 3
    }

    public enum GroupColor
    {
        Black = 0,
        Red = 1,
        Gold = 2,
        Green = 3,
        Blue = 4,
        Brown = 5,
        Purple = 6,
        Pink = 7
    }

    public enum GroupRequestStatus
    {
        Pending = 0,
        OnHold = 3
    }

    #endregion Group

    #region Relationship

    public enum ContactRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Refused = 2,
    }

    public enum ContactGroupOperationResult
    {
        InvalidName = 1,
        DuplicateName = 2,
        Ok = 3,
    }

    #endregion Relationship

    #endregion Members

    #region Moderation

    public enum MemberReportType
    {
        OffensiveChat = 0,
        Spamming = 1,
        OffensiveName = 2,
        Cheating = 3,
    }

    public enum MemberReportStatus
    {
        New = 0,
        Rejected = 1,
        Approved = 2,
    }

    public enum ModerationActionType
    {
        AccountPermanentBan = 0,
        AccountTemporaryBan = 1,
        ChatPermanentBan = 2,
        ChatTemporaryBan = 3,
        Warning = 4,
        Note = 5,
        AccountNameChange = 6,
        InvalidNameChange = 7,
        ItemExchange = 8,
        Refund = 9,
        RescueFromAccountStealing = 10,
        IpBan = 11,
        AccountEmailChange = 12
    }

    #endregion Moderation

    #region Notifications

    public enum EmailNotificationType
    {
        DeleteMember = 1,
        BanMemberPermanent = 2,
        MergeMembers = 3,
        ChangeMemberName = 8,
        ChangeMemberPassword = 9,
        ChangeMemberEmail = 11,
        BanMemberTemporary = 12,
        UnbanMember = 13,
        BanMemberChatPermanent = 14,
        BanMemberChatTemporary = 15,
        UnbanMemberChat = 16,
        ChangeClanTag = 17,
        ChangeClanName = 18,
        ChangeClanMotto = 19
    }

    #endregion Notifications

    #region Application versions management

    public enum PhotonGroupOperationResult
    {
        Ok = 0,
        InvalidName = 1,
        DuplicateName = 2,
        GroupNotFound = 3,
        UnknownError = 4
    }

    public enum BuildType
    {
        Prod = 0,
        Dev = 1,
        Staging = 2
    }

    public enum ChannelType
    {
        WebPortal = 0,
        WebFacebook = 1,
        OSXDashboard = 2,
        WebMySpace = 3,
        WindowsStandalone = 4,
        MacAppStore = 5,
        WebCyworld = 6,
        OSXStandalone = 7,
        IPhone = 8,
        IPad = 9,
        Android = 10,
        Kongregate = 11,
    }

    public enum EmbedType
    {
        None = 0,
        WeGame = 1
    }

    public enum RegionType
    {
        UsEast = 0,
        EuWest = 1,
        AsiaPacific = 2,
        UsWest = 3,
        SouthKorea = 4,
        Japan = 5,
    }

    public enum PhotonUsageType
    {
        None = 0,
        All = 1,
        Mobile = 2,
        CommServer = 6,
    }

    public enum IssueType
    {
        PlaySpanJsLoadingFail = 0,
        GetUnityJsFunctionFail = 1
    }

    #endregion Application versions management

    public enum IdentityValidationType
    {
        Email = 1,
    }

    public static class LocaleIdentifier
    {
        public const string EnUS = "en-US";
        public const string KoKR = "ko-KR";
    }

    public enum ApplicationRegistrationResult
    {
        Ok = 0,
        DuplicateHashCode = 1,
        InvalidApplication = 2,
        InvalidHash = 3,
    }

    #region Email Marketing

    public enum EmailAddressStatus
    {
        Unverified = 0,
        Verified = 1,
        Invalid = 2,
    }

    public enum MarketingSubscriptionStatus
    {
        Unsubscribed = 0,
        BasicSubscription = 1,
    }

    #endregion

    public enum PrizeElementType
    {
        Item,
        Credit,
        Point,
        LuckyDrawSet
    }
}