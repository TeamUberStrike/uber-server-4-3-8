
using System.Collections.Generic;
using System.Reflection;
using Cmune.Core.Types;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Utils;
using Cmune.Realtime.Photon.Server;
using Cmune.Util;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class PermanentRoomManager
    {
        protected PermanentRoomManager()
        {
            _myMethods = new Dictionary<int, MethodInfo>();
            _myType = this.GetType();

            InitMyInternalMethods();

            LobbyServerRoom.Instance.SubscribeToRoomMessages(OnLobbyMessage);
        }

        private static PermanentRoomManager _instance;
        public static PermanentRoomManager Instance
        {
            get
            {
                if (_instance == null) _instance = new PermanentRoomManager();
                return _instance;
            }
        }

        public void CreatePermanentRooms()
        {
            CreatePermanentGame("Veterans Battleground", 16 /*Space City*/, 10, GameModeID.TeamDeathMatch, 20, byte.MaxValue, 40);
            CreatePermanentGame("Pro Stadium", 4 /*Temple Of The Raven*/, 10, GameModeID.TeamDeathMatch, 30, byte.MaxValue, 40);
            CreatePermanentGame("Path Of Legends", 6 /*LevelGideonsTower*/, 10, GameModeID.TeamDeathMatch, 40, byte.MaxValue, 40);

            CreatePermanentGame("Basic Training Arena", 7 /*LevelSkyGarden*/, 10, GameModeID.TeamDeathMatch, 1, 5, 40);
            CreatePermanentGame("Intermediate Training Arena", 1 /*LevelMonkeyIsland*/, 10, GameModeID.TeamDeathMatch, 5, 10, 40);
            CreatePermanentGame("Advanced Training Arena", 13 /*The Warehouse 2*/, 10, GameModeID.TeamDeathMatch, 10, 20, 40);

            if (ServerSettings.BuildType != Cmune.DataCenter.Common.Entities.BuildType.Prod)
            {
                CreatePermanentGame("End of Match Game", 7 /*LevelSkyGarden*/, 0, GameModeID.TeamDeathMatch, 0, byte.MaxValue, 1);
            }
        }

        private void CreatePermanentGame(string name, int mapID, int minutes, short gameMode, int levelMin, int levelMax, int splatLimit)
        {
            GameMetaData data = new GameMetaData(mapID, string.Empty, minutes * 60, 16, gameMode);
            data.LevelMin = levelMin;
            data.LevelMax = levelMax;
            data.SplatLimit = splatLimit;
            data.RoomName = name;
            CmuneRoom r = CmuneRoomFactory.CreateCmuneGame(data, false, true);
            r.UpdateRoomInLobby();
        }

        [RoomMessage(RoomMessageType.GameUpdated)]
        private void OnGameUpdated(RoomMetaData room)
        {
            //only kick in if the room a room filled up
            if (room.IsFull && room is GameMetaData)
            {
                //check only level restricted game rooms, specifically only noob rooms (level 1-4)
                GameMetaData game = room as GameMetaData;
                if (game != null && game.HasLevelRestriction && game.LevelMax < 5)
                {
                    //iterate through all current rooms and find the ones with the same level restriction
                    bool createNewRoom = true;
                    foreach (CmuneRoom m in RoomCache.Instance.GetCurrentGames())
                    {
                        if (m != null && m.RoomData is GameMetaData)
                        {
                            //if we find at least 1 room with the same level restriction that is NOT full, we continue normally
                            GameMetaData g = m.RoomData as GameMetaData;
                            if (g != null && g.LevelMax == game.LevelMax && g.LevelMin == game.LevelMin && !g.IsFull)
                            {
                                createNewRoom = false;
                                break;
                            }
                        }
                    }

                    //if all rooms with the same noob level restriction are full, we create a new one (but this one will autodestroy if empty)
                    if (createNewRoom)
                    {
                        GameMetaData data = new GameMetaData(0, game.RoomName, string.Empty, game.MapID, game.Password, game.RoundTime, game.MaxPlayers, game.GameMode);
                        data.LevelMin = game.LevelMin;
                        data.LevelMax = game.LevelMax;
                        data.SplatLimit = game.SplatLimit;
                        CmuneRoom r = CmuneRoomFactory.CreateCmuneGame(data, true, true);
                        r.UpdateRoomInLobby();
                    }
                }
            }
        }

        #region MESSAGEING

        private void OnLobbyMessage(IMessage m)
        {
            CallInternalMethod(m.MessageID, m.Arguments);
        }

        private void InitMyInternalMethods()
        {
            List<MemberInfoMethod<RoomMessageAttribute>> info = AttributeFinder.GetMethods<RoomMessageAttribute>(_myType);
            foreach (MemberInfoMethod<RoomMessageAttribute> p in info)
            {
                _myMethods[p.Attribute.ID] = p.Method;
            }
        }

        private void CallInternalMethod(int internalID, params object[] args)
        {
            MethodInfo info;

            if (_myMethods.TryGetValue(internalID, out info))
            {
                try
                {
                    _myType.InvokeMember(info.Name, CmuneNetworkClass.Flags, null, this, args);
                }
                catch (System.Exception e)
                {
                    CmuneDebug.LogError("Exception when calling internal function {0}:{1}() by reflection: {2}", _myType.Name, info.Name, e.Message);

                    if (args != null)
                        CmuneDebug.LogError("Call with {0} Arguments: {1}", args.Length, CmunePrint.Types(args));
                    else
                        CmuneDebug.LogError("Call with NULL Argument");
                }
            }
        }

        private Dictionary<int, MethodInfo> _myMethods;

        private System.Type _myType;

        #endregion
    }
}
