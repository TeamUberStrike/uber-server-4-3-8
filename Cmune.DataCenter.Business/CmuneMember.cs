using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using System.Globalization;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages members
    /// </summary>
    public static class CmuneMember
    {
        #region Create user

        /// <summary>
        /// Creates a user from a non Esns channel (Portal, OSX Stand alone)
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password">non hashed</param>
        /// <param name="applicationId"></param>
        /// <param name="channelType"></param>
        /// <param name="networkAddress"></param>
        /// <param name="locale"></param>
        /// <param name="attributedItems"></param>
        /// <param name="cmid"></param>
        /// <param name="name"></param>
        /// <param name="accountCreationTime"></param>
        /// <returns>MemberRegistrationResult</returns>
        public static MemberRegistrationResult CreateUser(string emailAddress, string password, int applicationId, ChannelType channelType, long networkAddress, string locale, List<int> attributedItems, out int cmid, out string name, out DateTime accountCreationTime)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;
            cmid = 0;

            accountCreationTime = DateTime.Now;

            name = GenerateName(accountCreationTime);

            emailAddress = ValidationUtilities.StandardizeEmail(emailAddress);

            if (!ValidationUtilities.IsValidEmailAddress(emailAddress))
            {
                ret = MemberRegistrationResult.InvalidEmail;
            }

            if (!ValidationUtilities.IsValidPassword(password))
            {
                ret = MemberRegistrationResult.InvalidPassword;
            }

            if (!MemberOperationResult.Ok.Equals(ValidationUtils.IsMemberNameFullyValid(name, locale)))
            {
                throw new ArgumentException(String.Format("The generated name [{0}] is invalid.", name));
            }

            if (ret.Equals(MemberRegistrationResult.Ok))
            {
                if (IsDuplicateUserEmail(emailAddress))
                {
                    ret = MemberRegistrationResult.DuplicateEmail;
                }
            }

            if (ret.Equals(MemberRegistrationResult.Ok))
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    // Member

                    Member user = new Member();
                    user.Login = emailAddress;
                    user.Password = CryptographyUtilities.HashPassword(password);
                    user.LastSyncDate = accountCreationTime;
                    user.ResLastSyncDate = accountCreationTime;
                    user.LastAliveAck = accountCreationTime;
                    user.TagName = String.Empty;
                    // Full Cmune account (with an email address), it's always the case now
                    user.IsRegistered = true;

                    EmailAddressStatus emailAddressStatus = EmailAddressStatus.Unverified;

                    if (channelType == ChannelType.WebFacebook)
                    {
                        // Email address coming from Facebook are always verified
                        emailAddressStatus = EmailAddressStatus.Verified;
                    }

                    user.EmailAddressState = (byte)emailAddressStatus;
                    user.LastChannel = (int)channelType;
                    user.LastIp = networkAddress;
                    // The member didn't choose a name yet
                    user.IsAccountComplete = false;

                    cmuneDb.Members.InsertOnSubmit(user);
                    cmuneDb.SubmitChanges();

                    cmid = user.CMID;

                    // Id3

                    ID3 id3 = new ID3();
                    id3.Name = name;
                    id3.Type = (int)(Id3Type.Nickname);
                    id3.LastSyncDate = accountCreationTime;
                    id3.CMID = cmid;
                    cmuneDb.ID3s.InsertOnSubmit(id3);
                    cmuneDb.SubmitChanges();

                    // Credits

                    Credit userCredit = new Credit();
                    userCredit.NbPoints = 0;
                    userCredit.NbCredits = 0;
                    userCredit.UserId = cmid;
                    userCredit.ExpDatePoint = accountCreationTime;
                    userCredit.ExpDateCredit = accountCreationTime;
                    cmuneDb.Credits.InsertOnSubmit(userCredit);
                    cmuneDb.SubmitChanges();

                    CmuneEconomy.AttributePoints(cmid, CommonConfig.PointsAttributedOnRegistration, true, PointsDepositType.Registration);

                    if (channelType == ChannelType.WebFacebook)
                    {
                        // Email address coming from Facebook are always valid, we should attribute the points
                        CmuneEconomy.AttributePoints(cmid, CommonConfig.PointsAttributedOnEmailValidation, false, PointsDepositType.IdentityValidation);
                    }

                    // Contacts groups

                    CmuneRelationship.CreateContactGroups(cmid);

                    // Access Level

                    MemberAccess memberAccess = new MemberAccess();
                    memberAccess.AccessLevel = (int)MemberAccessLevel.Default;
                    memberAccess.AccountDisabledUntil = null;
                    memberAccess.ApplicationId = applicationId;
                    memberAccess.ChatDisabledUntil = null;
                    memberAccess.Cmid = cmid;
                    memberAccess.IsAccountDisabled = (int)BanMode.No;
                    memberAccess.IsChatDisabled = (int)BanMode.No;
                    memberAccess.IsMailDisabled = (int)BanMode.No;
                    memberAccess.MailDisabledUntil = null;
                    cmuneDb.MemberAccesses.InsertOnSubmit(memberAccess);
                    cmuneDb.SubmitChanges();

                    // Attribute items

                    CmuneEconomy.AddItemsToInventoryPermanently(cmid, attributedItems, accountCreationTime);
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates a user from an Esns (Facebook for the moment), requires the Esns to provide an email address for the user
        /// </summary>
        /// <param name="esnsId"></param>
        /// <param name="esnsUserId"></param>
        /// <param name="emailAddress"></param>
        /// <param name="applicationId"></param>
        /// <param name="channelType"></param>
        /// <param name="networkAddress"></param>
        /// <param name="locale"></param>
        /// <param name="attributedItems"></param>
        /// <param name="cmid"></param>
        /// <param name="name"></param>
        /// <param name="password">non hashed</param>
        /// <param name="accountCreationTime"></param>
        /// <returns>MemberRegistrationResult</returns>
        public static MemberRegistrationResult CreateUserFromEsns(EsnsType esnsId, string esnsUserId, string emailAddress, int applicationId, ChannelType channelType, long networkAddress, string locale, List<int> attributedItems, out int cmid, out string name, out string password, out DateTime accountCreationTime)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;
            cmid = 0;
            name = String.Empty;
            password = String.Empty;
            accountCreationTime = DateTime.Now;

            if (CmuneMember.IsEmailAlreadyLinkedToEsns(emailAddress, esnsId))
            {
                ret = MemberRegistrationResult.EmailAlreadyLinkedToActualEsns;
            }
            else if (!ValidationUtilities.IsValidEsndId(esnsId, esnsUserId))
            {
                ret = MemberRegistrationResult.InvalidHandle;
            }
            else if (CmuneMember.IsDuplicateHandle(esnsId, esnsUserId))
            {
                ret = MemberRegistrationResult.DuplicateHandle;
            }

            if (ret.Equals(MemberRegistrationResult.Ok))
            {
                password = GeneratePassword();

                ret = CreateUser(emailAddress, password, applicationId, channelType, networkAddress, locale, attributedItems, out cmid, out name, out accountCreationTime);

                if (ret.Equals(MemberRegistrationResult.Ok))
                {
                    AttachEsnsAccount(esnsId, esnsUserId, cmid, false, accountCreationTime);
                }
            }

            return ret;
        }




        /// <summary>
        /// Is account complete
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool IsAccountComplete(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isAccountComplete = cmuneDb.Members.Where(u => u.CMID == cmid).Select(u => u.IsAccountComplete).SingleOrDefault();

                return isAccountComplete;
            }
        }

        /// <summary>
        /// Complete account
        /// </summary>
        /// <param name="cmid"></param>
        public static void CompleteAccount(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Member user = GetMember(cmid, cmuneDb);

                if (user != null)
                {
                    user.IsAccountComplete = true;
                    cmuneDb.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        /// <param name="facebookId"></param>
        /// <param name="applicationId"></param>
        /// <returns>MemberRegistrationResult</returns>
        public static MemberRegistrationResult AttachFacebookAccount(string emailAddress, string password, long facebookId, int applicationId)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;

            if (CmuneMember.IsDuplicateHandle(EsnsType.Facebook, facebookId.ToString()))
            {
                ret = MemberRegistrationResult.DuplicateHandle;
            }

            if (ret == MemberRegistrationResult.Ok)
            {
                Member member = null;

                MemberAuthenticationResult authResult = CmuneLoginEmail(emailAddress, password, applicationId, out member);

                if (authResult == MemberAuthenticationResult.Ok)
                {
                    MemberOperationResult linkingResult = AttachEsnsAccount(EsnsType.Facebook, facebookId.ToString(), member.CMID, false);

                    ret = ConvertEntities.ConvertMemberOperation(linkingResult);
                }
                else
                {
                    ret = MemberRegistrationResult.MemberNotFound;
                }
            }

            return ret;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="kongregateId"></param>
        /// <returns></returns>
        public static MemberRegistrationResult AttachKongregateAccount(int cmid, long kongregateId)
        {
            MemberRegistrationResult ret = MemberRegistrationResult.Ok;

            if (CmuneMember.IsDuplicateHandle(EsnsType.Kongregate, kongregateId.ToString()))
            {
                ret = MemberRegistrationResult.DuplicateHandle;
            }

            if (ret == MemberRegistrationResult.Ok)
            {
                Member member = CmuneMember.GetMember(cmid);

                if (member != null)
                {
                    MemberOperationResult linkingResult = AttachEsnsAccount(EsnsType.Kongregate, kongregateId.ToString(), member.CMID, false);

                    ret = ConvertEntities.ConvertMemberOperation(linkingResult);
                }
                else
                {
                    ret = MemberRegistrationResult.MemberNotFound;
                }
            }

            return ret;
        }


        /// <summary>
        /// Generate a non duplicate name for new users
        /// </summary>
        /// <param name="accountCreationTime"></param>
        /// <returns></returns>
        private static string GenerateName(DateTime accountCreationTime)
        {
            int timestamp = Php.ConvertToTimestamp(accountCreationTime);
            Random random = new Random();
            string name = String.Format("{0}{1}{2}", timestamp, accountCreationTime.ToString("fff"), random.Next(100).ToString("D2"));

            if (IsDuplicateUserName(name))
            {
                name = GenerateUserName(name, true);

                //List<string> nonDuplicateNames = GenerateNonDuplicateUserNames(name, 3);

                //if (nonDuplicateNames.Count > 0)
                //{
                //    name = nonDuplicateNames[0];
                //}
                //else
                //{
                //    throw new InvalidOperationException(String.Format("Failed to generate a non duplicate name for {0}.", name));
                //}
            }

            return name;
        }

        #endregion

        #region Change member data

        /// <summary>
        /// Link a Cmid to an Esns Handle
        /// </summary>
        /// <param name="esnsId"></param>
        /// <param name="esnsUserId"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <returns></returns>
        private static MemberOperationResult AttachEsnsAccount(EsnsType esnsId, string esnsUserId, int cmid, bool checkDuplicate)
        {
            return AttachEsnsAccount(esnsId, esnsUserId, cmid, checkDuplicate, DateTime.Now);
        }

        /// <summary>
        /// Link a Cmid to an Esns Handle
        /// </summary>
        /// <param name="esnsId"></param>
        /// <param name="esnsUserId"></param>
        /// <param name="cmid"></param>
        /// <param name="checkDuplicate"></param>
        /// <param name="creationTime"></param>
        /// <returns></returns>
        private static MemberOperationResult AttachEsnsAccount(EsnsType esnsId, string esnsUserId, int cmid, bool checkDuplicate, DateTime creationTime)
        {
            MemberOperationResult ret = MemberOperationResult.Ok;

            if (checkDuplicate && CmuneMember.IsDuplicateHandle(esnsId, esnsUserId))
            {
                ret = MemberOperationResult.DuplicateHandle;
            }

            if (ret.Equals(MemberOperationResult.Ok))
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    ESNSIdentity newEsnsIdendity = new ESNSIdentity();
                    newEsnsIdendity.AutoAccept = false;
                    newEsnsIdendity.CMID = cmid;
                    newEsnsIdendity.Handle = esnsUserId;
                    newEsnsIdendity.IsVerified = true;
                    newEsnsIdendity.LastSyncDate = creationTime;
                    newEsnsIdendity.Type = (int)esnsId;

                    cmuneDb.ESNSIdentities.InsertOnSubmit(newEsnsIdendity);
                    cmuneDb.SubmitChanges();
                }
            }

            return ret;
        }

        /// <summary>
        /// Transforms a member coming from an ESNS to a full Cmune member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MemberOperationResult RegisterEsnsMember(int cmid, string email, string password)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                string standardizedEmail = ValidationUtilities.StandardizeEmail(email);
                string encryptedPassword = Crypto.fncSHA256Encrypt(password);

                #region Input validation

                if (!ValidationUtilities.IsValidEmailAddress(email))
                {
                    ret = MemberOperationResult.InvalidEmail;
                }
                if (!ValidationUtilities.IsValidPassword(password))
                {
                    ret = MemberOperationResult.InvalidPassword;
                }

                #endregion Input validation

                if (ret.Equals(MemberOperationResult.MemberNotFound))
                {
                    bool isDuplicateEmail = IsDuplicateUserEmail(email);

                    if (!isDuplicateEmail)
                    {
                        Member cmuneMember = CmuneMember.GetMember(cmid, cmuneDB);

                        if (cmuneMember != null)
                        {
                            cmuneMember.Login = standardizedEmail;
                            cmuneMember.EmailAddressState = (byte)EmailAddressStatus.Unverified;
                            cmuneMember.IsRegistered = true;
                            cmuneMember.Password = encryptedPassword;

                            cmuneDB.SubmitChanges();

                            ret = MemberOperationResult.Ok;
                        }
                    }
                    else
                    {
                        ret = MemberOperationResult.DuplicateEmail;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Change member password
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="newPassword">The password should be hashed</param>
        /// <returns></returns>
        public static bool ChangeMemberPassword(int cmid, string newPassword)
        {
            bool ret = false;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                Member cmuneMember = CmuneMember.GetMember(cmid, cmuneDB);

                if (cmuneMember != null)
                {
                    cmuneMember.Password = newPassword;
                    cmuneDB.SubmitChanges();

                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Request a new password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="applicationName"></param>
        /// <param name="resetUrl">Should contain a {0} that will be replaced by the cmid and a {1} that will be replaced by the hash</param>
        /// <param name="accountUrl"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static MemberOperationResult RequestNewPassword(string email, string applicationName, string resetUrl, string accountUrl, string ip)
        {
            MemberOperationResult ret = MemberOperationResult.InvalidData;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Member member = GetMember(email, cmuneDb);

                if (member != null)
                {
                    string newPassword = GeneratePassword();
                    string newPasswordHashed = CryptographyUtilities.HashPassword(newPassword);

                    string validationHash = GenerateValidationHash();

                    PasswordResetHashe passwordReset = new PasswordResetHashe();
                    passwordReset.Cmid = member.CMID;
                    passwordReset.NewPassword = newPasswordHashed;
                    passwordReset.ResetDate = DateTime.Now;
                    passwordReset.SourceIp = TextUtilities.InetAToN(ip);
                    passwordReset.ValidationHash = validationHash;

                    cmuneDb.PasswordResetHashes.InsertOnSubmit(passwordReset);
                    cmuneDb.SubmitChanges();

                    resetUrl = String.Format(resetUrl, member.CMID, validationHash);

                    ID3 memberId3 = CmuneMember.GetId3(member.CMID);

                    if (memberId3 != null)
                    {
                        CmuneMail.SendEmailNewPassword(member.Login, memberId3.Name, newPassword, applicationName, accountUrl, resetUrl, (EmailAddressStatus)member.EmailAddressState);

                        ret = MemberOperationResult.Ok;
                    }
                    else
                    {
                        ret = MemberOperationResult.MemberNotFound;
                    }
                }
                else
                {
                    ret = MemberOperationResult.MemberNotFound;
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets a PasswordResetHash
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static PasswordResetHashe GetPasswordResetHash(int cmid, string hash)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                PasswordResetHashe passwordResetHash = cmuneDb.PasswordResetHashes.SingleOrDefault(p => p.Cmid == cmid && p.ValidationHash == hash);

                return passwordResetHash;
            }
        }

        /// <summary>
        /// Deletes all the password reset hashes of a specific member
        /// </summary>
        /// <param name="cmid"></param>
        public static void DeletePasswordResetHashes(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<PasswordResetHashe> passwordResetHashes = cmuneDb.PasswordResetHashes.Where(p => p.Cmid == cmid).ToList();
                cmuneDb.PasswordResetHashes.DeleteAllOnSubmit(passwordResetHashes);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Generates a random password for our members when needed
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            return RandomPassword.Generate(6, 8);
        }

        /// <summary>
        /// Generates a standard hash for any kind of validation
        /// </summary>
        /// <returns></returns>
        public static string GenerateValidationHash()
        {
            Guid guid = Guid.NewGuid();
            string temp = guid.ToString();
            string validationCode = temp.Substring(0, 8);

            return validationCode;
        }

        /// <summary>
        /// Change Member name
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="name"></param>
        /// <param name="checkDuplicate"></param>
        /// <param name="locale"></param>
        /// <param name="sourceIp"></param>
        /// <param name="recordPreviousName"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeMemberName(int cmid, string name, bool checkDuplicate, string locale, string sourceIp, bool recordPreviousName = true)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;

                MemberOperationResult isNameFullyValid = ValidationUtils.IsMemberNameFullyValid(name, locale);

                if (isNameFullyValid.Equals(MemberOperationResult.Ok))
                {
                    ID3 cmuneMember = CmuneMember.GetId3(cmid, cmuneDB);

                    if (cmuneMember != null)
                    {
                        bool isDuplicatedMemberName = false;

                        if (checkDuplicate)
                        {
                            isDuplicatedMemberName = CmuneMember.IsDuplicateUserName(name);
                        }

                        if (!isDuplicatedMemberName)
                        {
                            if (recordPreviousName)
                            {
                                PreviousName previousName = new PreviousName();
                                previousName.ChangeDate = DateTime.Now;
                                previousName.Cmid = cmid;
                                previousName.PreviousUserName = cmuneMember.Name;
                                previousName.SourceIp = TextUtilities.InetAToN(sourceIp);

                                cmuneDB.PreviousNames.InsertOnSubmit(previousName);
                            }

                            ret = MemberOperationResult.Ok;

                            cmuneMember.Name = ValidationUtilities.StandardizeMemberName(name);
                            cmuneDB.SubmitChanges();
                        }
                        else
                        {
                            ret = MemberOperationResult.DuplicateName;
                        }
                    }
                }
                else
                {
                    ret = isNameFullyValid;
                }

                return ret;
            }
        }

        /// <summary>
        /// Change Member Email
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="sourceIp"></param>
        public static void ChangeMemberEmail(int cmid, string email, string sourceIp)
        {
            // TODO -> To recode whith an extra argument (bool to check duplicate). Change the return type also

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                string standardizedEmail = ValidationUtilities.StandardizeEmail(email);

                Member cmuneMember = CmuneMember.GetMember(cmid, cmuneDB);

                // We need to record the previous email for security reason (if we have one)
                if (cmuneMember.Login != null)
                {
                    PreviousEmail previousEmail = new PreviousEmail();
                    previousEmail.ChangeDate = DateTime.Now;
                    previousEmail.Cmid = cmid;
                    previousEmail.PreviousEmailAddress = cmuneMember.Login;
                    previousEmail.SourceIp = TextUtilities.InetAToN(sourceIp);

                    cmuneDB.PreviousEmails.InsertOnSubmit(previousEmail);
                }

                cmuneMember.EmailAddressState = (byte)EmailAddressStatus.Unverified;

                cmuneMember.Login = standardizedEmail;

                // We need to delete all the identity validations of the member to avoid him to validate his new email address with an email received on the old email address
                List<IdentityValidation> emailValidations = cmuneDB.IdentityValidations.Where<IdentityValidation>(iV => iV.CMID == cmid && iV.type == (int)IdentityValidationType.Email).ToList();
                cmuneDB.IdentityValidations.DeleteAllOnSubmit(emailValidations);

                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Get the total number of Cmune users
        /// </summary>
        /// <returns></returns>
        public static int GetUserCount()
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                int nbUser = 0;
                nbUser = (from tab in cmuneDB.ID3s select tab).Count();
                return nbUser;
            }
        }

        #region Getters

        /// <summary>
        /// Checks whether a member is activated or not
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static EmailAddressStatus GetEmailAddressStatus(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                EmailAddressStatus emailStatus = EmailAddressStatus.Unverified;

                Member member = cmuneDb.Members.SingleOrDefault<Member>(m => m.CMID == cmid);

                if (member != null)
                {
                    emailStatus = (EmailAddressStatus)member.EmailAddressState;
                }

                return emailStatus;
            }
        }

        /// <summary>
        /// Checks whether a member is only playing on an ESNS an never did a full registration (Password + email)
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool IsMemberEsnsOnly(int cmid)
        {
            bool isMemberEsnsOnly = false;

            #region Chek input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be >= 1.");
            }

            #endregion

            Member member = CmuneMember.GetMember(cmid);

            isMemberEsnsOnly = IsMemberEsnsOnly(member);

            return isMemberEsnsOnly;
        }

        /// <summary>
        /// Checks whether a member is only playing on an ESNS an never did a full registration (Password + email)
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsMemberEsnsOnly(Member member)
        {
            bool isMemberEsnsOnly = false;

            if (member != null)
            {
                isMemberEsnsOnly = TextUtilities.IsNullOrEmpty(member.Login) && TextUtilities.IsNullOrEmpty(member.Password);
            }

            return isMemberEsnsOnly;
        }

        #endregion

        /// <summary>
        /// Updates members balance
        /// </summary>
        /// <param name="points"></param>
        /// <param name="credits"></param>
        /// <param name="targetCmid"></param>
        /// <param name="sourceCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        public static void UpdateMemberBalance(int points, int credits, int targetCmid, int sourceCmid, int applicationId, string sourceIp, ModerationActionType actionType, string reason)
        {
            if (points >= 0)
            {
                CmuneEconomy.AttributePoints(targetCmid, points, true, PointsDepositType.Admin);
            }
            else
            {
                CmuneEconomy.RemovePoints(targetCmid, points, true);
            }

            if (credits >= 0)
            {
                CmuneEconomy.AttributeCredits(targetCmid, credits);
            }
            else
            {
                CmuneEconomy.RemoveCreditsToMember(targetCmid, -credits, true);
            }

            if (points != 0 || credits != 0)
            {
                reason = String.Format("Wallet modification: {0} points and {1} credits / {2}", points, credits, reason);

                RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, DateTime.Now, actionType, reason);
            }
        }

        /// <summary>
        /// Check if the user is validated
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="isStatsFrozen"></param>
        /// <returns></returns>
        public static void SetMemberStatsStatus(int memberID, Boolean isStatsFrozen)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                Member memberToModify = cmuneDB.Members.SingleOrDefault<Member>(f => f.CMID == memberID);
                memberToModify.IsStatsFrozen = isStatsFrozen;
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Set the access level of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="newMemberAccessLevel"></param>
        /// <returns></returns>
        public static bool SetMemberAccess(int cmid, int applicationId, MemberAccessLevel newMemberAccessLevel)
        {
            bool isAccessLevelUpdated = false;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberAccess memberAccess = GetMemberAccess(cmid, applicationId, cmuneDB);

                if (memberAccess != null)
                {
                    memberAccess.AccessLevel = (int)newMemberAccessLevel;
                    cmuneDB.SubmitChanges();

                    isAccessLevelUpdated = true;
                }
            }

            return isAccessLevelUpdated;
        }

        #endregion

        #region Login

        /// <summary>
        /// Log in the Cmune member with his email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="applicationId"></param>
        /// <param name="loggedMember"></param>
        /// <returns></returns>
        public static MemberAuthenticationResult CmuneLoginEmail(string email, string password, int applicationId, out Member loggedMember)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberAuthenticationResult memberAuthMessage = MemberAuthenticationResult.InvalidData;

                email = ValidationUtilities.StandardizeEmail(email);
                loggedMember = null;

                if (ValidationUtilities.IsValidEmailAddress(email) && ValidationUtilities.IsValidPassword(password))
                {
                    Member memberToLoad = cmuneDB.Members.SingleOrDefault<Member>(m => m.Login == email);

                    if (memberToLoad != null)
                    {
                        if (memberToLoad.Password.Equals(CryptographyUtilities.HashPassword(password)))
                        {
                            MemberAccess memberAccess = cmuneDB.MemberAccesses.SingleOrDefault<MemberAccess>(mA => mA.Cmid == memberToLoad.CMID && mA.ApplicationId == applicationId);

                            loggedMember = memberToLoad;

                            if (memberAccess.IsAccountDisabled == (int)BanMode.No)
                            {
                                memberAuthMessage = MemberAuthenticationResult.Ok;
                            }
                            else
                            {
                                memberAuthMessage = MemberAuthenticationResult.IsBanned;
                            }
                        }
                        else
                        {
                            memberAuthMessage = MemberAuthenticationResult.InvalidPassword;
                        }
                    }
                    else
                    {
                        memberAuthMessage = MemberAuthenticationResult.InvalidEmail;
                    }
                }

                return memberAuthMessage;
            }
        }

        public static LuckyDrawUnityView GetDailyLuckyDrawOnLogin(int cmid)
        {
            LuckyDrawView luckyDraw = null;
            bool dailyLogin = false;
            var deposit = GetLastLoginLuckyDraw(cmid);

            // user never got a daliy lucky draw before
            if (deposit == null)
            {
                // is an old user (from before we changed daily points to daily lucky draw)
                if (HasPointDeposit(cmid))
                {
                    dailyLogin = true;
                }
                else
                {
                    var welcomePackage = GetSignupLuckyDraw(cmid);

                    // is a new user, give him the starter package
                    if (welcomePackage == null)
                    {
                        var draws = CmuneLuckyDrawService.GetLuckyDraws(true, false).Where(d => d.Category == BundleCategoryType.Signup).ToList();
                        if (draws != null && draws.Count > 0)
                            luckyDraw = draws[new Random().Next(draws.Count)];
                    }
                    // returning user that is claiming his daily LD for the 1st time
                    else
                    {
                        dailyLogin = true;
                    }
                }
            }
            // returning user
            else if (DateTime.Now.Subtract(deposit.TransactionDate).Days > 0)
            {
                dailyLogin = true;
            }

            if (dailyLogin)
            {
                var draws = CmuneLuckyDrawService.GetLuckyDraws(true, false).Where(d => d.Category == BundleCategoryType.Login).ToList();
                if (draws != null && draws.Count > 0)
                    luckyDraw = draws[new Random().Next(draws.Count)];
            }

            return luckyDraw.ToLuckyDrawUnityView();
        }

        /// <summary>
        /// Attributes points to a member if he didn't login since more than 24 hours
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static int AttributeLoginPoints(int cmid)
        {
            int pointsAttributed = 0;
            DateTime? lastLoginDate = null;
            bool doDeposit = true;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                lastLoginDate = (from m in cmuneDb.Members
                                 where m.CMID == cmid
                                 select m.LastAliveAck).SingleOrDefault();
            }

            DateTime currentTime = DateTime.Now;

            if (lastLoginDate.HasValue)
            {
                PointDeposit lastLogOnDeposit = GetLastLoginDeposit(cmid);

                if (lastLogOnDeposit == null)
                {
                    pointsAttributed = CommonConfig.LoginMinPointsPerDay;
                }
                else
                {
                    int daysSinceLastLoginDeposit = currentTime.Subtract(lastLogOnDeposit.DepositDate).Days;
                    int lastDeposit = lastLogOnDeposit.NbPoints;

                    // We can't get points it last login deposit was less than 24 hours ago
                    if (daysSinceLastLoginDeposit >= 1)
                    {
                        // To attribute points we should consider the last login date, not the last login deposit date
                        int daysSinceLastLogin = currentTime.Subtract(lastLoginDate.Value).Days;

                        if (daysSinceLastLogin < 2)
                        {
                            pointsAttributed = lastDeposit + CommonConfig.LoginDailyGrowth;
                        }
                        else
                        {
                            pointsAttributed = lastDeposit - CommonConfig.LoginDailyDecay * daysSinceLastLogin;
                        }

                        if (pointsAttributed > CommonConfig.LoginMaxPointsPerDay)
                        {
                            pointsAttributed = CommonConfig.LoginMaxPointsPerDay;
                        }
                        else if (pointsAttributed < CommonConfig.LoginMinPointsPerDay)
                        {
                            pointsAttributed = CommonConfig.LoginMinPointsPerDay;
                        }
                    }
                    else
                    {
                        doDeposit = false;
                    }
                }

                if (doDeposit)
                {
                    CmuneEconomy.AttributePoints(cmid, pointsAttributed, false, PointsDepositType.Login);
                }
            }

            return pointsAttributed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static PointDeposit GetLastLoginDeposit(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                PointDeposit lastLogOnDeposit = cmuneDb.PointDeposits.Where(p => p.UserId == cmid && p.DepositType == (int)PointsDepositType.Login).OrderByDescending(p => p.id).Take(1).SingleOrDefault();

                return lastLogOnDeposit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool HasPointDeposit(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.PointDeposits.Any(p => p.UserId == cmid && p.DepositType == (int)PointsDepositType.Login);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static BoxTransaction GetLastLoginLuckyDraw(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.BoxTransactions.Where(p => p.Cmid == cmid && p.Category == (int)BundleCategoryType.Login).OrderByDescending(p => p.Id).Take(1).SingleOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static BoxTransaction GetSignupLuckyDraw(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.BoxTransactions.Where(p => p.Cmid == cmid && p.Category == (int)BundleCategoryType.Signup).OrderByDescending(p => p.Id).Take(1).SingleOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="channel"></param>
        /// <param name="networkAddress"></param>
        /// <param name="machineId"></param>
        public static void RecordLogin(int cmid, ChannelType channel, long networkAddress, string machineId)
        {
            DateTime currentTime = DateTime.Now;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Member member = GetMember(cmid, cmuneDb);

                if (member != null)
                {
                    member.LastAliveAck = currentTime;
                    // TODO: To remove
                    member.LastChannel = (int)channel;
                    // TODO: To remove
                    member.LastIp = networkAddress;

                    LoginIp loginIp = new LoginIp();
                    loginIp.Cmid = cmid;
                    loginIp.Ip = networkAddress;
                    loginIp.LoginDate = currentTime;
                    loginIp.Channel = (int)channel;
                    loginIp.MachineId = machineId;

                    cmuneDb.LoginIps.InsertOnSubmit(loginIp);

                    cmuneDb.SubmitChanges();
                }
            }
        }

        #endregion

        #region Duplicate check

        /// <summary>
        /// Check duplicate email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsDuplicateUserEmail(string email)
        {
            bool ret = false;

            if (!TextUtilities.IsNullOrEmpty(email))
            {
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    email = ValidationUtilities.StandardizeEmail(email);

                    if (cmuneDb.Members.Where(m => m.Login == email).Select(m => m.Login).ToList().Count() > 0)
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Check duplicate user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsDuplicateUserName(string userName)
        {
            bool ret = false;

            if (userName != null)
            {
                userName = ValidationUtilities.StandardizeMemberName(userName);

                if (!TextUtilities.IsNullOrEmpty(userName))
                {
                    using (CmuneDataContext cmuneDB = new CmuneDataContext())
                    {
                        ID3 id3 = cmuneDB.ID3s.SingleOrDefault<ID3>(f => f.Name == userName);
                        if (id3 != null)
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// This method generates unique user name, by appending a random number of 5 digits to an existing string.
        /// Use the checkAvailablity flag to ensure the name is unique and not already taken.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="checkAvailablity"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string GenerateUserName(string userName, bool checkAvailablity = true, int seed = 0)
        {
            //first check if the base string is ok
            if (!ValidationUtilities.IsValidMemberName(userName))
            {
                throw new Exception(string.Format("Error generating a unique user name because base string '{0}' is not a valid", userName));
            }

            int suffixLength = 5;
            string newName = string.Empty;
            Random random = new Random(seed == 0 ? DateTime.Now.Millisecond : seed);

            //userName = TextUtilities.CompleteTrim(userName);
            userName = userName.ShortenText(CommonConfig.MemberNameMaxLength - suffixLength);

            //if the base string is too short, extend the suffix accordingly
            if (userName.Length < CommonConfig.MemberNameMinLength)
            {
                suffixLength += CommonConfig.MemberNameMinLength - userName.Length;
            }

            if (checkAvailablity)
            {
                bool isNameTaken = true;
                int counter = 0;
                int range = (int)Math.Pow(10, suffixLength);

                //loop (max 10) until we find an available user name
                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    while (isNameTaken && counter < 10)
                    {
                        newName = userName + random.Next(range).ToString(String.Format("D{0}", suffixLength));
                        isNameTaken = cmuneDb.ID3s.Where(i => i.Name == newName).Count() != 0;
                        counter++;
                    }
                }

                if (isNameTaken && counter == 10)
                {
                    throw new Exception(string.Format("Error generating a unique user name based on the string: '{0}'", userName));
                }
            }
            else
            {
                newName = userName + random.Next((int)Math.Pow(10, suffixLength)).ToString(String.Format("D{0}", suffixLength));
            }

            return newName;
        }

        /// <summary>
        /// This method generates up to [count] unique user names that are based on an existing string.
        /// Of all generated names we only return the subset of names that are non duplicate.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<string> GenerateNonDuplicateUserNames(string userName, int count)
        {
            int seed = DateTime.Now.Millisecond;
            List<string> names = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                names.Add(GenerateUserName(userName, false, ++seed));
            }

            //ensure that all generated names are unique
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var takenNames = cmuneDb.ID3s.Where(i => names.Contains(i.Name)).Select(i => i.Name).ToList();
                names.RemoveAll(n => takenNames.Contains(n, StringComparer.CurrentCultureIgnoreCase));
            }

            return names;
        }

        /// <summary>
        /// Checks wether email and username are duplicated or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static MemberOperationResult AreDuplicateEmailUserName(string userName, string email)
        {
            MemberOperationResult ret = MemberOperationResult.Ok;
            bool isDuplicateUsername = CmuneMember.IsDuplicateUserName(userName);
            bool isDuplicateEmail = CmuneMember.IsDuplicateUserEmail(email);

            if (isDuplicateEmail && isDuplicateUsername)
            {
                ret = MemberOperationResult.DuplicateEmailName;
            }
            else if (isDuplicateEmail)
            {
                ret = MemberOperationResult.DuplicateEmail;
            }
            else if (isDuplicateUsername)
            {
                ret = MemberOperationResult.DuplicateName;
            }

            return ret;
        }

        /// <summary>
        /// Check wether a handle is duplicated
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="esnsId"></param>
        /// <returns></returns>
        public static bool IsDuplicateHandle(EsnsType esnsId, string handle)
        {
            bool isDuplicateHandle = false;

            if (!handle.IsNullOrFullyEmpty())
            {
                handle = handle.Trim().ToLower();

                using (CmuneDataContext cmuneDB = new CmuneDataContext())
                {
                    ESNSIdentity esnsIdenty = cmuneDB.ESNSIdentities.SingleOrDefault<ESNSIdentity>(e => e.Handle == handle && e.Type == (int)esnsId);

                    if (esnsIdenty != null)
                    {
                        isDuplicateHandle = true;
                    }
                }
            }

            return isDuplicateHandle;
        }

        /// <summary>
        /// Check if the email is already linked to an Esns specified by EsnsType
        /// </summary>
        /// <param name="email"></param>
        /// <param name="esnsId"></param>
        /// <returns></returns>
        public static bool IsEmailAlreadyLinkedToEsns(string email, EsnsType esnsId)
        {
            bool isLinked = false;

            CmuneDataContext cmuneDb = new CmuneDataContext();
            var member = CmuneMember.GetMember(email, cmuneDb);

            if (member != null)
            {
                if (member.ESNSIdentities.Where(d => d.Type == (int)esnsId).Count() > 0)
                {
                    isLinked = true;
                }
            }
            return isLinked;
        }

        #endregion

        #region Get members and Id3

        /// <summary>
        /// Get a READ ONLY member thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static Member GetMember(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetMember(cmid, cmuneDB);
            }
        }

        /// <summary>
        /// Get a member thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Member GetMember(int cmid, CmuneDataContext cmuneDB)
        {
            Member member = null;

            if (cmid > 0 && cmuneDB != null)
            {
                member = cmuneDB.Members.SingleOrDefault<Member>(f => f.CMID == cmid);
            }

            return member;
        }

        /// <summary>
        /// Get the Esns handles linked to a cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ESNSIdentity> GetMemberEsns(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<ESNSIdentity> esns = (from tab in cmuneDb.ESNSIdentities
                                           where tab.CMID == cmid
                                           select tab).ToList();
                return esns;
            }
        }

        /// <summary>
        /// Exact match
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetCmidByName(string name)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int cmid = 0;

                name = ValidationUtilities.StandardizeMemberName(name);

                var query = (from i in cmuneDb.ID3s
                             where i.Name == name
                             select new { Cmid = i.CMID }).Take(1);

                foreach (var row in query)
                {
                    cmid = row.Cmid;
                }

                return cmid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static int GetCmidByEmail(string emailAddress)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int cmid = 0;

                emailAddress = ValidationUtilities.StandardizeEmail(emailAddress);

                var query = (from m in cmuneDb.Members
                             where m.Login == emailAddress
                             select new { Cmid = m.CMID }).Take(1);

                foreach (var row in query)
                {
                    cmid = row.Cmid;
                }

                return cmid;
            }
        }

        /// <summary>
        /// Get the CMID owning a Handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetCmidByEsnsId(string handle, EsnsType type)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int cmid = (from e in cmuneDb.ESNSIdentities
                            where e.Type == (int)type && e.Handle == handle
                            select new { Cmid = e.CMID }).Select(q => q.Cmid).SingleOrDefault();

                return cmid;
            }
        }

        /// <summary>
        /// Get the Cmid linked to a Cyworld Id
        /// </summary>
        /// <param name="cyworldId"></param>
        /// <returns></returns>
        public static int GetCmidByCyworldId(int cyworldId)
        {
            int cmid = 0;

            cmid = GetCmidByEsnsId(cyworldId.ToString(), EsnsType.Cyworld);

            return cmid;
        }

        /// <summary>
        /// Get the Cmid linked to a Facebook Id
        /// </summary>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        public static int GetCmidByFacebookId(long facebookId)
        {
            int cmid = 0;

            cmid = GetCmidByEsnsId(facebookId.ToString(), EsnsType.Facebook);

            return cmid;
        }

        /// <summary>
        /// Get the Cmid linked to Kongregate Id
        /// </summary>
        /// <param name="kongregateId"></param>
        /// <returns></returns>
        public static int GetCmidByKongregateId(string kongregateId)
        {
            int cmid = 0;

            cmid = GetCmidByEsnsId(kongregateId, EsnsType.Kongregate);
            return cmid;
        }

        /// <summary>
        /// Get the Cmune Member linked to a specific Facebook ID
        /// </summary>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        public static Member GetMemberByFacebookId(long facebookId)
        {
            Member member = GetMember(facebookId.ToString(), EsnsType.Facebook);

            return member;
        }

        /// <summary>
        /// Get the Cmune Member linked to a specific Facebook Id
        /// </summary>
        /// <param name="facebookId"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Member GetMemberByFacebookId(long facebookId, CmuneDataContext cmuneDB)
        {
            return GetMember(facebookId.ToString(), EsnsType.Facebook, cmuneDB);
        }

        /// <summary>
        /// Get the Cmune Member with the specified kongregateId
        /// </summary>
        /// <param name="kongregateId"></param>
        /// <returns></returns>
        public static Member GetMemberByKongregateId(string kongregateId)
        {
            return GetMember(kongregateId.ToString(), EsnsType.Kongregate);
        }

        /// <summary>
        /// Get the member matching with a Handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="handleType"></param>
        /// <returns></returns>
        public static Member GetMember(string handle, EsnsType handleType)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetMember(handle, handleType, cmuneDB);
            }
        }

        /// <summary>
        /// Get the member matching with a Handle
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="handleType"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Member GetMember(string handle, EsnsType handleType, CmuneDataContext cmuneDB)
        {
            Member member = null;

            if (cmuneDB != null)
            {
                //try
                //{
                ESNSIdentity esnsIdentity = cmuneDB.ESNSIdentities.SingleOrDefault<ESNSIdentity>(e => e.Handle == handle.ToLower() && e.Type == (int)handleType);

                if (esnsIdentity != null)
                {
                    member = esnsIdentity.Member;
                }
                //}
                //catch (InvalidCastException ex)
                //{
                //    CmuneLog.LogException(ex, String.Format("handle={0}&handleType={1}", handle, handleType));
                //    throw ex;
                //}
            }

            return member;
        }

        /// <summary>
        /// Get the Cmids linked to a list of Esns Ids
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetCmidListByEsnsId(List<String> handles, EsnsType type)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Dictionary<String, int> cmids = new Dictionary<String, int>();

                var query = (from tab in cmuneDb.ESNSIdentities
                             where tab.Type == (int)type && handles.Contains(tab.Handle)
                             select new { Handle = tab.Handle, Cmid = tab.CMID }).ToList();

                foreach (var row in query)
                {
                    cmids.Add(row.Handle, row.Cmid);
                }

                return cmids;
            }
        }

        /// <summary>
        /// Get Cmids linked to Esnds Ids
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="esnsId"></param>
        /// <returns></returns>
        public static List<int> GetCmidByEsnsId(List<string> handles, EsnsType esnsId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<int> cmids = (from e in cmuneDb.ESNSIdentities
                                   where e.Type == (int)esnsId && handles.Contains(e.Handle)
                                   select new { Cmid = e.CMID }).Select(q => q.Cmid).ToList();

                return cmids;
            }
        }

        /// <summary>
        /// Get Cmids linked to Facebook Ids
        /// </summary>
        /// <param name="facebookIds"></param>
        /// <returns></returns>
        public static List<int> GetCmidByFacebookId(List<long> facebookIds)
        {
            return GetCmidByEsnsId(facebookIds.ConvertAll(q => q.ToString()), EsnsType.Facebook);
        }

        /// <summary>
        /// Get the READ ONLY Id3 of a member thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static ID3 GetId3(int cmid)
        {
            // TODO -> Method buggy: there is more than one ID3 by member -> We need to define a special type

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetId3(cmid, cmuneDB);
            }
        }

        /// <summary>
        /// Get the Id3 of a member thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static ID3 GetId3(int cmid, CmuneDataContext cmuneDB)
        {
            // TODO -> Method buggy: there is more than one ID3 by member -> We need to define a special type

            ID3 memberId3 = null;

            if (cmid > 0 && cmuneDB != null)
            {
                memberId3 = cmuneDB.ID3s.SingleOrDefault<ID3>(f => f.CMID == cmid);
            }

            return memberId3;
        }

        /// <summary>
        /// Get the name of a user
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static string GetUserName(int cmid)
        {
            // TODO -> Method buggy: there is more than one ID3 by member -> We need to define a special type

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                string userName = cmuneDb.ID3s.Where(i => i.CMID == cmid).Select(i => i.Name).SingleOrDefault();

                return userName;
            }
        }

        /// <summary>
        /// Get the email address of a user
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static string GetUserEmail(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                string emailAddress = cmuneDb.Members.Where(m => m.CMID == cmid).Select(i => i.Login).SingleOrDefault();

                return emailAddress;
            }
        }

        /// <summary>
        /// Retrieves the names of a list of members
        /// </summary>
        /// <param name="cmids"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetMembersNames(ICollection<int> cmids)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Dictionary<int, string> membersName = new Dictionary<int, string>();
                List<ID3> id3s = new List<ID3>();

                // Max number of param is 2100 BUT the whole query string can't be longer than 8044 characters
                int tdsLimit = 1000;
                int i = 0;

                while (i < cmids.Count)
                {
                    List<int> cmidsTmps = cmids.Skip(i).Take(tdsLimit).ToList();
                    id3s.AddRange(cmuneDb.ID3s.Where(id3 => cmidsTmps.Contains(id3.CMID)).ToList());
                    i += tdsLimit;
                }

                membersName = id3s.ToDictionary(id3 => id3.CMID, id3 => id3.Name);

                return membersName;
            }
        }

        /// <summary>
        /// Retrieves the Id3 of a list of members
        /// </summary>
        /// <param name="cmids"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Dictionary<int, ID3> GetMembersId3(List<int> cmids, CmuneDataContext cmuneDB)
        {
            Dictionary<int, ID3> membersId3 = new Dictionary<int, ID3>();

            if (cmuneDB != null)
            {
                membersId3 = cmuneDB.ID3s.Where(id3 => cmids.Contains(id3.CMID)).ToDictionary(id3 => id3.CMID);
            }

            return membersId3;
        }

        /// <summary>
        /// Gets a member thanks to a CMID
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static Member GetMember(String email, CmuneDataContext cmuneDB)
        {
            Member member = null;

            email = ValidationUtilities.StandardizeEmail(email);

            if (!TextUtilities.IsNullOrEmpty(email) && cmuneDB != null)
            {
                member = cmuneDB.Members.SingleOrDefault<Member>(f => f.Login == email);
            }

            return member;
        }

        /// <summary>
        /// Get the CMID from diverse information
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="handle"></param>
        /// <param name="esnsType"></param>
        /// <param name="byExactName"></param>
        /// <returns>the id of the Cmune user, 0 otherwise</returns>
        public static List<int> GetCmidFromInfo(string email, string memberName, string handle, EsnsType esnsType = EsnsType.Facebook, bool byExactName = true)
        {
            List<int> cmids = new List<int>();
            bool userfound = false;

            if (!userfound && !memberName.IsNullOrFullyEmpty())
            {
                if (byExactName)
                {
                    int cmid = GetCmidByName(memberName);

                    if (cmid != 0)
                    {
                        cmids.Add(cmid);
                    }
                }
                else
                {
                    using (CmuneDataContext cmuneDB = new CmuneDataContext())
                    {
                        var cmidsByName = (from i in cmuneDB.ID3s
                                           join m in cmuneDB.Members on i.CMID equals m.CMID
                                           where SqlMethods.Like(i.Name, "%" + memberName + "%")
                                           orderby m.LastAliveAck descending
                                           select new { Cmid = i.CMID }).ToList();

                        for (int i = 0; i < cmidsByName.Count; i++)
                        {
                            cmids.Add(cmidsByName[i].Cmid);
                        }
                    }
                }
            }

            if (!userfound && !email.IsNullOrFullyEmpty())
            {
                int cmid = GetCmidByEmail(email);

                if (cmid != 0)
                {
                    cmids.Add(cmid);
                    userfound = true;
                }
            }

            if (!userfound && !handle.IsNullOrFullyEmpty())
            {
                int cmid = GetCmidByEsnsId(handle, esnsType);

                if (cmid != 0)
                {
                    cmids.Add(cmid);
                    userfound = true;
                }
            }

            return cmids;
        }

        /// <summary>
        /// Find members by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public static Dictionary<int, string> FindMembers(string name, int maxResults)
        {
            Dictionary<int, string> members = new Dictionary<int, string>();

            if (!name.IsNullOrFullyEmpty() && maxResults > 0)
            {
                using (CmuneDataContext cmuneDB = new CmuneDataContext())
                {
                    if (maxResults > 100)
                    {
                        maxResults = 100;
                    }

                    members = (from tab in cmuneDB.ID3s
                               where SqlMethods.Like(tab.Name, "%" + name + "%")
                               select tab).Take(maxResults).ToDictionary(id3 => id3.CMID, id3 => id3.Name);
                }
            }

            return members;
        }

        /// <summary>
        /// Load a member thanks to his name
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static Member GetMemberByName(string memberName)
        {
            Member memberToLoad = null;

            memberName = ValidationUtilities.StandardizeMemberName(memberName);

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {

                ID3 id3ToLoad = cmuneDB.ID3s.SingleOrDefault<ID3>(i => i.Name == memberName);
                if (id3ToLoad != null)
                {
                    memberToLoad = GetMember(id3ToLoad.CMID);
                }
            }

            return memberToLoad;
        }

        /// <summary>
        /// Gets the Public Profile matching with the CMID
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static PublicProfileView GetPublicProfile(int cmid, int applicationId)
        {
            PublicProfileView publicProfile = null;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ID3 memberID3 = GetId3(cmid, cmuneDb);

                MemberAccess memberAccess = cmuneDb.MemberAccesses.SingleOrDefault(mA => mA.Cmid == cmid && mA.ApplicationId == applicationId);

                Member member = GetMember(cmid, cmuneDb);

                if (memberID3 != null && memberAccess != null)
                {
                    bool isChatDisable = true;

                    if (memberAccess.IsChatDisabled == (int)BanMode.No)
                    {
                        isChatDisable = false;
                    }

                    publicProfile = new PublicProfileView(cmid, memberID3.Name, (MemberAccessLevel)memberAccess.AccessLevel, isChatDisable, member.TagName, (DateTime)member.LastAliveAck, (EmailAddressStatus)member.EmailAddressState);
                }
            }

            return publicProfile;
        }

        /// <summary>
        /// Gets the Member view matching with the CMID
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static MemberView GetMember(int cmid, int applicationId)
        {
            MemberView memberView = null;

            PublicProfileView publicProfile = GetPublicProfile(cmid, applicationId);
            MemberWalletView memberWallet = CmuneEconomy.GetMemberWallet(cmid);
            List<int> memberItems = GetCurrentItemIdInventory(cmid);

            if (publicProfile != null && memberWallet != null)
            {
                memberView = new MemberView(publicProfile, memberWallet, memberItems);
            }

            return memberView;
        }

        /// <summary>
        /// Retrieves the public profiles of a list of members
        /// </summary>
        /// <param name="cmids"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<PublicProfileView> GetPublicProfiles(List<int> cmids, int applicationId)
        {
            List<PublicProfileView> publicProfiles = new List<PublicProfileView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = from m in cmuneDb.Members
                            join i in cmuneDb.ID3s on m.CMID equals i.CMID
                            join mA in cmuneDb.MemberAccesses on m.CMID equals mA.Cmid
                            where cmids.Contains(m.CMID) && mA.ApplicationId == applicationId
                            orderby m.LastAliveAck descending
                            select new { Cmid = m.CMID, Name = i.Name, AccessLevel = (MemberAccessLevel)mA.AccessLevel, IsChatDisabled = (mA.IsChatDisabled == (int)BanMode.No), GroupTag = m.TagName, LastLogin = (DateTime)m.LastAliveAck, EmailAddressStatus = (EmailAddressStatus)m.EmailAddressState };

                foreach (var row in query)
                {
                    publicProfiles.Add(new PublicProfileView(row.Cmid, row.Name, row.AccessLevel, row.IsChatDisabled, row.GroupTag, row.LastLogin, row.EmailAddressStatus));
                }
            }

            return publicProfiles;
        }

        /// <summary>
        /// Get a READ ONLY member access thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static MemberAccess GetMemberAccess(int cmid, int applicationId)
        {
            MemberAccess memberAccess = null;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                memberAccess = GetMemberAccess(cmid, applicationId, cmuneDb);
            }

            return memberAccess;
        }

        /// <summary>
        /// Get a member access thanks to his Cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static MemberAccess GetMemberAccess(int cmid, int applicationId, CmuneDataContext cmuneDb)
        {
            MemberAccess memberAccess = null;

            if (cmuneDb != null)
            {
                memberAccess = cmuneDb.MemberAccesses.SingleOrDefault<MemberAccess>(mA => mA.Cmid == cmid && mA.ApplicationId == applicationId);
            }

            return memberAccess;
        }

        /// <summary>
        /// Get the non default member access
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<MemberAccess> GetNonDefaultMemberAccess(int applicationId)
        {
            List<MemberAccess> memberAccess = new List<MemberAccess>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                memberAccess = cmuneDb.MemberAccesses.Where(mA => mA.AccessLevel != (int)MemberAccessLevel.Default).OrderByDescending(mA => mA.AccessLevel).ToList();
            }

            return memberAccess;
        }

        /// <summary>
        /// Get the names of admins or moderators...
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="accessLevel"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetNonDefaultMembersNames(int applicationId, MemberAccessLevel accessLevel)
        {
            Dictionary<int, string> memberNames = new Dictionary<int, string>();

            if (accessLevel != MemberAccessLevel.Default)
            {
                List<int> memberCmids = new List<int>();

                using (CmuneDataContext cmuneDb = new CmuneDataContext())
                {
                    memberCmids = cmuneDb.MemberAccesses.Where(mA => mA.AccessLevel == (int)MemberAccessLevel.Admin).Select(m => m.Cmid).ToList();
                }

                memberNames = GetMembersNames(memberCmids);
            }

            return memberNames;
        }

        /// <summary>
        /// Get all the member access linked to a list of Cmids
        /// </summary>
        /// <param name="cmids"></param>
        /// <param name="applicationId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<MemberAccess> GetGetMembersAccess(List<int> cmids, int applicationId, CmuneDataContext cmuneDb)
        {
            List<MemberAccess> membersAccess = new List<MemberAccess>();

            if (cmuneDb != null)
            {
                membersAccess = cmuneDb.MemberAccesses.Where(mA => cmids.Contains(mA.Cmid)).ToList();
            }

            return membersAccess;
        }

        /// <summary>
        /// Gets the emails off all the members that didn't finish their registration outside of Facebook
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static List<string> GetNonFacebookUnfinishedRegistrationEmails(DateTime startingDate, DateTime endingDate, ChannelType channel)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<string> emailAddresses = (from m in cmuneDb.Members
                                               where !m.IsAccountComplete && m.LastAliveAck >= startingDate && m.LastAliveAck < endingDate && m.LastChannel == (int)channel
                                               select m.Login).ToList();

                return emailAddresses;
            }
        }

        /// <summary>
        /// Get the emails and names of all the users that didn't finish their regsitration on our Facebook application
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns>Key: Email address, Value: First name on Facebook</returns>
        public static Dictionary<string, string> GetFacebookUnfinishedRegistration(DateTime startingDate, DateTime endingDate)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                Dictionary<string, string> emailsAndNames = (from m in cmuneDb.Members
                                                             join i in cmuneDb.ID3s on m.CMID equals i.CMID
                                                             where !m.IsAccountComplete && m.LastAliveAck >= startingDate && m.LastAliveAck < endingDate && m.LastChannel == (int)ChannelType.WebFacebook
                                                             select new { EmailAddress = m.Login, Name = i.Name }).ToDictionary(q => q.EmailAddress, q => q.Name);

                return emailsAndNames;
            }
        }

        internal static List<Member> GetMembers(List<string> emailAddresses, CmuneDataContext cmuneDb)
        {
            List<Member> members = cmuneDb.Members.Where(m => emailAddresses.Contains(m.Login)).ToList();

            return members;
        }

        #endregion

        #region Item Mall

        /// <summary>
        /// Gets the ItemInvetory of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ItemInventory> GetItemInventory(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return GetItemInventory(cmid, cmuneDb);
            }
        }

        /// <summary>
        /// Gets the ItemInvetory of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        public static List<ItemInventory> GetItemInventory(int cmid, CmuneDataContext cmuneDb)
        {
            List<ItemInventory> items = new List<ItemInventory>();

            if (cmuneDb != null)
            {
                items = (from tab in cmuneDb.ItemInventories where tab.Cmid == cmid select tab).ToList();
            }

            return items;
        }

        /// <summary>
        /// Gets a specific item in a member inventory
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static ItemInventory GetItemInventory(int cmid, int itemId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetItemInventory(cmid, itemId, cmuneDB);
            }
        }

        /// <summary>
        /// Gets a specific item in a member inventory
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemId"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static ItemInventory GetItemInventory(int cmid, int itemId, CmuneDataContext cmuneDB)
        {
            ItemInventory itemInventory = null;

            if (cmuneDB != null)
            {
                itemInventory = cmuneDB.ItemInventories.SingleOrDefault(iI => iI.Cmid == cmid && iI.ItemId == itemId);
            }

            return itemInventory;
        }

        /// <summary>
        /// Gets items in a member inventory matching the itemIds
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="itemIds"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static List<ItemInventory> GetItemsInventory(int cmid, List<int> itemIds, CmuneDataContext cmuneDB)
        {
            List<ItemInventory> itemsInventory = null;

            if (cmuneDB != null && itemIds != null)
            {
                itemsInventory = cmuneDB.ItemInventories.Where(iI => iI.Cmid == cmid && itemIds.Contains(iI.ItemId)).ToList();
            }

            return itemsInventory;
        }

        /// <summary>
        /// Gets the ItemInvetory of a member that are not expired
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ItemInventory> GetCurrentItemInventory(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return GetCurrentItemInventory(cmid, cmuneDB);
            }
        }

        /// <summary>
        /// Gets the ItemInvetory of a member that are not expired
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="cmuneDB"></param>
        /// <returns></returns>
        public static List<ItemInventory> GetCurrentItemInventory(int cmid, CmuneDataContext cmuneDB)
        {
            List<ItemInventory> items = new List<ItemInventory>();

            if (cmuneDB != null)
            {
                items = (from tab in cmuneDB.ItemInventories where tab.Cmid == cmid && (tab.ExpirationDate >= DateTime.Now || tab.ExpirationDate == null) select tab).ToList();
            }

            return items;
        }

        /// <summary>
        /// Gets the ItemInvetory of a member that are not expired
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<int> GetCurrentItemIdInventory(int cmid)
        {
            List<int> itemIds = new List<int>();

            List<ItemInventory> items = GetCurrentItemInventory(cmid);

            if (items != null)
            {
                itemIds = items.Select(i => i.ItemId).ToList();
            }

            return itemIds;
        }

        #endregion

        #region Moderation

        #region Notes

        /// <summary>
        /// Records a moderation action
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="actionTime"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        public static void RecordModerationAction(int sourceCmid, int targetCmid, int applicationId, string sourceIp, DateTime actionTime, ModerationActionType actionType, string reason)
        {
            int moderationActionId = 0;

            RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, actionTime, actionType, reason, out moderationActionId);
        }

        /// <summary>
        /// Records a moderation action
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="actionTime"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        /// <param name="moderationActionId"></param>
        public static void RecordModerationAction(int sourceCmid, int targetCmid, int applicationId, string sourceIp, DateTime actionTime, ModerationActionType actionType, string reason, out int moderationActionId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ModerationAction moderationAction = new ModerationAction();
                moderationAction.ActionDate = actionTime;
                moderationAction.ActionType = (int)actionType;
                moderationAction.ApplicationId = applicationId;
                moderationAction.SourceCmid = sourceCmid;
                moderationAction.SourceIp = TextUtilities.InetAToN(sourceIp);
                moderationAction.TargetCmid = targetCmid;
                moderationAction.Reason = TextUtilities.HtmlEncode(reason.Replace("\n", " ").ShortenText(1000));

                cmuneDb.ModerationActions.InsertOnSubmit(moderationAction);
                cmuneDb.SubmitChanges();

                moderationActionId = moderationAction.ModerationActionId;
            }
        }

        /// <summary>
        /// Get all the notes associated to a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ModerationAction> GetMemberNotes(int cmid)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<ModerationAction> notes = cmuneDb.ModerationActions.Where(m => m.TargetCmid == cmid).OrderByDescending(m => m.ModerationActionId).ToList();

                return notes;
            }
        }

        /// <summary>
        /// Get all the notes view associated to a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<ModerationActionView> GetMemberNotesView(int cmid)
        {
            List<ModerationActionView> notesView = new List<ModerationActionView>();
            List<ModerationAction> notes = GetMemberNotes(cmid);

            List<int> sourceCmids = notes.Select(u => u.SourceCmid).ToList();
            Dictionary<int, int> uniqueCmids = new Dictionary<int, int> { { cmid, cmid } };

            foreach (int sourceCmid in sourceCmids)
            {
                if (!uniqueCmids.ContainsKey(sourceCmid))
                {
                    uniqueCmids.Add(sourceCmid, sourceCmid);
                }
            }

            Dictionary<int, string> membersName = GetMembersNames(uniqueCmids.Keys.ToList());

            notesView = notes.ConvertAll(new Converter<ModerationAction, ModerationActionView>(u => new ModerationActionView(u.ModerationActionId,
                                                                                                                            (ModerationActionType)u.ActionType,
                                                                                                                            u.SourceCmid,
                                                                                                                            membersName[u.SourceCmid],
                                                                                                                            u.TargetCmid,
                                                                                                                            membersName[u.TargetCmid],
                                                                                                                            u.ActionDate,
                                                                                                                            u.ApplicationId,
                                                                                                                            u.Reason,
                                                                                                                            u.SourceIp)));

            return notesView;
        }

        /// <summary>
        /// Edits a member note
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="sourceCmid"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        public static void EditMemberNote(int noteId, int sourceCmid, ModerationActionType actionType, string reason)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ModerationAction note = GetMemberNote(noteId, cmuneDb);

                if (note != null)
                {
                    note.SourceCmid = sourceCmid;
                    note.ActionType = (int)actionType;
                    note.Reason = TextUtilities.HtmlEncode(reason.Replace("\n", " ").ShortenText(1000));

                    cmuneDb.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Deletes a member note
        /// </summary>
        /// <param name="noteId"></param>
        public static void DeleteMemberNote(int noteId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ModerationAction note = GetMemberNote(noteId, cmuneDb);

                if (note != null)
                {
                    cmuneDb.ModerationActions.DeleteOnSubmit(note);

                    cmuneDb.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Gets a member note
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        private static ModerationAction GetMemberNote(int noteId, CmuneDataContext cmuneDb)
        {
            ModerationAction note = null;

            if (cmuneDb != null)
            {
                note = cmuneDb.ModerationActions.SingleOrDefault(n => n.ModerationActionId == noteId);
            }

            return note;
        }

        #endregion

        #region Account banning / unbanning

        /// <summary>
        /// Ban a member
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        /// <param name="bannedUntil">null for permanent</param>
        /// <returns></returns>
        public static bool BanAccount(int sourceCmid, int targetCmid, int applicationId, string sourceIp, string reason, DateTime? bannedUntil)
        {
            bool isMemberBanned = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                MemberAccess sourceMemberAccess = GetMemberAccess(sourceCmid, applicationId);
                MemberAccess targetMemberAccess = GetMemberAccess(targetCmid, applicationId, cmuneDb);

                if (sourceMemberAccess != null && targetMemberAccess != null && sourceMemberAccess.AccessLevel == (int)MemberAccessLevel.Admin && targetMemberAccess.AccessLevel != (int)MemberAccessLevel.Admin)
                {
                    DateTime banningTime = DateTime.Now;

                    MemberOperationResult ret = BanAccount(targetMemberAccess, bannedUntil, cmuneDb);

                    if (MemberOperationResult.Ok.Equals(ret))
                    {
                        isMemberBanned = true;

                        ModerationActionType banType = ModerationActionType.AccountPermanentBan;

                        if (bannedUntil.HasValue)
                        {
                            banType = ModerationActionType.AccountTemporaryBan;
                            TimeSpan banDuration = bannedUntil.Value.Subtract(DateTime.Now);
                            reason = String.Format("Until {0} ({1} days): {2}", bannedUntil.Value.ToString("MM/dd"), Math.Ceiling(banDuration.TotalDays), reason);
                        }

                        RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, banningTime, banType, reason);
                    }
                }
                else
                {
                    // Source does not have enough right or target is an admin too
                }
            }

            return isMemberBanned;
        }

        /// <summary>
        /// Ban a member
        /// </summary>
        /// <param name="targetMemberAccess"></param>
        /// <param name="bannedUntil">null for permanent</param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        private static MemberOperationResult BanAccount(MemberAccess targetMemberAccess, DateTime? bannedUntil, CmuneDataContext cmuneDb)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            if (targetMemberAccess != null && cmuneDb != null)
            {
                if (bannedUntil.HasValue)
                {
                    targetMemberAccess.IsAccountDisabled = (int)BanMode.Temporary;
                    targetMemberAccess.AccountDisabledUntil = bannedUntil.Value.ToDateOnly();
                }
                else
                {
                    targetMemberAccess.IsAccountDisabled = (int)BanMode.Permanent;
                    targetMemberAccess.AccountDisabledUntil = null;
                }

                cmuneDb.SubmitChanges();

                ret = MemberOperationResult.Ok;
            }

            return ret;
        }

        /// <summary>
        /// Uban a member
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool UnbanAccount(int sourceCmid, int targetCmid, int applicationId, string sourceIp, string reason)
        {
            bool isMemberUnbanned = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                MemberAccess sourceMemberAccess = GetMemberAccess(sourceCmid, applicationId);
                MemberAccess targetMemberAccess = GetMemberAccess(targetCmid, applicationId, cmuneDb);

                if (sourceMemberAccess.AccessLevel == (int)MemberAccessLevel.Admin && targetMemberAccess.AccessLevel != (int)MemberAccessLevel.Admin)
                {
                    DateTime actionTime = DateTime.Now;

                    MemberOperationResult ret = UnbanAccount(targetCmid, applicationId);

                    if (MemberOperationResult.Ok.Equals(ret))
                    {
                        isMemberUnbanned = true;

                        RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, actionTime, ModerationActionType.Note, String.Format("Unban account: {0}", reason));
                    }
                }
                else
                {
                    // Source does not have enough right or target is an admin too
                }
            }

            return isMemberUnbanned;
        }

        /// <summary>
        /// Unban a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        private static MemberOperationResult UnbanAccount(int cmid, int applicationId)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberAccess currentMemberAccess = cmuneDB.MemberAccesses.SingleOrDefault<MemberAccess>(mA => mA.Cmid == cmid && mA.ApplicationId == applicationId);

                if (currentMemberAccess != null)
                {
                    currentMemberAccess.IsAccountDisabled = (int)BanMode.No;
                    currentMemberAccess.AccountDisabledUntil = null;

                    cmuneDB.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }
            }

            return ret;
        }

        /// <summary>
        /// Unban all the accounts that are banned until today
        /// </summary>
        /// <param name="debugMode"></param>
        public static void UnbanTemporaryBanAccounts(bool debugMode)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                string currentDate = DateTime.Now.ToShortDateString();
                DateTime today;
                DateTime.TryParse(currentDate, out today);
                List<MemberAccess> membersToUnban = cmuneDb.MemberAccesses.Where(mA => mA.IsAccountDisabled == (int)BanMode.Temporary && mA.AccountDisabledUntil < today).ToList();

                foreach (MemberAccess currentMemberToUnban in membersToUnban)
                {
                    currentMemberToUnban.IsAccountDisabled = (int)BanMode.No;
                    currentMemberToUnban.AccountDisabledUntil = null;
                }

                cmuneDb.SubmitChanges();

                if (debugMode)
                {
                    CmuneLog.CustomLogToDefaultPath("unban.members.log", String.Format("[Count: {0}]", CmuneLog.DisplayForLog(membersToUnban.Count, 5)));
                }
            }
        }

        /// <summary>
        /// Nofify users that they have been permanently banned
        /// </summary>
        /// <param name="cmidsToNotify"></param>
        /// <param name="reason"></param>
        public static void NotifyUserOfPermanentBan(List<int> cmidsToNotify, string reason)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                var query = from m in cmuneDb.Members
                            join i in cmuneDb.ID3s on m.CMID equals i.CMID
                            where cmidsToNotify.Contains(m.CMID) && m.Login != String.Empty
                            select new { Email = m.Login, Name = i.Name, EmailStatus = (EmailAddressStatus)m.EmailAddressState };

                foreach (var row in query)
                {
                    CmuneMail.SendEmailNotification(EmailNotificationType.BanMemberPermanent, reason, row.Email, row.Name, row.EmailStatus);
                }
            }
        }

        #endregion

        #region Chat banning / unbanning

        /// <summary>
        /// Ban from chat
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        /// <param name="bannedUntil">null for permanent</param>
        /// <returns></returns>
        public static bool BanFromChat(int sourceCmid, int targetCmid, int applicationId, string sourceIp, string reason, DateTime? bannedUntil)
        {
            bool isMemberBanned = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                MemberAccess sourceMemberAccess = GetMemberAccess(sourceCmid, applicationId);
                MemberAccess targetMemberAccess = GetMemberAccess(targetCmid, applicationId, cmuneDb);

                if (sourceMemberAccess != null && targetMemberAccess != null && sourceMemberAccess.AccessLevel == (int)MemberAccessLevel.Admin && targetMemberAccess.AccessLevel != (int)MemberAccessLevel.Admin)
                {
                    DateTime banningTime = DateTime.Now;

                    MemberOperationResult ret = BanFromChat(targetMemberAccess, bannedUntil, cmuneDb);

                    if (MemberOperationResult.Ok.Equals(ret))
                    {
                        isMemberBanned = true;

                        ModerationActionType banType = ModerationActionType.ChatPermanentBan;

                        if (bannedUntil.HasValue)
                        {
                            banType = ModerationActionType.ChatTemporaryBan;
                            TimeSpan banDuration = bannedUntil.Value.Subtract(DateTime.Now);
                            reason = String.Format("Until {0} ({1} days): {2}", bannedUntil.Value.ToString("MM/dd"), Math.Ceiling(banDuration.TotalDays), reason);
                        }

                        RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, banningTime, banType, reason);
                    }
                }
                else
                {
                    // Source does not have enough right or target is an admin too
                }
            }

            return isMemberBanned;
        }

        /// <summary>
        /// Ban a member from chat
        /// </summary>
        /// <param name="targetMemberAccess"></param>
        /// <param name="bannedUntil">null for permament</param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        private static MemberOperationResult BanFromChat(MemberAccess targetMemberAccess, DateTime? bannedUntil, CmuneDataContext cmuneDb)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            if (targetMemberAccess != null && cmuneDb != null)
            {
                if (bannedUntil.HasValue)
                {
                    targetMemberAccess.IsChatDisabled = (int)BanMode.Temporary;
                    targetMemberAccess.ChatDisabledUntil = bannedUntil.Value.ToDateOnly();
                }
                else
                {
                    targetMemberAccess.IsChatDisabled = (int)BanMode.Permanent;
                    targetMemberAccess.ChatDisabledUntil = null;
                }

                cmuneDb.SubmitChanges();

                ret = MemberOperationResult.Ok;
            }

            return ret;
        }

        /// <summary>
        /// Ban an account permanently
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool UnbanFromChat(int sourceCmid, int targetCmid, int applicationId, string sourceIp, string reason)
        {
            bool isMemberBanned = false;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                MemberAccess sourceMemberAccess = GetMemberAccess(sourceCmid, applicationId);
                MemberAccess targetMemberAccess = GetMemberAccess(targetCmid, applicationId, cmuneDb);

                if (sourceMemberAccess.AccessLevel == (int)MemberAccessLevel.Admin && targetMemberAccess.AccessLevel != (int)MemberAccessLevel.Admin)
                {
                    DateTime banningTime = DateTime.Now;

                    MemberOperationResult ret = UnbanFromChat(targetCmid, applicationId);

                    if (MemberOperationResult.Ok.Equals(ret))
                    {
                        isMemberBanned = true;

                        RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, banningTime, ModerationActionType.Note, String.Format("Unban chat: {0}", reason));
                    }
                }
                else
                {
                    // Source does not have enough right or target is an admin too
                }
            }

            return isMemberBanned;
        }

        /// <summary>
        /// Unban a member from chat
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        private static MemberOperationResult UnbanFromChat(int cmid, int applicationId)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be >= 1.");
            }

            #endregion

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberAccess currentMemberAccess = cmuneDB.MemberAccesses.SingleOrDefault<MemberAccess>(mA => mA.Cmid == cmid && mA.ApplicationId == applicationId);

                if (currentMemberAccess != null)
                {
                    currentMemberAccess.IsChatDisabled = (int)BanMode.No;
                    currentMemberAccess.ChatDisabledUntil = null;

                    cmuneDB.SubmitChanges();

                    ret = MemberOperationResult.Ok;
                }
            }

            return ret;
        }

        /// <summary>
        /// Unban all the accounts that are banned for the chat until today
        /// </summary>
        /// <param name="debugMode"></param>
        public static void UnbanTemporaryBanFromChat(bool debugMode)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                string currentDate = DateTime.Now.ToShortDateString();
                DateTime today;
                DateTime.TryParse(currentDate, out today);
                List<MemberAccess> membersToUnban = cmuneDB.MemberAccesses.Where(mA => mA.IsChatDisabled == (int)BanMode.Temporary && mA.ChatDisabledUntil < today).ToList();

                foreach (MemberAccess currentMemberToUnban in membersToUnban)
                {
                    currentMemberToUnban.IsChatDisabled = (int)BanMode.No;
                    currentMemberToUnban.ChatDisabledUntil = null;
                }

                cmuneDB.SubmitChanges();

                if (debugMode)
                {
                    CmuneLog.CustomLogToDefaultPath("unban.chat.log", "[Count: " + CmuneLog.DisplayForLog(membersToUnban.Count, 5) + "]");
                }
            }
        }

        #endregion

        #region Ip ban

        /// <summary>
        /// Bans an IP
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="ipToBan"></param>
        /// <param name="reason"></param>
        /// <param name="bannedUntil">If not set, will issue a permanent ban</param>
        public static void BanIp(int sourceCmid, int targetCmid, int applicationId, string sourceIp, string ipToBan, string reason, DateTime? bannedUntil = null)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                DateTime banningDate = DateTime.Now;
                int moderationActionId = 0;

                string banningDuration = "";

                if (bannedUntil.HasValue)
                {
                    banningDuration = String.Format("until {0} - {1} days)", bannedUntil.Value.ToString("MM/dd"), Math.Ceiling(bannedUntil.Value.Subtract(banningDate).TotalDays));
                }
                else
                {
                    banningDuration = "permanent";
                }

                reason = String.Format("IP ban - {0} ({1}) - {2} / {3}", ipToBan, TextUtilities.InetAToN(ipToBan), banningDuration, reason);
                RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, banningDate, ModerationActionType.IpBan, reason, out moderationActionId);

                BannedIp bannedIp = GetBannedIp(ipToBan, cmuneDb);

                if (bannedIp == null)
                {
                    bannedIp = new BannedIp();
                    cmuneDb.BannedIps.InsertOnSubmit(bannedIp);
                }

                bannedIp.BannedUntil = bannedUntil;
                bannedIp.Cmid = targetCmid;
                bannedIp.IpAddress = TextUtilities.InetAToN(ipToBan);
                bannedIp.BanningDate = banningDate;
                bannedIp.ModerationActionId = moderationActionId;

                cmuneDb.SubmitChanges();
            }
        }

        private static BannedIp GetBannedIp(string ip, CmuneDataContext cmuneDb)
        {
            BannedIp bannedIp = null;

            if (cmuneDb != null)
            {
                bannedIp = cmuneDb.BannedIps.SingleOrDefault(i => i.IpAddress == TextUtilities.InetAToN(ip));
            }

            return bannedIp;
        }

        private static BannedIp GetBannedIp(int bannedIpId, CmuneDataContext cmuneDb)
        {
            BannedIp bannedIp = null;

            if (cmuneDb != null)
            {
                bannedIp = cmuneDb.BannedIps.SingleOrDefault(i => i.BannedIpId == bannedIpId);
            }

            return bannedIp;
        }

        /// <summary>
        /// Unban IP
        /// </summary>
        /// <param name="ipAddressToUnban"></param>
        /// <param name="sourceCmid"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        public static void UnbanIp(string ipAddressToUnban, int sourceCmid, string sourceIp, string reason)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                BannedIp bannedIp = GetBannedIp(ipAddressToUnban, cmuneDb);

                UnbanIp(bannedIp, sourceCmid, sourceIp, reason, cmuneDb);
            }
        }

        /// <summary>
        /// Unban IP
        /// </summary>
        /// <param name="bannedIp"></param>
        /// <param name="sourceCmid"></param>
        /// <param name="sourceIp"></param>
        /// <param name="reason"></param>
        /// <param name="cmuneDb"></param>
        private static void UnbanIp(BannedIp bannedIp, int sourceCmid, string sourceIp, string reason, CmuneDataContext cmuneDb)
        {
            if (bannedIp != null && cmuneDb != null)
            {
                reason = String.Format("Unban IP - {0} ({1}): {2}", TextUtilities.InetNToA(bannedIp.IpAddress), bannedIp.IpAddress, reason);
                RecordModerationAction(sourceCmid, bannedIp.Cmid, bannedIp.ModerationAction.ApplicationId, sourceIp, DateTime.Now, ModerationActionType.Note, reason);

                cmuneDb.BannedIps.DeleteOnSubmit(bannedIp);
                cmuneDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Get banned IPs
        /// </summary>
        /// <param name="pageIndex">start from 1</param>
        /// <param name="elementsPerPage"></param>
        /// <param name="displayPermanentBanOnly">Set to true if you wish to get only the permament IP ban</param>
        /// <returns></returns>
        public static List<BannedIpView> GetBannedIpsViews(int pageIndex, int elementsPerPage, bool displayPermanentBanOnly = false)
        {
            List<BannedIpView> bannedIpsView = new List<BannedIpView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                int elementsToSkip = 0;

                if (pageIndex > 1)
                {
                    elementsToSkip = (pageIndex - 1) * elementsPerPage;
                }

                List<BannedIpViewTemp> bannedIps = new List<BannedIpViewTemp>(); ;

                if (displayPermanentBanOnly)
                {
                    bannedIps = (from i in cmuneDb.BannedIps
                                 join m in cmuneDb.ModerationActions on i.ModerationActionId equals m.ModerationActionId
                                 where i.BannedUntil == null
                                 orderby i.BanningDate
                                 select new BannedIpViewTemp { BannedIpId = i.BannedIpId, IpAddress = i.IpAddress, BanningDate = i.BanningDate, BannedUntil = i.BannedUntil, SourceCmid = m.SourceCmid, TargetCmid = i.Cmid, Reason = m.Reason }
                                     ).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }
                else
                {
                    bannedIps = (from i in cmuneDb.BannedIps
                                 join m in cmuneDb.ModerationActions on i.ModerationActionId equals m.ModerationActionId
                                 orderby i.BanningDate
                                 select new BannedIpViewTemp { BannedIpId = i.BannedIpId, IpAddress = i.IpAddress, BanningDate = i.BanningDate, BannedUntil = i.BannedUntil, SourceCmid = m.SourceCmid, TargetCmid = i.Cmid, Reason = m.Reason }
                                     ).Skip(elementsToSkip).Take(elementsPerPage).ToList();
                }

                Dictionary<int, string> names = new Dictionary<int, string>();

                foreach (var bannedIp in bannedIps)
                {
                    if (!names.ContainsKey(bannedIp.SourceCmid))
                    {
                        names.Add(bannedIp.SourceCmid, String.Empty);
                    }

                    if (!names.ContainsKey(bannedIp.TargetCmid))
                    {
                        names.Add(bannedIp.TargetCmid, String.Empty);
                    }
                }

                names = GetMembersNames(names.Keys.ToList());

                foreach (var bannedIp in bannedIps)
                {
                    string sourceName;
                    names.TryGetValue(bannedIp.SourceCmid, out sourceName);

                    string targetName;
                    names.TryGetValue(bannedIp.TargetCmid, out targetName);

                    bannedIpsView.Add(new BannedIpView(bannedIp.BannedIpId, bannedIp.IpAddress, bannedIp.BannedUntil, bannedIp.BanningDate, bannedIp.SourceCmid, sourceName, bannedIp.TargetCmid, targetName, bannedIp.Reason));
                }
            }

            return bannedIpsView;
        }

        /// <summary>
        /// Get the Ips banned count
        /// </summary>
        /// <param name="displayPermanentBanOnly">Set to true if you wish to get only the permament IP ban</param>
        /// <returns></returns>
        public static int GetBannedIpsViewsCount(bool displayPermanentBanOnly = false)
        {
            int count = 0;

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                if (displayPermanentBanOnly)
                {
                    count = (from i in cmuneDb.BannedIps
                             join m in cmuneDb.ModerationActions on i.ModerationActionId equals m.ModerationActionId
                             where i.BannedUntil == null
                             orderby i.BanningDate
                             select i.ModerationActionId).Count();
                }
                else
                {
                    count = (from i in cmuneDb.BannedIps
                             join m in cmuneDb.ModerationActions on i.ModerationActionId equals m.ModerationActionId
                             orderby i.BanningDate
                             select i.ModerationActionId).Count();
                }
            }

            return count;
        }

        /// <summary>
        /// Search if we have a ban for this IP
        /// </summary>
        /// <param name="bannedIpAddress"></param>
        /// <returns></returns>
        public static BannedIpView GetBannedIpView(string bannedIpAddress)
        {
            return GetBannedIpView(TextUtilities.InetAToN(bannedIpAddress));
        }

        /// <summary>
        /// Search if we have a ban for this network address
        /// </summary>
        /// <param name="bannedNetworkAddress"></param>
        /// <returns></returns>
        public static BannedIpView GetBannedIpView(long bannedNetworkAddress)
        {
            Dictionary<long, BannedIpView> bannedIpViews = GetBannedIpsViews(new List<long> { bannedNetworkAddress });

            BannedIpView bannedIpView = null;
            BannedIpView bannedIpViewTmp = null;

            if (bannedIpViews.TryGetValue(bannedNetworkAddress, out bannedIpViewTmp))
            {
                bannedIpView = bannedIpViewTmp;
            }

            return bannedIpView;
        }

        /// <summary>
        /// Search if we have a ban for those network addresses
        /// </summary>
        /// <param name="bannedIpNetworkAddresses"></param>
        /// <returns></returns>
        public static Dictionary<long, BannedIpView> GetBannedIpsViews(List<long> bannedIpNetworkAddresses)
        {
            Dictionary<long, BannedIpView> bannedIpViews = new Dictionary<long, BannedIpView>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<BannedIpViewTemp> bannedIps = (from i in cmuneDb.BannedIps
                                                    join m in cmuneDb.ModerationActions on i.ModerationActionId equals m.ModerationActionId
                                                    where bannedIpNetworkAddresses.Contains(i.IpAddress)
                                                    select new BannedIpViewTemp { BannedIpId = i.BannedIpId, IpAddress = i.IpAddress, BanningDate = i.BanningDate, BannedUntil = i.BannedUntil, SourceCmid = m.SourceCmid, TargetCmid = i.Cmid, Reason = m.Reason }
                                                    ).ToList();

                if (bannedIps != null && bannedIps.Count > 0)
                {
                    Dictionary<int, string> names = new Dictionary<int, string>();

                    foreach (BannedIpViewTemp bannedIp in bannedIps)
                    {
                        if (!names.ContainsKey(bannedIp.SourceCmid))
                        {
                            names.Add(bannedIp.SourceCmid, String.Empty);
                        }

                        if (!names.ContainsKey(bannedIp.TargetCmid))
                        {
                            names.Add(bannedIp.TargetCmid, String.Empty);
                        }
                    }

                    names = GetMembersNames(names.Keys.ToList());

                    foreach (BannedIpViewTemp bannedIp in bannedIps)
                    {
                        string sourceName;
                        names.TryGetValue(bannedIp.SourceCmid, out sourceName);

                        string targetName;
                        names.TryGetValue(bannedIp.TargetCmid, out targetName);

                        bannedIpViews.Add(bannedIp.IpAddress, new BannedIpView(bannedIp.BannedIpId, bannedIp.IpAddress, bannedIp.BannedUntil, bannedIp.BanningDate, bannedIp.SourceCmid, sourceName, bannedIp.TargetCmid, targetName, bannedIp.Reason));
                    }
                }
            }

            return bannedIpViews;
        }

        /// <summary>
        /// Checks whether an IP is banned
        /// </summary>
        /// <param name="networkAddress"></param>
        /// <returns></returns>
        public static bool IsIpBanned(long networkAddress)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isIpBanned = false;

                List<int> banedIpId = (from i in cmuneDb.BannedIps
                                       where i.IpAddress == networkAddress
                                       select i.BannedIpId).ToList();


                if (banedIpId != null && banedIpId.Count > 0)
                {
                    isIpBanned = true;
                }

                return isIpBanned;
            }
        }

        /// <summary>
        /// Unban all the IPs that are banned until today
        /// </summary>
        /// <param name="debugMode"></param>
        public static void UnbanTemporaryBanIps(bool debugMode)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                DateTime today = DateTime.Now.ToDateOnly();
                int ubannedIpsCount = 0;

                List<BannedIp> ipsToUnbanned = (from i in cmuneDb.BannedIps
                                                where i.BannedUntil != null && i.BannedUntil <= today
                                                select i).ToList();

                ubannedIpsCount = ipsToUnbanned.Count();

                cmuneDb.BannedIps.DeleteAllOnSubmit(ipsToUnbanned);
                cmuneDb.SubmitChanges();

                if (debugMode)
                {
                    CmuneLog.CustomLogToDefaultPath("unban.ips.log", "[Count: " + CmuneLog.DisplayForLog(ubannedIpsCount, 5) + "]");
                }
            }
        }

        /// <summary>
        /// Useless class used by LINQ
        /// </summary>
        private class BannedIpViewTemp
        {
            public int BannedIpId { get; set; }
            public long IpAddress { get; set; }
            public DateTime BanningDate { get; set; }
            public DateTime? BannedUntil { get; set; }
            public int SourceCmid { get; set; }
            public int TargetCmid { get; set; }
            public string Reason { get; set; }
        }

        #endregion

        #endregion

        #region Merge

        /// <summary>
        /// Merges 2 accounts:
        ///     - Items
        ///     - Credits
        ///     - Friends
        /// </summary>
        /// <param name="cmidToKeep"></param>
        /// <param name="cmidToDelete"></param>
        /// <param name="mergePointsMode"></param>
        /// <returns></returns>
        public static MemberMergeResult MergeAccounts(int cmidToKeep, int cmidToDelete, MergePointsMode mergePointsMode)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                MemberMergeResult ret = MemberMergeResult.CmidNotFound;

                Member memberToKeep = cmuneDb.Members.SingleOrDefault<Member>(m1 => m1.CMID == cmidToKeep);
                Member memberToDelete = cmuneDb.Members.SingleOrDefault<Member>(m2 => m2.CMID == cmidToDelete);

                if (memberToKeep != null && memberToDelete != null && cmidToKeep != cmidToDelete)
                {
                    #region Merge ESNS

                    /*
                     * Now we merge only if:
                     *      - None of the CMID has a FB ID
                     *      - If the account to keep does not have a FB ID and the account to delete has a FB ID
                     */

                    ESNSIdentity memberToKeepFacebook = cmuneDb.ESNSIdentities.SingleOrDefault<ESNSIdentity>(eI => eI.CMID == cmidToKeep && eI.Type == (int)EsnsType.Facebook);
                    ESNSIdentity memberToDeleteFacebook = cmuneDb.ESNSIdentities.SingleOrDefault<ESNSIdentity>(eI => eI.CMID == cmidToDelete && eI.Type == (int)EsnsType.Facebook);

                    bool isMemberToKeepHavingFacebook = false;
                    if (memberToKeepFacebook != null)
                    {
                        isMemberToKeepHavingFacebook = true;
                        ret = MemberMergeResult.CmidAlreadyLinkedToEsns;
                    }
                    else
                    {
                        if (memberToDeleteFacebook != null)
                        {
                            // We have to transfer the FB ID to the account we will keep
                            memberToDeleteFacebook.CMID = cmidToKeep;
                        }
                    }

                    #endregion

                    if (!isMemberToKeepHavingFacebook)
                    {
                        #region Merge items

                        /*
                         * Synchronizes items between two accounts.
                         * When the same item is on both accounts, keeps the one with the biggest expiration date
                         * When the items in only on the account to delete, we add it to the account to keep
                         */

                        DateTime now = DateTime.Now;

                        Dictionary<int, ItemInventory> memberToKeepInventory = cmuneDb.ItemInventories.Where(i1 => i1.Cmid == memberToKeep.CMID).ToDictionary(i1 => i1.ItemId);
                        Dictionary<int, ItemInventory> memberToDeleteInventory = cmuneDb.ItemInventories.Where(i2 => i2.Cmid == memberToDelete.CMID).ToDictionary(i2 => i2.ItemId);

                        int currentItemId = 0;

                        foreach (KeyValuePair<int, ItemInventory> currentItemInInventory in memberToKeepInventory)
                        {
                            currentItemId = currentItemInInventory.Key;

                            if (memberToDeleteInventory.ContainsKey(currentItemId))
                            {
                                if ((memberToDeleteInventory[currentItemId].ExpirationDate == null && memberToKeepInventory[currentItemId].ExpirationDate != null) ||
                                        memberToDeleteInventory[currentItemId].ExpirationDate > memberToKeepInventory[currentItemId].ExpirationDate)
                                {
                                    memberToKeepInventory[currentItemId].ExpirationDate = memberToDeleteInventory[currentItemId].ExpirationDate;
                                }
                            }
                        }

                        foreach (KeyValuePair<int, ItemInventory> itemInInventoryToDelete in memberToDeleteInventory)
                        {
                            currentItemId = itemInInventoryToDelete.Key;

                            if (!memberToKeepInventory.ContainsKey(currentItemId))
                            {
                                memberToDeleteInventory[currentItemId].Cmid = memberToKeep.CMID;
                            }
                        }

                        #endregion Merge items

                        #region Merge Credits and Points

                        /*
                     * Add the credits of the previous account to the current one.
                     */

                        Credit memberToKeepCredit = cmuneDb.Credits.SingleOrDefault<Credit>(c1 => c1.UserId == memberToKeep.CMID);
                        Credit memberToDeleteCredit = cmuneDb.Credits.SingleOrDefault<Credit>(c2 => c2.UserId == memberToDelete.CMID);

                        if (memberToDeleteCredit.ExpDateCredit > now && memberToDeleteCredit.NbCredits > 0)
                        {
                            memberToKeepCredit.NbCredits += memberToDeleteCredit.NbCredits;
                        }

                        if (memberToDeleteCredit.ExpDatePoint > now && memberToDeleteCredit.NbPoints > 0 && !mergePointsMode.Equals(MergePointsMode.Ignore))
                        {
                            switch (mergePointsMode)
                            {
                                case MergePointsMode.Add:
                                    memberToKeepCredit.NbPoints += memberToDeleteCredit.NbPoints;
                                    break;
                                case MergePointsMode.Penalize:
                                    int mergeCreditPenalize = CommonConfig.MemberMergePointsPenalizeDefault;
                                    if (!ConfigurationManager.AppSettings["memberMergePointsPenalize"].IsNullOrFullyEmpty())
                                    {
                                        Int32.TryParse(ConfigurationManager.AppSettings["memberMergePointsPenalize"], out mergeCreditPenalize);
                                    }
                                    memberToKeepCredit.NbPoints += memberToDeleteCredit.NbPoints * mergeCreditPenalize / 100;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("mergePointsMode", mergePointsMode, "This Enum values is not implemented yet.");
                            }
                        }

                        #endregion Merge Credits and Points

                        #region Merge Contacts

                        /*
                     * Compare the relationship of the two accounts and add those not in common to the account to keep
                     */

                        List<MembersRelationship> memberToKeepRelationships = cmuneDb.MembersRelationships.Where<MembersRelationship>(r1 => r1.MemberCMID == memberToKeep.CMID || r1.FriendCMID == memberToKeep.CMID).ToList<MembersRelationship>();
                        List<MembersRelationship> memberToDeleteRelationships = cmuneDb.MembersRelationships.Where<MembersRelationship>(r2 => r2.MemberCMID == memberToDelete.CMID || r2.FriendCMID == memberToDelete.CMID).ToList<MembersRelationship>();

                        List<int> memberToKeepContactsCmid = new List<int>();
                        Dictionary<int, MembersRelationship> contactsToAdd = new Dictionary<int, MembersRelationship>();

                        // 1) FIRST: we collect all the contacts CMID of the member to keep
                        foreach (MembersRelationship memberToKeepRelationship in memberToKeepRelationships)
                        {
                            if (memberToKeepRelationship.MemberCMID != memberToKeep.CMID)
                            {
                                memberToKeepContactsCmid.Add(memberToKeepRelationship.MemberCMID);
                            }
                            else
                            {
                                memberToKeepContactsCmid.Add(memberToKeepRelationship.FriendCMID);
                            }
                        }
                        // We add the current CMID just in case the current and the previous account are contacts
                        memberToKeepContactsCmid.Add(memberToKeep.CMID);

                        // 2) SECOND: then we collect the contacts CMID of the member to delete to delete that are NOT in the contacts of the member to keep
                        foreach (MembersRelationship memberToDeleteRelationship in memberToDeleteRelationships)
                        {
                            int currentContactCmid = 0;
                            if (memberToDeleteRelationship.MemberCMID != memberToDelete.CMID)
                            {
                                currentContactCmid = memberToDeleteRelationship.MemberCMID;
                            }
                            else
                            {
                                currentContactCmid = memberToDeleteRelationship.FriendCMID;
                            }

                            if (!memberToKeepContactsCmid.Contains(currentContactCmid))
                            {
                                contactsToAdd.Add(currentContactCmid, memberToDeleteRelationship);
                            }
                        }

                        // Now memberToDeleteContactsCmid contains only the MembersRelationship that we need to add to the current account

                        DateTime relationshipBeginning = now;
                        int contactToAddMemberCmid = 0;
                        int contactToAddContactCmid = 0;
                        List<MembersRelationship> memberToKeepNewRelationships = new List<MembersRelationship>();
                        foreach (KeyValuePair<int, MembersRelationship> contactToAdd in contactsToAdd)
                        {
                            AnalyticalRelationship analyticalRelationship = new AnalyticalRelationship();
                            analyticalRelationship.Created = relationshipBeginning;
                            cmuneDb.AnalyticalRelationships.InsertOnSubmit(analyticalRelationship);
                            cmuneDb.SubmitChanges();

                            contactToAddMemberCmid = contactToAdd.Value.MemberCMID;
                            contactToAddContactCmid = contactToAdd.Value.FriendCMID;

                            if (contactToAddMemberCmid == memberToDelete.CMID)
                            {
                                contactToAddMemberCmid = memberToKeep.CMID;
                            }
                            else
                            {
                                contactToAddContactCmid = memberToKeep.CMID;
                            }

                            MembersRelationship newMemberRelationship = new MembersRelationship();
                            newMemberRelationship.ARID = analyticalRelationship.ARID;
                            newMemberRelationship.FriendCMID = contactToAddContactCmid;
                            newMemberRelationship.MemberCMID = contactToAddMemberCmid;

                            memberToKeepNewRelationships.Add(newMemberRelationship);
                        }

                        cmuneDb.MembersRelationships.InsertAllOnSubmit<MembersRelationship>(memberToKeepNewRelationships);

                        #region Merge contacts groups

                        // TODO

                        #endregion Merge contacts groups

                        /*
                     * Compare the contact requests of the two accounts and add those not in common to the account to keep
                     */

                        List<RequestContact> memberToKeepRequestContacts = cmuneDb.RequestContacts.Where<RequestContact>(rC => rC.InitiatorCMID == memberToKeep.CMID || rC.ReceiverCMID == memberToKeep.CMID).ToList<RequestContact>();
                        List<RequestContact> memberToDeleteRequestContacts = cmuneDb.RequestContacts.Where<RequestContact>(rC => rC.InitiatorCMID == memberToDelete.CMID || rC.ReceiverCMID == memberToDelete.CMID).ToList<RequestContact>();

                        List<int> memberToKeepRequestContactsCmid = new List<int>();
                        Dictionary<int, RequestContact> contactRequestsToAdd = new Dictionary<int, RequestContact>();

                        // 1) FIRST: we collect all the contacts requests CMID of the member to keep
                        foreach (RequestContact memberToKeepRequestContact in memberToKeepRequestContacts)
                        {
                            if (memberToKeepRequestContact.InitiatorCMID != memberToKeep.CMID)
                            {
                                memberToKeepRequestContactsCmid.Add(memberToKeepRequestContact.InitiatorCMID);
                            }
                            else
                            {
                                memberToKeepRequestContactsCmid.Add(memberToKeepRequestContact.ReceiverCMID);
                            }
                        }
                        // We add the current CMID just in case the current and the previous account sent a request contact to each other
                        memberToKeepRequestContactsCmid.Add(memberToKeep.CMID);

                        // 2) SECOND: then we collect the contact requests CMID of the member to delete that are NOT in the contact requests of the member to keep
                        foreach (RequestContact memberToDeleteRequestContact in memberToDeleteRequestContacts)
                        {
                            int currentRequestContactCmid = 0;
                            if (memberToDeleteRequestContact.InitiatorCMID != memberToDelete.CMID)
                            {
                                currentRequestContactCmid = memberToDeleteRequestContact.InitiatorCMID;
                            }
                            else
                            {
                                currentRequestContactCmid = memberToDeleteRequestContact.ReceiverCMID;
                            }

                            if (!memberToKeepRequestContactsCmid.Contains(currentRequestContactCmid))
                            {
                                contactRequestsToAdd.Add(currentRequestContactCmid, memberToDeleteRequestContact);
                            }
                        }

                        int contactRequestToAddInitiatorCmid = 0;
                        int contactRequestToAddReceiverCmid = 0;
                        List<RequestContact> memberToKeepNewContactRequests = new List<RequestContact>();
                        foreach (KeyValuePair<int, RequestContact> contactRequestToAdd in contactRequestsToAdd)
                        {
                            contactRequestToAddInitiatorCmid = contactRequestToAdd.Value.InitiatorCMID;
                            contactRequestToAddReceiverCmid = contactRequestToAdd.Value.ReceiverCMID;

                            if (contactToAddMemberCmid == memberToDelete.CMID)
                            {
                                contactToAddMemberCmid = memberToKeep.CMID;
                            }
                            else
                            {
                                contactToAddContactCmid = memberToKeep.CMID;
                            }

                            RequestContact requestContact = new RequestContact();
                            requestContact.InitiatorCMID = contactRequestToAddInitiatorCmid;
                            requestContact.Message = contactRequestToAdd.Value.Message;
                            requestContact.ReceiverCMID = contactRequestToAddReceiverCmid;
                            requestContact.SentDate = now;
                            requestContact.Status = contactRequestToAdd.Value.Status;

                            memberToKeepNewContactRequests.Add(requestContact);
                        }

                        cmuneDb.RequestContacts.InsertAllOnSubmit<RequestContact>(memberToKeepNewContactRequests);

                        #endregion Merge Contacts

                        #region Merge groups

                        CmuneGroups.MergeMembers(cmidToKeep, cmidToDelete, 1);

                        #endregion Merge groups

                        cmuneDb.SubmitChanges();
                        ret = MemberMergeResult.Ok;
                    }
                }

                return ret;
            }
        }

        #endregion

        #region Email validation

        /// <summary>
        /// Generates a validation code in the DB to check the member identity
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="typeValid">validation type, 1 for email</param>
        /// <returns>the generated validation code</returns>
        private static string GenerateValidationCode(int cmid, IdentityValidationType typeValid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                Guid guid = Guid.NewGuid();
                string temp = guid.ToString();
                string validationCode = temp.Substring(0, 8);

                IdentityValidation validUser = new IdentityValidation();
                validUser.CMID = cmid;
                validUser.type = (int)typeValid;
                validUser.ValidationSent = DateTime.Now;
                validUser.ValidationString = validationCode;

                cmuneDB.IdentityValidations.InsertOnSubmit(validUser);

                cmuneDB.SubmitChanges();

                return validationCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static string GenerateValidationUrl(int cmid)
        {
            string validationCode = GenerateValidationCode(cmid, IdentityValidationType.Email);
            string validationUrl = String.Format("{0}Account/ValidateEmail?cmid={1}&hash={2}", ConfigurationUtilities.ReadConfigurationManager("PortalUrl"), cmid, validationCode);

            return validationUrl;
        }

        /// <summary>
        /// Validates the member email thanks to a code he provided
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="validationCode"></param>
        /// <returns></returns>
        public static bool ValidateMemberEmail(int cmid, string validationCode)
        {
            // TODO use an ENUM for the typeValid

            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool isValid = false;
                IdentityValidation emailValidation = cmuneDB.IdentityValidations.SingleOrDefault<IdentityValidation>(iV => iV.CMID == cmid && iV.type == (int)IdentityValidationType.Email && iV.ValidationString == validationCode);

                if (emailValidation != null)
                {
                    Member memberToUpdate = GetMember(cmid, cmuneDB);

                    if (memberToUpdate != null)
                    {
                        memberToUpdate.EmailAddressState = (byte)EmailAddressStatus.Verified;

                        // Now we need to delete all the email validations matching with this member
                        List<IdentityValidation> emailValidations = cmuneDB.IdentityValidations.Where<IdentityValidation>(iV => iV.CMID == cmid && iV.type == (int)IdentityValidationType.Email).ToList();
                        cmuneDB.IdentityValidations.DeleteAllOnSubmit(emailValidations);
                        cmuneDB.SubmitChanges();

                        isValid = true;
                    }
                }

                return isValid;
            }
        }

        /// <summary>
        /// Deletes all the old validation code
        /// </summary>
        /// <param name="debugMode"></param>
        public static void DeleteOldValidationCodes(bool debugMode)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                DateTime now = DateTime.Now;

                DateTime before = new DateTime(now.Year, now.Month, now.Day).AddDays(-CommonConfig.IdentityValidationLifetimeInDays);

                List<IdentityValidation> identityValidations = cmuneDB.IdentityValidations.Where<IdentityValidation>(iV => iV.ValidationSent < before).ToList();

                cmuneDB.IdentityValidations.DeleteAllOnSubmit(identityValidations);
                cmuneDB.SubmitChanges();

                if (debugMode)
                {
                    CmuneLog.CustomLogToDefaultPath("identity.validation.expiration.log", "[Count: " + CmuneLog.DisplayForLog(identityValidations.Count, 7) + "]");
                }
            }
        }



        #endregion Email validation

        #region Delete

        /// <summary>
        /// Deletes the member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteMember(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberOperationResult ret = MemberOperationResult.MemberNotFound;
                // We also need to delete the MembersRelationships as there are 2 FK referencing Member.
                Member member = CmuneMember.GetMember(cmid, cmuneDB);
                if (member != null)
                {
                    MemberOperationResult loginDeleteResult = CmuneRelationship.DeleteAllLoginIps(cmid);
                    MemberOperationResult contactsDeletionResult = CmuneRelationship.DeleteAllContacts(cmid);
                    MemberOperationResult contactRequestDeletionResult = CmuneRelationship.DeleteAllContactRequests(cmid);

                    // TODO Should we delete the PM?

                    if (contactsDeletionResult.Equals(MemberOperationResult.Ok) && contactRequestDeletionResult.Equals(MemberOperationResult.Ok))
                    {
                        // For the moment there is only PP
                        CmuneGroups.DeleteCommunityInvolvment(cmid, 1);

                        MemberAccess memberAccess = cmuneDB.MemberAccesses.SingleOrDefault(mA => mA.Cmid == cmid);

                        if (memberAccess != null)
                        {
                            cmuneDB.MemberAccesses.DeleteOnSubmit(memberAccess);
                            cmuneDB.SubmitChanges();
                        }

                        member = CmuneMember.GetMember(cmid, cmuneDB);

                        cmuneDB.Members.DeleteOnSubmit(member);
                        cmuneDB.SubmitChanges();
                        ret = MemberOperationResult.Ok;
                    }
                    else
                    {
                        if (contactsDeletionResult != MemberOperationResult.Ok)
                        {
                            ret = contactsDeletionResult;
                        }
                        else
                        {
                            ret = contactRequestDeletionResult;
                        }
                    }
                }

                return ret;
            }
        }

        #endregion Delete

        /// <summary>
        /// Checks whether a member has already validated his email in the past
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool HasAlreadyValidatedEmail(int cmid)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                bool hasAlreadyValidatedEmail = false;
                List<PointDeposit> pointDeposit = cmuneDB.PointDeposits.Where(pD => pD.UserId == cmid && pD.DepositType == (int)PointsDepositType.IdentityValidation).ToList();

                if (pointDeposit.Count > 0)
                {
                    hasAlreadyValidatedEmail = true;
                }

                return hasAlreadyValidatedEmail;
            }
        }

        /// <summary>
        /// Allows a member to report another member
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="reportType"></param>
        /// <param name="reason"></param>
        /// <param name="reportContext"></param>
        /// <param name="applicationId"></param>
        public static void ReportMember(int sourceCmid, int targetCmid, MemberReportType reportType, string reason, string reportContext, int applicationId)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                MemberReport memberReport = new MemberReport();
                memberReport.ApplicationId = applicationId;
                memberReport.Reason = reason.ShortenText(CommonConfig.MemberReportReasonMaxLength);
                memberReport.ReportContext = reportContext.ShortenText(CommonConfig.MemberReportContextMaxLength);
                memberReport.ReportDate = DateTime.Now;
                memberReport.ReportType = (int)reportType;
                memberReport.SourceCmid = sourceCmid;
                memberReport.Status = (int)MemberReportStatus.New;
                memberReport.TargetCmid = targetCmid;

                cmuneDB.MemberReports.InsertOnSubmit(memberReport);
                cmuneDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Get the biggest indexder of our test accounts
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static int GetLastTestAccountNameIndexer(string template)
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                List<string> testAccountsName = new List<string>();

                List<string> accountNames = cmuneDB.ID3s.Where(id3 => id3.Name.StartsWith(template)).Select(id3 => id3.Name).ToList();

                int templateLenght = template.Length;
                int currentIndexerValue = 0;
                int maxIndexerValue = 0;

                foreach (string accountName in accountNames)
                {
                    string currentIndexer = accountName.Substring(templateLenght);

                    if (Int32.TryParse(currentIndexer, out currentIndexerValue))
                    {
                        if (currentIndexerValue > maxIndexerValue)
                        {
                            maxIndexerValue = currentIndexerValue;
                        }
                    }
                }

                return maxIndexerValue;
            }
        }

        /// <summary>
        /// get the last index of the Member Table
        /// </summary>
        /// <returns></returns>
        public static int GetLastMemberId()
        {
            using (CmuneDataContext cmuneDB = new CmuneDataContext())
            {
                return cmuneDB.Members.OrderByDescending(d => d.CMID).First().CMID;
            }
        }

        /// <summary>
        /// Get all the countries name
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetAllCountriesName()
        {
            Dictionary<int, string> countriesName = new Dictionary<int, string>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                countriesName = cmuneDb.Countries.ToDictionary(u => u.CountryId, u => u.CountryName);
            }

            return countriesName;
        }

        #region Previous emails

        /// <summary>
        /// Get the previous emails of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<PreviousEmail> GetPreviousEmails(int cmid)
        {
            List<PreviousEmail> previousEmails = new List<PreviousEmail>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                previousEmails = cmuneDb.PreviousEmails.Where(e => e.Cmid == cmid).ToList();
            }

            return previousEmails;
        }

        /// <summary>
        /// Get all the accounts that were previously linked to this email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static List<PreviousEmail> GetAccountsLinkedToPreviousEmail(string emailAddress)
        {
            List<PreviousEmail> previousEmails = new List<PreviousEmail>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                previousEmails = cmuneDb.PreviousEmails.Where(e => e.PreviousEmailAddress == emailAddress).OrderByDescending(e => e.ChangeDate).ToList();
            }

            return previousEmails;
        }

        #endregion

        #region Previous names

        /// <summary>
        /// Get the previous names of a member
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static List<PreviousName> GetPreviousNames(int cmid)
        {
            List<PreviousName> previousNames = new List<PreviousName>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                previousNames = cmuneDb.PreviousNames.Where(e => e.Cmid == cmid).ToList();
            }

            return previousNames;
        }

        /// <summary>
        /// Get all the accounts that were previously linked to this name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<PreviousName> GetAccountsLinkedToPreviousName(string name)
        {
            List<PreviousName> previousEmails = new List<PreviousName>();

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                previousEmails = cmuneDb.PreviousNames.Where(e => e.PreviousUserName == name).OrderByDescending(e => e.ChangeDate).ToList();
            }

            return previousEmails;
        }

        #endregion

        /// <summary>
        /// Count the number of esns members that created an account on UberStrike
        /// </summary>
        /// <param name="esnsType"></param>
        /// <returns></returns>
        public static int GetEsnsCount(EsnsType esnsType)
        {
            int count = 0;

            using (var cmuneDb = new CmuneDataContext())
            {
                count = cmuneDb.ESNSIdentities.Where(e => e.Type == (int)esnsType).Count();
            }

            return count;
        }
    }
}