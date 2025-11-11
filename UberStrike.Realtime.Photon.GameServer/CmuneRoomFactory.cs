using Cmune.Realtime.Common;
using Cmune.Util;
using Cmune.Realtime.Photon.Server;

namespace UberStrike.Realtime.Photon.GameServer
{
    /// <summary>
    /// 
    /// </summary>
    public static class CmuneRoomFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static CmuneRoom GetOrCreateGame(RoomMetaData data)
        {
            //CmuneDebug.LogError("GetOrCreateGame: " + data.RoomName + "  " + data.RoomNumber);

            switch (data.RoomNumber)
            {
                case StaticRoomID.Auto:
                    {
                        return CreateCmuneGame(data, true, true);
                    }
                case StaticRoomID.CommCenter:
                    {
                        CmuneDebug.LogError("Wrong ID requested");
                        return null;
                    }
                case StaticRoomID.LobbyCenter:
                    {
                        return LobbyServerRoom.Instance;
                    }
                default:
                    {
                        bool hasRoom;
                        CmuneRoom room;

                        hasRoom = RoomCache.Instance.TryGetGame(data.RoomNumber, out room);

                        if (!hasRoom)
                        {
                            //the room we are looking for was created on this server, so it's safe to use the same room number
                            //additionally we check if the roomnumber is an old one to prevent that the request is a carry over from a server restart

                            room = CreateCmuneGame(data, data.RoomNumber, true, true);
                        }

                        return room;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CmuneRoom CreateCmuneGame(RoomMetaData data, bool autodestroy, bool sendLobbyEvents)
        {
            return CreateCmuneGame(data, RoomCache.NextRoomID(), autodestroy, sendLobbyEvents);
        }

        private static CmuneRoom CreateCmuneGame(RoomMetaData data, int roomID, bool autodestroy, bool sendLobbyEvents)
        {
            data.RoomID = new CmuneRoomID(roomID, ServerSettings.ConnectionString);

            CmuneRoom room = new CmuneRoom(data, autodestroy, sendLobbyEvents);

            RoomCache.Instance.AddRoom(room);

            return room;
        }
    }
}