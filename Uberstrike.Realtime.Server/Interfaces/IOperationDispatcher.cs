
namespace UberStrike.Realtime.Server
{
    public interface IOperationDispatcher
    {
        void OnOperation(byte id, byte[] data);
    }
}