using System;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business.Views
{
    public class PlayerDailyCard : PlayerCardView
    {
        #region Private fields

        private int _TotalSplats;
        private int _TotalSplatted;
        private decimal _KDRatio;
        private decimal _KDRatioTotal;
        private int _ShotsTotal;
        private int _HitsTotal;
        private int _NbPoints;
        private int _NbPointsTotal;
        private DateTime _MemberSince;
        private int _RankingTotal;
        private string _PrecisionTotal;

        #endregion

        #region Properties

        public int TotalSplats
        {
            get { return _TotalSplats; }
            set { _TotalSplats = value; }
        }

        public int TotalSplatted
        {
            get { return _TotalSplatted; }
            set { _TotalSplatted = value; }
        }

        public int ShotsTotal
        {
            get { return _ShotsTotal; }
            set { _ShotsTotal = value; }
        }

        public int HitsTotal
        {
            get { return _HitsTotal; }
            set { _HitsTotal = value; }
        }

        public decimal KDRatio
        {
            get { return _KDRatio; }
            set { _KDRatio = value; }
        }

        public decimal KDRatioTotal
        {
            get { return _KDRatioTotal; }
            set { _KDRatioTotal = value; }
        }

        public int NbPoints
        {
            get { return _NbPoints; }
            set { _NbPoints = value; }
        }

        public int NbPointsTotal
        {
            get { return _NbPointsTotal; }
            set { _NbPointsTotal = value; }
        }

        public DateTime MemberSince
        {
            get { return _MemberSince; }
            set { _MemberSince = value; }
        }

        public int RankingTotal
        {
            get { return _RankingTotal; }
            set { _RankingTotal = value; }
        }

        public string PrecisionTotal
        {
            get { return _PrecisionTotal; }
            set { _PrecisionTotal = value; }
        }

        #endregion

        #region Constructors

        public PlayerDailyCard() : base()
        {
        }

        public PlayerDailyCard(string name, int splats, int splatted, string precision, int ranking, int totalSplats, int totalSplatted, int shotsTotal, int hitsTotal, decimal kdRatio, decimal kdRatioTotal, int nbPoints, int nbPointsTotal, DateTime memberSince, int ranknumTotal, string precisionTotal, string groupTag, string tagName)
            : base(name, splats, splatted, precision, ranking, tagName)
        {
            TotalSplatted = totalSplatted;
            TotalSplats = totalSplats;
            ShotsTotal = shotsTotal;
            HitsTotal = hitsTotal;
            KDRatioTotal = kdRatioTotal;
            KDRatio = kdRatio;
            NbPoints = nbPoints;
            NbPointsTotal = nbPointsTotal;
            MemberSince = memberSince;
            RankingTotal = ranknumTotal;
            PrecisionTotal = precisionTotal;
        }

        public PlayerDailyCard(DailyRanking dailyRanking)
        {
            if (dailyRanking != null)
            {
                this.Name = dailyRanking.Name;
                this.Splats = dailyRanking.Kills;
                this.Splatted = dailyRanking.Deaths;

                if (dailyRanking.Shots != 0)
                {
                    this.Precision = (dailyRanking.Hits * 100 / (double)dailyRanking.Shots).ToString("F1");
                }
                else
                {
                    this.Precision = "-";
                }

                if (dailyRanking.TotalShots != 0)
                {
                    this.PrecisionTotal = (dailyRanking.TotalHits * 100 / (double)dailyRanking.TotalShots).ToString("F1");
                }
                else
                {
                    this.PrecisionTotal = "-";
                }

                this.Cmid = dailyRanking.Cmid;
                this.Ranking = dailyRanking.DailyRankingIndex;
                this.MemberSince = dailyRanking.JoinDate;
                this.HitsTotal = dailyRanking.TotalHits;
                this.TotalSplats = dailyRanking.TotalKills;
                this.TotalSplatted = dailyRanking.TotalDeaths;
                this.ShotsTotal = dailyRanking.TotalShots;

                this.KDRatio = 0;
                
                if (dailyRanking.Deaths != 0)
                {
                    this.KDRatio = dailyRanking.Kills / dailyRanking.Deaths;
                }

                this.KDRatioTotal = 0;

                if (dailyRanking.TotalDeaths != 0)
                {
                    this.KDRatioTotal = dailyRanking.TotalKills / dailyRanking.TotalDeaths;
                }

                this.TagName = dailyRanking.ClanTag;
            }
        }

        #endregion
    }
}