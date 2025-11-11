using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
    [Serializable]
    public class MysteryBoxWonItemUnityView
    {
        public int ItemIdWon { get; set; }
        public int CreditWon { get; set; }
        public int PointWon { get; set; }
    }
}
