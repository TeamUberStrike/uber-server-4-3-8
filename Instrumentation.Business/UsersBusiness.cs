using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils.Cryptography;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Utils;
using PPMember = UberStrike.DataCenter.Business.Users;
using UberStrike.DataCenter.Business;

namespace Cmune.Instrumentation.Business
{
    /// <summary>
    /// Manages the members
    /// </summary>
    public static class UsersBusiness
    {
        #region Registration

        /// <summary>
        /// Transform an ESNS only member into a full Cmune Member and eventually send a email to the member. Generates a randdom password.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="sendEmail"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static MemberOperationResult RegisterEsnsMember(int cmid, string email, bool sendEmail, string explanation)
        {
            string randomPassword = RandomPassword.Generate(6, 8);
            return RegisterEsnsMember(cmid, email, randomPassword, sendEmail, explanation);
        }

        /// <summary>
        /// Transform an ESNS only member into a full Cmune Member. Generates a randdom password.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static MemberOperationResult RegisterEsnsMember(int cmid, string email)
        {
            return RegisterEsnsMember(cmid, email, false, String.Empty);
        }

        /// <summary>
        /// Transform an ESNS only member into a full Cmune Member and eventually send a email to the member.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="sendEmail"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static MemberOperationResult RegisterEsnsMember(int cmid, string email, string password, bool sendEmail, string explanation)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            ret = CmuneMember.RegisterEsnsMember(cmid, email, password);

