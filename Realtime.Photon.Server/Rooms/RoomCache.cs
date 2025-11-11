using System.Collections.Generic;
using System.Threading;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RoomCache
    {
        /// <summary>
        /// 
        /// </summary>
        private RoomCache()
        {
            this._cacheLock = new ReaderWriterLockSlim();
            this._innerCache = new Dictionary<int, CmuneRoom>();
        }

        /// <summary>
        /// Gets a lite game.
        /// </summary>
        /// <param name="gameName">Game name.</param>
        /// <returns>Lite game.</returns>
        public bool TryGetGame(int roomNumber, out CmuneRoom room)
        {
            this._cacheLock.EnterReadLock();
            try
            {
                return this._innerCache.TryGetValue(roomNumber, out room);
            }
            finally
            {
                this._cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Get a List of all current games
        /// </summary>
        /// <returns></returns>
        public List<CmuneRoom> GetCurrentGames()
        {
            this._cacheLock.EnterReadLock();
            try
            {
                return new List<CmuneRoom>(this._innerCache.Values);
            }
            finally
            {
                this._cacheLock.ExitReadLock();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddRoom(CmuneRoom room)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                _innerCache.Add(room.Number, room);

                Counter.Games.Increment();
            }
            catch (System.Exception ex)
            {
                CmuneDebug.LogError("Failed to CreateCmuneRoom: RoomId = {0}\nException: {1}", room.Number, ex.Message);
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes a game from the cache.
        /// </summary>
        /// <param name="gameName">Game name.</param>
        /// <returns>Success flag.</returns>
        public bool Remove(int roomNumber)
        {
            bool isRemoved = false;

            this._cacheLock.EnterWriteLock();
            try
            {
                isRemoved = this._innerCache.Remove(roomNumber);

                if (isRemoved)
                {
                    Counter.Games.Decrement();
                }
                else
                {
                    if (CmuneDebug.IsWarningEnabled)
                        CmuneDebug.LogWarning("Failed to remove game '{0}'! All Games: {1}", roomNumber, CmunePrint.Values(_innerCache.Keys));
                }
            }
            finally
            {
                this._cacheLock.ExitWriteLock();
            }

            return isRemoved;
        }

        public static int NextRoomID()
        {
            int id;
            _roomIDLock.EnterWriteLock();
            try
            {
                id = ++_lastRoomID;
            }
            finally
            {
                _roomIDLock.ExitWriteLock();
            }
            return id;
        }

        #region PROPERTIES
        /// <summary>
        /// 
        /// </summary>
        public static RoomCache Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RoomCache();

                return _instance;
            }
        }
        #endregion

        #region FIELDS
        /// <summary>
        /// 
        /// </summary>
        private static int _lastRoomID = 100;

        /// <summary>
        /// Gets the dictionary internally used to store games.  
        /// </summary>
        private Dictionary<int, CmuneRoom> _innerCache;

        /// <summary>
        /// Gets the <see cref="ReaderWriterLockSlim"/> used to synchronize access to the cache. 
        /// </summary>
        private ReaderWriterLockSlim _cacheLock;

        private static ReaderWriterLockSlim _roomIDLock = new ReaderWriterLockSlim();

        private static RoomCache _instance;
        #endregion
    }
}
