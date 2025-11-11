using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.Models.Views
{
    [System.Serializable]
    public class UberStrikeItemWeaponView : BaseUberStrikeItemView
    {
        public override UberstrikeItemType ItemType { get { return UberstrikeItemType.Weapon; } }

        public int DamageKnockback { get; set; }

        public int DamagePerProjectile { get; set; }

        public int AccuracySpread { get; set; }

        public int RecoilKickback { get; set; }

        public int StartAmmo { get; set; }

        public int MaxAmmo { get; set; }

        public int MissileTimeToDetonate { get; set; }

        public int MissileForceImpulse { get; set; }

        public int MissileBounciness { get; set; }

        public int SplashRadius { get; set; }

        public int ProjectilesPerShot { get; set; }

        public int ProjectileSpeed { get; set; }

        public int RateOfFire { get; set; }

        public int RecoilMovement { get; set; }
    }
}