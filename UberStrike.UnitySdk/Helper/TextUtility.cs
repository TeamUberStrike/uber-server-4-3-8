using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UberStrike.Helper
{
    public static class TextUtility
    {
        /// <summary>
        /// This function will avoid SQL injection
        /// </summary>
        /// <param name="textToSecure">The input String</param>
        /// <returns>The secured input String</returns>
        public static string ConvertText(string textToSecure)
        {
            string resTxt = HtmlEncode(textToSecure);
            resTxt = resTxt.Replace("`", "&#96;");
            resTxt = resTxt.Replace("´", "&acute");
            resTxt = resTxt.Replace("'", "&#39");
            resTxt = resTxt.Replace("-", "&#45;");
            resTxt = resTxt.Replace("!", "&#33;");
            resTxt = resTxt.Replace("?", "&#63;");
            return resTxt;
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="text">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string HtmlEncode(string text)
        {
            if (text == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(text.Length);

            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (text[i] > 159)
                        {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                            sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// This function allows us to protect text field from sql injection
        /// </summary>
        /// <param name="textToSecure"></param>
        /// <returns></returns>
        public static string ProtectSqlField(string textToSecure)
        {
            return textToSecure.Replace("'", "''");
        }

        /// <summary>
        /// This function will avoid Javascript bugs
        /// </summary>
        /// <param name="textToSecure">The input String</param>
        /// <returns>The secured input String</returns>
        public static string ConvertTextForJavaScript(string textToSecure)
        {
            string resTxt = textToSecure.Replace("'", String.Empty);
            resTxt = resTxt.Replace("|", String.Empty);
            return resTxt;
        }

        /// <summary>
        /// Converts an IP address with dots to a network address
        /// IPV4 only
        /// </summary>
        /// <param name="addressIP"></param>
        /// <returns></returns>
        public static long InetAToN(string addressIP)
        {
            long networkAddress = 0;

            if (addressIP.Equals("::1"))
            {
                addressIP = "127.0.0.1";
            }

            if (!IsNullOrEmpty(addressIP))
            {
                string[] arrDec = addressIP.ToString().Split('.');

                if (arrDec.Length == 4)
                {
                    bool isValidIPAddress = true;
                    bool isInt = false;
                    int partialIP = 0;
                    long tmpNetworkAddress = 0;

                    for (int i = arrDec.Length - 1; i >= 0; i--)
                    {
                        isInt = Int32.TryParse(arrDec[i], out partialIP);
                        if (isInt && partialIP >= 0 && partialIP < 256)
                        {
                            tmpNetworkAddress += ((partialIP % 256) * (long)Math.Pow(256, (3 - i)));
                        }
                        else
                        {
                            isValidIPAddress = false;
                        }
                    }

                    if (isValidIPAddress)
                    {
                        networkAddress = tmpNetworkAddress;
                    }
                }
            }

            return networkAddress;
        }

        /// <summary>
        /// Converts a network address to an IP address with dots
        /// IPV4 only
        /// </summary>
        /// <param name="networkAddress"></param>
        /// <returns></returns>
        public static string InetNToA(long networkAddress)
        {
            string dottedIPAddress = String.Empty;
            long ipPart1 = 0;
            long ipPart2 = 0;
            long ipPart3 = 0;
            long ipPart4 = 0;

            if (networkAddress <= 4294967295)
            {
                ipPart1 = networkAddress / 16777216;

                if (ipPart1 == 0)
                {
                    ipPart1 = 255;
                    networkAddress += 16777216;
                }
                else if (ipPart1 < 0)
                {
                    if (networkAddress % 16777216 == 0)
                    {
                        ipPart1 += 256;
                    }
                    else
                    {
                        ipPart1 += 255;
                        if (ipPart1 == 128)
                        {
                            networkAddress += 2147483648;
                        }
                        else
                        {
                            networkAddress += 16777216 * (256 - ipPart1);
                        }
                    }
                }
                else
                {
                    networkAddress -= 16777216 * ipPart1;
                }

                networkAddress = networkAddress % 16777216;
                ipPart2 = networkAddress / 65536;
                networkAddress = networkAddress % 65536;
                ipPart3 = networkAddress / 256;
                networkAddress = networkAddress % 256;
                ipPart4 = networkAddress;

                dottedIPAddress = ipPart1.ToString() + "." + ipPart2.ToString() + "." + ipPart3.ToString() + "." + ipPart4.ToString();
            }

            return dottedIPAddress;
        }

        /// <summary>
        /// This function checks wether the input String is only composed by number
        /// </summary>
        /// <param name="numericText">The input String</param>
        /// <returns>TRUE whether there is only number, FALSE otherwise</returns>
        public static bool IsNumeric(string numericText)
        {
            bool isNumber = true;
            if (!IsNullOrEmpty(numericText))
            {
                if (numericText.StartsWith("-")) { numericText = numericText.Replace("-", String.Empty); }
                foreach (char c in numericText)
                {
                    isNumber = char.IsNumber(c);
                    if (!isNumber)
                    {
                        return isNumber;
                    }
                }
            }
            else { isNumber = false; }
            return isNumber;
        }

        /// <summary>
        /// This method allows us to shorten a text. If it adds "..." at the end of the shortened String, maxSize should be greater than 3
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="maxSize">The maz size (in character) of the output</param>
        /// <param name="addPoints">Wether to add dots at the end of the string</param>
        /// <returns>The shortened tring</returns>
        public static string ShortenText(string input, int maxSize, bool addPoints)
        {
            string output = input;
            if (maxSize < input.Length && maxSize > 3)
            {
                output = output.Substring(0, maxSize - 3);
                if (addPoints)
                {
                    output += "...";
                }
            }
            return output;
        }

        /// <summary>
        /// The REAL IsNullOrEmpty function, return true also when there is only spaces in the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string value)
        {
            bool isNullOrEmpty = true;

            if (!String.IsNullOrEmpty(value))
            {
                value = value.Trim();

                if (!String.IsNullOrEmpty(value))
                {
                    isNullOrEmpty = false;
                }
            }

            return isNullOrEmpty;
        }

        /// <summary>
        /// Get ALL the indexes of needle in the haystack
        /// </summary>
        /// <param name="haystack"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static List<int> IndexOfAll(string haystack, string needle)
        {
            int currentIndex = 0;
            int offset = 0;
            List<int> allIndexes = new List<int>();
            if (!TextUtility.IsNullOrEmpty(haystack) && !TextUtility.IsNullOrEmpty(needle))
            {
                int needleLenght = needle.Length;
                do
                {
                    currentIndex = haystack.IndexOf(needle);
                    if (currentIndex > -1)
                    {
                        haystack = haystack.Substring(currentIndex + needleLenght);
                        allIndexes.Add(currentIndex + offset);
                        offset += currentIndex + needleLenght;
                    }
                } while (currentIndex > -1 && !TextUtility.IsNullOrEmpty(haystack));
            }
            return allIndexes;
        }

        /// <summary>
        /// Encode a String in Base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Encode(string data)
        {
            string encodedData = String.Empty;

            if (data != null)
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                encodedData = Convert.ToBase64String(encData_byte);
            }

            return encodedData;
        }

        /// <summary>
        /// Decode a String from Base 64
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Decode(string data)
        {
            string decodedData = String.Empty;

            if (data != null)
            {
                UTF8Encoding encoder = new UTF8Encoding();
                Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                decodedData = new String(decoded_char);
            }

            return decodedData;
        }

        /// <summary>
        /// Converts a String to a Byte array.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string inputString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(inputString);
        }

        /// <summary>
        /// Will remove whitespaces before, after the String. Will also replace two and more whitespaces inside the String by only one whitespace
        /// </summary>
        /// <param name="text"></param>
        public static string CompleteTrim(string text)
        {
            if (text != null)
            {
                text = text.Trim();
                text = Regex.Replace(text, @"\s+", " ");
            }

            return text;
        }

        /// <summary>
        /// Tries to parse a Facebook Id from a string
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        public static bool TryParseFacebookId(string handle, out long facebookId)
        {
            bool isValidFacebookId = false;

            facebookId = 0;

            bool isFacebookIdLong = Int64.TryParse(handle, out facebookId);

            if (isFacebookIdLong && facebookId > 0)
            {
                isValidFacebookId = true;
            }

            return isValidFacebookId;
        }

        /// <summary>
        /// Tries to parse a MySpace Id from a string
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mySpaceId"></param>
        /// <returns></returns>
        public static bool TryParseMySpaceId(string handle, out int mySpaceId)
        {
            bool isValidMySpaceId = false;

            mySpaceId = 0;

            bool isMySpaceIdInt = Int32.TryParse(handle, out mySpaceId);

            if (isMySpaceIdInt && mySpaceId > 0)
            {
                isValidMySpaceId = true;
            }

            return isValidMySpaceId;
        }

        /// <summary>
        /// Tries to parse a Cyworld Id from a string
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="cyworldId"></param>
        /// <returns></returns>
        public static bool TryParseCyworldId(string handle, out int cyworldId)
        {
            bool isValidCyworldId = false;

            cyworldId = 0;

            bool isCyworldInt = Int32.TryParse(handle, out cyworldId);

            if (isCyworldInt && cyworldId > 0)
            {
                isValidCyworldId = true;
            }

            return isValidCyworldId;
        }

        /// <summary>
        /// Emulates the string.Join function for every type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Join<T>(string separator, List<T> list)
        {
            string joinedList = string.Empty;

            if (list != null && list.Count > 0)
            {
                string[] stringsList = new string[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    stringsList[i] = list[i].ToString();
                }

                joinedList = string.Join(separator, stringsList);
            }

            return joinedList;
        }
    }
}
