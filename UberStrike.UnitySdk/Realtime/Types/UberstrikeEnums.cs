
using Cmune.Realtime.Common;
using System;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Common
{
    public enum AchievementType
    {
        None = 0,
        MostValuable = 1,       //best k/d (i.e :   – 1.6 kills per death)
        MostAggressive = 2,     //highest killcount (i.e : Pwnst4r – 12 frags)
        SharpestShooter =3,     //highest Headshots + Nutshots count (i.e : Pwnst4r – 15 critical strikes)
        TriggerHappy = 4,       //Highest Kill count per life per game (i.e : Pwnst4r – 5 in-a-row)
        HardestHitter = 5,      //Highest DMG overall (i.e : Pwnst4r – 1523 hp dealt)
        CostEffective = 6,      //Most accurate (i.e : Pwnst4r – 78% accuracy)
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    [Flags]
    public enum BodyPart
    {
        Body = BIT_FLAGS.BIT_01,
        Head = BIT_FLAGS.BIT_02,
        Nuts = BIT_FLAGS.BIT_03,
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum FireMode
    {
        Primary = 0,
        Alternative = 1,
        Secondary = 2
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum PickupItemType
    {
        Armor,
        Health,
        Weapon,
        Ammo,
        Coin,
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum SurfaceType
    {
        Stone,
        Sand,
        Grass,
        Metal,
        Wood,
        Water,
    }

    /// <summary>
    /// USHORT (16 states)
    /// </summary>
    [Flags]
    public enum PlayerStates //: ushort
    {
        IDLE = BIT_FLAGS.BIT_NONE,
        GROUNDED = BIT_FLAGS.BIT_01,
        JUMPING = BIT_FLAGS.BIT_02,
        //SWIMMING = BIT_FLAGS.BIT_03,
        FLYING = BIT_FLAGS.BIT_04,
        DUCKED = BIT_FLAGS.BIT_05,
        DEAD = BIT_FLAGS.BIT_06,
        PAUSED = BIT_FLAGS.BIT_07,
        SPECTATOR = BIT_FLAGS.BIT_08,
        WADING = BIT_FLAGS.BIT_09,
        SWIMMING = BIT_FLAGS.BIT_10,
        DIVING = BIT_FLAGS.BIT_11,
    }

    public enum PlayerLevel
    {
        N00b = 0,
        Surviver = 1,
        IslandHopper = 2,
        PaintHunter = 3,
        SplatCaptain = 4,
    }

    public enum PlayerRank
    {
        None = 0,
        King = 1,
        Second = 2,
        Third = 3,
        Top10 = 4,
        Top20 = 5,
        Top50 = 6,
        Top100 = 7,
        Rest = 8,
    }

    public static class Captions
    {
        private static readonly string[] _gameModes = 
        { 
            "Free for All", 
            "Team VS Team", 
            "Capture The Flag", 
            "Just For Fun" ,
            "Cooperation" ,
            "Elimination" ,
            "Last Man Standing"
        };

        public static string GetGameMode(short mode)
        {
            switch (mode)
            {
                case GameModeID.DeathMatch: return _gameModes[0];
                case GameModeID.TeamDeathMatch: return _gameModes[1];
                case GameModeID.CaptureTheFlag: return _gameModes[2];
                case GameModeID.FunMode: return _gameModes[3];
                case GameModeID.CooperationMode: return _gameModes[4];
                case GameModeID.EliminationMode: return _gameModes[5];
                case GameModeID.LastManStanding: return _gameModes[6];
                default: return "<Unknown GameMode>";
            }
        }
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    [Flags]
    public enum KeyState
    {
        Still = BIT_FLAGS.BIT_NONE,

        Forward = BIT_FLAGS.BIT_01,
        Backward = BIT_FLAGS.BIT_02,

        Left = BIT_FLAGS.BIT_03,
        Right = BIT_FLAGS.BIT_04,

        Jump = BIT_FLAGS.BIT_05,
        Crouch = BIT_FLAGS.BIT_06,

        //Free = BIT_FLAGS.BIT_07,
        //Free = BIT_FLAGS.BIT_08,

        ///// Combinations
        Vertical = Forward | Backward,
        Horizontal = Left | Right,
        Walking = Vertical | Horizontal,
    }

    public enum TeamID
    {
        NONE = 0,
        BLUE = 1,
        RED = 2,
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum ChatMessageType
    {
        System = 0,
        Chat = 1,
        Hit = 2,
        Kill = 3,
        Success = 4,
        Admin = 5
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum BulletType
    {
        BULLET,
        SNIPER,
        ROCKET,
        SHELL,
    }
}