using UberStrike.Realtime.Common;
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;

namespace UberStrike.Realtime.Photon.GameServer
{
    [NetworkClass(GameModeID.ModerationMode)]
    public class ModeratorGameMode : ServerNetworkClass
    {
        protected ModeratorGameMode(RemoteMethodInterface rmi, CmuneRoom room)
            : base(rmi, room)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        [NetworkMethod(GameRPC.Join)]
        protected void OnJoinMode(ActorInfo actor)
        {
        }

        protected override void OnPlayerLeftGame(int actorID)
        {
        }
    }
}
