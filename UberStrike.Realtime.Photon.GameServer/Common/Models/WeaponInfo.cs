using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Common
{
    public class WeaponInfo : IByteArray
    {
        public WeaponInfo()
        {
            _itemIDs.AddRange(new int[] { 0, 0, 0, 0, 0 });
            _categories.AddRange(new byte[] { 0, 0, 0, 0, 0 });
        }

        public WeaponInfo(byte[] bytes, ref int index)
        {
            index = FromBytes(bytes, index);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(25);
            DefaultByteConverter.FromIntCollection(_itemIDs, ref bytes);
            DefaultByteConverter.FromByteCollection(_categories, ref bytes);
            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            _itemIDs = DefaultByteConverter.ToIntCollection(bytes, ref idx);
            _categories = DefaultByteConverter.ToByteCollection(bytes, ref idx);
            return idx;
        }

        public void ResetWeaponSlot(WeaponInfo.SlotType slot)
        {
            ItemIDs[(int)slot] = 0;
            Categories[(int)slot] = 0;
        }

        public void SetWeaponSlot(WeaponInfo.SlotType slot, int itemId, UberstrikeItemClass type)
        {
            ItemIDs[(int)slot] = itemId;
            Categories[(int)slot] = (byte)(int)type;
        }

        public string ItemIDsToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}", _itemIDs[0], _itemIDs[1], _itemIDs[2], _itemIDs[3], _itemIDs[4]);
        }

        public string CategoriesToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}", _categories[0], _categories[1], _categories[2], _categories[3], _categories[4]);
        }

        public override string ToString()
        {
            return ItemIDsToString();
        }

        public override int GetHashCode()
        {
            return _itemIDs[0] ^ _itemIDs[1] ^ _itemIDs[2] ^ _itemIDs[3] ^ _itemIDs[4];
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                return this.GetHashCode() == obj.GetHashCode();
            }
            else { return false; }
        }

        #region PROPERTIES
        public List<int> ItemIDs
        {
            get { return _itemIDs; }
        }
        public List<byte> Categories
        {
            get { return _categories; }
        }
        #endregion

        #region FIELDS
        private List<int> _itemIDs = new List<int>(5);
        private List<byte> _categories = new List<byte>(5);
        #endregion

        public enum SlotType
        {
            Melee = 0,
            Primary = 1,
            Secondary = 2,
            Tertiary = 3,
            Pickup = 4,
        }
    }
}
