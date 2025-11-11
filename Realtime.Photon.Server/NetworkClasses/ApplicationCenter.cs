using System;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using Photon.SocketServer;

/// <summary>
/// Describe what this center is doing
/// </summary>
public class ApplicationCenter : CmuneNetworkClass
{
    public ApplicationCenter() { }

    public static ApplicationCenter Instance { get; set; }

    public static void RecieveMessage(CmunePeer peer, MessageToApplication op)
    {
        //if (CmuneDebug.IsDebugEnabled)
        //    CmuneDebug.LogInfoFormat("ApplicationCenter.RecieveMessage {0}", op.LocalAddress);

        object[] args = op.GetParameters(peer);
        object[] param = new object[args.Length + 3];
        param[0] = peer;
        param[1] = op.InvocationId;
        param[2] = op.OperationRequest.OperationCode;
        if (args.Length > 0)
            Array.Copy(args, 0, param, 3, args.Length);

        Instance.CallMethod(op.LocalAddress, param);
    }

    [NetworkMethod(ApplicationRPC.PeerSpecification)]
    protected void OnPeerSpecification(CmunePeer peer, short invocId, byte opCode, byte peerType, string address)
    {
        if (peer != null)
        {
            peer.PeerType = (PeerType)peerType;

            if (peer.PeerType == PeerType.GamerServer)
            {
                peer.Address = new ConnectionAddress(address);
            }
            else
            {
                //check if client is connecting with the correct realtime protocol version
                if (!address.StartsWith(Cmune.Realtime.Common.Protocol.Version))
                {
                    CmuneDebug.LogError("PeerType Specification of peer {0} failed because protocol version outdated: {1}", peer.PeerType, address);
                    //peer.PublishOperationResponse(new OperationResponse( op.GetOperationResponse(1, string.Format("The current version '{0}' of your client is outdated", address)));
                    peer.Disconnect();
                }
            }
        }
    }

    [NetworkMethod(ApplicationRPC.QueryServerLoad)]
    protected void OnServerLoadQuery(CmunePeer peer, short invocId, byte opCode)
    {
        if (peer != null)
        {
            var response = new QueryResponse(invocId) { Bytes = RealtimeSerialization.ToBytes(CmunePerformanceCounter.Instance.CurrentLoad).ToArray() };
            peer.PublishOperationResponse(new OperationResponse(opCode, response), new SendParameters());
        }
    }

    [NetworkMethod(ApplicationRPC.QueryPerfCounters)]
    protected void OnQueryPerfCounters(CmunePeer peer, short invocId, byte opCode)
    {
        if (peer != null)
        {
            Dictionary<int, float> dict = CmunePerformanceCounter.Instance.GetNewCounterSnapshot();
            var response = new QueryResponse(invocId) { Bytes = RealtimeSerialization.ToBytes((object)dict.Keys, (object)dict.Values).ToArray() };
            peer.PublishOperationResponse(new OperationResponse(opCode, response), new SendParameters());
        }
    }
}