
using Cmune.Core.Types.Attributes;
using Cmune.Realtime.Common;

namespace UberStrike.Realtime.Common
{
    /// <summary>
    /// 
    /// </summary>
    [ExtendableEnumBounds(11, 30)]
    public class UberstrikeClassID : NetworkClassID
    {
        // public const short AdminControlCenter = 10;
        // public const short NoobsControlCenter = 11;

        public const short GameAICenter = 12;
    }

    /// <summary>
    /// 100 - 149
    /// Make sure not to collide with the GlobalNetworkID
    /// </summary>
    [ExtendableEnumBounds(100, 150)]
    public class GameModeID : UberstrikeClassID
    {
        public const short TeamDeathMatch = 100;
        public const short DeathMatch = 101;
        public const short FunMode = 102;
        public const short CooperationMode = 103;
        public const short CaptureTheFlag = 104;
        public const short LastManStanding = 105;
        public const short EliminationMode = 106;
        public const short ModerationMode = 107;
    }
}
