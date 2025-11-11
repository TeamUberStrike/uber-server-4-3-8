using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using System;

namespace UberStrike.Realtime.Common
{
    [System.Serializable]
    public class StatsCollection : IByteArray
    {
        public StatsCollection(byte[] bytes, ref int idx)
            : this()
        {
            idx = FromBytes(bytes, idx);
        }
        public StatsCollection()
        {
            _allValues = new List<PropertyInfo>();
            PropertyInfo[] all = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo i in all)
            {
                if (i.PropertyType == typeof(int) && i.CanRead && i.CanWrite)
                {
                    _allValues.Add(i);
                }
            }
        }

        public void Reset()
        {
            foreach (PropertyInfo i in _allValues)
            {
                i.SetValue(this, 0, null);
            }
        }

        public void TakeBestValues(StatsCollection that)
        {
            foreach (PropertyInfo i in _allValues)
            {
                int thisValue = (int)i.GetValue(this, null);
                int thatValue = (int)i.GetValue(that, null);

                if (thisValue < thatValue)
                    i.SetValue(this, thatValue, null);
            }
        }

        public void AddAllValues(StatsCollection that)
        {
            foreach (PropertyInfo i in _allValues)
            {
                int thisValue = (int)i.GetValue(this, null);
                int thatValue = (int)i.GetValue(that, null);
                i.SetValue(this, thisValue + thatValue, null);
            }
        }

        public int GetKills()
        {
            return MeleeKills + HandgunKills + MachineGunKills + ShotgunSplats + SniperKills + SplattergunKills + CannonKills + LauncherKills - Suicides;
        }

        public int GetShots()
        {
            return MeleeShotsFired + HandgunShotsFired + MachineGunShotsFired + ShotgunShotsFired + SniperShotsFired + SplattergunShotsFired + CannonShotsFired + LauncherShotsFired;
        }

        public int GetHits()
        {
            return MeleeShotsHit + HandgunShotsHit + MachineGunShotsHit + ShotgunShotsHit + SniperShotsHit + SplattergunShotsHit + CannonShotsHit + LauncherShotsHit;
        }

        public int GetDamageDealt()
        {
            return MeleeDamageDone + HandgunDamageDone + MachineGunDamageDone + ShotgunDamageDone + SniperDamageDone + SplattergunDamageDone + CannonDamageDone + LauncherDamageDone;
        }

        public float GetKdr()
        {
            return Math.Max(GetKills(), 0) / Math.Max(Deaths, 1f);
        }

        public float GetAccuracy()
        {
            return GetHits() / Math.Max(GetShots(), 1f);
        }

        public int Headshots { get; set; }
        public int Nutshots { get; set; }
        public int ConsecutiveSnipes { get; set; }
        public int Xp { get; set; }

        public int Deaths { get; set; }

        public int DamageReceived { get; set; }
        public int ArmorPickedUp { get; set; }
        public int HealthPickedUp { get; set; }

        public int MeleeKills { get; set; }
        public int MeleeShotsFired { get; set; }
        public int MeleeShotsHit { get; set; }
        public int MeleeDamageDone { get; set; }

        public int HandgunKills { get; set; }
        public int HandgunShotsFired { get; set; }
        public int HandgunShotsHit { get; set; }
        public int HandgunDamageDone { get; set; }

        public int MachineGunKills { get; set; }
        public int MachineGunShotsFired { get; set; }
        public int MachineGunShotsHit { get; set; }
        public int MachineGunDamageDone { get; set; }

        public int ShotgunSplats { get; set; }
        public int ShotgunShotsFired { get; set; }
        public int ShotgunShotsHit { get; set; }
        public int ShotgunDamageDone { get; set; }

        public int SniperKills { get; set; }
        public int SniperShotsFired { get; set; }
        public int SniperShotsHit { get; set; }
        public int SniperDamageDone { get; set; }

        public int SplattergunKills { get; set; }
        public int SplattergunShotsFired { get; set; }
        public int SplattergunShotsHit { get; set; }
        public int SplattergunDamageDone { get; set; }

        public int CannonKills { get; set; }
        public int CannonShotsFired { get; set; }
        public int CannonShotsHit { get; set; }
        public int CannonDamageDone { get; set; }

        public int LauncherKills { get; set; }
        public int LauncherShotsFired { get; set; }
        public int LauncherShotsHit { get; set; }
        public int LauncherDamageDone { get; set; }

        public int Suicides { get; set; }

        public int Points { get; set; }

        private List<PropertyInfo> _allValues;

        #region IByteArray Members

