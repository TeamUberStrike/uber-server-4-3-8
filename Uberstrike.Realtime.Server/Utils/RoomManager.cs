using System;
using System.Collections.Concurrent;

namespace UberStrike.Realtime.Server
{
    public static class RoomManager
    {
        private static object syncroot = new object();
        private static int RoomId = 99; // Room IDs 0 to 99 are reserved for static rooms
        private static ConcurrentDictionary<int, BaseRoom> rooms = new ConcurrentDictionary<int, BaseRoom>();

        public static BaseRoom CreateRoom<T>() where T : BaseRoom
        {
            lock (syncroot)
            {
                return CreateRoom<T>(++RoomId);
            }
        }

        private static BaseRoom CreateRoom<T>(int roomId) where T : BaseRoom
        {
            BaseRoom room = Activator.CreateInstance(typeof(T), roomId) as BaseRoom;
            rooms.TryAdd(roomId, room);
            return room;
        }

        public static void RemoveRoom(int roomId)
        {
            BaseRoom room;
            rooms.TryRemove(roomId, out room);
        }

        public static bool TryGetRoom(int roomId, out BaseRoom room)
        {
            return rooms.TryGetValue(roomId, out room) && room != null;
        }

        public static void RemovePeerFromAllRooms(int peerId)
        {
            foreach (BaseRoom room in rooms.Values)
            {
                room.LeaveRoom(peerId);
            }
        }
    }
}