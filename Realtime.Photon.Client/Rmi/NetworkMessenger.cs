//#define DEEP_DEBUG

using System;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Util;
using ExitGames.Client.Photon;

namespace Cmune.Realtime.Photon.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class NetworkMessenger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="peerListener"></param>
        public NetworkMessenger(PhotonPeerListener peerListener)
        {
            _peerListener = peerListener;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendMessageToAll(short networkID, byte localAddress, params object[] args)
        {
            return SendMessageToAll(networkID, true, localAddress, args);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="networkID"></param>
        ///// <param name="localAddress"></param>
        ///// <param name="args"></param>
        //public bool SendMessageToAllIncludingMe(short networkID, bool isReliable, byte address, params object[] args)
        //{
        //    if (networkID > 0)
        //    {
        //        if (IsConnectionReady)
        //        {
        //            byte code = CmuneOperationCodes.MessageToAll;

        //            OperationRequest request = new OperationRequest()
        //            {
        //                Parameters = OperationFactory.Create(code, networkID, address, RealtimeSerialization.ToBytes(args).ToArray()),
        //                OperationCode = code
        //            };

        //            _peerListener.SendOperationToServer(request, isReliable);

        //            return true;
        //        }
        //        else
        //        {
        //            CmuneDebug.LogWarning("({0}) - SendMessage '{1}:{2}' to All failed because connection not ready yet!", PeerListener.SessionID, networkID, address);

        //            return false;
        //        }
        //    }
        //    else return false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public bool SendMessageToAll(short networkID, bool isReliable, byte address, params object[] args)
        {
            if (networkID > 0)
            {
                if (IsConnectionReady)
                {
                    byte code = CmuneOperationCodes.MessageToOthers;

                    OperationRequest request = new OperationRequest()
                    {
                        Parameters = OperationFactory.Create(code, networkID, address, RealtimeSerialization.ToBytes(args).ToArray()),
                        OperationCode = code
                    };

                    _peerListener.SendOperationToServer(request, isReliable);

                    return true;
                }
                else
                {
                    CmuneDebug.LogWarning("({0}) - SendMessage '{1}:{2}' to Others failed because connection not ready yet!", PeerListener.SessionID, networkID, address);

                    return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="args"></param>
        public bool SendMessageToPlayer(int playerID, short networkID, byte address, params object[] args)
        {
            if (networkID > 0)
            {
                if (IsConnectionReady)
                {
                    byte code = CmuneOperationCodes.MessageToPlayer;

                    OperationRequest request = new OperationRequest()
                    {
                        Parameters = OperationFactory.Create(code, playerID, networkID, address, RealtimeSerialization.ToBytes(args).ToArray()),
                        OperationCode = code
                    };

                    _peerListener.SendOperationToServer(request, true);

                    return true;
                }
                else
                {
                    CmuneDebug.LogWarning("({0}) - SendMessage '{1}:{2}' to Player {3} failed because connection not ready yet!", PeerListener.SessionID, networkID, address);

                    return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="args"></param>
        public bool SendMessageToServer(short networkID, byte address, params object[] args)
        {
            return SendMessageToServer(networkID, true, address, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="args"></param>
        public bool SendMessageToServer(short networkID, bool isReliable, byte address, params object[] args)
        {
            if (networkID > 0)
            {
                if (IsConnectionReady)
                {
                    byte code = CmuneOperationCodes.MessageToServer;

                    OperationRequest request = new OperationRequest()
                    {
                        Parameters = OperationFactory.Create(code, networkID, address, RealtimeSerialization.ToBytes(args).ToArray()),
                        OperationCode = code
                    };

                    _peerListener.SendOperationToServer(request, isReliable);

                    return true;
                }
                else
                {
                    CmuneDebug.LogWarning("({0}) - SendMessage '{1}:{2}' to Server failed because connection not ready yet!", PeerListener.SessionID, networkID, address);

                    return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="operationCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendOperationToServerApplication(Action<int, object[]> action, byte address, params object[] args)
        {
            if (_peerListener.IsConnectedToServer)
            {
                //Hashtable data = OperationFactory.Create(CmuneOperationCodes.MessageToApplication, address, (object)RealtimeSerialization.ToBytes(args));

                _peerListener.SendOperationToServerApplication(action, address, args);

                return true;
            }
            else
            {
                CmuneDebug.LogWarning("({0}) - MessageToApplication to Server failed because connection not ready yet!", PeerListener.SessionID);

                return false;
            }
        }

        #region DEBUG

        public Dictionary<short, NetworkClassInfo> CallStatistics = new Dictionary<short, NetworkClassInfo>();

        public class NetworkClassInfo
        {
            public Dictionary<byte, int> _functionCalls = new Dictionary<byte, int>();
            public Dictionary<byte, double> _functionTime = new Dictionary<byte, double>();

            public void AddFunctionCall(byte id, double time)
            {
                if (_functionCalls.ContainsKey(id))
                {
                    _functionCalls[id]++;
                    _functionTime[id] += time;
                }
                else
                {
                    _functionCalls[id] = 1;
                    _functionTime[id] = time;
                }
            }

            public string GetAvarageExecutionTime(byte address)
            {
                if (_functionTime.ContainsKey(address))
                {
                    return (_functionTime[address] / _functionCalls[address]).ToString("f1");
                }
                else return string.Empty;
            }

            public string GetTotalExecutionTime(byte address)
            {
                if (_functionTime.ContainsKey(address))
                {
                    return (_functionTime[address]).ToString("f1");
                }
                else return string.Empty;
            }
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// check if network is ready
        /// </summary>
        public bool IsConnectionReady
        {
            get { return _peerListener.IsConnectedToServer; }
        }

        public PhotonPeerListener PeerListener
        {
            get { return _peerListener; }
        }

        #endregion

        #region FIELDS

        private PhotonPeerListener _peerListener;

        #endregion
    }

}