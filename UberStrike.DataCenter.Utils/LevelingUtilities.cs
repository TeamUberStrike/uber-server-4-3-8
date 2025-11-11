using System;
using System.Collections.Generic;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class LevelingUtilities
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, PlayerLevelCapView> Levels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MinLevel { get; protected set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levels"></param>
        public LevelingUtilities(List<PlayerLevelCapView> levels)
        {
            Levels = new Dictionary<int, PlayerLevelCapView>();
            MaxLevel = 1;
            MinLevel = UberStrikeCommonConfig.DefaultLevel;

            foreach (PlayerLevelCapView level in levels)
            {
                Levels.Add(level.Level, level);

                if (level.Level > MaxLevel)
                {
                    MaxLevel = level.Level;
                }
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Get the amount of XP required to level up
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <returns>Returns 0 if the level is invalid OR max level is already reached</returns>
        public int GetNextLevelXPRequired(int currentLevel)
        {
            int xpRequired = 0;

            if (currentLevel >= 1 && currentLevel < this.MaxLevel && this.Levels.ContainsKey(currentLevel + 1))
            {
                xpRequired = this.Levels[currentLevel + 1].XPRequired;
            }

            return xpRequired;
        }

        /// <summary>
        /// Get the XP cap of a level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetXPCap(int level)
        {
            int xp = 0;

            if (this.Levels.ContainsKey(level))
            {
                xp = this.Levels[level].XPRequired;
            }

            return xp;
        }

        /// <summary>
        /// Compute the XP that the player gained on the last match
        /// </summary>
        /// <param name="lastMatchStatistics"></param>
        /// <param name="xpEvents"></param>
        /// <returns></returns>
        public static int ComputeXP(PlayerStatisticsView lastMatchStatistics, Dictionary<int, PlayerXPEventView> xpEvents)
        {
            int xp = 0;

            if (lastMatchStatistics != null)
            {
                if (xpEvents.ContainsKey(PlayerXPEventViewId.Damage))
                {
                    int damageDealt = lastMatchStatistics.WeaponStatistics.CannonTotalDamageDone + lastMatchStatistics.WeaponStatistics.HandgunTotalDamageDone + lastMatchStatistics.WeaponStatistics.LauncherTotalDamageDone + lastMatchStatistics.WeaponStatistics.MachineGunTotalDamageDone + lastMatchStatistics.WeaponStatistics.MeleeTotalDamageDone + lastMatchStatistics.WeaponStatistics.ShotgunTotalDamageDone + lastMatchStatistics.WeaponStatistics.SniperTotalDamageDone + lastMatchStatistics.WeaponStatistics.SplattergunTotalDamageDone;

                    xp += (int)Math.Round(Math.Max(damageDealt, 0) * xpEvents[PlayerXPEventViewId.Damage].XPMultiplier, MidpointRounding.AwayFromZero);
                }

                if (xpEvents.ContainsKey(PlayerXPEventViewId.HeadShot))
                {
                    xp += (int)Math.Round(Math.Max(lastMatchStatistics.Headshots, 0) * xpEvents[PlayerXPEventViewId.HeadShot].XPMultiplier, MidpointRounding.AwayFromZero);
                }

                if (xpEvents.ContainsKey(PlayerXPEventViewId.Humiliation))
                {
                    xp += (int)Math.Round(Math.Max(lastMatchStatistics.WeaponStatistics.MeleeTotalSplats, 0) * xpEvents[PlayerXPEventViewId.Humiliation].XPMultiplier, MidpointRounding.AwayFromZero);
                }

                if (xpEvents.ContainsKey(PlayerXPEventViewId.Nutshot))
                {
                    xp += (int)Math.Round(Math.Max(lastMatchStatistics.Nutshots, 0) * xpEvents[PlayerXPEventViewId.Nutshot].XPMultiplier, MidpointRounding.AwayFromZero);
                }

                if (xpEvents.ContainsKey(PlayerXPEventViewId.Splat) && lastMatchStatistics.Splats > 0)
                {
                    xp += (int)Math.Round(Math.Max(lastMatchStatistics.Splats, 0) * xpEvents[PlayerXPEventViewId.Splat].XPMultiplier, MidpointRounding.AwayFromZero);
                }
            }

            return xp;
        }

        /// <summary>
        /// Get the XP matching with an event
        /// </summary>
        /// <param name="value"></param>
        /// <param name="eventId"></param>
        /// <param name="xpEvents"></param>
        /// <returns></returns>
        public static int ComputeXP(int value, int eventId, Dictionary<int, PlayerXPEventView> xpEvents)
        {
            PlayerXPEventView view;

            if (xpEvents.TryGetValue(eventId, out view))
            {
                return (int)Math.Round(Math.Max(value, 0) * view.XPMultiplier, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Level up one or more level according to the XP
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="totalXP"></param>
        /// <returns></returns>
        public int LevelUp(int currentLevel, int totalXP)
        {
            int newLevel = currentLevel;

            for (int i = currentLevel + 1; i <= UberStrikeCommonConfig.LevelCap && totalXP >= this.Levels[i].XPRequired; i++)
            {
                newLevel = i;
            }

            return newLevel;
        }

        /// <summary>
        /// Level down one or more level according to the XP
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="totalXP"></param>
        /// <returns></returns>
        public int LevelDown(int currentLevel, int totalXP)
        {
            int newLevel = currentLevel;

            for (int i = currentLevel - 1; i > this.MinLevel && totalXP < this.Levels[i].XPRequired; i--)
            {
                newLevel = i;
            }

            return newLevel;
        }

        #endregion Methods
    }
}