using System;
using System.Text;

namespace Cmune.Util
{
    public static partial class CmunePrint
    {
        private static readonly byte _byteBitCountConstant = 7;
        private static readonly byte _byteBitMaskConstant = 1 << 7;

        public static void DebugBitString(byte[] x)
        {
            CmuneDebug.Log(BitString(x));
        }

        public static void DebugBitString(int x)
        {
            CmuneDebug.Log(BitString(x));
        }

        public static void DebugBitString(string x)
        {
            CmuneDebug.Log(BitString(x));
        }

        public static void DebugBitString(byte x)
        {
            CmuneDebug.Log(BitString(x));
        }

        public static string BitString(byte x)
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i <= _byteBitCountConstant; ++i)
            {
                b.Append(((x & _byteBitMaskConstant) == 0) ? '0' : '1');
                x <<= 1;
            }

            return b.ToString();
        }

        public static string BitString(int x)
        {
            return BitString(BitConverter.GetBytes(x));
        }

        public static string BitString(string x)
        {
            return BitString(Encoding.Unicode.GetBytes(x));
        }

        public static string BitString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                builder.Append(BitString(bytes[i])).Append(' '); ;
            }

            return builder.ToString();
        }

        //public static String getBitString(int x)
        //{
        //    StringBuilder buf = new StringBuilder();
        //    for (int i = 1; i <= 32; i++) buf.Append(x >> (32 - i) & 0x00000001);
        //    return buf.ToString();
        //}
    }
}
