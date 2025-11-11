using System.Collections;
using System.Collections.Generic;

namespace Cmune.Util
{
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> inner;
        public ReverseComparer() : this(null) { }
        public ReverseComparer(IComparer<T> inner)
        {
            this.inner = inner ?? Comparer<T>.Default;
        }
        int IComparer<T>.Compare(T x, T y) { return inner.Compare(y, x); }
    }

    public static class Comparison
    {
        public static bool IsEqual(object a, object b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a is ICollection && b is ICollection)
            {
                return IsSequenceEqual(a as ICollection, b as ICollection);
            }
            else
            {
                return a.Equals(b);
            }
        }

        private static bool IsSequenceEqual(ICollection a1, ICollection a2)
        {
            if (a1 != null && a2 != null)
            {
                bool b = true;

                IEnumerator it1 = a1.GetEnumerator();
                IEnumerator it2 = a2.GetEnumerator();

                while (b && it1.MoveNext() && it2.MoveNext())
                {
                    if (it1.Current is ICollection && it2.Current is ICollection)
                        b = IsSequenceEqual(it1.Current as ICollection, it2.Current as ICollection);
                    else
                        b = it1.Current != null && it1.Current.Equals(it2.Current);
                }

                return b;
            }
            else { return false; }
        }
    }
}
