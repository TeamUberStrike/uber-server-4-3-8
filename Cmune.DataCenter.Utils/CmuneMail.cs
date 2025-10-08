using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using System.Collections.Generic;

namespace Cmune.DataCenter.Utils
{
    /// <summary>
    /// This class will allow us to centralize all the email sending
    /// </summary>
    public static class CmuneMail
    {
        /// <summary>
        /// To Send a notification to the user for an action taken by the admin
        /// </summary>
        /// <param name="emailNotificationType"></param>
        /// <param name="explanation">Shouldn't contain HTML</param>
        /// <param name="memberEmail"></param>
        /// <param name="memberName"></param>
        /// <param name="emailStatus"></param>
        public static void SendEmailNotification(EmailNotificationType emailNotificationType, string explanation, string memberEmail, string memberName, EmailAddressStatus emailStatus)
        {
            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string htmlBody = String.Empty;
                string textBody = String.Empty;
                string subject = String.Empty;
                string body = String.Empty;
                bool shouldWeSendEmail = true;

                string htmlGreetings = "<p><b>" + memberName + "</b>,<br /><br />";
                string htmlSignature = "<br /><br />If you want to have more information about this decision, feel free emailing us at " + CommonConfig.CmuneSupportEmail + "</p><p>Sincerely,<br />The Cmune Team</p>";

                string textGreetings = String.Format("{0}\n\n", memberName);
                string textSignature = String.Format("\n\nIf you want to have more information about this decision, feel free emailing us at {0}\n</p><p>Sincerely,<br />The Cmune Team</p>", CommonConfig.CmuneSupportEmail);

                string htmlExplanation = String.Empty;
                string textExplanation = String.Empty;

                if (!TextUtilities.IsNullOrEmpty(explanation))
                {
                    htmlExplanation = String.Format("<br />Here are a few words explaining this decision:<br /><br />{0}", explanation);
                    textExplanation = String.Format("\nHere are a few words explaining this decision:\n\n{0}", explanation);
                }

                switch (emailNotificationType)
                {
                    case EmailNotificationType.BanMemberPermanent:
                        body = "Your account on Cmune has been permanently disabled.";
                        subject = "Your Cmune account is permanently disabled";
                        break;
                    case EmailNotificationType.BanMemberTemporary:
                        body = "Your account on Cmune has been temporarily disabled.";
                        subject = "Your Cmune account is temporarily disabled";
                        break;
                    case EmailNotificationType.UnbanMember:
                        body = "Your account on Cmune is active again.";
                        subject = "Your Cmune account is active again";
                        break;
                    case EmailNotificationType.DeleteMember:
                        body = "Your account on Cmune has been deleted.";
                        subject = "Your Cmune account has been deleted";
                        break;
                    case EmailNotificationType.MergeMembers:
                        body = "Your account on Cmune has been merged.";
                        subject = "Your Cmune accounts have been merged";
                        break;
                    case EmailNotificationType.ChangeMemberName:
                        body = "Your Cmune name has been changed.";
                        subject = "Your Cmune name has been changed";
                        break;
                    case EmailNotificationType.ChangeMemberPassword:
                        body = "Your Cmune password has been changed.";
                        subject = "Your Cmune password has been changed";
                        break;
                    case EmailNotificationType.ChangeMemberEmail:
                        body = "Your Cmune email has been changed.";
                        subject = "Your Cmune email has been changed";
                        break;
                    case EmailNotificationType.BanMemberChatPermanent:
                        body = "Your account on Cmune has been disabled for the chat.";
                        subject = "Your Cmune account has been disabled for the chat";
                        break;
                    case EmailNotificationType.BanMemberChatTemporary:
                        body = "Your account on Cmune has been temporarily disabled for the chat.";
                        subject = "Your Cmune account is temporarily disabled for the chat";
                        break;
                    case EmailNotificationType.UnbanMemberChat:
                        body = "Your account on Cmune is active again for the chat.";
                        subject = "Your Cmune account is active again for the chat";
                        break;
                    case EmailNotificationType.ChangeClanTag:
                        body = "Your Cmune clan tag has been changed.";
                        subject = "Your Cmune clan tag has been changed";
                        break;
                    case EmailNotificationType.ChangeClanName:
                        body = "Your Cmune clan name has been changed.";
                        subject = "Your Cmune clan name has been changed";
                        break;
                    case EmailNotificationType.ChangeClanMotto:
                        body = "Your Cmune clan motto has been changed.";
                        subject = "Your Cmune clan motto has been changed";
                        break;
                    default:
                        shouldWeSendEmail = false;
                        CmuneLog.LogUnexpectedReturn(emailNotificationType, "explanation=" + explanation + "&memberEmail=" + memberEmail + "&memberName=" + memberName);
                        break;
                }

                if (!memberEmail.IsNullOrFullyEmpty() && shouldWeSendEmail)
                {
                    htmlBody = htmlGreetings + body + htmlExplanation + htmlSignature;
                    textBody = textGreetings + body + textExplanation + textSignature;

                    SendEmail(CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, memberEmail, memberName, subject, htmlBody, textBody);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">You don't need to HTML encode the name</param>
        /// <param name="email"></param>
        /// <param name="points"></param>
        /// <param name="credits"></param>
        /// <param name="explanation"></param>
        /// <param name="emailStatus"></param>
        /// <returns></returns>
        public static bool PointsCreditsModified(string name, string email, int points, int credits, string explanation, EmailAddressStatus emailStatus)
        {
            bool isEmailSent = false;

            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string subject = String.Empty;
                StringBuilder htmlBody = new StringBuilder();
                StringBuilder textBody = new StringBuilder();

                htmlBody.Append(String.Format("<p>Hi {0},<br /><br />", HttpUtility.HtmlEncode(name)));
                textBody.Append(String.Format("Hi {0},\n", name));

                if (points != 0 && credits != 0)
                {
                    subject = "Your Cmune balance has been modified";
                    htmlBody.Append("Your Cmune balance has been modified:<ul>");
                    textBody.Append("Your Cmune balance has been modified:\n");
                }
                else if (points != 0)
                {
                    subject = "Your Cmune points balance have been modified";
                    htmlBody.Append("Your points balance has been modified:<ul>");
                    textBody.Append("Your points balance has been modified:\n");
                }
                else if (credits != 0)
                {
                    subject = "Your Cmune credits balances have been modified";
                    htmlBody.Append("Your credits balance has been modified:<ul>");
                    textBody.Append("Your credits balance has been modified:\n");
                }

                if (points > 0)
                {
                    htmlBody.Append("<li>We added you ");
                    textBody.Append("We added you ");
                }
                else if (points < 0)
                {
                    htmlBody.Append("<li>We removed you ");
                    textBody.Append("We removed you ");
                    points = -points;
                }

                if (points != 0)
                {
                    htmlBody.Append(String.Format("{0} points.</li>", points));
                    textBody.Append(String.Format("{0} points.\n", points));
                }

                if (credits > 0)
                {
                    htmlBody.Append("<li>We added you ");
                    textBody.Append("We added you ");
                }
                else if (credits < 0)
                {
                    htmlBody.Append("<li>We removed you ");
                    textBody.Append("We removed you ");
                    credits = -credits;
                }

                if (credits != 0)
                {
                    htmlBody.Append(String.Format("{0} credits.</li>", credits));
                    textBody.Append(String.Format("{0} credits.\n", credits));
                }

                if (points != 0 || credits != 0)
                {
                    htmlBody.Append("</ul>");
                    textBody.Append("\n");
                }

                if (!explanation.IsNullOrFullyEmpty())
                {
                    htmlBody.Append(String.Format("Here are a few words explaining this decision:</p><p style=\"padding:0 0 0 20px\"><i>{0}</i></p>", explanation));
                    textBody.Append(String.Format("Here are a few words explaining this decision:\n{0}\n", explanation));
                }

                htmlBody.Append(String.Format("<p>If you want to have more information about this decision, feel free emailing us at {0} </p><p>Sincerely,<br />The Cmune Team</p>", CommonConfig.CmuneSupportEmail));
                textBody.Append(String.Format("\nIf you want to have more information about this decision, feel free emailing us at {0} \nSincerely,\nThe Cmune Team", CommonConfig.CmuneSupportEmail));

                if (!email.IsNullOrFullyEmpty())
                {
                    isEmailSent = SendEmail(CommonConfig.CmuneSupportEmail, CommonConfig.CmuneSupportEmailName, email, name, subject, htmlBody.ToString(), textBody.ToString());
                }
            }

            return isEmailSent;
        }
        
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="fromEmail"></param>
        /// <param name="fromName"></param>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="htmlBody"></param>
        /// <param name="textBody"></param>
        /// <returns></returns>
        public static bool SendEmail(string fromEmail, string fromName, string toEmail, string toName, string subject, string htmlBody, string textBody)
        {
            bool isEmailSend = true;

            try
            {
                SendGridSdk.SendGridMail.SendSingleEmail(toEmail, toName, fromEmail, fromName, subject, htmlBody, textBody);
                isEmailSend = true;
            }
            catch (Exception ex)
            {
                isEmailSend = false;
                CmuneLog.LogException(ex, String.Format("fromEmail={0}&fromName={1}&toEmail={2}&toName={3}&subject={4}&htmlBody={5}&textBody={6}", fromEmail, fromName, toEmail, toName, subject, htmlBody, textBody));
            }

            return isEmailSend;
        }

        /// <summary>
        /// Sends an email to a new user from the portal / stand alone and didn't use Fb Connect
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="userName"></param>
        /// <param name="verificationUrl"></param>
        public static void SendEmailNewUser(string emailAddress, string userName, string verificationUrl)
        {
            string from = CommonConfig.CmuneSupportEmail;
            string fromName = CommonConfig.CmuneSupportEmailName;
            string to = emailAddress;
            string toName = userName;
            string subject = String.Format(MailTemplates.NewUserSubject, userName);
            string htmlBody = String.Format(MailTemplates.NewUserHtml, userName, verificationUrl);
            string textBody = String.Format(MailTemplates.NewUserText, userName, verificationUrl);

            SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
        }

        /// <summary>
        /// Sends an email to a new user from Facebook / Fb Connect
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SendEmailNewFacebookUser(string emailAddress, string userName, string password)
        {
            string from = CommonConfig.CmuneSupportEmail;
            string fromName = CommonConfig.CmuneSupportEmailName;
            string to = emailAddress;
            string toName = userName;
            string subject = String.Format(MailTemplates.NewUserSubject, userName);
            string htmlBody = String.Format(MailTemplates.NewUserFacebookHtml, userName, emailAddress, password);
            string textBody = String.Format(MailTemplates.NewUserFacebookText, userName, emailAddress, password);

            SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
        }

        public static void SendEmailUnsuccessfulFacebookRegistration(List<string> emailAddress, List<string> firstName)
        {
            SendGridSdk.SendGridMail.SendMultipleEmails(CommonConfig.CmuneSupportEmail,
                                                        CommonConfig.CmuneSupportEmailName,
                                                        MailTemplates.UnsuccessfulRegistrationSubject,
                                                        MailTemplates.UnsuccessfulFacebookRegistrationHtml,
                                                        MailTemplates.UnsuccessfulFacebookRegistrationText,
                                                        emailAddress,
                                                        new Dictionary<string, List<string>> { { "#name#", firstName } },
                                                        new List<string>());
        }

        public static void SendEmailUnsuccessfulStandaloneRegistration(List<string> emailAddress)
        {
            SendGridSdk.SendGridMail.SendMultipleEmails(CommonConfig.CmuneSupportEmail,
                                                        CommonConfig.CmuneSupportEmailName,
                                                        MailTemplates.UnsuccessfulRegistrationSubject,
                                                        MailTemplates.UnsuccessfulStandAloneRegistrationHtml,
                                                        MailTemplates.UnsuccessfulStandAloneRegistrationText,
                                                        emailAddress,
                                                        new Dictionary<string, List<string>>(),
                                                        new List<string>());
        }

        public static void SendEmailUnsuccessfulPortalRegistration(List<string> emailAddress)
        {
            SendGridSdk.SendGridMail.SendMultipleEmails(CommonConfig.CmuneSupportEmail,
                                                        CommonConfig.CmuneSupportEmailName,
                                                        MailTemplates.UnsuccessfulRegistrationSubject,
                                                        MailTemplates.UnsuccessfulPortalRegistrationHtml,
                                                        MailTemplates.UnsuccessfulPortalRegistrationText,
                                                        emailAddress,
                                                        new Dictionary<string, List<string>>(),
                                                        new List<string>());
        }

        /// <summary>
        /// Sends a new password to a member
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="password"></param>
        /// <param name="applicationName"></param>
        /// <param name="accountUrl"></param>
        /// <param name="resetUrl"></param>
        /// <param name="emailStatus"></param>
        public static void SendEmailNewPassword(string email, string memberName, string password, string applicationName, string accountUrl, string resetUrl, EmailAddressStatus emailStatus)
        {
            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string from = CommonConfig.CmuneSupportEmail;
                string fromName = CommonConfig.CmuneSupportEmailName;
                string to = email;
                string toName = memberName;
                string subject = String.Format("Your new password for {0}", applicationName);
                string htmlBody = String.Format("<html><body><p>Hello {0},</p><p>You requested a new password, after clicking on this <a href=\"{1}\">link</a> (<a href=\"{1}\">{1}</a>) you'll be able to login using the following email address and password:</p><ul><li>Your email address is: {2}</li><li>Your new password is: <b>{3}</b></li></ul><p><b>Note</b> that if you don't click on the activation link you'll still be able to login using your old password.</p><p>You can change your password on your <a href=\"{4}\">Account page</a> (<a href=\"{4}\">{4}</a>).</p><p>Sincerely,<br />The Cmune Team</p></body></html>", HttpUtility.HtmlEncode(memberName), resetUrl, HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password), accountUrl);
                string textBody = String.Format("Hello {0},\nYou requested a new password, first go to this link: {1} then you'll be able to login using the following email address and password:\nYour email address is: {2}\nYour new password is: {3}\nNote that if you don't click on the activation link you'll still be able to login using your old password.\nYou can change your password on your account page: {4}.\nSincerely,\nThe Cmune Team", HttpUtility.HtmlEncode(memberName), resetUrl, HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password), accountUrl);

