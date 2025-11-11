using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Forum.Business;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.DataAccess;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Business;


namespace UberStrike.DataCenter.Business
{
    /// <summary>
    /// Allows us to authenticate, log in, log out... a user thanks to his cookie
    /// </summary>
    public class AuthenticationCookieManagement
    {
        static void ResetAuthenticationCookie(HttpCookie cmuneHttpCookie)
        {
            // logOff (id=0) the user if cookie cant not be decrypt
            cmuneHttpCookie["id"] = "0";
            cmuneHttpCookie["content"] = string.Empty;
            cmuneHttpCookie["hash"] = string.Empty;
            cmuneHttpCookie.Expires = DateTime.Now.AddDays(7);
            cmuneHttpCookie["expirationDate"] = cmuneHttpCookie.Expires.ToString("G", CultureInfo.CreateSpecificCulture("en-US"));
        }

        static void ResetAuthenticationCookieKeepingExpiration(HttpCookie cmuneHttpCookie)
        {
            // logOff (id=0) the user if cookie cant not be decrypt
            cmuneHttpCookie["id"] = "0";
            cmuneHttpCookie["content"] = string.Empty;
            cmuneHttpCookie["hash"] = string.Empty;
        }

        /// <summary>
        /// Authenticate a user against his cookie without accessing the database
        /// Should be called on every page restricted to user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static CmuneCookie ValidateAuthCookie(HttpRequestBase request, HttpResponseBase response, ChannelType channel)
        {
            string cookieName = GetCookieName(channel);

            CmuneCookie cmuneCookie;
            HttpCookie myCmuneHttpCookie;

            // if user is logged
            if (request.Cookies[cookieName] != null)
            {
                // get the original cookie in http request
                myCmuneHttpCookie = request.Cookies[cookieName];
                // create a object of this original cookie, used as a temporary data container
                cmuneCookie = new CmuneCookie(myCmuneHttpCookie);
                // update the expires date of the original cookie
                myCmuneHttpCookie.Expires = DateTime.Now.AddDays(7);
                myCmuneHttpCookie["expirationDate"] = myCmuneHttpCookie.Expires.ToString("G", CultureInfo.CreateSpecificCulture("en-US"));

                // if the cookie isn't expired and we have the channel id of the member
                if (cmuneCookie.ExpirationTime.CompareTo(DateTime.Now) > 0 && cmuneCookie.Cmid != 0)
                {
                    // if the cookie is decrptable then The member is authenticated
                    if (cmuneCookie.DecryptCookie())
                    {
                        int channelId;
                        // try to get the channel Id and then update it in the cookie
                        Int32.TryParse(myCmuneHttpCookie["platform"], out channelId);
                        cmuneCookie.Channel = (ChannelType)channelId;
                        // update hash code
                        myCmuneHttpCookie["hash"] = CmuneCookie.GenerateHash(cmuneCookie.Cmid, myCmuneHttpCookie.Expires, cmuneCookie.CookieDecryptedContent);
                    }
                    else
                        ResetAuthenticationCookieKeepingExpiration(myCmuneHttpCookie);
                }
                else
                    ResetAuthenticationCookieKeepingExpiration(myCmuneHttpCookie);
            }
            else
            {
                // It means than the user is not loged
                myCmuneHttpCookie = new HttpCookie(cookieName);
                ResetAuthenticationCookie(myCmuneHttpCookie);
                cmuneCookie = new CmuneCookie(myCmuneHttpCookie);
            }

            if (DoesServerHasDomainName(request.ServerVariables["LOCAL_ADDR"]))
                myCmuneHttpCookie.Domain = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");

            // set the updated cookie in the response
            response.Cookies.Set(myCmuneHttpCookie);
            // return the old cookie
            return cmuneCookie;
        }

