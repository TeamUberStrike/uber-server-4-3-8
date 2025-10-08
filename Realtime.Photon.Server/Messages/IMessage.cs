
namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// Interface of a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        int MessageID { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsCalled { get; set; }
    }
}