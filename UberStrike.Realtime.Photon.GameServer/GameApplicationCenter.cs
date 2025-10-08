using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Photon.Server;
using Photon.SocketServer;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class GameApplicationCenter : ApplicationCenter
    {
        [NetworkMethod(GameApplicationRPC.RoomRequest)]
        private void OnRoomRequest(CmunePeer peer, short invocId, byte opCode, int roomID)
        {
            if (peer != null)
            {
                CmuneRoom room;
                if (RoomCache.Instance.TryGetGame(roomID, out room))
                {
                    var response = new QueryResponse(invocId) { Bytes = RealtimeSerialization.ToBytes(room.RoomData).ToArray() };
                    peer.PublishOperationResponse(new OperationResponse(CmuneOperationCodes.MessageToApplication, response), new SendParameters());
                }
                else
                {
                    peer.PublishOperationResponse(new OperationResponse(opCode) { ReturnCode = 1, DebugMessage = "Room doesn't exist!" }, new SendParameters());
                }
            }
        }

        [NetworkMethod(GameApplicationRPC.BanPlayer)]
        private void OnKickPlayerFromGame(CmunePeer peer, short invocId, byte opCode, int cmid, int roomNumber, int durationInMinutes)
        {
            if (peer != null)
            {
                CmuneRoom room;
                if (RoomCache.Instance.TryGetGame(roomNumber, out room))
                {
                    room.BanPlayerFromRoom(cmid, durationInMinutes);
                }
            }
        }

        [NetworkMethod(GameApplicationRPC.CustomMessage)]
        private void OnCustomMessage(CmunePeer peer, short invocId, byte opCode, int roomNumber, int actorId, string message)
        {
            if (peer != null)
            {
                CmuneRoom room;

                if (RoomCache.Instance.TryGetGame(roomNumber, out room))
                {
                    room.PublishRoomMessage(new RoomMessage(RoomMessageType.CustomMessage, actorId, message));
                }
            }
        }

        [NetworkMethod(GameApplicationRPC.MutePlayer)]
        private void OnMutePlayer(CmunePeer peer, short invocId, byte opCode, int roomNumber, int actorId, bool mute)
        {
            if (peer != null)
            {
                CmuneRoom room;
                if (RoomCache.Instance.TryGetGame(roomNumber, out room))
                    room.PublishRoomMessage(new RoomMessage(RoomMessageType.MutePlayer, actorId, mute));
            }
        }
    }
}