//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Cmune.Realtime.Common;
//using UnityEngine;
//using Cmune.Realtime.Common.Utils;
//using Cmune.Core.Types.Attributes;

//namespace UberStrike.Realtime.Common.DataTypes
//{
//    public class RaidbotInfo : CmuneDeltaSync
//    {
//        public Vector3 Position;
//        public float TimeStamp;

//        //private Quaternion _realRotation;
//        //private bool _hasRealRotation = false;

//        [CMUNESYNC(FieldTag.Weapons)]
//        WeaponInfo _weapons = new WeaponInfo();
//        [CMUNESYNC(FieldTag.FunctionalItems)]
//        List<int> _functionalItems = new List<int>(3);
//        [CMUNESYNC(FieldTag.QuickItems)]
//        List<int> _quickItems = new List<int>(3);

//        [CMUNESYNC(FieldTag.Armor)]
//        ArmorInfo _armor = new ArmorInfo();

//        [CMUNESYNC(FieldTag.IsFiring)]
//        private bool _isFiring = false;

//        [CMUNESYNC(FieldTag.Health)]
//        private short _health;

//        [CMUNESYNC(FieldTag.HorizontalRotation)]
//        private byte _horizontalRotation;

//        //[CMUNESYNC(FieldTag.VerticalRotation)]
//        //private byte _verticalRotation;

//        [CMUNESYNC(FieldTag.OwnerId)]
//        private int _ownerId;

//        [CMUNESYNC(FieldTag.AIState)]
//        private byte _aiState;

//        public short NetworkId
//        {
//            get { return (short)_instanceID; }
//            set { _instanceID = value; }
//        }

//        public short Health
//        {
//            get { return _health; }
//            set { _health = value; }
//        }

//        public int OwnerId
//        {
//            get { return _ownerId; }
//            set { _ownerId = value; }
//        }

//        public byte AIState
//        {
//            get { return _aiState; }
//            set { _aiState = value; }
//        }

//        public bool IsFiring
//        {
//            get { return _isFiring; }
//            set { _isFiring = value; }
//        }

//        public Quaternion HorizontalRotation
//        {
//            get { return Quaternion.Euler(0, Conversion.Byte2Angle(_horizontalRotation), 0); }
//            set { _horizontalRotation = Conversion.Angle2Byte(value.eulerAngles.y); }
//        }

//        public RaidbotInfo()
//        {
//            _health = 100;
//            _aiState = BotState.HUNT;
//        }

//        public RaidbotInfo(SyncObject data)
//        {
//            ReadSyncData(data);
//        }

//        [ExtendableEnumBounds(BIT_FLAGS.BIT_07, BIT_FLAGS.BIT_31)]
//        public new class FieldTag : ActorInfo.FieldTag
//        {
//            public const int IsFiring = BIT_FLAGS.BIT_07;
//            public const int MoveState = BIT_FLAGS.BIT_08;
//            public const int HorizontalRotation = BIT_FLAGS.BIT_09;
//            public const int VerticalRotation = BIT_FLAGS.BIT_10;
//            public const int Direction = BIT_FLAGS.BIT_11;
//            public const int QuickItems = BIT_FLAGS.BIT_12;
//            public const int Velocity = BIT_FLAGS.BIT_13;
//            public const int Splatted = BIT_FLAGS.BIT_14;
//            public const int FunctionalItems = BIT_FLAGS.BIT_15;
//            public const int Health = BIT_FLAGS.BIT_16;
//            public const int Weapons = BIT_FLAGS.BIT_17;
//            public const int Armor = BIT_FLAGS.BIT_18;
//            public const int Gear = BIT_FLAGS.BIT_19;
//            public const int AssetId = BIT_FLAGS.BIT_20;
//            public const int OwnerId = BIT_FLAGS.BIT_21;
//            public const int AIState = BIT_FLAGS.BIT_22;
//        }

//        public class BotState
//        {
//            public const byte HUNT      = 1;
//            public const byte ATTACK    = 2;
//        }
//    }
//}
