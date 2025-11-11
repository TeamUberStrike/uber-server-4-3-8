using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using SendGridSdk.Models;

namespace SendGridSdk
{
    public static class SendGridMail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="fromEmail"></param>
        /// <param name="fromName"></param>
        /// <param name="subject">Variables should have been replaced already</param>
        /// <param name="htmlBody">Variables should have been replaced already</param>
        /// <param name="textBody">Variables should have been replaced already</param>
        public static void SendSingleEmail(string toEmail, string toName, string fromEmail, string fromName, string subject, string htmlBody, string textBody)
        {
            using (MailMessage email = new MailMessage())
            {
                email.To.Add(new MailAddress(toEmail, toName));

                email.From = new MailAddress(fromEmail, fromName);
                email.Subject = subject;

                using (AlternateView plainTextView = AlternateView.CreateAlternateViewFromString(textBody, null, MediaTypeNames.Text.Plain))
                using (AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html))
                {
                    email.AlternateViews.Add(plainTextView);

                    htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                    email.AlternateViews.Add(htmlView);

                    SendGridConfigurationSection config = SendGridConfig.GetConfig();

                    using (SmtpClient smtpClient = new SmtpClient(config.SmtpServer, SendGridConfig.SendGridPort))
                    {
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(config.ApiUser, config.ApiKey);
                        smtpClient.Credentials = credentials;

                        smtpClient.Send(email);
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fromEmail"></param>
        /// <param name="fromName"></param>
        /// <param name="subject"></param>
        /// <param name="htmlBody"></param>
        /// <param name="textBody"></param>
        /// <param name="recipients">email addresses</param>
        /// <param name="variablesToReplace">if htmlBody / textBody contains variables to replace, the key should be present and the List count should be the same than recipients</param>
        /// <param name="categories"></param>
        public static void SendMultipleEmails(string fromEmail, string fromName, string subject, string htmlBody, string textBody, IList<string> recipients, Dictionary<string, List<string>> variablesToReplace, IList<string> categories)
        {
            if (variablesToReplace == null)
            {
                throw new ArgumentNullException("variablesToReplace");
            }

            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }

            using (MailMessage email = new MailMessage())
            {
                email.To.Add("noreply@cmune.com");

                email.From = new MailAddress(fromEmail, fromName);
                email.Subject = subject;

                using (AlternateView plainTextView = AlternateView.CreateAlternateViewFromString(textBody, null, MediaTypeNames.Text.Plain))
                using (AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html))
                {
                    email.AlternateViews.Add(plainTextView);

                    htmlView.TransferEncoding = TransferEncoding.QuotedPrintable;
                    email.AlternateViews.Add(htmlView);

                    int skip = 0;

                    do
                    {
                        SmtpApiHeader header = new SmtpApiHeader();
                        header.AddRecipients(recipients.Skip(skip).Take(SendGridConfig.MaxSmtpRecipientsCount).ToList());

                        foreach (string tag in variablesToReplace.Keys)
                        {
                            header.AddSubstitutionValue(tag, variablesToReplace[tag].Skip(skip).Take(SendGridConfig.MaxSmtpRecipientsCount).ToList());
                        }

                        header.SetCategories(categories);

                        email.Headers.Add("X-SMTPAPI", header.ToJson());

                        SendGridConfigurationSection config = SendGridConfig.GetConfig();

                        using (SmtpClient smtpClient = new SmtpClient(config.SmtpServer, SendGridConfig.SendGridPort))
                        {
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(config.ApiUser, config.ApiKey);
                            smtpClient.Credentials = credentials;

                            smtpClient.Send(email);
                        }

                        skip += SendGridConfig.MaxSmtpRecipientsCount;

                    } while (skip <= recipients.Count);
                }
            }
        }
    }
}