using System;
using Cmune.Realtime.Common;

namespace Cmune.Realtime.Photon.Client
{
    public abstract class ClientNetworkClass : CmuneNetworkClass
    {
        private static int NextInstanceId = 0;

        protected ClientNetworkClass(RemoteMethodInterface rmi)
        {
            _rmi = rmi;
            _instanceID = ++NextInstanceId;

            //check if the class is a static network class. In this case we don't have to wait for a unique ID from the server but can intialize directly
            if (TryGetStaticNetworkClassId(out _networkClassID))
            {
                _rmi.RegisterGlobalNetworkClass(this, _networkClassID);
            }
            else
            {
                throw new Exception("New Instance of StaticNetworkClass without Attribute 'NetworkClass' assigned.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Leave()
        {
            if (_rmi != null && _rmi.Messenger != null && _rmi.Messenger.PeerListener.ActorIdSecure > 0)
                SendMethodToServer(RPC.Leave, _rmi.Messenger.PeerListener.ActorIdSecure);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool dispose)
        {
            _rmi.DisposeNetworkClass(this);

            base.Dispose(dispose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        protected void SendMethodToServer(byte localAddress, params object[] args)
        {
            _rmi.Messenger.SendMessageToServer(NetworkID, true, localAddress, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        protected void SendMethodToPlayer(int playerID, byte localAddress, params object[] args)
        {
            _rmi.Messenger.SendMessageToPlayer(playerID, NetworkID, localAddress, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        protected void SendMethodToAll(byte localAddress, params object[] args)
        {
            _rmi.Messenger.SendMessageToAll(NetworkID, localAddress, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        protected void SendUnreliableMethodToServer(byte localAddress, params object[] args)
        {
            _rmi.Messenger.SendMessageToServer(NetworkID, false, localAddress, args);
        }

        #region FIELDS

        protected RemoteMethodInterface _rmi;

        protected short _networkClassID = -1;

        #endregion
    }
}
