
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Client.Events;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Client
{
    /// <summary>
    /// The ClientSynchronizationCenter is the main hook between Server and Client.
    /// Instructions that effect the whole 3D enviroment (loading and deleting of objects) are
    /// sent to the ClientSynchronizationCenter and processed here. The class has a static global 
    /// Network Address.
    /// </summary>
    [NetworkClass(NetworkClassID.ClientSyncCenter)]
    public class SynchronizationCenter : ClientNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rmi"></param>
        public SynchronizationCenter(RemoteMethodInterface rmi)
            : base(rmi)
        {
        }

        /// <summary>
        /// Operation called by the SERVER to assign a public NetworkID to a class instance.
        /// This call finalizes the registration process.
        /// </summary>
        /// <param name="waitingID">The local temporary id for an unregistered network class</param>
        /// <param name="networkID">The global unique network ID for a class, waiting to be registered</param>
        [NetworkMethod(ClientSyncCenterRPC.RecieveNetworkID)]
        protected void OnRecieveNetworkID(int instanceID, short networkID)
        {
            //CmuneDebug.LogFormat("OnRecieveNetworkID({0},{1}) RMI ok: {2}", instanceID, networkID, (_rmi != null));
            _rmi.RecieveRegistrationConfirmation(instanceID, networkID);
        }

        [NetworkMethod(ClientSyncCenterRPC.RefreshMemberInfo)]
        protected void OnRefreshMemberInfo()
        {
            //CmuneEventHandler.Route(new AutoLoginEvent());
        }

        [NetworkMethod(ClientSyncCenterRPC.KickPlayer)]
        protected void OnKickPlayer(string message)
        {
            CmuneDebug.LogError("Kick Reason: " + message);
            CmuneEventHandler.Route(new PlayerBanEvent(message));
        }
    }

    public class ObjectInstantiatedEvent// : IEventMessage
    {
        public INetworkClass sync;
    }
}