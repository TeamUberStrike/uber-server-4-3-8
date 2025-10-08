using Photon.SocketServer;
using UberStrike.Core.Models;

namespace UberStrike.Realtime.Server
{
    public interface ICmunePeer : ICompressionContext
    {
        int Cmid { get; }
        string RemoteIP { get; }
        ActorInfo Info { get; }

        void PublishEvent(IEventData eventData, bool reliable = true);
        void Disconnect();
    }
}