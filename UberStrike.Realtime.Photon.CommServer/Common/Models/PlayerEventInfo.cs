using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Common.IO;
using Cmune.Util;

namespace UberStrike.Realtime.Common
{
    public class DamageEvent : IByteArray
    {
        public DamageEvent()
        {
            Damage = new Dictionary<byte, byte>(1);
        }

        public static DamageEvent FromBytes(byte[] bytes, ref int idx)
        {
            DamageEvent info = new DamageEvent();
            idx = info.FromBytes(bytes, idx);
            return info;
        }

        public void Clear()
        {
            _bodyPartFlag = 0;
            Damage.Clear();
        }

        public void AddDamage(byte angle, short damage, byte bodyPart, int damageEffectFlag, float damageEffectValue)
        {
            if (Damage.ContainsKey(angle))
                Damage[angle] += (byte)damage;
            else
                Damage[angle] = (byte)damage;

            _bodyPartFlag |= bodyPart;
            _damageEffectFlag = damageEffectFlag;
            _damageEffectValue = damageEffectValue;
        }

        #region FIELDS
        public readonly Dictionary<byte, byte> Damage;
        private byte _bodyPartFlag;

        private int _damageEffectFlag;
        private float _damageEffectValue;
        #endregion

        #region PROPERTIES

        public byte BodyPartFlag { get { return _bodyPartFlag; } }

        public int Count { get { return Damage.Count; } }

        public int DamageEffectFlag
        {
            get { return _damageEffectFlag; }
        }

        public float DamgeEffectValue
        {
            get { return _damageEffectValue; }
        }

        #endregion

        public override int GetHashCode()
        {
            return _bodyPartFlag ^ Damage.Count;
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                DamageEvent other = obj as DamageEvent;
                if (other != null)
                {
                    if (other._bodyPartFlag != this._bodyPartFlag) return false;
                    if (!Comparison.IsEqual(other.Damage.Keys, this.Damage.Keys)) return false;
                    if (!Comparison.IsEqual(other.Damage.Values, this.Damage.Values)) return false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            if (idx + 10 <= bytes.Length)
            {
                _damageEffectFlag = DefaultByteConverter.ToInt(bytes, ref idx);
                _damageEffectValue = DefaultByteConverter.ToFloat(bytes, ref idx);

                _bodyPartFlag = bytes[idx++];
                int len = bytes[idx++];
                if (len > 0 && idx + (2 * len) <= bytes.Length)
                {
                    for (int i = 0; i < len; i++)
                        Damage[bytes[idx + i]] = bytes[idx + len + i];
                }
                return idx + (2 * len);
            }
            else
            {
                return int.MaxValue;
            }
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>((Damage.Count * 2) + 2);

            DefaultByteConverter.FromInt(_damageEffectFlag, ref bytes);
            DefaultByteConverter.FromFloat(_damageEffectValue, ref bytes);

            bytes.Add(_bodyPartFlag);
            bytes.Add((byte)Damage.Keys.Count);
            bytes.AddRange(Damage.Keys);
            bytes.AddRange(Damage.Values);

            return bytes.ToArray();
        }
    }
}
