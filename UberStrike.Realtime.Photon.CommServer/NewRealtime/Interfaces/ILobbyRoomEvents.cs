using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Realtime.Server.Attributes;
using Cmune.DataCenter.Common.Entities;

namespace UberStrike.Realtime.CommServer
{
    [RoomEvents]
    public interface ILobbyRoomEvents
    {
        //void PlayerLeft(int cmid);
        void PlayerHide(int cmid);
        void PlayerLeft(int cmid, bool refreshComm);
        void PlayerUpdate(CommActorInfo data);
        void UpdateContacts(List<CommActorInfo> updated, List<int> removed);
        void FullPlayerListUpdate(List<CommActorInfo> players);
        void PlayerJoined(CommActorInfo data);
        void ClanChatMessage(int cmid, string name, string message);
        void InGameChatMessage(int cmid, string name, string message, MemberAccessLevel accessLevel, byte context);
        void LobbyChatMessage(int cmid, string name, string message);
        void PrivateChatMessage(int cmid, string name, string message);
        void GameInviteMessage(int actorId, string message, CmuneRoomID roomId);
        void DisconnectAndDisablePhoton(string message);
        void UpdateIngameGroup(List<int> cmids);
        void UpdateInboxRequests();
        void UpdateFriendsList();
        void UpdateInboxMessages(int messageId);
        void UpdateClanMembers();
        void UpdateClanData();
        void UpdateActorsForModeration(List<CommActorInfo> allHackers);

        void ModerationCustomMessage(string message);
        void ModerationMutePlayer(bool isPlayerMuted);
        void ModerationKickGame();
    }
}