                SendEmail(from, fromName, to, toName, subject, textBody, textBody);
            }
        }

        /// <summary>
        /// Sends a password to a member after an admin did reset his password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="password"></param>
        /// <param name="emailStatus"></param>
        public static void SendEmailPasswordReset(string email, string memberName, string password, EmailAddressStatus emailStatus)
        {
            if (emailStatus != EmailAddressStatus.Invalid)
            {
                string from = CommonConfig.CmuneSupportEmail;
                string fromName = CommonConfig.CmuneSupportEmailName;
                string to = email;
                string toName = memberName;
                string subject = "We did reset your Cmune password";
                string htmlBody = String.Format("<html><body><p>Hello {0},</p><p>We did reset your Cmune password, you'll be able to login using the following email address and password:</p><ul><li>Your email address is: {1}</li><li>Your new password is: <b>{2}</b></li></ul><p>Sincerely,<br />The Cmune Team</p></body></html>", HttpUtility.HtmlEncode(memberName), HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password));
                string textBody = String.Format("Hello {0},\nWe did reset your Cmune password, you'll be able to login using the following email address and password:\nYour email address is: {1}\nYour new password is: {2}\nSincerely,\nThe Cmune Team", HttpUtility.HtmlEncode(memberName), HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password));

                SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
            }
        }

        /// <summary>
        /// Sends an email when a user becomes a "full" Cmune member (email + password, not only name)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="password"></param>
        /// <param name="accountUrl"></param>
        public static void RegisterEsnsMember(string email, string memberName, string password, string accountUrl)
        {
            RegisterEsnsMember(email, memberName, password, String.Empty, accountUrl);
        }

        /// <summary>
        /// Sends an email when a user becomes a "full" Cmune member (email + password, not only name)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="memberName"></param>
        /// <param name="password"></param>
        /// <param name="explanation">no HTML</param>
        /// <param name="accountUrl"></param>
        public static void RegisterEsnsMember(string email, string memberName, string password, string explanation, string accountUrl)
        {
            string from = CommonConfig.CmuneSupportEmail;
            string fromName = CommonConfig.CmuneSupportEmailName;
            string to = email;
            string toName = memberName;

            string htmlExplanation = String.Empty;
            string textExplanation = String.Empty;

            if (!TextUtilities.IsNullOrEmpty(explanation))
            {
                htmlExplanation = String.Format("<p>This is the reason why we completed your account:<br /><b>{0}</b></p>", explanation);
                textExplanation = String.Format("\nThis is the reason why we completed your account:\n{0}\n", explanation);
            }

            string subject = "Your Cmune account has just been completed";
            string htmlBody = String.Format("<html><body><p>Hello {0},</p><p>Your Cmune account has just been completed. {3} You can now login using the following Id:</p><p><ul><li>Your username is: {0}</li><li>Email: {1}</li><li>Password: {2}</li></ul><p>You can change your password on your <a href=\"{4}\">Account page</a></p><p>Sincerely,<br />The Cmune Team</p></body></html>", HttpUtility.HtmlEncode(memberName), HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password), HttpUtility.HtmlEncode(htmlExplanation), accountUrl);
            string textBody = String.Format("Hello {0},\nYour Cmune account has just been completed. {3} You can now login using the following Id:\n\nYour username is: {0}\nEmail: {1}\nPassword: {2}\n<p>You can change your password on your account page: {4}\n\nSincerely,\nThe Cmune Team", HttpUtility.HtmlEncode(memberName), HttpUtility.HtmlEncode(email), HttpUtility.HtmlEncode(password), HttpUtility.HtmlEncode(htmlExplanation), accountUrl);

            SendEmail(from, fromName, to, toName, subject, htmlBody, textBody);
        }

        /// <summary>
        /// Sends an email when one of our service is down
        /// </summary>
        /// <param name="to"></param>
        /// <param name="memberName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static bool SendAlertEmail(string to, string memberName, string body)
        {
            string from = CommonConfig.CmuneDevteamEmail;
            string fromName = CommonConfig.CmuneDevteamEmailName;
            string subject = "SERVER DOWN";

            bool isEmailSend = SendEmail(from, fromName, to, memberName, subject, body, body);

            return isEmailSend;
        }
    }

    static class MailTemplates
    {
        public const string NewUserSubject = @"Welcome to the party, {0}!";
        public static readonly string NewUserHtml = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body bgcolor=""#000000"">
<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                        
                        <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#0e0c11"">
                            <tbody><tr>
                                <td valign=""top"" style=""border-collapse:collapse"">

                                	
                                    <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left"">Verify your account to receive free points to spend on huge artillery and other items in UberStrike</div>
                                            </td>
                                            
											<td valign=""top"" width=""190"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left""></div>
                                            </td>
                                        </tr>
                                    </tbody></table>
                                	

                                </td>
                            </tr>
                        </tbody></table>
                        
                    	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border:1px solid #000000;background-color:#000000"">
                       	  <tbody><tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#000000;border-bottom:0;padding:0px"">
                                        <tbody><tr>
                                            <td style=""border-collapse:collapse;color:#202020;font-family:Arial;font-size:34px;font-weight:bold;line-height:100%;padding:0;text-align:center;vertical-align:middle"">

                                            	
                                            	<div style=""text-align:left""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/header.jpg"" alt="""" border=""0"" style=""margin:0;padding:0;display:block;margin-top:0;margin-bottom:0;border:0;min-height:auto;line-height:100%;outline:none;text-decoration:none"" width=""600"" height=""120""></div>
                                            	

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                    	<tbody><tr>
                                        	<td colspan=""3"" valign=""top"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#505050;font-family:Arial;font-size:14px;line-height:150%;text-align:left""><h1 style=""text-align:center;color:#202020;display:block;font-family:Arial;font-size:20px;font-weight:bold;line-height:100%;margin-top:0;margin-right:0;margin-bottom:10px;margin-left:0"">
	<span style=""color:rgb(255,255,255);font-size:30px;line-height:30px"">Welcome to the party, {0}!</span></h1>
<br>
<table border=""0"" width=""100%"">
	<tbody>
		<tr>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""60%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
		<tr>
			<td bgcolor=""#000000"" height=""50"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td align=""center"" bgcolor=""#FFCC00"" style=""border-collapse:collapse"">
				<a href=""{1}"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">Verify Your Account</span></a></td>
			<td bgcolor=""#000000"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
	</tbody>
</table>
<br>
</div>
														</td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    	<tr>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse""><div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                          <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
	<span style=""font-size:18px;color:rgb(255,255,255)"">Earn " + CommonConfig.PointsAttributedOnEmailValidation + @" FREE points</span><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Verify your account and receive " + CommonConfig.PointsAttributedOnEmailValidation + @" free points to spend on new guns.</span></font></span></h4>
<br>
<div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%"">
	<h4 style=""color:rgb(32,32,32);display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;font-size:22px;font-weight:bold;line-height:100%"">
		<font color=""#ffffff""><font size=""4"">Expand your arsenal</font></font><br>
		<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Explore the shop to discover new guns to blast your opponents with!</span></font></span></h4></div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                              <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
	<font color=""#ffffff"" size=""4""><span style=""line-height:100%"">Earn again tomorrow</span></font><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Play consecutive days to earn bonus points &amp; items!&nbsp;<br />
	</span></font></span><br />
	<br>
</h4>
                                                              <div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%"">
                                                                <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
		<span style=""font-weight:bold;line-height:22px;font-size:18px;color:rgb(255,255,255)"">Compete for prizes</span><br>
		<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Participate in tournaments &amp; other community events to win prizes.</span></font></span></h4>
</div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#000;border-top:0"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">

                                                
                                                <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td colspan=""2"" valign=""middle"" style=""border-collapse:collapse;background-color:#3b5998;border:0"">
                                                            <div style=""color:#ffffff; font-family:Arial; font-size:12px; line-height:125%; text-align:center; font-weight: bold;"">
                                                                <strong>&nbsp;<a href=""http://www.facebook.com/uberstrike?sk=wall"" style=""color:#ffffff;font-weight:bold;text-decoration:underline"" target=""_blank"">Like</a> us on Facebook&nbsp;
                                                            </strong></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign=""top"" width=""350"" style=""border-collapse:collapse"">
                                                            <div style=""color:#646464;font-family:Arial;font-size:12px;line-height:125%;text-align:left"">
																<br><br><br>
                                                            <em>Copyright © 2011 Cmune, All rights reserved.</em>
                                                            </div>
                                                        </td>
                                                        <td width=""190"" align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                                        <br>
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:12px;line-height:125%""><a href=""http://www.cmune.com/"" target=""new""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/cmunelogo.jpg"" width=""75"" height=""53"" alt=""cmune logo"" /></a></div><br>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        </tbody></table>
                        <br>
                    </td>
</td>
</body>
</html>
";
        public static readonly string NewUserText = @"Welcome to the party, {0}!\n\n
Verify your account: {1}\n\n
Earn " + CommonConfig.PointsAttributedOnEmailValidation + @" FREE points:\n
Verify your account and receive " + CommonConfig.PointsAttributedOnEmailValidation + @" free points to spend on new guns & items.\n\n
Expand your arsenal:\n
Explore the shop to discover new guns to blast your opponents with!\n\n
Earn again tomorrow:\n
Play consecutive days to earn bonus points!\n\n
Compete for prizes:\n
Participate in tournaments & other community events to win prizes.\n\n
Like us on Facebook: http://www.facebook.com/uberstrike";
        public const string NewUserFacebookHtml = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body bgcolor=""#000000"">
<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                        
                        <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#0e0c11"">
                            <tbody><tr>
                                <td valign=""top"" style=""border-collapse:collapse"">

                                	
                                    <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left"">Play to receive free points to spend on huge artillery and other items in UberStrike</div>
                                            </td>
                                            
											<td valign=""top"" width=""190"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left""></div>
                                            </td>
                                        </tr>
                                    </tbody></table>
                                	

                                </td>
                            </tr>
                        </tbody></table>
                        
                    	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border:1px solid #000000;background-color:#000000"">
                       	  <tbody><tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#000000;border-bottom:0;padding:0px"">
                                        <tbody><tr>
                                            <td style=""border-collapse:collapse;color:#202020;font-family:Arial;font-size:34px;font-weight:bold;line-height:100%;padding:0;text-align:center;vertical-align:middle"">

                                            	
                                            	<div style=""text-align:left""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/header.jpg"" alt="""" border=""0"" style=""margin:0;padding:0;display:block;margin-top:0;margin-bottom:0;border:0;min-height:auto;line-height:100%;outline:none;text-decoration:none"" width=""600"" height=""120""></div>
                                            	

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                    	<tbody><tr>
                                        	<td colspan=""3"" valign=""top"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#505050;font-family:Arial;font-size:14px;line-height:150%;text-align:left""><h1 style=""text-align:center;color:#202020;display:block;font-family:Arial;font-size:20px;font-weight:bold;line-height:100%;margin-top:0;margin-right:0;margin-bottom:10px;margin-left:0"">
	<span style=""color:rgb(255,255,255);font-size:30px;line-height:30px"">Welcome to the party, {0}!</span></h1>
<br>
<table border=""0"" width=""100%"">
	<tbody>
		<tr>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""60%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
		<tr>
			<td bgcolor=""#000000"" height=""50"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td align=""center"" bgcolor=""#FFCC00"" style=""border-collapse:collapse"">
				<a href=""http://apps.facebook.com/uberstrike/"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">Play for Free</span></a></td>
			<td bgcolor=""#000000"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
	</tbody>
</table>
<table border=""0"" width=""100%"">
    <tbody>
        <tr>
            <td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">You can login on our <a href=""http://forum.uberstrike.com/"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">forum</span></a>:
			</td>
        </tr>
        <tr>
            <td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">Email address: {1}
		    </td>
        </tr>
        <tr>
            <td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">Password: {2}
		    </td>
        </tr>
    </tbody>
</table>
<br>
</div>
														</td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    	<tr>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse""><div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                          <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%""><span style=""color:rgb(32,32,32);display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;font-size:22px;font-weight:bold;line-height:100%""><font color=""#ffffff""><font size=""4"">Expand your arsenal</font></font><br />
                                                          <span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Explore the shop to discover new guns to blast your opponents with!</span></font></span></span></h4>
<br>
<div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%"">
	<h4 style=""color:rgb(32,32,32);display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;font-size:22px;font-weight:bold;line-height:100%""><span style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%""><span style=""font-weight:bold;line-height:22px;font-size:18px;color:rgb(255,255,255)"">Compete for prizes</span><br />
        <span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Participate in tournaments &amp; other community events to win prizes.</span></font></span></span></h4>
</div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                              <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
	<font color=""#ffffff"" size=""4""><span style=""line-height:100%"">Earn points every day</span></font><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Play consecutive days to earn bonus points!&nbsp;<br />
	</span></font></span><br />
	<br>
</h4>
                                                              <div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%"">
                                                                <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">&nbsp;</h4>
</div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#000;border-top:0"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">

                                                
                                                <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td colspan=""2"" valign=""middle"" style=""border-collapse:collapse;background-color:#3b5998;border:0"">
                                                            <div style=""color:#ffffff; font-family:Arial; font-size:12px; line-height:125%; text-align:center; font-weight: bold;"">
                                                                <strong>&nbsp;<a href=""http://www.facebook.com/uberstrike?sk=wall"" style=""color:#ffffff;font-weight:bold;text-decoration:underline"" target=""_blank"">Like</a> us on Facebook&nbsp;
                                                            </strong></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign=""top"" width=""350"" style=""border-collapse:collapse"">
                                                            <div style=""color:#646464;font-family:Arial;font-size:12px;line-height:125%;text-align:left"">
																<br><br><br>
                                                            <em>Copyright © 2011 Cmune, All rights reserved.</em>
                                                            </div>
                                                        </td>
                                                        <td width=""190"" align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                                        <br>
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:12px;line-height:125%""><a href=""http://www.cmune.com/"" target=""new""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/cmunelogo.jpg"" width=""75"" height=""53"" alt=""cmune logo"" /></a></div><br>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        </tbody></table>
                        <br>
                    </td>
</td>
</body>
</html>";
        public const string NewUserFacebookText = @"Welcome to the party, {0}!\n\n
Play fo free: http://apps.facebook.com/uberstrike/\n\n
You can login on our forum: http://forum.uberstrike.com/\n
Email address: {1}\n
Password: {2}\n\n
Expand your arsenal:\n
Explore the shop to discover new guns to blast your opponents with!\n\n
Earn points every day:\n
Play consecutive days to earn bonus points!\n\n
Compete for prizes:\n
Participate in tournaments & other community events to win prizes.\n\n
Like us on Facebook: http://www.facebook.com/uberstrike";
        public const string UnsuccessfulRegistrationSubject = @"Was it something we said?";
        public const string UnsuccessfulFacebookRegistrationHtml = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body bgcolor=""#000000"">
<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                        
                        <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#0e0c11"">
                            <tbody><tr>
                                <td valign=""top"" style=""border-collapse:collapse"">

                                	
                                    <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left"">It looks like you had trouble setting up your UberStrike account. Was it something we said?</div>
                                            </td>
                                            
											<td valign=""top"" width=""190"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left""></div>
                                            </td>
											
                                        </tr>
                                    </tbody></table>
                                	

                                </td>
                            </tr>
                        </tbody></table>
                        
                    	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border:1px solid #000000;background-color:#000000"">
                       	  <tbody><tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#000000;border-bottom:0;padding:0px"">
                                        <tbody><tr>
                                            <td style=""border-collapse:collapse;color:#202020;font-family:Arial;font-size:34px;font-weight:bold;line-height:100%;padding:0;text-align:center;vertical-align:middle"">

                                            	
                                            	<div style=""text-align:left""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/header.jpg"" alt="""" border=""0"" style=""margin:0;padding:0;display:block;margin-top:0;margin-bottom:0;border:0;min-height:auto;line-height:100%;outline:none;text-decoration:none"" width=""600"" height=""120""></div>
                                            	

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                    	<tbody><tr>
                                        	<td colspan=""3"" valign=""top"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#505050;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
<h1 style=""text-align:center;color:#202020;display:block;font-family:Arial;font-size:20px;font-weight:bold;line-height:100%;margin-top:0;margin-right:0;margin-bottom:10px;margin-left:0"">
	<span style=""color:rgb(255,255,255);font-size:30px;line-height:30px"">#name#, can we help?<br /><br />
    <span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">It looks like you signed up to play UberStrike but never completed your training. Was it something we said?</span>
	</span></h1>
<table border=""0"" width=""100%"">
	<tbody>
		<tr>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""60%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
		<tr>
			<td bgcolor=""#000000"" height=""50"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td align=""center"" bgcolor=""#FFCC00"" style=""border-collapse:collapse"">
				<a href=""http://apps.facebook.com/uberstrike/"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">Play for Free</span></a></td>
			<td bgcolor=""#000000"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
	</tbody>
</table>
<br>
</div>
														</td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    	<tr>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse""><div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                          <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%""><span style=""font-size: 18px; color: rgb(255,255,255)"">500 FREE Points</span><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Play today and receive 500 Bonus Points to spend on in-game items like huge guns and massive armor!</span></font></span></h4>
<br>
<div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%""></div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                              <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
	<font color=""#ffffff"" size=""4""><span style=""line-height:100%"">Technical issues?</span></font><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Install the Unity3D webplayer manually <a href=""http://unity3d.com/webplayer/"" style=""color:#ffffff;text-decoration:underline"" target=""_blank"">here</a> or reply to this e-mail.</span></font></span><br>
</h4>
                                                              <div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%""></div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#000;border-top:0"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">

                                                
                                                <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td colspan=""2"" valign=""middle"" style=""border-collapse:collapse;background-color:#3b5998;border:0"">
                                                            <div style=""color:#ffffff; font-family:Arial; font-size:12px; line-height:125%; text-align:center; font-weight: bold;"">
                                                                <strong>&nbsp;<a href=""http://www.facebook.com/uberstrike?sk=wall"" style=""color:#ffffff;font-weight:bold;text-decoration:underline"" target=""_blank"">Like</a> us on Facebook&nbsp;
                                                            </strong></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign=""top"" width=""350"" style=""border-collapse:collapse"">
                                                            <div style=""color:#646464;font-family:Arial;font-size:12px;line-height:125%;text-align:left"">
																<br><br><br>
                                                            <em>Copyright © 2011 Cmune, All rights reserved.</em>
                                                            </div>
                                                        </td>
                                                        <td width=""190"" align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                                        <br>
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:12px;line-height:125%""><a href=""http://www.cmune.com/"" target=""new""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/cmunelogo.jpg"" width=""75"" height=""53"" alt=""cmune logo"" /></a></div><br>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        </tbody></table>
                        <br>
                    </td>
</td>
</body>
</html>";
        public const string UnsuccessfulFacebookRegistrationText = @"#name#, can we help?\n\n
It looks like you signed up to play UberStrike but never completed your training. Was it something we said?\n\n
Play for Free: http://apps.facebook.com/uberstrike/\n\n
500 FREE Points:\n
Play today and receive 500 Bonus Points to spend on in-game items like huge guns and massive armor!\n\n
Technical issues?:\n
Install the Unity3D webplayer manually here: http://unity3d.com/webplayer/ or reply to this e-mail.";
        public const string UnsuccessfulStandAloneRegistrationHtml = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body bgcolor=""#000000"">
<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                        
                        <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#0e0c11"">
                            <tbody><tr>
                                <td valign=""top"" style=""border-collapse:collapse"">

                                	
                                    <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left"">It looks like you had trouble setting up your UberStrike account. Was it something we said?</div>
                                            </td>
                                            
											<td valign=""top"" width=""190"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left""></div>
                                            </td>
											
                                        </tr>
                                    </tbody></table>
                                	

                                </td>
                            </tr>
                        </tbody></table>
                        
                    	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border:1px solid #000000;background-color:#000000"">
                       	  <tbody><tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#000000;border-bottom:0;padding:0px"">
                                        <tbody><tr>
                                            <td style=""border-collapse:collapse;color:#202020;font-family:Arial;font-size:34px;font-weight:bold;line-height:100%;padding:0;text-align:center;vertical-align:middle"">

                                            	
                                            	<div style=""text-align:left""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/header.jpg"" alt="""" border=""0"" style=""margin:0;padding:0;display:block;margin-top:0;margin-bottom:0;border:0;min-height:auto;line-height:100%;outline:none;text-decoration:none"" width=""600"" height=""120""></div>
                                            	

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                    	<tbody><tr>
                                        	<td colspan=""3"" valign=""top"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#505050;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
<h1 style=""text-align:center;color:#202020;display:block;font-family:Arial;font-size:20px;font-weight:bold;line-height:100%;margin-top:0;margin-right:0;margin-bottom:10px;margin-left:0"">
	<span style=""color:rgb(255,255,255);font-size:30px;line-height:30px"">Can we help?<br /><br />
    <span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">It looks like you signed up to play UberStrike but never completed your training. Was it something we said?<br />
    <br />
    Visit our online community to get tips &amp; tricks.</span>
	</span></h1>
<table border=""0"" width=""100%"">
	<tbody>
		<tr>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""60%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
		<tr>
			<td bgcolor=""#000000"" height=""50"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td align=""center"" bgcolor=""#FFCC00"" style=""border-collapse:collapse"">
				<a href=""http://forum.uberstrike.com/"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">Visit the forums</span></a></td>
			<td bgcolor=""#000000"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
	</tbody>
</table>
<br>
</div>
														</td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    	<tr>

                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                      
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#000;border-top:0"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">

                                                
                                                <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td colspan=""2"" valign=""middle"" style=""border-collapse:collapse;background-color:#3b5998;border:0"">
                                                            <div style=""color:#ffffff; font-family:Arial; font-size:12px; line-height:125%; text-align:center; font-weight: bold;"">
                                                                <strong>&nbsp;<a href=""http://www.facebook.com/uberstrike?sk=wall"" style=""color:#ffffff;font-weight:bold;text-decoration:underline"" target=""_blank"">Like</a> us on Facebook&nbsp;
                                                            </strong></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign=""top"" width=""350"" style=""border-collapse:collapse"">
                                                            <div style=""color:#646464;font-family:Arial;font-size:12px;line-height:125%;text-align:left"">
																<br><br><br>
                                                            <em>Copyright © 2011 Cmune, All rights reserved.</em>
                                                            </div>
                                                        </td>
                                                        <td width=""190"" align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                                        <br>
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:12px;line-height:125%""><a href=""http://www.cmune.com/"" target=""new""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/cmunelogo.jpg"" width=""75"" height=""53"" alt=""cmune logo"" /></a></div><br>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        </tbody></table>
                        <br>
                    </td>
</td>
</body>
</html>
";
        public const string UnsuccessfulStandAloneRegistrationText = @"Can we help?\n\n
It looks like you signed up to play UberStrike but never completed your training. Was it something we said?\n\n
Visit our online community to get tips & tricks: http://forum.uberstrike.com/\n";
        public const string UnsuccessfulPortalRegistrationHtml = @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body bgcolor=""#000000"">
<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                        
                        <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#0e0c11"">
                            <tbody><tr>
                                <td valign=""top"" style=""border-collapse:collapse"">

                                	
                                    <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left"">It looks like you had trouble setting up your UberStrike account. Was it something we said?</div>
                                            </td>
                                            
											<td valign=""top"" width=""190"" style=""border-collapse:collapse"">
                                            	<div style=""color:#505050;font-family:Arial;font-size:10px;line-height:100%;text-align:left""></div>
                                            </td>
											
                                        </tr>
                                    </tbody></table>
                                	

                                </td>
                            </tr>
                        </tbody></table>
                        
                    	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border:1px solid #000000;background-color:#000000"">
                       	  <tbody><tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#000000;border-bottom:0;padding:0px"">
                                        <tbody><tr>
                                            <td style=""border-collapse:collapse;color:#202020;font-family:Arial;font-size:34px;font-weight:bold;line-height:100%;padding:0;text-align:center;vertical-align:middle"">

                                            	
                                            	<div style=""text-align:left""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/header.jpg"" alt="""" border=""0"" style=""margin:0;padding:0;display:block;margin-top:0;margin-bottom:0;border:0;min-height:auto;line-height:100%;outline:none;text-decoration:none"" width=""600"" height=""120""></div>
                                            	

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                    	<tbody><tr>
                                        	<td colspan=""3"" valign=""top"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                            <div style=""color:#505050;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
<h1 style=""text-align:center;color:#202020;display:block;font-family:Arial;font-size:20px;font-weight:bold;line-height:100%;margin-top:0;margin-right:0;margin-bottom:10px;margin-left:0"">
	<span style=""color:rgb(255,255,255);font-size:30px;line-height:30px"">Can we help?<br /><br />
    <span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">It looks like you signed up to play UberStrike but never completed your training. Was it something we said?</span>
	</span></h1>
<table border=""0"" width=""100%"">
	<tbody>
		<tr>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""60%"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td bgcolor=""#000000"" width=""20%"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
		<tr>
			<td bgcolor=""#000000"" height=""50"" style=""border-collapse:collapse"">&nbsp;
				</td>
			<td align=""center"" bgcolor=""#FFCC00"" style=""border-collapse:collapse"">
				<a href=""http://uberstrike.cmune.com/"" style=""color:#ffffff;font-size:26px;font-weight:bold;text-decoration:none""><span style=""color:#ffffff"">Play for Free</span></a></td>
			<td bgcolor=""#000000"" style=""border-collapse:collapse"">&nbsp;
				</td>
		</tr>
	</tbody>
</table>
<br>
</div>
														</td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    	<tr>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse""><div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                          <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%""><span style=""font-size: 18px; color: rgb(255,255,255)"">500 FREE Points</span><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Play today and receive 500 Bonus Points to spend on in-game items like huge guns and massive armor!</span></font></span></h4>
<br>
<div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%""></div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        	<td valign=""top"" width=""280"" style=""border-collapse:collapse;background-color:#000000"">

                                                
                                                <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td valign=""top"" style=""border-collapse:collapse"">
                                                         
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:14px;line-height:150%;text-align:left"">
                                                              <h4 style=""display:block;font-family:Arial;margin-top:0px;margin-right:0px;margin-bottom:10px;margin-left:0px;text-align:left;color:#202020;font-size:22px;font-weight:bold;line-height:100%"">
	<font color=""#ffffff"" size=""4""><span style=""line-height:100%"">Technical issues?</span></font><br>
	<span style=""color:#a9a9a9""><font><span style=""font-size:14px;font-weight:normal;line-height:21px"">Install the Unity3D webplayer manually <a href=""http://unity3d.com/webplayer/"" style=""color:#ffffff;text-decoration:underline"" target=""_blank"">here</a> or reply to this e-mail.</span></font></span><br>
</h4>
                                                              <div style=""margin-top:0px;margin-right:0px;margin-bottom:0px;margin-left:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:0px;min-height:100%!important;width:100%!important;font-family:Arial;text-align:left;color:#ffffff;font-size:14px;line-height:150%""></div>
</div>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        	<tr>
                            	<td align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                    
                                	<table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""600"" style=""background-color:#000;border-top:0"">
                                    	<tbody><tr>
                                        	<td valign=""top"" style=""border-collapse:collapse"">

                                                
                                                <table border=""0"" cellpadding=""10"" cellspacing=""0"" width=""100%"">
                                                    <tbody><tr>
                                                        <td colspan=""2"" valign=""middle"" style=""border-collapse:collapse;background-color:#3b5998;border:0"">
                                                            <div style=""color:#ffffff; font-family:Arial; font-size:12px; line-height:125%; text-align:center; font-weight: bold;"">
                                                                <strong>&nbsp;<a href=""http://www.facebook.com/uberstrike?sk=wall"" style=""color:#ffffff;font-weight:bold;text-decoration:underline"" target=""_blank"">Like</a> us on Facebook&nbsp;
                                                            </strong></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign=""top"" width=""350"" style=""border-collapse:collapse"">
                                                            <div style=""color:#646464;font-family:Arial;font-size:12px;line-height:125%;text-align:left"">
																<br><br><br>
                                                            <em>Copyright © 2011 Cmune, All rights reserved.</em>
                                                            </div>
                                                        </td>
                                                        <td width=""190"" align=""center"" valign=""top"" style=""border-collapse:collapse"">
                                                        <br>
                                                            <div style=""color:#ffffff;font-family:Arial;font-size:12px;line-height:125%""><a href=""http://www.cmune.com/"" target=""new""><img src=""http://distro.client.cloud.cmune.com/UberStrike/EmailMarketing/Common/cmunelogo.jpg"" width=""75"" height=""53"" alt=""cmune logo"" /></a></div><br>
                                                        </td>
                                                    </tr>
                                                </tbody></table>
                                                

                                            </td>
                                        </tr>
                                    </tbody></table>
                                    
                                </td>
                            </tr>
                        </tbody></table>
                        <br>
                    </td>
</td>
</body>
</html>";
        public const string UnsuccessfulPortalRegistrationText = @"Can we help?\n\n
It looks like you signed up to play UberStrike but never completed your training. Was it something we said?\n\n
Play for Free: http://uberstrike.cmune.com/\n\n
500 FREE Points:\n
Play today and receive 500 Bonus Points to spend on in-game items like huge guns and massive armor!\n\n
Technical issues?\n
Install the Unity3D webplayer manually here: http://unity3d.com/webplayer/ or reply to this e-mail.";
    }
}