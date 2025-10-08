using System;
using Cmune.Realtime.Common;
using Photon.SocketServer;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    public sealed class PhotonConnectionTest : IDisposable
    {
        private PhotonConnectionTest()
        {
            _photonListener = new ServerPhotonListener();

            _photonListener.Connected += OnConnected;
            _photonListener.Disconnected += OnDisconnected;
            _photonListener.ResponseReceived += OnOperationResponse;
        }

        public static PhotonConnectionTest Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PhotonConnectionTest();
                return _instance;
            }
        }

        public bool IsDone
        {
            get { return _isConnectionDone; }
        }

        public bool IsSuccsessfull
        {
            get { return _isConnectionSuccessfull; }
        }

        public int ConnectionCount
        {
            get { return _connectionAttemptCount; }
        }

        public bool DoConnectionTest(string connection, int connectionAttempts)
        {
            _connection = connection;
            _connectionAttemptMax = connectionAttempts;
            _connectionAttemptCount = 0;

            return Connect();
        }

        private bool Connect()
        {
            try
            {
                if (_photonListener.Peer.PeerState == 0)
                {
                    _isConnectionDone = false;
                    _isConnectionSuccessfull = false;

                    _connectionAttemptCount++;
                    _photonListener.Connect(new ConnectionAddress(_connection));
                }
                else
                {
                    CmuneDebug.LogError("ConnectToServer failed because another connection is still open " + _photonListener.Peer.PeerState);
                    return false;
                }
            }
            catch (NullReferenceException ex)
            {
                CmuneDebug.LogError("NullReferenceException in OnServerListUpdated: " + ex.Message);
            }

            return true;
        }

        private void OnConnected()
        {
            _isConnectionSuccessfull = true;

            _photonListener.Close();
        }

        private void OnDisconnected()
        {
            if (!_isConnectionSuccessfull && _connectionAttemptCount < _connectionAttemptMax)
            {
                Connect();
            }
            else
            {
                _isConnectionDone = true;
            }
        }

        private void OnOperationResponse(ExitGames.Client.Photon.OperationResponse r) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed = false;
        private void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                if (_instance != null)
                {
                    _photonListener.Connected -= OnConnected;
                    _photonListener.Disconnected -= OnDisconnected;
                    _photonListener.ResponseReceived -= OnOperationResponse;
                }

                _instance = null;
            }

            _isDisposed = true;
        }

        private ServerPhotonListener _photonListener;
        private static PhotonConnectionTest _instance;

        private string _connection = string.Empty;
        private bool _isConnectionDone = false;
        private bool _isConnectionSuccessfull = false;
        private int _connectionAttemptCount = 0;
        private int _connectionAttemptMax = 1;
    }
}