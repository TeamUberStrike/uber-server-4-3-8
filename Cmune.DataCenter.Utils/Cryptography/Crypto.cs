using System.Security.Cryptography;
using System.Text;

namespace Cmune.DataCenter.Common.Utils.Cryptography
{
    public static class Crypto
    {
        public static string fncSHA256Encrypt(string inputString)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(inputString);
            
            string hashString = "";

            using (SHA256Managed encSHA256 = new SHA256Managed())
            {
                byte[] hashBytes = encSHA256.ComputeHash(bytes);


                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
                }
            }

            return hashString.PadLeft(32, '0');
        }

        public static string fncRijndaelEncrypt(string inputClearText, string passPhrase, string initVector)
        {
            string cipherText = "";

            // Before encrypting data, we will append plain text to a random
            // salt value, which will be between 4 and 8 bytes long (implicitly
            // used defaults).
            RijndaelEnhanced rijndaelKey = new RijndaelEnhanced(passPhrase, initVector);

            cipherText = rijndaelKey.Encrypt(inputClearText);

            return cipherText;
        }

        public static string fncRijndaelDecrypt(string inputCipherText, string passPhrase, string initVector)
        {
            string clearText = "";

            // Before encrypting data, we will append plain text to a random
            // salt value, which will be between 4 and 8 bytes long (implicitly
            // used defaults).
            RijndaelEnhanced rijndaelKey = new RijndaelEnhanced(passPhrase, initVector);
            clearText = rijndaelKey.Decrypt(inputCipherText);
            
            return clearText;
        }
    }
}
