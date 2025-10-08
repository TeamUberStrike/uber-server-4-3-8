
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Realtime.Photon.Server
{
    public static class ServerSettings
    {
        static ServerSettings()
        {
            AppVersion = string.Empty;
            BuildType = BuildType.Prod;

            IP = "0.0.0.0";
            Port = 0;
            InstanceName = string.Empty;

            SendNotifications = false;
            HasMailServer = false;
            MailReciever = "devteam@cmune.com";

            EnableLoadChecker = false;
            DisconnectFromLobbyAt = 0;
            ConnectToLobbtAt = 0;

            MaxPlayerCount = 250;
        }

        public static string AppVersion { get; set; }
        public static BuildType BuildType { get; set; }
        public static string ConnectionString { get { return IP + ":" + Port; } }
        public static string IP { get; set; }
        public static int Port { get; set; }
        public static string InstanceName { get; set; }
        public static string NickName { get; set; }
        public static string InstrumentationWsBaseUrl { get; set; }

        public static int MaxPlayerCount { get; set; }

        #region NOTIFICATION
        public static bool SendNotifications { get; set; }
        public static bool HasMailServer { get; set; }
        public static string MailReciever { get; set; }
        #endregion

        #region LOAD BALANCE
        public static bool EnableLoadChecker { get; set; }
        public static int DisconnectFromLobbyAt { get; set; }
        public static int ConnectToLobbtAt { get; set; }
        #endregion
    }
}
