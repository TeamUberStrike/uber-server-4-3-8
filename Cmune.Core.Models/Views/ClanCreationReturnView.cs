
using Cmune.DataCenter.Common.Entities;

namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class ClanCreationReturnView
    {
        public int ResultCode { get; set; }
        public ClanView ClanView { get; set; }
    }
}