using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Photon.Server;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Realtime.Photon.CommServer;
using UberStrike.Realtime.Server;
using UberStrike.WebService.DotNet;

namespace UberStrike.Realtime.CommServer
{
    public class LobbyRoom : BaseLobbyRoom
    {
        #region FIELDS

        private Dictionary<int, DateTime> _mutedCmids;

        private LimitedQueue<int> _recentActorsChatting;
        private Dictionary<int, ContactList> _contactsByCmid;
        private Dictionary<CmuneRoomID, ChatGroup> _groups;

        #endregion

        public LobbyRoom(int roomId)
            : base(roomId)
        {
            _mutedCmids = new Dictionary<int, DateTime>(50);
            _recentActorsChatting = new LimitedQueue<int>(150);
            _contactsByCmid = new Dictionary<int, ContactList>(3000);
            _groups = new Dictionary<CmuneRoomID, ChatGroup>(500);
        }

        private static LobbyRoom _instance = new LobbyRoom(0);
        public static LobbyRoom Instance { get { return _instance; } }

        protected override void OnPeerEntered(ICmunePeer peer)
        {
            Events.SendPlayerJoined(peer.Info as CommActorInfo).ToPeer(peer.Cmid);
        }

        protected override void OnPeerLeft(int cmid)
        {
            Events.SendPlayerLeft(cmid, false).ToPeer(cmid);
        }

        #region RPCs

        protected override void OnFullPlayerListUpdate(ICmunePeer peer)
        {
            var chatters = new List<CommActorInfo>(_recentActorsChatting.Count + 1);
            chatters.Add(peer.Info as CommActorInfo);

            //grab all recent chatters
            ICmunePeer p;
            for (int i = 0; i < _recentActorsChatting.Count; i++)
            {
                if (_peerCollection.TryGetByPeerId(_recentActorsChatting[i], out p))
                {
                    chatters.Add(p.Info as CommActorInfo);
                }
            }

            //grab all friends & contatcs currently online
            ContactList contacts;
            if (_contactsByCmid.TryGetValue(peer.Cmid, out contacts) && contacts != null)
            {
                foreach (int id in contacts.ContactIds)
                {
                    //find the CommActorInfo in the current list of live Actors
                    if (_peerCollection.TryGetByPeerId(id, out p))
                    {
                        //contacts.Versions[id] = p.Info.VersionID;
                        chatters.Add(p.Info as CommActorInfo);
                    }
                }
            }

            //send it all back
            Events.SendFullPlayerListUpdate(chatters).ToPeer(peer.Cmid);
        }

        protected override void OnUpdatePlayerRoom(ICmunePeer peer, CmuneRoomID room)
        {
            CommActorInfo info = peer.Info as CommActorInfo;
            if (info != null)
            {
                //put user in a specific chat group
                MoveUserToChatGroup(peer, info.CurrentRoom, room);

                info.CurrentRoom = room;
                //info.IncrementVersion();

                //send the UPDATED playerinfo to all existing players
                if (_recentActorsChatting.Contains(peer.Cmid))
                    Events.SendPlayerUpdate(info).ToAll(false, true);
            }
        }

        protected override void OnResetPlayerRoom(ICmunePeer peer)
        {
            var info = peer.Info as CommActorInfo;
            if (info != null)
            {
                //remove user from the chat group
                RemoveUserFromChatGroup(peer, info.CurrentRoom);

                //reset room data to default
                info.CurrentRoom = new CmuneRoomID()
                {
                    RoomNumber = this.Id,
                    Server = new ConnectionAddress()
                    {
                        Ipv4 = ConnectionAddress.ToInteger(ServerSettings.IP),
                        Port = (short)ServerSettings.Port,
                    },
                };
                //info.IncrementVersion();

                //send the UPDATED playerinfo to all existing players
                if (_recentActorsChatting.Contains(peer.Cmid))
                    Events.SendPlayerUpdate(info).ToAll(false, true);
            }
            else
            {
                CmuneDebug.LogWarning("OnResetPlayerRoom failed with actorID '{0}', CommActorInfo exists: {1}", peer.Cmid, (info != null));
            }
        }

