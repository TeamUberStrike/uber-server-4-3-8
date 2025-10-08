
namespace Cmune.DataCenter.Common.Entities
{
    [System.Serializable]
    public class BundleItemView
    {
        public int BundleId { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public BuyingDurationType Duration { get; set; }
    }
}