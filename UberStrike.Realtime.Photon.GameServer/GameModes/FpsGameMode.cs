
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using UberStrike.Realtime.Common;
using UnityEngine;
using Cmune.Realtime.Common.Synchronization;
using UberStrike.Realtime.Common.Utils;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.Photon.GameServer
{
    public abstract class FpsGameMode : ServerGameMode
    {
        public static int MATCH_WAITING_TIME = 25;

        protected FpsGameMode(RemoteMethodInterface rmi, CmuneRoom room)
            : base(rmi, room)
        {
            _statistics = new MatchStatistics();

            _random = new System.Random();

            _spawnPointsByTeamId = new Dictionary<TeamID, SpawnPointManager>();

            _damageEventsByActorId = new Dictionary<int, DamageEvent>();

            _data = room.RoomData as GameMetaData;

            _scheduledJobs = new List<IDisposable>(3)
            {
                _room.ExecutionFiber.ScheduleOnInterval(OnGameFrameSynchronization, 1000, 100),
                _room.ExecutionFiber.ScheduleOnInterval(OnPositionSynchronization, 1000, 50),
                _room.ExecutionFiber.ScheduleOnInterval(DecreasePlayerHealthAndArmor, 1000, 1000)
            };

            _playerNumbers = new Queue<byte>(30);
            for (byte b = 1; b < 31; b++)
            {
                _playerNumbers.Enqueue(b);
            }

            _powerupManager = new PowerupManager(this, room);
        }

        [RoomMessage(RoomMessageType.AddedPeerToGame)]
        protected virtual void OnPlayerEnter(int actorId) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        protected override void OnJoinMode(CharacterInfo actor)
        {
            if (actor.IsSpectator)
            {
                base.OnJoinMode(actor);

                OnSpectatorJoin(actor);
            }
            else
            {
                //first set player data, before calling the base join method
                actor.ResetState();
                actor.PlayerNumber = _playerNumbers.Dequeue();

                base.OnJoinMode(actor);

                _statistics.AddPlayer(actor);

                _damageEventsByActorId[actor.ActorId] = new DamageEvent();

                OnNormalJoin(actor);

                UpdateRankingForAll();
            }
        }

        protected virtual void OnNormalJoin(CharacterInfo actor)
        {
            //game doesn't start but player will respawn
            if (_players.Count < _minPlayerCount)
            {
                //spawn player normally
                RespawnPlayerInSeconds(actor, 0);
            }
            //we have enough players -> start game
            else if (_players.Count == _minPlayerCount)
            {
                SendMethodToAll(FpsGameRPC.Begin);

                StartNewMatch();
            }
            //add player to a running game
            else
            {
                SendMethodToPlayer(actor.ActorId, FpsGameRPC.Begin);

                if (_roundRunning)
                {
                    //set respawn position for new player
                    RespawnPlayerInSeconds(actor, 0);
                    SendMethodToPlayer(actor.ActorId, FpsGameRPC.MatchStart, _roundCount, _endTime);
                }
                else
                {
                    CallMatchEnd(new CharacterInfo[] { actor });

                    SendMethodToPlayer(actor.ActorId, FpsGameRPC.SetEndOfRoundCountdown, GetNextMatchStartingTime());
                }
            }
        }

        protected virtual void OnSpectatorJoin(CharacterInfo actor)
        {
            SendMethodToPlayer(actor.ActorId, FpsGameRPC.Begin);

            if (_roundRunning)
            {
                SendMethodToPlayer(actor.ActorId, FpsGameRPC.MatchStart, _roundCount, _endTime);
            }
            //else
            //{
            //    SendMethodToPlayer(actor.ActorId, FpsGameRPC.Statistics, 0, new StatsCollection(), new StatsCollection(), new int[0]);
            //    SendMatchStatisticsToPlayer(actor.ActorId);
            //    SendMethodToPlayer(actor.ActorId, FpsGameRPC.SetEndOfRoundCountdown, GetNextMatchStartingTime());
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        protected override void OnPlayerLeftGame(int actorId)
        {
            //put playernumber back into the pool
            CharacterInfo i;
            if (_players.TryGetValue(actorId, out i))
            {
                _playerNumbers.Enqueue(i.PlayerNumber);
            }

            base.OnPlayerLeftGame(actorId);

            _damageEventsByActorId.Remove(actorId);

            UpdateRankingForAll();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void StartNewMatch()
        {
            _roundRunning = true;
            _powerupManager.Reset();

            ResetPlayers();

            //reset the statistics to only include the players currently in game
            _statistics.Clear();
            foreach (CharacterInfo c in _players.Values)
            {
                _statistics.AddPlayer(c);
            }

            //set respawn position for all players
            foreach (CharacterInfo c in _players.Values)
            {
                RespawnPlayerInSeconds(c, 0);
            }

            if (_nextRoundTimer != null) _nextRoundTimer.Dispose();
            StartRoundEndTimer();

            _startTime = SystemTime.Running;
            _endTime = _startTime + _data.RoundTime * 1000;
            SendMethodToAll(FpsGameRPC.MatchStart, _roundCount, _endTime);
        }

        protected void StopRoundEndTimer()
        {
            if (_roundEndTimer != null)
            {
                _roundEndTimer.Dispose();
                _roundEndTimer = null;
            }
        }

        protected void StartRoundEndTimer()
        {
            StopRoundEndTimer();
            _roundEndTimer = _room.ScheduleRoomMessage(new RoomMessage(GameRoomMessage.RoundTimeoutEvent, ++_roundCount), (long)_data.RoundTime * 1000);
        }

        /// <summary>
        /// There could be 3 conditions why a round is ending:
        /// 1) Splat limit is reached
        /// 2) Time limit is reached
        /// 3) player count fell below threshold
        /// </summary>
        protected virtual void StopCurrentMatch()
        {
            try
            {
                if (_roundRunning)
                {
                    _roundRunning = false;

                    StopRoundEndTimer();

                    if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

                    //record the statistics for everyone who was part of the current match at any time
                    RecordMatchStatistics();

                    //send match results to players
                    CallMatchEnd(_players.Values);

                    StartNextMatchInSeconds(MATCH_WAITING_TIME);
                }
                else
                {
                    if (CmuneDebug.IsWarningEnabled)
                        CmuneDebug.LogWarning("({0}) - Prevented StopCurrentRound() call of round number '{1}' from execution because round is not running", _room.Number, _roundCount);
                }
            }
            catch (Exception ex)
            {
                CmuneDebug.LogError("Failed to stop current match: " + ex.Message);
            }
        }

        protected void RecordMatchStatistics()
        {
            List<PlayerStatisticsView> scoringPlayers = new List<PlayerStatisticsView>();
            List<PlayerStatisticsView> otherPayers = new List<PlayerStatisticsView>();

            try
            {
                foreach (PlayerStatistics s in _statistics.PlayerStatistics)
                {
                    //include latest "perLife" stats
                    s.UpdateStatistics();
                }

                //now detect farmers
                var cheaters = CheatDetection.DetectXpFarmers(_statistics.PlayerStatistics);

                foreach (PlayerStatistics s in _statistics.PlayerStatistics)
                {
                    CharacterInfo info;
                    bool isPlayerInGame = _players.TryGetValue(s.ActorId, out info);

                    if (cheaters.Contains(s.Cmid))
                    {
                        OnKickPlayer(s.Cmid, 60);
                    }
                    else
                    {
                        if (isPlayerInGame)
                            scoringPlayers.Add(s.GetPlayerStatisticsView(s.TotalStats));
                        else
                            otherPayers.Add(s.GetPlayerStatisticsView(s.TotalStats));
                    }
                }

                UberStrike.WebService.DotNet.UserWebServiceClient.SetScore(new MatchView(scoringPlayers, otherPayers, _data.MapID, GameUtils.GetGameModeType(_data.GameMode), _data.RoundTime, _data.MaxPlayers));
            }
            catch (Exception ex)
            {
                CmuneDebug.LogError("Failed to Record Match Stats: " + ex.Message);
            }
        }

        protected void CallMatchEnd(IEnumerable<CharacterInfo> players)
        {
            //get all players present at end of round
            //var presentPlayers = new List<PlayerStatistics>(_statistics.PlayerStatistics.Where(s => _players.ContainsKey(s.ActorId)));
            var presentPlayers = new List<PlayerStatistics>();
            foreach (var p in players)
            {
                PlayerStatistics s;
                if (_statistics.TryGetStatistics(p.Cmid, out s))
                {
                    //make sure to update the player's current team
                    s.Team = p.TeamID;
                }
                else
                {
                    s = new PlayerStatistics(p.ActorId, p.Cmid, p.PlayerName, p.Level);
                }
                presentPlayers.Add(s);
            }

            //find achievers
            var achievers = new Dictionary<int, Dictionary<byte, ushort>>();
            presentPlayers.ForEach(p => achievers.Add(p.Cmid, new Dictionary<byte, ushort>()));

            foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
            {
                var winner = AchievementHelper.SelectWinnerOfAchievement(type, presentPlayers);
                if (achievers.ContainsKey(winner.Key))
                    achievers[winner.Key].Add((byte)type, winner.Value);
            }

            //create final players stats list
            var playerStats = new List<StatsSummary>(presentPlayers.ConvertAll(p =>
            {
                return new StatsSummary()
                {
                    Name = p.Name,
                    Level = p.Level,
                    Deaths = p.Deaths,
                    Kills = p.Kills,
                    //Achievements = new Dictionary<byte, ushort>() 
                    //{ 
                    //  { (byte)AchievementType.CostEffective, 340 },
                    //  { (byte)AchievementType.HardestHitter, 666 },
                    //  { (byte)AchievementType.MostAggressive, 26 },
                    //  { (byte)AchievementType.MostValuable, 15 },
                    //  { (byte)AchievementType.TriggerHappy, 5 },
                    //  { (byte)AchievementType.SharpestShooter, 8 },
                    //},
                    Achievements = achievers[p.Cmid],
                    Team = p.Team,
                    Cmid = p.Cmid
                };
            }));

            playerStats.Sort((a, b) => -(a.Kills / Mathf.Max(a.Deaths, 1f)).CompareTo(b.Kills / Mathf.Max(a.Deaths, 1f)));

            //choose best weapon
            var bestWeapon = _statistics.GetBestWeaponId();

            //send data
            foreach (var p in presentPlayers)
            {
                SendMethodToPlayer(p.ActorId, FpsGameRPC.MatchEnd, CreateEndOfMatchData(p, playerStats, bestWeapon));
            }
        }

        protected EndOfMatchData CreateEndOfMatchData(PlayerStatistics playerStats, List<StatsSummary> allPlayers, int bestWeapon)
        {
            //initialize RoundEndData
            var data = new EndOfMatchData()
            {
                RoundNumber = _roundCount,
                MostValuablePlayers = allPlayers,
                MostEffecientWeaponId = bestWeapon,
            };

            //Fill in personal statistics
            data.PlayerStatsTotal = playerStats.TotalStats;
            data.PlayerStatsBestPerLife = playerStats.BestLifeStats;
            data.PlayerXpEarned = new Dictionary<byte, ushort>()
            {
                { PlayerXPEventViewId.Splat, (ushort) playerStats.GetSplatsXP() },
                { PlayerXPEventViewId.HeadShot,  (ushort)playerStats.GetHeadshotXP() },
                { PlayerXPEventViewId.Humiliation,  (ushort)playerStats.GetSmackdownXP() },
                { PlayerXPEventViewId.Nutshot, (ushort) playerStats.GetNutshotXP() },
                { PlayerXPEventViewId.Damage,  (ushort)playerStats.GetDamageXP() },
            };

            return data;
        }

        protected virtual void StartNextMatchInSeconds(int seconds)
        {
            if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

            _nextRoundTimer = _room.ExecutionFiber.Schedule(StartNewMatch, seconds * 1000);
            _nextRoundStartingTime = SystemTime.Running + seconds * 1000;

            SendMethodToAll(FpsGameRPC.SetEndOfRoundCountdown, seconds);
        }

        protected int GetNextMatchStartingTime()
        {
            return (_nextRoundStartingTime - SystemTime.Running) / 1000;
        }

        protected void DecreasePlayerHealthAndArmor()
        {
            foreach (CharacterInfo p in _players.Values)
            {
                if (p.Health > 100) p.Health--;
                if (p.Armor.ArmorPoints > p.Armor.ArmorPointCapacity) p.Armor.ArmorPoints--;
            }
        }

        protected void KillPlayer(int actorId)
        {
            if (_roundRunning)
            {
                CharacterInfo t = null;

                //only continue if we find matching player data
                if (_players.TryGetValue(actorId, out t) && t != null)
                {
                    PlayerStatistics targetStats = _statistics.GetStatistics(t.Cmid);

                    t.Health = 0;

                    //update stats for TARGET
                    if (targetStats != null)
                    {
                        targetStats.RegisterDeath();

                        t.Deaths = (short)targetStats.Deaths;

                        //reset per life statistics
                        targetStats.UpdateStatistics();
                    }

                    UpdateRankingForAll();

                    OnPlayerSplatted(t, true);
                }
            }
        }

        protected virtual bool CanPlayersDoDamage
        {
            get { return _roundRunning; }
        }

        [NetworkMethod(FpsGameRPC.PlayerHit)]
        protected virtual void OnPlayerHit(int shooter, int target, short damage, byte bodyPart, int shotId,
            byte angle, int weaponId, byte weaponClass, int damageEffectFlag, float damageEffectValue)
        {
            if (GameFlags.IsFlagSet(GameFlags.GAME_FLAGS.Instakill, _data.GameModifierFlags))
                damage = 999;

            if (CanPlayersDoDamage)
            {
                CharacterInfo s = null;
                CharacterInfo t = null;

                //only continue if we find matching player data
                if (_players.TryGetValue(shooter, out s) && _players.TryGetValue(target, out t) && s != null && t != null)
                {
                    //only allow shots by a player that is alive
                    if (IsHitAllowed(s, t))
                    {
                        bool countGoodStats = IsHitPositive(s, t);

                        PlayerStatistics shooterStats = _statistics.GetStatistics(s.Cmid);
                        PlayerStatistics targetStats = _statistics.GetStatistics(t.Cmid);

                        t.Health -= t.Armor.AbsorbDamage(damage, (BodyPart)bodyPart);

                        DamageEvent eventInfo;
                        if (_damageEventsByActorId.TryGetValue(target, out eventInfo))
                        {
                            //don't add self damage feedback
                            if (shooter != target)
                                eventInfo.AddDamage(angle, damage, bodyPart, damageEffectFlag, damageEffectValue);
                        }

                        //update stats for TARGET
                        if (targetStats != null)
                        {
                            targetStats.RegisterTargetHit(damage, !t.IsAlive, shooter == target);

                            if (!t.IsAlive)
                            {
                                //reset per life statistics
                                targetStats.UpdateStatistics();

                                t.Kills = (short)targetStats.Kills;
                                t.Deaths = (short)targetStats.Deaths;
                                t.XP = (ushort)targetStats.GetTotalXp();
                                t.Points = (ushort)targetStats.Points;
                            }
                        }

                        int xp = 0;
                        //update stats for SHOOTER
                        if (shooterStats != null)
                        {
                            //don't count damage that goes below zero
                            int pureDamage = t.IsAlive ? damage : damage + t.Health;
                            xp = shooterStats.RegisterShooterHit(pureDamage, !t.IsAlive, (UberstrikeItemClass)weaponClass, (BodyPart)bodyPart, shotId, countGoodStats);

                            //increment the damage statistic per weapon ID
                            if (countGoodStats)
                            {
                                _statistics.AddDamage(weaponId, pureDamage);

                                if (!t.IsAlive)
                                {
                                    shooterStats.CurrentLifeStats.Xp += xp;
                                    shooterStats.CurrentLifeStats.Points += t.GetPointBonus();

                                    //update the player 
                                    s.Kills = (short)shooterStats.Kills;
                                    s.Deaths = (short)shooterStats.Deaths;
                                    s.XP = (ushort)shooterStats.GetTotalXp();
                                    s.Points = (ushort)shooterStats.Points;
                                }
                            }
                        }

                        //if a splat happend - update ranks
                        if (!t.IsAlive)
                        {
                            if (countGoodStats)
                            {
                                UpdateSplatCount(s);
                            }

                            UpdateRankingForAll();

                            SendMethodToAll(FpsGameRPC.SplatGameEvent, shooter, target, weaponClass, bodyPart);

                            OnPlayerSplatted(t, shooter == target);

                            ////update XP & Point display on client
                            //if (shooterStats != null && shooterStats.HavePointsOrXpChanged)
                            //    SendMethodToPlayer(s.ActorId, FpsGameRPC.UpdateXpAndPoints, shooterStats.GetCurrentSkillsXP(), shooterStats.CurrentPoints);
                        }
                    }
                }
            }
        }

        protected abstract void OnPlayerSplatted(CharacterInfo player, bool suicide);

        protected List<CharacterInfo> UpdatePlayerRanks()
        {
            List<CharacterInfo> rankPlayers = new List<CharacterInfo>(_players.Values);
            rankPlayers.Sort((p, q) => -p.Kills.CompareTo(q.Kills));

            return rankPlayers;
        }

        [NetworkMethod(FpsGameRPC.SetDamage)]
        protected void OnSetDamage(int target, short damage)
        {
            CharacterInfo t;
            if (_players.TryGetValue(target, out t))
            {
                t.Health -= t.Armor.AbsorbDamage(damage, BodyPart.Body);
            }
        }

        [NetworkMethod(FpsGameRPC.SetSpawnPoints)]
        protected void OnSetSpawnPoints(byte normalCount, byte redCount, byte blueCount)
        {
            if (_spawnPointsByTeamId.Count == 0)
            {
                _spawnPointsByTeamId[TeamID.NONE] = new SpawnPointManager(normalCount);
                _spawnPointsByTeamId[TeamID.RED] = new SpawnPointManager(redCount);
                _spawnPointsByTeamId[TeamID.BLUE] = new SpawnPointManager(blueCount);
            }
        }

        protected int GetRandomSpawnPoint(TeamID t)
        {
            SpawnPointManager list;
            if (_spawnPointsByTeamId.TryGetValue(t, out list))
            {
                return list.GetSpawnPoint();
            }
            else
            {
                return 0;
            }
        }

        [NetworkMethod(FpsGameRPC.RequestRespawnForPlayer)]
        protected void OnRequestRespawnForPlayer(int actorId)
        {
            CharacterInfo info;
            if (_players.TryGetValue(actorId, out info))
            {
                RespawnPlayerInSeconds(info, 4);
            }
        }

        [NetworkMethod(FpsGameRPC.SetPlayerReadyForNextRound)]
        protected virtual void OnSetPlayerReadyForNextRound(int actorId)
        {
            CharacterInfo character;
            if (_players.TryGetValue(actorId, out character))
            {
                //character.ResetState();
                character.IsReadyForGame = true;
            }

            if (GetNextMatchStartingTime() > 3)
            {
                bool isEverybodyReady = _players.Values.Aggregate(true, (current, i) => current & i.IsReadyForGame);

                if (isEverybodyReady)
                {
                    StartNextMatchInSeconds(3);
                }
            }
        }

        [NetworkMethod(FpsGameRPC.SetPowerUpCount)]
        protected void OnSetPowerupCount(int actorId, List<byte> respawnDurations)
        {
            if (PlayersCount == 1)
            {
                _powerupManager.SetPowerupCount(respawnDurations);

                //CmuneDebug.Log("OnSetPowerupCount: " + _pickupState.Count + " " + count);
            }
            else
            {
                //CmuneDebug.Log("OnSetPowerupCount: " + actorId + " " + PlayersCount);
                SendMethodToPlayer(actorId, FpsGameRPC.SetPowerupState, _powerupManager.GetState());
            }
        }

        [NetworkMethod(FpsGameRPC.PowerUpPicked)]
        protected void OnPowerUpPicked(int player, int powerupId, byte type, byte value)
        {
            //CmuneDebug.Log("OnPowerUpPicked: Player = " + player + " " + powerupId);

            CharacterInfo info;
            if (_players.TryGetValue(player, out info) && info.IsAlive)
            {
                _powerupManager.PickPowerup(powerupId);

                PlayerStatistics stats;
                if (_statistics.TryGetStatistics(info.Cmid, out stats))
                {
                    switch ((PickupItemType)type)
                    {
                        case PickupItemType.Armor:
                            {
                                int oldArmor = info.Armor.ArmorPoints;
                                info.Armor.ArmorPoints += value;
                                stats.CurrentLifeStats.ArmorPickedUp += (info.Armor.ArmorPoints - oldArmor);
                                break;
                            }
                        case PickupItemType.Health:
                            {
                                int oldHealth = info.Health;

                                switch (value)
                                {
                                    case 5:
                                    case 100:
                                        {
                                            if (info.Health < 200)
                                            {
                                                info.Health = (short)CmuneMath.Clamp(info.Health + value, 0, 200);
                                            }
                                        } break;
                                    case 25:
                                    case 50:
                                        {
                                            if (info.Health < 100)
                                            {
                                                info.Health = (short)CmuneMath.Clamp(info.Health + value, 0, 100);
                                            }
                                        } break;
                                }

                                stats.CurrentLifeStats.HealthPickedUp += (info.Health - oldHealth);
                                break;
                            }
                        case PickupItemType.Coin:
                            {
                                stats.CurrentLifeStats.Points++;
                                break;
                            }
                    }
                }
                else
                {
                    CmuneDebug.Log("OnPowerUpPicked: Cannot find stats with Id = " + player);
                }
            }
            else
            {
                CmuneDebug.Log("OnPowerUpPicked: Cannot find player with Id = " + player);
            }
        }

        [NetworkMethod(FpsGameRPC.IncreaseHealthAndArmor)]
        protected void OnIncreaseHealthAndArmor(int player, byte health, byte armor)
        {
            CharacterInfo info;
            if (_players.TryGetValue(player, out info) && info.IsAlive)
            {
                if (health > 0 && info.Health < 200)
                {
                    info.Health = (short)CmuneMath.Clamp(info.Health + health, 0, 200);
                }

                if (armor > 0 && info.Armor.ArmorPoints < 200)
                {
                    info.Armor.ArmorPoints = CmuneMath.Clamp(info.Armor.ArmorPoints + armor, 0, 200);
                }
            }
        }

        [NetworkMethod(FpsGameRPC.DoorOpen)]
        protected void OnDoorOpen(int doorId)
        {
            SendMethodToAll(FpsGameRPC.DoorOpen, doorId);
        }

        [NetworkMethod(FpsGameRPC.SetPlayerSpawnPosition)]
        protected void OnSetPlayerSpawnPosition(int actorId, Vector3 pos)
        {
            CharacterInfo player;
            if (_players.TryGetValue(actorId, out player))
            {
                /* why reset other players' delta cache when I respawn? */
                //foreach (CharacterInfo i in _players.Values)
                //{
                //    if (i.ActorId != actorId)
                //        i.ResetDeltaCache();
                //}

                player.Cache.Clear();
                player.Position = pos;

                SendMethodToOthers(actorId, FpsGameRPC.SetPlayerSpawnPosition, player.PlayerNumber, player.Position);
            }
        }

        private void SetSample(int actorId, byte number, Vector3 pos, int timestamp)
        {
            PositionSample sample;
            if (_positionSamples.TryGetValue(actorId, out sample))
            {
                sample.Velocity = (pos - sample.Position) / (float)Mathf.Abs(timestamp - sample.TimeStamp);
            }
            else
            {
                sample = new PositionSample();
                _positionSamples[actorId] = sample;
            }

            sample.PlayerNumber = number;
            sample.Position = pos;
            sample.TimeStamp = timestamp;
        }

        [NetworkMethod(FpsGameRPC.PositionUpdate)]
        protected void OnPositionUpdate(List<byte> bytes)
        {
            byte[] update = bytes.ToArray();
            int i = 0;

            int actorId = DefaultByteConverter.ToInt(update, ref i);
            CharacterInfo player;
            if (_players.TryGetValue(actorId, out player))
            {
                SetSample(actorId,
                    player.PlayerNumber,
                    ShortVector3.FromBytes(update, ref i),
                    DefaultByteConverter.ToInt(update, ref i));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnPositionSynchronization()
        {
            if (_positionSamples.Count > 1)
            {
                int timeStamp = SystemTime.Running;
                List<byte> bytes = new List<byte>(1 + (_players.Count * 11));

                bytes.Add((byte)_players.Count);
                foreach (var p in _positionSamples.Values)
                {
                    bytes.Add(p.PlayerNumber);
                    //fallback update, in case the packet it totally out of date
                    if (p.TimeStamp + 500 < SystemTime.Running)
                        p.TimeStamp = SystemTime.Running;
                    DefaultByteConverter.FromInt(p.TimeStamp, ref bytes);
                    ShortVector3.Bytes(bytes, p.Position);
                }
                SendMethodToAllUnreliable(FpsGameRPC.PositionUpdate, (object)bytes);
            }
        }

        protected virtual bool IsHitPositive(CharacterInfo a, CharacterInfo b)
        {
            return a.ActorId != b.ActorId;
        }

        protected virtual bool IsHitAllowed(CharacterInfo a, CharacterInfo b)
        {
            return a.IsAlive && b.IsAlive;
        }

        protected virtual bool IsSplatLimitReached
        {
            get { return _players.Values.Any(i => i.Kills >= _data.SplatLimit); }
        }

        protected virtual void UpdateSplatCount(CharacterInfo i) { }

        protected virtual void UpdateRankingForAll()
        {
            List<CharacterInfo> rankPlayers = UpdatePlayerRanks();

            if (rankPlayers.Count > 0)
            {
                short bestKills = rankPlayers[0].Kills;
                foreach (var p in rankPlayers)
                {
                    SendMethodToPlayer(p.ActorId, FpsGameRPC.UpdateSplatCount, p.Kills, bestKills, bestKills > 0 && p.Kills == bestKills && rankPlayers.Count(c => c.Kills == bestKills) == 1);
                }
            }
        }

        /// <summary>
        /// Internal Call
        /// </summary>
        protected virtual void OnGameFrameSynchronization()
        {
            SendMethodToAll(FpsGameRPC.DeltaPlayerListUpdate, (object)SyncObjectBuilder.GetSyncData(_players.Values, false));

            foreach (var ev in _damageEventsByActorId)
            {
                if (ev.Value.Count > 0)
                {
                    SendMethodToPlayer(ev.Key, FpsGameRPC.PlayerEvent, ev.Value);
                    ev.Value.Clear();
                }
            }
        }

        /// <summary>
        /// Internal Call
        /// </summary>
        /// <param name="roundNumber"></param>
        [RoomMessage(GameRoomMessage.RoundTimeoutEvent)]
        protected virtual void OnRoundTimeoutEvent(int roundNumber)
        {
            if (_roundCount == roundNumber)
            {
                StopCurrentMatch();
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("({0}) - The last RoundTimeoutEvent was not cancelled before the next round was started. Use _roundEndTimer.Cancel() to stop the current scheduled message from beeing executed!\nCurrent round = {1} Argument round = {2}", _room.Number, _roundCount, roundNumber);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ResetPlayers()
        {
            //scramble all spawn points to avoid repetitive spawn patterns
            foreach (var s in _spawnPointsByTeamId.Values)
            {
                s.Scramble();
            }

            foreach (CharacterInfo p in _players.Values)
            {
                p.ResetScore();
                p.ResetState();
            }

            SendMethodToAll(FpsGameRPC.DeltaPlayerListUpdate, (object)SyncObjectBuilder.GetSyncData(_players.Values, false));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="seconds"></param>
        protected void RespawnPlayerInSeconds(CharacterInfo actor, int seconds)
        {
            SendMethodToPlayer(actor.ActorId, FpsGameRPC.SetNextSpawnPointForPlayer, GetRandomSpawnPoint(actor.TeamID), seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(Name + ":\n");
            foreach (CharacterInfo p in _players.Values)
                builder.Append("  (").Append(p.ToString()).Append(")\n");
            return builder.ToString();
        }

        protected override void Dispose(bool dispose)
        {
            if (_isDisposed) return;

            if (dispose)
            {
                StopRoundEndTimer();
                if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

                //stop all class jobs
                foreach (IDisposable i in _scheduledJobs)
                {
                    if (i != null) i.Dispose();
                }
            }

            base.Dispose(dispose);
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsUnlimited
        {
            get { return _data.IsTimeUnlimited; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _data.RoomName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MatchTime
        {
            get { return _data.RoundTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PlayersCount
        {
            get { return _players.Count; }
        }

        public virtual bool HasEnoughPlayers
        {
            get { return _players.Count > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatchStatistics Statistics
        {
            get { return _statistics; }
        }

        #endregion

        #region Fields

        protected int _minPlayerCount = 1;

        protected Queue<byte> _playerNumbers;

        protected System.Random _random;

        private Dictionary<TeamID, SpawnPointManager> _spawnPointsByTeamId;
        protected Dictionary<int, DamageEvent> _damageEventsByActorId;
        protected MatchStatistics _statistics;

        private readonly List<IDisposable> _scheduledJobs;

        protected PowerupManager _powerupManager;

        protected IDisposable _roundEndTimer;
        protected int _startTime;
        protected int _endTime;

        protected IDisposable _nextRoundTimer;
        protected int _nextRoundStartingTime;

        protected GameMetaData _data;

        protected int _roundCount = 0;
        protected bool _roundRunning = false;
        protected bool _isScoringEnabled = true;
        #endregion
    }
}
