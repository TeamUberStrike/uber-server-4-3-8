using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Common.IO;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// Basic implementation of IRoomData
    /// 
    /// Bytes: [min: 7, avg: 31, max: 200] + 12
    /// </summary>
    public class RoomMetaData : IRoomMetaData, IByteArray
    {
        public static RoomMetaData Empty
        {
            get { return new RoomMetaData(); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected RoomMetaData() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomNumber"></param>
        /// <param name="roomName"></param>
        /// <param name="server"></param>
        public RoomMetaData(int roomNumber, string roomName, string server)
        {
            _roomID.Number = roomNumber;
            _roomID.Server = server;

            this._roomName = roomName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="idx"></param>
        public RoomMetaData(byte[] t, ref int idx)
        {
            idx = FromBytes(t, idx);
        }

        public override string ToString()
        {
            System.Text.StringBuilder b = new System.Text.StringBuilder();

            b.AppendFormat("ID: {0}\n", _roomID);
            b.AppendFormat("Name: {0}\n", _roomName);
            b.AppendFormat("Password: {0}\n", _password);
            b.AppendFormat("Players: {0}/{1}\n", _inPlayers, _maxPlayers);
            b.AppendFormat("Tag: {0}\n", Tag);
            b.AppendFormat("IP: {0}\n", _roomID.Server);

            return b.ToString();
        }

        #region Properties

        public float RoomJoinValue
        {
            get
            {
                float val = 0;
                float freeSlots = MaxPlayers - ConnectedPlayers;
                if (freeSlots <= 2)
                {
                    val = freeSlots * 0.5f;
                }
                else
                {
                    val = 1f / freeSlots;
                }
                return val * MaxPlayers * MaxPlayers;
            }
        }

        public virtual string RoomName
        {
            get { return _roomName; }
            set { _roomName = value; }
        }

        public int RoomNumber
        {
            get { return _roomID.Number; }
        }

        public CmuneRoomID RoomID
        {
            get { return _roomID; }
            set { _roomID = value; }
        }

        public virtual int ConnectedPlayers
        {
            get { return _inPlayers; }
            set { _inPlayers = value; }
        }

        public virtual int MaxPlayers
        {
            get { return _maxPlayers; }
            set { _maxPlayers = value; }
        }

        public virtual string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public virtual bool IsPublic
        {
            get { return string.IsNullOrEmpty(Password); }
        }

        public bool IsFull
        {
            get { return ConnectedPlayers >= MaxPlayers; }
        }

        public string ServerConnection
        {
            get { return _roomID.Server; }
            set { _roomID.Server = value; }
        }

        public PhotonUsageType Tag
        {
            get { return (PhotonUsageType)_tag; }
            set { _tag = (byte)value; }
        }

        #endregion

        #region IByteArray Members

        public virtual byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            DefaultByteConverter.FromString(this._roomName, ref bytes, true);
            DefaultByteConverter.FromString(this._password, ref bytes, true);
            DefaultByteConverter.FromByte((byte)this._maxPlayers, ref bytes);
            DefaultByteConverter.FromByte((byte)this._inPlayers, ref bytes);
            DefaultByteConverter.FromByte((byte)this._tag, ref bytes);
            bytes.AddRange(_roomID.GetBytes()); //12 byte

            return bytes.ToArray();
        }

        public virtual int FromBytes(byte[] bytes, int idx)
        {
            this._roomName = DefaultByteConverter.ToString(bytes, ref idx, true);
            this._password = DefaultByteConverter.ToString(bytes, ref idx, true);
            this._maxPlayers = (int)DefaultByteConverter.ToByte(bytes, ref idx);
            this._inPlayers = (int)DefaultByteConverter.ToByte(bytes, ref idx);
            this._tag = DefaultByteConverter.ToByte(bytes, ref idx);
            idx = _roomID.FromBytes(bytes, idx);

            return idx;
        }

        //public int? ByteCount
        //{
        //    get { return null; }
        //}
        #endregion

        #region FIELDS
        protected string _roomName = string.Empty;
        protected string _password = string.Empty;
        protected int _maxPlayers = 0;
        protected int _inPlayers = 0;
        protected byte _tag = 0;

        CmuneRoomID _roomID = CmuneRoomID.Empty;
        #endregion

    }
}