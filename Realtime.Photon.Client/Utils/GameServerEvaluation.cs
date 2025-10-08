
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Cmune.Realtime.Common;
//using Cmune.Realtime.Common.Utils;
//using ExitGames.Client.Photon;
//using UnityEngine;

//namespace Cmune.Realtime.Photon.Client
//{
//    public class GameServerEvaluation : IPhotonPeerListener
//    {
//        private bool _isWaitingForResponse = false;

//        private GameServerEvaluation()
//        {
//            //CmuneDebug.Log("ctor GameServerEvaluation");

//            _internalState = EVALUATION_STATE.DONE;
//            _lastCheck = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
//            _peer = new PhotonPeer(this, false);
//        }

//        public IEnumerator StartServerEvaluation(List<GameServerInfo> photons, Action<List<GameServerInfo>> action)
//        {
//            if (photons.Count > 0)
//            {
//                if (_internalState != EVALUATION_STATE.CHECKING)
//                {
//                    _internalState = EVALUATION_STATE.CHECKING;

//                    IEnumerator<GameServerInfo> iter = photons.GetEnumerator();

//                    try
//                    {
//                        int serverCount = 0;
//                        while (iter.MoveNext())
//                        {
//                            serverCount++;
//                            _latestLoadData = iter.Current.Data = ServerLoadData.Empty;
//                            _isWaitingForResponse = false;

//                            //CONNECT TO THE EACH PHOTON SERVER ONE BY ONE
//                            if (_peer.Connect(iter.Current.ConnectionString, CmuneNetworkManager.PhotonApplication))
//                            {
//                                _isWaitingForResponse = true;

//                                //CmuneDebug.LogFormat("Load testing {0}", iter.Current.ConnectionString);
//                            }
//                            //else
//                            //{
//                            //    CmuneDebug.LogFormat("Load testing failed for {0}", iter.Current.ConnectionString);
//                            //}

//                            if (_isWaitingForResponse)
//                            {
//                                float time = 0;
//                                while (_isWaitingForResponse)// && _peer.PeerState != (byte)PhotonPeer.PeerStateValue.Connected)
//                                {
//                                    _peer.Service();

//                                    yield return new WaitForSeconds(0.1f);
//                                    time += 0.1f;
//                                    if (time > 5)
//                                        _isWaitingForResponse = false;

//                                    //CmuneDebug.LogFormat("-{0}- processing ... {1} / {2}", serverCount, _peer.PeerState, time);
//                                }

//                                _peer.Disconnect();

//                                if (_latestLoadData.Latency > 0)
//                                {
//                                    iter.Current.Data = _latestLoadData;
//                                    //  CmuneDebug.LogWarning("SUCCESS");
//                                }

//                                time = 0;
//                                while (_peer.PeerState != (byte)PhotonPeer.PeerStateValue.Disconnected)
//                                {
//                                    //CmuneDebug.LogFormat("-{0}- disconnecting ... {1} / {2}", serverCount, _peer.PeerState, time);
//                                    _peer.Service();
//                                    yield return new WaitForSeconds(0.1f);
//                                }
//                            }

//                            // CmuneDebug.Log("########## Process next PhotonData");
//                        }

//                        // CmuneDebug.Log("FINISH");

//                        _lastCheck = DateTime.Now;
//                        _internalState = EVALUATION_STATE.DONE;
//                    }
//                    finally
//                    {
//                        _lastCheck = DateTime.Now;
//                        _internalState = EVALUATION_STATE.DONE;
//                    }
//                }

//                action(photons);
//            }
//        }

//        public void DebugReturn(DebugLevel level, string debug)
//        {
//            Debug.LogWarning("Debug " + debug);
//        }

//        public void EventAction(byte eventCode, Hashtable neutronEvent)
//        {
//            Debug.LogWarning("EventAction " + eventCode);
//        }

//        public void OperationResult(byte opCode, int returnCode, Hashtable returnValues, short invocID)
//        {
//            //CmuneDebug.LogError("OperationResult " + returnCode + "/" + opCode);

//            switch (opCode)
//            {
//                case (byte)CmuneOperationCodes.QueryServerLoad:
//                    {
//                        _isWaitingForResponse = false;

