using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cmune.Realtime.Common.Utils
{
    public static class Conversion
    {
        public static T[] ToArray<T>(ICollection<T> collection)
        {
            T[] list = new T[collection.Count];
            collection.CopyTo(list, 0);
            return list;
        }

        public static Array ToArray(ICollection collection)
        {
            object[] values = new object[collection.Count];
            collection.CopyTo(values, 0);
            return values;
        }

        public static T ToEnum<T>(string value)
        {
            if (typeof(T).IsEnum && !string.IsNullOrEmpty(value) && Enum.IsDefined(typeof(T), value))
                return (T)Enum.Parse(typeof(T), value);
            else
                return default(T);
        }

        //public static string ToEnum<T>(object value)
        //{
        //    if (typeof(T).IsEnum && value != null && Enum.IsDefined(typeof(T), value))
        //        return Enum.GetName(typeof(T), value);
        //    else
        //        return typeof(T).Name + ".Undefined";
        //}

        #region ANGLES

        public static float Deg2Rad(float angle)
        {
            return Mathf.Abs((((angle % 360) + 360) % 360) / 360F);
        }

        public static byte Angle2Byte(float angle)
        {
            return (byte)(255F * Deg2Rad(angle));
        }

        public static float Byte2Angle(byte angle)
        {
            float a = 360F * angle;
            return a / 255F;
        }

        public static ushort Angle2Short(float angle)
        {
            return (ushort)((float)ushort.MaxValue * Deg2Rad(angle));
        }

        public static float Short2Angle(ushort angle)
        {
            float a = 360F * angle;
            return a / (float)ushort.MaxValue;
        }

        #endregion
    }
}
