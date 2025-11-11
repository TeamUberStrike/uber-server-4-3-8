using UberStrike.Realtime.Server.Attributes;
using UberStrike.Core.Types;

namespace UberStrike.Realtime.CommServer
{
    [PeerOperations]
    public interface ICommPeerOperations
    {
        void GetServerLoad();

        void EnterLobby();
        void LeaveLobby();
    }
}