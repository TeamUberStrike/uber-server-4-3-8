using System;
using System.Collections.Generic;
using System.Text;

namespace Cmune.Realtime.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerIdCache
    {
        public void Add(int playerId)
        {
            if (!_allPlayerIDs.Contains(playerId))
            {
                _allPlayerIDs.Add(playerId);

                updateCache();
            }
        }

        public void Remove(int playerId)
        {
            if (_allPlayerIDs.Remove(playerId))
            {
                updateCache();
            }
        }

        public int[] GetAll()
        {
            int[] ids;
            if (_playerIdCache.TryGetValue(0, out ids))
                return ids;
            else return new int[0];
        }

        public int[] GetOthers(int playerId)
        {
            int[] ids;
            if (_playerIdCache.TryGetValue(playerId, out ids))
                return ids;
            else return new int[0];
        }

        private void updateCache()
        {
            _playerIdCache.Clear();
            int[] completeList = _allPlayerIDs.ToArray();

            //put the complete list under id 0
            _playerIdCache[0] = completeList;

            //now populate the dictionary with the other arrays
            foreach (int i in completeList)
            {
                _allPlayerIDs.Remove(i);
                _playerIdCache[i] = _allPlayerIDs.ToArray();
                _allPlayerIDs.Add(i);
            }
        }

        #region FIELDS

        private List<int> _allPlayerIDs = new List<int>();

        private Dictionary<int, int[]> _playerIdCache = new Dictionary<int, int[]>();

        #endregion
    }
}
