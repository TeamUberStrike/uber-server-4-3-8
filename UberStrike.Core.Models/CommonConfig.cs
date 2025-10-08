using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Common.Entities
{
    public struct UberStrikeCommonConfig
    {
        public const int ApplicationId = CommonConfig.ApplicationIdUberstrike;
        /// <summary>
        /// Items that a user gets when creating his loadout
        /// </summary>
        public static readonly List<int> FirstLoadoutItemIds = new List<int> { DefaultMeleeWeapon, DefaultWeapon, DefaultHead, DefaultFace, DefaultGloves, DefaultUpperBody, DefaultLowerBody, DefaultBoots, PrivateerLicenseId };
        /// <summary>
        /// Weapons that a user gets when creating his loadount
        /// </summary>
        public static readonly List<int> FirstLoadoutWeaponItemIds = new List<int> { DefaultMeleeWeapon, DefaultWeapon };
        /// <summary>
        /// Default Loadout when creating a new user
        /// </summary>
        public static readonly LoadoutView DefaultLoadout = new LoadoutView(0, 0, DefaultBoots, 0, DefaultFace, 0, 0, 0, DefaultGloves, DefaultHead, DefaultLowerBody, DefaultMeleeWeapon, 0, 0, 0, DefaultAvatarType, DefaultUpperBody, DefaultWeapon, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, DefaultSkinColor);
        /// <summary>
        /// Default level when creating a new user
        /// </summary>
        public const int DefaultLevel = 1;

        /// <summary>
        /// Default level when creating a new user
        /// </summary>
        public const int MaxPlayers = 16;

        /// <summary>
        /// Level after which the level up is blocked
        /// </summary>
        public const int LevelCap = 40;
        public const int DefaultMeleeWeapon = 1000;
        public const int DefaultWeapon = 1002;
        public const int DefaultHead = 1084;
        public const int DefaultFace = 1085;
        public const int DefaultGloves = 1086;
        public const int DefaultUpperBody = 1087;
        public const int DefaultLowerBody = 1088;
        public const int DefaultBoots = 1089;

        public const string DefaultSkinColor = "c69c6d";
        public const AvatarType DefaultAvatarType = AvatarType.LutzRavinoff;

        /// <summary>
        /// Mininum level for a member that wish to be a clan leader
        /// </summary>
        public const int ClanLeaderMinLevel = 4;
        /// <summary>
        /// Minimum number of contacts for a member that wish to be a clan leader
        /// </summary>
        public const int ClanLeaderMinContactsCount = 1;

        #region Live feeds

        public const int LiveFeedCriticalPriority = 0;
        public const int LiveFeedImportantPriority = 1;
        public const int LiveFeedNormalPriority = 2;
        public static readonly Dictionary<int, string> LiveFeedPriorityNames = new Dictionary<int, string> { { LiveFeedCriticalPriority, "Critical" }, { LiveFeedImportantPriority, "Important" }, { LiveFeedNormalPriority, "Normal" } };
        /// <summary>
        /// The min length is included
        /// </summary>
        public const int LiveFeedDescriptionMinLength = 1;
        /// <summary>
        /// The max length is included
        /// </summary>
        public const int LiveFeedDescriptionMaxLength = 140;
        /// <summary>
        /// The max length is included
        /// </summary>
        public const int LiveFeedUrlMaxLength = 255;

        #endregion

        #region Maps

        /// <summary>
        /// The min length is included
        /// </summary>
        public const int MapDisplayNameMinLength = 1;
        /// <summary>
        /// The max length is included
        /// </summary>
        public const int MapDisplayNameMaxLength = 20;
        /// <summary>
        /// The min length is included
        /// </summary>
        public const int MapDescriptionMinLength = 1;
        /// <summary>
        /// The max length is included
        /// </summary>
        public const int MapDescriptionMaxLength = 500;
        /// <summary>
        /// The min length is included
        /// </summary>
        public const int MapSceneNameMinLength = 1;
        /// <summary>
        /// The max length is included
        /// </summary>
        public const int MapSceneNameMaxLength = 50;

        #endregion

        /// <summary>
        /// This item is owned only by players that created a loadout for the version 4.0 of UberStrike
        /// </summary>
        public const int PrivateerLicenseId = CommonConfig.PrivateerUberStrikeLicenseId;
        /// <summary>
        /// This item is owned only by members that can own a clan in UberStrike
        /// </summary>
        public const int ClanLeaderLicenseId = CommonConfig.ClanLeaderUberStrikeLicenseId;
        /// <summary>
        /// This item is owned only by members that bought UberStrike on the Mac App Store
        /// </summary>
        public const int MacAppStoreLicenseId = CommonConfig.MacAppStoreUberStrikeLicenseId;

        /// <summary>
        /// Xp attributed when a user completes the tutorial. Should be enough to almost reache level 3
        /// </summary>
        public const int XpAttributedOnTutorialCompletion = 80;

        public const string WelcomeMessage = @"
Welcome UberStriker,

To make your gaming experience as enjoyable as possible, we have written out some guidelines that you need to follow while playing our game. 
Keep in mind that these guidelines are here to make sure everyone in the community enjoys the content! 

We hope that you have a pleasant stay!

In-game Rules:
 
Chatting -
• No swearing or inappropriate content.
• No ""Caps lock"" (using it for emphasis is okay).
• No spamming.
• Do not personally attack any person(s).
• No backseat moderating. Please use the report button.
• Do not discuss topics that involve race, color, creed, religion, sex, or politics.

 
General -
• Alternate or ""Second"" Accounts in-game ARE allowed.
• No account sharing! Your account is yours, and if another player is caught using it, all parties will get banned.
• Exploiting of glitches will not be tolerated. Cheating of any kind will result in a permanent ban.
• Be respectful to the Administrators/Moderators/QAs. These people work hard for you, so please show them respect.
• Advertising of any content unrelated to UberStrike is not permitted.
• Please do not try to cleverly circumvent the rules listed here. These rules are general guidelines and are very flexible, and will be enforced.
• Join a server in your area. You will not get banned for lagging, although you may get kicked from the current game.
• Above all, use common sense.
• Have fun:-)
";
    }

    public struct PlayerXPEventViewId
    {
        public const int Splat = 1;
        public const int HeadShot = 2;
        public const int Nutshot = 3;
        /// <summary>
        /// Splat by Melee
        /// </summary>
        public const int Humiliation = 4;
        public const int Damage = 5;
    }

    public struct UberstrikeAppSettings
    {
        public const string PortalCookieName = "PortalCookieName";
        public const string FacebookCookieName = "FacebookCookieName";
        public const string MySpaceCookieName = "MySpaceCookieName";
        public const string KongregateCookieName = "KongregateCookieName";
        public const string LocalhostIP = "LocalhostIP";
        public const string DatabaseDataSourceOverride = "DbAddressMvPP";
        public const string DatabaseDataSource = "appsAPIKey";
    }

    public struct UberStrikeCacheKeys
    {
        public const string ShopView = "UberstrikeItemShopView";
        public const string ShopFunctionalConfigParameters = "UberstrikeFunctionalConfig";
        public const string ShopGearConfigParameters = "UberstrikeGearConfig";
        public const string ShopQuickUseConfigParameters = "UberStrikeQuickUseConfig";
        public const string ShopSpecialConfigParameters = "UberStrikeSpecialConfig";
        public const string ShopWeaponModConfigParameters = "UberStrikeWeaponModConfig";
        public const string ShopWeaponConfigParameters = "UberStrikeShopWeaponConfig";
        public const string CheckDeprecatedApplicationVersionViewParameters = "UberStrikeDeprecatedCheckApplicationVersionView";
        public const string GetDeprecatedPhotonServersViewParameters = "UberStrikeDeprecatedPhotonServersView";
        public const string CheckApplicationVersionViewParameters = "UberStrikeCheckApplicationVersionView";
        public const string GetPhotonServersViewParameters = "UberStrikePhotonServersView";
        public const string XPEvents = "UberStrikeXPEvents";
        public const string LevelCaps = "UberStrikeLevelCaps";
        public const string ReferrerSource = "UberStrikeReferrerSource";
        public const string IsFacebookComingFromSixWavesParameters = "UberStrikeFacebookFromSixWaves";
        public const string Bundles = "UberstrikeBundles";
        public const string GetLiveFeed = "UberstrikeLiveFeedView";
    }
}