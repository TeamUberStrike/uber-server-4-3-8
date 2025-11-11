using System.Collections.Generic;
using UberStrike.Realtime.Server.Attributes;
using UberStrike.Core.ViewModel;

namespace UberStrike.Realtime.CommServer
{
    [PeerEvents]
    public interface ICommPeerEvents
    {
        void LoadData(ServerConnectionView data);
        void LobbyEntered();
    }
}