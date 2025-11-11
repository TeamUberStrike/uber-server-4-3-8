using System;
using System.Collections.Generic;
using UberStrike.Realtime.Common;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Photon.Server;
using Photon.SocketServer;

namespace UberStrike.Realtime.Photon.GameServer
{
    [NetworkClass(GameModeID.TeamDeathMatch)]
    public class TeamDeathMatchGameMode : FpsGameMode
    {
        public TeamDeathMatchGameMode(RemoteMethodInterface rmi, CmuneRoom room)
            : base(rmi, room)
        {
        }

        protected override bool IsHitPositive(CharacterInfo a, CharacterInfo b)
        {
            return a.ActorId != b.ActorId && a.TeamID != b.TeamID;
        }

        protected override bool IsHitAllowed(CharacterInfo a, CharacterInfo b)
        {
            return a.IsAlive && b.IsAlive && (a.TeamID != b.TeamID || a.ActorId == b.ActorId);
        }

        protected bool IsTeamUnbalanced(int blue, int red)
        {
            bool unbalanced = Math.Abs(blue - red) > 1;

            // disable team balancing for now
            unbalanced = false;

            return unbalanced;
        }

        protected override bool IsSplatLimitReached
        {
            get { return _blueSplats >= _data.SplatLimit || _redSplats >= _data.SplatLimit; }
        }

        protected override void UpdateSplatCount(CharacterInfo i)
        {
            if (i.TeamID == TeamID.BLUE)
                _blueSplats++;
            else if (i.TeamID == TeamID.RED)
                _redSplats++;
        }

        protected bool GetTeamBalance(out int blueCount, out int redCount)
        {
            blueCount = 0;
            redCount = 0;
            foreach (CharacterInfo i in _players.Values)
            {
                if (i.TeamID == TeamID.BLUE) blueCount++;
                else if (i.TeamID == TeamID.RED) redCount++;
            }

            return IsTeamUnbalanced(blueCount, redCount);
        }


        protected override void UpdateRankingForAll()
        {
            UpdatePlayerRanks();

            foreach (var p in _players)
            {
                SendMethodToPlayer(p.Key, FpsGameRPC.UpdateSplatCount, _blueSplats, _redSplats, p.Value.TeamID == TeamID.RED ? _redSplats > _blueSplats : _blueSplats > _redSplats);
            }
        }

        protected override void StartNewMatch()
        {
            _blueSplats = _redSplats = 0;

            int blue, red;
            if (GetTeamBalance(out blue, out red))
            {
                //first do autobalance
                foreach (CharacterInfo c in _players.Values)
                {
                    if (IsTeamUnbalanced(blue, red))
                    {
                        if (blue > red && c.TeamID == TeamID.BLUE)
                        {
                            c.TeamID = TeamID.RED;
                            blue--;
                        }
                        else if (red > blue && c.TeamID == TeamID.RED)
                        {
                            c.TeamID = TeamID.BLUE;
                            red--;
                        }
                    }
                }
            }

            //now restart round
            base.StartNewMatch();
        }

        protected override void OnPlayerSplatted(CharacterInfo player, bool suicide)
        {
            //only check splat limit if target died
            if (IsSplatLimitReached)
            {
                StopCurrentMatch();
            }
            else
            {
                int blueCount, redCount;

                if (GetTeamBalance(out blueCount, out redCount))
                {
                    if (player.TeamID == TeamID.RED && redCount > blueCount)
                    {
                        player.TeamID = TeamID.BLUE;
                        _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), blueCount + 1, redCount - 1);
                    }
                    else if (player.TeamID == TeamID.BLUE && blueCount > redCount)
                    {
                        player.TeamID = TeamID.RED;
                        _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), blueCount - 1, redCount + 1);
                    }
                }

                //set respawn position for splatted player
                RespawnPlayerInSeconds(player, suicide ? 8 : 5);
            }
        }

        protected override void OnPlayerEnter(int actorID)
        {
            int blueCount, redCount;
            GetTeamBalance(out blueCount, out redCount);

            SendMethodToPlayer(actorID, FpsGameRPC.TeamBalanceUpdate, blueCount, redCount);
        }

        protected override void OnNormalJoin(CharacterInfo player)
        {
            base.OnNormalJoin(player);

            int blueCount, redCount;
            GetTeamBalance(out blueCount, out redCount);
            _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), blueCount, redCount);
        }

        protected override void OnPlayerLeftGame(int actorID)
        {
            base.OnPlayerLeftGame(actorID);

            int blueCount, redCount;
            GetTeamBalance(out blueCount, out redCount);
            _room.NewMessageToAll(true, NetworkID, FpsGameRPC.TeamBalanceUpdate, new SendParameters(), blueCount, redCount);
        }

        [NetworkMethod(FpsGameRPC.PlayerTeamChange)]
        protected virtual void OnPlayerTeamChange(int actorID)
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

                actor.Health = 0;
                SendMethodToAll(FpsGameRPC.PlayerTeamChange, actorID, (byte)actor.TeamID);
                RespawnPlayerInSeconds(actor, 3);
            }
        }

        #region Fields

        private int _blueSplats, _redSplats;

        #endregion
    }
}