        /// <summary>
        /// Once a user is loged in, you should call this function to create his cookie
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="cmid"></param>
        /// <param name="channel"></param>
        /// <param name="channelMemberId"></param>
        public static CmuneCookie SetAuthCookie(HttpRequestBase request, HttpResponseBase response, int cmid, ChannelType channel, string channelMemberId)
        {
            CmuneCookie cmuneCookie = null;

            try
            {
                string cookieName = GetCookieName(channel);

                HttpCookie myCmuneCookie;
                // get the current cookie
                if (request.Cookies[cookieName] != null)
                    myCmuneCookie = request.Cookies[cookieName];
                else // or create a new one
                    myCmuneCookie = new HttpCookie(cookieName);

                if (DoesServerHasDomainName(request.ServerVariables["LOCAL_ADDR"]))
                    myCmuneCookie.Domain = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");

                myCmuneCookie["id"] = cmid.ToString();
                myCmuneCookie["platform"] = ((int)channel).ToString();

                using (CmuneDataContext cmuneDB = new CmuneDataContext())
                {
                    ID3 memberToLog = CmuneMember.GetId3(cmid, cmuneDB);
                    Member memberToLogCmune = memberToLog.Member;

                    DateTime cookieExpires;
                    string cookieExpirationDate;
                    string cookieContent;
                    string cookieHash;

                    // generate hash
                    GenerateCookieData(memberToLogCmune.CMID, memberToLogCmune.Login, memberToLog.Name, channelMemberId, out cookieExpires, out cookieExpirationDate, out cookieContent, out cookieHash);

                    myCmuneCookie.Expires = cookieExpires;
                    myCmuneCookie["expirationDate"] = cookieExpirationDate;
                    myCmuneCookie["content"] = cookieContent;
                    myCmuneCookie["hash"] = cookieHash;
                    // set the updated cookie in the response
                    response.Cookies.Set(myCmuneCookie);
                }
                cmuneCookie = new CmuneCookie(myCmuneCookie);
                cmuneCookie.ChannelMemberId = channelMemberId;
                cmuneCookie.DecryptCookie(); // verify encryption of cookie, permitting to authenticate it
            }
            catch (NullReferenceException ex)
            {
                CmuneLog.LogException(ex, String.Format("Cmid={0}&channel={1}&esnsId={2}", cmid, channel.ToString(), channelMemberId));
            }

            return cmuneCookie;
        }

        /// <summary>
        /// Terminates the user session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="channel"></param>
        public static CmuneCookie UnsetAuthCookie(HttpRequestBase request, HttpResponseBase response, ChannelType channel)
        {
            CmuneCookie cmuneCookie = null;
            string cookieName = GetCookieName(channel);

            HttpCookie myCmuneCookie;

            if (request.Cookies[cookieName] != null)
                myCmuneCookie = request.Cookies[cookieName];
            else
                myCmuneCookie = new HttpCookie(cookieName);

            if (DoesServerHasDomainName(request.ServerVariables["LOCAL_ADDR"]))
                myCmuneCookie.Domain = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");

            myCmuneCookie["platform"] = "-1";
            ResetAuthenticationCookie(myCmuneCookie);
            // set the updated cookie in the response
            response.Cookies.Set(myCmuneCookie);

            cmuneCookie = new CmuneCookie(myCmuneCookie);
            return cmuneCookie;
        }

        /// <summary>
        /// Get the last referrer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string GetLastReferer(HttpRequestBase request, HttpResponseBase response, ChannelType channel)
        {
            string cookieName = GetCookieName(channel);
            HttpCookie myCmuneCookie;

            if (request.Cookies[cookieName] != null)
            {
                myCmuneCookie = request.Cookies[cookieName];
            }
            else
            {
                myCmuneCookie = new HttpCookie(cookieName);
            }

            if (DoesServerHasDomainName(request.ServerVariables["LOCAL_ADDR"]))
            {
                myCmuneCookie.Domain = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");
            }

            if (myCmuneCookie["lastReferer"] == null)
            {
                myCmuneCookie["lastReferer"] = String.Empty;
            }

            response.Cookies.Set(myCmuneCookie);

            return myCmuneCookie["lastReferer"];
        }

