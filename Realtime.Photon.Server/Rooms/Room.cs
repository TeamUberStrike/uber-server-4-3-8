using System;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using log4net;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// Class implementing a Room.
    /// </summary>
    public class Room : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class without a room name.
        /// </summary>
        public Room()
        {
            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();
        }

        /// <summary>
        /// Enqueues an <see cref="OperationRequest"/> to the end of the execution queue.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <param name="operationRequest">
        /// The operation request to enqueue.
        /// </param>
        /// <remarks>
        /// <see cref="ExecuteOperation"/> is called sequentially for each operation request 
        /// stored in the execution queue.
        /// Using an execution queue ensures that operation request are processed in order
        /// and sequentially to prevent object synchronization (multi threading).
        /// </remarks>
        public void EnqueueOperation(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (_isDisposed) return;

            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }

        /// <summary>
        /// This method is invoked sequentially for each operation request 
        /// enqueued in the <see cref="ExecutionFiber"/> using the 
        /// <see cref="EnqueueOperation"/> method.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <param name="operation">
        /// The operation request.
        /// </param>
        protected virtual void ExecuteOperation(CmunePeer peer, OperationRequest operation, SendParameters sendParameters)
        {
        }

        /// <summary>
        /// Publishes an event to a single actor on a specified channel.
        /// </summary>
        /// <param name="e">
        /// The event to publish.
        /// </param>
        /// <param name="actor">
        /// The <see cref="Actor"/> who should receive the event.
        /// </param>
        /// <param name="reliability">
        /// The <see cref="Reliability"/> used to send the event.
        /// </param>
        /// <param name="channelId">
        /// The channel id.
        /// </param>
        protected void PublishEvent(EventData e, CmunePeer peer, SendParameters sendParameters)
        {
            peer.PublishEvent(e, sendParameters);
        }

        /// <summary>
        /// Publishes an event to a list of actors on a specified channel.
        /// </summary>
        /// <param name="e">
        /// The event to publish.
        /// </param>
        /// <param name="actorList">
        /// A list of <see cref="Actor"/> who should receive the event.
        /// </param>
        /// <param name="reliability">
        /// The <see cref="Reliability"/> used to send the event.
        /// </param>
        /// <param name="channelId">
        /// The channel id.
        /// </param>
        protected void PublishEvent(EventData e, IEnumerable<CmunePeer> peerList, SendParameters sendParameters)
        {
            foreach (CmunePeer peer in peerList)
            {
                peer.PublishEvent(e, sendParameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="peerList"></param>
        protected void PublishEvent(EventData e, IEnumerable<CmunePeer> peerList, CmunePeer exeption, SendParameters sendParameters)
        {
            foreach (CmunePeer peer in peerList)
            {
                if (peer.SessionId != exeption.SessionId)
                    peer.PublishEvent(e, sendParameters);
            }
        }

        #region Properties

        /// <summary>
        /// Gets a <see cref="PoolFiber"/> instance used to synchronize access to this instance.
        /// </summary>
        /// <value>A <see cref="PoolFiber"/> instance.</value>
        public PoolFiber ExecutionFiber { get; private set; }

        /// <summary>
        /// Gets the name (id) of the room.
        /// </summary>
        public string Name { get; protected set; }

        #endregion

        #region Fields

        /// <summary>
        /// An <see cref="ILog"/> instance used to log messages to the log4net framework.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region IDisposable

        /// <summary>
        /// Gets a value indicating whether IsDisposed.
        /// </summary>
        protected bool _isDisposed = false;

        /// <summary>
        /// Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="dispose">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                this.ExecutionFiber.Dispose();
            }

            _isDisposed = true;
        }

        #endregion
    }
}