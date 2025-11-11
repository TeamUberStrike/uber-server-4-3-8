using System;
using System.IO;
using UberStrike.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.ServiceModel.Web;
using UberStrike.Core.Types;

namespace Cmune.Instrumentation.WebServices
{
    public partial class WebServicesClass : IAllWebServicesInterfaces
    {
        /// <summary>
        /// Track the user on a specified location
        /// </summary>
        /// <returns></returns>
        public void SetInstallTracking(string stepId, string channel, string referrerPartnerId, string isJavaInstall, string operatingSystem, string tracking, string browsername, string browserversion)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string userHostAddress = endpointProperty.Address;

            ChannelType channelType;
            bool isChannelEnum = EnumUtilities.TryParseEnumByValue(channel, out channelType);
            UserInstallStepType stepType;
            bool isInstallStepEnum = EnumUtilities.TryParseEnumByValue(stepId, out stepType);
            ReferrerPartnerType referrerType;
            bool isReferrerPartnerEnum = EnumUtilities.TryParseEnumByValue(referrerPartnerId, out referrerType);
            bool isJavaInstallBool = false;
            bool isJavaInstallBoolean = false;

            if (String.Equals("1", isJavaInstall))
            {
                isJavaInstallBool = true;
                isJavaInstallBoolean = true;
            }
            else if (String.Equals("0", isJavaInstall))
            {
                isJavaInstallBool = false;
                isJavaInstallBoolean = true;
            }

            if (isChannelEnum && isInstallStepEnum && isReferrerPartnerEnum && isJavaInstallBoolean)
            {
                string userAgent = WebOperationContext.Current.IncomingRequest.Headers["User-Agent"];

                Tracking.UserInstallTracking(tracking, channelType, stepType, referrerType, isJavaInstallBool, operatingSystem, browsername, browserversion, userHostAddress, userAgent);
            }
        }

        /// <summary>
        /// Reports an issue
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void ReportIssue(string type, string data)
        {
            IssueType issueType;
            bool isIssueTypeParsed = EnumUtilities.TryParseEnumByValue(type, out issueType);

            if (isIssueTypeParsed)
            {
                OperationContext context = OperationContext.Current;
                MessageProperties messageProperties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string userHostAddress = endpointProperty.Address;

                DateTime now = DateTime.Now;

                StringBuilder htmlBody = new StringBuilder();
                htmlBody.Append("<p><b>Event date:</b> ");
                htmlBody.Append(now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                htmlBody.Append("<br /><b>IP</b>: ");
                htmlBody.Append(userHostAddress);
                htmlBody.Append("</p><p><b>Additional data:</b></p><p>");
                htmlBody.Append(HttpUtility.HtmlEncode(data));
                htmlBody.Append("</p>");

                StringBuilder textBody = new StringBuilder();
                textBody.Append("Event date: ");
                textBody.Append(now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                textBody.Append("\nIP: ");
                textBody.Append(userHostAddress);
                textBody.Append("\nAdditional data:\n");
                textBody.Append(HttpUtility.HtmlEncode(data));

                bool isEmailSent = CmuneMail.SendEmail(CommonConfig.CmuneDevteamEmail, CommonConfig.CmuneDevteamEmailName, "bugreport@cmune.com", "UberStrike automated bug report", ((int)issueType).ToString(), htmlBody.ToString(), textBody.ToString());
            }
        }
    }
}