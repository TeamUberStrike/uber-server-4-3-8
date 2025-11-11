using System.Collections.Generic;
using System.Reflection;

namespace Cmune.Core.Types
{
    public abstract class ExtendableEnum<T>
    {
        public ExtendableEnum()
        {
            _allMembers = new List<T>();
            _allDeclaredMembers = new List<T>();

            PopulateList(_allMembers, this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy));
            PopulateList(_allDeclaredMembers, this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly));
        }

        public IEnumerable<T> AllValues
        {
            get { return _allMembers; }
        }

        public IEnumerable<T> AllDeclaredValues
        {
            get { return _allDeclaredMembers; }
        }

        public bool IsDefined(T b)
        {
            return _allMembers.Contains(b);
        }

        private void PopulateList(List<T> list, FieldInfo[] fields)
        {
            foreach (FieldInfo f in fields)
            {
                if (f.FieldType == typeof(T))
                {
                    T val = (T)f.GetValue(this);
                    list.Add(val);
                }
            }
        }

        private List<T> _allMembers;
        private List<T> _allDeclaredMembers;
    }
}
