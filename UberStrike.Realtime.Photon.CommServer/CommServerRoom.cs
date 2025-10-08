
using System.Threading;
using Cmune.Realtime.Common;
using log4net;
using Photon.SocketServer;
using UberStrike.Realtime.Photon.CommServer;

namespace Cmune.Realtime.Photon.Server
{
    public class CommServerRoom : CmuneRoom
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameName">game name.</param>
        private CommServerRoom()
            : base(new RoomMetaData(StaticRoomID.CommCenter, "The Comm Server", ServerSettings.ConnectionString), false, false)
        {
            _cacheLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Every game instance (or process) has a queue of incoming operations to execute. Per game
        /// ExecuteOperation() is calles in order, serial and in one thread.
        /// </summary>
        /// <param name="operation">operation to execute.</param>
        protected override void ExecuteOperation(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case CmuneOperationCodes.MessageToPlayer:
                    {
                        HandleMessageToPlayer(peer, operationRequest, sendParameters);
                        break;
                    }

                case CmuneOperationCodes.MessageToServer:
                    {
                        HandleMessageToServer(peer, operationRequest);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameJoin:
                    {
                        HandleJoin(peer, operationRequest, sendParameters);

                        if (SecurityManager.IsPeerBanned(peer))
                            SecurityManager.DisconnectPeer(peer);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameLeave:
                    {
                        HandleLeave(peer, operationRequest, sendParameters);
                        break;
                    }
            }
        }

        #region PROPERTIES

        public static CommServerRoom Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommServerRoom();
                return _instance;
            }
        }

        #endregion

        #region FIELDS

        private ReaderWriterLockSlim _cacheLock;

        /// <summary>
        /// Logger for debug.
        /// </summary>
        private static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static CommServerRoom _instance;

        #endregion
    }
}