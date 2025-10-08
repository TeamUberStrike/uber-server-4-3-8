using System;
using System.Collections.Generic;
using System.Text;
using Cmune.Realtime.Common;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Client
{
    public class RemoteMethodInterface
    {
        public RemoteMethodInterface(NetworkMessenger messenger)
        {
            _messenger = messenger;

            _registeredClasses = new Dictionary<short, INetworkClass>();

            _waitingClasses = new Dictionary<int, RegistrationJob>();

            _networkInstantiatedObjects = new Dictionary<int, short>();

            _incomingRpcBuffer = new Dictionary<short, Queue<RemoteProcedureCall>>();

            _syncCenter = new SynchronizationCenter(this);
        }

        public void RegisterGlobalNetworkClass(INetworkClass netClass, short networkID)
        {
            if (!netClass.IsGlobal)
            {
                CmuneDebug.LogError("Use RegisterMonoNetworkClass(ClientNetworkClass) for Network classes with UNSET network ID");
            }
            else
            {
                if (networkID == NetworkClassID.ClientSyncCenter || networkID < 0)
                {
                    _registeredClasses[networkID] = netClass;

                    netClass.Initialize(networkID);
                }
                else
                {
                    _waitingClasses[netClass.InstanceID] = new RegistrationJob(netClass, networkID);

                    RegisterAllClasses();
                }
            }
        }

        public void RegisterMonoNetworkClass(INetworkClass netClass)
        {
            if (netClass.IsGlobal)
            {
                CmuneDebug.LogError("Use RegisterNetworkClass(ClientNetworkClass,int,short) for Network classes with FIXED network ID");
            }
            else
            {
                short networkID;
                if (_networkInstantiatedObjects.TryGetValue(netClass.InstanceID, out networkID))
                {
                    RegisterNetworkClass(netClass, networkID);

                    _networkInstantiatedObjects.Remove(netClass.InstanceID);
                }
                else
                {
                    _waitingClasses[netClass.InstanceID] = new RegistrationJob(netClass);

                    RegisterAllClasses();
                }
            }
        }

        public void RegisterAllClasses()
        {
            foreach (RegistrationJob r in _waitingClasses.Values)
            {
                if (_messenger.PeerListener.HasJoinedRoom && !r.IsRequestSent)
                {
                    if (r.NetworkID.HasValue)
                    {
                        _messenger.SendMessageToServer(NetworkClassID.ServerSyncCenter, ServerSyncCenterRPC.RegisterStaticNetworkClass, _messenger.PeerListener.ActorIdSecure, r.LocalID, r.NetworkID);
                        r.IsRequestSent = true;
                    }
                    else
                    {
                        CmuneDebug.LogError("RegisterAllClasses failed because NetworkClass is not static");
                    }
                }
                else
                {
                    //CmuneDebug.LogError("Failed to process RegistrationJob:\nHasJoinedRoom = " + _messenger.PeerListener.HasJoinedRoom + "\nIsRequestSent = " + r.IsRequestSent);
                }
            }
        }

        public void UnregisterAllClasses()
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("{0} - UnregisterAllClasses", _messenger.PeerListener.SessionID);

            List<INetworkClass> permanentClasses = new List<INetworkClass>();

            foreach (RegistrationJob j in _waitingClasses.Values)
            {
                j.IsRequestSent = false;
            }

            foreach (INetworkClass c in _registeredClasses.Values)
            {
                if (c.IsGlobal)
                {
                    if (c.NetworkID == NetworkClassID.ClientSyncCenter || c.NetworkID < 0)
                    {
                        permanentClasses.Add(c);
                    }
                    else if (!_waitingClasses.ContainsKey(c.InstanceID))
                    {
                        _waitingClasses.Add(c.InstanceID, new RegistrationJob(c, c.NetworkID));
                        c.Uninitialize();
                    }
                }
                else if (!_waitingClasses.ContainsKey(c.InstanceID))
                {
                    _waitingClasses.Add(c.InstanceID, new RegistrationJob(c));
                    c.Uninitialize();
                }
            }

            //the SyncCenter is the ONLY class that doesn't need any registration and has the position of a guarantueed connection point
            _registeredClasses.Clear();
            foreach (INetworkClass c in permanentClasses)
            {
                _registeredClasses.Add(c.NetworkID, c);
            }

            _incomingRpcBuffer.Clear();
        }

        internal void RecieveRegistrationConfirmation(int instanceID, short networkID)
        {
            RegistrationJob job;
            if (_waitingClasses.TryGetValue(instanceID, out job))
            {
                if (job.NetworkClass != null)
                {
                    try
                    {
                        if (CmuneNetworkState.DebugMessaging)
                            CmuneDebug.Log("ServerStaticRegistrationConfirmation " + networkID);

                        RegisterNetworkClass(job.NetworkClass, networkID);
                    }
                    catch (Exception e)
                    {
                        CmuneDebug.LogError("Failed Registering Static NetworkClass ({0}): {1}", networkID, e.Message);
                        CmuneDebug.LogError("Registered Classes: {0}", CmunePrint.Values(_registeredClasses.Keys));
                    }
                    finally
                    {
                        _waitingClasses.Remove(instanceID);
                    }
                }
            }
            else
            {
                CmuneDebug.LogError(string.Format("RecieveRegistrationConfirmation({0}, {1}) failed because Instance was deleted!", instanceID, networkID));
            }
        }

        public void RegisterNetworkClass(INetworkClass netClass, short networkID)
        {
            _registeredClasses.Add(networkID, netClass);

            netClass.Initialize(networkID);

            ExcecuteWaitingFunctionCalls(netClass);
        }

        internal void ExcecuteWaitingFunctionCalls(INetworkClass netClass)
        {
            Queue<RemoteProcedureCall> rpc;
            if (_incomingRpcBuffer.TryGetValue(netClass.NetworkID, out rpc))
            {
                while (rpc.Count > 0)
                {
                    RemoteProcedureCall r = rpc.Dequeue();
                    netClass.CallMethod(r.FunctionID, r.Args);
                }
            }
        }

        public void DisposeNetworkClass(INetworkClass netClass)
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("DisposeNetworkClass " + netClass.GetType().Name);

            //remove all references to this instance from the waiting list
            _waitingClasses.Remove(netClass.InstanceID);

            //remove all references to this instance from the registered list
            _registeredClasses.Remove(netClass.NetworkID);

            //remove from netInstance list
            _networkInstantiatedObjects.Remove(netClass.InstanceID);

            //clear waiting function calls for this instance
            _incomingRpcBuffer.Remove(netClass.NetworkID);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            foreach (INetworkClass j in _registeredClasses.Values)
                if (j.IsGlobal)
                    b.AppendFormat("REG GL {0} with LID {1} and NID {2}\n", j.GetType().Name, j.InstanceID, j.NetworkID);
                else
                    b.AppendFormat("REG {0} with LID {1} and NID {2}\n", j.GetType().Name, j.InstanceID, j.NetworkID);

            foreach (RegistrationJob j in _waitingClasses.Values)
            {
                if (j.NetworkClass.IsGlobal)
                    b.AppendFormat("WAIT GL {0} with LID {1} ({2})\n", j.NetworkClass.GetType().Name, j.LocalID, j.IsRequestSent);
                else
                    b.AppendFormat("WAIT {0} with LID {1} ({2})\n", j.NetworkClass.GetType().Name, j.LocalID, j.IsRequestSent);
            }
            foreach (KeyValuePair<int, short> j in _networkInstantiatedObjects)
                b.AppendFormat("Net Instance with LID {0} and NID {1}\n", j.Key, j.Value);

            return b.ToString();
        }

        public string GetAddress(short networkID, byte address)
        {
            string name = string.Format("<{0}.{1}>", networkID, address);

            if (_registeredClasses.ContainsKey(networkID) && _registeredClasses[networkID] != null)
            {
                INetworkClass i = _registeredClasses[networkID];
                name = string.Format("{0}.{1}(registered)", i.GetType().Name, i.GetMethodName(address));
            }
            else
            {
                //RegistrationJob2 job = _waitingClasses2.Find(j => j.StaticNetworkID == networkID);
                //if (job != null && job.NetworkClass != null)
                //{
                //    name = string.Format("{0}.{1}(waiting)", job.Instance.GetType().Name, job.Instance.GetMethodName(address));
                //}
                name = string.Format("{0}.{1}(waiting)", "asdf", "asdf");
            }

            return name;
        }

        /// <summary>
        /// Remove all old Network registrations except: [ registrations,  pending jobs]
        /// </summary>
        public void Clear()
        {
            if (CmuneNetworkState.DebugMessaging)
                CmuneDebug.Log("Clear RMI");

            _networkInstantiatedObjects.Clear();

            _incomingRpcBuffer.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="net"></param>
        /// <returns></returns>
        public bool TryGetNetworkClassWithID(short id, out INetworkClass net)
        {
            return _registeredClasses.TryGetValue(id, out net);
        }

        /// <summary>
        /// We save the globally valid network ID for this network instance in a list
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="networkID"></param>
        internal void AddRemoteNetworkClassInstance(int instanceID, short networkID)
        {
            //CmuneDebug.Log(string.Format("Recieve NetworkID for Mono NetworkClass {0}/{1}!", instanceID, networkID));

            _networkInstantiatedObjects.Add(instanceID, networkID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        internal void RecieveMessage(short networkID, byte functionID, object[] args)
        {
            if (_registeredClasses.ContainsKey(networkID))
            {
                //call the local method invokation by reflection
                _registeredClasses[networkID].CallMethod(functionID, args);
            }
            else
            {
                if (!_incomingRpcBuffer.ContainsKey(networkID))
                    _incomingRpcBuffer.Add(networkID, new Queue<RemoteProcedureCall>());

                if (_incomingRpcBuffer[networkID].Count < IncomingMessageQueueLimit)
                {
                    _incomingRpcBuffer[networkID].Enqueue(new RemoteProcedureCall(functionID, args));

                    CmuneDebug.LogWarning(string.Format("Recieved Message {0} for NetworkID {1} but instance not registered. #{2}", functionID, networkID, _incomingRpcBuffer[networkID].Count));
                }
                else
                {
                    throw new CmuneException(string.Format("Recieved Message {0} for NetworkID {1} but instance not registered. QUEUE FULL!", functionID, networkID));
                }
            }
        }

        #region PROPERTIES

        public NetworkMessenger Messenger
        {
            get { return _messenger; }
        }

        public ICollection<INetworkClass> RegisteredClasses
        {
            get { return _registeredClasses.Values; }
        }

        public ICollection<RegistrationJob> RegistrationJobs
        {
            get { return _waitingClasses.Values; }
        }

        public ICollection<short> NetworkInstantiatedObjects
        {
            get { return _networkInstantiatedObjects.Values; }
        }

        #endregion

        #region FIELDS
        public const int IncomingMessageQueueLimit = 100;

        private Dictionary<short, INetworkClass> _registeredClasses;

        private Dictionary<short, Queue<RemoteProcedureCall>> _incomingRpcBuffer;

        private Dictionary<int, RegistrationJob> _waitingClasses;

        private Dictionary<int, short> _networkInstantiatedObjects;

        private NetworkMessenger _messenger;

        private SynchronizationCenter _syncCenter;
        #endregion

        private class RemoteProcedureCall
        {
            public RemoteProcedureCall(byte functionID, object[] args)
            {
                FunctionID = functionID;

                if (args != null)
                    Args = args;
                else
                    Args = new object[0];
            }

            public override string ToString()
            {
                return string.Format("RPC {0} with {1} args", FunctionID, Args.Length);
            }

            public byte FunctionID;
            public object[] Args;
        }

        public class RegistrationJob
        {
            public RegistrationJob(INetworkClass netClass) { NetworkClass = netClass; LocalID = netClass.InstanceID; }
            public RegistrationJob(INetworkClass netClass, short networkID) : this(netClass) { NetworkID = networkID; }

            public INetworkClass NetworkClass;
            public bool IsRequestSent = false;
            public short? NetworkID;
            public int LocalID;

            public override string ToString()
            {
                if (NetworkClass != null)
                    return string.Format("Iid {0}, Nid {1}, ReqSent {2}", NetworkClass.InstanceID, NetworkID ?? -1, IsRequestSent);
                else
                    return string.Format("RegistrationJob for {0} is NULL", LocalID);
            }
        }
    }
}