        protected override void OnPlayerUpdate(ICmunePeer peer, CommActorInfo data)
        {
            peer.Info.PlayerName = data.PlayerName;
            peer.Info.Ping = data.Ping;
            peer.Info.CurrentRoom = data.CurrentRoom;
            peer.Info.ClanTag = data.ClanTag;

            //send the UPDATED playerinfo to all existing players);
            if (_recentActorsChatting.Contains(peer.Cmid))
                Events.SendPlayerUpdate(data).ToAll(false);
        }

        protected override void OnUpdateFriendsList(ICmunePeer peer, int cmid)
        {
            Events.SendUpdateFriendsList().ToPeer(cmid);
        }

        protected override void OnUpdateInboxMessages(ICmunePeer peer, int cmid, int messageId)
        {
            Events.SendUpdateInboxMessages(messageId).ToPeer(cmid);
        }

        protected override void OnUpdateClanMembers(ICmunePeer peer, List<int> clanMembers)
        {
            foreach (int cmid in clanMembers)
            {
                Events.SendUpdateClanMembers().ToPeer(cmid);
            }
        }

        protected override void OnUpdateClanData(ICmunePeer peer, int cmid)
        {
            Events.SendUpdateClanMembers().ToPeer(cmid);
        }

        protected override void OnGetPlayersWithMatchingName(ICmunePeer peer, string search)
        {
            search = search.ToLowerInvariant();

            List<CommActorInfo> result = new List<CommActorInfo>(30);

            /* Get search result */
            foreach (var p in _peerCollection)
            {
                if (p.Info.PlayerName.ToLowerInvariant().Contains(search))
                {
                    result.Add(p.Info as CommActorInfo);
                    if (result.Count == 30) break;
                }
            }

            Events.SendFullPlayerListUpdate(result).ToPeer(peer.Cmid);
        }

        protected override void OnChatMessageInGame(ICmunePeer peer, string message, byte context)
        {
            if (!IsPlayerMuted(peer.Cmid))
            {
                SendInGameChatMessageFromPlayer(peer.Info as CommActorInfo, message, context);
            }
        }

        protected override void OnChatMessageToAll(ICmunePeer peer, string message)
        {
            if (!IsPlayerMuted(peer.Cmid))
            {
                UpdateListOfActiveChatters(peer.Info as CommActorInfo);
                Events.SendLobbyChatMessage(peer.Cmid, peer.Info.PlayerName, message).ToAllExcept(peer.Cmid, false);
            }
        }

        protected override void OnChatMessageToPlayer(ICmunePeer peer, int recieverID, string message)
        {
            ICmunePeer reciever;
            if (!IsPlayerMuted(peer.Cmid) && _peerCollection.TryGetByPeerId(recieverID, out reciever))
            {
                //make sure that the receiver actually knows who he is talking to
                UpdateContactsOfUser(reciever.Info as CommActorInfo, peer.Info as CommActorInfo);

                //now send the message
                Events.SendPrivateChatMessage(peer.Cmid, peer.Info.PlayerName, message);
            }
        }

        protected override void OnModerationMutePlayer(ICmunePeer peer, int durationInMinutes, int actorID, bool disableChat)
        {
            Events.SendModerationMutePlayer(disableChat).ToPeer(peer.Cmid);

            var info = peer.Info as CommActorInfo;
            info.ModerationFlag |= disableChat ? (byte)ModerationTag.Muted : (byte)ModerationTag.Ghosted;

            //perform the longterm ban
            if (durationInMinutes > 0)
            {
                _mutedCmids[peer.Cmid] = DateTime.Now.AddMinutes(durationInMinutes);
            }
            else
            {
                _mutedCmids.Remove(peer.Cmid);
            }
        }

