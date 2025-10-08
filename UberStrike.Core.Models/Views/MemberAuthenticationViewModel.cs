using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class MemberAuthenticationViewModel
    {
        public MemberAuthenticationResult MemberAuthenticationResult { get; set; }
        public MemberView MemberView { get; set; }
    }
}
