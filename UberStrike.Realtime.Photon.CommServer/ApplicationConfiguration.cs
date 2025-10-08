using System;
using Cmune.DataCenter.Common.Entities;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using UberStrike.WebService.DotNet;

namespace UberStrike.Realtime.Photon.CommServer
{
    public static class ApplicationConfiguration
    {
        public static void ReadCmuneConfigFile()
        {
            string appVersion;
            if (ConfigurationReader.ReadAppSetting("ApplicationVersion", out appVersion))
            {
                ServerSettings.AppVersion = appVersion;
            }

            string name;
            if (ConfigurationReader.ReadAppSetting("ServerName", out name))
            {
                ServerSettings.NickName = name;
            }

            string instruWsUrl;
            if (ConfigurationReader.ReadAppSetting("InstrumentationBaseUrl", out instruWsUrl))
            {
                ServerSettings.InstrumentationWsBaseUrl = instruWsUrl;
            }

            string buildType;
            if (ConfigurationReader.ReadAppSetting("BuildType", out buildType))
            {
                BuildType type;
                if (Enum.TryParse(buildType, out type))
                    ServerSettings.BuildType = type;
            }

            string wsUrl;
            if (ConfigurationReader.ReadAppSetting("WebserviceBaseUrl", out wsUrl))
            {
                Configuration.WebserviceBaseUrl = wsUrl;
            }

            int level;
            if (ConfigurationReader.ReadAppSetting("DebugLevel", out level))
            {
                CmuneDebug.DebugLevel = level;
            }

            bool sendNotifications;
            if (ConfigurationReader.ReadAppSetting("SendNotifications", out sendNotifications))
            {
                ServerSettings.SendNotifications = sendNotifications;
            }

            string reciever;
            if (ConfigurationReader.ReadAppSetting("NotificationReciever", out reciever))
            {
                ServerSettings.MailReciever = reciever;
            }

            string smtpID, user, password;
            if (ConfigurationReader.ReadAppSetting("SmtpIP", out smtpID) && ConfigurationReader.ReadAppSetting("SmtpUser", out user) && ConfigurationReader.ReadAppSetting("SmtpPassword", out password))
            {
                ServerSettings.HasMailServer = true;
            }

            string pass;
            if (ConfigurationReader.ReadAppSetting("CmunePassPhrase", out pass))
            {
                Configuration.EncryptionPassPhrase = pass;
            }

            string init;
            if (ConfigurationReader.ReadAppSetting("CmuneInitVector", out init))
            {
                Configuration.EncryptionInitVector = init;
            }
        }
    }
}