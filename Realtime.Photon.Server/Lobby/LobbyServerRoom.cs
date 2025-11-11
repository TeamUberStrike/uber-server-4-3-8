
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;
using log4net;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// Most of the code should be moved to ServerLobbyCenter
    /// </summary>
    public class LobbyServerRoom : CmuneRoom
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameName">game name.</param>
        private LobbyServerRoom()
            : base(new RoomMetaData(StaticRoomID.LobbyCenter, "The Lobby", ServerSettings.ConnectionString), false, false)
        {
            _cacheLock = new ReaderWriterLockSlim();

            _roomListChanges = new Dictionary<CmuneRoomID, RoomMetaData>();
            _roomList = new Dictionary<CmuneRoomID, RoomMetaData>();
            _roomListDeletes = new List<CmuneRoomID>();
            _roomListSubscribers = new Dictionary<int, CmunePeer>();

            ExecutionFiber.ScheduleOnInterval(() => SendDeltaGameList(), 2000, 2000);
        }

        /// <summary>
        /// Every game instance (or process) has a queue of incoming operations to execute. Per game
        /// ExecuteOperation() is called in order, serial and in one thread.
        /// </summary>
        /// <param name="operation">operation to execute.</param>
        protected override void ExecuteOperation(CmunePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case CmuneOperationCodes.MessageToServer:
                    {
                        HandleMessageToServer(peer, operationRequest);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameJoin:
                    {
                        HandleJoin(peer, operationRequest, sendParameters);
                        break;
                    }

                case CmuneOperationCodes.PhotonGameLeave:
                    {
                        HandleLeave(peer, operationRequest, sendParameters);
                        break;
                    }
            }
        }

        #region GAME LISTS

        public void RegisterToLobbyEvents(int peerID)
        {
            CmunePeer p;
            if (Actors.TryGetPeerByActorID(peerID, out p))
            {
                //first send him all games again
                SendCompleteGameList(p);

                //now register him for the delta updates
                _roomListSubscribers[peerID] = p;
            }
        }

        public void UnregisterToLobbyEvents(int peerID)
        {
            _roomListSubscribers.Remove(peerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        [RoomMessage(RoomMessageType.GameRemoved)]
        public void OnGameRemoved(CmuneRoomID id)
        {
            //CmuneDebug.Log("RoomMessageType.GameRemoved " + id);

            _cacheLock.EnterWriteLock();
            try
            {
                _roomList.Remove(id);

                _roomListDeletes.Add(id);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Copies the game's info to an internal game-list as either String or String[] and updates a "changed" list.
        /// The Hashtable's Key is always the gameID (gameInfo[0]).
        /// The Value sent to clients can be either a single String (gameInfo.Length <= 2) or a String[].
        /// </summary>
        /// <param name="gameInfo">See summary.</param>
        [RoomMessage(RoomMessageType.GameUpdated)]
        public void OnGameUpdated(RoomMetaData gameInfo)
        {
            //if (CmuneDebug.IsDebugEnabled)
            //    CmuneDebug.Log("RoomMessageType.GameUpdated " + gameInfo.RoomID.ID);

            _cacheLock.EnterWriteLock();
            try
            {
                //CmuneDebug.LogWarning("updateGameWithID " + gameInfo.RoomID.ID);

                _roomList[gameInfo.RoomID] = gameInfo;

                _roomListChanges[gameInfo.RoomID] = gameInfo;

                //remove it from the removelist if this room is getting fresh updates
                _roomListDeletes.Remove(gameInfo.RoomID);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        private void GameServerDown(string server)
        {
            CmuneDebug.LogError("GameServerDown {0}, current game count: {1}", server, _roomList.Count);

            _cacheLock.EnterWriteLock();
            try
            {
                int roomCount = 0;
                Queue<CmuneRoomID> deletedRooms = new Queue<CmuneRoomID>();
                foreach (KeyValuePair<CmuneRoomID, RoomMetaData> r in _roomList)
                {
                    if (r.Key.Server == server)
                    {
                        deletedRooms.Enqueue(r.Key);
                    }
                    roomCount++;
                }

                CmuneDebug.LogError("{0} games removed from lobby", roomCount);

                while (deletedRooms.Count > 0)
                {
                    CmuneRoomID r = deletedRooms.Dequeue();
                    bool ok1 = _roomList.Remove(r);
                    bool ok2 = _roomListChanges.Remove(r);
                    _roomListDeletes.Add(r);

                    //CmuneDebug.LogErrorFormat("GameServerDown {0}, delete game: {1}/{2}", r.Server, ok1, ok2);
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public bool GetDeltaGameList(out List<Hashtable> tables)
        {
            //allocate a new hashtable and don't clear the current one, 
            //as it's still in use by the operation thread
            tables = new List<Hashtable>(1);

            _cacheLock.EnterWriteLock();
            try
            {
                foreach (CmuneRoomID d in _roomListDeletes)
                {
                    //CmuneDebug.Log("  -- clean deleted game from lists " + d.ID);
                    _roomListChanges.Remove(d);

                    _roomList.Remove(d);
                }

                //secondly get all deletes
                if (_roomListDeletes.Count > 0)
                {
                    CreateGameDataPackages(10, _roomListDeletes, tables, ParameterKeys.LobbyRoomDelete);

                    //if (CmuneDebug.IsDebugEnabled)
                    //{
                    //    foreach (CmuneRoomID room in _roomListDeletes)
                    //        CmuneDebug.Log("  -- send deleteted room " + room.ID);
                    //}

                    _roomListDeletes.Clear();
                }

                //first gather all changes
                if (_roomListChanges.Count > 0)
                {
                    CreateGameDataPackages(10, _roomListChanges.Values, tables, ParameterKeys.LobbyRoomUpdate);

                    //if (CmuneDebug.IsDebugEnabled)
                    //{
                    //    foreach (RoomMetaData room in _roomListChanges.Values)
                    //        CmuneDebug.Log("  -- send changed room " + room.RoomID.ID);
                    //}

                    _roomListChanges.Clear();
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }

            return tables.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public bool GetCompleteGameList(out List<Hashtable> tables)
        {
            //allocate a new hashtable and don't clear the current one, 
            //as it's still in use by the operation thread
            tables = new List<Hashtable>(10);

            _cacheLock.EnterReadLock();
            try
            {
                //first gather all changes
                if (_roomList.Count > 0)
                {
                    CreateGameDataPackages(10, _roomList.Values, tables, ParameterKeys.LobbyRoomUpdate);
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }

            return tables.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tables"></param>
        /// <returns></returns>
        private bool CreateGameDataPackages<T>(int maxCount, ICollection<T> list, List<Hashtable> tables, byte tag)
        {
            List<T> data = new List<T>((int)Math.Ceiling(list.Count / (float)maxCount));

            IEnumerator<T> iter = list.GetEnumerator();
            int i = 0;
            while (iter.MoveNext())
            {
                i++;

                data.Add(iter.Current);

                //if you reached the last element or the packetsize threshold, we store our data in the hashtable
                if (i == list.Count || (i > 0 && i % maxCount == 0))
                {
                    Hashtable t = new Hashtable(1);

                    t.Add(tag, RealtimeSerialization.ToBytes(data).ToArray());

                    tables.Add(t);

                    data.Clear();
                }
            }

            return tables.Count > 0;
        }

        /// <summary>
        /// Sends a list of updated game-info to the players, since last send.
        /// </summary>
        private void SendDeltaGameList()
        {
            List<Hashtable> gamelist;
            if (_roomListSubscribers.Count > 0 && GetDeltaGameList(out gamelist))
            {
                foreach (Hashtable h in gamelist)
                {
                    if (h.ContainsKey((byte)ParameterKeys.LobbyRoomDelete))
                    {
                        PublishEvent(CreateEventData(CmuneEventCodes.GameListRemoval, h), _roomListSubscribers.Values, new SendParameters());
                    }
                    else
                    {
                        PublishEvent(CreateEventData(CmuneEventCodes.GameListUpdate, h), _roomListSubscribers.Values, new SendParameters());
                    }
                }
            }
        }

        /// <summary>
        /// Sends an initial list of game-info to a player (on join).
        /// </summary>
        private void SendCompleteGameList(CmunePeer player)
        {
            Hashtable games = new Hashtable();

            games.Add(ParameterKeys.LobbyRoomUpdate, RealtimeSerialization.ToBytes(_roomList.Values).ToArray());

            PublishEvent(CreateEventData(CmuneEventCodes.GameListInit, games), player, new SendParameters());
        }

        #endregion

        protected override int RemovePeerFromGame(CmunePeer peer)
        {
            int actorID = base.RemovePeerFromGame(peer);

            _roomListSubscribers.Remove(actorID);

            if (peer.PeerType == PeerType.GamerServer)
            {
                GameServerDown(peer.Address.ConnectionString);
            }

            return actorID;
        }

        #region PROPERTIES

        public static LobbyServerRoom Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LobbyServerRoom();
                return _instance;
            }
        }

        public override bool IsRoomFull
        {
            get { return false; }
        }

        #endregion

        #region FIELDS

        private ReaderWriterLockSlim _cacheLock;

        /// <summary>
        /// Logger for debug.
        /// </summary>
        private static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// List of changes between two consecutive GameListPublish() calls.
        /// </summary>
        private Dictionary<CmuneRoomID, RoomMetaData> _roomListChanges;

        /// <summary>
        /// Complete list of known, open games with game info (send on Lobby-Join).
        /// </summary>
        private Dictionary<CmuneRoomID, RoomMetaData> _roomList;

        public RoomMetaData[] CurrentRooms
        {
            get { return Conversion.ToArray<RoomMetaData>(_roomList.Values); }
        }

        /// <summary>
        /// List of deleted games, to be send in GameListPublish().
        /// </summary>
        private List<CmuneRoomID> _roomListDeletes;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, CmunePeer> _roomListSubscribers;

        private static LobbyServerRoom _instance;

        #endregion
    }
}