            if (ret.Equals(MemberOperationResult.Ok))
            {
                MemberOperationResult miniverseRet = RegisterEsnsMemberMiniverses(cmid, email, password);

                if (miniverseRet.Equals(MemberOperationResult.Ok))
                {
                    if (sendEmail)
                    {
                        ID3 memberID3 = CmuneMember.GetId3(cmid);
                        if (memberID3 != null)
                        {
                            CmuneMail.RegisterEsnsMember(ValidationUtilities.StandardizeEmail(email), memberID3.Name, password, explanation);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Transform an ESNS only member into a full Cmune Member.
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MemberOperationResult RegisterEsnsMember(int cmid, string email, string password)
        {
            return RegisterEsnsMember(cmid, email, password, false, String.Empty);
        }

        #endregion Registration

        #region Data modification

        /// <summary>
        /// Change the name of a member and sync it over all the Miniverses
        /// </summary>
        /// <param name="targetCmid"></param>
        /// <param name="newName"></param>
        /// <param name="locale"></param>
        /// <param name="sourceIp"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeMemberName(int targetCmid, string newName, string locale, string sourceIp)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (targetCmid < 1)
            {
                ret = MemberOperationResult.InvalidCmid;
            }

            newName = ValidationUtilities.StandardizeMemberName(newName);

            if (newName.IsNullOrFullyEmpty())
            {
                ret = MemberOperationResult.InvalidName;
            }

            #endregion Check input

            if (ret.Equals(MemberOperationResult.MemberNotFound))
            {
                bool isDuplicatedUsername = CmuneMember.IsDuplicateUserName(newName);

                if (isDuplicatedUsername)
                {
                    ret = MemberOperationResult.DuplicateName;
                }
                else
                {
                    ret = CmuneMember.ChangeMemberName(targetCmid, newName, false, locale, sourceIp);

                    if (ret.Equals(MemberOperationResult.Ok))
                    {
                        ret = ChangeUsernameMiniverses(targetCmid, newName);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Change the name of a member and sync it over all the Miniverses and record the action
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        /// <param name="newName"></param>
        /// <param name="oldName"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeMemberName(int sourceCmid, int targetCmid, int applicationId, string sourceIp, ModerationActionType actionType, string reason, string newName, string oldName, string locale)
        {
            MemberOperationResult ret = ChangeMemberName(targetCmid, newName, locale, sourceIp);

            if (ret.Equals(MemberOperationResult.Ok))
            {
                reason = String.Format("Old name: {0} / {1}", oldName, reason);

                CmuneMember.RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, DateTime.Now, actionType, reason);
            }

            return ret;
        }

        /// <summary>
        /// Change the email of a member and sync it over all the Miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="sourceIp"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeMemberEmail(int cmid, string email, string sourceIp)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (!ValidationUtilities.IsValidEmailAddress(email))
            {
                ret = MemberOperationResult.InvalidEmail;
            }

            #endregion

            if (ret.Equals(MemberOperationResult.MemberNotFound))
            {
                bool isDuplicatedEmail = CmuneMember.IsDuplicateUserEmail(email);

                if (isDuplicatedEmail)
                {
                    ret = MemberOperationResult.DuplicateEmail;
                }
                else
                {
                    CmuneMember.ChangeMemberEmail(cmid, email, sourceIp);
                    ChangeEmailMiniverses(cmid, email);
                    ret = MemberOperationResult.Ok;
                }
            }

            return ret;
        }

        /// <summary>
        /// Change the email of a member and sync it over all the Miniverses and record the action
        /// </summary>
        /// <param name="sourceCmid"></param>
        /// <param name="targetCmid"></param>
        /// <param name="applicationId"></param>
        /// <param name="sourceIp"></param>
        /// <param name="actionType"></param>
        /// <param name="reason"></param>
        /// <param name="newEmail"></param>
        /// <param name="oldEmail"></param>
        /// <returns></returns>
        public static MemberOperationResult ChangeMemberEmail(int sourceCmid, int targetCmid, int applicationId, string sourceIp, ModerationActionType actionType, string reason, string newEmail, string oldEmail)
        {
            MemberOperationResult ret = ChangeMemberEmail(targetCmid, newEmail, sourceIp);

            if (ret.Equals(MemberOperationResult.Ok))
            {
                reason = String.Format("Old email: {0} / {1}", oldEmail, reason);

                CmuneMember.RecordModerationAction(sourceCmid, targetCmid, applicationId, sourceIp, DateTime.Now, actionType, reason);
            }

            return ret;
        }

        /// <summary>
        /// Resets a member password after a member clicked on a link to confirm he wishes to reset his password
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool ResetPassword(int cmid, string hash)
        {
            bool isPasswordReset = false;

            PasswordResetHashe passwordResetHash = CmuneMember.GetPasswordResetHash(cmid, hash);

            if (passwordResetHash != null)
            {
                isPasswordReset = Users.ChangeHashedPassword(passwordResetHash.Cmid, passwordResetHash.NewPassword);

                if (isPasswordReset)
                {
                    CmuneMember.DeletePasswordResetHashes(cmid);
                }
            }

            return isPasswordReset;
        }

        /// <summary>
        /// Resets password
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static bool ResetPassword(int cmid)
        {
            string newPassword = CmuneMember.GeneratePassword();

            bool isPasswordChanged = Users.ChangePassword(cmid, newPassword);

            if (isPasswordChanged)
            {
                ID3 id3 = CmuneMember.GetId3(cmid);
                Member member = CmuneMember.GetMember(cmid);

                if (id3 != null && member != null)
                {
                    CmuneMail.SendEmailPasswordReset(member.Login, id3.Name, newPassword, (EmailAddressStatus) member.EmailAddressState);
                }
            }

            return isPasswordChanged;
        }
   
        #endregion Data modification

        /// <summary>
        /// Deletes a member and everything connected to it.
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public static MemberOperationResult DeleteMember(int cmid)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                throw new ArgumentOutOfRangeException("cmid", cmid, "The CMID should be >= 1.");
            }

            #endregion

            ret = CmuneMember.DeleteMember(cmid);
            if (ret.Equals(MemberOperationResult.Ok))
            {
                DeleteMemberMiniverses(cmid);
            }

            return ret;
        }

        #region Sync over Miniverses

        /// <summary>
        /// Sync a password changing over all the miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        private static bool ChangeHashedPasswordMiniverses(int cmid, string hashedPassword)
        {
            bool ret = false;

            if (!CmuneMember.IsMemberEsnsOnly(cmid))
            {
                ret = PPMember.ChangePasswordMiniverse(cmid, hashedPassword);
            }

            return ret;
        }

        /// <summary>
        /// Sync a user name changing over all the miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private static MemberOperationResult ChangeUsernameMiniverses(int cmid, string username)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;
            bool isMemberEsnsOnly = CmuneMember.IsMemberEsnsOnly(cmid);

            ret = PPMember.ChangeUsernameMiniverse(username, cmid, false, isMemberEsnsOnly);

            return ret;
        }

        /// <summary>
        /// Sync a user name changing over all the miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private static MemberOperationResult ChangeEmailMiniverses(int cmid, string email)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            ret = PPMember.ChangeEmailMiniverse(email, cmid, false);

            return ret;
        }

        /// <summary>
        /// Sync the transformation of an ESNS only member into a full Cmune Member to all the miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static MemberOperationResult RegisterEsnsMemberMiniverses(int cmid, string email, string password)
        {
            return PPMember.CompleteEsnsUser(cmid, email, password, false);
        }

        /// <summary>
        /// Deletes the member on all the miniverses
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        private static MemberOperationResult DeleteMemberMiniverses(int cmid)
        {
            MemberOperationResult ret = MemberOperationResult.MemberNotFound;

            #region Check input

            if (cmid < 1)
            {
                ret = MemberOperationResult.InvalidCmid;
            }

            #endregion Check input

            if (ret.Equals(MemberOperationResult.MemberNotFound))
            {
                ret = PPMember.DeletePPUser(cmid);
            }

            return ret;
        }

        #endregion Sync over Miniverses

        #region Merge

        /// <summary>
        /// Merges 2 accounts
        /// </summary>
        /// <param name="cmidToKeep"></param>
        /// <param name="cmidToDelete"></param>
        /// <param name="mergePointsMode"></param>
        /// <returns></returns>
        public static MemberMergeResult MergeAccounts(int cmidToKeep, int cmidToDelete, MergePointsMode mergePointsMode)
        {
            MemberMergeResult ret = MemberMergeResult.CmidNotFound;

            ret = CmuneMember.MergeAccounts(cmidToKeep, cmidToDelete, mergePointsMode);

            if (ret.Equals(MemberMergeResult.Ok))
            {
                // TODO Merge applications
                DeleteMember(cmidToDelete);
            }

            return ret;
        }

        #endregion Merge
    }
}