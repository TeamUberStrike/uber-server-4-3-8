
using System;
using System.Collections;
using Cmune.Util;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Cmune.Realtime.Photon.Client.Network
{
    public class ServerRequest
    {
        protected int _requestTimeout = 5;
        protected PhotonClient _client;

        private MonoBehaviour _mono;
        private event Action<int, object[]> _customCallbackEvent;

        public PhotonClient.ConnectionStatus ConnectionState { get { return _client.ConnectionState; } }
        public PeerStateValue PeerState { get { return _client.PeerListener.PeerState; } }

        protected ServerRequest(MonoBehaviour mono)
        {
            _mono = mono;
            _client = new PhotonClient(mono);
        }

        public static void Run(MonoBehaviour mono, string serverConnection, Action<int, object[]> callback, byte methodID, params object[] args)
        {
            ServerRequest request = new ServerRequest(mono);
            request.Execute(serverConnection, callback, methodID, args);
        }

        public static void Run(MonoBehaviour mono, string serverConnection, byte methodID, params object[] args)
        {
            ServerRequest request = new ServerRequest(mono);

            request.Execute(serverConnection, null, methodID, args);
        }

        protected bool Execute(string serverConnection, Action<int, object[]> callback, byte methodID, params object[] args)
        {
            if (_client.ConnectionState == PhotonClient.ConnectionStatus.STOPPED)
            {
                _customCallbackEvent = callback;
                _mono.StartCoroutine(StartUpdateLoop());
                _mono.StartCoroutine(StartRequest(serverConnection, methodID, args));
                return true;
            }
            else
            {
                CmuneDebug.LogWarning("ServerRequest to " + serverConnection + " ignored because connection " + _client.ConnectionState);
                return false;
            }
        }

        private IEnumerator StartRequest(string serverConnection, byte methodID, params object[] args)
        {
            //connect to server
            yield return _client.ConnectToServer(serverConnection, 0, 0);

            //CmuneDebug.Log(_client.PeerListener.PeerState + " " + _client.ConnectionState);

            if (_client.IsConnected)
            {
                //send a request to the server to to return the gameMetaDatat of a certain room
                _client.Rmi.Messenger.SendOperationToServerApplication(OnRequestCallback, methodID, args);
            }
            else
            {
                //server not reachable
                OnRequestCallback(-1, null);
            }

            //wait for operation response
            yield return new WaitForSeconds(_requestTimeout);

            //close connection
            _client.Disconnect();
        }

        private IEnumerator StartUpdateLoop()
        {
            do
            {
                _client.Update();
                yield return new WaitForSeconds(0.1f);
            }
            while (_client.ConnectionState != PhotonClient.ConnectionStatus.STOPPED);
        }

        protected virtual void OnRequestCallback(int result, object[] table)
        {
            if (_customCallbackEvent != null)
            {
                _customCallbackEvent(result, table);
            }
        }
    }
}