using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Models.Display
{
    public class StatsHistoryDisplay
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public float Kdr { get; set; }
        public float Precision { get; set; }

        public StatsHistoryDisplay(int kills, int deaths, int shots, int hits)
        {
            Kills = kills;
            Deaths = deaths;
            Kdr = 0;

            if (deaths != 0)
            {
                Kdr = (float)kills / (float)deaths;
            }

            Precision = 0;

            if (shots != 0)
            {
                Precision = (float)hits / (float)shots;
            }
        }
    }
}