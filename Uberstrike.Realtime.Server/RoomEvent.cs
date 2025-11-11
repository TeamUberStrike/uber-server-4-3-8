using Photon.SocketServer;
using UberStrike.Realtime.Server;
using System;
using System.Collections.Generic;
using UberStrike.Core.Serialization.Utils;

public class RoomEvent
{
    private BaseRoom room;

    public IEventData Data { get; private set; }

    public RoomEvent(BaseRoom room, IEventData data)
    {
        this.room = room;
        this.Data = data;
    }

    public void ToAll(bool reliable = true, bool deltaCompressed = false)
    {
        if (deltaCompressed)
        {
            Compress(room);
        }

        foreach (var peer in room.Peers)
        {
            peer.PublishEvent(Data, reliable);
        }
    }

    public void ToPeer(int cmid, bool deltaCompressed = false)
    {
        ICmunePeer peer;
        if (room.TryGetPeer(cmid, out peer))
        {
            if (deltaCompressed)
                Compress(peer);

            peer.PublishEvent(Data);
        }
    }

    public void ToAllExcept(int cmid, bool reliable = true)
    {
        foreach (var peer in room.Peers)
        {
            if (peer.Cmid != cmid) peer.PublishEvent(Data, reliable);
        }
    }

    private void Compress(ICompressionContext context)
    {
        byte[] data = Data.Parameters[0] as byte[];
        byte[] baseData = context.ReadCache(Data.Code);

        Data.Parameters[0] = DeltaCompression.Deflate(baseData, data);
        Data.Parameters[10] = true;

        context.WriteCache(Data.Code, data);
    }
}

public interface ICompressionContext
{
    byte[] ReadCache(byte id);
    void WriteCache(byte id, byte[] data);
}