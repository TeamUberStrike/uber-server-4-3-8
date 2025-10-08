using System;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using UberStrike.Core.Types;

namespace UberStrike.DataCenter.Utils
{
    /// <summary>
    /// This class will allow us to centralize all the email sending
    /// </summary>
    public static class UberStrikeMail
    {
        /// <summary>
        /// To Send a notification to the user for an action taken by the admin
        /// </summary>
        /// <param name="typeEmailNotification"></param>
        /// <param name="explanation"></param>
        /// <param name="memberEmail"></param>
        /// <param name="memberName"></param>
        /// <param name="emailStatus"></param>
        public static void SendEmailNotification(UberStrikeEmailNotificationType typeEmailNotification, string explanation, string memberEmail, string memberName, EmailAddressStatus emailStatus)
        {
            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string body = String.Empty;
                string htmlBody = String.Empty;
                string textBody = String.Empty;
                string subject = String.Empty;
                bool shouldWeSendEmail = true;

                string htmlGreetings = String.Format("<p><b>{0}</b>,<br /><br />", memberName);
                string textGreetings = String.Format("{0},\n\n", memberName);

                string htmlSignature = String.Format("<br /><br />If you want to have more information about this decision, feel free emailing us at {0}.</p><p>Sincerely,<br />The Cmune Team</p>", CommonConfig.CmuneSupportEmail);
                string textSignature = String.Format("\n\nIf you want to have more information about this decision, feel free emailing us at {0}.\n\nSincerely,\nThe Cmune Team", CommonConfig.CmuneSupportEmail);

                string htmlExplanation = String.Empty;
                string textExplanation = String.Empty;

                if (!TextUtilities.IsNullOrEmpty(explanation))
                {
                    htmlExplanation = String.Format("<br />Here are a few words explaining this decision:<br /><br />{0}", explanation);
                    textExplanation = String.Format("\nHere are a few words explaining this decision:\n\n{0}", explanation);
                }

                switch (typeEmailNotification)
                {
                    case UberStrikeEmailNotificationType.ItemAttributed:
                        body = "Your Cmune live inventory has been changed, we added your new items.";
                        subject = "Your Cmune live inventory has been changed";
                        break;
                    default:
                        shouldWeSendEmail = false;
                        CmuneLog.LogUnexpectedReturn(typeEmailNotification, "explanation=" + explanation + "&memberEmail=" + memberEmail + "&memberName=" + memberName);
                        break;
                }

                if (!memberEmail.IsNullOrFullyEmpty() && shouldWeSendEmail)
                {
                    htmlBody = htmlGreetings + body + htmlExplanation + htmlSignature;
                    textBody = textGreetings + body + textExplanation + textSignature;

                    CmuneMail.SendEmail(CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, memberEmail, memberName, subject, htmlBody, textBody);
                }
            }
        }

        /// <summary>
        /// Sends an email for activating an account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="validationUrl"></param>
        /// <param name="emailStatus"></param>
        public static void SendEmailActivateMember(string email, string memberName, string validationUrl, EmailAddressStatus emailStatus)
        {
            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string from = CommonConfig.CmuneSupportEmail;
                string fromName = CommonConfig.CmuneSupportEmailName;
                string to = email;
                string toName = memberName;
                string subject = "UberStrike - Activate your account";
                string htmlBody = String.Format("<html><body><p>Hello {0},</p><p>To activate your account please click on the following link <a href=\"{1}\">{1}</a></p><p>Please note that this link will expire in {2} days.</p><p>If you didn't sign up to UberStrike, please discard this e-mail and we won't e-mail you again.</p><p>Sincerely,<br />The Cmune Team</p></body></html>", HttpUtility.HtmlEncode(memberName), validationUrl, CommonConfig.IdentityValidationLifetimeInDays);
                string textBody = String.Format("Hello {0},\nTo activate your account please go this URL: {1}\nPlease note that this link will expire in {2} days.\nIf you didn't sign up to UberStrike, please discard this e-mail and we won't e-mail you again.\n\nSincerely,\nThe Cmune Team", HttpUtility.HtmlEncode(memberName), validationUrl, CommonConfig.IdentityValidationLifetimeInDays);

                CmuneMail.SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
            }
        }

        /// <summary>
        /// Send a bug report to our beloved team
        /// </summary>
        /// <param name="bugContent">no HTML</param>
        /// <param name="bugSubject"></param>
        /// <param name="email"></param>
        public static void BugReport(string bugContent, string bugSubject, string email)
        {
            string from = CommonConfig.CmuneSupportEmail;
            string fromName = CommonConfig.CmuneSupportEmailName;
            string to = email;
            string toName = "Neel, the great Cmune bug hunter!";
            string htmlBody = String.Format("<html><body><p>Hello,</p><br /><br /><p>Here is a bug report on the {0}:</p><br /><p>{1}</p><br /><br /><p>Sincerely,<br />The annoying reporting team.</p></body></html>", DateTime.Now, bugContent);
            string textBody = String.Format("Hello,\n\nHere is a bug report on the {0}:\n\n{1}\n\nSincerely,\nThe annoying reporting team.", DateTime.Now, bugContent);

            CmuneMail.SendEmail(from, fromName, to, toName, bugSubject, htmlBody, textBody);
        }

        /// <summary>
        /// Reports an unauthorized access to staging
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="memberName"></param>
        /// <param name="networkAddress"></param>
        public static void ReportUnauthorizedStagingAccess(int cmid, string memberName, long networkAddress)
        {
            string from = CommonConfig.CmuneDevteamEmail;
            string fromName = CommonConfig.CmuneDevteamEmailName;
            string to = ConfigurationUtilities.ReadConfigurationManager("EmailToReportUnauthorizedLogin");
            string toName = "Lee";
            string subject = "[Staging][Unauthorized access]";
            string htmlBody = String.Format("<html><body><p>Hello dude,</p><p>We have a leak in staging</p><ul><li>Name: {0}</li><li>Cmid: {1}</li><li>Prod profile: <a href=\"http://admin.cmune.com/Member/See?Cmid={1}\">http://admin.cmune.com/Member/See?Cmid={1}</a></li><li>Staging profile: <a href=\"http://staging.admin.cmune.com/Member/See?Cmid={1}\">http://staging.admin.cmune.com/Member/See?Cmid={1}</a></li><li>IP: {2} ({3})</li></ul><p>Together let's catch the hackers!</p></body></html>", memberName, cmid, TextUtilities.InetNToA(networkAddress), networkAddress);
            string textBody = String.Format("Hello dude,\n\nWe have a leak in staging\n\nName: {0}\nCmid: {1}\nProd profile: http://admin.cmune.com/Member/See?Cmid={1}\nStaging profile: http://staging.admin.cmune.com/Member/See?Cmid={1}\nIP: {2} ({3})\n\nTogether let's catch the hackers!", memberName, cmid, TextUtilities.InetNToA(networkAddress), networkAddress);

            CmuneMail.SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
        }
    }
}