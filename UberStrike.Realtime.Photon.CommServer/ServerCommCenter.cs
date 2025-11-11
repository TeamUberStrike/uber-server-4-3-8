using System;
using System.Collections.Generic;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Synchronization;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using Photon.SocketServer;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Realtime.CommServer;
using UberStrike.Realtime.Photon.CommServer;
using UberStrike.WebService.DotNet;
using UberStrike.Realtime.Server;

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
            _contactsByCmid = new Dictionary<int, ContactList>(3000);

            //for (int i = 1000; i < 3000; i++)
            //{
            //    _actorsByActorId.Add(i, new CommActorInfo("ABCDEFGHI_" + i, i, i, "ABC", ChannelType.WebPortal));
            //    _actorIdByCmid.Add(i, i);
            //}

            _groups = new Dictionary<CmuneRoomID, ChatGroup>(500);
            _mutedCmids = new Dictionary<int, DateTime>(50);
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
            if (CmuneDebug.IsDebugEnabled)
                CmuneDebug.Log("OnJoin: " + actor.ActorId + "/" + actor.Cmid + ", current playerList: " + _actorIdByCmid.Count + "==" + _actorsByActorId.Count + " and contactList: " + _contactsByCmid.Count);

            //CmuneDebug.LogErrorFormat("onJoinMode: " + player.ToString());
            if (actor.ActorId > 0 && !_actorsByActorId.ContainsKey(actor.ActorId))
            {
                _actorsByActorId.Add(actor.ActorId, actor);

                // Disconnect the old player if they are already connected
                int oldActorId;
                if (_actorIdByCmid.TryGetValue(actor.Cmid, out oldActorId))
                {
                    // Send a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                    SendMethodToPlayer(oldActorId, CommRPC.DisconnectAndDisablePhoton, "You have been logged out because someone else has logged in with your account.");

                    // Remove all traces of the old peer to make space for the current connection (important to avoid Cmid collisions)
                    OnPlayerLeftGame(oldActorId);
                }

                _actorIdByCmid[actor.Cmid] = actor.ActorId;

                ContactList contacts;
                if (_contactsByCmid.TryGetValue(actor.Cmid, out contacts))
                {
                   // CmuneDebug.LogError("Actor " + actor.Cmid + " joined and we found an old contacts list: " + CmunePrint.Dictionary(contacts.ContactIds));
                }

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
                    CmuneDebug.LogWarning("Recieved OnJoin but Actor '{0}' already joined or Null! Current actor count: {1}", actor.ActorId, _actorsByActorId.Count);
            }
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
            int cmid = 0;

            //remove the peer from all lists
            if (_actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                cmid = info.Cmid;

                _actorIdByCmid.Remove(cmid);
                _contactsByCmid.Remove(cmid);
            }

            _actorsByActorId.Remove(actorId);

            //update chat groups
            RemoveUserFromAllChatGroups(actorId);

            //send out the leave information to _all_ members of this mode (inclusive the  leaving client)
            if (_recentActorsChatting.Remove(actorId) && cmid > 0)
            {
                SendMethodToAll(CommRPC.Leave, new SendParameters() { Unreliable = true }, cmid);
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

            ContactList contacts;
            if (_contactsByCmid.TryGetValue(actor.Cmid, out contacts) && contacts != null)
            {
                CommActorInfo info;
                //int aid;
                foreach (int cmid in contacts.ContactIds)
                {
                    //find the CommActorInfo in the current list of live Actors
                    if (TryGetActorByCmid(cmid, out info))
                    {
                        //contacts.ContactIds[cmid] = info.VersionID;
                        chatters.Add(info);
                    }
                }
            }

            SendMethodToPlayer(actor.ActorId, CommRPC.FullPlayerListUpdate, (object)SyncObjectBuilder.GetSyncData(chatters, true));
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
                    SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, SyncObjectBuilder.GetSyncData(info, false));
            }
            else
            {
                CmuneDebug.LogError("OnUpdatePlayerRoom failed with actorID '{0}', CommActorInfo exists: {1}", actorId, (info != null));
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
                    SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, SyncObjectBuilder.GetSyncData(info, false));
            }
            else
            {
                CmuneDebug.LogWarning("OnResetPlayerRoom failed with actorID '{0}', CommActorInfo exists: {1}", actorId, (info != null));
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
        [NetworkMethod(CommRPC.UpdateFriendsList)]
        protected void OnUpdateFriendsList(int cmid)
        {
            int actorId;
            if (_actorIdByCmid.TryGetValue(cmid, out actorId))
            {
                SendMethodToPlayer(actorId, CommRPC.UpdateFriendsList);
            }
        }

        [NetworkMethod(CommRPC.UpdateInboxMessages)]
        protected void OnUpdateInboxMessages(int cmid, int messageId)
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
        protected void OnUpdateClanMembers(List<int> clanMembers)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.UpdateClanData)]
        protected void OnUpdateClanData(int cmid)
        {
            int actorId;
            if (_actorIdByCmid.TryGetValue(cmid, out actorId))
            {
                SendMethodToPlayer(actorId, CommRPC.UpdateClanData);
            }
        }

        [NetworkMethod(CommRPC.SendPlayerNameSearchString)]
        protected void OnGetPlayersWithMatchingName(int cmid, string search)
        {
            search = search.ToLowerInvariant();

            List<CommActorInfo> result = new List<CommActorInfo>(30);

            /* Get search result */
            foreach (CommActorInfo c in _actorsByActorId.Values)
            {
                if (c.PlayerName.ToLowerInvariant().Contains(search))
                {
                    result.Add(c);
                    if (result.Count == 30) break;
                }
            }

            CommActorInfo actor;
            if (TryGetActorByCmid(cmid, out actor))
                SendMethodToPlayer(actor.ActorId, CommRPC.FullPlayerListUpdate, (object)SyncObjectBuilder.GetSyncData(result, true));
        }

        #region Chat

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="player"></param>
        [NetworkMethod(CommRPC.ChatMessageInGame)]
        protected void OnChatMessageInGame(int actorId, string message, byte context)
        {
            CommActorInfo user;
            if (_actorsByActorId.TryGetValue(actorId, out user) && !IsPlayerMuted(user.Cmid))
            {
                //UpdateListOfActiveChatters(user);
                SendInGameChatMessageFromPlayer(user, message, context);
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
                //make sure that the receiver actually knows who he is talking to
                UpdateContactsOfUser(reciever, sender);

                //now send the message
                SendMethodToPlayer(reciever.ActorId, CommRPC.ChatMessageToPlayer, sender.Cmid, sender.ActorId, sender.PlayerName, message);
            }
        }

        private void UpdateContactsOfUser(CommActorInfo actor, CommActorInfo contact)
        {
            ContactList contacts;
            if (_contactsByCmid.TryGetValue(actor.Cmid, out contacts) && !contacts.ContactIds.Contains(contact.Cmid))
            {
                //contacts.ContactIds[contact.Cmid] = contact.VersionID;
                SendMethodToPlayer(actor.ActorId, CommRPC.PlayerUpdate, SyncObjectBuilder.GetSyncData(contact, true));
            }
        }

        private void UpdateListOfActiveChatters(CommActorInfo actor)
        {
            if (_recentActorsChatting.EnqueueUnique(actor.ActorId))
            {
                SendMethodToAll(CommRPC.PlayerUpdate, new SendParameters() { Unreliable = true }, SyncObjectBuilder.GetSyncData(actor, true));

                if (_recentActorsChatting.LastItem > 0)
                {
                    CommActorInfo hideActor = null;
                    if (_actorsByActorId.TryGetValue(_recentActorsChatting.LastItem, out hideActor) && hideActor != null)
                        SendMethodToAll(CommRPC.HidePlayer, new SendParameters() { Unreliable = true }, hideActor.Cmid);
                }
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
                    g.RemoveUser(null);
                }
                //add him to the new chat group
                if (!_groups.TryGetValue(newRoom, out g))
                {
                    g = new ChatGroup();
                    _groups[newRoom] = g;
                }

                g.AddUser(null);
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
                g.RemoveUser(null);

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
                g.Value.RemoveUser(null);

                //enqueue room for deletion if room is empty
                if (g.Value.IsEmpty)
                {
                    _emptyRooms.Enqueue(g.Key);
                }
            }

            while (_emptyRooms.Count > 0)
                _groups.Remove(_emptyRooms.Dequeue());
        }

        private void SendInGameChatMessageFromPlayer(CommActorInfo user, string msg, byte context)
        {
            ChatGroup g;
            if (_groups.TryGetValue(user.CurrentRoom, out g))
            {
                g.SendMessageToGroup(user.Cmid, user.ActorId, user.PlayerName, msg, (MemberAccessLevel)user.AccessLevel, context);
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
            _room.ExecutionFiber.Enqueue(() =>
            {
                try
                {
                    ModerationWebServiceClient.BanPermanently(modCmid, playerCmid, UberStrikeCommonConfig.ApplicationId, GetIpAddressOfCmid(modCmid));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("{0} Exception when calling WS BanPermanently with: {1}", e.GetType(), e.Message);
                }
            });

            CommActorInfo info;
            if (TryGetActorByCmid(playerCmid, out info))
            {
                //notify the player that he is beeing disconnected
                //SendMethodToPlayer(info.ActorId, CommRPC.ModerationCustomMessage, "You are permanently banned! Please contact support@cmune.com for any questions.");

                //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                SendMethodToPlayer(info.ActorId, CommRPC.DisconnectAndDisablePhoton, "You are permanently banned! Please contact support@cmune.com for any questions.");

                MailNotification.SendReport(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
            }
            else
            {
                MailNotification.SendReport(modCmid, "Admin" + modCmid, string.Format("Player {0} currently not online", playerCmid), "Permanent Ban", string.Format("Ban performed by Admin with Cmid {0}", modCmid));
            }

            CmunePeer peer;
            if (CommServerRoom.Instance.Actors.TryGetPeerByCmuneID(playerCmid, out peer))
                SecurityManager.ReportPlayer(modCmid, peer, SecurityManager.ReportType.HumanReport, "Perma banned by Moderator  " + modCmid);
            else
                SecurityManager.ReportPlayer(modCmid, playerCmid, SecurityManager.ReportType.HumanReport, "Perma banned by Moderator " + modCmid);
        }

        [NetworkMethod(CommRPC.ModerationBanPlayer)]
        protected void OnModerationBanPlayer(int modCmid, int playerCmid)
        {
            CommActorInfo actor;
            if (TryGetActorByCmid(playerCmid, out actor) && actor != null)
            {
                actor.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Banned;
                actor.IncrementVersion();

                // Send a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                SendMethodToPlayer(actor.ActorId, CommRPC.DisconnectAndDisablePhoton, "You are temporarily banned! Please contact support@cmune.com for any questions.");

                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", actor.PlayerName, actor.Cmid, actor.Channel), "Moderation Ban", string.Format("Ban performaned by Admin with Cmid {0}", modCmid));
            }

            CmunePeer peer;
            if (CommServerRoom.Instance.Actors.TryGetPeerByCmuneID(playerCmid, out peer))
                SecurityManager.ReportPlayer(modCmid, peer, SecurityManager.ReportType.HumanReport, "Banned by Moderator " + modCmid);
            else
                SecurityManager.ReportPlayer(modCmid, peer, SecurityManager.ReportType.HumanReport, "Banned by Moderator " + modCmid);
        }

        [NetworkMethod(CommRPC.ModerationKickGame)]
        protected void OnModerationKickGame(int actorId, int modCmid)
        {
            CommActorInfo info;
            if (_actorsByActorId.TryGetValue(actorId, out info) && info != null)
            {
                MailNotification.LogModeration(modCmid, "Admin " + modCmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Kick Out Of Game", string.Format("Kicked out of game by Admin with Cmid {0}", modCmid));
            }

            CmunePeer peer;
            if (CommServerRoom.Instance.Actors.TryGetPeerByActorID(actorId, out peer))
                SecurityManager.ReportPlayer(modCmid, peer, SecurityManager.ReportType.HumanReport, "Kicked by Moderator  " + modCmid);
            else
                SecurityManager.ReportPlayer(modCmid, peer, SecurityManager.ReportType.HumanReport, "Kicked by Moderator " + modCmid);
        }

        [NetworkMethod(CommRPC.ModerationUnbanPlayer)]
        protected void OnModerationUnbanPlayer(int cmid)
        {
            SecurityManager.UnbanPlayer(cmid);
        }

        [NetworkMethod(CommRPC.SpeedhackDetection)]
        protected void OnSpeedhackDetection(int cmid)
        {
            CmunePeer peer;
            if (_room.Actors.TryGetPeerByCmuneID(cmid, out peer))
                SecurityManager.ReportPlayer(0, peer, SecurityManager.ReportType.SpeedHack, "Auto Reported");
            else
                SecurityManager.ReportPlayer(0, cmid, SecurityManager.ReportType.SpeedHack, "Auto Reported");
        }

        [NetworkMethod(CommRPC.SpeedhackDetectionNew)]
        protected void OnSpeedhackDetectionNew(int cmid, List<float> timeDifferences)
        {
            CommActorInfo actor;
            if (TryGetActorByCmid(cmid, out actor))
            {
                MailNotification.SendSpeedhackReport(cmid, actor.PlayerName, string.Format("{0} - {1} - {2}", actor.PlayerName, actor.Cmid, actor.Channel), CmunePrint.Values(timeDifferences));
            }

            CmunePeer peer;
            if (_room.Actors.TryGetPeerByCmuneID(cmid, out peer))
                SecurityManager.ReportPlayer(0, peer, SecurityManager.ReportType.SpeedHack, "Samples: " + CmunePrint.Values(timeDifferences));
            else
                SecurityManager.ReportPlayer(0, cmid, SecurityManager.ReportType.SpeedHack, "Samples: " + CmunePrint.Values(timeDifferences));
        }

        //[NetworkMethod(CommRPC.SpeedhackDetectionNew)]
        //protected void OnSpeedhackDetectionNew(int cmid, float mean, float variance, float probability, string debug)
        //{
        //    CommActorInfo actor;
        //    if (TryGetActorByCmid(cmid, out actor))
        //    {
        //        // only send mail on first detection
        //        if ((actor.ModerationFlag & (byte)CommActorInfo.ModerationTag.Speedhacking) == 0)
        //            MailNotification.SendSpeedhackReport(cmid, string.Format("Player '{0}' on {1}", actor.PlayerName, actor.Channel), "Speedhack Autodetection", debug);

        //        if (mean < 1.2f)
        //        {
        //            actor.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Speedhacking;
        //            actor.ModInformation = string.Format("{0:N1}x, var: {1:N1} => {2:N1}%", mean, variance, probability);
        //            actor.IncrementVersion();
        //        }
        //    }

        //    if (mean >= 1.2f)
        //    {
        //        CmunePeer peer;
        //        if (_room.Actors.TryGetPeerByCmuneID(cmid, out peer))
        //            SecurityManager.ReportPlayer(0, peer, SecurityManager.ReportType.SpeedHack);
        //        else
        //            SecurityManager.ReportPlayer(0, cmid, SecurityManager.ReportType.SpeedHack);
        //    }
        //}

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
                string comment = "";

                if (TryGetActorByCmid(cmid, out info))
                {
                    abusername += info.PlayerName;

                    switch (type)
                    {
                        case (int)MemberReportType.OffensiveChat:
                            comment = "Offensive Chat";
                            info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Language;
                            break;
                        case (int)MemberReportType.OffensiveName:
                            info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Name;
                            comment = "Offensive Name";
                            break;
                        case (int)MemberReportType.Spamming:
                            info.ModerationFlag |= (byte)CommActorInfo.ModerationTag.Spamming;
                            comment = "Spamming";
                            break;
                    }
                }
                else
                {
                    abusername += "(not found)";
                }

                MailNotification.SendPlayerReport(reportername, abusername, ((MemberReportType)type).ToString(), details, logs);

                CmunePeer peer;
                if (_room.Actors.TryGetPeerByCmuneID(cmid, out peer))
                    SecurityManager.ReportPlayer(reporterCmid, peer, SecurityManager.ReportType.HumanReport, comment);
                else
                    SecurityManager.ReportPlayer(reporterCmid, cmid, SecurityManager.ReportType.HumanReport, comment);
            }
        }

        #endregion

        #region Actor List Updates

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="contactCmids"></param>
        [NetworkMethod(CommRPC.SetContactList)]
        protected void OnSetContactList(int cmid, List<int> contactCmids)
        {
            if (contactCmids.Count > 100)
                contactCmids = contactCmids.GetRange(0, 100);

            ContactList friends;
            if (_contactsByCmid.TryGetValue(cmid, out friends))
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("OnSetContactList for cmid: " + cmid + " - updated existing contacts '" + friends.ContactIds.Count + "' to new '" + contactCmids.Count + "'");
                friends.Update(contactCmids);
            }
            else
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("OnSetContactList: " + cmid + " - new contacts '" + contactCmids.Count + "'");
                _contactsByCmid[cmid] = new ContactList(contactCmids);
            }

            OnUpdateContacts(cmid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.UpdateActorsForModeration)]
        protected void OnSendNaughtyList(int actorId)
        {
            var naughtyList = new Dictionary<int, SyncObject>(SecurityManager.PlayerReports.Count);

            //all online players
            foreach (var rep in SecurityManager.PlayerReports)
            {
                StringBuilder info = new StringBuilder();
                rep.Value.ForEach(s => info.Append(s.Date).Append(": ").Append(s.Type).Append(" - ").AppendLine(s.Info));


                CommActorInfo naughty;
                if (!TryGetActorByCmid(rep.Key, out naughty))
                {
                    naughty = new CommActorInfo()
                   {
                       Cmid = rep.Key,
                       PlayerName = "Offline: " + rep.Key,
                   };
                }

                naughty.ModInformation = info.ToString();

                naughtyList.Add(rep.Key, SyncObjectBuilder.GetSyncData(naughty, true));
            }

            ////all online players
            //foreach (CommActorInfo i in _actorsByActorId.Values)
            //{
            //    if (i.ModerationFlag != 0)
            //    {
            //        naughtyList.Add(i.Cmid, SyncObjectBuilder.GetSyncData(i, true));
            //    }
            //}
            ////all banned players
            //foreach (int i in SecurityManager.BannedCmids)
            //{
            //    if (!naughtyList.ContainsKey(i))
            //    {
            //        naughtyList.Add(i, SyncObjectBuilder.GetSyncData(new CommActorInfo()
            //            {
            //                PlayerName = "Banned: " + i,
            //                Cmid = i,
            //                ModerationFlag = (byte)(CommActorInfo.ModerationTag.Banned | CommActorInfo.ModerationTag.Speedhacking)
            //            }, true));
            //    }
            //}

            SendMethodToPlayer(actorId, CommRPC.UpdateActorsForModeration, (object)naughtyList.Values);
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

            SecurityManager.UnbanPlayer(cmid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorId"></param>
        [NetworkMethod(CommRPC.UpdateAllActors)]
        protected void OnUpdateAllActors(int actorId)
        {
            //int counter = 0;
            List<SyncObject> actors = new List<SyncObject>(_actorsByActorId.Count);
            IEnumerator<CommActorInfo> iter = _actorsByActorId.Values.GetEnumerator();

            while (iter.MoveNext())
            {
                actors.Add(SyncObjectBuilder.GetSyncData(iter.Current, true));

                //if (++counter == 100)
                //{
                //    SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, (object)actors);
                //    actors.Clear();
                //    counter = 0;
                //}
            }

            // for leftovers
            if (actors.Count > 0)
            {
                SendMethodToPlayer(actorId, CommRPC.FullPlayerListUpdate, (object)actors);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        [NetworkMethod(CommRPC.UpdateContacts)]
        protected void OnUpdateContacts(int cmid)
        {
            if (CmuneDebug.IsDebugEnabled)
                CmuneDebug.Log("OnUpdateContacts for cmid: " + cmid);

            ContactList contacts;
            CommActorInfo actor;
            if (_contactsByCmid.TryGetValue(cmid, out contacts) && TryGetActorByCmid(cmid, out actor))
            {
                List<CommActorInfo> updated = new List<CommActorInfo>();
                List<int> removed = new List<int>();

                CommActorInfo info;
                //int aid;
                int[] cmids = Conversion.ToArray<int>(contacts.ContactIds);
                foreach (int c in cmids)
                {
                    //find the CommActorInfo in the current list of live Actors
                    if (TryGetActorByCmid(c, out info))
                    {
                        //if (info.VersionID != contacts.ContactIds[c])
                        //{
                        //    contacts.ContactIds[c] = info.VersionID;
                        //    updated.Add(info);
                        //}
                    }
                    //if friend is not online anymore but his version was not zero (means that the client has him in his active list)
                    //else if (contacts.ContactIds[c] > 0)
                    //{
                    //    contacts.ContactIds[c] = 0;
                    //    removed.Add(c);
                    //}
                }

                //if (CmuneDebug.IsDebugEnabled)
                //    CmuneDebug.Log("final updates: " + updated.Count + " - " + CmunePrint.Values(updated));//+ "/" + removed.Count);

                //send result to player
                if (updated.Count > 0 || removed.Count > 0)
                {
                    //CmuneDebug.Log("versions: " + CmunePrint.Dictionary(contacts.Versions));//+ "/" + removed.Count);
                    SendMethodToPlayer(actor.ActorId, CommRPC.UpdateContacts, (object)SyncObjectBuilder.GetSyncData(updated, true), removed);
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

        private LimitedQueue<int> _recentActorsChatting;
        private Dictionary<int, CommActorInfo> _actorsByActorId;
        private Dictionary<int, int> _actorIdByCmid;
        private Dictionary<int, ContactList> _contactsByCmid;
        private Dictionary<CmuneRoomID, ChatGroup> _groups;

        #endregion
    }
}
