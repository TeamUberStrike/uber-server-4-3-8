
using Cmune.DataCenter.Utils;
using System;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server.Diagnostics
{
    public static class MailNotification
    {
        public static void SendMessage(string subject, string body)
        {
            if (ServerSettings.SendNotifications && ServerSettings.HasMailServer)
            {
                CmuneMail.SendEmail("Photon@cmune.com",
                    ServerSettings.ConnectionString,
                    ServerSettings.MailReciever,
                    "Cmune", subject, body, body);
            }
        }

        public static void SendSpeedhackReport(int reportedCmid, string abuserName, string info, string context)
        {
            if (ServerSettings.HasMailServer)
            {
                try
                {
                    bool success = CmuneMail.SendEmail("photon@cmune.com",
                         "Automatic Reporter",
                         "abuse@cmune.com",
                         "Speedhack Detection",
                         "Speedhack Detection - " + reportedCmid.ToString(),
                         string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, info, context),
                         string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, info, context));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("SendPlayerReport failed with: {0}|{1}|{2}\n{3}", reportedCmid, abuserName, info, e.Message);
                }
            }
        }

        public static void SendReport(int reporterCmid, string reporterName, string abuserName, string subject, string context)
        {
            if (ServerSettings.HasMailServer)
            {
                try
                {
                    CmuneMail.SendEmail("Photon@cmune.com",
                        reporterName,
                        "Abuse@cmune.com",
                        "Cmune",
                        subject,
                        string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, subject, context),
                        string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, subject, context));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("SendPlayerReport failed with: {0}|{1}|{2}\n{3}", reporterCmid, abuserName, subject, e.Message);
                }
            }
        }

        public static void SendPlayerReport(string reporterName, string subject, string type, string reason, string context)
        {
            if (ServerSettings.HasMailServer)
            {
                try
                {
                    CmuneMail.SendEmail("photon@cmune.com",
                        reporterName,
                        "abuse@cmune.com",
                        "Abuse Detection",
                        subject,
                        string.Format("Type: {0}\nReason: {1}\n\n--------------\n\n{2}", type, reason, context),
                        string.Format("Type: {0}\nReason: {1}\n\n--------------\n\n{2}", type, reason, context));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("SendPlayerReport failed with: {0}|{1}|{2}|{3}\n{4}", reporterName, subject, type, reason, e.Message);
                }
            }
        }

        public static void SendCheatReport(int reporterCmid, string reporterName, string comment, string context)
        {
            if (ServerSettings.HasMailServer)
            {
                try
                {
                    CmuneMail.SendEmail("Photon@cmune.com",
                        reporterName,
                        "Cheatreport@cmune.com",
                        "Cmune",
                        "# Cheat Detection #",
                        string.Format("{0} ({1})\nComment: {2}\n--------------\n\n{3}", reporterName, reporterCmid, comment, context),
                        string.Format("{0} ({1})\nComment: {2}\n--------------\n\n{3}", reporterName, reporterCmid, comment, context));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("SendCheatReport failed with: {0}|{1}|{2}\n{3}", reporterCmid, comment, context, e.Message);
                }
            }
        }

        public static void LogModeration(int reporterCmid, string reporterName, string abuserName, string type, string context)
        {
            if (ServerSettings.HasMailServer)
            {
                try
                {
                    CmuneMail.SendEmail("Photon@cmune.com",
                        reporterName,
                        "support@cmune.com",
                        "Cmune",
                        type,
                        string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, type, context),
                        string.Format("Player: {0}\nType: {1}\n\n{2}", abuserName, type, context));
                }
                catch (Exception e)
                {
                    CmuneDebug.LogError("SendPlayerReport failed with: {0}|{1}|{2}\n{3}", reporterCmid, abuserName, type, e.Message);
                }
            }
        }
    }
}
