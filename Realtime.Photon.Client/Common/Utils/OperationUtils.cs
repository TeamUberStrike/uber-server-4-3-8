using System;
using System.Collections;
using System.Text;
using Cmune.Util;
using System.Collections.Generic;

namespace Cmune.Realtime.Common.Utils
{
    /// <summary>
    /// This Utils class is used to guarantee safe access to the Photon-Operation-Hashtable.
    /// There is a set of generic Get and Set functions that throw apropiate expections and 
    /// error logs if the input is not correct or the expected values can't be found.
    /// </summary>
    public static class OperationUtil
    {
        #region GETTER

        public static bool HasArg(IDictionary<byte,object> t, byte k)
        {
            return t.ContainsKey(k);
        }

        public static bool HasGeneralArg(IDictionary t, byte k)
        {
            return t.Contains(k);
        }

        public static T GetGeneralArg<T>(IDictionary t, byte k)
        {
            try
            {
                return (T)t[k];
            }
            catch
            {
                throw CmuneDebug.Exception("Error in OperationUtils.GetGeneralArg(<{2}>)! Key {0}({1}) not found in Table or expecting wrong Type!", k, (byte)k, typeof(T));
            }
        }

        public static T GetArg<T>(IDictionary<byte, object> t, byte k)
        {
            try
            {
                return (T)t[k];
            }
            catch
            {
                throw CmuneDebug.Exception("Error in OperationUtils.GetArg(<{2}>)! Key {0}({1}) not found in Table or expecting wrong Type!", k, (byte)k, typeof(T));
            }
        }

        public static int GetActor(IDictionary<byte, object> t)
        {
            return GetArg<int>(t, ParameterKeys.ActorNr);
        }

        public static Hashtable GetData(IDictionary<byte, object> t)
        {
            return GetArg<Hashtable>(t, ParameterKeys.Data);
        }

        public static byte[] GetBytes(IDictionary<byte, object> t)
        {
            return GetArg<byte[]>(t, ParameterKeys.Bytes);
        }

        #endregion

        #region SETTER

        public static void SetArg<T>(IDictionary<byte, object> t, byte k, object arg)
        {
            try
            {
                t[k] = (T)arg;
            }
            catch
            {
                if (arg != null)
                {
                    throw CmuneDebug.Exception("Error in OperationUtils.SetArg()! Key {0}({1}) is expecting value of Type {2} but found {3}", k, (byte)k, typeof(T), arg.GetType());
                }
                else
                {
                    throw CmuneDebug.Exception("Error in OperationUtils.SetArg()! Key {0}({1}) is expecting value of Type {2} but found NULL", k, (byte)k, typeof(T));
                }
            }
        }

        public static void SetMethodId(IDictionary<byte, object> t, object arg)
        {
            SetArg<byte>(t, ParameterKeys.MethodId, arg);
        }

        public static void SetInstanceID(IDictionary<byte, object> t, object arg)
        {
            SetArg<short>(t, ParameterKeys.InstanceId, arg);
        }

        public static void SetBytes(IDictionary<byte, object> t, object arg)
        {
            SetArg<byte[]>(t, ParameterKeys.Bytes, arg);
        }

        #endregion

        public static string PrintHashtable(IDictionary t)
        {
            StringBuilder b = new StringBuilder();

            if (t != null)
            {
                foreach (DictionaryEntry k in t)
                {
                    b.AppendFormat("[{0}]: {1} of Type {2}\n", k.Key, CmunePrint.Values(k.Value), CmunePrint.Types(k.Value));
                }
            }
            else
            {
                b.Append("NULL");
            }
            return b.ToString();
        }
    }
}
