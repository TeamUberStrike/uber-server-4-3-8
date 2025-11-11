using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.Forum.Business.Utils;
using Cmune.DataCenter.Forum.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Forum.Business
{
    public enum DeleteMode { Retain, Remove };

    /// <summary>
    /// This class allows us to create a new member on the forum and to modify his data (password, email, username...)
    /// </summary>
    public static class ForumMember
    {
        /// <summary>
        /// Add a member only if:
        ///     1. The email is not already stored in DB as main email
        ///     2. The userName is not already stored in DB
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password">Non hashed</param>
        /// <param name="name"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        public static MemberOperationResult AddUser(string emailAddress, string password, string name, int cmid, bool checkDuplicate)
        {
            MemberOperationResult ret = MemberOperationResult.Ok;

            try
            {
                #region Check input

                if (name.IsNullOrFullyEmpty())
                {
                    throw new ArgumentNullException("userName", "The user name shouldn't be empty or NULL.");
                }

                name = UtilsMisc.RemoveBadCharacterInForumUserName(name);

                if (name.IsNullOrFullyEmpty())
                {
                    throw new ArgumentOutOfRangeException("userName", name, "The user name shouldn't contain only forbidden characters");
                }

                if (emailAddress.IsNullOrFullyEmpty() || !ValidationUtilities.IsValidEmailAddress(emailAddress))
                {
                    if (emailAddress.IsNullOrFullyEmpty())
                    {
                        throw new ArgumentNullException("userEmail", "The email shouldn't be empty or NULL.");
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("userEmail", emailAddress, "This email is not valid.");
                    }
                }

                if (password.IsNullOrFullyEmpty())
                {
                    throw new ArgumentNullException("userPassword", "The password shouldn't be empty or NULL.");
                }

                if (cmid < 1)
                {
                    throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
                }

                #endregion

                emailAddress = UtilsMisc.StandardizeEmail(emailAddress);

                if (checkDuplicate)
                {
                    // SECOND STEP: we need to check if the userEmail and the clean userName are not duplicate
                    ret = CmuneMember.AreDuplicateEmailUserName(name, emailAddress);
                }

                if (ret.Equals(MemberOperationResult.Ok))
                {
                    using (ForumDataContext forumDB = new ForumDataContext())
                    {
                        // User insertion

                        DateTime joiningDate = DateTime.Now;
                        Crc32 crc32 = new Crc32();

                        string cleanUserName = ForumMember.Utf8CleanString(name);

                        phpbb_user newUser = new phpbb_user();

                        newUser.user_type = 0;
                        newUser.group_id = 2;
                        newUser.user_permissions = "";
                        newUser.user_perm_from = 0;
                        newUser.user_ip = "";
                        newUser.user_regdate = Php.ConvertToTimestamp(joiningDate);
                        newUser.username = name;
                        newUser.username_clean = cleanUserName;
                        newUser.user_password = Crypto.fncSHA256Encrypt(password);
                        newUser.user_passchg = Php.ConvertToTimestamp(joiningDate);
                        newUser.user_pass_convert = 0;
                        newUser.user_email = emailAddress;
                        newUser.user_email_hash = crc32.ComputeEmailHashPhpBB(emailAddress);
                        newUser.user_birthday = "";
                        newUser.user_lastvisit = 0;
                        newUser.user_lastmark = 0;
                        newUser.user_lastpost_time = 0;
                        newUser.user_lastpage = "";
                        newUser.user_last_confirm_key = "";
                        newUser.user_last_search = 0;
                        newUser.user_warnings = 0;
                        newUser.user_last_warning = 0;
                        newUser.user_login_attempts = 0;
                        newUser.user_inactive_reason = 0;
                        newUser.user_inactive_time = 0;
                        newUser.user_posts = 0;
                        newUser.user_lang = "en";
                        newUser.user_timezone = 0;
                        newUser.user_dst = 0;
                        newUser.user_dateformat = "D M d, Y g:i a";
                        newUser.user_style = 2;
                        newUser.user_rank = 0;
                        newUser.user_colour = "";
                        newUser.user_new_privmsg = 0;
                        newUser.user_unread_privmsg = 0;
                        newUser.user_last_privmsg = 0;
                        newUser.user_message_rules = 0;
                        newUser.user_full_folder = -3;
                        newUser.user_emailtime = 0;
                        newUser.user_topic_show_days = 0;
                        newUser.user_topic_sortby_type = 't';
                        newUser.user_topic_sortby_dir = 'd';
                        newUser.user_post_show_days = 0;
                        newUser.user_post_sortby_type = 't';
                        newUser.user_post_sortby_dir = 'a';
                        newUser.user_notify = 0;
                        newUser.user_notify_pm = 1;
                        newUser.user_notify_type = 0;
                        newUser.user_allow_pm = 1;
                        newUser.user_allow_viewonline = 1;
                        newUser.user_allow_viewemail = 1;
                        newUser.user_allow_massemail = 1;
                        newUser.user_options = 895;
                        newUser.user_avatar = "";
                        newUser.user_avatar_type = 0;
                        newUser.user_avatar_width = 0;
                        newUser.user_avatar_height = 0;
                        newUser.user_sig = "";
                        newUser.user_sig_bbcode_uid = "";
                        newUser.user_sig_bbcode_bitfield = "";
                        newUser.user_from = "";
                        newUser.user_icq = "";
                        newUser.user_aim = "";
                        newUser.user_yim = "";
                        newUser.user_msnm = "";
                        newUser.user_jabber = "";
                        newUser.user_website = "";
                        newUser.user_occ = "";
                        newUser.user_interests = "";
                        newUser.user_actkey = "";
                        newUser.user_newpasswd = "";
                        newUser.user_form_salt = ForumMember.PhpBBUniqueID();
                        newUser.Cmid = cmid;

                        forumDB.phpbb_users.InsertOnSubmit(newUser);
                        forumDB.SubmitChanges();

                        // User group insertion

                        /*
                         * LINQ does not allow to insert without a PK (So we'll write SQL code...)
                         */

                        forumDB.ExecuteCommand("INSERT INTO [phpbb_user_group] ([group_id], [group_leader], [user_id], [user_pending]) VALUES (2, 0, " + newUser.user_id + ", 0)");

                        // Config modification

                        /*
                         * Badly LINQ doesn't allow do perform update without selecting before...
                         * For performance reason, I will update only the num_users (normally we should update the newest_user_id and newest_username also)
                        */

                        phpbb_config config = forumDB.phpbb_configs.SingleOrDefault<phpbb_config>(c => c.config_name == "num_users");
                        config.config_value = (Convert.ToInt32(config.config_value) + 1).ToString();

                        forumDB.SubmitChanges();

                        // Done! We have a new phpBB user!
                    }
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, "Failed to add member to forum on registration: cmid=" + cmid.ToString() + "&memberEmail=" + emailAddress + "&memberName=" + name + "&checkDuplicate=" + checkDuplicate.ToString());
            }

            return ret;
        }

        #region Get user

        /// <summary>
        /// Gets a READ ONLY user thanks to his email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static phpbb_user GetUser(string email)
        {
            phpbb_user user = null;

            if (!email.IsNullOrFullyEmpty())
            {
                using (ForumDataContext forumDB = new ForumDataContext())
                {
                    email = email.Trim().ToLower();
                    Crc32 crc32 = new Crc32();
                    double userEmailHash = crc32.ComputeEmailHashPhpBB(email);
                    user = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.user_email_hash == userEmailHash);
                }
            }

            return user;
        }

        /// <summary>
        /// Gets an user thanks to his email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="forumDB"></param>
        /// <returns></returns>
        public static phpbb_user GetUser(string email, ForumDataContext forumDB)
        {
            phpbb_user user = null;

            if (!email.IsNullOrFullyEmpty() && forumDB != null)
            {
                email = email.Trim().ToLower();
                Crc32 crc32 = new Crc32();
                double userEmailHash = crc32.ComputeEmailHashPhpBB(email);
                user = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.user_email_hash == userEmailHash);
            }

            return user;
        }

        /// <summary>
        /// Gets a READ ONLY user thanks to his cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static phpbb_user GetUser(int cmid)
        {
            phpbb_user user = null;

            if (cmid > 0)
            {
                using (ForumDataContext forumDB = new ForumDataContext())
                {
                    user = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.Cmid == cmid);
                }
            }

            return user;
        }

        /// <summary>
        /// Gets an user thanks to his cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="forumDB"></param>
        /// <returns></returns>
        public static phpbb_user GetUser(int cmid, ForumDataContext forumDB)
        {
            phpbb_user user = null;

            if (cmid > 0 && forumDB != null)
            {
                user = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.Cmid == cmid);
            }

            return user;
        }

        #endregion

        #region Change data

        /// <summary>
        /// Change the main email of a user:
        ///     1. cmid should exist in the DB.
        ///     2. newMemberEmail is not already stored in DB as main email
        /// I strongly advise you to check the new email validity before changing it.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        /// <remarks>Not implemented</remarks>
        public static MemberOperationResult ChangeEmail(string emailAddress, int cmid, bool checkDuplicate)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Chek input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
            }
            if (emailAddress.IsNullOrFullyEmpty() || !ValidationUtilities.IsValidEmailAddress(emailAddress))
            {
                if (emailAddress.IsNullOrFullyEmpty())
                {
                    throw new ArgumentNullException("newUserEmail", "The new email shouldn't be empty or NULL.");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("newUserEmail", emailAddress, "This email address is not valid.");
                }
            }

            #endregion

            emailAddress = UtilsMisc.StandardizeEmail(emailAddress);

            using (ForumDataContext forumDB = new ForumDataContext())
            {
                phpbb_user phpbbUser = GetUser(cmid, forumDB);
                if (phpbbUser != null)
                {
                    ret = MemberOperationResult.Ok;

                    if (checkDuplicate && CmuneMember.IsDuplicateUserEmail(emailAddress))
                    {
                        ret = MemberOperationResult.DuplicateEmail;
                    }

                    if (ret.Equals(MemberOperationResult.Ok))
                    {
                        phpbbUser.user_email = emailAddress;

                        Crc32 crc32 = new Crc32();
                        phpbbUser.user_email_hash = crc32.ComputeEmailHashPhpBB(emailAddress);

                        forumDB.SubmitChanges();
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Update the user name of the user matching with the user email
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        /// <remarks>Not implemented</remarks>
        public static MemberOperationResult ChangeUsername(string name, int cmid, bool checkDuplicate)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be greater than 0.");
            }

            if (name.IsNullOrFullyEmpty())
            {
                throw new ArgumentNullException("newUserName", "The new user name shouldn't be empty or NULL.");
            }

            name = UtilsMisc.RemoveBadCharacterInForumUserName(name);

            if (name.IsNullOrFullyEmpty())
            {
                throw new ArgumentNullException("newUserName", "The new user name shouldn't contains only forbidden characters.");
            }

            #endregion

            using (ForumDataContext forumDB = new ForumDataContext())
            {
                phpbb_user phpbbUser = GetUser(cmid, forumDB);

                if (phpbbUser != null)
                {
                    ret = MemberOperationResult.Ok;

                    if (checkDuplicate && CmuneMember.IsDuplicateUserName(name))
                    {
                        ret = MemberOperationResult.DuplicateName;
                    }

                    if (ret.Equals(MemberOperationResult.Ok))
                    {
                        phpbbUser.username = name;
                        phpbbUser.username_clean = ForumMember.Utf8CleanString(name);

                        forumDB.SubmitChanges();
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Change the password of a member
        /// </summary>
        /// <param name="newMemberPassword">This password should be hashed already</param>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangePassword(int cmid, string newMemberPassword)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            if (cmid > 1)
            {
                if (!newMemberPassword.IsNullOrFullyEmpty())
                {
                    using (ForumDataContext forumDB = new ForumDataContext())
                    {
                        phpbb_user user = GetUser(cmid, forumDB);

                        if (user != null)
                        {
                            ret = ChangePassword(newMemberPassword, user, forumDB);
                        }
                        else
                        {
                            ret = MemberOperationResult.MemberNotFound;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Changes the password of a member
        /// </summary>
        /// <param name="newMemberPassword">This password should be hashed already</param>
        /// <param name="currentUser"></param>
        /// <param name="forumDB"></param>
        /// <returns></returns>
        private static MemberOperationResult ChangePassword(string newMemberPassword, phpbb_user currentUser, ForumDataContext forumDB)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            if (currentUser != null && forumDB != null)
            {
                currentUser.user_password = newMemberPassword;
                currentUser.user_passchg = Php.ConvertToTimestamp(DateTime.Now);

                forumDB.SubmitChanges();
                ret = MemberOperationResult.Ok;
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// Generates a clean_name according to phpBB
        /// WARNING: Calls a web service
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Utf8CleanString(string text)
        {
            string cleanText = String.Empty;

            try
            {
                PhpBBSync.phpBBSync phpBBWS = new PhpBBSync.phpBBSync();
                string phpBBSecretKey = ConfigurationUtilities.ReadConfigurationManager("PhpBBSecretKey");
                cleanText = phpBBWS.ws_clean_string(text, phpBBSecretKey);
                return cleanText;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Message", String.Format("Error while retrieving the clean user name [{0}]", text));
                throw;
            }
        }

        /// <summary>
        /// Generates the same unique ID than phpBB
        /// </summary>
        /// <returns></returns>
        public static string PhpBBUniqueID()
        {
            // First we need to read the config from the DB
            using (ForumDataContext forumDB = new ForumDataContext())
            {
                phpbb_config config = forumDB.phpbb_configs.SingleOrDefault<phpbb_config>(c => c.config_name == "rand_seed");
                string val = config.config_value + Php.MicroTime(DateTime.Now);

                val = CryptographyUtilities.Md5Hash(val);

                string randSeed = CryptographyUtilities.Md5Hash(config.config_value + val);

                // Now we write the new value of the field
                config.config_value = randSeed;
                forumDB.SubmitChanges();

                return randSeed.Substring(4, 16);
            }
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="cmid"></param>
        public static void DeleteUser(int cmid)
        {
            DeleteUser(cmid, DeleteMode.Remove, null);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="deleteMode"></param>
        /// <param name="postUsername"></param>
        public static void DeleteUser(int cmid, DeleteMode deleteMode, string postUsername)
        {
            #region check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be superior to 0.");
            }

            #endregion

            using (ForumDataContext forumDB = new ForumDataContext())
            {
                phpbb_user forumUserToDelete = forumDB.phpbb_users.SingleOrDefault<phpbb_user>(u => u.Cmid == cmid);
                if (forumUserToDelete != null)
                {
                    // Before we begin, we will remove the reports the user issued.

                    List<phpbb_report> userReports = forumDB.phpbb_reports.Where<phpbb_report>(r => r.user_id == forumUserToDelete.user_id).ToList<phpbb_report>();

                    if (userReports != null)
                    {
                        List<int> reportedPostsId = new List<int>();
                        List<int> reportedTopicsId = new List<int>();
                        foreach (phpbb_report userReport in userReports)
                        {
                            if (!reportedPostsId.Contains(userReport.post_id))
                            {
                                reportedPostsId.Add(userReport.post_id);
                            }
                            if (!reportedTopicsId.Contains(userReport.phpbb_post.topic_id))
                            {
                                reportedTopicsId.Add(userReport.phpbb_post.topic_id);
                            }
                        }

                        List<phpbb_post> reportedPosts = forumDB.phpbb_posts.Where<phpbb_post>(p => p.post_reported == 1 && reportedPostsId.Contains(p.post_id) && reportedTopicsId.Contains(p.topic_id)).ToList();
                        reportedPosts = reportedPosts.GroupBy(x => x.topic_id).Select(x => x.First()).ToList<phpbb_post>();

                        List<int> reportedTopicsToKeep = new List<int>();
                        foreach (phpbb_post topic in reportedPosts)
                        {
                            reportedTopicsToKeep.Add(topic.topic_id);
                        }

                        List<int> finalTopics = new List<int>();
                        foreach (int topicId in reportedTopicsId)
                        {
                            if (!reportedTopicsToKeep.Contains(topicId))
                            {
                                finalTopics.Add(topicId);
                            }
                        }

                        // Now set the flags back
                        reportedPosts = forumDB.phpbb_posts.Where<phpbb_post>(p1 => reportedPostsId.Contains(p1.post_id)).ToList<phpbb_post>();
                        foreach (phpbb_post reportedPost in reportedPosts)
                        {
                            reportedPost.post_reported = 0;
                        }
                        forumDB.SubmitChanges();

                        if (finalTopics != null)
                        {
                            //List<phpbb_t reportedTopics = forumDB.phpbb_posts.Where<phpbb_post>(p1 => reportedPostsId.Contains(p1.post_id)).ToList<phpbb_post>();
                            //foreach (phpbb_post reportedPost in reportedPosts)
                            //{
                            //    reportedPost.post_reported = 0;
                            //}
                            //forumDB.SubmitChanges();
                        }
                    }
                }


                //phpbb_report

                /*
                    
                $sql = 'SELECT r.post_id, p.topic_id
                        FROM ' . REPORTS_TABLE . ' r, ' . POSTS_TABLE . ' p
                        WHERE r.user_id = ' . $user_id . '
                            AND p.post_id = r.post_id';
                    
                 */
                //}
                //else
                //{
                //    ret = false;
                //}
            }
        }

        /// <summary>
        /// We have a problem with this account: The columns
        ///     - user_topic_sortby_type
        ///     - user_topic_sortby_dir
        ///     - user_post_sortby_type
        ///     - user_post_sortby_dir
        /// are empty or they should have a default value.
        /// Each time that the Exception FormatException is raised by LINQ with this messsage
        /// 'String must be exactly one character long.', you should call this function to heal the account.
        /// TODO phpBB migh modify this value, so we need to investigate before removing this function
        /// </summary>
        /// <param name="cmid"></param>
        public static void FixSort(int cmid)
        {
            // We'll nee to write the query by hand as LINQ doesn't allow us to select

            object[] sqlParams = { cmid };
            string updateQuery = "UPDATE [phpbb_users] SET [user_topic_sortby_type] = 't', [user_topic_sortby_dir] = 'd', [user_post_sortby_type] = 't', [user_post_sortby_dir] = 'a' WHERE [user_id] = {0}";

            using (ForumDataContext forumDB = new ForumDataContext())
            {
                forumDB.ExecuteCommand(updateQuery, sqlParams);
            }
        }
    }
}