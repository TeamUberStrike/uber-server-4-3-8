using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Common.Entities
{
    public static class CoreEnumsErrorMessage
    {
        public static string GetMemberRegistrationErrorMessage(this MemberAuthenticationResult memberRegistrationResult)
        {
            string message = string.Empty;

            switch (memberRegistrationResult)
            {
                case MemberAuthenticationResult.Ok:
                    break;
                case MemberAuthenticationResult.InvalidData:
                    message = "Your e-mail address or password is incorrect. Double check you typed them in correctly and try again!";
                    break;
                case MemberAuthenticationResult.InvalidEmail:
                    message = "Your e-mail address or password is incorrect. Double check you typed them in correctly and try again!";
                    break;
                case MemberAuthenticationResult.InvalidPassword:
                    message = "Your e-mail address or password is incorrect. Double check you typed them in correctly and try again!";
                    break;
                case MemberAuthenticationResult.IsBanned:
                    message = "";
                    break;
                default:
                    message = "Sorry, our systems are having trouble logging you in. Please try again in a few minutes.";
                    break;
            }

            return message;
        }

        public static string GetMemberOperationErrorMessage(this MemberOperationResult memberOperationResult)
        {
            string message = string.Empty;

            switch (memberOperationResult)
            {
                case MemberOperationResult.DuplicateEmail:
                    message = "Your email address is already in use by another account.";
                    break;
                case MemberOperationResult.InvalidPassword:
                    message = "That doesn't look like a valid password to me!";
                    break;
                case MemberOperationResult.Ok:
                    break;
                case MemberOperationResult.MemberNotFound:
                    message = "That e-mail address doesn't exist in our systems!";
                    break;
                case MemberOperationResult.InvalidEmail:
                    message = "That doesn't look like a valid e-mail address to me!";
                    break;
                default:
                    message = "Sorry, our system encountered a problem. Please try again and if the problem continues visit " + CommonConfig.CmuneSupportCenterUrl + ".";
                    break;
            }

            return message;
        }

        public static string GetBanModeErrorMessage(this BanMode banMode, string info)
        {
            string message = string.Empty;

            switch (banMode)
            {
                case BanMode.No:
                    break;
                case BanMode.Temporary:
                    message = "Your account has been banned until " + info + ". If you want to dispute the ban please visit " + CommonConfig.CmuneSupportCenterUrl + ".";
                    break;
                case BanMode.Permanent:
                    message = "Your account has been permanently banned. Common ban offences include cheating and in-game abuse. If you want to dispute the ban please visit " + CommonConfig.CmuneSupportCenterUrl + ".";
                    break;
                default:
                    break;
            }

            return message;
        }
    }
}
