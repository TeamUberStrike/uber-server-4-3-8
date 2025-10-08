using System;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business.Views
{
    public class PlayerWeeklyCard : PlayerCardView
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

        public PlayerWeeklyCard() : base()
        {
        }

        public PlayerWeeklyCard(string name, int splats, int splatted, string precision, int ranking, int totalSplats, int totalSplatted, int shotsTotal, int hitsTotal, decimal kdRatio, decimal kdRatioTotal, int nbPoints, int nbPointsTotal, DateTime memberSince, string tagName)
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
        }

        public PlayerWeeklyCard(int UserID, Boolean isCmuneUser, int WeekId)
        {
            using (UberstrikeDataContext ppDB = new UberstrikeDataContext())
            {
                if (isCmuneUser)
                {
                    User currentUser = ppDB.Users.SingleOrDefault<User>(u => u.CMID == UserID);
                    if (currentUser != null)
                    {
                        UserID = currentUser.UserID;
                    }
                }

                WeeklyTotalRanking PPRankingUser = ppDB.WeeklyTotalRankings.SingleOrDefault<WeeklyTotalRanking>(f => f.UserId == UserID && f.WeekId == WeekId);
                if (PPRankingUser != null && PPRankingUser.Name != null)
                {
                    this.Name = PPRankingUser.Name.ToString();
                    if (PPRankingUser.Splats != null) this.Splats = PPRankingUser.Splats.Value;
                    else this.Splats = 0;
                    if (PPRankingUser.Splatted != null) this.Splatted = PPRankingUser.Splatted.Value;
                    else this.Splatted = 0;
                    if (PPRankingUser.Hits != null && PPRankingUser.Shots != null && PPRankingUser.Shots != 0)
                    {
                        this.Precision = (PPRankingUser.Hits.Value * 100 / (double)PPRankingUser.Shots.Value).ToString("F1");
                    }
                    else
                    {
                        this.Precision = "-";
                    }

                    if (PPRankingUser.HitsTotal != null && PPRankingUser.ShotsTotal != null && PPRankingUser.ShotsTotal != 0)
                    {
                        this.PrecisionTotal = (PPRankingUser.HitsTotal.Value * 100 / (double)PPRankingUser.ShotsTotal.Value).ToString("F1");
                    }
                    else
                    {
                        this.PrecisionTotal = "-";
                    }

                    this.Cmid = PPRankingUser.CMID;
                    this.Ranking = (int)PPRankingUser.Ranknum;
                    this.MemberSince = PPRankingUser.MemberSince;
                    if (PPRankingUser.HitsTotal != null)
                    {
                        this.HitsTotal = PPRankingUser.HitsTotal.Value;
                    }

                    if (PPRankingUser.TotalSplats != null)
                    {
                        this.TotalSplats = PPRankingUser.TotalSplats.Value;
                    }

                    if (PPRankingUser.TotalSplatted != null)
                    {
                        this.TotalSplatted = PPRankingUser.TotalSplatted.Value;
                    }

                    if (PPRankingUser.ShotsTotal != null)
                    {
                        this.ShotsTotal = PPRankingUser.ShotsTotal.Value;
                    }

                    if (PPRankingUser.KDRatio != null)
                    {
                        this.KDRatio = PPRankingUser.KDRatio.Value;
                    }

                    if (PPRankingUser.KDRatioTotal != null)
                    {
                        this.KDRatioTotal = PPRankingUser.KDRatioTotal.Value;
                    }

                    if (PPRankingUser.NbPoints != null)
                    {
                        this.NbPoints = PPRankingUser.NbPoints.Value;
                    }

                    if (PPRankingUser.NbPointsTotal != null)
                    {
                        this.NbPointsTotal = PPRankingUser.NbPointsTotal.Value;
                    }

                    if (PPRankingUser.TagName != null)
                    {
                        if (!HttpUtility.HtmlEncode(PPRankingUser.TagName).Equals(""))
                        {
                            this.TagName = "[" + HttpUtility.HtmlEncode(PPRankingUser.TagName) + "]";
                        }
                    }
                }
            }
        }

        public PlayerWeeklyCard(WeeklyTotalRanking PPRankingUser)
        {
            if (PPRankingUser != null)
            {
                this.Name = PPRankingUser.Name.ToString();
                if (PPRankingUser.Splats != null) this.Splats = PPRankingUser.Splats.Value;
                else this.Splats = 0;
                if (PPRankingUser.Splatted != null) this.Splatted = PPRankingUser.Splatted.Value;
                else this.Splatted = 0;
                if (PPRankingUser.Hits != null && PPRankingUser.Shots != null && PPRankingUser.Shots != 0)
                {
                    this.Precision = (PPRankingUser.Hits.Value * 100 / (double)PPRankingUser.Shots.Value).ToString("F1");
                }
                else
                {
                    this.Precision = "-";
                }

                if (PPRankingUser.HitsTotal != null && PPRankingUser.ShotsTotal != null && PPRankingUser.ShotsTotal != 0)
                {
                    this.PrecisionTotal = (PPRankingUser.HitsTotal.Value * 100 / (double)PPRankingUser.ShotsTotal.Value).ToString("F1");
                }
                else
                {
                    this.PrecisionTotal = "-";
                }

                this.Cmid = PPRankingUser.CMID;
                this.Ranking = (int)PPRankingUser.Ranknum;
                this.MemberSince = PPRankingUser.MemberSince;
                if (PPRankingUser.HitsTotal != null)
                {
                    this.HitsTotal = PPRankingUser.HitsTotal.Value;
                }

                if (PPRankingUser.TotalSplats != null)
                {
                    this.TotalSplats = PPRankingUser.TotalSplats.Value;
                }

                if (PPRankingUser.TotalSplatted != null)
                {
                    this.TotalSplatted = PPRankingUser.TotalSplatted.Value;
                }

                if (PPRankingUser.ShotsTotal != null)
                {
                    this.ShotsTotal = PPRankingUser.ShotsTotal.Value;
                }

                if (PPRankingUser.KDRatio != null)
                {
                    this.KDRatio = PPRankingUser.KDRatio.Value;
                }

                if (PPRankingUser.KDRatioTotal != null)
                {
                    this.KDRatioTotal = PPRankingUser.KDRatioTotal.Value;
                }
                
                if (PPRankingUser.NbPoints != null)
                {
                    this.NbPoints = PPRankingUser.NbPoints.Value;
                }

                if (PPRankingUser.NbPointsTotal != null)
                {
                    this.NbPointsTotal = PPRankingUser.NbPointsTotal.Value;
                }

                if (PPRankingUser.TagName != null)
                {
                    if (!HttpUtility.HtmlEncode(PPRankingUser.TagName).Equals(""))
                    {
                        this.TagName = "[" + HttpUtility.HtmlEncode(PPRankingUser.TagName) + "]";
                    }
                }
            }
        }

        #endregion
    }
}