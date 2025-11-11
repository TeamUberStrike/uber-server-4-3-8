using Cmune.Realtime.Common;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server.Rooms
{
    public class DefaultServerNetworkClassFactory : IServerNetworkClassFactory
    {
        public virtual bool CreateInstance(short id, RemoteMethodInterface rmi, CmuneRoom room, out ServerNetworkClass instance)
        {
            switch (id)
            {
                case NetworkClassID.LobbyCenter:
                    instance = new ServerLobbyCenter(rmi);
                    return true;

                default:
                    instance = null;
                    CmuneDebug.LogError("Mode with ID '{0}' not Implemented on Server!", id);
                    return false;
            }
        }
    }
}