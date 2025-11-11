
using Cmune.Core.Types.Attributes;
using Cmune.Core.Types;

namespace Cmune.Realtime.Common
{
    [ExtendableEnumBounds(0, byte.MaxValue)]
    public class CmuneEventCodes : ExtendableEnum<byte>
    {
        public const byte Standard = 0;

        public const byte JoinEvent = 1;
        public const byte LeaveEvent = 2;

        public const byte GameListInit = 3;
        public const byte GameListUpdate = 4;
        public const byte GameListRemoval = 5;

        public const byte OldMessage = 100;
    }

    [ExtendableEnumBounds(0, byte.MaxValue)]
    public class CmuneOperationCodes : ExtendableEnum<byte>
    {
        public const byte MessageToApplication = 66;

        //use this operation right after the connection is establiahed to specify your peer type on the server
        //possible types are GamePeer, CommServer, GamerServer, LobbyServer
        //public const byte PeerSpecification = 77;

        public const byte MessageToPlayer = 80;
        public const byte MessageToAll = 81;
        public const byte MessageToServer = 82;
        public const byte MessageToOthers = 83;

        public const byte PhotonGameJoin = 88;
        public const byte PhotonGameLeave = 89;

        public const byte GameListUpdate = 90;
        public const byte GameListRemoval = 91;
        public const byte RegisterGameServer = 92;

        //public const byte QueryServerLoad = 93;
        //public const byte RoomRequest = 94;
        //public const byte BanPlayerFromRoom = 95;
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum PeerType 
    {
        None = 0,
        GamePeer = 1,
        CommServer = 2,
        GamerServer = 3,
        LobbyServer = 4,
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum CreateOperationParameter
    {
        ModeID,
        DataRaw
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum JoinOperationParameter
    {
        ModeID,
        DataRaw
    }

    /// <summary>
    /// Parameter keys are used as event-keys, operation-parameter keys and operation-return keys alike.
    /// The values are partly taken from Exit Games Photon, which contains many more keys.
    /// </summary>
    [ExtendableEnumBounds(0, byte.MaxValue)]
    public class ParameterKeys : ExtendableEnum<byte>
    {
        public const byte GameId = 4;
        public const byte ActorNr = 9; // used as op-key and ev-key
        public const byte Actors = 11;
        public const byte Data = 42;
        public const byte InvocationId = 61;

        public const byte MethodId = 100;
        public const byte InstanceId = 101;

        public const byte ActorId = 102;
        public const byte Bytes = 103;

        public const byte RoomId = 120;

        public const byte LobbyRoomUpdate = 122;
        public const byte LobbyRoomDelete = 123;

        //SERVER RETURN VALUES
        public const byte InitRoom = 200;
        public const byte ServerTicks = 201;

        public const byte AccessLevel = 205;
        public const byte Cmid = 206;
    }
}
