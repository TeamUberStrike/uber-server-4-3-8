using System;
using System.Collections.Generic;
using System.Text;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;

namespace Cmune.Realtime.Common.IO
{
    public class StringConversion
    {
        public static string ToString(byte[] bytes)
        {
            int len = bytes.Length;
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < len; )
            {
                builder.Append(DefaultByteConverter.ToChar(bytes, ref j));
            }
            return builder.ToString();
        }

        public static byte[] FromString(string s)
        {
            char[] chars = s.ToCharArray();
            List<byte> bytes = new List<byte>(chars.Length * 2);

            foreach (char c in chars)
                DefaultByteConverter.FromChar(c, ref bytes);

            return bytes.ToArray();
        }

        public static string Base64Decode(string data)
        {
            try
            {
                byte[] decbuff = Convert.FromBase64String(data);
                return System.Text.Encoding.UTF8.GetString(decbuff);
            }
            catch (Exception ex)
            {
                CmuneDebug.LogError("Error in base64Decode decoding:" + data + "\n" + ex.Message);
                throw new Exception("Error in base64Decode: " + ex.Message, ex);
            }
        }

        public static string Base64Encode(string data)
        {
            try
            {
                byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(encbuff);
            }
            catch (Exception ex)
            {
                CmuneDebug.LogError("Error in base64Encode" + ex.Message);
                throw new Exception("Error in base64Decode: " + ex.Message, ex);
            }
        }

        public static string Replace(string expr, string find, string repl)
        {
            return string.Equals(expr, find, StringComparison.CurrentCultureIgnoreCase) ? repl : expr;
        }

        public static string Replace(string expr, string find, string repl, bool bIgnoreCase)
        {
            // Get input string length
            int exprLen = expr.Length;
            int findLen = find.Length;

            // Check inputs
            if (0 == exprLen || 0 == findLen || findLen > exprLen)
                return expr;

            // Use the original method if the case is required
            if (!bIgnoreCase)
                return expr.Replace(find, repl);

            StringBuilder sbRet = new StringBuilder(exprLen);
            int pos = 0;

            while (pos + findLen <= exprLen)
            {
                if (0 == string.Compare(expr, pos, find, 0, findLen, bIgnoreCase))
                {
                    // Add the replaced string
                    sbRet.Append(repl);
                    pos += findLen;
                    continue;
                }
                // Advance one character
                sbRet.Append(expr, pos++, 1);
            }

            // Append remaining characters
            sbRet.Append(expr, pos, exprLen - pos);

            // Return string
            return sbRet.ToString();
        }
    }
}