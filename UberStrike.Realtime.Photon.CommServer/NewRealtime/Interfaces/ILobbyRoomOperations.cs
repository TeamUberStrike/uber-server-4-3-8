using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Realtime.Server.Attributes;

namespace UberStrike.Realtime.CommServer
{
    [RoomOperations]
    public interface ILobbyRoomOperations
    {
        //NOTICE:
        //All operations will automatically have the peerId of the sender as first argument

        void FullPlayerListUpdate();
        void UpdatePlayerRoom(CmuneRoomID room);
        void ResetPlayerRoom();
        void PlayerUpdate(CommActorInfo data);
        void UpdateFriendsList(int cmid);
        void UpdateInboxMessages(int cmid, int messageId);
        void UpdateClanMembers(List<int> clanMembers);
        void UpdateClanData(int cmid);
        void GetPlayersWithMatchingName(string search);
        void ChatMessageInGame(string message, byte context);
        void ChatMessageToAll(string message);
        void ChatMessageToPlayer(int cmid, string message);

        void ModerationMutePlayer(int durationInMinutes, int cmid, bool disableChat);
        void ModerationPermanentBan(int cmid);
        void ModerationBanPlayer(int cmid);
        void ModerationKickGame(int cmid);
        void ModerationUnbanPlayer(int cmid);
        void SpeedhackDetection();
        void SpeedhackDetectionNew(List<float> timeDifferences);
        void PlayersReported(List<int> cmids, int type, string details, string logs);
        void SendNaughtyList();
        void ClearModeratorFlags(int cmid);

        void SetContactList(List<int> cmids);
        void UpdateAllActors();
        void UpdateContacts();
    }
}