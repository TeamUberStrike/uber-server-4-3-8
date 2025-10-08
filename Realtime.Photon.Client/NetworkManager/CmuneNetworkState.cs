
using System.Collections.Generic;
using Cmune.Realtime.Common;

namespace Cmune.Realtime.Photon.Client
{
    public static class CmuneNetworkState
    {
        private readonly static Dictionary<CmuneRoomID, RoomMetaData> _currentGames = new Dictionary<CmuneRoomID, RoomMetaData>();
        private static int _currentSessionID = 0;

        public static bool DebugMessaging = false;

        public static int TotalRecievedBytes = 0;
        public static int TotalSentBytes = 0;

        public static int IncomingMessagesCount = 0;

        public static int UnreliableOutgoingMessagesCount = 0;
        public static int ReliableOutgoingMessagesCount = 0;

        public static int OutgoingMessagesCount
        {
            get { return ReliableOutgoingMessagesCount + UnreliableOutgoingMessagesCount; }
        }

        public static int GetNextSessionID()
        {
            return _currentSessionID++;
        }

        public static bool TryGetRoom(CmuneRoomID roomID, out RoomMetaData meta)
        {
            return _currentGames.TryGetValue(roomID, out meta);
        }

        internal static bool AddRoom(RoomMetaData data)
        {
            if (data.RoomID.IsVersionCompatible)
            {
                _currentGames[data.RoomID] = data;
                return true;
            }
            else return false;
        }

        internal static bool RemoveRoom(CmuneRoomID id)
        {
            return _currentGames.Remove(id);
        }

        public static IEnumerable<RoomMetaData> AllRooms
        {
            get { return CmuneNetworkState._currentGames.Values; }
        }

        public static int RoomCount
        {
            get { return CmuneNetworkState._currentGames.Count; }
        }

        internal static void ClearRooms()
        {
            _currentGames.Clear();
        }
    }

    public enum NetworkState
    {
        STATE_NPEER_CREATED = 0,
        STATE_CONNECT = 1,
        STATE_CONNECTING = 2,
        STATE_CONNECTED = 3,
        STATE_JOINING = 4,
        STATE_ERROR_JOINING = 5,
        STATE_JOINED = 6,
        STATE_LEAVE = 7,
        STATE_LEAVING = 8,
        STATE_ERROR_LEAVING = 9,
        STATE_LEFT = 10,
        STATE_ERROR_CONNECTING = 11,
        STATE_RECEIVING = 12,
        STATE_DISCONNECTING = 13,
        STATE_DISCONNECTED = 14,
        STATE_JOIN = 15,

        STATE_DISCONNECT = 16,

        STATE_UNDEFINED = 100,
    }

    public class RoomListInitializedEvent { }

    public class RoomListUpdatedEvent
    {
        public List<RoomMetaData> Rooms;
        public bool IsInitialList { get; private set; }

        internal RoomListUpdatedEvent(IEnumerable<RoomMetaData> rooms, bool isInitialList = false)
        {
            Rooms = new List<RoomMetaData>(rooms);

            IsInitialList = isInitialList;
        }
    }
}