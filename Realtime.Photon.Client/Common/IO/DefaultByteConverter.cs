
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Cmune.Util;
using Cmune.Realtime.Common.Synchronization;

namespace Cmune.Realtime.Common.IO
{
    public class DefaultByteConverter : IByteConverter
    {
        protected CmuneDataType _dataTypes = new CmuneDataType();

        public bool IsTypeSupported(System.Type type)
        {
            return GetCmuneDataType(type) != CmuneDataType.None;
        }

        protected bool IsCmuneTypeDefined(byte type)
        {
            return _dataTypes.IsDefined(type);
        }

        public virtual byte GetCmuneDataType(Type o)
        {
            if (o == typeof(Byte)) return CmuneDataType.Byte;
            else if (o == typeof(SByte)) return CmuneDataType.SByte;
            else if (o == typeof(Int32)) return CmuneDataType.Int32;
            else if (o == typeof(Int16)) return CmuneDataType.Int16;
            else if (o == typeof(UInt16)) return CmuneDataType.UInt16;
            else if (o == typeof(Int64)) return CmuneDataType.Long;
            else if (o == typeof(Single)) return CmuneDataType.Float;
            else if (o == typeof(String)) return CmuneDataType.String;
            else if (o == typeof(Boolean)) return CmuneDataType.Bool;

            else if (o == typeof(Vector3)) return CmuneDataType.Vector3;
            else if (o == typeof(Quaternion)) return CmuneDataType.Quaternion;
            else if (o == typeof(Color)) return CmuneDataType.Color;

            else if (o == typeof(AssetType)) return CmuneDataType.AssetType;
            else if (o == typeof(SyncObject)) return CmuneDataType.SyncObject;
            else if (o == typeof(CommActorInfo)) return CmuneDataType.CommActorInfo;
            //else if (o == typeof(ActorInfo)) return CmuneDataType.USERINFO;
            else if (o == typeof(NetworkPackage)) return CmuneDataType.PhysicsPack;
            else if (o == typeof(RoomMetaData)) return CmuneDataType.RoomData;
            else if (o == typeof(CmuneTransform)) return CmuneDataType.Transform;
            else if (o == typeof(CmuneRoomID)) return CmuneDataType.RoomId;
            else if (o == typeof(ServerLoadData)) return CmuneDataType.PerformanceData;

            else if (o.IsArray)
            {
                if (o.GetElementType() == typeof(Byte)) return CmuneDataType.Array_Byte;
                else if (o.GetElementType() == typeof(Int32)) return CmuneDataType.Array_Int;
                else if (o.GetElementType() == typeof(Int16)) return CmuneDataType.Array_Short;
                else if (o.GetElementType() == typeof(Single)) return CmuneDataType.Array_Float;
                else if (o.GetElementType() == typeof(String)) return CmuneDataType.Array_String;

                else if (o.GetElementType() == typeof(SyncObject)) return CmuneDataType.Array_SyncObject;
                //else if (o.GetElementType() == typeof(CommActorInfo)) return CmuneDataType.ARRAY_COMMUSERINFO;
                else if (o.GetElementType() == typeof(CmuneRoomID)) return CmuneDataType.Array_RoomId;

                else if (o.GetElementType() == typeof(Quaternion)) return CmuneDataType.Array_Quaternion;
                else if (o.GetElementType() == typeof(Vector3)) return CmuneDataType.Array_Vector3;
                else
                {
                    return CmuneDataType.None;
                }
            }
            else if (o.IsGenericType)
            {
                if (typeof(ICollection<Byte>).IsAssignableFrom(o)) return CmuneDataType.Array_Byte;
                else if (typeof(ICollection<Int32>).IsAssignableFrom(o)) return CmuneDataType.Array_Int;
                else if (typeof(ICollection<Int16>).IsAssignableFrom(o)) return CmuneDataType.Array_Short;
                else if (typeof(ICollection<UInt16>).IsAssignableFrom(o)) return CmuneDataType.Array_UShort;
                else if (typeof(ICollection<Single>).IsAssignableFrom(o)) return CmuneDataType.Array_Float;
                else if (typeof(ICollection<String>).IsAssignableFrom(o)) return CmuneDataType.Array_String;

                else if (typeof(ICollection<SyncObject>).IsAssignableFrom(o)) return CmuneDataType.Array_SyncObject;
                //else if (typeof(ICollection<CommActorInfo>).IsAssignableFrom(o)) return CmuneDataType.ARRAY_COMMUSERINFO;
                else if (typeof(ICollection<CmuneRoomID>).IsAssignableFrom(o)) return CmuneDataType.Array_RoomId;

                else if (typeof(ICollection<Quaternion>).IsAssignableFrom(o)) return CmuneDataType.Array_Quaternion;
                else if (typeof(ICollection<Vector3>).IsAssignableFrom(o)) return CmuneDataType.Array_Vector3;
                else
                {
                    return CmuneDataType.None;
                }
            }
            else
            {
                return CmuneDataType.None;
            }
        }

