
using System.Collections.Generic;
using Cmune.Realtime.Common.IO;
using Cmune.Util;

namespace Cmune.Realtime.Common
{
    public class SyncObject
    {
        /// <summary>
        /// 
        /// </summary>
        private SyncObject()
        {
            Id = 0;
            DeltaCode = 0;
            Data = new Dictionary<int, object>(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public SyncObject(int id, Dictionary<int, object> data)
        {
            Id = id;
            DeltaCode = 0;
            Data = data;// new Dictionary<int, object>(data.Count);

            foreach (var o in data)
            {
                if (o.Value != null)
                {
                    DeltaCode |= o.Key;
                    //Data[o.Key] = o.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            ToBytes(bytes);
            return bytes.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        public void ToBytes(List<byte> bytes)
        {
            bytes.AddRange(DefaultByteConverter.FromInt(DeltaCode));
            if (DeltaCode != 0)
            {
                bytes.AddRange(DefaultByteConverter.FromInt(Id));

                //it's very important to iterate through all 32 bit flags IN ORDER to ensure a correct de/serialization of the byte array
                for (int i = 0; i <= 32; i++)
                {
                    object v;
                    if (Data.TryGetValue(1 << i, out v))
                    {
                        RealtimeSerialization.ToBytes(true, ref bytes, v);
                        //CmuneDebug.Log("{0}|{1} {2} at bytes[{3}]", 1 << i, v, v.GetType(), bytes.Count);
                        //CmuneDebug.Log("{0}|{1} ({2}) - {3}", RealtimeSerialization.GetCmuneDataType(v.GetType()), v, v.GetType(), i + 1);
                    }
                }
            }
            //CmuneDebug.LogErrorFormat("SyncObject ToBytes decode flags {0} of {1}", _flags, bytes.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static SyncObject FromBytes(byte[] bytes, ref int idx)
        {
            SyncObject sync = new SyncObject();

            //get the delta mask to find out what data is inside the byte array (up to 32 data entries supported)
            sync.DeltaCode = DefaultByteConverter.ToInt(bytes, ref idx);
            //CmuneDebug.LogErrorFormat("SyncObject FromBytes decode flags {0} at index {1} of {2}", sync._flags, idx, bytes.Length);

            if (sync.DeltaCode != 0)
            {
                sync.Id = DefaultByteConverter.ToInt(bytes, ref idx);

                //it's very important to iterate through all 32 bit flags IN ORDER to ensure a correct de/serialization of the byte array
                for (int i = 0; i <= 32 && idx < bytes.Length && idx < int.MaxValue; i++)
                {
                    int flag = 1 << i;
                    //if we found a data entry
                    //CmuneDebug.LogErrorFormat("Has flags {0}: {1}", flag, (sync._flags & flag) != 0);
                    if ((sync.DeltaCode & flag) != 0)
                    {
                        byte cdt = bytes[idx++];
                        object o;
                        //deserialize the object
                        if (RealtimeSerialization.TryDecodeObject(bytes, ref idx, cdt, out o))
                        {
                            sync.Data[flag] = o;
                            //CmuneDebug.Log("{0}|{1} - {2}", cdt, o, i + 1);
                        }
                        else
                        {
                            CmuneDebug.LogError("Error deserializing Field with Bit Tag '{0}' and of CmuneDataType {1}", i, cdt);
                        }
                    }
                }
            }

            return sync;
        }

        public bool Contains(int deltaCode)
        {
            return (DeltaCode & deltaCode) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id ^ DeltaCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(obj, null))
            {
                SyncObject other = obj as SyncObject;
                if (other != null)
                {
                    if (this.DeltaCode != other.DeltaCode) return false;
                    if (this.DeltaCode != 0 && this.Id != other.Id) return false;

                    return Comparison.IsEqual(this.Data.Values, other.Data.Values);
                }
                else { return false; }
            }
            else { return false; }
        }

        #region Properties

        public int DeltaCode
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get { return DeltaCode == 0; }
        }

        public int Id
        {
            get;
            private set;
        }

        public readonly Dictionary<int, object> Data;

        #endregion
    }
}