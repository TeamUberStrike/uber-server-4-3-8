//using System.Collections.Generic;
//using Cmune.DataCenter.Common.Entities;
//using Cmune.Realtime.Common;
//using Cmune.Util;

//namespace Cmune.Realtime.Photon.Client
//{
//    public static class CmuneNetworkManager
//    {
//        public static string PhotonApplication = "Uberstrike";

//        public static bool UseLocalCommServer = true;

//        public static int RoomCreationMethod = StaticRoomID.Auto;

//        public static GameServerInfo CurrentGameServer = GameServerInfo.Empty;
//        public static GameServerInfo CurrentLobbyServer = GameServerInfo.Empty;
//        public static GameServerInfo CurrentCommServer = GameServerInfo.Empty;

//        public static List<GameServerInfo> PhotonServerList = new List<GameServerInfo>();

//        public static int GetServerLatency(string connection)
//        {
//            foreach (GameServerInfo info in CmuneNetworkManager.PhotonServerList)
//            {
//                if (info.ConnectionString == connection)
//                    return info.Latency;
//            }

//            return 0;
//        }

//        //public static void UpdateCommServer()
//        //{
//        //    if (UpdateCommServerDynamically)
//        //    {
//        //        GameServerInfo commServer = PhotonServerList.Find(s => s.UsageType == PhotonUsageType.CommServer);
//        //        if (commServer != null)
//        //        {
//        //            CurrentCommServer = commServer;
//        //        }
//        //        else
//        //        {
//        //            CmuneDebug.LogWarning("No CommServer found");
//        //        }
//        //    }
//        //}

//        //public static void UpdateGameServer()
//        //{
//        //    if (UpdateGameServerDynamically)
//        //    {
//        //        List<GameServerInfo> gameServer = new List<GameServerInfo>();
//        //        gameServer = PhotonServerList.FindAll(s => s.IsValid && (s.UsageType == PhotonUsageType.All));

//        //        if (gameServer.Count > 0)
//        //        {
//        //            gameServer.Sort(delegate(GameServerInfo a, GameServerInfo b) { return a.ServerLoad.CompareTo(b.ServerLoad); });

//        //            CurrentGameServer = gameServer[0];
//        //        }
//        //    }
//        //    else
//        //    {
//        //        CmuneDebug.LogWarning("No GameServer found");
//        //    }
//        //}
//    }
//}