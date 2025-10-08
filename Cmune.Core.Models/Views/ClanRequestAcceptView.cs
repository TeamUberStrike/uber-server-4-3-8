
using Cmune.DataCenter.Common.Entities;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class ClanRequestAcceptView
    {
        public int ActionResult { get; set; }
        public int ClanRequestId { get; set; }
        public ClanView ClanView { get; set; }
    }
}