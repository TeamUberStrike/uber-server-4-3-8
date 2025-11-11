
using Cmune.Core.Types.Attributes;
using Cmune.Core.Types;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// [1, 20]
    /// 'GlobalServerMethodID.ServerSynchronizationCenter' IDs
    /// Static addresses for all network methods of this class.
    /// These are used on the CLIENT to address a specific method
    /// </summary>
    [ExtendableEnumBounds(1, 20)]
    public class ServerSyncCenterRPC : ExtendableEnum<byte>
    {
        public const byte UnregisterNetworkClass = 1;        //Unregister any kind of class

        public const byte RegisterMonoNetworkGameObject = 2;  //Register a class of type GameObject   (triggers a routing of all calls to other clients)
        public const byte RegisterStaticNetworkClass = 3;     //Register a static class on the Server (triggers a routing of all calls to a server logic instance)

        public const byte InitializeRoomOnClient = 4;

        public const byte SynchronizeProperties = 5;
        public const byte NetworkPhysicsUpdate = 6;

        public const byte MailNotification = 7;

        //public const byte RegisterToLobbyUpdates = 8;
    }

    /// <summary>
    /// 'NetworkClass.Global.ClientSynchronizationCenter' IDs
    /// Static addresses for all network methods of this class.
    /// These are used on the SERVER to address a specific method
    /// </summary>
    [ExtendableEnumBounds(1, 20)]
    public class ClientSyncCenterRPC : ExtendableEnum<byte>
    {
        public const byte RecieveNetworkID = 1;
        public const byte LoadAssetWithID = 2;
        public const byte RemoveInstanceWithID = 3;
        public const byte RecieveStaticClassRegistration = 4;
        public const byte SynchronizeServerTime = 5;

        public const byte ClearRoom = 6;
        public const byte RoomInitialized = 7;
        public const byte RefreshMemberInfo = 8;

        public const byte KickPlayer = 9;
    }
}
