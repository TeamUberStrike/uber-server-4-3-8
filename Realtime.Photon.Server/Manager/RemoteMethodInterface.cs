using System;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteMethodInterface
    {
        public RemoteMethodInterface()
        {
            _registeredClasses = new Dictionary<short, INetworkClass>();
        }

        public short RegisterClientNetworkClass()
        {
            return ++_nextNetworkID;
        }

        public void UnregisterNetworkClass(INetworkClass net)
        {
            //nothing
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        public void RegisterGlobalNetworkClass(INetworkClass sync, short networkID)
        {
            try
            {
                _registeredClasses.Add(networkID, sync);

                sync.Initialize(networkID);
            }
            catch (Exception)
            {
                throw CmuneDebug.Exception("Trying to register static NetworkClass with ID {0} but ID already registered!", networkID);
            }
        }

        public bool TryGetNetworkClassWithID(short id, out INetworkClass net)
        {
            return _registeredClasses.TryGetValue(id, out net);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public void RecieveMessage(short networkID, byte localAddress, params object[] args)
        {
            INetworkClass c;
            if (_registeredClasses.TryGetValue(networkID, out c))
            {
                //call the local method invokation by reflection
                c.CallMethod(localAddress, args);
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning(string.Format("Recieved Message {0} but instance for NetworkID {1} not registered.", localAddress, networkID));
            }
        }

        /// <summary>
        /// We start to register custom client NetworkClasses beginning at ID 1001.
        /// That means that we have ID [0 - 1000] reserved for static NetworkClass IDs
        /// </summary>
        public static readonly short NetworkIDStart = 1000;

        private short _nextNetworkID = NetworkIDStart;

        protected Dictionary<short, INetworkClass> _registeredClasses;
    }
}