        protected override void OnModerationPermanentBan(ICmunePeer peer, int playerCmid)
        {
            //perma ban
            this._fiber.Enqueue(() =>
            {
                try
                {
                    if (peer != null)
                        ModerationWebServiceClient.BanPermanently(peer.Cmid, playerCmid, UberStrikeCommonConfig.ApplicationId, peer.RemoteIP);
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("{0} Exception when calling WS BanPermanently with: {1}", e.GetType(), e.Message);
                }
            });

            ICmunePeer info;
            if (_peerCollection.TryGetByPeerId(playerCmid, out info))
            {
                //cast a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                Events.SendDisconnectAndDisablePhoton("You are permanently banned! Please contact support@cmune.com for any questions.").ToPeer(playerCmid);

                MailNotification.SendReport(peer.Cmid, "Admin " + peer.Cmid, string.Format("{0} - {1} - {2}", info.Info.PlayerName, info.Cmid, info.Info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", peer.Cmid));
                MailNotification.LogModeration(peer.Cmid, "Admin " + peer.Cmid, string.Format("{0} - {1} - {2}", info.Info.PlayerName, info.Cmid, info.Info.Channel), "Permanent Ban", string.Format("Ban performaned by Admin with Cmid {0}", peer.Cmid));

                SecurityManager.ReportPlayer(peer.Cmid, info, SecurityManager.ReportType.HumanReport, "Perma banned by Moderator  " + peer.Cmid);
            }
            else
            {
                MailNotification.SendReport(peer.Cmid, "Admin" + peer.Cmid, string.Format("Player {0} currently not online", playerCmid), "Permanent Ban", string.Format("Ban performed by Admin with Cmid {0}", peer.Cmid));

                SecurityManager.ReportPlayer(peer.Cmid, playerCmid, SecurityManager.ReportType.HumanReport, "Perma banned by Moderator " + peer.Cmid);
            }
        }

        protected override void OnModerationBanPlayer(ICmunePeer peer, int bannedCmid)
        {
            ICmunePeer bannedPeer;
            if (_peerCollection.TryGetByPeerId(bannedCmid, out bannedPeer))
            {
                var info = bannedPeer.Info as CommActorInfo;
                info.ModerationFlag |= (byte)ModerationTag.Banned;
                //actor.IncrementVersion();

                // Send a disconnect to the client to shutdown & lock all photon connections -> forces reload of client & login
                Events.SendDisconnectAndDisablePhoton("You are temporarily banned! Please contact support@cmune.com for any questions.").ToPeer(bannedCmid);
                MailNotification.LogModeration(peer.Cmid, "Admin " + peer.Cmid, string.Format("{0} - {1} - {2}", bannedPeer.Info.PlayerName, bannedPeer.Cmid, bannedPeer.Info.Channel), "Moderation Ban", string.Format("Ban performaned by Admin with Cmid {0}", peer.Cmid));

                SecurityManager.ReportPlayer(peer.Cmid, bannedPeer, SecurityManager.ReportType.HumanReport, "Banned by Moderator " + peer.Cmid);
            }
            else
            {
                SecurityManager.ReportPlayer(peer.Cmid, bannedCmid, SecurityManager.ReportType.HumanReport, "Banned by Moderator " + peer.Cmid);
            }
        }

        protected override void OnModerationKickGame(ICmunePeer peer, int kickedCmid)
        {
            ICmunePeer kickedPeer;
            if (_peerCollection.TryGetByPeerId(kickedCmid, out kickedPeer))
            {
                var info = kickedPeer.Info as CommActorInfo;
                //MailNotification.LogModeration(peer.Cmid, "Admin " + peer.Cmid, string.Format("{0} - {1} - {2}", info.PlayerName, info.Cmid, info.Channel), "Kicked out of game", string.Format("You were kicked out of your game by {0}", peer.Info.PlayerName));
                SecurityManager.ReportPlayer(peer.Cmid, peer, SecurityManager.ReportType.HumanReport, "Kicked by Moderator  " + peer.Cmid);
            }
            else
            {
                SecurityManager.ReportPlayer(peer.Cmid, peer, SecurityManager.ReportType.HumanReport, "Kicked by Moderator " + peer.Cmid);
            }
        }

        protected override void OnModerationUnbanPlayer(ICmunePeer peer, int cmid)
        {
            SecurityManager.UnbanPlayer(cmid);
        }

        protected override void OnSpeedhackDetection(ICmunePeer peer)
        {
            SecurityManager.ReportPlayer(0, peer, SecurityManager.ReportType.SpeedHack, "Auto Reported");
        }

        protected override void OnSpeedhackDetectionNew(ICmunePeer peer, List<float> timeDifferences)
        {
            MailNotification.SendSpeedhackReport(peer.Cmid, peer.Info.PlayerName, string.Format("{0} - {1} - {2}", peer.Info.PlayerName, peer.Cmid, peer.Info.Channel), CmunePrint.Values(timeDifferences));

            SecurityManager.ReportPlayer(0, peer, SecurityManager.ReportType.SpeedHack, "Samples: " + CmunePrint.Values(timeDifferences));
        }

        protected override void OnPlayersReported(ICmunePeer peer, List<int> cmids, int type, string details, string logs)
        {
            string reportername = string.Format("{0} - {1}", peer.Cmid, peer.Info.PlayerName);

            foreach (int cmid in cmids)
            {
                string abusername = string.Format("{0} - ", cmid);
                string comment = "";

                ICmunePeer reportedPeer;
                if (_peerCollection.TryGetByPeerId(cmid, out reportedPeer))
                {
                    var info = reportedPeer.Info as CommActorInfo;
                    abusername += reportedPeer.Info.PlayerName;

                    switch (type)
                    {
                        case (int)MemberReportType.OffensiveChat:
                            comment = "Offensive Chat";
                            info.ModerationFlag |= (byte)ModerationTag.Language;
                            break;
                        case (int)MemberReportType.OffensiveName:
                            info.ModerationFlag |= (byte)ModerationTag.Name;
                            comment = "Offensive Name";
                            break;
                        case (int)MemberReportType.Spamming:
                            info.ModerationFlag |= (byte)ModerationTag.Spamming;
                            comment = "Spamming";
                            break;
                    }
                    SecurityManager.ReportPlayer(peer.Cmid, reportedPeer, SecurityManager.ReportType.HumanReport, comment);
                }
                else
                {
                    abusername += "(not found)";
                    SecurityManager.ReportPlayer(peer.Cmid, cmid, SecurityManager.ReportType.HumanReport, comment);
                }

                MailNotification.SendPlayerReport(reportername, abusername, ((MemberReportType)type).ToString(), details, logs);
            }
        }

        protected override void OnSetContactList(ICmunePeer peer, List<int> contactCmids)
        {
            if (contactCmids.Count > 100)
                contactCmids = contactCmids.GetRange(0, 100);

            ContactList friends;
            if (_contactsByCmid.TryGetValue(peer.Cmid, out friends))
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("OnSetContactList for cmid: " + peer.Cmid + " - updated existing contacts '" + friends.ContactIds.Count + "' to new '" + contactCmids.Count + "'");
                friends.Update(contactCmids);
            }
            else
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("OnSetContactList: " + peer.Cmid + " - new contacts '" + contactCmids.Count + "'");
                _contactsByCmid[peer.Cmid] = new ContactList(contactCmids);
            }

            OnUpdateContacts(peer);
        }

