using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenFlashChart;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class SocialChart : StatisticChart
    {
        #region Properties

        public Dictionary<int, int> PlayerLevelDistribution { get; private set; }

        #endregion

        #region Constructors

        public SocialChart(DateTime FromDate, DateTime ToDate)
                : base(FromDate, ToDate)
        {
        }

        #endregion

        #region Methods

        protected void InitPlayerLevelDistributionChart(DateTime statDate, bool getActivePlayersOnly)
        {
            PlayerLevelDistribution = AdminCache.LoadPlayerLevelDistribution(statDate, getActivePlayersOnly);
        }

        public OpenFlashChart.OpenFlashChart DrawPlayerLevelDistributionChart(DateTime statDate, bool getActivePlayersOnly)
        {
            InitPlayerLevelDistributionChart(statDate, getActivePlayersOnly);

            // We need to do some magic on the data, the boys want to aggregate some data and to display % instead of total value

            Dictionary<string, FloatPair> PlayerLevelDistributionDisplay = new Dictionary<string, FloatPair>();

            int totalPlayers = 0;

            // First we group the levels that need to be grouped

            int i = 1;

            for (; i < 11; i++)
            {
                PlayerLevelDistributionDisplay.Add("Level " + i.ToString(), new FloatPair(PlayerLevelDistribution[i], 0));
                totalPlayers += PlayerLevelDistribution[i];
            }

            while (i < UberStrikeCommonConfig.LevelCap)
            {
                int j = 1;
                int levelCount = 0;

                for (j = 1; j < 5; j++)
                {
                    levelCount += PlayerLevelDistribution[i + j];
                }

                totalPlayers += levelCount;
                PlayerLevelDistributionDisplay.Add(String.Format("Level {0} - {1}", i, i + j), new FloatPair(levelCount, 0));

                i += j;
            }

            // Now we compute the %

            foreach (string key in PlayerLevelDistributionDisplay.Keys.ToList())
            {
                float value = PlayerLevelDistributionDisplay[key].Float1;
                PlayerLevelDistributionDisplay[key] = new FloatPair(value, (value / (float) totalPlayers) * 100);
            }
            
            string dotColor = "#0078a4";
            BarFilled levelCountValues = new BarFilled();

            float max = 0;

            foreach (FloatPair kvp in PlayerLevelDistributionDisplay.Values)
            {
                float percent = (float) Math.Round(kvp.Float2, 2);

                if (percent > max)
                {
                    max = percent;
                }

                levelCountValues.Add(percent);
            }

            levelCountValues.Text = "%";
            levelCountValues.Colour = "#ff0000";

            OpenFlashChart.OpenFlashChart playerLevelDistributionChart = ChartUtil.CreateDefaultBarChart(String.Format("Player Level Distribution {0}", statDate.ToYMDDateTime()), 80, "#6fb60a", dotColor);

            playerLevelDistributionChart.X_Axis.Labels.Steps = 1;
            playerLevelDistributionChart.X_Axis.Labels.Rotate = "-20";
            playerLevelDistributionChart.X_Axis.SetLabels(PlayerLevelDistributionDisplay.Keys.ToArray());
            playerLevelDistributionChart.Y_Axis.Steps = 10;

            playerLevelDistributionChart.AddElement(levelCountValues);

            return playerLevelDistributionChart;
        }

        #endregion
    }
}