using System;
using System.Web;
using Cmune.DataCenter.Common.Utils;

namespace Cmune.DataCenter.Forum.Business.Utils
{
    /// <summary>
    /// This class contains
    /// </summary>
    public static class UtilsMisc
    {
        /// <summary>
        /// Remove bad characters and replace HTML entities
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static String RemoveBadCharacterInForumUserName(String username)
        {
            username = HttpUtility.HtmlEncode(username);
            username = ValidationUtilities.StandardizeMemberName(username);
            return username;
        }

        /// <summary>
        /// Removes space and trim the String
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static String StandardizeEmail(String email)
        {
            if (email != null)
            {
                email = email.Trim().ToLower();
            }

            return email;
        }
    }
}
