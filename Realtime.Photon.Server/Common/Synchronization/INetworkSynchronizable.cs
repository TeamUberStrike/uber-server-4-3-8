
namespace Cmune.Realtime.Common
{
    /// <summary>
    /// The interface 
    /// </summary>
    public interface INetworkSynchronizable
    {
        bool IsSynchronizationEnabled { get; }

        void SyncChanges();
    }

    namespace Synchronization
    {
        public enum LocalAddress
        {
            SynchronizeProperties
        }
    }
}