        /// <summary>
        /// Set the last referer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="lastReferer"></param>
        /// <param name="channel"></param>
        public static void SetLastReferer(HttpRequestBase request, HttpResponseBase response, string lastReferer, ChannelType channel)
        {
            string cookieName = GetCookieName(channel);
            HttpCookie myCmuneCookie;

            if (request.Cookies[cookieName] != null)
            {
                myCmuneCookie = request.Cookies[cookieName];
            }
            else
            {
                myCmuneCookie = new HttpCookie(cookieName);
            }

            if (DoesServerHasDomainName(request.ServerVariables["LOCAL_ADDR"]))
            {
                myCmuneCookie.Domain = ConfigurationUtilities.ReadConfigurationManager("CookieDomainName");
            }

            if (!lastReferer.Equals(String.Empty) && lastReferer.ToLower().IndexOf("Cmune").Equals(-1))
            {
                if (myCmuneCookie["lastReferer"] != null && !myCmuneCookie["lastReferer"].Equals(lastReferer.ToString()))
                {
                    myCmuneCookie["lastReferer"] = lastReferer.ToString();
                }
            }
            else
            {
                if (myCmuneCookie["lastReferer"] == null)
                {
                    myCmuneCookie["lastReferer"] = String.Empty;
                }
            }

            response.Cookies.Set(myCmuneCookie);
        }

        /// <summary>
        /// Get the cookie name
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string GetCookieName(ChannelType channel)
        {
            string cookieName = String.Empty;

            switch (channel)
            {
                case ChannelType.WebPortal:
                    cookieName = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.PortalCookieName);
                    break;
                case ChannelType.WebFacebook:
                    cookieName = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.FacebookCookieName);
                    break;
                case ChannelType.Kongregate:
                    cookieName = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.KongregateCookieName);
                    break;
                default:
                    CmuneLog.LogUnexpectedReturn(channel, "This platform doesn't have a cookie yet.");
                    throw new ArgumentOutOfRangeException("platform", channel, "This platform doesn't have a cookie yet.");
            }

            return cookieName;
        }

        /// <summary>
        /// Checks wether the current server is a public server
        /// </summary>
        /// <param name="serverIP"></param>
        /// <returns></returns>
        public static bool DoesServerHasDomainName(string serverIP)
        {
            bool doesServerHasDomainName = false;

            if (!serverIP.Equals("127.0.0.1") && !serverIP.Equals("::1") && !serverIP.Equals(ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.LocalhostIP)) && !serverIP.Equals("192.168.1.200"))
            {
                doesServerHasDomainName = true;
            }

            return doesServerHasDomainName;
        }

        /// <summary>
        /// Generates the cookie data
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="login"></param>
        /// <param name="name"></param>
        /// <param name="channelMemberId"></param>
        /// <param name="cookieExpirationTime"></param>
        /// <param name="cookieTime"></param>
        /// <param name="cookieContent"></param>
        /// <param name="cookieHash"></param>
        public static void GenerateCookieData(int cmid, string login, string name, string channelMemberId, out DateTime cookieExpirationTime, out string cookieTime, out string cookieContent, out string cookieHash)
        {
            cookieExpirationTime = DateTime.Now.AddDays(7);
            cookieTime = cookieExpirationTime.ToString("G", CultureInfo.CreateSpecificCulture("en-US"));
            string decryptedContent = TextUtilities.Base64Encode(login) + CmuneCookie.Separator + TextUtilities.Base64Encode(name) + CmuneCookie.Separator + channelMemberId.ToString();
            cookieContent = CmuneCookie.EncryptContent(decryptedContent);
            cookieHash = CmuneCookie.GenerateHash(cmid, cookieExpirationTime, decryptedContent);
        }
    }
}