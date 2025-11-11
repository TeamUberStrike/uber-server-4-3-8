using System;
using System.Collections.Generic;
using System.Reflection;
using Cmune.Core.Types;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ServerNetworkClass : CmuneNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rmi"></param>
        /// <param name="room"></param>
        protected ServerNetworkClass(RemoteMethodInterface rmi, CmuneRoom room)
        {
            _myMethods = new Dictionary<int, MethodInfo>();

            _myType = this.GetType();
            _rmi = rmi;
            _room = room;

            SubscribeToRoomMessages();

            //check if the class is a static network class. In this case we don't have to wait for a unique ID from the server but can intialize directly
            short netid;
            if (TryGetStaticNetworkClassId(out netid))
            {
                _rmi.RegisterGlobalNetworkClass(this, netid);
            }
            else
            {
                throw new Exception("New Instance of StaticNetworkSynchronization without Attribute 'NetworkClass' assigned.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SubscribeToRoomMessages()
        {
            if (!_hasSubscribedToMessages)
            {
                _hasSubscribedToMessages = true;

                //if (CmuneDebug.IsDebugEnabled)
                //    CmuneDebug.Log("SubscribeToRoomMessages -> " + _myType.Name);

                //reflect all internal com methods and initialize event handling
                InitMyInternalMethods();

                //subsribe to the message channel for internal asynchronous event communication
                _room.SubscribeToRoomMessages(ProcessMessages);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                _room.UnsubscribeToRoomMessages(ProcessMessages);
                _rmi.UnregisterNetworkClass(this);
            }

            base.Dispose(dispose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        [RoomMessage(RoomMessageType.RemovePeerFromGame)]
        protected abstract void OnPlayerLeftGame(int actorID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="parameter"></param>
        public void SendMethodToPlayer(int actorId, byte localAddress, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToActor(actorId, true, NetworkID, localAddress, new SendParameters(), args);
            }
            else
            {
                CmuneDebug.LogError(string.Format("Send Message failed because instance was not initialized yet!"));
            }
        }

        #region INTERNAL MESSAGING

        private void InitMyInternalMethods()
        {
            List<MemberInfoMethod<RoomMessageAttribute>> info = AttributeFinder.GetMethods<RoomMessageAttribute>(_myType);
            foreach (MemberInfoMethod<RoomMessageAttribute> p in info)
            {
                _myMethods[p.Attribute.ID] = p.Method;
            }
        }

        private void ProcessMessages(IMessage message)
        {
            CallInternalMethod(message.MessageID, message.Arguments);
        }

        private void CallInternalMethod(int internalID, params object[] args)
        {
            MethodInfo info;

            if (_myMethods.TryGetValue(internalID, out info))
            {
                try
                {
                    _myType.InvokeMember(info.Name, CmuneNetworkClass.Flags, null, this, args);
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("Exception when calling internal function {0}:{1}() by reflection: {2}", _myType.Name, info.Name, e.Message);

                    if (args != null)
                        CmuneDebug.LogError("Call with {0} Arguments: {1}", args.Length, CmunePrint.Types(args));
                    else
                        CmuneDebug.LogError("Call with NULL Argument");
                }
            }
        }

        #endregion

        #region Fields

        private bool _hasSubscribedToMessages = false;

        protected CmuneRoom _room;

        protected RemoteMethodInterface _rmi;

        private Dictionary<int, MethodInfo> _myMethods;

        private Type _myType;

        #endregion
    }
}