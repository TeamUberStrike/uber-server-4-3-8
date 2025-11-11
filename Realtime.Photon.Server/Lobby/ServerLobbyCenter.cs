using Cmune.Realtime.Common;

namespace Cmune.Realtime.Photon.Server
{
    [NetworkClass(NetworkClassID.LobbyCenter)]
    public class ServerLobbyCenter : ServerNetworkClass
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        public ServerLobbyCenter(RemoteMethodInterface center)
            : base(center, LobbyServerRoom.Instance)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(LobbyRPC.Join)]
        protected void OnJoin(ActorInfo player)
        {
            LobbyServerRoom.Instance.RegisterToLobbyEvents(player.ActorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        [NetworkMethod(LobbyRPC.Leave)]
        protected void OnLeave(int playerID)
        {
            OnPlayerLeftGame(playerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        protected override void OnPlayerLeftGame(int actorID)
        { }
    }
}
