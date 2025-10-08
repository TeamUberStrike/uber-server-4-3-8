using System;
using System.Collections;
using System.Text;
using System.Reflection;

namespace Cmune.Util
{
    public static partial class CmunePrint
    {
        public static string Properties(object instance, bool publicOnly = true)
        {
            StringBuilder builder = new StringBuilder();
            if (instance == null)
            {
                builder.Append("[Class=null]");
            }
            else
            {
                builder.AppendFormat("[Class={0}] ", instance.GetType().Name);
                foreach (var p in instance.GetType().GetProperties(publicOnly ? BindingFlags.Instance | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    builder.AppendFormat("[{0}={1}],", p.Name, Object(p.GetValue(instance, null)));
                }
            }
            return builder.ToString();
        }

        public static string Object(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is string)
            {
                return value as string;
            }
            else if (value.GetType().IsValueType)
            {
                return value.ToString();
            }
            else if (value is ICollection)
            {
                return Values(value);
            }
            else
            {
                return value.ToString();
            }
        }

        public static int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else if (obj is ICollection)
            {
                int code = 0;
                foreach (object o in obj as ICollection)
                    code += o.GetHashCode();
                return code;
            }
            else
            {
                return obj.GetHashCode();
            }
        }

        public static string Percent(float f)
        {
            return string.Format("{0:N0}%", Math.Round(f * 100));
        }

        public static string Order(int time)
        {
            if (time > 0)
            {
                if (time == 1) return "1st";
                else if (time == 2) return "2nd";
                else if (time == 3) return "3rd";
                else return time + "th";
            }
            else return time.ToString();
        }

        public static string Time(DateTime time)
        {
            return time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss.fffffffK");
        }

        public static string Time(TimeSpan s)
        {
            if (s.Days > 0)
            {
                //string appendix = s.Days > 1 ? "s" : string.Empty;
                return string.Format("{0:D1}d, {1:D2}:{2:D2}h", s.Days, s.Hours, s.Minutes);
            }
            else if (s.Hours > 0)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", s.Hours, s.Minutes, s.Seconds);
            }
            else if (s.Minutes > 0)
            {
                return string.Format("{0:D2}:{1:D2}", s.Minutes, s.Seconds);
            }
            else if (s.Seconds > 10)
            {
                return string.Format("{0:D2}", s.Seconds);
            }
            else
            {
                return string.Format("{0:D1}", s.Seconds);
            }
        }

        public static string Time(int seconds)
        {
            return Time(TimeSpan.FromSeconds(Math.Max(seconds, 0)));
        }

        public static string Flag(sbyte flag) { return Flag((uint)flag, 7, null); }
        public static string Flag(byte flag) { return Flag((uint)flag, 7, null); }
        public static string Flag(ushort flag) { return Flag((uint)flag, 15, null); }
        public static string Flag(short flag) { return Flag((uint)flag, 15, null); }
        public static string Flag(int flag) { return Flag((uint)flag, 31, null); }
        public static string Flag(uint flag) { return Flag(flag, 31, null); }

        public static string Flag<T>(ushort flag) { return Flag(flag, 15, typeof(T)); }
        public static string Flag<T>(uint flag) { return Flag(flag, 31, typeof(T)); }

        private static string Flag(uint flag, int bytes, Type type)
        {
            int mask = 1 << bytes;
            StringBuilder b = new StringBuilder();
            for (int i = bytes; i >= 0; --i)
            {
                if (type != null)
                {
                    if ((flag & mask) != 0 && type.IsEnum && Enum.IsDefined(type, (1 << i)))
                        b.Append(Enum.GetName(type, (1 << i)) + " ");
                }
                else
                {
                    b.Append(((flag & mask) == 0) ? '0' : '1');
                    if (i % 8 == 0) b.Append(' ');
                }
                flag <<= 1;
            }
            return b.ToString();
        }

        public static string Values(params object[] args)
        {
            StringBuilder b = new StringBuilder();

            if (args != null)
            {
                if (args.Length == 0)
                {
                    b.Append("EMPTY");
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        object o = args[i];
                        if (o != null)
                        {
                            if (o is IEnumerable)
                            {
                                IEnumerable collection = o as IEnumerable;
                                b.Append("|");
                                IEnumerator iter = collection.GetEnumerator();
                                int j = 0;
                                while (iter.MoveNext() && j < 50)
                                {
                                    if (iter.Current != null)
                                        b.AppendFormat("{0}|", iter.Current);
                                    else
                                        b.Append("null|");
                                    j++;
                                }

                                if (j == 0)
                                    b.Append("empty|");
                                else if (j == 50)
                                    b.Append("...");
                            }
                            else
                            {
                                b.AppendFormat("{0}", o);
                            }
                        }
                        else
                        {
                            b.AppendFormat("null");
                        }
                        if (i < args.Length - 1)
                            b.AppendFormat(", ");
                    }
                }
            }
            else
            {
                //b.Append("Args: NULL");
                b.Append("NULL");
            }

            return b.ToString();
        }

        public static string Types(params object[] args)
        {
            StringBuilder b = new StringBuilder();

            if (args != null)
            {
                if (args.Length == 0)
                {
                    b.Append("EMPTY");
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        object o = args[i];
                        if (o != null)
                        {
                            if (o is ICollection)
                            {
                                ICollection collection = o as ICollection;
                                b.AppendFormat("{0}({1})", collection.GetType().Name, collection.Count);
                            }
                            else
                            {
                                b.AppendFormat("{0}", o.GetType().Name);
                            }
                        }
                        else
                        {
                            b.AppendFormat("null");
                        }
                        if (i < args.Length - 1)
                            b.AppendFormat(", ");
                    }
                }
            }
            else
            {
                b.Append("NULL");
            }

            return b.ToString();
        }

        public static string Dictionary(IDictionary t)
        {
            StringBuilder b = new StringBuilder();

            foreach (DictionaryEntry k in t)
            {
                b.AppendFormat("{0}: {1}\n", k.Key, k.Value);
            }

            return b.ToString();
        }
    }
}
