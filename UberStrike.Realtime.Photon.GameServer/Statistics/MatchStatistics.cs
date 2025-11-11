using System.Collections.Generic;
using Cmune.Realtime.Common.Utils;
using UberStrike.Realtime.Common;
using System.Linq;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class MatchStatistics
    {
        public MatchStatistics()
        {
            _statsPerPlayer = new Dictionary<int, PlayerStatistics>();
            _damageByWeapon = new Dictionary<int, int>();
        }

        public void Clear()
        {
            _statsPerPlayer.Clear();
            _damageByWeapon.Clear();
        }

        public void AddPlayer(CharacterInfo info)
        {
            if (info.IsLoggedIn)
            {
                PlayerStatistics stats;
                if (_statsPerPlayer.TryGetValue(info.Cmid, out stats))
                {
                    //start a new life
                    stats.UpdateStatistics();

                    stats.ActorId = info.ActorId;
                    stats.Level = info.Level;
                    stats.Name = info.PlayerName;

                    info.Kills = (short)stats.Kills;
                    info.Deaths = (short)stats.Deaths;
                    info.XP = (ushort)stats.GetTotalXp();
                    info.Points = (ushort)stats.Points;
                }
                else
                {
                    _statsPerPlayer[info.Cmid] = new PlayerStatistics(info.ActorId, info.Cmid, info.PlayerName, info.Level);

                    info.Kills = 0;
                    info.Deaths = 0;
                    info.XP = 0;
                    info.Points = 0;
                }
            }
        }

        public PlayerStatistics GetStatistics(int cmid)
        {
            PlayerStatistics stats = null;
            _statsPerPlayer.TryGetValue(cmid, out stats);
            return stats;
        }

        public bool TryGetStatistics(int cmid, out PlayerStatistics stats)
        {
            return _statsPerPlayer.TryGetValue(cmid, out stats);
        }

        public ICollection<PlayerStatistics> PlayerStatistics
        {
            get
            {
                return _statsPerPlayer.Values;
            }
        }

        public CmunePairList<string, int> GetBestPlayers()
        {
            return GetBestPlayers(-1);
        }

        public CmunePairList<string, int> GetBestPlayers(int max)
        {
            CmunePairList<string, int> players = new CmunePairList<string, int>(_statsPerPlayer.Count);

            foreach (PlayerStatistics s in _statsPerPlayer.Values)
                players.Add(s.Name, s.Kills);

            players.Sort((p, q) => -p.Value.CompareTo(q.Value));

            if (max > 0)
                players.Clamp(max);

            return players;
        }

        public static CmunePairList<string, int> GetBestPlayers(int max, ICollection<PlayerStatistics> stats)
        {
            CmunePairList<string, int> players = new CmunePairList<string, int>(stats.Count);
            foreach (PlayerStatistics s in stats)
                players.Add(s.Name, s.Kills);
            players.Sort((p, q) => -p.Value.CompareTo(q.Value));
            if (max > 0)
                players.Clamp(max);
            return players;
        }

        public int GetBestWeaponId()
        {
            return _damageByWeapon.OrderByDescending(w => w.Value).Select(w => w.Key).FirstOrDefault();
        }

        public CmunePairList<int, int> GetBestWeapons(int max)
        {
            CmunePairList<int, int> weapons = new CmunePairList<int, int>(_damageByWeapon);
            weapons.Sort((p, q) => -p.Value.CompareTo(q.Value));
            weapons.Clamp(max);
            return weapons;
        }

        public void AddDamage(int weaponID, int damage)
        {
            if (_damageByWeapon.ContainsKey(weaponID))
                _damageByWeapon[weaponID] += damage;
            else
                _damageByWeapon[weaponID] = damage;
        }

        private Dictionary<int, PlayerStatistics> _statsPerPlayer;

        private Dictionary<int, int> _damageByWeapon;
    }
}