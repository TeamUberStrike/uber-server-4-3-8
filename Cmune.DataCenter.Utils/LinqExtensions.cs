using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cmune.DataCenter.Utils
{
    /*
     * These classes are used to perform a Distinct with LINQ
     */

    public static class LinqExtensions
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
        {
            return source.Distinct(new DelegateComparer<T>(comparer));
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer, Func<T, int> hashMethod)
        {
            return source.Distinct(new DelegateComparer<T>(comparer, hashMethod));
        }

        public static string SerializeForLog(this ChangeConflictException e, DataContext context)
        {
            StringBuilder builder = new StringBuilder();

            using (StringWriter sw = new StringWriter(builder))
            {
                sw.WriteLine("Optimistic concurrency error:");
                sw.WriteLine(e.Message);

                foreach (ObjectChangeConflict occ in context.ChangeConflicts)
                {
                    Type objType = occ.Object.GetType();
                    MetaTable metatable = context.Mapping.GetTable(objType);
                    object entityInConflict = occ.Object;

                    sw.WriteLine("Table name: {0}", metatable.TableName);

                    var noConflicts =
                        from property in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanRead &&
                              property.CanWrite &&
                              property.GetIndexParameters().Length == 0 &&
                              !occ.MemberConflicts.Any(c => c.Member.Name != property.Name)
                        orderby property.Name
                        select property;

                    foreach (var property in noConflicts)
                    {
                        sw.WriteLine("\tMember: {0}", property.Name);
                        sw.WriteLine("\t\tCurrent value: {0}",
                            property.GetGetMethod().Invoke(occ.Object, new object[0]));
                    }

                    sw.WriteLine("\t-- Conflicts Start Here --", metatable.TableName);

                    foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                    {
                        sw.WriteLine("\tMember: {0}", mcc.Member.Name);
                        sw.WriteLine("\t\tCurrent value: {0}", mcc.CurrentValue);
                        sw.WriteLine("\t\tOriginal value: {0}", mcc.OriginalValue);
                        sw.WriteLine("\t\tDatabase value: {0}", mcc.DatabaseValue);
                    }
                }

                sw.WriteLine();
                sw.WriteLine("Attempted SQL: ");

                TextWriter tw = context.Log;

                try
                {
                    context.Log = sw;
                    context.SubmitChanges();
                }
                catch (ChangeConflictException)
                {
                    // This is what we wanted.
                }
                catch
                {
                    sw.WriteLine("Unable to recreate SQL!");
                }
                finally
                {
                    context.Log = tw;
                }

                sw.WriteLine();

                //sw.WriteLine(e.SerializeForLog());

            }

            return builder.ToString();
        }
    }

    public class DelegateComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _equals;
        private Func<T, int> _getHashCode;

        public DelegateComparer(Func<T, T, bool> equals)
        {
            this._equals = equals;
        }

        public DelegateComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            this._equals = equals;
            this._getHashCode = getHashCode;
        }

        public bool Equals(T a, T b)
        {
            return _equals(a, b);
        }

        public int GetHashCode(T a)
        {
            if (_getHashCode != null)
            {
                return _getHashCode(a);
            }
            else
            {
                return a.GetHashCode();
            }
        }
    }
}
