using System.Collections.Specialized;
using Cmune.DataCenter.Common.Utils;
using System.Web;
using System.Text;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// Extension methods for the String class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The REAL IsNullOrEmpty function, return true also when there is only spaces in the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrFullyEmpty(this string value)
        {
            return TextUtilities.IsNullOrEmpty(value);
        }

        /// <summary>
        /// This function checks wether the input string is only composed by number
        /// </summary>
        /// <param name="text">The input String</param>
        /// <returns>TRUE whether there is only number, FALSE otherwise</returns>
        public static bool IsNumeric(this string text)
        {
            return TextUtilities.IsNumeric(text);
        }

        /// <summary>
        /// This method allows us to shorten a text.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="maxSize">The maz size (in character) of the output</param>
        /// <returns>The shortened tring</returns>
        public static string ShortenText(this string input, int maxSize)
        {
            return TextUtilities.ShortenText(input, maxSize, false);
        }

        /// <summary>
        /// This method allows us to shorten a text. As it might add "..." at the end of the shortened String, maxSize should be greater than 3
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="maxSize">The maz size (in character) of the output</param>
        /// <param name="addPoints"></param>
        /// <returns>The shortened tring</returns>
        public static string ShortenText(this string input, int maxSize, bool addPoints)
        {
            return TextUtilities.ShortenText(input, maxSize, addPoints);
        }

        /// <summary>
        /// Encode a String in Base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Encode(this string data)
        {
            return TextUtilities.Base64Encode(data);
        }

        /// <summary>
        /// Decode a String from Base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Decode(this string data)
        {
            return TextUtilities.Base64Decode(data);
        }

        /// <summary>
        /// Encode a string to a base 64 url string 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlBase64Encode(this string str)
        {
            return str.Base64Encode().Replace("=", "").Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Decode a string from Base 64 with special paddng
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlBase64Decode(this string str)
        {
            var strWithOriginalBase64Char = str.Replace('-', '+').Replace('_', '/');
            var decodedStr = (strWithOriginalBase64Char.PadRight(strWithOriginalBase64Char.Length + (4 - strWithOriginalBase64Char.Length % 4) % 4, '=')).Base64Decode();
            return decodedStr; 
        }

        /// <summary>
        /// Converts a String to a Byte array.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string inputString)
        {
            return TextUtilities.StringToByteArray(inputString);
        }

        /// <summary>
        /// Will remove whitespaces before, after the String. Will also replace two and more whitespaces inside the String by only one whitespace
        /// </summary>
        /// <param name="text"></param>
        public static string CompleteTrim(this string text)
        {
            return TextUtilities.CompleteTrim(text);
        }

        /// <summary>
        /// Convert To NameValueCollection
        /// </summary>
        /// <param name="queryToConvert"></param>
        /// <returns></returns>
        public static NameValueCollection ConvertToNameValueCollection(this string queryToConvert)
        {
            NameValueCollection referrerQuery = new NameValueCollection();
            try
            {
                if (!queryToConvert.IndexOf('?').Equals(-1))
                {
                    string[] querySegments = queryToConvert.Split('&');
                    if (!querySegments.Length.Equals(0))
                    {
                        foreach (string segment in querySegments)
                        {
                            string[] parts = segment.Split('=');
                            if (parts.Length > 0)
                            {
                                string key = parts[0].Trim(new char[] {'?', ' '});
                                string val = parts[1].Trim();

                                referrerQuery.Add(key, val);
                            }
                        }
                    }
                }
            }
            catch
            {
                
            }

            return referrerQuery;
        }
    }
}
