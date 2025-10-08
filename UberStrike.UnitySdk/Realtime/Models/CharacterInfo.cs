
using System.Collections.Generic;
using System.Text;
using Cmune.Core.Types.Attributes;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using UberStrike.DataCenter.Common.Entities;
using UnityEngine;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Common
{
    /// <summary>
    /// Serialization Size: 62 byte + x
    /// </summary>
    public class CharacterInfo : ActorInfo
    {
        public CharacterInfo(SyncObject data)
            : this(string.Empty, 0, ChannelType.WebPortal)
        {
            ReadSyncData(data);
        }

        public CharacterInfo()
            : this(string.Empty, 0, ChannelType.WebPortal)
        { }

        public CharacterInfo(string name, int actorId, ChannelType channel)
            : base(name, actorId, channel)
        {
            _gear.AddRange(new int[] { 0, 0, 0, 0, 0, 0, 0 });
            _functionalItems.AddRange(new int[] { 0, 0, 0 });
            _quickItems.AddRange(new int[] { 0, 0, 0 });
        }

        public void ResetScore()
        {
            _stats.Kills = 0;
            _stats.Deaths = 0;
            _stats.XP = 0;
            _stats.Points = 0;
        }

        public void ResetState()
        {
            //next update will include all defined values
            Cache.Clear();

            _keys = 0;
            _moveState = 0;
            _horizontalRotation = 0;
            _verticalRotation = 0;
            _health = 100;
            _currentWeaponSlot = 1;
            _currentFiringMode = 0;
            _isFiring = false;
            _isReadyForGame = false;
            Armor.Reset();
            Weapons.ResetWeaponSlot(WeaponInfo.SlotType.Pickup);
        }

        #region MOVESTATE

        public bool Is(PlayerStates state)
        {
            return (PlayerState & state) == state;
        }

        public void Set(PlayerStates state)
        {
            PlayerState |= state;
        }

        public void Set(PlayerStates state, bool b)
        {
            if (b) PlayerState |= state; else PlayerState &= ~state;
        }

        public void Unset(PlayerStates state)
        {
            PlayerState &= ~state;
        }

        #endregion

        #region SYNC FIELDS

        //MOVEMENT
        [CMUNESYNC(FieldTag.Keys)]
        private byte _keys;
        [CMUNESYNC(FieldTag.MoveState)]
        private ushort _moveState;
        [CMUNESYNC(FieldTag.HorizontalRotation)]
        private byte _horizontalRotation;
        [CMUNESYNC(FieldTag.VerticalRotation)]
        private byte _verticalRotation;
        [CMUNESYNC(FieldTag.PlayerNumber)]
        private byte _playerNumber;

        //STATISTICS (8 byte) 
        [CMUNESYNC(FieldTag.Health)]
        private short _health;
        [CMUNESYNC(FieldTag.Stats)]
        private StatsInfo _stats = new StatsInfo();
        [CMUNESYNC(FieldTag.Armor)]
        ArmorInfo _armor = new ArmorInfo();

        [CMUNESYNC(FieldTag.Level)]
        private byte _level;
        [CMUNESYNC(FieldTag.TeamID)]
        private byte _teamID;

        [CMUNESYNC(FieldTag.CurrentWeaponSlot)]
        private byte _currentWeaponSlot = 1;
        [CMUNESYNC(FieldTag.CurrentFiringMode)]
        private byte _currentFiringMode;
        [CMUNESYNC(FieldTag.IsFiring)]
        private bool _isFiring = false;
        [CMUNESYNC(FieldTag.SkinColor)]
        private Color _skinColor = Color.white;

        [CMUNESYNC(FieldTag.Weapons)]
        WeaponInfo _weapons = new WeaponInfo();
        [CMUNESYNC(FieldTag.Gear)]
        List<int> _gear = new List<int>(6);
        [CMUNESYNC(FieldTag.FunctionalItems)]
        List<int> _functionalItems = new List<int>(3);
        [CMUNESYNC(FieldTag.QuickItems)]
        List<int> _quickItems = new List<int>(3);
        [CMUNESYNC(FieldTag.ReadyForGame)]
        private bool _isReadyForGame = false;
        [CMUNESYNC(FieldTag.SurfaceSound)]
        private byte _surfaceSound;

        private Quaternion _realRotation;
        private bool _hasRealRotation = false;
        private Vector3 _standingOffset = new Vector3(0, 0.65f, 0);
        private Vector3 _crouchingOffset = new Vector3(0, 0.1f, 0);

        public Vector3 Position;
        public float TimeStamp;

        #endregion

        #region PROPERTIES

        public float KDR
        {
            get { return (Kills > 0 ? (float)Kills : 1f) / (Deaths > 0 ? (float)Deaths : 1f); }
        }

        public StatsInfo Stats
        {
            get { return _stats; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roundTime">Round time in seconds</param>
        /// <returns>Number of Points dropped when Character is killed</returns>
        public int GetPointBonus()
        {
            return 10;
            //if (Level >= 20) return 3;
            //else if (Level >= 10) return 2;
            //else if (Level > 1) return 1;
            //else return 0;
        }

        public byte PlayerNumber
        {
            get { return _playerNumber; }
            set { _playerNumber = value; }
        }

        public Vector3 CurrentOffset
        {
            get { return (Is(PlayerStates.DUCKED) ? _crouchingOffset : _standingOffset); }
        }

        public Vector3 ShootingPoint
        {
            get { return Position + CurrentOffset; }
        }

        public Vector3 ShootingDirection
        {
            get { return Rotation * Vector3.forward; }
        }

        public KeyState Keys
        {
            get { return (KeyState)_keys; }
            set { _keys = (byte)value; }
        }

        public PlayerStates PlayerState
        {
            get { return (PlayerStates)_moveState; }
            set { _moveState = (ushort)value; }
        }

        public Quaternion HorizontalRotation
        {
            get { return Quaternion.Euler(0, Conversion.Byte2Angle(_horizontalRotation), 0); }
            set
            {
                Rotation = value;
                _horizontalRotation = Conversion.Angle2Byte(value.eulerAngles.y);
            }
        }

        public float VerticalRotation
        {
            get { return _verticalRotation / 255f; }
            set
            {
                _verticalRotation = (byte)(Mathf.Clamp01(value) * 255); ;
            }
        }

        public Quaternion Rotation
        {
            private set
            {
                _hasRealRotation = true;
                _realRotation = value;
            }
            get { return _hasRealRotation ? _realRotation : (HorizontalRotation * Quaternion.AngleAxis(VerticalRotation * 180 - 90, Vector3.left)); }
        }

        public TeamID TeamID
        {
            get { return (TeamID)_teamID; }
            set { _teamID = (byte)value; }
        }

        public ushort XP
        {
            get { return _stats.XP; }
            set { _stats.XP = value; }
        }

        public ushort Points
        {
            get { return _stats.Points; }
            set { _stats.Points = value; }
        }

        public short Kills
        {
            get { return _stats.Kills; }
            set { _stats.Kills = value; }
        }

        public short Deaths
        {
            get { return _stats.Deaths; }
            set { _stats.Deaths = value; }
        }

        public int Level
        {
            get { return _level; }
            set { _level = (byte)Mathf.Clamp(value, 0, byte.MaxValue); }
        }

        public short Health
        {
            get { return _health; }
            set
            {
                if (value < -100) _health = -100;
                else if (value > 200) _health = 200;
                else _health = value;
            }
        }

        public bool IsFiring
        {
            get { return _isFiring; }
            set { _isFiring = value; }
        }

        public bool IsUnderWater
        {
            get { return PlayerState == PlayerStates.SWIMMING || PlayerState == PlayerStates.DIVING; }
        }

        public byte CurrentWeaponSlot
        {
            get { return _currentWeaponSlot; }
            set { _currentWeaponSlot = value; }
        }

        public FireMode CurrentFiringMode
        {
            get { return (FireMode)_currentFiringMode; }
            set { _currentFiringMode = (byte)value; }
        }

        public WeaponInfo Weapons
        {
            get { return _weapons; }
            set { _weapons = value; }
        }

        public ArmorInfo Armor
        {
            get { return _armor; }
            set { _armor = value; }
        }

        public Color SkinColor
        {
            get { return _skinColor; }
            set { _skinColor = value; }
        }

        public List<int> Gear
        {
            get { return _gear; }
            set { _gear = value; }
        }

        public List<int> FunctionalItems
        {
            get { return _functionalItems; }
            set { _functionalItems = value; }
        }

        public List<int> QuickItems
        {
            get { return _quickItems; }
            set { _quickItems = value; }
        }

        public bool IsReadyForGame
        {
            get { return _isReadyForGame; }
            set { _isReadyForGame = value; }
        }

        public SurfaceType SurfaceSound
        {
            get { return (SurfaceType)_surfaceSound; }
            set { _surfaceSound = (byte)value; }
        }

        public int CurrentWeaponID
        {
            get { return (int)_weapons.ItemIDs[_currentWeaponSlot]; }
        }

        public UberstrikeItemClass CurrentWeaponCategory
        {
            get { return (UberstrikeItemClass)(int)_weapons.Categories[_currentWeaponSlot]; }
        }

        public bool IsAlive
        {
            get { return _health > 0; }
        }

        public float Velocity { get; set; }
        public float Distance { get; set; }

        public bool IsSpectator
        {
            get { return Is(PlayerStates.SPECTATOR); }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append(base.ToString());
            b.AppendLine("Number: " + _playerNumber);
            b.AppendLine("Keys: " + CmunePrint.Flag(_keys));
            b.AppendLine("State: " + CmunePrint.Flag<PlayerStates>(_moveState));
            b.AppendLine("Health: " + _health);
            b.AppendLine("Armor: " + _armor);
            b.AppendLine("IsFiring: " + _isFiring);
            b.AppendLine("TeamID: " + _teamID);
            b.AppendLine("Weapon: " + _currentWeaponSlot);
            b.AppendLine("Mode: " + _currentFiringMode);
            b.AppendLine("Rotation: " + string.Format("{0}/{1}", _horizontalRotation, _verticalRotation));
            b.AppendLine("Weapons: " + _weapons.CategoriesToString());
            b.AppendLine("Gear: " + CmunePrint.Values(_gear));
            b.AppendLine("Funcs: " + CmunePrint.Values(_functionalItems));
            return b.ToString();
        }

        [ExtendableEnumBounds(BIT_FLAGS.BIT_08, BIT_FLAGS.BIT_31)]
        public new class FieldTag : ActorInfo.FieldTag
        {
            public const int IsFiring = BIT_FLAGS.BIT_08;
            public const int MoveState = BIT_FLAGS.BIT_09;
            public const int HorizontalRotation = BIT_FLAGS.BIT_10;
            public const int VerticalRotation = BIT_FLAGS.BIT_11;
            //public const int Direction = BIT_FLAGS.BIT_12;
            public const int QuickItems = BIT_FLAGS.BIT_13;
            //public const int Velocity = BIT_FLAGS.BIT_14;
            public const int Stats = BIT_FLAGS.BIT_15;
            //public const int Splatted = BIT_FLAGS.BIT_16;
            public const int FunctionalItems = BIT_FLAGS.BIT_17;
            public const int Level = BIT_FLAGS.BIT_18;
            public const int TeamID = BIT_FLAGS.BIT_19;
            public const int SurfaceSound = BIT_FLAGS.BIT_20;
            public const int Keys = BIT_FLAGS.BIT_21;
            public const int Health = BIT_FLAGS.BIT_22;
            public const int SkinColor = BIT_FLAGS.BIT_23;
            public const int Weapons = BIT_FLAGS.BIT_24;
            public const int CurrentWeaponSlot = BIT_FLAGS.BIT_25;
            public const int CurrentFiringMode = BIT_FLAGS.BIT_26;
            public const int Armor = BIT_FLAGS.BIT_27;
            public const int Gear = BIT_FLAGS.BIT_28;
            public const int ReadyForGame = BIT_FLAGS.BIT_29;
            //public const int XP = BIT_FLAGS.BIT_30;
            public const int PlayerNumber = BIT_FLAGS.BIT_31;
        }
    }
}