
using System.Collections.Generic;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using ExitGames.Concurrency.Fibers;
using UberStrike.Core.Models.Views;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class GameDataManager
    {
        private readonly IFiber _sendFiber;

        public Dictionary<int, MapView> Maps { get; set; }
        public List<PlayerLevelCapView> PlayerLevelCaps { get; private set; }
        public Dictionary<int, PlayerXPEventView> PlayerXPEvents { get; private set; }
        public static readonly GameDataManager Instance = new GameDataManager();

        private GameDataManager()
        {
            _sendFiber = new PoolFiber();
            _sendFiber.Start();

            PlayerLevelCaps = new List<PlayerLevelCapView>();
            PlayerXPEvents = new Dictionary<int, PlayerXPEventView>();
            Maps = new Dictionary<int, MapView>();
        }

        public decimal GetXpMultiplier(int playerXPEventViewId)
        {
            PlayerXPEventView view;
            if (PlayerXPEvents.TryGetValue(playerXPEventViewId, out view) && view != null)
            {
                return view.XPMultiplier;
            }
            else
            {
                CmuneDebug.LogError("GetXpMultiplier can't find PlayerXPEventView for ID=" + playerXPEventViewId);
                return 0;
            }
        }

        public void CheckGameData(GameMetaData data)
        {
            MapView map;
            if (Maps.TryGetValue(data.MapID, out map) && map.Settings != null)
            {
                MapSettings settings;
                if (map.Settings.TryGetValue(0, out settings) && settings != null)
                {
                    data.RoundTime = CmuneMath.Clamp(data.RoundTime, settings.TimeMin, settings.TimeMax);
                    data.SplatLimit = CmuneMath.Clamp(data.RoundTime, settings.KillsMin, settings.KillsMax);
                    data.MaxPlayers = CmuneMath.Clamp(data.RoundTime, settings.PlayersMin, settings.PlayersMax);
                }
            }
        }

        public void StartUpdateGameEventData(int minutes)
        {
            long start = 10 * 1000;
            long interval = minutes * 60 * 1000;

            _sendFiber.ScheduleOnInterval(() =>
            {
                try
                {
                    OnXPEventsEvent(UberStrike.WebService.DotNet.UserWebServiceClient.GetXPEventsView());
                }
                catch (System.Exception ex)
                {
                    CmuneDebug.LogError(ex.Message);
                }
            }, start, interval);

            _sendFiber.ScheduleOnInterval(() =>
            {
                try
                {
                    OnLevelCapsViewEvent(UberStrike.WebService.DotNet.UserWebServiceClient.GetLevelCapsView());
                }
                catch (System.Exception ex)
                {
                    CmuneDebug.LogError(ex.Message);
                }
            }, start, interval);

            _sendFiber.ScheduleOnInterval(() =>
            {
                try
                {
                    OnMapsViewEvent(UberStrike.WebService.DotNet.ApplicationWebServiceClient.GetMaps(ServerSettings.AppVersion, Core.Types.LocaleType.en_US, Core.Types.MapType.StandardDefinition));
                }
                catch (System.Exception ex)
                {
                    CmuneDebug.LogError(ex.Message);
                }
            }, start, interval);
        }

        private void OnMapsViewEvent(List<MapView> list)
        {
            Maps = new Dictionary<int, MapView>();
            foreach (var m in list)
            {
                Maps.Add(m.MapId, m);
            }
        }

        private static void OnXPEventsEvent(Dictionary<int, PlayerXPEventView> ev)
        {
            foreach (var k in ev)
            {
                Instance.PlayerXPEvents[k.Key] = k.Value;
            }
        }

        private static void OnLevelCapsViewEvent(List<PlayerLevelCapView> ev)
        {
            Instance.PlayerLevelCaps.Clear();
            Instance.PlayerLevelCaps.AddRange(ev);
        }
    }
}