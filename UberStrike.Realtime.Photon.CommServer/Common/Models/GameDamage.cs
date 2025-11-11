//using System.Collections.Generic;
//using Cmune.Realtime.Common;
//using Cmune.Realtime.Common.IO;
//using UnityEngine;

//namespace UberStrike.Realtime.Common
//{
//    public class GameDamage : IByteArray
//    {
//        public GameDamage(short damage)
//        {
//            this._part = BodyPart.Body;
//            this._damage = damage;
//            this._force = 0;
//            this._playerID = -1;
//            this._team = TeamID.NONE;
//            this._effect = PowerUpEffect.None;
//            this._bullet = BulletType.BULLET;
//            this._direction = Vector3.zero;
//            this._hitpoint = Vector3.zero;
//        }

//        public GameDamage(short damage, Vector3 direction, short force)
//            : this(damage)
//        {
//            Direction = direction.normalized;
//            Force = force;
//        }

//        public GameDamage(short damage, Vector3 direction, short force, Vector3 hitPoint)
//            : this(damage, direction, force)
//        {
//            _hitpoint = hitPoint;
//        }

//        public GameDamage(short damage, int playerID, Vector3 direction, TeamID team, PowerUpEffect effect)
//            : this(damage, direction, 10)
//        {
//            this._playerID = playerID;
//            this._team = team;
//            this._effect = effect;
//        }

//        public GameDamage(short damage, int playerID, Vector3 direction, Vector3 hitPoint, short force, TeamID team, PowerUpEffect effect, BulletType bullet)
//            : this(damage, playerID, direction, team, effect)
//        {
//            Force = force;
//            this._bullet = bullet;
//            this._hitpoint = hitPoint;
//        }

//        public static bool IsForcePower(PowerUpEffect p)
//        {
//            return (byte)p >= 20;
//        }

//        public int GetHits()
//        {
//            return GameDamage.GetHits(Bullet, Damage);
//        }

//        public static int GetBullets(BulletType bullet)
//        {
//            switch (bullet)
//            {
//                case BulletType.BULLET:
//                    return 1;

//                case BulletType.SNIPER:
//                case BulletType.ROCKET:
//                case BulletType.SHELL:
//                    return 5;

//                default: return 1;
//            }
//        }

//        public static int GetHits(BulletType bullet, short damage)
//        {
//            switch (bullet)
//            {
//                case BulletType.BULLET: return 1;

//                case BulletType.SNIPER: return 5;

//                case BulletType.ROCKET:
//                    return Mathf.RoundToInt(5 * damage / 40f);

//                case BulletType.SHELL:
//                    return Mathf.RoundToInt(5 * damage / 100f);

//                default: return 0;
//            }
//        }

//        public override string ToString()
//        {
//            return string.Format("A {0} on {1} with {2} Damage and {3} Power, by Player {4}", Bullet, Part, Damage, Effect, ActorId);
//        }

//        #region IByteArray Members

//        public GameDamage(byte[] bytes, ref int idx)
//        {
//            idx = FromBytes(bytes, idx);
//        }

//        public int FromBytes(byte[] bytes, int idx)
//        {
//            _playerID = DefaultByteConverter.ToInt(bytes, ref idx);
//            _damage = DefaultByteConverter.ToShort(bytes, ref idx);
//            _force = DefaultByteConverter.ToShort(bytes, ref idx);
//            _team = (TeamID)(int)DefaultByteConverter.ToByte(bytes, ref idx);
//            _direction = DefaultByteConverter.ToVector3(bytes, ref idx);
//            _part = (BodyPart)(int)DefaultByteConverter.ToByte(bytes, ref idx);
//            _bullet = (BulletType)(int)DefaultByteConverter.ToByte(bytes, ref idx);
//            _effect = (PowerUpEffect)(int)DefaultByteConverter.ToByte(bytes, ref idx);
//            return idx;
//        }

//        public byte[] GetBytes()
//        {
//            List<byte> bytes = new List<byte>();
//            DefaultByteConverter.FromInt(_playerID, ref bytes);
//            DefaultByteConverter.FromShort(_damage, ref bytes);
//            DefaultByteConverter.FromShort(_force, ref bytes);
//            DefaultByteConverter.FromByte((byte)_team, ref bytes);
//            DefaultByteConverter.FromVector3(_direction, ref bytes);
//            DefaultByteConverter.FromByte((byte)_part, ref bytes);
//            DefaultByteConverter.FromByte((byte)_bullet, ref bytes);
//            DefaultByteConverter.FromByte((byte)_effect, ref bytes);
//            return bytes.ToArray();
//        }

//        #endregion

//        public override bool Equals(object obj)
//        {
//            if (obj is GameDamage)
//            {
//                GameDamage other = obj as GameDamage;
//                return this.Damage == other.Damage && this.ActorId == other.ActorId;
//            }
//            else return false;
//        }

//        public override int GetHashCode()
//        {
//            return base.GetHashCode();
//        }

//        #region PROPERTIES
//        public int ActorId { get { return _playerID; } }
//        public PowerUpEffect Effect { get { return _effect; } }
//        public BulletType Bullet { get { return _bullet; } }
//        public TeamID Team { get { return _team; } }
//        public short Damage
//        {
//            get { return _damage; }
//            set { _damage = value; }
//        }
//        public short Deflected
//        {
//            get { return _deflected; }
//            set { _deflected = value; }
//        }
//        public short Absorbed
//        {
//            get { return _absorbed; }
//            set { _absorbed = value; }
//        }
//        public BodyPart Part
//        {
//            get { return _part; }
//            set { _part = value; }
//        }
//        public Vector3 HitPoint
//        {
//            get { return _hitpoint; }
//        }

//        public Vector3 PhysicalForce
//        {
//            get { return Force * Direction; }
//        }
//        public short Force
//        {
//            get { return _force; }
//            private set { _force = value; }
//        }
//        public Vector3 Direction
//        {
//            get { return _direction; }
//            private set { _direction = value; }
//        }
//        public short NettoDamage
//        {
//            get { return (short)(Damage - Deflected - Absorbed); }
//        }
//        public short DeflectedDamageOnly
//        {
//            get { return (short)(Damage - Deflected); }
//        }
//        #endregion

//        #region FIELDS
//        private short _absorbed = 0;
//        private short _deflected = 0;
//        private short _damage = 0;
//        private TeamID _team = 0;
//        private BulletType _bullet = BulletType.BULLET;
//        private PowerUpEffect _effect = PowerUpEffect.None;
//        private int _playerID = -1;
//        private Vector3 _direction = Vector3.zero;
//        private Vector3 _hitpoint = Vector3.zero;

//        public short _force = 0;
//        public bool IsAbsorbed = false;
//        public BodyPart _part = BodyPart.Body;
//        public bool SelfDamage = false;
//        #endregion
//    }
//}