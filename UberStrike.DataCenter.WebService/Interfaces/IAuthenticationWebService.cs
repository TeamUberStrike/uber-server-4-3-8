using System;
using Cmune.DataCenter.Common.Entities;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.WebService.Attributes;

namespace UberStrike.DataCenter.WebService.Interfaces
{
    [CmuneWebServiceInterface]
    public interface IAuthenticationWebService
    {
        MemberRegistrationResult CreateUser(string emailAddress, string password, ChannelType channel, string locale, string machineId);
        AccountCompletionResultView CompleteAccount(int cmid, string name, ChannelType channel, string locale, string machineId);
        bool UncompleteAccount(int cmid);
        MemberAuthenticationResultView LoginMemberEmail(string email, string password, ChannelType channelType, string machineId);
        MemberAuthenticationResultView LoginMemberCookie(int cmid, DateTime expirationTime, string encryptedContent, string hash, ChannelType channelType, string machineId);
    }
}