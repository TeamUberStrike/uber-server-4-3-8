using System.Collections.Generic;
using System.Linq;

namespace UberStrike.DataCenter.Common.Entities
{
    public class FacebookBasicStatisticView : EsnsBasicStatisticView
    {
        public long FacebookId { get; set; }
        public string FirstName { get; set; }
        string _picturePath;

        public string PicturePath
        {
            get { return _picturePath; }
            set
            {
                if (value.StartsWith("http:"))
                {
                    value = value.Replace("http:", "https:");
                }

                _picturePath = value;
            }
        }

        public FacebookBasicStatisticView(long facebookId, string firstName, string picturePath, string name, int xp, int level, int cmid)
            : base(name, xp, level, cmid)
        {
            this.FacebookId = facebookId;
            this.FirstName = firstName;
            this.PicturePath = picturePath;
        }

        public FacebookBasicStatisticView(long facebookId, string firstName, string picturePath)
            : base()
        {
            this.FacebookId = facebookId;
            this.FirstName = firstName;
            this.PicturePath = picturePath;
        }

        public FacebookBasicStatisticView()
            : base()
        {
            this.FacebookId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="views">The viewer should be the 1st element of the list</param>
        /// <param name="friendsDisplayedCount"></param>
        /// <returns></returns>
        public static List<FacebookBasicStatisticView> Rank(List<FacebookBasicStatisticView> views, int friendsDisplayedCount)
        {
            List<FacebookBasicStatisticView> res = new List<FacebookBasicStatisticView>();

            FacebookBasicStatisticView currentViewer = null;

            if (views.Count > 0)
            {
                currentViewer = views[0];
            }

            // 1st, let's do the ranking

            views = views.OrderByDescending(v => v.XP).ToList();
            int i = 1;

            foreach (FacebookBasicStatisticView view in views)
            {
                if (view.Cmid != 0)
                {
                    view.SocialRank = i;
                    i++;
                }
            }

            // We only need the 1st id of the list + the best friendsDisplayedCount ones

            res.Add(currentViewer);

            i = 0;
            int j = 0;

            while (j < friendsDisplayedCount && j < views.Count)
            {
                if (views[j].FacebookId != currentViewer.FacebookId)
                {
                    res.Add(views[j]);
                    i++;
                }

                j++;
            }

            // We need to complete the list in case the viewer is having less than friendsDisplayedCount playing UberStrike

            while (res.Count < friendsDisplayedCount + 1)
            {
                res.Add(new FacebookBasicStatisticView());
            }

            return res;
        }
    }
}