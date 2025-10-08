using System;
using System.Collections.Generic;

namespace Cmune.Realtime.Common.Utils
{
    public class PerformanceStats
    {
        Dictionary<PerfCounter, float> _counter = new Dictionary<PerfCounter, float>(20);

        public void SetCounter(PerfCounter c, float f)
        {
            _counter[c] = f;
        }

        public void SetCounter(int c, float f)
        {
            if (Enum.IsDefined(typeof(PerfCounter), c))
                _counter[(PerfCounter)c] = f;
        }

        public void SetCounter(Dictionary<PerfCounter, float> counter)
        {
            foreach (KeyValuePair<PerfCounter, float> c in counter)
            {
                SetCounter(c.Key, c.Value);
            }
        }

        public void SetCounter(Dictionary<int, float> counter)
        {
            foreach (KeyValuePair<int, float> c in counter)
            {
                SetCounter(c.Key, c.Value);
            }
        }

        public bool HasCounter(PerfCounter counter)
        {
            return _counter.ContainsKey(counter);
        }

        public float GetCounter(PerfCounter counter)
        {
            float v = 0;
            _counter.TryGetValue(counter, out v);
            return v;
        }

        public void Reset()
        {
            _counter.Clear();
        }
    }
}
