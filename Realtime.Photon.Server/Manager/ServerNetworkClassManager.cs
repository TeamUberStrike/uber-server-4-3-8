using System;
using System.Collections.Generic;
using Cmune.Realtime.Photon.Server.Rooms;

namespace Cmune.Realtime.Photon.Server
{
    public sealed class ServerNetworkClassManager : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="room"></param>
        public ServerNetworkClassManager(RemoteMethodInterface rmi, CmuneRoom room)
        {
            _currentRoom = room;

            _rmi = rmi;

            _networkClassInstances = new Dictionary<short, ServerNetworkClass>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalNetworkID"></param>
        public bool AddClass(short id)
        {
            if (!_networkClassInstances.ContainsKey(id))
            {
                ServerNetworkClass c;
                if (Factory.CreateInstance(id, _rmi, _currentRoom, out c))
                {
                    _networkClassInstances.Add(id, c);
                    return true;
                }
                else return false;
            }
            else return true;
        }

        public ServerNetworkClass GetGlobalClass(short id)
        {
            ServerNetworkClass c = null;
            _networkClassInstances.TryGetValue(id, out c);
            return c;
        }

        #region Properties

        public static IServerNetworkClassFactory Factory
        {
            get
            {
                if (_factory == null) _factory = new DefaultServerNetworkClassFactory();
                return _factory;
            }
            set
            {
                _factory = value;
            }
        }

        #endregion

        #region Fields

        public static IServerNetworkClassFactory _factory;
        private Dictionary<short, ServerNetworkClass> _networkClassInstances;
        private RemoteMethodInterface _rmi;
        private CmuneRoom _currentRoom;

        #endregion

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                _currentRoom = null;
                foreach (ServerNetworkClass c in _networkClassInstances.Values)
                {
                    c.Dispose();
                }
            }

            _isDisposed = true;
        }
    }
}