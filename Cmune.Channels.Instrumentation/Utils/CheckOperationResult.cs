using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Utils
{
    public static class CheckOperationResult
    {
        public static void CheckMemberRegistrationResult(MemberRegistrationResult memberRegistrationResult, ref bool isDataValid, ref string message)
        {
            switch (memberRegistrationResult)
            {
                case MemberRegistrationResult.Ok:
                    message = "The user is successfully created";
                    isDataValid = true;
                    break;
                case MemberRegistrationResult.DuplicateEmail:
                    message = "This email address is already use by a member.";
                    break;
                case MemberRegistrationResult.DuplicateName:
                    message = "This member name is already used by a member.";
                    break;
                case MemberRegistrationResult.DuplicateEmailName:
                    message = "This member name and this email address are already used by a member.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(String.Format("The adding method returning an inexpected value ({0}). This error has been logged", memberRegistrationResult));
            }
        }

        public static void CheckMemberOperationResult(MemberOperationResult memberOperationResult, ref bool isModified, ref string message)
        {
            switch (memberOperationResult)
            {
                case MemberOperationResult.Ok:
                    message = "Operation successfully DONE<br />";
                    isModified = true;
                    break;
                case MemberOperationResult.MemberNotFound:
                    message = "This user is not stored in our database.<br />";
                    break;
                case MemberOperationResult.DuplicateEmail:
                    message = "This email is alredy in use. Please select another one.<br />";
                    break;
                case MemberOperationResult.InvalidEmail:
                    message = "This email is not valid. Please choose another one.<br />";
                    break;
                case MemberOperationResult.DuplicateName:
                    message = "This name is already in use. Please choose another one.<br />";
                    break;
                case MemberOperationResult.InvalidName:
                    message = "This name is not valid. Please choose another one.<br />";
                    break;
                default:
                    message = "Error occured";
                    break;
            }
        }

        public static void CheckGroupOperationResult(int uberStrikeGroupOperationResult, ref bool isModified, ref string message)
        {
            switch (uberStrikeGroupOperationResult)
            {
                // match also UberStrikeGroupOperationResult.Ok
                case GroupOperationResult.Ok:
                    message = "Operation successfully DONE!";
                    isModified = true;
                    break;
                case UberStrikeGroupOperationResult.InvalidTag:
                    message = "This tag is not valid.<br />";
                    break;
                case UberStrikeGroupOperationResult.OffensiveTag:
                    message = "This tag is offensive.<br />";
                    break;
                // match also UberStrikeGroupOperationResult.GroupNotFound:
                case GroupOperationResult.GroupNotFound:
                    message = "This group was not found.<br />";
                    break;
                case UberStrikeGroupOperationResult.DuplicateTag:
                    message = "This tag is already used.<br />";
                    break;
                case GroupOperationResult.InvalidName:
                    message = "This name is not valid.<br />";
                    break;
                case GroupOperationResult.OffensiveName:
                    message = "This name is offensive.<br />";
                    break;
                case GroupOperationResult.DuplicateName:
                    message = "This name is already used.<br />";
                    break;
                case GroupOperationResult.InvalidMotto:
                    message = "This motto is not valid.<br />";
                    break;
                case GroupOperationResult.OffensiveMotto:
                    message = "This motto is offensive.<br />";
                    break;
                default:
                    message = "This return type is not handled by our system yet.";
                    break;
            }
        }

        public static void CheckMonitoringOperationResult(MonitoringOperationResult monitoring, ref string message, ref bool isAddOrEdit)
        {
            switch (monitoring)
            {
                case MonitoringOperationResult.Ok:
                    message = "Operation Successfully Done";
                    isAddOrEdit = true;
                    break;
                case MonitoringOperationResult.DuplicateName:
                    message = "This name is already taken.";
                    break;
                case MonitoringOperationResult.InvalidIP:
                    message = "This IP is not well formed.";
                    break;
                case MonitoringOperationResult.InvalidName:
                    message = "This name is invalid.";
                    break;
                case MonitoringOperationResult.ServerNotFound:
                    message = "This server does not exist.";
                    break;
                case MonitoringOperationResult.TestInvalidConnectionString:
                    message = "This ConnectionString is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidSqlQuery:
                    message = "This SQL query is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidSocket:
                    message = "This socket is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidUrl:
                    message = "This URL is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidPassPhrase:
                    message = "This Pass Phrase is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidInitVector:
                    message = "This Init Vector is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidPostContent:
                    message = "This POST content is invalid.";
                    break;
                case MonitoringOperationResult.TestInvalidPostField:
                    message = "This POST field is invalid.";
                    break;
            }
        }


    }
}