        protected override void OnSendNaughtyList(ICmunePeer peer)
        {
            var naughtyList = new Dictionary<int, CommActorInfo>(SecurityManager.PlayerReports.Count);

            //all online players
            foreach (var report in SecurityManager.PlayerReports)
            {
                StringBuilder info = new StringBuilder();
                report.Value.ForEach(s => info.Append(s.Date).Append(": ").Append(s.Type).Append(" - ").AppendLine(s.Info));

                CommActorInfo naughtyActor;
                ICmunePeer naughtyPeer;
                if (!_peerCollection.TryGetByPeerId(report.Key, out naughtyPeer))
                {
                    naughtyActor = new CommActorInfo()
                   {
                       Cmid = report.Key,
                       PlayerName = "Offline: " + report.Key,
                   };
                }
                else
                {
                    naughtyActor = naughtyPeer.Info as CommActorInfo;
                }

                naughtyActor.ModInformation = info.ToString();

                naughtyList.Add(report.Key, naughtyActor);
            }

            Events.SendUpdateActorsForModeration(new List<CommActorInfo>(naughtyList.Values)).ToPeer(peer.Cmid);
        }

        protected override void OnClearModeratorFlags(ICmunePeer peer, int moderatedCmid)
        {
            if (_mutedCmids.Remove(moderatedCmid))
            {
                Events.SendModerationMutePlayer(false).ToPeer(moderatedCmid);
            }

            SecurityManager.UnbanPlayer(moderatedCmid);

            //update moderation flags
            CommActorInfo actor;
            ICmunePeer moderatedPeer;
            if (_peerCollection.TryGetByPeerId(moderatedCmid, out moderatedPeer))
            {
                actor = moderatedPeer.Info as CommActorInfo;
                actor.ModerationFlag = 0;
                //actor.IncrementVersion();
            }
        }

