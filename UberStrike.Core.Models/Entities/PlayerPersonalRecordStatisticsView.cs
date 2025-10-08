using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class PlayerPersonalRecordStatisticsView
    {
        #region Properties

        public int MostHeadshots { get; set; }

        public int MostNutshots { get; set; }

        public int MostConsecutiveSnipes { get; set; }

        public int MostXPEarned { get; set; }

        public int MostSplats { get; set; }

        public int MostDamageDealt { get; set; }

        public int MostDamageReceived { get; set; }

        public int MostArmorPickedUp { get; set; }

        public int MostHealthPickedUp { get; set; }

        public int MostMeleeSplats { get; set; }

        public int MostHandgunSplats { get; set; }

        public int MostMachinegunSplats { get; set; }

        public int MostShotgunSplats { get; set; }

        public int MostSniperSplats { get; set; }

        public int MostSplattergunSplats { get; set; }

        public int MostCannonSplats { get; set; }

        public int MostLauncherSplats { get; set; }

        #endregion Properties

        #region Constructors

        public PlayerPersonalRecordStatisticsView()
        {
        }

        public PlayerPersonalRecordStatisticsView(int mostHeadshots, int mostNutshots, int mostConsecutiveSnipes, int mostXPEarned, int mostSplats,
                                                    int mostDamageDealt, int mostDamageReceived, int mostArmorPickedUp, int mostHealthPickedUp, int mostMeleeSplats,
                                                    int mostHandgunSplats, int mostMachinegunSplats, int mostShotgunSplats, int mostSniperSplats,
                                                    int mostSplattergunSplats, int mostCannonSplats, int mostLauncherSplats)
        {
            this.MostArmorPickedUp = mostArmorPickedUp;
            this.MostCannonSplats = mostCannonSplats;
            this.MostConsecutiveSnipes = mostConsecutiveSnipes;
            this.MostDamageDealt = mostDamageDealt;
            this.MostDamageReceived = mostDamageReceived;
            this.MostHandgunSplats = mostHandgunSplats;
            this.MostHeadshots = mostHeadshots;
            this.MostHealthPickedUp = mostHealthPickedUp;
            this.MostLauncherSplats = mostLauncherSplats;
            this.MostMachinegunSplats = mostMachinegunSplats;
            this.MostMeleeSplats = mostMeleeSplats;
            this.MostNutshots = mostNutshots;
            this.MostShotgunSplats = mostShotgunSplats;
            this.MostSniperSplats = mostSniperSplats;
            this.MostSplats = mostSplats;
            this.MostSplattergunSplats = mostSplattergunSplats;
            this.MostXPEarned = mostXPEarned;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            StringBuilder display = new StringBuilder();

            display.Append("[PlayerPersonalRecordStatisticsView: ");
            display.Append("[MostArmorPickedUp: ");
            display.Append(this.MostArmorPickedUp);
            display.Append("][MostCannonSplats: ");
            display.Append(this.MostCannonSplats);
            display.Append("][MostConsecutiveSnipes: ");
            display.Append(this.MostConsecutiveSnipes);
            display.Append("][MostDamageDealt: ");
            display.Append(this.MostDamageDealt);
            display.Append("][MostDamageReceived: ");
            display.Append(this.MostDamageReceived);
            display.Append("][MostHandgunSplats: ");
            display.Append(this.MostHandgunSplats);
            display.Append("][MostHeadshots: ");
            display.Append(this.MostHeadshots);
            display.Append("][MostHealthPickedUp: ");
            display.Append(this.MostHealthPickedUp);
            display.Append("][MostLauncherSplats: ");
            display.Append(this.MostLauncherSplats);
            display.Append("][MostMachinegunSplats: ");
            display.Append(this.MostMachinegunSplats);
            display.Append("][MostMeleeSplats: ");
            display.Append(this.MostMeleeSplats);
            display.Append("][MostNutshots: ");
            display.Append(this.MostNutshots);
            display.Append("][MostShotgunSplats: ");
            display.Append(this.MostShotgunSplats);
            display.Append("][MostSniperSplats: ");
            display.Append(this.MostSniperSplats);
            display.Append("][MostSplats: ");
            display.Append(this.MostSplats);
            display.Append("][MostSplattergunSplats: ");
            display.Append(this.MostSplattergunSplats);
            display.Append("][MostXPEarned: ");
            display.Append(this.MostXPEarned);
            display.Append("]]");

            return display.ToString();
        }

        #endregion Methods
    }
}