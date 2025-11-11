
using Cmune.Core.Types;
using Cmune.Core.Types.Attributes;

namespace Cmune.Realtime.Common
{
    ///<summary>
    /// 0-10
    /// </summary>
    [ExtendableEnumBounds(1, 10)]
    public class NetworkClassID : ExtendableEnum<short>
    {
        public const short ClientSyncCenter = 1;
        public const short ServerSyncCenter = 2;

        public const short LobbyCenter = 3;
        public const short CommCenter = 4;
    }

    [ExtendableEnumBounds(0, 99)]
    public class StaticRoomID : ExtendableEnum<int>
    {
        public const int Auto = 0;

        //tbr
        public const int Test = 1;

        public const int LobbyCenter = 66;
        public const int CommCenter = 88;
    }
}