        //private static bool IsArrayOfType(Type coll, Type type)
        //{
        //    if (coll.GetElementType() == type)
        //        return true;
        //    else
        //        return false;
        //}

        //private static bool IsCollectionOfType<T>(Type coll, Type type)
        //{
        //    return typeof(ICollection<T>).IsAssignableFrom(coll);
        //}

        public void FromObject(object o, ref List<byte> bytes)
        {
            FromObject(o, true, ref bytes);
        }

        public bool FromObject(object o, bool encodeType, ref List<byte> bytes)
        {
            if (o != null)
            {
                byte type = GetCmuneDataType(o.GetType());

                return FromObject(o, type, encodeType, ref bytes);
            }
            else return false;
        }

        public bool TrySerializeObject<T>(T o, bool encodeType, ref List<byte> bytes)
        {
            byte type = GetCmuneDataType(typeof(T));

            return FromObject(o, type, encodeType, ref bytes);
        }

        protected virtual bool FromObject(object o, byte type, bool encodeType, ref List<byte> bytes)
        {
            //CmuneDebug.LogFormat("Encode Object of type {0}", type);

            switch (type)
            {
                case CmuneDataType.Int32:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Int32);
                        FromInt((int)o, ref bytes);
                        break;
                    }
                case CmuneDataType.UInt16:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.UInt16);
                        FromUShort((ushort)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Int16:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Int16);
                        FromShort((short)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Float:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Float);
                        FromFloat((float)o, ref bytes);
                        break;
                    }
                case CmuneDataType.SByte:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.SByte);
                        FromSByte((sbyte)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Byte:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Byte);
                        FromByte((byte)o, ref bytes);
                        break;
                    }
                case CmuneDataType.String:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.String);
                        FromString((string)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Long:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Long);
                        FromLong((System.Int64)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Vector3:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Vector3);
                        FromVector3((Vector3)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Quaternion:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Quaternion);
                        FromQuaternion((Quaternion)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Color:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Color);
                        FromColor((Color)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Bool:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Bool);
                        FromBool((bool)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_Byte:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Byte);
                        FromByteCollection((ICollection<byte>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_Short:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Short);
                        FromShortCollection((ICollection<short>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_UShort:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_UShort);
                        FromUShortCollection((ICollection<ushort>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_Int:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Int);
                        FromIntCollection((ICollection<int>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_Float:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Float);
                        FromFloatCollection((ICollection<float>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_String:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_String);
                        FromCollectionString((ICollection<string>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.SyncObject:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.SyncObject);
                        ((SyncObject)o).ToBytes(bytes);
                        break;
                    }
                case CmuneDataType.Array_SyncObject:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_SyncObject);
                        FromSyncObjectCollection((ICollection<SyncObject>)o, ref bytes);
                        break;
                    }

                //case CmuneDataType.ARRAY_USERINFO:
                //    {
                //        if (encodeType) bytes.Add(CmuneDataType.ARRAY_USERINFO);
                //        FromUserInfoCollection((ICollection<ActorInfo>)o, ref bytes);
                //        break;
                //    }
                //case CmuneDataType.ARRAY_COMMUSERINFO:
                //    {
                //        if (encodeType) bytes.Add(CmuneDataType.ARRAY_COMMUSERINFO);
                //        FromCommUserInfoCollection((ICollection<CommActorInfo>)o, ref bytes);
                //        break;
                //    }
                case CmuneDataType.Array_RoomId:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_RoomId);
                        FromRoomIDCollection((ICollection<CmuneRoomID>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.Array_Vector3:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Vector3);
                        FromVector3Collection((ICollection<Vector3>)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Array_Quaternion:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Array_Quaternion);
                        FromQuaternionCollection((ICollection<Quaternion>)o, ref bytes);
                        break;
                    }

                case CmuneDataType.CommActorInfo:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.CommActorInfo);
                        SyncObjectBuilder.GetSyncData((CommActorInfo)o, true).ToBytes(bytes);
                        break;
                    }
                case CmuneDataType.RoomId:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.RoomId);
                        bytes.AddRange(((CmuneRoomID)o).GetBytes());
                        break;
                    }
                case CmuneDataType.PerformanceData:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.PerformanceData);
                        bytes.AddRange(((ServerLoadData)o).GetBytes());
                        break;
                    }
                case CmuneDataType.PhysicsPack:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.PhysicsPack);
                        bytes.AddRange(((NetworkPackage)o).GetBytes());
                        break;
                    }
                case CmuneDataType.RoomData:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.RoomData);
                        bytes.AddRange(((RoomMetaData)o).GetBytes());
                        break;
                    }
                case CmuneDataType.AssetType:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.AssetType);
                        DefaultByteConverter.FromByte((byte)(int)o, ref bytes);
                        break;
                    }
                case CmuneDataType.Transform:
                    {
                        if (encodeType) bytes.Add(CmuneDataType.Transform);
                        bytes.AddRange(((CmuneTransform)o).GetBytes());
                        break;
                    }
                default:
                    {
                        if (type == 0)
                            CmuneDebug.LogWarning("Not supported Type '{0}' ", o.GetType());
                        else
                            CmuneDebug.LogError("TYPE '{0}' NOT IMPLEMENTED for '{1}'", type, o.GetType());
                        return false;
                    }
            }

            return true;
        }

        public bool TryDecodeObject(byte[] bytes, System.Type type, ref int i, out object obj)
        {
            byte t = GetCmuneDataType(type);
            return TryDecodeObject(bytes, t, ref i, out obj);
        }

        public bool TryDecodeObject(byte[] bytes, byte type, ref int i, out object obj)
        {
            if (type != CmuneDataType.None)
            {
                obj = ToObject(bytes, type, ref i);
                return obj != null && i < int.MaxValue;
            }
            else
            {
                i = int.MaxValue;
                obj = null;
                return false;
            }
        }

        public object ToObject(byte[] bytes, ref int i)
        {
            if (i >= 0 && i < bytes.Length)
            {
                if (IsCmuneTypeDefined(bytes[i]))
                {
                    byte t = bytes[i];
                    i++;
                    return ToObject(bytes, t, ref i);
                }
                else
                {
                    CmuneDebug.LogError("Decode Object failed because found unrecognized datatype {0}", bytes[i]);
                    i = int.MaxValue;
                    return null;
                }
            }
            else
            {
                CmuneDebug.LogError("Trying to call ToObject but index at {0} of byte[]({1}) ", i, bytes.Length);
                i = int.MaxValue;
                return null;
            }
        }

        public virtual object ToObject(byte[] bytes, byte type, ref int i)
        {
            if (i < bytes.Length)
            {
                switch (type)
                {
                    //ATOMIC TYPES
                    case CmuneDataType.Int32:
                        {
                            return ToInt(bytes, ref i);
                        }
                    case CmuneDataType.Int16:
                        {
                            return ToShort(bytes, ref i);
                        }
                    case CmuneDataType.UInt16:
                        {
                            return ToUShort(bytes, ref i);
                        }
                    case CmuneDataType.Float:
                        {
                            return ToFloat(bytes, ref i);
                        }
                    case CmuneDataType.SByte:
                        {
                            return ToSByte(bytes, ref i);
                        }
                    case CmuneDataType.Byte:
                        {
                            return ToByte(bytes, ref i);
                        }
                    case CmuneDataType.String:
                        {
                            return ToString(bytes, ref i);
                        }
                    case CmuneDataType.Long:
                        {
                            return ToLong(bytes, ref i);
                        }
                    case CmuneDataType.Vector3:
                        {
                            return ToVector3(bytes, ref i);
                        }
                    case CmuneDataType.Quaternion:
                        {
                            return ToQuaternion(bytes, ref i);
                        }
                    case CmuneDataType.Color:
                        {
                            return ToColor(bytes, ref i);
                        }
                    case CmuneDataType.Bool:
                        {
                            return ToBool(bytes, ref i);
                        }

                    //ARRAY TYPES
                    case CmuneDataType.Array_Short:
                        {
                            return ToShortCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_UShort:
                        {
                            return ToUShortCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_Int:
                        {
                            return ToIntCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_Float:
                        {
                            return ToFloatCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_Byte:
                        {
                            return ToByteCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_String:
                        {
                            return ToCollectionString(bytes, ref i);
                        }
                    //case CmuneDataType.ARRAY_USERINFO:
                    //    {
                    //        return ToUserInfoCollection(bytes, ref i);
                    //    }
                    //case CmuneDataType.ARRAY_COMMUSERINFO:
                    //    {
                    //        return ToCommUserInfoCollection(bytes, ref i);
                    //    }
                    case CmuneDataType.Array_RoomId:
                        {
                            return ToRoomIDCollection(bytes, ref i);
                        }
                    case CmuneDataType.Array_Vector3:
                        {
                            return ToVector3Collection(bytes, ref i);
                        }
                    case CmuneDataType.Array_Quaternion:
                        {
                            return ToQuaternionCollection(bytes, ref i);
                        }

                    //STRUCTURED DATA (IByteArray Implemetations)

                    case CmuneDataType.SyncObject:
                        {
                            return SyncObject.FromBytes(bytes, ref i);//((SyncObject)o).ToBytes(bytes);)
                        }
                    case CmuneDataType.Array_SyncObject:
                        {
                            return ToSyncObjectCollection(bytes, ref i);//((SyncObject)o).ToBytes(bytes);)
                        }

                    //case CmuneDataType.USERINFO:
                    //    {
                    //        return new ActorInfo(bytes, ref i);
                    //    }
                    case CmuneDataType.CommActorInfo:
                        {
                            return new CommActorInfo(SyncObject.FromBytes(bytes, ref i));
                        }
                    case CmuneDataType.RoomId:
                        {
                            return new CmuneRoomID(bytes, ref i);
                        }
                    case CmuneDataType.PerformanceData:
                        {
                            return new ServerLoadData(bytes, ref i);
                        }
                    case CmuneDataType.PhysicsPack:
                        {
                            return new NetworkPackage(bytes, ref i);
                        }
                    case CmuneDataType.RoomData:
                        {
                            return new RoomMetaData(bytes, ref i);
                        }
                    case CmuneDataType.AssetType:
                        {
                            return (AssetType)(int)DefaultByteConverter.ToByte(bytes, ref i);
                        }
                    case CmuneDataType.Transform:
                        {
                            return new CmuneTransform(bytes, ref i);
                        }

                    //FINALS
                    case CmuneDataType.None:
                        {
                            CmuneDebug.LogError("Bad call of ToObject with CmuneType '{0}' at index {1} and byte[] of Length {2}", type, i, bytes.Length);
                            i = int.MaxValue;
                            return null;
                        }
                    default:
                        {
                            CmuneDebug.LogError("Cmune Type {0} not implemented!", type);
                            i = int.MaxValue;
                            return null;
                        }
                }
            }
            else
            {
                return null;
            }
        }

        #region COLLECTIONS

        public static void FromVector3Collection(ICollection<Vector3> array, ref List<byte> bytes)
        {
            if (array.Count < byte.MaxValue)
            {
                bytes.Add((byte)array.Count);

                foreach (Vector3 i in array)
                {
                    FromVector3(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a ushort collection of length {0}, but only marked as SMALL (up to 255 elements))", array.Count));
            }
        }

        public static List<Vector3> ToVector3Collection(byte[] bytes, ref int i)
        {
            int len = (int)bytes[i++];

            List<Vector3> list = new List<Vector3>(len);

            for (int j = 0; j < len; j++)
            {
                list.Add(ToVector3(bytes, ref i));
            }

            return list;
        }

        public static void FromQuaternionCollection(ICollection<Quaternion> array, ref List<byte> bytes)
        {
            if (array.Count < byte.MaxValue)
            {
                bytes.Add((byte)array.Count);

                foreach (Quaternion i in array)
                {
                    FromQuaternion(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a ushort collection of length {0}, but only marked as SMALL (up to 255 elements))", array.Count));
            }
        }

        public static List<Quaternion> ToQuaternionCollection(byte[] bytes, ref int i)
        {
            int len = (int)bytes[i++];

            List<Quaternion> list = new List<Quaternion>(len);

            for (int j = 0; j < len; j++)
            {
                list.Add(ToQuaternion(bytes, ref i));
            }

            return list;
        }

        public static void FromUShortCollection(ICollection<ushort> array, ref List<byte> bytes)
        {
            if (array.Count < byte.MaxValue)
            {
                bytes.Add((byte)array.Count);

                foreach (ushort i in array)
                {
                    FromUShort(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a ushort collection of length {0}, but only marked as SMALL (up to 255 elements))", array.Count));
            }
        }

        public static List<ushort> ToUShortCollection(byte[] bytes, ref int i)
        {
            int len = (int)bytes[i++];

            if (len >= 0 && i >= 0 && bytes.Length >= i + len * 2)
            {
                List<ushort> list = new List<ushort>(len);

                for (int j = 0; j < len; j++)
                {
                    list.Add(ToUShort(bytes, ref i));
                }

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an short[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<ushort>(0);
            }
        }

        public static void FromShortCollection(ICollection<short> array, ref List<byte> bytes)
        {
            if (array.Count < byte.MaxValue)
            {
                bytes.Add((byte)array.Count);

                foreach (short i in array)
                {
                    FromShort(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a short collection of length {0}, but only marked as SMALL (up to 255 elements))", array.Count));
            }
        }

        public static List<short> ToShortCollection(byte[] bytes, ref int i)
        {
            int len = (int)bytes[i++];

            if (len >= 0 && i >= 0 && bytes.Length >= i + len * 2)
            {
                List<short> list = new List<short>(len);

                for (int j = 0; j < len; j++)
                    list.Add(ToShort(bytes, ref i));

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an short[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<short>(0);
            }
        }

        public static void FromIntCollection(ICollection<int> array, ref List<byte> bytes)
        {
            if (array.Count < short.MaxValue)
            {
                FromShort((short)array.Count, ref bytes);

                foreach (int i in array)
                {
                    FromInt(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a int collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, short.MaxValue));
            }
        }

        public static List<int> ToIntCollection(byte[] bytes, ref int i)
        {
            int len = ToShort(bytes, ref i);

            if (len >= 0 && i >= 0 && bytes.Length >= i + len * 4)
            {
                List<int> list = new List<int>(len);

                for (int j = 0; j < len; j++)
                    list.Add(ToInt(bytes, ref i));

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an int[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<int>(0);
            }
        }

        public virtual void FromRoomIDCollection(ICollection<CmuneRoomID> array, ref List<byte> bytes)
        {
            if (array.Count < short.MaxValue)
            {
                DefaultByteConverter.FromShort((short)array.Count, ref bytes);

                foreach (CmuneRoomID i in array)
                {
                    bytes.AddRange(i.GetBytes());
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a CmuneRoomID collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, short.MaxValue));
            }
        }

        public virtual List<CmuneRoomID> ToRoomIDCollection(byte[] bytes, ref int i)
        {
            int len = DefaultByteConverter.ToShort(bytes, ref i);

            if (len >= 0 && i >= 0 && bytes.Length >= i + len * 12)
            {
                List<CmuneRoomID> list = new List<CmuneRoomID>(len);

                for (int j = 0; j < len; j++)
                {
                    list.Add(new CmuneRoomID(bytes, ref i));
                }

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an CmuneRoomID[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<CmuneRoomID>(0);
            }
        }

        //public virtual void FromCommUserInfoCollection(ICollection<CommActorInfo> array, ref List<byte> bytes)
        //{
        //    if (array.Count < short.MaxValue)
        //    {
        //        FromShort((short)array.Count, ref bytes);

        //        foreach (CommActorInfo i in array)
        //        {
        //            bytes.AddRange(i.GetBytes());
        //        }
        //    }
        //    else
        //    {
        //        throw new CmuneException(string.Format("Trying to encode a ActorInfo collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, short.MaxValue));
        //    }
        //}

        //public virtual List<CommActorInfo> ToCommUserInfoCollection(byte[] bytes, ref int i)
        //{
        //    int len = ToShort(bytes, ref i);

        //    if (len >= 0 && bytes.Length >= i + len * 4)
        //    {
        //        List<CommActorInfo> list = new List<CommActorInfo>(len);

        //        for (int j = 0; j < len; j++)
        //        {
        //            list.Add(new CommActorInfo(bytes, ref i));
        //        }

        //        return list;
        //    }
        //    else
        //    {
        //        CmuneDebug.LogErrorFormat("Trying to decode an CommActorInfo[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
        //        i = int.MaxValue;
        //        return new List<CommActorInfo>(0);
        //    }
        //}

        public virtual void FromSyncObjectCollection(ICollection<SyncObject> array, ref List<byte> bytes)
        {
            if (array.Count < short.MaxValue)
            {
                FromShort((short)array.Count, ref bytes);

                foreach (SyncObject i in array)
                {
                    i.ToBytes(bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a ActorInfo collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, short.MaxValue));
            }
        }

        public virtual List<SyncObject> ToSyncObjectCollection(byte[] bytes, ref int i)
        {
            int len = ToShort(bytes, ref i);

            if (len >= 0 && bytes.Length >= i + len * 4)
            {
                List<SyncObject> list = new List<SyncObject>(len);

                for (int j = 0; j < len; j++)
                {
                    list.Add(SyncObject.FromBytes(bytes, ref i));
                }

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an SyncObject[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<SyncObject>(0);
            }
        }

        public static void FromFloatCollection(ICollection<float> array, ref List<byte> bytes)
        {
            if (array.Count < byte.MaxValue)
            {
                bytes.Add((byte)array.Count);

                foreach (float i in array)
                {
                    FromFloat(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a float collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, byte.MaxValue));
            }
        }

        public static List<float> ToFloatCollection(byte[] bytes, ref int i)
        {
            int len = (int)bytes[i++];

            if (len >= 0 && i >= 0 && bytes.Length >= i + len * 4)
            {
                List<float> list = new List<float>(len);

                for (int j = 0; j < len; j++)
                    list.Add(ToFloat(bytes, ref i));

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an float[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<float>(0);
            }
        }

        public static void FromByteCollection(ICollection<byte> array, ref List<byte> bytes)
        {
            if (array.Count < int.MaxValue)
            {
                //save the index of where to put the length of the array
                FromInt(array.Count, ref bytes);
                bytes.AddRange(array);
            }
            else
            {
                throw new CmuneException("Trying to encode a byte collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, int.MaxValue);
            }
        }

        public static List<byte> ToByteCollection(byte[] bytes, ref int i)
        {
            int len = ToInt(bytes, ref i);

            if (len >= 0 && i >= 0 && bytes.Length >= i + len)
            {
                List<byte> list = new List<byte>(len);
                for (int j = 0; j < len; j++)
                    list.Add(bytes[i + j]);
                i += len;
                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an byte[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<byte>(0);
            }
        }

        public static void FromCollectionString(ICollection<string> array, ref List<byte> bytes)
        {
            if (array.Count < 255)
            {
                bytes.Add((byte)array.Count);

                foreach (string i in array)
                {
                    FromString(i, ref bytes);
                }
            }
            else
            {
                throw new CmuneException(string.Format("Trying to encode a string collection of length {0}, but only marked as SMALL (up to 255 elements))", array.Count));
            }
        }

        public static List<string> ToCollectionString(byte[] bytes, ref int i)
        {
            int len = bytes[i++];

            if (len >= 0 && i >= 0 && bytes.Length >= i + len)
            {
                List<string> list = new List<string>(len);

                for (int j = 0; j < len; j++)
                    list.Add(ToString(bytes, ref i));

                return list;
            }
            else
            {
                CmuneDebug.LogError("Trying to decode an string[] of length {0} at index {1} of byte[]({2}) ", len, i, bytes.Length);
                i = int.MaxValue;
                return new List<string>(0);
            }
        }

        #endregion

        #region LONG

        public static void FromLong(long l, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(l)));
        }

        public static long ToLong(byte[] bytes, ref int i)
        {
            if (i + 8 <= bytes.Length)
            {
                int j = i; i += 8;
                return BitConverter.ToInt64(CheckEndian(bytes, j, 8), j);
            }
            else
            {
                i = int.MaxValue;
                return 0;
            }
        }

        #endregion

        #region FLOATS

        public static void FromFloat(float f, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(f)));
        }

        public static float ToFloat(byte[] bytes, ref int i)
        {
            if (i + 4 <= bytes.Length)
            {
                int j = i; i += 4;
                return BitConverter.ToSingle(CheckEndian(bytes, j, 4), j);
            }
            else
            {
                i = int.MaxValue;
                return 0;
            }
        }

        #endregion

        #region SHORT

        public static byte[] FromShort(short f)
        {
            return CheckEndian(BitConverter.GetBytes(f));
        }

        public static void FromShort(short f, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(f)));
        }

        public static short ToShort(byte[] bytes, ref int i)
        {
            int j = i; i += 2;
            if (j >= 0 && j + 2 <= bytes.Length)
            {
                return BitConverter.ToInt16(CheckEndian(bytes, j, 2), j);
            }
            else
            {
                i = int.MaxValue;
                return 0;
            }
        }

        public static void FromUShort(ushort f, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(f)));
        }

        public static ushort ToUShort(byte[] bytes, ref int i)
        {
            int j = i; i += 2;
            if (j >= 0 && j + 2 <= bytes.Length)
            {
                return BitConverter.ToUInt16(CheckEndian(bytes, j, 2), j);
            }
            else
            {
                i = int.MaxValue;
                return 0;
            }
        }

        #endregion

        #region INTS

        public static void FromInt(int f, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(f)));
        }

        public static byte[] FromInt(int f)
        {
            return CheckEndian(BitConverter.GetBytes(f));
        }

        public static int ToInt(byte[] bytes, ref int i)
        {
            int j = i; i += 4;
            if (j >= 0 && j + 4 <= bytes.Length)
            {
                return BitConverter.ToInt32(CheckEndian(bytes, j, 4), j);
            }
            else
            {
                i = int.MaxValue;
                return 0;
            }
        }

        #endregion

        #region BOOLS

        public static void FromBool(bool b, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(b)));
        }
        public static bool ToBool(byte[] bytes, ref int i)
        {
            int j = i; i += 1;
            return BitConverter.ToBoolean(CheckEndian(bytes, j, 1), j);
        }

        #endregion

        #region BYTES

        public static void FromSByte(sbyte sb, ref List<byte> bytes)
        {
            bytes.Add((byte)((int)sb + sbyte.MaxValue));
        }

        public static sbyte ToSByte(byte[] bytes, ref int i)
        {
            return (sbyte)(bytes[i++] - sbyte.MaxValue);
        }

        public static void FromByte(byte b, ref List<byte> bytes)
        {
            bytes.Add(b);
        }

        public static byte ToByte(byte[] bytes, ref int i)
        {
            return bytes[i++];
        }

        #endregion

        #region CHAR

        public static void FromChar(Char c, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(c)));
        }

        public static Char ToChar(byte[] bytes, ref  int i)
        {
            int j = i;
            i += 2;
            return BitConverter.ToChar(CheckEndian(bytes, j, 2), j);
        }

        #endregion

        #region STRINGS

        public static void FromString(string s, ref List<byte> bytes)
        {
            FromString(s, ref bytes, false);
        }
        public static void FromString(string s, ref List<byte> bytes, bool small)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (small) bytes.Add((byte)0);
                else bytes.AddRange(BitConverter.GetBytes((short)0));
            }
            else
            {
                int maxLength = small ? byte.MaxValue >> 1 : short.MaxValue >> 1;
                if (s.Length > maxLength) s = s.Substring(0, s.Length - maxLength);

                if (small) bytes.Add((byte)s.Length);
                else bytes.AddRange(CheckEndian(BitConverter.GetBytes((short)s.Length)));

                bytes.AddRange(Encoding.Unicode.GetBytes(s));
            }
        }

        public static string ToString(byte[] bytes, ref int i)
        {
            return ToString(bytes, ref i, false);
        }
        public static string ToString(byte[] bytes, ref int i, bool small)
        {
            string ret = string.Empty;

            int len = -1;
            if (small)
            {
                if (i >= 0 && bytes.Length >= i + 1)
                {
                    len = bytes[i];
                    i += 1;
                }
            }
            else
            {
                if (i >= 0 && bytes.Length >= i + 2)
                {
                    len = BitConverter.ToInt16(CheckEndian(bytes, i, 2), i);
                    i += 2;
                }
            }

            if (len >= 0 && bytes.Length >= i + len * 2)
            {
                if (len > 0)
                {
                    ret = Encoding.Unicode.GetString(bytes, i, len * 2);
                    i += len * 2;
                }
            }
            else
            {
                if (CmuneDebug.IsWarningEnabled)
                    CmuneDebug.LogWarning("Error in ToString! Array size {0} not long enough for {1} bytes, starting from {2}", bytes.Length, len * 2, i);
                i = int.MaxValue;
            }

            return ret;
        }
        #endregion

        #region QUATERNION

        public static void FromQuaternion(Quaternion q, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(q[0])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(q[1])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(q[2])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(q[3])));
        }

        public static Quaternion ToQuaternion(byte[] bytes, ref int i)
        {
            Quaternion q = new Quaternion(
                BitConverter.ToSingle(CheckEndian(bytes, i, 4), i),
                BitConverter.ToSingle(CheckEndian(bytes, i + 4, 4), i + 4),
                BitConverter.ToSingle(CheckEndian(bytes, i + 8, 4), i + 8),
                BitConverter.ToSingle(CheckEndian(bytes, i + 12, 4), i + 12));
            i += 16;
            return q;
        }

        #endregion

        #region VECTOR3

        public static void FromVector3(Vector3 v, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[0])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[1])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[2])));
        }

        public static Vector3 ToVector3(byte[] bytes, ref int i)
        {
            Vector3 v = new Vector3(
                BitConverter.ToSingle(CheckEndian(bytes, i, 4), i),
                BitConverter.ToSingle(CheckEndian(bytes, i + 4, 4), i + 4),
                BitConverter.ToSingle(CheckEndian(bytes, i + 8, 4), i + 8));
            i += 12;
            return v;
        }

        #endregion

        #region COLOR

        public static void FromColor(Color v, ref List<byte> bytes)
        {
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[0])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[1])));
            bytes.AddRange(CheckEndian(BitConverter.GetBytes(v[2])));
        }

        public static Color ToColor(byte[] bytes, ref int i)
        {
            Color v = new Color(
                BitConverter.ToSingle(CheckEndian(bytes, i, 4), i),
                BitConverter.ToSingle(CheckEndian(bytes, i + 4, 4), i + 4),
                BitConverter.ToSingle(CheckEndian(bytes, i + 8, 4), i + 8));
            i += 12;
            return v;
        }

        #endregion

        #region -- Endian Checking Routines

        private static byte[] CheckEndian(byte[] bytes, int start = 0, int length = -1)
        {
            if (BitConverter.IsLittleEndian)
                return bytes;
            else
                return ReverseArray(bytes, start, length);
        }

        public static byte[] ReverseArray(byte[] bytes, int start, int length)
        {
            if (length < 0 || length > 1)
            {
                //keep the start and length inside the bounds of the array
                start = Mathf.Clamp(start, 0, bytes.Length);
                length = length > 0 ? Mathf.Clamp(length, 0, bytes.Length - start) : bytes.Length - start;
                int end = start + length;
                int len = length >> 1;

                byte t;
                for (int i = 0; i < len; i++)
                {
                    t = bytes[start + i];
                    bytes[start + i] = bytes[end - i - 1];
                    bytes[end - i - 1] = t;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Reverse the bits in a byte
        /// </summary>
        /// <param name="inByte">byte</param>
        /// <returns>byte</returns>
        public static byte ReverseByte(byte inByte)
        {
            byte result = 0x00;
            byte mask = 0x00;

            for (mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                result >>= 1;
                byte tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00) result |= 0x80;
            }
            return (result);
        }

        #endregion
    }
}