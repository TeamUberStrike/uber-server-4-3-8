using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Synchronization;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using Photon.SocketServer;
using UberStrike.Realtime.Common;
using UnityEngine;

namespace UberStrike.Realtime.Photon.GameServer
{
    public abstract class ServerGameMode : ServerNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="room"></param>
        public ServerGameMode(RemoteMethodInterface center, CmuneRoom room)
            : base(center, room)
        {
            _players = new Dictionary<int, CharacterInfo>();
            _peers = new Dictionary<int, CmunePeer>();
            _positionSamples = new Dictionary<int, PositionSample>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(GameRPC.Join)]
        protected virtual void OnJoinMode(CharacterInfo player)
        {
            CmunePeer peer;
            if (_room.TryGetPeer(player.ActorId, out peer))
            {
                if (!_players.ContainsKey(player.ActorId))
                {
                    _peers.Add(player.ActorId, peer);

                    List<Vector3> positions = new List<Vector3>(_players.Count);
                    foreach (var v in _players) positions.Add(v.Value.Position);

                    SendMethodToPlayer(player.ActorId, GameRPC.FullPlayerListUpdate, (object)SyncObjectBuilder.GetSyncData(_players.Values, true), positions);

                    if (player.IsSpectator)
                    {
                        //send join feedback to spectator
                        SendMethodToPlayer(player.ActorId, GameRPC.Join, SyncObjectBuilder.GetSyncData(player, true), player.Position);
                    }
                    else
                    {
                        _players.Add(player.ActorId, player);

                        //send the NEW playerinfo to all existing players
                        SendMethodToAll(GameRPC.Join, SyncObjectBuilder.GetSyncData(player, true), player.Position);
                    }
                }
                else
                {
                    CmuneDebug.LogError("({0}) - Recieved onJoinMode but player already JOINED! {1}", _room.Number, player);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        [NetworkMethod(GameRPC.Leave)]
        protected void OnLeaveMode(int actorID)
        {
            OnPlayerLeftGame(actorID);
        }

        protected override void OnPlayerLeftGame(int actorID)
        {
            //send out the leave information to _all_ members of this mode (inclusive the  leaving client)
            SendMethodToAll(GameRPC.Leave, actorID);

            //remove the client from the list
            _players.Remove(actorID);
            _peers.Remove(actorID);
            _positionSamples.Remove(actorID);

            //lock (Counter.Players)
            //{
            //    Counter.Players.Decrement();
            //}
        }

        [NetworkMethod(GameRPC.ResetPlayer)]
        protected virtual void OnResetPlayer(int actorId)
        {
            CharacterInfo info;
            if (_players.TryGetValue(actorId, out info))
            {
                info.ResetState();
            }
        }

        /// <summary>
        /// Incoming player data update
        /// </summary>
        /// <param name="data"></param>
        [NetworkMethod(GameRPC.PlayerUpdate)]
        protected virtual void OnPlayerUpdate(SyncObject data)
        {
            if (!data.IsEmpty && data.Id > 0)
            {
                CharacterInfo info;
                if (_players.TryGetValue(data.Id, out info))
                {
                    //only update the armor value if the gear changed
                    int mask = PlayerSyncMask;
                    if (data.Contains(CharacterInfo.FieldTag.Gear))
                        mask |= CharacterInfo.FieldTag.Armor;

                    SyncObjectBuilder.ReadSyncData(data, false, mask, info);
                }
            }
        }

        /// <summary>
        /// only these values will synced from server to client
        /// </summary>
        private const int PlayerSyncMask = ~(CharacterInfo.FieldTag.Health | CharacterInfo.FieldTag.Stats | CharacterInfo.FieldTag.TeamID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorID"></param>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public void SendMethodToOthers(int actorID, byte localAddress, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToPeersExceptActor(actorID, _peers.Values, true, NetworkID, localAddress, new SendParameters(), args);
            }
            else
            {
                CmuneDebug.LogWarning("({0}) - Send Message failed because instance was not initialized yet!", _room.Number);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public void SendMethodToAll(byte localAddress, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToPeers(_peers.Values, true, NetworkID, localAddress, new SendParameters(), args);
            }
            else
            {
                CmuneDebug.LogWarning("({0}) - Send Message failed because instance was not initialized yet!", _room.Number);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="args"></param>
        public void SendMethodToAllUnreliable(byte localAddress, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToPeers(_peers.Values, false, NetworkID, localAddress, new SendParameters() { Unreliable = true }, args);
            }
            else
            {
                CmuneDebug.LogWarning("({0}) - Send Message failed because instance was not initialized yet!", _room.Number);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="durationInMinutes"></param>
        [RoomMessage(RoomMessageType.KickPlayer)]
        protected void OnKickPlayer(int cmid, int durationInMinutes)
        {
            //find player ids that match the cmid (should be only 1, in cases of double login it can be more)
            Queue<int> players = new Queue<int>(1);
            foreach (CharacterInfo i in _players.Values)
            {
                if (i.Cmid == cmid)
                    players.Enqueue(i.ActorId);
            }

            //actually kick the player
            while (players.Count > 0)
            {
                int actorId = players.Dequeue();

                SendMethodToPlayer(actorId, FpsGameRPC.KickFromGame, "You were kicked from this game!");
                OnLeaveMode(actorId);
            }
        }

        [RoomMessage(RoomMessageType.CustomMessage)]
        protected void OnCustomMessage(int actorId, string message)
        {
            SendMethodToPlayer(actorId, CommRPC.ModerationCustomMessage, message);
        }

        [RoomMessage(RoomMessageType.MutePlayer)]
        protected void OnMutePlayer(int actorId, bool mute)
        {
            SendMethodToPlayer(actorId, CommRPC.ModerationMutePlayer, mute);
        }

        #region FIELDS

        protected Dictionary<int, CharacterInfo> _players;
        private Dictionary<int, CmunePeer> _peers;
        protected Dictionary<int, PositionSample> _positionSamples;

        #endregion
    }
}
