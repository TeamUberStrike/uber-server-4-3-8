using System;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using Photon.SocketServer;
using UberStrike.Realtime.Common;
using UberStrike.Realtime.Common.Utils;

namespace UberStrike.Realtime.Photon.GameServer
{
    [NetworkClass(GameModeID.EliminationMode)]
    public class TeamEliminationGameMode : TeamDeathMatchGameMode
    {
        public TeamEliminationGameMode(RemoteMethodInterface rmi, CmuneRoom room)
            : base(rmi, room)
        {
            _redWins = _blueWins = 0;
            _redPlayers = _bluePlayers = 0;
            _currentRoundCount = 0;

            _minPlayerCount = 2;

            _endTime = SystemTime.Running;
        }

        protected override void OnNormalJoin(CharacterInfo actor)
        {
            //CmuneDebug.Log("OnNormalJoin " + actor.ActorId + " " + CurrentState);

            //update team count
            UpdateTeamBalance();

            SendMethodToPlayer(actor.ActorId, FpsGameRPC.Begin);

            switch (CurrentState)
            {
                case State.WaitForPlayers:
                    {
                        //we can start a match now
                        if (HasEnoughPlayers)
                        {
                            StartNewMatchAfterGracetime(5);
                        }
                        //load my avatar but keep waiting for other dudes
                        else
                        {
                            RespawnPlayerInSeconds(actor, 0);
                            SendMethodToPlayer(actor.ActorId, FpsGameRPC.SyncRoundTime, _endTime);
                        }
                    }
                    break;

                case State.BetweenRounds:
                case State.RoundRunning:
                    {
                        SendMethodToPlayer(actor.ActorId, FpsGameRPC.UpdateRoundStats, CurrentRoundCount, _blueWins, _redWins);

                        _waitingPlayers.Add(actor.ActorId);
                        //CmuneDebug.Log("Add _waitingPlayers " + actor.ActorId + "=> " + CmunePrint.Values(_waitingPlayers));

                        LoadPendingAvatarOfActivePlayers(actor.ActorId);

                        // put the player into spectator mode
                        SendMethodToPlayer(actor.ActorId, FpsGameRPC.PlayerSpectator, actor.ActorId);
                        SendMethodToPlayer(actor.ActorId, FpsGameRPC.MatchStart, _roundCount, _endTime);
                    }
                    break;
                case State.Stopped:
                    {
                        _waitingPlayers.Add(actor.ActorId);

                        SendMethodToPlayer(actor.ActorId, FpsGameRPC.UpdateRoundStats, CurrentRoundCount, _blueWins, _redWins);

                        CallMatchEnd(new CharacterInfo[] { actor });
                        SendMethodToPlayer(actor.ActorId, FpsGameRPC.SetEndOfRoundCountdown, GetNextMatchStartingTime());
                    }
                    break;
            }

            _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), _bluePlayers, _redPlayers);
        }

        protected override void OnSpectatorJoin(CharacterInfo actor)
        {
            SendMethodToPlayer(actor.ActorId, FpsGameRPC.Begin);

            if (_roundRunning)
            {
                SendMethodToPlayer(actor.ActorId, FpsGameRPC.MatchStart, _roundCount, _endTime);
            }
            else
            {
                SendMethodToPlayer(actor.ActorId, FpsGameRPC.SyncRoundTime, new SendParameters(), _endTime);
                LoadPendingAvatarOfActivePlayers(actor.ActorId);
            }
        }

        private HashSet<int> _waitingPlayers = new HashSet<int>();

        protected override void OnPlayerLeftGame(int actorID)
        {
            base.OnPlayerLeftGame(actorID);

            //update team count
            UpdateTeamBalance();
            _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), _bluePlayers, _redPlayers);

            //stop the running game if players not enough anymore
            if (!EnoughPlayersToKeepTheGameRunning())
            {
                OnRoundEnd();
            }
        }

        protected override void OnRoundTimeoutEvent(int roundNumber)
        {
            if (_roundCount == roundNumber)
            {
                //end round because countdown finished
                OnRoundEnd();
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("({0}) - The last RoundTimeoutEvent was not cancelled before the next round was started. Use _roundEndTimer.Cancel() to stop the current scheduled message from beeing executed!\nCurrent round = {1} Argument round = {2}", _room.Number, _roundCount, roundNumber);
            }
        }

        protected override bool CanPlayersDoDamage
        {
            get { return CurrentState == State.RoundRunning || CurrentState == State.WaitForPlayers; }
        }

        protected override void OnPlayerSplatted(CharacterInfo player, bool suicide)
        {
            if (CurrentState == State.RoundRunning)
            {
                int blue = 0, red = 0;
                GetPlayersAlive(out blue, out red);

                if (red == 0 || blue == 0)
                {
                    //end round because one team was eliminated
                    OnRoundEnd();
                }
                else
                {
                    //inform your team mates about the kill
                    foreach (var v in _players.Values)
                    {
                        if (v.TeamID == player.TeamID)
                            SendMethodToPlayer(v.ActorId, FpsGameRPC.PlayerSpectator, player.ActorId);
                    }
                }
            }
            else if (CurrentState == State.WaitForPlayers)
            {
                RespawnPlayerInSeconds(player, 5);
            }
        }

        protected void ResetPlayer(int actorId)
        {
            CharacterInfo info;
            if (_players.TryGetValue(actorId, out info))
            {
                info.ResetState();
            }
        }

        /// <summary>
        /// stop the match
        /// </summary>
        protected override void StopCurrentMatch()
        {
            if (CurrentState == State.RoundRunning)
            {
                _state = State.Stopped;
                _roundRunning = false;

                StopRoundEndTimer();
                if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

                //record the statistics for everyone who was part of the current match at any time
                RecordMatchStatistics();

                //send match results to players
                CallMatchEnd(_players.Values);
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("Prevented StopCurrentRound() call of round number '{0}' from execution because round is not running", _roundCount);
            }
        }

        /// <summary>
        /// Called only after endofmatch screen is over
        /// </summary>
        /// <param name="seconds"></param>
        protected override void StartNextMatchInSeconds(int seconds)
        {
            _blueWins = _redWins = 0;
            _currentRoundCount = 0;

            if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

            _nextRoundStartingTime = SystemTime.Running + seconds * 1000;

            SendMethodToAll(FpsGameRPC.SetEndOfRoundCountdown, seconds);

            //schedule the round restart
            _nextRoundTimer = _room.ExecutionFiber.Schedule(() =>
            {
                RespawnPlayers();
                StartNewMatch();
            }, (long)(seconds * 1000));
        }

        private void StartNewMatchAfterGracetime(int seconds)
        {
            //CmuneDebug.Log("StartNewMatchAfterGracetime " + seconds);

            _state = State.BetweenRounds;

            //load all waiting players, reset & respawn all
            RespawnPlayers();

            // start grace count down for all players
            SendMethodToAll(FpsGameRPC.GraceTimeCountDown, 1, seconds);

            _nextRoundTimer = _room.ExecutionFiber.Schedule(StartNewMatch, seconds * 1000);
        }

        protected override void StartNewMatch()
        {
            //CmuneDebug.Log("StartNewMatch");

            //check if somebody left during the grace time
            if (EnoughPlayersToKeepTheGameRunning())
            {
                UpdateTeamBalance();

                ResetPlayers();

                _blueWins = _redWins = 0;
                _currentRoundCount = 0;

                //reset the statistics to only include the players currently in game
                _statistics.Clear();
                foreach (CharacterInfo c in _players.Values)
                {
                    _statistics.AddPlayer(c);
                }

                StartNewRound();
            }
            else
            {
                UpdateTeamBalance();
                if (HasEnoughPlayers)
                {
                    StartNextMatchInSeconds(3);
                }
                else
                {
                    _state = State.WaitForPlayers;
                    SendMethodToAll(FpsGameRPC.SetWaitingForPlayers);
                }
            }
        }

        private void StartNextRoundInSeconds(int seconds)
        {
            //CmuneDebug.Log("StartNextRoundInSeconds " + seconds);

            //load all waiting players, reset & respawn all
            RespawnPlayers();

            _state = State.BetweenRounds;

            //trigger countdown on client
            SendMethodToAll(FpsGameRPC.GraceTimeCountDown, CurrentRoundCount + 1, seconds);

            //trigger countdown on server
            _room.ExecutionFiber.Schedule(StartNewRound, seconds * 1000);
        }

        private void StartNewRound()
        {
            //check if somebody left during the grace time
            if (EnoughPlayersToKeepTheGameRunning())
            {
                // each round has n seconds (normally a multiply of 60)
                int seconds = _data.RoundTime;

                _roundRunning = true;
                _state = State.RoundRunning;
                _currentRoundCount++;
                _powerupManager.Reset();

                SendMethodToAll(FpsGameRPC.UpdateRoundStats, CurrentRoundCount, _blueWins, _redWins);

                StopRoundEndTimer();

                _roundEndTimer = _room.ScheduleRoomMessage(new RoomMessage(GameRoomMessage.RoundTimeoutEvent, ++_roundCount), seconds * 1000);

                _startTime = SystemTime.Running;
                _endTime = _startTime + (seconds * 1000);

                SendMethodToAll(FpsGameRPC.MatchStart, _roundCount, _endTime);
            }
            else
            {
                UpdateTeamBalance();
                if (HasEnoughPlayers)
                {
                    StartNextRoundInSeconds(3);
                }
                else
                {
                    _state = State.WaitForPlayers;
                    SendMethodToAll(FpsGameRPC.SetWaitingForPlayers);
                }
            }
        }

        private void RespawnPlayers()
        {
            //make everyone readu for next round
            foreach (CharacterInfo i in _players.Values)
            {
                i.ResetState();
                RespawnPlayerInSeconds(i, 0);
            }

            //CmuneDebug.Log("Clear Waiting Players: " + CmunePrint.Values(_waitingPlayers));
            _waitingPlayers.Clear();
        }

        //[NetworkMethod(FpsGameRPC.PlayerTeamChange)]
        protected override void OnPlayerTeamChange(int actorID)
        {

            CharacterInfo actor = null;
            if (_players.TryGetValue(actorID, out actor))
            {
                switch (actor.TeamID)
                {
                    case TeamID.BLUE:
                        actor.TeamID = TeamID.RED;
                        break;

                    case TeamID.RED:
                        actor.TeamID = TeamID.BLUE;
                        break;
                }

                CmuneDebug.Log("OnPlayerTeamChange " + actor.TeamID + " " + CurrentState);

                SendMethodToAll(FpsGameRPC.PlayerTeamChange, actorID, (byte)actor.TeamID);

                if (actor.IsAlive && !_waitingPlayers.Contains(actorID))
                    KillPlayer(actorID);

                UpdateTeamBalance();

                if (CurrentState == State.RoundRunning)
                {
                    if (!EnoughPlayersToKeepTheGameRunning())
                    {
                        OnRoundEnd();
                    }
                }

                if (CurrentState == State.WaitForPlayers && HasEnoughPlayers)
                {
                    UpdateRankingForAll();

                    GetTeamBalance(out _bluePlayers, out _redPlayers);
                    _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), _bluePlayers, _redPlayers);

                    StartNewMatchAfterGracetime(5);
                }
            }
        }

        private void UpdateTeamBalance()
        {
            GetTeamBalance(out _bluePlayers, out _redPlayers);
        }

        private void GetPlayersAlive(out int blue, out int red)
        {
            blue = 0; red = 0;

            foreach (var v in _players.Values)
            {
                if (v.IsAlive && !_waitingPlayers.Contains(v.ActorId))
                {
                    if (v.TeamID == TeamID.RED) red++;
                    else if (v.TeamID == TeamID.BLUE) blue++;
                }
            }
        }

        private TeamID UpdateWinCounter()
        {
            int blue, red;
            GetPlayersAlive(out blue, out red);

            if (blue > 0 && red == 0)
            {
                _blueWins++;
                return TeamID.BLUE;
            }
            else if (blue == 0 && red > 0)
            {
                _redWins++;
                return TeamID.RED;
            }
            else
            {
                return TeamID.NONE;
            }
        }

        private void OnRoundEnd()
        {
            if (CurrentState == State.RoundRunning)
            {
                if (_nextRoundTimer != null) _nextRoundTimer.Dispose();

                TeamID winner = UpdateWinCounter();

                //send winner notifications
                SendMethodToAll(FpsGameRPC.UpdateRoundStats, CurrentRoundCount, _blueWins, _redWins);

                StopRoundEndTimer();

                UpdateTeamBalance();

                // stop match and send statistics to all players
                if (!HasEnoughPlayers || _blueWins == _data.SplatLimit || _redWins == _data.SplatLimit)
                {
                    StopCurrentMatch();

                    StartNextMatchInSeconds(MATCH_WAITING_TIME);
                }
                // only stop current round and start next one in 5 seconds
                else if (HasEnoughPlayers)
                {
                    //only send the WIN message if the match is still going on
                    SendMethodToAll(FpsGameRPC.TeamEliminationRoundEnd, (int)winner);

                    //wait 5 seconds and then show restart counter for next round
                    int seconds = 5;
                    _room.ExecutionFiber.Schedule(() => StartNextRoundInSeconds(seconds), seconds * 1000);

                    _state = State.BetweenRounds;
                }
                else
                {
                    _state = State.WaitForPlayers;
                }
            }
        }

        private void LoadPendingAvatarOfActivePlayers(int actorId)
        {
            List<int> activePlayers = new List<int>(_players.Count);

            foreach (CharacterInfo i in _players.Values)
            {
                if (!_waitingPlayers.Contains(i.ActorId))
                    activePlayers.Add(i.ActorId);
            }

            SendMethodToPlayer(actorId, FpsGameRPC.LoadPendingAvatarOfPlayers, (object)activePlayers);
        }

        //private void LoadPendingAvatars()
        //{
        //    _waitingPlayers.Clear();
        //    SendMethodToAll(FpsGameRPC.LoadPendingAvatars);
        //}

        public bool EnoughPlayersToKeepTheGameRunning()
        {
            int blueCount = 0;
            int redCount = 0;
            foreach (CharacterInfo i in _players.Values)
            {
                if (i.TeamID == TeamID.BLUE && i.IsAlive && !_waitingPlayers.Contains(i.ActorId)) blueCount++;
                else if (i.TeamID == TeamID.RED && i.IsAlive && !_waitingPlayers.Contains(i.ActorId)) redCount++;
            }

            return blueCount > 0 && redCount > 0;
        }

        #region Properties
        public int CurrentRoundCount
        {
            get { return _currentRoundCount; }
        }
        public State CurrentState
        {
            get { return _state; }
        }
        public override bool HasEnoughPlayers
        {
            get
            {
                return _bluePlayers > 0 && _redPlayers > 0;
            }
        }


        #endregion

        #region Fields
        const int MAX_WIN_COUNT = 2;

        private int _currentRoundCount;
        private int _blueWins, _redWins;
        private int _bluePlayers, _redPlayers;

        private State _state = State.WaitForPlayers;
        #endregion

        public enum State
        {
            WaitForPlayers,
            BetweenRounds,
            RoundRunning,
            Stopped
        }
    }
}