using System;
using System.Collections.Generic;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// Bytes: 12
    /// </summary>
    public struct CmuneRoomID : IByteArray, IComparable<CmuneRoomID>
    {
        private CmuneRoomID(int number)
        {
            _major = Protocol.Major;
            _minor = Protocol.Minor;
            _uniqueID = string.Empty;
            _hashCode = 0;

            _server = ConnectionAddress.Empty;
            _number = number;
        }

        public CmuneRoomID(int number, string server)
            : this(number)
        {
            _server.ConnectionString = server;
            UpdateID();
        }

        public CmuneRoomID(byte[] bytes, ref int idx)
            : this(0)
        {
            idx = FromBytes(bytes, idx);
            UpdateID();
        }

        public CmuneRoomID(byte[] bytes)
            : this(0)
        {
            FromBytes(bytes, 0);
            UpdateID();
        }

        private void UpdateID()
        {
            _uniqueID = string.Format("{0}.{1}.{2}:{3}", _major, _minor, Number, Server);
            _hashCode = _uniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return _uniqueID;
        }

        #region IByteArray Members

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(12);

            bytes.Add(_major);
            bytes.Add(_minor);

            DefaultByteConverter.FromInt(_number, ref bytes);

            bytes.AddRange(_server.GetBytes());

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            _major = bytes[idx++];
            _minor = bytes[idx++];

            _number = DefaultByteConverter.ToInt(bytes, ref idx);

            idx = _server.FromBytes(bytes, idx);

            UpdateID();

            return idx;
        }

        #endregion

        #region Comparison

        public int CompareTo(CmuneRoomID other)
        {
            if (!ReferenceEquals(other, null))
                return this.ID.CompareTo(other.ID);
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                if (obj is CmuneRoomID)
                {
                    CmuneRoomID other = (CmuneRoomID)obj;
                    return this.ID == other.ID;
                }
                else return false;
            }
            else return false;
        }

        public static bool operator ==(CmuneRoomID a, CmuneRoomID b)
        {
            if (!ReferenceEquals(a, null) && !ReferenceEquals(b, null))
            {
                return a.ID == b.ID;
            }
            else return ReferenceEquals(a, b);
        }

        public static bool operator !=(CmuneRoomID a, CmuneRoomID b)
        {
            if (!ReferenceEquals(a, null) && !ReferenceEquals(b, null))
            {
                return a.ID != b.ID;
            }
            else return !ReferenceEquals(a, b);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        #endregion

        #region PROPERTIES
        public static CmuneRoomID Empty
        {
            get { CmuneRoomID r = new CmuneRoomID(0); r.UpdateID(); return r; }
        }
        public bool IsEmpty
        {
            get { return Number == 0; }
        }
        public bool CanConnectToServer
        {
            get { return _server.IsValid; }
        }
        public bool IsVersionCompatible
        {
            get { return (this._minor == Protocol.Minor && this._major == Protocol.Major); }
        }
        public string ID
        {
            get { return _uniqueID; }
        }
        public string Server
        {
            set { _server.ConnectionString = value; UpdateID(); }
            get { return _server.ConnectionString; }
        }
        public int Number
        {
            set { _number = value; UpdateID(); }
            get { return _number; }
        }
        #endregion

        #region FIELDS
        //PROTOCOL VERSION
        private byte _major;
        private byte _minor;

        //PHOTON INSTANCE
        private ConnectionAddress _server;

        //ROOM NUMBER
        private int _number;

        //internalHashcode
        private string _uniqueID;
        private int _hashCode;
        #endregion
    }
}
