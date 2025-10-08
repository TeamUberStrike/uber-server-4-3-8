using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Server;
using UberStrike.Realtime.Common;


namespace UberStrike.Realtime.Photon.GameServer
{
    [NetworkClass(GameModeID.DeathMatch)]
    public class DeathMatchGameMode : FpsGameMode
    {
        public DeathMatchGameMode(RemoteMethodInterface rmi, CmuneRoom room)
            : base(rmi, room)
        {
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
                //set respawn position for splatted player
                RespawnPlayerInSeconds(player, suicide ? 8 : 5);
            }
        }
    }
}