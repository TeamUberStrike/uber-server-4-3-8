using System;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class MemberAuthenticationResultView
    {
        #region Properties

        public MemberAuthenticationResult MemberAuthenticationResult { get; set; }
        public MemberView MemberView { get; set; }
        public PlayerStatisticsView PlayerStatisticsView { get; set; }
        public DateTime ServerTime { get; set; }
        public bool IsAccountComplete { get; set; }
        public bool IsTutorialComplete { get; set; }

        public WeeklySpecialView WeeklySpecial { get; set; }
        public LuckyDrawUnityView LuckyDraw { get; set; }

        #endregion
    }
}