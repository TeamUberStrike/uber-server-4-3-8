using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Util;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using ExitGames.Client.Photon;

namespace Cmune.Realtime.Photon.Server
{
    public sealed class ServerPhotonListener : IDisposable, ExitGames.Client.Photon.IPhotonPeerListener
    {
        public ServerPhotonListener()
        {
            this._executionFiber = new PoolFiber();
            this._executionFiber.Start();

            Peer = new ExitGames.Client.Photon.PhotonPeer(this, ConnectionProtocol.Udp);
        }

        public void Connect(ConnectionAddress connection)
        {
            if (connection.IsValid && Peer.Connect(connection.ConnectionString, "Uberstrike"))
            {
                if (_timer != null) _timer.Dispose();

                _timer = this._executionFiber.ScheduleOnInterval(() => Peer.Service(), 100, 100);
            }
            else
            {
                CmuneDebug.LogError("ServerPhotonListener:Connect to {0} failed", connection);
            }
        }

        public void Close()
        {
            CmuneDebug.LogError("ServerPhotonListener:Close");

            try
            {
                Peer.Disconnect();
            }
            catch (Exception e)
            {
                CmuneDebug.LogError("ServerPhotonListener:Close Failed with Exception: {0}", e.Message);
            }
        }

        public void OnEvent(ExitGames.Client.Photon.EventData eventData)
        {   }

        public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) { }

        public void OnOperationResponse(ExitGames.Client.Photon.OperationResponse operationResponse)
        {
            if (ResponseReceived != null) ResponseReceived(operationResponse);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            CmuneDebug.Log("PeerStatusCallback: {0}", statusCode);

            switch (statusCode)
            {
                case StatusCode.Connect:
                    {
                        if (Connected != null)
                            Connected();

                    } break;
                case StatusCode.Disconnect:
                    {
                        if (Disconnected != null)
                            Disconnected();

                        _timer.Dispose();
                    } break;
                default:
                    {
                        break;
                    }
            }
        }

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
        private void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                Close();

                Connected = null;
                Disconnected = null;
                ResponseReceived = null;

                if (_timer != null) _timer.Dispose();

                _executionFiber.Dispose();
            }

            _isDisposed = true;
        }

        #region Fields

        private bool _isDisposed = false;

        /// <summary>
        /// The connected events
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// The connected events
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// The response received.
        /// </summary>
        public event Action<ExitGames.Client.Photon.OperationResponse> ResponseReceived;

        /// <summary>
        /// Gets the underling <see cref="Peer"/>.
        /// </summary>
        public ExitGames.Client.Photon.PhotonPeer Peer { get; private set; }

        private PoolFiber _executionFiber;
        private IDisposable _timer;

        #endregion

    }
}