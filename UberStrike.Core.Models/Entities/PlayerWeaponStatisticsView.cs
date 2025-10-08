using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PlayerWeaponStatisticsView
    {
        #region Properties

        public int MeleeTotalSplats { get; set; }

        public int HandgunTotalSplats { get; set; }

        public int MachineGunTotalSplats { get; set; }

        public int ShotgunTotalSplats { get; set; }

        public int SniperTotalSplats { get; set; }

        public int SplattergunTotalSplats { get; set; }

        public int CannonTotalSplats { get; set; }

        public int LauncherTotalSplats { get; set; }

        public int MeleeTotalShotsFired { get; set; }

        public int MeleeTotalShotsHit { get; set; }

        public int MeleeTotalDamageDone { get; set; }

        public int HandgunTotalShotsFired { get; set; }

        public int HandgunTotalShotsHit { get; set; }

        public int HandgunTotalDamageDone { get; set; }

        public int MachineGunTotalShotsFired { get; set; }

        public int MachineGunTotalShotsHit { get; set; }

        public int MachineGunTotalDamageDone { get; set; }

        public int ShotgunTotalShotsFired { get; set; }

        public int ShotgunTotalShotsHit { get; set; }

        public int ShotgunTotalDamageDone { get; set; }

        public int SniperTotalShotsFired { get; set; }

        public int SniperTotalShotsHit { get; set; }

        public int SniperTotalDamageDone { get; set; }

        public int SplattergunTotalShotsFired { get; set; }

        public int SplattergunTotalShotsHit { get; set; }

        public int SplattergunTotalDamageDone { get; set; }

        public int CannonTotalShotsFired { get; set; }

        public int CannonTotalShotsHit { get; set; }

        public int CannonTotalDamageDone { get; set; }

        public int LauncherTotalShotsFired { get; set; }

        public int LauncherTotalShotsHit { get; set; }

        public int LauncherTotalDamageDone { get; set; }

        #endregion Properties

        #region Constructors

        public PlayerWeaponStatisticsView()
        {
        }

        public PlayerWeaponStatisticsView(int meleeTotalSplats, int handgunTotalSplats, int machineGunTotalSplats, int shotgunTotalSplats, int sniperTotalSplats,
                                            int splattergunTotalSplats, int cannonTotalSplats, int launcherTotalSplats, int meleeTotalShotsFired, int meleeTotalShotsHit,
                                            int meleeTotalDamageDone, int handgunTotalShotsFired, int handgunTotalShotsHit, int handgunTotalDamageDone,
                                            int machineGunTotalShotsFired, int machineGunTotalShotsHit, int machineGunTotalDamageDone, int shotgunTotalShotsFired,
                                            int shotgunTotalShotsHit, int shotgunTotalDamageDone, int sniperTotalShotsFired, int sniperTotalShotsHit, int sniperTotalDamageDone,
                                            int splattergunTotalShotsFired, int splattergunTotalShotsHit, int splattergunTotalDamageDone, int cannonTotalShotsFired,
                                            int cannonTotalShotsHit, int cannonTotalDamageDone, int launcherTotalShotsFired, int launcherTotalShotsHit,
                                            int launcherTotalDamageDone)
        {
            this.CannonTotalDamageDone = cannonTotalDamageDone;
            this.CannonTotalShotsFired = cannonTotalShotsFired;
            this.CannonTotalShotsHit = cannonTotalShotsHit;
            this.CannonTotalSplats = cannonTotalSplats;
            this.HandgunTotalDamageDone = handgunTotalDamageDone;
            this.HandgunTotalShotsFired = handgunTotalShotsFired;
            this.HandgunTotalShotsHit = handgunTotalShotsHit;
            this.HandgunTotalSplats = handgunTotalSplats;
            this.LauncherTotalDamageDone = launcherTotalDamageDone;
            this.LauncherTotalShotsFired = launcherTotalShotsFired;
            this.LauncherTotalShotsHit = launcherTotalShotsHit;
            this.LauncherTotalSplats = launcherTotalSplats;
            this.MachineGunTotalDamageDone = machineGunTotalDamageDone;
            this.MachineGunTotalShotsFired = machineGunTotalShotsFired;
            this.MachineGunTotalShotsHit = machineGunTotalShotsHit;
            this.MachineGunTotalSplats = machineGunTotalSplats;
            this.MeleeTotalDamageDone = meleeTotalDamageDone;
            this.MeleeTotalShotsFired = meleeTotalShotsFired;
            this.MeleeTotalShotsHit = meleeTotalShotsHit;
            this.MeleeTotalSplats = meleeTotalSplats;
            this.ShotgunTotalDamageDone = shotgunTotalDamageDone;
            this.ShotgunTotalShotsFired = shotgunTotalShotsFired;
            this.ShotgunTotalShotsHit = shotgunTotalShotsHit;
            this.ShotgunTotalSplats = shotgunTotalSplats;
            this.SniperTotalDamageDone = sniperTotalDamageDone;
            this.SniperTotalShotsFired = sniperTotalShotsFired;
            this.SniperTotalShotsHit = sniperTotalShotsHit;
            this.SniperTotalSplats = sniperTotalSplats;
            this.SplattergunTotalDamageDone = splattergunTotalDamageDone;
            this.SplattergunTotalShotsFired = splattergunTotalShotsFired;
            this.SplattergunTotalShotsHit = splattergunTotalShotsHit;
            this.SplattergunTotalSplats = splattergunTotalSplats;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[PlayerWeaponStatisticsView: ");
            display.Append("[CannonTotalDamageDone: ");
            display.Append(this.CannonTotalDamageDone);
            display.Append("][CannonTotalShotsFired: ");
            display.Append(this.CannonTotalShotsFired);
            display.Append("][CannonTotalShotsHit: ");
            display.Append(this.CannonTotalShotsHit);
            display.Append("][CannonTotalSplats: ");
            display.Append(this.CannonTotalSplats);
            display.Append("][HandgunTotalDamageDone: ");
            display.Append(this.HandgunTotalDamageDone);
            display.Append("][HandgunTotalShotsFired: ");
            display.Append(this.HandgunTotalShotsFired);
            display.Append("][HandgunTotalShotsHit: ");
            display.Append(this.HandgunTotalShotsHit);
            display.Append("][HandgunTotalSplats: ");
            display.Append(this.HandgunTotalSplats);
            display.Append("][LauncherTotalDamageDone: ");
            display.Append(this.LauncherTotalDamageDone);
            display.Append("][LauncherTotalShotsFired: ");
            display.Append(this.LauncherTotalShotsFired);
            display.Append("][LauncherTotalShotsHit: ");
            display.Append(this.LauncherTotalShotsHit);
            display.Append("][LauncherTotalSplats: ");
            display.Append(this.LauncherTotalSplats);
            display.Append("][MachineGunTotalDamageDone: ");
            display.Append(this.MachineGunTotalDamageDone);
            display.Append("][MachineGunTotalShotsFired: ");
            display.Append(this.MachineGunTotalShotsFired);
            display.Append("][MachineGunTotalShotsHit: ");
            display.Append(this.MachineGunTotalShotsHit);
            display.Append("][MachineGunTotalSplats: ");
            display.Append(this.MachineGunTotalSplats);
            display.Append("][MeleeTotalDamageDone: ");
            display.Append(this.MeleeTotalDamageDone);
            display.Append("][MeleeTotalShotsFired: ");
            display.Append(this.MeleeTotalShotsFired);
            display.Append("][MeleeTotalShotsHit: ");
            display.Append(this.MeleeTotalShotsHit);
            display.Append("][MeleeTotalSplats: ");
            display.Append(this.MeleeTotalSplats);
            display.Append("][ShotgunTotalDamageDone: ");
            display.Append(this.ShotgunTotalDamageDone);
            display.Append("][ShotgunTotalShotsFired: ");
            display.Append(this.ShotgunTotalShotsFired);
            display.Append("][ShotgunTotalShotsHit: ");
            display.Append(this.ShotgunTotalShotsHit);
            display.Append("][ShotgunTotalSplats: ");
            display.Append(this.ShotgunTotalSplats);
            display.Append("][SniperTotalDamageDone: ");
            display.Append(this.SniperTotalDamageDone);
            display.Append("][SniperTotalShotsFired: ");
            display.Append(this.SniperTotalShotsFired);
            display.Append("][SniperTotalShotsHit: ");
            display.Append(this.SniperTotalShotsHit);
            display.Append("][SniperTotalSplats: ");
            display.Append(this.SniperTotalSplats);
            display.Append("][SplattergunTotalDamageDone: ");
            display.Append(this.SplattergunTotalDamageDone);
            display.Append("][SplattergunTotalShotsFired: ");
            display.Append(this.SplattergunTotalShotsFired);
            display.Append("][SplattergunTotalShotsHit: ");
            display.Append(this.SplattergunTotalShotsHit);
            display.Append("][SplattergunTotalSplats: ");
            display.Append(this.SplattergunTotalSplats);

            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}