using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class RankingView
    {
        public int Cmid { get; set; }
        public int Rank { get; set; }
        public string ClanTag { get; set; }
        public string Name { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public decimal Kdr
        {
            get
            {
                if (Deaths != 0)
                {
                    return (decimal)Kills / (decimal)Deaths;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}