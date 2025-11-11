
namespace UberStrike.DataCenter.Common.Entities
{
    [System.Serializable]
    public class DailyPointsView
    {
        public DailyPointsView() { }

        public int Current { get; set; }
        public int PointsTomorrow { get; set; }
        public int PointsMax { get; set; }
    }
}