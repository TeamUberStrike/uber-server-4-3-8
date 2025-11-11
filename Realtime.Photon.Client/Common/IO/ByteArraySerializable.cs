
namespace Cmune.Realtime.Common.IO
{
    public abstract class ByteArraySerializable : IByteArray
    {
        public ByteArraySerializable()
        { }

        public ByteArraySerializable(byte[] bytes, int index)
        {
            _index = FromBytes(bytes, index);
        }

        public int Index
        {
            get { return _index; }
        }

        #region IByteArray Members

        public abstract byte[] GetBytes();

        public abstract int FromBytes(byte[] bytes, int idx);

        #endregion

        //public abstract int? ByteCount { get; }

        private int _index = 0;
    }
}
