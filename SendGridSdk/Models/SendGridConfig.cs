using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace SendGridSdk.Models
{
    internal static class SendGridConfig
    {
        internal const string ApiUrl = "https://sendgrid.com/api/{0}.{1}.{2}?api_user={3}&api_key={4}";

        internal const int SendGridPort = 587;
        
        internal const int MaxCategoriesCount = 10;
        internal const int MaxSmtpRecipientsCount = 1000;

        internal static SendGridConfigurationSection GetConfig()
        {
            return (SendGridConfigurationSection)System.Configuration.ConfigurationManager.GetSection("sendGridSettings");
        }
    }
}