using System;
using System.Collections.Generic;
using System.Reflection;
using Cmune.Util;
using Cmune.Core.Types;

namespace Cmune.Realtime.Common.Utils
{
    //public class MemberInfoMethod<T>
    //{
    //    public T Attribute;
    //    public MethodInfo Method;

    //    public MemberInfoMethod(MethodInfo method, T attribute)
    //    {
    //        Method = method;
    //        Attribute = attribute;
    //    }

    //    public string Name
    //    {
    //        get { return Method.Name; }
    //    }
    //}

    //public class MemberInfoField<T>
    //{
    //    public T Attribute;
    //    public FieldInfo Field;

    //    public MemberInfoField(FieldInfo field, T attribute)
    //    {
    //        Field = field;
    //        Attribute = attribute;
    //    }

    //    public string Name
    //    {
    //        get { return Field.Name; }
    //    }
    //}

    public static class AttributeFinder
    {
        public static void FindNetworkMethods(Type type, ref Dictionary<string, byte> _lookupNameIndex, ref Dictionary<byte, MethodInfo> _lookupIndexMethod)
        {
            FindNetworkMethods<NetworkMethodAttribute>(type, ref _lookupNameIndex, ref  _lookupIndexMethod);
        }

        /// <summary>
        /// Use reflection top create an index table of all network methods and their local IDs.
        /// Network methods are indicated by the attribute, defined by the generic basetype of this class.
        /// </summary>
        public static void FindNetworkMethods<T>(Type type, ref Dictionary<string, byte> _lookupNameIndex, ref Dictionary<byte, MethodInfo> _lookupIndexMethod) where T : IAttributeID<byte>
        {
            //CmuneDebug.Log(string.Format("------ {0}", type.Name));

            List<MemberInfoMethod<T>> methods = GetMethods<T>(type);
            byte genericIndex = 0;

            List<byte> reservedIDs = new List<byte>();
            foreach (MemberInfoMethod<T> it in methods)
            {
                if (it.Attribute.HasID)//(bool)typeof(T).GetProperty("HasID").GetValue(it.Attribute, null))
                {
                    byte id = it.Attribute.ID;//(byte)typeof(T).GetProperty("ID").GetValue(it.Attribute, null);
                    if (!reservedIDs.Contains(id))
                        reservedIDs.Add(id);
                    else
                        throw CmuneDebug.Exception("Reflection.FindNetworkMethods Detected a Collision of ID {0} in {1}", id, type.Name);
                }
            }

            //Check if the current attribute implements the static Attribute ID
            foreach (MemberInfoMethod<T> it in methods)
            {
                byte index = 0;

                if (it.Attribute.HasID)//(bool)typeof(T).GetProperty("HasID").GetValue(it.Attribute, null))
                {
                    //byte id = (byte)typeof(T).GetProperty("ID").GetValue(it.Attribute, null);
                    index = it.Attribute.ID;//id;
                }
                else
                {
                    //find the first index that is not used already
                    while (reservedIDs.Contains(genericIndex))
                        ++genericIndex;

                    //add it to the reserved ID list
                    reservedIDs.Add(genericIndex);
                    index = genericIndex;
                }

                try
                {
                    //CmuneDebug.Log(string.Format("-- Network function registered {0}/{1}", index, it.Key.Name));
                    _lookupIndexMethod.Add(index, it.Method);
                }
                catch// (Exception)
                {
                    throw CmuneDebug.Exception("Failed registering network function with name {0}! Static ID {1} already existing and reserved by function {2}!", it.Method.Name, index, _lookupIndexMethod[index].Name);
                }

                if (!_lookupNameIndex.ContainsKey(it.Method.Name))
                {
                    _lookupNameIndex.Add(it.Method.Name, index);
                }
                else
                {
                    if (CmuneDebug.IsWarningEnabled)
                        CmuneDebug.LogWarning(string.Format("Network function with name {0} can't be called by name because ambigous! Use ID {1} instead or Rename!", it.Method.Name, index));
                }
            }
        }

