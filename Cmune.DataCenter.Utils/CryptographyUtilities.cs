using System;
using System.Security.Cryptography;
using System.Text;
using Cmune.DataCenter.Common.Utils.Cryptography;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Cryptography Utilities
    /// </summary>
    public static class CryptographyUtilities
    {
        /// <summary>
        /// Centralization of the password hashing
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            return Crypto.fncSHA256Encrypt(password);
        }

        /// <summary>
        /// Computes the MD5 hashing of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(inputBytes);

                // Step 2, convert byte array to hexadecimal string
                return ByteToString(hash);
            }
        }

        /// <summary>
        /// Computes the HMAC MD5 hashing of a string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HmacMd5(string input, string key)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            using (HMACMD5 hmacmd5 = new HMACMD5(keyByte))
            {
                byte[] messageBytes = encoding.GetBytes(input);
                byte[] hashmessage = hmacmd5.ComputeHash(messageBytes);

                return ByteToString(hashmessage);
            }
        }

        /// <summary>
        /// Computes the HMAC SHA256 hashing of a string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string HmacSha256(string input, string secret)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                return ByteToString(hmacsha256.Hash);
            }
        }

        /// <summary>
        /// Convert a byte array to an hexadecimal string
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}