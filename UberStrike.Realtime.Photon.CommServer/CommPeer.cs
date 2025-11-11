
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace UberStrike.Realtime.Photon.CommServer
{
    /// <summary>
    /// The CommPeer
    /// </summary>
    public class CommPeer : CmunePeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommPeer"/> class.
        /// </summary>
        /// <param name="photonPeer">
        /// The photon peer.
        /// </param>
        public CommPeer(IRpcProtocol protocol, IPhotonPeer photonPeer, int cmid)
            : base(protocol, photonPeer, cmid)
        { }

        /// <summary>
        /// The on disconnect.
        /// </summary>
        protected override void OnDisconnect()
        {
            CommServerRoom.Instance.LeaveGame(this);

            base.OnDisconnect();
        }

        /// <summary>
        /// The on operation request.
        /// </summary>
        /// <param name="operationRequest">
        /// The operation Request.
        /// </param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (PeerType != Cmune.Realtime.Common.PeerType.None)
            {
                switch (operationRequest.OperationCode)
                {
                    case CmuneOperationCodes.MessageToPlayer:
                    case CmuneOperationCodes.MessageToServer:
                    case CmuneOperationCodes.PhotonGameJoin:
                    case CmuneOperationCodes.PhotonGameLeave:
                        {
                            CommServerRoom.Instance.EnqueueOperation(this, operationRequest, sendParameters);
                            break;
                        }
                    default:
                        {
                            base.OnOperationRequest(operationRequest, sendParameters);
                            break;
                        }
                }
            }
            else
            {
                base.OnOperationRequest(operationRequest, sendParameters);
            }
        }
    }
}