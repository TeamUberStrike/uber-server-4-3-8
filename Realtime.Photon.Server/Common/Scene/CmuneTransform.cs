
using UnityEngine;
using System.Collections.Generic;
using Cmune.Realtime.Common.IO;

namespace Cmune.Realtime.Common
{
    public class CmuneTransform : IByteArray
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Vector3 BoundingBox;

        public CmuneTransform()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            BoundingBox = Vector3.one;
        }

        public CmuneTransform(Vector3 p)
            : this()
        {
            Position = p;
        }

        public CmuneTransform(Vector3 p, Quaternion r, Vector3 s)
            : this()
        {
            Position = p;
            Rotation = r;
            Scale = s;
        }

        public CmuneTransform(byte[] bytes, ref int idx)
        {
            idx = FromBytes(bytes, idx);
        }

        #region IByteArray Members

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(52);

            DefaultByteConverter.FromVector3(Position, ref bytes);
            DefaultByteConverter.FromQuaternion(Rotation, ref bytes);
            DefaultByteConverter.FromVector3(Scale, ref bytes);
            DefaultByteConverter.FromVector3(BoundingBox, ref bytes);

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int idx)
        {
            Position = DefaultByteConverter.ToVector3(bytes, ref idx);
            Rotation = DefaultByteConverter.ToQuaternion(bytes, ref idx);
            Scale = DefaultByteConverter.ToVector3(bytes, ref idx);
            BoundingBox = DefaultByteConverter.ToVector3(bytes, ref idx);

            return idx;
        }

        //public int? ByteCount
        //{
        //    get { return 52; }
        //}

        #endregion


        public override bool Equals(object obj)
        {
            if (obj is CmuneTransform)
            {
                CmuneTransform other = obj as CmuneTransform;

                return this.BoundingBox.Equals(other.BoundingBox) &&
                    this.Position.Equals(other.Position) &&
                    this.Rotation.Equals(other.Rotation) &&
                    this.Scale.Equals(other.Scale);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
