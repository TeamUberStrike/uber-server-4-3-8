using System.Collections.Generic;
using System.Collections;
using Cmune.Util;

namespace Cmune.Realtime.Common.IO
{
    public class RealtimeSerialization
    {
        private static IByteConverter _converter;
        public static IByteConverter Converter
        {
            get
            {
                if (_converter == null)
                    _converter = new DefaultByteConverter();
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }

        public static List<byte> ToBytes(params object[] args)
        {
            List<byte> bytes = new List<byte>();

            ToBytes(ref bytes, args);

            return bytes;
        }

        public static void ToBytes(ref List<byte> bytes, params object[] args)
        {
            //CmuneDebug.LogFormat("ToBytes: {0}", args.Length);

            foreach (object o in args)
            {
                Converter.FromObject(o, ref bytes);
            }
        }

        public static void ToBytes(bool encodeType, ref List<byte> bytes, params object[] args)
        {
            //CmuneDebug.LogFormat("ToBytes: {0}", args.Length);

            foreach (object o in args)
            {
                Converter.FromObject(o, encodeType, ref bytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object[] ToObjects(byte[] bytes)
        {
            int i = 0;
            return ToObjects(bytes, ref i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static object[] ToObjects(byte[] bytes, ref int i)
        {
            List<object> objects = new List<object>(5);
            int lastIdx = -1;

            while (i < bytes.Length - 1 && lastIdx != i)
            {
                lastIdx = i;
                object obj = Converter.ToObject(bytes, ref i);
                if (obj != null)
                {
                    objects.Add(obj);
                }
            }

            if (i == int.MaxValue)
            {
                CmuneDebug.LogError("Error when deserializing Byte[] of Length {0}", bytes.Length);
            }
            else if (i == lastIdx)
            {
                CmuneDebug.LogError("Error when deserializing Byte[] of Length {0} because index didn't change at {1}", bytes.Length, lastIdx);
                i = int.MaxValue;
            }

            return objects.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool TryToObjects(byte[] bytes, out object[] parameters)
        {
            int i = 0;
            parameters = ToObjects(bytes, ref i);
            return (i != int.MaxValue);
        }

        public static object ToObject(byte[] bytes, ref int i)
        {
            return Converter.ToObject(bytes, ref i);
        }

        public static bool TryDecodeObject(byte[] bytes, ref int i, byte type, out object obj)
        {
            return Converter.TryDecodeObject(bytes, type, ref i, out obj);
        }

        public static object ToObject(byte[] bytes)
        {
            int i = 0;
            return Converter.ToObject(bytes, ref i);
        }

        public static bool IsTypeSupported(System.Type t)
        {
            return Converter.IsTypeSupported(t);
        }

        public static byte GetCmuneDataType(System.Type t)
        {
            return Converter.GetCmuneDataType(t);
        }
    }
}
