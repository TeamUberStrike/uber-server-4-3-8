using System;
using System.Linq;
using System.Web;
using UberStrike.DataCenter.DataAccess;

namespace UberStrike.DataCenter.Business.Views
{
    public class PlayerCardFacebook : PlayerCard
    {
        #region Private properties

        private string _color;
        private string _realName;
        private string _status;
        private string _pictureUrl;
        private string _jsFunctionAjax;

        #endregion

        #region Public properties

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string RealName
        {
            get { return _realName; }
            set { _realName = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string PictureUrl
        {
            get { return _pictureUrl; }
            set { _pictureUrl = value; }
        }

        public string JsFunctionAjax
        {
            get { return _jsFunctionAjax; }
            set { _jsFunctionAjax = value; }
        }

        #endregion

        #region Constructors

        public PlayerCardFacebook() : base()
        {
        }

        /// <summary>
        /// PP ranking with Stats args
        /// </summary>
        /// <param name="PPRankingUser"></param>
        /// <param name="color"></param>
        /// <param name="status"></param>
        /// <param name="jsFunctionAjax"></param>
        public PlayerCardFacebook(AllTimeTotalRanking PPRankingUser, String color, String status, String jsFunctionAjax)
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
                this.Color = color;
                this.Status = status;
                this.JsFunctionAjax = jsFunctionAjax;

                if (PPRankingUser.TagName != null)
                {
                    if (!HttpUtility.HtmlEncode(PPRankingUser.TagName).Equals(""))
                    {
                        this.TagName = "[" + HttpUtility.HtmlEncode(PPRankingUser.TagName) + "]";
                    }
                }
            }
        }
        /// <summary>
        /// Cmune user or PP user
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="isCmuneUser"></param>
        public PlayerCardFacebook(int UserID, Boolean isCmuneUser)
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
                        if (!HttpUtility.HtmlEncode(PPRankingUser.TagName).Equals(""))
                        {
                            this.TagName = "[" + HttpUtility.HtmlEncode(PPRankingUser.TagName) + "]";
                        }
                    }
                }
            }
        }

        #endregion
    }
}
