using System;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business.Views
{
    public class PlayerAllTimeCard : PlayerCardView
    {
        #region Private fields

        private decimal _KDRatio;
        private int _NbPoints;
        private DateTime _MemberSince;
 
        #endregion

        #region Properties

        public decimal KDRatio
        {
            get { return _KDRatio; }
            set { _KDRatio = value; }
        }

        public int NbPoints
        {
            get { return _NbPoints; }
            set { _NbPoints = value; }
        }
      
        public DateTime MemberSince
        {
            get { return _MemberSince; }
            set { _MemberSince = value; }
        }


        #endregion

        #region Constructors

        public PlayerAllTimeCard() : base()
        {
        }

        public PlayerAllTimeCard(string name, int splats, int splatted, string precision, int ranking, decimal kdRatio, int nbPoints, DateTime memberSince, string tagName)
            : base(name, splats, splatted, precision, ranking, tagName)
        {
            KDRatio = kdRatio;
            NbPoints = nbPoints;
            MemberSince = memberSince;
        }

        public PlayerAllTimeCard(int UserID, Boolean isCmuneUser)
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

                AllTimeTotalRanking PPRankingUser = ppDB.AllTimeTotalRankings.SingleOrDefault<AllTimeTotalRanking>(f => f.UserId == UserID);
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

                    this.Cmid = PPRankingUser.CMID;
                    this.Ranking = (int)PPRankingUser.Ranknum;
                    this.MemberSince = PPRankingUser.MemberSince;

                    if (PPRankingUser.KDRatio != null)
                    {
                        this.KDRatio = PPRankingUser.KDRatio.Value;
                    }

                    if (PPRankingUser.NbPoints != null)
                    {
                        this.NbPoints = PPRankingUser.NbPoints.Value;
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

        public PlayerAllTimeCard(AllTimeTotalRanking PPRankingUser)
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

                this.Cmid = PPRankingUser.CMID;
                this.Ranking = (int)PPRankingUser.Ranknum;
                this.MemberSince = PPRankingUser.MemberSince;

                if (PPRankingUser.KDRatio != null)
                {
                    this.KDRatio = PPRankingUser.KDRatio.Value;
                }

                if (PPRankingUser.NbPoints != null)
                {
                    this.NbPoints = PPRankingUser.NbPoints.Value;
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