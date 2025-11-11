
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    public static class CmuneOperations
    {
        public static void DebugRequest(CmunePeer peer, OperationRequest operationRequest)
        {
            try
            {
                string s = (peer != null) + " && " + (peer.PhotonPeer != null);
                if (peer != null)
                    s = string.Format("Peer with SessionId '{0}' @ '{1}'", peer.SessionId, peer.Address.ToString());

                CmuneDebug.LogError("Operation Request by {0} and Parameters:\n{1}", s, OperationUtil.PrintHashtable(operationRequest.Parameters));
            }
            catch (System.Exception e)
            {
                CmuneDebug.Exception("Exception in DebugRequest with opCode '{0}' because: {1}\n{2}", operationRequest.OperationCode, e.Message, e.StackTrace);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageToServer : Operation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationRequest"></param>
        public MessageToServer(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        { }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.InstanceId)]
        public short NetworkID { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.MethodId)]
        public byte LocalAddress { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.Bytes)]
        public byte[] Bytes { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGetParameters(CmunePeer peer, out object[] param)
        {
            param = new object[0];
            bool success = false;

            try
            {
                if (Bytes == null)
                {
                    CmuneDebug.LogError("Byte Array is NULL of Operation '{0}'", OperationRequest.OperationCode);
                    CmuneOperations.DebugRequest(peer, OperationRequest);
                }
                else if (!RealtimeSerialization.TryToObjects(Bytes, out param))
                {
                    CmuneOperations.DebugRequest(peer, OperationRequest);
                }
                else
                {
                    success = true;
                }
            }
            catch (System.NullReferenceException e)
            {
                CmuneDebug.Exception("NullReferenceException in MessageToServer with opCode '{0}' because: {1}\n{2}", OperationRequest.OperationCode, e.Message, e.StackTrace);
            }
            catch (System.Exception e)
            {
                CmuneDebug.Exception("Exception in MessageToServer with opCode '{0}' because: {1}\n{2}", OperationRequest.OperationCode, e.Message, e.StackTrace);
            }

            return success;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageToOthers : Operation
    {
        public MessageToOthers(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageToAll : Operation
    {
        public MessageToAll(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageToPlayer : Operation
    {
        public MessageToPlayer(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest) { }

        [DataMember(Code = ParameterKeys.ActorId)]
        public int ActorId { get; protected set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageToApplication : Operation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationRequest"></param>
        public MessageToApplication(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        { }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.InvocationId)]
        public short InvocationId { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.MethodId)]
        public byte LocalAddress { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Code = ParameterKeys.Bytes)]
        public byte[] Bytes { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public object[] GetParameters(CmunePeer peer)
        {
            object[] param = new object[0];

            try
            {
                if (Bytes == null)
                {
                    CmuneDebug.LogError("Byte Array is NULL of Operation '{0}'", OperationRequest.OperationCode);
                    CmuneOperations.DebugRequest(peer, OperationRequest);
                }
                else if (!RealtimeSerialization.TryToObjects(Bytes, out param))
                {
                    //CmuneDebug.LogErrorFormat("Deserialization failed with Byte[] of Length {0}", Bytes.Length);
                    CmuneOperations.DebugRequest(peer, OperationRequest);
                }
            }
            catch (System.NullReferenceException e)
            {
                CmuneDebug.Exception("NullReferenceException in MessageToApplication with opCode '{0}' because: {1}\n{2}", OperationRequest.OperationCode, e.Message, e.StackTrace);
            }
            catch (System.Exception e)
            {
                CmuneDebug.Exception("Exception in MessageToApplication with opCode '{0}' because: {1}\n{2}", OperationRequest.OperationCode, e.Message, e.StackTrace);
            }

            return param;
        }
    }
}
