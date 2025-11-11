using System;
using System.Collections.Generic;
using Cmune.Realtime.Common.IO;
using Cmune.Util;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// Bytes: 6
    /// </summary>
    public struct ConnectionAddress : IByteArray
    {
        public static ConnectionAddress Empty
        {
            get { return new ConnectionAddress(0); }
        }

        private ConnectionAddress(int dummy)
        {
            _connectionString = string.Empty;
            _connectionBytes = new byte[6] { 0, 0, 0, 0, 0, 0 };

            UpdateConnectionString();
        }

        public ConnectionAddress(string ipAddress, short port)
        {
            _connectionString = string.Empty;
            _connectionBytes = new byte[6] { 0, 0, 0, 0, 0, 0 };

            FromString(string.Format("{0}:{1}", ipAddress, port));
        }

        public ConnectionAddress(string connectionString)
        {
            _connectionString = string.Empty;
            _connectionBytes = new byte[6] { 0, 0, 0, 0, 0, 0 };

            FromString(connectionString);
        }

        public bool IsValid
        {
            get { return _connectionBytes[0] > 0; }
        }

        public string ServerIP
        {
            get { return string.Format("{0}.{1}.{2}.{3}", _connectionBytes[0], _connectionBytes[1], _connectionBytes[2], _connectionBytes[3]); }
        }

        public string ServerPort
        {
            get { int i = 4; return DefaultByteConverter.ToUShort(_connectionBytes, ref i).ToString(); }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                FromString(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ConnectionAddress)
                return this == (ConnectionAddress)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _connectionString.GetHashCode();
        }

        public static bool operator ==(ConnectionAddress a, ConnectionAddress b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.ConnectionString == b.ConnectionString;
        }

        public static bool operator !=(ConnectionAddress a, ConnectionAddress b)
        {
            return !(a == b);
        }

        private void UpdateConnectionString()
        {
            int i = 4;
            _connectionString = string.Format("{0}.{1}.{2}.{3}:{4}", _connectionBytes[0], _connectionBytes[1], _connectionBytes[2], _connectionBytes[3], DefaultByteConverter.ToUShort(_connectionBytes, ref i));
        }

        private void FromString(string connection)
        {
            string[] token = connection.Split(':');
            if (token.Length == 2)
            {
                if (token[0].Equals("localhost", System.StringComparison.InvariantCultureIgnoreCase))
                    token[0] = "127.0.0.1";

                string[] ips = token[0].Split('.');
                ushort port = 0;

                if (!byte.TryParse(ips[0], out _connectionBytes[0]) ||
                    !byte.TryParse(ips[1], out _connectionBytes[1]) ||
                    !byte.TryParse(ips[2], out _connectionBytes[2]) ||
                    !byte.TryParse(ips[3], out _connectionBytes[3]) ||
                    !ushort.TryParse(token[1], out port))
                {
                    if (CmuneDebug.IsWarningEnabled)
                        CmuneDebug.LogWarning("The Server Connection string '{0}' is not a combination of IP addess and port. Use the format <xxx.xxx.xxx.xxx:port>", connection);
                }
                else
                {
                    List<byte> bytes = new List<byte>(2);
                    DefaultByteConverter.FromUShort(port, ref bytes);
                    _connectionBytes[4] = bytes[0];
                    _connectionBytes[5] = bytes[1];
                }
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("The Server Connection string '{0}' is not a combination of IP addess and port. Use the format <xxx.xxx.xxx.xxx:port>", connection);
            }

            UpdateConnectionString();
        }

        public override string ToString()
        {
            return _connectionString;
        }

        #region IByteArray Members

        public byte[] GetBytes()
        {
            return _connectionBytes;
        }

        public int FromBytes(byte[] bytes, int index)
        {
            Array.Copy(bytes, index, _connectionBytes, 0, 6);

            int idx = 4;
            _connectionString = string.Format("{0}.{1}.{2}.{3}:{4}", _connectionBytes[0], _connectionBytes[1], _connectionBytes[2], _connectionBytes[3], DefaultByteConverter.ToUShort(_connectionBytes, ref idx));

            return index + 6;
        }

        //public int? ByteCount
        //{
        //    get { return 6; }
        //}

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private byte[] _connectionBytes;

        /// <summary>
        /// 
        /// </summary>
        private string _connectionString;
    }
}
