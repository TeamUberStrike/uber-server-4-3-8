using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using System.Globalization;

namespace UberStrike.DataCenter.Business
{
    public class CmuneCookie
    {
        #region Fields

        protected int _cmid;
        protected DateTime _expirationTime;
        protected string _cookieEncryptedContent;
        protected string _cookieDecryptedContent;
        protected string _cookieHash;
        protected string _lastReferer;
        protected string _email;
        protected string _memberName;
        protected bool _isValid;
        protected ChannelType _channel;
        protected string _channelMemberId;

        public static char Separator = '|';

        #endregion Fields

        #region Properties

        public int Cmid
        {
            get { return _cmid; }
            protected set { _cmid = value; }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            protected set { _expirationTime = value; }
        }

        public string CookieEncryptedContent
        {
            get { return _cookieEncryptedContent; }
            protected set { _cookieEncryptedContent = value; }
        }

        public string CookieDecryptedContent
        {
            get { return _cookieDecryptedContent; }
            protected set { _cookieDecryptedContent = value; }
        }

        public string CookieHash
        {
            get { return _cookieHash; }
            protected set { _cookieHash = value; }
        }

        public string LastReferer
        {
            get { return _lastReferer; }
            set
            {
                if (value != null)
                {
                    _lastReferer = value;
                }
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
            protected set { _isValid = value; }
        }

        public ChannelType Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public string Email
        {
            get { return _email; }
            protected set { _email = value; }
        }

        public string MemberName
        {
            get { return _memberName; }
            protected set { _memberName = value; }
        }

        public string ChannelMemberId
        {
            get { return _channelMemberId; }
            set { _channelMemberId = value; }
        }

        #endregion Properties

        #region Constructors

        public CmuneCookie(HttpCookie cmuneCookie)
        {
            if (cmuneCookie != null)
            {
                Int32.TryParse(cmuneCookie["id"], out this._cmid);

                var expirationTime = DateTime.Now;

                try
                {
                    expirationTime = Convert.ToDateTime(cmuneCookie["expirationDate"], CultureInfo.CreateSpecificCulture("en-US"));
                }
                catch
                {
                    expirationTime = Convert.ToDateTime(DateTime.Now.AddDays(7).ToString("G", CultureInfo.CreateSpecificCulture("en-US")), CultureInfo.CreateSpecificCulture("en-US"));
                }
                finally
                {
                    this._expirationTime = expirationTime;
                    this._cookieEncryptedContent = cmuneCookie["content"];
                    this._cookieHash = cmuneCookie["hash"];
                    this._isValid = false;
                    this._cookieDecryptedContent = string.Empty;
                    this._email = string.Empty;
                    this._memberName = string.Empty;
                }
            }
        }

        public CmuneCookie(int cmid, DateTime expirationDate)
        {
            this._cmid = cmid;
            this._expirationTime = expirationDate;
            this.CryptCookie();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// You need to call this function before reading encrypted data from the cookie
        /// </summary>
        /// <returns>A bool indicating if the cookie was altered. TRUE: the cookie is safe, FALSE: the cookie is corrupted</returns>
        public bool DecryptCookie()
        {
            if (!this.IsValid)
            {
                try
                {
                    this.CookieDecryptedContent = DecryptContent(this.CookieEncryptedContent);

                    if (IsMemberAuthenticated(this.Cmid, this.ExpirationTime, this.CookieEncryptedContent, this.CookieHash))
                    {
                        this.IsValid = true;
                        string[] cookieFields = this.CookieDecryptedContent.Split(Separator);

                        if (cookieFields.Length == 3)
                        {
                            this.Email = TextUtilities.Base64Decode(cookieFields[0]);
                            this.MemberName = TextUtilities.Base64Decode(cookieFields[1]);
                            this.ChannelMemberId = cookieFields[2];
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    CmuneLog.LogException(ex, "encryptedContent=" + this.CookieEncryptedContent + "&cmid=" + this.Cmid + "&expirationTime=" + this.ExpirationTime.ToString("G", CultureInfo.CreateSpecificCulture("en-US")));
                }
            }

            return this.IsValid;
        }

        /// <summary>
        /// Checks whether a member is authentified
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="expirationTime"></param>
        /// <param name="encryptedContent">Base64 encoded</param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsMemberAuthenticated(int cmid, DateTime expirationTime, string encryptedContent, string hash)
        {
            string currentHash = GenerateHash(cmid, expirationTime, DecryptContent(encryptedContent));

            return currentHash.Equals(hash);
        }

        public bool IsFullyValid(string channelId)
        {
            return (this.ChannelMemberId == channelId && IsValid);
        }

        /// <summary>
        /// Generates our cookie hash
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="expirationTime"></param>
        /// <param name="decryptedContent">Base64 encoded</param>
        /// <returns></returns>
        public static string GenerateHash(int cmid, DateTime expirationTime, string decryptedContent)
        {
            return Crypto.fncSHA256Encrypt(cmid.ToString() + Separator + expirationTime.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + Separator + decryptedContent);
        }

        /// <summary>
        /// Decrypts the content of the cookie
        /// </summary>
        /// <param name="encryptedContent">Base64 encoded</param>
        /// <returns></returns>
        public static string DecryptContent(string encryptedContent)
        {
            return Crypto.fncRijndaelDecrypt(encryptedContent, ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.PassPhrase), ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.InitVector));
        }

        /// <summary>
        /// Encrypts the content of the cookie
        /// </summary>
        /// <param name="decryptedContent">Base64 encoded</param>
        /// <returns></returns>
        public static string EncryptContent(string decryptedContent)
        {
            return Crypto.fncRijndaelEncrypt(decryptedContent, ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.PassPhrase), ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.InitVector));
        }

        /// <summary>
        /// You need to call this function before writing the cookie
        /// </summary>
        private void CryptCookie()
        {
            this.CookieEncryptedContent = EncryptContent(this.CookieDecryptedContent);
            this.CookieHash = GenerateHash(this.Cmid, this.ExpirationTime, this.CookieDecryptedContent);
        }

        #endregion
    }
}