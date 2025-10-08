using Cmune.DataCenter.Common;

namespace Cmune.Channels.Instrumentation.Models
{
    public class RollElementSummary
    {
        public PrizeElementView PrizeElement { get; set; }
        public int NumberOfRoll { get; set; }
        public int Price { get; set; }
        public int PointPrice { get; set; }
    }
}