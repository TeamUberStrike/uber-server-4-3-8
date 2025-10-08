using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Utils;
using UberStrike.Realtime.Common;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class PlayerStatistics
    {
        public PlayerStatistics(int actorId, int cmid, string name, int level)
        {
            ActorId = actorId;
            Cmid = cmid;
            Name = name;
            Level = level;

            TotalStats = new StatsCollection();
            CurrentLifeStats = new StatsCollection();
            BestLifeStats = new StatsCollection();
        }

        public void UpdateStatistics()
        {
            //only important for the stats page of the player
            CurrentLifeStats.Xp = ComputeXp(GetPlayerStatisticsView(CurrentLifeStats));

            TotalStats.AddAllValues(CurrentLifeStats);

            //compute the total XP based onthe total stats - not based on the sum of all perlife stats
            TotalStats.Xp = ComputeXp(GetPlayerStatisticsView(TotalStats));

            BestLifeStats.TakeBestValues(CurrentLifeStats);

            CurrentLifeStats.Reset();
        }

        public int GetSplatsXP()
        {
            return LevelingUtilities.ComputeXP(TotalStats.GetKills() + CurrentLifeStats.GetKills(), PlayerXPEventViewId.Splat, GameDataManager.Instance.PlayerXPEvents);
        }
        public int GetHeadshotXP()
        {
            return LevelingUtilities.ComputeXP(TotalStats.Headshots + CurrentLifeStats.Headshots, PlayerXPEventViewId.HeadShot, GameDataManager.Instance.PlayerXPEvents);
        }
        public int GetSmackdownXP()
        {
            return LevelingUtilities.ComputeXP(TotalStats.MeleeKills + CurrentLifeStats.MeleeKills, PlayerXPEventViewId.Humiliation, GameDataManager.Instance.PlayerXPEvents);
        }
        public int GetNutshotXP()
        {
            return LevelingUtilities.ComputeXP(TotalStats.Nutshots + CurrentLifeStats.Nutshots, PlayerXPEventViewId.Nutshot, GameDataManager.Instance.PlayerXPEvents);
        }
        public int GetDamageXP()
        {
            return LevelingUtilities.ComputeXP(TotalStats.GetDamageDealt() + CurrentLifeStats.GetDamageDealt(), PlayerXPEventViewId.Damage, GameDataManager.Instance.PlayerXPEvents);
        }
        public int GetTotalXp()
        {
            return GetDamageXP() + GetNutshotXP() + GetSmackdownXP() + GetHeadshotXP() + GetSplatsXP();
        }

        public void RegisterTargetHit(int damage, bool isLethal, bool isSelfInflicted)
        {
            CurrentLifeStats.DamageReceived += damage;

            if (isLethal)
            {
                CurrentLifeStats.Deaths++;

                if (isSelfInflicted)
                {
                    CurrentLifeStats.Suicides++;
                }
            }
        }

        public void RegisterDeath()
        {
            CurrentLifeStats.Deaths++;
        }

        public int RegisterShooterHit(int damage, bool isLethal, UberstrikeItemClass category, BodyPart bodyPart, int shotId, bool countBonus)
        {
            int xp = 0;

            if (isLethal && countBonus)
            {
                if (bodyPart == BodyPart.Head)
                {
                    CurrentLifeStats.Headshots++;
                    xp += (int)GameDataManager.Instance.GetXpMultiplier(PlayerXPEventViewId.HeadShot);
                }
                else if (bodyPart == BodyPart.Nuts)
                {
                    CurrentLifeStats.Nutshots++;
                    xp += (int)GameDataManager.Instance.GetXpMultiplier(PlayerXPEventViewId.Nutshot);
                }

                xp += (int)GameDataManager.Instance.GetXpMultiplier(PlayerXPEventViewId.Splat);
            }

            switch (category)
            {
                case UberstrikeItemClass.WeaponCannon:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.CannonKills++;
                            CurrentLifeStats.CannonDamageDone += damage;

                            //make sure not to count mutliple hits for a single shotgun shot
                            if (CurrentLifeStats.CannonShotsFired != shotId)
                                CurrentLifeStats.CannonShotsHit++;
                        }
                        if (CurrentLifeStats.CannonShotsFired < shotId)
                            CurrentLifeStats.CannonShotsFired = shotId;
                        break;
                    }
                case UberstrikeItemClass.WeaponLauncher:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.LauncherKills++;
                            CurrentLifeStats.LauncherDamageDone += damage;

                            //make sure not to count mutliple hits for a single shotgun shot
                            if (CurrentLifeStats.LauncherShotsFired != shotId)
                                CurrentLifeStats.LauncherShotsHit++;
                        }
                        if (CurrentLifeStats.LauncherShotsFired < shotId)
                            CurrentLifeStats.LauncherShotsFired = shotId;
                        break;
                    }
                case UberstrikeItemClass.WeaponSplattergun:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.SplattergunKills++;
                            CurrentLifeStats.SplattergunDamageDone += damage;

                            //make sure not to count mutliple hits for a single shotgun shot
                            if (CurrentLifeStats.SplattergunShotsFired != shotId)
                                CurrentLifeStats.SplattergunShotsHit++;
                        }
                        if (CurrentLifeStats.SplattergunShotsFired < shotId)
                            CurrentLifeStats.SplattergunShotsFired = shotId;
                        break;
                    }
                case UberstrikeItemClass.WeaponHandgun:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.HandgunKills++;
                            CurrentLifeStats.HandgunDamageDone += damage;
                            CurrentLifeStats.HandgunShotsHit++;
                        }
                        if (CurrentLifeStats.HandgunShotsFired < shotId)
                            CurrentLifeStats.HandgunShotsFired = shotId;
                        break;
                    }

                case UberstrikeItemClass.WeaponMachinegun:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.MachineGunKills++;
                            CurrentLifeStats.MachineGunDamageDone += damage;
                            CurrentLifeStats.MachineGunShotsHit++;
                        }
                        if (CurrentLifeStats.MachineGunShotsFired < shotId)
                            CurrentLifeStats.MachineGunShotsFired = shotId;
                        break;
                    }
                case UberstrikeItemClass.WeaponMelee:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.MeleeKills++;
                            CurrentLifeStats.MeleeDamageDone += damage;
                            CurrentLifeStats.MeleeShotsHit++;
                        }

                        if (CurrentLifeStats.MeleeShotsFired < shotId)
                            CurrentLifeStats.MeleeShotsFired = shotId;

                        xp += (int)GameDataManager.Instance.GetXpMultiplier(PlayerXPEventViewId.Humiliation);

                        break;
                    }
                case UberstrikeItemClass.WeaponShotgun:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.ShotgunSplats++;
                            CurrentLifeStats.ShotgunDamageDone += damage;

                            //make sure not to count mutliple hits for a single shotgun shot
                            if (CurrentLifeStats.SplattergunShotsFired != shotId)
                                CurrentLifeStats.SplattergunShotsHit++;
                        }
                        if (CurrentLifeStats.ShotgunShotsFired < shotId)
                        {
                            CurrentLifeStats.ShotgunShotsFired = shotId;
                        }
                        break;
                    }
                case UberstrikeItemClass.WeaponSniperRifle:
                    {
                        if (countBonus)
                        {
                            if (isLethal) CurrentLifeStats.SniperKills++;
                            CurrentLifeStats.SniperDamageDone += damage;
                            CurrentLifeStats.SniperShotsHit++;

                            if (_lastSniperShotCount + 1 == shotId)
                            {
                                CurrentLifeStats.ConsecutiveSnipes++;
                            }
                            _lastSniperShotCount = shotId;
                        }
                        if (CurrentLifeStats.SniperShotsFired < shotId)
                            CurrentLifeStats.SniperShotsFired = shotId;
                        break;
                    }
            }

            return xp;
        }

        public PlayerStatisticsView GetPlayerStatisticsView(StatsCollection c)
        {
            PlayerStatisticsView view = new PlayerStatisticsView(
                Cmid,

                c.GetKills(),
                c.Deaths,
                c.GetShots(),
                c.GetHits(),
                c.Headshots,
                c.Nutshots,

                0, //Player XP
                0, //Player Level

                CreatePersonalStatistics(BestLifeStats),
                CreateWeaponStatistics(c),

                c.Points //Player Points
                );

            return view;
        }

        private PlayerPersonalRecordStatisticsView CreatePersonalStatistics(StatsCollection c)
        {
            int xp = c.Xp;

            //CmuneDebug.Log("CreatePersonalStatistics for " + Name + ": XP = " + xp);

            return new PlayerPersonalRecordStatisticsView(
                c.Headshots, c.Nutshots, c.ConsecutiveSnipes, xp, c.GetKills(), c.GetDamageDealt(), c.DamageReceived, c.ArmorPickedUp, c.HealthPickedUp,
                c.MeleeKills, c.HandgunKills, c.MachineGunKills, c.ShotgunSplats, c.SniperKills, c.SplattergunKills, c.CannonKills, c.LauncherKills);
        }

        private PlayerWeaponStatisticsView CreateWeaponStatistics(StatsCollection c)
        {
            return new PlayerWeaponStatisticsView(
                c.MeleeKills, c.HandgunKills, c.MachineGunKills, c.ShotgunSplats, c.SniperKills, c.SplattergunKills, c.CannonKills, c.LauncherKills,
                c.MeleeShotsFired, c.MeleeShotsHit, c.MeleeDamageDone,
                c.HandgunShotsFired, c.HandgunShotsHit, c.HandgunDamageDone,
                c.MachineGunShotsFired, c.MachineGunShotsHit, c.MachineGunDamageDone,
                c.ShotgunShotsFired, c.ShotgunShotsHit, c.ShotgunDamageDone,
                c.SniperShotsFired, c.SniperShotsHit, c.SniperDamageDone,
                c.SplattergunShotsFired, c.SplattergunShotsHit, c.SplattergunDamageDone,
                c.CannonShotsFired, c.CannonShotsHit, c.CannonDamageDone,
                c.LauncherShotsFired, c.LauncherShotsHit, c.LauncherDamageDone);
        }

        private int ComputeXp(PlayerStatisticsView stats)
        {
            return System.Math.Max(LevelingUtilities.ComputeXP(stats, GameDataManager.Instance.PlayerXPEvents), 0);
        }

        #region PROPERTIES
        public StatsCollection BestLifeStats { get; private set; }
        public StatsCollection CurrentLifeStats { get; private set; }
        public StatsCollection TotalStats { get; private set; }

        public int Cmid { get; private set; }
        public int ActorId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public TeamID Team { get; set; }

        public int Kills { get { return TotalStats.GetKills() + CurrentLifeStats.GetKills(); } }
        public int Deaths { get { return TotalStats.Deaths + CurrentLifeStats.Deaths; } }
        public int Shots { get { return TotalStats.GetShots() + CurrentLifeStats.GetShots(); } }
        public int Hits { get { return TotalStats.GetHits() + CurrentLifeStats.GetHits(); } }

        public int Points { get { return TotalStats.Points + CurrentLifeStats.Points; } }
        #endregion

        #region FIELDS
        private int _lastSniperShotCount = 0;
        #endregion
    }
}

