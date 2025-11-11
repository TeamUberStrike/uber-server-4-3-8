using System;
using System.Collections;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using System.Collections.Generic;

namespace Cmune.Realtime.Common
{
    public static class OperationFactory
    {
        public static Dictionary<byte, object> Create(byte code, params object[] args)
        {
            Dictionary<byte, object> table = new Dictionary<byte, object>();

            try
            {
                switch (code)
                {
                    case CmuneOperationCodes.MessageToServer:
                        {
                            OperationUtil.SetInstanceID(table, args[0]);
                            OperationUtil.SetMethodId(table, args[1]);
                            OperationUtil.SetBytes(table, args[2]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.LogFormat("--> MessageToServer:{0}|{1} Length: {2}", (short)args[0], (byte)args[1], ((byte[])args[2]).Length);
                            break;
                        }
                  
                    case CmuneOperationCodes.MessageToAll:
                    case CmuneOperationCodes.MessageToOthers:
                        {
                            OperationUtil.SetInstanceID(table, args[0]);
                            OperationUtil.SetMethodId(table, args[1]);
                            OperationUtil.SetBytes(table, args[2]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.LogFormat("--> MessageToOthers:{0}|{1} Length: {2}", (short)args[0], (byte)args[1], ((byte[])args[2]).Length);
                            break;
                        }
                    case CmuneOperationCodes.MessageToPlayer:
                        {
                            OperationUtil.SetArg<int>(table, ParameterKeys.ActorId, args[0]);
                            OperationUtil.SetInstanceID(table, args[1]);
                            OperationUtil.SetMethodId(table, args[2]);
                            OperationUtil.SetBytes(table, args[3]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.LogFormat("--> MessageToPlayer:{0}|{1}|{2} Length: {3}", (int)args[0], (short)args[1], (byte)args[2], ((byte[])args[3]).Length);
                            break;
                        }
                    case CmuneOperationCodes.MessageToApplication:
                        {
                            OperationUtil.SetMethodId(table, args[0]);
                            OperationUtil.SetArg<short>(table, ParameterKeys.InvocationId, args[1]);
                            OperationUtil.SetBytes(table, args[2]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.Log("--> PeerSpecification");
                            break;
                        }
                    case CmuneOperationCodes.PhotonGameJoin:
                        {
                            OperationUtil.SetBytes(table, args[0]);
                            OperationUtil.SetArg<int>(table, ParameterKeys.Cmid, args[1]);
                            OperationUtil.SetArg<int>(table, ParameterKeys.AccessLevel, args[2]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.Log("--> PhotonGameJoin");
                            break;
                        }
                    case CmuneOperationCodes.PhotonGameLeave:
                        {
                            OperationUtil.SetArg<byte[]>(table, ParameterKeys.RoomId, args[0]);

                            //if (CmuneNetworkState.DebugMessageSending)
                            //    CmuneDebug.Log("--> PhotonGameLeave");
                            break;
                        }
                }
            }
            catch (Exception)
            {
                table.Clear();
                CmuneDebug.Exception("Wrong number of Parameters while creating Operation of type {0}", code);
            }

            return table;
        }
    }
}