        public int FromBytes(byte[] bytes, int idx)
        {
            Headshots = DefaultByteConverter.ToInt(bytes, ref idx);
            Nutshots = DefaultByteConverter.ToInt(bytes, ref idx);
            ConsecutiveSnipes = DefaultByteConverter.ToInt(bytes, ref idx);
            Xp = DefaultByteConverter.ToInt(bytes, ref idx);
            Deaths = DefaultByteConverter.ToInt(bytes, ref idx);

            DamageReceived = DefaultByteConverter.ToInt(bytes, ref idx);
            ArmorPickedUp = DefaultByteConverter.ToInt(bytes, ref idx);
            HealthPickedUp = DefaultByteConverter.ToInt(bytes, ref idx);

            MeleeKills = DefaultByteConverter.ToInt(bytes, ref idx);
            MeleeShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            MeleeShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            MeleeDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            HandgunKills = DefaultByteConverter.ToInt(bytes, ref idx);
            HandgunShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            HandgunShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            HandgunDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            MachineGunKills = DefaultByteConverter.ToInt(bytes, ref idx);
            MachineGunShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            MachineGunShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            MachineGunDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            ShotgunSplats = DefaultByteConverter.ToInt(bytes, ref idx);
            ShotgunShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            ShotgunShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            ShotgunDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            SniperKills = DefaultByteConverter.ToInt(bytes, ref idx);
            SniperShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            SniperShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            SniperDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            SplattergunKills = DefaultByteConverter.ToInt(bytes, ref idx);
            SplattergunShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            SplattergunShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            SplattergunDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            CannonKills = DefaultByteConverter.ToInt(bytes, ref idx);
            CannonShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            CannonShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            CannonDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            LauncherKills = DefaultByteConverter.ToInt(bytes, ref idx);
            LauncherShotsFired = DefaultByteConverter.ToInt(bytes, ref idx);
            LauncherShotsHit = DefaultByteConverter.ToInt(bytes, ref idx);
            LauncherDamageDone = DefaultByteConverter.ToInt(bytes, ref idx);

            Suicides = DefaultByteConverter.ToInt(bytes, ref idx);
            Points = DefaultByteConverter.ToInt(bytes, ref idx);

            return idx;
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(_allValues.Count * 4);

            DefaultByteConverter.FromInt(Headshots, ref bytes);
            DefaultByteConverter.FromInt(Nutshots, ref bytes);
            DefaultByteConverter.FromInt(ConsecutiveSnipes, ref bytes);
            DefaultByteConverter.FromInt(Xp, ref bytes);
            DefaultByteConverter.FromInt(Deaths, ref bytes);

            DefaultByteConverter.FromInt(DamageReceived, ref bytes);
            DefaultByteConverter.FromInt(ArmorPickedUp, ref bytes);
            DefaultByteConverter.FromInt(HealthPickedUp, ref bytes);

            DefaultByteConverter.FromInt(MeleeKills, ref bytes);
            DefaultByteConverter.FromInt(MeleeShotsFired, ref bytes);
            DefaultByteConverter.FromInt(MeleeShotsHit, ref bytes);
            DefaultByteConverter.FromInt(MeleeDamageDone, ref bytes);

            DefaultByteConverter.FromInt(HandgunKills, ref bytes);
            DefaultByteConverter.FromInt(HandgunShotsFired, ref bytes);
            DefaultByteConverter.FromInt(HandgunShotsHit, ref bytes);
            DefaultByteConverter.FromInt(HandgunDamageDone, ref bytes);

            DefaultByteConverter.FromInt(MachineGunKills, ref bytes);
            DefaultByteConverter.FromInt(MachineGunShotsFired, ref bytes);
            DefaultByteConverter.FromInt(MachineGunShotsHit, ref bytes);
            DefaultByteConverter.FromInt(MachineGunDamageDone, ref bytes);

            DefaultByteConverter.FromInt(ShotgunSplats, ref bytes);
            DefaultByteConverter.FromInt(ShotgunShotsFired, ref bytes);
            DefaultByteConverter.FromInt(ShotgunShotsHit, ref bytes);
            DefaultByteConverter.FromInt(ShotgunDamageDone, ref bytes);

            DefaultByteConverter.FromInt(SniperKills, ref bytes);
            DefaultByteConverter.FromInt(SniperShotsFired, ref bytes);
            DefaultByteConverter.FromInt(SniperShotsHit, ref bytes);
            DefaultByteConverter.FromInt(SniperDamageDone, ref bytes);

            DefaultByteConverter.FromInt(SplattergunKills, ref bytes);
            DefaultByteConverter.FromInt(SplattergunShotsFired, ref bytes);
            DefaultByteConverter.FromInt(SplattergunShotsHit, ref bytes);
            DefaultByteConverter.FromInt(SplattergunDamageDone, ref bytes);

            DefaultByteConverter.FromInt(CannonKills, ref bytes);
            DefaultByteConverter.FromInt(CannonShotsFired, ref bytes);
            DefaultByteConverter.FromInt(CannonShotsHit, ref bytes);
            DefaultByteConverter.FromInt(CannonDamageDone, ref bytes);

            DefaultByteConverter.FromInt(LauncherKills, ref bytes);
            DefaultByteConverter.FromInt(LauncherShotsFired, ref bytes);
            DefaultByteConverter.FromInt(LauncherShotsHit, ref bytes);
            DefaultByteConverter.FromInt(LauncherDamageDone, ref bytes);

            DefaultByteConverter.FromInt(Suicides, ref bytes);
            DefaultByteConverter.FromInt(Points, ref bytes);

            return bytes.ToArray();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            foreach (PropertyInfo i in _allValues)
            {
                b.AppendFormat("{0}:{1}\n", i.Name, i.GetValue(this, null));
            }

            return b.ToString();
        }
    }
}
