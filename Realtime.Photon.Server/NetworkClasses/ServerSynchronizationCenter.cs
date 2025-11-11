using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Synchronization;
using Cmune.Util;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// The Server Synchronization Center is the Main Anchor for any Remote Procedure Calls arriving on the Server.
    /// 
    /// </summary>
    [NetworkClass(NetworkClassID.ServerSyncCenter)]
    public sealed class ServerSynchronizationCenter : ServerNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="room"></param>
        public ServerSynchronizationCenter(RemoteMethodInterface regCenter, CmuneRoom room)
            : base(regCenter, room)
        {
            _center = regCenter;

            _worldManager = new ServerRoomObjectManager(room);

            _classManager = new ServerNetworkClassManager(regCenter, room);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderID"></param>
        public void InitRoomOnClient(int senderID)
        {
            OnInitializeRoomOnClient(senderID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorID"></param>
        protected override void OnPlayerLeftGame(int actorID)
        {
            foreach (RoomObject obj in _worldManager.GetPlayerDependentObjects(actorID))
            {
                SendMethodToAll(ClientSyncCenterRPC.RemoveInstanceWithID, obj.NetworkID);

                CmuneDebug.Log("Remove player {0} dependant object with id {1}", actorID, obj.NetworkID);

                _worldManager.RemoveObject(obj.NetworkID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="localID"></param>
        /// <param name="networkID"></param>
        [NetworkMethod(ServerSyncCenterRPC.RegisterStaticNetworkClass)]
        private void OnRegisterGlobalNetworkClass(int peerID, int localID, short networkID)
        {
            //safe code - makes sure that we c=don't have static network classes on the client without a counterpart on the server
            //if (_classManager.AddClass(networkID))
            //{
            //    _room.NewMessageToActor(
            //        peerID,
            //        NetworkClassID.ClientSyncCenter,
            //        ClientSyncCenterRPC.RecieveNetworkID,
            //        localID,
            //        networkID);
            //}

            //unsafe code - allows static network classes on the client
            _classManager.AddClass(networkID);
            _room.NewMessageToActor(
                peerID,
                true,
                NetworkClassID.ClientSyncCenter,
                ClientSyncCenterRPC.RecieveNetworkID,
                new SendParameters(),
                localID,
                networkID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="localID"></param>
        /// <param name="assetID"></param>
        /// <param name="type"></param>
        /// <param name="transform"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [NetworkMethod(ServerSyncCenterRPC.RegisterMonoNetworkGameObject)]
        private void OnRegisterMonoNetworkGameObject(int peerID, int localID, int assetID, AssetType type, CmuneTransform transform, List<byte> config)
        {
            //WE DISABLE NET CLASS REGISTRATIONS UNTIL WE FIX THE MEM LEAK AND HAVE NETWORK PHYSICS BACK WORKING

            short networkID = _center.RegisterClientNetworkClass();

            ////Here add object into server list
            //if (_worldManager.AddObject(peerID, assetID, networkID, type, transform, config))
            //{
            //Register mono Class for creator
            _room.NewMessageToActor(
                peerID,
                true,
                NetworkClassID.ClientSyncCenter,
                ClientSyncCenterRPC.RecieveNetworkID,
                new SendParameters(),
                localID,
                networkID);

            //    //Create and Register mono Class for all players in the room
            //    _room.NewMessageToAllActorsExcept(
            //        peerID,
            //        NetworkClassID.ClientSyncCenter,
            //        ClientSyncCenterRPC.LoadAssetWithID,
            //        assetID, networkID,
            //        config, transform);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="networkID"></param>
        [NetworkMethod(ServerSyncCenterRPC.UnregisterNetworkClass)]
        private void OnUnregisterMonoNetworkGameObject(int peerID, short networkID)
        {
            //WE DISABLE NET CLASS REGISTRATIONS UNTIL WE FIX THE MEM LEAK AND HAVE NETWORK PHYSICS BACK WORKING

            //if (networkID >= RemoteMethodInterface.NetworkIDStart && _worldManager.IsOwnerOf(peerID, networkID))
            //{
            //    if (CmuneDebug.IsDebugEnabled)
            //        CmuneDebug.LogFormat("Unregister MonoNetworkGameObject {0} on request of {1} ", networkID, peerID);

            //    _worldManager.RemoveObject(networkID);

            //    SendMethodToAll(ClientSyncCenterRPC.RemoveInstanceWithID, networkID);
            //}
        }

        /// <summary>
        /// Send a message to the client to clean his enviroment and send the building instructions of all currently loaded objects
        /// </summary>
        /// <param name="peerID"></param>
        [NetworkMethod(ServerSyncCenterRPC.InitializeRoomOnClient)]
        private void OnInitializeRoomOnClient(int peerID)
        {
            //for each object in room
            foreach (RoomObject s in _worldManager.GetAllObjects())
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("- Send to client building instruction for item: " + s.AssetID);

                _room.NewMessageToActor(
                    peerID,
                    true,
                    NetworkClassID.ClientSyncCenter,
                    ClientSyncCenterRPC.LoadAssetWithID,
                    new SendParameters(),
                    s.AssetID, s.NetworkID, s.GetBytes(), s.Transform);
            }
        }

        /// <summary>
        /// Send a message to the client to clean his enviroment and send the building instructions of all currently loaded objects
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="networkID"></param>
        /// <param name="bytes"></param>
        [NetworkMethod(ServerSyncCenterRPC.SynchronizeProperties)]
        private void OnSynchronizeProperties(int peerID, short networkID, List<byte> bytes)
        {
            _worldManager.SynchronizeObjectFields(networkID, bytes);

            //make sure you don't pass a byte[] here but a list<byte>. 
            //If you want to pass a byte[] then cast it to (object) before calling the method 
            //otherwise it is treated as a number of single bytes instead of a list!
            _room.NewMessageToAllExceptActor(peerID, true, networkID, (byte)LocalAddress.SynchronizeProperties, new SendParameters(), bytes);
        }

        /// <summary>
        /// Send a message to the client to clean his enviroment and send the building instructions of all currently loaded objects
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="package"></param>
        [NetworkMethod(ServerSyncCenterRPC.NetworkPhysicsUpdate)]
        private void OnNetworkPhysicsUpdate(int senderID, NetworkPackage package)
        {
            _worldManager.UpdateObject(package);

            _room.NewMessageToAllExceptActor(senderID, true, package.netID, ServerSyncCenterRPC.NetworkPhysicsUpdate, new SendParameters(), package);
        }

        protected override void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                _classManager.Dispose();
            }

            base.Dispose(dispose);
        }

        #region Send Methods

        public void SendMethodToAll(byte localAddress, params object[] args)
        {
            _room.NewMessageToAll(
                true,
                NetworkClassID.ClientSyncCenter,
                localAddress,
                new SendParameters(),
                args);
        }

        #endregion

        #region Fields

        private RemoteMethodInterface _center;
        private ServerRoomObjectManager _worldManager;
        private ServerNetworkClassManager _classManager;

        #endregion
    }
}