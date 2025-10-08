using System;
using Cmune.Realtime.Common.IO;
using Cmune.Util;

namespace Cmune.Realtime.Common.Security
{
    public class SecureMemory<T> : ISecureMemory
    {
        /// <summary>
        /// 14 characters
        /// </summary>
        private const string pp = "h&dk2Ks901HenM";
        /// <summary>
        /// 16 characters
        /// </summary>
        private const string iv = "huSj39Dl)2kJ4nat";

        private byte[] _encryptedData;
        private T _cachedValue;

        public SecureMemory(T value, bool monitorMemory = true)
        {
            WriteData(value);

            if (monitorMemory)
            {
                SecureMemoryMonitor.Instance.AddToMonitor += ValidateData;
                if (CmuneDebug.IsDebugEnabled)
                {
                    var s = new System.Diagnostics.StackTrace(1);
                    // CmuneDebug.Log("SecureMemory " + s.ToString());
                }
            }
        }

        public static void ReleaseData(SecureMemory<T> instance)
        {
            if (instance != null)
                SecureMemoryMonitor.Instance.AddToMonitor -= instance.ValidateData;
        }

        public void SimulateMemoryHack(T value)
        {
            _cachedValue = value;
        }

        public void WriteData(T value)
        {
            try
            {
                _cachedValue = value;

                _encryptedData = Cryptography.RijndaelEncrypt(RealtimeSerialization.ToBytes((object)value).ToArray(), pp, iv);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("CmuneSecureVariable failed encrypting Data: {0}", e.Message), e.InnerException);
            }
        }

        public void ValidateData()
        {
            if (!Comparison.IsEqual(_cachedValue, DecryptValue()))
            {
                throw new Exception("Failed to validate data due to a corrupted memory");
            }
            //else
            //{
            //    CmuneDebug.Log("ValidateData successful");
            //}
        }

        public object ReadObject(bool secure)
        {
            return (object)ReadData(secure);
        }

        public T ReadData(bool secure)
        {
            if (secure) _cachedValue = DecryptValue();

            return _cachedValue;
        }

        private T DecryptValue()
        {
            try
            {
                byte[] decrypted = Cryptography.RijndaelDecrypt(_encryptedData, pp, iv);
                if (decrypted != null)
                {
                    object o = RealtimeSerialization.ToObject(decrypted);
                    if (o != null)
                    {
                        return (T)o;
                    }
                    else
                    {
                        throw new Exception("CmuneSecureVariable failed decrypting Data becauase RealtimeSerialization.ToObject returned NULL");
                    }
                }
                else
                {
                    throw new Exception("CmuneSecureVariable failed decrypting Data becauase CmuneSecurity.Decrypt returned NULL");
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("CmuneSecureVariable failed decrypting Data: {0}", e.Message), e.InnerException);
            }
        }
    }
}
