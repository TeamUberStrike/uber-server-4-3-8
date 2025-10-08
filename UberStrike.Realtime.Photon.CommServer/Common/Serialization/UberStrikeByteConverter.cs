using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Synchronization;
using Cmune.Util;

namespace UberStrike.Realtime.Common.IO
{
    public class UberStrikeByteConverter : DefaultByteConverter
    {
        public UberStrikeByteConverter()
        {
            _dataTypes = new UberStrikeDataType();
        }

        public override byte GetCmuneDataType(System.Type type)
        {
            if (type == typeof(CharacterInfo)) return UberStrikeDataType.ActorInfo;
            else if (type == typeof(ShortVector3)) return UberStrikeDataType.ShortVector;
            else if (type == typeof(DamageEvent)) return UberStrikeDataType.DamageEvent;
            else if (type == typeof(GameMetaData)) return UberStrikeDataType.GameData;
            else if (type == typeof(ArmorInfo)) return UberStrikeDataType.Armor;
            else if (type == typeof(StatsInfo)) return UberStrikeDataType.Stats;
            else if (type == typeof(EndOfMatchData)) return UberStrikeDataType.EndOfMatch;
            else if (type == typeof(WeaponInfo)) return UberStrikeDataType.Weapons;
            else if (type == typeof(StatsCollection)) return UberStrikeDataType.Statistics;

            else if (typeof(ICollection<RoomMetaData>).IsAssignableFrom(type)) return UberStrikeDataType.Array_GameData;
            else
            {
                return base.GetCmuneDataType(type);
            }
        }

        protected override bool FromObject(object o, byte type, bool encodeType, ref List<byte> bytes)
        {
            //CmuneDebug.LogFormat("(PP) Encode Object of type {0} as {1}", o.GetType(), type);

            switch (type)
            {
                case UberStrikeDataType.ActorInfo:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.ActorInfo);
                        SyncObjectBuilder.GetSyncData((CharacterInfo)o, true).ToBytes(bytes);
                        break;
                    }
                case UberStrikeDataType.ShortVector:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.ShortVector);
                        bytes.AddRange(((ShortVector3)o).GetBytes());
                        break;
                    }
                case UberStrikeDataType.DamageEvent:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.DamageEvent);
                        bytes.AddRange(((DamageEvent)o).GetBytes());
                        break;
                    }
                case UberStrikeDataType.Armor:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.Armor);
                        bytes.AddRange(((ArmorInfo)o).GetBytes());
                        break;
                    }
                case UberStrikeDataType.Stats:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.Stats);
                        bytes.AddRange(((StatsInfo)o).GetBytes());
                        break;
                    }
                case UberStrikeDataType.EndOfMatch:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.EndOfMatch);
                        bytes.AddRange(((EndOfMatchData)o).GetBytes());
                        break;
                    }
                case UberStrikeDataType.Weapons:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.Weapons);
                        bytes.AddRange(((WeaponInfo)o).GetBytes());
                        break;
                    }

                case UberStrikeDataType.GameData:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.GameData);
                        bytes.AddRange(((GameMetaData)o).GetBytes());
                        break;
                    }

                case UberStrikeDataType.Statistics:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.Statistics);
                        bytes.AddRange(((StatsCollection)o).GetBytes());
                        break;
                    }

                case UberStrikeDataType.Array_GameData:
                    {
                        if (encodeType) bytes.Add(UberStrikeDataType.Array_GameData);
                        FromRoomDataCollection((ICollection<RoomMetaData>)o, ref bytes);
                        break;
                    }

                default:
                    {
                        return base.FromObject(o, type, encodeType, ref bytes);
                    }
            }

            return true;
        }

        public override object ToObject(byte[] bytes, byte type, ref int i)
        {
            object obj = null;

            //CmuneDebug.LogFormat("(PP) Decode Object of type {0}", type);
            if (i < bytes.Length)
            {
                switch (type)
                {
                    case UberStrikeDataType.Array_GameData:
                        {
                            obj = ToRoomDataCollection(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.ShortVector:
                        {
                            obj = new ShortVector3(bytes, ref i);
                            break;
                        }

                    case UberStrikeDataType.Statistics:
                        {
                            obj = new StatsCollection(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.ActorInfo:
                        {
                            obj = new CharacterInfo(SyncObject.FromBytes(bytes, ref i));
                            break;
                        }

                    case UberStrikeDataType.DamageEvent:
                        {
                            obj = DamageEvent.FromBytes(bytes, ref i);
                            break;
                        }

                    case UberStrikeDataType.Armor:
                        {
                            obj = new ArmorInfo(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.Stats:
                        {
                            obj = new StatsInfo(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.EndOfMatch:
                        {
                            obj = new EndOfMatchData(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.Weapons:
                        {
                            obj = new WeaponInfo(bytes, ref i);
                            break;
                        }
                    case UberStrikeDataType.GameData:
                        {
                            obj = new GameMetaData(bytes, ref i);
                            break;
                        }
                    default:
                        {
                            obj = base.ToObject(bytes, type, ref  i);
                            break;
                        }
                }
            }

            return obj;
        }

        #region COLLECTIONS

        public void FromRoomDataCollection(ICollection<RoomMetaData> array, ref List<byte> bytes)
        {
            if (array.Count < short.MaxValue)
                DefaultByteConverter.FromShort((short)array.Count, ref bytes);
            else
                throw new CmuneException(string.Format("Trying to encode a RoomMetaData collection of length {0}, but only marked as SMALL (up to {1} elements))", array.Count, short.MaxValue));

            foreach (RoomMetaData i in array)
            {
                bytes.Add(GetCmuneDataType(i.GetType()));

                bytes.AddRange(i.GetBytes());
            }
        }

        public List<RoomMetaData> ToRoomDataCollection(byte[] bytes, ref int i)
        {
            int len = (int)DefaultByteConverter.ToShort(bytes, ref i);

            List<RoomMetaData> list = new List<RoomMetaData>(len);

            for (int j = 0; j < len; j++)
            {
                byte type = bytes[i++];
                switch (type)
                {
                    case CmuneDataType.RoomData:
                        list.Add(new RoomMetaData(bytes, ref i));
                        break;

                    case UberStrikeDataType.GameData:
                        list.Add(new GameMetaData(bytes, ref i));
                        break;
                }
            }

            return list;
        }

        #endregion
    }
}