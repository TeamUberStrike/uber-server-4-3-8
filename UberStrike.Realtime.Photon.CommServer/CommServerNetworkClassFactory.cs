using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Rooms;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.CommServer
{
    public class CommServerNetworkClassFactory : DefaultServerNetworkClassFactory
    {
        public override bool CreateInstance(short id, RemoteMethodInterface rmi, CmuneRoom room, out ServerNetworkClass instance)
        {
            //CmuneDebug.LogError("Create GameMode: " + room.RoomData.RoomName + " id = " + id);

            if (room.RoomData is GameMetaData)
            {
                GameConstants.CheckGameData(id, room.RoomData as GameMetaData);
            }

            switch (id)
            {
                case NetworkClassID.CommCenter:
                    instance = new ServerCommCenter(rmi);
                    return true;

                //BASE CLASSES
                default:
                    return base.CreateInstance(id, rmi, room, out instance);
            }
        }
    }
}
