using System;
using System.Collections.Generic;
using System.Reflection;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using Cmune.Core.Types;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// To call a method function, provide a [short] ID to retrieve the instance, provide a [byte] ID for the method.
    /// </summary>
    public abstract class CmuneNetworkClass : INetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        protected CmuneNetworkClass()
        {
            _myType = this.GetType();

            _instanceID = 0;

            _lookupNameIndex = new Dictionary<string, byte>();

            _lookupIndexMethod = new Dictionary<byte, System.Reflection.MethodInfo>();

            AttributeFinder.FindNetworkMethods(_myType, ref _lookupNameIndex, ref _lookupIndexMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Initialize(short id)
        {
            _netid = id;

            OnInitialized();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Uninitialize()
        {
            OnUninitialized();

            _netid = null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnInitialized() { CastEvent(EventType.Initialized); }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnUninitialized() { CastEvent(EventType.Uninitialized); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void SubscribeToEvents(Action<int> callback) { _castEvents += callback; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void UnsubscribeToEvents(Action<int> callback) { _castEvents -= callback; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventCode"></param>
        protected void CastEvent(int eventCode)
        {
            if (_castEvents != null) _castEvents(eventCode);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                Uninitialize();

                _castEvents = null;
            }

            _isDisposed = true;
        }

        protected bool _isDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool HasMethod(byte address)
        {
            return _lookupIndexMethod.ContainsKey(address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string GetMethodName(byte address)
        {
            MethodInfo info;
            if (_lookupIndexMethod.TryGetValue(address, out info))
            {
                return info.Name;
            }
            else
            {
                CmuneDebug.LogError("GetMethodName({0}) failed because not found: {1}", address, CmunePrint.Dictionary(_lookupIndexMethod));
                return string.Format("<{0}>", address);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netid"></param>
        /// <returns></returns>
        protected virtual bool TryGetStaticNetworkClassId(out short netid)
        {
            object[] all = _myType.GetCustomAttributes(typeof(NetworkClassAttribute), true);
            if (all.Length > 0)
            {
                netid = ((NetworkClassAttribute)all[0]).ID;
                return true;
            }
            else
            {
                netid = -1;
                return false;
            }
        }

        /// <summary>
        /// Invoke method by reflection
        /// 
        /// Exceptions:
        ///  System.MissingMethodException
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public void CallMethod(byte localAddress, params object[] args)
        {
            MethodInfo info;
            if (_lookupIndexMethod.TryGetValue(localAddress, out info))
            {
                try
                {
                    _myType.InvokeMember(info.Name, Flags, null, this, args);
                }
                catch (MissingMethodException)
                {
                    CmuneDebug.LogWarning("{0}:CallMethod('{1}') failed when called with arguments: {2}", this.GetType(), localAddress, CmunePrint.Types(args));
                    throw;
                }
                catch (Exception e)
                {
                    if (args != null)
                        CmuneDebug.LogWarning("Method with address '{0}' was called with {1} arguments: {2}", localAddress, args.Length, CmunePrint.Types(args));
                    else
                        CmuneDebug.LogWarning("Method with address '{0}' was called NULL argument", localAddress);

                    throw CmuneDebug.Exception("Exception when calling {0}:{1}() by reflection:\n>{2}\n{3}", _myType.Name, info.Name, e.InnerException.Message, e.InnerException.StackTrace);
                }
            }
            else
            {
                CmuneDebug.LogError("{0}:CallMethod failed because local address '{1}' not linked to a function!", _myType.Name, localAddress);
            }
        }

        #region PROPERTIES

        public short NetworkID
        {
            get { return _netid.HasValue ? _netid.Value : (short)-1; }
        }

        public int InstanceID
        {
            get { return _instanceID; }
        }

        public bool IsInitialized
        {
            get { return _netid.HasValue; }
        }

        public bool IsGlobal
        {
            get { return true; }
        }

        #endregion

        #region FIELDS

        private Action<int> _castEvents;

        public static readonly BindingFlags Flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected Dictionary<string, byte> _lookupNameIndex;

        protected Dictionary<byte, System.Reflection.MethodInfo> _lookupIndexMethod;

        protected int _instanceID;

        private short? _netid;

        private Type _myType;

        #endregion

        public class EventType : ExtendableEnum<int>
        {
            public const int Uninitialized = 0;
            public const int Initialized = 1;
        }
    }
}