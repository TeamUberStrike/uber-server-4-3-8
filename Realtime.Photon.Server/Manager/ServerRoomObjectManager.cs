
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Cmune.Realtime.Common;
using System;
using Cmune.Util;

namespace Cmune.Realtime.Photon.Server
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerRoomObjectManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public ServerRoomObjectManager(CmuneRoom room)
        {
            _objects = new Dictionary<int, RoomObject>();

            _room = room;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (RoomObject o in _objects.Values)
                b.AppendLine(o.ToString());
            return b.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRoom()
        {
            if (CmuneDebug.IsDebugEnabled)
                CmuneDebug.Log("ClearRoom");

            _objects.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public ICollection<RoomObject> GetPlayerDependentObjects(int playerID)
        {
            List<RoomObject> all = new List<RoomObject>(_objects.Count);
            foreach (RoomObject o in _objects.Values)
            {
                if (o.Type == AssetType.AVATAR && o.CreatorID == playerID)
                    all.Add(o);
            }

            return all;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="networkID"></param>
        /// <returns></returns>
        public bool IsOwnerOf(int playerID, int networkID)
        {
            RoomObject obj;
            if (_objects.TryGetValue(networkID, out obj))
            {
                return obj.Type == AssetType.AVATAR && obj.CreatorID == playerID;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<RoomObject> GetAllObjects()
        {
            List<RoomObject> list = new List<RoomObject>(_objects.Values);
            list.Sort();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValidRoom
        {
            get { return _debugSpaceID > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="assetID"></param>
        /// <param name="netID"></param>
        /// <param name="type"></param>
        /// <param name="transform"></param>
        /// <param name="configfile"></param>
        /// <returns></returns>
        public bool AddObject(int player, int assetID, short netID, AssetType type, CmuneTransform transform, List<byte> configfile)
        {
            bool isObjectAdded = false;

            if (_roomInitiator < 0)
            {
                _roomInitiator = player;
            }

            if (type == AssetType.AVATAR)
            {
                if (!_objects.ContainsKey(netID))
                {
                    _objects.Add(netID, new RoomObject(player, assetID, netID, type, transform, configfile));

                    isObjectAdded = true;
                }
                else
                {
                    //ignore duplicate entries
                }
            }
            else
            {
                //only the room initiator can add new objects
                if (_roomInitiator == player)
                {
                    if (type == AssetType.SPACE)
                    {
                        _debugSpaceID = assetID;

                        _debugSpaceCreationDate = DateTime.Now;

                        _room.UpdateSpaceAssetID(assetID);
                    }

                    if (!_objects.ContainsKey(netID))
                    {
                        _objects.Add(netID, new RoomObject(player, assetID, netID, type, transform, configfile));

                        isObjectAdded = true;
                    }
                }
                else
                {
                    CmuneDebug.LogError("The player '{0}' wants to add the object '{1}/{2}' but is not creator of the room! Current level {3} is created by {4} and has {5} objects. (Creation date: {6} - Now: {7})", player, assetID, type, _debugSpaceID, _roomInitiator, _objects.Count, _debugSpaceCreationDate, DateTime.Now);
                }
            }

            return isObjectAdded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="assetID"></param>
        /// <param name="netID"></param>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="sca"></param>
        /// <param name="config"></param>
        public void AddObject(int player, int assetID, short netID, AssetType type, Vector3 pos, Quaternion rot, Vector3 sca, List<byte> config)
        {
            if (!_objects.ContainsKey(netID))
            {
                _objects.Add(netID, new RoomObject(player, assetID, netID, type, pos, rot, sca, config));
            }
            else
            {
                //CmuneDebug.LogError("addObject: object with same netID already inserted: " + netID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netID"></param>
        public void RemoveObject(short netID)
        {
            if (_objects.ContainsKey(netID))
            {
                if (CmuneDebug.IsDebugEnabled)
                    CmuneDebug.Log("RemoveObject: " + netID);

                _objects.Remove(netID);
            }
            else
            {
                //CmuneDebug.LogError("removeObject: no object found");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pack"></param>
        public void UpdateObject(NetworkPackage pack)
        {
            if (_objects.ContainsKey(pack.netID))
            {
                RoomObject nos = _objects[pack.netID] as RoomObject;
                nos.State = pack;
            }
            else
            {
                //CmuneDebug.LogError("updateObject: no object found");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netID"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public RoomObject SynchronizeObjectFields(int netID, List<byte> bytes)
        {
            if (_objects.ContainsKey(netID))
            {
                RoomObject nos = _objects[netID] as RoomObject;
                nos.FromBytes(bytes.ToArray(), 0);

                return nos;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="netID"></param>
        /// <returns></returns>
        public byte[] GetObjectFields(int netID)
        {
            if (_objects.ContainsKey(netID))
            {
                RoomObject nos = _objects[netID] as RoomObject;
                return nos.GetBytes();
            }
            else
            {
                return new byte[] { };
            }
        }

        #region Fields

        private int _debugSpaceID = -1;
        private DateTime _debugSpaceCreationDate = DateTime.Now;

        private Dictionary<int, RoomObject> _objects;
        private CmuneRoom _room;
        private int _roomInitiator = -1;

        #endregion
    }
}