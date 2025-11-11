
using UnityEngine;
using System.Collections.Generic;
using Cmune.Realtime.Common.IO;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// Store the current state of an network object and its owner
    /// if the owner id is smaller 0, the object state is 
    /// </summary>
    public class NetworkPackage : IByteArray
    {
        public short netID = -1;
        public int playerID = -1;

        public PackageState state = PackageState.PAO;

        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 velocity = Vector3.zero;
        public Vector3 angularVelocity = Vector3.zero;

        public short timeStamp;

        public NetworkPackage(byte[] bytes, ref int idx)
        {
            idx = FromBytes(bytes, idx);
        }

        public NetworkPackage(short netID, Vector3 pos, Quaternion rot)
        {
            this.state = PackageState.PAO;

            this.netID = netID;
            this.position = pos;
            this.rotation = rot;
        }

        public NetworkPackage(short netID, Vector3 pos, Quaternion rot, short time, int playerID, Vector3 vel, Vector3 avel)
            : this(netID, pos, rot)
        {
            this.state = PackageState.DCO;

            this.playerID = playerID;
            this.velocity = vel;
            this.angularVelocity = avel;

            this.timeStamp = time;
        }

        #region Implementation IByteArray

        public virtual byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();

            DefaultByteConverter.FromShort(netID, ref bytes);

            DefaultByteConverter.FromByte((byte)state, ref bytes);
            DefaultByteConverter.FromVector3(position, ref bytes);
            DefaultByteConverter.FromQuaternion(rotation, ref bytes);

            //if we send a (PAO) Physically Affected Object, we know that the playerID is -1 and
            //the object's velocities are not simulated but calculated by the local physics engine
            //-> by not sending them we save (7 * 4) = 28 bytes per package
            if (state != PackageState.PAO)
            {
                DefaultByteConverter.FromShort(timeStamp, ref bytes);
                DefaultByteConverter.FromInt(playerID, ref bytes);
                DefaultByteConverter.FromVector3(velocity, ref bytes);
                DefaultByteConverter.FromVector3(angularVelocity, ref bytes);
            }

            return bytes.ToArray();
        }

        public virtual int FromBytes(byte[] bytes, int idx)
        {
            netID = DefaultByteConverter.ToShort(bytes, ref idx);

            state = (PackageState)DefaultByteConverter.ToByte(bytes, ref idx);
            position = DefaultByteConverter.ToVector3(bytes, ref idx);
            rotation = DefaultByteConverter.ToQuaternion(bytes, ref idx);

            //Values are not provided for PAO packages
            if (state != PackageState.PAO)
            {
                timeStamp = DefaultByteConverter.ToShort(bytes, ref idx);
                playerID = DefaultByteConverter.ToInt(bytes, ref idx);
                velocity = DefaultByteConverter.ToVector3(bytes, ref idx);
                angularVelocity = DefaultByteConverter.ToVector3(bytes, ref idx);
            }

            return idx;
        }

        //public int? ByteCount
        //{
        //    get
        //    {
        //        if (state != PackageState.PAO)
        //        {
        //            return 61;
        //        }
        //        else
        //        {
        //            return 31;
        //        }
        //    }
        //}

        public override string ToString()
        {
            System.Text.StringBuilder t = new System.Text.StringBuilder("NetPackage\n");
            t.AppendLine(state.ToString());
            t.AppendLine(position.ToString());
            t.AppendLine(rotation.ToString());
            t.AppendLine(velocity.ToString());
            t.AppendLine(angularVelocity.ToString());

            return t.ToString();
        }

        #endregion

        /// <summary>
        /// DCO: Direct Controlled Object (Object is under direct user control, e.g. dragging)
        /// DAO: Directly Affected Object (Object collided with an DCO)
        /// PAO: Physically Affected Object (State of rest, set if an object falls into sleep)
        ///
        /// BYTE (8 states)
        /// </summary>
        public enum PackageState
        {
            DCO = 2,
            DAO = 1,
            PAO = 0,
        }
    }
}