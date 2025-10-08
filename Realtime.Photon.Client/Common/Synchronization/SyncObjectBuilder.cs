
using System;
using System.Collections.Generic;
using System.Reflection;
using Cmune.Core.Types;
using Cmune.Realtime.Common.IO;
using Cmune.Util;

namespace Cmune.Realtime.Common.Synchronization
{
    public static class SyncObjectBuilder
    {
        private static Dictionary<Type, Dictionary<int, FieldInfo>> _indexFields = new Dictionary<Type, Dictionary<int, FieldInfo>>(5);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="syncData"></param>
        /// <param name="updateCache"></param>
        /// <param name="mask"></param>
        /// <param name="obj"></param>
        public static void ReadSyncData(SyncObject syncData, bool updateCache, int mask, CmuneDeltaSync obj)
        {
            var fields = GetFieldInfoLookup(obj.GetType());

            obj.InstanceId = syncData.Id;

            FieldInfo info;
            foreach (var s in syncData.Data)
            {
                if ((s.Key & mask) != 0 && fields.TryGetValue(s.Key, out info))
                {
                    try
                    {
                        //update field
                        info.SetValue(obj, s.Value);

                        //updated cache
                        if (updateCache)
                            obj.Cache[s.Key] = CmunePrint.GetHashCode(s.Value);
                    }
                    catch (Exception e)
                    {
                        throw CmuneDebug.Exception("Error in ReadSyncData at key {0} with {1}", s.Key, e.Message);
                    }
                }
            }

            if (!syncData.IsEmpty) obj.VersionID++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        public static List<SyncObject> GetSyncData<T>(IEnumerable<T> list, bool full) where T : CmuneDeltaSync
        {
            List<SyncObject> objects = new List<SyncObject>();

            foreach (var obj in list)
            {
                objects.Add(GetSyncData(obj, full));
            }

            return objects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        public static SyncObject GetSyncData(CmuneDeltaSync obj, bool full)
        {
            var fieldInfoLookup = GetFieldInfoLookup(obj.GetType());

            Dictionary<int, object> data = new Dictionary<int, object>(full ? 32 : 1);
            foreach (var o in fieldInfoLookup)
            {
                object currentValue = o.Value.GetValue(obj);
                int lastHash = 0, thisHash = CmunePrint.GetHashCode(currentValue);
                if (full || !obj.Cache.TryGetValue(o.Key, out lastHash) || lastHash != thisHash)
                {
                    //update field
                    data[o.Key] = currentValue;
                    //updated cache
                    obj.Cache[o.Key] = thisHash;
                }
            }

            return new SyncObject(obj.InstanceId, data);
        }

        private static Dictionary<int, FieldInfo> CreateFieldInfoLookup(Type type)
        {
            //reflect all fields by attribute CMUNESYNC and store the list of FieldInfo seperated by type
            //this leads to one single reflection per inherited type
            var fields = ReflectionHelper.GetFieldsWithAttribute<CMUNESYNC>(type, true);
            CmuneDebug.Assert((fields.Count) < 32, "CmuneDeltaSync has more than 31 synchronizable fields!");
            fields.Sort((p, q) => p.Attribute.TagId.CompareTo(q.Attribute.TagId));

            var lookupIndexFields = new Dictionary<int, FieldInfo>(32);
            var lookupIndexTags = new Dictionary<int, CMUNESYNC>(32);

            //FIELDS
            //foreach (MemberInfoField<CMUNESYNC> p in fields)
            for (int i = 0; i < fields.Count; i++)
            {
                MemberInfoField<CMUNESYNC> p = fields[i];
                if (RealtimeSerialization.IsTypeSupported(p.Field.FieldType))
                {
                    if (p.Attribute.IsTagged)
                    {
                        try
                        {
                            lookupIndexFields.Add(p.Attribute.TagId, p.Field);
                            lookupIndexTags.Add(p.Attribute.TagId, p.Attribute);
                            //CmuneDebug.LogFormat("Add Field {0} with ID {1}", p.Key.Name, p.Value.Tag);
                        }
                        catch
                        {
                            throw CmuneDebug.Exception("Sync table Exception at field ({0}) because ID {1} is already in use", p.Field.Name, p.Attribute.TagId);
                        }
                    }
                }
                else
                {
                    throw CmuneDebug.Exception("CmuneDeltaSync can't sync field ({0}) of type {1}", p.Field.Name, p.Field.FieldType);
                }
            }

            return lookupIndexFields;
        }

        private static Dictionary<int, FieldInfo> GetFieldInfoLookup(Type type)
        {
            Dictionary<int, FieldInfo> lookUp;
            if (!_indexFields.TryGetValue(type, out lookUp))
            {
                lookUp = CreateFieldInfoLookup(type);
                _indexFields[type] = lookUp;
            }
            return lookUp;
        }
    }
}