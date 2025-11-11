
namespace Cmune.Realtime.Photon.Server
{
    public class RoomMessage : IMessage
    {
        public RoomMessage(int id, params object[] args)
        {
            _messageID = id;
            _args = args;
            _isUsed = false;
        }

        public RoomMessage()
        {
            _messageID = -1;
            _args = null;
            _isUsed = false;
        }

        #region IMessage Members

        public int MessageID
        {
            get { return _messageID; }
        }

        public object[] Arguments
        {
            get { return _args; }
        }

        public bool IsCalled
        {
            get { return _isUsed; }
            set { _isUsed = value; }
        }

        #endregion

        private object[] _args;
        private int _messageID;
        private bool _isUsed;
    }
}
