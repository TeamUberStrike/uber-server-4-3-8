using System.Collections.Generic;

namespace UberStrike.Core.ViewModel
{
    [System.Serializable]
    public class PlaySpanHashesViewModel
    {
        public string MerchTrans { get; set; }
        public Dictionary<decimal, string> Hashes { get; set; }
    }
}
