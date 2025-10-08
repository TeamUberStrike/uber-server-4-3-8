
using UnityEngine;
using Cmune.Realtime.Common;
using System.Collections.Generic;
using Cmune.Util;

namespace UberStrike.Realtime.Common
{
    public struct RotationVector3
    {
        private Vector3 _vector;

        public RotationVector3(float x, float y, float z)
            : this(new Vector3(x, y, z))
        { }

        public RotationVector3(byte[] bytes, ref int idx)
        {
            byte x = bytes[idx++];
            byte y = bytes[idx++];
            byte z = bytes[idx++];

            int d2 = x; d2 |= y << 8;
            _vector.x = d2 % 360;
            _vector.y = Mathf.Clamp(d2 / 360, 0, 180) - 90;
            _vector.z = Mathf.Clamp(z, 0, 180) - 90;
        }

        public RotationVector3(Vector3 v)
        {
            CmuneDebug.Assert(v.x >= -360 && v.x <= 360, "Pitch Angle out of Range");
            CmuneDebug.Assert(v.y >= -90 && v.y <= 90, "Yaw Angle out of Range");
            CmuneDebug.Assert(v.z >= -90 && v.z <= 90, "Roll Angle out of Range");
            _vector = v;
        }

        public Vector3 Vector3
        {
            get { return _vector; }
        }

        public static void Bytes(List<byte> bytes, Vector3 v)
        {
            byte[] b = new byte[3];
            uint d1 = (uint)((v.x < 0 ? 360 + v.x % 360 : v.x % 360) + Mathf.Clamp(v.y + 90, 0, 180) * 360);
            b[0] = (byte)(d1 & 0xFF);
            b[1] = (byte)((d1 >> 8) & 0xFF);
            b[2] = (byte)(Mathf.Clamp(v.z + 90, 0, 180));
            bytes.AddRange(b);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(3);
            RotationVector3.Bytes(bytes, _vector);
            return bytes.ToArray();
        }
    }
}
