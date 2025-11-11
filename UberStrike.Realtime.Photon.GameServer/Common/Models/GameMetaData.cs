using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.IO;
using Cmune.Realtime.Common.Utils;
using Cmune.Util;

namespace UberStrike.Realtime.Common
{
    /// <summary>
    /// Bytes: 16 + 
    /// </summary>
    public class GameMetaData : RoomMetaData, IGameMetaData
    {
        public static new GameMetaData Empty
        {
            get { return new GameMetaData(); }
        }

        protected GameMetaData()
            : base()
        { }

        public GameMetaData(byte[] t, ref int idx)
        {
            try
            {
                idx = FromBytes(t, idx);
            }
            catch
            {
                CmuneDebug.LogError("EXCEPTION: Error Deserializing GameMetaData at {0}:\n{1}", idx, CmunePrint.Values(t));
            }
        }

        public GameMetaData(int roomNumber, string roomName, string server)
            : base(roomNumber, roomName, server)
        { }

        public GameMetaData(int roomNumber, string roomName, string server,
            int mapID, string password, int maxtime, int maxplayers, short mode)
            : this(roomNumber, roomName, server)
        {
            this._mapID = mapID;
            this._password = password;
            this.RoundTime = maxtime;
            this._maxPlayers = maxplayers;
            this._gameMode = mode;
        }

        public GameMetaData(int mapID, string password, int maxtime, int maxplayers, short mode)
        {
            this._mapID = mapID;
            this._password = password;
            this.RoundTime = maxtime;
            this._maxPlayers = maxplayers;
            this._gameMode = mode;
        }

        public bool CanEnterRoom
        {
            get { return RoomID.CanConnectToServer && RoomID.IsVersionCompatible && !IsFull; }
        }

        public virtual int RoundTime
        {
            get { return _timeLimitSeconds; }
            set
            {
                _timeLimitSeconds = value;

                if (IsTimeUnlimited)
                {
                    _rountimeString = "Unlimited";
                }
                else if (_timeLimitSeconds < 120)
                {
                    _rountimeString = string.Format("{0} Seconds", _timeLimitSeconds);
                }
                else
                {
                    _rountimeString = string.Format("{0} Minutes", _timeLimitSeconds / 60);
                }
            }
        }

        public string RoundTimeString
        {
            get
            {
                return _rountimeString;
            }
        }

        public string ConnectedPlayersString
        {
            get
            {
                return _connectedPlayersString;
            }
        }

        public override int ConnectedPlayers
        {
            get
            {
                return base.ConnectedPlayers;
            }
            set
            {
                base.ConnectedPlayers = value;

                _connectedPlayersString = string.Format("{0} / {1}", ConnectedPlayers, MaxPlayers);
            }
        }

        public int InGamePlayers
        {
            get { return _playersInGame; }
            set
            {
                _playersInGame = value;
                _playersInGameString = string.Format("{0} / {1}", value, MaxPlayers);
            }
        }

        public string InGamePlayersString
        {
            get { return _playersInGameString; }
        }

        public bool IsTimeUnlimited
        {
            get { return _timeLimitSeconds <= 0; }
        }

        public virtual int MapID
        {
            get { return _mapID; }
            set { _mapID = value; }
        }

        public virtual short GameMode
        {
            get { return _gameMode; }
            set { _gameMode = value; }
        }

        public int GameModifierFlags
        {
            get { return _gameFlags; }
            set { _gameFlags = value; }
        }

        public bool HasGameFlag(GameFlags.GAME_FLAGS flag)
        {
            return GameFlags.IsFlagSet(flag, _gameFlags);
        }

        public int SplatLimit
        {
            get { return _splatLimit; }
            set { _splatLimit = value; }
        }

        public int Latency
        {
            get { return _latency; }
            set { _latency = value; }
        }

        public int LevelMin
        {
            get { return _levelMin; }
            set { _levelMin = (byte)CmuneMath.Clamp(value, 0, byte.MaxValue); }
        }

        public int LevelMax
        {
            get { return _levelMax; }
            set { _levelMax = (byte)CmuneMath.Clamp(value, 0, byte.MaxValue); }
        }

        public bool HasLevelRestriction
        {
            get { return _levelMin != 0 || _levelMax != byte.MaxValue; }
        }

        public bool IsLevelAllowed(int level)
        {
            return level >= _levelMin && level <= _levelMax;
        }

        public override string ToString()
        {
            System.Text.StringBuilder b = new System.Text.StringBuilder(base.ToString());

            b.AppendFormat("Map: {0}\n", _mapID);
            b.AppendFormat("Time: {0}\n", _timeLimitSeconds);
            b.AppendFormat("Mode: {0}\n", _gameMode);

            return b.ToString();
        }

        #region De/Serialization

        public override byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(base.GetBytes());

            DefaultByteConverter.FromShort((short)this._mapID, ref bytes);
            DefaultByteConverter.FromInt(this._timeLimitSeconds, ref bytes);
            DefaultByteConverter.FromInt(this._splatLimit, ref bytes);
            DefaultByteConverter.FromShort(this._gameMode, ref bytes);
            DefaultByteConverter.FromInt(this._gameFlags, ref bytes);

            DefaultByteConverter.FromByte(this._levelMin, ref bytes);
            DefaultByteConverter.FromByte(this._levelMax, ref bytes);
            //DefaultByteConverter.FromByte(this._isBlueLevel, ref bytes);

            DefaultByteConverter.FromInt(_playersInGame, ref bytes);

            return bytes.ToArray();
        }

        public override int FromBytes(byte[] bytes, int idx)
        {
            idx = base.FromBytes(bytes, idx);

            this._mapID = (int)DefaultByteConverter.ToShort(bytes, ref idx);
            this._timeLimitSeconds = DefaultByteConverter.ToInt(bytes, ref idx);
            this._splatLimit = DefaultByteConverter.ToInt(bytes, ref idx);
            this._gameMode = DefaultByteConverter.ToShort(bytes, ref idx);
            this._gameFlags = DefaultByteConverter.ToInt(bytes, ref idx);

            this._levelMin = DefaultByteConverter.ToByte(bytes, ref idx);
            this._levelMax = DefaultByteConverter.ToByte(bytes, ref idx);
            //this._isBlueLevel = DefaultByteConverter.ToByte(bytes, ref idx);

            _playersInGame = DefaultByteConverter.ToInt(bytes, ref idx);
            _connectedPlayersString = string.Format("{0} / {1}", _playersInGame, MaxPlayers);

            RoundTime = _timeLimitSeconds;
            //ConnectedPlayers = _inPlayers;
            _connectedPlayersString = string.Format("{0} / {1}", ConnectedPlayers, MaxPlayers);

            return idx;
        }

        #endregion

        #region FIELDS
        protected int _latency = 0;
        protected int _gameFlags = 0;
        protected int _mapID = 0;
        protected int _timeLimitSeconds = 0;
        protected int _splatLimit = 0;

        protected byte _levelMin = 0;
        protected byte _levelMax = byte.MaxValue;

        protected short _gameMode = GameModeID.DeathMatch;

        private string _rountimeString = string.Empty;
        private string _connectedPlayersString = string.Empty;

        // players who are not spectators
        private int _playersInGame = 0;
        private string _playersInGameString = string.Empty;
        #endregion
    }
}