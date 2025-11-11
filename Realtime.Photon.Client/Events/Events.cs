using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;

namespace Cmune.Realtime.Photon.Client.Events
{
    #region SCENE EVENTS

    public class MemberListUpdatedEvent// : IEventMessage
    {
        public ActorInfo[] Players;

        public MemberListUpdatedEvent(ICollection<ActorInfo> info)
        {
            this.Players = Conversion.ToArray<ActorInfo>(info);
        }
    }

    public class AssetLoadedEvent<Type>// : IEventMessage
    {
        protected Type _asset;
        protected int _instanceID;

        public Type Asset
        {
            get
            {
                return _asset;
            }
        }

        public int InstanceID
        {
            get
            {
                return _instanceID;
            }
        }

        public AssetLoadedEvent(int instanceID, Type asset)
        {
            _instanceID = instanceID;
            _asset = asset;
        }
    }

    public class AbortAllLoadingJobsEvent// : IEventMessage
    {
    }

    public class PlayerBanEvent// : IEventMessage
    {
        public string Message;

        public PlayerBanEvent(string message) { this.Message = message; }
    }

    #endregion

    public class LobbyGameListUpdatedEvent// : IEventMessage
    {
        public ICollection<RoomMetaData> Rooms;
        public int PlayerCount;

        public LobbyGameListUpdatedEvent(ICollection<RoomMetaData> rooms, int playerCount)
        {
            Rooms = rooms;
            PlayerCount = playerCount;
        }
    }
}
