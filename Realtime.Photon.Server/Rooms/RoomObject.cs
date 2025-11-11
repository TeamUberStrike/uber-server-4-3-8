
using UnityEngine;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;


namespace Cmune.Realtime.Photon.Server
{
    public class RoomObject : System.IComparable<RoomObject>, IByteArray
    {
        public RoomObject(int player, int assetID, short netID, AssetType type, CmuneTransform transform, List<byte> config)
        {
            this._creatorID = player;
            this._assetID = assetID;
            this._netID = netID;
            this._type = type;
            this._transform = transform;

            this._state = new NetworkPackage(netID, transform.Position, transform.Rotation);

            _syncData = new Dictionary<byte, object>();

            _debugBuilder = new System.Text.StringBuilder();

            if (config != null)
                FromBytes(config.ToArray(), 0);

            updateFieldDebug();
        }

        public RoomObject(int player, int assetID, short netID, AssetType type, Vector3 pos, Quaternion rot, Vector3 sca, List<byte> config)
            : this(player, assetID, netID, type, new CmuneTransform(pos, rot, sca), config)
        { }

        public override string ToString()
        {
            return _debugBuilder.ToString();
        }

        public int CompareTo(RoomObject obj)
        {
            if (obj != null)
                return (int)this._type - (int)obj._type;
            else 
                return 0;
        }

        #region IByteArray Members

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (KeyValuePair<byte, object> k in _syncData)
            {
                if (RealtimeSerialization.IsTypeSupported(k.Value.GetType()))
                {
                    bytes.Add(k.Key);
                    RealtimeSerialization.ToBytes(ref bytes, k.Value);
                }
            }
            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            int i = idx;
            for (; i < bytes.Length - 1; )
            {
                byte b = bytes[i++];
                object obj = RealtimeSerialization.ToObject(bytes, ref i);
                if (obj != null)
                    _syncData[b] = obj;
            }

            updateFieldDebug();
            return i;
        }

        //public int? ByteCount
        //{
        //    get { return null; }
        //}

        private void updateFieldDebug()
        {
            //            _debugBuilder.Remove(0, _debugBuilder.Length);

            //            _debugBuilder.AppendLine("Object: " + _netID.ToString() + "/" + _type.ToString());

            //            //builder.Append(config);
            //            foreach (KeyValuePair<byte, object> k in _synchronization)
            //            {
            //                if (k.Value is IEnumerable<byte>)
            //                {
            //                    _debugBuilder.Append("[");
            //                    foreach (byte b in k.Value as IEnumerable<byte>)
            //                    {
            //                        _debugBuilder.Append(b).Append(",");
            //                    }
            //                    _debugBuilder.Append("]").Append("|");
            //                }
            //                else
            //                    _debugBuilder.Append(k.Value.ToString()).Append("|");
            //            }
        }

        #endregion

        #region PROPERTIES
        public short NetworkID
        {
            get { return _netID; }
            private set { _netID = value; }
        }

        public int AssetID
        {
            get { return _assetID; }
            private set { _assetID = value; }
        }

        public int CreatorID
        {
            get { return _creatorID; }
            private set { _creatorID = value; }
        }

        public AssetType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public NetworkPackage State
        {
            get { return _state; }
            set { _state = value; }
        }

        public CmuneTransform Transform
        {
            get
            {
                _transform.Position = _state.position;
                _transform.Rotation = _state.rotation;
                return _transform;
            }
        }

        public Vector3 Scaling
        {
            get { return _transform.Scale; }
            set { _transform.Scale = value; }
        }
        #endregion

        #region FIELDS
        private int _assetID;
        private short _netID;
        private int _creatorID;
        private AssetType _type;
        private CmuneTransform _transform;
        private NetworkPackage _state;
        private Dictionary<byte, object> _syncData;
        private System.Text.StringBuilder _debugBuilder;
        #endregion
    }
}