        protected override void OnUpdateAllActors(ICmunePeer peer)
        {
            List<CommActorInfo> actors = new List<CommActorInfo>(_peerCollection.Count);
            foreach (var p in _peerCollection)
            {
                actors.Add(p.Info as CommActorInfo);
            }

            if (actors.Count > 0)
            {
                Events.SendFullPlayerListUpdate(actors).ToPeer(peer.Cmid);
            }
        }

        protected override void OnUpdateContacts(ICmunePeer peer)
        {
            if (CmuneDebug.IsDebugEnabled)
                CmuneDebug.Log("OnUpdateContacts for cmid: " + peer.Cmid);

            ContactList contacts;
            if (_contactsByCmid.TryGetValue(peer.Cmid, out contacts) && contacts.ContactIds.Count > 0)
            {
                var actors = new List<CommActorInfo>(contacts.ContactIds.Count);

                ICmunePeer info;
                foreach (int c in contacts.ContactIds.ToArray())
                {
                    if (_peerCollection.TryGetByPeerId(c, out info))
                    {
                        actors.Add(info.Info as CommActorInfo);
                    }
                }

                Events.SendUpdateContacts(actors, null).ToPeer(peer.Cmid, true);
            }
        }

        #endregion

        #region chat stuff

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
                    Events.SendModerationMutePlayer(false).ToPeer(cmid);
                }
            }
        }

        public bool IsPlayerMuted(int cmid)
        {
            DateTime time;
            return (_mutedCmids.TryGetValue(cmid, out time) && time.CompareTo(DateTime.Now) > 0);
        }

        private void UpdateContactsOfUser(CommActorInfo actor, CommActorInfo contact)
        {
            ContactList contacts;
            if (_contactsByCmid.TryGetValue(actor.Cmid, out contacts) && !contacts.ContactIds.Contains(contact.Cmid))
            {
                contacts.ContactIds.Add(contact.Cmid);
                Events.SendPlayerUpdate(contact).ToPeer(actor.Cmid);
            }
        }

        private void UpdateListOfActiveChatters(CommActorInfo actor)
        {
            if (_recentActorsChatting.EnqueueUnique(actor.Cmid))
            {
                Events.SendPlayerUpdate(actor).ToAll(false);

                int cmid = _recentActorsChatting.LastItem;
                if (cmid > 0)
                {
                    if (_peerCollection.Contains(cmid))
                        Events.SendPlayerHide(cmid).ToAll(false);
                }
            }
        }

        private void MoveUserToChatGroup(ICmunePeer peer, CmuneRoomID oldRoom, CmuneRoomID newRoom)
        {
            int i = 0;
            try
            {
                ChatGroup g;
                //first remove the user from the old chat group
                if (_groups.TryGetValue(oldRoom, out g))
                {
                    g.RemoveUser(peer);
                }
                //add him to the new chat group
                if (!_groups.TryGetValue(newRoom, out g))
                {
                    g = new ChatGroup();
                    _groups[newRoom] = g;
                }

                g.AddUser(peer);
            }
            catch (Exception e)
            {
                CmuneDebug.Exception("{0} {1} with Message {2} at {3}", i, e.GetType(), e.Message, e.StackTrace);
            }
        }

        private void RemoveUserFromChatGroup(ICmunePeer peer, CmuneRoomID oldRoom)
        {
            ChatGroup g;
            //first remove the user from the old chat group
            if (_groups.TryGetValue(oldRoom, out g))
            {
                g.RemoveUser(peer);

                if (g.IsEmpty)
                {
                    _groups.Remove(oldRoom);
                }
            }
        }

        private void RemoveUserFromAllChatGroups(ICmunePeer peer)
        {
            Queue<CmuneRoomID> _emptyRooms = new Queue<CmuneRoomID>(1);

            //remove the user from the old chat group
            foreach (KeyValuePair<CmuneRoomID, ChatGroup> g in _groups)
            {
                g.Value.RemoveUser(peer);

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
                g.SendMessageToGroup(user.Cmid, user.Cmid, user.PlayerName, msg, user.AccessLevel, context);
            }
        }


        #endregion
    }
}