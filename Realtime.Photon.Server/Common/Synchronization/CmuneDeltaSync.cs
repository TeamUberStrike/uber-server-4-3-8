
using System.Collections.Generic;
using Cmune.Core.Types;
using Cmune.Realtime.Common;
using Cmune.Realtime.Common.Synchronization;

/// <summary>
/// 
/// </summary>
public abstract class CmuneDeltaSync
{
    /// <summary>
    /// 
    /// </summary>
    protected CmuneDeltaSync()
    {
        VersionID = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void ReadSyncData(SyncObject obj)
    {
        SyncObjectBuilder.ReadSyncData(obj, true, ~0, this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="syncMask"></param>
    public void ReadSyncData(SyncObject obj, int syncMask)
    {
        SyncObjectBuilder.ReadSyncData(obj, true, syncMask, this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="updateCache"></param>
    public void ReadSyncData(SyncObject obj, bool updateCache)
    {
        SyncObjectBuilder.ReadSyncData(obj, updateCache, ~0, this);
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="obj"></param>
    ///// <returns></returns>
    //public override bool Equals(object obj)
    //{
    //    if (!ReferenceEquals(obj, null))
    //    {
    //        CmuneDeltaSync other = obj as CmuneDeltaSync;
    //        if (other != null)
    //        {
    //            //check every field for equality
    //            foreach (KeyValuePair<int, FieldInfo> k in _lookupIndexFields)
    //            {
    //                object a = k.Value.GetValue(this);
    //                object b = k.Value.GetValue(other);
    //                if (!Comparison.IsEqual(a, b))
    //                    return false;
    //            }

    //            return true;
    //        }
    //        else return false;
    //    }
    //    else return false;
    //}

    public void IncrementVersion() { VersionID++; }

    /// <summary>
    /// 
    /// </summary>
    public uint VersionID { get; internal set; }
    public int InstanceId { get; internal set; }
    public readonly Dictionary<int, int> Cache = new Dictionary<int, int>();

    /// <summary>
    /// 
    /// </summary>
    public class FieldTag : ExtendableEnum<int> { }
}