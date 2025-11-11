
using System;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.Types
{
    public enum LocaleType
    {
        en_US,
        ko_KR,
        tr_TR,
        fr_FR,
        it_IT,
        de_DE,
    }

    public enum ItemShopHighlightType
    {
        None = 0,
        Featured = 1,
        Popular = 2,
        New = 3,
    }

    public enum GameModeType
    {
        None = 0,
        DeathMatch = 1,
        TeamDeathMatch = 2,
        EliminationMode = 3,
    }

    [Flags]
    public enum GameModeFlag
    {
        None = 0,
        All = ~0,
        DeathMatch = 0x1,
        TeamDeathMatch = 0x2,
        EliminationMode = 0x4,
    }

    public enum ChannelElement
    {
        Banner = 1,
        RightPromotion = 2
    }

    public enum UserInstallStepType
    {
        InvalidWsCall = 0,
        NoUnity = 1,
        ClickDownload = 2,
        UnityInstalled = 3,
        FullGameLoaded = 4,
        ClickCancel = 5,
        UnityInitialized = 6,
        AccountCreated = 7,
        HasUnity = 8
    }

    public enum TutorialStepType
    {
        MouseLook = 1,
        KeyboardMove = 2,
        WalkToArmory = 3,
        PickUpWeapon = 4,
        ShootFirstGroup = 5,
        ShootSecondGroup = 6,
        TutorialComplete = 7,
        NameSelection = 8,
        TutorialStart = 9,
    }

    #region Members

    public enum AvatarType
    {
        LutzRavinoff = 0,
        JuliaEnzo = 1,
        MorgenRavinoff = 2,
        DanaHoyt = 3,
        HumeZombie = 4,
        TechZombie = 5,
        JuliaNinja = 6
    }

    public class UberStrikeAccountCompletionResult : AccountCompletionResult
    {

    }

    #endregion Members

    public enum UberStrikeEmailNotificationType
    {
        ItemAttributed = 1,
    }

    #region Shop

    public enum UberstrikeItemType
    {
        Weapon = 1,
        WeaponMod = 2,
        Gear = 3,
        QuickUse = 4,
        Functional = 5,
        Special = 6,
    }

    public enum UberstrikeItemClass
    {
        WeaponMelee = 1,
        WeaponHandgun = 2,
        WeaponMachinegun = 3,
        WeaponShotgun = 4,
        WeaponSniperRifle = 5,
        WeaponCannon = 6,
        WeaponSplattergun = 7,
        WeaponLauncher = 8,
        WeaponModScope = 9,
        WeaponModMuzzle = 10,
        WeaponModWeaponMod = 11,
        GearBoots = 12,
        GearHead = 13,
        GearFace = 14,
        GearUpperBody = 15,
        GearLowerBody = 16,
        GearGloves = 17,
        QuickUseGeneral = 18,
        QuickUseGrenade = 19,
        QuickUseMine = 20,
        FunctionalGeneral = 21,
        SpecialGeneral = 22,
        GearHolo = 23
    }

    public enum QuickItemLogic
    {
        None = 0,
        SpringGrenade = 1,
        HealthPack = 2,
        ArmorPack = 3,
        AmmoPack = 4,
        ExplosiveGrenade = 5,
    }

    public class UberstrikeBuyItemResult : BuyItemResult
    {
        public const int InvalidLevel = 100;
    }

    #endregion Shop

    #region Groups

    public class UberStrikeGroupOperationResult : GroupOperationResult
    {
        public const int InvalidLevel = 100;
        public const int InvalidContactsCount = 101;
        public const int ClanLicenceNotFound = 102;
    }

    #endregion Groups

    #region Maps

    public enum MapOperationResult
    {
        Error = 0,
        InvalidDisplayName = 1,
        DuplicateDisplayName = 2,
        InvalidDescription = 3,
        InvalidSceneName = 4,
        DuplicateSceneName = 5,
        Ok = 6,
        DuplicateMapId = 7,
        InvalidMapId = 8,
        InvalidApplicationVersion = 9,
        DuplicateMapIdDisplayNameSceneName = 10,
        DuplicateMapIdDisplayName = 11,
        DuplicateMapIdSceneName = 12,
        DuplicateDisplayNameSceneName = 13,
        NotFound = 14,
        DuplicateApplicationVersion = 15,
    }

    public enum MapType
    {
        None = 0,
        StandardDefinition = 1,
        HighDefinition = 2,
    }

    #endregion

    public enum WeeklySpecialOperationResult
    {
        Error = 0,
        InvalidTitle = 1,
        InvalidText = 2,
        InvalidImageUrl = 3,
        Ok = 4,
        InvalidItemId = 5,
        ExistingWeeklySpecial = 6,
        NonExistingWeeklySpecial = 7,
    }

    public sealed class BIT_FLAGS
    {
        public const int BIT_NONE = 0x00000000;
        public const int BIT_ALL = ~BIT_NONE;

        public const int BIT_01 = 0x00000001;
        public const int BIT_02 = 0x00000002;
        public const int BIT_03 = 0x00000004;
        public const int BIT_04 = 0x00000008;

        public const int BIT_05 = 0x00000010;
        public const int BIT_06 = 0x00000020;
        public const int BIT_07 = 0x00000040;
        public const int BIT_08 = 0x00000080;

        public const int BIT_09 = 0x00000100;
        public const int BIT_10 = 0x00000200;
        public const int BIT_11 = 0x00000400;
        public const int BIT_12 = 0x00000800;

        public const int BIT_13 = 0x00001000;
        public const int BIT_14 = 0x00002000;
        public const int BIT_15 = 0x00004000;
        public const int BIT_16 = 0x00008000;

        public const int BIT_17 = 0x00010000;
        public const int BIT_18 = 0x00020000;
        public const int BIT_19 = 0x00040000;
        public const int BIT_20 = 0x00080000;

        public const int BIT_21 = 0x00100000;
        public const int BIT_22 = 0x00200000;
        public const int BIT_23 = 0x00400000;
        public const int BIT_24 = 0x00800000;

        public const int BIT_25 = 0x01000000;
        public const int BIT_26 = 0x02000000;
        public const int BIT_27 = 0x04000000;
        public const int BIT_28 = 0x08000000;

        public const int BIT_29 = 0x10000000;
        public const int BIT_30 = 0x20000000;
        public const int BIT_31 = 0x40000000;
        //BIT_32 = 0x80000000, //only when using uint as sign is encoded in last bit
    }

    [Flags]
    public enum ModerationTag
    {
        None = BIT_FLAGS.BIT_NONE,
        Muted = BIT_FLAGS.BIT_01,
        Ghosted = BIT_FLAGS.BIT_02,
        Banned = BIT_FLAGS.BIT_03,

        Speedhacking = BIT_FLAGS.BIT_04,
        Spamming = BIT_FLAGS.BIT_05,
        Language = BIT_FLAGS.BIT_06,
        Name = BIT_FLAGS.BIT_07,
    }
}