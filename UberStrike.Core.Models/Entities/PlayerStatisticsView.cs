using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PlayerStatisticsView
    {
        #region Properties

        public int Cmid { get; set; }
        
        public int Splats { get; set; }
        
        public int Splatted { get; set; }
        
        public long Shots { get; set; }
        
        public long Hits { get; set; }
        
        public int Headshots { get; set; }
        
        public int Nutshots { get; set; }
        
        public int Xp { get; set; }
        
        public int Level { get; set; }

        /// <summary>
        /// Time spent in game in minutes
        /// </summary>
        public int TimeSpentInGame { get; set; }
        
        public PlayerPersonalRecordStatisticsView PersonalRecord { get; set; }
        
        public PlayerWeaponStatisticsView WeaponStatistics { get; set; }
        
        public int Points { get; set; }

        #endregion Properties

        #region Constructors

        public PlayerStatisticsView()
        {
            this.PersonalRecord = new PlayerPersonalRecordStatisticsView();
            this.WeaponStatistics = new PlayerWeaponStatisticsView();
        }

        public PlayerStatisticsView(int cmid, int splats, int splatted, long shots, long hits, int headshots, int nutshots, PlayerPersonalRecordStatisticsView personalRecord, PlayerWeaponStatisticsView weaponStatistics, int points)
        {
            this.Cmid = cmid;
            this.Hits = hits;
            this.Level = 0;
            this.Shots = shots;
            this.Splats = splats;
            this.Splatted = splatted;
            this.Headshots = headshots;
            this.Nutshots = nutshots;
            this.Xp = 0;
            this.PersonalRecord = personalRecord;
            this.WeaponStatistics = weaponStatistics;
            this.Points = points;
        }

        public PlayerStatisticsView(int cmid, int splats, int splatted, long shots, long hits, int headshots, int nutshots, int xp, int level, PlayerPersonalRecordStatisticsView personalRecord, PlayerWeaponStatisticsView weaponStatistics, int points)
        {
            this.Cmid = cmid;
            this.Hits = hits;
            this.Level = level;
            this.Shots = shots;
            this.Splats = splats;
            this.Splatted = splatted;
            this.Headshots = headshots;
            this.Nutshots = nutshots;
            this.Xp = xp;
            this.PersonalRecord = personalRecord;
            this.WeaponStatistics = weaponStatistics;
            this.Points = points;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder playerStatisticsDisplay = new StringBuilder();

            playerStatisticsDisplay.Append("[PlayerStatisticsView: ");
            playerStatisticsDisplay.Append("[Cmid: ");
            playerStatisticsDisplay.Append(this.Cmid);
            playerStatisticsDisplay.Append("][Hits: ");
            playerStatisticsDisplay.Append(this.Hits);
            playerStatisticsDisplay.Append("][Level: ");
            playerStatisticsDisplay.Append(this.Level);
            playerStatisticsDisplay.Append("][Shots: ");
            playerStatisticsDisplay.Append(this.Shots);
            playerStatisticsDisplay.Append("][Splats: ");
            playerStatisticsDisplay.Append(this.Splats);
            playerStatisticsDisplay.Append("][Splatted: ");
            playerStatisticsDisplay.Append(this.Splatted);
            playerStatisticsDisplay.Append("][Headshots: ");
            playerStatisticsDisplay.Append(this.Headshots);
            playerStatisticsDisplay.Append("][Nutshots: ");
            playerStatisticsDisplay.Append(this.Nutshots);
            playerStatisticsDisplay.Append("][Xp: ");
            playerStatisticsDisplay.Append(this.Xp);
            playerStatisticsDisplay.Append("][Points: ");
            playerStatisticsDisplay.Append(this.Points);
            playerStatisticsDisplay.Append("]");
            playerStatisticsDisplay.Append(this.PersonalRecord);
            playerStatisticsDisplay.Append(this.WeaponStatistics);
            playerStatisticsDisplay.Append("]");

            return playerStatisticsDisplay.ToString();
        }

        #endregion Methods
    }
}