//                        byte[] bytes = OperationUtil.GetArg<byte[]>(returnValues, ParameterKeys.Bytes);

//                        _latestLoadData = new ServerLoadData(bytes);

//                        _latestLoadData.Latency = _peer.RoundTripTime;

//                        break;
//                    }
//                default:
//                    {
//                        _isWaitingForResponse = false;
//                        break;
//                    }
//            }
//        }

//        public void PeerStatusCallback(StatusCode returnCode)
//        {
//            //CmuneDebug.LogError("PeerStatusCallback " + returnCode);

//            switch (returnCode)
//            {
//                case StatusCode.Connect:                          // = 1024
//                    {
//                        _peer.OpCustom((byte)CmuneOperationCodes.QueryServerLoad, new Hashtable(0), true);

//                        break;
//                    }

//                case StatusCode.DisconnectByServerLogic:                  // = 1041
//                case StatusCode.DisconnectByServerUserLimit:                  // = 1041
//                case StatusCode.DisconnectByServer:                  // = 1041
//                    {
//                        if (CmuneNetworkState.DebugMessaging)
//                            CmuneDebug.LogError("RETURN DisconnectByServer");

//                        break;
//                    }
//                case StatusCode.TimeoutDisconnect:                   // = 1040
//                    {
//                        if (CmuneNetworkState.DebugMessaging)
//                            CmuneDebug.LogError("RETURN TimeoutDisconnect");

//                        break;
//                    }
//                case StatusCode.Disconnect:                           // = 1025
//                    {

//                        break;
//                    }
//                case StatusCode.QueueOutgoingReliableWarning:               // = 1032;
//                case StatusCode.QueueOutgoingReliableError:             // = 1031;
//                case StatusCode.QueueOutgoingUnreliableWarning:           // = 1028;
//                case StatusCode.QueueOutgoingAcksWarning:         // = 1027;
//                case StatusCode.QueueSentWarning:                     // = 1037;
//                    {
//                        CmuneDebug.LogWarning("RETURN <OUT-QUEUE> FILLING UP: " + returnCode);
//                        break;
//                    }
//                case StatusCode.QueueIncomingReliableWarning:           // = 1034;
//                case StatusCode.QueueIncomingUnreliableWarning:         // = 1033;
//                    {
//                        CmuneDebug.LogWarning("RETURN <IN-QUEUE> FILLING UP: " + returnCode);
//                        break;
//                    }

//                case StatusCode.Exception:                            // = 1026;
//                case StatusCode.Exception_Connect:                    // = 1023;
//                case StatusCode.InternalReceiveException:             // = 1039;
//                    {
//                        if (CmuneNetworkState.DebugMessaging)
//                            CmuneDebug.LogError("RETURN Exception: " + returnCode);
//                        break;
//                    }

//                default:
//                    {
//                        CmuneDebug.LogError("UNHANDLED RETURN with returnCode:" + returnCode);
//                        break;
//                    }
//            }
//        }

//        #region PROPERTIES
//        public EVALUATION_STATE CurrentState
//        {
//            get
//            {
//                if (_internalState == EVALUATION_STATE.CHECKING)
//                {
//                    return EVALUATION_STATE.CHECKING;
//                }
//                else if (LastCheckedBeforeMinutes > 60)
//                {
//                    return EVALUATION_STATE.DEPRECATED;
//                }
//                else
//                {
//                    return EVALUATION_STATE.DONE;
//                }
//            }
//        }

//        public static GameServerEvaluation Instance
//        {
//            get
//            {
//                if (_instance == null) _instance = new GameServerEvaluation();
//                return _instance;
//            }
//        }

//        public double LastCheckedBeforeMinutes
//        {
//            get { return DateTime.Now.Subtract(_lastCheck).TotalMinutes; }
//        }
//        #endregion

//        #region FIELDS
//        private static GameServerEvaluation _instance;
//        private PhotonPeer _peer;
//        private ServerLoadData _latestLoadData;
//        private DateTime _lastCheck;
//        private EVALUATION_STATE _internalState;
//        #endregion

//        public enum EVALUATION_STATE { DONE, CHECKING, DEPRECATED }
//    }
//}