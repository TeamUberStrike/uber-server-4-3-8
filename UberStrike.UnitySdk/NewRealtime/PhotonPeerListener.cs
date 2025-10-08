using System;
using System.Runtime.Serialization;
using ExitGames.Client.Photon;
using UnityEngine;

namespace UberStrike.Realtime.Client
{
    public sealed class PhotonPeerListener : IPhotonPeerListener
    {
        public event Action<byte, byte[]> EventDispatcher;

        private bool isConnected = false;


        public void OnEvent(EventData eventData)
        {
            try
            {
                if (EventDispatcher != null)
                {
                    EventDispatcher(eventData.Code, (byte[])eventData.Parameters[0]);
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.Log("PhotonEvent: " + eventData.ToStringFull());
                Debug.LogWarning("Source: " + e.Source);
                Debug.LogWarning("Stack: " + e.StackTrace);
                throw new Exception(e.GetType() + " thrown when executing EventAction with Id " + eventData.Code);
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            Debug.Log("OperationResult " + operationResponse.OperationCode);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            Debug.Log("PeerStatusCallback " + statusCode);

            switch (statusCode)
            {
                case StatusCode.Connect:
                    {
                        if (!isConnected && OnConnect != null)
                            OnConnect();
                        isConnected = true;
                    } break;
                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:
                    {
                        if (isConnected && OnDisconnect != null)
                            OnDisconnect();
                        isConnected = false;
                    } break;
                default:
                    Debug.LogWarning("Unhandled OnStatusChanged " + statusCode);
                    break;
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log("DebugReturn " + message);
        }

        public event Action OnConnect;
        public event Action OnDisconnect;
    }
}
