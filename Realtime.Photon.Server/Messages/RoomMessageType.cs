
using Cmune.Realtime.Common;
using Cmune.Core.Types.Attributes;
using Cmune.Core.Types;

namespace Cmune.Realtime.Photon.Server
{
    [ExtendableEnumBounds(1, 50)]
    public class RoomMessageType : ExtendableEnum<int>
    {
        public const int RemovePeerFromGame = 1;
        public const int MemberLogin = 2;
        public const int MemberLogoff = 3;
        public const int AddedPeerToGame = 4;

        public const int GameUpdated = 10;
        public const int GameRemoved = 11;

        public const int GameServerDown = 13;
        public const int KickPlayer = 14;
        public const int CustomMessage = 15;
        public const int MutePlayer = 16;
    }
}
