namespace SendGridSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;

    internal class SendGridConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("apiUser", IsRequired = true)]
        public string ApiUser
        {
            get
            {
                return (string)this["apiUser"];
            }

            set
            {
                this["apiUser"] = value;
            }
        }

        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string ApiKey
        {
            get
            {
                return (string)this["apiKey"];
            }

            set
            {
                this["apiKey"] = value;
            }
        }

        [ConfigurationProperty("smtpServer", IsRequired = true)]
        public string SmtpServer
        {
            get
            {
                return (string)this["smtpServer"];
            }

            set
            {
                this["smtpServer"] = value;
            }
        }
    }
}
