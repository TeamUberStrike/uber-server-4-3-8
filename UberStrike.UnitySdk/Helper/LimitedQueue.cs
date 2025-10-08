using System.Collections.Generic;

namespace UberStrike.Helper
{
    public class LimitedQueue<T> : IEnumerable<T>
    {
        public T LastItem { get; private set; }

        private List<T> _list;
        private int _capacity;

        public LimitedQueue(int capacity)
        {
            _capacity = capacity;
            _list = new List<T>(capacity);
        }

        /// <summary>
        /// Sets or Gets the element at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public bool EnqueueUnique(T item)
        {
            int existed = _list.RemoveAll(p => p.Equals(item));
            Enqueue(item);
            return existed == 0;
        }

        public void Enqueue(T item)
        {
            if (_list.Count + 1 > _capacity) LastItem = Dequeue();
            else LastItem = default(T);

            _list.Add(item);
        }

        public T Dequeue()
        {
            T item = default(T);
            if (_list.Count > 0)
            {
                item = _list[0];
                _list.RemoveAt(0);
            }
            return item;
        }

        public T Peek()
        {
            if (_list.Count > 0) return _list[0];
            else return default(T);
        }

        public T Tail()
        {
            if (_list.Count > 0) return _list[_list.Count - 1];
            else return default(T);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
