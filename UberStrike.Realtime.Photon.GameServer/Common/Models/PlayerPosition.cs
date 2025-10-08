
using UnityEngine;

namespace UberStrike.Realtime.Common
{
    /// <summary>
    /// 11 bytes
    /// </summary>
    public struct PlayerPosition
    {
        public byte Player;
        public Vector3 Position;
        public int Time;

        public PlayerPosition(byte id, ShortVector3 p, int time)
        {
            Player = id;
            Position = p.Vector3;
            Time = time;
        }
    }
}
