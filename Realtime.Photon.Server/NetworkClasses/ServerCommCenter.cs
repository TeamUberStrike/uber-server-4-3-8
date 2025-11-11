using System;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using Cmune.ParadisePaintball.DataCenter.Common.Entities;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using Photon.SocketServer;

namespace Cmune.Realtime.Photon.Server
{
    [NetworkClass(NetworkClassID.CommCenter)]
    public class ServerCommCenter : ServerNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        public ServerCommCenter(RemoteMethodInterface center)
            : base(center, CommServerRoom.Instance)
        {
            _actorsByActorId = new Dictionary<int, CommActorInfo>(3000);
            _actorIdByCmid = new Dictionary<int, int>(3000);
            _friendsByActorId = new Dictionary<int, Friends>(3000);

            //for (int i = 1000; i < 3000; i++)
            //{
            //    _actorsByActorId.Add(i, new CommActorInfo("ABCDEFGHI_" + i, i, i, "ABC", ChannelType.WebPortal));
            //    _actorIdByCmid.Add(i, i);
            //}

            _groups = new Dictionary<CmuneRoomID, ChatGroup>(500);
            _mutedCmids = new Dictionary<int, DateTime>(50);
            _bannedActorsByCmids = new Dictionary<int, CommActorInfo>(100);
            _recentActorsChatting = new LimitedQueue<int>(150);

            int oneMinute = 60 * 1000;
            CommServerRoom.Instance.ExecutionFiber.ScheduleOnInterval(UpdateMutedCmids, oneMinute, oneMinute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        [NetworkMethod(CommRPC.Join)]
        protected void OnJoin(CommActorInfo actor)
        {
            //CmuneDebug.LogErrorFormat("onJoinMode: " + player.ToString());

            if (actor.Cmid != 0 && _bannedActorsByCmids.ContainsKey(actor.Cmid))
            {
                //notify the player that he is beeing disconnected
                SendMethodToPlayer(actor.ActorId, CommRPC.ModerationCustomMessage, "You're currently banned. Please contact support@cmune.com for any questions.");

                //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                SendMethodToPlayer(actor.ActorId, CommRPC.DisconnectAndDisablePhoton);
            }
            else if (actor.ActorId > 0 && !_actorsByActorId.ContainsKey(actor.ActorId))
            {
                _actorsByActorId.Add(actor.ActorId, actor);

                //disconnect the old player
                int oldActorId;
                if (_actorIdByCmid.TryGetValue(actor.Cmid, out oldActorId))
                {
                    //message for default web player build
                    string msg = "You have been logged out because someone else has logged in with your account.";

                    //message for widget & standalone
                    CommActorInfo oldActor;
                    if (_actorsByActorId.TryGetValue(oldActorId, out oldActor) && oldActor != null)
                    {
                        switch (oldActor.Channel)
                        {
                            case ChannelType.WindowsStandalone:
                            case ChannelType.OSXDashboard:
                            case ChannelType.MacAppStore:
                            case ChannelType.OSXStandalone:
                                msg += " Please quit and restart the application to log in again.";
                                break;
                            case ChannelType.WebFacebook:
                            case ChannelType.WebMySpace:
                            case ChannelType.WebPortal:
                                msg += " Please refresh your page to log in again.";
                                break;
                            default:
                                break;
                        }
                    }

                    //notify the player that he is beeing disconnected
                    SendMethodToPlayer(oldActorId, CommRPC.ModerationCustomMessage, msg);

                    //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                    SendMethodToPlayer(oldActorId, CommRPC.DisconnectAndDisablePhoton);

                    //remove all traces of the old peer to make space for the current connection (important to avoid Cmid collisions)
                    OnPlayerLeftGame(oldActorId);
                }

                _actorIdByCmid[actor.Cmid] = actor.ActorId;

                SendFullPlayerListUpdate(actor);

                if (IsPlayerMuted(actor.Cmid))
                {
                    actor.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Muted;
                    SendMethodToPlayer(actor.ActorId, CommRPC.ModerationMutePlayer, true);
                }
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarningFormat("Recieved OnJoin but Actor '{0}' already joined or Null! Current actor count: {1}", actor.ActorId, _actorsByActorId.Count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.FullPlayerListUpdate)]
        protected void OnFullPlayerListUpdate(int actorId)
        {
            CommActorInfo actor;
            if (_actorsByActorId.TryGetValue(actorId, out actor))
            {
                SendFullPlayerListUpdate(actor);
            }
        }

        protected void SendFullPlayerListUpdate(CommActorInfo actor)
        {
            List<CommActorInfo> chatters = new List<CommActorInfo>(_recentActorsChatting.Count + 1);
            chatters.Add(actor);
            for (int i = 0; i < _recentActorsChatting.Count; i++)
            {
                CommActorInfo c;
                if (_actorsByActorId.TryGetValue(_recentActorsChatting[i], out c))
                    chatters.Add(c);
            }

            Friends friends;
            if (_friendsByActorId.TryGetValue(actor.ActorId, out friends) && friends != null)
            {
                CommActorInfo info;
                //int aid;
                foreach (int cmid in friends.Versions.Values)
                {
                    //find the CommActorInfo in the current list of live Actors
                    if (TryGetActorByCmid(cmid, out info))
                    {
                        friends.Versions[cmid] = info.VersionID;
                        chatters.Add(info);
                    }
                }
            }

            SendMethodToPlayer(actor.ActorId, CommRPC.FullPlayerListUpdate, CmuneDeltaSync.GetSyncData(chatters, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.Leave)]
        protected void OnLeave(int actorId)
        {
            OnPlayerLeftGame(actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        protected override void OnPlayerLeftGame(int actorId)
        {
            CommActorInfo info;

            //remove the peer from all lists
            if (_actorsByActorId.TryGetValue(actorId, out info))
            {
                if (info != null) _actorIdByCmid.Remove(info.Cmid);
            }

            _actorsByActorId.Remove(actorId);

            //update chat groups
            RemoveUserFromAllChatGroups(actorId);

            //send out the leave information to _all_ members of this mode (inclusive the  leaving client)
            if (_recentActorsChatting.Remove(actorId))
                SendMethodToAll(CommRPC.Leave, new SendParameters() { Unreliable = true }, actorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.UpdatePlayerRoom)]
        protected void OnUpdatePlayerRoom(int actorId, CmuneRoomID room)
        {
            CommActorInfo info = null;
            if (actorId > 0 && _actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                //put user in a specific chat group
                MoveUserToChatGroup(info.ActorId, info.CurrentRoom, room);

                info.CurrentRoom = room;
                info.IncrementVersion();

                //send the UPDATED playerinfo to all existing players
                if (_recentActorsChatting.Contains(actorId))
                    SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, info.GetSyncData(false));
            }
            else
            {
                CmuneDebug.LogErrorFormat("OnUpdatePlayerRoom failed with actorID '{0}', CommActorInfo exists: {1}", actorId, (info != null));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.ResetPlayerRoom)]
        protected void OnResetPlayerRoom(int actorId)
        {
            CommActorInfo info = null;
            if (actorId > 0 && _actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                //remove user from the chat group
                RemoveUserFromChatGroup(info.ActorId, info.CurrentRoom);

                //reset room data to default
                info.CurrentRoom = _room.RoomData.RoomID;
                info.IncrementVersion();

                //send the UPDATED playerinfo to all existing players
                if (_recentActorsChatting.Contains(actorId))
                    SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, info.GetSyncData(false));
            }
            else
            {
                CmuneDebug.LogWarningFormat("OnResetPlayerRoom failed with actorID '{0}', CommActorInfo exists: {1}", actorId, (info != null));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.PlayerUpdate)]
        protected void OnPlayerUpdate(SyncObject data)
        {
            if (!data.IsEmpty && data.Id > 0)
            {
                CommActorInfo info;
                if (_actorsByActorId.TryGetValue(data.Id, out info))
                    info.ReadSyncData(data);

                //send the UPDATED playerinfo to all existing players);
                if (_recentActorsChatting.Contains(data.Id))
                    SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.UpdateInboxMessages)]
        protected void OnPlayerUpdate(int cmid, int messageId)
        {
            int actorId;
            if (_actorIdByCmid.TryGetValue(cmid, out actorId))
            {
                SendMethodToPlayer(actorId, CommRPC.UpdateInboxMessages, messageId);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.UpdateClanMembers)]
        protected void OnPlayerUpdate(List<int> clanMembers)
        {
            int actorId;
            foreach (int cmid in clanMembers)
            {
                if (_actorIdByCmid.TryGetValue(cmid, out actorId))
                {
                    SendMethodToPlayer(actorId, CommRPC.UpdateClanMembers);
                }
            }
        }

        [NetworkMethod(CommRPC.SendPlayerNameSearchString)]
        protected void OnSendPlayerNameSearchString(int actorId, string search)
        {
            List<CommActorInfo> result = new List<CommActorInfo>(_actorsByActorId.Count);

            /* Get search result */
            foreach (CommActorInfo c in _actorsByActorId.Values)
            {
                if (c.PlayerName.Contains(search))
                {
                    result.Add(c);
                }
            }

            /* Send result back to player */
            List<CommActorInfo> buf = new List<CommActorInfo>(100);
            List<CommActorInfo>.Enumerator itor = result.GetEnumerator();

            int i = 0;
            while (itor.MoveNext())
            {
                buf.Add(itor.Current);

                if (++i == 100)
                {
                    i = 0;
                    SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, CmuneDeltaSync.GetSyncData<CommActorInfo>(buf, true));
                    buf.Clear();
                }
            }

            if (buf.Count > 0)
                SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, CmuneDeltaSync.GetSyncData<CommActorInfo>(buf, true));
        }

        #region Chat

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.ChatMessageInGame)]
        protected void OnChatMessageInGame(int actorId, string message)
        {
            CommActorInfo user;
            if (_actorsByActorId.TryGetValue(actorId, out user) && !IsPlayerMuted(user.Cmid))
            {
                //UpdateListOfActiveChatters(user);

                //if (user.AccessLevel == 0)
                //    CrispWordFilter.FilterMessage(user.Cmid.ToString(), user.CurrentRoom.ToString(), message, false, (cleanMessage) => SendInGameChatMessageFromPlayer(user, cleanMessage));
                //else
                SendInGameChatMessageFromPlayer(user, message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.ChatMessageToAll)]
        protected void OnChatMessageToAll(int actorId, string message)
        {
            CommActorInfo user;
            if (_actorsByActorId.TryGetValue(actorId, out user) && !IsPlayerMuted(user.Cmid))
            {
                UpdateListOfActiveChatters(user);

                //if (user.AccessLevel == 0)
                //    CrispWordFilter.FilterMessage(user.Cmid.ToString(), "Lobby", message, false, (cleanMessage) => SendMethodToOthers(actorId, CommRPC.ChatMessageToAll, user.Cmid, user.ActorId, user.PlayerName, cleanMessage));
                //else
                SendMethodToOthers(actorId, CommRPC.ChatMessageToAll, new SendParameters() { Unreliable = true }, user.Cmid, user.ActorId, user.PlayerName, message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.ChatMessageToPlayer)]
        protected void OnChatMessageToPlayer(int senderID, int recieverID, string message)
        {
            CommActorInfo reciever, sender;
            if (_actorsByActorId.TryGetValue(senderID, out sender) && !IsPlayerMuted(sender.Cmid) && _actorsByActorId.TryGetValue(recieverID, out reciever))
            {
                UpdateListOfActiveChatters(sender);

                //if (sender.AccessLevel == 0)
                //    CrispWordFilter.FilterMessage(sender.Cmid.ToString(), reciever.Cmid.ToString(), message, true, (cleanMessage) => SendMethodToPlayer(reciever.ActorId, CommRPC.ChatMessageToPlayer, sender.Cmid, sender.ActorId, sender.PlayerName, cleanMessage));
                //else
                SendMethodToPlayer(reciever.ActorId, CommRPC.ChatMessageToPlayer, new SendParameters() { Unreliable = true }, sender.Cmid, sender.ActorId, sender.PlayerName, message);
            }
        }

        private void UpdateListOfActiveChatters(CommActorInfo actor)
        {
            if (_recentActorsChatting.EnqueueUnique(actor.ActorId))
            {
                SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, actor.GetSyncData(true));

                if (_recentActorsChatting.LastItem > 0)
                    SendMethodToAll(CommRPC.HidePlayer, new SendParameters() { Unreliable = true }, _recentActorsChatting.LastItem);
            }
        }

        private void MoveUserToChatGroup(int actorId, CmuneRoomID oldRoom, CmuneRoomID newRoom)
        {
            int i = 0;
            try
            {
                ChatGroup g;
                //first remove the user from the old chat group
                if (_groups.TryGetValue(oldRoom, out g))
                {
                    g.RemoveUser(actorId);
                }
                //add him to the new chat group
                if (!_groups.TryGetValue(newRoom, out g))
                {
                    g = new ChatGroup(actorId);
                    _groups[newRoom] = g;
                }

                g.AddUser(actorId);
            }
            catch (Exception e)
            {
                CmuneDebug.Exception("{0} {1} with Message {2} at {3}", i, e.GetType(), e.Message, e.StackTrace);
            }
        }

        private void RemoveUserFromChatGroup(int actorId, CmuneRoomID oldRoom)
        {
            ChatGroup g;
            //first remove the user from the old chat group
            if (_groups.TryGetValue(oldRoom, out g))
            {
                g.RemoveUser(actorId);

                if (g.IsEmpty)
                {
                    _groups.Remove(oldRoom);
                }
            }
        }

        private void RemoveUserFromAllChatGroups(int actorId)
        {
            Queue<CmuneRoomID> _emptyRooms = new Queue<CmuneRoomID>(1);

            //remove the user from the old chat group
            foreach (KeyValuePair<CmuneRoomID, ChatGroup> g in _groups)
            {
                g.Value.RemoveUser(actorId);

                //enqueue room for deletion if room is empty
                if (g.Value.IsEmpty)
                {
                    _emptyRooms.Enqueue(g.Key);
                }
            }

            while (_emptyRooms.Count > 0)
                _groups.Remove(_emptyRooms.Dequeue());
        }

        private void SendInGameChatMessageFromPlayer(CommActorInfo user, string msg)
        {
            ChatGroup g;
            if (_groups.TryGetValue(user.CurrentRoom, out g))
            {
                g.SendMessageToGroup(user.Cmid, user.ActorId, user.PlayerName, msg);
            }
        }

        #endregion

        #region Moderator Actions

        public string GetIpAddressOfCmid(int cmid)
        {
            string ipAddress = "127.0.0.1";

            CommActorInfo actor;
            CmunePeer peer;
            if (TryGetActorByCmid(cmid, out actor) && CommServerRoom.Instance != null && CommServerRoom.Instance.TryGetPeer(actor.ActorId, out peer))
            {
                if (peer != null)
                    ipAddress = peer.IpAddress;
            }

            return ipAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="muteTimeMinutes"></param>
        /// <param name="actorID"></param>
        /// <param name="disableChat"></param>
        [NetworkMethod(CommRPC.ModerationMutePlayer)]
        protected void OnModerationMutePlayer(int cmid, int durationInMinutes, int actorID, bool disableChat)
        {
            SendMethodToPlayer(actorID, CommRPC.ModerationMutePlayer, disableChat);

            CommActorInfo actor;
            if (TryGetActorByCmid(cmid, out actor) && actor != null)
            {
                actor.ModerationFlag |= disableChat ? (byte)CommActorInfo.ModerationTag.Muted : (byte)CommActorInfo.ModerationTag.Ghosted;
            }

            //perform the longterm ban
            if (durationInMinutes > 0)
            {
                _mutedCmids[cmid] = DateTime.Now.AddMinutes(durationInMinutes);
            }
            else
            {
                _mutedCmids.Remove(cmid);
            }
        }

        [NetworkMethod(CommRPC.ModerationPermanentBan)]
        protected void OnModerationPermanentBan(int modCmid, int playerCmid)
        {
            //perma ban
            WebserviceCaller.Instance.ExecutionFiber.Enqueue(() =>
            {
                try
                {
                    Uberstrike.WebService.DotNet.ModerationWebServiceClient.BanPermanently(modCmid, playerCmid, ParadisePaintballCommonConfig.ApplicationId, GetIpAddressOfCmid(modCmid));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogErrorFormat("{0} Exception when calling WS BanPermanently with: {1}", e.GetType(), e.Message);
                }
            });

            CommActorInfo info;
            if (TryGetActorByCmid(playerCmid, out info))
            {
                //notify the player that he is beeing disconnected
                SendMethodToPlayer(info.ActorId, CommRPC.ModerationCustomMessage, "You are permanently banned! Please contact support@cmune.com for any questions.");

                //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                SendMethodToPlayer(info.ActorId, CommRPC.DisconnectAndDisablePhoton);

                MailNotification.SendReport(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
            }
            else
            {
                MailNotification.SendReport(modCmid, "Admin" + modCmid, string.Format("Player {0} currently not online", playerCmid), "Permanent Ban", string.Format("Ban performed by Admin with Cmid {0}", modCmid));
            }
        }

        [NetworkMethod(CommRPC.ModerationBanPlayer)]
        protected void OnModerationBanPlayer(int actorId, int modCmid)
        {
            CommActorInfo info;
            if (_actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Banned;
                info.IncrementVersion();

                _bannedActorsByCmids.Add(info.Cmid, info);

                //notify the player that he is beeing disconnected
                SendMethodToPlayer(actorId, CommRPC.ModerationCustomMessage, "You are banned! Please contact support@cmune.com for any questions.");

                //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                SendMethodToPlayer(actorId, CommRPC.DisconnectAndDisablePhoton);

                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Moderation Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
            }
        }

        [NetworkMethod(CommRPC.ModerationKickGame)]
        protected void OnModerationKickGame(int actorId, int modCmid)
        {
            CommActorInfo info;
            if (_actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Kick Out Of Game", string.Format("Kicked out of game by Admin with Cmid {0}", modCmid));
            }
        }

        [NetworkMethod(CommRPC.ModerationUnbanPlayer)]
        protected void OnModerationUnbanPlayer(int cmid)
        {
            _bannedActorsByCmids.Remove(cmid);
        }

        [NetworkMethod(CommRPC.SpeedhackDetection)]
        protected void OnSpeedhackDetection(int actorId, float probability, string debug)
        {
            CommActorInfo info;
            if (_actorsByActorId.TryGetValue(actorId, out info))
            {
                //only send mail on first detection
                if ((info.ModerationFlag & (byte)CommActorInfo.ModerationTag.Speedhacking) == 0)
                    MailNotification.SendPlayerReport(0, string.Empty, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Speedhack Autodetection", string.Empty, debug);

                info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Speedhacking;
                info.ModInformation = string.Format("{0:N1}%", probability);
                info.IncrementVersion();
            }
        }

        [NetworkMethod(CommRPC.SpeedhackDetectionNew)]
        protected void OnSpeedhackDetection(int actorId, float mean, float variance, float probability, string debug)
        {
            CommActorInfo info;
            if (_actorsByActorId.TryGetValue(actorId, out info))
            {
                //only send mail on first detection
                if ((info.ModerationFlag & (byte)CommActorInfo.ModerationTag.Speedhacking) == 0)
                    MailNotification.SendReport(0, string.Empty, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Speedhack Autodetection", debug);

                if (mean > 1.6f)
                {
                    //notify the player that he is beeing disconnected
                    SendMethodToPlayer(actorId, CommRPC.ModerationCustomMessage, "Sorry, you have to reload your page.\nIf the problem persists, please contact support@cmune.com");

                    //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                    SendMethodToPlayer(actorId, CommRPC.DisconnectAndDisablePhoton);
                }
                else
                {
                    info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Speedhacking;
                    info.ModInformation = string.Format("{0:N1}x, var: {1:N1} => {2:N1}%", mean, variance, probability);
                    info.IncrementVersion();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reporterCmid"></param>
        /// <param name="cmids"></param>
        /// <param name="type"></param>
        /// <param name="details"></param>
        /// <param name="logs"></param>
        [NetworkMethod(CommRPC.ReportPlayers)]
        protected void OnPlayersReported(int reporterCmid, List<int> cmids, int type, string details, string logs)
        {
            string reportername = string.Format("{0} - ", reporterCmid);

            CommActorInfo info;
            if (TryGetActorByCmid(reporterCmid, out info))
            {
                reportername += info.PlayerName;
            }

            foreach (int cmid in cmids)
            {
                string abusername = string.Format("{0} - ", cmid);

                if (TryGetActorByCmid(cmid, out info))
                {
                    switch (type)
                    {
                        case (int)MemberReportType.OffensiveChat: info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Language; break;
                        case (int)MemberReportType.OffensiveName: info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Name; break;
                        case (int)MemberReportType.Spamming: info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Spamming; break;
                    }
                }

                MailNotification.SendPlayerReport(reporterCmid, reportername, abusername, ((MemberReportType)type).ToString(), details, logs);
            }
        }

        #endregion

        #region Actor List Updates

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="friendsCmids"></param>
        [NetworkMethod(CommRPC.SendFriendsList)]
        protected void OnSendFriendsList(int actorId, List<int> friendsCmids)
        {
            Friends friends;
            if (_friendsByActorId.TryGetValue(actorId, out friends))
            {
                friends.UpdateFriends(friendsCmids);
            }
            else
            {
                _friendsByActorId[actorId] = new Friends(friendsCmids);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.UpdateActorsForModeration)]
        protected void OnUpdateUsersForModeration(int actorId)
        {
            Dictionary<int, SyncObject> actors = new Dictionary<int, SyncObject>();

            //all online players
            foreach (CommActorInfo i in _actorsByActorId.Values)
            {
                if (i.ModerationFlag != 0)
                    actors.Add(i.Cmid, i.GetSyncData(true));
            }
            //all banned players
            foreach (CommActorInfo i in _bannedActorsByCmids.Values)
            {
                if (!actors.ContainsKey(i.Cmid))
                    actors.Add(i.Cmid, i.GetSyncData(true));
            }

            SendMethodToPlayer(actorId, CommRPC.UpdateActorsForModeration, actors.Values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.ClearModeratorFlags)]
        protected void OnClearModeratorFlags(int cmid)
        {
            bool isMuted = _mutedCmids.Remove(cmid);

            CommActorInfo actor;
            if (TryGetActorByCmid(cmid, out actor))
            {
                actor.ModerationFlag = 0;
                actor.IncrementVersion();

                if (isMuted)
                    SendMethodToPlayer(actor.ActorId, CommRPC.ModerationMutePlayer, false);
            }

            if (_bannedActorsByCmids.TryGetValue(cmid, out actor) && actor != null)
            {
                actor.ModerationFlag = 0;
                _bannedActorsByCmids.Remove(cmid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.UpdateAllActors)]
        protected void OnUpdateAllActors(int actorId)
        {
            int counter = 0;
            List<SyncObject> actors = new List<SyncObject>(100);
            IEnumerator<CommActorInfo> iter = _actorsByActorId.Values.GetEnumerator();

            while (iter.MoveNext())
            {
                actors.Add(iter.Current.GetSyncData(true));

                if (++counter == 100)
                {
                    SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, actors);
                    actors.Clear();
                    counter = 0;
                }
            }

            // for leftovers
            if (actors.Count > 0)
            {
                SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, actors);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.UpdateFriendsState)]
        protected void OnUpdateFriendsState(int actorId)
        {
            Friends friends;
            if (_friendsByActorId.TryGetValue(actorId, out friends))
            {
                List<CommActorInfo> updated = new List<CommActorInfo>();
                List<int> removed = new List<int>(friends.Removals);

                CommActorInfo info;
                //int aid;
                int[] cmids = Conversion.ToArray<int>(friends.Versions.Keys);
                foreach (int cmid in cmids)
                {
                    //find the CommActorInfo in the current list of live Actors
                    if (TryGetActorByCmid(cmid, out info))
                    {
                        if (info.VersionID != friends.Versions[cmid])
                        {
                            friends.Versions[cmid] = info.VersionID;
                            updated.Add(info);
                        }
                    }
                    //if friend is not online anymore but his version was not zero (means that the client has him in his active list)
                    else if (friends.Versions[cmid] > 0)
                    {
                        friends.Versions[cmid] = 0;
                        removed.Add(cmid);
                    }
                }

                //clear the "not friends anymore" list, already added to the removed list
                friends.Removals.Clear();

                //send result to player
                if (updated.Count > 0 || removed.Count > 0)
                {
                    SendMethodToPlayer(actorId, CommRPC.UpdateFriendsState, CmuneDeltaSync.GetSyncData(updated, true), removed);
                }
            }
        }

        #endregion

        private void UpdateMutedCmids()
        {
            List<int> cmids = new List<int>(_mutedCmids.Keys);
            DateTime now = DateTime.Now;
            foreach (int cmid in cmids)
            {
                DateTime time;
                if (_mutedCmids.TryGetValue(cmid, out time) && time.CompareTo(now) <= 0)
                {
                    _mutedCmids.Remove(cmid);

                    int actorId;
                    if (_actorIdByCmid.TryGetValue(cmid, out actorId))
                        SendMethodToPlayer(actorId, CommRPC.ModerationMutePlayer, false);
                }
            }
        }

        public bool IsPlayerMuted(int cmid)
        {
            DateTime time;
            return (_mutedCmids.TryGetValue(cmid, out time) && time.CompareTo(DateTime.Now) > 0);
        }

        private bool TryGetActorByCmid(int cmid, out CommActorInfo info)
        {
            info = null;
            int actorId;
            return _actorIdByCmid.TryGetValue(cmid, out actorId) && _actorsByActorId.TryGetValue(actorId, out info) && info != null;
        }

        #region SENDING METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="parameter"></param>
        public void SendMethodToOthers(int playerID, byte localAddress, SendParameters sendParameters, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToAllExceptActor(playerID, true, NetworkID, localAddress, sendParameters, args);
            }
            else
            {
                CmuneDebug.LogError(string.Format("Send Message failed because instance was not initialized yet!"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localAddress"></param>
        /// <param name="parameter"></param>
        public void SendMethodToAll(byte localAddress, SendParameters sendParameters, params object[] args)
        {
            if (IsInitialized)
            {
                _room.NewMessageToAll(true, NetworkID, localAddress, sendParameters, args);
            }
            else
            {
                CmuneDebug.LogError(string.Format("Send Message failed because instance was not initialized yet!"));
            }
        }

        #endregion

        #region FIELDS

        private Dictionary<int, DateTime> _mutedCmids;
        private Dictionary<int, CommActorInfo> _bannedActorsByCmids;

        private LimitedQueue<int> _recentActorsChatting;
        private Dictionary<int, CommActorInfo> _actorsByActorId;
        private Dictionary<int, int> _actorIdByCmid;
        private Dictionary<int, Friends> _friendsByActorId;
        private Dictionary<CmuneRoomID, ChatGroup> _groups;

        #endregion

        private class Friends
        {
            public Queue<int> Removals;
            public Dictionary<int, uint> Versions;

            public Friends(List<int> cmids)
            {
                Removals = new Queue<int>(0);
                Versions = new Dictionary<int, uint>(cmids.Count);

                foreach (int i in cmids)
                {
                    Versions[i] = 0;
                }
            }

            public void UpdateFriends(List<int> friends)
            {
                //add friends to the removal list that are not our friends anymore
                //covers the case of update friend list after friend removal
                foreach (int cmid in Versions.Keys)
                {
                    if (!friends.Contains(cmid)) Removals.Enqueue(cmid);
                }

                //add missing friend to the list
                foreach (int cmid in friends)
                {
                    if (!Versions.ContainsKey(cmid)) Versions[cmid] = 0;
                }

                //fill up the friends to remove, for the next UpdateFriendsState call
                foreach (int i in Removals)
                {
                    Versions.Remove(i);
                }
            }
        }

        protected class ChatGroup
        {
            public ChatGroup(int actorId)
            {
                _peersByActorId = new Dictionary<int, CmunePeer>(16);

                AddUser(actorId);
            }

            public void AddUser(int actorId)
            {
                CmunePeer p;
                if (CommServerRoom.Instance.TryGetPeer(actorId, out p))
                {
                    _peersByActorId[actorId] = p;

                    CommServerRoom.Instance.NewMessageToPeers(_peersByActorId.Values, true, NetworkClassID.CommCenter, CommRPC.UpdateIngameGroup, new SendParameters() { Unreliable = true }, (object)_peersByActorId.Keys);
                }
                else
                {
                    CmuneDebug.LogErrorFormat("ChatGroup.AddUser({0}) failed because peer not found!", actorId);
                }
            }

            public void RemoveUser(int actorId)
            {
                _peersByActorId.Remove(actorId);

                CommServerRoom.Instance.NewMessageToPeers(_peersByActorId.Values, true, NetworkClassID.CommCenter, CommRPC.UpdateIngameGroup, new SendParameters() { Unreliable = true }, (object)_peersByActorId.Keys);
            }

            public void SendMessageToGroup(string msg)
            {
                CommServerRoom.Instance.NewMessageToPeers(_peersByActorId.Values, true, NetworkClassID.CommCenter, CommRPC.ChatMessageInGame, new SendParameters() { Unreliable = true }, msg);
            }

            public void SendMessageToGroup(int cmid, int actorId, string name, string msg)
            {
                try
                {
                    CommServerRoom.Instance.NewMessageToPeersExceptActor(actorId, _peersByActorId.Values, true, NetworkClassID.CommCenter, CommRPC.ChatMessageInGame, new SendParameters() { Unreliable = true }, cmid, actorId, name, msg);
                }
                catch (InvalidOperationException e)
                {
                    CmuneDebug.LogErrorFormat("InvalidOperationException at SendMessageToGroup where group length '{0}': {1}", _peersByActorId.Count, e.Message);
                }
            }

            public bool IsEmpty
            {
                get { return _peersByActorId.Count == 0; }
            }

            private Dictionary<int, CmunePeer> _peersByActorId;
        }
    }
}
