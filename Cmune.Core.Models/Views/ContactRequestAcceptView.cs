
using Cmune.DataCenter.Common.Entities;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class ContactRequestAcceptView
    {
        public int ActionResult { get; set; }
        public int RequestId { get; set; }
        public PublicProfileView Contact { get; set; }
    }
}