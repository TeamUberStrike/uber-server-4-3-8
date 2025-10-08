using Cmune.Realtime.Common;
using UnityEngine;

namespace UberStrike.Realtime.Common
{
    public class ArmorInfo : IByteArray
    {
        public ArmorInfo()
        {
            _bytes = new byte[3];
        }

        public ArmorInfo(byte[] bytes, ref int index)
            : this()
        {
            index = FromBytes(bytes, index);
        }

        public short AbsorbDamage(short damage, BodyPart part)
        {
            if (HasArmorPoints)
            {
                //get absorbed amount of damage
                int absorb = Mathf.CeilToInt(_totalDefense * damage);

                //make sure we neve absorb more than we can
                absorb = Mathf.Clamp(absorb, 0, _armorPoints);

                //reduce armor points
                _armorPoints -= (byte)absorb;

                //return unabsorbed damage
                return (short)(damage - absorb);
            }
            else
            {
                return damage;
            }
        }

        public void SimulateAbsorbDamage(short damage, BodyPart part, out short finalDamage, out  byte finalArmorPoints)
        {
            if (HasArmorPoints)
            {
                //get absorbed amount of damage
                int absorb = Mathf.CeilToInt(_totalDefense * damage);

                //make sure we neve absorb more than we can
                absorb = Mathf.Clamp(absorb, 0, _armorPoints);

                //reduce armor points
                finalArmorPoints = (byte)(_armorPoints - absorb);

                //return unabsorbed damage
                finalDamage = (short)(damage - absorb);
            }
            else
            {
                finalArmorPoints = _armorPoints;
                finalDamage = damage;
            }
        }

        public bool HasArmorPoints
        {
            get { return _armorPoints > 0; }
        }

        public bool HasArmor
        {
            get { return _bonusDefense > 0; }
        }

        public void Reset()
        {
            ArmorPoints = ArmorPointCapacity;
        }

        public byte[] GetBytes()
        {
            _bytes[0] = _armorPoints;
            _bytes[1] = _armorPointCapacity;
            _bytes[2] = _bonusDefense;

            return _bytes;
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            _armorPoints = bytes[idx++];
            _armorPointCapacity = bytes[idx++];
            _bonusDefense = bytes[idx++];

            AbsorbtionPercentage = _bonusDefense;

            return idx;
        }

        public override int GetHashCode()
        {
            return _armorPoints ^ _armorPointCapacity ^ _bonusDefense;
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                return this.GetHashCode() == obj.GetHashCode();
            }
            else { return false; }
        }

        public override string ToString()
        {
            return string.Format("{0}/{1} @ {2}%", _armorPoints, _armorPointCapacity, _bonusDefense);
        }

        #region PROPERTIES

        public int ArmorPoints
        {
            get { return _armorPoints; }
            set { _armorPoints = (byte)Mathf.Clamp(value, 0, 200); }
        }

        public int ArmorPointCapacity
        {
            get { return _armorPointCapacity; }
            set { _armorPointCapacity = (byte)Mathf.Clamp(value, 0, 200); }
        }

        public byte AbsorbtionPercentage
        {
            get { return _bonusDefense; }
            // Defense bonus up to 50% from armor
            set
            {
                _bonusDefense = (byte)Mathf.Clamp(value, 0, 50);

                // Figure out the players defense by adding natural and armor 
                _totalDefense = _naturalDefense + (_bonusDefense / 100.0f);
            }
        }

        #endregion

        #region FIELDS
        private const float _naturalDefense = 0.5f;
        private byte _bonusDefense = 0;

        private byte[] _bytes;
        private byte _armorPointCapacity = 0;
        private byte _armorPoints = 0;

        private float _totalDefense;
        #endregion
    }
}
