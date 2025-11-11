
namespace Cmune.Realtime.Photon.Server
{
    public interface IServerNetworkClassFactory
    {
        bool CreateInstance(short id, RemoteMethodInterface rmi, CmuneRoom room, out ServerNetworkClass instance);
    }
}