using System;
using System.Text.RegularExpressions;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.DataCenter.Common.Utils
{
    public static class ValidationUtilities
    {
        /// <summary>
        /// This function will allow us to control the characters that we want to replace in a user name
        /// This function is not checking if the member name is valid or not.
        /// </summary>
        /// <param name="memberName">The input String</param>
        /// <returns>The secured input String</returns>
        public static string StandardizeMemberName(string memberName)
        {
            string cleanMemberName = TextUtilities.CompleteTrim(memberName);

            return cleanMemberName;
        }

        /// <summary>
        /// The goal of this method is to check the validity of an email address
        /// </summary>
        /// <param name="email">email address to check</param>
        /// <returns>TRUE if the email address is valid, FALSE otherwise</returns>
        public static bool IsValidEmailAddress(string email)
        {
            if (TextUtilities.IsNullOrEmpty(email) || email.Length > CommonConfig.MemberEmailMaxLength)
            {
                return false;
            }

            int nFirstAt = email.IndexOf('@');
            int nLastAt = email.LastIndexOf('@');

            if ((nFirstAt > 0) && (nLastAt == nFirstAt) && (nFirstAt < (email.Length - 1)))
            {
                return (Regex.IsMatch(email, @"^([a-zA-Z0-9_'+*$%\^&!\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9:]{2,4})+$"));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes spaces and lower the email.
        /// This function is not checking if the email is valid or not.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string StandardizeEmail(string email)
        {
            if (email != null)
            {
                email = TextUtilities.CompleteTrim(email).ToLower();
            }

            return email;
        }

        /// <summary>
        /// Checks wether a member password is compliant (for now it shouldn't be empty)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            bool isPasswordCompliant = false;

            if (!TextUtilities.IsNullOrEmpty(password) && 
                password.Length > CommonConfig.MemberPasswordMinLength && 
                password.Length < CommonConfig.MemberPasswordMaxLength)
            {
                isPasswordCompliant = true;
            }

            return isPasswordCompliant;
        }

        /// <summary>
        /// Checks wether a category name is compliant with our rules
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static bool IsValidCategoryName(string categoryName)
        {
            bool isValidCategory = false;

            if (!String.IsNullOrEmpty(categoryName))
            {
                isValidCategory = Regex.IsMatch(categoryName, "^[a-zA-Z0-9]{" + CommonConfig.ItemCategoryNameMinLength + "," + CommonConfig.ItemCategoryNameMaxLength + "}$");
            }

            return isValidCategory;
        }

        /// <summary>
        /// Checks wether a member name is compliant with our rules
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static bool IsValidMemberName(string memberName)
        {
            return IsValidMemberName(memberName, LocaleIdentifier.EnUS);
        }

        /// <summary>
        /// Checks wether a member name is compliant with our rules
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static bool IsValidMemberName(string memberName, string locale)
        {
            bool isValidMemberName = false;

            if (!String.IsNullOrEmpty(memberName))
            {
                memberName = memberName.Trim();

                if (memberName.Equals(TextUtilities.CompleteTrim(memberName)))
                {
                    string regExCharset = String.Empty;

                    switch (locale)
                    {
                        case LocaleIdentifier.KoKR:
                            regExCharset = "\\p{IsHangulSyllables}";
                            break;
                        default:
                            regExCharset = String.Empty;
                            break;
                    }

                    isValidMemberName = Regex.IsMatch(memberName, "^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?" + regExCharset + "]{" + CommonConfig.MemberNameMinLength + "," + CommonConfig.MemberNameMaxLength + "}$");
                }

                if (!memberName.ToLower().IndexOf("admin").Equals(-1) || !memberName.ToLower().IndexOf("cmune").Equals(-1))
                {
                    isValidMemberName = false;
                }
            }

            return isValidMemberName;
        }

        /// <summary>
        /// Checks wether an Esns member Id is valid for a specific Esns
        /// </summary>
        /// <param name="esnsType"></param>
        /// <param name="esnsMemberId"></param>
        /// <returns></returns>
        public static bool IsValidEsndId(EsnsType esnsType, string esnsMemberId)
        {
            bool isValidEsnsMemberId = false;

            switch (esnsType)
            {
                case EsnsType.Facebook:
                    // The Facebook ID has to be a long > 0
                    long facebookId;
                    bool isFacebookIdLong = Int64.TryParse(esnsMemberId, out facebookId);

                    if (isFacebookIdLong && facebookId > 0)
                    {
                        isValidEsnsMemberId = true;
                    }

                    break;
                case EsnsType.MySpace:
                    // The MySpace ID has to be a int > 0
                    int mySpaceId;
                    bool isMySpaceIdInt = Int32.TryParse(esnsMemberId, out mySpaceId);

                    if (isMySpaceIdInt && mySpaceId > 0)
                    {
                        isValidEsnsMemberId = true;
                    }

                    break;
                case EsnsType.Cyworld:
                    // The Cyworld ID has to be a int > 0
                    int cyworldId;
                    bool isCyworldIdInt = Int32.TryParse(esnsMemberId, out cyworldId);

                    if (isCyworldIdInt && cyworldId > 0)
                    {
                        isValidEsnsMemberId = true;
                    }

                    break;

                case EsnsType.Kongregate:
                    long kongregateId;
                    bool isKongregateIdLong = Int64.TryParse(esnsMemberId, out kongregateId);

                    if (isKongregateIdLong && kongregateId > 0)
                    {
                        isValidEsnsMemberId = true;
                    }

                    break;
            }

            return isValidEsnsMemberId;
        }

        /// <summary>
        /// Standardizes a contact group name
        /// </summary>
        /// <param name="contactGroupName"></param>
        /// <returns></returns>
        public static string StandardizeContactGroupName(string contactGroupName)
        {
            string cleanContactGroupName = TextUtilities.CompleteTrim(contactGroupName);

            return cleanContactGroupName;
        }

        /// <summary>
        /// Checks wether a contact group name is compliant with our rules
        /// </summary>
        /// <param name="contactGroupName"></param>
        /// <returns></returns>
        public static bool IsValidContactGroupName(string contactGroupName)
        {
            return IsValidContactGroupName(contactGroupName, true);
        }

        /// <summary>
        /// Checks wether a contact group name is compliant with our rules
        /// </summary>
        /// <param name="contactGroupName"></param>
        /// <param name="checkDefaultName"></param>
        /// <returns></returns>
        public static bool IsValidContactGroupName(string contactGroupName, bool checkDefaultName)
        {
            bool isValidContactGroupName = false;

            if (!String.IsNullOrEmpty(contactGroupName))
            {
                contactGroupName = contactGroupName.Trim();
                if (contactGroupName.Equals(TextUtilities.CompleteTrim(contactGroupName)))
                {
                    isValidContactGroupName = Regex.IsMatch(contactGroupName, "^[a-zA-Z0-9]{" + CommonConfig.ContacGroupNameMinLength + "," + CommonConfig.ContactGroupNameMaxLength + "}$");
                    bool isNameDefaultName = false;
                    if (checkDefaultName)
                    {
                        isNameDefaultName = contactGroupName.Equals(CommonConfig.ContactGroupDefaultName);
                    }

                    bool containForbiddenWords = false;
                    if (!contactGroupName.ToLower().IndexOf("admin").Equals(-1) || !contactGroupName.ToLower().IndexOf("cmune").Equals(-1))
                    {
                        containForbiddenWords = true;
                    }

                    if (isValidContactGroupName && !isNameDefaultName && !containForbiddenWords)
                    {
                        isValidContactGroupName = true;
                    }
                }
            }

            return isValidContactGroupName;
        }

        #region Groups

        /// <summary>
        /// Standardizes a clan name
        /// </summary>
        /// <param name="clanName"></param>
        /// <returns></returns>
        public static string StandardizeClanName(string clanName)
        {
            string cleanClanName = TextUtilities.CompleteTrim(clanName);

            return cleanClanName;
        }

        /// <summary>
        /// Standardizes a clan tag name
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static string StandardizeClanTag(string tagName)
        {
            string cleanClanTagName = TextUtilities.CompleteTrim(tagName);

            return cleanClanTagName;
        }

        /// <summary>
        /// Checks wether a clan name is compliant with our rules
        /// </summary>
        /// <param name="clanName"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static bool IsValidClanName(string clanName, string locale)
        {
            bool isValidClanName = false;

            if (!String.IsNullOrEmpty(clanName))
            {
                clanName = clanName.Trim();

                if (clanName.Equals(TextUtilities.CompleteTrim(clanName)))
                {
                    string regExCharset = String.Empty;

                    switch (locale)
                    {
                        case LocaleIdentifier.KoKR:
                            regExCharset = "\\p{IsHangulSyllables}";
                            break;
                        default:
                            regExCharset = String.Empty;
                            break;
                    }

                    isValidClanName = Regex.IsMatch(clanName, "^[a-zA-Z0-9 \\-" + regExCharset + "]{" + CommonConfig.GroupNameMinLength + "," + CommonConfig.GroupNameMaxLength + "}$");
                }

                if (!clanName.ToLower().IndexOf("admin").Equals(-1) || !clanName.ToLower().IndexOf("cmune").Equals(-1))
                {
                    isValidClanName = false;
                }

            }

            return isValidClanName;
        }

        /// <summary>
        /// Checks wether a clan tag is compliant with our rules
        /// </summary>
        /// <param name="clanTag"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static bool IsValidClanTag(string clanTag, string locale)
        {
            bool isValidClanTagName = false;

            if (!String.IsNullOrEmpty(clanTag))
            {
                clanTag = clanTag.Trim();

                if (clanTag.Equals(TextUtilities.CompleteTrim(clanTag)))
                {
                    string regExCharset = String.Empty;

                    switch (locale)
                    {
                        case LocaleIdentifier.KoKR:
                            regExCharset = "\\p{IsHangulSyllables}";
                            break;
                        default:
                            regExCharset = String.Empty;
                            break;
                    }

                    isValidClanTagName = Regex.IsMatch(clanTag, "^[a-zA-Z0-9 .!_\\-<>[\\]{}~@#$%^&*()=+|:?" + regExCharset + "]{" + CommonConfig.GroupTagMinLength + "," + CommonConfig.GroupTagMaxLenght + "}$");
                }

                if (!clanTag.ToLower().IndexOf("admin").Equals(-1) || !clanTag.ToLower().IndexOf("cmune").Equals(-1))
                {
                    isValidClanTagName = false;
                }
            }

            return isValidClanTagName;
        }

        /// <summary>
        /// Checks wether a clan motto is compliant with our rules
        /// </summary>
        /// <param name="clanMotto"></param>
        /// <returns></returns>
        public static bool IsValidClanMotto(string clanMotto)
        {
            bool isValidClanMotto = false;

            if (!String.IsNullOrEmpty(clanMotto))
            {
                if (clanMotto.Length >= CommonConfig.GroupMottoMinLength && clanMotto.Length <= CommonConfig.GroupMottoMaxLength)
                {
                    isValidClanMotto = true;
                }
            }

            return isValidClanMotto;
        }

        /// <summary>
        /// Checks wether a clan description is compliant with our rules
        /// </summary>
        /// <param name="clanDescription"></param>
        /// <returns></returns>
        public static bool IsValidClanDesciption(string clanDescription)
        {
            bool isValidClanDescription = false;

            if (!String.IsNullOrEmpty(clanDescription))
            {
                if (clanDescription.Length >= CommonConfig.GroupDescriptionMinLength && clanDescription.Length <= CommonConfig.GroupDescriptionMaxLength)
                {
                    isValidClanDescription = true;
                }
            }

            return isValidClanDescription;
        }

        #endregion Groups

        #region Photons groups

        /// <summary>
        /// Checks wether a photons group name is compliant with our rules
        /// </summary>
        /// <param name="photonsGroupsName"></param>
        /// <returns></returns>
        public static bool IsValidPhotonsGroupName(string photonsGroupsName)
        {
            bool isValidPhotonsGroupName = false;

            if (!String.IsNullOrEmpty(photonsGroupsName))
            {
                photonsGroupsName = photonsGroupsName.Trim();
                if (photonsGroupsName.Equals(TextUtilities.CompleteTrim(photonsGroupsName)))
                {
                    isValidPhotonsGroupName = (photonsGroupsName.Length >= CommonConfig.PhotonsGroupNameMinLenght && photonsGroupsName.Length <= CommonConfig.PhotonsGroupNameMaxLenght);
                }
            }

            return isValidPhotonsGroupName;
        }

        /// <summary>
        /// Standardizes a photons group name
        /// </summary>
        /// <param name="photonsGroupsName"></param>
        /// <returns></returns>
        public static string StandardizePhotonsGroupName(string photonsGroupsName)
        {
            string cleanPhotonsGroupsName = TextUtilities.CompleteTrim(photonsGroupsName);

            return cleanPhotonsGroupsName;
        }

        /// <summary>
        /// Checks wether a photon server name is compliant with our rules
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidPhotonServerName(string name)
        {
            bool isValidName = false;

            if (!String.IsNullOrEmpty(name))
            {
                name = name.Trim();

                if (name.Equals(TextUtilities.CompleteTrim(name)))
                {
                    isValidName = (name.Length >= CommonConfig.PhotonsServerNameMinLenght && name.Length <= CommonConfig.PhotonsServerNameMaxLenght);
                }
            }

            return isValidName;
        }

        /// <summary>
        /// Standardizes a photon server name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string StandardizePhotonServerName(string name)
        {
            string cleanName = TextUtilities.CompleteTrim(name);

            return cleanName;
        }

        #endregion Photons groups

        #region Server monitoring

        /// <summary>
        /// Checks wether a managed server name is compliant with our rules
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static bool IsValidManagedServerName(string serverName)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(serverName))
            {
                serverName = serverName.Trim();
                if (serverName.Equals(TextUtilities.CompleteTrim(serverName)))
                {
                    isValid = (serverName.Length >= CommonConfig.ManagedServerNameMinLenght && serverName.Length <= CommonConfig.ManagedServerNameMaxLenght);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a managed server name
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static string StandardizeManagedServerName(string serverName)
        {
            string cleanName = TextUtilities.CompleteTrim(serverName);

            return cleanName;
        }

        /// <summary>
        /// Checks wether a managed server name is compliant with our rules
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsValidManagedServerTestName(string testName)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(testName))
            {
                testName = testName.Trim();
                if (testName.Equals(TextUtilities.CompleteTrim(testName)))
                {
                    isValid = (testName.Length >= CommonConfig.PhotonsGroupNameMinLenght && testName.Length <= CommonConfig.PhotonsGroupNameMaxLenght);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a managed server name
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static string StandardizeManagedServerTestName(string serverName)
        {
            string cleanName = TextUtilities.CompleteTrim(serverName);

            return cleanName;
        }

        /// <summary>
        /// Checks wether a rotation member name is compliant with our rules
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static bool IsValidRotationMemberName(string memberName)
        {
            bool isValid = false;

            if (!String.IsNullOrEmpty(memberName))
            {
                memberName = memberName.Trim();
                if (memberName.Equals(TextUtilities.CompleteTrim(memberName)))
                {
                    isValid = (memberName.Length >= CommonConfig.RotationMemberNameMinLenght && memberName.Length <= CommonConfig.RotationMemberNameMaxLenght);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Standardizes a rotation member name
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static string StandardizeRotationMemberName(string memberName)
        {
            string cleanName = TextUtilities.CompleteTrim(memberName);

            return cleanName;
        }

        #endregion Server monitoring

        /// <summary>
        /// Checks whether a port number is valid
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        public static bool IsValidPortNumber(int portNumber)
        {
            bool isValidPortNumber = false;

            if (portNumber >= CommonConfig.PortMinNumber && portNumber <= CommonConfig.PortMaxNumber)
            {
                isValidPortNumber = true;
            }

            return isValidPortNumber;
        }

        /// <summary>
        /// Checks whether a port number is valid
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        public static bool IsValidPortNumber(string portNumber)
        {
            bool isValidPortNumber = false;
            int port = 0;

            if (Int32.TryParse(portNumber, out port))
            {
                isValidPortNumber = IsValidPortNumber(port);
            }

            return isValidPortNumber;
        }

        /// <summary>
        /// Checks that an IPV4 IP is well formed (0.0.0.0 -> 255.255.255.255)
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsValidIPAddress(string ipAddress)
        {
            bool isValidIPAddress = false;

            if (!String.IsNullOrEmpty(ipAddress))
            {
                string[] ipGroups = ipAddress.Split('.');

                if (ipGroups.Length == 4)
                {
                    byte group1;
                    byte group2;
                    byte group3;
                    byte group4;

                    bool isGroup1Byte = Byte.TryParse(ipGroups[0], out group1);
                    bool isGroup2Byte = Byte.TryParse(ipGroups[1], out group2);
                    bool isGroup3Byte = Byte.TryParse(ipGroups[2], out group3);
                    bool isGroup4Byte = Byte.TryParse(ipGroups[3], out group4);

                    if (isGroup1Byte && isGroup2Byte && isGroup3Byte && isGroup4Byte)
                    {
                        isValidIPAddress = true;
                    }
                }
            }

            return isValidIPAddress;
        }

        /// <summary>
        /// Checks whether a socket is valid
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static bool IsValidSocket(string socket)
        {
            bool isValid = false;

            string[] ipAndPort = socket.Split(':');

            if (ipAndPort.Length == 2)
            {
                isValid = IsValidIPAddress(ipAndPort[0]) && IsValidPortNumber(ipAndPort[1]);
            }

            return isValid;
        }
    }
}