
namespace UberStrike.Core.Models
{
    [System.Serializable]
    public class CommActorInfo : ActorInfo
    {
        public byte ModerationFlag { get; set; }
        public string ModInformation { get; set; }
    }
}