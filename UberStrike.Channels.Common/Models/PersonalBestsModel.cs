using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberStrike.Channels.Common.Models
{
    public class PersonalBestsModel
    {
        public PersonalBestsModel() { }

        public int MostArmorPickedUp { get; set; }
        public int MostConsecutiveSnipes { get; set; }
        public int MostDamageDealt { get; set; }
        public int MostDamageReceived { get; set; }
        public int MostHeadShots { get; set; }
        public int MostHealthPickedUp { get; set; }
        public int MostKills { get; set; }
        public int MostNutShots { get; set; }
        public int MostXpEarned { get; set; }
    }

    public class AllTimeStatsModel
    {
        public AllTimeStatsModel() { }

        public int Kills { get; set; }
        public int Deaths { get; set; }
        public decimal Kdr
        {
            get
            {
                if (Deaths != 0)
                {
                    return (decimal) Kills / (decimal) Deaths;
                }
                else
                {
                    return 0;
                }
            }
        }
        public long Shots { get; set; }
        public long Hits { get; set; }
        public decimal Accuracy
        {
            get
            {
                if (Shots != 0)
                {
                    return (decimal)Hits / (decimal)Shots;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int Headshots { get; set; }
        public int Nutshots { get; set; }
    }

    public class WeaponsStatsModel
    {
        public WeaponsStatsModel() { }

        public int MostCannonKills { get; set; }
        public int MostHandGunKills { get; set; }
        public int MostLauncherKills { get; set; }
        public int MostMachineGunKills { get; set; }
        public int MostMeleeKills { get; set; }
        public int MostShotGunKills { get; set; }
        public int MostSniperKills { get; set; }
        public int MostSplatterGunKills { get; set; }

        public int TotalCannonKills { get; set; }
        public int TotalHandGunKills { get; set; }
        public int TotalLauncherKills { get; set; }
        public int TotalMachineGunKills { get; set; }
        public int TotalMeleeKills { get; set; }
        public int TotalShotGunKills { get; set; }
        public int TotalSniperKills { get; set; }
        public int TotalSplatterGunKills { get; set; }

        public int CannonShots { get; set; }
        public int CannonHits { get; set; }
        public decimal CannonAccuracy
        {
            get
            {
                if (CannonShots != 0)
                {
                    return (decimal)CannonHits / (decimal)CannonShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int HandGunShots { get; set; }
        public int HandGunHits { get; set; }
        public decimal HandGunAccuracy
        {
            get
            {
                if (HandGunShots != 0)
                {
                    return (decimal)HandGunHits / (decimal)HandGunShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int LauncherShots { get; set; }
        public int LauncherHits { get; set; }
        public decimal LauncherAccuracy
        {
            get
            {
                if (LauncherShots != 0)
                {
                    return (decimal)LauncherHits / (decimal)LauncherShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int MachineGunShots { get; set; }
        public int MachineGunHits { get; set; }
        public decimal MachineGunAccuracy
        {
            get
            {
                if (MachineGunShots != 0)
                {
                    return (decimal)MachineGunHits / (decimal)MachineGunShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int MeleeShots { get; set; }
        public int MeleeHits { get; set; }
        public decimal MeleeAccuracy
        {
            get
            {
                if (MeleeShots != 0)
                {
                    return (decimal)MeleeHits / (decimal)MeleeShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int ShotGunShots { get; set; }
        public int ShotGunHits { get; set; }
        public decimal ShotGunAccuracy
        {
            get
            {
                if (ShotGunShots != 0)
                {
                    return (decimal)ShotGunHits / (decimal)ShotGunShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int SniperShots { get; set; }
        public int SniperHits { get; set; }
        public decimal SniperAccuracy
        {
            get
            {
                if (SniperShots != 0)
                {
                    return (decimal)SniperHits / (decimal)SniperShots;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int SplatterGunShots { get; set; }
        public int SplatterGunHits { get; set; }
        public decimal SplatterGunAccuracy
        {
            get
            {
                if (SplatterGunShots != 0)
                {
                    return (decimal)SplatterGunHits / (decimal)SplatterGunShots;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}