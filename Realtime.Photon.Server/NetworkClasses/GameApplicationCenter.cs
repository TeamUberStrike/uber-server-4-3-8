using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Photon.Server;
using Photon.SocketServer;

public class GameApplicationCenter : ApplicationCenter
{
    //[NetworkMethod(GameApplicationRPC.RoomRequest)]
    //private void OnRoomRequest(CmunePeer peer, MessageToApplication op, int roomID)
    //{
    //    if (peer != null)
    //    {
    //        CmuneRoom room;
    //        if (CmuneRoomFactory.Instance.TryGetGame(roomID, out room))
    //        {
    //            op.ReturnValue = RealtimeSerialization.ToBytes(room.RoomData).ToArray();
    //            peer.PublishOperationResponse(op.GetOperationResponse(0, string.Empty));
    //        }
    //        else
    //        {
    //            peer.PublishOperationResponse(new OperationResponse(op.OperationRequest, 1, "Room not found"));
    //        }
    //    }
    //}

    [NetworkMethod(GameApplicationRPC.BanPlayer)]
    private void OnKickPlayerFromGame(CmunePeer peer, MessageToApplication op, int cmid, int roomNumber, int durationInMinutes)
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
}
