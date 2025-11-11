using System;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.WebService.Interfaces;
using System.Linq;

namespace UberStrike.DataCenter.WebService
{
    public class AuthenticationWebService : IAuthenticationWebService
    {
        public MemberRegistrationResult CreateUser(string emailAddress, string password, ChannelType channel, string locale, string machineId)
        {
            MemberRegistrationResult result = MemberRegistrationResult.InvalidData;
            int cmid = 0;

            long networkAddress = WebServiceUtil.GetCurrentContextNetworkAddress();

            if (!CmuneMember.IsIpBanned(networkAddress))
            {
                result = Users.CreateUser(emailAddress, password, channel, networkAddress, locale, out cmid);
            }
            else
            {
                result = MemberRegistrationResult.IsIpBanned;
            }

            return result;
        }

        public AccountCompletionResultView CompleteAccount(int cmid, string name, ChannelType channel, string locale, string machineId)
        {
            AccountCompletionResultView result = new AccountCompletionResultView();

            long networkAddress = WebServiceUtil.GetCurrentContextNetworkAddress();

            if (!CmuneMember.IsIpBanned(networkAddress))
            {
                result = Users.CompleteAccount(cmid, name, channel, locale, TextUtilities.InetNToA(networkAddress));
            }
            else
            {
                result = new AccountCompletionResultView(UberStrikeAccountCompletionResult.IsIpBanned);
            }

            return result;
        }

        public bool UncompleteAccount(int cmid)
        {
            return Users.UncompleteAccount(cmid);
        }

        public MemberAuthenticationResultView LoginMemberEmail(string email, string password, ChannelType channelType, string machineId)
        {
            long networkAddress = WebServiceUtil.GetCurrentContextNetworkAddress();

            Member loggedMember = null;
            MemberAuthenticationResult memberAuth = CmuneMember.CmuneLoginEmail(email, password, UberStrikeCommonConfig.ApplicationId, out loggedMember);

            int cmid = 0;
            if (loggedMember != null) cmid = loggedMember.CMID;

            var temp = TreatLogin(cmid, memberAuth, channelType, networkAddress, machineId);
            return temp;
        }

        public MemberAuthenticationResultView LoginMemberCookie(int cmid, System.DateTime expirationTime, string encryptedContent, string hash, ChannelType channelType, string machineId)
        {
            MemberAuthenticationResult memberAuth = MemberAuthenticationResult.InvalidCookie;
            long networkAddress = WebServiceUtil.GetCurrentContextNetworkAddress();

            if (CmuneCookie.IsMemberAuthenticated(cmid, expirationTime, encryptedContent, hash))
            {
                MemberAccess memberAccess = CmuneMember.GetMemberAccess(cmid, UberStrikeCommonConfig.ApplicationId);

                if (memberAccess.IsAccountDisabled == (int)BanMode.No)
                {
                    memberAuth = MemberAuthenticationResult.Ok;
                }
                else
                {
                    memberAuth = MemberAuthenticationResult.IsBanned;
                }
            }

            return TreatLogin(cmid, memberAuth, channelType, networkAddress, machineId);
        }

        /// <summary>
        /// Centralizes the login returned variables
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="memberAuth"></param>
        /// <param name="channelType"></param>
        /// <param name="networkAddress"></param>
        /// <param name="machineId"></param>
        /// <param name="ret"></param>
        private static MemberAuthenticationResultView TreatLogin(int cmid, MemberAuthenticationResult memberAuth, ChannelType channelType, long networkAddress, string machineId)
        {
            MemberAuthenticationResult result = memberAuth;
            MemberView memberView = null;
            PlayerStatisticsView statistics = null;
            DateTime serverTime = DateTime.Now;
            bool isAccountComplete = false;
            bool isTutorialComplete = false;
            WeeklySpecialView weeklySpecial = null;
            LuckyDrawUnityView luckyDraw = null;

            string memberName = String.Empty;

            if (memberAuth.Equals(MemberAuthenticationResult.Ok))
            {
                Users.ExpireLoadout(cmid);
                if (!CmuneMember.IsIpBanned(networkAddress))
                {
                    isAccountComplete = CmuneMember.IsAccountComplete(cmid);

                    if (!isAccountComplete)
                    {
                        if (ConfigurationUtilities.ReadConfigurationManagerBool("IsTutorialEnable"))
                        {
                            isTutorialComplete = Tracking.HasCompletedTutorial(cmid);
                        }
                        else
                        {
                            isTutorialComplete = true;
                        }
                    }
                    else
                    {
                        isTutorialComplete = true;
                    }

                    memberView = CmuneMember.GetMember(cmid, UberStrikeCommonConfig.ApplicationId);

                    if (memberView != null)
                    {
                        memberName = memberView.PublicProfile.Name;
                    }

                    statistics = Statistics.GetCompleteStatisticsView(cmid);

                    luckyDraw = CmuneMember.GetDailyLuckyDrawOnLogin(cmid);

                    weeklySpecial = WeeklySpecialService.GetCurrentWeeklySpecial();

                    CmuneMember.RecordLogin(cmid, channelType, networkAddress, machineId);
                }
                else
                {
                    result = MemberAuthenticationResult.IsIpBanned;
                }
            }

            return new MemberAuthenticationResultView()
            {
                MemberAuthenticationResult = result,
                MemberView = memberView,
                PlayerStatisticsView = statistics,
                ServerTime = serverTime,
                IsAccountComplete = isAccountComplete,
                IsTutorialComplete = isTutorialComplete,
                WeeklySpecial = weeklySpecial,
                LuckyDraw = luckyDraw,
            };
        }
    }
}
