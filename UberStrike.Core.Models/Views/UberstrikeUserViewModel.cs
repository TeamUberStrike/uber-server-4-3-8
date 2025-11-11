using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class UberstrikeUserViewModel
    {
        public MemberView CmuneMemberView { get; set; }
        public UberstrikeMemberView UberstrikeMemberView { get; set; }
    }
}
