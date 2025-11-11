using System;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace UberStrike.DataCenter.Business.Views
{
    public class PlayerCard : PlayerCardView
    {
        #region Constructors

        public PlayerCard() : base()
        {
        }

        public PlayerCard(string name, int splats, int splatted, string precision, int ranking, string tagName)
            : base(name, splats, splatted, precision, ranking, tagName)
        {
        }

        //public PlayerCard(Paradise_FacebookTopRank_SResult player)
        //{
        //    this.Name = player.Name;
        //    this.Splats = player.Splats;
        //    this.Splatted = player.Splatted;
        //    if (player.Shots != 0)
        //        this.Precision = (player.Hits * 100 / (double)player.Shots).ToString("F1");
        //    else
        //        this.Precision = "-";
        //    this.Ranking = (int)player.rankNum;
        //}

        //public PlayerCard(Paradise_UserRank_Cmid_SResult player)
        //{
        //    this.Name = player.Name;
        //    this.Splats = player.Splats;
        //    this.Splatted = player.Splatted;
        //    if (player.Shots != 0)
        //    {
        //        this.Precision = (player.Hits * 100 / (double)player.Shots).ToString("F1");
        //    }
        //    else
        //    {
        //        this.Precision = "-";
        //    }
        //    this.Ranking = (int)player.rankNum;
        //}

        public PlayerCard(int UserID, Boolean isCmuneUser)
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

                    if (PPRankingUser.TagName != null)
                    {
                        if (!PPRankingUser.TagName.IsNullOrFullyEmpty())
                        {
                            this.TagName = "[" + PPRankingUser.TagName + "]";
                        }
                    }
                }
            }
        }

        public PlayerCard(AllTimeTotalRanking PPRankingUser)
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

                if (PPRankingUser.TagName != null)
                {
                    if (!PPRankingUser.TagName.IsNullOrFullyEmpty())
                    {
                        this.TagName = "[" + PPRankingUser.TagName + "]";
                    }
                }
            }
        }

        #endregion
    }
}
