using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Rooms;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class GameServerNetworkClassFactory : DefaultServerNetworkClassFactory
    {
        public override bool CreateInstance(short id, RemoteMethodInterface rmi, CmuneRoom room, out ServerNetworkClass instance)
        {
            //CmuneDebug.LogError("Create GameMode: " + room.RoomData.RoomName + " id = " + id);

            if (room.RoomData is GameMetaData)
            {
                GameDataManager.Instance.CheckGameData(room.RoomData as GameMetaData);
            }

            switch (id)
            {
                case GameModeID.DeathMatch:
                    instance = new DeathMatchGameMode(rmi, room);
                    return true;

                case GameModeID.TeamDeathMatch:
                    instance = new TeamDeathMatchGameMode(rmi, room);
                    return true;

                case GameModeID.EliminationMode:
                    instance = new TeamEliminationGameMode(rmi, room);
                    return true;

                //BASE CLASSES
                default:
                    return base.CreateInstance(id, rmi, room, out instance);
            }
        }
    }
}
