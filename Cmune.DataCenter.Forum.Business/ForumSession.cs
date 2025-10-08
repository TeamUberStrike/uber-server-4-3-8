using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Forum.Business.Utils;
using Cmune.DataCenter.Forum.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Forum.Business
{
    /// <summary>
    /// This class manages the phpBB session (login and logout)
    /// </summary>
    public static class ForumSession
    {
        // TODO -> Maybe we should read that from the config
        static int timeDiffWinPhp = 18000; // PHP and the Windows server do not have the same time... PHP is +5h
        static int anonymousID = 1;

        /// <summary>
        /// Creates a session for a registered user identified by his CMID.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public static List<HttpCookie> CreateSession(int cmid, HttpRequest userRequest)
        {
            List<HttpCookie> cookies = new List<HttpCookie>();
            try
            {
                if (cmid > 0)
                {
                    cookies = CreateSessionStep(cmid, userRequest);
                }
                return cookies;
            }
            catch (Exception ex)
            {
                if (ex.GetType().Equals(typeof(FormatException)) && ex.Message.Equals("String must be exactly one character long."))
                {
                    /*
                     * We have a problem with this account: The columns
                     *      - user_topic_sortby_type
                     *      - user_topic_sortby_dir
                     *      - user_post_sortby_type
                     *      - user_post_sortby_dir
                     * are empty or they should have a default value.
                     */
                    ForumMember.FixSort(cmid);
                    cookies = CreateSessionStep(cmid, userRequest);
                    return cookies;
                }
                else
                {
                    ex.Data.Add("Message", cmid);
                    throw;
                }
            }
        }

        /// <summary>
        /// Created for avoiding code duplicate
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        private static List<HttpCookie> CreateSessionStep(int cmid, HttpRequest userRequest)
        {
            List<HttpCookie> cookies = new List<HttpCookie>();

            phpbb_user currentUser = ForumMember.GetUser(cmid);
            if (currentUser != null)
            {
                int currentTimestamp = Php.ConvertToTimestamp(DateTime.Now) + timeDiffWinPhp;
                DeleteSession(currentUser.user_id);

                cookies = CreateSession(currentUser.user_id, currentTimestamp, currentUser.user_lastvisit, userRequest);
            }

            return cookies;
        }

        /// <summary>
        /// Creates an anonymous session (kind of session that not loged users are using)
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public static List<HttpCookie> CreateAnonymousSession(HttpRequest userRequest)
        {
            int currentTimestamp = Php.ConvertToTimestamp(DateTime.Now) + timeDiffWinPhp;
            return CreateSession(anonymousID, currentTimestamp, currentTimestamp, userRequest);
        }

        /// <summary>
        /// Creates a session for a user identified by his ID.
        /// Does not check if the user is existing.
        /// </summary>
        /// <param name="phpBBUserID">This a phpBB user ID</param>
        /// <param name="sessionStart">sessionStart should be superior or the same than lastVisit</param>
        /// <param name="lastVisit"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        private static List<HttpCookie> CreateSession(int phpBBUserID, int sessionStart, int lastVisit, HttpRequest userRequest)
        {
            /*
             * We'll do it the easy way:
             *  1. Delete existing session in the DB (if there is one)
             *  2. Create new session in the DB
             *  3. Create or replace the phpBB cookies
             */

            if (sessionStart < lastVisit)
            {
                throw new ArgumentOutOfRangeException(String.Format("sessionStart ({0}) should be superior or the same than lastVisit ({1})", sessionStart, lastVisit));
            }

            List<HttpCookie> cookies = new List<HttpCookie>();
            using (ForumDataContext forumDB = new ForumDataContext())
            {
                string sidCookieName = ConfigurationManager.AppSettings["phpBBSidCookieName"];
                string trackingCookieName = ConfigurationManager.AppSettings["phpBBTrackingCookieName"];
                string IDCookieName = ConfigurationManager.AppSettings["phpBBIDCookieName"];

                // Create the new session

                string userIP = userRequest.UserHostAddress;
                string userAgent = userRequest.UserAgent.ShortenText(150);

                phpbb_session newSession = new phpbb_session();
                newSession.session_admin = 0;
                newSession.session_autologin = 0;
                newSession.session_browser = userAgent;
                newSession.session_forum_id = 0;
                newSession.session_forwarded_for = String.Empty;

                newSession.session_id = CryptographyUtilities.Md5Hash(ForumMember.PhpBBUniqueID());

                newSession.session_ip = userIP;
                newSession.session_last_visit = lastVisit;
                newSession.session_page = "index.php";
                newSession.session_start = sessionStart;
                newSession.session_time = sessionStart + 6;
                newSession.session_user_id = phpBBUserID;
                newSession.session_viewonline = 1;

                forumDB.phpbb_sessions.InsertOnSubmit(newSession);
                forumDB.SubmitChanges();

                // Create the Cookies

                HttpCookie sidCookie = new HttpCookie(sidCookieName, newSession.session_id);
                HttpCookie trackingCookie = new HttpCookie(trackingCookieName, String.Empty);
                HttpCookie IDCookie = new HttpCookie(IDCookieName, phpBBUserID.ToString());

                cookies.Add(sidCookie);
                cookies.Add(trackingCookie);
                cookies.Add(IDCookie);

                FixCookieData(ref cookies, userRequest);

                if (phpBBUserID != anonymousID)
                {
                    phpbb_user currentUser = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.user_id == phpBBUserID);
                    if (currentUser != null)
                    {
                        currentUser.user_form_salt = ForumMember.PhpBBUniqueID();
                        forumDB.SubmitChanges();
                    }
                }

                return cookies;
            }
        }

        /// <summary>
        /// Log out a user: deletes the current session and creates an anonymous session.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public static List<HttpCookie> EndSession(int cmid, HttpRequest userRequest)
        {
            using (ForumDataContext forumDB = new ForumDataContext())
            {
                if (cmid > 0)
                {
                    phpbb_user currentUser = ForumMember.GetUser(cmid, forumDB);
                    if (currentUser != null)
                    {
                        int currentTimestamp = Php.ConvertToTimestamp(DateTime.Now) + timeDiffWinPhp;
                        currentUser.user_lastvisit = currentTimestamp;

                        forumDB.SubmitChanges();

                        DeleteSession(currentUser.user_id);
                    }
                }

                return CreateAnonymousSession(userRequest);
            }
        }

        /// <summary>
        /// Deletes all the sessions of the users
        /// We can't use the CMID as it's querying another table
        /// </summary>
        /// <param name="phpBBUserID"></param>
        public static void DeleteSession(int phpBBUserID)
        {
            using (ForumDataContext forumDB = new ForumDataContext())
            {
                forumDB.phpbb_sessions.DeleteAllOnSubmit<phpbb_session>(forumDB.phpbb_sessions.Where<phpbb_session>(s => s.session_user_id == phpBBUserID));
                forumDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Puts the correct domain name and expiration date on the cookies.
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        private static void FixCookieData(ref List<HttpCookie> cookies, HttpRequest userRequest)
        {
            foreach (HttpCookie cookie in cookies)
            {
                cookie.Expires = DateTime.Now.AddSeconds(31536000);
                if (!userRequest.ServerVariables["LOCAL_ADDR"].Equals("127.0.0.1"))
                {
                    cookie.Domain = ConfigurationManager.AppSettings["phpBBDomainName"];
                }
            }
        }

        //TODO Login fct -> increase expiration date
    }
}