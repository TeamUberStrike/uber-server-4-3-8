using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class UberstrikeWeaponConfigView
    {
        #region Properties

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

        public int LevelRequired { get; set; }

        #endregion Properties

        #region Constructors

        public UberstrikeWeaponConfigView()
        {
        }

        public UberstrikeWeaponConfigView(int damageKnockback, int damagePerProjectile, int accuracySpread, int recoilKickback, int startAmmo, int maxAmmo, int missileTimeToDetonate, int missileForceImpulse, int missileBounciness, int rateOfire, int splashRadius, int projectilesPerShot, int projectileSpeed, int recoilMovement)
        {
            this.DamageKnockback = damageKnockback;
            this.DamagePerProjectile = damagePerProjectile;
            this.AccuracySpread = accuracySpread;
            this.RecoilKickback = recoilKickback;
            this.StartAmmo = startAmmo;
            this.MaxAmmo = maxAmmo;
            this.MissileTimeToDetonate = missileTimeToDetonate;
            this.MissileForceImpulse = missileForceImpulse;
            this.MissileBounciness = missileBounciness;
            this.SplashRadius = splashRadius;
            this.ProjectilesPerShot = projectilesPerShot;
            this.ProjectileSpeed = projectileSpeed;
            this.RateOfFire = rateOfire;
            this.RecoilMovement = recoilMovement;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder configDisplay = new StringBuilder();

            configDisplay.Append("[UberstrikeWeaponConfigView: [DamageKnockback: ");
            configDisplay.Append(this.DamageKnockback);
            configDisplay.Append("][DamagePerProjectile: ");
            configDisplay.Append(this.DamagePerProjectile);
            configDisplay.Append("][AccuracySpread: ");
            configDisplay.Append(this.AccuracySpread);
            configDisplay.Append("][RecoilKickback: ");
            configDisplay.Append(this.RecoilKickback);
            configDisplay.Append("][StartAmmo: ");
            configDisplay.Append(this.StartAmmo);
            configDisplay.Append("][MaxAmmo: ");
            configDisplay.Append(this.MaxAmmo);
            configDisplay.Append("][MissileTimeToDetonate: ");
            configDisplay.Append(this.MissileTimeToDetonate);
            configDisplay.Append("][MissileForceImpulse: ");
            configDisplay.Append(this.MissileForceImpulse);
            configDisplay.Append("][MissileBounciness: ");
            configDisplay.Append(this.MissileBounciness);
            configDisplay.Append("][RateOfFire: ");
            configDisplay.Append(this.RateOfFire);
            configDisplay.Append("][SplashRadius: ");
            configDisplay.Append(this.SplashRadius);
            configDisplay.Append("][ProjectilesPerShot: ");
            configDisplay.Append(this.ProjectilesPerShot);
            configDisplay.Append("][ProjectileSpeed: ");
            configDisplay.Append(this.ProjectileSpeed);
            configDisplay.Append("][RecoilMovement: ");
            configDisplay.Append(this.RecoilMovement);
            configDisplay.Append("]]");

            return configDisplay.ToString();
        }

        #endregion Methods
    }
}