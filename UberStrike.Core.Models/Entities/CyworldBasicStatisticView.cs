using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class CyworldBasicStatisticView : EsnsBasicStatisticView
    {
        public int CyworldId { get; private set; }

        public CyworldBasicStatisticView(int cyworldId, string name, int xp, int level, int cmid)
            : base(name, xp, level, cmid)
        {
            this.CyworldId = cyworldId;
        }

        public CyworldBasicStatisticView(int cyworldId)
            : base()
        {
            this.CyworldId = cyworldId;
        }

        public CyworldBasicStatisticView()
            : base()
        {
            this.CyworldId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="views">The viewer should be the 1st element of the list</param>
        /// <param name="friendsDisplayedCount"></param>
        /// <returns></returns>
        public static List<CyworldBasicStatisticView> Rank(List<CyworldBasicStatisticView> views, int friendsDisplayedCount)
        {
            List<CyworldBasicStatisticView> res = new List<CyworldBasicStatisticView>();

            CyworldBasicStatisticView currentViewer = null;

            if (views.Count > 0)
            {
                currentViewer = views[0];
            }

            // 1st, let's do the ranking

            views = views.OrderByDescending(v => v.XP).ToList();
            int i = 1;

            foreach (CyworldBasicStatisticView view in views)
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
                if (views[j].CyworldId != currentViewer.CyworldId)
                {
                    res.Add(views[j]);
                    i++;
                }

                j++;
            }

            // We need to complete the list in case the viewer is having less than friendsDisplayedCount playing UberStrike

            while (res.Count < friendsDisplayedCount + 1)
            {
                res.Add(new CyworldBasicStatisticView());
            }

            return res;
        }
    }
}
