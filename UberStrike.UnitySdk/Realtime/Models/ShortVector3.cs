
using UnityEngine;
using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Util;


namespace UberStrike.Realtime.Common
{
    public struct ShortVector3
    {
        private Vector3 _vector;

        public ShortVector3(byte[] bytes, ref int idx)
        {
            short x = DefaultByteConverter.ToShort(bytes, ref idx);
            short y = DefaultByteConverter.ToShort(bytes, ref idx);
            short z = DefaultByteConverter.ToShort(bytes, ref idx);

            _vector = new Vector3(x / 10f, y / 100f, z / 10f);
        }

        public ShortVector3(Vector3 v)
        {
            _vector = v;
        }

        public Vector3 Vector3
        {
            get { return _vector; }
        }

        public static Vector3 OptimizedVector3(Vector3 v)
        {
            short x = (short)Mathf.Clamp(v.x * 10, short.MinValue, short.MaxValue);
            short y = (short)Mathf.Clamp(v.y * 100, short.MinValue, short.MaxValue);
            short z = (short)Mathf.Clamp(v.z * 10, short.MinValue, short.MaxValue);

            return new Vector3(x / 10f, y / 100f, z / 10f);
        }

        public Vector3 GetOptimizedVector3()
        {
            short x = (short)Mathf.Clamp(_vector.x * 10, short.MinValue, short.MaxValue);
            short y = (short)Mathf.Clamp(_vector.y * 100, short.MinValue, short.MaxValue);
            short z = (short)Mathf.Clamp(_vector.z * 10, short.MinValue, short.MaxValue);

            return new Vector3(x / 10f, y / 100f, z / 10f);
        }

        /// <summary>
        /// The vector will be first converted to a ShortVector3
        /// and then serialzed to 6 bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="v"></param>
        public static void Bytes(List<byte> bytes, Vector3 v)
        {
            short x = (short)Mathf.Clamp(v.x * 10, short.MinValue, short.MaxValue);
            short y = (short)Mathf.Clamp(v.y * 100, short.MinValue, short.MaxValue);
            short z = (short)Mathf.Clamp(v.z * 10, short.MinValue, short.MaxValue);

            DefaultByteConverter.FromShort(x, ref bytes);
            DefaultByteConverter.FromShort(y, ref bytes);
            DefaultByteConverter.FromShort(z, ref bytes);
        }

        /// <summary>
        /// The vector will be first converted to a ShortVector3
        /// and then serialzed to 6 bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="v"></param>
        public static Vector3 FromBytes(byte[] bytes, ref int idx)
        {
            short x = DefaultByteConverter.ToShort(bytes, ref idx);
            short y = DefaultByteConverter.ToShort(bytes, ref idx);
            short z = DefaultByteConverter.ToShort(bytes, ref idx);

            return new Vector3(x / 10f, y / 100f, z / 10f);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(6);
            ShortVector3.Bytes(bytes, _vector);
            return bytes.ToArray();
        }
    }

}
