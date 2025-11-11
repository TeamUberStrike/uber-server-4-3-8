using System.Collections.Generic;

namespace Cmune.Realtime.Common.Utils
{
    public class CmunePairList<T1, T2> : List<KeyValuePair<T1, T2>>
    {
        public CmunePairList() : base() { }
        public CmunePairList(int capacity) : base(capacity) { }
        public CmunePairList(IEnumerable<KeyValuePair<T1, T2>> collection) : base(collection) { }
        public CmunePairList(IEnumerable<T1> collection1, IEnumerable<T2> collection2)
        {
            IEnumerator<T1> it1 = collection1.GetEnumerator();
            IEnumerator<T2> it2 = collection2.GetEnumerator();
            while (it1.MoveNext() && it2.MoveNext())
            {
                this.Add(new KeyValuePair<T1, T2>(it1.Current, it2.Current));
            }
        }

        public ICollection<KeyValuePair<T1, T2>> GetPairsWithKey(T1 key)
        {
            return this.FindAll(p => p.Key.Equals(key));
        }

        public ICollection<KeyValuePair<T1, T2>> GetPairsWithValue(T2 value)
        {
            return this.FindAll(p => p.Value.Equals(value));
        }

        public ICollection<T1> Keys
        {
            get
            {
                List<T1> l = new List<T1>(Count);
                ForEach(p => l.Add(p.Key));
                return l;
            }
        }

        public ICollection<T2> Values
        {
            get
            {
                List<T2> l = new List<T2>(Count);
                ForEach(p => l.Add(p.Value));
                return l;
            }
        }

        public void Add(T1 first, T2 second)
        {
            this.Add(new KeyValuePair<T1, T2>(first, second));
        }

        public void Clamp(int max)
        {
            if (Count > max)
                this.RemoveRange(max, Count - max);
        }
    }
}