        /// <summary>
        /// Get the methods indicated by the attribute, defined by the generic basetype of this class.
        /// </summary>
        /// <returns></returns>
        public static List<MemberInfoMethod<T>> GetMethods<T>(Type type)
        {
            List<MemberInfoMethod<T>> list = new List<MemberInfoMethod<T>>();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo p in methods)
            {
                object[] atts = p.GetCustomAttributes(typeof(T), true);
                if (atts.Length > 0)
                {
                    list.Add(new MemberInfoMethod<T>(p, (T)atts[0]));
                }
            }

            try
            {
                list.Sort((p, q) => p.Name.CompareTo(q.Name));
            }
            catch (Exception e)
            {
                CmuneDebug.LogError("GetMethods - Exception in ReflectionHelper: {0}", e.Message);
                //CmuneDebug.LogWarning(CmunePrint.Values(list));
                list.Clear();
            }

            return list;
        }
    }

    //    /// <summary>
    //    /// Get the methods indicated by the attribute, defined by the generic basetype of this class.
    //    /// </summary>
    //    /// <returns></returns>
    //    public static List<KeyValuePair<PropertyInfo, T>> GetProperties<T>(Type type)
    //    {
    //        List<KeyValuePair<PropertyInfo, T>> list = new List<KeyValuePair<PropertyInfo, T>>();

    //        PropertyInfo[] methods = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //        foreach (PropertyInfo p in methods)
    //        {
    //            object[] atts = p.GetCustomAttributes(typeof(T), true);
    //            if (atts.Length > 0)
    //            {
    //                list.Add(new KeyValuePair<PropertyInfo, T>(p, (T)atts[0]));
    //            }
    //        }

    //        try
    //        {
    //            //CmuneDebug.Log(CmunePrint.Values(list));
    //            list.Sort((p, q) => p.Key.Name.CompareTo(q.Key.Name));
    //        }
    //        catch (Exception e)
    //        {
    //            CmuneDebug.LogErrorFormat("GetProperties - Exception in ReflectionHelper: {0}", e.Message);
    //            //CmuneDebug.LogWarning(CmunePrint.Values(list));
    //            list.Clear();
    //        }

    //        return list;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public static List<MemberInfoField<T>> GetFields<T>(Type type)
    //    {
    //        return GetFields<T>(type, true);
    //    }

    //    /// <summary>
    //    /// Get the methods indicated by the attribute, defined by the generic basetype of this class.
    //    /// This procedure makes sure to retrieve even private members of base classes that are not naturally
    //    /// inherited by the derived class.
    //    /// </summary>
    //    /// <returns></returns>
    //    public static List<MemberInfoField<T>> GetFields<T>(Type type, bool sortByName)
    //    {
    //        List<MemberInfoField<T>> list = new List<MemberInfoField<T>>();

    //        bool hasBaseClass = true;
    //        while (hasBaseClass)
    //        {
    //            //private fields and methods of a baseclass are never accessible by reflection in a derived class.
    //            //to get them anyway we iterate recursivle through the derivation hierarchy and
    //            //perform the GetFields for each class separately
    //            FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //            foreach (FieldInfo p in fields)
    //            {
    //                object[] atts = p.GetCustomAttributes(typeof(T), false);
    //                if (atts.Length > 0)
    //                {
    //                    list.Add(new MemberInfoField<T>(p, (T)atts[0]));
    //                }
    //            }
    //            type = type.BaseType;
    //            hasBaseClass = (type != typeof(object));
    //        }

    //        if (sortByName)
    //        {
    //            try
    //            {
    //                //CmuneDebug.Log(CmunePrint.Values(list));
    //                list.Sort((p, q) => p.Name.CompareTo(q.Name));
    //            }
    //            catch (Exception e)
    //            {
    //                CmuneDebug.LogErrorFormat("GetFields - Exception in ReflectionHelper: {0}", e.Message);
    //                //CmuneDebug.LogWarning(CmunePrint.Values(list));
    //                list.Clear();
    //            }
    //        }

    //        return list;
    //    }

    //    public static List<T> GetAllStaticFields<T>(Type type)
    //    {
    //        List<T> allCounters = new List<T>();

    //        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    //        FieldInfo[] fields = type.GetFields(flags);
    //        foreach (FieldInfo f in fields)
    //        {
    //            if (f.FieldType == typeof(T))
    //            {
    //                allCounters.Add((T)(object)f.GetValue(null));
    //            }
    //        }

    //        return allCounters;
    //    }
    //}
}