/*
       [UserID]
      ,[CMID]
      ,[XP]
      ,[Level]
 * 
      ,[Splats]
      ,[Splatted]
      ,[Shots]
      ,[Hits]
      ,[Headshots]
      ,[Nutshots]
      ,[TimeSpentInGame]
 * 
      ,[MostHeadshots]
      ,[MostNutshots]
      ,[MostConsecutiveSnipes]
      ,[MostXPEarned]
      ,[MostSplats]
      ,[MostDamageDealt]
      ,[MostDamageReceived]
      ,[MostArmorPickedUp]
      ,[MostHealthPickedUp]
 * 
      ,[MostMeleeSplats]
      ,[MostHandgunSplats]
      ,[MostMachinegunSplats]
      ,[MostShotgunSplats]
      ,[MostSniperSplats]
      ,[MostSplattergunSplats]
      ,[MostCannonSplats]
      ,[MostLauncherSplats]
 * 
      ,[MeleeTotalSplats]
      ,[MeleeTotalShotsFired]
      ,[MeleeTotalShotsHit]
      ,[MeleeTotalDamageDone]
 * 
      ,[HandgunTotalSplats]
      ,[HandgunTotalShotsFired]
      ,[HandgunTotalShotsHit]
      ,[HandgunTotalDamageDone]
 * 
      ,[MachineGunTotalSplats]
      ,[MachineGunTotalShotsFired]
      ,[MachineGunTotalShotsHit]
      ,[MachineGunTotalDamageDone]
 * 
      ,[ShotgunTotalSplats]
      ,[ShotgunTotalShotsFired]
      ,[ShotgunTotalShotsHit]
      ,[ShotgunTotalDamageDone]
 * 
      ,[SniperTotalSplats]
      ,[SniperTotalShotsFired]
      ,[SniperTotalShotsHit]
      ,[SniperTotalDamageDone]
 * 
      ,[SplattergunTotalSplats]
      ,[SplattergunTotalShotsFired]
      ,[SplattergunTotalShotsHit]
      ,[SplattergunTotalDamageDone]
 * 
      ,[CannonTotalSplats]
      ,[CannonTotalShotsFired]
      ,[CannonTotalShotsHit]
      ,[CannonTotalDamageDone]
 * 
      ,[LauncherTotalSplats]
      ,[LauncherTotalShotsFired]
      ,[LauncherTotalShotsHit]
      ,[LauncherTotalDamageDone]
*/