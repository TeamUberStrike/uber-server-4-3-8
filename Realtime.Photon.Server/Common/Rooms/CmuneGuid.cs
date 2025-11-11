using System;

namespace Cmune.Realtime.Common
{
    public class CmuneGuid : IByteArray
    {
        private Guid _guid;
        private string _stringID;

        public CmuneGuid()
        {
            _guid = new Guid();
            _stringID = _guid.ToString();
        }

        public CmuneGuid(string id)
        {
            _guid = new Guid(id);
            _stringID = _guid.ToString();
        }

        public string ID
        {
            get { return _stringID; }
            private set { _stringID = value; }
        }

        #region IByteArray Members

        public byte[] GetBytes()
        {
            return _guid.ToByteArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            byte[] array = new byte[16];
            System.Array.Copy(bytes, idx, array, 0, 16);

            _guid = new Guid(array);

            _stringID = _guid.ToString();

            idx += 16;
            return idx;
        }

        #endregion

        //public int? ByteCount
        //{
        //    get { return 16; }
        //}
    }
}
